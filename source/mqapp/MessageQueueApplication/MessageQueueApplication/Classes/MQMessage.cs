using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MessageQueueApplication.Classes
{
    public class MQMessage
    {
        private int _code;
        private Object _object;
        private Object ObjectGetterLocker = new object();
        public int Code { get => _code; }

        public MQMessage(int code, Object linkedObject = null)
        {
            _code = code;
            _object = linkedObject;
        }

        public Object ExtractObject()
        {
            lock(ObjectGetterLocker)
            {
                Object res = _object;
                _object = null;
                return res;
            }
        }
    }

    public class MQMessageStorage: IMessageQueue
    {
        public delegate void MessageTrigger();

        private ConcurrentQueue<MQMessage> queue = new ConcurrentQueue<MQMessage>();
        private MessageTrigger trigger;
        private Object locker = new object();
        private IntPtr OwnerHandle;

        public void MQNotify(int code, Object linkedObject = null)
        {
            MQMessage message = new MQMessage(code, linkedObject);
            queue.Enqueue(message);
            if (trigger == null) return;
            trigger();
        }

        public MQMessage ExtractMessage()
        {
            lock(locker)
            {
                MQMessage res = null;
                if (!queue.TryDequeue(out res)) res = null;
                if ((queue.Count > 0) && (trigger != null)) trigger();
                return res;
            }
        }

        private MQMessageStorage()
        {
        }

        public MQMessageStorage(MessageTrigger trigger)
        {
            this.trigger = trigger;
        }

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        public const uint WM_MESSAGE_DISPATCH = 13000 + 1;

        public static MQMessageStorage CreateFromWndHandle(IntPtr Handle)
        {
            MQMessageStorage ms = new MQMessageStorage();
            ms.OwnerHandle = Handle;
            ms.trigger = () =>
            {
                PostMessage(ms.OwnerHandle, WM_MESSAGE_DISPATCH, 100, 0);
            };
            return ms;
        }
    }
}
