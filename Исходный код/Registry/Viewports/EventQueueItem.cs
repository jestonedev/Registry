using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Registry.Viewport
{
    internal class EventQueueItem
    {
        public EventInfo EventInfo { get; set; }
        public object Target { get; set; }
        public Delegate Handler { get; set; }
    }
}
