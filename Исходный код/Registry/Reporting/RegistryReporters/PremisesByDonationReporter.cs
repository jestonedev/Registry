using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.RegistryReporters
{
    internal sealed class PremisesByDonationReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Помещения по дарению";
            var arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\premises_by_donation.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
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
