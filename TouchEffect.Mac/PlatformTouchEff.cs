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

        protected override void OnAttached()
        {
            _effect = Element.GetTouchEff();
            _effect.Control = Element as VisualElement;
            _effect.ForceUpdateState(false);
            if (Container != null)
            {
                _gesture = new TouchNSClickGestureRecognizer(_effect, Container);
                Container.AddGestureRecognizer(_gesture);
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


    internal sealed class TouchNSClickGestureRecognizer : NSGestureRecognizer
    {
        private TouchEff _effect;
        private NSView _container;
        /*private NSTrackingArea _trackingarea;*/

        public TouchNSClickGestureRecognizer(TouchEff effect, NSView container)
        {
            _effect = effect;
            _container = container;
            /*
            _trackingarea = new NSTrackingArea(container.Frame, NSTrackingAreaOptions.MouseEnteredAndExited, container, null);
            _container.AddTrackingArea(_trackingarea);
            */
        }

        private Rectangle ViewRect
        {
            get
            {
                var frame = _container.Frame;
                var parent = _container.Superview;
                while(parent != null)
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
            var touchPoint = mouseEvent.LocationInWindow.ToPoint();
            var status = ViewRect.Contains(touchPoint)
                ? TouchStatus.Completed
                : TouchStatus.Canceled;

            _effect.HandleTouch(status);
            base.MouseUp(mouseEvent);
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