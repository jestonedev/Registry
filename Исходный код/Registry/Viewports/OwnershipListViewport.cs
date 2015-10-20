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
    internal sealed class OwnershipListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_ownership_right;
        private DataGridViewTextBoxColumn number;
        private DataGridViewDateTimeColumn date;
        private DataGridViewTextBoxColumn description;
        private DataGridViewComboBoxColumn id_ownership_right_type;
        #endregion Components

        #region Models
        DataModel ownership_rights;
        DataModel ownerships_rights_types;
        DataModel ownership_assoc;
        DataTable snapshot_ownerships_rights = new DataTable("snapshot_ownerships_rights");
        #endregion Models

        #region Views
        BindingSource v_ownership_rights;
        BindingSource v_ownership_right_types;
        BindingSource v_ownership_assoc;
        BindingSource v_snapshot_ownerships_rights;
        #endregion Views

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private OwnershipListViewport()
            : this(null)
        {
        }

        public OwnershipListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            snapshot_ownerships_rights.Locale = CultureInfo.InvariantCulture;
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
            v_ownership_rights.Filter = ownershipFilter;
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
            for (var i = 0; i < v_ownership_rights.Count; i++)
            {
                var or = new OwnershipRight();
                var row = ((DataRowView)v_ownership_rights[i]);
                or.IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right");
                or.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                or.Number = ViewportHelper.ValueOrNull(row, "number");
                or.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
                or.Description = ViewportHelper.ValueOrNull(row, "description");
                list.Add(or);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_ownerships_rights.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            ownership_rights = DataModel.GetInstance(DataModelType.OwnershipsRightsDataModel);
            ownerships_rights_types = DataModel.GetInstance(DataModelType.OwnershipRightTypesDataModel);
            // Дожидаемся дозагрузки данных, если это необходимо
            ownership_rights.Select();
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

            v_ownership_rights = new BindingSource();
            v_ownership_rights.DataMember = "ownership_rights";
            v_ownership_rights.DataSource = DataModel.DataSet;
            //Перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();

            v_ownership_right_types = new BindingSource();
            v_ownership_right_types.DataMember = "ownership_right_types";
            v_ownership_right_types.DataSource = DataModel.DataSet;

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < ownership_rights.Select().Columns.Count; i++)
                snapshot_ownerships_rights.Columns.Add(new DataColumn(ownership_rights.Select().Columns[i].ColumnName, 
                    ownership_rights.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_ownership_rights.Count; i++)
                snapshot_ownerships_rights.Rows.Add(DataRowViewToArray(((DataRowView)v_ownership_rights[i])));
            v_snapshot_ownerships_rights = new BindingSource();
            v_snapshot_ownerships_rights.DataSource = snapshot_ownerships_rights;
            v_snapshot_ownerships_rights.CurrentItemChanged += v_snapshot_ownerships_rights_CurrentItemChanged;
            snapshot_ownerships_rights.RowChanged += snapshot_ownerships_rights_RowChanged;
            snapshot_ownerships_rights.RowDeleted += snapshot_ownerships_rights_RowDeleted;

            dataGridView.DataSource = v_snapshot_ownerships_rights;

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
            ownership_rights.Select().RowChanged += OwnershipListViewport_RowChanged;
            ownership_rights.Select().RowDeleting += OwnershipListViewport_RowDeleting;
            ownership_assoc.Select().RowChanged += OwnershipAssoc_RowChanged;
            ownership_assoc.Select().RowDeleted += OwnershipAssoc_RowDeleted;
        }
        
        public override void MoveFirst()
        {
            v_snapshot_ownerships_rights.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_ownerships_rights.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_ownerships_rights.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_ownerships_rights.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_ownerships_rights.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_ownerships_rights.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_ownerships_rights.Position > -1) && (v_snapshot_ownerships_rights.Position < (v_snapshot_ownerships_rights.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_ownerships_rights.Position > -1) && (v_snapshot_ownerships_rights.Position < (v_snapshot_ownerships_rights.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)v_snapshot_ownerships_rights.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_ownerships_rights.Position != -1) && 
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_ownerships_rights[v_snapshot_ownerships_rights.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_ownerships_rights.Clear();
            for (var i = 0; i < v_ownership_rights.Count; i++)
                snapshot_ownerships_rights.Rows.Add(DataRowViewToArray(((DataRowView)v_ownership_rights[i])));
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
            ownership_rights.EditingNewRecord = true;
            var list = OwnershipRightsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                ownership_rights.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = ownership_rights.Select().Rows.Find(list[i].IdOwnershipRight);
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
                        ownership_rights.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    var id_ownership_right = ownership_rights.Insert(list[i]);
                    if (id_ownership_right == -1)
                    {
                        sync_views = true;
                        ownership_rights.EditingNewRecord = false;
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
                    ((DataRowView)v_snapshot_ownerships_rights[i])["id_ownership_right"] = id_ownership_right;
                    ownership_rights.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_ownerships_rights[i]));
                    ownership_assoc.Select().Rows.Add(id_parent, id_ownership_right);
                }
                else
                {
                    if (RowToOwnershipRight(row) == list[i])
                        continue;
                    if (ownership_rights.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        ownership_rights.EditingNewRecord = false;
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
                    if (ownership_rights.Delete(list[i].IdOwnershipRight.Value) == -1)
                    {
                        sync_views = true;
                        ownership_rights.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    ownership_rights.Select().Rows.Find(list[i].IdOwnershipRight).Delete();
                    RebuildFilter();
                }
            }
            RebuildFilter();
            sync_views = true;
            ownership_rights.EditingNewRecord = false;
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
            if (e == null)
                return;
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
            ownership_rights.Select().RowChanged -= OwnershipListViewport_RowChanged;
            ownership_rights.Select().RowDeleting -= OwnershipListViewport_RowDeleting;
            ownership_assoc.Select().RowChanged -= OwnershipAssoc_RowChanged;
            ownership_assoc.Select().RowDeleted -= OwnershipAssoc_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            ownership_rights.Select().RowChanged -= OwnershipListViewport_RowChanged;
            ownership_rights.Select().RowDeleting -= OwnershipListViewport_RowDeleting;
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
            var row_index = v_ownership_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
            if (row_index == -1)
                return;
            var row = (DataRowView)v_ownership_rights[row_index];
            if ((v_snapshot_ownerships_rights.Find("id_ownership_right", e.Row["id_ownership_right"]) == -1) && (row_index != -1))
            {
                snapshot_ownerships_rights.Rows.Add(row["id_ownership_right"], row["id_ownership_right_type"], row["number"], row["date"], row["description"]);
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
                var row_index = v_snapshot_ownerships_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_ownerships_rights[row_index]).Delete();
            }
        }

        void OwnershipListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                var row_index = v_snapshot_ownerships_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
                if (row_index != -1)
                {
                    var row = ((DataRowView)v_snapshot_ownerships_rights[row_index]);
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
                    var row_index = v_ownership_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
                    if (row_index != -1)
                        snapshot_ownerships_rights.Rows.Add(e.Row["id_ownership_right"], e.Row["id_ownership_right_type"], e.Row["number"], e.Row["date"], e.Row["description"]);
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

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(OwnershipListViewport));
            dataGridView = new DataGridView();
            id_ownership_right = new DataGridViewTextBoxColumn();
            number = new DataGridViewTextBoxColumn();
            date = new DataGridViewDateTimeColumn();
            description = new DataGridViewTextBoxColumn();
            id_ownership_right_type = new DataGridViewComboBoxColumn();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_ownership_right, number, date, description, id_ownership_right_type);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(819, 328);
            dataGridView.TabIndex = 2;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            // 
            // id_ownership_right
            // 
            id_ownership_right.HeaderText = "Идентификатор основания";
            id_ownership_right.Name = "id_ownership_right";
            id_ownership_right.Visible = false;
            // 
            // number
            // 
            number.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            number.HeaderText = "Номер";
            number.MinimumWidth = 150;
            number.Name = "number";
            number.Width = 150;
            // 
            // date
            // 
            date.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            date.HeaderText = "Дата";
            date.MinimumWidth = 150;
            date.Name = "date";
            date.Width = 150;
            // 
            // description
            // 
            description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            description.HeaderText = "Наименование";
            description.MinimumWidth = 300;
            description.Name = "description";
            // 
            // id_ownership_right_type
            // 
            id_ownership_right_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            id_ownership_right_type.HeaderText = "Тип ограничения";
            id_ownership_right_type.MinimumWidth = 150;
            id_ownership_right_type.Name = "id_ownership_right_type";
            id_ownership_right_type.Width = 150;
            // 
            // OwnershipListViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(825, 334);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "OwnershipListViewport";
            Padding = new Padding(3);
            Text = "Ограничения";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
