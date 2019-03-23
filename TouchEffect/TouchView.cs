using System.Windows.Input;
using TouchEffect.Delegates;
using TouchEffect.EventArgs;
using Xamarin.Forms;
using TouchEffect.Enums;
using static System.Math;
using TouchEffect.Extensions;
using System.Threading.Tasks;
using System.Threading;

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
            TouchStatus.Completed,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty StateProperty = BindableProperty.Create(
            nameof(State),
            typeof(TouchState),
            typeof(TouchView),
            TouchState.Regular,
            BindingMode.OneWayToSource);

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

        public static readonly BindableProperty RegularScaleProperty = BindableProperty.Create(
            nameof(RegularScale),
            typeof(double),
            typeof(TouchView),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedScaleProperty = BindableProperty.Create(
            nameof(PressedScale),
            typeof(double),
            typeof(TouchView),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty RegularTranslationXProperty = BindableProperty.Create(
            nameof(RegularTranslationX),
            typeof(double),
            typeof(TouchView),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedTranslationXProperty = BindableProperty.Create(
            nameof(PressedTranslationX),
            typeof(double),
            typeof(TouchView),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty RegularTranslationYProperty = BindableProperty.Create(
            nameof(RegularTranslationY),
            typeof(double),
            typeof(TouchView),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedTranslationYProperty = BindableProperty.Create(
            nameof(PressedTranslationY),
            typeof(double),
            typeof(TouchView),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty RegularRotationProperty = BindableProperty.Create(
            nameof(RegularRotation),
            typeof(double),
            typeof(TouchView),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedRotationProperty = BindableProperty.Create(
            nameof(PressedRotation),
            typeof(double),
            typeof(TouchView),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty RegularRotationXProperty = BindableProperty.Create(
            nameof(RegularRotationX),
            typeof(double),
            typeof(TouchView),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedRotationXProperty = BindableProperty.Create(
            nameof(PressedRotationX),
            typeof(double),
            typeof(TouchView),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty RegularRotationYProperty = BindableProperty.Create(
            nameof(RegularRotationY),
            typeof(double),
            typeof(TouchView),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedRotationYProperty = BindableProperty.Create(
            nameof(PressedRotationY),
            typeof(double),
            typeof(TouchView),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.AsTouchView().ForceStateChanged();
            });

        public static readonly BindableProperty PressedAnimationDurationProperty = BindableProperty.Create(
            nameof(PressedAnimationDuration),
            typeof(int),
            typeof(TouchView),
            default(int));

        public static readonly BindableProperty PressedAnimationEasingProperty = BindableProperty.Create(
            nameof(PressedAnimationEasing),
            typeof(Easing),
            typeof(TouchView),
            null);

        public static readonly BindableProperty RegularAnimationDurationProperty = BindableProperty.Create(
            nameof(RegularAnimationDuration),
            typeof(int),
            typeof(TouchView),
            default(int));

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

        public static readonly BindableProperty RippleCountProperty = BindableProperty.Create(
            nameof(RippleCount),
            typeof(int),
            typeof(TouchView),
            default(int),
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

        private readonly object _backgroundImageLocker = new object();
        private CancellationTokenSource _animationTokenSource;

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

        public double RegularScale
        {
            get => (double)GetValue(RegularScaleProperty);
            set => SetValue(RegularScaleProperty, value);
        }

        public double PressedScale
        {
            get => (double)GetValue(PressedScaleProperty);
            set => SetValue(PressedScaleProperty, value);
        }

        public double RegularTranslationX
        {
            get => (double)GetValue(RegularTranslationXProperty);
            set => SetValue(RegularTranslationXProperty, value);
        }

        public double PressedTranslationX
        {
            get => (double)GetValue(PressedTranslationXProperty);
            set => SetValue(PressedTranslationXProperty, value);
        }

        public double RegularTranslationY
        {
            get => (double)GetValue(RegularTranslationYProperty);
            set => SetValue(RegularTranslationYProperty, value);
        }

        public double PressedTranslationY
        {
            get => (double)GetValue(PressedTranslationYProperty);
            set => SetValue(PressedTranslationYProperty, value);
        }

        public double RegularRotation
        {
            get => (double)GetValue(RegularRotationProperty);
            set => SetValue(RegularRotationProperty, value);
        }

        public double PressedRotation
        {
            get => (double)GetValue(PressedRotationProperty);
            set => SetValue(PressedRotationProperty, value);
        }

        public double RegularRotationX
        {
            get => (double)GetValue(RegularRotationXProperty);
            set => SetValue(RegularRotationXProperty, value);
        }

        public double PressedRotationX
        {
            get => (double)GetValue(PressedRotationXProperty);
            set => SetValue(PressedRotationXProperty, value);
        }

        public double RegularRotationY
        {
            get => (double)GetValue(RegularRotationYProperty);
            set => SetValue(RegularRotationYProperty, value);
        }

        public double PressedRotationY
        {
            get => (double)GetValue(PressedRotationYProperty);
            set => SetValue(PressedRotationYProperty, value);
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

        public int RippleCount
        {
            get => (int)GetValue(RippleCountProperty);
            set => SetValue(RippleCountProperty, value);
        }

        public Image BackgroundImage
        {
            get => GetValue(BackgroundImageProperty) as Image;
            set => SetValue(BackgroundImageProperty, value);
        }

        public void HandleTouch(TouchStatus status)
        {
            var canExecuteCommand = IsEnabled && (Command != null || Completed != null);

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

        protected async void OnStateChanged(TouchView sender, TouchStateChangedEventArgs args)
        {
            _animationTokenSource?.Cancel();
            _animationTokenSource = new CancellationTokenSource();
            var token = _animationTokenSource.Token;

            var state = args.State;
            SetBackgroundImage(state);
            var rippleCount = RippleCount;

            if (rippleCount == 0 || state == TouchState.Regular)
            {
                await GetAnimationTask(state);
                return;
            }
            do
            {
                await GetAnimationTask(TouchState.Pressed);
                if(token.IsCancellationRequested)
                {
                    return;
                }
                await GetAnimationTask(TouchState.Regular);
                if (token.IsCancellationRequested)
                {
                    return;
                }
            } while (--rippleCount != 0);
        }

        protected void ForceStateChanged()
            => OnStateChanged(this, new TouchStateChangedEventArgs(State));

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
                Children.Insert(0, BackgroundImage);
            }
            BackgroundImage.Aspect = aspect;
            BackgroundImage.Source = source;
        }

        protected async Task SetBackgroundColor(TouchState state)
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

            var animationCompletionSource = new TaskCompletionSource<bool>();
            new Animation{
                {0, 1,  new Animation(v => BackgroundColor = new Color(v, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A), BackgroundColor.R, color.R) },
                {0, 1,  new Animation(v => BackgroundColor = new Color(BackgroundColor.R, v, BackgroundColor.B, BackgroundColor.A), BackgroundColor.G, color.G) },
                {0, 1,  new Animation(v => BackgroundColor = new Color(BackgroundColor.R, BackgroundColor.G, v, BackgroundColor.A), BackgroundColor.B, color.B) },
                {0, 1,  new Animation(v => BackgroundColor = new Color(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, v), BackgroundColor.A, color.A) },
            }.Commit(this, ChangeBackgroundColorAnimationName, 16, (uint)duration, easing, (d, b) => animationCompletionSource.SetResult(true));
            await animationCompletionSource.Task;
        }

        protected async Task SetOpacity(TouchState state)
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
            await this.FadeTo(opacity, (uint)Abs(duration), easing);
        }

        protected async Task SetScale(TouchState state)
        {
            var regularScale = RegularScale;
            var pressedScale = PressedScale;

            if (Abs(regularScale - 1) <= double.Epsilon &&
               Abs(pressedScale - 1) <= double.Epsilon)
            {
                return;
            }

            var scale = regularScale;
            var duration = RegularAnimationDuration;
            var easing = RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                scale = pressedScale;
                duration = PressedAnimationDuration;
                easing = PressedAnimationEasing;
            }
            await this.ScaleTo(scale, (uint)Abs(duration), easing);
        }

        protected async Task SetTranslation(TouchState state)
        {
            var regularTranslationX = RegularTranslationX;
            var pressedTranslationX = PressedTranslationX;

            var regularTranslationY = RegularTranslationY;
            var pressedTranslationY = PressedTranslationY;

            if (Abs(regularTranslationX) <= double.Epsilon &&
                Abs(pressedTranslationX) <= double.Epsilon && 
                Abs(regularTranslationY) <= double.Epsilon &&
                Abs(pressedTranslationY) <= double.Epsilon)
            {
                return;
            }

            var translationX = regularTranslationX;
            var translationY = regularTranslationY;
            var duration = RegularAnimationDuration;
            var easing = RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                translationX = pressedTranslationX;
                translationY = pressedTranslationY;
                duration = PressedAnimationDuration;
                easing = PressedAnimationEasing;
            }

            await this.TranslateTo(translationX, translationY, (uint)Abs(duration), easing);
        }

        protected async Task SetRotation(TouchState state)
        {
            var regularRotation = RegularRotation;
            var pressedRotation = PressedRotation;

            if (Abs(regularRotation) <= double.Epsilon &&
               Abs(pressedRotation) <= double.Epsilon)
            {
                return;
            }

            var rotation = regularRotation;
            var duration = RegularAnimationDuration;
            var easing = RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                rotation = pressedRotation;
                duration = PressedAnimationDuration;
                easing = PressedAnimationEasing;
            }
            await this.RotateTo(rotation, (uint)Abs(duration), easing);
        }

        protected async Task SetRotationX(TouchState state)
        {
            var regularRotationX = RegularRotationX;
            var pressedRotationX = PressedRotationX;

            if (Abs(regularRotationX) <= double.Epsilon &&
               Abs(pressedRotationX) <= double.Epsilon)
            {
                return;
            }

            var rotationX = regularRotationX;
            var duration = RegularAnimationDuration;
            var easing = RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                rotationX = pressedRotationX;
                duration = PressedAnimationDuration;
                easing = PressedAnimationEasing;
            }
            await this.RotateXTo(rotationX, (uint)Abs(duration), easing);
        }

        protected async Task SetRotationY(TouchState state)
        {
            var regularRotationY = RegularRotationY;
            var pressedRotationY = PressedRotationY;

            if (Abs(regularRotationY) <= double.Epsilon &&
               Abs(pressedRotationY) <= double.Epsilon)
            {
                return;
            }

            var rotationY = regularRotationY;
            var duration = RegularAnimationDuration;
            var easing = RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                rotationY = pressedRotationY;
                duration = PressedAnimationDuration;
                easing = PressedAnimationEasing;
            }
            await this.RotateYTo(rotationY, (uint)Abs(duration), easing);
        }

        private Task GetAnimationTask(TouchState state)
            => Task.WhenAll(SetBackgroundColor(state),
                SetOpacity(state),
                SetScale(state),
                SetTranslation(state),
                SetRotation(state),
                SetRotationX(state),
                SetRotationY(state));
    }
}