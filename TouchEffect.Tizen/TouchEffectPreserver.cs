﻿using Xamarin.Forms.Internals;

namespace TouchEffect.Tizen
{
    [Preserve(AllMembers = true)]
    public static class TouchEffectPreserver
    {
        public static void Preserve()
        {
            PlatformTouchEff.Preserve();
        }
    }
}
