using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OcuBri
{
    class Win32Api
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        public const int SWP_NOSIZE = 0x0001;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(CallBackPtr lpEnumFunc, string lParam);
        public delegate bool CallBackPtr(IntPtr hwnd, string lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
        public const int GWL_STYLE = (-16);
        public const int GWL_EXSTYLE = (-20);
        public const uint WS_CAPTION = 0xC00000;
        public const uint WS_MAXIMIZE = 0x1000000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_EX_NOACTIVATE = 0x8000000;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, Int32 wParam, IntPtr lParam);
        public const Int32 WM_SYSCOMMAND = 0x112;
        public const Int32 WM_CLOSE = 0x10;

        [DllImport("user32.dll")]
        public static extern bool keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        public const byte VK_F11 = 0x7A;
        public const uint KEYEVENTF_KEYUP = 0x0002;

    }
}
