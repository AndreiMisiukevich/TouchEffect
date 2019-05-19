using System.Windows.Input;
using TouchEffect.Delegates;
using TouchEffect.EventArgs;
using Xamarin.Forms;
using TouchEffect.Enums;
using TouchEffect.Extensions;
using System;

namespace TouchEffect
{
    [Obsolete("Please use TEffect as Effect instead, or TouchImage if you wish to update image source during touch interaction")]
    public class TouchView : AbsoluteLayout, ITouchEff
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

        private readonly TouchVisualManager _visualManager;

        public TouchView()
        {
            _visualManager = new TouchVisualManager(Device.iOS);
            StateChanged += (sender, args) => ForceStateChanged();
            if (Device.RuntimePlatform == Device.iOS)
            {
                GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => _visualManager.OnTapped(this))
                });
            }
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

        public bool IsCompletedSet => Completed != null;

        public VisualElement Control
        {
            get => this;
#pragma warning disable
            set
            {
            }
#pragma warning restore
        }

        protected override void OnChildAdded(Element child)
        {
            if (GetLayoutFlags(child) == AbsoluteLayoutFlags.None &&
               GetLayoutBounds(child) == new Rectangle(0, 0, -1, -1))
            {
                SetLayoutFlags(child, AbsoluteLayoutFlags.All);
                SetLayoutBounds(child, new Rectangle(0, 0, 1, 1));
            }
            base.OnChildAdded(child);
        }

        public void HandleTouch(TouchStatus status)
            => _visualManager.HandleTouch(this, status);

        public void RaiseStateChanged()
            => StateChanged?.Invoke(this, new TouchStateChangedEventArgs(State));

        public void RaiseStatusChanged()
            => StatusChanged?.Invoke(this, new TouchStatusChangedEventArgs(Status));

        public void RaiseCompleted()
            => Completed?.Invoke(this, new TouchCompletedEventArgs(CommandParameter));

        private void SetBackgroundImage(TouchState state)
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

        private void ForceStateChanged()
        {
            var state = State;
            SetBackgroundImage(state);
            _visualManager.ChangeStateAsync(this, state, true);
        }
    }
}