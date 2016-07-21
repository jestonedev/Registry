using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Settings;

namespace Registry.Viewport
{
    internal partial class ClaimFiles : Form
    {
        private int _idClaim;

        public ClaimFiles()
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
        }

        public void Initialize(int idClaim)
        {
            _idClaim = idClaim;
            Text = string.Format("Прикрепленные файлы исковой работы №{0}", _idClaim);
            DataBind(idClaim);
        }

        private void DataBind(int idClaim)
        {
            dataGridView.DataSource = ClaimFilesDataModel.GetInstance().Select(idClaim);
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
                File.Copy(fileInfo.FullName, Path.Combine(RegistrySettings.ClaimsAttachmentsPath, newFileName));
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    string.Format("Отсутствуют права на запись в каталог {0}", RegistrySettings.ClaimsAttachmentsPath),
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
            var claimFile = new ClaimFile
            {
                DisplayName = displayName,
                FileName = newFileName,
                IdClaim = _idClaim
            };
            if (ClaimFilesDataModel.GetInstance().Insert(claimFile) == -1) return;
            DataBind(_idClaim);
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
                var fullName = Path.Combine(RegistrySettings.ClaimsAttachmentsPath, fileName);
                if (ClaimFilesDataModel.GetInstance().Delete(idFile) == -1)
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
            DataBind(_idClaim);
        }

        private void vButtonOpenFile_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow selectedRow in dataGridView.SelectedRows)
            {
                var fileName = selectedRow.Cells["file_name"].Value.ToString();
                var fullName = Path.Combine(RegistrySettings.ClaimsAttachmentsPath, fileName);
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
