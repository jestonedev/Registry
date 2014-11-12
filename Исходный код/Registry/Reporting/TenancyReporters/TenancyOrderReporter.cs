using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyOrderReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Распоряжение на найм жилья";
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\order.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (TenancyOrderSettingsForm tosForm = new TenancyOrderSettingsForm())
            {
                if (tosForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("id_rent_type", tosForm.IdRentType.ToString(CultureInfo.CurrentCulture));
                    arguments.Add("id_executor", tosForm.IdExecutor.ToString(CultureInfo.CurrentCulture));
                    arguments.Add("protocol_num", tosForm.ProtocolNum.ToString());
                    arguments.Add("protocol_date", tosForm.ProtocolDate.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    arguments.Add("registration_date_from", tosForm.RegistrationDateFrom.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    arguments.Add("registration_date_to", tosForm.RegistrationDateTo.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    arguments.Add("order_date_from", tosForm.OrderDateFrom.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    base.Run(arguments);
                }
                else
                    base.Cancel();
            }
        }
    }
}
