using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.RegistryReporters
{
    internal sealed class TenancyHistoryReporter : Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "История найма помещений";
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            var filter = arguments["filter"];
            var fileName = Path.GetTempFileName();
            using (var sw = new StreamWriter(fileName))
                sw.Write(filter);
            arguments.Remove("filter");
            arguments.Add("filterTmpFile", fileName);
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\tenancy_history.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
