using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;

namespace Registry.Viewport.SearchForms
{
    internal partial class ExtendedSearchBuildingForm : SearchForm
    {
        private readonly BindingSource _vKladr;

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedBuildings = null;
            if (checkBoxStreetEnable.Checked && (comboBoxStreet.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_street = '" + comboBoxStreet.SelectedValue + "'";
            }
            if (checkBoxRegionEnable.Checked && comboBoxRegion.SelectedValue != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_street LIKE '"+comboBoxRegion.SelectedValue+"%'";
            }
            if (checkBoxHouseEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "house = '{0}'", textBoxHouse.Text.Trim().Replace("'", ""));
            }
            if (checkBoxFloorsEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "floors = " + numericUpDownFloors.Value.ToString(CultureInfo.InvariantCulture);
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
            if (checkBoxStartupYearEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "startup_year = " + numericUpDownStartupYear.Value;
            }
            if (checkBoxImprovementEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "improvement = " + (checkBoxImprovement.Checked ? 1 : 0).ToString(CultureInfo.InvariantCulture);
            }
            if (checkBoxElevatorEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "elevator = " + (checkBoxElevator.Checked ? 1 : 0).ToString(CultureInfo.InvariantCulture);
            }
            if (checkBoxStateEnable.Checked && (checkedListBox1.CheckedItems.Count > 0))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                var array = string.Empty;
                for (var i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                {
                    var row = (DataRowView)checkedListBox1.CheckedItems[i];
                    array += checkedListBox1.CheckedItems.IndexOf(row) == checkedListBox1.CheckedItems.Count - 1 ?
                        row["id_state"] : row["id_state"] + ", ";
                }
                filter += "id_state IN (" + array + ")";
            }
            if (checkBoxStructureTypeEnable.Checked && (comboBoxStructureType.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format("id_structure_type = {0}", comboBoxStructureType.SelectedValue);
            }
            if (checkBoxIDBuildingEnable.Checked)
                includedBuildings = DataModelHelper.Intersect(null, new List<int> { Convert.ToInt32(numericUpDownIDBuilding.Value) });          
            if (checkBoxOwnershipTypeEnable.Checked && (comboBoxOwnershipType.SelectedValue != null))
            {
                var buildingsIds = BuildingService.BuildingIDsByOwnershipType(
                    int.Parse(comboBoxOwnershipType.SelectedValue.ToString(), CultureInfo.InvariantCulture));
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingsIds);
            }
            if (checkBoxOwnershipNumberEnable.Checked && !string.IsNullOrEmpty(textBoxOwnershipNumber.Text.Trim()))
            {
                var buildingsIds = BuildingService.BuildingIDsByOwnershipNumber(textBoxOwnershipNumber.Text.Trim());
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingsIds);
            }
            if (checkBoxRestrictionTypeEnable.Checked && (comboBoxRestrictionType.SelectedValue != null))
            {
                var buildingsIds = BuildingService.BuildingIDsByRestricitonType(
                    int.Parse(comboBoxRestrictionType.SelectedValue.ToString(), CultureInfo.InvariantCulture));
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingsIds);
            }
            if (checkBoxRestrictionNumberEnable.Checked && !string.IsNullOrEmpty(textBoxRestrictionNumber.Text.Trim()))
            {
                var buildingsIds = BuildingService.BuildingIDsByRestricitonNumber(textBoxRestrictionNumber.Text.Trim());
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingsIds);
            }
            if (includedBuildings == null) return filter == "" ? "0 = 1" : filter;
            if (!string.IsNullOrEmpty(filter.Trim()))
                filter += " AND ";
            var buildingsFilter = BuildFilter(includedBuildings, "id_building");
            if (!string.IsNullOrEmpty(buildingsFilter))
                filter += "(" + buildingsFilter + ")";
            return filter == "" ? "0 = 1" : filter;
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
            string entropicPIdsStr;
            if (entropicPremisesIds.Count > 1000)
            {
                entropicPIdsStr = ids.Aggregate("", (current, premisesId) => current + (premisesId + ","));
                filter = "";
            }
            else
                entropicPIdsStr = entropicPremisesIds.Aggregate("",
                    (current, premisesId) => current + (premisesId + ","));
            entropicPIdsStr = entropicPIdsStr.Trim(',');
            if (string.IsNullOrEmpty(entropicPIdsStr))
                return string.IsNullOrEmpty(filter) ? string.Format("{0} IN (0)", fieldName) : filter;
            if (!string.IsNullOrEmpty(filter))
                filter += " OR ";
            filter += string.Format("{0} IN ({1})", fieldName, entropicPIdsStr);
            return filter;
        }

        public ExtendedSearchBuildingForm()
        {
            InitializeComponent();
            var kladr = DataModel.GetInstance<KladrStreetsDataModel>();
            var structureTypes = EntityDataModel<StructureType>.GetInstance();
            var objectStates = DataModel.GetInstance<ObjectStatesDataModel>();
            var regions = DataModel.GetInstance<KladrRegionsDataModel>();
            DataModel ownershipRightTypes = EntityDataModel<OwnershipRightType>.GetInstance();
            DataModel restrictionTypes = EntityDataModel<RestrictionType>.GetInstance();

            _vKladr = new BindingSource {DataSource = kladr.Select()};

            var vRegions = new BindingSource {DataSource = regions.Select()};

            var vStructureTypes = new BindingSource {DataSource = structureTypes.Select()};

            var vObjectStates = new BindingSource {DataSource = objectStates.Select()};

            var vOwnershipRightTypes = new BindingSource { DataSource = ownershipRightTypes.Select() };
            var vRestrictionTypes = new BindingSource { DataSource = restrictionTypes.Select() };

            comboBoxStreet.DataSource = _vKladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxStructureType.DataSource = vStructureTypes;
            comboBoxStructureType.ValueMember = "id_structure_type";
            comboBoxStructureType.DisplayMember = "structure_type";

            checkedListBox1.DataSource = vObjectStates;
            checkedListBox1.ValueMember = "id_state";
            checkedListBox1.DisplayMember = "state_neutral";            

            comboBoxRegion.DataSource = vRegions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            comboBoxOwnershipType.DataSource = vOwnershipRightTypes;
            comboBoxOwnershipType.ValueMember = "id_ownership_right_type";
            comboBoxOwnershipType.DisplayMember = "ownership_right_type";

            comboBoxRestrictionType.DataSource = vRestrictionTypes;
            comboBoxRestrictionType.ValueMember = "id_restriction_type";
            comboBoxRestrictionType.DisplayMember = "restriction_type";

            numericUpDownStartupYear.Maximum = DateTime.Now.Year;

            HandleHotKeys(Controls, vButtonSearch_Click);
        }

        private void checkBoxStreetEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxStreet.Enabled = checkBoxStreetEnable.Checked;
        }

        private void checkBoxHouseEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxHouse.Enabled = checkBoxHouseEnable.Checked;
        }

        private void checkBoxFloorsEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownFloors.Enabled = checkBoxFloorsEnable.Checked;
        }

        private void checkBoxCadastralNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxCadastralNum.Enabled = checkBoxCadastralNumEnable.Checked;
        }

        private void checkBoxStartupYearEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownStartupYear.Enabled = checkBoxStartupYearEnable.Checked;
        }

        private void checkBoxFundTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxStructureType.Enabled = checkBoxStructureTypeEnable.Checked;
        }

        private void checkBoxImprovementEnable_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxImprovement.Enabled = checkBoxImprovementEnable.Checked;
        }

        private void checkBoxElevatorEnable_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxElevator.Enabled = checkBoxElevatorEnable.Checked;
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

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (checkBoxStreetEnable.Checked && (comboBoxStreet.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите улицу или уберите галочку поиска по улице", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return;
            }
            if (checkBoxHouseEnable.Checked && string.IsNullOrEmpty(textBoxHouse.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер здания или уберите галочку поиска по номеру дома", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return;
            }
            if (checkBoxOwnershipNumberEnable.Checked && string.IsNullOrEmpty(textBoxOwnershipNumber.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер ограничения или уберите галочку поиска по номеру ограничения",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxOwnershipNumber.Focus();
                return;
            }
            if (checkBoxRestrictionNumberEnable.Checked && string.IsNullOrEmpty(textBoxRestrictionNumber.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер реквизита или уберите галочку поиска по номеру реквизита",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxRestrictionNumber.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void checkBoxStateEnable_CheckedChanged(object sender, EventArgs e)
        {
           checkedListBox1.Enabled = checkBoxStateEnable.Checked;
        }

        private void checkBoxIDBuildingEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownIDBuilding.Enabled = checkBoxIDBuildingEnable.Checked;
        }

        private void checkBoxRegionEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegion.Enabled = checkBoxRegionEnable.Checked;
        }

        private void checkBoxOwnershipTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxOwnershipType.Enabled = checkBoxOwnershipTypeEnable.Checked;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        private void checkBoxOwnershipNumberEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxOwnershipNumber.Enabled = checkBoxOwnershipNumberEnable.Checked;
        }

        private void checkBoxRestrictionTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRestrictionType.Enabled = checkBoxRestrictionTypeEnable.Checked;
        }

        private void checkBoxRestrictionNumberEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRestrictionNumber.Enabled = checkBoxRestrictionNumberEnable.Checked;
        }
    }
}
