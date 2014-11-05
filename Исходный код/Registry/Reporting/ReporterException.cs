using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Reporting
{
    public class ReporterException : Exception
    {
        public ReporterException(string message) : base(message) { }
    }
}
