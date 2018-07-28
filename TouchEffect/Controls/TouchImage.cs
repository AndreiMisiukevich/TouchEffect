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
            default(ImageSource));

		public static readonly BindableProperty PressedSourceProperty = BindableProperty.Create(
            nameof(PressedSource),
            typeof(ImageSource),
            typeof(TouchView),
            default(ImageSource));

		public TouchImage()
        {
			var image = new Image
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Aspect = Aspect.AspectFit
			};

			Image = Content = image;

			StateChanged += (sender, args) =>
			{
				image.Source = args.State == TouchState.Regular
					? RegularSource ?? PressedSource
					: PressedSource ?? RegularSource;
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

		public new Image Content { get; } 

		public Image Image { get; }
        
	}
}
