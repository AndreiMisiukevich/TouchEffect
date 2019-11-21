﻿using Windows.UI.Xaml.Input;
using TouchEffect;
using TouchEffect.Enums;
using TouchEffect.Extensions;
using TouchEffect.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
using System;
using System.Collections.Generic;

[assembly: ResolutionGroupName(nameof(TouchEffect))]
[assembly: ExportEffect(typeof(PlatformTouchEff), nameof(TouchEff))]

namespace TouchEffect.UWP
{
    [Preserve(AllMembers = true)]
    public class PlatformTouchEff : PlatformEffect
    {
        public static void Preserve()
        {
        }

        private TouchEff _effect;

        private bool _pressed;
        private bool _intentionalCaptureLoss;

        private Storyboard _pointerDownStoryboard;
        private Storyboard _pointerUpStoryboard;

        protected override void OnAttached()
        {
            _effect = Element.GetTouchEff();
            _effect.Control = Element as VisualElement;
            _effect.ForceUpdateState(false);
            if (_effect.NativeAnimation && _effect.UWPTilt)
            {
                Debug.WriteLine("PLAY NATIVE ANIMATION!!");

                if (String.IsNullOrEmpty(Container.Name))
                    Container.Name = Guid.NewGuid().ToString();
                if (Container.Resources.ContainsKey("PointerDownAnimation"))
                    _pointerDownStoryboard = (Storyboard)Container.Resources["PointerDownAnimation"];
                else
                {
                    _pointerDownStoryboard = new Storyboard();
                    var downThemeAnimation = new PointerDownThemeAnimation();
                    Storyboard.SetTargetName(downThemeAnimation, Container.Name);
                    _pointerDownStoryboard.Children.Add(downThemeAnimation);
                    Container.Resources.Add(new KeyValuePair<object, object>("PointerDownAnimation", _pointerDownStoryboard));
                }
                if (Container.Resources.ContainsKey("PointerUpAnimation"))
                    _pointerUpStoryboard = (Storyboard)Container.Resources["PointerUpAnimation"];
                else
                {
                    _pointerUpStoryboard = new Storyboard();
                    var upThemeAnimation = new PointerUpThemeAnimation();
                    Storyboard.SetTargetName(upThemeAnimation, Container.Name);
                    _pointerUpStoryboard.Children.Add(upThemeAnimation);
                    Container.Resources.Add(new KeyValuePair<object, object>("PointerUpAnimation", _pointerUpStoryboard));
                }
            }

            if (Container != null)
            {
                Container.PointerPressed += OnPointerPressed;
                Container.PointerReleased += OnPointerReleased;
                Container.PointerCanceled += OnPointerCanceled;
                Container.PointerExited += OnPointerExited;
                Container.PointerEntered += OnPointerEntered;
                Container.PointerCaptureLost += OnPointerCaptureLost;
            }
        }

        protected override void OnDetached()
        {
            _effect.Control = null;
            _effect = null;
            if (Container != null)
            {
                Container.PointerPressed -= OnPointerPressed;
                Container.PointerReleased -= OnPointerReleased;
                Container.PointerCanceled -= OnPointerCanceled;
                Container.PointerExited -= OnPointerExited;
                Container.PointerEntered -= OnPointerEntered;
                Container.PointerCaptureLost -= OnPointerCaptureLost;

                _pressed = false;
            }
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (_pressed)
            {
                Element.GetTouchEff().HandleTouch(TouchStatus.Started);
                AnimateTilt(_pointerDownStoryboard);
            }
            Element.GetTouchEff().HandleHover(HoverStatus.Entered);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (_pressed)
            {
                Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                AnimateTilt(_pointerUpStoryboard);
            }
            Element.GetTouchEff().HandleHover(HoverStatus.Exited);
        }

        private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            _pressed = false;
            Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
            Element.GetTouchEff().HandleHover(HoverStatus.Exited);
            AnimateTilt(_pointerUpStoryboard);
        }

        private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (_intentionalCaptureLoss) return;
            _pressed = false;
            Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
            Element.GetTouchEff().HandleHover(HoverStatus.Exited);
            AnimateTilt(_pointerUpStoryboard);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {

            if(_pressed && (Element.GetTouchEff().HoverStatus == HoverStatus.Entered))
            {
                Element.GetTouchEff().HandleTouch(TouchStatus.Completed);
                AnimateTilt(_pointerUpStoryboard);
            }
            else if(Element.GetTouchEff().HoverStatus != HoverStatus.Exited)
            {
                Element.GetTouchEff().HandleTouch(TouchStatus.Canceled);
                AnimateTilt(_pointerUpStoryboard);
            }
               
            _pressed = false;
            _intentionalCaptureLoss = true;
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pressed = true;
            Container.CapturePointer(e.Pointer);
            Element.GetTouchEff().HandleTouch(TouchStatus.Started);
            AnimateTilt(_pointerDownStoryboard);
            _intentionalCaptureLoss = false;
        }

        private void AnimateTilt(Storyboard storyboard)
        {
            if (_effect.NativeAnimation && _effect.UWPTilt && storyboard != null) {
                storyboard.Stop();
                storyboard.Begin();
            }
        }
    }
}
