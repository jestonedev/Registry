using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.Entities;

namespace Registry.Reporting
{
    internal partial class SelectRegionsForm : Form
    {
        private KladrRegionsDataModel regions = null;
        private BindingSource v_regions = null;
        public SelectRegionsForm()
        {
            InitializeComponent();
            regions = KladrRegionsDataModel.GetInstance();
            v_regions = new BindingSource();
            v_regions.DataSource = regions.Select();
            v_regions.Sort = "region ASC";
            for (int i = 0; i < v_regions.Count; i++)
            {
                DataRowView row = (DataRowView)v_regions[i];
                checkedListBoxRegions.Items.Add(new Entities.Region((string)row["id_region"], (string)row["region"]));
            }

            foreach (Control control in this.Controls)
                control.KeyDown += (sender, e) =>
                {
                    ComboBox comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        vButton2_Click(null, new EventArgs());
                    else
                        if (e.KeyCode == Keys.Escape)
                            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                };
        }

        public List<string> CheckedRegionIDs()
        {
            List<string> regionIDs = new List<string>();
            foreach (object region in checkedListBoxRegions.CheckedItems)
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
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void checkBoxCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxRegions.Items.Count; i++)
                checkedListBoxRegions.SetItemChecked(i, checkBoxCheckAll.Checked);
        }
    }
}
