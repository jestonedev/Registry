using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyContractCommercialReporter : Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\contract_commercial.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            var dialogResult = MessageBox.Show(@"Печатать с открытой датой?", "Внимание", 
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            switch (dialogResult)
            {
                case DialogResult.Cancel:
                    Cancel();
                    return;
                case DialogResult.Yes:
                    arguments.Add("opened_date", "1");
                    break;
                default:
                    arguments.Add("opened_date", "0");
                    break;
            }
            base.Run(arguments);
        }
    }
}
