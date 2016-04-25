using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ResettlePersonsViewport: EditableDataGridViewport
    {
        private ResettlePersonsViewport()
            : this(null, null)
        {
        }

        public ResettlePersonsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_resettle_persons")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_person"], 
                dataRowView["id_process"], 
                dataRowView["surname"], 
                dataRowView["name"],
                dataRowView["patronymic"],
                dataRowView["document_num"],
                dataRowView["document_seria"],
                dataRowView["founding_doc"]
            };
        }

        private static bool ValidateResettlePersons(IEnumerable<Entity> resettlePersons)
        {
            foreach (var entity in resettlePersons)
            {
                var resettlePerson = (ResettlePerson) entity;
                if (resettlePerson.Surname == null)
                {
                    MessageBox.Show(@"Фамилия и имя являются обязательными для заполнения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Name == null)
                {
                    MessageBox.Show(@"Фамилия и имя являются обязательными для заполнения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Surname != null && resettlePerson.Surname.Length > 50)
                {
                    MessageBox.Show(@"Длина фамилии не может превышать 50 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Name != null && resettlePerson.Name.Length > 50)
                {
                    MessageBox.Show(@"Длина имени не может превышать 50 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Patronymic != null && resettlePerson.Patronymic.Length > 255)
                {
                    MessageBox.Show(@"Длина отчества не может превышать 255 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static ResettlePerson RowToResettlePerson(DataRow row)
        {
            var resettlePerson = new ResettlePerson
            {
                IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                Surname = ViewportHelper.ValueOrNull(row, "surname"),
                Name = ViewportHelper.ValueOrNull(row, "name"),
                Patronymic = ViewportHelper.ValueOrNull(row, "patronymic"),
                DocumentNum = ViewportHelper.ValueOrNull(row, "document_num"),
                DocumentSeria = ViewportHelper.ValueOrNull(row, "document_seria"),
                FoundingDoc = ViewportHelper.ValueOrNull(row, "founding_doc")
            };
            return resettlePerson;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var rp = new ResettlePerson();
                var row = dataGridView.Rows[i];
                rp.IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person");
                rp.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                rp.Surname = ViewportHelper.ValueOrNull(row, "surname");
                rp.Name = ViewportHelper.ValueOrNull(row, "name");
                rp.Patronymic = ViewportHelper.ValueOrNull(row, "patronymic");
                rp.DocumentNum = ViewportHelper.ValueOrNull(row, "document_num");
                rp.DocumentSeria = ViewportHelper.ValueOrNull(row, "document_seria");
                rp.FoundingDoc = ViewportHelper.ValueOrNull(row, "founding_doc");
                list.Add(rp);
            }
            return list;
        }

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var rp = new ResettlePerson();
                var row = ((DataRowView)GeneralBindingSource[i]);
                rp.IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person");
                rp.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
                rp.Surname = ViewportHelper.ValueOrNull(row, "surname");
                rp.Name = ViewportHelper.ValueOrNull(row, "name");
                rp.Patronymic = ViewportHelper.ValueOrNull(row, "patronymic");
                rp.DocumentNum = ViewportHelper.ValueOrNull(row, "document_num");
                rp.DocumentSeria = ViewportHelper.ValueOrNull(row, "document_seria");
                rp.FoundingDoc = ViewportHelper.ValueOrNull(row, "founding_doc");
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
            GeneralDataModel = DataModel.GetInstance<ResettlePersonsDataModel>();
            // Дожидаемся дозагрузки данных, если это необходимо
            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource
            {
                DataMember = "resettle_persons",
                Filter = StaticFilter
            };
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
            GeneralSnapshotBindingSource = new BindingSource {DataSource = GeneralSnapshot};
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_resettle_persons_CurrentItemChanged;

            dataGridView.DataSource = GeneralSnapshotBindingSource;
            id_person.DataPropertyName = "id_person";
            id_process.DataPropertyName = "id_process";
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            document_num.DataPropertyName = "document_num";
            document_seria.DataPropertyName = "document_seria";
            founding_doc.DataPropertyName = "founding_doc";
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
            if (row == null) return;
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
            dataGridView.EndEdit();
            GeneralDataModel.EditingNewRecord = true;
            var list = EntitiesListFromViewport();
            if (!ValidateResettlePersons(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var person = (ResettlePerson) list[i];
                var row = GeneralDataModel.Select().Rows.Find(person.IdPerson);
                if (row == null)
                {
                    var idPerson = GeneralDataModel.Insert(person);
                    if (idPerson == -1)
                    {
                        sync_views = true; 
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_person"] = idPerson;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    if (RowToResettlePerson(row) == person)
                        continue;
                    if (GeneralDataModel.Update(person) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["id_process"] = person.IdProcess == null ? DBNull.Value : (object)person.IdProcess;
                    row["surname"] = person.Surname == null ? DBNull.Value : (object)person.Surname;
                    row["name"] = person.Name == null ? DBNull.Value : (object)person.Name;
                    row["patronymic"] = person.Patronymic == null ? DBNull.Value : (object)person.Patronymic;
                    row["document_num"] = person.DocumentNum == null ? DBNull.Value : (object)person.DocumentNum;
                    row["document_seria"] = person.DocumentSeria == null ? DBNull.Value : (object)person.DocumentSeria;
                    row["founding_doc"] = person.FoundingDoc == null ? DBNull.Value : (object)person.FoundingDoc;
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var person = (ResettlePerson)entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_person"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_person"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_person"].Value == person.IdPerson))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (person.IdPerson != null && GeneralDataModel.Delete(person.IdPerson.Value) == -1)
                {
                    sync_views = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                GeneralDataModel.Select().Rows.Find(person.IdPerson).Delete();
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
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения об участниках переселения в базу данных?", @"Внимание",
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
            GeneralSnapshotBindingSource.CurrentItemChanged -= v_snapshot_resettle_persons_CurrentItemChanged;
            dataGridView.CellValidated -= dataGridView_CellValidated;
            dataGridView.CellValueChanged -= dataGridView_CellValueChanged;
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
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 50 ? 
                        "Длина фамилии не может превышать 50 символов" : "";
                    break;
                case "name":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 50 ? 
                        "Длина имени не может превышать 50 символов" : "";
                    break;
                case "patronymic":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 255 ? 
                        "Длина отчества не может превышать 255 символов" : "";
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
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_person", e.Row["id_person"]);
            if (rowIndex != -1)
                ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
        }

        void ResettlePersonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_person", e.Row["id_person"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_person", e.Row["id_person"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_person"], e.Row["id_process"], e.Row["surname"], e.Row["name"],
                    e.Row["patronymic"], e.Row["document_num"], e.Row["document_seria"], e.Row["founding_doc"]);
            } else
            if (rowIndex != -1)
            {
                var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
                row["id_process"] = e.Row["id_process"];
                row["surname"] = e.Row["surname"];
                row["name"] = e.Row["name"];
                row["patronymic"] = e.Row["patronymic"];
                row["document_num"] = e.Row["document_num"];
                row["document_seria"] = e.Row["document_seria"];
                row["founding_doc"] = e.Row["founding_doc"];
            }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }
    }
}
