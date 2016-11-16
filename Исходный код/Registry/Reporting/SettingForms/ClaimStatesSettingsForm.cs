using System;
using System.Windows.Forms;
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
            get { return textBoxImplementer.Text.Trim(); }
        }

        public ClaimStatesSettingsForm()
        {
            InitializeComponent();
            textBoxImplementer.Text = UserDomain.Current.DisplayName;
        }
    }
}
