using TouchEffect.Enums;
using TouchEffect.Extensions;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace TouchEffect
{
    public class TouchImage : Image
    {
        private readonly object _setImageLocker = new object();

        public TouchImage()
            => Effects.Add(new TouchEff(GetAnimationTask));

        public static readonly BindableProperty RegularBackgroundImageSourceProperty = BindableProperty.Create(
            nameof(RegularBackgroundImageSource),
            typeof(ImageSource),
            typeof(TouchImage),
            default(ImageSource),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty HoveredBackgroundImageSourceProperty = BindableProperty.Create(
            nameof(HoveredBackgroundImageSource),
            typeof(ImageSource),
            typeof(TouchImage),
            default(ImageSource),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedBackgroundImageSourceProperty = BindableProperty.Create(
            nameof(PressedBackgroundImageSource),
            typeof(ImageSource),
            typeof(TouchImage),
            default(ImageSource),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty RegularBackgroundImageAspectProperty = BindableProperty.Create(
            nameof(RegularBackgroundImageAspect),
            typeof(Aspect),
            typeof(TouchImage),
            default(Aspect),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty HoveredBackgroundImageAspectProperty = BindableProperty.Create(
            nameof(HoveredBackgroundImageAspect),
            typeof(Aspect),
            typeof(TouchImage),
            default(Aspect),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty PressedBackgroundImageAspectProperty = BindableProperty.Create(
            nameof(PressedBackgroundImageAspect),
            typeof(Aspect),
            typeof(TouchImage),
            default(Aspect),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                bindable.GetTouchEff()?.ForceUpdateState();
            });

        public static readonly BindableProperty ShouldSetImageOnAnimationEndProperty = BindableProperty.Create(
            nameof(ShouldSetImageOnAnimationEnd),
            typeof(bool),
            typeof(TouchImage),
            default(bool));

        public ImageSource RegularBackgroundImageSource
        {
            get => GetValue(RegularBackgroundImageSourceProperty) as ImageSource;
            set => SetValue(RegularBackgroundImageSourceProperty, value);
        }

        public ImageSource HoveredBackgroundImageSource
        {
            get => GetValue(HoveredBackgroundImageSourceProperty) as ImageSource;
            set => SetValue(HoveredBackgroundImageSourceProperty, value);
        }

        public ImageSource PressedBackgroundImageSource
        {
            get => GetValue(PressedBackgroundImageSourceProperty) as ImageSource;
            set => SetValue(PressedBackgroundImageSourceProperty, value);
        }

        public Aspect RegularBackgroundImageAspect
        {
            get => (Aspect)GetValue(RegularBackgroundImageAspectProperty);
            set => SetValue(RegularBackgroundImageAspectProperty, value);
        }

        public Aspect HoveredBackgroundImageAspect
        {
            get => (Aspect)GetValue(HoveredBackgroundImageAspectProperty);
            set => SetValue(HoveredBackgroundImageAspectProperty, value);
        }

        public Aspect PressedBackgroundImageAspect
        {
            get => (Aspect)GetValue(PressedBackgroundImageAspectProperty);
            set => SetValue(PressedBackgroundImageAspectProperty, value);
        }

        public bool ShouldSetImageOnAnimationEnd
        {
            get => (bool)GetValue(ShouldSetImageOnAnimationEndProperty);
            set => SetValue(ShouldSetImageOnAnimationEndProperty, value);
        }

        private async Task GetAnimationTask(TouchEff sender, TouchState touchState, HoverState hoverState, int duration, Easing easing, CancellationToken token)
        {
            var regularBackgroundImageSource = RegularBackgroundImageSource;
            var pressedBackgroundImageSource = PressedBackgroundImageSource;
            var hoveredBackgroundImageSource = HoveredBackgroundImageSource;

            if (regularBackgroundImageSource == null &&
                pressedBackgroundImageSource == null)
            {
                return;
            }

            var aspect = RegularBackgroundImageAspect;
            var source = regularBackgroundImageSource;
            if (touchState == TouchState.Pressed)
            {
                aspect = PressedBackgroundImageAspect;
                source = pressedBackgroundImageSource;
            }
            else if (hoverState == HoverState.Hovering)
            {
                aspect = HoveredBackgroundImageAspect;
                source = hoveredBackgroundImageSource;
            }

            if (ShouldSetImageOnAnimationEnd)
            {
                await Task.Delay(duration, token);
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            lock (_setImageLocker)
            {
                BatchBegin();
                Aspect = aspect;
                Source = source;
                BatchCommit();
            }
        }
    }
}