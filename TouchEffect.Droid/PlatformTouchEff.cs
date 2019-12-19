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
using Color = Android.Graphics.Color;
using Android.Content.Res;
using Android.Content;
using static System.Math;

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
        private AView View => Control ?? Container;
        private float _startX;
        private float _startY;
        private bool _canceled;

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

            if(_effect.NativeAnimation)
            {
                View.Clickable = true;
                View.LongClickable = true;
                _viewOverlay = new FrameLayout(Container.Context)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(-1, -1),
                    Clickable = false,
                    Focusable = false,
                };
                View.LayoutChange += LayoutChange;

                _ripple = CreateRipple();
                _ripple.Radius = (int)(View.Context.Resources.DisplayMetrics.Density * _effect.NativeAnimationRadius);
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
                Container.LayoutChange -= LayoutChange;
                if (Container != null)
                {
                    Container.Touch -= OnTouch;
                }
                if (Control != null)
                {
                    Control.Touch -= OnTouch;
                }
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
                    _canceled = false;
                    _startX = e.Event.GetX();
                    _startY = e.Event.GetY();
                    Element.GetTouchEff().HandleTouch(TouchStatus.Started);
                    StartRipple(e.Event.GetX(), e.Event.GetY());
                    if (_effect.DisallowTouchThreshold > 0)
                    {
                        Container.Parent?.RequestDisallowInterceptTouchEvent(true);
                    }
                    break;
                case MotionEventActions.Up:
                    HandleEnd(Element.GetTouchEff().Status == TouchStatus.Started ? TouchStatus.Completed : TouchStatus.Canceled);
                    break;
                case MotionEventActions.Cancel:
                    HandleEnd(TouchStatus.Canceled);
                    break;
                case MotionEventActions.Move:
                    if(_canceled)
                    {
                        return;
                    }
                    var diffX = Abs(e.Event.GetX() - _startX) / Container.Context.Resources.DisplayMetrics.Density;
                    var diffY = Abs(e.Event.GetY() - _startY) / Container.Context.Resources.DisplayMetrics.Density;
                    var maxDiff = Max(diffX, diffY);
                    var disallowTouchThreshold = _effect.DisallowTouchThreshold;
                    if (disallowTouchThreshold > 0 && maxDiff > disallowTouchThreshold)
                    {
                        HandleEnd(TouchStatus.Canceled);
                        _canceled = true;
                        return;
                    }
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

        private void HandleEnd(TouchStatus status)
        {
            if(_canceled)
            {
                return;
            }
            if (_effect.DisallowTouchThreshold > 0)
            {
                Container.Parent?.RequestDisallowInterceptTouchEvent(false);
            }
            Element.GetTouchEff().HandleTouch(status);
            EndRipple();
        }

        private bool StartRipple(float x, float y)
        {
            if (_effect.NativeAnimation && _viewOverlay.Background is RippleDrawable)
            {
                _viewOverlay.BringToFront();
                _ripple.SetHotspot(x, y);
                _viewOverlay.Pressed = true;
                return true;
            }
            return false;
        }

        private bool EndRipple()
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
            if (defaultcolor != Xamarin.Forms.Color.Default)
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
