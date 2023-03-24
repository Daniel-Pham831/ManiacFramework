/*
    How to use

    class Test : MonoBehaviour extends IMessageListener

    private void Awake()
    {
        Messenger.RegisterNewMessageListener(this, typeof(TestMessage));
    }

    public void OnMessagesReceived(Message receivedMessage)
    {
        switch (receivedMessage.Type)
        {
            case nameof(TestMessage):
                var message = (TestMessage)receivedMessage;
                break;
        }
    }
*/

using Maniac.MessengerSystem.Messages;

namespace Maniac.MessengerSystem.Base
{
    public interface IMessageListener
    {
        void OnMessagesReceived(Message receivedMessage);
    }
}