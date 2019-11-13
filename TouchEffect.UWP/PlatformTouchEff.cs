using Windows.UI.Xaml.Input;
using TouchEffect;
using TouchEffect.Enums;
using TouchEffect.Extensions;
using TouchEffect.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;
using System.Diagnostics;

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

        private bool _pressed;
        private bool _inrange;

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
                Container.PointerExited += Container_PointerExited;
                Container.PointerEntered += Container_PointerEntered;
            }
        }

        private void Container_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _inrange = true;
            Element.GetTouchEff().HandleHover(HoverStatus.Entered);
            if (_pressed)
                Element.GetTouchEff().HandleTouch(TouchStatus.Started);
        }

        private void Container_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _inrange = false;
            if (_pressed)
                Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
            Element.GetTouchEff().HandleHover(HoverStatus.Exited);
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
                Container.PointerExited -= Container_PointerExited;
                Container.PointerEntered -= Container_PointerEntered;

                _pressed = false;
                _inrange = false;
            }
        }

        private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            _pressed = false;

            Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _pressed = false;
            if (_pressed && _inrange)
            {
                Element.GetTouchEff().HandleTouch(TouchStatus.Completed);
            }
            else
            {
                Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Container.CapturePointer(e.Pointer);
            _pressed = true;

            Element.GetTouchEff().HandleTouch(TouchStatus.Started);
        }
    }
}