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

        [EditorBrowsable(EditorBrowsableState.Never)]
        Color HoveredBackgroundColor { get; }

        Color PressedBackgroundColor { get; }

        double RegularOpacity { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double HoveredOpacity { get; }

        double PressedOpacity { get; }

        double RegularScale { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double HoveredScale { get; }

        double PressedScale { get; }

        double RegularTranslationX { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double HoveredTranslationX { get; }

        double PressedTranslationX { get; }

        double RegularTranslationY { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double HoveredTranslationY { get; }

        double PressedTranslationY { get; }

        double RegularRotation { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double HoveredRotation { get; }

        double PressedRotation { get; }

        double RegularRotationX { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double HoveredRotationX { get; }

        double PressedRotationX { get; }

        double RegularRotationY { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double HoveredRotationY { get; }

        double PressedRotationY { get; }

        int RegularAnimationDuration { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int HoveredAnimationDuration { get; }

        int PressedAnimationDuration { get; }

        Easing RegularAnimationEasing { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        Easing HoveredAnimationEasing { get; }

        Easing PressedAnimationEasing { get; }

        int RippleCount { get; }

        bool? IsToggled { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool IsCompletedSet { get; }

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
