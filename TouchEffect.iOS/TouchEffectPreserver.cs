using Foundation;

namespace TouchEffect.iOS
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
