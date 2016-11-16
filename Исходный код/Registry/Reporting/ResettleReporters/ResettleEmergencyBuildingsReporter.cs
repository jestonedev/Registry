using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.ResettleReporters
{
    public sealed class ResettleEmergencyBuildingsReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Сводный список переселения";
            var arguments = new Dictionary<string, string>
            {
                {
                    "config",
                    Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "resettle\\emergency_buildings.xml")
                },
                {"connectionString", RegistrySettings.ConnectionString}
            };
            base.Run(arguments);
        }
    }
}
