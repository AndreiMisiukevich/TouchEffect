using System.Windows.Input;
using TouchEffect.Delegates;
using TouchEffect.EventArgs;
using Xamarin.Forms;
using TouchEffect.Enums;
using TouchEffect.Extensions;
using System.ComponentModel;
using System;

namespace TouchEffect
{
    public class TouchEff : RoutingEffect, ITouchEff
    {
        private readonly TouchVisualManager _visualManager;

        public TouchEff() : base($"{nameof(TouchEffect)}.{nameof(TouchEff)}")
        {
            _visualManager = new TouchVisualManager();
            StateChanged += (sender, args) => ForceUpdateState();
        }

        internal event EventHandler StateForceUpdated;

        public event TEffectStatusChangedHandler StatusChanged;

        public event TEffectStateChangedHandler StateChanged;

        public event TEffectCompletedHandler Completed;

        public static readonly BindableProperty CommandProperty = BindableProperty.CreateAttached(
            nameof(Command),
            typeof(ICommand),
            typeof(TouchEff),
            default(ICommand));

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached(
            "CommandParameter",
            typeof(object),
            typeof(TouchEff),
            default(object));

        public static readonly BindableProperty StatusProperty = BindableProperty.CreateAttached(
            "Status",
            typeof(TouchStatus),
            typeof(TouchEff),
            TouchStatus.Completed,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty StateProperty = BindableProperty.CreateAttached(
            "State",
            typeof(TouchState),
            typeof(TouchEff),
            TouchState.Regular,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty RegularBackgroundColorProperty = BindableProperty.CreateAttached(
            "RegularBackgroundColor",
            typeof(Color),
            typeof(TouchEff),
            default(Color),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedBackgroundColorProperty = BindableProperty.CreateAttached(
            "PressedBackgroundColor",
            typeof(Color),
            typeof(TouchEff),
            default(Color),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularOpacityProperty = BindableProperty.CreateAttached(
            "RegularOpacity",
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedOpacityProperty = BindableProperty.CreateAttached(
            "PressedOpacity",
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularScaleProperty = BindableProperty.CreateAttached(
            "RegularScale",
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedScaleProperty = BindableProperty.CreateAttached(
            "PressedScale",
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularTranslationXProperty = BindableProperty.CreateAttached(
            "RegularTranslationX",
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedTranslationXProperty = BindableProperty.CreateAttached(
            "PressedTranslationX",
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularTranslationYProperty = BindableProperty.CreateAttached(
            "RegularTranslationY",
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedTranslationYProperty = BindableProperty.CreateAttached(
            "PressedTranslationY",
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularRotationProperty = BindableProperty.CreateAttached(
            "RegularRotation",
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedRotationProperty = BindableProperty.CreateAttached(
            "PressedRotation",
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularRotationXProperty = BindableProperty.CreateAttached(
            "RegularRotationX",
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedRotationXProperty = BindableProperty.CreateAttached(
            "PressedRotationX",
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularRotationYProperty = BindableProperty.CreateAttached(
            "RegularRotationY",
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedRotationYProperty = BindableProperty.CreateAttached(
            "PressedRotationY",
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedAnimationDurationProperty = BindableProperty.CreateAttached(
            "PressedAnimationDuration",
            typeof(int),
            typeof(TouchEff),
            default(int));

        public static readonly BindableProperty PressedAnimationEasingProperty = BindableProperty.CreateAttached(
            "PressedAnimationEasing",
            typeof(Easing),
            typeof(TouchEff),
            null);

        public static readonly BindableProperty RegularAnimationDurationProperty = BindableProperty.CreateAttached(
            "RegularAnimationDuration",
            typeof(int),
            typeof(TouchEff),
            default(int));

        public static readonly BindableProperty RegularAnimationEasingProperty = BindableProperty.CreateAttached(
            "RegularAnimationEasing",
            typeof(Easing),
            typeof(TouchEff),
            null);

        public static readonly BindableProperty RippleCountProperty = BindableProperty.CreateAttached(
            "RippleCount",
            typeof(int),
            typeof(TouchEff),
            default(int),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsCompletedSet => Completed != null;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public VisualElement Control { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void HandleTouch(TouchStatus status)
            => _visualManager.HandleTouch(this, status);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseStateChanged()
            => StateChanged?.Invoke(Control, new TouchStateChangedEventArgs(State));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseStatusChanged()
            => StatusChanged?.Invoke(Control, new TouchStatusChangedEventArgs(Status));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseCompleted()
            => Completed?.Invoke(Control, new TouchCompletedEventArgs(CommandParameter));
            
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ForceUpdateState()
        {
            if(Control == null)
            {
                return;
            }
            _visualManager.ChangeStateAsync(this, new TouchStateChangedEventArgs(State));
            StateForceUpdated?.Invoke(this, System.EventArgs.Empty);
        }
    }
}
