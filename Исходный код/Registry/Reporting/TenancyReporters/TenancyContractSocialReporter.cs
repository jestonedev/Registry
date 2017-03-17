using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyContractSocialReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "Формирование договора";
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\contract_social.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            var dialogResult = MessageBox.Show(@"Печатать с открытой датой?", @"Внимание",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Cancel)
            {
                Cancel();
                return;
            }
            arguments.Add("opened_date", dialogResult == DialogResult.Yes ? "1" : "0");
            base.Run(arguments);
        }
    }
}
