namespace Virtupad
{
    public class UIRegainFocusMessage
    {
        public enum RegainFocusMessageType
        {
            Accept,
            Abort,
            Restore,
            None
        };

        public RegainFocusMessageType messageType;

        public UIRegainFocusMessage(RegainFocusMessageType messageType)
        {
            this.messageType = messageType;
        }

        public static UIRegainFocusMessage GetBasic(RegainFocusMessageType type) => new UIRegainFocusMessage(type);

        public static UIRegainFocusMessage Empty => new UIRegainFocusMessage(RegainFocusMessageType.None);
    }
}
