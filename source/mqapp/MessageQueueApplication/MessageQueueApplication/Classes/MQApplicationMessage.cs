using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MessageQueueApplication.Classes
{
    public class MQApplicationMessageStorage
    {
        private ConcurrentQueue<MQApplicationMessage> MessageQueue = new ConcurrentQueue<MQApplicationMessage>();
        private long id = 1;

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

    public class MQApplicationMessage
    {
        internal int _code;
        public int Code { get => _code; }
    }
}
