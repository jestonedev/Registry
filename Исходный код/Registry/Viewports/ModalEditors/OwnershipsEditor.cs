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
    internal partial class OwnershipsEditor : Form
    {
        private ViewportState state = ViewportState.NewRowState;
        private OwnershipRight ownershipRight;
        private ParentTypeEnum parentType;
        private DataModel ownership_rights = DataModel.GetInstance<OwnershipsRightsDataModel>();
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
                        ownership_assoc = DataModel.GetInstance<OwnershipPremisesAssocDataModel>();
                        break;
                    case ParentTypeEnum.Building:
                        ownership_assoc = DataModel.GetInstance<OwnershipBuildingsAssocDataModel>();
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
            ownership_right_types = DataModel.GetInstance<OwnershipRightTypesDataModel>();
            v_ownership_right_types = new BindingSource {DataSource = ownership_right_types.Select()};
            comboBoxIdOwnershipType.DataSource = v_ownership_right_types;
            comboBoxIdOwnershipType.ValueMember = "id_ownership_right_type";
            comboBoxIdOwnershipType.DisplayMember = "ownership_right_type";
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
            var ownershipRight = OwnershipRightValue;
            if (!ValidateData(ownershipRight))
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
                ownership_rights.EditingNewRecord = true;
                var id_ownership_right = ownership_rights.Insert(ownershipRight);
                if (id_ownership_right == -1)
                    return;
                var assoc = new OwnershipRightObjectAssoc(id_parent, id_ownership_right);
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        DataModel.GetInstance<OwnershipBuildingsAssocDataModel>().Insert(assoc);
                        break;
                    case ParentTypeEnum.Premises:
                        DataModel.GetInstance<OwnershipPremisesAssocDataModel>().Insert(assoc);
                        break;
                }
                ownership_rights.Select().Rows.Add(id_ownership_right, ownershipRight.IdOwnershipRightType, ownershipRight.Number, ownershipRight.Date, ownershipRight.Description);
                ownership_assoc.Select().Rows.Add(id_parent, id_ownership_right);
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
