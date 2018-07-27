using System;
using Xamarin.Forms;
using TouchEffect;
namespace TouchEffectSample
{
    public class App : Application
    {
        public App()
        {
            new TouchView(); // Sample fix
            MainPage = new SamplePage();
        }
    }
}
