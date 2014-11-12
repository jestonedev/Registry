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
        public DateTime ExcerptDateFrom
        {
            get
            {
                return dateTimePickerExcertFrom.Value;
            }
        }

        public DateTime RegistryInsertDate
        {
            get
            {
                return dateTimePickerRegistryInsert.Value;
            }
        }

        public bool IsCultureMemorial
        {
            get
            {
                return checkBoxIsCultureMemorial.Checked;
            }
        }

        public string ExcerptNumber
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
                    ComboBox comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
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
