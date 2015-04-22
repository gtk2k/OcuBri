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
    internal class OculusBehavior : WebSocketBehavior
    {
        MainForm uiForm;
        public bool IsOver = false;

        public OculusBehavior(){}

        public OculusBehavior(MainForm form)
        {
            this.uiForm = form;
        }

        protected override void OnOpen()
        {
            this.uiForm.OculusRift_OnOpen(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            this.uiForm.Oculus_OnClose(this);
        }
    }

}
