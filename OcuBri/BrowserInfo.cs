using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcuBri
{
    enum browserType
    {
        Chrome,
        Firefox,
        InternetExplorer,
        Opera,
        Vivaldi,
    }

    enum OptionPos
    {
        Before,
        After
    }

    class BrowserInfo
    {
        public string Browser;
        public string FileName;
        public string WindowClassName;
        public string Arguments;
        public OptionPos OptionPosition = OptionPos.After;

        public const string CHROME = "chrome";
        public const string FIREFOX = "firefox";
        public const string IEXPLORER = "iexplorer";
        public const string OPERA = "opera";
        public const string VIVALDI = "vivaldi";
        const string CHROME_CLASS_NAME = "Chrome_WidgetWin_1";
        const string FIREFOX_CLASS_NAME = "MozillaWindowClass";
        const string IEXPLORER_CLASS_NAME = "IEFrame";
        const string OPERA_CLASS_NAME = "Chrome_WidgetWin_1"; // Chromeと同じウインドウクラス名
        const string VIVALDI_CLASS_NAME = "Chrome_WidgetWin_1"; // Chromeと同じウインドウクラス名

        static string chromeFileName;
        static string firefoxFileName;
        static string iexplorerFileName;
        static string operaFileName;
        static string vivaldiFileName;

        public BrowserInfo(string browser, string fileName, string windowClassName, string arguments, OptionPos optionPos)
        {
            this.Browser = browser;
            this.FileName = fileName;
            this.WindowClassName = windowClassName;
            this.Arguments = arguments;
            this.OptionPosition = optionPos;
        }

        public string GetArguments(string url)
        {
            return this.OptionPosition == OptionPos.Before ? 
                this.Arguments + " " + url : url + " " + this.Arguments; 
        }

        static void getRegistry()
        {
            var appPathKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\";
            var hklm = Registry.LocalMachine;
            var chromePathKey = hklm.OpenSubKey(appPathKey + "chrome.exe");
            var firefoxPathKey = hklm.OpenSubKey(appPathKey + "firefox.exe");
            var iexplorerPathKey = hklm.OpenSubKey(appPathKey + "IEXPLORE.exe");
            var operaPathKey = hklm.OpenSubKey(appPathKey + "Opera.exe");
            chromeFileName = chromePathKey == null ? null : (string)chromePathKey.GetValue(null);
            firefoxFileName = firefoxPathKey == null ? null : (string)firefoxPathKey.GetValue(null);
            iexplorerFileName = iexplorerPathKey == null ? null : (string)iexplorerPathKey.GetValue(null);
            operaFileName = operaPathKey == null ? null : operaPathKey.GetValue(null).ToString().Replace("\"", "");
            vivaldiFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Vivaldi\Application\vivaldi.exe";
            if (!File.Exists(vivaldiFileName)) vivaldiFileName = "";
            chromePathKey.Close();
            firefoxPathKey.Close();
            iexplorerPathKey.Close();
            operaPathKey.Close();
        }

        public static Dictionary<string, BrowserInfo> List()
        {
            getRegistry();

            var browsers = new Dictionary<string, BrowserInfo>();
            if(chromeFileName !=null){
                browsers.Add(CHROME, new BrowserInfo (CHROME, chromeFileName, CHROME_CLASS_NAME, "--new-window", OptionPos.After));
            }
            if (firefoxFileName != null)
            {
                browsers.Add(FIREFOX, new BrowserInfo(FIREFOX, firefoxFileName, FIREFOX_CLASS_NAME, "-new-window", OptionPos.Before));
            }
            if(iexplorerFileName != null){
                browsers.Add(IEXPLORER, new BrowserInfo (IEXPLORER, iexplorerFileName, IEXPLORER_CLASS_NAME, "", OptionPos.After));
            }
            if(operaFileName != null){
                browsers.Add(OPERA, new BrowserInfo (OPERA, operaFileName, OPERA_CLASS_NAME, "--new-window", OptionPos.Before));
            }
            if (vivaldiFileName != null)
            {
                browsers.Add(VIVALDI, new BrowserInfo(VIVALDI, vivaldiFileName, VIVALDI_CLASS_NAME, "--new-window", OptionPos.After));
            }

            return browsers;
        }

        public static BrowserInfo FromName(string name)
        {
            getRegistry();

            switch (name)
            {
                case CHROME:
                    return chromeFileName == null ? 
                        null : new BrowserInfo(CHROME, chromeFileName, CHROME_CLASS_NAME, "--new-window", OptionPos.After);
                case FIREFOX:
                    return firefoxFileName == null ? 
                        null : new BrowserInfo(FIREFOX, firefoxFileName, FIREFOX_CLASS_NAME, "-new-window", OptionPos.Before);
                case IEXPLORER:
                    return iexplorerFileName == null ? 
                        null : new BrowserInfo(IEXPLORER, iexplorerFileName, IEXPLORER_CLASS_NAME, "", OptionPos.After);
                //case OPERA:
                //    return operaFileName == null ? 
                //        null : new BrowserInfo(OPERA, operaFileName, OPERA_CLASS_NAME, "--new-window", OptionPos.Before);
                //case VIVALDI:
                //    return vivaldiFileName == null ? null : new BrowserInfo(VIVALDI, vivaldiFileName, VIVALDI_CLASS_NAME, "--new-window", OptionPos.After);
                default:
                    return null;
            }
        }
    }
}
