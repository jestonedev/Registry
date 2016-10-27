using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.RegistryReporters
{
    class MunicipalBuildingsReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Статистика по муниципальным жилым зданиям";
            var arguments = new Dictionary<string, string>
            {
                {
                    "config",
                    Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\municipal_buildings.xml")
                },
                {"connectionString", RegistrySettings.ConnectionString}
            };
            base.Run(arguments);
        }
    }
}
