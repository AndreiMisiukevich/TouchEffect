# TouchView control for Xamarin Forms (based on Xamarin Forms AbsoluteLayout)
This plugin provides opportunity to create views with touch effects without using TapGestureRecognizer. It makes it possible to change the appearance of any control in response to touch events, either directly via xaml or with your custom logic hooked up to the events which this plugin exposes.

With this plugin it is also possible to respond to hover events (if the platform exposes them) and to display native touch feedback events (Tilt on UWP, Ripple on Android, Opacity/Color changing on iOS).

### Build Status
* Azure DevOps: [![Build status](https://dev.azure.com/andreimisiukevich/TouchView/_apis/build/status/TouchView-nuget-CI)](https://dev.azure.com/andreimisiukevich/TouchView/_build/latest?definitionId=1)

## GIF
<html>
  <table style="width:100%">
    <tr>
      <th>TouchImage</th>
      <th>Fade / Ripple</th> 
      <th>Background color / Transformations</th>
      <th>IsToggled / image</th>
    </tr>
    <tr>
      <td><img src="https://github.com/AndreiMisiukevich/TouchEffect/blob/master/images/1.gif?raw=true"></td>
      <td><img src="https://github.com/AndreiMisiukevich/TouchEffect/blob/master/images/2.gif?raw=true"></td>
      <td><img src="https://github.com/AndreiMisiukevich/TouchEffect/blob/master/images/3.gif?raw=true"></td>
    <td><img src="https://github.com/AndreiMisiukevich/TouchEffect/blob/master/images/4.gif?raw=true"></td>
    </tr>
  </table>
</html>

## Setup
* Available on NuGet: [TouchView](http://www.nuget.org/packages/TouchView) [![NuGet](https://img.shields.io/nuget/v/TouchView.svg?label=NuGet)](https://www.nuget.org/packages/TouchView)
* Add nuget package to your Xamarin.Forms .netStandard/PCL project and to your platform-specific projects (iOS and Android)
* Add *TouchEffectPreserver.Preserve()* line to your AppDelegate and MainActivity (preserve from linker)

|Platform|Version|
| ------------------- | ------------------- |
|Xamarin.iOS|8.0+|
|Xamarin.Android|15+|
|Xamarin.Mac|All|
|Xamarin.UWP|10+|

## Samples
The samples you can find here https://github.com/AndreiMisiukevich/TouchEffect/tree/master/TouchEffectSample

**XAML:** use TouchEff for achieving repsonsive UI (Changing background image or/and background color or/and opacity or/and scale).

Add TouchEff to element's Effects collection and use TouchEff attached properties for setting up touch visual effect.

```xaml
...
  xmlns:touch="clr-namespace:TouchEffect;assembly=TouchEffect"
...
       <ContentView
            touch:TouchEff.PressedAnimationDuration="800"
            touch:TouchEff.RegularAnimationDuration="800"
            touch:TouchEff.PressedScale="0.9"
            touch:TouchEff.PressedOpacity="0.6"
            touch:TouchEff.RippleCount="-1"
            
            Padding="10, 5"
            BackgroundColor="Black"
            VerticalOptions="CenterAndExpand"
            HorizontalOptions="CenterAndExpand">
            
            <ContentView.Effects>
                <touch:TouchEff Completed="Handle_TouchCompleted"/>
            </ContentView.Effects>
            
            <Label Text="CLICK ME" 
                   TextColor="White" 
                   FontSize="60"/>
            
        </ContentView>
...
       <StackLayout
            touch:TouchEff.RegularBackgroundColor="Green"
            touch:TouchEff.PressedBackgroundColor="Red"
            touch:TouchEff.PressedScale="1.2"
            touch:TouchEff.RippleCount="1"
            touch:TouchEff.PressedRotation="10"
            touch:TouchEff.PressedRotationX="15"
            touch:TouchEff.PressedRotationY="15"
            touch:TouchEff.PressedTranslationX="5"
            touch:TouchEff.PressedTranslationY="5"
            touch:TouchEff.PressedAnimationDuration="500"
            touch:TouchEff.RegularAnimationDuration="500"
            Padding="10, 5"
            VerticalOptions="CenterAndExpand"
            HorizontalOptions="CenterAndExpand">
            <StackLayout.Effects>
                <touch:TouchEff Completed="Handle_TouchCompleted" />
            </StackLayout.Effects>

            <Label Text="CLICK ME" 
                   TextColor="Black" 
                   FontSize="60"/>
        </StackLayout>
```

If you wish to change Image Source on touch, you should use TouchImage control. It has several bindable properties for managing Pressed/Regular Source/Aspect of the image.

```xaml
...
  xmlns:touch="clr-namespace:TouchEffect;assembly=TouchEffect"
...
       <touch:TouchImage
            VerticalOptions="CenterAndExpand"
            HorizontalOptions="CenterAndExpand"
            HeightRequest="250"
            WidthRequest="250"
            RegularBackgroundImageSource="button"
            PressedBackgroundImageSource="button_pressed"
            touch:TouchEff.Command="{Binding Command}"
            />
```

### TouchEff Attached Properties
Property | Type | Default | Description
--- | --- | --- | ---
Command | `ICommand` | null | Touch Command handler
CommandParameter | `object` | null | Touch Command handler parameter
Status | `TouchStatus` | Completed | Current touch status
State | `TouchState` | Regular | Current touch state
RegularBackgroundColor | `Color` | Default | Background color of regular state
PressedBackgroundColor | `Color` | Default | Background color of pressed state
RegularOpacity | `double` | 1.0 | Opacity of regular state
PressedOpacity | `double` | 1.0 | Opacity of pressed state
RegularScale | `double` | 1.0 | Scale of regular state
PressedScale | `double` | 1.0 | Scale of pressed state
RegularTranslationX | `double` | 0.0 | TranslationX of regular state
PressedTranslationX | `double` | 0.0 | TranslationX of pressed state
RegularTranslationY | `double` | 0.0 | TranslationY of regular state
PressedTranslationY | `double` | 0.0 | TranslationY of pressed state
RegularRotation | `double` | 0.0 | Rotation of regular state
PressedRotation | `double` | 0.0 | Rotation of pressed state
RegularRotationX | `double` | 0.0 | RotationX of regular state
PressedRotationX | `double` | 0.0 | RotationX of pressed state
RegularRotationY | `double` | 0.0 | RotationY of regular state
PressedRotationY | `double` | 0.0 | RotationY of pressed state
PressedAnimationDuration | `int` | 0 | The duration of animation by applying PressedOpacity and/or PressedBackgroundColor and/or PressedScale
PressedAnimationEasing | `Easing` | null | The easing of animation by applying PressedOpacity and/or PressedBackgroundColor and/or PressedScale
RegularAnimationDuration | `int` | 0 | The duration of animation by applying RegularOpacity and/or RegularBackgroundColor and/or RegularScale
RegularAnimationEasing | `Easing` | null | The easing of animation by applying RegularOpacity and/or RegularBackgroundColor and/or RegularScale
RippleCount | `int` | 0 | This property allows to set ripple of animation (Pressed/Regular animation loop). '**0**: disabled'; '**-1**: infinite loop'; '**1, 2, 3 ... n**: Ripple's interations'
IsToggled | `bool?` | null | This property allows to achieve "switch" behavior. **null** means that feature is disabled and view will return to inital state after touch releasing
DisallowTouchThreshold | `int` | 0 | Movement threshold for considering **android** touch as canceled
NativeAnimation | `bool` | false | If native platform touch feedback animations are present (Tilt on UWP, Ripple on Android, Opacity/Color on iOS)
NativeAnimationColor | `Color` | Color.Default | The color used for the native touch feedback animation
NativeAnimationRadius | `int` | -1 | The radius of the native ripple animation on Android or Layer radius on iOS

### TouchEff Attached events
Event | Type | Default | Description
--- | --- | --- | ---
StatusChanged | `TEffectStatusChangedHandler` | null | Touch status changed
StateChanged | `TEffectStateChangedHandler` | null | Touch state changed
HoverStatusChanged | `TEffectHoverStatusChangedHandler` | null | Hover status changed
HoverStateChanged | `TEffectHoverStateChangedHandler` | null | Hover state changed
Completed | `TEffectCompletedHandler` | null | User tapped
AnimationStarted | `AnimationStartedHandler` | null | Animation started

### TouchImage Bindable Properties
Property | Type | Default | Description
--- | --- | --- | ---
RegularBackgroundImageSource | `ImageSource` | null | Background image source of regular state
PressedBackgroundImageSource | `ImageSource` | null | Background image source of pressed state
RegularBackgroundImageAspect | `Aspect` | AspectFit | Background image aspect of regular state
PressedBackgroundImageAspect | `Aspect` | AspectFit | Background image aspect of pressed state

**If you want to customize/extend existing controls, you may observe State property via triggers**


Check source code for more info, or 🇧🇾 ***just ask me =)*** 🇧🇾

## License
The MIT License (MIT) see [License file](LICENSE)

## Contribution
Feel free to create issues and PRs 😃
