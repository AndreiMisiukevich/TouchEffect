using TouchEffect.Enums;
using TouchEffect.EventArgs;
using Xamarin.Forms;

namespace TouchEffect.Controls
{
	public class TouchColorView : TouchView
	{
		public static readonly BindableProperty RegularColorProperty = BindableProperty.Create(
			nameof(RegularColor),
			typeof(Color),
			typeof(TouchView),
			default(Color),
			propertyChanged: (bindable, oldValue, newValue) =>
			{
				(bindable as TouchColorView)?.ForceStateChanged();
			});

		public static readonly BindableProperty PressedColorProperty = BindableProperty.Create(
			nameof(PressedColor),
			typeof(Color),
			typeof(TouchView),
			default(Color),
			propertyChanged: (bindable, oldValue, newValue) =>
			{
				(bindable as TouchColorView)?.ForceStateChanged();
			});

		public Color RegularColor
		{
			get => (Color)GetValue(RegularColorProperty);
			set => SetValue(RegularColorProperty, value);
		}

		public Color PressedColor
		{
			get => (Color)GetValue(PressedColorProperty);
			set => SetValue(PressedColorProperty, value);
		}

		protected override void OnStateChanged(TouchView sender, TouchStateChangedEventArgs args)
		{
			BackgroundColor = args.State == TouchState.Regular
					? GetRegularColor()
					: GetPressedColor();
		}

		private Color GetRegularColor()
		=> RegularColor != default(Color) ? RegularColor : PressedColor;

		private Color GetPressedColor()
		=> PressedColor != default(Color) ? PressedColor : RegularColor;
	}
}

