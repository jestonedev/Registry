using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Security;

namespace Registry.Viewport.ModalEditors
{
    internal partial class SubPremisesEditor : Form
    {
        private ViewportState state = ViewportState.NewRowState;
        private SubPremise subPremise;
        private ParentTypeEnum parentType;
        private DataModel sub_premises = EntityDataModel<SubPremise>.GetInstance();
        private DataModel object_states;
        private BindingSource v_object_states;

        public ParentTypeEnum ParentType
        {
            get
            {
                return parentType;
            }
            set
            {
                if (value != ParentTypeEnum.Premises)
                    throw new ViewportException("Неизвестный тип родительского объекта");
                parentType = value;
            }
        }

        public DataRow ParentRow { get; set; }

        public ViewportState State
        {
            get
            {
                return state;
            }
            set
            {
                if (value == ViewportState.ReadState)
                {
                    throw new ViewportException("Передано некорректное состояние формы редактирования");
                }
                if (value == ViewportState.ModifyRowState)
                {
                    Text = @"Изменить комнату";
                    vButtonSave.Text = @"Изменить";
                }
                else
                {
                    Text = @"Добавить комнату";
                    vButtonSave.Text = @"Добавить";
                }
                state = value;
            }
        }

        public SubPremise SubPremiseValue
        {
            get
            {
                var subPremiseValue = new SubPremise
                {
                    TotalArea = (double) numericUpDownTotalArea.Value,
                    Description = ViewportHelper.ValueOrNull(textBoxDescription),
                    SubPremisesNum = ViewportHelper.ValueOrNull(textBoxSubPremisesNum),
                    IdState = ViewportHelper.ValueOrNull<int>(comboBoxIdState),
                    CadastralNum = ViewportHelper.ValueOrNull(textBoxCadastralNum),
                    CadastralCost = numericUpDownCadastralCost.Value,
                    BalanceCost = numericUpDownBalanceCost.Value,
                    Account = ViewportHelper.ValueOrNull(textBoxAccount)
                };
                if ((double)numericUpDownLivingArea.Value == 0)
                    subPremiseValue.LivingArea = (double)numericUpDownTotalArea.Value;
                else
                    subPremiseValue.LivingArea = (double)numericUpDownLivingArea.Value;
                if (state == ViewportState.ModifyRowState)
                    subPremiseValue.IdSubPremises = subPremise.IdSubPremises;
                if (dateTimePickerStateDate.Checked)
                    subPremiseValue.StateDate = dateTimePickerStateDate.Value;
                return subPremiseValue;
            }
            set
            {
                subPremise = value;
                if (value == null)
                    return;
                textBoxSubPremisesNum.Text = value.SubPremisesNum;
                textBoxDescription.Text = value.Description;
                numericUpDownTotalArea.Value = value.TotalArea == null ? 0 : (decimal)value.TotalArea;
                numericUpDownLivingArea.Value = value.LivingArea == null ? 0 : (decimal)value.LivingArea;
                comboBoxIdState.SelectedValue = value.IdState;
                textBoxCadastralNum.Text = value.CadastralNum;
                numericUpDownCadastralCost.Value = value.CadastralCost ?? 0;
                numericUpDownBalanceCost.Value = value.BalanceCost ?? 0;
                textBoxAccount.Text = value.Account;
                if (value.StateDate != null)
                {
                    dateTimePickerStateDate.Value = value.StateDate.Value;
                    dateTimePickerStateDate.Checked = true;
                } else
                {
                    dateTimePickerStateDate.Value = DateTime.Now.Date;
                    dateTimePickerStateDate.Checked = false;
                }
            }
        }

        public SubPremisesEditor()
        {
            InitializeComponent();
            object_states = DataModel.GetInstance<ObjectStatesDataModel>();
            v_object_states = new BindingSource {DataSource = object_states.Select()};
            comboBoxIdState.DataSource = v_object_states;
            comboBoxIdState.ValueMember = "id_state";
            comboBoxIdState.DisplayMember = "state_female";
        }

        private bool ValidatePermissions(SubPremise subPremise)
        {
            if (DataModelHelper.MunicipalObjectStates().Contains(subPremise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на добавление в базу муниципальных жилых помещений", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.NonMunicipalAndUnknownObjectStates().Contains(subPremise.IdState.Value) && 
                !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на добавление в базу немуниципальных жилых помещений", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private bool ValidateData(SubPremise subPremise)
        {
            if (ValidatePermissions(subPremise) == false)
                return false;
            if (subPremise.IdState == null)
            {
                MessageBox.Show(@"Необходимо выбрать состояние помещения", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (subPremise.SubPremisesNum != null && !Regex.IsMatch(subPremise.SubPremisesNum, "^([0-9]+[а-я]{0,1}|[а-я])$"))
            {
                MessageBox.Show(@"Некорректно задан номер комнаты. Можно использовать только цифры и не более одной строчной буквы кириллицы",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            var subPremise = SubPremiseValue;
            if (!ValidateData(subPremise))
                return;
            var idParent = ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] : -1;
            subPremise.IdPremises = idParent;
            if (state == ViewportState.NewRowState)
            {
                if (idParent == -1)
                {
                    MessageBox.Show(@"Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                sub_premises.EditingNewRecord = true;
                var idSubPremise = sub_premises.Insert(subPremise);
                if (idSubPremise == -1)
                    return;
                sub_premises.Select().Rows.Add(idSubPremise, subPremise.IdPremises, subPremise.IdState, subPremise.SubPremisesNum, 
                    subPremise.TotalArea, subPremise.LivingArea, subPremise.Description, subPremise.StateDate, subPremise.CadastralNum, 
                    subPremise.CadastralCost, subPremise.BalanceCost, subPremise.Account);
                sub_premises.EditingNewRecord = false;
            } else
            {
                if (sub_premises.Update(subPremise) == -1)
                    return;
                var row = sub_premises.Select().Rows.Find(subPremise.IdSubPremises);
                row["id_state"] = ViewportHelper.ValueOrDbNull(subPremise.IdState);
                row["sub_premises_num"] = ViewportHelper.ValueOrDbNull(subPremise.SubPremisesNum);
                row["total_area"] = ViewportHelper.ValueOrDbNull(subPremise.TotalArea);
                row["living_area"] = ViewportHelper.ValueOrDbNull(subPremise.LivingArea);
                row["description"] = ViewportHelper.ValueOrDbNull(subPremise.Description);
                row["state_date"] = ViewportHelper.ValueOrDbNull(subPremise.StateDate);
                row["cadastral_num"] = ViewportHelper.ValueOrDbNull(subPremise.CadastralNum);
                row["cadastral_cost"] = ViewportHelper.ValueOrDbNull(subPremise.CadastralCost);
                row["balance_cost"] = ViewportHelper.ValueOrDbNull(subPremise.BalanceCost);
                row["account"] = ViewportHelper.ValueOrDbNull(subPremise.Account);
            }
            DialogResult = DialogResult.OK;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData != Keys.Enter) return base.ProcessCmdKey(ref msg, keyData);
            SendKeys.Send("{TAB}");
            return true;
        }
    }
}
