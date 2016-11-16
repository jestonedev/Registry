using System.Collections.Generic;
using System.IO;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyStatisticForCoMSReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Выписка для МКУ \"ЦПМУ\" по найму жилья";
            var arguments = new Dictionary<string, string>
            {
                {
                    "config",
                    Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\tenancy_registry_for_CoMS.xml")
                },
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var tsfForm = new TenancyStatisticFilterForm())
            {
                if (tsfForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var filter = tsfForm.GetFilter();
                    arguments.Add("filter", string.IsNullOrEmpty(filter.Trim()) ? "1=1" : filter);
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
