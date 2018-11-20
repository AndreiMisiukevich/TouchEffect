using System.Windows.Input;
using TouchEffect.Delegates;
using TouchEffect.EventArgs;
using Xamarin.Forms;
using TouchEffect.Enums;
using static System.Math;

namespace TouchEffect
{   
    public class TouchView : AbsoluteLayout
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

        public static readonly BindableProperty RegularBackgroundColorProperty = BindableProperty.Create(
            nameof(RegularBackgroundColor),
            typeof(Color),
            typeof(TouchView),
            default(Color),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as TouchView)?.ForceStateChanged();
            });

        public static readonly BindableProperty PressedBackgroundColorProperty = BindableProperty.Create(
            nameof(PressedBackgroundColor),
            typeof(Color),
            typeof(TouchView),
            default(Color),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as TouchView)?.ForceStateChanged();
            });

        public static readonly BindableProperty RegularOpacityProperty = BindableProperty.Create(
            nameof(RegularOpacity),
            typeof(double),
            typeof(TouchView),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as TouchView)?.ForceStateChanged();
            });

        public static readonly BindableProperty PressedOpacityProperty = BindableProperty.Create(
            nameof(PressedOpacity),
            typeof(double),
            typeof(TouchView),
            0.6,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as TouchView)?.ForceStateChanged();
            });

        public static readonly BindableProperty FadeDurationProperty = BindableProperty.Create(
            nameof(FadeDuration),
            typeof(int),
            typeof(TouchView),
            0);

        public static readonly BindableProperty FadeEasingProperty = BindableProperty.Create(
            nameof(FadeEasing),
            typeof(Easing),
            typeof(TouchView),
            null);

        public static readonly BindableProperty RecoverDurationProperty = BindableProperty.Create(
            nameof(RecoverDuration),
            typeof(int),
            typeof(TouchView),
            0);

        public static readonly BindableProperty RecoverEasingProperty = BindableProperty.Create(
            nameof(RecoverEasing),
            typeof(Easing),
            typeof(TouchView),
            null);

        public static readonly BindableProperty RegularBackgroundImageSourceProperty = BindableProperty.Create(
            nameof(RegularBackgroundImageSource),
            typeof(ImageSource),
            typeof(TouchView),
            default(ImageSource),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as TouchView)?.ForceStateChanged();
            });

        public static readonly BindableProperty PressedBackgroundImageSourceProperty = BindableProperty.Create(
            nameof(PressedBackgroundImageSource),
            typeof(ImageSource),
            typeof(TouchView),
            default(ImageSource),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as TouchView)?.ForceStateChanged();
            });

        public static readonly BindableProperty RegularBackgroundImageAspectProperty = BindableProperty.Create(
            nameof(RegularBackgroundImageAspect),
            typeof(Aspect),
            typeof(TouchView),
            default(Aspect),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as TouchView)?.ForceStateChanged();
            });

        public static readonly BindableProperty PressedBackgroundImageAspectProperty = BindableProperty.Create(
            nameof(PressedBackgroundImageAspect),
            typeof(Aspect),
            typeof(TouchView),
            default(Aspect),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as TouchView)?.ForceStateChanged();
            });

        public static readonly BindableProperty BackgroundImageProperty = BindableProperty.Create(
            nameof(BackgroundImage),
            typeof(Image),
            typeof(TouchView),
            default(Image),
            BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as TouchView)?.ForceStateChanged();
            });

        public TouchView()
		{
			StateChanged += OnStateChanged;
		}

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

        public Color RegularBackgroundColor
        {
            get => (Color)GetValue(RegularBackgroundColorProperty);
            set => SetValue(RegularBackgroundColorProperty, value);
        }

        public Color PressedBackgroundColor
        {
            get => (Color)GetValue(PressedBackgroundColorProperty);
            set => SetValue(PressedBackgroundColorProperty, value);
        }

        public double RegularOpacity
        {
            get => (double)GetValue(RegularOpacityProperty);
            set => SetValue(RegularOpacityProperty, value);
        }

        public double PressedOpacity
        {
            get => (double)GetValue(PressedOpacityProperty);
            set => SetValue(PressedOpacityProperty, value);
        }

        public int FadeDuration
        {
            get => (int)GetValue(FadeDurationProperty);
            set => SetValue(FadeDurationProperty, value);
        }

        public Easing FadeEasing
        {
            get => GetValue(FadeEasingProperty) as Easing;
            set => SetValue(FadeEasingProperty, value);
        }

        public int RecoverDuration
        {
            get => (int)GetValue(RecoverDurationProperty);
            set => SetValue(RecoverDurationProperty, value);
        }

        public Easing RecoverEasing
        {
            get => GetValue(RecoverEasingProperty) as Easing;
            set => SetValue(RecoverEasingProperty, value);
        }

        public ImageSource RegularBackgroundImageSource
        {
            get => GetValue(RegularBackgroundImageSourceProperty) as ImageSource;
            set => SetValue(RegularBackgroundImageSourceProperty, value);
        }

        public ImageSource PressedBackgroundImageSource
        {
            get => GetValue(PressedBackgroundImageSourceProperty) as ImageSource;
            set => SetValue(PressedBackgroundImageSourceProperty, value);
        }

        public Aspect RegularBackgroundImageAspect
        {
            get => (Aspect)GetValue(RegularBackgroundImageAspectProperty);
            set => SetValue(RegularBackgroundImageAspectProperty, value);
        }

        public Aspect PressedBackgroundImageAspect
        {
            get => (Aspect)GetValue(PressedBackgroundImageAspectProperty);
            set => SetValue(PressedBackgroundImageAspectProperty, value);
        }

        public Image BackgroundImage
        {
            get => GetValue(BackgroundImageProperty) as Image;
            set => SetValue(BackgroundImageProperty, value);
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

        protected virtual void OnStateChanged(TouchView sender, TouchStateChangedEventArgs args)
        {
            var state = args.State;

            BatchBegin();
            SetBackgroundColor(state);
            SetBackgroundImage(state);
            BatchCommit();

            SetOpacity(state);
        }

        protected override void OnChildAdded(Element child)
        {
            if (GetLayoutFlags(child) == default(AbsoluteLayoutFlags) &&
               GetLayoutBounds(child) == default(Rectangle))
            {
                SetLayoutFlags(child, AbsoluteLayoutFlags.PositionProportional);
                SetLayoutBounds(child, new Rectangle(.5, .5, -1, -1));
            }
            base.OnChildAdded(child);
        }

        protected void ForceStateChanged()
        => OnStateChanged(this, new TouchStateChangedEventArgs(State));

        protected void SetBackgroundColor(TouchState state)
        {
            var regularBackgroundColor = RegularBackgroundColor;
            var pressedBackgroundColor = PressedBackgroundColor;

            if(regularBackgroundColor == Color.Default && 
               pressedBackgroundColor == Color.Default)
            {
                return;
            }

            BackgroundColor = state == TouchState.Regular
                    ? regularBackgroundColor
                    : pressedBackgroundColor;
        }

        protected void SetBackgroundImage(TouchState state)
        {
            var regularBackgroundImageSource = RegularBackgroundImageSource;
            var pressedBackgroundImageSource = PressedBackgroundImageSource;

            if (regularBackgroundImageSource == null && 
                pressedBackgroundImageSource == null)
            {
                return;
            }

            var aspect = RegularBackgroundImageAspect;
            var source = regularBackgroundImageSource;
            if(state == TouchState.Pressed)
            {
                aspect = PressedBackgroundImageAspect;
                source = pressedBackgroundImageSource;
            }

            if(BackgroundImage == null)
            {
                BackgroundImage = new Image();
                SetLayoutFlags(BackgroundImage, AbsoluteLayoutFlags.All);
                SetLayoutBounds(BackgroundImage, new Rectangle(0, 0, 1, 1));
                Children.Insert(0, BackgroundImage);
            }
            BackgroundImage.Aspect = aspect;
            BackgroundImage.Source = source;
        }

        protected void SetOpacity(TouchState state)
        {
            var regularOpacity = RegularOpacity;
            var pressedOpacity = PressedOpacity;

            if (Abs(regularOpacity - 1) <= double.Epsilon &&
               Abs(pressedOpacity - 1) <= double.Epsilon)
            {
                return;
            }

            if (state == TouchState.Regular)
            {
                this.FadeTo(regularOpacity, (uint)Abs(RecoverDuration), RecoverEasing);
                return;
            }
            this.FadeTo(pressedOpacity, (uint)Abs(FadeDuration), FadeEasing);
        }
    }
}