using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyContractSpecial1711Reporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\contract_special_1711.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
