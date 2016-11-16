using System;
using System.Windows.Forms;
using Registry.DataModels.DataModels;

namespace Registry.Reporting.SettingForms
{
    public partial class RequestToBksSettingsForm : Form
    {
        public DateTime RequestDateFrom
        {
            get
            {
                return dateTimePickerRequestDateFrom.Value;
            }
        }

        public int SignerId
        {
            get { return (int)comboBoxSigner.SelectedValue; }
        }

        public RequestToBksSettingsForm()
        {
            InitializeComponent();
            var signers = DataModel.GetInstance<SelectableSigners>().Select();
            comboBoxSigner.DataSource = new BindingSource {DataSource = signers, Filter = "id_signer_group = 2"};
            comboBoxSigner.ValueMember = "id_record";
            comboBoxSigner.DisplayMember = "snp";

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
            if (comboBoxSigner.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Ошибка", "Необходимо выбрать подписывающего", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSigner.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
