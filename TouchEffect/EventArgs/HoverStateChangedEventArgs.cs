using TouchEffect.Enums;
namespace TouchEffect.EventArgs
{
    public class HoverStateChangedEventArgs : System.EventArgs
    {
        public HoverStateChangedEventArgs(HoverState state)
        {
            State = state;
        }

        public HoverState State { get; }
    }
}
