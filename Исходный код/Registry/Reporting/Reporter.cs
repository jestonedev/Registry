﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using Settings;

namespace Registry.Reporting
{
    public class Reporter
    {
        public event EventHandler<EventArgs> ReportComplete = null;
        public event EventHandler<EventArgs> ReportCanceled = null;
        public event EventHandler<ReportOutputStreamEventArgs> ReportOutputStreamResponse = null;
        public string ReportTitle { get; set; }

        public Reporter()
        {
            ReportTitle = "Unknown report";
        }

        public virtual void Run()
        {
            Run(new Dictionary<string, string>());
        }

        public virtual void Run(Dictionary<string, string> arguments)
        {
            if (!File.Exists(RegistrySettings.ActivityManagerPath))
            {
                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                    "Не удалось найти генератор отчетов ActivityManager. Возможно указанный путь {0} является некорректным.",
                    RegistrySettings.ActivityManagerPath), 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem((args) =>
            {
                using (var process = new Process())
                {
                    var psi = new ProcessStartInfo(RegistrySettings.ActivityManagerPath,
                        GetArguments((Dictionary<string, string>)args));
                    psi.CreateNoWindow = true;
                    psi.RedirectStandardOutput = true;
                    psi.StandardOutputEncoding = Encoding.GetEncoding(RegistrySettings.ActivityManagerOutputCodePage);
                    psi.UseShellExecute = false;
                    process.StartInfo = psi;
                    process.Start();
                    if (ReportOutputStreamResponse != null)
                    {
                        var reader = process.StandardOutput;
                        do
                        {
                            var line = reader.ReadLine();
                            context.Post(
                                _ =>
                                {
                                    try
                                    {
                                        ReportOutputStreamResponse(this, new ReportOutputStreamEventArgs(line));
                                    }
                                    catch (NullReferenceException)
                                    {
                                        //Исключение происходит, когда подписчики отписываются после проверки условия на null
                                    }
                                }, null);
                        } while (!process.HasExited && ReportOutputStreamResponse != null);
                    }
                    process.WaitForExit();
                }
                if (ReportComplete != null)
                    context.Post(
                        _ =>
                        {
                            try
                            {
                                ReportComplete(this, new EventArgs());
                            }
                            catch (NullReferenceException)
                            {
                                //Исключение происходит, когда подписчики отписываются после проверки условия на null
                            }
                        }, null);
            }, arguments);
        }

        private static string GetArguments(Dictionary<string, string> arguments)
        {
            var argumentsString = "";
            foreach (var argument in arguments)
                argumentsString += String.Format(CultureInfo.InvariantCulture, "{0}=\"{1}\" ", 
                    argument.Key.Replace("\"", "\\\""), 
                    argument.Value == null ? "" : argument.Value.Replace("\"", "\\\""));
            return argumentsString;
        }

        public virtual void Cancel()
        {
            if (ReportCanceled != null)
                try
                {
                    ReportCanceled(this, new EventArgs());
                }
                catch (NullReferenceException)
                {
                    //Исключение происходит, когда подписчики отписываются после проверки условия на null в многопоточном режиме
                }
        }
    }
}
