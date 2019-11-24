using Xamarin.Forms;
using System.Windows.Input;
using TouchEffect;

namespace TouchEffectSample
{
    public partial class TouchImagePage : ContentPage
    {
        public TouchImagePage()
        {
            InitializeComponent();
        }

        public ICommand Command { get; } = new Command(() =>
        {
            Application.Current.MainPage.DisplayAlert("BOOOM", ":(", "OOOOOPS");
            TouchEff tef = new TouchEff()
        });

    }
}
