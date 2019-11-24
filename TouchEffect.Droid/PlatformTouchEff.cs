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
        private bool _isHoverSupported;
        private RippleDrawable _ripple;
        private FrameLayout _viewOverlay;
        public AView _view => Control ?? Container;

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
                _view.Clickable = true;
                _view.LongClickable = true;
                _viewOverlay = new FrameLayout(Container.Context)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(-1, -1),
                    Clickable = false,
                    Focusable = false,
                };
                Container.LayoutChange += LayoutChange;

                _ripple = CreateRipple();
                _ripple.Radius = _effect.AndroidRippleRadius;
                _viewOverlay.Background = _ripple;

                Container.AddView(_viewOverlay);
                _viewOverlay.BringToFront();
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
                Container.LayoutChange -= LayoutChange;
                if (_viewOverlay != null)
                {
                    Container.RemoveView(_viewOverlay);
                    _viewOverlay.Pressed = false;
                    _viewOverlay.Foreground = null;
                    _viewOverlay.Dispose();
                    _ripple?.Dispose();
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
                    StartRipple(e.Event.GetX(), e.Event.GetY());
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

                    if (_isHoverSupported && ((status == TouchStatus.Canceled && _effect.HoverStatus == HoverStatus.Entered) ||
                        (status == TouchStatus.Started && _effect.HoverStatus == HoverStatus.Exited)))
                    {
                        _effect.HandleHover(status == TouchStatus.Started ? HoverStatus.Entered : HoverStatus.Exited);
                    }

                    if (Element.GetTouchEff().Status != status)
                    {
                        Element.GetTouchEff().HandleTouch(status);
                        if(status == TouchStatus.Started)
                            StartRipple(e.Event.GetX(), e.Event.GetY());
                        if (status == TouchStatus.Canceled)
                            EndRipple();
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

        public bool StartRipple(float x, float y)
        {
            if (_effect.NativeAnimation && _effect.AndroidRipple && _viewOverlay.Background is RippleDrawable)
            {
                _viewOverlay.BringToFront();
                _ripple.SetHotspot(x, y);
                _viewOverlay.Pressed = true;
                return true;
            }
            return false;
        }

        public bool EndRipple()
        {
            if (_viewOverlay != null && _viewOverlay.Pressed)
            {
                _viewOverlay.Pressed = false;
                return true;
            }
            return false;
        }

        private RippleDrawable CreateRipple()
        {
            if (Element is Layout)
            {
                var mask = new ColorDrawable(Color.White);
                return new RippleDrawable(GetColorStateList(), null, mask);
            }

            var background = (Control ?? Container).Background;
            if (background == null)
            {
                var mask = new ColorDrawable(Color.White);
                return new RippleDrawable(GetColorStateList(), null, mask);
            }

            if (background is RippleDrawable)
            {
                var ripple = (RippleDrawable)background.GetConstantState().NewDrawable();
                ripple.SetColor(GetColorStateList());
                return ripple;
            }
            return new RippleDrawable(GetColorStateList(), background, null);
        }

        private ColorStateList GetColorStateList()
        {
            int color;

            var defaultcolor = TouchEff.GetNativeAnimationColor(Element);
            var androidcolor = TouchEff.GetAndroidRippleColor(Element);

            if (androidcolor != Xamarin.Forms.Color.Default && androidcolor != null)
                color = androidcolor.ToAndroid();
            else if (defaultcolor != Xamarin.Forms.Color.Default)
                color = defaultcolor.ToAndroid();
            else
                color = Color.Argb(31, 0, 0, 0);


            return new ColorStateList(
                new[] { new int[] { } },
                new[] { color, });
        }

        private void LayoutChange(object sender, AView.LayoutChangeEventArgs e)
        {
            var group = (ViewGroup)sender;
            if (group == null || (Container as IVisualElementRenderer)?.Element == null) return;
            _viewOverlay.Right = group.Width;
            _viewOverlay.Bottom = group.Height;
        }
    }
}
