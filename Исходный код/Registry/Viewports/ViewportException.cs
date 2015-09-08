using System;
using System.Runtime.Serialization;

namespace Registry.Viewport
{
    [Serializable]
    public class ViewportException : Exception
    {
        public ViewportException(string message)
            : base(message)
        {
        }

        public ViewportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ViewportException()
            : base()
        {
        }

        protected ViewportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
