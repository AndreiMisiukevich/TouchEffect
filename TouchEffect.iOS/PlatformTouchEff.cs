using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using TouchEffect.iOS;
using Xamarin.Forms;
using TouchEffect;
using TouchEffect.Extensions;
using TouchEffect.Enums;
using CoreGraphics;
using static System.Math;

[assembly: ResolutionGroupName(nameof(TouchEffect))]
[assembly: ExportEffect(typeof(PlatformTouchEff), nameof(TouchEff))]
namespace TouchEffect.iOS
{
    [Preserve(AllMembers = true)]
    public class PlatformTouchEff : PlatformEffect
    {
        public static void Preserve() { }

        private UIGestureRecognizer _gesture;
        private TouchEff _effect;

        protected override void OnAttached()
        {
            _effect = Element.PickTouchEff();
            if (_effect?.IsDisabled ?? true) return;

            _effect.Control = Element as VisualElement;

            _gesture = new TouchUITapGestureRecognizer(_effect);
            if (Container != null)
            {
                Container.AddGestureRecognizer(_gesture);
                Container.UserInteractionEnabled = true;
            }
        }

        protected override void OnDetached()
        {
            if (_effect?.Control == null) return;

            Container?.RemoveGestureRecognizer(_gesture);
            _gesture?.Dispose();
            _gesture = null;
            _effect.Control = null;
            _effect = null;
        }
    }

    internal sealed class TouchUITapGestureRecognizer : UIGestureRecognizer
    {
        private TouchEff _effect;
        private float? _defaultRadius;
        private float? _defaultShadowRadius;
        private float? _defaultShadowOpacity;
        private CGPoint? _startPoint;

        public TouchUITapGestureRecognizer(TouchEff effect)
        {
            _effect = effect;
            CancelsTouchesInView = false;
            Delegate = new TouchUITapGestureRecognizerDelegate();
        }

        public bool IsCanceled { get; set; } = true;

        private UIView Renderer => _effect?.Control.GetRenderer() as UIView;

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            if (_effect?.IsDisabled ?? true) return;

            IsCanceled = false;
            _startPoint = GetTouchPoint(touches);
            HandleTouch(TouchStatus.Started, TouchInteractionStatus.Started);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (_effect?.IsDisabled ?? true) return;

            HandleTouch(_effect?.Status == TouchStatus.Started ? TouchStatus.Completed : TouchStatus.Canceled, TouchInteractionStatus.Completed);
            IsCanceled = true;
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            if (_effect?.IsDisabled ?? true) return;

            HandleTouch(TouchStatus.Canceled, TouchInteractionStatus.Completed);
            IsCanceled = true;
            base.TouchesCancelled(touches, evt);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            if (_effect?.IsDisabled ?? true) return;

            var disallowTouchThreshold = _effect.DisallowTouchThreshold;
            var point = GetTouchPoint(touches);
            if (point != null && _startPoint != null && disallowTouchThreshold > 0)
            {
                var diffX = Abs(point.Value.X - _startPoint.Value.X);
                var diffY = Abs(point.Value.Y - _startPoint.Value.Y);
                var maxDiff = Max(diffX, diffY);
                if (maxDiff > disallowTouchThreshold)
                {
                    HandleTouch(TouchStatus.Canceled, TouchInteractionStatus.Completed);
                    IsCanceled = true;
                    base.TouchesMoved(touches, evt);
                    return;
                }
            }

            var status = point != null && Renderer.Bounds.Contains(point.Value)
                     ? TouchStatus.Started
                     : TouchStatus.Canceled;

            if (_effect?.Status != status)
            {
                HandleTouch(status);
            }

            base.TouchesMoved(touches, evt);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _effect = null;
                Delegate = null;
            }
            base.Dispose(disposing);
        }

        private CGPoint? GetTouchPoint(NSSet touches)
            => Renderer != null ? (touches?.AnyObject as UITouch)?.LocationInView(Renderer) : null;

        public void HandleTouch(TouchStatus status, TouchInteractionStatus? userInteractionState = null)
        {
            if (IsCanceled || _effect == null)
            {
                return;
            }

            if (_effect?.IsDisabled ?? true) return;

            _effect.HandleTouch(status);
            if (userInteractionState.HasValue)
            {
                _effect?.HandleUserInteraction(userInteractionState.Value);
            }

            if (_effect == null || !_effect.NativeAnimation || !_effect.CanExecute)
            {
                return;
            }
            var control = _effect.Control;
            var renderer = control?.GetRenderer() as UIView;
            if (renderer == null) return;

            var color = _effect.NativeAnimationColor;
            var radius = _effect.NativeAnimationRadius;
            var shadowRadius = _effect.NativeAnimationShadowRadius;
            var isStarted = status == TouchStatus.Started;
            _defaultRadius = (float?)(_defaultRadius ?? renderer.Layer.CornerRadius);
            _defaultShadowRadius = (float?)(_defaultShadowRadius ?? renderer.Layer.ShadowRadius);
            _defaultShadowOpacity = _defaultShadowOpacity ?? renderer.Layer.ShadowOpacity;


            UIView.AnimateAsync(.2, () =>
            {
                if (color == Color.Default)
                {
                    renderer.Layer.Opacity = isStarted ? 0.5f : (float)control.Opacity;
                }
                else
                {
                    renderer.Layer.BackgroundColor = (isStarted ? color : control.BackgroundColor).ToCGColor();
                }
                renderer.Layer.CornerRadius = isStarted ? radius : _defaultRadius.GetValueOrDefault();

                if (shadowRadius >= 0)
                {
                    renderer.Layer.ShadowRadius = isStarted ? shadowRadius : _defaultShadowRadius.GetValueOrDefault();
                    renderer.Layer.ShadowOpacity = isStarted ? 0.7f : _defaultShadowOpacity.GetValueOrDefault();
                }
            });
        }
    }

    internal class TouchUITapGestureRecognizerDelegate : UIGestureRecognizerDelegate
    {
        public override bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer,
            UIGestureRecognizer otherGestureRecognizer)
        {
            if (gestureRecognizer is TouchUITapGestureRecognizer touchGesture && otherGestureRecognizer is UIPanGestureRecognizer &&
                otherGestureRecognizer.State == UIGestureRecognizerState.Began)
            {
                touchGesture.HandleTouch(TouchStatus.Canceled, TouchInteractionStatus.Completed);
                touchGesture.IsCanceled = true;
            }

            return true;
        }
    }
}
