using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.ClaimsAndPaymentsReporters
{
    internal sealed class ClaimsCourtOrderPrepareReporter : Reporter
    {
        public override void Run()
        {
            ReportTitle = "Отчет о подготовленных судебных приказах";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "claims\\claim_сcourt_orders_prepare.xml")},
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var cssfForm = new ClaimCourtOrdersPrepareSettingsForm())
            {
                if (cssfForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("date_from", cssfForm.DateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("date_to", cssfForm.DateTo.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("id_executor", cssfForm.IdExecutor.ToString());
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
