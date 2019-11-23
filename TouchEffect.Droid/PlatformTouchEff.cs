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
using Android.Graphics.Drawables;
using Android.Widget;
using Android.Animation;
using Android.Graphics;
using Color = Android.Graphics.Color;
using Android.Content.Res;

[assembly: ResolutionGroupName(nameof(TouchEffect))]
[assembly: ExportEffect(typeof(PlatformTouchEff), nameof(TouchEff))]
namespace TouchEffect.Android
{
    [Preserve(AllMembers = true)]
    public class PlatformTouchEff : PlatformEffect
    {
        public static void Preserve() { }

        private TouchEff _effect;
        private Color _color;
        private byte _alpha;
        private RippleDrawable _ripple;
        private FrameLayout _viewOverlay;
        private ObjectAnimator _animator;

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

            if(_effect.NativeAnimation && _effect.AndroidRipple)
            {
                if (_effect.AndroidRippleColor == null)
                    _color = _effect.AndroidRippleColor.ToAndroid();
                else
                    _color = _effect.NativeAnimationColor.ToAndroid();

                _viewOverlay = new FrameLayout(Container.Context)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(-1, -1),
                    Clickable = false,
                    Focusable = false,
                };
                Container.LayoutChange += LayoutChange;

                var mask = new ColorDrawable(Color.Transparent);
                _ripple = new RippleDrawable(GetPressedColorSelector(_color), null, mask);
               // _ripple = new RippleDrawable(GetPressedColorSelector(_color), back, null);
                _viewOverlay.Background = _ripple;
                Container.AddView(_viewOverlay);
                _viewOverlay.BringToFront();
            }
        }
        static ColorStateList GetPressedColorSelector(int pressedColor)
        {
            return new ColorStateList(
                new[] { new int[] { } },
                new[] { pressedColor, });
        }

        private void LayoutChange(object sender, AView.LayoutChangeEventArgs e)
        {
            var group = (ViewGroup)sender;
            if (group == null || (Container as IVisualElementRenderer)?.Element == null) return;
            _viewOverlay.Right = group.Width;
            _viewOverlay.Bottom = group.Height;
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
                Container.LayoutChange -= LayoutChange;
            }
            catch (ObjectDisposedException)
            {
                //suppress exception
            }
        }

        private void OnTouch(object sender, AView.TouchEventArgs e)
        {
            e.Handled = true;
            switch (e.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                    Element.GetTouchEff().HandleTouch(TouchStatus.Started);
                    StartRipple(e);
                    break;
                case MotionEventActions.Up:
                    Element.GetTouchEff().HandleTouch(Element.GetTouchEff().Status == TouchStatus.Started ? TouchStatus.Completed : TouchStatus.Canceled);
                    EndRipple();
                    break;
                case MotionEventActions.Cancel:
                    Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                    EndRipple();
                    break;
                case MotionEventActions.Move:
                    var view = sender as AView;
                    var screenPointerCoords = new Xamarin.Forms.Point(view.Left + e.Event.GetX(), view.Top + e.Event.GetY());
                    var viewRect = new Rectangle(view.Left, view.Top, view.Right - view.Left, view.Bottom - view.Top);
                    var status = viewRect.Contains(screenPointerCoords) ? TouchStatus.Started : TouchStatus.Canceled;
                    if (Element.GetTouchEff().Status != status)
                    {
                        Element.GetTouchEff().HandleTouch(status);
                        if(status == TouchStatus.Started)
                            StartRipple(e);
                    }
                    break;
                case MotionEventActions.HoverEnter:
                    Element.GetTouchEff().HandleHover(HoverStatus.Entered);
                    break;
                case MotionEventActions.HoverExit:
                    Element.GetTouchEff().HandleHover(HoverStatus.Exited);
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        private void StartRipple(AView.TouchEventArgs e)
        {
            if(_effect.NativeAnimation && _effect.AndroidRipple)
            {

                _viewOverlay.BringToFront();
                _ripple.SetHotspot(e.Event.GetX(), e.Event.GetY());
                _viewOverlay.Pressed = true;
            }
        }

        void EndRipple()
        {
            if(_viewOverlay != null)
                _viewOverlay.Pressed = false;
        }
    }
}
