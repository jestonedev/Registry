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
    internal sealed class RestrictionListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_restriction;
        private DataGridViewTextBoxColumn number;
        private DataGridViewDateTimeColumn date;
        private DataGridViewTextBoxColumn description;
        private DataGridViewComboBoxColumn id_restriction_type;
        #endregion Components

        #region Models
        DataModel restrictions;
        DataModel restriction_types;
        DataModel restriction_assoc;
        DataTable snapshot_restrictions = new DataTable("snapshot_restrictions");
        #endregion Models

        #region Views
        BindingSource v_restrictions;
        BindingSource v_restriction_types;
        BindingSource v_restriction_assoc;
        BindingSource v_snapshot_restrictions;
        #endregion Views


        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private RestrictionListViewport()
            : this(null)
        {
        }

        public RestrictionListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            snapshot_restrictions.Locale = CultureInfo.InvariantCulture;
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
            v_restrictions.Filter = restrictionFilter;
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
            for (var i = 0; i < v_restrictions.Count; i++)
            {
                var r = new Restriction();
                var row = ((DataRowView)v_restrictions[i]);
                r.IdRestriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction");
                r.IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type");
                r.Number = ViewportHelper.ValueOrNull(row, "number");
                r.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
                r.Description = ViewportHelper.ValueOrNull(row, "description");
                list.Add(r);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_restrictions.Count;
        }

        public override void MoveFirst()
        {
            v_snapshot_restrictions.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_restrictions.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_restrictions.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_restrictions.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_restrictions.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_restrictions.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_restrictions.Position > -1) && (v_snapshot_restrictions.Position < (v_snapshot_restrictions.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_restrictions.Position > -1) && (v_snapshot_restrictions.Position < (v_snapshot_restrictions.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            restrictions = DataModel.GetInstance(DataModelType.RestrictionsDataModel);
            restriction_types = DataModel.GetInstance(DataModelType.RestrictionTypesDataModel);
            // Дожидаемся дозагрузки данных, если это необходимо
            restrictions.Select();
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

            v_restrictions = new BindingSource();
            v_restrictions.DataMember = "restrictions";
            v_restrictions.DataSource = DataModel.DataSet;
            //Перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();

            v_restriction_types = new BindingSource();
            v_restriction_types.DataMember = "restriction_types";
            v_restriction_types.DataSource = DataModel.DataSet;

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < restrictions.Select().Columns.Count; i++)
                snapshot_restrictions.Columns.Add(new DataColumn(restrictions.Select().Columns[i].ColumnName,
                    restrictions.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_restrictions.Count; i++)
                snapshot_restrictions.Rows.Add(DataRowViewToArray(((DataRowView)v_restrictions[i])));
            v_snapshot_restrictions = new BindingSource();
            v_snapshot_restrictions.DataSource = snapshot_restrictions;
            v_snapshot_restrictions.CurrentItemChanged += v_snapshot_restrictions_CurrentItemChanged;
            snapshot_restrictions.RowChanged += snapshot_restrictions_RowChanged;
            snapshot_restrictions.RowDeleted += snapshot_restrictions_RowDeleted;

            dataGridView.DataSource = v_snapshot_restrictions;

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
            restrictions.Select().RowChanged += RestrictionListViewport_RowChanged;
            restrictions.Select().RowDeleting += RestrictionListViewport_RowDeleting;
            restriction_assoc.Select().RowChanged += RestrictionAssoc_RowChanged;
            restriction_assoc.Select().RowDeleted += RestrictionAssoc_RowDeleted;
        }
        
        public override bool CanInsertRecord()
        {
            return (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)v_snapshot_restrictions.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_restrictions.Position != -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_restrictions[v_snapshot_restrictions.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_restrictions.Clear();
            for (var i = 0; i < v_restrictions.Count; i++)
                snapshot_restrictions.Rows.Add(DataRowViewToArray(((DataRowView)v_restrictions[i])));
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
            restrictions.EditingNewRecord = true;
            var list = RestrictionsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = restrictions.Select().Rows.Find(list[i].IdRestriction);
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
                        restrictions.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    var id_restriction = restrictions.Insert(list[i]);
                    if (id_restriction == -1)
                    {
                        sync_views = true;
                        restrictions.EditingNewRecord = false;
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
                    ((DataRowView)v_snapshot_restrictions[i])["id_restriction"] = id_restriction;
                    restrictions.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_restrictions[i]));
                    restriction_assoc.Select().Rows.Add(id_parent, id_restriction);
                }
                else
                {
                    if (RowToRestriction(row) == list[i])
                        continue;
                    if (restrictions.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        restrictions.EditingNewRecord = false;
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
                    if (restrictions.Delete(list[i].IdRestriction.Value) == -1)
                    {
                        sync_views = true;
                        restrictions.EditingNewRecord = false;
                        RebuildFilter();
                        return;
                    }
                    restrictions.Select().Rows.Find(list[i].IdRestriction).Delete();
                }
            }
            RebuildFilter();
            sync_views = true;
            restrictions.EditingNewRecord = false;
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
            restrictions.Select().RowChanged -= RestrictionListViewport_RowChanged;
            restrictions.Select().RowDeleting -= RestrictionListViewport_RowDeleting;
            restriction_assoc.Select().RowChanged -= RestrictionAssoc_RowChanged;
            restriction_assoc.Select().RowDeleted -= RestrictionAssoc_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            restrictions.Select().RowChanged -= RestrictionListViewport_RowChanged;
            restrictions.Select().RowDeleting -= RestrictionListViewport_RowDeleting;
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
            var row_index = v_restrictions.Find("id_restriction", e.Row["id_restriction"]);
            if (row_index == -1)
                return;
            var row = (DataRowView)v_restrictions[row_index];
            if ((v_snapshot_restrictions.Find("id_restriction", e.Row["id_restriction"]) == -1) && (row_index != -1))
            {
                snapshot_restrictions.Rows.Add(row["id_restriction"], row["id_restriction_type"], row["number"], row["date"], row["description"]);
            }
        }

        void RestrictionListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = v_snapshot_restrictions.Find("id_restriction", e.Row["id_restriction"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_restrictions[row_index]).Delete();
            }
        }

        void RestrictionListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                var row_index = v_snapshot_restrictions.Find("id_restriction", e.Row["id_restriction"]);
                if (row_index != -1)
                {
                    var row = ((DataRowView)v_snapshot_restrictions[row_index]);
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
                    var row_index = v_restrictions.Find("id_restriction", e.Row["id_restriction"]);
                    if (row_index != -1)
                        snapshot_restrictions.Rows.Add(e.Row["id_restriction"], e.Row["id_restriction_type"], e.Row["number"], e.Row["date"], e.Row["description"]);
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

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(RestrictionListViewport));
            dataGridView = new DataGridView();
            id_restriction = new DataGridViewTextBoxColumn();
            number = new DataGridViewTextBoxColumn();
            date = new DataGridViewDateTimeColumn();
            description = new DataGridViewTextBoxColumn();
            id_restriction_type = new DataGridViewComboBoxColumn();
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
            dataGridView.Columns.AddRange(id_restriction, number, date, description, id_restriction_type);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(867, 385);
            dataGridView.TabIndex = 1;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            // 
            // id_restriction
            // 
            id_restriction.HeaderText = "Идентификатор реквизита";
            id_restriction.Name = "id_restriction";
            id_restriction.Visible = false;
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
            // id_restriction_type
            // 
            id_restriction_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            id_restriction_type.FillWeight = 200F;
            id_restriction_type.HeaderText = "Тип реквизита";
            id_restriction_type.MinimumWidth = 200;
            id_restriction_type.Name = "id_restriction_type";
            id_restriction_type.Width = 200;
            // 
            // RestrictionListViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(873, 391);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "RestrictionListViewport";
            Padding = new Padding(3);
            Text = "Реквизиты";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
