using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TouchEffect.Enums;
using TouchEffect.EventArgs;
using Xamarin.Forms;

namespace TouchEffectSample.Issues
{
    public partial class AndroidTogglePage : ContentPage
    {
        public ICommand Command { get; }

        private readonly Dictionary<VisualElement, CancellationTokenSource> _animatedElements = new Dictionary<VisualElement, CancellationTokenSource>();

        public AndroidTogglePage()
        {
            var count = 0;
            Command = new Command(p =>
            {
                Title = $"TAPS COUNT: {++count}";
            });
            InitializeComponent();
        }

        private async void OnAnimStarted(VisualElement sender, AnimationStartedEventArgs args)
        {
            if (_animatedElements.ContainsKey(sender))
            {
                _animatedElements[sender].Cancel();
            }
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            _animatedElements[sender] = tokenSource;

            var image = (sender as ContentView).Content as Image;
            await Task.Delay(args.Duration / 2);
            if (token.IsCancellationRequested)
            {
                return;
            }

            image.Source = args.State == TouchState.Pressed
                ? "x.png"
                : "check.png";

            _animatedElements.Remove(sender);
        }
    }
}
