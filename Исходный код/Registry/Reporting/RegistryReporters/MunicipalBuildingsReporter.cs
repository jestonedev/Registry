using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Registry.Reporting.SettingForms;
using Settings;

namespace Registry.Reporting.RegistryReporters
{
    internal class MunicipalBuildingsReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Статистика по муниципальным жилым зданиям";
            var arguments = new Dictionary<string, string>
            {
                {
                    "config",
                    Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "registry\\municipal_buildings.xml")
                },
                {"connectionString", RegistrySettings.ConnectionString}
            };
            using (var srdForm = new StatisticReportDialog(ReporterType.RegistryMunicipalBuildingsReporter))
            {
                if (srdForm.ShowDialog() == DialogResult.OK)
                {
                    if (srdForm.StatisticReportType == null)
                    {
                        throw new ReporterException("Не задан тип формируемой статистики");
                    }
                    arguments.Add("reportType", srdForm.StatisticReportType.ToString());
                    base.Run(arguments);
                }
                else
                {
                    Cancel();
                }
            }
        }
    }
}
