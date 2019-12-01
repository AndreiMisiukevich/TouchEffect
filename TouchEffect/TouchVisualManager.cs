using System.Threading;
using System.Threading.Tasks;
using TouchEffect.Enums;
using Xamarin.Forms;
using static System.Math;
using TouchEffect.Extensions;
using System;

namespace TouchEffect
{
    internal sealed class TouchVisualManager
    {
        private const string ChangeBackgroundColorAnimationName = nameof(ChangeBackgroundColorAnimationName);

        private const int AnimationProgressDelay = 10;

        private CancellationTokenSource _animationTokenSource;

        private Func<TouchEff, TouchState, int, CancellationToken, Task> _customAnimationTaskGetter;

        private double? _durationMultiplier;

        private double _animationProgress;

        private TouchState _animationState;

        private bool CanExecuteAction(TouchEff sender)
            => sender.Control.IsEnabled && ((sender.Command?.CanExecute(sender.CommandParameter) ?? false) || sender.IsCompletedSet);

        internal void HandleTouch(TouchEff sender, TouchStatus status)
        {
            var canExecuteAction = CanExecuteAction(sender);
            if (status != TouchStatus.Started || canExecuteAction)
            {
                if (!canExecuteAction)
                {
                    status = TouchStatus.Canceled;
                }

                var state = status == TouchStatus.Started
                  ? TouchState.Pressed
                  : TouchState.Regular;

                if (status == TouchStatus.Started)
                {
                    _animationProgress = 0;
                    _animationState = state;
                }

                var isToggled = sender.IsToggled;
                if (isToggled.HasValue)
                {
                    if (status != TouchStatus.Started)
                    {
                        _durationMultiplier = (_animationState == TouchState.Pressed && !isToggled.Value) ||
                            (_animationState == TouchState.Regular && isToggled.Value)
                            ? 1 - _animationProgress
                            : _animationProgress;

                        sender.Status = status;
                        sender.RaiseStatusChanged();
                        if (status == TouchStatus.Canceled)
                        {
                            sender.ForceUpdateState(false);
                            return;
                        }
                        OnTapped(sender);
                        sender.IsToggled = !isToggled;
                        return;
                    }
                    state = isToggled.Value
                        ? TouchState.Regular
                        : TouchState.Pressed;

                    if (status == TouchStatus.Canceled)
                    {
                        state = state == TouchState.Pressed
                            ? TouchState.Regular
                            : TouchState.Pressed;
                    }
                }

                if (sender.State != state || status != TouchStatus.Canceled)
                {
                    sender.State = state;
                    sender.RaiseStateChanged();
                }
                sender.Status = status;
                sender.RaiseStatusChanged();
            }

            if (status == TouchStatus.Completed)
            {
                OnTapped(sender);
            }
        }

        internal void HandleHover(TouchEff sender, HoverStatus status)
        {
            if (!sender.Control.IsEnabled) {
                return;
            }

            var hoverState = status == HoverStatus.Entered
                ? HoverState.Hovering
                : HoverState.Regular;

            if(sender.HoverState != hoverState)
            {
                sender.HoverState = hoverState;
                sender.RaiseHoverStateChanged();
            }

            sender.HoverStatus = status;
            sender.RaiseHoverStatusChanged();
        }

        internal async void ChangeStateAsync(TouchEff sender, bool animated)
        {
            var state = sender.State;

            AbortAnimations(sender);
            _animationTokenSource = new CancellationTokenSource();
            var token = _animationTokenSource.Token;

            var isToggled = sender.IsToggled;

            if (!animated)
            {
                if (isToggled.HasValue)
                {
                    state = isToggled.Value
                        ? TouchState.Pressed
                        : TouchState.Regular;
                }
                var durationMultiplier = _durationMultiplier;
                _durationMultiplier = null;
                await GetAnimationTask(sender, state, durationMultiplier.GetValueOrDefault());
                return;
            }

            var rippleCount = sender.RippleCount;

            if (rippleCount == 0 || (state == TouchState.Regular && !isToggled.HasValue))
            {
                await GetAnimationTask(sender, state);
                return;
            }
            do
            {
                var rippleState = isToggled.HasValue && isToggled.Value
                    ? TouchState.Regular
                    : TouchState.Pressed;

                await GetAnimationTask(sender, rippleState);
                if (token.IsCancellationRequested)
                {
                    return;
                }

                rippleState = isToggled.HasValue && isToggled.Value
                    ? TouchState.Pressed
                    : TouchState.Regular;

                await GetAnimationTask(sender, rippleState);
                if (token.IsCancellationRequested)
                {
                    return;
                }
            } while (--rippleCount != 0);
        }

        internal void SetCustomAnimationTask(Func<TouchEff, TouchState, int, CancellationToken, Task> animationTaskGetter)
            => _customAnimationTaskGetter = animationTaskGetter;

        internal void OnTapped(TouchEff sender)
        {
            if (!CanExecuteAction(sender))
            {
                return;
            }
            sender.Command?.Execute(sender.CommandParameter);
            sender.RaiseCompleted();
        }

        internal void AbortAnimations(TouchEff sender)
        {
            _animationTokenSource?.Cancel();
            var control = sender.Control;
            if (control == null)
            {
                return;
            }
            ViewExtensions.CancelAnimations(control);
            AnimationExtensions.AbortAnimation(control, ChangeBackgroundColorAnimationName);
        }

        private async Task SetBackgroundColorAsync(TouchEff sender, TouchState state, int duration)
        {
            var regularBackgroundColor = sender.RegularBackgroundColor;
            var pressedBackgroundColor = sender.PressedBackgroundColor;

            if (regularBackgroundColor == Color.Default &&
               pressedBackgroundColor == Color.Default)
            {
                return;
            }

            var color = regularBackgroundColor;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                color = pressedBackgroundColor;
                easing = sender.PressedAnimationEasing;
            }

            var control = sender.Control;
            if (duration <= 0)
            {
                control.BackgroundColor = color;
                return;
            }

            var animationCompletionSource = new TaskCompletionSource<bool>();
            new Animation{
                {0, 1,  new Animation(v => control.BackgroundColor = new Color(v, control.BackgroundColor.G, control.BackgroundColor.B, control.BackgroundColor.A), control.BackgroundColor.R, color.R) },
                {0, 1,  new Animation(v => control.BackgroundColor = new Color(control.BackgroundColor.R, v, control.BackgroundColor.B, control.BackgroundColor.A), control.BackgroundColor.G, color.G) },
                {0, 1,  new Animation(v => control.BackgroundColor = new Color(control.BackgroundColor.R, control.BackgroundColor.G, v, control.BackgroundColor.A), control.BackgroundColor.B, color.B) },
                {0, 1,  new Animation(v => control.BackgroundColor = new Color(control.BackgroundColor.R, control.BackgroundColor.G, control.BackgroundColor.B, v), control.BackgroundColor.A, color.A) },
            }.Commit(sender.Control, ChangeBackgroundColorAnimationName, 16, (uint)duration, easing, (d, b) => animationCompletionSource.SetResult(true));
            await animationCompletionSource.Task;
        }

        private async Task SetOpacityAsync(TouchEff sender, TouchState state, int duration)
        {
            var regularOpacity = sender.RegularOpacity;
            var pressedOpacity = sender.PressedOpacity;

            if (Abs(regularOpacity - 1) <= double.Epsilon &&
               Abs(pressedOpacity - 1) <= double.Epsilon)
            {
                return;
            }

            var opacity = regularOpacity;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                opacity = pressedOpacity;
                easing = sender.PressedAnimationEasing;
            }
            await sender.Control.FadeTo(opacity, (uint)Abs(duration), easing);
        }

        private async Task SetScaleAsync(TouchEff sender, TouchState state, int duration)
        {
            var regularScale = sender.RegularScale;
            var pressedScale = sender.PressedScale;

            if (Abs(regularScale - 1) <= double.Epsilon &&
               Abs(pressedScale - 1) <= double.Epsilon)
            {
                return;
            }

            var scale = regularScale;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                scale = pressedScale;
                easing = sender.PressedAnimationEasing;
            }
            await sender.Control.ScaleTo(scale, (uint)Abs(duration), easing);
        }

        private async Task SetTranslationAsync(TouchEff sender, TouchState state, int duration)
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
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                translationX = pressedTranslationX;
                translationY = pressedTranslationY;
                easing = sender.PressedAnimationEasing;
            }
            await sender.Control.TranslateTo(translationX, translationY, (uint)Abs(duration), easing);
        }

        private async Task SetRotationAsync(TouchEff sender, TouchState state, int duration)
        {
            var regularRotation = sender.RegularRotation;
            var pressedRotation = sender.PressedRotation;

            if (Abs(regularRotation) <= double.Epsilon &&
               Abs(pressedRotation) <= double.Epsilon)
            {
                return;
            }

            var rotation = regularRotation;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                rotation = pressedRotation;
                easing = sender.PressedAnimationEasing;
            }
            await sender.Control.RotateTo(rotation, (uint)Abs(duration), easing);
        }

        private async Task SetRotationXAsync(TouchEff sender, TouchState state, int duration)
        {
            var regularRotationX = sender.RegularRotationX;
            var pressedRotationX = sender.PressedRotationX;

            if (Abs(regularRotationX) <= double.Epsilon &&
               Abs(pressedRotationX) <= double.Epsilon)
            {
                return;
            }

            var rotationX = regularRotationX;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                rotationX = pressedRotationX;
                easing = sender.PressedAnimationEasing;
            }
            await sender.Control.RotateXTo(rotationX, (uint)Abs(duration), easing);
        }

        private async Task SetRotationYAsync(TouchEff sender, TouchState state, int duration)
        {
            var regularRotationY = sender.RegularRotationY;
            var pressedRotationY = sender.PressedRotationY;

            if (Abs(regularRotationY) <= double.Epsilon &&
               Abs(pressedRotationY) <= double.Epsilon)
            {
                return;
            }

            var rotationY = regularRotationY;
            var easing = sender.RegularAnimationEasing;

            if (state == TouchState.Pressed)
            {
                rotationY = pressedRotationY;
                easing = sender.PressedAnimationEasing;
            }
            await sender.Control.RotateYTo(rotationY, (uint)Abs(duration), easing);
        }

        private Task GetAnimationTask(TouchEff sender, TouchState state, double? durationMultiplier = null)
        {
            if (sender.Control == null)
            {
                return Task.CompletedTask;
            }
            var token = _animationTokenSource.Token;
            var duration = (state == TouchState.Regular
                ? sender.RegularAnimationDuration
                : sender.PressedAnimationDuration).AdjustDurationMultiplier(durationMultiplier);

            if(duration <= 0 && Device.RuntimePlatform == Device.Android)
            {
                duration = 1;
            }

            sender.RaiseAnimationStarted(state, duration);
            return Task.WhenAll(
                _customAnimationTaskGetter?.Invoke(sender, state, duration, token) ?? Task.FromResult(true),
                SetBackgroundColorAsync(sender, state, duration),
                SetOpacityAsync(sender, state, duration),
                SetScaleAsync(sender, state, duration),
                SetTranslationAsync(sender, state, duration),
                SetRotationAsync(sender, state, duration),
                SetRotationXAsync(sender, state, duration),
                SetRotationYAsync(sender, state, duration),
                Task.Run(async () =>
                {
                    _animationProgress = 0;
                    _animationState = state;

                    for (var progress = AnimationProgressDelay; progress < duration; progress += AnimationProgressDelay)
                    {
                        await Task.Delay(AnimationProgressDelay).ConfigureAwait(false);
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }
                        _animationProgress = (double)progress / duration;
                    }
                    _animationProgress = 1;
                }));
        }
    }
}