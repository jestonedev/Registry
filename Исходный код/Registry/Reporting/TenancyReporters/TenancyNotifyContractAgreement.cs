using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal class TenancyNotifyContractAgreement : Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "Уведомление";
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\notify_contract_agreement.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
