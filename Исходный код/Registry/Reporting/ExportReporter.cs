using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Registry.Reporting
{
    internal sealed class ExportReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "export.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
