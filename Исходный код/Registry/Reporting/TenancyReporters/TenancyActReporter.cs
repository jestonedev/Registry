using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyActReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            using (var form = new ActPremiseExtInfoForm())
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\act-10052016.xml"));
                    arguments.Add("connectionString", RegistrySettings.ConnectionString);                  
                    arguments.Add("opened_date", form.OpenedDate ? "1" : "0");
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
