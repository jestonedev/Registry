using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Settings;

namespace Registry.Reporting.RegistryReporters
{
    public sealed class MultiExcerptReporter : Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\premises_mx.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            var filter = arguments["filter"];
            var fileName = Path.GetTempFileName();
            using (var sw = new StreamWriter(fileName))
                sw.Write(filter);
            arguments.Remove("filter");
            arguments.Add("filterTmpFile", fileName);
            using (var resForm = new RegistryExcerptSettingsForm())
            {
                if (resForm.ShowDialog() != DialogResult.OK) return;
                arguments.Add("excerpt_date_from", resForm.ExcerptDateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                arguments.Add("excerpt_number", resForm.ExcerptNumber);
                arguments.Add("executor", UserDomain.Current.sAMAccountName.Replace("\\","\\\\"));
                arguments.Add("signer", resForm.SignerId.ToString());
                base.Run(arguments);
            }
        }
    }
}
