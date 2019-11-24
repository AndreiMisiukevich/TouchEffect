using System.ComponentModel;
using System.Windows.Input;
using TouchEffect.Enums;
using Xamarin.Forms;

namespace TouchEffect.Interfaces
{
    public interface ITouchEff
    {
        ICommand Command { get; }

        object CommandParameter { get; }

        TouchStatus Status { get; set; }

        TouchState State { get; set; }

        HoverStatus HoverStatus { get; set; }

        HoverState HoverState { get; set; }

        Color RegularBackgroundColor { get; }

        Color PressedBackgroundColor { get; }

        double RegularOpacity { get; }

        double PressedOpacity { get; }

        double RegularScale { get; }

        double PressedScale { get; }

        double RegularTranslationX { get; }

        double PressedTranslationX { get; }

        double RegularTranslationY { get; }

        double PressedTranslationY { get; }

        double RegularRotation { get; }

        double PressedRotation { get; }

        double RegularRotationX { get; }

        double PressedRotationX { get; }

        double RegularRotationY { get; }

        double PressedRotationY { get; }

        int PressedAnimationDuration { get; }

        Easing PressedAnimationEasing { get; }

        int RegularAnimationDuration { get; }

        Easing RegularAnimationEasing { get; }

        int RippleCount { get; }

        bool? IsToggled { get; set; }

        bool IsCompletedSet { get; }

        bool NativeAnimation { get; }

        Color NativeAnimationColor { get; }
        
        bool AndroidRipple { get; }
        
        Color AndroidRippleColor { get; }

        int AndroidRippleRadius { get; }

        bool UWPTilt { get; }

        VisualElement Control { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void HandleTouch(TouchStatus status);

        [EditorBrowsable(EditorBrowsableState.Never)]
        void RaiseStateChanged();

        [EditorBrowsable(EditorBrowsableState.Never)]
        void RaiseHoverStateChanged();

        [EditorBrowsable(EditorBrowsableState.Never)]
        void RaiseStatusChanged();

        [EditorBrowsable(EditorBrowsableState.Never)]
        void RaiseHoverStatusChanged();

        [EditorBrowsable(EditorBrowsableState.Never)]
        void RaiseCompleted();

        [EditorBrowsable(EditorBrowsableState.Never)]
        void RaiseAnimationStarted(TouchState state, int duration);

        [EditorBrowsable(EditorBrowsableState.Never)]
        void ForceUpdateState(bool animated = true);
    }
}
