using Xamarin.Forms;

namespace TouchEffectSample
{
    public partial class TouchFadeViewPage : ContentPage
    {
        public TouchFadeViewPage()
        {
            InitializeComponent();
        }

		private void Handle_TouchCompleted(TouchEffect.TouchView sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
			DisplayAlert("Thank you", ":)", "OK");
        }
    }
}
