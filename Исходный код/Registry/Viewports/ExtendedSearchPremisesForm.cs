using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Viewport;

namespace Registry.SearchForms
{
    internal partial class ExtendedSearchPremisesForm : SearchForm
    {
        KladrRegionsDataModel regions;
        OwnershipRightTypesDataModel ownership_right_types;

        BindingSource v_kladr;
        BindingSource v_regions;
        BindingSource v_fundTypes;
        BindingSource v_object_states;
        BindingSource v_ownership_right_types;

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedPremises = null;
            IEnumerable<int> includedBuildings = null;
            if (checkBoxPremisesNumEnable.Checked && !string.IsNullOrEmpty(textBoxPremisesNum.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "premises_num LIKE '%{0}%'", textBoxPremisesNum.Text.Trim().Replace("'", ""));
            }
            if (checkBoxFloorEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "floor = " + numericUpDownFloor.Value.ToString(CultureInfo.InvariantCulture);
            }
            if (checkBoxCadastralNumEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                if (!string.IsNullOrEmpty(textBoxCadastralNum.Text.Trim()))
                    filter += string.Format(CultureInfo.InvariantCulture, "cadastral_num = '{0}'", textBoxCadastralNum.Text.Trim().Replace("'", ""));
                else
                    filter += "cadastral_num is null";
            }
            if ((checkBoxStateEnable.Checked) && (comboBoxState.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_state = " + comboBoxState.SelectedValue;
            }
            if (checkBoxIDPremisesEnable.Checked)
                includedPremises = DataModelHelper.Intersect(null, new List<int>() { Convert.ToInt32(numericUpDownIDPremises.Value) });
            if ((checkBoxRegionEnable.Checked) && (comboBoxRegion.SelectedValue != null))
            {
                var buildingsIds = DataModelHelper.BuildingIDsByRegion(comboBoxRegion.SelectedValue.ToString());
                includedBuildings = DataModelHelper.Intersect(null, buildingsIds);
            }
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue != null))
            {
                var buildingsIds = DataModelHelper.BuildingIDsByStreet(comboBoxStreet.SelectedValue.ToString());
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingsIds);
            }
            if (checkBoxHouseEnable.Checked)
            {
                var buildingsIds = DataModelHelper.BuildingIDsByHouse(textBoxHouse.Text.Trim().Replace("'", ""));
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingsIds);
            }
            if ((checkBoxFundTypeEnable.Checked) && (comboBoxStreet.SelectedValue != null))
            {
                var premisesIds = DataModelHelper.PremiseIDsByCurrentFund(
                    Convert.ToInt32(comboBoxFundType.SelectedValue, CultureInfo.InvariantCulture));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);        
            }
            if (checkBoxContractNumberEnable.Checked)
            {
                var premisesIds = DataModelHelper.PremiseIDsByRegistrationNumber(textBoxContractNumber.Text.Trim().Replace("'", ""));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);    
            }
            if (checkBoxTenantSNPEnable.Checked)
            {
                var snp = textBoxTenantSNP.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var premisesIds = DataModelHelper.PremisesIDsBySNP(snp, row => row.Field<int?>("id_kinship") == 1);
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);    
            }
            if ((checkBoxOwnershipTypeEnable.Checked) && (comboBoxOwnershipType.SelectedValue != null))
            {
                var premisesIds = DataModelHelper.PremiseIDsByOwnershipType(
                    int.Parse(comboBoxOwnershipType.SelectedValue.ToString(), CultureInfo.InvariantCulture));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            }
            if (includedPremises != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "(" + BuildFilter(includedPremises, "id_premises") + ")";
            }
            if (includedBuildings != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "(" + BuildFilter(includedBuildings, "id_building") + ")";
            }
            return filter;
        }

        private static string BuildFilter(IEnumerable<int> ids, string fieldName)
        {
            var startId = -1;
            var count = 0;
            var filter = "";
            var entropicPremisesIds = new List<int>();
            foreach (var id in ids.Union(new List<int> { -1 }))
            {
                if (id != startId + count)
                {
                    if (count < 5)
                    {
                        if (startId != -1)
                            for (var i = 0; i < count; i++)
                                entropicPremisesIds.Add(startId + i);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(filter))
                            filter += " OR ";
                        filter += string.Format("({1} <= {0} AND {0} <= {2})", fieldName, startId, startId + count - 1);
                    }
                    startId = id;
                    count = 1;
                }
                else
                    count++;
            }
            var entropicPIdsStr = entropicPremisesIds.Aggregate("", (current, premisesId) => current + (premisesId + ","));
            entropicPIdsStr = entropicPIdsStr.Trim(',');
            if (string.IsNullOrEmpty(entropicPIdsStr)) return filter;
            if (!string.IsNullOrEmpty(filter))
                filter += " OR ";
            filter += string.Format("{0} IN ({1})", fieldName, entropicPIdsStr);
            return filter;
        }

        public ExtendedSearchPremisesForm()
        {
            InitializeComponent();
            KladrStreetsDataModel.GetInstance().Select();
            FundTypesDataModel.GetInstance().Select();
            ObjectStatesDataModel.GetInstance().Select();
            regions = KladrRegionsDataModel.GetInstance();
            ownership_right_types = OwnershipRightTypesDataModel.GetInstance();

            var ds = DataSetManager.DataSet;

            v_kladr = new BindingSource
            {
                DataSource = ds,
                DataMember = "kladr"
            };

            v_regions = new BindingSource {DataSource = regions.Select()};

            v_fundTypes = new BindingSource
            {
                DataSource = ds,
                DataMember = "fund_types"
            };

            v_object_states = new BindingSource
            {
                DataSource = ds,
                DataMember = "object_states"
            };

            v_ownership_right_types = new BindingSource {DataSource = ownership_right_types.Select()};

            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxFundType.DataSource = v_fundTypes;
            comboBoxFundType.ValueMember = "id_fund_type";
            comboBoxFundType.DisplayMember = "fund_type";

            comboBoxState.DataSource = v_object_states;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";

            comboBoxRegion.DataSource = v_regions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            comboBoxOwnershipType.DataSource = v_ownership_right_types;
            comboBoxOwnershipType.ValueMember = "id_ownership_right_type";
            comboBoxOwnershipType.DisplayMember = "ownership_right_type";

            foreach (Control control in Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            vButtonSearch_Click(sender, e);
                            break;
                        case Keys.Escape:
                            DialogResult = DialogResult.Cancel;
                            break;
                    }
                };
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите улицу или уберите галочку поиска по улице", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return;
            }
            if ((checkBoxHouseEnable.Checked) && string.IsNullOrEmpty(textBoxHouse.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер дома или уберите галочку поиска по номеру дома", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return;
            }
            if ((checkBoxPremisesNumEnable.Checked) && string.IsNullOrEmpty(textBoxPremisesNum.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер помещения или уберите галочку поиска по номеру помещения", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNum.Focus();
                return;
            }
            if ((checkBoxCadastralNumEnable.Checked) && string.IsNullOrEmpty(textBoxCadastralNum.Text.Trim()))
            {
                MessageBox.Show(@"Введите кадастровый номер или уберите галочку поиска по кадастровому номеру", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxCadastralNum.Focus();
                return;
            }
            if ((checkBoxContractNumberEnable.Checked) && string.IsNullOrEmpty(textBoxContractNumber.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер договора найма или уберите галочку поиска по номеру договора найма",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxContractNumber.Focus();
                return;
            }
            if ((checkBoxTenantSNPEnable.Checked) && string.IsNullOrEmpty(textBoxTenantSNP.Text.Trim()))
            {
                MessageBox.Show(@"Введите ФИО нанимателя или уберите галочку поиска по ФИО нанимателя",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxTenantSNP.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
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

        private void checkBoxFloorEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownFloor.Enabled = checkBoxFloorEnable.Checked;
        }

        private void checkBoxCadastralNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxCadastralNum.Enabled = checkBoxCadastralNumEnable.Checked;
        }

        private void checkBoxFundTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxFundType.Enabled = checkBoxFundTypeEnable.Checked;
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

        private void checkBoxStateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxState.Enabled = checkBoxStateEnable.Checked;
        }

        private void checkBoxIDBuildingEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownIDPremises.Enabled = checkBoxIDPremisesEnable.Checked;
        }

        private void checkBoxRegionEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegion.Enabled = checkBoxRegionEnable.Checked;
        }

        private void checkBoxContractNumberEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxContractNumber.Enabled = checkBoxContractNumberEnable.Checked;
        }

        private void checkBoxTenantSNPEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTenantSNP.Enabled = checkBoxTenantSNPEnable.Checked;
        }

        private void checkBoxOwnershipTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxOwnershipType.Enabled = checkBoxOwnershipTypeEnable.Checked;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
