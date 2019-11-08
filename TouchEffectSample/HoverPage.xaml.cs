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

        private void TouchEff_StatusChanged(VisualElement sender, TouchEffect.EventArgs.TouchStatusChangedEventArgs args)
        {
            Debug.WriteLine(args.Status);
            if(args.Status == TouchEffect.Enums.TouchStatus.HoverEnter)
            {
                sender.Opacity = 0.5;
            }
            else if(args.Status == TouchEffect.Enums.TouchStatus.HoverLeave || args.Status == TouchEffect.Enums.TouchStatus.Canceled)
            {
                sender.Opacity = 1;
            }
        }
    }
}