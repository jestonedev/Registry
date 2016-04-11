using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;

namespace Registry.Viewport
{
    internal partial class RestrictionsEditor : Form
    {
        private ViewportState state = ViewportState.NewRowState;
        private Restriction restriction;
        private ParentTypeEnum parentType;
        private DataModel restrictions = DataModel.GetInstance<RestrictionsDataModel>();
        private DataModel restriction_assoc;
        private DataModel restriction_types;
        private BindingSource v_restriction_types;

        public ParentTypeEnum ParentType
        {
            get
            {
                return parentType;
            }
            set
            {
                if (value == ParentTypeEnum.Premises)
                    restriction_assoc = DataModel.GetInstance<RestrictionsPremisesAssocDataModel>();
                else
                    if (value == ParentTypeEnum.Building)
                        restriction_assoc = DataModel.GetInstance<RestrictionsBuildingsAssocDataModel>();
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
                var restrictionValue = new Restriction();
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
            restriction_types = DataModel.GetInstance<RestrictionTypesDataModel>();
            v_restriction_types = new BindingSource();
            v_restriction_types.DataSource = restriction_types.Select();
            comboBoxIdRestrictionType.DataSource = v_restriction_types;
            comboBoxIdRestrictionType.ValueMember = "id_restriction_type";
            comboBoxIdRestrictionType.DisplayMember = "restriction_type";
        }

        private bool ValidatePermissions()
        {
            var entity = EntityType.Unknown;
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
            var restriction = RestrictionValue;
            if (!ValidateData(restriction))
                return;
            var id_parent = ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] :
                        ((ParentType == ParentTypeEnum.Building) && ParentRow != null) ? (int)ParentRow["id_building"] : -1;
            if (state == ViewportState.NewRowState)
            {
                if (id_parent == -1)
                {
                    MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                var id_restriction = restrictions.Insert(restriction);
                if (id_restriction == -1)
                    return;
                var assoc = new RestrictionObjectAssoc(id_parent, id_restriction, null);
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        DataModel.GetInstance<RestrictionsBuildingsAssocDataModel>().Insert(assoc);
                        break;
                    case ParentTypeEnum.Premises:
                        DataModel.GetInstance<RestrictionsPremisesAssocDataModel>().Insert(assoc);
                        break;
                }
                restrictions.EditingNewRecord = true;
                restrictions.Select().Rows.Add(id_restriction, restriction.IdRestrictionType, restriction.Number, restriction.Date, restriction.Description);
                restriction_assoc.Select().Rows.Add(id_parent, id_restriction);
                restrictions.EditingNewRecord = false;
            } else
            {
                if (restrictions.Update(restriction) == -1)
                    return;
                var row = restrictions.Select().Rows.Find(restriction.IdRestriction);
                row["id_restriction_type"] = restriction.IdRestrictionType == null ? DBNull.Value : (object)restriction.IdRestrictionType;
                row["number"] = restriction.Number == null ? DBNull.Value : (object)restriction.Number;
                row["date"] = restriction.Date == null ? DBNull.Value : (object)restriction.Date;
                row["description"] = restriction.Description == null ? DBNull.Value : (object)restriction.Description;
            }
            DialogResult = DialogResult.OK;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
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
