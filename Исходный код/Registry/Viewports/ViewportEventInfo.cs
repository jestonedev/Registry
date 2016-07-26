using System;
using System.Reflection;

namespace Registry.Viewport
{
    internal sealed class ViewportEventInfo
    {
        public object Sender { get; set; }
        public EventInfo EventInfo { get; set; }

        public Delegate Handler { get; set; }
    }
}
