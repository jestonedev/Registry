using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class GisZkhExport : Reporter
    {
        public override void Run()
        {
            ReportTitle = "Экспорт для ГИС \"ЖКХ\"";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\gis_zkh_export.xml")},
                {"connectionString", RegistrySettings.ConnectionString}
            };
            base.Run(arguments);
        }
    }
}
