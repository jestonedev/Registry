using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Settings;

namespace Registry.Viewport
{
    internal partial class TenancyFiles : Form
    {
        private int _idProcess;

        public TenancyFiles()
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
        }

        public void Initialize(int idProcess)
        {
            _idProcess = idProcess;
            Text = string.Format("Прикрепленные файлы процесса найма №{0}", _idProcess);
            DataBind(idProcess);
        }

        private void DataBind(int idProcess)
        {
            dataGridView.DataSource = TenancyFilesDataModel.GetInstance().Select(idProcess);
            id_file.DataPropertyName = "id_file";
            file_name.DataPropertyName = "file_name";
            display_name.DataPropertyName = "display_name";
        }

        private void vButtonAddFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;
            var fileName = dialog.FileName;
            if (!File.Exists(fileName))
            {
                MessageBox.Show(string.Format("Файла {0} не существует", fileName), @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var fileInfo = new FileInfo(fileName);
            var displayName = fileInfo.Name;
            var newFileName = Guid.NewGuid()+fileInfo.Extension;
            try
            {
                File.Copy(fileInfo.FullName, Path.Combine(RegistrySettings.TenanciesAttachmentsPath, newFileName));
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    string.Format("Отсутствуют права на запись в каталог {0}", RegistrySettings.TenanciesAttachmentsPath),
                    @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            catch (SystemException exc)
            {
                MessageBox.Show(
                    string.Format("Ошибка при сохранении файла. Подробнее: {0}", exc.Message), @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var tenancyFile = new TenancyFile
            {
                DisplayName = displayName,
                FileName = newFileName,
                IdProcess = _idProcess
            };
            if (TenancyFilesDataModel.GetInstance().Insert(tenancyFile) == -1) return;
            DataBind(_idProcess);
        }

        private void vButtonDeleteFile_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show(@"Выберите файл, который хотите удалить", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            foreach (DataGridViewRow selectedRow in dataGridView.SelectedRows)
            {
                var idFile = (int)selectedRow.Cells["id_file"].Value;
                var fileName = selectedRow.Cells["file_name"].Value.ToString();
                var fullName = Path.Combine(RegistrySettings.TenanciesAttachmentsPath, fileName);
                if (TenancyFilesDataModel.GetInstance().Delete(idFile) == -1)
                    return;
                if (!File.Exists(fullName)) continue;
                try
                {
                    File.Delete(fullName);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        string.Format("Отсутствуют права на удаление файла {0}", fullName), @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                catch (SystemException exc)
                {
                    MessageBox.Show(
                        string.Format("Ошибка при удалении файла. Подробнее: {0}", exc.Message), @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
            }
            DataBind(_idProcess);
        }

        private void vButtonOpenFile_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show(@"Выберите файл, который хотите открыть", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            foreach (DataGridViewRow selectedRow in dataGridView.SelectedRows)
            {
                var fileName = selectedRow.Cells["file_name"].Value.ToString();
                var fullName = Path.Combine(RegistrySettings.TenanciesAttachmentsPath, fileName);
                using (var process = new Process())
                {
                    var psi = new ProcessStartInfo(fullName) {UseShellExecute = true};
                    process.StartInfo = psi;
                    process.Start();
                }
            }
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            vButtonOpenFile_Click(sender, e);
        }
    }
}
