using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using TouchEffect.iOS;
using TouchEffect;
using TouchEffect.Enums;

[assembly: ExportRenderer(typeof(TouchView), typeof(TouchViewRenderer))]
namespace TouchEffect.iOS
{
    [Preserve(AllMembers = true)]
    public class TouchViewRenderer : VisualElementRenderer<TouchView>
    {
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
			Element?.HandleTouch(TouchStatus.Started);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
			Element?.HandleTouch(TouchStatus.Completed);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
			Element?.HandleTouch(TouchStatus.Canceled);
        }
    }
}