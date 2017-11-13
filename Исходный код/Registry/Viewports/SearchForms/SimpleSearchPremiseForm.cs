using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.Services;
using Registry.Entities.Infrastructure;

namespace Registry.Viewport.SearchForms
{
    internal partial class SimpleSearchPremiseForm : SearchForm
    {
        public SimpleSearchPremiseForm()
        {
            InitializeComponent();
            comboBoxCriteriaType.SelectedIndex = 0;
            HandleHotKeys(Controls, vButtonSearch_Click);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            textBoxCriteria.Focus();
            textBoxCriteria.SelectAll();
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedPremises = null;
            IEnumerable<int> includedBuildings = null;
            if (comboBoxCriteriaType.SelectedIndex == 0)
            {
                //по адресу
                var addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                if (addressParts.Length == 3)
                {
                    var premisesIds = PremisesService.PremiseIDsByAddress(addressParts);
                    includedPremises = DataModelHelper.Intersect(null, premisesIds);
                } else
                {
                    var buildingIds = BuildingService.BuildingIDsByAddress(addressParts);
                    includedBuildings = DataModelHelper.Intersect(null, buildingIds);
                }
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                //по ФИО нанимателя
                var snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var premisesIds = PremisesService.PremisesIdsBySnp(snp, row => row.Field<int?>("id_kinship") == 1);
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            } else
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                // по ФИО участника
                var snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var premisesIds = PremisesService.PremisesIdsBySnp(snp, row => true);
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            }
            else
            if (comboBoxCriteriaType.SelectedIndex == 3)
            {
                //по кадастровому номеру
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "cadastral_num = '{0}'", textBoxCriteria.Text.Trim().Replace("'", ""));
            }
            else
            if (comboBoxCriteriaType.SelectedIndex == 4)
            {
                // по номеру договора
                var premisesIds = PremisesService.PremiseIDsByRegistrationNumber(textBoxCriteria.Text.Trim().Replace("'", ""));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            }
            else
            if (comboBoxCriteriaType.SelectedIndex == 5)
            {
                // по номеру документа-основания
                var premisesIds = PremisesService.PremiseIDsByReasonNumber(textBoxCriteria.Text.Trim().Replace("'", ""));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            }
            else
            if (comboBoxCriteriaType.SelectedIndex == 6)
            {
                // по номеру протокола ЖК
                var premisesIds = PremisesService.PremiseIDsByProtocolNumber(textBoxCriteria.Text.Trim().Replace("'", ""));
                includedPremises = DataModelHelper.Intersect(includedPremises, premisesIds);
            }
            if (includedPremises != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_premises IN (0";
                filter = includedPremises.Aggregate(filter, (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
                filter = filter.TrimEnd(',') + ")";
            }
            if (includedBuildings != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_building IN (0";
                filter = includedBuildings.Aggregate(filter, (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
                filter = filter.TrimEnd(',') + ")";
            }
            if (checkBoxExcludeDemolished.Checked)
            {
                var demolishedPremises = PremisesService.DemolishedPremisesIDs();
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_premises NOT IN (0";
                filter = demolishedPremises.Aggregate(filter, (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
                filter = filter.TrimEnd(',') + ")";

                var demolishedBuildings = BuildingService.DemolishedBuildingIDs();
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_building NOT IN (0";
                filter = demolishedBuildings.Aggregate(filter, (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
                filter = filter.TrimEnd(',') + ")";
            }
            if (!checkBoxMunicipalOnly.Checked) return filter;
            if (!string.IsNullOrEmpty(filter.Trim()))
                filter += " AND ";
            var municipalIds = OtherService.ObjectIdsByStates(EntityType.Premise, DataModelHelper.MunicipalObjectStates().ToArray());
            var ids = municipalIds.Aggregate("", (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
            ids = ids.TrimEnd(',');
            var municipalStateIds = DataModelHelper.MunicipalObjectStates().
                Aggregate("", (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
            municipalStateIds = municipalStateIds.TrimEnd(',');
            filter += string.Format("(id_state IN ({0}) OR (id_state = 1 AND id_premises IN (0{1})))", municipalStateIds, ids);
            return filter;
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCriteria.Text.Trim()))
            {
                MessageBox.Show(@"Не ввиден критерий поиска", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void comboBoxCriteriaType_DropDownClosed(object sender, EventArgs e)
        {
            textBoxCriteria.Focus();
            textBoxCriteria.SelectAll();
        }

        private void textBoxCriteria_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
