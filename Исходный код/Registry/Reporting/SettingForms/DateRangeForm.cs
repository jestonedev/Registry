using System;
using System.Windows.Forms;

namespace Registry.Reporting.SettingForms
{
    public partial class DateRangeForm : Form
    {
        public DateTime DateFrom
        {
            get
            {
                return dateTimePickerFrom.Value;
            }
        }

        public DateTime DateTo
        {
            get
            {
                return dateTimePickerTo.Value;
            }
        }

        public DateRangeForm()
        {
            InitializeComponent();
        }
    }
}
