using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;

namespace Registry.Viewport
{
    internal partial class SelectWarrantForm : Form
    {
        private DataModel _warrants;

        private BindingSource _vWarrants;

        public SelectWarrantForm()
        {
            InitializeComponent();
        }

        public int? WarrantId
        {
            get
            {
                if (_vWarrants.Position == -1)
                    return null;
                return Convert.ToInt32(((DataRowView)_vWarrants[_vWarrants.Position])["id_warrant"], CultureInfo.InvariantCulture);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void SelectWarrantForm_Load(object sender, EventArgs e)
        {
            _warrants = DataModel.GetInstance<WarrantsDataModel>();

            _vWarrants = new BindingSource {DataSource = _warrants.Select()};

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = _vWarrants;
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
            var filter = "";
            if (!string.IsNullOrEmpty(textBoxRegNumber.Text.Trim()))
            {
                filter += string.Format(CultureInfo.InvariantCulture, "registration_num LIKE '{0}%'", textBoxRegNumber.Text.Trim());
            }
            if (dateTimePickerDate.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "registration_date = '{0}'", dateTimePickerDate.Value.Date);
            }
            _vWarrants.Filter = filter;
        }

        private void textBoxRegNumber_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
