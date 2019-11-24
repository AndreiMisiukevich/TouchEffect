using System.Windows.Input;
using TouchEffect.Delegates;
using TouchEffect.EventArgs;
using Xamarin.Forms;
using TouchEffect.Enums;
using TouchEffect.Extensions;
using System.ComponentModel;
using System;
using System.Threading.Tasks;
using System.Threading;
using TouchEffect.Interfaces;

namespace TouchEffect
{
    public class TouchEff : RoutingEffect, ITouchEff
    {
        private readonly TouchVisualManager _visualManager;
        private VisualElement _control;

        public TouchEff() : base($"{nameof(TouchEffect)}.{nameof(TouchEff)}")
        {
            _visualManager = new TouchVisualManager();
            StateChanged += (sender, args) => ForceUpdateState();
        }

        internal TouchEff(Func<ITouchEff, TouchState, int, CancellationToken, Task> animationTaskGetter) : this()
            => _visualManager.SetCustomAnimationTask(animationTaskGetter);

        public event TEffectStatusChangedHandler StatusChanged;

        public event TEffectStateChangedHandler StateChanged;

        public event TEffectHoverStatusChangedHandler HoverStatusChanged;

        public event TEffectHoverStateChangedHandler HoverStateChanged;

        public event TEffectCompletedHandler Completed;

        public event AnimationStartedHandler AnimationStarted;

        public static readonly BindableProperty CommandProperty = BindableProperty.CreateAttached(
            nameof(Command),
            typeof(ICommand),
            typeof(TouchEff),
            default(ICommand));

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached(
            nameof(CommandParameter),
            typeof(object),
            typeof(TouchEff),
            default(object));

        public static readonly BindableProperty StatusProperty = BindableProperty.CreateAttached(
            nameof(Status),
            typeof(TouchStatus),
            typeof(TouchEff),
            TouchStatus.Completed,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty StateProperty = BindableProperty.CreateAttached(
            nameof(State),
            typeof(TouchState),
            typeof(TouchEff),
            TouchState.Regular,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty HoverStatusProperty = BindableProperty.CreateAttached(
            nameof(HoverStatus),
            typeof(HoverStatus),
            typeof(TouchEff),
            HoverStatus.Exited,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty HoverStateProperty = BindableProperty.CreateAttached(
            nameof(HoverState),
            typeof(HoverState),
            typeof(TouchEff),
            HoverState.Regular,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty RegularBackgroundColorProperty = BindableProperty.CreateAttached(
            nameof(RegularBackgroundColor),
            typeof(Color),
            typeof(TouchEff),
            default(Color),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedBackgroundColorProperty = BindableProperty.CreateAttached(
            nameof(PressedBackgroundColor),
            typeof(Color),
            typeof(TouchEff),
            default(Color),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularOpacityProperty = BindableProperty.CreateAttached(
            nameof(RegularOpacity),
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedOpacityProperty = BindableProperty.CreateAttached(
            nameof(PressedOpacity),
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularScaleProperty = BindableProperty.CreateAttached(
            nameof(RegularScale),
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedScaleProperty = BindableProperty.CreateAttached(
            nameof(PressedScale),
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularTranslationXProperty = BindableProperty.CreateAttached(
            nameof(RegularTranslationX),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedTranslationXProperty = BindableProperty.CreateAttached(
            nameof(PressedTranslationX),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularTranslationYProperty = BindableProperty.CreateAttached(
            nameof(RegularTranslationY),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedTranslationYProperty = BindableProperty.CreateAttached(
            nameof(PressedTranslationY),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularRotationProperty = BindableProperty.CreateAttached(
            nameof(RegularRotation),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedRotationProperty = BindableProperty.CreateAttached(
            nameof(PressedRotation),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularRotationXProperty = BindableProperty.CreateAttached(
            nameof(RegularRotationX),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedRotationXProperty = BindableProperty.CreateAttached(
            nameof(PressedRotationX),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularRotationYProperty = BindableProperty.CreateAttached(
            nameof(RegularRotationY),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedRotationYProperty = BindableProperty.CreateAttached(
            nameof(PressedRotationY),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedAnimationDurationProperty = BindableProperty.CreateAttached(
            nameof(PressedAnimationDuration),
            typeof(int),
            typeof(TouchEff),
            default(int));

        public static readonly BindableProperty PressedAnimationEasingProperty = BindableProperty.CreateAttached(
            nameof(PressedAnimationEasing),
            typeof(Easing),
            typeof(TouchEff),
            null);

        public static readonly BindableProperty RegularAnimationDurationProperty = BindableProperty.CreateAttached(
            nameof(RegularAnimationDuration),
            typeof(int),
            typeof(TouchEff),
            default(int));

        public static readonly BindableProperty RegularAnimationEasingProperty = BindableProperty.CreateAttached(
            nameof(RegularAnimationEasing),
            typeof(Easing),
            typeof(TouchEff),
            null);

        public static readonly BindableProperty RippleCountProperty = BindableProperty.CreateAttached(
            nameof(RippleCount),
            typeof(int),
            typeof(TouchEff),
            default(int),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty IsToggledProperty = BindableProperty.CreateAttached(
            nameof(IsToggled),
            typeof(bool?),
            typeof(TouchEff),
            default(bool?),
            BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState(false);
            });

        public static readonly BindableProperty NativeAnimationProperty = BindableProperty.CreateAttached(
            nameof(NativeAnimation),
            typeof(bool),
            typeof(TouchEff),
            false,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty NativeAnimationColorProperty = BindableProperty.CreateAttached(
            nameof(NativeAnimationColor),
            typeof(Color),
            typeof(TouchEff),
            Color.Default,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty AndroidRippleProperty = BindableProperty.CreateAttached(
            nameof(AndroidRipple),
            typeof(bool),
            typeof(TouchEff),
            true,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty AndroidRippleColorProperty = BindableProperty.CreateAttached(
            nameof(AndroidRippleColor),
            typeof(Color),
            typeof(TouchEff),
            Color.Default,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty AndroidRippleRadiusProperty = BindableProperty.CreateAttached(
            nameof(AndroidRippleRadius),
            typeof(int),
            typeof(TouchEff),
            -1,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty UWPTiltProperty = BindableProperty.CreateAttached(
            nameof(UWPTilt),
            typeof(bool),
            typeof(TouchEff),
            true,
            BindingMode.OneWayToSource);

        public static ICommand GetCommand(BindableObject bindable)
            => bindable.GetValue(CommandProperty) as ICommand;

        public static void SetCommand(BindableObject bindable, ICommand value)
            => bindable.SetValue(CommandProperty, value);

        public static object GetCommandParameter(BindableObject bindable)
            => bindable.GetValue(CommandParameterProperty);

        public static void SetCommandParameter(BindableObject bindable, object value)
            => bindable.SetValue(CommandParameterProperty, value);

        public static TouchStatus GetStatus(BindableObject bindable)
            => (TouchStatus)bindable.GetValue(StatusProperty);

        public static void SetStatus(BindableObject bindable, TouchStatus value)
            => bindable.SetValue(StatusProperty, value);

        public static TouchState GetState(BindableObject bindable)
            => (TouchState)bindable.GetValue(StateProperty);

        public static void SetState(BindableObject bindable, TouchState value)
            => bindable.SetValue(StateProperty, value);

        public static HoverStatus GetHoverStatus(BindableObject bindable)
            => (HoverStatus)bindable.GetValue(HoverStatusProperty);

        public static void SetHoverStatus(BindableObject bindable, HoverStatus value)
            => bindable.SetValue(HoverStatusProperty, value);

        public static HoverState GetHoverState(BindableObject bindable)
            => (HoverState)bindable.GetValue(HoverStateProperty);

        public static void SetHoverState(BindableObject bindable, HoverState value)
            => bindable.SetValue(HoverStateProperty, value);

        public static Color GetRegularBackgroundColor(BindableObject bindable)
            => (Color)bindable.GetValue(RegularBackgroundColorProperty);

        public static void SetRegularBackgroundColor(BindableObject bindable, Color value)
            => bindable.SetValue(RegularBackgroundColorProperty, value);

        public static Color GetPressedBackgroundColor(BindableObject bindable)
            => (Color)bindable.GetValue(PressedBackgroundColorProperty);

        public static void SetPressedBackgroundColor(BindableObject bindable, Color value)
            => bindable.SetValue(PressedBackgroundColorProperty, value);

        public static double GetRegularOpacity(BindableObject bindable)
            => (double)bindable.GetValue(RegularOpacityProperty);

        public static void SetRegularOpacity(BindableObject bindable, double value)
            => bindable.SetValue(RegularOpacityProperty, value);

        public static double GetPressedOpacity(BindableObject bindable)
            => (double)bindable.GetValue(PressedOpacityProperty);

        public static void SetPressedOpacity(BindableObject bindable, double value)
            => bindable.SetValue(PressedOpacityProperty, value);

        public static double GetRegularScale(BindableObject bindable)
            => (double)bindable.GetValue(RegularScaleProperty);

        public static void SetRegularScale(BindableObject bindable, double value)
            => bindable.SetValue(RegularScaleProperty, value);

        public static double GetPressedScale(BindableObject bindable)
            => (double)bindable.GetValue(PressedScaleProperty);

        public static void SetPressedScale(BindableObject bindable, double value)
            => bindable.SetValue(PressedScaleProperty, value);

        public static double GetRegularTranslationX(BindableObject bindable)
            => (double)bindable.GetValue(RegularTranslationXProperty);

        public static void SetRegularTranslationX(BindableObject bindable, double value)
            => bindable.SetValue(RegularTranslationXProperty, value);

        public static double GetPressedTranslationX(BindableObject bindable)
            => (double)bindable.GetValue(PressedTranslationXProperty);

        public static void SetPressedTranslationX(BindableObject bindable, double value)
            => bindable.SetValue(PressedTranslationXProperty, value);

        public static double GetRegularTranslationY(BindableObject bindable)
            => (double)bindable.GetValue(RegularTranslationYProperty);

        public static void SetRegularTranslationY(BindableObject bindable, double value)
            => bindable.SetValue(RegularTranslationYProperty, value);

        public static double GetPressedTranslationY(BindableObject bindable)
            => (double)bindable.GetValue(PressedTranslationYProperty);

        public static void SetPressedTranslationY(BindableObject bindable, double value)
            => bindable.SetValue(PressedTranslationYProperty, value);

        public static double GetRegularRotation(BindableObject bindable)
            => (double)bindable.GetValue(RegularRotationProperty);

        public static void SetRegularRotation(BindableObject bindable, double value)
            => bindable.SetValue(RegularRotationProperty, value);

        public static double GetPressedRotation(BindableObject bindable)
            => (double)bindable.GetValue(PressedRotationProperty);

        public static void SetPressedRotation(BindableObject bindable, double value)
            => bindable.SetValue(PressedRotationProperty, value);

        public static double GetRegularRotationX(BindableObject bindable)
            => (double)bindable.GetValue(RegularRotationXProperty);

        public static void SetRegularRotationX(BindableObject bindable, double value)
            => bindable.SetValue(RegularRotationXProperty, value);

        public static double GetPressedRotationX(BindableObject bindable)
            => (double)bindable.GetValue(PressedRotationXProperty);

        public static void SetPressedRotationX(BindableObject bindable, double value)
            => bindable.SetValue(PressedRotationXProperty, value);

        public static double GetRegularRotationY(BindableObject bindable)
            => (double)bindable.GetValue(RegularRotationYProperty);

        public static void SetRegularRotationY(BindableObject bindable, double value)
            => bindable.SetValue(RegularRotationYProperty, value);

        public static double GetPressedRotationY(BindableObject bindable)
            => (double)bindable.GetValue(PressedRotationYProperty);

        public static void SetPressedRotationY(BindableObject bindable, double value)
            => bindable.SetValue(PressedRotationYProperty, value);

        public static int GetRegularAnimationDuration(BindableObject bindable)
            => (int)bindable.GetValue(RegularAnimationDurationProperty);

        public static void SetRegularAnimationDuration(BindableObject bindable, int value)
            => bindable.SetValue(RegularAnimationDurationProperty, value);

        public static int GetPressedAnimationDuration(BindableObject bindable)
            => (int)bindable.GetValue(PressedAnimationDurationProperty);

        public static void SetPressedAnimationDuration(BindableObject bindable, int value)
            => bindable.SetValue(PressedAnimationDurationProperty, value);

        public static Easing GetRegularAnimationEasing(BindableObject bindable)
            => bindable.GetValue(RegularAnimationEasingProperty) as Easing;

        public static void SetRegularAnimationEasing(BindableObject bindable, Easing value)
            => bindable.SetValue(RegularAnimationEasingProperty, value);

        public static Easing GetPressedAnimationEasing(BindableObject bindable)
            => bindable.GetValue(PressedAnimationEasingProperty) as Easing;

        public static void SetPressedAnimationEasing(BindableObject bindable, Easing value)
            => bindable.SetValue(PressedAnimationEasingProperty, value);

        public static int GetRippleCount(BindableObject bindable)
            => (int)bindable.GetValue(RippleCountProperty);

        public static void SetRippleCount(BindableObject bindable, int value)
            => bindable.SetValue(RippleCountProperty, value);

        public static bool? GetIsToggled(BindableObject bindable)
            => (bool?)bindable.GetValue(IsToggledProperty);

        public static void SetIsToggled(BindableObject bindable, bool? value)
            => bindable.SetValue(IsToggledProperty, value);

        public static bool GetNativeAnimation(BindableObject bindable)
            => (bool)bindable.GetValue(NativeAnimationProperty);

        public static void SetNativeAnimation(BindableObject bindable, bool value)
            => bindable.SetValue(NativeAnimationProperty, value);

        public static Color GetNativeAnimationColor(BindableObject bindable)
            => (Color)bindable.GetValue(NativeAnimationColorProperty);

        public static void SetNativeAnimationColor(BindableObject bindable, Color value)
            => bindable.SetValue(NativeAnimationColorProperty, value);

        public static bool GetAndroidRipple(BindableObject bindable)
            => (bool)bindable.GetValue(AndroidRippleProperty);

        public static void SetAndroidRipple(BindableObject bindable, bool value)
            => bindable.SetValue(AndroidRippleProperty, value);

        public static Color GetAndroidRippleColor(BindableObject bindable)
            => (Color)bindable.GetValue(AndroidRippleColorProperty);

        public static void SetAndroidRippleColor(BindableObject bindable, Color value)
            => bindable.SetValue(AndroidRippleColorProperty, value);

        public static int GetAndroidRippleRadius(BindableObject bindable)
            => (int)bindable.GetValue(AndroidRippleRadiusProperty);

        public static void SetAndroidRippleRadius(BindableObject bindable, int value)
            => bindable.SetValue(AndroidRippleRadiusProperty, value);

        public static void SetUWPTilt(BindableObject bindable, bool value)
            => bindable.SetValue(UWPTiltProperty, value);

        public static bool GetUWPTilt(BindableObject bindable)
            => (bool)bindable.GetValue(UWPTiltProperty);

        public ICommand Command => GetCommand(Control);

        public object CommandParameter => GetCommandParameter(Control);

        public TouchStatus Status
        {
            get => GetStatus(Control);
            set => SetStatus(Control, value);
        }

        public TouchState State
        {
            get => GetState(Control);
            set => SetState(Control, value);
        }

        public HoverStatus HoverStatus
        {
            get => GetHoverStatus(Control);
            set => SetHoverStatus(Control, value);
        }

        public HoverState HoverState
        {
            get => GetHoverState(Control);
            set => SetHoverState(Control, value);
        }

        public bool NativeAnimation => GetNativeAnimation(Control);

        public Color NativeAnimationColor => GetNativeAnimationColor(Control);

        public bool AndroidRipple => GetAndroidRipple(Control);

        public Color AndroidRippleColor => GetAndroidRippleColor(Control);

        public int AndroidRippleRadius => GetAndroidRippleRadius(Control);

        public bool UWPTilt => GetUWPTilt(Control);

        public Color RegularBackgroundColor => GetRegularBackgroundColor(Control);

        public Color PressedBackgroundColor => GetPressedBackgroundColor(Control);

        public double RegularOpacity => GetRegularOpacity(Control);

        public double PressedOpacity => GetPressedOpacity(Control);

        public double RegularScale => GetRegularScale(Control);

        public double PressedScale => GetPressedScale(Control);

        public double RegularTranslationX => GetRegularTranslationX(Control);

        public double PressedTranslationX => GetPressedTranslationX(Control);

        public double RegularTranslationY => GetRegularTranslationY(Control);

        public double PressedTranslationY => GetPressedTranslationY(Control);

        public double RegularRotation => GetRegularRotation(Control);

        public double PressedRotation => GetPressedRotation(Control);

        public double RegularRotationX => GetRegularRotationX(Control);

        public double PressedRotationX => GetPressedRotationX(Control);

        public double RegularRotationY => GetRegularRotationY(Control);

        public double PressedRotationY => GetPressedRotationY(Control);

        public int PressedAnimationDuration => GetPressedAnimationDuration(Control);

        public Easing PressedAnimationEasing => GetPressedAnimationEasing(Control);

        public int RegularAnimationDuration => GetRegularAnimationDuration(Control);

        public Easing RegularAnimationEasing => GetRegularAnimationEasing(Control);

        public int RippleCount => GetRippleCount(Control);

        public bool? IsToggled
        {
            get => GetIsToggled(Control);
            set => SetIsToggled(Control, value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsCompletedSet => Completed != null;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public VisualElement Control
        {
            get => _control;
            set
            {
                _visualManager.AbortAnimations(this);
                _control = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void HandleTouch(TouchStatus status)
            => _visualManager.HandleTouch(this, status);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void HandleHover(HoverStatus status)
            => _visualManager.HandleHover(this, status);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseStateChanged()
            => StateChanged?.Invoke(Control, new TouchStateChangedEventArgs(State));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseStatusChanged()
            => StatusChanged?.Invoke(Control, new TouchStatusChangedEventArgs(Status));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseHoverStateChanged()
            => HoverStateChanged?.Invoke(Control, new HoverStateChangedEventArgs(HoverState));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseHoverStatusChanged()
            => HoverStatusChanged?.Invoke(Control, new HoverStatusChangedEventArgs(HoverStatus));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseCompleted()
            => Completed?.Invoke(Control, new TouchCompletedEventArgs(CommandParameter));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseAnimationStarted(TouchState state, int duration)
            => AnimationStarted?.Invoke(Control, new AnimationStartedEventArgs(state, duration));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ForceUpdateState(bool animated = true)
        {
            if (Control == null)
            {
                return;
            }
            _visualManager.ChangeStateAsync(this, State, animated);
        }

        protected override void OnDetached()
        {
            base.OnDetached();
            _visualManager.SetCustomAnimationTask(null);
        }
    }
}
