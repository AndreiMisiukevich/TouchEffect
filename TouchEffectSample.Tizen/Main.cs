using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using TouchEffect.Tizen;

namespace TouchEffectSample.Tizen
{
    class Program : FormsApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            LoadApplication(new App());
        }

        static void Main(string[] args)
        {
            var app = new Program();
            Forms.Init(app, true);
            TouchEffectPreserver.Preserve();
            app.Run(args);
        }
    }
}
