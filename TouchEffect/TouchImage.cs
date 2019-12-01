using TouchEffect.Enums;
using TouchEffect.Extensions;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

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

        [EditorBrowsable(EditorBrowsableState.Never)]
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

        [EditorBrowsable(EditorBrowsableState.Never)]
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

        [EditorBrowsable(EditorBrowsableState.Never)]
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

        [EditorBrowsable(EditorBrowsableState.Never)]
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

        private async Task GetAnimationTask(TouchEff sender, TouchState state, int duration, CancellationToken token)
        {
            var regularBackgroundImageSource = RegularBackgroundImageSource;
            var pressedBackgroundImageSource = PressedBackgroundImageSource;

            if (regularBackgroundImageSource == null &&
                pressedBackgroundImageSource == null)
            {
                return;
            }

            var aspect = RegularBackgroundImageAspect;
            var source = regularBackgroundImageSource;
            if (state == TouchState.Pressed)
            {
                aspect = PressedBackgroundImageAspect;
                source = pressedBackgroundImageSource;
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