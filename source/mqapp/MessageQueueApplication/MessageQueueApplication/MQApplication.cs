using System;
using System.Threading;
using MessageQueueApplication.Classes;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MessageQueueApplication
{
    public enum MQApplicationMessageCode
    {
        MC_TERMINATE = 0,
        MC_DISPATCH_MESSAGE = 1,
        MC_TIMER = 2
    }

    public abstract class MQApplication: IMQApplication, IMessageQueue
    {
        protected int Active = 0;
        private int _terminated = 0;
        protected MQApplicationMessageStorage MessageStorage = new MQApplicationMessageStorage();
        protected MQMessageStorage MQMessageStorage;
        private Thread ActiveThread = null;
        private Timer timer = null;

        private EventWaitHandle ewh;

        private void InternalSendMessage()
        {
            ewh.Set();
        }
        private void InternalLoopMessages()
        {
            if (ewh == null) return;
            if (Terminated) return;

            while (ewh.WaitOne())
            {
                if (Active == 0) return;
                ProcessMessages();
            }
        }
        private void InternalPrepare()
        {
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        private void InternalStop()
        {
            ewh.Set();
        }

        protected virtual void OnTimer()
        {

        }

        protected virtual void OnStartup()
        {

        }

        protected virtual void OnShutdown()
        {

        }

        protected virtual void OnException(Exception e)
        {

        }

        protected virtual void DispatchMessage(MQMessage message)
        {
            throw new Exception("Неизвестный тип сообщения " + message.Code.ToString());
        }

        protected virtual void DispatchMessageException(Exception e)
        {
            throw e;
        }

        private void DispatchMessageQueue()
        {
            MQMessage message;
            while (true)
            {
                message = MQMessageStorage.ExtractMessage();
                if (message == null) return;
                try
                {
                    DispatchMessage(message);
                } catch (Exception e)
                {
                    DispatchMessageException(e);
                }
            }
        }

        internal void ProcessMessages() 
        {
            MQApplicationMessage message;
            while (true)
            {
                message = MessageStorage.ExtractMessage();
                if (message == null) return;

                switch (message.Code)
                {
                    case (int)MQApplicationMessageCode.MC_TERMINATE:
                        Terminate();
                        return;
                    case (int)MQApplicationMessageCode.MC_DISPATCH_MESSAGE:
                        DispatchMessageQueue();
                        return;
                    case (int)MQApplicationMessageCode.MC_TIMER:
                        OnTimer();
                        return;
                }
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

        private void TimerSystem(object sender, ElapsedEventArgs e)
        {
            SendMessage(MQApplicationMessageCode.MC_TIMER);
        }

        private void Proc()
        {
            Prepare();
            OnStartup();
            timer = new Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += TimerSystem;
            try
            {
                timer.Start();

                while (!Terminated)
                {
                    try
                    {
                        LoopMessages();
                    }
                    catch (Exception e)
                    {
                        OnException(e);
                    }
                }
            } finally
            {
                timer.Dispose();
                OnShutdown();
            }
        }

        public bool Terminated { get => _terminated != 0; }
        public void Terminate()
        {
            if (!Terminated)
            {
                Interlocked.Exchange(ref _terminated, 1);
                DoTerminate();
            }
        }

        protected void DoTerminate()
        {
            Stop();
        }

        public bool Run()
        {
            if (Active != 0) return false;
            if (Terminated) return false;
            if (ActiveThread != null) return false;
            Active = 1;
            ActiveThread = new Thread(new ThreadStart(Proc));
            ActiveThread.IsBackground = true;
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
            throw new Exception("Плохая реализация метода Wait!");
        }
        public bool Stop()
        {
            if (Active == 0) return false;
            Interlocked.Exchange(ref Active, 0);
            //Terminate();
            InternalStop();
            return true;
        }
        public bool SendMessage(int Code)
        {
            if (Active == 0) return false;
            MessageStorage.CreateMessage(Code);
            InternalSendMessage();
            return true;
        }

        public bool SendMessage(MQApplicationMessageCode Code)
        {
            return SendMessage((int)Code);
        }

        ~MQApplication()
        {
            Terminate();
        }

        public MQApplication()
        {
            MQMessageStorage = new MQMessageStorage(() => SendMessage((int)MQApplicationMessageCode.MC_DISPATCH_MESSAGE));
        }

        public void MQNotify(int code, Object obj = null)
        {
            MQMessageStorage.MQNotify(code, obj);
        }
    }
}
