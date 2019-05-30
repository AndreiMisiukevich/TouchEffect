using Windows.UI.Xaml.Input;
using TouchEffect;
using TouchEffect.Enums;
using TouchEffect.Extensions;
using TouchEffect.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;

[assembly: ResolutionGroupName(nameof(TouchEffect))]
[assembly: ExportEffect(typeof(PlatformTouchEff), nameof(TouchEff))]

namespace TouchEffect.UWP
{
    [Preserve(AllMembers = true)]
    public class PlatformTouchEff : PlatformEffect
    {
        public static void Preserve()
        {
        }

        private TouchEff _effect;

        protected override void OnAttached()
        {
            _effect = Element.GetTouchEff();
            _effect.Control = Element as VisualElement;
            _effect.ForceUpdateState(false);

            if (Container != null)
            {
                Container.PointerPressed += OnPointerPressed;
                Container.PointerReleased += OnPointerReleased;
                Container.PointerCanceled += OnPointerCanceled;
            }
        }

        protected override void OnDetached()
        {
            _effect.Control = null;
            _effect = null;
            if (Container != null)
            {
                Container.PointerPressed -= OnPointerPressed;
                Container.PointerReleased -= OnPointerReleased;
                Container.PointerCanceled -= OnPointerCanceled;
            }
        }

        private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Element.GetTouchEff().HandleTouch(TouchStatus.Completed);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Element.GetTouchEff().HandleTouch(TouchStatus.Started);
        }
    }
}