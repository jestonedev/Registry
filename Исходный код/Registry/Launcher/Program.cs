﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;

namespace Launcher
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string exeApp = ConfigurationManager.AppSettings["exeApp"];
            Process process = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(exeApp);
            process.StartInfo = psi;
            process.Start();
        }
    }
}
