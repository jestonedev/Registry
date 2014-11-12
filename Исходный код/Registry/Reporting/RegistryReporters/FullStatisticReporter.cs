using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Registry.Reporting.RegistryReporters
{
    internal sealed class FullStatisticReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Полная статистика по жилому фонду";
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\full_statistic.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (SelectRegionsForm srForm = new SelectRegionsForm())
            {
                if (srForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string regionsStr = "";
                    List<string> regionIDs = srForm.CheckedRegionIDs();
                    foreach (string regionID in regionIDs)
                        regionsStr += "'" + regionID + "',";
                    regionsStr = regionsStr.TrimEnd(new char[] { ',' });
                    arguments.Add("regions", regionsStr);
                    base.Run(arguments);
                }
                else
                    base.Cancel();
            }
        }
    }
}
