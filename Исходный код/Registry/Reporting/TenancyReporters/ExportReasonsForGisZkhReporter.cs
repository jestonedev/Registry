using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Settings;

namespace Registry.Reporting.TenancyReporters
{
    internal sealed class ExportReasonsForGisZkhReporter : Reporter
    {
        public override void Run(Dictionary<string, string> arguments)
        {
            ReportTitle = "Экспорт документов-оснвоаний для ГИС \"ЖКХ\"";
            arguments.Add("config", Path.Combine(RegistrySettings.ActivityManagerConfigsPath, "tenancy\\gis_zkh_rent_reason.xml"));
            arguments.Add("connectionString", RegistrySettings.ConnectionString);
            if (arguments.ContainsKey("id_process"))
            {
                arguments.Add("ids", arguments["id_process"]);
                arguments.Remove("id_process");
            }
            var fileName = Path.GetTempFileName();
            using (var sw = new StreamWriter(fileName))
                sw.Write(arguments["ids"]);
            arguments.Remove("ids");
            arguments.Add("idsTmpFile", fileName);
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                dialog.Description = @"Выберите каталог, в который будут сохранены файлы документов-оснований";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    arguments.Add("destPath", dialog.SelectedPath + "\\" + (dialog.SelectedPath.EndsWith("\\") ? "" : "\\"));
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
