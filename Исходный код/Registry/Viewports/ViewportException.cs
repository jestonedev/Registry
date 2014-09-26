using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Viewport
{
    public class ViewportException: Exception
    {
        public ViewportException(string message):base(message) {}
    }
}
