using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TouchEffectSample
{
    public partial class TouchColorViewPage : ContentPage
    {
        public TouchColorViewPage()
        {
            InitializeComponent();
        }

		private void Handle_TouchCompleted(TouchEffect.TouchView sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            DisplayAlert("Thank you", ":)", "OK");
        }
    }
}
