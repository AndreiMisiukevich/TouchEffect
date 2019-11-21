using System;
using Xamarin.Forms;
using TouchEffect;
using TouchEffectSample.Issues;
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
                            },
                            new Button
                            {
                                Text = "(Toggle sample)",
                                Command = new Command(() => MainPage.Navigation.PushAsync(Device.RuntimePlatform == Device.Android 
                                    ? (Page) new AndroidTogglePage() 
                                    : new TogglePage()))
                            },
                            new Button
                            {
                                Text = "(Hover sample)",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new HoverPage()))
                            },
                            new Button
                            {
                                Text = "(Scroll sample)",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new ScrollPage()))
                            },
                            new Button
                            {
                                Text = "(Native animation sample)",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new NativeAnimationPage()))
                            }
                        }
					}
				}
			});
        }
    }
}
