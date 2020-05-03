using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchEffect.Enums;
using TouchEffect.EventArgs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TouchEffectSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScrollPage : ContentPage
    {
        public ScrollPage()
        {
            InitializeComponent();
        }

        private void Completed(VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            Application.Current.MainPage.DisplayAlert("Clicked on a square", "The Completed event was fired", "Cancel");
        }
    }
}