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
                                Text = "(Background Image)",
								Command = new Command(() => MainPage.Navigation.PushAsync(new TouchImagePage()))
                            },
							new Button
                            {
                                Text = "(Fade Effect)",
								Command = new Command(() => MainPage.Navigation.PushAsync(new TouchFadeViewPage()))
                            },
							new Button
                            {
                                Text = "(Background Color and transofrmations)",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new TouchColorViewPage()))
                            }
                        }
					}
				}
			});
        }
    }
}
