using Android.Runtime;

namespace TouchEffect.Android
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
