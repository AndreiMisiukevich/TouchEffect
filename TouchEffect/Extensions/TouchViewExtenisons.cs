using Xamarin.Forms;

namespace TouchEffect.Extensions
{
    public static class TouchViewExtenisons
    {
        public static TouchView AsTouchView(this BindableObject bindable)
            => bindable as TouchView;
    }
}
