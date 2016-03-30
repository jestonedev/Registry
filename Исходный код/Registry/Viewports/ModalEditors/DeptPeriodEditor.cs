using System;
using System.Windows.Forms;

namespace Registry.Viewport.ModalEditors
{
    public partial class DeptPeriodEditor : Form
    {
        public bool HasDeptPeriodFrom
        {
            get { return checkBoxSetDeptPeriodFrom.Checked; }
        }

        public bool HasDeptPeriodTo
        {
            get { return checkBoxSetDeptPeriodTo.Checked; }
        }

        public DateTime? DeptPeriodFrom {
            get { return ViewportHelper.ValueOrNull(dateTimePickerDeptPeriodFrom); }
        }

        public DateTime? DeptPeriodTo
        {
            get { return ViewportHelper.ValueOrNull(dateTimePickerDeptPeriodTo); }
        }

        public DeptPeriodEditor()
        {
            InitializeComponent();
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

        private void checkBoxSetDeptPeriodFrom_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerDeptPeriodFrom.Enabled = checkBoxSetDeptPeriodFrom.Checked;
        }

        private void checkBoxSetDeptPeriodTo_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerDeptPeriodTo.Enabled = checkBoxSetDeptPeriodTo.Checked;
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            if (!HasDeptPeriodFrom && !HasDeptPeriodTo)
            {
                MessageBox.Show(@"Вы не выбрали ни одной даты", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
