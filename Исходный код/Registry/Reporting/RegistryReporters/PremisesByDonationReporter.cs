using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Settings;

namespace Registry.Reporting.RegistryReporters
{
    internal sealed class PremisesByDonationReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Помещения по дарению";
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\premises_by_donation.xml"));
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
