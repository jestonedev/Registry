using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.ClaimsReporters
{
    internal sealed class ClaimsStatesReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Отчет по стадиям исковых работ";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "claims\\claim_states.xml")},
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var cssfForm = new ClaimStatesSettingsForm())
            {
                if (cssfForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("date_from", cssfForm.DateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("date_to", cssfForm.DateTo.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("implementer", cssfForm.Implementer);
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
