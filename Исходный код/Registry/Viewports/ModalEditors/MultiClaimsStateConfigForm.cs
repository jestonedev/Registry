using System;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Settings;

namespace Registry.Viewport.ModalEditors
{
    internal partial class MultiClaimsStateConfigForm : Form
    {
        public int? IdStateType
        {
            get
            {
                return comboBoxClaimStateType.SelectedValue == DBNull.Value
                    ? null
                    : (int?) comboBoxClaimStateType.SelectedValue;
            }
        }

        public DateTime DateStartState
        {
            get { return dateTimePickerStartState.Value.Date; }
        }

        public string Description
        {
            get { return string.IsNullOrEmpty(textBoxDescription.Text) ? null : textBoxDescription.Text; }
        }

        public DateTime? TransfertToLegalDepartmentDate
        {
            get { return dateTimePickerTransfertToLegalDepartmentDate.Checked ? (DateTime?)dateTimePickerTransfertToLegalDepartmentDate.Value.Date : null; }
        }

        public DateTime? AcceptedByLegalDepartmentDate
        {
            get { return dateTimePickerAcceptedByLegalDepartmentDate.Checked ? (DateTime?)dateTimePickerAcceptedByLegalDepartmentDate.Value.Date : null; }
        }

        public string TransfertToLegalDepartmentWho
        {
            get { return string.IsNullOrEmpty(textBoxTransferToLegalDepartmentWho.Text) ? null : textBoxTransferToLegalDepartmentWho.Text; }
        }

        public string AcceptedByLegalDepartmentWho
        {
            get { return string.IsNullOrEmpty(textBoxAcceptedByLegalDepartmentWho.Text) ? null : textBoxAcceptedByLegalDepartmentWho.Text; }
        }


        public MultiClaimsStateConfigForm()
        {
            InitializeComponent();
            var source = new BindingSource
            {
                DataSource = DataModel.GetInstance<EntityDataModel<ClaimStateType>>().Select()
            };
            comboBoxClaimStateType.ValueMember = "id_state_type";
            comboBoxClaimStateType.DisplayMember = "state_type";
            comboBoxClaimStateType.DataSource = source;
            textBoxTransferToLegalDepartmentWho.Text = UserDomain.Current.DisplayName;
            textBoxAcceptedByLegalDepartmentWho.Text = UserDomain.Current.DisplayName;
        }

        private void comboBoxClaimStateType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBoxClaimStateType.SelectedValue == DBNull.Value || comboBoxClaimStateType.SelectedValue == null)
            {
                tabControlWithoutTabs1.Visible = false;
                return;
            }
            tabControlWithoutTabs1.Visible = true;
            switch ((int)comboBoxClaimStateType.SelectedValue)
            {
                case 2:
                    tabControlWithoutTabs1.SelectTab(tabPageToLegalDepartment);
                    break;
                case 3:
                    tabControlWithoutTabs1.SelectTab(tabPageAcceptedByLegalDepartment);
                    break;
                default:
                    tabControlWithoutTabs1.Visible = false;
                    break;
            }
        }

        private void vButtonOk_Click(object sender, EventArgs e)
        {
            if (comboBoxClaimStateType.SelectedValue == null || comboBoxClaimStateType.SelectedValue == DBNull.Value)
            {
                MessageBox.Show(@"Не выбран тип состояния претензионно-исковой работы",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxClaimStateType.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
