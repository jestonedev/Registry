using System;
using System.Windows.Forms;
using Registry.DataModels.DataModels;

namespace Registry.Reporting.SettingForms
{
    public partial class RegistryExcerptSettingsForm : Form
    {
        public DateTime ExcerptDateFrom
        {
            get
            {
                return dateTimePickerExcertFrom.Value;
            }
        }

        public string ExcerptNumber
        {
            get
            {
                return textBoxExcerptNumber.Text;
            }
        }

        public int SignerId
        {
            get { return (int)comboBoxSigner.SelectedValue; }
        }

        public RegistryExcerptSettingsForm()
        {
            InitializeComponent();
            var signers = DataModel.GetInstance<SelectableSignersDataModel>().Select();
            comboBoxSigner.DataSource = new BindingSource {DataSource = signers, Filter = "id_signer_group = 1"};
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
