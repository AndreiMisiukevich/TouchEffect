using System.Windows.Input;
using TouchEffect.Delegates;
using TouchEffect.EventArgs;
using Xamarin.Forms;
using TouchEffect.Enums;

namespace TouchEffect
{   
    public class TouchView : ContentView
    {
		public event TouchViewStatusChangedHandler StatusChanged;

		public event TouchViewStateChangedHandler StateChanged;
        
        public event TouchViewCompletedHandler Completed;
       
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(TouchView),
            default(ICommand));

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(TouchView),
            default(object));

		public static readonly BindableProperty StatusProperty = BindableProperty.Create(
			nameof(Status),
			typeof(TouchStatus),
			typeof(TouchView),
			TouchStatus.Completed); 
        
		public static readonly BindableProperty StateProperty = BindableProperty.Create(
			nameof(State),
			typeof(TouchState),
            typeof(TouchView),
            TouchState.Regular); 

        public ICommand Command
        {
            get => GetValue(CommandProperty) as ICommand;
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

		public TouchStatus Status
        {
			get => (TouchStatus)GetValue(StatusProperty);
			set => SetValue(StatusProperty, value);
        }

		public TouchState State
        {
			get => (TouchState)GetValue(StateProperty);
			set => SetValue(StateProperty, value);
        }

		public void HandleTouch(TouchStatus status)
        {
			var canExecuteCommand = !IsEnabled || Command == null || Completed == null;
            
			if(status != TouchStatus.Started || canExecuteCommand)
            {

				State = status == TouchStatus.Started
                  ? TouchState.Pressed
                  : TouchState.Regular;
				
				Status = status;
                StateChanged?.Invoke(this, new TouchStateChangedEventArgs(State));
				StatusChanged?.Invoke(this, new TouchStatusChangedEventArgs(Status));
            }

			if(status == TouchStatus.Completed && canExecuteCommand)
            {
                Command?.Execute(CommandParameter);
				Completed?.Invoke(this, new TouchCompletedEventArgs(CommandParameter));
            }
        }
    }
}