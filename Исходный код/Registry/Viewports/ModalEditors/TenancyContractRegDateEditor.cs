using System;
using System.Windows.Forms;

namespace Registry.Viewport.ModalEditors
{
    internal partial class TenancyContractRegDateEditor : Form
    {
        public DateTime RegDate
        {
            get { return dateTimePickerRegDate.Value.Date; }
        }

        public TenancyContractRegDateEditor()
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
    }
}
