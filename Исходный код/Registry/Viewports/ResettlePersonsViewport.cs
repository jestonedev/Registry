using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ResettlePersonsViewport: EditableDataGridViewport
    {

        private ResettlePersonsViewport()
            : this(null)
        {
        }

        public ResettlePersonsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_resettle_persons")
            {
                Locale = CultureInfo.InvariantCulture
            };
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
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var rp = new ResettlePerson();
                var row = ((DataRowView)GeneralBindingSource[i]);
                rp.IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person");
                rp.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                rp.Surname = ViewportHelper.ValueOrNull(row, "surname");
                rp.Name = ViewportHelper.ValueOrNull(row, "name");
                rp.Patronymic = ViewportHelper.ValueOrNull(row, "patronymic");
                list.Add(rp);
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.ResettlePersonsDataModel);
            // Дожидаемся дозагрузки данных, если это необходимо
            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.DataMember = "resettle_persons";
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralBindingSource.DataSource = DataModel.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.ResettleProcess)
                Text = string.Format(CultureInfo.InvariantCulture, "Участники переселения №{0}", ParentRow["id_process"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName, GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource();
            GeneralSnapshotBindingSource.DataSource = GeneralSnapshot;
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_resettle_persons_CurrentItemChanged;

            dataGridView.DataSource = GeneralSnapshotBindingSource;
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
            GeneralDataModel.Select().RowChanged += ResettlePersonsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += ResettlePersonsViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted += ResettlePersonsViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return (ParentType == ParentTypeEnum.ResettleProcess) && (ParentRow != null) && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void InsertRecord()
        {
            if ((ParentRow == null) || (ParentType != ParentTypeEnum.ResettleProcess))
                return;
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            row["id_process"] = ParentRow["id_process"];
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralSnapshotBindingSource.Position != -1) && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
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
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            GeneralDataModel.EditingNewRecord = true;
            var list = ResettlePersonsFromViewport();
            if (!ValidateResettlePersons(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = GeneralDataModel.Select().Rows.Find(list[i].IdPerson);
                if (row == null)
                {
                    var id_person = GeneralDataModel.Insert(list[i]);
                    if (id_person == -1)
                    {
                        sync_views = true; 
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_person"] = id_person;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (RowToResettlePerson(row) == list[i])
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
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
                    if (GeneralDataModel.Delete(list[i].IdPerson.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(list[i].IdPerson).Delete();
                }
            }
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
            GeneralDataModel.Select().RowChanged -= ResettlePersonsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= ResettlePersonsViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= ResettlePersonsViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            GeneralDataModel.Select().RowChanged -= ResettlePersonsViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= ResettlePersonsViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= ResettlePersonsViewport_RowDeleted;
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
                var row_index = GeneralSnapshotBindingSource.Find("id_person", e.Row["id_person"]);
                if (row_index != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[row_index]).Delete();
            }
        }

        void ResettlePersonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = GeneralSnapshotBindingSource.Find("id_person", e.Row["id_person"]);
            if (row_index == -1 && GeneralBindingSource.Find("id_person", e.Row["id_person"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_person"], e.Row["id_process"], e.Row["surname"], e.Row["name"], e.Row["patronymic"]);
            } else
            if (row_index != -1)
            {
                var row = ((DataRowView)GeneralSnapshotBindingSource[row_index]);
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
    }
}
