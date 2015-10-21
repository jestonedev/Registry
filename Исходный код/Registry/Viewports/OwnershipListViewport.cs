using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CustomControls;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class OwnershipListViewport : EditableDataGridViewport
    {
        #region Models
        DataModel ownerships_rights_types;
        DataModel ownership_assoc;
        #endregion Models

        #region Views
        BindingSource v_ownership_right_types;
        BindingSource v_ownership_assoc;
        #endregion Views

        private OwnershipListViewport()
            : this(null)
        {
        }

        public OwnershipListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_ownerships_rights")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        public OwnershipListViewport(OwnershipListViewport ownershipListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = ownershipListViewport.DynamicFilter;
            StaticFilter = ownershipListViewport.StaticFilter;
            ParentRow = ownershipListViewport.ParentRow;
            ParentType = ownershipListViewport.ParentType;
        }

        private void RebuildFilter()
        {
            var ownershipFilter = "id_ownership_right IN (0";
            for (var i = 0; i < v_ownership_assoc.Count; i++)
                ownershipFilter += ((DataRowView)v_ownership_assoc[i])["id_ownership_right"] + ",";
            ownershipFilter = ownershipFilter.TrimEnd(',');
            ownershipFilter += ")";
            GeneralBindingSource.Filter = ownershipFilter;
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = OwnershipRightsFromView();
            var list_from_viewport = OwnershipRightsFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            var founded = false;
            for (var i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (var j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
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

        private bool ValidateViewportData(List<OwnershipRight> list)
        {
            if (ValidatePermissions() == false)
                return false;
            foreach (var ownershipRight in list)
            {
                if (ownershipRight.Number != null && ownershipRight.Number.Length > 20)
                {
                    MessageBox.Show("Длина номера основания не может превышать 20 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.Date == null)
                {
                    MessageBox.Show("Не заполнена дата начала действия ограничения, установленного в отношении муниципальной собственности",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.Description != null && ownershipRight.Description.Length > 255)
                {
                    MessageBox.Show("Длина наименования ограничения не может превышать 255 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.IdOwnershipRightType == null)
                {
                    MessageBox.Show("Не выбран тип ограничения, установленного в отношении муниципальной собственности", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static OwnershipRight RowToOwnershipRight(DataRow row)
        {
            var ownershipRight = new OwnershipRight();
            ownershipRight.IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right");
            ownershipRight.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
            ownershipRight.Number = ViewportHelper.ValueOrNull(row, "number");
            ownershipRight.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
            ownershipRight.Description = ViewportHelper.ValueOrNull(row, "description");
            return ownershipRight;
        }

        private List<OwnershipRight> OwnershipRightsFromViewport()
        {
            var list = new List<OwnershipRight>();
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

        private List<OwnershipRight> OwnershipRightsFromView()
        {
            var list = new List<OwnershipRight>();
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
            ownerships_rights_types = DataModel.GetInstance(DataModelType.OwnershipRightTypesDataModel);
            // Дожидаемся дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            ownerships_rights_types.Select();
            switch (ParentType)
            {
                case ParentTypeEnum.Premises:
                    ownership_assoc = DataModel.GetInstance(DataModelType.OwnershipPremisesAssocDataModel);
                    break;
                case ParentTypeEnum.Building:
                    ownership_assoc = DataModel.GetInstance(DataModelType.OwnershipBuildingsAssocDataModel);
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
            ownership_assoc.Select();

            v_ownership_assoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
            {
                v_ownership_assoc.DataMember = "ownership_premises_assoc";
                v_ownership_assoc.Filter = "id_premises = " + ParentRow["id_premises"];
                Text = string.Format(CultureInfo.InvariantCulture, "Ограничения помещения №{0}", ParentRow["id_premises"]);
            }
            else
                if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                {
                    v_ownership_assoc.DataMember = "ownership_buildings_assoc";
                    v_ownership_assoc.Filter = "id_building = " + ParentRow["id_building"];
                    Text = string.Format(CultureInfo.InvariantCulture, "Ограничения здания №{0}", ParentRow["id_building"]);
                }
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");
            v_ownership_assoc.DataSource = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "ownership_rights";
            GeneralBindingSource.DataSource = DataModel.DataSet;
            //Перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();

            v_ownership_right_types = new BindingSource();
            v_ownership_right_types.DataMember = "ownership_right_types";
            v_ownership_right_types.DataSource = DataModel.DataSet;

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName,
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource();
            GeneralSnapshotBindingSource.DataSource = GeneralSnapshot;
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_ownerships_rights_CurrentItemChanged;
            GeneralSnapshot.RowChanged += snapshot_ownerships_rights_RowChanged;
            GeneralSnapshot.RowDeleted += snapshot_ownerships_rights_RowDeleted;

            dataGridView.DataSource = GeneralSnapshotBindingSource;

            id_ownership_right.DataPropertyName = "id_ownership_right";
            id_ownership_right_type.DataSource = v_ownership_right_types;
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
            ownership_assoc.Select().RowChanged += OwnershipAssoc_RowChanged;
            ownership_assoc.Select().RowDeleted += OwnershipAssoc_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            row.EndEdit();
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
            var list = OwnershipRightsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = GeneralDataModel.Select().Rows.Find(list[i].IdOwnershipRight);
                if (row == null)
                {
                    var id_parent = ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] :
                        ((ParentType == ParentTypeEnum.Building) && ParentRow != null) ? (int)ParentRow["id_building"] :
                        -1;
                    if (id_parent == -1)
                    {
                        MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору", 
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    var id_ownership_right = GeneralDataModel.Insert(list[i]);
                    if (id_ownership_right == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    var assoc = new OwnershipRightObjectAssoc(id_parent, id_ownership_right);
                    switch (ParentType)
                    {
                        case ParentTypeEnum.Building:
                            DataModel.GetInstance(DataModelType.OwnershipBuildingsAssocDataModel).Insert(assoc);
                            break;
                        case ParentTypeEnum.Premises:
                            DataModel.GetInstance(DataModelType.OwnershipPremisesAssocDataModel).Insert(assoc);
                            break;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_ownership_right"] = id_ownership_right;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                    ownership_assoc.Select().Rows.Add(id_parent, id_ownership_right);
                }
                else
                {
                    if (RowToOwnershipRight(row) == list[i])
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    row["id_ownership_right_type"] = list[i].IdOwnershipRightType == null ? DBNull.Value : (object)list[i].IdOwnershipRightType;
                    row["number"] = list[i].Number == null ? DBNull.Value : (object)list[i].Number;
                    row["date"] = list[i].Date == null ? DBNull.Value : (object)list[i].Date;
                    row["description"] = list[i].Description == null ? DBNull.Value : (object)list[i].Description;
                }
            }
            list = OwnershipRightsFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_ownership_right"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_ownership_right"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_ownership_right"].Value == list[i].IdOwnershipRight))
                        row_index = j;
                if (row_index == -1)
                {
                    if (GeneralDataModel.Delete(list[i].IdOwnershipRight.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(list[i].IdOwnershipRight).Delete();
                    RebuildFilter();
                }
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
                var result = MessageBox.Show("Сохранить изменения об ограничениях в базу данных?", "Внимание",
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
            ownership_assoc.Select().RowChanged -= OwnershipAssoc_RowChanged;
            ownership_assoc.Select().RowDeleted -= OwnershipAssoc_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            GeneralDataModel.Select().RowChanged -= OwnershipListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= OwnershipListViewport_RowDeleting;
            ownership_assoc.Select().RowChanged -= OwnershipAssoc_RowChanged;
            ownership_assoc.Select().RowDeleted -= OwnershipAssoc_RowDeleted;
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
                    if (cell.Value.ToString().Trim().Length > 20)
                        cell.ErrorText = "Длина номера основания не может превышать 20 символов";
                    else
                        cell.ErrorText = "";
                    break;
                case "date":
                    if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                        cell.ErrorText = "Не заполнена дата начала действия ограничения, установленного в отношении муниципальной собственности";
                    else
                        cell.ErrorText = "";
                    break;
                case "description":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина наименования ограничения не может превышать 255 символов";
                    else
                        cell.ErrorText = "";
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
            var row_index = GeneralBindingSource.Find("id_ownership_right", e.Row["id_ownership_right"]);
            if (row_index == -1)
                return;
            var row = (DataRowView)GeneralBindingSource[row_index];
            if ((GeneralSnapshotBindingSource.Find("id_ownership_right", e.Row["id_ownership_right"]) == -1) && (row_index != -1))
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
                var row_index = GeneralSnapshotBindingSource.Find("id_ownership_right", e.Row["id_ownership_right"]);
                if (row_index != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[row_index]).Delete();
            }
        }

        void OwnershipListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                var row_index = GeneralSnapshotBindingSource.Find("id_ownership_right", e.Row["id_ownership_right"]);
                if (row_index != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[row_index]);
                    row["id_ownership_right_type"] = e.Row["id_ownership_right_type"];
                    row["number"] = e.Row["number"];
                    row["date"] = e.Row["date"];
                    row["description"] = e.Row["description"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                    //иначе - объект не принадлежит текущему родителю
                    var row_index = GeneralBindingSource.Find("id_ownership_right", e.Row["id_ownership_right"]);
                    if (row_index != -1)
                        GeneralSnapshot.Rows.Add(e.Row["id_ownership_right"], e.Row["id_ownership_right_type"], e.Row["number"], e.Row["date"], e.Row["description"]);
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
                editingControl.DropDownClosed -= editingControl_DropDownClosed;
                editingControl.DropDownClosed += editingControl_DropDownClosed;
            }
        }

        void editingControl_DropDownClosed(object sender, EventArgs e)
        {
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            dataGridView.CurrentCell.Value = editingControl.SelectedValue;
            dataGridView.EndEdit();
        }
    }
}
