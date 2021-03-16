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

    public abstract class MQApplicationBase: IMQApplication
    {
        protected int Active = 0;
        private int _terminated = 0;
        protected MQApplicationMessageStorage MessageStorage = new MQApplicationMessageStorage();
        private Thread ActiveThread = null;

        protected abstract void InternalSendMessage(int Code);
        protected abstract void InternalLoopMessages();
        protected abstract void InternalPrepare();
        protected abstract void InternalStop();

        protected virtual void DispatchMessage(MQApplicationMessage message)
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
            if (Active != 0) return false;
            if (Terminated) return false;
            if (ActiveThread != null) return false;
            Active = 1;
            ActiveThread = new Thread(new ThreadStart(Proc));
            ActiveThread.Start();
            return true;
        }
        public void Wait()
        {
            if (Active == 0) return;
            while (!Terminated)
            {
                Thread.Sleep(1);
            }
        }
        public bool Stop()
        {
            if (Active == 0) return false;
            Interlocked.Exchange(ref Active, 0);
            InternalStop();
            //SendMessage(MQApplicationMessageCode.MC_TERMINATE);
            return true;
        }
        public bool SendMessage(int Code, Object obj = null)
        {
            if (Active == 0) return false;
            MessageStorage.CreateMessage(Code, obj);
            InternalSendMessage(Code);
            return true;
        }

        public bool SendMessage(MQApplicationMessageCode Code, Object obj = null)
        {
            return SendMessage((int)Code, obj);
        }

        public static MQApplicationBase CreateApplication()
        {
            return new MQApplicationWin();
        }
    }

    public class MQApplicationWin: MQApplicationBase
    {
        private EventWaitHandle ewh;

        protected override void InternalSendMessage(int Code)
        {
            ewh.Set();
        }
        protected override void InternalLoopMessages()
        {
            if (ewh == null) return;
            MQApplicationMessage message;

            while (ewh.WaitOne())
            {
                if (Active == 0) return;
                while (true)
                {
                    message = MessageStorage.ExtractMessage();
                    if (message == null) break;
                    DispatchMessage(message);
                }
            }
        }
        protected override void InternalPrepare()
        {
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        protected override void InternalStop()
        {
            ewh.Set();
            ewh = null;
        }

    }

    public class MQApplication : MQApplicationWin { }
}
