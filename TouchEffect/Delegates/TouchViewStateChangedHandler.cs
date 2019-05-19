using System;
using TouchEffect.EventArgs;
using Xamarin.Forms;

namespace TouchEffect.Delegates
{
    [Obsolete]
    public delegate void TouchViewStateChangedHandler(TouchView sender, TouchStateChangedEventArgs args);

    public delegate void TEffectStateChangedHandler(VisualElement sender, TouchStateChangedEventArgs args);
}