using TouchEffect.Enums;
namespace TouchEffect.EventArgs
{
    public class UserInteractionStateChangedEventArgs : System.EventArgs
    {
        public UserInteractionStateChangedEventArgs(UserInteractionState userInteractionstate)
        {
            UserInteractionState = userInteractionstate;
        }

        public UserInteractionState UserInteractionState { get; }
    }
}
