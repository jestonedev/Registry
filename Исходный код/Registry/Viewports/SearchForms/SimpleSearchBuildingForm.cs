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
    internal partial class SimpleSearchBuildingForm : SearchForm
    {
        public SimpleSearchBuildingForm()
        {
            InitializeComponent();
            comboBoxCriteriaType.SelectedIndex = 0;
            foreach (Control control in Controls)
            {
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
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            textBoxCriteria.Focus();
            textBoxCriteria.SelectAll();
        }

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedBuildings = null;
            if (comboBoxCriteriaType.SelectedIndex == 0)
            {
                //по адресу
                var addressParts = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                var buildingIds = BuildingService.BuildingIDsByAddress(addressParts);
                includedBuildings = DataModelHelper.Intersect(null, buildingIds);
            }
            if (comboBoxCriteriaType.SelectedIndex == 1)
            {
                //по ФИО нанимателя
                var snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var buildingIds = BuildingService.BuildingIdsBySnp(snp, row => row.Field<int?>("id_kinship") == 1);
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingIds);
            }
            if (comboBoxCriteriaType.SelectedIndex == 2)
            {
                // по ФИО участника
                var snp = textBoxCriteria.Text.Trim().Replace("'", "").Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var buildingIds = BuildingService.BuildingIdsBySnp(snp, row => true);
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingIds);
            }
            if (comboBoxCriteriaType.SelectedIndex == 3)
            {
                //по кадастровому номеру
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "cadastral_num = '{0}'", textBoxCriteria.Text.Trim().Replace("'", ""));
            }
            if (comboBoxCriteriaType.SelectedIndex == 4)
            {
                // по номеру договора
                var buildingIds = BuildingService.BuildingIDsByRegistrationNumber(textBoxCriteria.Text.Trim().Replace("'", ""));
                includedBuildings = DataModelHelper.Intersect(includedBuildings, buildingIds);
            }
            if (includedBuildings != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_building IN (0";
                filter = includedBuildings.Aggregate(filter, (current, id) => current + (id.ToString(CultureInfo.InvariantCulture) + ","));
                filter = filter.TrimEnd(',') + ")";
            }
            if (!checkBoxMunicipalOnly.Checked) return filter;
            if (!string.IsNullOrEmpty(filter.Trim()))
                filter += " AND ";
            var municipalIds = OtherService.ObjectIdsByStates(EntityType.Building, DataModelHelper.MunicipalObjectStates().ToArray());
            var ids = municipalIds.Aggregate("", (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
            var municipalStateIds = DataModelHelper.MunicipalObjectStates().
                Aggregate("", (current, id) => current + id.ToString(CultureInfo.InvariantCulture) + ",");
            ids = ids.TrimEnd(',');
            filter += string.Format("(id_state IN ({0}) OR (id_state = 1 AND id_building IN (0{1})))", municipalStateIds, ids);

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
