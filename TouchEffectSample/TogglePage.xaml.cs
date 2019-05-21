using System;
using System.Collections.Generic;

using Xamarin.Forms;
using TouchEffect;
using System.Windows.Input;
using TouchEffect.EventArgs;
using System.Threading.Tasks;
using TouchEffect.Enums;
using System.Threading;

namespace TouchEffectSample
{
    public partial class TogglePage : ContentPage
    {
        public ICommand Command { get; }

        private readonly Dictionary<VisualElement, CancellationTokenSource> _animatedElements = new Dictionary<VisualElement, CancellationTokenSource>();

        public TogglePage()
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
            if(_animatedElements.ContainsKey(sender))
            {
                _animatedElements[sender].Cancel();
            }
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            _animatedElements[sender] = tokenSource;

            var image = (sender as Frame).Content as Image;
            await Task.Delay(args.Duration / 2);
            if(token.IsCancellationRequested)
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
