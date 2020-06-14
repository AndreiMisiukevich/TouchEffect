using TouchEffect.Enums;

namespace TouchEffect.EventArgs
{
    public class AnimationStartedEventArgs : System.EventArgs
    {
        public AnimationStartedEventArgs(TouchState touchState, HoverState hoverState, int duration)
        {
            State = touchState;
            HoverState = hoverState;
            Duration = duration;
        }

        public TouchState State { get; }
        public HoverState HoverState { get; }
        public int Duration { get; }
    }
}