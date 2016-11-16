using System.Collections.Generic;
using System.IO;
using Settings;

namespace Registry.Reporting.ClaimsAndPaymentsReporters
{
    internal class AccountsDuplicateStatistic: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Статистика по разделенным лицевым счетам";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "claims\\payment_accounts_duplicate_statistic.xml")},
                {"connectionString", RegistrySettings.ConnectionString}
            };
            base.Run(arguments);
        }
    }
}
