using System;
using System.Windows.Forms;
using Registry.DataModels.DataModels;

namespace Registry.Viewport.ModalEditors
{
    public partial class ObjectStateEditor : Form
    {
        public int? IdObjectState {
            get { return ViewportHelper.ValueOrNull<int>(comboBoxIdState); }
        }

        public ObjectStateEditor()
        {
            InitializeComponent();
            var objectStates = DataModel.GetInstance(DataModelType.ObjectStatesDataModel);
            var vObjectStates = new BindingSource {DataSource = objectStates.Select()};
            comboBoxIdState.DataSource = vObjectStates;
            comboBoxIdState.ValueMember = "id_state";
            comboBoxIdState.DisplayMember = "state_neutral";
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            if (IdObjectState == null)
            {
                MessageBox.Show(@"Не выбран вид состояния", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxIdState.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
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
