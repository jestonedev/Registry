using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class GisZkhExport : Reporter
    {
        public override void Run()
        {
            Run(new Dictionary<string, string> { { "ids", "" } });
        }

        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "Экспорт для ГИС \"ЖКХ\"";
            var filter = arguments["ids"];
            if (!string.IsNullOrEmpty(filter))
            {
                filter = string.Format("tp.id_process IN ({0}) AND ", filter);
            }
            var fileName = Path.GetTempFileName();
            using (var sw = new StreamWriter(fileName))
                sw.Write(filter);
            arguments.Remove("filter");
            arguments.Add("filterTmpFile", fileName);
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\gis_zkh_export.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
