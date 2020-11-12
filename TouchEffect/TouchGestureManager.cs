using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Math;
using System;

namespace TouchEffect
{
    sealed class TouchGestureManager
    {
        const string ChangeBackgroundColorAnimationName = nameof(ChangeBackgroundColorAnimationName);

        const int AnimationProgressDelay = 10;

        Color defaultBackgroundColor;

        CancellationTokenSource longPressTokenSource;

        CancellationTokenSource animationTokenSource;

        Func<TouchEff, TouchState, HoverState, int, Easing, CancellationToken, Task> animationTaskFactory;

        double? durationMultiplier;

        double animationProgress;

        TouchState animationState;

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
                  : TouchState.Normal;

                if (status == TouchStatus.Started)
                {
                    animationProgress = 0;
                    animationState = state;
                }

                var isToggled = sender.IsToggled;
                if (isToggled.HasValue)
                {
                    if (status != TouchStatus.Started)
                    {
                        durationMultiplier = (animationState == TouchState.Pressed && !isToggled.Value) ||
                            (animationState == TouchState.Normal && isToggled.Value)
                            ? 1 - animationProgress
                            : animationProgress;

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
                        ? TouchState.Normal
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
                ? HoverState.Hovered
                : HoverState.Normal;

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
            animationTokenSource = new CancellationTokenSource();
            var token = animationTokenSource.Token;

            var isToggled = sender.IsToggled;

            UpdateVisualState(sender.Control, state, hoverState);

            if (!animated)
            {
                if (isToggled.HasValue)
                {
                    state = isToggled.Value
                        ? TouchState.Pressed
                        : TouchState.Normal;
                }
                var durationMultiplier = this.durationMultiplier;
                this.durationMultiplier = null;
                await GetAnimationTask(sender, state, hoverState, durationMultiplier.GetValueOrDefault());
                return;
            }

            var rippleCount = sender.RippleCount;

            if (rippleCount == 0 || (state == TouchState.Normal && !isToggled.HasValue))
            {
                await GetAnimationTask(sender, state, hoverState);
                return;
            }
            do
            {
                var rippleState = isToggled.HasValue && isToggled.Value
                    ? TouchState.Normal
                    : TouchState.Pressed;

                await GetAnimationTask(sender, rippleState, hoverState);
                if (token.IsCancellationRequested)
                {
                    return;
                }

                rippleState = isToggled.HasValue && isToggled.Value
                    ? TouchState.Pressed
                    : TouchState.Normal;

                await GetAnimationTask(sender, rippleState, hoverState);
                if (token.IsCancellationRequested)
                {
                    return;
                }
            } while (--rippleCount != 0);
        }

        internal void HandleLongPress(TouchEff sender)
        {
            if (sender.State == TouchState.Normal)
            {
                longPressTokenSource?.Cancel();
                longPressTokenSource?.Dispose();
                longPressTokenSource = null;
                return;
            }

            if (sender.LongPressCommand == null || sender.UserInteractionState == TouchInteractionStatus.Completed)
                return;

            longPressTokenSource = new CancellationTokenSource();
            Task.Delay(sender.LongPressDuration, longPressTokenSource.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                sender.HandleUserInteraction(TouchInteractionStatus.Completed);
                var executeLongPressCommand = new Action(() => sender.LongPressCommand?.Execute(sender.LongPressCommandParameter ?? sender.CommandParameter));
                if (Device.IsInvokeRequired)
                    executeLongPressCommand = () => Device.BeginInvokeOnMainThread(executeLongPressCommand);

                executeLongPressCommand.Invoke();
            });
        }

        internal void SetCustomAnimationTask(Func<TouchEff, TouchState, HoverState, int, Easing, CancellationToken, Task> animationTaskFactory)
            => this.animationTaskFactory = animationTaskFactory;

        internal void Reset()
        {
            SetCustomAnimationTask(null);
            defaultBackgroundColor = default(Color);
        }

        internal void OnTapped(TouchEff sender)
        {
            if (!sender.CanExecute || (sender.LongPressCommand != null && sender.UserInteractionState == TouchInteractionStatus.Completed))
            {
                return;
            }
            sender.Command?.Execute(sender.CommandParameter);
            sender.RaiseCompleted();
        }

        internal void AbortAnimations(TouchEff sender)
        {
            animationTokenSource?.Cancel();
            animationTokenSource?.Dispose();
            animationTokenSource = null;
            var control = sender.Control;
            if (control == null)
            {
                return;
            }
            ViewExtensions.CancelAnimations(control);
            AnimationExtensions.AbortAnimation(control, ChangeBackgroundColorAnimationName);
        }

        void UpdateStatusAndState(TouchEff sender, TouchStatus status, TouchState state)
        {
            if (sender.State != state || status != TouchStatus.Canceled)
            {
                sender.State = state;
                sender.RaiseStateChanged();
            }
            sender.Status = status;
            sender.RaiseStatusChanged();
        }

        void UpdateVisualState(VisualElement visualElement, TouchState touchState, HoverState hoverState)
        {
            var state = touchState == TouchState.Pressed
                ? nameof(TouchState.Pressed)
                : hoverState == HoverState.Hovered
                    ? nameof(HoverState.Hovered)
                    : nameof(TouchState.Normal);

            VisualStateManager.GoToState(visualElement, state);
        }

        async Task SetBackgroundImageAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration, CancellationToken token)
        {
            var regularBackgroundImageSource = sender.RegularBackgroundImageSource;
            var pressedBackgroundImageSource = sender.PressedBackgroundImageSource;
            var hoveredBackgroundImageSource = sender.HoveredBackgroundImageSource;

            if (regularBackgroundImageSource == null &&
                pressedBackgroundImageSource == null &&
                hoveredBackgroundImageSource == null)
            {
                return;
            }

            var aspect = sender.RegularBackgroundImageAspect;
            var source = regularBackgroundImageSource;
            if (touchState == TouchState.Pressed)
            {
                aspect = sender.PressedBackgroundImageAspect;
                source = pressedBackgroundImageSource;
            }
            else if (hoverState == HoverState.Hovered)
            {
                aspect = sender.HoveredBackgroundImageAspect;
                source = hoveredBackgroundImageSource;
            }

            try
            {
                if (sender.ShouldSetImageOnAnimationEnd)
                {
                    await Task.Delay(duration, token);
                }
            }
            catch (TaskCanceledException)
            {
                return;
            }

            if (sender.Control is Image image)
            {
                image.BatchBegin();
                image.Aspect = aspect;
                image.Source = source;
                image.BatchCommit();
            }
        }

        async Task SetBackgroundColorAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration, Easing easing)
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

            var control = sender.Control;
            if (defaultBackgroundColor == default(Color))
                defaultBackgroundColor = control.BackgroundColor;

            var color = GetBackgroundColor(regularBackgroundColor);

            if (touchState == TouchState.Pressed)
            {
                color = GetBackgroundColor(pressedBackgroundColor);
            }
            else if (hoverState == HoverState.Hovered)
            {
                color = GetBackgroundColor(hoveredBackgroundColor);
            }

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

        async Task SetOpacityAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration, Easing easing)
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

            if (touchState == TouchState.Pressed)
            {
                opacity = pressedOpacity;
            }
            else if (hoverState == HoverState.Hovered)
            {
                opacity = hoveredOpacity;
            }

            await sender.Control.FadeTo(opacity, (uint)Abs(duration), easing);
        }

        async Task SetScaleAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration, Easing easing)
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

            if (touchState == TouchState.Pressed)
            {
                scale = pressedScale;
            }
            else if (hoverState == HoverState.Hovered)
            {
                scale = hoveredScale;
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

        async Task SetTranslationAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration, Easing easing)
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

            if (touchState == TouchState.Pressed)
            {
                translationX = pressedTranslationX;
                translationY = pressedTranslationY;
            }
            else if (hoverState == HoverState.Hovered)
            {
                translationX = hoveredTranslationX;
                translationY = hoveredTranslationY;
            }

            await sender.Control.TranslateTo(translationX, translationY, (uint)Abs(duration), easing);
        }

        async Task SetRotationAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration, Easing easing)
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

            if (touchState == TouchState.Pressed)
            {
                rotation = pressedRotation;
            }
            else if (hoverState == HoverState.Hovered)
            {
                rotation = hoveredRotation;
            }

            await sender.Control.RotateTo(rotation, (uint)Abs(duration), easing);
        }

        async Task SetRotationXAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration, Easing easing)
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

            if (touchState == TouchState.Pressed)
            {
                rotationX = pressedRotationX;
            }
            else if (hoverState == HoverState.Hovered)
            {
                rotationX = hoveredRotationX;
            }

            await sender.Control.RotateXTo(rotationX, (uint)Abs(duration), easing);
        }

        async Task SetRotationYAsync(TouchEff sender, TouchState touchState, HoverState hoverState, int duration, Easing easing)
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

            if (touchState == TouchState.Pressed)
            {
                rotationY = pressedRotationY;
            }
            else if (hoverState == HoverState.Hovered)
            {
                rotationY = hoveredRotationY;
            }

            await sender.Control.RotateYTo(rotationY, (uint)Abs(duration), easing);
        }

        Color GetBackgroundColor(Color color)
            => color != Color.Default
                ? color
                : defaultBackgroundColor;

        Task GetAnimationTask(TouchEff sender, TouchState touchState, HoverState hoverState, double? durationMultiplier = null)
        {
            if (sender.Control == null)
            {
                return Task.CompletedTask;
            }
            var token = animationTokenSource.Token;
            var duration = sender.RegularAnimationDuration;
            var easing = sender.RegularAnimationEasing;

            if (touchState == TouchState.Pressed)
            {
                duration = sender.RegularAnimationDuration;
                easing = sender.PressedAnimationEasing;
            }
            else if (hoverState == HoverState.Hovered)
            {
                duration = sender.HoveredAnimationDuration;
                easing = sender.HoveredAnimationEasing;
            }
            duration = duration.AdjustDurationMultiplier(durationMultiplier);

            if (duration <= 0 &&
                Device.RuntimePlatform != Device.iOS &&
                Device.RuntimePlatform != Device.macOS)
            {
                duration = 1;
            }

            return Task.WhenAll(
                animationTaskFactory?.Invoke(sender, touchState, hoverState, duration, easing, token) ?? Task.FromResult(true),
                SetBackgroundImageAsync(sender, touchState, hoverState, duration, token),
                SetBackgroundColorAsync(sender, touchState, hoverState, duration, easing),
                SetOpacityAsync(sender, touchState, hoverState, duration, easing),
                SetScaleAsync(sender, touchState, hoverState, duration, easing),
                SetTranslationAsync(sender, touchState, hoverState, duration, easing),
                SetRotationAsync(sender, touchState, hoverState, duration, easing),
                SetRotationXAsync(sender, touchState, hoverState, duration, easing),
                SetRotationYAsync(sender, touchState, hoverState, duration, easing),
                Task.Run(async () =>
                {
                    animationProgress = 0;
                    animationState = touchState;

                    for (var progress = AnimationProgressDelay; progress < duration; progress += AnimationProgressDelay)
                    {
                        await Task.Delay(AnimationProgressDelay).ConfigureAwait(false);
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }
                        animationProgress = (double)progress / duration;
                    }
                    animationProgress = 1;
                }));
        }
    }
}