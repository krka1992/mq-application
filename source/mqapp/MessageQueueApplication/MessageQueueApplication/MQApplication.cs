using System;
using System.Threading;
using MessageQueueApplication.Classes;

namespace MessageQueueApplication
{
    public enum MQApplicationMessageCode
    {
        MC_TERMINATE = 0,
        MC_DISPATCH_MESSAGE = 1
    }

    public abstract class MQApplication: IMQApplication
    {
        private bool Active = false;
        private int _terminated = 0;
        private MQApplicationMessageStorage MessageStorage = new MQApplicationMessageStorage();
        private Thread ActiveThread = null;

        protected abstract void InternalSendMessage(int Code);
        protected abstract void InternalLoopMessages();
        protected abstract void InternalPrepare();

        protected void DispatchMessage(MQApplicationMessage message)
        {

        }

        internal void ProcessMessage(long id) 
        {
            MQApplicationMessage message = MessageStorage.ExtractMessage(id);
            if (message == null) return;

            switch (message.Code)
            {
                case (int)MQApplicationMessageCode.MC_TERMINATE: 
                    Terminate();
                    return;
                case (int)MQApplicationMessageCode.MC_DISPATCH_MESSAGE:
                    DispatchMessage(message);
                    return;
            }            
        }

        private void LoopMessages()
        {
            InternalLoopMessages();
        }

        private void Prepare()
        {
            InternalPrepare();
        }

        private void Proc()
        {
            Prepare();
            LoopMessages();
        }

        public bool Terminated { get => _terminated != 0; }
        public void Terminate()
        {
            Interlocked.Exchange(ref _terminated, 1);
        }

        public bool Run()
        {
            if (Active) return false;
            if (Terminated) return false;
            if (ActiveThread != null) return false;
            ActiveThread = new Thread(new ThreadStart(Proc));
            ActiveThread.Start();
            Active = true;
            return true;
        }
        public void Wait()
        {
            if (!Active) return;
            while (!Terminated)
            {
                Thread.Sleep(1);
            }
        }
        public bool Stop()
        {
            if (!Active) return false;
            SendMessage(MQApplicationMessageCode.MC_TERMINATE);
            Active = false;
            return true;
        }
        public bool SendMessage(int Code, Object obj = null)
        {
            if (!Active) return false;
            MessageStorage.CreateMessage(Code, obj);
            InternalSendMessage(Code);
            return true;
        }

        public bool SendMessage(MQApplicationMessageCode Code, Object obj = null)
        {
            return SendMessage((int)Code, obj);
        }

        public static MQApplication CreateApplication()
        {
            return new MQApplicationWin();
        }
    }

    public class MQApplicationWin: MQApplication
    {
        protected abstract void InternalSendMessage(int Code)
        {

        }
        protected abstract void InternalLoopMessages()
        {

        }
        protected abstract void InternalPrepare()
        {

        }
    }
}
