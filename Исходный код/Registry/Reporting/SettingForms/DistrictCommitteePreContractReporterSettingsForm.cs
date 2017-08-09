using System;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Reporting.SettingForms
{
    public partial class DistrictCommitteePreContractReporterSettingsForm : Form
    {
        public int IdCommittee
        {
            get
            {
                return (int)comboBoxCommittee.SelectedValue;
            }
        }

        public int IdPreamble
        {
            get { return (int)comboBoxPreamble.SelectedValue; }
        }

        public DistrictCommitteePreContractReporterSettingsForm()
        {
            InitializeComponent();
            var districtCommitteesPreContractPreambles = EntityDataModel<DistrictCommitteePreContractPreamble>.GetInstance().Select();
            comboBoxPreamble.DataSource = new BindingSource { DataSource = districtCommitteesPreContractPreambles };
            comboBoxPreamble.ValueMember = "id_preamble";
            comboBoxPreamble.DisplayMember = "name";
            if (districtCommitteesPreContractPreambles.Rows.Count > 0)
            {
                comboBoxPreamble.SelectedIndex = 0;
            }

            var districtCommittees = EntityDataModel<DistrictCommittee>.GetInstance().Select();
            comboBoxCommittee.DataSource = new BindingSource { DataSource = districtCommittees };
            comboBoxCommittee.ValueMember = "id_committee";
            comboBoxCommittee.DisplayMember = "name_nominative";
            if (districtCommittees.Rows.Count > 0)
            {
                comboBoxCommittee.SelectedIndex = 0;
            }

            foreach (Control control in Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            DialogResult = DialogResult.OK;
                            break;
                        case Keys.Escape:
                            DialogResult = DialogResult.Cancel;
                            break;
                    }
                };
        }

        private void vButtonOk_Click(object sender, EventArgs e)
        {
            if (comboBoxPreamble.SelectedValue == DBNull.Value)
            {
                MessageBox.Show(@"Ошибка", @"Необходимо выбрать преамбулу", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxPreamble.Focus();
                return;
            }
            if (comboBoxCommittee.SelectedValue == DBNull.Value)
            {
                MessageBox.Show(@"Ошибка", @"Необходимо выбрать комитет",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxCommittee.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
