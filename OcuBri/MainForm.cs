using Ovr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;

namespace OcuBri
{
    public partial class MainForm : Form
    {
        
        Hmd hmd = null;
        HmdDesc? hmdDesc = null;

        BrowserInfo targetBrowser = null;
        IntPtr targetWindow = IntPtr.Zero;
        IntPtr browserMainWindow = IntPtr.Zero;

        byte[] oculusProfileBinary = null;
        
        WebSocketServer wssv = new WebSocketServer(4649, false);
        MainBehavior mainWebSocket = null;
        OculusBehavior oculusWebSocket = null;

        bool before = true;
        List<IntPtr> beforeChromeWindows = new List<IntPtr>();
        List<IntPtr> afterChromeWindows = new List<IntPtr>();

        System.Threading.Timer loopTimer;
        
        enum RunState
        {
            Init,
            Wait,
            SendTracking
        }
        RunState rState = RunState.Init;
        StringBuilder classNameBuffer = new StringBuilder(256);

        #region Form 関数

        public MainForm()
        {
            InitializeComponent();

            this.lblOculusRiftConnect.Text = "None";
            this.lblMainWSConnect.Text = "None";
            this.lblOculusWSConnect.Text = "None";

            // WebSocketサービスの設定
            var hmdService = new MainBehavior();
            wssv.AddWebSocketService<MainBehavior>("/main", () => new MainBehavior(this));
            wssv.AddWebSocketService<OculusBehavior>("/oculus", () => new OculusBehavior(this));
            // WebSocketサーバーを開始
            wssv.Start();

            // Hmdの初期化
            Hmd.Initialize();

            // ループタイマー初期化
            loopTimer = new System.Threading.Timer(new TimerCallback(loop), null, 0, 1000); //new MicroTimer();            
        }

        /// <summary>
        /// Oculus Rift 接続状態表示ラベルテキスト設定
        /// </summary>
        /// <param name="txt">設定するテキスト</param>
        void setOculusConnectText(string txt)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    this.lblOculusRiftConnect.Text = txt;
                }));
            }
        }

        /// <summary>
        /// Main側 WebSocket 接続状態表示ラベルテキスト設定
        /// </summary>
        /// <param name="txt">設定するテキスト</param>
        void setMainWSConnectText(string txt)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    this.lblMainWSConnect.Text = txt;
                }));
            }
        }

        /// <summary>
        /// Oculus Rift側 WebSocket 接続状態表示ラベルテキスト設定
        /// </summary>
        /// <param name="txt">設定するテキスト</param>
        void setOculusWSConnectText(string txt)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    this.lblOculusWSConnect.Text = txt;
                }));
            }
        }

        #endregion

        #region Main側 WebSocket イベント

        /// <summary>
        /// Main WebSocket接続イベント
        /// </summary>
        /// <param name="sessions">WebSocketセッション</param>
        internal void Main_OnOpen(MainBehavior session)
        {
            if (mainWebSocket == null)
            {
                mainWebSocket = session;
            } else{
                session.IsOver = true;
                mainWebSocket.Send(Serializer.Serialize(((object)new Dictionary<string, string> { { "cmd", "o" } })));
                mainWebSocket.Context.WebSocket.Close();
                return;
            }
            setMainWSConnectText("接続中");
        }

        /// <summary>
        /// Main WebSocketメッセージ受信イベント
        /// </summary>
        /// <param name="msg">受信メッセージ</param>
        internal void Main_OnMessage(MainBehavior session, MessageEventArgs e)
        {
            var msg = JsonConvert.DeserializeObject<Dictionary<string, string>>(e.Data);
            var response = new Dictionary<string, string>();
            switch (msg["cmd"])
            {
                case "oculusWindowOpen":
                    if (hmd == null)
                    {
                        response.Add("cmd", "no connect");
                    }
                    else
                    {

                        // Oculus Riftのモニターを取得
                        var monitor = Screen.AllScreens
                            .Where(x => x.DeviceFriendlyName() == "Rift " + hmdDesc.Value.Type.ToString())
                            .FirstOrDefault();
                        
                        // デバッグ用モニターを取得
                        //var monitor = Screen.AllScreens
                        //    .Where(x => x.DeviceFriendlyName() == "HDMI")
                        //    .FirstOrDefault();

                        targetBrowser = BrowserInfo.FromName(msg["browser"]);
                        try
                        {
                            oculusWindowOpen(targetBrowser, msg["url"], monitor);
                            response.Add("cmd", "opened");
                        }
                        catch (Exception ex)
                        {
                            response.Add("cmd", "open error");
                            response.Add("msg", ex.Message);
                        }
                    }
                    break;
                case "oculusWindowClose":
                    try
                    {
                        oculusWindowClose();
                        response.Add("cmd", "closed");
                    }
                    catch (Exception ex)
                    {
                        response.Add("cmd", "close error");
                        response.Add("msg", ex.Message);
                    }
                    break;
            }
            session.Send(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// WebSocket切断イベント
        /// </summary>
        /// <param name="sessions">WebSocketセッション</param>
        internal void Main_OnClose(MainBehavior session)
        {
            if (session.IsOver) return;
            mainWebSocket = null;
            setMainWSConnectText("None");
        }

        #endregion

        #region Oculus Rift側 WebSocket イベント

        /// <summary>
        /// Oculus Rift WebSocket接続イベント
        /// </summary>
        /// <param name="session">WebSocketセッション</param>
        internal void OculusRift_OnOpen(OculusBehavior session)
        {
            if (oculusWebSocket == null)
            {
                oculusWebSocket = session;
            }
            else
            {
                session.IsOver = true;
                oculusWebSocket.Send(Serializer.Serialize(((object)new Dictionary<string, string> { { "cmd", "o" } })));
                oculusWebSocket.Context.WebSocket.Close();
                return;
            }
            if (oculusProfileBinary != null)
            {
                oculusWebSocket.Send(oculusProfileBinary);
            }
            setOculusWSConnectText("接続中");
        }


        /// <summary>
        /// Oculus Rift側 WebSocket切断イベント
        /// </summary>
        /// <param name="sessions">WebSocketセッション</param>
        internal void Oculus_OnClose(OculusBehavior session)
        {
            if (session.IsOver) return;

            oculusWebSocket = null;
            setOculusWSConnectText("None");
            if (rState == RunState.SendTracking)
            {
                rState = RunState.Wait;
                loopTimer.Change(0, 1000);
            }
        }

        #endregion


        #region 処理関数

        /// <summary>
        /// ループ処理
        /// </summary>
        void loop(object state)
        {
            switch (rState)
            {
                case RunState.Init: // 初期化ステート
                    if (Ovr.Hmd.Detect() > 0)
                    {
                        // Hmdインスタンスを作成
                        hmd = new Hmd(0);
                        hmdDesc = hmd.GetDesc();

                        // Oculus Riftのプロファイル情報取得
                        oculusProfileBinary = ((object)new OculusProfile(hmd, hmdDesc)).Serialize();
                        // Oculus Riftプロファイル情報送信
                        if (oculusWebSocket != null)
                        {
                            oculusWebSocket.Send(oculusProfileBinary);
                        }
                        
                        // ヘッドトラッキング(Orientation)およびポジショントラッキング(Position)を取得できるように設定
                        hmd.SetEnabledCaps(HmdCaps.LowPersistence | HmdCaps.DynamicPrediction);
                        hmd.ConfigureTracking(TrackingCaps.Orientation | TrackingCaps.MagYawCorrection | TrackingCaps.Position, 0);
                        setOculusConnectText(hmdDesc.Value.Type + " 接続中");
                        // 待機ステートに移行
                        rState = RunState.Wait;
                    }
                    break;
                case RunState.Wait: // 待機ステート
                    if (!checkOculusConnect()) break;

                    if (oculusWebSocket != null) 
                    {
                        // Oculus RiftのWebSocketのセッションが開始され(て)たらセンサー情報取得ステートに移行
                        rState = RunState.SendTracking;
                        // ループタイマーを(約)60fpsに変更
                        loopTimer.Change(0, 14);
                        break;
                    }
                    break;
                case RunState.SendTracking: // トラッキングデータ送信ステート
                    if (!checkOculusConnect()) break;
 
                    // センサーのデータを取得
                    Posef[] poses = hmd.GetEyePoses(0);
                    var lO = poses[(int)Eye.Left].Orientation;
                    var lP = poses[(int)Eye.Left].Position;
                    var rO = poses[(int)Eye.Right].Orientation;
                    var rP = poses[(int)Eye.Right].Position;
                    var sendData = (new[]{
                        lO.x, lO.y, lO.z, lO.w, rO.x, rO.y, rO.z, rO.w,
                        lP.x, lP.y, lP.z, rP.x, rP.y, rP.z 
                    }).Serialize();
                    // WebSocketのセッションがあればセンサーのデータを送信
                    if (oculusWebSocket != null)
                    {
                        oculusWebSocket.Send(sendData);
                    }
                    break;
            }
        }

        /// <summary>
        /// Oculus Riftの接続チェック
        /// </summary>
        /// <returns>接続フラグ</returns>
        bool checkOculusConnect()
        {
            if (hmd == null) return false;

            // デバイスの状態を取得
            var state = hmd.GetTrackingState();
            if ((state.StatusFlags & Ovr.StatusBits.HmdConnected) != Ovr.StatusBits.HmdConnected)
            {
                // ケーブルが抜かれたなどにより切断された場合は
                // Oculus Riftに表示しているウィンドウを閉じ、
                // hmdを開放し初期化ステートに戻る
                oculusWindowClose();
                hmdDesc = null;
                hmd.Dispose();
                hmd = null;
                setOculusConnectText("None");
                rState = RunState.Init;
                return false;
            }
            return true;
        }

        /// <summary>
        /// EnumWindowsのコールバック関数
        /// </summary>
        /// <param name="hWnd">列挙されたウィンドウハンドル</param>
        /// <param name="lParam">ユーザーステータス引数</param>
        /// <returns>列挙続行フラグ</returns>
        bool enumWindowsProc(IntPtr hWnd, string windowClassName)
        {
            classNameBuffer.Length = 0;
            // 列挙されたウィンドウのクラス名を取得
            var ret = Win32Api.GetClassName(hWnd, classNameBuffer, classNameBuffer.Capacity);
            if (classNameBuffer.ToString() == windowClassName)
            {
                // ウィンドウクラス名がターゲットブラウザーのウィンドウクラス名の場合
                if (before) // ブラウザープロセス起動前
                    beforeChromeWindows.Add(hWnd);
                else // ブラウザープロセス起動後
                    afterChromeWindows.Add(hWnd);
            }
            return true;
        }

        /// <summary>
        /// Oculus Rift用のウィンドウを開く
        /// </summary>
        void oculusWindowOpen(BrowserInfo targetBrowser, string targetURL, Screen monitor)
        {
            if (monitor == null) return;
            browserMainWindow = Win32Api.GetForegroundWindow();

            // ブラウザーのメインウインドウ(コンテンツがあるウィンドウ)が
            // 最大化されていると新しいウィンドウが(F11をシミュレートしたとき)
            // メインモニターに戻ってしまうためいったん最大化を解除する。
            var browserMainWindowStyle = Win32Api.GetWindowLong(browserMainWindow, Win32Api.GWL_STYLE);
            var maximized = false;
            if ((browserMainWindowStyle & Win32Api.WS_MAXIMIZE) == Win32Api.WS_MAXIMIZE)
            {
                maximized = true;
                browserMainWindowStyle ^= Win32Api.WS_MAXIMIZE;
                Win32Api.SetWindowLongPtr(browserMainWindow, Win32Api.GWL_STYLE, (IntPtr)browserMainWindowStyle);
                Win32Api.SetWindowPos(
                    browserMainWindow,
                    IntPtr.Zero,
                    0, 0, 0, 0,
                    Win32Api.SWP_NOSIZE);
            }

            // EnumWindowsコールバック関数の設定
            var callback = new Win32Api.CallBackPtr(enumWindowsProc);
            before = true;
            beforeChromeWindows.Clear();
            // ブラウザーを起動する前のブラウザーウィンドウを列挙
            Win32Api.EnumWindows(callback, targetBrowser.WindowClassName);

            Thread.Sleep(1000);

            // ターゲットブラウザーを起動
            var p = Process.Start(targetBrowser.FileName, targetBrowser.GetArguments(targetURL));
            p.WaitForInputIdle();
            before = false;

            // タイムアウトを見るため開始時間を保持
            var start = DateTime.Now;

            // プロセススタート後のブラウザーウィンドウを列挙
            while (true)
            {
                afterChromeWindows.Clear();
                Thread.Sleep(33);
                Win32Api.EnumWindows(callback, targetBrowser.WindowClassName);
                // 起動前より増えてたらOK
                if (beforeChromeWindows.Count < afterChromeWindows.Count) break;
                // タイムアウト(5秒)したらループ抜ける
                if ((DateTime.Now - start).Seconds >= 5)
                {
                    MessageBox.Show("ターゲットウィンドウ取得でタイムアウトしました。");
                    return;
                }
            }

            Thread.Sleep(500);

            // ターゲットウィンドウのハンドルを取得
            // 初期値として起動後のウィンドウを設定(起動前の取得数が0だった場合)
            targetWindow = afterChromeWindows.First();
            if (beforeChromeWindows.Count > 0)
            {
                // 起動前の取得数が0でない場合、起動後から起動前にないものを抽出しそれをターゲットウィンドウとする
                targetWindow = afterChromeWindows
                    .Except(beforeChromeWindows)
                    // Chromeの場合２つのウィンドウが生成されるが一方はWS_EX_NOACTIVATEなものであるため、
                    // WS_EX_NOACTIVATEでないもを選択
                    .Where(x => (Win32Api.GetWindowLong(x, Win32Api.GWL_STYLE) & Win32Api.WS_VISIBLE) == Win32Api.WS_VISIBLE)
                    .First();
            }
            // 500ミリ秒待機(秒数は適当)
            Thread.Sleep(100);
            // Oculus Riftのモニターにウィンドウを移動
            Win32Api.SetWindowPos(
                targetWindow, 
                IntPtr.Zero,
                monitor.Bounds.Left, monitor.Bounds.Top, monitor.Bounds.Width, monitor.Bounds.Height,
                0);
            Thread.Sleep(100);

            // F11キーをシミュレート
            var winStyle = Win32Api.GetWindowLong(targetWindow, Win32Api.GWL_STYLE);
            if ((winStyle & Win32Api.WS_CAPTION) == Win32Api.WS_CAPTION)
            {
                Win32Api.keybd_event(Win32Api.VK_F11, 0, 0, 0);
                //Thread.Sleep(33);
                Win32Api.keybd_event(Win32Api.VK_F11, 0, Win32Api.KEYEVENTF_KEYUP, 0);
            }

            // メインウィンドウが最大化してた場合は最大化に戻す
            if (maximized)
            {
                browserMainWindowStyle |= Win32Api.WS_MAXIMIZE;
                Win32Api.SetWindowLongPtr(browserMainWindow, Win32Api.GWL_STYLE, (IntPtr)browserMainWindowStyle);
            }

            Thread.Sleep(200);
            Win32Api.SetForegroundWindow(browserMainWindow);
        }

        /// <summary>
        /// Oculus Rift用のウィンドウを閉じる
        /// </summary>
        void oculusWindowClose()
        {
            if (targetWindow != IntPtr.Zero)
            {
                if (targetBrowser.Browser == BrowserInfo.IEXPLORER)
                {
                    var winStyle = Win32Api.GetWindowLong(targetWindow, Win32Api.GWL_STYLE);
                    if ((winStyle & Win32Api.WS_CAPTION) != Win32Api.WS_CAPTION)
                    {
                        Win32Api.keybd_event(Win32Api.VK_F11, 0, 0, 0);
                        Thread.Sleep(33);
                        Win32Api.keybd_event(Win32Api.VK_F11, 0, Win32Api.KEYEVENTF_KEYUP, 0);
                    }
                }
                Win32Api.SendMessage(targetWindow, Win32Api.WM_CLOSE, 0, IntPtr.Zero);
                targetBrowser = null;
                targetWindow = IntPtr.Zero;
            }
        }

        #endregion
    }
}
