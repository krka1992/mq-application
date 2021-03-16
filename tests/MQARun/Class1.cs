using System;
using System.Collections.Generic;
using System.Text;
using MessageQueueApplication;
using MessageQueueApplication.Classes;
using System.Runtime.InteropServices;
using System.Threading;

namespace MQARun
{
    public class CustomApplication : MQApplication
    {
        private int counter = 0;
        public IntPtr ownerHandle;

        public const uint WM_MESSAGE_DISPATCH = 13000 + 1;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        protected override void DispatchMessage(MQApplicationMessage message)
        {
            PostMessage(ownerHandle, WM_MESSAGE_DISPATCH, counter++, 0);
            Thread.Sleep(100);
            //rich.AppendText("DispatchMessage" + Environment.NewLine);
        }
    }
}
