using Xamarin.Forms;

namespace TouchEffectSample
{
    public partial class TouchViewPage : ContentPage
    {
		public TouchViewPage()
        {
			InitializeComponent();
        }

		private void Handle_TouchCompleted(TouchEffect.TouchView sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
		{
			DisplayAlert("BOOOM", ":(", "OOOOOPS");
		}
    }
}
