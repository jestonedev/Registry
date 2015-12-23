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
using Registry.SearchForms;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ResettleProcessListViewport: FormWithGridViewport
    {
        #region Models
        DataModel documents_residence;
        CalcDataModel resettle_aggregate;
        #endregion Models

        #region Views
        BindingSource v_documents_residence;
        BindingSource v_resettle_aggregate;
        #endregion Views

        private SearchForm spExtendedSearchForm;
        private SearchForm spSimpleSearchForm;

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
            if (viewportState == ViewportState.NewRowState)
                Text = "Новая исковая работа";
            else
                if (GeneralBindingSource.Position != -1)
                        Text = string.Format(CultureInfo.InvariantCulture, "Процесс переселения №{0}",
                            ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"]);
                else
                        Text = "Процессы переселения отсутствуют";
        }

        private void DataBind()
        {
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerResettleDate.DataBindings.Clear();
            dateTimePickerResettleDate.DataBindings.Add("Value", GeneralBindingSource, "resettle_date", true, DataSourceUpdateMode.Never, null);
            numericUpDownDebts.DataBindings.Clear();
            numericUpDownDebts.DataBindings.Add("Value", GeneralBindingSource, "debts", true, DataSourceUpdateMode.Never, 0);
            comboBoxDocumentResidence.DataSource = v_documents_residence;
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
            viewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidateResettleProcess(ResettleProcess resettleProcess)
        {
            if (resettleProcess.IdDocumentResidence == null)
            {
                MessageBox.Show("Необходимо выбрать документ-основание на проживание", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDocumentResidence.Focus();
                return false;
            }
            return true;
        }

        protected override Entity EntityFromView()
        {
            var resettle_process = new ResettleProcess();
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            resettle_process.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            resettle_process.Debts = ViewportHelper.ValueOrNull<decimal>(row, "debts");
            resettle_process.ResettleDate = ViewportHelper.ValueOrNull<DateTime>(row, "resettle_date");
            resettle_process.IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence");
            resettle_process.Description = ViewportHelper.ValueOrNull(row, "description");
            return resettle_process;
        }

        protected override Entity EntityFromViewport()
        {
            var resettle_process = new ResettleProcess();
            if ((GeneralBindingSource.Position == -1) || ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"] is DBNull)
                resettle_process.IdProcess = null;
            else
                resettle_process.IdProcess = 
                    Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], CultureInfo.InvariantCulture);
            resettle_process.Debts = numericUpDownDebts.Value;
            resettle_process.ResettleDate = ViewportHelper.ValueOrNull(dateTimePickerResettleDate);
            resettle_process.IdDocumentResidence = ViewportHelper.ValueOrNull<int>(comboBoxDocumentResidence);
            resettle_process.Description = ViewportHelper.ValueOrNull(textBoxDescription);
            return resettle_process;
        }

        private void ViewportFromResettleProcess(ResettleProcess resettleProcess)
        {
            numericUpDownDebts.Value = ViewportHelper.ValueOrDefault(resettleProcess.Debts);
            dateTimePickerResettleDate.Value = ViewportHelper.ValueOrDefault(resettleProcess.ResettleDate);
            comboBoxDocumentResidence.SelectedValue = ViewportHelper.ValueOrDBNull(resettleProcess.IdDocumentResidence);
            textBoxDescription.Text = resettleProcess.Description;
        }

        private static void FillRowFromResettleProcess(ResettleProcess resettleProcess, DataRowView row)
        {
            row.BeginEdit();
            row["id_process"] = ViewportHelper.ValueOrDBNull(resettleProcess.IdProcess);
            row["resettle_date"] = ViewportHelper.ValueOrDBNull(resettleProcess.ResettleDate);
            row["debts"] = ViewportHelper.ValueOrDBNull(resettleProcess.Debts);
            row["id_document_residence"] = ViewportHelper.ValueOrDBNull(resettleProcess.IdDocumentResidence);
            row["description"] = ViewportHelper.ValueOrDBNull(resettleProcess.Description);
            row.EndEdit();
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            DockAreas = DockAreas.Document;
            dataGridView.AutoGenerateColumns = false;
            GeneralDataModel = DataModel.GetInstance(DataModelType.ResettleProcessesDataModel);
            documents_residence = DataModel.GetInstance(DataModelType.DocumentsResidenceDataModel);
            resettle_aggregate = CalcDataModel.GetInstance(CalcDataModelType.CalcDataModelResettleAggregated);

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            documents_residence.Select();

            var ds = DataModel.DataSet;

            v_documents_residence = new BindingSource();
            v_documents_residence.DataMember = "documents_residence";
            v_documents_residence.DataSource = ds;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataMember = "resettle_processes";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;

            v_resettle_aggregate = new BindingSource();
            v_resettle_aggregate.DataSource = resettle_aggregate.Select();

            DataBind();

            GeneralDataModel.Select().RowChanged += ResettleProcessListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += ResettleProcessListViewport_RowDeleted;

            dataGridView.RowCount = GeneralBindingSource.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridView);
            resettle_aggregate.RefreshEvent += resettles_aggregate_RefreshEvent;
            is_editable = true;
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
            is_editable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            GeneralBindingSource.AddNew();
            is_editable = true;
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
            is_editable = false;
            var resettleProcess = (ResettleProcess) EntityFromView();
            dataGridView.RowCount = dataGridView.RowCount + 1;
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromResettleProcess(resettleProcess);
            dateTimePickerResettleDate.Checked = (resettleProcess.ResettleDate != null);
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_process"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
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
                    if (spSimpleSearchForm == null)
                        spSimpleSearchForm = new SimpleSearchResettleForm();
                    if (spSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = spSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (spExtendedSearchForm == null)
                        spExtendedSearchForm = new ExtendedSearchResettleForm();
                    if (spExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = spExtendedSearchForm.GetFilter();
                    break;
            }
            var Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            dataGridView.RowCount = 0;
            GeneralBindingSource.Filter = Filter;
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
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void SaveRecord()
        {
            var resettleProcess = (ResettleProcess) EntityFromViewport();
            if (!ValidateResettleProcess(resettleProcess))
                return;
            var Filter = "";
            if (!string.IsNullOrEmpty(GeneralBindingSource.Filter))
                Filter += " OR ";
            else
                Filter += "(1 = 1) OR ";
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var id_process = GeneralDataModel.Insert(resettleProcess);
                    if (id_process == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    resettleProcess.IdProcess = id_process;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", resettleProcess.IdProcess);
                    GeneralBindingSource.Filter += Filter;
                    FillRowFromResettleProcess(resettleProcess, newRow);
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (resettleProcess.IdProcess == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись о процессе переселения без внутреннего номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(resettleProcess) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    is_editable = false;
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", resettleProcess.IdProcess);
                    GeneralBindingSource.Filter += Filter;
                    FillRowFromResettleProcess(resettleProcess, row);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            dataGridView.Enabled = true;
            is_editable = true;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        dataGridView.RowCount = dataGridView.RowCount - 1;
                        if (GeneralBindingSource.Position != -1)
                            dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                GeneralBindingSource.CurrentItemChanged -= GeneralBindingSource_CurrentItemChanged;
                resettle_aggregate.RefreshEvent -= resettles_aggregate_RefreshEvent;
                GeneralDataModel.Select().RowChanged -= ResettleProcessListViewport_RowChanged;
                GeneralDataModel.Select().RowDeleted -= ResettleProcessListViewport_RowDeleted;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                GeneralDataModel.EditingNewRecord = false;
            GeneralDataModel.Select().RowChanged -= ResettleProcessListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= ResettleProcessListViewport_RowDeleted;
            Close();
        }

        public override bool HasAssocViewport(ViewportType viewportType)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.ResettlePersonsViewport,
                ViewportType.ResettleFromBuildingsViewport,
                ViewportType.ResettleFromPremisesViewport,
                ViewportType.ResettleToBuildingsViewport,
                ViewportType.ResettleToPremisesViewport
            };
            return reports.Contains(viewportType) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport(ViewportType viewportType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбран процесс переселения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            switch (viewportType)
            {               
                case ViewportType.ResettlePersonsViewport:                   
                    ShowAssocViewport(MenuCallback, viewportType,
                        "id_process = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], CultureInfo.InvariantCulture),
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                        ParentTypeEnum.ResettleProcess);
                    break;
                case ViewportType.ResettleFromBuildingsViewport:
                case ViewportType.ResettleFromPremisesViewport:
                    ShowAssocViewport(viewportType, ResettleEstateObjectWay.From);
                    break;
                case ViewportType.ResettleToBuildingsViewport:
                case ViewportType.ResettleToPremisesViewport:
                    ShowAssocViewport(viewportType, ResettleEstateObjectWay.To);
                    break;
            }
        }

        private void ShowAssocViewport(ViewportType viewportType, ResettleEstateObjectWay way)
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
            var viewport = ViewportFactory.CreateViewport(MenuCallback, viewportType);
            viewport.StaticFilter = "id_process = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], 
                CultureInfo.InvariantCulture);
            viewport.ParentRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row;
            viewport.ParentType = ParentTypeEnum.ResettleProcess;
            switch (viewportType)
            {
                case ViewportType.ResettleFromBuildingsViewport:
                case ViewportType.ResettleToBuildingsViewport:
                    ((ResettleBuildingsViewport)viewport).Way = way;
                    break;
                case ViewportType.ResettleFromPremisesViewport:
                case ViewportType.ResettleToPremisesViewport:
                    ((ResettlePremisesViewport)viewport).Way = way;
                    break;
                default:
                    throw new ViewportException("Неподдерживаемый тип viewport");
            }
            if (((IMenuController) viewport).CanLoadData())
                ((IMenuController) viewport).LoadData();
            MenuCallback.AddViewport(viewport);
        }

        void resettles_aggregate_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
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
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
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
            if (dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                changeSortColumn(SortOrder.Descending);
            else
                changeSortColumn(SortOrder.Ascending);
            dataGridView.Refresh();
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
            dataGridView.Refresh();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_process":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["id_process"];
                    break;
                case "resettle_date":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["resettle_date"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)GeneralBindingSource[e.RowIndex])["resettle_date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "resettle_persons":
                    var row_index = v_resettle_aggregate.Find("id_process", ((DataRowView)GeneralBindingSource[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_resettle_aggregate[row_index])["resettlers"];
                    break;
                case "address_from":
                    row_index = v_resettle_aggregate.Find("id_process", ((DataRowView)GeneralBindingSource[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_resettle_aggregate[row_index])["address_from"];
                    break;
                case "address_to":
                    row_index = v_resettle_aggregate.Find("id_process", ((DataRowView)GeneralBindingSource[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_resettle_aggregate[row_index])["address_to"];
                    break;   
            }
        }

        void ResettleProcessListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
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

        void ResettleProcessListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
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
