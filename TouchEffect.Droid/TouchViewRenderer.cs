using Android.Content;
using Android.Runtime;
using Android.Views;
using TouchEffect;
using TouchEffect.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System;

[assembly: ExportRenderer(typeof(TouchView), typeof(TouchViewRenderer))]
namespace TouchEffect.Droid
{
    [Preserve(AllMembers = true)]
    public class TouchViewRenderer : VisualElementRenderer<TouchView>
    {
        [Obsolete("Forms 2.4 support")]
        public TouchViewRenderer()
        {
        }

        public TouchViewRenderer(Context context) : base(context)
        {
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                    Element?.HandleTouch(GestureStatus.Started);
                    break;
                case MotionEventActions.Up:
                    Element?.HandleTouch(GestureStatus.Completed);
                    break;
                case MotionEventActions.Cancel:
                    Element?.HandleTouch(GestureStatus.Canceled);
                    break;
            }
            return base.OnTouchEvent(e);
        }
    }
}