using System;
using System.Data;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ModalEditors
{
    internal partial class TenancyReasonsEditorMultiMaster : Form
    {
        private readonly BindingSource _vReasonTypes;

        public string ReasonNumber {
            get { return string.IsNullOrEmpty(textBoxReasonNumber.Text) ? null : textBoxReasonNumber.Text; }
        }

        public int? IdReasonType {
            get { return ViewportHelper.ValueOrNull<int>(comboBoxIdReasonType); }
        }

        public DateTime ReasonDate
        {
            get { return dateTimePickerReasonDate.Value.Date; }
        }

        public string ReasonPrepared
        {
            get
            {
                if (_vReasonTypes.Position == -1)
                {
                    return null;
                }
                var reasonTemplate = (string)((DataRowView) _vReasonTypes[_vReasonTypes.Position])["reason_template"];
                return reasonTemplate.Replace("@reason_number@", ReasonNumber).Replace("@reason_date@", ReasonDate.ToString("dd.MM.yyyy"));
            }
        }

        public bool DeletePrevReasons
        {
            get { return checkBoxDeletePrevReasons.Checked; }
        }

        public TenancyReasonsEditorMultiMaster()
        {
            InitializeComponent();
            var reasonTypes = EntityDataModel<ReasonType>.GetInstance();
            _vReasonTypes = new BindingSource { DataSource = reasonTypes.Select() };
            comboBoxIdReasonType.DataSource = _vReasonTypes;
            comboBoxIdReasonType.ValueMember = "id_reason_type";
            comboBoxIdReasonType.DisplayMember = "reason_name";
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            if (IdReasonType == null)
            {
                MessageBox.Show(@"Не выбран тип основания", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxIdReasonType.Focus();
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
