using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;
using System.Linq;
using Registry.DataModels;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;

namespace Registry.Viewport
{
    internal sealed partial class ResettleProcessListViewport: FormWithGridViewport
    {
        #region Models

        private DataModel _documentsResidence;
        private CalcDataModel _resettleAggregate;
        #endregion Models

        #region Views

        private BindingSource _vDocumentsResidence;
        private BindingSource _vResettleAggregate;
        #endregion Views

        private SearchForm _spExtendedSearchForm;
        private SearchForm _spSimpleSearchForm;

        private ResettleProcessListViewport()
            : this(null, null)
        {
        }

        public ResettleProcessListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        private void SetViewportCaption()
        {
            if (ViewportState == ViewportState.NewRowState)
                Text = @"Новая исковая работа";
            else
                if (GeneralBindingSource.Position != -1)
                        Text = string.Format(CultureInfo.InvariantCulture, "Процесс переселения №{0}",
                            ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"]);
                else
                        Text = @"Процессы переселения отсутствуют";
        }

        private void DataBind()
        {
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
            textBoxDocNumber.DataBindings.Clear();
            textBoxDocNumber.DataBindings.Add("Text", GeneralBindingSource, "doc_number", true, DataSourceUpdateMode.Never, "");
            dateTimePickerResettleDate.DataBindings.Clear();
            dateTimePickerResettleDate.DataBindings.Add("Value", GeneralBindingSource, "resettle_date", true, DataSourceUpdateMode.Never, null);
            numericUpDownDebts.DataBindings.Clear();
            numericUpDownDebts.DataBindings.Add("Value", GeneralBindingSource, "debts", true, DataSourceUpdateMode.Never, 0);
            comboBoxDocumentResidence.DataSource = _vDocumentsResidence;
            comboBoxDocumentResidence.ValueMember = "id_document_residence";
            comboBoxDocumentResidence.DisplayMember = "document_residence";
            comboBoxDocumentResidence.DataBindings.Clear();
            comboBoxDocumentResidence.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_document_residence", true, DataSourceUpdateMode.Never, DBNull.Value);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            if (GeneralBindingSource.Count == 0) return;
            var row = GeneralBindingSource.Position >= 0 ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["resettle_date"] != DBNull.Value)))
                dateTimePickerResettleDate.Checked = true;
            else
            {
                dateTimePickerResettleDate.Value = DateTime.Now.Date;
                dateTimePickerResettleDate.Checked = false;
            }
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.ResettleWrite))
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidateResettleProcess(ResettleProcess resettleProcess)
        {
            if (resettleProcess.IdDocumentResidence == null)
            {
                MessageBox.Show(@"Необходимо выбрать документ-основание на проживание", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDocumentResidence.Focus();
                return false;
            }
            return true;
        }

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return EntityConverter<ResettleProcess>.FromRow(row);
        }

        protected override Entity EntityFromViewport()
        {
            var resettleProcess = new ResettleProcess();
            if ((GeneralBindingSource.Position == -1) || ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"] is DBNull)
                resettleProcess.IdProcess = null;
            else
                resettleProcess.IdProcess = 
                    Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], CultureInfo.InvariantCulture);
            resettleProcess.Debts = numericUpDownDebts.Value;
            resettleProcess.DocNumber = ViewportHelper.ValueOrNull(textBoxDocNumber);
            resettleProcess.ResettleDate = ViewportHelper.ValueOrNull(dateTimePickerResettleDate);
            resettleProcess.IdDocumentResidence = ViewportHelper.ValueOrNull<int>(comboBoxDocumentResidence);
            resettleProcess.Description = ViewportHelper.ValueOrNull(textBoxDescription);
            return resettleProcess;
        }

        private void ViewportFromResettleProcess(ResettleProcess resettleProcess)
        {
            numericUpDownDebts.Value = ViewportHelper.ValueOrDefault(resettleProcess.Debts);
            dateTimePickerResettleDate.Value = ViewportHelper.ValueOrDefault(resettleProcess.ResettleDate);
            comboBoxDocumentResidence.SelectedValue = ViewportHelper.ValueOrDbNull(resettleProcess.IdDocumentResidence);
            textBoxDescription.Text = resettleProcess.Description;
            textBoxDocNumber.Text = resettleProcess.DocNumber;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            DockAreas = DockAreas.Document;
            dataGridView.AutoGenerateColumns = false;
            GeneralDataModel = EntityDataModel<ResettleProcess>.GetInstance();
            _documentsResidence = DataModel.GetInstance<EntityDataModel<DocumentResidence>>();
            _resettleAggregate = CalcDataModel.GetInstance<CalcDataModelResettleAggregated>();

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            _documentsResidence.Select();

            var ds = DataStorage.DataSet;

            _vDocumentsResidence = new BindingSource
            {
                DataMember = "documents_residence",
                DataSource = ds
            };

            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);
            GeneralBindingSource.DataMember = "resettle_processes";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;

            _vResettleAggregate = new BindingSource {DataSource = _resettleAggregate.Select()};

            DataBind();

            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", ResettleProcessListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", ResettleProcessListViewport_RowDeleted);

            dataGridView.RowCount = GeneralBindingSource.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridView);
            AddEventHandler<EventArgs>(_resettleAggregate, "RefreshEvent", resettles_aggregate_RefreshEvent);
            IsEditable = true;
            DataChangeHandlersInit();
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            GeneralBindingSource.AddNew();
            IsEditable = true;
            dataGridView.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            var resettleProcess = (ResettleProcess) EntityFromView();
            dataGridView.RowCount = dataGridView.RowCount + 1;
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromResettleProcess(resettleProcess);
            dateTimePickerResettleDate.Checked = (resettleProcess.ResettleDate != null);
            IsEditable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_process"]) == -1)
                    return;
                IsEditable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                IsEditable = true;
                ViewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            if (!string.IsNullOrEmpty(DynamicFilter))
                return true;
            else
                return false;
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (_spSimpleSearchForm == null)
                        _spSimpleSearchForm = new SimpleSearchResettleForm();
                    if (_spSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _spSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (_spExtendedSearchForm == null)
                        _spExtendedSearchForm = new ExtendedSearchResettleForm();
                    if (_spExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _spExtendedSearchForm.GetFilter();
                    break;
            }
            var filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            dataGridView.RowCount = 0;
            GeneralBindingSource.Filter = filter;
            dataGridView.RowCount = GeneralBindingSource.Count;
        }

        public override void ClearSearch()
        {
            GeneralBindingSource.Filter = StaticFilter;
            dataGridView.RowCount = GeneralBindingSource.Count;
            DynamicFilter = "";
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void SaveRecord()
        {
            var resettleProcess = (ResettleProcess) EntityFromViewport();
            if (!ValidateResettleProcess(resettleProcess))
                return;
            var filter = "";
            if (!string.IsNullOrEmpty(GeneralBindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idProcess = GeneralDataModel.Insert(resettleProcess);
                    if (idProcess == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    resettleProcess.IdProcess = idProcess;
                    IsEditable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", resettleProcess.IdProcess);
                    GeneralBindingSource.Filter += filter;
                    EntityConverter<ResettleProcess>.FillRow(resettleProcess, newRow);
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (resettleProcess.IdProcess == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о процессе переселения без внутреннего номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(resettleProcess) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    IsEditable = false;
                    filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", resettleProcess.IdProcess);
                    GeneralBindingSource.Filter += filter;
                    EntityConverter<ResettleProcess>.FillRow(resettleProcess, row);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            dataGridView.Enabled = true;
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool CanCancelRecord()
        {
            return (ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (ViewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        IsEditable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        dataGridView.RowCount = dataGridView.RowCount - 1;
                        if (GeneralBindingSource.Position != -1)
                            dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    IsEditable = false;
                    DataBind();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.ResettlePersonsViewport,
                ViewportType.ResettleFromBuildingsViewport,
                ViewportType.ResettleFromPremisesViewport,
                ViewportType.ResettleToBuildingsViewport,
                ViewportType.ResettleToPremisesViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
        }

        public override bool HasAssocViewport<T>(ResettleEstateObjectWay way)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.ResettlePersonsViewport,              
                ViewportType.ResettleBuildingsViewport,
                ViewportType.ResettlePremisesViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport<T>(ResettleEstateObjectWay way)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбран процесс переселения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MenuCallback == null)
                throw new ViewportException("Не заданна ссылка на интерфейс menuCallback");
            var viewport = ViewportFactory.CreateViewport<T>(MenuCallback);
            viewport.StaticFilter = "id_process = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"],
                    CultureInfo.InvariantCulture);
            viewport.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
            viewport.ParentType = ParentTypeEnum.ResettleProcess;
            if (typeof(T) == typeof(ResettleBuildingsViewport) || typeof(T) == typeof(ResettlePremisesViewport))
            {
                if (viewport is ResettleBuildingsViewport)
                    ((ResettleBuildingsViewport)viewport).Way = way;
                if (viewport is ResettlePremisesViewport)
                    ((ResettlePremisesViewport)viewport).Way = way;
            }
            if (((IMenuController)viewport).CanLoadData())
                ((IMenuController)viewport).LoadData();
            MenuCallback.AddViewport(viewport);                     
        }

        public override void ShowAssocViewport<T>()
        {
            ShowAssocViewport<T>(ResettleEstateObjectWay.None);
        }

        private void resettles_aggregate_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        private void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            if (GeneralBindingSource.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (GeneralBindingSource.Position >= dataGridView.RowCount)
                {
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
                }
                else
                    if (dataGridView.Rows[GeneralBindingSource.Position].Selected != true)
                    {
                        dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                        dataGridView.CurrentCell = dataGridView.Rows[GeneralBindingSource.Position].Cells[0];
                    }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            if (GeneralBindingSource.Position == -1)
                return;
            if (ViewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            ViewportState = ViewportState.ReadState;
            IsEditable = true;
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
            dataGridView.Refresh();
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            var row = (DataRowView)GeneralBindingSource[e.RowIndex];
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_process":
                case "doc_number":
                    e.Value = row[dataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "resettle_date":
                    e.Value = row["resettle_date"] == DBNull.Value ? "" :
                        ((DateTime)row["resettle_date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "resettle_persons":
                    var rowIndex = _vResettleAggregate.Find("id_process", row["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vResettleAggregate[rowIndex])["resettlers"];
                    break;
                case "address_from":
                    rowIndex = _vResettleAggregate.Find("id_process", row["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vResettleAggregate[rowIndex])["address_from"];
                    break;
                case "address_to":
                    rowIndex = _vResettleAggregate.Find("id_process", row["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vResettleAggregate[rowIndex])["address_to"];
                    break;   
            }
        }

        private void ResettleProcessListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                dataGridView.RowCount = GeneralBindingSource.Count;
                dataGridView.Refresh();
                UnbindedCheckBoxesUpdate();
                MenuCallback.ForceCloseDetachedViewports();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        private void ResettleProcessListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = GeneralBindingSource.Count;
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            numericUpDownDebts.Focus();
            base.OnVisibleChanged(e);
        }
    }
}
