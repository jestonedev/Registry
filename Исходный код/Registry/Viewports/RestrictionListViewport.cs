using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class RestrictionListViewport : EditableDataGridViewport
    {
        #region Models

        private DataModel _restrictionTypes;
        private DataModel _restrictionAssoc;
        #endregion Models

        #region Views

        private BindingSource _vRestrictionTypes;
        private BindingSource _vRestrictionAssoc;
        #endregion Views

        private RestrictionListViewport()
            : this(null, null)
        {
        }

        public RestrictionListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_restrictions")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        private void RebuildFilter()
        {
            var restrictionFilter = "id_restriction IN (0";
            for (var i = 0; i < _vRestrictionAssoc.Count; i++)
                restrictionFilter += ((DataRowView)_vRestrictionAssoc[i])["id_restriction"] + ",";
            restrictionFilter = restrictionFilter.TrimEnd(',');
            restrictionFilter += ")";
            GeneralBindingSource.Filter = restrictionFilter;
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
                MessageBox.Show(@"У вас нет прав на изменение информации о реквизитах НПА муниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.HasNotMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации о реквизитах НПА немуниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private bool ValidateViewportData(IEnumerable<Entity> list)
        {
            if (ValidatePermissions() == false)
                return false;
            foreach (var entity in list)
            {
                var restriction = (Restriction) entity;
                if (restriction.Number != null && restriction.Number.Length > 10)
                {
                    MessageBox.Show(@"Номер реквизита не может привышать 10 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restriction.Date == null)
                {
                    MessageBox.Show(@"Не заполнена дата", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restriction.Description != null && restriction.Description.Length > 255)
                {
                    MessageBox.Show(@"Длина наименования реквизита не может превышать 255 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restriction.IdRestrictionType == null)
                {
                    MessageBox.Show(@"Не выбран тип реквизита", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var row = dataGridView.Rows[i];
                list.Add(RestrictionConverter.FromRow(row));
            }
            return list;
        }

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var row = (DataRowView)GeneralBindingSource[i];
                list.Add(RestrictionConverter.FromRow(row));
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
            GeneralDataModel = DataModel.GetInstance<RestrictionsDataModel>();
            _restrictionTypes = DataModel.GetInstance<RestrictionTypesDataModel>();
            // Дожидаемся дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _restrictionTypes.Select();

            switch (ParentType)
            {
                case ParentTypeEnum.Premises:
                    _restrictionAssoc =  DataModel.GetInstance<RestrictionsPremisesAssocDataModel>();
                    break;
                case ParentTypeEnum.Building:
                    _restrictionAssoc = DataModel.GetInstance<RestrictionsBuildingsAssocDataModel>();
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
            _restrictionAssoc.Select();

            _vRestrictionAssoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
            {
                _vRestrictionAssoc.DataMember = "restrictions_premises_assoc";
                _vRestrictionAssoc.Filter = "id_premises = " + ParentRow["id_premises"];
                Text = string.Format(CultureInfo.InvariantCulture, "Реквизиты помещения №{0}", ParentRow["id_premises"]);
            }
            else
                if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                {
                    _vRestrictionAssoc.DataMember = "restrictions_buildings_assoc";
                    _vRestrictionAssoc.Filter = "id_building = " + ParentRow["id_building"];
                    Text = string.Format(CultureInfo.InvariantCulture, "Реквизиты здания №{0}", ParentRow["id_building"]);
                }
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");
            _vRestrictionAssoc.DataSource = DataModel.DataSet;

            GeneralBindingSource = new BindingSource
            {
                DataMember = "restrictions",
                DataSource = DataModel.DataSet
            };
            //Перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();

            _vRestrictionTypes = new BindingSource
            {
                DataMember = "restriction_types",
                DataSource = DataModel.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName,
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(RestrictionConverter.ToArray((DataRowView)GeneralBindingSource[i]));
            GeneralSnapshotBindingSource = new BindingSource { DataSource = GeneralSnapshot };
            AddEventHandler<EventArgs>(GeneralSnapshotBindingSource, "CurrentItemChanged", v_snapshot_restrictions_CurrentItemChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralSnapshot, "RowChanged", snapshot_restrictions_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralSnapshot, "RowDeleted", snapshot_restrictions_RowDeleted);

            dataGridView.DataSource = GeneralSnapshotBindingSource;

            id_restriction.DataPropertyName = "id_restriction";
            id_restriction_type.DataSource = _vRestrictionTypes;
            id_restriction_type.ValueMember = "id_restriction_type";
            id_restriction_type.DisplayMember = "restriction_type";
            id_restriction_type.DataPropertyName = "id_restriction_type";
            number.DataPropertyName = "number";
            date.DataPropertyName = "date";
            description.DataPropertyName = "description";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            AddEventHandler<DataGridViewCellEventArgs>(dataGridView, "CellValidated", dataGridView_CellValidated);

            //События изменения данных для проверки соответствия реальным данным в модели
            AddEventHandler<DataGridViewCellEventArgs>(dataGridView, "CellValueChanged", dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", RestrictionListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleting", RestrictionListViewport_RowDeleting);

            AddEventHandler<DataRowChangeEventArgs>(_restrictionAssoc.Select(), "RowChanged", RestrictionAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_restrictionAssoc.Select(), "RowDeleted", RestrictionAssoc_RowDeleted);
        }
        
        public override bool CanInsertRecord()
        {
            return AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            if (row != null) row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralSnapshotBindingSource.Position != -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
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
                GeneralSnapshot.Rows.Add(RestrictionConverter.ToArray((DataRowView)GeneralBindingSource[i]));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void SaveRecord()
        {
            SyncViews = false;
            dataGridView.EndEdit();
            GeneralDataModel.EditingNewRecord = true;
            var list = EntitiesListFromViewport();
            if (!ValidateViewportData(list))
            {
                SyncViews = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var restriction = (Restriction) list[i];
                var row = GeneralDataModel.Select().Rows.Find(restriction.IdRestriction);
                if (row == null)
                {
                    var idParent = (ParentType == ParentTypeEnum.Premises) && ParentRow != null ? (int)ParentRow["id_premises"] :
                        (ParentType == ParentTypeEnum.Building) && ParentRow != null ? (int)ParentRow["id_building"] :
                        -1;
                    if (idParent == -1)
                    {
                        MessageBox.Show(@"Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                            @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        SyncViews = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    var idRestriction = GeneralDataModel.Insert(restriction);
                    if (idRestriction == -1)
                    {
                        SyncViews = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    var assoc = new RestrictionObjectAssoc(idParent, idRestriction, null);
                    switch (ParentType)
                    {
                        case ParentTypeEnum.Building:
                            DataModel.GetInstance<RestrictionsBuildingsAssocDataModel>().Insert(assoc);
                            break;
                        case ParentTypeEnum.Premises:
                            DataModel.GetInstance<RestrictionsPremisesAssocDataModel>().Insert(assoc);
                            break;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_restriction"] = idRestriction;
                    GeneralDataModel.Select().Rows.Add(RestrictionConverter.ToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                    _restrictionAssoc.Select().Rows.Add(idParent, idRestriction);
                }
                else
                {
                    if (RestrictionConverter.FromRow(row) == restriction)
                        continue;
                    if (GeneralDataModel.Update(restriction) == -1)
                    {
                        SyncViews = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    RestrictionConverter.FillRow(restriction, row);
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var restriction = (Restriction) entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_restriction"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_restriction"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_restriction"].Value == restriction.IdRestriction))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (restriction.IdRestriction != null && 
                    GeneralDataModel.Delete(restriction.IdRestriction.Value) == -1)
                {
                    SyncViews = true;
                    GeneralDataModel.EditingNewRecord = false;
                    RebuildFilter();
                    return;
                }
                GeneralDataModel.Select().Rows.Find(restriction.IdRestriction).Delete();
            }
            RebuildFilter();
            SyncViews = true;
            GeneralDataModel.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new RestrictionListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения о реквизитах в базу данных?", @"Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case DialogResult.Yes:
                        SaveRecord();
                        break;
                    case DialogResult.No:
                        CancelRecord();
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }
            }
            base.OnClosing(e);
        }

        private void snapshot_restrictions_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.EditingStateUpdate();
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
        }

        private void snapshot_restrictions_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add && Selected)
            {
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
            }
        }

        private void RestrictionAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            //Если удалена ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            if (e.Action == DataRowAction.Delete)
                RebuildFilter();
        }

        private void RestrictionAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            //Если добавлена новая ассоциативная связь, то перестраиваем фильтр v_restriction.Filter
            RebuildFilter();
            //Если в модели есть запись, а в снапшоте нет, то добавляем в снапшот
            if (e.Row["id_restriction"] == DBNull.Value)
                return;
            var rowIndex = GeneralBindingSource.Find("id_restriction", e.Row["id_restriction"]);
            if (rowIndex == -1)
                return;
            var row = (DataRowView)GeneralBindingSource[rowIndex];
            if ((GeneralSnapshotBindingSource.Find("id_restriction", e.Row["id_restriction"]) == -1) && (rowIndex != -1))
            {
                GeneralSnapshot.Rows.Add(row["id_restriction"], row["id_restriction_type"], row["number"], row["date"], row["description"]);
            }
        }

        private void RestrictionListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = GeneralSnapshotBindingSource.Find("id_restriction", e.Row["id_restriction"]);
                if (rowIndex != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
            }
        }

        private void RestrictionListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            switch (e.Action)
            {
                case DataRowAction.Change:
                case DataRowAction.ChangeCurrentAndOriginal:
                case DataRowAction.ChangeOriginal:
                    var rowIndex = GeneralSnapshotBindingSource.Find("id_restriction", e.Row["id_restriction"]);
                    if (rowIndex == -1) return;
                    var row = (DataRowView)GeneralSnapshotBindingSource[rowIndex];
                    row["id_restriction_type"] = e.Row["id_restriction_type"];
                    row["number"] = e.Row["number"];
                    row["date"] = e.Row["date"];
                    row["description"] = e.Row["description"];
                    break;
                case DataRowAction.Add:
                    //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                    //иначе - объект не принадлежит текущему родителю
                    rowIndex = GeneralBindingSource.Find("id_restriction", e.Row["id_restriction"]);
                    if (rowIndex != -1)
                        GeneralSnapshot.Rows.Add(RestrictionConverter.ToArray(e.Row));
                    break;
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "number":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 10 ? 
                        "Длина номера реквизита не может превышать 10 символов" : "";
                    break;
                case "date":
                    cell.ErrorText = string.IsNullOrEmpty(cell.Value.ToString().Trim()) ? 
                        "Не заполнена дата" : "";
                    break;
                case "description":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 255 ? 
                        "Длина наименования реквизита не может превышать 255 символов" : "";
                    break;
            }
        }

        private void v_snapshot_restrictions_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name != "id_restriction_type") return;
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            if (editingControl == null) return;
            editingControl.DropDownClosed -= editingControl_DropDownClosed;
            editingControl.DropDownClosed += editingControl_DropDownClosed;
        }

        private void editingControl_DropDownClosed(object sender, EventArgs e)
        {
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            if (editingControl != null) dataGridView.CurrentCell.Value = editingControl.SelectedValue;
            dataGridView.EndEdit();
        }
    }
}
