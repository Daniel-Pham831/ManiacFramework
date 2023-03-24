namespace Maniac.MessengerSystem.Messages
{
    /// <summary>
    /// Inherits this class to make a custom Message
    /// </summary>
    public abstract class Message
    {
        public string Type => ToString();
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}