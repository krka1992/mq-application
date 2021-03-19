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
        public CAStates State = CAStates.None;
        public IntPtr ownerHandle;
        public IMessageQueue OwnerMQ;

        public const uint WM_MESSAGE_DISPATCH = 13000 + 1;
        public const uint WM_MESSAGE_TIMER = 13000 + 2;

        protected override void OnStartup()
        {
            CustomObj obj = new CustomObj();
            obj.text = "Start application";
            OwnerMQ.MQNotify(1, obj);
        }

        protected override void OnShutdown()
        {
            CustomObj obj = new CustomObj();
            obj.text = "Shutdown";
            OwnerMQ.MQNotify(1, obj);
        }

        protected override void DispatchMessageException(Exception e)
        {
            CustomObj obj = new CustomObj();
            obj.text = e.Message;
            OwnerMQ.MQNotify(1, obj);
        }

        protected override void DispatchMessage(MQMessage message)
        {
            CustomObj obj;

            switch (message.Code)
            {
                case 1:
                    obj = new CustomObj();
                    obj.text = "Dispatch message";
                    OwnerMQ.MQNotify(1, obj);
                    break;
                case 2:
                    obj = new CustomObj();
                    obj.text = "Обработка";
                    OwnerMQ.MQNotify(2, obj);
                    break;
                case 3:
                    obj = new CustomObj();
                    obj.text = "Вызов";
                    OwnerMQ.MQNotify(3, obj);
                    break;
                default:
                    base.DispatchMessage(message);
                    break;
            }

            
        }
    }
}
