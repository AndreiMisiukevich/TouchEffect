using System.Windows.Input;
using TouchEffect.Delegates;
using TouchEffect.EventArgs;
using Xamarin.Forms;

namespace TouchEffect
{   
    public class TouchView : ContentView
    {
        public event TouchViewStatusChangedHandler TouchStatusChanged;
        public event TouchViewCompletedHandler TouchCompleted;

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

        public void HandleTouch(GestureStatus status)
        {
            var canExecuteCommand = !IsEnabled || Command == null || TouchCompleted == null;
            
            if(status != GestureStatus.Started || canExecuteCommand)
            {
                OnTouchHandled(status);
                TouchStatusChanged?.Invoke(this, new TouchStatusChangedEventArgs(status));
            }

            if(status == GestureStatus.Completed && canExecuteCommand)
            {
                Command?.Execute(CommandParameter);
                TouchCompleted?.Invoke(this, new TouchCompletedEventArgs(CommandParameter));
            }
        }

        protected virtual void OnTouchHandled(GestureStatus status)
        {
            Opacity = status == GestureStatus.Started ? 0.7 : 1.0; //TODO: add ability to manage it
        }
    }
}