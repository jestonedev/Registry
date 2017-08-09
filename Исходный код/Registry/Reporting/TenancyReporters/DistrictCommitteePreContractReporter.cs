using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class DistrictCommitteePreContractReporter : Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "Предварительный договор";
            arguments.Add("config",
                Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\district_committee_pre_contract.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            using (var dcpcrForm = new DistrictCommitteePreContractReporterSettingsForm())
            {
                if (dcpcrForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("id_preamble", dcpcrForm.IdPreamble.ToString(CultureInfo.InvariantCulture));
                    arguments.Add("id_commitee", dcpcrForm.IdCommittee.ToString(CultureInfo.InvariantCulture));
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
