using System;
using System.Globalization;
using System.Reflection;

namespace Registry.Viewport
{
    public static class ViewportFactory
    {
        public static Viewport CreateViewport<T>(IMenuCallback menuCallback) where T : Viewport
        {
            Type viewport = typeof(T);
            var constructorViewport = viewport.GetConstructor( new Type[] { typeof(Viewport), typeof(IMenuCallback) });
            //var constructorViewport = viewport.GetMethod(".ctor", BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance);
            //return (T) constructorViewport.Invoke(null, new object[] {  menuCallback });
            return (T) Activator.CreateInstance(viewport, new object[] { null, menuCallback });            
        }
    }
}
