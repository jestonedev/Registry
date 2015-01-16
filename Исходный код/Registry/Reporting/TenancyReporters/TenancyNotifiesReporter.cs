using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Reporting.TenancyReporters
{
    internal class TenancyNotifiesReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Уведомления";
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\notifies.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (TenancyNotifiesSettingsForm tnsForm = new TenancyNotifiesSettingsForm())
            {
                if (tnsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("id_executor", tnsForm.IdExecutor.ToString(CultureInfo.InvariantCulture));
                    arguments.Add("report_type", tnsForm.ReportType == TenancyNotifiesReportType.ExportAsIs ? "1" : "2");
                    string processesStr = "";
                    Collection<int> processIds = tnsForm.TenancyProcessIds;
                    foreach (int processID in processIds)
                        processesStr += processID.ToString(CultureInfo.InvariantCulture) + ",";
                    processesStr = processesStr.TrimEnd(new char[] { ',' });
                    arguments.Add("process_ids", processesStr);
                    base.Run(arguments);
                }
                else
                    base.Cancel();
            }
        }
    }
}
