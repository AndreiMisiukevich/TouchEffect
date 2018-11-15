# TouchView group controls for Xamarin Forms (based on Xamarin Forms ContentView)

## Setup
* Available on NuGet: [TouchView](http://www.nuget.org/packages/TouchView) [![NuGet](https://img.shields.io/nuget/v/TouchView.svg?label=NuGet)](https://www.nuget.org/packages/TouchView)
* Add nuget package to your Xamarin.Forms .netStandard/PCL project and to your platform-specific projects (iOS and Android)
* **iOS:** add *TouchViewRenderer.Initialize()* line to your AppDelegate (preserve from linker)
```csharp
using TouchEffect.iOS;
namespace YourApp.iOS
{
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            TouchViewRenderer.Initialize();
            LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }
    }
}
```

|Platform|Version|
| ------------------- | ------------------- |
|Xamarin.iOS|8.0+|
|Xamarin.Android|15+|

## TouchView, TouchImage, TouchFadeView, TouchColorView
This plugin provides opportunity to create views with touch effects without using any gestureRecognizers

![Sample GIF](https://media.giphy.com/media/5BUTDOexcuBUvKxkPy/giphy.gif)

## Samples
The samples you can find here https://github.com/AndreiMisiukevich/TouchEffect/tree/master/TouchEffectSample

**XAML:**
* TouchImage (Set RegularSource and PressedSource)
```xml
       <controls:TouchImage
            VerticalOptions="CenterAndExpand"
            HorizontalOptions="CenterAndExpand"
            HeightRequest="250"
            WidthRequest="250"
            RegularSource="button"
            PressedSource="button_pressed"
            Completed="Handle_TouchCompleted" />
```

* TouchFadeView (You may set RegularOpacity (1.0 by default), PressedOpacity (0.6 by default), FadeDuration (0 by default) and FadeEasing (null by default))
```xml
       <controls:TouchFadeView
            Padding="10, 5"
            BackgroundColor="Black"
            VerticalOptions="CenterAndExpand"
            HorizontalOptions="CenterAndExpand"
            Completed="Handle_TouchCompleted">

            <Label Text="CLICK ME" 
                   TextColor="White" 
                   FontSize="60"/>
        </controls:TouchFadeView>
```

* TouchColorView (Set RegularColor and PressedColor)
```xml
       <controls:TouchColorView
            RegularColor="Black"
            PressedColor="Maroon"
            Padding="10, 5"
            VerticalOptions="CenterAndExpand"
            HorizontalOptions="CenterAndExpand"
            Completed="Handle_TouchCompleted">

            <Label Text="CLICK ME" 
                   TextColor="White" 
                   FontSize="60"/>
        </controls:TouchColorView>
```

* TouchView - if you want to customize/extend existing controls, you may add touchView and handle UI changes with triggers (Getting TouchImage is below)
```xml
       <touch:TouchView x:Name="container"
            HeightRequest="250"
            WidthRequest="250"
            Completed="Handle_TouchCompleted">
            <Image>
                <Image.Triggers>
                    <DataTrigger TargetType="Image" 
                                 Binding="{Binding Source={x:Reference container}, Path=State}"
                                 Value="Regular">
                        <Setter Property="Source" Value="button" />
                    </DataTrigger>
                    <DataTrigger TargetType="Image" 
                                 Binding="{Binding Source={x:Reference container}, Path=State}"
                                 Value="Pressed">
                        <Setter Property="Source" Value="button_pressed" />
                    </DataTrigger>
                </Image.Triggers>
            </Image>
        </touch:TouchView>
```

Check source code for more info, or 🇧🇾 ***just ask me =)*** 🇧🇾

## License
The MIT License (MIT) see [License file](LICENSE)

## Contribution
Feel free to create issues and PRs 😃

