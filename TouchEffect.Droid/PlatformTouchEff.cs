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
            catch (ObjectDisposedException)
            {
                //suppress exception
            }
        }

        bool inRange;
        private int[] _viewLocation = new int[2];
        private void OnTouch(object sender, AView.TouchEventArgs e)
        {
            var senderView = sender as AView;

            switch (e.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                    inRange = true;
                    Element.GetTouchEff().HandleTouch(TouchStatus.Started);
                    break;

                case MotionEventActions.Up:
                    if (inRange)
                    {
                        Element.GetTouchEff().HandleTouch(TouchStatus.Completed);
                        inRange = false;
                    }
                    else
                    {
                        Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                    }
                    break;
                case MotionEventActions.Cancel:
                    Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                    break;

                case MotionEventActions.HoverEnter:
                    Element.GetTouchEff().HandleHover(HoverStatus.Entered);
                    break;
                case MotionEventActions.HoverExit:
                    Element.GetTouchEff().HandleHover(HoverStatus.Exited);
                    break;
                case MotionEventActions.Move:
                    senderView.GetLocationOnScreen(_viewLocation);
                    var screenPointerCoords = new Point(_viewLocation[0] + e.Event.GetX(), _viewLocation[1] + e.Event.GetY());
                    Rectangle viewRect = new Rectangle(_viewLocation[0], _viewLocation[1], senderView.Width, senderView.Height);

                    if (viewRect.Contains(screenPointerCoords))
                    {
                        if (!inRange)
                            Element.GetTouchEff().HandleTouch(TouchStatus.Started);
                        inRange = true;

                    }
                    else
                    {
                        inRange = false;
                        Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                    }
                    break;
            }
            e.Handled = true;
        }
    }
}
