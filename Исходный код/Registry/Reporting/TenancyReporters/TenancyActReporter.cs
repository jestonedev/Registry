using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyActReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            var dialogResult =
                MessageBox.Show(@"Сформировать с открытой датой?", @"Информация", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            if (dialogResult == DialogResult.Cancel)
            {
                Cancel();
                return;
            }
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\act.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            arguments.Add("opened_date", dialogResult == DialogResult.Yes ? "1" : "0");
            base.Run(arguments);
        }
    }
}
