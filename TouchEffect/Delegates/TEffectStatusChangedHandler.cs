using System;
using TouchEffect.EventArgs;
using Xamarin.Forms;

namespace TouchEffect.Delegates
{
    [Obsolete]
    public delegate void TouchViewStatusChangedHandler(TouchView sender, TouchStatusChangedEventArgs args);

    public delegate void TEffectStatusChangedHandler(VisualElement sender, TouchStatusChangedEventArgs args);
}