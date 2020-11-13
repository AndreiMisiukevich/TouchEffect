using System.Windows.Input;
using Xamarin.Forms;
using System.ComponentModel;
using System;
using System.Linq;

namespace TouchEffect
{
    public class TouchEff : RoutingEffect
    {
        public event EventHandler<TouchStatusChangedEventArgs> StatusChanged;

        public event EventHandler<TouchStateChangedEventArgs> StateChanged;

        public event EventHandler<TouchInteractionStatusChangedEventArgs> InteractionStatusChanged;

        public event EventHandler<HoverStatusChangedEventArgs> HoverStatusChanged;

        public event EventHandler<HoverStateChangedEventArgs> HoverStateChanged;

        public event EventHandler<TouchCompletedEventArgs> Completed;

        public static readonly BindableProperty IsAvailableProperty = BindableProperty.CreateAttached(
            nameof(IsAvailable),
            typeof(bool),
            typeof(TouchEff),
            true,
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty ShouldMakeChildrenInputTransparentProperty = BindableProperty.CreateAttached(
            nameof(ShouldMakeChildrenInputTransparent),
            typeof(bool),
            typeof(TouchEff),
            true,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.SetChildrenInputTransparent((bool)newValue);
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty CommandProperty = BindableProperty.CreateAttached(
            nameof(Command),
            typeof(ICommand),
            typeof(TouchEff),
            default(ICommand),
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty LongPressCommandProperty = BindableProperty.CreateAttached(
            nameof(LongPressCommand),
            typeof(ICommand),
            typeof(TouchEff),
            default(ICommand),
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached(
            nameof(CommandParameter),
            typeof(object),
            typeof(TouchEff),
            default(object),
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty LongPressCommandParameterProperty = BindableProperty.CreateAttached(
            nameof(LongPressCommandParameter),
            typeof(object),
            typeof(TouchEff),
            default(object),
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty LongPressDurationProperty = BindableProperty.CreateAttached(
            nameof(LongPressDuration),
            typeof(int),
            typeof(TouchEff),
            1500,
            propertyChanged: TryGenerateEffect);

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
            TouchState.Normal,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty UserInteractionStateProperty = BindableProperty.CreateAttached(
            nameof(UserInteractionState),
            typeof(TouchInteractionStatus),
            typeof(TouchEff),
            TouchInteractionStatus.Completed,
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
            HoverState.Normal,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty NormalBackgroundColorProperty = BindableProperty.CreateAttached(
            nameof(NormalBackgroundColor),
            typeof(Color),
            typeof(TouchEff),
            default(Color),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty HoveredBackgroundColorProperty = BindableProperty.CreateAttached(
            nameof(HoveredBackgroundColor),
            typeof(Color),
            typeof(TouchEff),
            default(Color),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty PressedBackgroundColorProperty = BindableProperty.CreateAttached(
            nameof(PressedBackgroundColor),
            typeof(Color),
            typeof(TouchEff),
            default(Color),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty NormalOpacityProperty = BindableProperty.CreateAttached(
            nameof(NormalOpacity),
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty HoveredOpacityProperty = BindableProperty.CreateAttached(
            nameof(HoveredOpacity),
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty PressedOpacityProperty = BindableProperty.CreateAttached(
            nameof(PressedOpacity),
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty NormalScaleProperty = BindableProperty.CreateAttached(
            nameof(NormalScale),
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty HoveredScaleProperty = BindableProperty.CreateAttached(
            nameof(HoveredScale),
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty PressedScaleProperty = BindableProperty.CreateAttached(
            nameof(PressedScale),
            typeof(double),
            typeof(TouchEff),
            1.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty NormalTranslationXProperty = BindableProperty.CreateAttached(
            nameof(NormalTranslationX),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty HoveredTranslationXProperty = BindableProperty.CreateAttached(
            nameof(HoveredTranslationX),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty PressedTranslationXProperty = BindableProperty.CreateAttached(
            nameof(PressedTranslationX),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty NormalTranslationYProperty = BindableProperty.CreateAttached(
            nameof(NormalTranslationY),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty HoveredTranslationYProperty = BindableProperty.CreateAttached(
            nameof(HoveredTranslationY),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty PressedTranslationYProperty = BindableProperty.CreateAttached(
            nameof(PressedTranslationY),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty NormalRotationProperty = BindableProperty.CreateAttached(
            nameof(NormalRotation),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty HoveredRotationProperty = BindableProperty.CreateAttached(
            nameof(HoveredRotation),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty PressedRotationProperty = BindableProperty.CreateAttached(
            nameof(PressedRotation),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty NormalRotationXProperty = BindableProperty.CreateAttached(
            nameof(NormalRotationX),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty HoveredRotationXProperty = BindableProperty.CreateAttached(
            nameof(HoveredRotationX),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty PressedRotationXProperty = BindableProperty.CreateAttached(
            nameof(PressedRotationX),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty NormalRotationYProperty = BindableProperty.CreateAttached(
            nameof(NormalRotationY),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty HoveredRotationYProperty = BindableProperty.CreateAttached(
            nameof(HoveredRotationY),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty PressedRotationYProperty = BindableProperty.CreateAttached(
            nameof(PressedRotationY),
            typeof(double),
            typeof(TouchEff),
            0.0,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty AnimationDurationProperty = BindableProperty.CreateAttached(
            nameof(AnimationDuration),
            typeof(int),
            typeof(TouchEff),
            default(int),
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty AnimationEasingProperty = BindableProperty.CreateAttached(
            nameof(AnimationEasing),
            typeof(Easing),
            typeof(TouchEff),
            null,
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty PressedAnimationDurationProperty = BindableProperty.CreateAttached(
            nameof(PressedAnimationDuration),
            typeof(int),
            typeof(TouchEff),
            default(int),
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty PressedAnimationEasingProperty = BindableProperty.CreateAttached(
            nameof(PressedAnimationEasing),
            typeof(Easing),
            typeof(TouchEff),
            null,
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty NormalAnimationDurationProperty = BindableProperty.CreateAttached(
            nameof(NormalAnimationDuration),
            typeof(int),
            typeof(TouchEff),
            default(int),
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty NormalAnimationEasingProperty = BindableProperty.CreateAttached(
            nameof(NormalAnimationEasing),
            typeof(Easing),
            typeof(TouchEff),
            null,
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty HoveredAnimationDurationProperty = BindableProperty.CreateAttached(
            nameof(HoveredAnimationDuration),
            typeof(int),
            typeof(TouchEff),
            default(int),
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty HoveredAnimationEasingProperty = BindableProperty.CreateAttached(
            nameof(HoveredAnimationEasing),
            typeof(Easing),
            typeof(TouchEff),
            null,
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty RippleCountProperty = BindableProperty.CreateAttached(
            nameof(RippleCount),
            typeof(int),
            typeof(TouchEff),
            default(int),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
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
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty DisallowTouchThresholdProperty = BindableProperty.CreateAttached(
            nameof(DisallowTouchThreshold),
            typeof(int),
            typeof(TouchEff),
            default(int),
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty NativeAnimationProperty = BindableProperty.CreateAttached(
            nameof(NativeAnimation),
            typeof(bool),
            typeof(TouchEff),
            false,
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty NativeAnimationColorProperty = BindableProperty.CreateAttached(
            nameof(NativeAnimationColor),
            typeof(Color),
            typeof(TouchEff),
            Color.Default,
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty NativeAnimationRadiusProperty = BindableProperty.CreateAttached(
            nameof(NativeAnimationRadius),
            typeof(int),
            typeof(TouchEff),
            -1,
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty NativeAnimationShadowRadiusProperty = BindableProperty.CreateAttached(
            nameof(NativeAnimationShadowRadius),
            typeof(int),
            typeof(TouchEff),
            -1,
            propertyChanged: TryGenerateEffect);

        public static readonly BindableProperty NormalBackgroundImageSourceProperty = BindableProperty.CreateAttached(
            nameof(NormalBackgroundImageSource),
            typeof(ImageSource),
            typeof(TouchEff),
            default(ImageSource),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty HoveredBackgroundImageSourceProperty = BindableProperty.CreateAttached(
            nameof(HoveredBackgroundImageSource),
            typeof(ImageSource),
            typeof(TouchEff),
            default(ImageSource),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty PressedBackgroundImageSourceProperty = BindableProperty.CreateAttached(
            nameof(PressedBackgroundImageSource),
            typeof(ImageSource),
            typeof(TouchEff),
            default(ImageSource),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty NormalBackgroundImageAspectProperty = BindableProperty.CreateAttached(
            nameof(NormalBackgroundImageAspect),
            typeof(Aspect),
            typeof(TouchEff),
            default(Aspect),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty HoveredBackgroundImageAspectProperty = BindableProperty.CreateAttached(
            nameof(HoveredBackgroundImageAspect),
            typeof(Aspect),
            typeof(TouchEff),
            default(Aspect),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty PressedBackgroundImageAspectProperty = BindableProperty.CreateAttached(
            nameof(PressedBackgroundImageAspect),
            typeof(Aspect),
            typeof(TouchEff),
            default(Aspect),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
                TryGenerateEffect(bindable, oldValue, newValue);
            });

        public static readonly BindableProperty ShouldSetImageOnAnimationEndProperty = BindableProperty.CreateAttached(
            nameof(ShouldSetImageOnAnimationEnd),
            typeof(bool),
            typeof(TouchEff),
            default(bool),
            propertyChanged: TryGenerateEffect);

        readonly GestureManager gestureManager = new GestureManager();

        VisualElement control;

        public TouchEff() : base($"{nameof(TouchEffect)}.{nameof(TouchEff)}")
        {
        }

        public static bool GetIsAvailable(BindableObject bindable)
            => (bool)bindable.GetValue(IsAvailableProperty);

        public static void SetIsAvailable(BindableObject bindable, bool value)
            => bindable.SetValue(IsAvailableProperty, value);

        public static bool GetShouldMakeChildrenInputTransparent(BindableObject bindable)
            => (bool)bindable.GetValue(ShouldMakeChildrenInputTransparentProperty);

        public static void SetShouldMakeChildrenInputTransparent(BindableObject bindable, bool value)
            => bindable.SetValue(ShouldMakeChildrenInputTransparentProperty, value);

        public static ICommand GetCommand(BindableObject bindable)
            => bindable.GetValue(CommandProperty) as ICommand;

        public static void SetCommand(BindableObject bindable, ICommand value)
            => bindable.SetValue(CommandProperty, value);

        public static ICommand GetLongPressCommand(BindableObject bindable)
            => bindable.GetValue(LongPressCommandProperty) as ICommand;

        public static void SetLongPressCommand(BindableObject bindable, ICommand value)
            => bindable.SetValue(LongPressCommandProperty, value);

        public static object GetCommandParameter(BindableObject bindable)
            => bindable.GetValue(CommandParameterProperty);

        public static void SetCommandParameter(BindableObject bindable, object value)
            => bindable.SetValue(CommandParameterProperty, value);

        public static object GetLongPressCommandParameter(BindableObject bindable)
            => bindable.GetValue(LongPressCommandParameterProperty);

        public static void SetLongPressCommandParameter(BindableObject bindable, object value)
            => bindable.SetValue(LongPressCommandParameterProperty, value);

        public static int GetLongPressDuration(BindableObject bindable)
            => (int)bindable.GetValue(LongPressDurationProperty);

        public static void SetLongPressDuration(BindableObject bindable, int value)
            => bindable.SetValue(LongPressDurationProperty, value);
        
        public static TouchStatus GetStatus(BindableObject bindable)
            => (TouchStatus)bindable.GetValue(StatusProperty);

        public static void SetStatus(BindableObject bindable, TouchStatus value)
            => bindable.SetValue(StatusProperty, value);

        public static TouchState GetState(BindableObject bindable)
            => (TouchState)bindable.GetValue(StateProperty);

        public static void SetState(BindableObject bindable, TouchState value)
            => bindable.SetValue(StateProperty, value);

        public static TouchInteractionStatus GetUserInteractionState(BindableObject bindable)
            => (TouchInteractionStatus)bindable.GetValue(UserInteractionStateProperty);

        public static void SetUserInteractionState(BindableObject bindable, TouchInteractionStatus value)
            => bindable.SetValue(UserInteractionStateProperty, value);

        public static HoverStatus GetHoverStatus(BindableObject bindable)
            => (HoverStatus)bindable.GetValue(HoverStatusProperty);

        public static void SetHoverStatus(BindableObject bindable, HoverStatus value)
            => bindable.SetValue(HoverStatusProperty, value);

        public static HoverState GetHoverState(BindableObject bindable)
            => (HoverState)bindable.GetValue(HoverStateProperty);

        public static void SetHoverState(BindableObject bindable, HoverState value)
            => bindable.SetValue(HoverStateProperty, value);

        public static Color GetNormalBackgroundColor(BindableObject bindable)
            => (Color)bindable.GetValue(NormalBackgroundColorProperty);

        public static void SetNormalBackgroundColor(BindableObject bindable, Color value)
            => bindable.SetValue(NormalBackgroundColorProperty, value);

        public static Color GetHoveredBackgroundColor(BindableObject bindable)
            => (Color)bindable.GetValue(HoveredBackgroundColorProperty);

        public static void SetHoveredBackgroundColor(BindableObject bindable, Color value)
            => bindable.SetValue(HoveredBackgroundColorProperty, value);

        public static Color GetPressedBackgroundColor(BindableObject bindable)
            => (Color)bindable.GetValue(PressedBackgroundColorProperty);

        public static void SetPressedBackgroundColor(BindableObject bindable, Color value)
            => bindable.SetValue(PressedBackgroundColorProperty, value);

        public static double GetNormalOpacity(BindableObject bindable)
            => (double)bindable.GetValue(NormalOpacityProperty);

        public static void SetNormalOpacity(BindableObject bindable, double value)
            => bindable.SetValue(NormalOpacityProperty, value);

        public static double GetHoveredOpacity(BindableObject bindable)
            => (double)bindable.GetValue(HoveredOpacityProperty);

        public static void SetHoveredOpacity(BindableObject bindable, double value)
            => bindable.SetValue(HoveredOpacityProperty, value);

        public static double GetPressedOpacity(BindableObject bindable)
            => (double)bindable.GetValue(PressedOpacityProperty);

        public static void SetPressedOpacity(BindableObject bindable, double value)
            => bindable.SetValue(PressedOpacityProperty, value);

        public static double GetNormalScale(BindableObject bindable)
            => (double)bindable.GetValue(NormalScaleProperty);

        public static void SetNormalScale(BindableObject bindable, double value)
            => bindable.SetValue(NormalScaleProperty, value);

        public static double GetHoveredScale(BindableObject bindable)
            => (double)bindable.GetValue(HoveredScaleProperty);

        public static void SetHoveredScale(BindableObject bindable, double value)
            => bindable.SetValue(HoveredScaleProperty, value);

        public static double GetPressedScale(BindableObject bindable)
            => (double)bindable.GetValue(PressedScaleProperty);

        public static void SetPressedScale(BindableObject bindable, double value)
            => bindable.SetValue(PressedScaleProperty, value);

        public static double GetNormalTranslationX(BindableObject bindable)
            => (double)bindable.GetValue(NormalTranslationXProperty);

        public static void SetNormalTranslationX(BindableObject bindable, double value)
            => bindable.SetValue(NormalTranslationXProperty, value);

        public static double GetHoveredTranslationX(BindableObject bindable)
            => (double)bindable.GetValue(HoveredTranslationXProperty);

        public static void SetHoveredTranslationX(BindableObject bindable, double value)
            => bindable.SetValue(HoveredTranslationXProperty, value);

        public static double GetPressedTranslationX(BindableObject bindable)
            => (double)bindable.GetValue(PressedTranslationXProperty);

        public static void SetPressedTranslationX(BindableObject bindable, double value)
            => bindable.SetValue(PressedTranslationXProperty, value);

        public static double GetNormalTranslationY(BindableObject bindable)
            => (double)bindable.GetValue(NormalTranslationYProperty);

        public static void SetNormalTranslationY(BindableObject bindable, double value)
            => bindable.SetValue(NormalTranslationYProperty, value);

        public static double GetHoveredTranslationY(BindableObject bindable)
            => (double)bindable.GetValue(HoveredTranslationYProperty);

        public static void SetHoveredTranslationY(BindableObject bindable, double value)
            => bindable.SetValue(HoveredTranslationYProperty, value);

        public static double GetPressedTranslationY(BindableObject bindable)
            => (double)bindable.GetValue(PressedTranslationYProperty);

        public static void SetPressedTranslationY(BindableObject bindable, double value)
            => bindable.SetValue(PressedTranslationYProperty, value);

        public static double GetNormalRotation(BindableObject bindable)
            => (double)bindable.GetValue(NormalRotationProperty);

        public static void SetNormalRotation(BindableObject bindable, double value)
            => bindable.SetValue(NormalRotationProperty, value);

        public static double GetHoveredRotation(BindableObject bindable)
            => (double)bindable.GetValue(HoveredRotationProperty);

        public static void SetHoveredRotation(BindableObject bindable, double value)
            => bindable.SetValue(HoveredRotationProperty, value);

        public static double GetPressedRotation(BindableObject bindable)
            => (double)bindable.GetValue(PressedRotationProperty);

        public static void SetPressedRotation(BindableObject bindable, double value)
            => bindable.SetValue(PressedRotationProperty, value);

        public static double GetNormalRotationX(BindableObject bindable)
            => (double)bindable.GetValue(NormalRotationXProperty);

        public static void SetNormalRotationX(BindableObject bindable, double value)
            => bindable.SetValue(NormalRotationXProperty, value);

        public static double GetHoveredRotationX(BindableObject bindable)
            => (double)bindable.GetValue(HoveredRotationXProperty);

        public static void SetHoveredRotationX(BindableObject bindable, double value)
            => bindable.SetValue(HoveredRotationXProperty, value);

        public static double GetPressedRotationX(BindableObject bindable)
            => (double)bindable.GetValue(PressedRotationXProperty);

        public static void SetPressedRotationX(BindableObject bindable, double value)
            => bindable.SetValue(PressedRotationXProperty, value);

        public static double GetNormalRotationY(BindableObject bindable)
            => (double)bindable.GetValue(NormalRotationYProperty);

        public static void SetNormalRotationY(BindableObject bindable, double value)
            => bindable.SetValue(NormalRotationYProperty, value);

        public static double GetHoveredRotationY(BindableObject bindable)
            => (double)bindable.GetValue(HoveredRotationYProperty);

        public static void SetHoveredRotationY(BindableObject bindable, double value)
            => bindable.SetValue(HoveredRotationYProperty, value);

        public static double GetPressedRotationY(BindableObject bindable)
            => (double)bindable.GetValue(PressedRotationYProperty);

        public static void SetPressedRotationY(BindableObject bindable, double value)
            => bindable.SetValue(PressedRotationYProperty, value);

        public static int GetAnimationDuration(BindableObject bindable)
            => (int)bindable.GetValue(AnimationDurationProperty);

        public static void SetAnimationDuration(BindableObject bindable, int value)
            => bindable.SetValue(AnimationDurationProperty, value);

        public static Easing GetAnimationEasing(BindableObject bindable)
            => bindable.GetValue(AnimationEasingProperty) as Easing;

        public static void SetAnimationEasing(BindableObject bindable, Easing value)
            => bindable.SetValue(AnimationEasingProperty, value);

        public static int GetPressedAnimationDuration(BindableObject bindable)
           => (int)bindable.GetValue(PressedAnimationDurationProperty);

        public static void SetPressedAnimationDuration(BindableObject bindable, int value)
            => bindable.SetValue(PressedAnimationDurationProperty, value);

        public static Easing GetPressedAnimationEasing(BindableObject bindable)
            => bindable.GetValue(PressedAnimationEasingProperty) as Easing;

        public static void SetPressedAnimationEasing(BindableObject bindable, Easing value)
            => bindable.SetValue(PressedAnimationEasingProperty, value);

        public static int GetNormalAnimationDuration(BindableObject bindable)
            => (int)bindable.GetValue(NormalAnimationDurationProperty);

        public static void SetNormalAnimationDuration(BindableObject bindable, int value)
            => bindable.SetValue(NormalAnimationDurationProperty, value);

        public static Easing GetNormalAnimationEasing(BindableObject bindable)
            => bindable.GetValue(NormalAnimationEasingProperty) as Easing;

        public static void SetNormalAnimationEasing(BindableObject bindable, Easing value)
            => bindable.SetValue(NormalAnimationEasingProperty, value);

        public static int GetHoveredAnimationDuration(BindableObject bindable)
            => (int)bindable.GetValue(HoveredAnimationDurationProperty);

        public static void SetHoveredAnimationDuration(BindableObject bindable, int value)
            => bindable.SetValue(HoveredAnimationDurationProperty, value);

        public static Easing GetHoveredAnimationEasing(BindableObject bindable)
            => bindable.GetValue(HoveredAnimationEasingProperty) as Easing;

        public static void SetHoveredAnimationEasing(BindableObject bindable, Easing value)
            => bindable.SetValue(HoveredAnimationEasingProperty, value);

        public static int GetRippleCount(BindableObject bindable)
            => (int)bindable.GetValue(RippleCountProperty);

        public static void SetRippleCount(BindableObject bindable, int value)
            => bindable.SetValue(RippleCountProperty, value);

        public static bool? GetIsToggled(BindableObject bindable)
            => (bool?)bindable.GetValue(IsToggledProperty);

        public static void SetIsToggled(BindableObject bindable, bool? value)
            => bindable.SetValue(IsToggledProperty, value);

        public static int GetDisallowTouchThreshold(BindableObject bindable)
            => (int)bindable.GetValue(DisallowTouchThresholdProperty);

        public static void SetDisallowTouchThreshold(BindableObject bindable, int value)
            => bindable.SetValue(DisallowTouchThresholdProperty, value);

        public static bool GetNativeAnimation(BindableObject bindable)
            => (bool)bindable.GetValue(NativeAnimationProperty);

        public static void SetNativeAnimation(BindableObject bindable, bool value)
            => bindable.SetValue(NativeAnimationProperty, value);

        public static Color GetNativeAnimationColor(BindableObject bindable)
            => (Color)bindable.GetValue(NativeAnimationColorProperty);

        public static void SetNativeAnimationColor(BindableObject bindable, Color value)
            => bindable.SetValue(NativeAnimationColorProperty, value);

        public static int GetNativeAnimationRadius(BindableObject bindable)
            => (int)bindable.GetValue(NativeAnimationRadiusProperty);

        public static void SetNativeAnimationRadius(BindableObject bindable, int value)
            => bindable.SetValue(NativeAnimationRadiusProperty, value);

        public static int GetNativeAnimationShadowRadius(BindableObject bindable)
            => (int)bindable.GetValue(NativeAnimationShadowRadiusProperty);

        public static void SetNativeAnimationShadowRadius(BindableObject bindable, int value)
            => bindable.SetValue(NativeAnimationShadowRadiusProperty, value);

        public static ImageSource GetNormalBackgroundImageSource(BindableObject bindable)
            => (ImageSource)bindable.GetValue(NormalBackgroundImageSourceProperty);

        public static void SetNormalBackgroundImageSource(BindableObject bindable, ImageSource value)
            => bindable.SetValue(NormalBackgroundImageSourceProperty, value);

        public static ImageSource GetHoveredBackgroundImageSource(BindableObject bindable)
            => (ImageSource)bindable.GetValue(HoveredBackgroundImageSourceProperty);

        public static void SetHoveredBackgroundImageSource(BindableObject bindable, ImageSource value)
            => bindable.SetValue(HoveredBackgroundImageSourceProperty, value);

        public static ImageSource GetPressedBackgroundImageSource(BindableObject bindable)
            => (ImageSource)bindable.GetValue(PressedBackgroundImageSourceProperty);

        public static void SetPressedBackgroundImageSource(BindableObject bindable, ImageSource value)
            => bindable.SetValue(PressedBackgroundImageSourceProperty, value);

        public static Aspect GetNormalBackgroundImageAspect(BindableObject bindable)
            => (Aspect)bindable.GetValue(NormalBackgroundImageAspectProperty);

        public static void SetNormalBackgroundImageAspect(BindableObject bindable, Aspect value)
            => bindable.SetValue(NormalBackgroundImageAspectProperty, value);

        public static Aspect GetHoveredBackgroundImageAspect(BindableObject bindable)
            => (Aspect)bindable.GetValue(HoveredBackgroundImageAspectProperty);

        public static void SetHoveredBackgroundImageAspect(BindableObject bindable, Aspect value)
            => bindable.SetValue(HoveredBackgroundImageAspectProperty, value);

        public static Aspect GetPressedBackgroundImageAspect(BindableObject bindable)
            => (Aspect)bindable.GetValue(PressedBackgroundImageAspectProperty);

        public static void SetPressedBackgroundImageAspect(BindableObject bindable, Aspect value)
            => bindable.SetValue(PressedBackgroundImageAspectProperty, value);

        public static bool GetShouldSetImageOnAnimationEnd(BindableObject bindable)
            => (bool)bindable.GetValue(ShouldSetImageOnAnimationEndProperty);

        public static void SetShouldSetImageOnAnimationEnd(BindableObject bindable, bool value)
            => bindable.SetValue(ShouldSetImageOnAnimationEndProperty, value);

        private static void TryGenerateEffect(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is VisualElement view) || view.Effects.OfType<TouchEff>().Any())
            {
                return;
            }
            view.Effects.Add(new TouchEff { IsAutoGenerated = true });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsDisabled { get; set; }

        internal bool IsUsed { get; set; }

        internal bool IsAutoGenerated { get; set; }

        public bool IsAvailable => GetIsAvailable(Control);

        public bool ShouldMakeChildrenInputTransparent => GetShouldMakeChildrenInputTransparent(Control);

        public ICommand Command => GetCommand(Control);

        public ICommand LongPressCommand => GetLongPressCommand(Control);

        public object CommandParameter => GetCommandParameter(Control);

        public object LongPressCommandParameter => GetLongPressCommandParameter(Control);

        public int LongPressDuration => GetLongPressDuration(Control);

        public TouchStatus Status
        {
            get => GetStatus(Control);
            internal set => SetStatus(Control, value);
        }

        public TouchState State
        {
            get => GetState(Control);
            internal set => SetState(Control, value);
        }

        public TouchInteractionStatus UserInteractionState
        {
            get => GetUserInteractionState(Control);
            internal set => SetUserInteractionState(Control, value);
        }

        public HoverStatus HoverStatus
        {
            get => GetHoverStatus(Control);
            internal set => SetHoverStatus(Control, value);
        }

        public HoverState HoverState
        {
            get => GetHoverState(Control);
            internal set => SetHoverState(Control, value);
        }

        public int DisallowTouchThreshold => GetDisallowTouchThreshold(Control);

        public bool NativeAnimation => GetNativeAnimation(Control);

        public Color NativeAnimationColor => GetNativeAnimationColor(Control);

        public int NativeAnimationRadius => GetNativeAnimationRadius(Control);

        public int NativeAnimationShadowRadius => GetNativeAnimationShadowRadius(Control);

        public Color NormalBackgroundColor => GetNormalBackgroundColor(Control);

        public Color HoveredBackgroundColor => GetHoveredBackgroundColor(Control);

        public Color PressedBackgroundColor => GetPressedBackgroundColor(Control);

        public double NormalOpacity => GetNormalOpacity(Control);

        public double HoveredOpacity => GetHoveredOpacity(Control);

        public double PressedOpacity => GetPressedOpacity(Control);

        public double NormalScale => GetNormalScale(Control);

        public double HoveredScale => GetHoveredScale(Control);

        public double PressedScale => GetPressedScale(Control);

        public double NormalTranslationX => GetNormalTranslationX(Control);

        public double HoveredTranslationX => GetHoveredTranslationX(Control);

        public double PressedTranslationX => GetPressedTranslationX(Control);

        public double NormalTranslationY => GetNormalTranslationY(Control);

        public double HoveredTranslationY => GetHoveredTranslationY(Control);

        public double PressedTranslationY => GetPressedTranslationY(Control);

        public double NormalRotation => GetNormalRotation(Control);

        public double HoveredRotation => GetHoveredRotation(Control);

        public double PressedRotation => GetPressedRotation(Control);

        public double NormalRotationX => GetNormalRotationX(Control);

        public double HoveredRotationX => GetHoveredRotationX(Control);

        public double PressedRotationX => GetPressedRotationX(Control);

        public double NormalRotationY => GetNormalRotationY(Control);

        public double HoveredRotationY => GetHoveredRotationY(Control);

        public double PressedRotationY => GetPressedRotationY(Control);

        public int AnimationDuration => GetAnimationDuration(Control);

        public Easing AnimationEasing => GetAnimationEasing(Control);

        public int PressedAnimationDuration => GetPressedAnimationDuration(Control);

        public Easing PressedAnimationEasing => GetPressedAnimationEasing(Control);

        public int NormalAnimationDuration => GetNormalAnimationDuration(Control);

        public Easing NormalAnimationEasing => GetNormalAnimationEasing(Control);

        public int HoveredAnimationDuration => GetHoveredAnimationDuration(Control);

        public Easing HoveredAnimationEasing => GetHoveredAnimationEasing(Control);

        public int RippleCount => GetRippleCount(Control);

        public bool? IsToggled
        {
            get => GetIsToggled(Control);
            internal set => SetIsToggled(Control, value);
        }

        public ImageSource NormalBackgroundImageSource => GetNormalBackgroundImageSource(Control);

        public ImageSource HoveredBackgroundImageSource => GetHoveredBackgroundImageSource(Control);

        public ImageSource PressedBackgroundImageSource => GetPressedBackgroundImageSource(Control);

        public Aspect NormalBackgroundImageAspect => GetNormalBackgroundImageAspect(Control);

        public Aspect HoveredBackgroundImageAspect => GetHoveredBackgroundImageAspect(Control);

        public Aspect PressedBackgroundImageAspect => GetPressedBackgroundImageAspect(Control);

        public bool ShouldSetImageOnAnimationEnd => GetShouldSetImageOnAnimationEnd(Control);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CanExecute
            => IsAvailable &&
            Control.IsEnabled &&
            (Command?.CanExecute(CommandParameter) ?? true);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public VisualElement Control
        {
            get => control;
            set
            {
                if (control != null)
                {
                    IsUsed = false;
                    gestureManager.Reset();
                    SetChildrenInputTransparent(false);
                }
                gestureManager.AbortAnimations(this);
                control = value;
                if (value != null)
                {
                    SetChildrenInputTransparent(ShouldMakeChildrenInputTransparent);
                    if(!IsAutoGenerated)
                    {
                        IsUsed = true;
                        foreach (var effect in value.Effects.OfType<TouchEff>())
                        {
                            effect.IsDisabled = effect != this;
                        }
                    }
                    ForceUpdateState(false);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void HandleTouch(TouchStatus status)
            => gestureManager.HandleTouch(this, status);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void HandleUserInteraction(TouchInteractionStatus userInteractionState)
        {
            if (UserInteractionState != userInteractionState)
            {
                UserInteractionState = userInteractionState;
                RaiseUserInteractionStateChanged();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void HandleHover(HoverStatus status)
            => gestureManager.HandleHover(this, status);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseStateChanged()
        {
            ForceUpdateState();
            HandleLongPress();
            StateChanged?.Invoke(Control, new TouchStateChangedEventArgs(State));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseUserInteractionStateChanged()
            => InteractionStatusChanged?.Invoke(Control, new TouchInteractionStatusChangedEventArgs(UserInteractionState));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseStatusChanged()
            => StatusChanged?.Invoke(Control, new TouchStatusChangedEventArgs(Status));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseHoverStateChanged()
        {
            ForceUpdateState();
            HoverStateChanged?.Invoke(Control, new HoverStateChangedEventArgs(HoverState));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseHoverStatusChanged()
            => HoverStatusChanged?.Invoke(Control, new HoverStatusChangedEventArgs(HoverStatus));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseCompleted()
            => Completed?.Invoke(Control, new TouchCompletedEventArgs(CommandParameter));

        internal void ForceUpdateState(bool animated = true)
        {
            if (Control == null)
            {
                return;
            }
            gestureManager.ChangeStateAsync(this, animated);
        }

        internal void HandleLongPress()
        {
            if (Control == null)
            {
                return;
            }
            gestureManager.HandleLongPress(this);
        }

        private void SetChildrenInputTransparent(bool value)
        {
            if (!(Control is Layout layout))
            {
                return;
            }

            layout.ChildAdded -= OnLayoutChildAdded;

            if (!value)
            {
                return;
            }

            layout.InputTransparent = false;
            foreach (var view in layout.Children)
            {
                OnLayoutChildAdded(layout, new ElementEventArgs(view));
            }

            layout.ChildAdded += OnLayoutChildAdded;
        }

        private void OnLayoutChildAdded(object sender, ElementEventArgs e)
        {
            if (!(e.Element is View view))
            {
                return;
            }
            view.InputTransparent = ShouldMakeChildrenInputTransparent &&
                !(view.GetTouchEff()?.IsAvailable ?? false);
        }
    }
}
