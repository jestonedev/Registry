using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.ClaimsAndPaymentsReporters
{
    internal sealed class ClaimsStatesExecutorsReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Отчет по исполнителям стадий ИР";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "claims\\claim_states_executors.xml")},
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var cssfForm = new ClaimStatesExecutorsSettingsForm())
            {
                if (cssfForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("date_from", cssfForm.DateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("date_to", cssfForm.DateTo.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("claim_state_type", cssfForm.ClaimStateType.ToString());
                    arguments.Add("only_current_claim_state", cssfForm.OnlyCurrentClaimState ? "1" : "0");
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
