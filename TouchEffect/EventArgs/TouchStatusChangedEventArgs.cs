using System;

namespace TouchEffect
{
    public class TouchStatusChangedEventArgs : EventArgs
    {
        internal TouchStatusChangedEventArgs(TouchStatus status)
            => Status = status;

        public TouchStatus Status { get; }
    }
}
