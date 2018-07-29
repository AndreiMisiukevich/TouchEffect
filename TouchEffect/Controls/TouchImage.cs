using System;
using TouchEffect.Enums;
using Xamarin.Forms;

namespace TouchEffect.Controls
{
	public class TouchImage : TouchView
	{
		public static readonly BindableProperty RegularSourceProperty = BindableProperty.Create(
            nameof(RegularSource),
            typeof(ImageSource),
            typeof(TouchView),
			default(ImageSource),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
			    (bindable as TouchImage)?.ForceStateChanged();
            });

		public static readonly BindableProperty PressedSourceProperty = BindableProperty.Create(
			nameof(PressedSource),
			typeof(ImageSource),
			typeof(TouchView),
			default(ImageSource),
			propertyChanged: (bindable, oldValue, newValue) =>
			{
				(bindable as TouchImage)?.ForceStateChanged();
			});

		public TouchImage()
        {         
			Content = Image = new Image
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Aspect = Aspect.AspectFit
            };
        }

		public ImageSource RegularSource
        {
            get => GetValue(RegularSourceProperty) as ImageSource;
            set => SetValue(RegularSourceProperty, value);
        }

		public ImageSource PressedSource
        {
            get => GetValue(PressedSourceProperty) as ImageSource;
            set => SetValue(PressedSourceProperty, value);
        }

		public Image Image { get; }

		protected override void OnStateChanged(TouchView sender, EventArgs.TouchStateChangedEventArgs args)
		{
			Image.Source = args.State == TouchState.Regular
                    ? RegularSource ?? PressedSource
                    : PressedSource ?? RegularSource;
		}
	}
}
