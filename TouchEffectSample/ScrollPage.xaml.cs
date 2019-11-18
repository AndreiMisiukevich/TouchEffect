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
    public partial class ScrollPage : ContentPage
    {
        public ScrollPage()
        {
            InitializeComponent();
            foreach(BoxView box in boxstack.Children)
            {
                TouchEffect.TouchEff touch = new TouchEffect.TouchEff();
                box.Effects.Add(touch);
                touch.Completed += Completed;
                touch.HoverStateChanged += HoverStateChanged;
                touch.StateChanged += StateChanged;
            }
        }

        private void StateChanged(VisualElement sender, TouchEffect.EventArgs.TouchStateChangedEventArgs args)
        {
            sender.BackgroundColor = args.State == TouchEffect.Enums.TouchState.Pressed ? Color.Red : Color.Black;
        }

        private void HoverStateChanged(VisualElement sender, TouchEffect.EventArgs.HoverStateChangedEventArgs args)
        {
            sender.Opacity = args.State == TouchEffect.Enums.HoverState.Hovering ? 0.5 : 1;
        }

        private void Completed(VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            Application.Current.MainPage.DisplayAlert("Clicked on a square", "The Completed event was fired", "Cancel");
        }
    }
}