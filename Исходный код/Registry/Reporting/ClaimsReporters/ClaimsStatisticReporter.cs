using System;
using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.ClaimsReporters
{
    internal sealed class ClaimsStatisticReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Общий отчет по исковой работе";
            var arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "claims\\claims_statistic.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (var cfForm = new ClaimsFilterForm())
            {
                if (cfForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var filter = "";
                    filter = cfForm.GetFilter();
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
