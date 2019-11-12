using System;
using TouchEffect.EventArgs;
using Xamarin.Forms;

namespace TouchEffect.Delegates
{
    [Obsolete]
    public delegate void TouchViewHoverStateChangedHandler(TouchView sender, HoverStateChangedEventArgs args);

    public delegate void TEffectHoverStateChangedHandler(VisualElement sender, HoverStateChangedEventArgs args);
}