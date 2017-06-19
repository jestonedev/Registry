using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Settings;

namespace Registry.Reporting.SettingForms
{
    public partial class ClaimCourtOrdersPrepareSettingsForm : Form
    {
        public DateTime DateFrom
        {
            get
            {
                return dateTimePickerFrom.Value;
            }
        }

        public DateTime DateTo
        {
            get
            {
                return dateTimePickerTo.Value;
            }
        }

        public int IdExecutor
        {
            get
            {
                return (int?) comboBoxExecutor.SelectedValue ?? -1;
            }
        }

        public ClaimCourtOrdersPrepareSettingsForm()
        {
            InitializeComponent();
            HandleHotKeys(Controls, (s,e) =>
            {
                DialogResult = DialogResult.OK;
            });
            comboBoxExecutor.DataSource = new BindingSource
            {
                DataSource = EntityDataModel<Executor>.GetInstance().Select(),
                Filter = "is_inactive = 0"
            };
            comboBoxExecutor.DisplayMember = "executor_name";
            comboBoxExecutor.ValueMember = "id_executor";

            var executor = EntityDataModel<Executor>.GetInstance().FilterDeletedRows().
                FirstOrDefault(r => r.Field<string>("executor_name") == UserDomain.Current.DisplayName);
            if (executor != null)
            {
                comboBoxExecutor.SelectedValue = executor.Field<int>("id_executor");
            }
        }

        protected void HandleHotKeys(Control.ControlCollection controls, Action<object, EventArgs> eventClick)
        {
            foreach (Control control in controls)
            {
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            eventClick(sender, e);
                            break;
                        case Keys.Escape:
                            DialogResult = DialogResult.Cancel;
                            break;
                    }
                };
                if (control.Controls.Count > 0)
                {
                    HandleHotKeys(control.Controls, eventClick);
                }
            }
        }

        private void vButtonOk_Click(object sender, EventArgs e)
        {
            if (comboBoxExecutor.SelectedValue == null)
            {
                MessageBox.Show(@"Необходимо выбрать исполнителя", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
