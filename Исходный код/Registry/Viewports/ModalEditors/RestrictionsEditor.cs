using System;
using System.Data;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Security;

namespace Registry.Viewport.ModalEditors
{
    internal partial class RestrictionsEditor : Form
    {
        private ViewportState state = ViewportState.NewRowState;
        private Restriction restriction;
        private ParentTypeEnum parentType;
        private DataModel restrictions = EntityDataModel<Restriction>.GetInstance();
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
                switch (value)
                {
                    case ParentTypeEnum.Premises:
                        restriction_assoc = EntityDataModel<RestrictionPremisesAssoc>.GetInstance();
                        break;
                    case ParentTypeEnum.Building:
                        restriction_assoc = EntityDataModel<RestrictionBuildingAssoc>.GetInstance();
                        break;
                    default:
                        throw new ViewportException("Неизвестный тип родительского объекта");
                }
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
                    Text = @"Изменить реквизит";
                    vButtonSave.Text = @"Изменить";
                }
                else
                {
                    Text = @"Добавить реквизит";
                    vButtonSave.Text = @"Добавить";
                }
                state = value;
            }
        }

        public Restriction RestrictionValue
        {
            get
            {
                var restrictionValue = new Restriction
                {
                    Date = ViewportHelper.ValueOrNull(dateTimePickerRestrictionDate),
                    Description = ViewportHelper.ValueOrNull(textBoxRestrictionDescription),
                    Number = ViewportHelper.ValueOrNull(textBoxRestrictionNumber),
                    IdRestrictionType = ViewportHelper.ValueOrNull<int>(comboBoxIdRestrictionType),
                    DateStateReg = ViewportHelper.ValueOrNull(dateTimePickerRestrictionDateStateReg)
                };
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
                dateTimePickerRestrictionDate.Value = value.Date ?? DateTime.Now;
                dateTimePickerRestrictionDateStateReg.Checked = value.DateStateReg != null;
                dateTimePickerRestrictionDateStateReg.Value = value.DateStateReg ?? DateTime.Now;
                comboBoxIdRestrictionType.SelectedValue = value.IdRestrictionType;
            }
        }
        
        public RestrictionsEditor()
        {
            InitializeComponent();
            restriction_types = EntityDataModel<RestrictionType>.GetInstance();
            v_restriction_types = new BindingSource {DataSource = restriction_types.Select()};
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
            if (OtherService.HasMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации о реквизитах НПА муниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (OtherService.HasNotMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации о реквизитах НПА немуниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                MessageBox.Show(@"Не выбран тип реквизита", @"Ошибка",
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
            var idParent = (ParentType == ParentTypeEnum.Premises) && ParentRow != null ? (int)ParentRow["id_premises"] :
                        (ParentType == ParentTypeEnum.Building) && ParentRow != null ? (int)ParentRow["id_building"] : -1;
            if (state == ViewportState.NewRowState)
            {
                if (idParent == -1)
                {
                    MessageBox.Show(@"Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                var idRestriction = restrictions.Insert(restriction);
                if (idRestriction == -1)
                    return;
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        EntityDataModel<RestrictionBuildingAssoc>.GetInstance().Insert(new RestrictionBuildingAssoc(idParent, idRestriction, null));
                        break;
                    case ParentTypeEnum.Premises:
                        EntityDataModel<RestrictionPremisesAssoc>.GetInstance().Insert(new RestrictionPremisesAssoc(idParent, idRestriction, null));
                        break;
                }
                restrictions.EditingNewRecord = true;
                restrictions.Select().Rows.Add(idRestriction, restriction.IdRestrictionType, restriction.Number, restriction.Date, restriction.Description, restriction.DateStateReg);
                restriction_assoc.Select().Rows.Add(idParent, idRestriction);
                restrictions.EditingNewRecord = false;
            } else
            {
                if (restrictions.Update(restriction) == -1)
                    return;
                var row = restrictions.Select().Rows.Find(restriction.IdRestriction);
                EntityConverter<Restriction>.FillRow(restriction, row);
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
