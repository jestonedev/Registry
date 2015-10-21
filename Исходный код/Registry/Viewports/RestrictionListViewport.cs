﻿using System;
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
    internal sealed partial class RestrictionListViewport : EditableDataGridViewport
    {
        #region Models
        DataModel restriction_types;
        DataModel restriction_assoc;
        #endregion Models

        #region Views
        BindingSource v_restriction_types;
        BindingSource v_restriction_assoc;
        #endregion Views

        private RestrictionListViewport()
            : this(null)
        {
        }

        public RestrictionListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_restrictions")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        public RestrictionListViewport(RestrictionListViewport restrictionListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = restrictionListViewport.DynamicFilter;
            StaticFilter = restrictionListViewport.StaticFilter;
            ParentRow = restrictionListViewport.ParentRow;
            ParentType = restrictionListViewport.ParentType;
        }

        private void RebuildFilter()
        {
            var restrictionFilter = "id_restriction IN (0";
            for (var i = 0; i < v_restriction_assoc.Count; i++)
                restrictionFilter += ((DataRowView)v_restriction_assoc[i])["id_restriction"] + ",";
            restrictionFilter = restrictionFilter.TrimEnd(',');
            restrictionFilter += ")";
            GeneralBindingSource.Filter = restrictionFilter;
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = RestrictionsFromView();
            var list_from_viewport = RestrictionsFromViewport();
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
                dataRowView["id_restriction"], 
                dataRowView["id_restriction_type"], 
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

        private bool ValidateViewportData(List<Restriction> list)
        {
            if (ValidatePermissions() == false)
                return false;
            foreach (var restriction in list)
            {
                if (restriction.Number != null && restriction.Number.Length > 10)
                {
                    MessageBox.Show("Номер реквизита не может привышать 10 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restriction.Date == null)
                {
                    MessageBox.Show("Не заполнена дата", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restriction.Description != null && restriction.Description.Length > 255)
                {
                    MessageBox.Show("Длина наименования реквизита не может превышать 255 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restriction.IdRestrictionType == null)
                {
                    MessageBox.Show("Не выбран тип реквизита", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static Restriction RowToRestriction(DataRow row)
        {
            var restriction = new Restriction();
            restriction.IdRestriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction");
            restriction.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
            restriction.Number = ViewportHelper.ValueOrNull(row, "number");
            restriction.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
            restriction.Description = ViewportHelper.ValueOrNull(row, "description");
            return restriction;
        }

        private List<Restriction> RestrictionsFromViewport()
        {
            var list = new List<Restriction>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var r = new Restriction();
                    var row = dataGridView.Rows[i];
                    r.IdRestriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction");
                    r.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
                    r.Number = ViewportHelper.ValueOrNull(row, "number");
                    r.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
                    r.Description = ViewportHelper.ValueOrNull(row, "description");
                    list.Add(r);
                }
            }
            return list;
        }

        private List<Restriction> RestrictionsFromView()
        {
            var list = new List<Restriction>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var r = new Restriction();
                var row = ((DataRowView)GeneralBindingSource[i]);
                r.IdRestriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction");
                r.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
                r.Number = ViewportHelper.ValueOrNull(row, "number");
                r.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
                r.Description = ViewportHelper.ValueOrNull(row, "description");
                list.Add(r);
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.RestrictionsDataModel);
            restriction_types = DataModel.GetInstance(DataModelType.RestrictionTypesDataModel);
            // Дожидаемся дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            restriction_types.Select();

            if (ParentType == ParentTypeEnum.Premises)
                restriction_assoc =  DataModel.GetInstance(DataModelType.RestrictionsPremisesAssocDataModel);
            else
                if (ParentType == ParentTypeEnum.Building)
                    restriction_assoc = DataModel.GetInstance(DataModelType.RestrictionsBuildingsAssocDataModel);
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");
            restriction_assoc.Select();

            v_restriction_assoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
            {
                v_restriction_assoc.DataMember = "restrictions_premises_assoc";
                v_restriction_assoc.Filter = "id_premises = " + ParentRow["id_premises"];
                Text = string.Format(CultureInfo.InvariantCulture, "Реквизиты помещения №{0}", ParentRow["id_premises"]);
            }
            else
                if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                {
                    v_restriction_assoc.DataMember = "restrictions_buildings_assoc";
                    v_restriction_assoc.Filter = "id_building = " + ParentRow["id_building"];
                    Text = string.Format(CultureInfo.InvariantCulture, "Реквизиты здания №{0}", ParentRow["id_building"]);
                }
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");
            v_restriction_assoc.DataSource = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "restrictions";
            GeneralBindingSource.DataSource = DataModel.DataSet;
            //Перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();

            v_restriction_types = new BindingSource();
            v_restriction_types.DataMember = "restriction_types";
            v_restriction_types.DataSource = DataModel.DataSet;

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName,
                    GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource();
            GeneralSnapshotBindingSource.DataSource = GeneralSnapshot;
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_restrictions_CurrentItemChanged;
            GeneralSnapshot.RowChanged += snapshot_restrictions_RowChanged;
            GeneralSnapshot.RowDeleted += snapshot_restrictions_RowDeleted;

            dataGridView.DataSource = GeneralSnapshotBindingSource;

            id_restriction.DataPropertyName = "id_restriction";
            id_restriction_type.DataSource = v_restriction_types;
            id_restriction_type.ValueMember = "id_restriction_type";
            id_restriction_type.DisplayMember = "restriction_type";
            id_restriction_type.DataPropertyName = "id_restriction_type";
            number.DataPropertyName = "number";
            date.DataPropertyName = "date";
            description.DataPropertyName = "description";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;

            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            GeneralDataModel.Select().RowChanged += RestrictionListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += RestrictionListViewport_RowDeleting;
            restriction_assoc.Select().RowChanged += RestrictionAssoc_RowChanged;
            restriction_assoc.Select().RowDeleted += RestrictionAssoc_RowDeleted;
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
            var list = RestrictionsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = GeneralDataModel.Select().Rows.Find(list[i].IdRestriction);
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
                    var id_restriction = GeneralDataModel.Insert(list[i]);
                    if (id_restriction == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    var assoc = new RestrictionObjectAssoc(id_parent, id_restriction, null);
                    switch (ParentType)
                    {
                        case ParentTypeEnum.Building:
                            DataModel.GetInstance(DataModelType.RestrictionsBuildingsAssocDataModel).Insert(assoc);
                            break;
                        case ParentTypeEnum.Premises:
                            DataModel.GetInstance(DataModelType.RestrictionsPremisesAssocDataModel).Insert(assoc);
                            break;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_restriction"] = id_restriction;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                    restriction_assoc.Select().Rows.Add(id_parent, id_restriction);
                }
                else
                {
                    if (RowToRestriction(row) == list[i])
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    row["id_restriction_type"] = list[i].IdRestrictionType == null ? DBNull.Value : (object)list[i].IdRestrictionType;
                    row["number"] = list[i].Number == null ? DBNull.Value : (object)list[i].Number;
                    row["date"] = list[i].Date == null ? DBNull.Value : (object)list[i].Date;
                    row["description"] = list[i].Description == null ? DBNull.Value : (object)list[i].Description;
                }
            }
            list = RestrictionsFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_restriction"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_restriction"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_restriction"].Value == list[i].IdRestriction))
                        row_index = j;
                if (row_index == -1)
                {
                    if (GeneralDataModel.Delete(list[i].IdRestriction.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(list[i].IdRestriction).Delete();
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
            var viewport = new RestrictionListViewport(this, MenuCallback);
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
                var result = MessageBox.Show("Сохранить изменения о реквизитах в базу данных?", "Внимание",
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
            GeneralDataModel.Select().RowChanged -= RestrictionListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= RestrictionListViewport_RowDeleting;
            restriction_assoc.Select().RowChanged -= RestrictionAssoc_RowChanged;
            restriction_assoc.Select().RowDeleted -= RestrictionAssoc_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            GeneralDataModel.Select().RowChanged -= RestrictionListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= RestrictionListViewport_RowDeleting;
            restriction_assoc.Select().RowChanged -= RestrictionAssoc_RowChanged;
            restriction_assoc.Select().RowDeleted -= RestrictionAssoc_RowDeleted;
            Close();
        }

        void snapshot_restrictions_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void snapshot_restrictions_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add && Selected)
            {
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
            }
        }

        void RestrictionAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            //Если удалена ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            if (e.Action == DataRowAction.Delete)
                RebuildFilter();
        }

        void RestrictionAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            //Если добавлена новая ассоциативная связь, то перестраиваем фильтр v_restriction.Filter
            RebuildFilter();
            //Если в модели есть запись, а в снапшоте нет, то добавляем в снапшот
            if (e.Row["id_restriction"] == DBNull.Value)
                return;
            var row_index = GeneralBindingSource.Find("id_restriction", e.Row["id_restriction"]);
            if (row_index == -1)
                return;
            var row = (DataRowView)GeneralBindingSource[row_index];
            if ((GeneralSnapshotBindingSource.Find("id_restriction", e.Row["id_restriction"]) == -1) && (row_index != -1))
            {
                GeneralSnapshot.Rows.Add(row["id_restriction"], row["id_restriction_type"], row["number"], row["date"], row["description"]);
            }
        }

        void RestrictionListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = GeneralSnapshotBindingSource.Find("id_restriction", e.Row["id_restriction"]);
                if (row_index != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[row_index]).Delete();
            }
        }

        void RestrictionListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                var row_index = GeneralSnapshotBindingSource.Find("id_restriction", e.Row["id_restriction"]);
                if (row_index != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[row_index]);
                    row["id_restriction_type"] = e.Row["id_restriction_type"];
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
                    var row_index = GeneralBindingSource.Find("id_restriction", e.Row["id_restriction"]);
                    if (row_index != -1)
                        GeneralSnapshot.Rows.Add(e.Row["id_restriction"], e.Row["id_restriction_type"], e.Row["number"], e.Row["date"], e.Row["description"]);
                }
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
                    if (cell.Value.ToString().Trim().Length > 10)
                        cell.ErrorText = "Длина номера реквизита не может превышать 10 символов";
                    else
                        cell.ErrorText = "";
                    break;
                case "date":
                    if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                        cell.ErrorText = "Не заполнена дата";
                    else
                        cell.ErrorText = "";
                    break;
                case "description":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина наименования реквизита не может превышать 255 символов";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        void v_snapshot_restrictions_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name == "id_restriction_type")
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
