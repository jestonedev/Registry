using Registry.CalcDataModels;
using Registry.DataModels;
using Registry.Entities;
using Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Registry.Viewport
{
    public partial class SubPremisesEditor : Form
    {
        private ViewportState state = ViewportState.NewRowState;
        private SubPremise subPremise;
        private ParentTypeEnum parentType;
        private SubPremisesDataModel sub_premises = SubPremisesDataModel.GetInstance();
        private ObjectStatesDataModel object_states = null;
        private BindingSource v_object_states = null;

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

        public SubPremise SubPremise_
        {
            get
            {
                SubPremise subPremise_ = new SubPremise();
                subPremise_.TotalArea = (double)numericUpDownTotalArea.Value;
                subPremise_.Description = ViewportHelper.ValueOrNull(textBoxDescription);
                subPremise_.SubPremisesNum = ViewportHelper.ValueOrNull(textBoxSubPremisesNum);
                subPremise_.IdState = ViewportHelper.ValueOrNull<int>(comboBoxIdState);
                if (state == ViewportState.ModifyRowState)
                    subPremise_.IdSubPremises = subPremise.IdSubPremises;
                return subPremise_;
            }
            set
            {
                subPremise = value;
                if (value == null)
                    return;
                textBoxSubPremisesNum.Text = value.SubPremisesNum;
                textBoxDescription.Text = value.Description;
                numericUpDownTotalArea.Value = value.TotalArea == null ? 0 : (decimal)value.TotalArea;
                comboBoxIdState.SelectedValue = value.IdState;
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
            if (new int[] { 4, 5 }.Contains(subPremise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("У вас нет прав на добавление в базу муниципальных жилых помещений", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (new int[] { 1, 3 }.Contains(subPremise.IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
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
            SubPremise subPremise = SubPremise_;
            if (!ValidateData(subPremise))
                return;
            int id_parent = ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] : -1;
            subPremise.IdPremises = id_parent;
            if (state == ViewportState.NewRowState)
            {
                if (id_parent == -1)
                {
                    MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                int id_sub_premise = SubPremisesDataModel.Insert(subPremise);
                if (id_sub_premise == -1)
                    return;
                sub_premises.Select().Rows.Add(
                    new object[] { 
                        id_sub_premise, 
                        subPremise.IdPremises,
                        subPremise.IdState, 
                        subPremise.SubPremisesNum, 
                        subPremise.TotalArea, 
                        subPremise.Description
                    }
                );
            } else
            {
                if (SubPremisesDataModel.Update(subPremise) == -1)
                    return;
                DataRow row = sub_premises.Select().Rows.Find(subPremise.IdSubPremises);
                row["id_state"] = subPremise.IdState == null ? DBNull.Value : (object)subPremise.IdState;
                row["sub_premises_num"] = subPremise.SubPremisesNum == null ? DBNull.Value : (object)subPremise.SubPremisesNum;
                row["total_area"] = subPremise.TotalArea == null ? DBNull.Value : (object)subPremise.TotalArea;
                row["description"] = subPremise.Description == null ? DBNull.Value : (object)subPremise.Description;
            }
            CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
            CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
            CalcDataModelPremiseSubPremisesSumArea.GetInstance().Refresh(EntityType.Premise,
                Int32.Parse(ParentRow["id_premises"].ToString(), CultureInfo.InvariantCulture), true);
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                Int32.Parse(ParentRow["id_building"].ToString(), CultureInfo.InvariantCulture), true);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
