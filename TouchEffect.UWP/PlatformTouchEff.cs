﻿using Windows.UI.Xaml.Input;
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
		private bool _isHoverSupported;

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

				_pressed = false;
			}
		}

		private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
			_isHoverSupported = true;
			if (_pressed)
			{
				Element.GetTouchEff().HandleTouch(TouchStatus.Started);
			}
			Element.GetTouchEff().HandleHover(HoverStatus.Entered);
		}

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
			_isHoverSupported = true;
			if (_pressed)
			{
				Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
			}
			Element.GetTouchEff().HandleHover(HoverStatus.Exited);
		}

        private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            _pressed = false;
            Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
			_pressed = false;
			Element.GetTouchEff().HandleTouch(_pressed && (Element.GetTouchEff().HoverStatus == HoverStatus.Entered || !_isHoverSupported) ? TouchStatus.Completed : TouchStatus.Canceled);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
			_pressed = true;
			Container.CapturePointer(e.Pointer);
            Element.GetTouchEff().HandleTouch(TouchStatus.Started);
        }
    }
}
