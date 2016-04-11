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
    internal partial class RestrictionsEditorMultiMaster : Form
    {
        public DateTime? Date
        {
            get { return dateTimePickerRestrictionDate.Value.Date; }
        }

        public string RestrictionNumber {
            get { return string.IsNullOrEmpty(textBoxRestrictionNumber.Text) ? null : textBoxRestrictionNumber.Text; }
        }
        
        public string RestrictionDescription {
            get { return string.IsNullOrEmpty(textBoxRestrictionDescription.Text) ? null : textBoxRestrictionDescription.Text; }
        }

        public int? IdRestrictionType
        {
            get { return ViewportHelper.ValueOrNull<int>(comboBoxIdRestrictionType); }
        }

        public RestrictionsEditorMultiMaster()
        {
            InitializeComponent();
            var restrictionTypes = DataModel.GetInstance<RestrictionTypesDataModel>();
            var vRestrictionTypes = new BindingSource {DataSource = restrictionTypes.Select()};
            comboBoxIdRestrictionType.DataSource = vRestrictionTypes;
            comboBoxIdRestrictionType.ValueMember = "id_restriction_type";
            comboBoxIdRestrictionType.DisplayMember = "restriction_type";
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData != Keys.Enter) return base.ProcessCmdKey(ref msg, keyData);
            SendKeys.Send("{TAB}");
            return true;
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            if (IdRestrictionType == null)
            {
                MessageBox.Show(@"Не выбран тип реквизита", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxRestrictionNumber.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
