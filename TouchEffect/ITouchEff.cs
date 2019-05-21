using System.Windows.Input;
using TouchEffect.Enums;
using Xamarin.Forms;

namespace TouchEffect
{
    internal interface ITouchEff
    {
        ICommand Command { get; }

        object CommandParameter { get; }

        TouchStatus Status { get; set; }

        TouchState State { get; set; }

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

        VisualElement Control { get; set; }

        void HandleTouch(TouchStatus status);

        void RaiseStateChanged();

        void RaiseStatusChanged();

        void RaiseCompleted();
    }
}
