using System;
using Xamarin.Forms;
using TouchEffect;
namespace TouchEffectSample
{
    public class App : Application
    {
        public App()
        {
			MainPage = new NavigationPage(new ContentPage
			{
		        Content = new ScrollView
				{
					Content = new StackLayout
					{
						Children = {
							new Button
							{
								Text = "TouchView (Base)",
								Command = new Command(() => MainPage.Navigation.PushAsync(new TouchViewPage()))
							},
							new Button
                            {
                                Text = "TouchImage",
								Command = new Command(() => MainPage.Navigation.PushAsync(new TouchImagePage()))
                            },
							new Button
                            {
                                Text = "TouchFadeView",
								Command = new Command(() => MainPage.Navigation.PushAsync(new TouchFadeViewPage()))
                            },
							new Button
                            {
                                Text = "TouchColorView",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new TouchColorViewPage()))
                            },
						}
					}
				}
			});
        }
    }
}
