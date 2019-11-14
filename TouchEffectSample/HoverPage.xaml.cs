using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TouchEffectSample
{
    public partial class HoverPage : ContentPage
    {
        public HoverPage()
        {
            InitializeComponent();
        }

        private void TouchEff_HoverStateChanged(VisualElement sender, TouchEffect.EventArgs.HoverStateChangedEventArgs args)
        {
            sender.Opacity = args.State == TouchEffect.Enums.HoverState.Hovering ? 0.5 : 1.0;
        }
    }
}