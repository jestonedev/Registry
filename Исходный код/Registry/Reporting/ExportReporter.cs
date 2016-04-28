using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Settings;

namespace Registry.Reporting
{
    internal sealed class ExportReporter: Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            if (arguments == null)
                arguments = new Dictionary<string, string>();
            var filter = arguments["filter"];
            var resultFilter = Regex.Replace(filter,
              "\\'\\b(?<month>\\d{1,2})\\.(?<day>\\d{1,2})\\.(?<year>\\d{2,4})\\b\\'",
              "STR_TO_DATE('" + "${day}.${month}.${year}" + "'" + ",'%d.%m.%Y')", RegexOptions.None);
            var fileName = Path.GetTempFileName();
            using (var sw = new StreamWriter(fileName))
                sw.Write(resultFilter);
            arguments.Remove("filter");
            arguments.Add("filterTmpFile", fileName);
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "export.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
