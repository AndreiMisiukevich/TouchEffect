using TouchEffect.EventArgs;
using Xamarin.Forms;
using System;

namespace TouchEffect.Delegates
{
    [Obsolete]
    public delegate void TouchViewCompletedHandler(TouchView sender, TouchCompletedEventArgs args);

    public delegate void TEffectCompletedHandler(VisualElement sender, TouchCompletedEventArgs args);
}