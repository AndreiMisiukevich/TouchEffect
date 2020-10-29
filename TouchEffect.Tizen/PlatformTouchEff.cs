using ElmSharp;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Tizen;
using TouchEffect;
using TouchEffect.Extensions;
using TouchEffect.Enums;
using TouchEffect.Tizen;
using EColor = ElmSharp.Color;

[assembly: ResolutionGroupName(nameof(TouchEffect))]
[assembly: ExportEffect(typeof(PlatformTouchEff), nameof(TouchEff))]
namespace TouchEffect.Tizen
{
    [Preserve(AllMembers = true)]
    public class PlatformTouchEff : PlatformEffect
    {
        public static void Preserve() { }

        private GestureLayer _gestureLayer;
        private TouchEff _effect;

        protected override void OnAttached()
        {
            _effect = Element.PickTouchEff();
            if (_effect?.IsDisabled ?? true) return;

            _effect.Control = Element as VisualElement;
            _gestureLayer = new TouchTapGestureRecognizer(Control, _effect);
        }

        protected override void OnDetached()
        {
            if (_effect?.Control == null) return;

            if (_gestureLayer != null)
            {
                _gestureLayer.ClearCallbacks();
                _gestureLayer.Unrealize();
                _gestureLayer = null;
            }
            _effect.Control = null;
            _effect = null;
        }
    }

    internal sealed class TouchTapGestureRecognizer : GestureLayer
    {
        private TouchEff _effect;
        private bool _tapCompleted;
        private bool _longTapStarted;

        public TouchTapGestureRecognizer(EvasObject parent) : base(parent)
        {
            SetTapCallback(GestureType.Tap, GestureLayer.GestureState.Start, (data) => { OnTapStarted(data); });
            SetTapCallback(GestureType.Tap, GestureLayer.GestureState.End, (data) => { OnGestureEnded(data); });
            SetTapCallback(GestureType.Tap, GestureLayer.GestureState.Abort, (data) => { OnGestureAborted(data); });

            SetTapCallback(GestureType.LongTap, GestureLayer.GestureState.Start, (data) => { OnLongTapStarted(data); });
            SetTapCallback(GestureType.LongTap, GestureLayer.GestureState.End, (data) => { OnGestureEnded(data); });
            SetTapCallback(GestureType.LongTap, GestureLayer.GestureState.Abort, (data) => { OnGestureAborted(data); });
        }

        public TouchTapGestureRecognizer(EvasObject parent, TouchEff effect) : this(parent)
        {
            Attach(parent);
            _effect = effect;
        }

        public bool IsCanceled { get; set; } = true;

        private void OnTapStarted(object data)
        {
            if (_effect?.IsDisabled ?? true) return;

            IsCanceled = false;
            HandleTouch(TouchStatus.Started, UserInteractionState.Running);
        }

        private void OnLongTapStarted(object data)
        {
            if (_effect?.IsDisabled ?? true) return;

            IsCanceled = false;

            _longTapStarted = true;
            HandleTouch(TouchStatus.Started, UserInteractionState.Running);
        }

        private void OnGestureEnded(object data)
        {
            if (_effect?.IsDisabled ?? true) return;

            HandleTouch(_effect?.Status == TouchStatus.Started ? TouchStatus.Completed : TouchStatus.Canceled, UserInteractionState.Idle);
            IsCanceled = true;
            _tapCompleted = true;
        }

        private void OnGestureAborted(object data)
        {
            if (_effect?.IsDisabled ?? true) return;

            if (_tapCompleted || _longTapStarted)
            {
                _tapCompleted = false;
                _longTapStarted = false;
                return;
            }

            HandleTouch(TouchStatus.Canceled, UserInteractionState.Idle);
            IsCanceled = true;
        }

        public void HandleTouch(TouchStatus status, UserInteractionState? userInteractionState = null)
        {
            if (IsCanceled || _effect == null) return;

            if (_effect?.IsDisabled ?? true) return;

           _effect.HandleTouch(status);
            if (userInteractionState.HasValue)
            {
                _effect.HandleUserInteraction(userInteractionState.Value);
            }

            if (!_effect.NativeAnimation) return;

            if (_longTapStarted && !_tapCompleted) return;

            var control = _effect.Control;
            var nativeView = Platform.GetOrCreateRenderer(control)?.NativeView as Widget;
            if (nativeView == null) return;

            if (status == TouchStatus.Started)
            {
                var startColor = nativeView.BackgroundColor;
                if (startColor.IsDefault)
                    return;

                var endColor = _effect.NativeAnimationColor.ToNative(); ;
                if (endColor.IsDefault)
                {
                    startColor = EColor.FromRgba(startColor.R, startColor.G, startColor.B, startColor.A/2);
                    endColor = startColor;
                }

                Transit transit = new Transit
                {
                    Repeat = 1,
                    Duration = .2
                };
                var colorEffect = new ColorEffect(startColor, endColor);
                colorEffect.EffectEnded += (s, e) => { transit?.Dispose(); };
                transit.Objects.Add(nativeView);
                transit.AddEffect(colorEffect);
                transit.Go(.2);
            }
        }
    }
}
