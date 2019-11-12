using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TouchEffectSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HoverPage : ContentPage
    {
        public HoverPage()
        {
            InitializeComponent();
        }

        private void TouchEff_HoverStateChanged(VisualElement sender, TouchEffect.EventArgs.HoverStateChangedEventArgs args)
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

        private void TouchEff_HoverStatusChanged(VisualElement sender, TouchEffect.EventArgs.HoverStatusChangedEventArgs args)
        {
            Debug.WriteLine(args.Status);
        }
    }
}