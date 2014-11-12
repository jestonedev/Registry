using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyAgreementReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\agreement.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
