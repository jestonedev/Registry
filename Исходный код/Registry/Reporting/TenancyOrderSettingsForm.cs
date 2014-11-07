using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;

namespace Registry.Reporting
{
    public partial class TenancyOrderSettingsForm : Form
    {
        ExecutorsDataModel executors = null;
        RentTypesDataModel rent_types = null;

        BindingSource v_rent_types = null;
        BindingSource v_executors = null;

        public int id_rent_type
        {
            get
            {
                if (comboBoxRentType.SelectedValue != null)
                    return Convert.ToInt32(comboBoxRentType.SelectedValue);
                else
                    return -1;
            }
        }

        public int id_executor
        {
            get
            {
                if (comboBoxRentType.SelectedValue != null)
                    return Convert.ToInt32(comboBoxExecutor.SelectedValue);
                else
                    return -1;
            }
        }

        public string protocol_num
        {
            get
            {
                return textBoxProtocolNum.Text;
            }
        }

        public DateTime protocol_date
        {
            get
            {
                return dateTimePickerProtocolDate.Value;
            }
        }

        public DateTime registration_date_from
        {
            get
            {
                return dateTimePickerRegistrationFrom.Value;
            }
        }

        public DateTime registration_date_to
        {
            get
            {
                return dateTimePickerRegistrationTo.Value;
            }
        }

        public DateTime order_date_from
        {
            get
            {
                return dateTimePickerOrderFrom.Value;
            }
        }

        public TenancyOrderSettingsForm()
        {
            InitializeComponent();
            rent_types = RentTypesDataModel.GetInstance();
            executors = ExecutorsDataModel.GetInstance();

            DataSet ds = DataSetManager.GetDataSet();

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
                    if (sender is ComboBox && ((ComboBox)sender).DroppedDown)
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
