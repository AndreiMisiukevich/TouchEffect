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
        private void OnTouch(object sender, AView.TouchEventArgs e)
        {
            Console.WriteLine(e.Event.Action);
            switch (e.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                    {
                        _pressed = true;
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
                        Element.GetTouchEff().HandleTouch(TouchStatus.HoverEnter);
                        break;
                    }
                case MotionEventActions.HoverExit:
                    {
                        Element.GetTouchEff().HandleTouch(TouchStatus.HoverLeave);
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        //TODO Determine if pointer is within the control
                        /*
                        if(pointer in control) 
                        {
                            Console.WriteLine("INRANGE");
                            _inrange = true;
                            if(_leftrange)
                                Element.GetTouchEff().HandleTouch(TouchStatus.Started);
                        }
                        else
                        {
                            _leftrange = true;
                            Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                        }
                        */
                        break;

                    }

            }
            e.Handled = true;
        }
    }
}
