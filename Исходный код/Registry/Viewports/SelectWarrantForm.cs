using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;

namespace Registry.Viewport
{
    public partial class SelectWarrantForm : Form
    {
        private WarrantsDataModel warrants = null;

        private BindingSource v_warrants = null;

        public SelectWarrantForm()
        {
            InitializeComponent();
        }

        public int? WarrantID
        {
            get
            {
                if (v_warrants.Position == -1)
                    return null;
                else
                    return Convert.ToInt32(((DataRowView)v_warrants[v_warrants.Position])["id_warrant"]);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void SelectWarrantForm_Load(object sender, EventArgs e)
        {
            warrants = WarrantsDataModel.GetInstance();

            v_warrants = new BindingSource();
            v_warrants.DataSource = warrants.Select();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = v_warrants;
            RegNumber.DataPropertyName = "registration_num";
            Date.DataPropertyName = "registration_date";
            Notary.DataPropertyName = "notary";
            OnBehalfOf.DataPropertyName = "on_behalf_of";
            Description.DataPropertyName = "description";
        }

        private void textBoxRegNumber_TextChanged(object sender, EventArgs e)
        {
            BuildWarrantsFilter();
        }

        private void dateTimePickerDate_ValueChanged(object sender, EventArgs e)
        {
            BuildWarrantsFilter();
        }

        private void BuildWarrantsFilter()
        {
            string filter = "";
            if (textBoxRegNumber.Text.Trim() != "")
            {
                filter += String.Format("registration_num LIKE '{0}%'", textBoxRegNumber.Text.Trim());
            }
            if (dateTimePickerDate.Checked)
            {
                if (filter != "")
                    filter += " AND ";
                filter += String.Format("registration_date = '{0}'", dateTimePickerDate.Value.Date);
            }
            v_warrants.Filter = filter;
        }
    }
}
