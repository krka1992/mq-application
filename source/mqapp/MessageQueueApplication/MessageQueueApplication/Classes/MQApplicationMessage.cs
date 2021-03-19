using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MessageQueueApplication.Classes
{
    public class MQApplicationMessage
    {
        internal int _code;
        public int Code { get => _code; }
    }

    public class MQApplicationMessageStorage
    {
        private ConcurrentQueue<MQApplicationMessage> MessageQueue = new ConcurrentQueue<MQApplicationMessage>();

        public MQApplicationMessage CreateMessage(int Code)
        {
            MQApplicationMessage message = new MQApplicationMessage();
            message._code = Code;
            MessageQueue.Enqueue(message);
            return message;
        }

        public MQApplicationMessage ExtractMessage()
        {
            MQApplicationMessage message;
            if (MessageQueue.TryDequeue(out message)) return message;
            return null;
        }
    }
}
