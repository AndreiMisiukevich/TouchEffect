using Xamarin.Forms;

namespace TouchEffectSample
{
    public partial class TouchImagePage : ContentPage
    {
        public TouchImagePage()
        {
            InitializeComponent();
        }

		private void Handle_TouchCompleted(TouchEffect.TouchView sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            DisplayAlert("BOOOM", ":(", "OOOOOPS");
        }
    }
}
