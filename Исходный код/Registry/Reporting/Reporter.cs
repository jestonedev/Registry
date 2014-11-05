using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Registry.Reporting
{
    public class Reporter
    {
        public event EventHandler<EventArgs> ReportComplete = null;
        public event EventHandler<ReportOutputStreamEventArgs> ReportOutputStreamResponse = null;

        public virtual void Run()
        {
            Run(new Dictionary<string, string>());
        }

        public virtual void Run(Dictionary<string, string> arguments)
        {
            SynchronizationContext context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem((args) =>
            {
                Process process = new Process();
                ProcessStartInfo psi = new ProcessStartInfo(RegistrySettings.ActivityManagerPath,
                    GetArguments((Dictionary<string, string>)args));
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.StandardOutputEncoding = Encoding.GetEncoding(RegistrySettings.ActivityManagerOutputCodepage);
                psi.UseShellExecute = false;
                process.StartInfo = psi;
                process.Start();
                if (ReportOutputStreamResponse != null)
                {
                    StreamReader reader = process.StandardOutput;
                    do
                    {
                        string line = reader.ReadLine();
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

        private string GetArguments(Dictionary<string, string> arguments)
        {
            string argumentsString = "";
            foreach (var argument in arguments)
                argumentsString += String.Format("{0}=\"{1}\" ", argument.Key.Replace("\"", "\\\""), argument.Value.Replace("\"", "\\\""));
            return argumentsString; ;
        }
    }
}
