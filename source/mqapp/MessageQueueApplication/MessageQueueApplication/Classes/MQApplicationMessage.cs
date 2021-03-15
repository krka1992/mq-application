using System;
using System.Collections.Generic;
using System.Text;

namespace MessageQueueApplication.Classes
{
    public class MQApplicationMessageStorage
    {
        private List<MQApplicationMessage> MessageList;
        private long id = 1;

        public MQApplicationMessage CreateMessage(int Code, Object obj)
        {
            MQApplicationMessage message = new MQApplicationMessage();
            message.id = ++id;
            message._code = Code;
            message.obj = obj;
            lock (MessageList)
            {
                MessageList.Add(message);
            }
            return message;
        }

        public MQApplicationMessage GetMessage(long id)
        {
            lock (MessageList)
            {
                return MessageList.Find(delegate (MQApplicationMessage message) { return message.id == id; });
            }
        }

        public void DeleteMessage(long id)
        {
            lock (MessageList)
            {
                MQApplicationMessage message = MessageList.Find(delegate (MQApplicationMessage message) { return message.id == id; });
                MessageList.Remove(message);
            }
        }

        public MQApplicationMessage ExtractMessage(long id)
        {
            lock (MessageList)
            {
                MQApplicationMessage message = MessageList.Find(delegate (MQApplicationMessage message) { return message.id == id; });
                MessageList.Remove(message);
                return message;
            }
        }
    }

    public class MQApplicationMessage
    {
        internal long id;
        internal Object obj;
        internal int _code;
        public int Code { get => _code; }

        public Object GetObject()
        {
            return obj;
        }
    }
}
