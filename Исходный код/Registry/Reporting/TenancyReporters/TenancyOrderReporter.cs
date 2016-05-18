using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyOrderReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Распоряжение на найм жилья";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\order.xml")},
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var tosForm = new TenancyOrderSettingsForm())
            {
                if (tosForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("id_rent_type", tosForm.IdRentType.ToString(CultureInfo.InvariantCulture));
                    arguments.Add("id_executor", tosForm.IdExecutor.ToString(CultureInfo.InvariantCulture));
                    arguments.Add("protocol_num", tosForm.ProtocolNum);
                    arguments.Add("protocol_date", tosForm.ProtocolDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("registration_date_from", tosForm.RegistrationDateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("registration_date_to", tosForm.RegistrationDateTo.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("order_date_from", tosForm.OrderDateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("main_text", tosForm.MainText);
                    arguments.Add("report_type", tosForm.MainText == null ? "1" : "0");
                    arguments.Add("show_address", tosForm.AddressFilter == "(1=1)" ? "0" : "1");
                    arguments.Add("id_street", tosForm.IdStreet.Trim());
                    arguments.Add("house", tosForm.House.Trim());
                    arguments.Add("address_filter", tosForm.AddressFilter);
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
