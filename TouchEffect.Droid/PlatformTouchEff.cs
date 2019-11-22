using Xamarin.Forms.Platform.Android;
using TouchEffect.Android;
using Xamarin.Forms;
using TouchEffect;
using TouchEffect.Extensions;
using TouchEffect.Enums;
using Android.Runtime;
using Android.Views;
using AView = Android.Views.View;
using System;

[assembly: ResolutionGroupName(nameof(TouchEffect))]
[assembly: ExportEffect(typeof(PlatformTouchEff), nameof(TouchEff))]
namespace TouchEffect.Android
{
    [Preserve(AllMembers = true)]
    public class PlatformTouchEff : PlatformEffect
    {
        public static void Preserve() { }

        private TouchEff _effect;
        private bool _isHoverSupported;

        protected override void OnAttached()
        {
            _effect = Element.GetTouchEff();
            _effect.Control = Element as VisualElement;
            _effect.ForceUpdateState(false);

            if (Container != null)
            {
                Container.Touch += OnTouch;
            }
            else if (Control != null)
            {
                Control.Touch += OnTouch;
            }
        }

        protected override void OnDetached()
        {
            try
            {
                _effect.Control = null;
                _effect = null;
                if (Container != null)
                {
                    Container.Touch -= OnTouch;
                }
                if (Control != null)
                {
                    Control.Touch -= OnTouch;
                }
            }
            catch (ObjectDisposedException)
            {
                //suppress exception
            }
            _isHoverSupported = false;
        }

        private void OnTouch(object sender, AView.TouchEventArgs e)
        {
            e.Handled = true;
            switch (e.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                    Element.GetTouchEff().HandleTouch(TouchStatus.Started);
                    break;
                case MotionEventActions.Up:
                    Element.GetTouchEff().HandleTouch(Element.GetTouchEff().Status == TouchStatus.Started ? TouchStatus.Completed : TouchStatus.Canceled);
                    break;
                case MotionEventActions.Cancel:
                    Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                    break;
                case MotionEventActions.Move:
                    var view = sender as AView;
                    var screenPointerCoords = new Point(view.Left + e.Event.GetX(), view.Top + e.Event.GetY());
                    var viewRect = new Rectangle(view.Left, view.Top, view.Right - view.Left, view.Bottom - view.Top);
                    var status = viewRect.Contains(screenPointerCoords) ? TouchStatus.Started : TouchStatus.Canceled;

                    if (_isHoverSupported && ((status == TouchStatus.Canceled && _effect.HoverStatus == HoverStatus.Entered) ||
                        (status == TouchStatus.Started && _effect.HoverStatus == HoverStatus.Exited)))
                    {
                        _effect.HandleHover(status == TouchStatus.Started ? HoverStatus.Entered : HoverStatus.Exited);
                    }

                    if (Element.GetTouchEff().Status != status)
                    {
                        Element.GetTouchEff().HandleTouch(status);
                    }
                    break;
                case MotionEventActions.HoverEnter:
                    _isHoverSupported = true;
                    Element.GetTouchEff().HandleHover(HoverStatus.Entered);
                    break;
                case MotionEventActions.HoverExit:
                    _isHoverSupported = true;
                    Element.GetTouchEff().HandleHover(HoverStatus.Exited);
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }
    }
}
