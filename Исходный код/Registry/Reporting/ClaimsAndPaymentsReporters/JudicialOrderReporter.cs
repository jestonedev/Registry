using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.ClaimsAndPaymentsReporters
{
    public sealed class JudicialOrderReporter : Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "Судебные приказы";
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "claims\\judicial_order.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
