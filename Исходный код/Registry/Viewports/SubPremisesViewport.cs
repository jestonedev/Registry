using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class SubPremisesViewport: EditableDataGridViewport
    {

        #region Models
        DataModel _objectStates;
        #endregion Models

        #region Views
        BindingSource _vObjectStates;
        #endregion Views

        private SubPremisesViewport()
            : this(null)
        {
        }

        public SubPremisesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_sub_premises") {Locale = CultureInfo.InvariantCulture};
        }

        public SubPremisesViewport(Viewport subPremisesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = subPremisesViewport.DynamicFilter;
            StaticFilter = subPremisesViewport.StaticFilter;
            ParentRow = subPremisesViewport.ParentRow;
            ParentType = subPremisesViewport.ParentType;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_sub_premises"], 
                dataRowView["id_premises"], 
                dataRowView["id_state"], 
                dataRowView["sub_premises_num"], 
                dataRowView["total_area"],
                dataRowView["living_area"],
                dataRowView["description"],
                dataRowView["state_date"]
            };
        }

        private static bool ValidateSubPremises(IEnumerable<Entity> subPremises)
        {
            foreach (var entity in subPremises)
            {
                var subPremise = (SubPremise) entity;
                if (subPremise.IdState == null)
                {
                    MessageBox.Show(@"Необходимо выбрать состояние помещения", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.SubPremisesNum != null && subPremise.SubPremisesNum.Length > 20)
                {
                    MessageBox.Show(@"Длина номера комнаты не может превышать 20 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.SubPremisesNum != null && !Regex.IsMatch(subPremise.SubPremisesNum, "^([0-9]+[а-я]{0,1}|[а-я])$"))
                {
                    MessageBox.Show(@"Некорректно задан номер комнаты. Можно использовать только цифры и не более одной строчной буквы кириллицы", 
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.Description != null && subPremise.Description.Length > 65535)
                {
                    MessageBox.Show(@"Длина примечания комнаты не может превышать 65535 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SubPremise RowToSubPremise(DataRow row)
        {
            var subPremise = new SubPremise
            {
                IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises"),
                IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                SubPremisesNum = ViewportHelper.ValueOrNull(row, "sub_premises_num"),
                TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area"),
                LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date")
            };
            return subPremise;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var sp = new SubPremise();
                var row = dataGridView.Rows[i];
                sp.IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
                sp.IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises");
                sp.IdState = ViewportHelper.ValueOrNull<int>(row, "id_state");
                sp.SubPremisesNum = ViewportHelper.ValueOrNull(row, "sub_premises_num");
                sp.TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area");
                if (ViewportHelper.ValueOrNull<double>(row, "living_area") == 0)
                    if (sp.TotalArea != null) 
                        dataGridView.Rows[i].Cells["living_area"].Value = sp.TotalArea.Value;
                sp.LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area");
                sp.Description = ViewportHelper.ValueOrNull(row, "description");
                sp.StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date");
                list.Add(sp);
            }
            return list;
        }

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            for (var i = 0; i < GeneralBindingSource.Count; i++)
            {
                var sp = new SubPremise();
                var row = ((DataRowView)GeneralBindingSource[i]);
                sp.IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
                sp.IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises");
                sp.IdState = ViewportHelper.ValueOrNull<int>(row, "id_state");
                sp.SubPremisesNum = ViewportHelper.ValueOrNull(row, "sub_premises_num");
                sp.TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area");
                sp.LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area");
                sp.Description = ViewportHelper.ValueOrNull(row, "description");
                sp.StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date");
                list.Add(sp);
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
            GeneralDataModel = DataModel.GetInstance(DataModelType.SubPremisesDataModel);
            _objectStates = DataModel.GetInstance(DataModelType.ObjectStatesDataModel);
            // Дожидаемся дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _objectStates.Select();

            _vObjectStates = new BindingSource
            {
                DataMember = "object_states",
                DataSource = DataModel.DataSet
            };

            GeneralBindingSource = new BindingSource
            {
                DataMember = "sub_premises",
                Filter = StaticFilter
            };
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralBindingSource.DataSource = DataModel.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.Premises)
                Text = string.Format(CultureInfo.InvariantCulture, "Комнаты помещения №{0}", ParentRow["id_premises"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(GeneralDataModel.Select().Columns[i].ColumnName, GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < GeneralBindingSource.Count; i++)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)GeneralBindingSource[i])));
            GeneralSnapshotBindingSource = new BindingSource {DataSource = GeneralSnapshot};
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_sub_premises_CurrentItemChanged;

            dataGridView.DataSource = GeneralSnapshotBindingSource;
            id_sub_premises.DataPropertyName = "id_sub_premises";
            id_premises.DataPropertyName = "id_premises";
            sub_premises_num.DataPropertyName = "sub_premises_num";
            total_area.DataPropertyName = "total_area";
            living_area.DataPropertyName = "living_area";
            description.DataPropertyName = "description";
            id_state.DataSource = _vObjectStates;
            id_state.ValueMember = "id_state";
            id_state.DisplayMember = "state_female";
            id_state.DataPropertyName = "id_state";
            state_date.DataPropertyName = "state_date";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            dataGridView.CellValidated += dataGridView_CellValidated;
            dataGridView.DataError += dataGridView_DataError;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            GeneralDataModel.Select().RowChanged += SubPremisesViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += SubPremisesViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted += SubPremisesViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return (ParentType == ParentTypeEnum.Premises) && (ParentRow != null) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            if ((ParentRow == null) || (ParentType != ParentTypeEnum.Premises))
                return;
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
            if (row == null) return;
            row["id_premises"] = ParentRow["id_premises"];
            row["total_area"] = 0;
            row["living_area"] = 0;
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
            var list = EntitiesListFromViewport();
            if (!ValidateSubPremises(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var subPremise = (SubPremise) list[i];
                var row = GeneralDataModel.Select().Rows.Find(subPremise.IdSubPremises);
                if (row == null)
                {
                    if (subPremise.IdState != null && (new[] { 4, 5, 9 }.Contains(subPremise.IdState.Value) && 
                        !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal)))
                    {
                        MessageBox.Show(@"У вас нет прав на добавление в базу муниципальных жилых помещений", @"Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    if (subPremise.IdState != null && (new[] { 1, 3, 6, 7, 8 }.Contains(subPremise.IdState.Value) && 
                        !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)))
                    {
                        MessageBox.Show(@"У вас нет прав на добавление в базу немуниципальных жилых помещений", @"Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    var idSubPremises = GeneralDataModel.Insert(subPremise);
                    if (idSubPremises == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_sub_premises"] = idSubPremises;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {
                    var subPremiseFromView = RowToSubPremise(row);
                    if (subPremiseFromView == subPremise)
                        continue;
                    if (subPremiseFromView.IdSubPremises != null 
                        && (DataModelHelper.HasMunicipal(subPremiseFromView.IdSubPremises.Value, EntityType.SubPremise)
                        && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal)))
                    {
                        MessageBox.Show(@"Вы не можете изменить информацию по данной комнате, т.к. она является муниципальной",
                            @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    if (subPremiseFromView.IdSubPremises != null 
                        && (DataModelHelper.HasNotMunicipal(subPremiseFromView.IdSubPremises.Value, EntityType.SubPremise)
                        && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)))
                    {
                        MessageBox.Show(@"Вы не можете изменить информацию по данной комнате, т.к. она является немуниципальной",
                            @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    if (GeneralDataModel.Update(subPremise) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["id_premises"] = subPremise.IdPremises == null ? DBNull.Value : (object)subPremise.IdPremises;
                    row["id_state"] = subPremise.IdState == null ? DBNull.Value : (object)subPremise.IdState;
                    row["sub_premises_num"] = subPremise.SubPremisesNum == null ? DBNull.Value : (object)subPremise.SubPremisesNum;
                    row["total_area"] = subPremise.TotalArea == null ? DBNull.Value : (object)subPremise.TotalArea;
                    row["living_area"] = subPremise.LivingArea == null ? DBNull.Value : (object)subPremise.LivingArea;
                    row["description"] = subPremise.Description == null ? DBNull.Value : (object)subPremise.Description;
                    row["state_date"] = subPremise.StateDate == null ? DBNull.Value : (object)subPremise.StateDate;
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var subPremise = (SubPremise) entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_sub_premises"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_sub_premises"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_sub_premises"].Value == subPremise.IdSubPremises))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (subPremise.IdSubPremises != null 
                    && (DataModelHelper.HasMunicipal(subPremise.IdSubPremises.Value, EntityType.SubPremise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal)))
                {
                    MessageBox.Show(@"Вы не можете удалить муниципальную комнату, т.к. не имеете на это прав",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    sync_views = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                if (subPremise.IdSubPremises != null 
                    && (DataModelHelper.HasNotMunicipal(subPremise.IdSubPremises.Value, EntityType.SubPremise)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)))
                {
                    MessageBox.Show(@"Вы не можете удалить немуниципальную комнату, т.к. не имеете на это прав",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    sync_views = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                if (subPremise.IdSubPremises != null 
                    && GeneralDataModel.Delete(subPremise.IdSubPremises.Value) == -1)
                {
                    sync_views = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                GeneralDataModel.Select().Rows.Find(subPremise.IdSubPremises).Delete();
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
            var viewport = new SubPremisesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения о комнатах в базу данных?", @"Внимание",
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
            GeneralDataModel.Select().RowChanged -= SubPremisesViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= SubPremisesViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= SubPremisesViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            GeneralDataModel.Select().RowChanged -= SubPremisesViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= SubPremisesViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= SubPremisesViewport_RowDeleted;
            Close();
        }

        public override bool HasAssocViewport(ViewportType viewportType)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.FundsHistoryViewport,
                ViewportType.TenancyListViewport
            };
            return reports.Contains(viewportType) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport(ViewportType viewportType)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Перед открытием связных объектов необходимо сохранить изменения в базу данных. " +
                    @"Вы хотите это сделать?", @"Внимание",
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
                        return;
                }
            }
            if (GeneralSnapshotBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрана комната для отображения истории принадлежности к фондам", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType, "id_sub_premises = " +
                Convert.ToInt32(((DataRowView)GeneralSnapshotBindingSource[GeneralSnapshotBindingSource.Position])["id_sub_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralSnapshotBindingSource[GeneralSnapshotBindingSource.Position]).Row, ParentTypeEnum.SubPremises);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            switch (reporterType)
            {
                case  ReporterType.RegistryExcerptReporterSubPremise:
                    return (GeneralSnapshotBindingSource.Count > 0);
                case  ReporterType.RegistryExcerptReporterPremise:
                case ReporterType.RegistryExcerptReporterAllMunSubPremises:
                    return true;
            }
            return false;
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Перед открытием истории принадлежности фондам необходимо сохранить изменения в базу данных. " +
                    @"Вы хотите это сделать?", @"Внимание",
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
                        return;
                }
            }
            if (GeneralSnapshotBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрана комната для отображения истории принадлежности к фондам", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var reporter = ReporterFactory.CreateReporter(reporterType);
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.RegistryExcerptReporterSubPremise:
                    arguments = RegistryExcerptReporterSubPremiseArguments();
                    break;
                case ReporterType.RegistryExcerptReporterPremise:
                    arguments = RegistryExcerptPremiseReportArguments();
                    break;
                case ReporterType.RegistryExcerptReporterAllMunSubPremises:
                    arguments = RegistryExcerptReporterAllMunSubPremisesArguments();
                    break;
            }
            reporter.Run(arguments);
        }

        private Dictionary<string, string> RegistryExcerptPremiseReportArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", ParentRow["id_premises"].ToString()},
                {"excerpt_type", "1"}
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptReporterSubPremiseArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {
                    "ids",
                    ((DataRowView) GeneralSnapshotBindingSource[GeneralSnapshotBindingSource.Position])[
                        "id_sub_premises"].ToString()
                },
                {"excerpt_type", "2"}
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptReporterAllMunSubPremisesArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", ParentRow["id_premises"].ToString()},
                {"excerpt_type", "3"}
            };
            return arguments;
        } 

        void v_snapshot_sub_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
                MenuCallback.DocumentsStateUpdate();
            }
        }
        
        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "total_area":
                case "living_area":
                    double stub;
                    if ((string.IsNullOrEmpty(cell.Value.ToString()) || (!double.TryParse(cell.Value.ToString(), out stub))))
                        cell.Value = 0;
                    break;
                case "sub_premises_num":
                    if (cell.Value.ToString().Trim().Length > 20)
                        cell.ErrorText = "Длина номера комнаты не может превышать 20 символов";
                    else
                        if ((cell.Value.ToString().Trim().Length > 0) && !Regex.IsMatch(cell.Value.ToString().Trim(), "^([0-9]+[а-я]{0,1}|[а-я])$"))
                            cell.ErrorText = "Номер комнаты может содержать в себе только цифры и не более одной строчной буквы кирилицы";
                        else
                            cell.ErrorText = "";
                    break;
                case "description":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 65535 ? 
                        "Длина примечания комнаты не может превышать 65535 символов" : "";
                    break;
            }
        }

        void SubPremisesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
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

        void SubPremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = GeneralSnapshotBindingSource.Find("id_sub_premises", e.Row["id_sub_premises"]);
                if (rowIndex != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
            }
        }

        void SubPremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_sub_premises", e.Row["id_sub_premises"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_sub_premises", e.Row["id_sub_premises"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_sub_premises"], e.Row["id_premises"], e.Row["id_state"], e.Row["sub_premises_num"], e.Row["total_area"], e.Row["living_area"], e.Row["description"], e.Row["state_date"]);
            } else
            if (rowIndex != -1)
            {
                var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
                row["id_premises"] = e.Row["id_premises"];
                row["id_state"] = e.Row["id_state"];
                row["sub_premises_num"] = e.Row["sub_premises_num"];
                row["total_area"] = e.Row["total_area"];
                row["living_area"] = e.Row["living_area"];
                row["description"] = e.Row["description"];
                row["state_date"] = e.Row["state_date"];
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

        void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name == "total_area" || dataGridView.CurrentCell.OwningColumn.Name == "living_area")
            {
                dataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
                dataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
                if (string.IsNullOrEmpty(((TextBox)e.Control).Text.Trim()))
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text = @"0";
                else
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text.Substring(0, ((TextBox)e.Control).Text.Length - 3);
            } else
                if (dataGridView.CurrentCell.OwningColumn.Name == "sub_premises_num")
                {
                    dataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
                    dataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
                }
                else
                    if (dataGridView.CurrentCell.OwningColumn.Name == "id_state")
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

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "total_area":
                    MessageBox.Show(@"Значение общей площади комнаты является некорректным",@"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.ThrowException = false;
                    break;
                case "living_area":
                    MessageBox.Show(@"Значение жилой площади комнаты является некорректным", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.ThrowException = false;
                    break;
            }
        }
        
        void EditingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView.SelectedCells[0].OwningColumn.Name == "total_area" || dataGridView.SelectedCells[0].OwningColumn.Name == "living_area")
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == (char)8))
                    e.Handled = false;
                else
                    if ((e.KeyChar == '.') || (e.KeyChar == ','))
                    {
                        e.KeyChar = ',';
                        e.Handled = ((TextBox)dataGridView.EditingControl).Text.IndexOf(',') != -1;
                    }
                    else
                        e.Handled = true;
            } else
                if (dataGridView.SelectedCells[0].OwningColumn.Name == "sub_premises_num")
                {
                    if (e.KeyChar >= 'А' && e.KeyChar <= 'Я')
                        e.KeyChar = e.KeyChar.ToString().ToLower(CultureInfo.CurrentCulture)[0];
                    if (e.KeyChar == ' ')
                        e.Handled = true;
                }
        }
    }
}
