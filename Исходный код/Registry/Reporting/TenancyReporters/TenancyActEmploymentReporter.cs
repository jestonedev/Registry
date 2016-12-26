using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class TenancyActEmploymentReporter: Reporter
    {
        private readonly ActEmploymentType _actEmploymentType;

        public TenancyActEmploymentReporter(ActEmploymentType actEmploymentType)
        {
            _actEmploymentType = actEmploymentType;
        }

        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "Формирование акта";
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            switch (_actEmploymentType)
            {
                case ActEmploymentType.FromEmployment:
                    arguments.Add("act_type", "1");
                    break;
                case ActEmploymentType.ToEmployment:
                    arguments.Add("act_type", "2");
                    break;
            }
            var dialogResult = MessageBox.Show(@"Сформировать с открытой датой?", @"Информация", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
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
