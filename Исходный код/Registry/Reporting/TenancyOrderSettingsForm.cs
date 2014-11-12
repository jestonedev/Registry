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

namespace Registry.Reporting
{
    public partial class TenancyOrderSettingsForm : Form
    {

        BindingSource v_rent_types = null;
        BindingSource v_executors = null;

        public int IdRentType
        {
            get
            {
                if (comboBoxRentType.SelectedValue != null)
                    return Convert.ToInt32(comboBoxRentType.SelectedValue, CultureInfo.CurrentCulture);
                else
                    return -1;
            }
        }

        public int IdExecutor
        {
            get
            {
                if (comboBoxRentType.SelectedValue != null)
                    return Convert.ToInt32(comboBoxExecutor.SelectedValue, CultureInfo.CurrentCulture);
                else
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

        public TenancyOrderSettingsForm()
        {
            InitializeComponent();
            RentTypesDataModel.GetInstance().Select();
            ExecutorsDataModel.GetInstance().Select();

            DataSet ds = DataSetManager.DataSet;

            v_rent_types = new BindingSource();
            v_rent_types.DataSource = ds;
            v_rent_types.DataMember = "rent_types";

            v_executors = new BindingSource();
            v_executors.DataSource = ds;
            v_executors.DataMember = "executors";
            v_executors.Filter = "is_inactive = 0";

            comboBoxRentType.DataSource = v_rent_types;
            comboBoxRentType.ValueMember = "id_rent_type";
            comboBoxRentType.DisplayMember = "rent_type";

            comboBoxExecutor.DataSource = v_executors;
            comboBoxExecutor.ValueMember = "id_executor";
            comboBoxExecutor.DisplayMember = "executor_name";

            foreach (Control control in this.Controls)
                control.KeyDown += (sender, e) =>
                {
                    ComboBox comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    else
                        if (e.KeyCode == Keys.Escape)
                            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                };
        }
    }
}
