using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TouchEffectSample
{
    public partial class SamplePage : ContentPage
    {
        public SamplePage()
        {
			InitializeComponent();
        }

		void Handle_TouchCompleted(TouchEffect.TouchView sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
		{
			DisplayAlert("BOOOM", ":(", "OOOOOPS");
		}
    }
}
