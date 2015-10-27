using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Settings;

namespace Registry.Reporting.ResettleReporters
{
    public sealed class ResettleTotalStatisticReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Сводная статистика переселения";
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "resettle\\total_statistic.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (DateRangeForm drForm = new DateRangeForm())
            {
                if (drForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("date_from", drForm.DateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("date_to", drForm.DateTo.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    base.Run(arguments);
                }
                else
                    base.Cancel();
            }
        }
    }
}
