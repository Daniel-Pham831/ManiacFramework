using System;
using System.Collections.Generic;
using System.Linq;
using Maniac.MessengerSystem.Messages;
using UnityEngine;

namespace Maniac.MessengerSystem.Base
{
    public class Messenger
    {
        private static Dictionary<Type, List<IMessageListener>> messagesMap = new Dictionary<Type, List<IMessageListener>>();

        public static void Register(IMessageListener messageListener, Type type)
        {
            if (!Messenger.messagesMap.ContainsKey(type))
            {
                Messenger.messagesMap.Add(type, new List<IMessageListener>());
            }

            List<IMessageListener> listeners = Messenger.messagesMap[type];
            if (listeners.IndexOf(messageListener) < 0)
            {
                listeners.Add(messageListener);
            }
        }

        public static void Unregister(IMessageListener messageListener, Type typeToRemove)
        {
            if (Messenger.messagesMap.ContainsKey(typeToRemove))
            {
                List<IMessageListener> listeners = Messenger.messagesMap[typeToRemove];
                if (listeners.IndexOf(messageListener) != -1)
                {
                    listeners.Remove(messageListener);
                }
            }
        }

        public static void ResetMessenger()
        {
            Messenger.messagesMap.Clear();
        }

        public static void UnregisterAll(IMessageListener messageListener)
        {
            foreach (KeyValuePair<Type, List<IMessageListener>> item in Messenger.messagesMap)
            {
                if (item.Value.Contains(messageListener))
                {
                    item.Value.Remove(messageListener);
                }
            }
        }

        public static void SendMessage(Message messageToSend)
        {
            if (Messenger.messagesMap.TryGetValue(messageToSend.GetType(), out List<IMessageListener> listeners))
            {
                List<IMessageListener> copyListeners = listeners.ToList(); // to fix modified list bug
                copyListeners.ForEach(x => x.OnMessagesReceived(messageToSend));
            }
            else
            {
                Debug.LogWarning($"This {messageToSend.Type} isn't being listened by anything!\nYou can cleanup if needed.");
            }
        }
    }
}