using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Globalization;
using Registry.DataModels.DataModels;

namespace Registry.Reporting
{
    public partial class TenancyOrderSettingsForm : Form
    {

        BindingSource v_rent_types = null;
        BindingSource v_executors = null;
        private BindingSource v_kladr = null;

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

        public string ProtocolNum
        {
            get
            {
                return textBoxProtocolNum.Text;
            }
        }

        public DateTime ProtocolDate
        {
            get
            {
                return dateTimePickerProtocolDate.Value;
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

        public bool IsResettle2013To2017
        {
            get { return checkBoxResettle2013to2017.Checked; }
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
            DataModel.GetInstance(DataModelType.RentTypesDataModel).Select();
            DataModel.GetInstance(DataModelType.ExecutorsDataModel).Select();
            DataModel.GetInstance(DataModelType.KladrStreetsDataModel).Select();

            var ds = DataModel.DataSet;

            v_rent_types = new BindingSource
            {
                DataSource = ds,
                DataMember = "rent_types"
            };

            v_executors = new BindingSource
            {
                DataSource = ds,
                DataMember = "executors",
                Filter = "is_inactive = 0"
            };

            v_kladr = new BindingSource
            {
                DataSource = ds,
                DataMember = "kladr"
            };

            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxRentType.DataSource = v_rent_types;
            comboBoxRentType.ValueMember = "id_rent_type";
            comboBoxRentType.DisplayMember = "rent_type";

            comboBoxExecutor.DataSource = v_executors;
            comboBoxExecutor.ValueMember = "id_executor";
            comboBoxExecutor.DisplayMember = "executor_name";

            foreach (Control control in Controls)
                control.KeyDown += (sender, e) =>
                {
                    ComboBox comboBox = sender as ComboBox;
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
                    comboBoxStreet.SelectedValue = v_kladr[v_kladr.Position];
                comboBoxStreet.Text = ((DataRowView)v_kladr[v_kladr.Position])["street_name"].ToString();
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
                v_kladr.Filter = "street_name like '%" + comboBoxStreet.Text + "%'";
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
