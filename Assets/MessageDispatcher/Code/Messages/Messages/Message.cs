namespace Dispatcher
{
    public class Message : IMessage
    {
        private static ObjectPool<Message> pool = new ObjectPool<Message>(40, 10);
        public static Message Allocate()
        {
            Message instance = pool.Allocate();
            if (instance == null)
                instance = new Message();

            instance.Clear();
            
            return instance;
        }

        public static void Release(Message message)
        {
            if (message == null)
                return;

            message.IsSend = true;
            message.IsHandled = true;

            pool.Release(message);
        }


        public static void Release(IMessage message)
        {
            if (message == null)
                return;

            message.Clear();

            message.IsSend = true;
            message.IsHandled = true;

            pool.Release((Message)message);
        }

        // ====

        protected string @type = "";
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        protected object sender = null;
        public object Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        protected object recipient = null;
        public object Recipient
        {
            get { return recipient; }
            set { recipient = value; }
        }

        protected float delay = 0;
        public float Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        protected int id = 0;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        protected object data = null;
        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        protected bool isSend = false;
        public bool IsSend
        {
            get { return isSend; }
            set { isSend = value; }
        }

        protected bool isHandled = false;
        public bool IsHandled
        {
            get { return isHandled; }
            set { isHandled = value; }
        }

        protected int frameIndex = 0;
        public int FrameIndex
        {
            get { return frameIndex; }
            set { frameIndex = value; }
        }

        public virtual void Clear()
        {
            type = string.Empty;
            sender = null;
            recipient = null;
            id = 0;
            data = null;
            isSend = false;
            isHandled = false;
            delay = 0.0f;
        }


        public virtual void Release()
        {
            Clear();

            IsSend = true;
            IsHandled = true;

            pool.Release(this);
        }

    }
}
