using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TouchEffectSample
{
    public partial class TouchEffPage : ContentPage
    {
        public TouchEffPage()
        {
            InitializeComponent();
        }

        void Handle_Completed(Xamarin.Forms.VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            DisplayAlert("TAPPED", null, "OK");
        }
    }
}
