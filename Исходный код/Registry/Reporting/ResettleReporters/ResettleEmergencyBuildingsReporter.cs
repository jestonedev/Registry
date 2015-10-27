using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Settings;

namespace Registry.Reporting.ResettleReporters
{
    public sealed class ResettleEmergencyBuildingsReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Сводный список переселения";
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "resettle\\emergency_buildings.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
