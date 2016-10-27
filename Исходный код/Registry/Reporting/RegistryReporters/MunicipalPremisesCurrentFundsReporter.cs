using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.RegistryReporters
{
    internal sealed class MunicipalPremisesCurrentFundsReporter : Reporter
    {
        public override void Run()
        {
            ReportTitle = "Текущий фонд муниципальных жилых помещений";
            var arguments = new Dictionary<string, string>
            {
                {
                    "config",
                    Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\municipal_premises_current_fund.xml")
                },
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var srForm = new SelectRegionsForm())
            {
                if (srForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var regionsStr = "";
                    var regionIDs = srForm.CheckedRegionIDs();
                    foreach (var regionId in regionIDs)
                        regionsStr += "'" + regionId + "',";
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
