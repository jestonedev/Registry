using System;
using System.Globalization;
using System.Reflection;

namespace Registry.Viewport
{
    public static class ViewportFactory
    {
        public static Viewport CreateViewport<T>(IMenuCallback menuCallback) where T : Viewport
        {
            var viewport = typeof(T);
            return (T) Activator.CreateInstance(viewport, null, menuCallback);            
        }
    }
}
