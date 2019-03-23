using TouchEffect.Enums;
namespace TouchEffect.EventArgs
{
    public class TouchStateChangedEventArgs : System.EventArgs
	{
		public TouchStateChangedEventArgs(TouchState state)
		{
			State = state;
		}

		public TouchState State { get; }
	}
}
