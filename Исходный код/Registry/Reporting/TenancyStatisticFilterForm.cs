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
    public partial class TenancyStatisticFilterForm : Form
    {
        KladrRegionsDataModel regions = null;

        BindingSource v_kladr = null;
        BindingSource v_regions = null;
        BindingSource v_rent_types = null;
        BindingSource v_reason_types = null;

        public TenancyStatisticFilterForm()
        {
            InitializeComponent();
            KladrStreetsDataModel.GetInstance().Select();
            regions = KladrRegionsDataModel.GetInstance();
            RentTypesDataModel.GetInstance().Select();
            TenancyReasonTypesDataModel.GetInstance().Select();

            DataSet ds = DataSetManager.DataSet;

            v_kladr = new BindingSource();
            v_kladr.DataSource = ds;
            v_kladr.DataMember = "kladr";

            v_regions = new BindingSource();
            v_regions.DataSource = regions.Select();

            v_rent_types = new BindingSource();
            v_rent_types.DataSource = ds;
            v_rent_types.DataMember = "rent_types";

            v_reason_types = new BindingSource();
            v_reason_types.DataSource = ds;
            v_reason_types.DataMember = "tenancy_reason_types";

            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxRegion.DataSource = v_regions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            comboBoxRentType.DataSource = v_rent_types;
            comboBoxRentType.ValueMember = "id_rent_type";
            comboBoxRentType.DisplayMember = "rent_type";

            comboBoxReasonType.DataSource = v_reason_types;
            comboBoxReasonType.ValueMember = "id_reason_type";
            comboBoxReasonType.DisplayMember = "reason_name";

            foreach (Control control in this.Controls)
                control.KeyDown += (sender, e) =>
                {
                    ComboBox comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        vButtonOk_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                };
        }

        internal string GetFilter()
        {
            string filter = "";
            if (checkBoxRegionEnable.Checked && (comboBoxRegion.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "SUBSTRING(v.id_street, 1, 13) = '{0}'", comboBoxRegion.SelectedValue.ToString());
            }
            if (checkBoxStreetEnable.Checked && (comboBoxStreet.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "v.id_street = '{0}'", comboBoxStreet.SelectedValue.ToString());
            }
            if (checkBoxHouseEnable.Checked && (!String.IsNullOrEmpty(textBoxHouse.Text.Trim())))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "v.house = '{0}'", textBoxHouse.Text.Trim().Replace("'", ""));
            }
            if (checkBoxPremisesNumEnable.Checked && (!String.IsNullOrEmpty(textBoxPremisesNum.Text.Trim())))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "(v.premises_num = '{0}' OR v.sub_premises_num = '{0}')", 
                    textBoxPremisesNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxRentTypeEnable.Checked && (comboBoxRentType.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "rt.id_rent_type = {0}", comboBoxRentType.SelectedValue.ToString());
            }
            if (checkBoxReasonTypeEnable.Checked && (comboBoxReasonType.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "tp.id_process IN (SELECT id_process FROM tenancy_reasons tr WHERE rt.id_rent_type = {0})",
                    comboBoxReasonType.SelectedValue.ToString());
            }
            if (dateTimePickerRegistrationFrom.Checked || dateTimePickerRegistrationTo.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerRegistrationFrom, dateTimePickerRegistrationTo, "registration_date");
            }
            if (dateTimePickerBeginDateFrom.Checked || dateTimePickerBeginDateTo.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerBeginDateFrom, dateTimePickerBeginDateTo, "begin_date");
            }
            if (dateTimePickerEndDateFrom.Checked || dateTimePickerEndDateTo.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerEndDateFrom, dateTimePickerEndDateTo, "end_date");
            }
            return filter;
        }

        private static string FilterByDateRange(DateTimePicker dateFrom, DateTimePicker dateTo, string name)
        {
            if (dateFrom.Checked && dateTo.Checked)
            {
                return String.Format(CultureInfo.InvariantCulture, "tp.{0} BETWEEN STR_TO_DATE('{1}','%d.%m.%Y') AND STR_TO_DATE('{2}','%d.%m.%Y')",
                    name, dateFrom.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), 
                    dateTo.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            }
            else
                if (dateFrom.Checked)
                {
                    return String.Format(CultureInfo.InvariantCulture, "tp.{0} >= STR_TO_DATE('{1}','%d.%m.%Y')",
                    name, dateFrom.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                }
                else
                    if (dateTo.Checked)
                    {
                        return String.Format(CultureInfo.InvariantCulture, "tp.{0} <= STR_TO_DATE('{1}','%d.%m.%Y')",
                        name, dateTo.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    }
            throw new ReporterException("Невозможно построить фильтр для формирования статистики найма");
        }

        private void vButtonOk_Click(object sender, EventArgs e)
        {
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue == null))
            {
                MessageBox.Show("Выберите улицу или уберите галочку поиска по улице", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return;
            }
            if ((checkBoxHouseEnable.Checked) && (String.IsNullOrEmpty(textBoxHouse.Text.Trim())))
            {
                MessageBox.Show("Введите номер дома или уберите галочку поиска по номеру дома", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return;
            }
            if ((checkBoxPremisesNumEnable.Checked) && (String.IsNullOrEmpty(textBoxPremisesNum.Text.Trim())))
            {
                MessageBox.Show("Введите номер помещения или уберите галочку поиска по номеру помещения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNum.Focus();
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void checkBoxRegionEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegion.Enabled = checkBoxRegionEnable.Checked;
        }

        private void checkBoxStreetEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxStreet.Enabled = checkBoxStreetEnable.Checked;
        }

        private void checkBoxHouseEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxHouse.Enabled = checkBoxHouseEnable.Checked;
        }

        private void checkBoxPremisesNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPremisesNum.Enabled = checkBoxPremisesNumEnable.Checked;
        }

        private void checkBoxRentTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRentType.Enabled = checkBoxRentTypeEnable.Checked;
        }

        private void checkBoxReasonTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxReasonType.Enabled = checkBoxReasonTypeEnable.Checked;
        }
    }
}
