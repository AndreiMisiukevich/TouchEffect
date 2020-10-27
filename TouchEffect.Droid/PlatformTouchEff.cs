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
using static System.Math;
using Android.Views.Accessibility;
using Android.Content;
using AndroidOS = Android.OS;

[assembly: ResolutionGroupName(nameof(TouchEffect))]
[assembly: ExportEffect(typeof(PlatformTouchEff), nameof(TouchEff))]
namespace TouchEffect.Android
{
    [Preserve(AllMembers = true)]
    public class PlatformTouchEff : PlatformEffect
    {
        public static void Preserve() { }

        private AccessibilityManager _accessibilityManager;
        private TouchEff _effect;
        private bool _isHoverSupported;
        private RippleDrawable _ripple;
        private FrameLayout _viewOverlay;
        private AView View => Control ?? Container;
        private ViewGroup Group => Container ?? Control as ViewGroup;
        private float _startX;
        private float _startY;
        private Xamarin.Forms.Color _rippleColor;
        private int _rippleRadius = -1;

        internal bool IsCanceled { get; set; }

        private bool IsAccessibilityMode =>
            _accessibilityManager != null &&
            _accessibilityManager.IsEnabled &&
            _accessibilityManager.IsTouchExplorationEnabled;

        protected override void OnAttached()
        {
            if (View == null)
            {
                return;
            }
            _effect = Element.PickTouchEff();
            if (_effect?.IsDisabled ?? true) return;

            _effect.Control = Element as VisualElement;

            _accessibilityManager = View.Context.GetSystemService(Context.AccessibilityService) as AccessibilityManager;

            View.Touch += OnTouch;
            View.Click += OnClick;

            if (_effect.NativeAnimation && Group != null && AndroidOS.Build.VERSION.SdkInt >= AndroidOS.BuildVersionCodes.Lollipop)
            {
                View.Clickable = true;
                View.LongClickable = true;
                _viewOverlay = new FrameLayout(Group.Context)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(-1, -1),
                    Clickable = false,
                    Focusable = false,
                };
                View.LayoutChange += LayoutChange;

                CreateRipple();
                _viewOverlay.Background = _ripple;
                Group.AddView(_viewOverlay);
                _viewOverlay.BringToFront();
            }
        }

        protected override void OnDetached()
        {
            if (_effect?.Control == null) return;
            try
            {
                if (View != null)
                {
                    View.LayoutChange -= LayoutChange;
                    View.Touch -= OnTouch;
                    View.Click -= OnClick;
                }

                _accessibilityManager = null;
                _effect.Control = null;
                _effect = null;

                if (_viewOverlay != null)
                {
                    if (Group != null)
                    {
                        Group.RemoveView(_viewOverlay);
                    }

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
            e.Handled = false;

            if (_effect?.IsDisabled ?? true) return;

            if (IsAccessibilityMode)
            {
                return;
            }

            switch (e.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                    IsCanceled = false;
                    _startX = e.Event.GetX();
                    _startY = e.Event.GetY();
                    _effect?.HandleUserInteraction(UserInteractionState.Running);
                    _effect?.HandleTouch(TouchStatus.Started);
                    StartRipple(e.Event.GetX(), e.Event.GetY());
                    if (_effect.DisallowTouchThreshold > 0)
                    {
                        Group.Parent?.RequestDisallowInterceptTouchEvent(true);
                    }
                    break;
                case MotionEventActions.Up:
                    HandleEnd(_effect.Status == TouchStatus.Started ? TouchStatus.Completed : TouchStatus.Canceled);
                    break;
                case MotionEventActions.Cancel:
                    HandleEnd(TouchStatus.Canceled);
                    break;
                case MotionEventActions.Move:
                    if (IsCanceled)
                    {
                        return;
                    }
                    var diffX = Abs(e.Event.GetX() - _startX) / View.Context.Resources.DisplayMetrics.Density;
                    var diffY = Abs(e.Event.GetY() - _startY) / View.Context.Resources.DisplayMetrics.Density;
                    var maxDiff = Max(diffX, diffY);
                    var disallowTouchThreshold = _effect.DisallowTouchThreshold;
                    if (disallowTouchThreshold > 0 && maxDiff > disallowTouchThreshold)
                    {
                        HandleEnd(TouchStatus.Canceled);
                        return;
                    }
                    var view = sender as AView;
                    var screenPointerCoords = new Xamarin.Forms.Point(view.Left + e.Event.GetX(), view.Top + e.Event.GetY());
                    var viewRect = new Rectangle(view.Left, view.Top, view.Right - view.Left, view.Bottom - view.Top);
                    var status = viewRect.Contains(screenPointerCoords) ? TouchStatus.Started : TouchStatus.Canceled;

                    if (_isHoverSupported && ((status == TouchStatus.Canceled && _effect.HoverStatus == HoverStatus.Entered) ||
                        (status == TouchStatus.Started && _effect.HoverStatus == HoverStatus.Exited)))
                    {
                        _effect?.HandleHover(status == TouchStatus.Started ? HoverStatus.Entered : HoverStatus.Exited);
                    }

                    if (_effect.Status != status)
                    {
                        _effect?.HandleTouch(status);
                        if (status == TouchStatus.Started)
                            StartRipple(e.Event.GetX(), e.Event.GetY());
                        if (status == TouchStatus.Canceled)
                            EndRipple();
                    }
                    break;
                case MotionEventActions.HoverEnter:
                    _isHoverSupported = true;
                    _effect?.HandleHover(HoverStatus.Entered);
                    break;
                case MotionEventActions.HoverExit:
                    _isHoverSupported = true;
                    _effect?.HandleHover(HoverStatus.Exited);
                    break;
            }
        }

        private void OnClick(object sender, System.EventArgs args)
        {
            if (_effect?.IsDisabled ?? true) return;

            if (!IsAccessibilityMode)
            {
                return;
            }
            IsCanceled = false;
            HandleEnd(TouchStatus.Completed);
        }

        private void HandleEnd(TouchStatus status)
        {
            if (IsCanceled)
            {
                return;
            }
            IsCanceled = true;
            if (_effect.DisallowTouchThreshold > 0)
            {
                Group.Parent?.RequestDisallowInterceptTouchEvent(false);
            }
            _effect?.HandleTouch(status);
            _effect?.HandleUserInteraction(UserInteractionState.Idle);
            EndRipple();
        }

        private void StartRipple(float x, float y)
        {
            if (_effect?.IsDisabled ?? true) return;

            if (_effect.CanExecute && _effect.NativeAnimation && _viewOverlay?.Background is RippleDrawable)
            {
                UpdateRipple();
                _viewOverlay.BringToFront();
                _ripple.SetHotspot(x, y);
                _viewOverlay.Pressed = true;
            }
        }

        private void EndRipple()
        {
            if (_effect?.IsDisabled ?? true) return;

            if (_viewOverlay?.Pressed ?? false)
            {
                _viewOverlay.Pressed = false;
            }
        }

        private void CreateRipple()
        {
            var background = View?.Background;

            if (background is RippleDrawable)
            {
                _ripple = (RippleDrawable)background.GetConstantState().NewDrawable();
                return;
            }
            else
            {
                var noBackground = Element is Layout || background == null;
                _ripple = new RippleDrawable(GetColorStateList(), noBackground ? null : background, noBackground ? new ColorDrawable(Color.White) : null);
            }
            UpdateRipple();
        }

        private void UpdateRipple()
        {
            if (_effect?.IsDisabled ?? true) return;

            if (_effect.NativeAnimationColor == _rippleColor && _effect.NativeAnimationRadius == _rippleRadius)
            {
                return;
            }
            _rippleColor = _effect.NativeAnimationColor;
            _rippleRadius = _effect.NativeAnimationRadius;
            _ripple.SetColor(GetColorStateList());
            if (AndroidOS.Build.VERSION.SdkInt >= AndroidOS.BuildVersionCodes.M)
            {
                _ripple.Radius = (int)(View.Context.Resources.DisplayMetrics.Density * _effect.NativeAnimationRadius);
            }
        }

        private ColorStateList GetColorStateList()
        {
            int color;
            var defaultcolor = _effect.NativeAnimationColor;
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
            if (group == null || (Group as IVisualElementRenderer)?.Element == null) return;
            _viewOverlay.Right = group.Width;
            _viewOverlay.Bottom = group.Height;
        }
    }
}