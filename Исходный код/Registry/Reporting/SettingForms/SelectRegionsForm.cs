using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.DataModels.DataModels;

namespace Registry.Reporting.SettingForms
{
    internal partial class SelectRegionsForm : Form
    {
        private DataModel regions = null;
        private BindingSource v_regions = null;
        public SelectRegionsForm()
        {
            InitializeComponent();
            regions = DataModel.GetInstance<KladrRegionsDataModel>();
            v_regions = new BindingSource();
            v_regions.DataSource = regions.Select();
            v_regions.Sort = "region ASC";
            for (var i = 0; i < v_regions.Count; i++)
            {
                var row = (DataRowView)v_regions[i];
                checkedListBoxRegions.Items.Add(new Entities.Region((string)row["id_region"], (string)row["region"]));
            }

            foreach (Control control in Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        vButton2_Click(null, new EventArgs());
                    else
                        if (e.KeyCode == Keys.Escape)
                            DialogResult = DialogResult.Cancel;
                };
        }

        public List<string> CheckedRegionIDs()
        {
            var regionIDs = new List<string>();
            foreach (var region in checkedListBoxRegions.CheckedItems)
                regionIDs.Add(((Entities.Region)region).IdRegion);
            return regionIDs;
        }

        private void vButton2_Click(object sender, EventArgs e)
        {
            if (checkedListBoxRegions.CheckedItems.Count == 0)
            {
                MessageBox.Show("Необходимо выбрать хотя бы один жилой район", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void checkBoxCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBoxRegions.Items.Count; i++)
                checkedListBoxRegions.SetItemChecked(i, checkBoxCheckAll.Checked);
        }
    }
}
