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

        private bool _pressed;
        private bool _intentionalCaptureLoss;

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
                Container.PointerExited += OnPointerExited;
                Container.PointerEntered += OnPointerEntered;
                Container.PointerCaptureLost += OnPointerCaptureLost;
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
                Container.PointerExited -= OnPointerExited;
                Container.PointerEntered -= OnPointerEntered;
                Container.PointerCaptureLost -= OnPointerCaptureLost;

                _pressed = false;
            }
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
			_effect.HandleHover(HoverStatus.Entered);
			if (_pressed)
            {
				_effect.HandleTouch(TouchStatus.Started);
            }
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
			_effect.HandleHover(HoverStatus.Exited);
			if (_pressed)
            {
				_effect.HandleTouch(TouchStatus.Canceled);
            }
        }

        private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            _pressed = false;
			_effect.HandleHover(HoverStatus.Exited);
			_effect.HandleTouch(TouchStatus.Canceled);
        }

        private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (_intentionalCaptureLoss) return;
            _pressed = false;

			if (_effect.HoverStatus != HoverStatus.Exited)
			{
				_effect.HandleHover(HoverStatus.Exited);
			}

			if (_effect.Status != TouchStatus.Canceled)
			{
				_effect.HandleTouch(TouchStatus.Canceled);
			}
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {

            if(_pressed && (_effect.HoverStatus == HoverStatus.Entered))
				_effect.HandleTouch(TouchStatus.Completed);
            else if(_effect.HoverStatus != HoverStatus.Exited)
				_effect.HandleTouch(TouchStatus.Canceled);
            _pressed = false;
            _intentionalCaptureLoss = true;
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pressed = true;
            Container.CapturePointer(e.Pointer);
			_effect.HandleTouch(TouchStatus.Started);
            _intentionalCaptureLoss = false;
        }
    }
}
