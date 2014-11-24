using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyExcerptReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\excerpt.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (TenancyExcerptSettingsForm tesForm = new TenancyExcerptSettingsForm())
            {
                if (tesForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("is_culture_memorial", tesForm.IsCultureMemorial.ToString());
                    arguments.Add("registry_insert_date", tesForm.RegistryInsertDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("excerpt_date_from", tesForm.ExcerptDateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("excerpt_number", tesForm.ExcerptNumber);
                    base.Run(arguments);
                }
                else
                    base.Cancel();
            }
        }
    }
}
