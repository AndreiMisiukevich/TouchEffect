using System.Threading;
using System.Threading.Tasks;
using TouchEffect.Enums;
using TouchEffect.EventArgs;
using Xamarin.Forms;
using static System.Math;
using System.Collections.Generic;

namespace TouchEffect
{
    internal sealed class TouchVisualManager
    {
        private const string ChangeBackgroundColorAnimationName = nameof(ChangeBackgroundColorAnimationName);

        private CancellationTokenSource _animationTokenSource;

        private readonly HashSet<string> _skipTapHandlingPlatforms;

        private bool CanExecuteAction(ITouchEff sender) => sender.Control.IsEnabled && (sender.Command != null || sender.IsCompletedSet);

        internal TouchVisualManager(params string[] skipTapHandlingPlatforms)
            => _skipTapHandlingPlatforms = new HashSet<string>(skipTapHandlingPlatforms);

        public void HandleTouch(ITouchEff sender, TouchStatus status)
        {
            if (status != TouchStatus.Started || CanExecuteAction(sender))
            {
                sender.State = status == TouchStatus.Started
                  ? TouchState.Pressed
                  : TouchState.Regular;

                sender.Status = status;
                sender.RaiseStateChanged();
                sender.RaiseStatusChanged();
            }

            if (!_skipTapHandlingPlatforms.Contains(Device.RuntimePlatform) && status == TouchStatus.Completed)
            {
                OnTapped(sender);
            }
        }

        public async void ChangeStateAsync(ITouchEff sender, TouchState state, bool animated)
        {
            _animationTokenSource?.Cancel();
            _animationTokenSource = new CancellationTokenSource();
            var token = _animationTokenSource.Token;
            ViewExtensions.CancelAnimations(sender.Control);
            AnimationExtensions.AbortAnimation(sender.Control, ChangeBackgroundColorAnimationName);

            if(!animated)
            {
                await GetAnimationTask(sender, state, 0);
                return;
            }

            var rippleCount = sender.RippleCount;

            if (rippleCount == 0 || state == TouchState.Regular)
            {
                await GetAnimationTask(sender, state);
                return;
            }
            do
            {
                await GetAnimationTask(sender, TouchState.Pressed);
                if (token.IsCancellationRequested)
                {
                    return;
                }
                await GetAnimationTask(sender, TouchState.Regular);
                if (token.IsCancellationRequested)
                {
                    return;
                }
            } while (--rippleCount != 0);
        }

        internal void OnTapped(ITouchEff sender)
        {
            if (!CanExecuteAction(sender))
            {
                return;
            }
            sender.Command?.Execute(sender.CommandParameter);
            sender.RaiseCompleted();
        }

        private async Task SetBackgroundColorAsync(ITouchEff sender, TouchState state, int? forcedDuration)
        {
            var regularBackgroundColor = sender.RegularBackgroundColor;
            var pressedBackgroundColor = sender.PressedBackgroundColor;

            if (regularBackgroundColor == Color.Default &&
               pressedBackgroundColor == Color.Default)
            {
                return;
            }

            var color = regularBackgroundColor;
            var duration = sender.RegularAnimationDuration;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                color = pressedBackgroundColor;
                duration = sender.PressedAnimationDuration;
                easing = sender.PressedAnimationEasing;
            }

            if (duration <= 0)
            {
                sender.Control.BackgroundColor = color;
                return;
            }

            var animationCompletionSource = new TaskCompletionSource<bool>();
            duration = forcedDuration ?? duration;
            new Animation{
                {0, 1,  new Animation(v => sender.Control.BackgroundColor = new Color(v, sender.Control.BackgroundColor.G, sender.Control.BackgroundColor.B, sender.Control.BackgroundColor.A), sender.Control.BackgroundColor.R, color.R) },
                {0, 1,  new Animation(v => sender.Control.BackgroundColor = new Color(sender.Control.BackgroundColor.R, v, sender.Control.BackgroundColor.B, sender.Control.BackgroundColor.A), sender.Control.BackgroundColor.G, color.G) },
                {0, 1,  new Animation(v => sender.Control.BackgroundColor = new Color(sender.Control.BackgroundColor.R, sender.Control.BackgroundColor.G, v, sender.Control.BackgroundColor.A), sender.Control.BackgroundColor.B, color.B) },
                {0, 1,  new Animation(v => sender.Control.BackgroundColor = new Color(sender.Control.BackgroundColor.R, sender.Control.BackgroundColor.G, sender.Control.BackgroundColor.B, v), sender.Control.BackgroundColor.A, color.A) },
            }.Commit(sender.Control, ChangeBackgroundColorAnimationName, 16, (uint)duration, easing, (d, b) => animationCompletionSource.SetResult(true));
            await animationCompletionSource.Task;
        }

        private async Task SetOpacityAsync(ITouchEff sender, TouchState state, int? forcedDuration)
        {
            var regularOpacity = sender.RegularOpacity;
            var pressedOpacity = sender.PressedOpacity;

            if (Abs(regularOpacity - 1) <= double.Epsilon &&
               Abs(pressedOpacity - 1) <= double.Epsilon)
            {
                return;
            }

            var opacity = regularOpacity;
            var duration = sender.RegularAnimationDuration;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                opacity = pressedOpacity;
                duration = sender.PressedAnimationDuration;
                easing = sender.PressedAnimationEasing;
            }
            duration = forcedDuration ?? duration;
            await sender.Control.FadeTo(opacity, (uint)Abs(duration), easing);
        }

        private async Task SetScaleAsync(ITouchEff sender, TouchState state, int? forcedDuration)
        {
            var regularScale = sender.RegularScale;
            var pressedScale = sender.PressedScale;

            if (Abs(regularScale - 1) <= double.Epsilon &&
               Abs(pressedScale - 1) <= double.Epsilon)
            {
                return;
            }

            var scale = regularScale;
            var duration = sender.RegularAnimationDuration;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                scale = pressedScale;
                duration = sender.PressedAnimationDuration;
                easing = sender.PressedAnimationEasing;
            }
            duration = forcedDuration ?? duration;
            await sender.Control.ScaleTo(scale, (uint)Abs(duration), easing);
        }

        private async Task SetTranslationAsync(ITouchEff sender, TouchState state, int? forcedDuration)
        {
            var regularTranslationX = sender.RegularTranslationX;
            var pressedTranslationX = sender.PressedTranslationX;

            var regularTranslationY = sender.RegularTranslationY;
            var pressedTranslationY = sender.PressedTranslationY;

            if (Abs(regularTranslationX) <= double.Epsilon &&
                Abs(pressedTranslationX) <= double.Epsilon &&
                Abs(regularTranslationY) <= double.Epsilon &&
                Abs(pressedTranslationY) <= double.Epsilon)
            {
                return;
            }

            var translationX = regularTranslationX;
            var translationY = regularTranslationY;
            var duration = sender.RegularAnimationDuration;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                translationX = pressedTranslationX;
                translationY = pressedTranslationY;
                duration = sender.PressedAnimationDuration;
                easing = sender.PressedAnimationEasing;
            }
            duration = forcedDuration ?? duration;
            await sender.Control.TranslateTo(translationX, translationY, (uint)Abs(duration), easing);
        }

        private async Task SetRotationAsync(ITouchEff sender, TouchState state, int? forcedDuration)
        {
            var regularRotation = sender.RegularRotation;
            var pressedRotation = sender.PressedRotation;

            if (Abs(regularRotation) <= double.Epsilon &&
               Abs(pressedRotation) <= double.Epsilon)
            {
                return;
            }

            var rotation = regularRotation;
            var duration = sender.RegularAnimationDuration;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                rotation = pressedRotation;
                duration = sender.PressedAnimationDuration;
                easing = sender.PressedAnimationEasing;
            }
            duration = forcedDuration ?? duration;
            await sender.Control.RotateTo(rotation, (uint)Abs(duration), easing);
        }

        private async Task SetRotationXAsync(ITouchEff sender, TouchState state, int? forcedDuration)
        {
            var regularRotationX = sender.RegularRotationX;
            var pressedRotationX = sender.PressedRotationX;

            if (Abs(regularRotationX) <= double.Epsilon &&
               Abs(pressedRotationX) <= double.Epsilon)
            {
                return;
            }

            var rotationX = regularRotationX;
            var duration = sender.RegularAnimationDuration;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                rotationX = pressedRotationX;
                duration = sender.PressedAnimationDuration;
                easing = sender.PressedAnimationEasing;
            }
            duration = forcedDuration ?? duration;
            await sender.Control.RotateXTo(rotationX, (uint)Abs(duration), easing);
        }

        private async Task SetRotationYAsync(ITouchEff sender, TouchState state, int? forcedDuration)
        {
            var regularRotationY = sender.RegularRotationY;
            var pressedRotationY = sender.PressedRotationY;

            if (Abs(regularRotationY) <= double.Epsilon &&
               Abs(pressedRotationY) <= double.Epsilon)
            {
                return;
            }

            var rotationY = regularRotationY;
            var duration = sender.RegularAnimationDuration;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                rotationY = pressedRotationY;
                duration = sender.PressedAnimationDuration;
                easing = sender.PressedAnimationEasing;
            }
            duration = forcedDuration ?? duration;
            await sender.Control.RotateYTo(rotationY, (uint)Abs(duration), easing);
        }

        private Task GetAnimationTask(ITouchEff sender, TouchState state, int? forcedDuration = null)
            => Task.WhenAll(SetBackgroundColorAsync(sender, state, forcedDuration),
                SetOpacityAsync(sender, state, forcedDuration),
                SetScaleAsync(sender, state, forcedDuration),
                SetTranslationAsync(sender, state, forcedDuration),
                SetRotationAsync(sender, state, forcedDuration),
                SetRotationXAsync(sender, state, forcedDuration),
                SetRotationYAsync(sender, state, forcedDuration));
    }
}
