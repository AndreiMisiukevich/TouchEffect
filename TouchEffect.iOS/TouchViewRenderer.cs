using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using TouchEffect.iOS;
using TouchEffect;
using TouchEffect.Enums;
using System;

[assembly: ExportRenderer(typeof(TouchView), typeof(TouchViewRenderer))]
namespace TouchEffect.iOS
{
    [Preserve(AllMembers = true)]
    public class TouchViewRenderer : VisualElementRenderer<TouchView>
    {
        [Obsolete("Please use Preserve method instead.")]
        public static void Initialize() { }

        public static void Preserve() { }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            Element?.HandleTouch(TouchStatus.Started);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            Element?.HandleTouch(TouchStatus.Completed);
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            Element?.HandleTouch(TouchStatus.Canceled);
            base.TouchesCancelled(touches, evt);
        }
    }
}