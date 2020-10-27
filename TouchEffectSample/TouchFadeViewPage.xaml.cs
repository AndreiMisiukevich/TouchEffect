using System.Windows.Input;
using Xamarin.Forms;

namespace TouchEffectSample
{
    public partial class TouchFadeViewPage : ContentPage
    {
        public TouchFadeViewPage()
        {
            InitializeComponent();
        }

        public ICommand LongPressCommand { get; } = new Command(() =>
        {
            Application.Current.MainPage.DisplayAlert("Long Press", ":)", "OK");
        });

        public ICommand Command { get; } = new Command(() =>
        {
            Application.Current.MainPage.DisplayAlert("Thank you", ":)", "OK");
        });
    }
}
