using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.CalcDataModels;
using Registry.DataModels;
using Registry.Entities;
using Security;

namespace Registry.Viewport
{
    public partial class SubPremisesEditor : Form
    {
        private ViewportState state = ViewportState.NewRowState;
        private SubPremise subPremise;
        private ParentTypeEnum parentType;
        private SubPremisesDataModel sub_premises = SubPremisesDataModel.GetInstance();
        private ObjectStatesDataModel object_states;
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
                    Text = "Изменить комнату";
                    vButtonSave.Text = "Изменить";
                }
                else
                {
                    Text = "Добавить комнату";
                    vButtonSave.Text = "Добавить";
                }
                state = value;
            }
        }

        public SubPremise SubPremiseValue
        {
            get
            {
                var subPremiseValue = new SubPremise();
                subPremiseValue.TotalArea = (double)numericUpDownTotalArea.Value;
                if ((double)numericUpDownLivingArea.Value == 0)
                    subPremiseValue.LivingArea = (double)numericUpDownTotalArea.Value;
                else
                    subPremiseValue.LivingArea = (double)numericUpDownLivingArea.Value;
                subPremiseValue.Description = ViewportHelper.ValueOrNull(textBoxDescription);
                subPremiseValue.SubPremisesNum = ViewportHelper.ValueOrNull(textBoxSubPremisesNum);
                subPremiseValue.IdState = ViewportHelper.ValueOrNull<int>(comboBoxIdState);
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
            object_states = ObjectStatesDataModel.GetInstance();
            v_object_states = new BindingSource();
            v_object_states.DataSource = object_states.Select();
            comboBoxIdState.DataSource = v_object_states;
            comboBoxIdState.ValueMember = "id_state";
            comboBoxIdState.DisplayMember = "state_female";
        }

        private bool ValidatePermissions(SubPremise subPremise)
        {
            if (new[] { 4, 5, 9 }.Contains(subPremise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("У вас нет прав на добавление в базу муниципальных жилых помещений", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (new[] { 1, 3, 6, 7, 8 }.Contains(subPremise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show("У вас нет прав на добавление в базу немуниципальных жилых помещений", "Ошибка",
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
                MessageBox.Show("Необходимо выбрать состояние помещения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (subPremise.SubPremisesNum != null && !Regex.IsMatch(subPremise.SubPremisesNum, "^([0-9]+[а-я]{0,1}|[а-я])$"))
            {
                MessageBox.Show("Некорректно задан номер комнаты. Можно использовать только цифры и не более одной строчной буквы кириллицы",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            var subPremise = SubPremiseValue;
            if (!ValidateData(subPremise))
                return;
            var id_parent = ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] : -1;
            subPremise.IdPremises = id_parent;
            if (state == ViewportState.NewRowState)
            {
                if (id_parent == -1)
                {
                    MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                sub_premises.EditingNewRecord = true;
                var id_sub_premise = SubPremisesDataModel.Insert(subPremise);
                if (id_sub_premise == -1)
                    return;
                sub_premises.Select().Rows.Add(id_sub_premise, subPremise.IdPremises, subPremise.IdState, subPremise.SubPremisesNum, subPremise.TotalArea, subPremise.LivingArea, subPremise.Description, subPremise.StateDate);
                sub_premises.EditingNewRecord = false;
            } else
            {
                if (SubPremisesDataModel.Update(subPremise) == -1)
                    return;
                var row = sub_premises.Select().Rows.Find(subPremise.IdSubPremises);
                row["id_state"] = subPremise.IdState == null ? DBNull.Value : (object)subPremise.IdState;
                row["sub_premises_num"] = subPremise.SubPremisesNum == null ? DBNull.Value : (object)subPremise.SubPremisesNum;
                row["total_area"] = subPremise.TotalArea == null ? DBNull.Value : (object)subPremise.TotalArea;
                row["living_area"] = subPremise.LivingArea == null ? DBNull.Value : (object)subPremise.LivingArea;
                row["description"] = subPremise.Description == null ? DBNull.Value : (object)subPremise.Description;
                row["state_date"] = subPremise.StateDate == null ? DBNull.Value : (object)subPremise.StateDate;
            }
            CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
            CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
            CalcDataModelPremiseSubPremisesSumArea.GetInstance().Refresh(EntityType.Premise,
                int.Parse(ParentRow["id_premises"].ToString(), CultureInfo.InvariantCulture), true);
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                int.Parse(ParentRow["id_building"].ToString(), CultureInfo.InvariantCulture), true);
            DialogResult = DialogResult.OK;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
