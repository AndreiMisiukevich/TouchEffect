using System;
using TouchEffect.EventArgs;
using Xamarin.Forms;

namespace TouchEffect.Delegates
{
    [Obsolete]
    public delegate void TouchViewHoverStatusChangedHandler(TouchView sender, HoverStatusChangedEventArgs args);

    public delegate void TEffectHoverStatusChangedHandler(VisualElement sender, HoverStatusChangedEventArgs args);
}