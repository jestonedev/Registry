using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Reporting
{
    public partial class TenancyExcerptSettingsForm : Form
    {
        public DateTime excerpt_date_from
        {
            get
            {
                return dateTimePickerExcertFrom.Value;
            }
        }

        public DateTime registry_insert_date
        {
            get
            {
                return dateTimePickerRegistryInsert.Value;
            }
        }

        public bool is_culture_memorial
        {
            get
            {
                return checkBoxIsCultureMemorial.Checked;
            }
        }

        public string excerpt_number
        {
            get
            {
                return textBoxExcerptNumber.Text;
            }
        }

        public TenancyExcerptSettingsForm()
        {
            InitializeComponent();
            foreach (Control control in this.Controls)
                control.KeyDown += (sender, e) =>
                {
                    if (sender is ComboBox && ((ComboBox)sender).DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    else
                        if (e.KeyCode == Keys.Escape)
                            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                };
        }
    }
}
