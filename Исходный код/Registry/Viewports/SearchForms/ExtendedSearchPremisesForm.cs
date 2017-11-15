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
    internal partial class ExtendedSearchPremisesForm : SearchForm
    {
        private readonly BindingSource _vKladr;

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedPremises = null;
            IEnumerable<int> excludedPremises = null;
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
            if (checkBoxStateEnable.Checked && (checkedListBox1.CheckedItems.Count > 0))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                var stateFilter = string.Empty;
                var stateIds = new List<int>();
                for(var i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                {
                    var row =(DataRowView) checkedListBox1.CheckedItems[i];
                    stateFilter += checkedListBox1.CheckedItems.IndexOf(row) == checkedListBox1.CheckedItems.Count - 1 ?
                        row["id_state"] : row["id_state"] + ", ";
                    stateIds.Add((int)row["id_state"]);
                }
                var premisesIds = SubPremisesService.GetPremisesIdsBySubPremiseStates(stateIds).ToList();
                var premisesFilter = "";
                if (premisesIds.Any())
                {
                    premisesFilter = premisesIds.Select(v => v.ToString()).Aggregate((acc, v) => acc + "," + v);
                }
                filter += string.Format("(id_state IN (0{0}) OR id_premises IN (0{1}))", stateFilter, premisesFilter);
            }
            if (checkBoxIDPremisesEnable.Checked)
                includedPremises = DataModelHelper.Intersect(null, new List<int> { Convert.ToInt32(numericUpDownIDPremises.Value) });
            if (checkBoxRegionEnable.Checked && (comboBoxRegion.SelectedValue != null))
            {
                var buildingsIds = BuildingService.BuildingIDsByRegion(comboBoxRegion.SelectedValue.ToString());
                includedBuildings = DataModelHelper.Intersect(null, buildingsIds);
            }
            if (checkBoxStreetEnable.Checked && (comboBoxStreet.SelectedValue != null))
            {
                var buildingsIds = BuildingService.BuildingIDsByStreet(comboBoxStreet.SelectedValue.ToString());
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingsIds);
            }
            if (checkBoxHouseEnable.Checked)
            {
                var buildingsIds = BuildingService.BuildingIDsByHouse(textBoxHouse.Text.Trim().Replace("'", ""));
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingsIds);
            }
            if (checkBoxFundTypeEnable.Checked && (comboBoxStreet.SelectedValue != null))
            {
                var premisesIds = PremisesService.PremiseIDsByCurrentFund(
                    Convert.ToInt32(comboBoxFundType.SelectedValue, CultureInfo.InvariantCulture));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);        
            }
            if (checkBoxContractNumberEnable.Checked)
            {
                var premisesIds = PremisesService.PremiseIDsByRegistrationNumber(textBoxContractNumber.Text.Trim().Replace("'", ""));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);    
            }
            if (checkBoxProtocolNumberEnable.Checked)
            {
                var premisesIds = PremisesService.PremiseIDsByProtocolNumber(textBoxProtocolNumber.Text.Trim().Replace("'", ""));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            }
            if (checkBoxReasonNumberEnable.Checked)
            {
                var premisesIds = PremisesService.PremiseIDsByReasonNumber(textBoxReasonNumber.Text.Trim().Replace("'", ""));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            }
            if (checkBoxTenantSNPEnable.Checked)
            {
                var snp = textBoxTenantSNP.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var premisesIds = PremisesService.PremisesIdsBySnp(snp, row => row.Field<int?>("id_kinship") == 1);
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);    
            }
            if (checkBoxOwnershipTypeEnable.Checked && (comboBoxOwnershipType.SelectedValue != null))
            {
                var premisesIds = PremisesService.PremiseIDsByOwnershipType(
                    int.Parse(comboBoxOwnershipType.SelectedValue.ToString(), CultureInfo.InvariantCulture));
                if (comboBoxOwnershipTypeCondition.SelectedIndex == 0)
                {
                    includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
                }
                else
                {
                    excludedPremises = (excludedPremises ?? new List<int>()).Union(premisesIds);
                }
            }
            if (checkBoxOwnershipNumberEnable.Checked && !string.IsNullOrEmpty(textBoxOwnershipNumber.Text.Trim()))
            {
                var premisesIds = PremisesService.PremiseIDsByOwnershipNumber(textBoxOwnershipNumber.Text.Trim());
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            }
            if (checkBoxRestrictionTypeEnable.Checked && (comboBoxRestrictionType.SelectedValue != null))
            {
                var premisesIds = PremisesService.PremiseIDsByRestricitonType(
                    int.Parse(comboBoxRestrictionType.SelectedValue.ToString(), CultureInfo.InvariantCulture));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            }
            if (checkBoxRestrictionNumberEnable.Checked && !string.IsNullOrEmpty(textBoxRestrictionNumber.Text.Trim()))
            {
                var premisesIds = PremisesService.PremiseIDsByRestricitonNumber(textBoxRestrictionNumber.Text.Trim());
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            }
            if (checkBoxSubTenancy.Checked)
            {
                var processesWithSubTenancy =
                    (from row in EntityDataModel<TenancyProcess>.GetInstance().FilterDeletedRows()
                    where row.Field<DateTime?>("sub_tenancy_date") != null && row.Field<string>("sub_tenancy_num") != null &&
                        (row.Field<string>("registration_num") == null || !row.Field<string>("registration_num").EndsWith("н"))
                    select row.Field<int>("id_process")).ToList();
                var idPremises = from row in EntityDataModel<TenancyPremisesAssoc>.GetInstance().FilterDeletedRows()
                    join processId in processesWithSubTenancy
                        on row.Field<int>("id_process") equals processId
                    select row.Field<int>("id_premises");
                var idPremisesBySubPremises = from row in EntityDataModel<TenancySubPremisesAssoc>.GetInstance().FilterDeletedRows()
                                              join processId in processesWithSubTenancy
                                                  on row.Field<int>("id_process") equals processId
                                              join subPremisesRow in EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows()
                                              on row.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                                              select subPremisesRow.Field<int>("id_premises");
                includedPremises = DataModelHelper.Intersect(includedPremises, idPremises.Union(idPremisesBySubPremises));
            }
            
            // Оптимизация: если идентификаторов зданий значительно меньше, чем помещений, то сделать пересечение помещений для уменьшения итогового фильтра
            if (includedBuildings != null && includedPremises != null && 
                includedBuildings.Count() * 10 < includedPremises.Count())
            {
                var premiseIds = (from premise in EntityDataModel<Premise>.GetInstance().FilterDeletedRows()
                    join building in includedBuildings
                        on premise.Field<int?>("id_building") equals building
                    select premise.Field<int>("id_premises")).ToList();
                includedBuildings = null;
                includedPremises = DataModelHelper.Intersect(includedPremises, premiseIds).ToList();
            }
            //

            if (includedPremises != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                var premisesFilter = BuildFilter(includedPremises, "id_premises");
                if (!string.IsNullOrEmpty(premisesFilter))
                    filter += "(" + premisesFilter + ")";
            }

            if (excludedPremises != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_premises NOT IN (0";
                filter = excludedPremises.Aggregate(filter, (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
                filter = filter.TrimEnd(',') + ")";
            }

            if (includedBuildings == null) 
                return filter == "" ? "0 = 1" : filter;
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

        public ExtendedSearchPremisesForm()
        {
            InitializeComponent();

            DataModel.GetInstance<KladrStreetsDataModel>().Select();
            DataModel.GetInstance<FundTypesDataModel>().Select();
            DataModel.GetInstance<ObjectStatesDataModel>().Select();
            var regions = DataModel.GetInstance<KladrRegionsDataModel>();
            DataModel ownershipRightTypes = EntityDataModel<OwnershipRightType>.GetInstance();
            DataModel restrictionTypes = EntityDataModel<RestrictionType>.GetInstance();

            var ds = DataStorage.DataSet;

            _vKladr = new BindingSource
            {
                DataSource = ds,
                DataMember = "kladr"
            };

            var vRegions = new BindingSource { DataSource = regions.Select() };

            var vFundTypes = new BindingSource
            {
                DataSource = ds,
                DataMember = "fund_types"
            };

            var vObjectStates = new BindingSource
            {
                DataSource = ds,
                DataMember = "object_states"
            };

            var vOwnershipRightTypes = new BindingSource { DataSource = ownershipRightTypes.Select() };
            var vRestrictionTypes = new BindingSource { DataSource = restrictionTypes.Select() };

            comboBoxStreet.DataSource = _vKladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxFundType.DataSource = vFundTypes;
            comboBoxFundType.ValueMember = "id_fund_type";
            comboBoxFundType.DisplayMember = "fund_type";            

            checkedListBox1.DataSource = vObjectStates;
            checkedListBox1.ValueMember = "id_state";
            checkedListBox1.DisplayMember = "state_neutral";

            comboBoxRegion.DataSource = vRegions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            comboBoxOwnershipType.DataSource = vOwnershipRightTypes;
            comboBoxOwnershipType.ValueMember = "id_ownership_right_type";
            comboBoxOwnershipType.DisplayMember = "ownership_right_type";

            comboBoxOwnershipTypeCondition.SelectedIndex = 0;

            comboBoxRestrictionType.DataSource = vRestrictionTypes;
            comboBoxRestrictionType.ValueMember = "id_restriction_type";
            comboBoxRestrictionType.DisplayMember = "restriction_type";

            HandleHotKeys(Controls, vButtonSearch_Click);
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
                MessageBox.Show(@"Введите номер дома или уберите галочку поиска по номеру дома", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return;
            }
            if (checkBoxPremisesNumEnable.Checked && string.IsNullOrEmpty(textBoxPremisesNum.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер помещения или уберите галочку поиска по номеру помещения", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxPremisesNum.Focus();
                return;
            }
            if (checkBoxCadastralNumEnable.Checked && string.IsNullOrEmpty(textBoxCadastralNum.Text.Trim()))
            {
                MessageBox.Show(@"Введите кадастровый номер или уберите галочку поиска по кадастровому номеру", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxCadastralNum.Focus();
                return;
            }
            if (checkBoxContractNumberEnable.Checked && string.IsNullOrEmpty(textBoxContractNumber.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер договора найма или уберите галочку поиска по номеру договора найма",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxContractNumber.Focus();
                return;
            }
            if (checkBoxReasonNumberEnable.Checked && string.IsNullOrEmpty(textBoxReasonNumber.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер документа-основания найма или уберите галочку поиска по номеру документа-основания",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxReasonNumber.Focus();
                return;
            }
            if (checkBoxProtocolNumberEnable.Checked && string.IsNullOrEmpty(textBoxProtocolNumber.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер протокола жилищной комиссии или уберите галочку поиска по номеру протокола жилищной комиссии",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxProtocolNumber.Focus();
                return;
            }
            if (checkBoxTenantSNPEnable.Checked && string.IsNullOrEmpty(textBoxTenantSNP.Text.Trim()))
            {
                MessageBox.Show(@"Введите ФИО нанимателя или уберите галочку поиска по ФИО нанимателя",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxTenantSNP.Focus();
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

        private void checkBoxStateEnable_CheckedChanged(object sender, EventArgs e)
        {
           checkedListBox1.Enabled = checkBoxStateEnable.Checked;
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
            comboBoxOwnershipTypeCondition.Enabled = checkBoxOwnershipTypeEnable.Checked;
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

        private void checkBoxReasonNumberEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxReasonNumber.Enabled = checkBoxReasonNumberEnable.Checked;
        }

        private void checkBoxProtocolNumberEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxProtocolNumber.Enabled = checkBoxProtocolNumberEnable.Checked;
        }
    }
}
