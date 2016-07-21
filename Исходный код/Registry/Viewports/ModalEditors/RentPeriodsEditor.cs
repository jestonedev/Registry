using System;
using System.Windows.Forms;

namespace Registry.Viewport.ModalEditors
{
    internal partial class RentPeriodsEditor : Form
    {
        public DateTime? BeginDate
        {
            get { return ViewportHelper.ValueOrNull(dateTimePickerBeginDate); }
        }

        public DateTime? EndDate
        {
            get { return ViewportHelper.ValueOrNull(dateTimePickerEndDate); }
        }

        public bool UntilDismissal
        {
            get { return checkBoxUntilDismissal.Checked; }
        }

        public RentPeriodsEditor()
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

        private void checkBoxUntilDismissal_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerEndDate.Enabled = !checkBoxUntilDismissal.Checked;
        }
    }
}
