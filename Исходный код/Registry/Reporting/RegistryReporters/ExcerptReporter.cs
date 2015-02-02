using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Registry.Reporting.RegistryReporters
{
    public sealed class ExcerptReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\excerpt.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
