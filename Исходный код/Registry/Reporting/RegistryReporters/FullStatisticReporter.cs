using System.Collections.Generic;
using System.IO;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.RegistryReporters
{
    internal sealed class FullStatisticReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Полная статистика по жилому фонду";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\full_statistic.xml")},
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var srForm = new SelectRegionsForm())
            {
                if (srForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var regionsStr = "";
                    var regionIDs = srForm.CheckedRegionIDs();
                    foreach (var regionID in regionIDs)
                        regionsStr += "'" + regionID + "',";
                    regionsStr = regionsStr.TrimEnd(',');
                    arguments.Add("regions", regionsStr);
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
