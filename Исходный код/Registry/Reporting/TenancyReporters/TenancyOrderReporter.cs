using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
            TenancyOrderSettingsForm tosForm = new TenancyOrderSettingsForm();
            if (tosForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                arguments.Add("id_rent_type", tosForm.id_rent_type.ToString());
                arguments.Add("id_executor", tosForm.id_executor.ToString());
                arguments.Add("protocol_num", tosForm.protocol_num.ToString());
                arguments.Add("protocol_date", tosForm.protocol_date.ToString("dd.MM.yyyy"));
                arguments.Add("registration_date_from", tosForm.registration_date_from.ToString("dd.MM.yyyy"));
                arguments.Add("registration_date_to", tosForm.registration_date_to.ToString("dd.MM.yyyy"));
                arguments.Add("order_date_from", tosForm.order_date_from.ToString("dd.MM.yyyy"));
                base.Run(arguments);
            }
            else
                base.Cancel();
        }
    }
}
