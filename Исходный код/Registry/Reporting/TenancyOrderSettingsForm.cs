using System;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using Registry.DataModels.DataModels;

namespace Registry.Reporting
{
    public partial class TenancyOrderSettingsForm : Form
    {
        private readonly BindingSource _vKladr;

        public string GeneralNumber
        {
            get { return textBoxGeneralNumber.Text; }
        }

        public DateTime GeneralDate
        {
            get { return dateTimePickerGeneralDate.Value; }
        }

        public string OrphansNumber
        {
            get { return textBoxOrphansNumber.Text; }
        }

        public DateTime OrphansDate
        {
            get { return dateTimePickerOrphansDate.Value; }
        }

        public string ResettleNumber
        {
            get { return textBoxResettleNumber.Text; }
        }

        public DateTime ResettleDate
        {
            get { return dateTimePickerResettleDate.Value; }
        }

        public int ResettleType
        {
            get
            {
                return radioButtonResettleFreeList.Checked ? 1
                    : radioButtonResettleRetransfer.Checked ? 2 
                    : radioButtonGeneral.Checked ? 3 : -1;
            }
        }

        public string CourtNumber
        {
            get { return textBoxCourtNumber.Text; }
        }

        public DateTime CourtDate
        {
            get { return dateTimePickerCourtDate.Value; }
        }

        public string Court
        {
            get
            {
                return comboBoxCourt.SelectedItem == null
                    ? ""
                    : comboBoxCourt.SelectedItem.ToString();
            }
        }

        public int OrderType
        {
            get
            {
                int type;
                if (!int.TryParse(tabControlExtInfo.SelectedTab.Tag.ToString(), out type))
                {
                    return -1;
                }
                return type;
            }
        }

        public int IdRentType
        {
            get
            {
                if (comboBoxRentType.SelectedValue != null)
                    return Convert.ToInt32(comboBoxRentType.SelectedValue, CultureInfo.InvariantCulture);
                return -1;
            }
        }

        public int IdExecutor
        {
            get
            {
                if (comboBoxRentType.SelectedValue != null)
                    return Convert.ToInt32(comboBoxExecutor.SelectedValue, CultureInfo.InvariantCulture);
                return -1;
            }
        }

        public DateTime RegistrationDateFrom
        {
            get
            {
                return dateTimePickerRegistrationFrom.Value;
            }
        }

        public DateTime RegistrationDateTo
        {
            get
            {
                return dateTimePickerRegistrationTo.Value;
            }
        }

        public DateTime OrderDateFrom
        {
            get
            {
                return dateTimePickerOrderFrom.Value;
            }
        }

        public string AddressFilter
        {
            get
            {
                if (!checkBoxEnableAddress.Checked) return "(1=1)";
                var filter = "";
                if (comboBoxStreet.SelectedValue != null)
                    filter += string.Format("v.id_street = '{0}'", comboBoxStreet.SelectedValue);
                if (string.IsNullOrEmpty(textBoxHouse.Text.Trim())) return string.IsNullOrEmpty(filter) ? "(1=0)" : filter;
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += string.Format("v.house = '{0}'", textBoxHouse.Text.Trim());
                return string.IsNullOrEmpty(filter) ? "(1=0)" : filter;
            }
        }

        public string IdStreet
        {
            get { return comboBoxStreet.SelectedValue.ToString(); }
        }

        public string House
        {
            get { return textBoxHouse.Text; }
        }

        public TenancyOrderSettingsForm()
        {
            InitializeComponent();
            DataModel.GetInstance<RentTypesDataModel>().Select();
            DataModel.GetInstance<ExecutorsDataModel>().Select();
            DataModel.GetInstance<KladrStreetsDataModel>().Select();

            var ds = DataModel.DataSet;

            var vRentTypes = new BindingSource
            {
                DataSource = ds,
                DataMember = "rent_types"
            };

            var vExecutors = new BindingSource
            {
                DataSource = ds,
                DataMember = "executors",
                Filter = "is_inactive = 0"
            };

            _vKladr = new BindingSource
            {
                DataSource = ds,
                DataMember = "kladr"
            };

            comboBoxStreet.DataSource = _vKladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxRentType.DataSource = vRentTypes;
            comboBoxRentType.ValueMember = "id_rent_type";
            comboBoxRentType.DisplayMember = "rent_type";

            comboBoxExecutor.DataSource = vExecutors;
            comboBoxExecutor.ValueMember = "id_executor";
            comboBoxExecutor.DisplayMember = "executor_name";

            comboBoxCourt.SelectedIndex = 0;

            foreach (Control control in Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            DialogResult = DialogResult.OK;
                            break;
                        case Keys.Escape:
                            DialogResult = DialogResult.Cancel;
                            break;
                    }
                };
        }

        private void comboBoxStreet_Leave(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count > 0)
            {
                if (comboBoxStreet.SelectedValue == null)
                    comboBoxStreet.SelectedValue = _vKladr[_vKladr.Position];
                comboBoxStreet.Text = ((DataRowView)_vKladr[_vKladr.Position])["street_name"].ToString();
            }
            if (comboBoxStreet.SelectedValue == null)
                comboBoxStreet.Text = "";
        }

        private void comboBoxStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxStreet.Text;
                var selectionStart = comboBoxStreet.SelectionStart;
                var selectionLength = comboBoxStreet.SelectionLength;
                _vKladr.Filter = "street_name like '%" + comboBoxStreet.Text + "%'";
                comboBoxStreet.Text = text;
                comboBoxStreet.SelectionStart = selectionStart;
                comboBoxStreet.SelectionLength = selectionLength;
            }
        }

        private void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count == 0)
                comboBoxStreet.SelectedIndex = -1;
        }

        private void checkBoxEnableAddress_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxAddress.Enabled = checkBoxEnableAddress.Checked;
        }
    }
}
