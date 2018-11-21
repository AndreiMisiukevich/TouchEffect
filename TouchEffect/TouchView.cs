using System.Windows.Input;
using TouchEffect.Delegates;
using TouchEffect.EventArgs;
using Xamarin.Forms;
using TouchEffect.Enums;
using static System.Math;
using TouchEffect.Extensions;

namespace TouchEffect
{
    public class TouchView : AbsoluteLayout
    {
        public const string ChangeBackgroundColorAnimationName = nameof(ChangeBackgroundColorAnimationName);

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
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedBackgroundColorProperty = BindableProperty.Create(
            nameof(PressedBackgroundColor),
            typeof(Color),
            typeof(TouchView),
            default(Color),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty RegularOpacityProperty = BindableProperty.Create(
            nameof(RegularOpacity),
            typeof(double),
            typeof(TouchView),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedOpacityProperty = BindableProperty.Create(
            nameof(PressedOpacity),
            typeof(double),
            typeof(TouchView),
            0.6,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedAnimationDurationProperty = BindableProperty.Create(
            nameof(PressedAnimationDuration),
            typeof(int),
            typeof(TouchView),
            0);

        public static readonly BindableProperty PressedAnimationEasingProperty = BindableProperty.Create(
            nameof(PressedAnimationEasing),
            typeof(Easing),
            typeof(TouchView),
            null);

        public static readonly BindableProperty RegularAnimationDurationProperty = BindableProperty.Create(
            nameof(RegularAnimationDuration),
            typeof(int),
            typeof(TouchView),
            0);

        public static readonly BindableProperty RegularAnimationEasingProperty = BindableProperty.Create(
            nameof(RegularAnimationEasing),
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
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedBackgroundImageSourceProperty = BindableProperty.Create(
            nameof(PressedBackgroundImageSource),
            typeof(ImageSource),
            typeof(TouchView),
            default(ImageSource),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty RegularBackgroundImageAspectProperty = BindableProperty.Create(
            nameof(RegularBackgroundImageAspect),
            typeof(Aspect),
            typeof(TouchView),
            default(Aspect),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedBackgroundImageAspectProperty = BindableProperty.Create(
            nameof(PressedBackgroundImageAspect),
            typeof(Aspect),
            typeof(TouchView),
            default(Aspect),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty BackgroundImageProperty = BindableProperty.Create(
            nameof(BackgroundImage),
            typeof(Image),
            typeof(TouchView),
            default(Image),
            BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
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

        public int PressedAnimationDuration
        {
            get => (int)GetValue(PressedAnimationDurationProperty);
            set => SetValue(PressedAnimationDurationProperty, value);
        }

        public Easing PressedAnimationEasing
        {
            get => GetValue(PressedAnimationEasingProperty) as Easing;
            set => SetValue(PressedAnimationEasingProperty, value);
        }

        public int RegularAnimationDuration
        {
            get => (int)GetValue(RegularAnimationDurationProperty);
            set => SetValue(RegularAnimationDurationProperty, value);
        }

        public Easing RegularAnimationEasing
        {
            get => GetValue(RegularAnimationEasingProperty) as Easing;
            set => SetValue(RegularAnimationEasingProperty, value);
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

            if (status != TouchStatus.Started || canExecuteCommand)
            {
                State = status == TouchStatus.Started
                  ? TouchState.Pressed
                  : TouchState.Regular;

                Status = status;
                StateChanged?.Invoke(this, new TouchStateChangedEventArgs(State));
                StatusChanged?.Invoke(this, new TouchStatusChangedEventArgs(Status));
            }

            if (status == TouchStatus.Completed && canExecuteCommand)
            {
                Command?.Execute(CommandParameter);
                Completed?.Invoke(this, new TouchCompletedEventArgs(CommandParameter));
            }
        }

        protected virtual void OnStateChanged(TouchView sender, TouchStateChangedEventArgs args)
        {
            var state = args.State;
            SetBackgroundImage(state);
            SetBackgroundColor(state);
            SetOpacity(state);
        }

        protected override void OnChildAdded(Element child)
        {
            if (GetLayoutFlags(child) == default(AbsoluteLayoutFlags) &&
               GetLayoutBounds(child) == default(Rectangle))
            {
                SetLayoutFlags(child, AbsoluteLayoutFlags.All);
                SetLayoutBounds(child, new Rectangle(0, 0, 1, 1));
            }
            base.OnChildAdded(child);
        }

        protected void ForceStateChanged()
        => OnStateChanged(this, new TouchStateChangedEventArgs(State));

        protected void SetBackgroundColor(TouchState state)
        {
            var regularBackgroundColor = RegularBackgroundColor;
            var pressedBackgroundColor = PressedBackgroundColor;

            if (regularBackgroundColor == Color.Default &&
               pressedBackgroundColor == Color.Default)
            {
                return;
            }

            var color = regularBackgroundColor;
            var duration = RegularAnimationDuration;
            var easing = RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                color = pressedBackgroundColor;
                duration = PressedAnimationDuration;
                easing = PressedAnimationEasing;
            }

            if (duration <= 0)
            {
                BackgroundColor = color;
                return;
            }

            new Animation{
                {0, 1,  new Animation(v => BackgroundColor = new Color(v, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A), BackgroundColor.R, color.R) },
                {0, 1,  new Animation(v => BackgroundColor = new Color(BackgroundColor.R, v, BackgroundColor.B, BackgroundColor.A), BackgroundColor.G, color.G) },
                {0, 1,  new Animation(v => BackgroundColor = new Color(BackgroundColor.R, BackgroundColor.G, v, BackgroundColor.A), BackgroundColor.B, color.B) },
                {0, 1,  new Animation(v => BackgroundColor = new Color(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, v), BackgroundColor.A, color.A) },
            }.Commit(this, ChangeBackgroundColorAnimationName, 16, (uint)duration, easing);
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
            if (state == TouchState.Pressed)
            {
                aspect = PressedBackgroundImageAspect;
                source = pressedBackgroundImageSource;
            }

            if (BackgroundImage == null)
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

            var opacity = regularOpacity;
            var duration = RegularAnimationDuration;
            var easing = RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                opacity = pressedOpacity;
                duration = PressedAnimationDuration;
                easing = PressedAnimationEasing;
            }
            this.FadeTo(opacity, (uint)Abs(duration), easing);
        }
    }
}