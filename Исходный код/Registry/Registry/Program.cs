using System;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

namespace Registry
{
    static class Program
    {
        private const string MAppName = "Registry";  
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool canCreateNewApp;
            using (new Mutex(true, MAppName, out canCreateNewApp))
            {
                if (canCreateNewApp)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    if (args.Length > 0 && args.Contains("--config"))
                    {
                        using (var sf = new SettingsForm())
                        {
                            sf.ShowDialog();
                        }
                    }
                    try
                    {
                        Application.Run(new MainForm());
                    }
                    catch (TargetInvocationException)
                    {
                        //На данный момент не знаю, чем вызвано данное исключение, скорее всего Ribbon-панелью
                    }
                }
                else
                    MessageBox.Show(@"Приложение уже запущено", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
        }
    }
}
