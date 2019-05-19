using Xamarin.Forms.Platform.Android;
using TouchEffect.Android;
using Xamarin.Forms;
using TouchEffect;
using TouchEffect.Extensions;
using TouchEffect.Enums;
using Android.Runtime;
using Android.Views;
using AView = Android.Views.View;

[assembly: ResolutionGroupName(nameof(TouchEffect))]
[assembly: ExportEffect(typeof(PlatformTouchEff), nameof(TouchEff))]
namespace TouchEffect.Android
{
    [Preserve(AllMembers = true)]
    public class PlatformTouchEff : PlatformEffect
    {
        public static void Preserve() { }

        private TouchEff _effect;

        protected override void OnAttached()
        {
            _effect = Element.GetTouchEff();
            _effect.Control = Element as VisualElement;
            _effect.ForceUpdateState();

            if (Container != null)
            {
                Container.Touch += OnTouch;
            }
        }

        protected override void OnDetached()
        {
            _effect.Control = null;
            _effect = null;
            if (Container != null)
            {
                Container.Touch -= OnTouch;
            }
        }

        private void OnTouch(object sender, AView.TouchEventArgs e)
        {
            switch (e.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                    Element.GetTouchEff().HandleTouch(TouchStatus.Started);
                    break;
                case MotionEventActions.Up:
                    Element.GetTouchEff().HandleTouch(TouchStatus.Completed);
                    break;
                case MotionEventActions.Cancel:
                    Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                    break;
            }
            e.Handled = true;
        }
    }
}
