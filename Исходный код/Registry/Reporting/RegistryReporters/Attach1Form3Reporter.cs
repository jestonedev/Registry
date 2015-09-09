using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Registry.Reporting.RegistryReporters
{
    public class Attach1Form3Reporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\attach_1_form_3.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
