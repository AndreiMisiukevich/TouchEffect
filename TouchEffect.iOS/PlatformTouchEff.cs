﻿using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using TouchEffect.iOS;
using Xamarin.Forms;
using TouchEffect;
using TouchEffect.Extensions;
using TouchEffect.Enums;

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

        public TouchUITapGestureRecognizer(TouchEff effect)
            => _effect = effect;

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            _effect.HandleTouch(TouchStatus.Started);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            _effect.HandleTouch(_effect.Status == TouchStatus.Started ? TouchStatus.Completed : TouchStatus.Canceled);
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            _effect.HandleTouch(TouchStatus.Canceled);
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
                _effect.HandleTouch(status);
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
    }
}
