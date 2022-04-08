using System;
namespace Dispatcher
{
    public class MessageInstance
    {
        private static ObjectPool<MessageInstance> pool = new ObjectPool<MessageInstance>(40, 10);


        public string MessageType;
        public string Filter;
        public MessageHandler Handler;
        public int Priority = 0;

        public static MessageInstance Allocate()
        {
            MessageInstance lInstance = pool.Allocate();
            lInstance.MessageType = "";
            lInstance.Filter = "";
            lInstance.Handler = null;

            if (lInstance == null) { lInstance = new MessageInstance(); }
            return lInstance;
        }

        public static void Release(MessageInstance rInstance)
        {
            if (rInstance == null) { return; }

            rInstance.MessageType = "";
            rInstance.Filter = "";
            rInstance.Handler = null;

            pool.Release(rInstance);
        }
    }
}