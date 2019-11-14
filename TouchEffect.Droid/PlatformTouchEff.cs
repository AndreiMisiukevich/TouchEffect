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
            catch(ObjectDisposedException)
            {
                //suppress exception
            }
        }

        bool _inrange;
        bool _pressed;
        bool _leftrange = false;
        int[] twoIntArray = new int[2];
        private void OnTouch(object sender, AView.TouchEventArgs e)
        {
            AView senderView = sender as AView;

            switch (e.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                    {
                        _pressed = true;
                        _inrange = true;
                        Element.GetTouchEff().HandleTouch(TouchStatus.Started);
                        break;
                    }

                case MotionEventActions.Up:
                    {
                        _pressed = false;
                        if (_inrange)
                        {
                            Element.GetTouchEff().HandleTouch(TouchStatus.Completed);
                            _inrange = false;
                        }
                        else
                        {
                            Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                        }
                        break;
                    }
                case MotionEventActions.Cancel:
                    {
                        _pressed = false;
                        Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                        break;
                    }

                case MotionEventActions.HoverEnter:
                    {
                        Element.GetTouchEff().HandleHover(HoverStatus.Entered);
                        break;
                    }
                case MotionEventActions.HoverExit:
                    {
                        Element.GetTouchEff().HandleHover(HoverStatus.Exited);
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        senderView.GetLocationOnScreen(twoIntArray);
                        var screenPointerCoords = new Point(twoIntArray[0] + e.Event.GetX(), twoIntArray[1] + e.Event.GetY());
                        Rectangle viewRect = new Rectangle(twoIntArray[0], twoIntArray[1], senderView.Width, senderView.Height);

                        if (viewRect.Contains(screenPointerCoords))
                        {
                            _inrange = true;
                            if (_leftrange)
                                Element.GetTouchEff().HandleTouch(TouchStatus.Started);
                        }
                        else
                        {
                            _leftrange = true;
                            _inrange = false;
                            Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                        }
                        break;
                    }
            }
            e.Handled = true;
        }
    }
}
