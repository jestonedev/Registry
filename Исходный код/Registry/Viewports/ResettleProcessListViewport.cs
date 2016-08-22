using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;
using System.Linq;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.Presenters;

namespace Registry.Viewport
{
    internal sealed partial class ResettleProcessListViewport: FormWithGridViewport
    {

        private ResettleProcessListViewport()
            : this(null, null)
        {
        }

        public ResettleProcessListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new ResettleProcessListPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            ViewportHelper.SetDoubleBuffered(DataGridView);
        }

        private void SetViewportCaption()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (ViewportState == ViewportState.NewRowState)
                Text = @"Новая исковая работа";
            else if (row != null)
                Text = string.Format(CultureInfo.InvariantCulture, "Процесс переселения №{0}", row["id_process"]);
            else
                Text = @"Процессы переселения отсутствуют";
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindProperty(textBoxDescription, "Text", bindingSource, "description", "");
            ViewportHelper.BindProperty(textBoxDocNumber, "Text", bindingSource, "doc_number", "");
            ViewportHelper.BindProperty(dateTimePickerResettleDate, "Value", bindingSource, "resettle_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(numericUpDownDebts, "Value", bindingSource, "debts", 0m);
            ViewportHelper.BindSource(comboBoxDocumentResidence, Presenter.ViewModel["documents_residence"].BindingSource, "document_residence",
                Presenter.ViewModel["documents_residence"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxDocumentResidence, "SelectedValue", bindingSource,
                Presenter.ViewModel["documents_residence"].PrimaryKeyFirst, DBNull.Value);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            IsEditable = false;
            if (row != null && (row["resettle_date"] != DBNull.Value))
                dateTimePickerResettleDate.Checked = true;
            else
            {
                dateTimePickerResettleDate.Value = DateTime.Now.Date;
                dateTimePickerResettleDate.Checked = false;
            }
            IsEditable = true;
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
            var row = Presenter.ViewModel["general"].CurrentRow;
            return EntityConverter<ResettleProcess>.FromRow(row);
        }

        protected override Entity EntityFromViewport()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var resettleProcess = new ResettleProcess
            {
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                Debts = numericUpDownDebts.Value,
                DocNumber = ViewportHelper.ValueOrNull(textBoxDocNumber),
                ResettleDate = ViewportHelper.ValueOrNull(dateTimePickerResettleDate),
                IdDocumentResidence = ViewportHelper.ValueOrNull<int>(comboBoxDocumentResidence),
                Description = ViewportHelper.ValueOrNull(textBoxDescription)
            };
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
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;

            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            SetViewportCaption();

            DataBind();

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, 
                "RowChanged", ResettleProcessListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource,
                "RowDeleted", ResettleProcessListViewport_RowDeleted);
            AddEventHandler<EventArgs>(Presenter.ViewModel["resettle_aggregate"].Model, "RefreshEvent", resettles_aggregate_RefreshEvent);

            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());

            DataChangeHandlersInit();
            IsEditable = true;
        }

        public override bool CanInsertRecord()
        {
            return !Presenter.ViewModel["general"].Model.EditingNewRecord && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            DataGridView.RowCount = DataGridView.RowCount + 1;
            DataGridView.Enabled = false;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            IsEditable = true;
        }

        public override bool CanCopyRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) 
                && !Presenter.ViewModel["general"].Model.EditingNewRecord
                && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            var resettleProcess = (ResettleProcess)EntityFromView();
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            DataGridView.RowCount = DataGridView.RowCount + 1;
            DataGridView.Enabled = false;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            ViewportFromResettleProcess(resettleProcess);
            dateTimePickerResettleDate.Checked = resettleProcess.ResettleDate != null;
            IsEditable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                IsEditable = false;
                if (!((ResettleProcessListPresenter)Presenter).DeleteRecord())
                {
                    IsEditable = true;
                    return;
                }
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
            return !string.IsNullOrEmpty(DynamicFilter);
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (Presenter.SimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = Presenter.SimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (Presenter.ExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = Presenter.ExtendedSearchForm.GetFilter();
                    break;
            }
            DataGridView.RowCount = 0;
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
        }

        public override void ClearSearch()
        {
            DataGridView.RowCount = 0;
            Presenter.ViewModel["general"].BindingSource.Filter = StaticFilter;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
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
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    if (!((ResettleProcessListPresenter)Presenter).InsertRecord(resettleProcess))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((ResettleProcessListPresenter)Presenter).UpdateRecord(resettleProcess))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            DataGridView.Enabled = true;
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
                    Presenter.ViewModel["general"].Model.EditingNewRecord = false;
                    var row = Presenter.ViewModel["general"].CurrentRow;
                    if (row != null)
                    {
                        IsEditable = false;
                        row.Delete();
                        DataGridView.RowCount = DataGridView.RowCount - 1;
                        if (Presenter.ViewModel["general"].CurrentRow != null)
                            DataGridView.Rows[Presenter.ViewModel["general"].BindingSource.Position].Selected = true;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            DataGridView.Enabled = true;
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
            return reports.Any(v => v.ToString() == typeof(T).Name) && (Presenter.ViewModel["general"].CurrentRow != null);
        }

        public override bool HasAssocViewport<T>(ResettleEstateObjectWay way)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.ResettlePersonsViewport,              
                ViewportType.ResettleBuildingsViewport,
                ViewportType.ResettlePremisesViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (Presenter.ViewModel["general"].CurrentRow != null);
        }

        public override void ShowAssocViewport<T>(ResettleEstateObjectWay way)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            var viewModel = Presenter.ViewModel["general"];
            if (viewModel.CurrentRow == null)
            {
                MessageBox.Show(@"Не выбран процесс переселения", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MenuCallback == null)
                throw new ViewportException("Не заданна ссылка на интерфейс menuCallback");
            
            
            var viewport = ViewportFactory.CreateViewport<T>(MenuCallback);
            viewport.StaticFilter = viewModel.PrimaryKeyFirst + " = " +
                    Convert.ToInt32(viewModel.CurrentRow[viewModel.PrimaryKeyFirst], CultureInfo.InvariantCulture);
            viewport.ParentRow = viewModel.CurrentRow.Row;
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
            DataGridView.Refresh();
        }

        protected override void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();

            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            if (Presenter.ViewModel["general"].CurrentRow == null || DataGridView.RowCount == 0)
                DataGridView.ClearSelection();
            else if (bindingSource.Position >= DataGridView.RowCount)
            {
                DataGridView.Rows[DataGridView.RowCount - 1].Selected = true;
                DataGridView.CurrentCell = DataGridView.Rows[DataGridView.RowCount - 1].Cells[0];
            }
            else if (DataGridView.Rows[bindingSource.Position].Selected != true)
            {
                DataGridView.Rows[bindingSource.Position].Selected = true;
                DataGridView.CurrentCell = DataGridView.Rows[bindingSource.Position].Cells[0];
            }

            var isEditable = IsEditable;
            UnbindedCheckBoxesUpdate();
            IsEditable = isEditable;

            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in DataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                Presenter.ViewModel["general"].BindingSource.Sort = 
                    DataGridView.Columns[e.ColumnIndex].Name + " " + (way == SortOrder.Ascending ? "ASC" : "DESC");
                DataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(DataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            DataGridView.Refresh();
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (DataGridView.SelectedRows.Count > 0)
                Presenter.ViewModel["general"].BindingSource.Position = DataGridView.SelectedRows[0].Index;
            else
                Presenter.ViewModel["general"].BindingSource.Position = -1;
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (Presenter.ViewModel["general"].BindingSource.Count <= e.RowIndex) return;
            var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[e.RowIndex];
            var resettleAggregateBindingSource = Presenter.ViewModel["resettle_aggregate"].BindingSource;
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_process":
                case "doc_number":
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "resettle_date":
                    e.Value = row["resettle_date"] == DBNull.Value ? "" :
                        ((DateTime)row["resettle_date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "resettle_persons":
                    var rowIndex = resettleAggregateBindingSource.Find("id_process", row["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)resettleAggregateBindingSource[rowIndex])["resettlers"];
                    break;
                case "address_from":
                    rowIndex = resettleAggregateBindingSource.Find("id_process", row["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)resettleAggregateBindingSource[rowIndex])["address_from"];
                    break;
                case "address_to":
                    rowIndex = resettleAggregateBindingSource.Find("id_process", row["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)resettleAggregateBindingSource[rowIndex])["address_to"];
                    break;   
            }
        }

        private void ResettleProcessListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DataGridView.Invalidate();
            UnbindedCheckBoxesUpdate();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void ResettleProcessListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                DataGridView.Invalidate();
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count; 
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
