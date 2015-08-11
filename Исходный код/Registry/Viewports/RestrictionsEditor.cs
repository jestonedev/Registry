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
    public partial class RestrictionsEditor : Form
    {
        private ViewportState state = ViewportState.NewRowState;
        private Restriction restriction;
        private ParentTypeEnum parentType;
        private RestrictionsDataModel restrictions = RestrictionsDataModel.GetInstance();
        private DataModel restriction_assoc = null;
        private RestrictionTypesDataModel restriction_types = null;
        private BindingSource v_restriction_types = null;

        public ParentTypeEnum ParentType
        {
            get
            {
                return parentType;
            }
            set
            {
                if (value == ParentTypeEnum.Premises)
                    restriction_assoc = RestrictionsPremisesAssocDataModel.GetInstance();
                else
                    if (value == ParentTypeEnum.Building)
                        restriction_assoc = RestrictionsBuildingsAssocDataModel.GetInstance();
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
                    Text = "Изменить реквизит";
                    vButtonSave.Text = "Изменить";
                }
                else
                {
                    Text = "Добавить реквизит";
                    vButtonSave.Text = "Добавить";
                }
                state = value;
            }
        }

        public Restriction RestrictionValue
        {
            get
            {
                Restriction restrictionValue = new Restriction();
                restrictionValue.Date = ViewportHelper.ValueOrNull(dateTimePickerRestrictionDate);
                restrictionValue.Description = ViewportHelper.ValueOrNull(textBoxRestrictionDescription);
                restrictionValue.Number = ViewportHelper.ValueOrNull(textBoxRestrictionNumber);
                restrictionValue.IdRestrictionType = ViewportHelper.ValueOrNull<int>(comboBoxIdRestrictionType);
                if (state == ViewportState.ModifyRowState)
                    restrictionValue.IdRestriction = restriction.IdRestriction;
                return restrictionValue;
            }
            set
            {
                restriction = value;
                if (value == null)
                    return;
                textBoxRestrictionNumber.Text = value.Number;
                textBoxRestrictionDescription.Text = value.Description;
                dateTimePickerRestrictionDate.Value = value.Date == null ? DateTime.Now : value.Date.Value;
                comboBoxIdRestrictionType.SelectedValue = value.IdRestrictionType;
            }
        }
        
        public RestrictionsEditor()
        {
            InitializeComponent();
            restriction_types = RestrictionTypesDataModel.GetInstance();
            v_restriction_types = new BindingSource();
            v_restriction_types.DataSource = restriction_types.Select();
            comboBoxIdRestrictionType.DataSource = v_restriction_types;
            comboBoxIdRestrictionType.ValueMember = "id_restriction_type";
            comboBoxIdRestrictionType.DisplayMember = "restriction_type";
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
                MessageBox.Show("У вас нет прав на изменение информации о реквизитах НПА муниципальных объектов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.HasNotMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show("У вас нет прав на изменение информации о реквизитах НПА немуниципальных объектов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private bool ValidateData(Restriction restriction)
        {
            if (ValidatePermissions() == false)
                return false;
            if (restriction.IdRestrictionType == null)
            {
                MessageBox.Show("Не выбран тип реквизита", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            Restriction restriction = RestrictionValue;
            if (!ValidateData(restriction))
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
                int id_restriction = RestrictionsDataModel.Insert(restriction, ParentType, id_parent);
                if (id_restriction == -1)
                    return;
                restrictions.EditingNewRecord = true;
                restrictions.Select().Rows.Add(
                    new object[] { 
                        id_restriction, 
                        restriction.IdRestrictionType, 
                        restriction.Number, 
                        restriction.Date, 
                        restriction.Description
                    }
                );
                restriction_assoc.Select().Rows.Add(new object[] { id_parent, id_restriction });
                restrictions.EditingNewRecord = false;
            } else
            {
                if (RestrictionsDataModel.Update(restriction) == -1)
                    return;
                DataRow row = restrictions.Select().Rows.Find(restriction.IdRestriction);
                row["id_restriction_type"] = restriction.IdRestrictionType == null ? DBNull.Value : (object)restriction.IdRestrictionType;
                row["number"] = restriction.Number == null ? DBNull.Value : (object)restriction.Number;
                row["date"] = restriction.Date == null ? DBNull.Value : (object)restriction.Date;
                row["description"] = restriction.Description == null ? DBNull.Value : (object)restriction.Description;
            }
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                    Int32.Parse(ParentRow["id_building"].ToString(), CultureInfo.InvariantCulture), true);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
