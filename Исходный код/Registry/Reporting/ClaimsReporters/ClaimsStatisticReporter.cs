using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Registry.Reporting.ClaimsReporters
{
    internal sealed class ClaimsStatisticReporter: Reporter
    {
        public override void Run()
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "claims\\claims_statistic.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            ClaimsFilterForm cfForm = new ClaimsFilterForm();
            if (cfForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filter = "";
                filter = cfForm.GetFilter();
                if (filter == "")
                    arguments.Add("filter", "1=1");
                else
                    arguments.Add("filter", filter);
                base.Run(arguments);
            }
        }
    }
}
