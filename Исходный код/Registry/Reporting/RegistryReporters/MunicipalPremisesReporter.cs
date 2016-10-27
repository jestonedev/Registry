using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.RegistryReporters
{
    class MunicipalPremisesReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Статистика по муниципальным жилым помещениям";
            var arguments = new Dictionary<string, string>
            {
                {
                    "config",
                    Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\municipal_premises.xml")
                },
                {"connectionString", RegistrySettings.ConnectionString}
            };
            base.Run(arguments);
        }
    }
}
