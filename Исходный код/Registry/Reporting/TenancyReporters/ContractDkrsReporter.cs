using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class ContractDkrsReporter : Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "Договор (ДКСР)";
            arguments.Add("config",
                Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\contract_dksr.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
