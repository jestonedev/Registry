using System;
using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyStatisticReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Статистика по найму жилья";
            var arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\statistic.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (var tsfForm = new TenancyStatisticFilterForm())
            {
                if (tsfForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var filter = tsfForm.GetFilter();
                    if (String.IsNullOrEmpty(filter.Trim()))
                        arguments.Add("filter", "1=1");
                    else
                        arguments.Add("filter", filter);
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
