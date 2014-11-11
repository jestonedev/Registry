using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Reflection;

namespace Registry
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0 && args.Contains("--config"))
            {
                using (SettingsForm sf = new SettingsForm())
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
                //На данный момент не знаю, чем вызвано данное исключение
            }
        }
    }
}
