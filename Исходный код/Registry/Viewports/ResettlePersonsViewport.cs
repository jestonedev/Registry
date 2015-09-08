using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.CalcDataModels;
using Registry.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class ResettlePersonsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_person;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        #endregion Components

        #region Models
        ResettlePersonsDataModel resettle_persons;
        DataTable snapshot_resettle_persons = new DataTable("snapshot_resettle_persons");
        #endregion Models

        #region Views
        BindingSource v_resettle_persons;
        BindingSource v_snapshot_resettle_persons;
        #endregion Views


        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private ResettlePersonsViewport()
            : this(null)
        {
        }

        public ResettlePersonsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshot_resettle_persons.Locale = CultureInfo.InvariantCulture;
        }

        public ResettlePersonsViewport(ResettlePersonsViewport resettlePersonsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = resettlePersonsViewport.DynamicFilter;
            StaticFilter = resettlePersonsViewport.StaticFilter;
            ParentRow = resettlePersonsViewport.ParentRow;
            ParentType = resettlePersonsViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = ResettlePersonsFromView();
            var list_from_viewport = ResettlePersonsFromViewport();
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
                dataRowView["id_person"], 
                dataRowView["id_process"], 
                dataRowView["surname"], 
                dataRowView["name"],
                dataRowView["patronymic"]
            };
        }

        private static bool ValidateResettlePersons(List<ResettlePerson> resettlePersons)
        {
            foreach (var resettlePerson in resettlePersons)
            {
                if (resettlePerson.Surname == null)
                {
                    MessageBox.Show("Фамилия и имя являются обязательными для заполнения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Name == null)
                {
                    MessageBox.Show("Фамилия и имя являются обязательными для заполнения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Surname != null && resettlePerson.Surname.Length > 50)
                {
                    MessageBox.Show("Длина фамилии не может превышать 50 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Name != null && resettlePerson.Name.Length > 50)
                {
                    MessageBox.Show("Длина имени не может превышать 50 символов", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Patronymic != null && resettlePerson.Patronymic.Length > 255)
                {
                    MessageBox.Show("Длина отчества не может превышать 255 символов", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static ResettlePerson RowToResettlePerson(DataRow row)
        {
            var resettlePerson = new ResettlePerson();
            resettlePerson.IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person");
            resettlePerson.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            resettlePerson.Surname = ViewportHelper.ValueOrNull(row, "surname");
            resettlePerson.Name = ViewportHelper.ValueOrNull(row, "name");
            resettlePerson.Patronymic = ViewportHelper.ValueOrNull(row, "patronymic");
            return resettlePerson;
        }

        private List<ResettlePerson> ResettlePersonsFromViewport()
        {
            var list = new List<ResettlePerson>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var rp = new ResettlePerson();
                    var row = dataGridView.Rows[i];
                    rp.IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person");
                    rp.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                    rp.Surname = ViewportHelper.ValueOrNull(row, "surname");
                    rp.Name = ViewportHelper.ValueOrNull(row, "name");
                    rp.Patronymic = ViewportHelper.ValueOrNull(row, "patronymic");
                    list.Add(rp);
                }
            }
            return list;
        }

        private List<ResettlePerson> ResettlePersonsFromView()
        {
            var list = new List<ResettlePerson>();
            for (var i = 0; i < v_resettle_persons.Count; i++)
            {
                var rp = new ResettlePerson();
                var row = ((DataRowView)v_resettle_persons[i]);
                rp.IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person");
                rp.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                rp.Surname = ViewportHelper.ValueOrNull(row, "surname");
                rp.Name = ViewportHelper.ValueOrNull(row, "name");
                rp.Patronymic = ViewportHelper.ValueOrNull(row, "patronymic");
                list.Add(rp);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_resettle_persons.Count;
        }

        public override void MoveFirst()
        {
            v_snapshot_resettle_persons.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_resettle_persons.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_resettle_persons.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_resettle_persons.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_resettle_persons.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_resettle_persons.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_resettle_persons.Position > -1) && (v_snapshot_resettle_persons.Position < (v_snapshot_resettle_persons.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_resettle_persons.Position > -1) && (v_snapshot_resettle_persons.Position < (v_snapshot_resettle_persons.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            resettle_persons = ResettlePersonsDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            resettle_persons.Select();

            v_resettle_persons = new BindingSource();
            v_resettle_persons.DataMember = "resettle_persons";
            v_resettle_persons.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_resettle_persons.Filter += " AND ";
            v_resettle_persons.Filter += DynamicFilter;
            v_resettle_persons.DataSource = DataSetManager.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.ResettleProcess)
                Text = string.Format(CultureInfo.InvariantCulture, "Участники переселения №{0}", ParentRow["id_process"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < resettle_persons.Select().Columns.Count; i++)
                snapshot_resettle_persons.Columns.Add(new DataColumn(resettle_persons.Select().Columns[i].ColumnName, resettle_persons.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_resettle_persons.Count; i++)
                snapshot_resettle_persons.Rows.Add(DataRowViewToArray(((DataRowView)v_resettle_persons[i])));
            v_snapshot_resettle_persons = new BindingSource();
            v_snapshot_resettle_persons.DataSource = snapshot_resettle_persons;
            v_snapshot_resettle_persons.CurrentItemChanged += v_snapshot_resettle_persons_CurrentItemChanged;

            dataGridView.DataSource = v_snapshot_resettle_persons;
            id_person.DataPropertyName = "id_person";
            id_process.DataPropertyName = "id_process";
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            resettle_persons.Select().RowChanged += ResettlePersonsViewport_RowChanged;
            resettle_persons.Select().RowDeleting += ResettlePersonsViewport_RowDeleting;
            resettle_persons.Select().RowDeleted += ResettlePersonsViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return (ParentType == ParentTypeEnum.ResettleProcess) && (ParentRow != null) && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void InsertRecord()
        {
            if ((ParentRow == null) || (ParentType != ParentTypeEnum.ResettleProcess))
                return;
            var row = (DataRowView)v_snapshot_resettle_persons.AddNew();
            row["id_process"] = ParentRow["id_process"];
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_resettle_persons.Position != -1) && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_resettle_persons[v_snapshot_resettle_persons.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_resettle_persons.Clear();
            for (var i = 0; i < v_resettle_persons.Count; i++)
                snapshot_resettle_persons.Rows.Add(DataRowViewToArray(((DataRowView)v_resettle_persons[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            resettle_persons.EditingNewRecord = true;
            var list = ResettlePersonsFromViewport();
            if (!ValidateResettlePersons(list))
            {
                sync_views = true;
                resettle_persons.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = resettle_persons.Select().Rows.Find(list[i].IdPerson);
                if (row == null)
                {
                    var id_person = ResettlePersonsDataModel.Insert(list[i]);
                    if (id_person == -1)
                    {
                        sync_views = true; 
                        resettle_persons.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_resettle_persons[i])["id_person"] = id_person;
                    resettle_persons.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_resettle_persons[i]));
                }
                else
                {
                    if (RowToResettlePerson(row) == list[i])
                        continue;
                    if (ResettlePersonsDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        resettle_persons.EditingNewRecord = false;
                        return;
                    }
                    row["id_process"] = list[i].IdProcess == null ? DBNull.Value : (object)list[i].IdProcess;
                    row["surname"] = list[i].Surname == null ? DBNull.Value : (object)list[i].Surname;
                    row["name"] = list[i].Name == null ? DBNull.Value : (object)list[i].Name;
                    row["patronymic"] = list[i].Patronymic == null ? DBNull.Value : (object)list[i].Patronymic;
                }
            }
            list = ResettlePersonsFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_person"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_person"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_person"].Value == list[i].IdPerson))
                        row_index = j;
                if (row_index == -1)
                {
                    if (ResettlePersonsDataModel.Delete(list[i].IdPerson.Value) == -1)
                    {
                        sync_views = true;
                        resettle_persons.EditingNewRecord = false;
                        return;
                    }
                    resettle_persons.Select().Rows.Find(list[i].IdPerson).Delete();
                }
            }
            sync_views = true;
            resettle_persons.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
            if (ParentType == ParentTypeEnum.ResettleProcess)
                CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.ResettleProcess, (int)ParentRow["id_process"], true);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new ResettlePersonsViewport(this, MenuCallback);
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
                var result = MessageBox.Show("Сохранить изменения об участниках переселения в базу данных?", "Внимание",
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
            resettle_persons.Select().RowChanged -= ResettlePersonsViewport_RowChanged;
            resettle_persons.Select().RowDeleting -= ResettlePersonsViewport_RowDeleting;
            resettle_persons.Select().RowDeleted -= ResettlePersonsViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            resettle_persons.Select().RowChanged -= ResettlePersonsViewport_RowChanged;
            resettle_persons.Select().RowDeleting -= ResettlePersonsViewport_RowDeleting;
            resettle_persons.Select().RowDeleted -= ResettlePersonsViewport_RowDeleted;
            Close();
        }

        void v_snapshot_resettle_persons_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }
        
        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "surname":
                    if (cell.Value.ToString().Trim().Length > 50)
                        cell.ErrorText = "Длина фамилии не может превышать 50 символов";
                    else
                        cell.ErrorText = "";
                    break;
                case "name":
                    if (cell.Value.ToString().Trim().Length > 50)
                        cell.ErrorText = "Длина имени не может превышать 50 символов";
                    else
                        cell.ErrorText = "";
                    break;
                case "patronymic":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина отчества не может превышать 255 символов";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        void ResettlePersonsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        void ResettlePersonsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var row_index = v_snapshot_resettle_persons.Find("id_person", e.Row["id_person"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_resettle_persons[row_index]).Delete();
            }
        }

        void ResettlePersonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = v_snapshot_resettle_persons.Find("id_person", e.Row["id_person"]);
            if (row_index == -1 && v_resettle_persons.Find("id_person", e.Row["id_person"]) != -1)
            {
                snapshot_resettle_persons.Rows.Add(e.Row["id_person"], e.Row["id_process"], e.Row["surname"], e.Row["name"], e.Row["patronymic"]);
            } else
            if (row_index != -1)
            {
                var row = ((DataRowView)v_snapshot_resettle_persons[row_index]);
                row["id_process"] = e.Row["id_process"];
                row["surname"] = e.Row["surname"];
                row["name"] = e.Row["name"];
                row["patronymic"] = e.Row["patronymic"];
            }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(ResettlePersonsViewport));
            dataGridView = new DataGridView();
            id_person = new DataGridViewTextBoxColumn();
            id_process = new DataGridViewTextBoxColumn();
            surname = new DataGridViewTextBoxColumn();
            name = new DataGridViewTextBoxColumn();
            patronymic = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
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
            dataGridView.Columns.AddRange(id_person, id_process, surname, name, patronymic);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(0, 0);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(856, 415);
            dataGridView.TabIndex = 1;
            // 
            // id_person
            // 
            id_person.HeaderText = "Внутренний идентификатор участника";
            id_person.Name = "id_person";
            id_person.Visible = false;
            // 
            // id_process
            // 
            id_process.HeaderText = "Внутренний идентификатор процесса переселения";
            id_process.Name = "id_process";
            id_process.Visible = false;
            // 
            // surname
            // 
            surname.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            surname.HeaderText = "Фамилия";
            surname.MinimumWidth = 150;
            surname.Name = "surname";
            // 
            // name
            // 
            name.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            name.HeaderText = "Имя";
            name.MinimumWidth = 150;
            name.Name = "name";
            // 
            // patronymic
            // 
            patronymic.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            patronymic.HeaderText = "Отчество";
            patronymic.MinimumWidth = 150;
            patronymic.Name = "patronymic";
            // 
            // ResettlePersonsViewport
            // 
            ClientSize = new Size(856, 415);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "ResettlePersonsViewport";
            Text = "Участники переселения №{0}";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
