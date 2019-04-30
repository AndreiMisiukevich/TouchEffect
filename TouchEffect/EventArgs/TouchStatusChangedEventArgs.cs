using TouchEffect.Enums;

namespace TouchEffect.EventArgs
{
    public class TouchStatusChangedEventArgs : System.EventArgs
    {
        public TouchStatusChangedEventArgs(TouchStatus status)
        {
            Status = status;
        }

        public TouchStatus Status { get; }
    }
}
