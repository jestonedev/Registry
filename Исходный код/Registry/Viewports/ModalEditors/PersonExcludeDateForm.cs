using System;
using System.Windows.Forms;

namespace Registry.Viewport.ModalEditors
{
    public partial class PersonExcludeDateForm : Form
    {
        public DateTime ExcludeDate
        {
            get
            {
                return dateTimePickerExcludeDate.Value.Date;
            }
        }

        public PersonExcludeDateForm()
        {
            InitializeComponent();
        }
    }
}
