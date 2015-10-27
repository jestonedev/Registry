using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var fileName = Path.GetTempFileName();
            using (var sw = new StreamWriter(fileName))
                sw.Write(filter);
            arguments.Remove("filter");
            arguments.Add("filterTmpFile", fileName);
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "export.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            base.Run(arguments);
        }
    }
}
