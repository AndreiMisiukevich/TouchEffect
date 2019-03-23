namespace TouchEffect.EventArgs
{
    public class TouchCompletedEventArgs : System.EventArgs
	{
		public TouchCompletedEventArgs(object parameter)
		{
			Parameter = parameter;
		}

		public object Parameter { get; }
	}
}
