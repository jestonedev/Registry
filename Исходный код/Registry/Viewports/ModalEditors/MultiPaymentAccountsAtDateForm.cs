using System;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class MultiPaymentAccountsAtDateForm : Form
    {
        public DateTime DateAt
        {
            get
            {
                return dateTimePickerAt.Value;
            }
        }

        public MultiPaymentAccountsAtDateForm()
        {
            InitializeComponent();
        }
    }
}
