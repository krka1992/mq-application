using System;
using System.Collections.Generic;
using System.Text;
using MessageQueueApplication;
using MessageQueueApplication.Classes;
using System.Runtime.InteropServices;
using System.Threading;

namespace MQARun
{
    public enum CAStates
    {
        None    = 0,
        Start   = 1,
        Busy    = 2,
        Ready   = 3,
        Error   = 4
    }

    public class CustomObj
    {
        public string text;
    }

    public class CustomApplication : MQApplication
    {
        private int counter = 0;
        public CAStates State = CAStates.None;
        public IntPtr ownerHandle;
        public IMessageQueue OwnerMQ;

        public const uint WM_MESSAGE_DISPATCH = 13000 + 1;
        public const uint WM_MESSAGE_TIMER = 13000 + 2;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        protected override void DispatchMessage(MQMessage message)
        {
            CustomObj obj = new CustomObj();
            obj.text = "Dispatch message";
            OwnerMQ.MQNotify(1, obj);
            obj = new CustomObj();
            obj.text = "Обработка";
            OwnerMQ.MQNotify(2, obj);
            obj = new CustomObj();
            obj.text = "Вызов";
            OwnerMQ.MQNotify(3, obj);
            obj = new CustomObj();
            obj.text = "Извлечение";
            OwnerMQ.MQNotify(2, obj);
        }

        protected override void DispatchMessageException(Exception e)
        {
            PostMessage(ownerHandle, WM_MESSAGE_DISPATCH, 100, 0);
        }

        protected override void OnTimer()
        {
            PostMessage(ownerHandle, WM_MESSAGE_TIMER, 0, 0);
        }
    }
}
