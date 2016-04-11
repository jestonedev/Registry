using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;

namespace Registry.Viewport
{
    internal partial class OwnershipsEditorMultiMaster : Form
    {
        public string OwnershipNumber {
            get { return string.IsNullOrEmpty(textBoxOwnershipNumber.Text) ? null : textBoxOwnershipNumber.Text; }
        }

        public string OwnershipDescription
        {
            get { return string.IsNullOrEmpty(textBoxOwnershipDescription.Text) ? null : textBoxOwnershipDescription.Text; }
        }

        public int? IdOwnershipType {
            get { return ViewportHelper.ValueOrNull<int>(comboBoxIdOwnershipType); }
        }

        public DateTime OwnershipDate
        {
            get { return dateTimePickerOwnershipDate.Value.Date; }
        }

        public OwnershipsEditorMultiMaster()
        {
            InitializeComponent();
            var ownershipRightTypes = DataModel.GetInstance<OwnershipRightTypesDataModel>();
            var vOwnershipRightTypes = new BindingSource {DataSource = ownershipRightTypes.Select()};
            comboBoxIdOwnershipType.DataSource = vOwnershipRightTypes;
            comboBoxIdOwnershipType.ValueMember = "id_ownership_right_type";
            comboBoxIdOwnershipType.DisplayMember = "ownership_right_type";
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            if (IdOwnershipType == null)
            {
                MessageBox.Show(@"Не выбран тип ограничения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxIdOwnershipType.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
