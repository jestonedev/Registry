using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal class TenancyNotifiesReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Уведомления";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\notifies.xml")},
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var tnsForm = new TenancyNotifiesSettingsForm())
            {
                if (tnsForm.ShowDialog() == DialogResult.OK)
                {
                    arguments.Add("id_executor", tnsForm.IdExecutor.ToString(CultureInfo.InvariantCulture));
                    arguments.Add("report_type", tnsForm.ReportType == TenancyNotifiesReportType.ExportAsIs ? "1" : (
                        tnsForm.ReportType == TenancyNotifiesReportType.PrintNotifiesPrimary ? "2" : "3"));
                    var processesStr = "";
                    var processIds = tnsForm.TenancyProcessIds;
                    foreach (var processID in processIds)
                        processesStr += processID.ToString(CultureInfo.InvariantCulture) + ",";
                    processesStr = processesStr.TrimEnd(',');
                    arguments.Add("process_ids", processesStr);
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
