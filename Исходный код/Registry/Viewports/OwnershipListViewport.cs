using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class OwnershipListViewport : EditableDataGridViewport
    {
        #region Models
        DataModel _ownershipsRightsTypes;
        DataModel _ownershipAssoc;
        #endregion Models

        #region Views
        BindingSource _vOwnershipRightTypes;
        BindingSource _vOwnershipAssoc;
        #endregion Views

        private OwnershipListViewport()
            : this(null, null)
        {
        }

        public OwnershipListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_ownerships_rights")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        private void RebuildFilter()
        {
            var ownershipFilter = "id_ownership_right IN (0";
            for (var i = 0; i < _vOwnershipAssoc.Count; i++)
                ownershipFilter += ((DataRowView)_vOwnershipAssoc[i])["id_ownership_right"] + ",";
            ownershipFilter = ownershipFilter.TrimEnd(',');
            ownershipFilter += ")";
            GeneralBindingSource.Filter = ownershipFilter;
        }


        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_ownership_right"], 
                dataRowView["id_ownership_right_type"], 
                dataRowView["number"], 
                dataRowView["date"], 
                dataRowView["description"]
            };
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
            if (fieldName == null)
                return false;
            if (DataModelHelper.HasMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации об ограничениях муниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (!DataModelHelper.HasNotMunicipal((int) ParentRow[fieldName], entity) ||
                AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)) return true;
            MessageBox.Show(@"У вас нет прав на изменение информации об ограничениях немуниципальных объектов",
                @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            return false;
        }

        private bool ValidateViewportData(IEnumerable<Entity> list)
        {
            if (ValidatePermissions() == false)
                return false;
            foreach (var entity in list)
            {
                var ownershipRight = (OwnershipRight) entity;
                if (ownershipRight.Number != null && ownershipRight.Number.Length > 20)
                {
                    MessageBox.Show(@"Длина номера основания не может превышать 20 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.Date == null)
                {
                    MessageBox.Show(@"Не заполнена дата начала действия ограничения, установленного в отношении муниципальной собственности",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.Description != null && ownershipRight.Description.Length > 255)
                {
                    MessageBox.Show(@"Длина наименования ограничения не может превышать 255 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.IdOwnershipRightType == null)
                {
                    MessageBox.Show(@"Не выбран тип ограничения, установленного в отношении муниципальной собственности", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static OwnershipRight RowToOwnershipRight(DataRow row)
        {
            var ownershipRight = new OwnershipRight
            {
                IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right"),
                IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type"),
                Number = ViewportHelper.ValueOrNull(row, "number"),
                Date = ViewportHelper.ValueOrNull<DateTime>(row, "date"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
            return ownershipRight;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var or = new OwnershipRight();
                    var row = dataGridView.Rows[i];
                    or.IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right");
                    or.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                    or.Number = ViewportHelper.ValueOrNull(row, "number");
                    or.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
                    or.Description = ViewportHelper.ValueOrNull(row, "description");
                    list.Add(or);
                }
            }
            return list;
        }

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var or = new OwnershipRight();
                var row = ((DataRowView)GeneralBindingSource[i]);
                or.IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right");
                or.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                or.Number = ViewportHelper.ValueOrNull(row, "number");
                or.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
                or.Description = ViewportHelper.ValueOrNull(row, "description");
                list.Add(or);
            }
            return list;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance(DataModelType.OwnershipsRightsDataModel);
            _ownershipsRightsTypes = DataModel.GetInstance(DataModelType.OwnershipRightTypesDataModel);
            // Дожидаемся дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _ownershipsRightsTypes.Select();
            switch (ParentType)
            {
                case ParentTypeEnum.Premises:
                    _ownershipAssoc = DataModel.GetInstance(DataModelType.OwnershipPremisesAssocDataModel);
                    break;
                case ParentTypeEnum.Building:
                    _ownershipAssoc = DataModel.GetInstance(DataModelType.OwnershipBuildingsAssocDataModel);
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
            _ownershipAssoc.Select();

            _vOwnershipAssoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
            {
                _vOwnershipAssoc.DataMember = "ownership_premises_assoc";
                _vOwnershipAssoc.Filter = "id_premises = " + ParentRow["id_premises"];
                Text = string.Format(CultureInfo.InvariantCulture, "Ограничения помещения №{0}", ParentRow["id_premises"]);
            }
            else
                if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                {
                    _vOwnershipAssoc.DataMember = "ownership_buildings_assoc";
                    _vOwnershipAssoc.Filter = "id_building = " + ParentRow["id_building"];
                    Text = string.Format(CultureInfo.InvariantCulture, "Ограничения здания №{0}", ParentRow["id_building"]);
                }
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");
            _vOwnershipAssoc.DataSource = DataModel.DataSet;

            GeneralBindingSource = new BindingSource
            {
                DataMember = "ownership_rights",
                DataSource = DataModel.DataSet
            };
            //Перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();

            _vOwnershipRightTypes = new BindingSource
            {
                DataMember = "ownership_right_types",
                DataSource = DataModel.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName,
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource {DataSource = GeneralSnapshot};
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_ownerships_rights_CurrentItemChanged;
            GeneralSnapshot.RowChanged += snapshot_ownerships_rights_RowChanged;
            GeneralSnapshot.RowDeleted += snapshot_ownerships_rights_RowDeleted;

            dataGridView.DataSource = GeneralSnapshotBindingSource;

            id_ownership_right.DataPropertyName = "id_ownership_right";
            id_ownership_right_type.DataSource = _vOwnershipRightTypes;
            id_ownership_right_type.ValueMember = "id_ownership_right_type";
            id_ownership_right_type.DisplayMember = "ownership_right_type";
            id_ownership_right_type.DataPropertyName = "id_ownership_right_type";
            number.DataPropertyName = "number";
            date.DataPropertyName = "date";
            description.DataPropertyName = "description";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;

            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            GeneralDataModel.Select().RowChanged += OwnershipListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += OwnershipListViewport_RowDeleting;
            _ownershipAssoc.Select().RowChanged += OwnershipAssoc_RowChanged;
            _ownershipAssoc.Select().RowDeleted += OwnershipAssoc_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            if (row != null) row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralSnapshotBindingSource.Position != -1) && 
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            ((DataRowView)GeneralSnapshotBindingSource[GeneralSnapshotBindingSource.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            GeneralSnapshot.Clear();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && 
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            sync_views = false;
            GeneralDataModel.EditingNewRecord = true;
            var list = EntitiesListFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var ownershipRight = (OwnershipRight) list[i];
                var row = GeneralDataModel.Select().Rows.Find(ownershipRight.IdOwnershipRight);
                if (row == null)
                {
                    var idParent = ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] :
                        ((ParentType == ParentTypeEnum.Building) && ParentRow != null) ? (int)ParentRow["id_building"] :
                        -1;
                    if (idParent == -1)
                    {
                        MessageBox.Show(@"Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору", 
                            @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    var idOwnershipRight = GeneralDataModel.Insert(ownershipRight);
                    if (idOwnershipRight == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    var assoc = new OwnershipRightObjectAssoc(idParent, idOwnershipRight);
                    switch (ParentType)
                    {
                        case ParentTypeEnum.Building:
                            DataModel.GetInstance(DataModelType.OwnershipBuildingsAssocDataModel).Insert(assoc);
                            break;
                        case ParentTypeEnum.Premises:
                            DataModel.GetInstance(DataModelType.OwnershipPremisesAssocDataModel).Insert(assoc);
                            break;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_ownership_right"] = idOwnershipRight;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                    _ownershipAssoc.Select().Rows.Add(idParent, idOwnershipRight);
                }
                else
                {
                    if (RowToOwnershipRight(row) == ownershipRight)
                        continue;
                    if (GeneralDataModel.Update(ownershipRight) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    row["id_ownership_right_type"] = ownershipRight.IdOwnershipRightType == null ? DBNull.Value : (object)ownershipRight.IdOwnershipRightType;
                    row["number"] = ownershipRight.Number == null ? DBNull.Value : (object)ownershipRight.Number;
                    row["date"] = ownershipRight.Date == null ? DBNull.Value : (object)ownershipRight.Date;
                    row["description"] = ownershipRight.Description == null ? DBNull.Value : (object)ownershipRight.Description;
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var ownershipRight = (OwnershipRight) entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_ownership_right"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_ownership_right"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_ownership_right"].Value == ownershipRight.IdOwnershipRight))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (ownershipRight.IdOwnershipRight != null && 
                    GeneralDataModel.Delete(ownershipRight.IdOwnershipRight.Value) == -1)
                {
                    sync_views = true;
                    GeneralDataModel.EditingNewRecord = false;
                    RebuildFilter();
                    return;
                }
                GeneralDataModel.Select().Rows.Find(ownershipRight.IdOwnershipRight).Delete();
                RebuildFilter();
            }
            RebuildFilter();
            sync_views = true;
            GeneralDataModel.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new OwnershipListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения об ограничениях в базу данных?", @"Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
            }
            GeneralDataModel.Select().RowChanged -= OwnershipListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= OwnershipListViewport_RowDeleting;
            _ownershipAssoc.Select().RowChanged -= OwnershipAssoc_RowChanged;
            _ownershipAssoc.Select().RowDeleted -= OwnershipAssoc_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            GeneralDataModel.Select().RowChanged -= OwnershipListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= OwnershipListViewport_RowDeleting;
            _ownershipAssoc.Select().RowChanged -= OwnershipAssoc_RowChanged;
            _ownershipAssoc.Select().RowDeleted -= OwnershipAssoc_RowDeleted;
            Close();
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "number":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 20 ? 
                        "Длина номера основания не может превышать 20 символов" : "";
                    break;
                case "date":
                    cell.ErrorText = string.IsNullOrEmpty(cell.Value.ToString().Trim()) ? 
                        "Не заполнена дата начала действия ограничения, установленного в отношении муниципальной собственности" : "";
                    break;
                case "description":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 255 ? 
                        "Длина наименования ограничения не может превышать 255 символов" : "";
                    break;
            }
        }

        void snapshot_ownerships_rights_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void snapshot_ownerships_rights_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add && Selected)
            {
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
            }
        }

        void OwnershipAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            //Если добавлена новая ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();
            //Если в модели есть запись, а в снапшоте нет, то добавляем в снапшот
            if (e.Row["id_ownership_right"] == DBNull.Value)
                return;
            var rowIndex = GeneralBindingSource.Find("id_ownership_right", e.Row["id_ownership_right"]);
            if (rowIndex == -1)
                return;
            var row = (DataRowView)GeneralBindingSource[rowIndex];
            if ((GeneralSnapshotBindingSource.Find("id_ownership_right", e.Row["id_ownership_right"]) == -1) && (rowIndex != -1))
            {
                GeneralSnapshot.Rows.Add(row["id_ownership_right"], row["id_ownership_right_type"], row["number"], row["date"], row["description"]);
            }
        }

        void OwnershipAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            //Если удалена ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            if (e.Action == DataRowAction.Delete)
                RebuildFilter();
        }

        void OwnershipListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = GeneralSnapshotBindingSource.Find("id_ownership_right", e.Row["id_ownership_right"]);
                if (rowIndex != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
            }
        }

        void OwnershipListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            switch (e.Action)
            {
                case DataRowAction.Change:
                case DataRowAction.ChangeCurrentAndOriginal:
                case DataRowAction.ChangeOriginal:
                    var rowIndex = GeneralSnapshotBindingSource.Find("id_ownership_right", e.Row["id_ownership_right"]);
                    if (rowIndex != -1)
                    {
                        var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
                        row["id_ownership_right_type"] = e.Row["id_ownership_right_type"];
                        row["number"] = e.Row["number"];
                        row["date"] = e.Row["date"];
                        row["description"] = e.Row["description"];
                    }
                    break;
                case DataRowAction.Add:
                    rowIndex = GeneralBindingSource.Find("id_ownership_right", e.Row["id_ownership_right"]);
                    //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                    //иначе - объект не принадлежит текущему родителю
                    if (rowIndex != -1)
                        GeneralSnapshot.Rows.Add(e.Row["id_ownership_right"], e.Row["id_ownership_right_type"], e.Row["number"], e.Row["date"], e.Row["description"]);
                    break;
            }
        }

        void v_snapshot_ownerships_rights_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name == "id_ownership_right_type")
            {
                var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                if (editingControl == null) return;
                editingControl.DropDownClosed -= editingControl_DropDownClosed;
                editingControl.DropDownClosed += editingControl_DropDownClosed;
            }
        }

        void editingControl_DropDownClosed(object sender, EventArgs e)
        {
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            if (editingControl != null) dataGridView.CurrentCell.Value = editingControl.SelectedValue;
            dataGridView.EndEdit();
        }
    }
}
