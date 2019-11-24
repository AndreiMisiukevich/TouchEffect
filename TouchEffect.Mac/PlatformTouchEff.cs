using Xamarin.Forms.Platform.MacOS;
using AppKit;
using Foundation;
using TouchEffect.Mac;
using Xamarin.Forms;
using TouchEffect;
using TouchEffect.Extensions;
using TouchEffect.Enums;

[assembly: ResolutionGroupName(nameof(TouchEffect))]
[assembly: ExportEffect(typeof(PlatformTouchEff), nameof(TouchEff))]
namespace TouchEffect.Mac
{
    [Preserve(AllMembers = true)]
    public class PlatformTouchEff : PlatformEffect
    {
        public static void Preserve() { }

        private NSGestureRecognizer _gesture;
        private TouchEff _effect;
        private MouseTrackingView _mouseTrackingView;

        protected override void OnAttached()
        {
            _effect = Element.GetTouchEff();
            _effect.Control = Element as VisualElement;
            _effect.ForceUpdateState(false);
            if (Container != null)
            {
                _gesture = new TouchNSClickGestureRecognizer(_effect, Container);
                Container.AddGestureRecognizer(_gesture);
                Container.AddSubview(_mouseTrackingView = new MouseTrackingView(_effect));
            }
        }

        protected override void OnDetached()
        {
            _mouseTrackingView?.RemoveFromSuperview();
            _mouseTrackingView?.Dispose();
            _mouseTrackingView = null;
            _effect.Control = null;
            _effect = null;
            if (_gesture != null)
            {
                Container?.RemoveGestureRecognizer(_gesture);
            }
            _gesture?.Dispose();
            _gesture = null;
        }
    }

    internal sealed class MouseTrackingView : NSView
    {
        private NSTrackingArea _trackingArea;
        private TouchEff _effect;

        public MouseTrackingView(TouchEff effect)
        {
            _effect = effect;
            AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
        }

        public override void UpdateTrackingAreas()
        {
            if (_trackingArea != null)
            {
                RemoveTrackingArea(_trackingArea);
                _trackingArea.Dispose();
            }
            _trackingArea = new NSTrackingArea(Frame, NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.ActiveAlways, this, null);
            AddTrackingArea(_trackingArea);
        }

        public override void MouseEntered(NSEvent theEvent) => _effect.HandleHover(HoverStatus.Entered);

        public override void MouseExited(NSEvent theEvent) => _effect.HandleHover(HoverStatus.Exited);

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(_trackingArea != null)
                {
                    RemoveTrackingArea(_trackingArea);
                    _trackingArea.Dispose();
                }
                _effect = null;
            }
            base.Dispose(disposing);
        }
    }

    internal sealed class TouchNSClickGestureRecognizer : NSGestureRecognizer
    {
        private TouchEff _effect;
        private NSView _container;

        public TouchNSClickGestureRecognizer(TouchEff effect, NSView container)
        {
            _effect = effect;
            _container = container;
        }

        private Rectangle ViewRect
        {
            get
            {
                var frame = _container.Frame;
                var parent = _container.Superview;
                while (parent != null)
                {
                    frame = new CoreGraphics.CGRect(frame.X + parent.Frame.X, frame.Y + parent.Frame.Y, frame.Width, frame.Height);
                    parent = parent.Superview;
                }
                return frame.ToRectangle();
            }
        }

        public override void MouseDown(NSEvent mouseEvent)
        {
            _effect.HandleTouch(TouchStatus.Started);
            base.MouseDown(mouseEvent);
        }

        public override void MouseUp(NSEvent mouseEvent)
        {
            if (_effect.HoverStatus == HoverStatus.Entered)
            {
                var touchPoint = mouseEvent.LocationInWindow.ToPoint();
                var status = ViewRect.Contains(touchPoint)
                    ? TouchStatus.Completed
                    : TouchStatus.Canceled;

                _effect.HandleTouch(status);
            }
            base.MouseUp(mouseEvent);
        }

        public override void MouseDragged(NSEvent mouseEvent)
        {
            var status = ViewRect.Contains(mouseEvent.LocationInWindow.ToPoint()) ? TouchStatus.Started : TouchStatus.Canceled;

            if ((status == TouchStatus.Canceled && _effect.HoverStatus == HoverStatus.Entered) ||
                (status == TouchStatus.Started && _effect.HoverStatus == HoverStatus.Exited))
            {
                _effect.HandleHover(status == TouchStatus.Started ? HoverStatus.Entered : HoverStatus.Exited);
            }

            if (_effect.Status != status)
            {
                _effect.HandleTouch(status);
            }

            base.MouseDragged(mouseEvent);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _effect = null;
                _container = null;
            }
            base.Dispose(disposing);
        }
    }
}