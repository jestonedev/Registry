﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            base.Run(arguments);
        }
    }
}
