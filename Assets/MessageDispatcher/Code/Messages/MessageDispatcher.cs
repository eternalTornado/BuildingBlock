using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Dispatcher
{
    // This class just propose for findReferences easier
    public class MessageSender
    {
        public static void SendMessage(string _type, float delayTime = 0f)
        {
            Message message = Message.Allocate();
            message.Sender = null;
            message.Recipient = string.Empty;
            message.Type = _type;
            message.Data = null;
            message.Delay = delayTime;

            SendMessage(message);
        }

        public static void SendMessage(string _type, string rFilter, float delayTime = 0f)
        {
            Message message = Message.Allocate();
            message.Sender = null;
            message.Recipient = rFilter;
            message.Type = _type;
            message.Data = null;
            message.Delay = delayTime;

            SendMessage(message);
        }

        public static void SendMessageData(string _type, object data, float delayTime = 0f)
        {
            Message message = Message.Allocate();
            message.Sender = null;
            message.Recipient = string.Empty;
            message.Type = _type;
            message.Data = data;
            message.Delay = delayTime;

            SendMessage(message);
        }

        public static void SendMessage(object sender, string _type, object data, float delayTime)
        {
            Message message = Message.Allocate();
            message.Sender = sender;
            message.Recipient = string.Empty;
            message.Type = _type;
            message.Data = data;
            message.Delay = delayTime;

            SendMessage(message);
        }

        public static void SendMessage(object sender, object rRecipient, string _type, object data, float delayTime)
        {
            Message message = Message.Allocate();
            message.Sender = sender;
            message.Recipient = (rRecipient != null ? rRecipient : string.Empty);
            message.Type = _type;
            message.Data = data;
            message.Delay = delayTime;

            SendMessage(message);
        }

        public static void SendMessage(object sender, string rRecipient, string _type, object data, float delayTime)
        {
            Message message = Message.Allocate();
            message.Sender = sender;
            message.Recipient = rRecipient;
            message.Type = _type;
            message.Data = data;
            message.Delay = delayTime;

            SendMessage(message);
        }

        protected static void SendMessage(IMessage message)
        {
            MessageDispatcher.SendMessage(message);

            if (message.Delay == DelayType.Immdiate)
                message.Release();
        }
    }
    public class MessageDispatcher
    {
        private static int recipientType = Filter.Name;

        public static MessageHandler MessageNotHandled = null;
        public static int FrameCounter = 0;

        // for using unity Update.
        private static DispatcherTimer dispatchTimer = (new UnityEngine.GameObject("DispatcherTimer")).AddComponent<DispatcherTimer>();

        private static List<IMessage> messages = new List<IMessage>();

        private static Dictionary<string, Dictionary<string, MessageHandler>> messageHandlers = new Dictionary<string, Dictionary<string, MessageHandler>>();

        private static List<MessageInstance> listenersWaitingAdd = new List<MessageInstance>();
        private static List<MessageInstance> listenersWaitingRemove = new List<MessageInstance>();

        public static void ClearMessages()
        {
            messages.Clear();
        }

        public static void ClearListeners()
        {
            foreach (string lType in messageHandlers.Keys)
            {
                messageHandlers[lType].Clear();
            }

            messageHandlers.Clear();

            listenersWaitingAdd.Clear();
            listenersWaitingRemove.Clear();
        }

        #region Add Listener
        public static void AddListener(string messageType, MessageHandler handler)
        {
            AddListener(messageType, string.Empty, handler, true);
        }
        
        public static void AddListener(string messageType, MessageHandler handler, bool immediate)
        {
            AddListener(messageType, string.Empty, handler, immediate);
        }

        public static void AddListener(UnityEngine.Object owner, string messageType, MessageHandler handler)
        {
            AddListener(owner, messageType, handler, false);
        }

        public static void AddListener(UnityEngine.Object owner, string messageType, MessageHandler handler, bool immediate)
        {
            if (owner == null)
            {
                AddListener(messageType, string.Empty, handler, immediate);
                return;
            }

            switch (recipientType)
            {
                case Filter.Name:
                    {
                        if (owner is UnityEngine.Object)
                        {
                            AddListener(messageType, ((UnityEngine.Object)owner).name, handler, immediate);
                        }
                    }
                    break;
                case Filter.Tag:
                    {
                        if (owner is UnityEngine.GameObject)
                        {
                            AddListener(messageType, ((UnityEngine.GameObject)owner).tag, handler, immediate);
                        }
                    }
                    break;
                default:
                    {
                        AddListener(messageType, string.Empty, handler, immediate);
                    }
                    break;
            }
        }

        public static void AddListener(string messageType, string rFilter, MessageHandler handler)
        {
            AddListener(messageType, rFilter, handler, false);
        }

        public static void AddListener(string messageType, string rFilter, MessageHandler handler, bool immediate)
        {
            MessageInstance message = MessageInstance.Allocate();
            message.MessageType = messageType;
            message.Filter = rFilter;
            message.Handler = handler;

            if (immediate)
            {
                AddListener(message);
                //MessageListenerDefinition.Release(message);
            }
            else
            {
                listenersWaitingAdd.Add(message);
            }
        }

        private static void AddListener(MessageInstance listener)
        {
            Dictionary<string, MessageHandler> lRecipientDictionary = null;

            if (messageHandlers.ContainsKey(listener.MessageType))
            {
                lRecipientDictionary = messageHandlers[listener.MessageType];
            }
            else
            {
                lRecipientDictionary = new Dictionary<string, MessageHandler>();
                messageHandlers.Add(listener.MessageType, lRecipientDictionary);
            }


            if (!lRecipientDictionary.ContainsKey(listener.Filter))
            {
                lRecipientDictionary.Add(listener.Filter, null);
            }

            lRecipientDictionary[listener.Filter] += listener.Handler;

            
            MessageInstance.Release(listener);
        }
        #endregion

        #region Remove Listener
        public static void RemoveListener(string messageType, MessageHandler handler)
        {
            RemoveListener(messageType, string.Empty, handler, true);
        }

        public static void RemoveListener(string messageType, MessageHandler handler, bool immediate)
        {
            RemoveListener(messageType, string.Empty, handler, immediate);
        }

        public static void RemoveListener(UnityEngine.Object owner, string messageType, MessageHandler handler)
        {
            RemoveListener(owner, messageType, handler, false);
        }

        public static void RemoveListener(UnityEngine.Object owner, string messageType, MessageHandler handler, bool immediate)
        {
            if (owner == null)
            {
                RemoveListener(messageType, string.Empty, handler, immediate);
                return;
            }

            switch (recipientType)
            {
                case Filter.Name:
                    {
                        if (owner is UnityEngine.Object)
                        {
                            RemoveListener(messageType, ((UnityEngine.Object)owner).name, handler, immediate);
                        }
                    }
                    break;
                case Filter.Tag:
                    {
                        if (owner is UnityEngine.GameObject)
                        {
                            RemoveListener(messageType, ((UnityEngine.GameObject)owner).tag, handler, immediate);
                        }
                    }
                    break;
                default:
                    {
                        RemoveListener(messageType, string.Empty, handler, immediate);
                    }
                    break;
            }
        }

        public static void RemoveListener(string messageType, string rFilter, MessageHandler handler)
        {
            RemoveListener(messageType, rFilter, handler, false);
        }

        
        public static void RemoveListener(string messageType, string rFilter, MessageHandler handler, bool immediate)
        {
            MessageInstance message = MessageInstance.Allocate();
            message.MessageType = messageType;
            message.Filter = rFilter;
            message.Handler = handler;

            if (immediate)
            {
                RemoveListener(message);
                //MessageListenerDefinition.Release(message);
            }
            else
            {
                listenersWaitingRemove.Add(message);
            }
        }

        private static void RemoveListener(MessageInstance listener)
        {
            if (string.IsNullOrEmpty(listener.MessageType))
            {
                MessageInstance.Release(listener);
                return;
            }

            if (messageHandlers.ContainsKey(listener.MessageType))
            {
                if (listener.Filter == null)
                {
                    MessageInstance.Release(listener);
                    return;
                }

                if (messageHandlers[listener.MessageType].ContainsKey(listener.Filter))
                {
                    if (messageHandlers[listener.MessageType][listener.Filter] != null 
                                                                            && listener.Handler != null)
                    {
                        messageHandlers[listener.MessageType][listener.Filter] -= listener.Handler;
                    }

                    if (messageHandlers[listener.MessageType][listener.Filter] == null)
                    {
                        messageHandlers[listener.MessageType].Remove(listener.Filter);
                    }

                    if (messageHandlers[listener.MessageType].Count == 0)
                    {
                        messageHandlers.Remove(listener.MessageType);
                    }
                }
            }

            MessageInstance.Release(listener);
        }
        #endregion

        #region SendMessage
        public static void SendMessage(IMessage message)
        {
            if (message == null) { return; }

            bool isMissing = true;

            if (message.Delay > 0 || message.Delay < 0)
            {
                EnWaittingQueue(message);
                isMissing = false;
            }

            else if (messageHandlers.ContainsKey(message.Type))
            {                
                Dictionary<string, MessageHandler> AllHandlers = messageHandlers[message.Type];

                var listHandlers = AllHandlers.ToList();
                foreach (var pair in listHandlers)
                {
                    string filter = pair.Key;

                    if (AllHandlers[filter] == null)
                    {
                        RemoveListener(message.Type, filter, null);
                        continue;
                    }


                    if (filter.Equals(string.Empty))
                    {
                        message.IsSend = true;
                        AllHandlers[filter].Invoke(message);

                        isMissing = false;
                    }
                    else if (recipientType == Filter.Name && message.Recipient is UnityEngine.Object)
                    {
                        int instId = 0;
                        int.TryParse(filter, out instId);

                        if (instId == ((UnityEngine.Object)message.Recipient).GetInstanceID())
                        {
                            message.IsSend = true;
                            AllHandlers[filter](message);

                            isMissing = false;
                        }
                    }
                    else if (recipientType == Filter.Tag && message.Recipient is UnityEngine.GameObject)
                    {
                        if (filter == ((UnityEngine.GameObject)message.Recipient).tag) {
                            message.IsSend = true;
                            AllHandlers[filter](message);

                            isMissing = false;
                        }
                    }
                    else if (message.Recipient is string)
                    {
                        if (filter.ToLower() == ((string)message.Recipient).ToLower())
                        {
                            AllHandlers[filter].Invoke(message);
                            message.IsSend = true;
                            isMissing = false;
                        }
                    }
                }
            }

            if (isMissing)
            {
                if (MessageNotHandled == null)
                {

                }
                else
                {
                    MessageNotHandled(message);
                }

                message.IsHandled = true;
            }
        }

        protected static void EnWaittingQueue(IMessage message)
        {
            if (!messages.Contains(message))
            {
                message.FrameIndex = FrameCounter;
                messages.Add(message);
            }
        }
        #endregion

        #region ================ Logic ==========
        protected static void RemoveMessgageHandled()
        {
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                IMessage message = messages[i];
                if (message.IsSend || message.IsHandled)
                {
                    messages.RemoveAt(i);

                    if (message.IsHandled)
                    {
                        message.Release();
                    }
                }
            }

            for (int i = listenersWaitingRemove.Count - 1; i >= 0; i--)
            {
                RemoveListener(listenersWaitingRemove[i]);
            }

            listenersWaitingRemove.Clear();
        }

        protected static void AddWaitingMessage()
        {
            for (int i = listenersWaitingAdd.Count - 1; i >= 0; i--)
            {
                AddListener(listenersWaitingAdd[i]);
            }

            listenersWaitingAdd.Clear();
        }
        protected static void ExecuteMessageDelay()
        {
            for (int i = 0; i < messages.Count; i++)
            {
                IMessage message = messages[i];

                if (message.Delay == DelayType.Next)
                {
                    if (message.FrameIndex < FrameCounter)
                    {
                        message.Delay = DelayType.Immdiate;
                    }
                }
                else
                {
                    message.Delay -= UnityEngine.Time.deltaTime;
                    if (message.Delay < 0)
                    {
                        message.Delay = DelayType.Immdiate;
                    }
                }

                if (!message.IsSend && message.Delay == DelayType.Immdiate)
                {
                    SendMessage(message);
                }
            }
        }
        public static void Update()
        {
            ExecuteMessageDelay();

            RemoveMessgageHandled();

            AddWaitingMessage();
        }
        #endregion
    }
}
