using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using TouchEffect.iOS;
using Xamarin.Forms;
using TouchEffect;
using TouchEffect.Extensions;
using TouchEffect.Enums;
using CoreGraphics;

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
            _effect = Element.GetTouchEff();
            _effect.Control = Element as VisualElement;
            _effect.ForceUpdateState(false);
            _gesture = new TouchUITapGestureRecognizer(_effect);
            if (Container != null)
            {
                Container.AddGestureRecognizer(_gesture);
                Container.UserInteractionEnabled = true;
            }
        }

        protected override void OnDetached()
        {
            _effect.Control = null;
            _effect = null;
            Container?.RemoveGestureRecognizer(_gesture);
            _gesture?.Dispose();
            _gesture = null;
        }
    }

    internal sealed class TouchUITapGestureRecognizer : UIGestureRecognizer
    {
        private TouchEff _effect;

        private float? _defaultRadius;
        private float? _defaultShadowRadius;
        private float? _defaultShadowOpacity;

        public TouchUITapGestureRecognizer(TouchEff effect)
            => _effect = effect;

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            HandleTouch(TouchStatus.Started);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            HandleTouch(_effect.Status == TouchStatus.Started ? TouchStatus.Completed : TouchStatus.Canceled);
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            HandleTouch(TouchStatus.Canceled);
            base.TouchesCancelled(touches, evt);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            var touch = touches?.AnyObject as UITouch;
            var renderer = _effect.Control.GetRenderer() as UIView;
            var point = renderer != null ? touch?.LocationInView(renderer) : null;

            var status = point != null && renderer.Bounds.Contains(point.Value) ? TouchStatus.Started : TouchStatus.Canceled;
            if (_effect.Status != status)
            {
                HandleTouch(status);
            }

            base.TouchesMoved(touches, evt);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _effect = null;
            }
            base.Dispose(disposing);
        }

        private void HandleTouch(TouchStatus status)
        {
            _effect.HandleTouch(status);
            if (!_effect.NativeAnimation)
            {
                return;
            }
            var renderer = _effect.Control.GetRenderer() as UIView;
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
                    renderer.Layer.Opacity = isStarted ? 0.5f : (float)_effect.Control.Opacity;
                }
                else
                {
                    renderer.Layer.BackgroundColor = (isStarted ? color : _effect.Control.BackgroundColor).ToCGColor();
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
}
