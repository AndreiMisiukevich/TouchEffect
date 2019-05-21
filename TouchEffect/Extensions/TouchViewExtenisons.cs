using Xamarin.Forms;
using System.Linq;
using System;

namespace TouchEffect.Extensions
{
    public static class TouchViewExtenisons
    {
        [Obsolete]
        public static TouchView AsTouchView(this BindableObject bindable)
            => bindable as TouchView;

        public static TouchImage AsTouchImage(this BindableObject bindable)
            => bindable as TouchImage;

        public static TouchEff GetTouchEff(this BindableObject bindable)
            => (bindable as VisualElement)?.Effects.FirstOrDefault(x => x is TouchEff) as TouchEff;

        public static int AdjustDurationMultiplier(this int duration, double? multiplier)
            => multiplier.HasValue
                ? (int)(multiplier.Value * duration)
                : duration;
    }
}
