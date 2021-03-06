﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.ResettleReporters
{
    public sealed class ResettleFullProcessingReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Сводный список переселения";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "resettle\\full_processing.xml")},
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var drForm = new DateRangeForm())
            {
                if (drForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("date_from", drForm.DateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("date_to", drForm.DateTo.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
