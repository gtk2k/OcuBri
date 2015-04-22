using Ovr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace OcuBri
{
    internal class MainBehavior : WebSocketBehavior
    {
        MainForm uiForm;
        public bool IsOver = false;

        public MainBehavior(){}

        public MainBehavior(MainForm form)
        {
            this.uiForm = form;
        }

        protected override void OnOpen()
        {
            this.uiForm.Main_OnOpen(this);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            this.uiForm.Main_OnMessage(this, e);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            this.uiForm.Main_OnClose(this);
        }
    }
}
