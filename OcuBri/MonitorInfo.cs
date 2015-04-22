using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcuBri
{
    class MonitorInfo
    {
        public bool IsPrimary;
        Rectangle Bounds;

        public MonitorInfo(bool isPrimary, Rectangle bounds)
        {
            this.IsPrimary = isPrimary;
            this.Bounds = bounds;
        }

        public static Dictionary<string, MonitorInfo> List()
        {
            var monitors = new Dictionary<string, MonitorInfo>();

            // モニター情報の収集
            Screen.AllScreens.ToList().ForEach(scr =>
            {
                var monitorName = scr.DeviceFriendlyName();
                // Monitor Name
                // DK1:"Rift DK" DK2:"Rift DK2"
                var mi = new MonitorInfo(scr.Primary, scr.Bounds);
                monitors.Add(monitorName, mi);
            });

            return monitors;
        }
    }
}
