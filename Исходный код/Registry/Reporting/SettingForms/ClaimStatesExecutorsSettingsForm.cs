using System;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Reporting.SettingForms
{
    public partial class ClaimStatesExecutorsSettingsForm : Form
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

        public int ClaimStateType
        {
            get {
                return (int)comboBoxClaimStateType.SelectedValue;
            }
        }

        public bool OnlyCurrentClaimState
        {
            get { return checkBoxCurrentClaimState.Checked; }
        }

        public ClaimStatesExecutorsSettingsForm()
        {
            InitializeComponent();
            HandleHotKeys(Controls, vButtonOk_Click);
            comboBoxClaimStateType.DataSource = new BindingSource
            {
                DataSource = EntityDataModel<ClaimStateType>.GetInstance().Select()
            };
            comboBoxClaimStateType.DisplayMember = "state_type";
            comboBoxClaimStateType.ValueMember = "id_state_type";
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
            if (comboBoxClaimStateType.SelectedValue == null)
            {
                MessageBox.Show(@"Необходимо выбрать стадию претензионно исковой работы", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxClaimStateType.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
