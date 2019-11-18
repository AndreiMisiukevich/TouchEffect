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
            if(args.State == TouchEffect.Enums.TouchState.Pressed)
            {
                sender.BackgroundColor = Color.Red;
            }
            else
            {
                sender.BackgroundColor = Color.Black;
            }
        }

        private void HoverStateChanged(VisualElement sender, TouchEffect.EventArgs.HoverStateChangedEventArgs args)
        {
            if(args.State == TouchEffect.Enums.HoverState.Hovering)
            {
                sender.Opacity = 0.5;
            }
            else
            {
                sender.Opacity = 1;
            }
        }

        private async void Completed(VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            await sender.ScaleTo(1.1, 120);
            await sender.ScaleTo(1, 120);
        }
    }
}