using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyExcerptReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\excerpt.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            TenancyExcerptSettingsForm tesForm = new TenancyExcerptSettingsForm();
            if (tesForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                arguments.Add("is_culture_memorial", tesForm.is_culture_memorial.ToString());
                arguments.Add("registry_insert_date", tesForm.registry_insert_date.ToString("dd.MM.yyyy"));
                arguments.Add("excerpt_date_from", tesForm.excerpt_date_from.ToString("dd.MM.yyyy"));
                arguments.Add("excerpt_number", tesForm.excerpt_number);
                base.Run(arguments);
            }
            else
                base.Cancel();
        }
    }
}
