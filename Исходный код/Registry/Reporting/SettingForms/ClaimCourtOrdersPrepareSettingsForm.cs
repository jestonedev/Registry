using System;
using System.Windows.Forms;

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

        public ClaimCourtOrdersPrepareSettingsForm()
        {
            InitializeComponent();
            HandleHotKeys(Controls, (s,e) =>
            {
                DialogResult = DialogResult.OK;
            });
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
