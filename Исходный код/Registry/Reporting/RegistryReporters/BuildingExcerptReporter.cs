using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.RegistryReporters
{
    public sealed class BuildingExcerptReporter : Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "Выписка";
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\excerpt_build.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (var resForm = new RegistryExcerptSettingsForm())
            {
                if (resForm.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                arguments.Add("excerpt_date_from", resForm.ExcerptDateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                arguments.Add("excerpt_number", resForm.ExcerptNumber);
                arguments.Add("executor", UserDomain.Current.sAMAccountName.Replace("\\","\\\\"));
                arguments.Add("signer", resForm.SignerId.ToString());
                base.Run(arguments);
            }
        }
    }
}
