using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class RequestToMvdReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "Запрос в МВД";
            if (arguments.ContainsKey("id_process"))
            {
                arguments.Add("ids", arguments["id_process"]);
                arguments.Remove("id_process");
            }
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\requestMVD.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
