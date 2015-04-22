using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace OcuBri
{
    class WebSocketEventArgs
    {
        public WebSocketSessionManager wsSessions;
        public string Data;
        public byte[] RawData;
    }
}
