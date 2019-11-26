using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TouchEffectSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NativeAnimationPage : ContentPage
    {
        public NativeAnimationPage()
        {
            InitializeComponent();
        }

        private void TouchEff_Completed(VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            Application.Current.MainPage.DisplayAlert("Tap!", "The Completed event was fired", "Cancel");
        }
    }
}