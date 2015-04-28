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
using System.Windows.Forms;

namespace Registry.Viewport
{
    public partial class OwnershipsEditor : Form
    {
        private ViewportState state = ViewportState.NewRowState;
        private OwnershipRight ownershipRight;
        private ParentTypeEnum parentType;
        private OwnershipsRightsDataModel ownership_rights = OwnershipsRightsDataModel.GetInstance();
        private DataModel ownership_assoc = null;
        private OwnershipRightTypesDataModel ownership_right_types = null;
        private BindingSource v_ownership_right_types = null;

        public ParentTypeEnum ParentType
        {
            get
            {
                return parentType;
            }
            set
            {
                if (value == ParentTypeEnum.Premises)
                    ownership_assoc = OwnershipPremisesAssocDataModel.GetInstance();
                else
                    if (value == ParentTypeEnum.Building)
                        ownership_assoc = OwnershipBuildingsAssocDataModel.GetInstance();
                    else
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
                    Text = "Изменить ограничение";
                    vButtonSave.Text = "Изменить";
                }
                else
                {
                    Text = "Добавить ограничение";
                    vButtonSave.Text = "Добавить";
                }
                state = value;
            }
        }

        public OwnershipRight OwnershipRight_
        {
            get
            {
                OwnershipRight ownershipRight_ = new OwnershipRight();
                ownershipRight_.Date = ViewportHelper.ValueOrNull(dateTimePickerOwnershipDate);
                ownershipRight_.Description = ViewportHelper.ValueOrNull(textBoxOwnershipDescription);
                ownershipRight_.Number = ViewportHelper.ValueOrNull(textBoxOwnershipNumber);
                ownershipRight_.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(comboBoxIdOwnershipType);
                if (state == ViewportState.ModifyRowState)
                    ownershipRight_.IdOwnershipRight = ownershipRight.IdOwnershipRight;
                return ownershipRight_;
            }
            set
            {
                ownershipRight = value;
                if (value == null)
                    return;
                textBoxOwnershipNumber.Text = value.Number;
                textBoxOwnershipDescription.Text = value.Description;
                dateTimePickerOwnershipDate.Value = value.Date == null ? DateTime.Now : value.Date.Value;
                comboBoxIdOwnershipType.SelectedValue = value.IdOwnershipRightType;
            }
        }

        public OwnershipsEditor()
        {
            InitializeComponent();
            ownership_right_types = OwnershipRightTypesDataModel.GetInstance();
            v_ownership_right_types = new BindingSource();
            v_ownership_right_types.DataSource = ownership_right_types.Select();
            comboBoxIdOwnershipType.DataSource = v_ownership_right_types;
            comboBoxIdOwnershipType.ValueMember = "id_ownership_right_type";
            comboBoxIdOwnershipType.DisplayMember = "ownership_right_type";
        }

        private bool ValidatePermissions()
        {
            EntityType entity = EntityType.Unknown;
            string fieldName = null;
            if (ParentType == ParentTypeEnum.Building)
            {
                entity = EntityType.Building;
                fieldName = "id_building";
            }
            else
                if (ParentType == ParentTypeEnum.Premises)
                {
                    entity = EntityType.Premise;
                    fieldName = "id_premises";
                }
            if (DataModelHelper.HasMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("У вас нет прав на изменение информации об ограничениях муниципальных объектов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.HasNotMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show("У вас нет прав на изменение информации об ограничениях немуниципальных объектов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private bool ValidateData(OwnershipRight ownershipRight)
        {
            if (ValidatePermissions() == false)
                return false;
            if (ownershipRight.IdOwnershipRightType == null)
            {
                MessageBox.Show("Не выбран тип ограничения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            OwnershipRight ownershipRight = OwnershipRight_;
            if (!ValidateData(ownershipRight))
                return;
            int id_parent = ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] :
                        ((ParentType == ParentTypeEnum.Building) && ParentRow != null) ? (int)ParentRow["id_building"] : -1;
            if (state == ViewportState.NewRowState)
            {
                if (id_parent == -1)
                {
                    MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                int id_ownership_right = OwnershipsRightsDataModel.Insert(ownershipRight, ParentType, id_parent);
                if (id_ownership_right == -1)
                    return;
                ownership_rights.Select().Rows.Add(
                    new object[] { 
                        id_ownership_right, 
                        ownershipRight.IdOwnershipRightType, 
                        ownershipRight.Number, 
                        ownershipRight.Date, 
                        ownershipRight.Description
                    }
                );
                ownership_assoc.Select().Rows.Add(new object[] { id_parent, id_ownership_right });
            } else
            {
                if (OwnershipsRightsDataModel.Update(ownershipRight) == -1)
                    return;
                DataRow row = ownership_rights.Select().Rows.Find(ownershipRight.IdOwnershipRight);
                row["id_ownership_right_type"] = ownershipRight.IdOwnershipRightType == null ? DBNull.Value : (object)ownershipRight.IdOwnershipRightType;
                row["number"] = ownershipRight.Number == null ? DBNull.Value : (object)ownershipRight.Number;
                row["date"] = ownershipRight.Date == null ? DBNull.Value : (object)ownershipRight.Date;
                row["description"] = ownershipRight.Description == null ? DBNull.Value : (object)ownershipRight.Description;
            }
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                    Int32.Parse(ParentRow["id_building"].ToString(), CultureInfo.InvariantCulture), true);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
