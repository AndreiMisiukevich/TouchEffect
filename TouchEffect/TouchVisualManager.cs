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

        private CancellationTokenSource _longPressTokenSource;

        private CancellationTokenSource _animationTokenSource;

        private Func<TouchEff, TouchState, HoverState, int, CancellationToken, Task> _customAnimationTaskGetter;

        private double? _durationMultiplier;

        private double _animationProgress;

        private TouchState _animationState;

        internal void HandleTouch(TouchEff sender, TouchStatus status)
        {
            if (sender.IsDisabled)
            {
                return;
            }

            var canExecuteAction = sender.CanExecute;
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

                        UpdateStatusAndState(sender, status, state);
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
                }

                UpdateStatusAndState(sender, status, state);
            }

            if (status == TouchStatus.Completed)
            {
                OnTapped(sender);
            }
        }

        internal void HandleHover(TouchEff sender, HoverStatus status)
        {
            if (!sender.Control.IsEnabled)
            {
                return;
            }

            var hoverState = status == HoverStatus.Entered
                ? HoverState.Hovering
                : HoverState.Regular;

            if (sender.HoverState != hoverState)
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
            var hoverState = sender.HoverState;

            AbortAnimations(sender);
            _animationTokenSource = new CancellationTokenSource();
            var token = _animationTokenSource.Token;

            var isToggled = sender.IsToggled;

            UpdateVisualState(sender.Control, state, hoverState);

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
                await GetAnimationTask(sender, state, hoverState, durationMultiplier.GetValueOrDefault());
                return;
            }

            var rippleCount = sender.RippleCount;

            if (rippleCount == 0 || (state == TouchState.Regular && !isToggled.HasValue))
            {
                await GetAnimationTask(sender, state, hoverState);
                return;
            }
            do
            {
                var rippleState = isToggled.HasValue && isToggled.Value
                    ? TouchState.Regular
                    : TouchState.Pressed;

                await GetAnimationTask(sender, rippleState, hoverState);
                if (token.IsCancellationRequested)
                {
                    return;
                }

                rippleState = isToggled.HasValue && isToggled.Value
                    ? TouchState.Pressed
                    : TouchState.Regular;

                await GetAnimationTask(sender, rippleState, hoverState);
                if (token.IsCancellationRequested)
                {
                    return;
                }
            } while (--rippleCount != 0);
        }

        internal void HandleLongPress(TouchEff sender)
        {
            if (sender.State == TouchState.Regular)
            {
                _longPressTokenSource?.Cancel();
                _longPressTokenSource?.Dispose();
                _longPressTokenSource = null;
                return;
            }

            if (sender.LongPressCommand == null || sender.UserInteractionState == UserInteractionState.Idle)
                return;

            _longPressTokenSource = new CancellationTokenSource();
            Task.Delay(sender.LongPressDuration, _longPressTokenSource.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                sender.HandleUserInteraction(UserInteractionState.Idle);
                var involeLongPressCommand = new Action(() => sender.LongPressCommand?.Execute(sender.LongPressCommandParameter));
                if (Device.IsInvokeRequired)
                {
                    Device.BeginInvokeOnMainThread(involeLongPressCommand);
                    return;
                }
                involeLongPressCommand.Invoke();
            });
        }

        internal void SetCustomAnimationTask(Func<TouchEff, TouchState, HoverState, int, CancellationToken, Task> animationTaskGetter)
            => _customAnimationTaskGetter = animationTaskGetter;

        internal void OnTapped(TouchEff sender)
        {
            if (!sender.CanExecute || (sender.LongPressCommand != null && sender.UserInteractionState == UserInteractionState.Idle))
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

        private void UpdateStatusAndState(TouchEff sender, TouchStatus status, TouchState state)
        {
            if (sender.State != state || status != TouchStatus.Canceled)
            {
                sender.State = state;
                sender.RaiseStateChanged();
            }
            sender.Status = status;
            sender.RaiseStatusChanged();
        }

        private void UpdateVisualState(VisualElement visualElement, TouchState touchState, HoverState hoverState)
        {
            var state = touchState == TouchState.Pressed
                ? nameof(TouchState.Pressed)
                : hoverState == HoverState.Hovering
                    ? nameof(HoverState.Hovering)
                    : nameof(TouchState.Regular);

            VisualStateManager.GoToState(visualElement, state);
        }

        private async Task SetBackgroundColorAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration)
        {
            var regularBackgroundColor = sender.RegularBackgroundColor;
            var pressedBackgroundColor = sender.PressedBackgroundColor;
            var hoveredBackgroundColor = sender.HoveredBackgroundColor;

            if (regularBackgroundColor == Color.Default &&
                pressedBackgroundColor == Color.Default &&
                hoveredBackgroundColor == Color.Default)
            {
                return;
            }

            var color = regularBackgroundColor;
            var easing = sender.RegularAnimationEasing;

            if (touchState == TouchState.Pressed)
            {
                color = pressedBackgroundColor;
                easing = sender.PressedAnimationEasing;
            }
            else if (hoverState == HoverState.Hovering)
            {
                color = hoveredBackgroundColor;
                easing = sender.HoveredAnimationEasing;
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

        private async Task SetOpacityAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration)
        {
            var regularOpacity = sender.RegularOpacity;
            var pressedOpacity = sender.PressedOpacity;
            var hoveredOpacity = sender.HoveredOpacity;

            if (Abs(regularOpacity - 1) <= double.Epsilon &&
                Abs(pressedOpacity - 1) <= double.Epsilon &&
                Abs(hoveredOpacity - 1) <= double.Epsilon)
            {
                return;
            }

            var opacity = regularOpacity;
            var easing = sender.RegularAnimationEasing;

            if (touchState == TouchState.Pressed)
            {
                opacity = pressedOpacity;
                easing = sender.PressedAnimationEasing;
            }
            else if (hoverState == HoverState.Hovering)
            {
                opacity = hoveredOpacity;
                easing = sender.HoveredAnimationEasing;
            }

            await sender.Control.FadeTo(opacity, (uint)Abs(duration), easing);
        }

        private async Task SetScaleAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration)
        {
            var regularScale = sender.RegularScale;
            var pressedScale = sender.PressedScale;
            var hoveredScale = sender.HoveredScale;

            if (Abs(regularScale - 1) <= double.Epsilon &&
                Abs(pressedScale - 1) <= double.Epsilon &&
                Abs(hoveredScale - 1) <= double.Epsilon)
            {
                return;
            }

            var scale = regularScale;
            var easing = sender.RegularAnimationEasing;

            if (touchState == TouchState.Pressed)
            {
                scale = pressedScale;
                easing = sender.PressedAnimationEasing;
            }
            else if (hoverState == HoverState.Hovering)
            {
                scale = hoveredScale;
                easing = sender.HoveredAnimationEasing;
            }

            var control = sender.Control;
            var tcs = new TaskCompletionSource<bool>();
            control.Animate($"{nameof(SetScaleAsync)}{control.Id}", v =>
            {
                if (double.IsNaN(v))
                {
                    return;
                }
                control.Scale = v;
            }, control.Scale, scale, 16, (uint)Abs(duration), easing, (v, b) => tcs.SetResult(b));
            await tcs.Task;
        }

        private async Task SetTranslationAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration)
        {
            var regularTranslationX = sender.RegularTranslationX;
            var pressedTranslationX = sender.PressedTranslationX;
            var hoveredTranslationX = sender.HoveredTranslationX;

            var regularTranslationY = sender.RegularTranslationY;
            var pressedTranslationY = sender.PressedTranslationY;
            var hoveredTranslationY = sender.HoveredTranslationY;

            if (Abs(regularTranslationX) <= double.Epsilon &&
                Abs(pressedTranslationX) <= double.Epsilon &&
                Abs(hoveredTranslationX) <= double.Epsilon &&
                Abs(regularTranslationY) <= double.Epsilon &&
                Abs(pressedTranslationY) <= double.Epsilon &&
                Abs(hoveredTranslationY) <= double.Epsilon)
            {
                return;
            }

            var translationX = regularTranslationX;
            var translationY = regularTranslationY;
            var easing = sender.RegularAnimationEasing;

            if (touchState == TouchState.Pressed)
            {
                translationX = pressedTranslationX;
                translationY = pressedTranslationY;
                easing = sender.PressedAnimationEasing;
            }
            else if (hoverState == HoverState.Hovering)
            {
                translationX = hoveredTranslationX;
                translationY = hoveredTranslationY;
                easing = sender.HoveredAnimationEasing;
            }

            await sender.Control.TranslateTo(translationX, translationY, (uint)Abs(duration), easing);
        }

        private async Task SetRotationAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration)
        {
            var regularRotation = sender.RegularRotation;
            var pressedRotation = sender.PressedRotation;
            var hoveredRotation = sender.HoveredRotation;

            if (Abs(regularRotation) <= double.Epsilon &&
                Abs(pressedRotation) <= double.Epsilon &&
                Abs(hoveredRotation) <= double.Epsilon)
            {
                return;
            }

            var rotation = regularRotation;
            var easing = sender.RegularAnimationEasing;

            if (touchState == TouchState.Pressed)
            {
                rotation = pressedRotation;
                easing = sender.PressedAnimationEasing;
            }
            else if (hoverState == HoverState.Hovering)
            {
                rotation = hoveredRotation;
                easing = sender.HoveredAnimationEasing;
            }

            await sender.Control.RotateTo(rotation, (uint)Abs(duration), easing);
        }

        private async Task SetRotationXAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration)
        {
            var regularRotationX = sender.RegularRotationX;
            var pressedRotationX = sender.PressedRotationX;
            var hoveredRotationX = sender.HoveredRotationX;

            if (Abs(regularRotationX) <= double.Epsilon &&
                Abs(pressedRotationX) <= double.Epsilon &&
                Abs(hoveredRotationX) <= double.Epsilon)
            {
                return;
            }

            var rotationX = regularRotationX;
            var easing = sender.RegularAnimationEasing;

            if (touchState == TouchState.Pressed)
            {
                rotationX = pressedRotationX;
                easing = sender.PressedAnimationEasing;
            }
            else if (hoverState == HoverState.Hovering)
            {
                rotationX = hoveredRotationX;
                easing = sender.HoveredAnimationEasing;
            }

            await sender.Control.RotateXTo(rotationX, (uint)Abs(duration), easing);
        }

        private async Task SetRotationYAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration)
        {
            var regularRotationY = sender.RegularRotationY;
            var pressedRotationY = sender.PressedRotationY;
            var hoveredRotationY = sender.HoveredRotationY;

            if (Abs(regularRotationY) <= double.Epsilon &&
                Abs(pressedRotationY) <= double.Epsilon &&
                Abs(hoveredRotationY) <= double.Epsilon)
            {
                return;
            }

            var rotationY = regularRotationY;
            var easing = sender.RegularAnimationEasing;

            if (touchState == TouchState.Pressed)
            {
                rotationY = pressedRotationY;
                easing = sender.PressedAnimationEasing;
            }
            else if (hoverState == HoverState.Hovering)
            {
                rotationY = hoveredRotationY;
                easing = sender.HoveredAnimationEasing;
            }

            await sender.Control.RotateYTo(rotationY, (uint)Abs(duration), easing);
        }

        private Task GetAnimationTask(TouchEff sender, TouchState touchState, HoverState hoverState, double? durationMultiplier = null)
        {
            if (sender.Control == null)
            {
                return Task.CompletedTask;
            }
            var token = _animationTokenSource.Token;
            var duration = sender.RegularAnimationDuration;

            if (touchState == TouchState.Pressed)
            {
                duration = sender.RegularAnimationDuration;
            }
            else if (hoverState == HoverState.Hovering)
            {
                duration = sender.HoveredAnimationDuration;
            }
            duration = duration.AdjustDurationMultiplier(durationMultiplier);

            if (duration <= 0 && (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.UWP))
            {
                duration = 1;
            }

            sender.RaiseAnimationStarted(touchState, hoverState, duration);
            return Task.WhenAll(
                _customAnimationTaskGetter?.Invoke(sender, touchState, hoverState, duration, token) ?? Task.FromResult(true),
                SetBackgroundColorAsync(sender, touchState, hoverState, duration),
                SetOpacityAsync(sender, touchState, hoverState, duration),
                SetScaleAsync(sender, touchState, hoverState, duration),
                SetTranslationAsync(sender, touchState, hoverState, duration),
                SetRotationAsync(sender, touchState, hoverState, duration),
                SetRotationXAsync(sender, touchState, hoverState, duration),
                SetRotationYAsync(sender, touchState, hoverState, duration),
                Task.Run(async () =>
                {
                    _animationProgress = 0;
                    _animationState = touchState;

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