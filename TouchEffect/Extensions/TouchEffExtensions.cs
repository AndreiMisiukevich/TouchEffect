﻿using Xamarin.Forms;
using System.Linq;
using System.ComponentModel;

namespace TouchEffect
{
    public static class TouchEffExtensions
    {
        public static TouchEff GetTouchEff(this BindableObject bindable)
        {
            var effects = (bindable as VisualElement)?.Effects?.OfType<TouchEff>();
            return effects?.FirstOrDefault(x => !x.IsAutoGenerated)
                ?? effects?.FirstOrDefault(x => x.IsAutoGenerated);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TouchEff PickTouchEff(this BindableObject bindable)
        {
            var effects = (bindable as VisualElement)?.Effects?.OfType<TouchEff>();
            return effects?.FirstOrDefault(x => !x.IsAutoGenerated && !x.IsUsed)
                ?? effects?.FirstOrDefault(x => x.IsAutoGenerated)
                ?? effects?.FirstOrDefault();
        }
    }
}
