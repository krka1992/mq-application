using MessageQueueApplication.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageQueueApplication
{
    interface IMQApplication
    {
        bool Run();
        void Wait();
        bool Stop();
        bool SendMessage(int Code);
    }

    public interface IMessageQueue
    {
        void MQNotify(int code, Object obj = null);
    }
}
