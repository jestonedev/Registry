using System;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Settings;

namespace Registry.Reporting.SettingForms
{
    public partial class ClaimStatesSettingsForm : Form
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

        public string Implementer
        {
            get {
                return comboBoxExecutor.SelectedValue == null ? 
                    comboBoxExecutor.Text.Trim() : 
                    comboBoxExecutor.SelectedValue.ToString().Trim();
            }
        }

        public ClaimStatesSettingsForm()
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
            comboBoxExecutor.ValueMember = "executor_name";

            comboBoxExecutor.SelectedValue = UserDomain.Current.DisplayName;
            comboBoxExecutor.Text = UserDomain.Current.DisplayName;
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
    }
}
