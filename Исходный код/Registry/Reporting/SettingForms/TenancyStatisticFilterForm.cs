using System;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Reporting.SettingForms
{
    public partial class TenancyStatisticFilterForm : Form
    {
        public TenancyStatisticFilterForm()
        {
            InitializeComponent();
            DataModel.GetInstance<KladrStreetsDataModel>().Select();
            var regions = DataModel.GetInstance<KladrRegionsDataModel>();
            
            DataModel.GetInstance<RentTypesDataModel>().Select();
            EntityDataModel<ReasonType>.GetInstance().Select();

            var ds = DataStorage.DataSet;

            var vKladr = new BindingSource
            {
                DataSource = ds,
                DataMember = "kladr"
            };

            var vRegions = new BindingSource {DataSource = regions.Select()};

            var vRentTypes = new BindingSource
            {
                DataSource = ds,
                DataMember = "rent_types"
            };

            var vReasonTypes = new BindingSource
            {
                DataSource = ds,
                DataMember = "tenancy_reason_types"
            };

            comboBoxStreet.DataSource = vKladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxRegion.DataSource = vRegions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            comboBoxRentType.DataSource = vRentTypes;
            comboBoxRentType.ValueMember = "id_rent_type";
            comboBoxRentType.DisplayMember = "rent_type";

            comboBoxReasonType.DataSource = vReasonTypes;
            comboBoxReasonType.ValueMember = "id_reason_type";
            comboBoxReasonType.DisplayMember = "reason_name";

            foreach (Control control in Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            vButtonOk_Click(sender, e);
                            break;
                        case Keys.Escape:
                            DialogResult = DialogResult.Cancel;
                            break;
                    }
                };
        }

        internal string GetFilter()
        {
            var filter = "";
            if (checkBoxRegionEnable.Checked && (comboBoxRegion.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "SUBSTRING(v.id_street, 1, 12) = '{0}'", comboBoxRegion.SelectedValue);
            }
            if (checkBoxStreetEnable.Checked && (comboBoxStreet.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "v.id_street = '{0}'", comboBoxStreet.SelectedValue);
            }
            if (checkBoxHouseEnable.Checked && (!string.IsNullOrEmpty(textBoxHouse.Text.Trim())))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "v.house = '{0}'", textBoxHouse.Text.Trim().Replace("'", ""));
            }
            if (checkBoxPremisesNumEnable.Checked && (!string.IsNullOrEmpty(textBoxPremisesNum.Text.Trim())))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "(v.premises_num = '{0}' OR v.sub_premises_num = '{0}')", 
                    textBoxPremisesNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxRentTypeEnable.Checked && (comboBoxRentType.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "rt.id_rent_type = {0}", comboBoxRentType.SelectedValue);
            }
            if (checkBoxReasonTypeEnable.Checked && (comboBoxReasonType.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "tp.id_process IN (SELECT id_process FROM tenancy_reasons tr WHERE rt.id_rent_type = {0})",
                    comboBoxReasonType.SelectedValue);
            }
            if (dateTimePickerRegistrationFrom.Checked || dateTimePickerRegistrationTo.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerRegistrationFrom, dateTimePickerRegistrationTo, "registration_date");
            }
            if (dateTimePickerBeginDateFrom.Checked || dateTimePickerBeginDateTo.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerBeginDateFrom, dateTimePickerBeginDateTo, "begin_date");
            }
            if (dateTimePickerEndDateFrom.Checked || dateTimePickerEndDateTo.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += FilterByDateRange(dateTimePickerEndDateFrom, dateTimePickerEndDateTo, "end_date");
            }
            return filter;
        }

        private static string FilterByDateRange(DateTimePicker dateFrom, DateTimePicker dateTo, string name)
        {
            if (dateFrom.Checked && dateTo.Checked)
            {
                return string.Format(CultureInfo.InvariantCulture, "tp.{0} BETWEEN STR_TO_DATE('{1}','%d.%m.%Y') AND STR_TO_DATE('{2}','%d.%m.%Y %H:%i:%S')",
                    name, dateFrom.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), 
                    dateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("dd.MM.yyyy hh:mm:ss", CultureInfo.InvariantCulture));
            }
            else
                if (dateFrom.Checked)
                {
                    return string.Format(CultureInfo.InvariantCulture, "tp.{0} >= STR_TO_DATE('{1}','%d.%m.%Y')",
                    name, dateFrom.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                }
                else
                    if (dateTo.Checked)
                    {
                        return string.Format(CultureInfo.InvariantCulture, "tp.{0} <= STR_TO_DATE('{1}','%d.%m.%Y %H:%i:%S')",
                        name, dateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("dd.MM.yyyy hh:mm:ss", CultureInfo.InvariantCulture));
                    }
            throw new ReporterException("Невозможно построить фильтр для формирования статистики найма");
        }

        private void vButtonOk_Click(object sender, EventArgs e)
        {
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите улицу или уберите галочку поиска по улице", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return;
            }
            if ((checkBoxHouseEnable.Checked) && (string.IsNullOrEmpty(textBoxHouse.Text.Trim())))
            {
                MessageBox.Show(@"Введите номер дома или уберите галочку поиска по номеру дома", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return;
            }
            if ((checkBoxPremisesNumEnable.Checked) && (string.IsNullOrEmpty(textBoxPremisesNum.Text.Trim())))
            {
                MessageBox.Show(@"Введите номер помещения или уберите галочку поиска по номеру помещения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNum.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
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
