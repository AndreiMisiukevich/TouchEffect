using Foundation;

namespace TouchEffect.Mac
{
    [Preserve(AllMembers = true)]
    public static class TouchEffectPreserver
    {
        public static void Preserve()
            => PlatformTouchEff.Preserve();
    }
}
