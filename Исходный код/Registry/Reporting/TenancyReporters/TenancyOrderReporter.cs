using System.Collections.Generic;
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
                    arguments.Add("general_num", tosForm.GeneralNumber);
                    arguments.Add("general_date", tosForm.GeneralDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("orphans_num", tosForm.OrphansNumber);
                    arguments.Add("orphans_date", tosForm.OrphansDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("resettle_num", tosForm.ResettleNumber);
                    arguments.Add("resettle_date", tosForm.ResettleDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("resettle_type", tosForm.ResettleType.ToString());
                    arguments.Add("court_num", tosForm.CourtNumber);
                    arguments.Add("court_date", tosForm.CourtDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("court", tosForm.Court);
                    arguments.Add("order_type", tosForm.OrderType.ToString());
                    arguments.Add("registration_date_from", tosForm.RegistrationDateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("registration_date_to", tosForm.RegistrationDateTo.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("order_date_from", tosForm.OrderDateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
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
