using TouchEffect.Enums;

namespace TouchEffect.EventArgs
{
    public class HoverStatusChangedEventArgs : System.EventArgs
    {
        public HoverStatusChangedEventArgs(HoverStatus status)
        {
            Status = status;
        }

        public HoverStatus Status { get; }
    }
}
