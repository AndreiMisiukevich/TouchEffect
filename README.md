# TouchView control for Xamarin Forms (based on Xamarin Forms AbsoluteLayout)

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

## TouchView
This plugin provides opportunity to create views with touch effects without using TapGestureRecognizer

![Sample GIF](https://media.giphy.com/media/5BUTDOexcuBUvKxkPy/giphy.gif)

## Samples
The samples you can find here https://github.com/AndreiMisiukevich/TouchEffect/tree/master/TouchEffectSample

**XAML:** use TouchView for achieving responisve UI (Changing background image or/and background color, Fading/Recovering)

```xml
...
        <touch:TouchView
            RegularBackgroundImageSource="button"
            PressedBackgroundImageSource="button_pressed"
            PressedOpacity="1"
            Completed="Handle_TouchCompleted" />
...
        <touch:TouchView
            FadeDuration="250"
            RecoverDuration="250"             
            Completed="Handle_TouchCompleted">

            <Label Text="CLICK ME" />

        </touch:TouchView>
...
       <touch:TouchView
            RegularBackgroundColor="Black"
            PressedBackgroundColor="Maroon"
            PressedOpacity="1"
            Completed="Handle_TouchCompleted">

            <Label Text="CLICK ME"/>

        </touch:TouchView>
...
```

### All Properties
Property | Type | Default | Description
--- | --- | --- | ---
Command | `ICommand` | null | Touch Command handler
CommandParameter | `object` | null | Touch Command handler parameter
Status | `TouchStatus` | Completed | Current touch status
State | `TouchState` | Regular | Current touch state
RegularBackgroundColor | `Color` | Default | Background color of regular state
PressedBackgroundColor | `Color` | Default | Background color of pressed state
RegularOpacity | `double` | 1.0 | Opacity of regular state
PressedOpacity | `double` | 0.6 | Opacity of pressed state
RegularScale | `double` | 1.0 | Scale of regular state
PressedScale | `double` | 1.0 | Scale of pressed state
PressedAnimationDuration | `int` | 0 | The duration of animation by applying PressedOpacity and/or PressedBackgroundColor and/or PressedScale
PressedAnimationEasing | `Easing` | null | The easing of animation by applying PressedOpacity and/or PressedBackgroundColor and/or PressedScale
RegularAnimationDuration | `int` | 0 | The duration of animation by applying RegularOpacity and/or RegularBackgroundColor and/or RegularScale
RegularAnimationEasing | `Easing` | null | The easing of animation by applying RegularOpacity and/or RegularBackgroundColor and/or RegularScale
RegularBackgroundImageSource | `ImageSource` | null | Background image source of regular state
PressedBackgroundImageSource | `ImageSource` | null | Background image source of pressed state
RegularBackgroundImageAspect | `Aspect` | AspectFit | Background image aspect of pressed state
PressedBackgroundImageAspect | `Aspect` | AspectFit | Background image aspect of pressed state
RippleCount | `int` | 0 | This property allows to set ripple of animation (Pressed/Regular animation loop). '**0**: disabled'; '**-1**: infinite loop'; '**1, 2, 3 ... n**: Ripple's interations'
BackgroundImage | `Image` | null | Background control (it will be created automatically, if RegularBackgroundImageAspect isn't null or PressedBackgroundImageSource aren't isn't null

**If you want to customize/extend existing controls, you may observe State via triggers
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
                        <Setter Property="Source" Value="icon_regular" />
                    </DataTrigger>
                    <DataTrigger TargetType="Image" 
                                 Binding="{Binding Source={x:Reference container}, Path=State}"
                                 Value="Pressed">
                        <Setter Property="Source" Value="icon_pressed" />
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
