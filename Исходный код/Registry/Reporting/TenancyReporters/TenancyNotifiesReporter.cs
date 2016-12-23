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
                    string reportType = null;
                    switch (tnsForm.ReportType)
                    {
                        case TenancyNotifiesReportType.ExportAsIs:
                            reportType = "1";
                            break;
                        case TenancyNotifiesReportType.PrintNotifiesPrimary:
                            reportType = "2";
                            break;
                        case TenancyNotifiesReportType.PrintNotifiesSecondary:
                            reportType = "3";
                            break;
                        case TenancyNotifiesReportType.PrintNotifiesProlongContract:
                            reportType = "4";
                            break;

                    }
                    arguments.Add("id_executor", tnsForm.IdExecutor.ToString(CultureInfo.InvariantCulture));
                    arguments.Add("report_type", reportType);
                    var processesStr = "";
                    var processIds = tnsForm.TenancyProcessIds;
                    foreach (var processId in processIds)
                        processesStr += processId.ToString(CultureInfo.InvariantCulture) + ",";
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
