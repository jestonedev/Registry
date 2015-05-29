using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Registry.Reporting.RegistryReporters
{
    public sealed class ExcerptReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\excerpt.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (RegistryExcerptSettingsForm resForm = new RegistryExcerptSettingsForm())
            {
                if (resForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("excerpt_date_from", resForm.ExcerptDateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("excerpt_number", resForm.ExcerptNumber);
                    arguments.Add("executor", UserDomain.Current.sAMAccountName.Replace("\\","\\\\"));
                    base.Run(arguments);
                }
            }
        }
    }
}
