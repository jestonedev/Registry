using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.DataModels;
using Registry.Entities;
using System.Globalization;
using Security;

namespace Registry.Viewport
{
    internal sealed class ResettlePersonsViewport: Viewport
    {
        #region Components
        private System.Windows.Forms.DataGridView dataGridView;
        #endregion Components

        #region Models
        ResettlePersonsDataModel resettle_persons = null;
        DataTable snapshot_resettle_persons = new DataTable("snapshot_resettle_persons");
        #endregion Models

        #region Views
        BindingSource v_resettle_persons = null;
        BindingSource v_snapshot_resettle_persons = null;
        #endregion Views
        private DataGridViewTextBoxColumn id_person;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;


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
            this.DynamicFilter = resettlePersonsViewport.DynamicFilter;
            this.StaticFilter = resettlePersonsViewport.StaticFilter;
            this.ParentRow = resettlePersonsViewport.ParentRow;
            this.ParentType = resettlePersonsViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            List<ResettlePerson> list_from_view = ResettlePersonsFromView();
            List<ResettlePerson> list_from_viewport = ResettlePersonsFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            bool founded = false;
            for (int i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (int j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_person"], 
                dataRowView["id_process"], 
                dataRowView["surname"], 
                dataRowView["name"],
                dataRowView["patronymic"]
            };
        }

        private static bool ValidateResettlePersons(List<ResettlePerson> resettlePersons)
        {
            foreach (ResettlePerson resettlePerson in resettlePersons)
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
            ResettlePerson resettlePerson = new ResettlePerson();
            resettlePerson.IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person");
            resettlePerson.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            resettlePerson.Surname = ViewportHelper.ValueOrNull(row, "surname");
            resettlePerson.Name = ViewportHelper.ValueOrNull(row, "name");
            resettlePerson.Patronymic = ViewportHelper.ValueOrNull(row, "patronymic");
            return resettlePerson;
        }

        private List<ResettlePerson> ResettlePersonsFromViewport()
        {
            List<ResettlePerson> list = new List<ResettlePerson>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    ResettlePerson rp = new ResettlePerson();
                    DataGridViewRow row = dataGridView.Rows[i];
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
            List<ResettlePerson> list = new List<ResettlePerson>();
            for (int i = 0; i < v_resettle_persons.Count; i++)
            {
                ResettlePerson rp = new ResettlePerson();
                DataRowView row = ((DataRowView)v_resettle_persons[i]);
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            resettle_persons = ResettlePersonsDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            resettle_persons.Select();

            v_resettle_persons = new BindingSource();
            v_resettle_persons.DataMember = "resettle_persons";
            v_resettle_persons.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_resettle_persons.Filter += " AND ";
            v_resettle_persons.Filter += DynamicFilter;
            v_resettle_persons.DataSource = DataSetManager.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.ResettleProcess)
                this.Text = String.Format(CultureInfo.InvariantCulture, "Участники переселения №{0}", ParentRow["id_process"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < resettle_persons.Select().Columns.Count; i++)
                snapshot_resettle_persons.Columns.Add(new DataColumn(resettle_persons.Select().Columns[i].ColumnName, resettle_persons.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_resettle_persons.Count; i++)
                snapshot_resettle_persons.Rows.Add(DataRowViewToArray(((DataRowView)v_resettle_persons[i])));
            v_snapshot_resettle_persons = new BindingSource();
            v_snapshot_resettle_persons.DataSource = snapshot_resettle_persons;
            v_snapshot_resettle_persons.CurrentItemChanged += new EventHandler(v_snapshot_resettle_persons_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_resettle_persons;
            id_person.DataPropertyName = "id_person";
            id_process.DataPropertyName = "id_process";
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            resettle_persons.Select().RowChanged += new DataRowChangeEventHandler(ResettlePersonsViewport_RowChanged);
            resettle_persons.Select().RowDeleting += new DataRowChangeEventHandler(ResettlePersonsViewport_RowDeleting);
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
            DataRowView row = (DataRowView)v_snapshot_resettle_persons.AddNew();
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
            for (int i = 0; i < v_resettle_persons.Count; i++)
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
            List<ResettlePerson> list = ResettlePersonsFromViewport();
            if (!ValidateResettlePersons(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = resettle_persons.Select().Rows.Find(((ResettlePerson)list[i]).IdPerson);
                if (row == null)
                {
                    int id_person = ResettlePersonsDataModel.Insert(list[i]);
                    if (id_person == -1)
                    {
                        sync_views = true;
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
                        return;
                    }
                    row["id_process"] = list[i].IdProcess == null ? DBNull.Value : (object)list[i].IdProcess;
                    row["surname"] = list[i].Surname == null ? DBNull.Value : (object)list[i].Surname;
                    row["name"] = list[i].Name == null ? DBNull.Value : (object)list[i].Name;
                    row["patronymic"] = list[i].Patronymic == null ? DBNull.Value : (object)list[i].Patronymic;
                }
            }
            list = ResettlePersonsFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_person"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_person"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_person"].Value == list[i].IdPerson))
                        row_index = j;
                if (row_index == -1)
                {
                    if (ResettlePersonsDataModel.Delete(list[i].IdPerson.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    resettle_persons.Select().Rows.Find(((ResettlePerson)list[i]).IdPerson).Delete();
                }
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
            /*
             * TODO: CalcResettleAggregated
            CalcDataModelTenancyAggregated.GetInstance().Refresh(CalcDataModelFilterEnity.All, null, true);
            CalcDataModelPremiseSubPremisesSumArea.GetInstance().Refresh(CalcDataModelFilterEnity.Premise,
                Int32.Parse(ParentRow["id_premises"].ToString(), CultureInfo.InvariantCulture), true);
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(CalcDataModelFilterEnity.Building,
                Int32.Parse(ParentRow["id_building"].ToString(), CultureInfo.InvariantCulture), true);*/
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            ResettlePersonsViewport viewport = new ResettlePersonsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения об участниках переселения в базу данных?", "Внимание",
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
            resettle_persons.Select().RowChanged -= new DataRowChangeEventHandler(ResettlePersonsViewport_RowChanged);
            resettle_persons.Select().RowDeleting -= new DataRowChangeEventHandler(ResettlePersonsViewport_RowDeleting);
            resettle_persons.Select().RowDeleted -= new DataRowChangeEventHandler(ResettlePersonsViewport_RowDeleted);
        }

        public override void ForceClose()
        {
            resettle_persons.Select().RowChanged -= new DataRowChangeEventHandler(ResettlePersonsViewport_RowChanged);
            resettle_persons.Select().RowDeleting -= new DataRowChangeEventHandler(ResettlePersonsViewport_RowDeleting);
            resettle_persons.Select().RowDeleted -= new DataRowChangeEventHandler(ResettlePersonsViewport_RowDeleted);
            base.Close();
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
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
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
                int row_index = v_snapshot_resettle_persons.Find("id_person", e.Row["id_person"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_resettle_persons[row_index]).Delete();
            }
        }

        void ResettlePersonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            int row_index = v_snapshot_resettle_persons.Find("id_person", e.Row["id_person"]);
            if (row_index == -1 && v_resettle_persons.Find("id_person", e.Row["id_person"]) != -1)
            {
                snapshot_resettle_persons.Rows.Add(new object[] { 
                        e.Row["id_person"], 
                        e.Row["id_process"],                
                        e.Row["surname"],
                        e.Row["name"],
                        e.Row["patronymic"]
                    });
            } else
            if (row_index != -1)
            {
                DataRowView row = ((DataRowView)v_snapshot_resettle_persons[row_index]);
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResettlePersonsViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_person = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_person,
            this.id_process,
            this.surname,
            this.name,
            this.patronymic});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(856, 415);
            this.dataGridView.TabIndex = 1;
            // 
            // id_person
            // 
            this.id_person.HeaderText = "Внутренний идентификатор участника";
            this.id_person.Name = "id_person";
            this.id_person.Visible = false;
            // 
            // id_process
            // 
            this.id_process.HeaderText = "Внутренний идентификатор процесса переселения";
            this.id_process.Name = "id_process";
            this.id_process.Visible = false;
            // 
            // surname
            // 
            this.surname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.surname.HeaderText = "Фамилия";
            this.surname.MinimumWidth = 150;
            this.surname.Name = "surname";
            // 
            // name
            // 
            this.name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.name.HeaderText = "Имя";
            this.name.MinimumWidth = 150;
            this.name.Name = "name";
            // 
            // patronymic
            // 
            this.patronymic.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.patronymic.HeaderText = "Отчество";
            this.patronymic.MinimumWidth = 150;
            this.patronymic.Name = "patronymic";
            // 
            // ResettlePersonsViewport
            // 
            this.ClientSize = new System.Drawing.Size(856, 415);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ResettlePersonsViewport";
            this.Text = "Участники переселения №{0}";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
