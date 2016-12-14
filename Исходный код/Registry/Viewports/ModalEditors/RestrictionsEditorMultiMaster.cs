using System;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ModalEditors
{
    internal partial class RestrictionsEditorMultiMaster : Form
    {
        public DateTime? Date
        {
            get { return ViewportHelper.ValueOrNull(dateTimePickerRestrictionDate); }
        }

        public DateTime? DateStateReg
        {
            get { return ViewportHelper.ValueOrNull(dateTimePickerRestrictionDateStateReg); }
        }

        public string RestrictionNumber {
            get { return ViewportHelper.ValueOrNull(textBoxRestrictionNumber); }
        }
        
        public string RestrictionDescription {
            get { return ViewportHelper.ValueOrNull(textBoxRestrictionDescription); }
        }

        public int? IdRestrictionType
        {
            get { return ViewportHelper.ValueOrNull<int>(comboBoxIdRestrictionType); }
        }

        public RestrictionsEditorMultiMaster()
        {
            InitializeComponent();
            var restrictionTypes = EntityDataModel<RestrictionType>.GetInstance();
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
