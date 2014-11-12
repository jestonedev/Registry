using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyStatisticForCoMSReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Выписка для МКУ \"ЦПМУ\" по найму жилья";
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\tenancy_registry_for_CoMS.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (TenancyStatisticFilterForm tsfForm = new TenancyStatisticFilterForm())
            {
                if (tsfForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string filter = tsfForm.GetFilter();
                    if (String.IsNullOrEmpty(filter.Trim()))
                        arguments.Add("filter", "1=1");
                    else
                        arguments.Add("filter", filter);
                    base.Run(arguments);
                }
                else
                    base.Cancel();
            }
        }
    }
}
