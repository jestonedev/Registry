using System;
using System.Collections.Generic;
using System.IO;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.ClaimsReporters
{
    internal sealed class ClaimsStatisticReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Общий отчет по исковой работе";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "claims\\claims_statistic.xml")},
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var cfForm = new ClaimsFilterForm())
            {
                if (cfForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var filter = cfForm.GetFilter();
                    arguments.Add("filter", string.IsNullOrEmpty(filter.Trim()) ? "1=1" : filter);
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
