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

        public string MainText
        {
            get
            {
                if (checkBoxResettle2013to2017freeLists.Checked)
                {
                    return "Руководствуясь распоряжением заместителя мэра по городскому хозяйству и строительству от $protocol_date$ № $protocol_num$ «Об утверждении сводного предварительного списка переселения граждан из аварийного жилищного фонда территориального округа города Братска, Правобережного территориального округа города Братска за счет средств бюджета города Братска, выделенных в 2015 году на реализацию региональной адресной программы Иркутской области «Переселение граждан, проживающих на территории Иркутской области, из аварийного жилищного фонда, признанного непригодным для проживания, в 2013 - 2017 годах», утвержденной постановлением Правительства Иркутской области от 29.05.2013 № 199-пп, и муниципальной программы города Братска «Развитие градостроительного комплекса и обеспечение населения доступным жильем» на 2014-2025 годы», утвержденной постановлением администрации муниципального образования города Братска от 15.10.2013 № 2759», руководствуясь статьями 45, 67 Устава муниципального образования города Братска, ";
                }
                if (checkBoxResettle2013to2017retransfer.Checked)
                {
                    return null;
                }
                if (checkBoxCheckanovskiy.Checked)
                {
                    return "Во исполнение решения Братского городского суда Иркутской области от 02.09.2011 по гражданскому делу № 2-2355/2011 по иску Западно-Байкальского межрайонного прокурора в защиту интересов Российской Федерации, неопределенного круга лиц к Открытому акционерному обществу «РУСАЛ Братский алюминиевый завод» (далее - ОАО «РУСАЛ Братск»), администрации муниципального образования города Братска об обязании переселить жителей жилого района Чекановский города Братска в жилье, соответствующее нормам действующего законодательства, за пределы санитарно-защитной зоны ОАО «РУСАЛ Братск», руководствуясь статьями 45, 67 Устава муниципального образования города Братска, ";
                }
                return "Рассмотрев протокол заседания комиссии по жилищным вопросам  администрации муниципального образования города Братска от $protocol_date$ № $protocol_num$, в соответствии с Порядком предоставления жилых помещений жилищного фонда $rent_type$ использования муниципального образования города Братска, утвержденным постановлением мэра города Братска от 19.09.2007 № 2706, руководствуясь статьями 45, 67 Устава муниципального образования города Братска, ";

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

        private void checkBoxCheckanovskiy_CheckedChanged(object sender, EventArgs e)
        {
            var state = checkBoxCheckanovskiy.Checked;
            checkBoxResettle2013to2017freeLists.Checked = false;
            checkBoxResettle2013to2017retransfer.Checked = false;
            checkBoxCheckanovskiy.Checked = state;
            UpdateForm();
        }

        private void checkBoxResettle2013to2017freeLists_CheckedChanged(object sender, EventArgs e)
        {
            var state = checkBoxResettle2013to2017freeLists.Checked;
            checkBoxCheckanovskiy.Checked = false;
            checkBoxResettle2013to2017retransfer.Checked = false;
            checkBoxResettle2013to2017freeLists.Checked = state;
            UpdateForm();
        }

        private void checkBoxResettle2013to2017retransfer_CheckedChanged(object sender, EventArgs e)
        {

            var state = checkBoxResettle2013to2017retransfer.Checked;
            checkBoxResettle2013to2017freeLists.Checked = false;
            checkBoxCheckanovskiy.Checked = false;
            checkBoxResettle2013to2017retransfer.Checked = state;
            UpdateForm();
        }

        private void UpdateForm()
        {
            if (!checkBoxResettle2013to2017retransfer.Checked && !checkBoxResettle2013to2017freeLists.Checked)
            {
                label1.Text = @"Дата протокола";
                label2.Text = @"Номер протокола";
            }
            else
            {
                label1.Text = @"От указанной даты";
                label2.Text = @"Руководствуясь распоряжением номер";
            }
        }
    }
}
