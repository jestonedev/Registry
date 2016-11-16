using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.ClaimsReporters
{
    public sealed class RequestToBksReporter : Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "claims\\request_BKS.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            var filter = arguments["filter"];
            var fileName = Path.GetTempFileName();
            using (var sw = new StreamWriter(fileName))
                sw.Write(filter);
            arguments.Remove("filter");
            arguments.Add("filterTmpFile", fileName);
            using (var resForm = new RequestToBksSettingsForm())
            {
                if (resForm.ShowDialog() != DialogResult.OK) return;
                arguments.Add("request_date_from", resForm.RequestDateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                arguments.Add("executor", UserDomain.Current.sAMAccountName.Replace("\\","\\\\"));
                arguments.Add("signer", resForm.SignerId.ToString());
                base.Run(arguments);
            }
        }
    }
}
