using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Registry.Viewport
{
    internal static class ViewportHelper
    {
        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, control, new object[] { true });
        }
    }
}
