using System;
using System.Data;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Security;

namespace Registry.Viewport.ModalEditors
{
    internal partial class OwnershipsEditor : Form
    {
        private ViewportState state = ViewportState.NewRowState;
        private OwnershipRight ownershipRight;
        private ParentTypeEnum parentType;
        private DataModel ownership_rights = EntityDataModel<OwnershipRight>.GetInstance();
        private DataModel ownership_assoc;
        private DataModel ownership_right_types;
        private BindingSource v_ownership_right_types;

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
                        ownership_assoc = EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance();
                        break;
                    case ParentTypeEnum.Building:
                        ownership_assoc = EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance();
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
                    Text = @"Изменить ограничение";
                    vButtonSave.Text = @"Изменить";
                }
                else
                {
                    Text = @"Добавить ограничение";
                    vButtonSave.Text = @"Добавить";
                }
                state = value;
            }
        }

        public OwnershipRight OwnershipRightValue
        {
            get
            {
                var ownershipRightValue = new OwnershipRight
                {
                    Date = ViewportHelper.ValueOrNull(dateTimePickerOwnershipDate),
                    Description = ViewportHelper.ValueOrNull(textBoxOwnershipDescription),
                    Number = ViewportHelper.ValueOrNull(textBoxOwnershipNumber),
                    IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(comboBoxIdOwnershipType)
                };
                if (state == ViewportState.ModifyRowState)
                    ownershipRightValue.IdOwnershipRight = ownershipRight.IdOwnershipRight;
                return ownershipRightValue;
            }
            set
            {
                ownershipRight = value;
                if (value == null)
                    return;
                textBoxOwnershipNumber.Text = value.Number;
                textBoxOwnershipDescription.Text = value.Description;
                dateTimePickerOwnershipDate.Value = value.Date ?? DateTime.Now;
                comboBoxIdOwnershipType.SelectedValue = value.IdOwnershipRightType;
            }
        }

        public OwnershipsEditor()
        {
            InitializeComponent();
            ownership_right_types = EntityDataModel<OwnershipRightType>.GetInstance();
            v_ownership_right_types = new BindingSource {DataSource = ownership_right_types.Select()};
            comboBoxIdOwnershipType.DataSource = v_ownership_right_types;
            comboBoxIdOwnershipType.ValueMember = "id_ownership_right_type";
            comboBoxIdOwnershipType.DisplayMember = "ownership_right_type";
        }

        private bool ValidatePermissions()
        {
            var entity = EntityType.Unknown;
            string fieldName = null;
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    entity = EntityType.Building;
                    fieldName = "id_building";
                    break;
                case ParentTypeEnum.Premises:
                    entity = EntityType.Premise;
                    fieldName = "id_premises";
                    break;
            }
            if (OtherService.HasMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации об ограничениях муниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (OtherService.HasNotMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации об ограничениях немуниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                MessageBox.Show(@"Не выбран тип ограничения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private void vButtonSave_Click(object sender, EventArgs e)
        {
            var ownershipRight = OwnershipRightValue;
            if (!ValidateData(ownershipRight))
                return;
            var id_parent = ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] :
                        ((ParentType == ParentTypeEnum.Building) && ParentRow != null) ? (int)ParentRow["id_building"] : -1;
            if (state == ViewportState.NewRowState)
            {
                if (id_parent == -1)
                {
                    MessageBox.Show(@"Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                ownership_rights.EditingNewRecord = true;
                var idOwnershipRight = ownership_rights.Insert(ownershipRight);
                if (idOwnershipRight == -1)
                    return;
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance().Insert(
                            new OwnershipRightBuildingAssoc(id_parent, idOwnershipRight));
                        break;
                    case ParentTypeEnum.Premises:
                        EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance().Insert(
                            new OwnershipRightPremisesAssoc(id_parent, idOwnershipRight));
                        break;
                }
                ownership_rights.Select().Rows.Add(idOwnershipRight, ownershipRight.IdOwnershipRightType, 
                    ownershipRight.Number, ownershipRight.Date, ownershipRight.Description);
                ownership_assoc.Select().Rows.Add(id_parent, idOwnershipRight);
                ownership_rights.EditingNewRecord = false;
            } else
            {
                if (ownership_rights.Update(ownershipRight) == -1)
                    return;
                var row = ownership_rights.Select().Rows.Find(ownershipRight.IdOwnershipRight);
                row["id_ownership_right_type"] = ownershipRight.IdOwnershipRightType == null ? DBNull.Value : (object)ownershipRight.IdOwnershipRightType;
                row["number"] = ownershipRight.Number == null ? DBNull.Value : (object)ownershipRight.Number;
                row["date"] = ownershipRight.Date == null ? DBNull.Value : (object)ownershipRight.Date;
                row["description"] = ownershipRight.Description == null ? DBNull.Value : (object)ownershipRight.Description;
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
