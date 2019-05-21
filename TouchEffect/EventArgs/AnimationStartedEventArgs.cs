using TouchEffect.Enums;

namespace TouchEffect.EventArgs
{
    public class AnimationStartedEventArgs : System.EventArgs
    {
        public AnimationStartedEventArgs(TouchState state, int duration)
        {
            State = state;
            Duration = duration;
        }

        public TouchState State { get; }
        public int Duration { get; }
    }
}