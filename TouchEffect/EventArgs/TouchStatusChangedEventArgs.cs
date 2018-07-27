using Xamarin.Forms;

namespace TouchEffect.EventArgs
{
    public class TouchStatusChangedEventArgs : System.EventArgs
    {
        public TouchStatusChangedEventArgs(GestureStatus status)
        {
            Status = status;
        }

        public GestureStatus Status { get; }
    }
}
