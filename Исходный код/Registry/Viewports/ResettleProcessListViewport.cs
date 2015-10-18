using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.CalcDataModels;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class ResettleProcessListViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel6;
        private GroupBox groupBox15;
        private TextBox textBoxDescription;
        private GroupBox groupBox14;
        private NumericUpDown numericUpDownDebts;
        private DateTimePicker dateTimePickerResettleDate;
        private Label label37;
        private Label label36;
        private ComboBox comboBoxDocumentResidence;
        private Label label35;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewTextBoxColumn resettle_date;
        private DataGridViewTextBoxColumn resettle_persons;
        private DataGridViewTextBoxColumn address_from;
        private DataGridViewTextBoxColumn address_to;
        #endregion Components

        #region Models
        ResettleProcessesDataModel resettle_processes;
        DocumentsResidenceDataModel documents_residence;
        CalcDataModelResettleAggregated resettle_aggregate;
        #endregion Models

        #region Views
        BindingSource v_resettle_processes;
        BindingSource v_documents_residence;
        BindingSource v_resettle_aggregate;
        #endregion Views

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable; 
        private SearchForm spExtendedSearchForm;
        private SearchForm spSimpleSearchForm;

        private ResettleProcessListViewport()
            : this(null)
        {
        }

        public ResettleProcessListViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public ResettleProcessListViewport(ResettleProcessListViewport resettleProcessListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = resettleProcessListViewport.DynamicFilter;
            StaticFilter = resettleProcessListViewport.StaticFilter;
            ParentRow = resettleProcessListViewport.ParentRow;
            ParentType = resettleProcessListViewport.ParentType;
        }

        private void LocateResettleProcessBy(int id)
        {
            var Position = v_resettle_processes.Find("id_process", id);
            is_editable = false;
            if (Position > 0)
                v_resettle_processes.Position = Position;
            is_editable = true;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
                Text = "Новая исковая работа";
            else
                if (v_resettle_processes.Position != -1)
                        Text = string.Format(CultureInfo.InvariantCulture, "Процесс переселения №{0}",
                            ((DataRowView)v_resettle_processes[v_resettle_processes.Position])["id_process"]);
                else
                        Text = "Процессы переселения отсутствуют";
        }

        private void DataBind()
        {
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", v_resettle_processes, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerResettleDate.DataBindings.Clear();
            dateTimePickerResettleDate.DataBindings.Add("Value", v_resettle_processes, "resettle_date", true, DataSourceUpdateMode.Never, null);
            numericUpDownDebts.DataBindings.Clear();
            numericUpDownDebts.DataBindings.Add("Value", v_resettle_processes, "debts", true, DataSourceUpdateMode.Never, 0);
            comboBoxDocumentResidence.DataSource = v_documents_residence;
            comboBoxDocumentResidence.ValueMember = "id_document_residence";
            comboBoxDocumentResidence.DisplayMember = "document_residence";
            comboBoxDocumentResidence.DataBindings.Clear();
            comboBoxDocumentResidence.DataBindings.Add("SelectedValue", v_resettle_processes, "id_document_residence", true, DataSourceUpdateMode.Never, DBNull.Value);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = (v_resettle_processes.Position >= 0) ? (DataRowView)v_resettle_processes[v_resettle_processes.Position] : null;
            if ((v_resettle_processes.Position >= 0) && (row["resettle_date"] != DBNull.Value))
                dateTimePickerResettleDate.Checked = true;
            else
            {
                dateTimePickerResettleDate.Value = DateTime.Now.Date;
                dateTimePickerResettleDate.Checked = false;
            }
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_resettle_processes.Position != -1) && (ResettleProcessFromView() != ResettleProcessViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    dataGridView.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    dataGridView.Enabled = true;
                }
            }
            if (Selected)
                MenuCallback.EditingStateUpdate();
        }

        bool ChangeViewportStateTo(ViewportState state)
        {
            switch (state)
            {
                case ViewportState.ReadState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            return true;
                        case ViewportState.NewRowState:
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else return false;
                            if (viewportState == ViewportState.ReadState)
                                return true;
                            else
                                return false;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            if (resettle_processes.EditingNewRecord)
                                return false;
                            else
                            {
                                viewportState = ViewportState.NewRowState;
                                return true;
                            }
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.NewRowState);
                            else
                                return false;
                    }
                    break;
                case ViewportState.ModifyRowState: ;
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.ModifyRowState);
                            else
                                return false;
                    }
                    break;
            }
            return false;
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

        private ResettleProcess ResettleProcessFromView()
        {
            var resettle_process = new ResettleProcess();
            var row = (DataRowView)v_resettle_processes[v_resettle_processes.Position];
            resettle_process.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            resettle_process.Debts = ViewportHelper.ValueOrNull<decimal>(row, "debts");
            resettle_process.ResettleDate = ViewportHelper.ValueOrNull<DateTime>(row, "resettle_date");
            resettle_process.IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence");
            resettle_process.Description = ViewportHelper.ValueOrNull(row, "description");
            return resettle_process;
        }

        private ResettleProcess ResettleProcessViewport()
        {
            var resettle_process = new ResettleProcess();
            if ((v_resettle_processes.Position == -1) || ((DataRowView)v_resettle_processes[v_resettle_processes.Position])["id_process"] is DBNull)
                resettle_process.IdProcess = null;
            else
                resettle_process.IdProcess = 
                    Convert.ToInt32(((DataRowView)v_resettle_processes[v_resettle_processes.Position])["id_process"], CultureInfo.InvariantCulture);
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

        public override int GetRecordCount()
        {
            return v_resettle_processes.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_resettle_processes.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_resettle_processes.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_resettle_processes.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_resettle_processes.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_resettle_processes.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_resettle_processes.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_resettle_processes.Position > -1) && (v_resettle_processes.Position < (v_resettle_processes.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_resettle_processes.Position > -1) && (v_resettle_processes.Position < (v_resettle_processes.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            DockAreas = DockAreas.Document;
            dataGridView.AutoGenerateColumns = false;
            resettle_processes = ResettleProcessesDataModel.GetInstance();
            documents_residence = DocumentsResidenceDataModel.GetInstance();
            resettle_aggregate = CalcDataModelResettleAggregated.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            resettle_processes.Select();
            documents_residence.Select();

            var ds = DataSetManager.DataSet;

            v_documents_residence = new BindingSource();
            v_documents_residence.DataMember = "documents_residence";
            v_documents_residence.DataSource = ds;

            v_resettle_processes = new BindingSource();
            v_resettle_processes.CurrentItemChanged += v_resettle_processes_CurrentItemChanged;
            v_resettle_processes.DataMember = "resettle_processes";
            v_resettle_processes.DataSource = ds;
            v_resettle_processes.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_resettle_processes.Filter += " AND ";
            v_resettle_processes.Filter += DynamicFilter;

            v_resettle_aggregate = new BindingSource();
            v_resettle_aggregate.DataSource = resettle_aggregate.Select();

            DataBind();

            resettle_processes.Select().RowChanged += ResettleProcessListViewport_RowChanged;
            resettle_processes.Select().RowDeleted += ResettleProcessListViewport_RowDeleted;

            dataGridView.RowCount = v_resettle_processes.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridView);
            resettle_aggregate.RefreshEvent += resettles_aggregate_RefreshEvent;
            is_editable = true;
        }

        public override bool CanInsertRecord()
        {
            return (!resettle_processes.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            v_resettle_processes.AddNew();
            is_editable = true;
            dataGridView.Enabled = false;
            resettle_processes.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (v_resettle_processes.Position != -1) && (!resettle_processes.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var resettleProcess = ResettleProcessFromView();
            dataGridView.RowCount = dataGridView.RowCount + 1;
            v_resettle_processes.AddNew();
            dataGridView.Enabled = false;
            resettle_processes.EditingNewRecord = true;
            ViewportFromResettleProcess(resettleProcess);
            dateTimePickerResettleDate.Checked = (resettleProcess.ResettleDate != null);
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (v_resettle_processes.Position > -1) && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (ResettleProcessesDataModel.Delete((int)((DataRowView)v_resettle_processes.Current)["id_process"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_resettle_processes[v_resettle_processes.Position]).Delete();
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
            v_resettle_processes.Filter = Filter;
            dataGridView.RowCount = v_resettle_processes.Count;
        }

        public override void ClearSearch()
        {
            v_resettle_processes.Filter = StaticFilter;
            dataGridView.RowCount = v_resettle_processes.Count;
            DynamicFilter = "";
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void SaveRecord()
        {
            var resettleProcess = ResettleProcessViewport();
            if (!ValidateResettleProcess(resettleProcess))
                return;
            var Filter = "";
            if (!string.IsNullOrEmpty(v_resettle_processes.Filter))
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
                    var id_process = ResettleProcessesDataModel.Insert(resettleProcess);
                    if (id_process == -1)
                    {
                        resettle_processes.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    resettleProcess.IdProcess = id_process;
                    is_editable = false;
                    if (v_resettle_processes.Position == -1)
                        newRow = (DataRowView)v_resettle_processes.AddNew();
                    else
                        newRow = ((DataRowView)v_resettle_processes[v_resettle_processes.Position]);
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", resettleProcess.IdProcess);
                    v_resettle_processes.Filter += Filter;
                    FillRowFromResettleProcess(resettleProcess, newRow);
                    resettle_processes.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (resettleProcess.IdProcess == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись о процессе переселения без внутреннего номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (ResettleProcessesDataModel.Update(resettleProcess) == -1)
                        return;
                    var row = ((DataRowView)v_resettle_processes[v_resettle_processes.Position]);
                    is_editable = false;
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", resettleProcess.IdProcess);
                    v_resettle_processes.Filter += Filter;
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
                    resettle_processes.EditingNewRecord = false;
                    if (v_resettle_processes.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)v_resettle_processes[v_resettle_processes.Position]).Delete();
                        dataGridView.RowCount = dataGridView.RowCount - 1;
                        if (v_resettle_processes.Position != -1)
                            dataGridView.Rows[v_resettle_processes.Position].Selected = true;
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

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new ResettleProcessListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_resettle_processes.Count > 0)
                viewport.LocateResettleProcessBy((((DataRowView)v_resettle_processes[v_resettle_processes.Position])["id_process"] as int?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                resettle_processes.Select().RowChanged -= ResettleProcessListViewport_RowChanged;
                resettle_processes.Select().RowDeleted -= ResettleProcessListViewport_RowDeleted;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                resettle_processes.EditingNewRecord = false;
            resettle_processes.Select().RowChanged -= ResettleProcessListViewport_RowChanged;
            resettle_processes.Select().RowDeleted -= ResettleProcessListViewport_RowDeleted;
            Close();
        }

        public override bool HasAssocResettlePersons()
        {
            return (v_resettle_processes.Position > -1);
        }

        public override bool HasAssocResettleFromObjects()
        {
            return (v_resettle_processes.Position > -1);
        }

        public override bool HasAssocResettleToObjects()
        {
            return (v_resettle_processes.Position > -1);
        }

        public override void ShowResettlePersons()
        {
            ShowAssocViewport(ViewportType.ResettlePersonsViewport);
        }

        public override void ShowResettleFromBuildings()
        {
            ShowAssocViewport(ViewportType.ResettleFromBuildingsViewport, ResettleEstateObjectWay.From);
        }

        public override void ShowResettleToBuildings()
        {
            ShowAssocViewport(ViewportType.ResettleToBuildingsViewport, ResettleEstateObjectWay.To);
        }

        public override void ShowResettleFromPremises()
        {
            ShowAssocViewport(ViewportType.ResettleFromPremisesViewport, ResettleEstateObjectWay.From);
        }

        public override void ShowResettleToPremises()
        {
            ShowAssocViewport(ViewportType.ResettleToPremisesViewport, ResettleEstateObjectWay.To);
        }

        private void ShowAssocViewport(ViewportType viewportType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_resettle_processes.Position == -1)
            {
                MessageBox.Show("Не выбран процесс переселения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_process = " + Convert.ToInt32(((DataRowView)v_resettle_processes[v_resettle_processes.Position])["id_process"], CultureInfo.InvariantCulture),
                ((DataRowView)v_resettle_processes[v_resettle_processes.Position]).Row,
                ParentTypeEnum.ResettleProcess);
        }

        private Viewport ShowAssocViewport(ViewportType viewportType, ResettleEstateObjectWay way)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return null;
            if (v_resettle_processes.Position == -1)
            {
                MessageBox.Show("Не выбран процесс переселения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return null;
            }
            if (MenuCallback == null)
                throw new ViewportException("Не заданна ссылка на интерфейс menuCallback");
            var viewport = ViewportFactory.CreateViewport(MenuCallback, viewportType);
            viewport.StaticFilter = "id_process = " + Convert.ToInt32(((DataRowView)v_resettle_processes[v_resettle_processes.Position])["id_process"], 
                CultureInfo.InvariantCulture);
            viewport.ParentRow = ((DataRowView)v_resettle_processes[v_resettle_processes.Position]).Row;
            viewport.ParentType = ParentTypeEnum.ResettleProcess;
            if (viewportType == ViewportType.ResettleFromBuildingsViewport || viewportType == ViewportType.ResettleToBuildingsViewport)
                ((ResettleBuildingsViewport)viewport).Way = way;
            else
            if (viewportType == ViewportType.ResettleFromPremisesViewport || viewportType == ViewportType.ResettleToPremisesViewport)
                ((ResettlePremisesViewport)viewport).Way = way;
            else
                throw new ViewportException("Неподдерживаемый тип viewport");
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            MenuCallback.AddViewport(viewport);
            return viewport;
        }

        void resettles_aggregate_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        private void comboBoxDocumentResidence_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void numericUpDownDebts_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void dateTimePickerResettleDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void v_resettle_processes_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            if (v_resettle_processes.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (v_resettle_processes.Position >= dataGridView.RowCount)
                {
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
                }
                else
                    if (dataGridView.Rows[v_resettle_processes.Position].Selected != true)
                    {
                        dataGridView.Rows[v_resettle_processes.Position].Selected = true;
                        dataGridView.CurrentCell = dataGridView.Rows[v_resettle_processes.Position].Cells[0];
                    }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            if (v_resettle_processes.Position == -1)
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
                v_resettle_processes.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
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
                v_resettle_processes.Position = dataGridView.SelectedRows[0].Index;
            else
                v_resettle_processes.Position = -1;
            dataGridView.Refresh();
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_resettle_processes.Count <= e.RowIndex) return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_process":
                    e.Value = ((DataRowView)v_resettle_processes[e.RowIndex])["id_process"];
                    break;
                case "resettle_date":
                    e.Value = ((DataRowView)v_resettle_processes[e.RowIndex])["resettle_date"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)v_resettle_processes[e.RowIndex])["resettle_date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "resettle_persons":
                    var row_index = v_resettle_aggregate.Find("id_process", ((DataRowView)v_resettle_processes[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_resettle_aggregate[row_index])["resettlers"];
                    break;
                case "address_from":
                    row_index = v_resettle_aggregate.Find("id_process", ((DataRowView)v_resettle_processes[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_resettle_aggregate[row_index])["address_from"];
                    break;
                case "address_to":
                    row_index = v_resettle_aggregate.Find("id_process", ((DataRowView)v_resettle_processes[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_resettle_aggregate[row_index])["address_to"];
                    break;   
            }
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        void ResettleProcessListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                dataGridView.RowCount = v_resettle_processes.Count;
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
            dataGridView.RowCount = v_resettle_processes.Count;
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

        private void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(ResettleProcessListViewport));
            tableLayoutPanel6 = new TableLayoutPanel();
            dataGridView = new DataGridView();
            id_process = new DataGridViewTextBoxColumn();
            resettle_date = new DataGridViewTextBoxColumn();
            resettle_persons = new DataGridViewTextBoxColumn();
            address_from = new DataGridViewTextBoxColumn();
            address_to = new DataGridViewTextBoxColumn();
            groupBox15 = new GroupBox();
            textBoxDescription = new TextBox();
            groupBox14 = new GroupBox();
            numericUpDownDebts = new NumericUpDown();
            dateTimePickerResettleDate = new DateTimePicker();
            label37 = new Label();
            label36 = new Label();
            comboBoxDocumentResidence = new ComboBox();
            label35 = new Label();
            tableLayoutPanel6.SuspendLayout();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            groupBox15.SuspendLayout();
            groupBox14.SuspendLayout();
            ((ISupportInitialize)(numericUpDownDebts)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Controls.Add(dataGridView, 0, 1);
            tableLayoutPanel6.Controls.Add(groupBox15, 1, 0);
            tableLayoutPanel6.Controls.Add(groupBox14, 0, 0);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Absolute, 116F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel6.Size = new Size(708, 336);
            tableLayoutPanel6.TabIndex = 0;
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_process, resettle_date, resettle_persons, address_from, address_to);
            tableLayoutPanel6.SetColumnSpan(dataGridView, 2);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView.Location = new Point(3, 119);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(702, 214);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridView.DataError += dataGridView_DataError;
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
            // 
            // id_process
            // 
            id_process.HeaderText = "№";
            id_process.MinimumWidth = 100;
            id_process.Name = "id_process";
            id_process.ReadOnly = true;
            // 
            // resettle_date
            // 
            resettle_date.HeaderText = "Дата переселения";
            resettle_date.MinimumWidth = 150;
            resettle_date.Name = "resettle_date";
            resettle_date.ReadOnly = true;
            resettle_date.Width = 150;
            // 
            // resettle_persons
            // 
            resettle_persons.HeaderText = "Участники";
            resettle_persons.MinimumWidth = 250;
            resettle_persons.Name = "resettle_persons";
            resettle_persons.ReadOnly = true;
            resettle_persons.SortMode = DataGridViewColumnSortMode.NotSortable;
            resettle_persons.Width = 250;
            // 
            // address_from
            // 
            address_from.HeaderText = "Адрес (откуда)";
            address_from.MinimumWidth = 500;
            address_from.Name = "address_from";
            address_from.ReadOnly = true;
            address_from.SortMode = DataGridViewColumnSortMode.NotSortable;
            address_from.Width = 500;
            // 
            // address_to
            // 
            address_to.HeaderText = "Адрес (куда)";
            address_to.MinimumWidth = 500;
            address_to.Name = "address_to";
            address_to.ReadOnly = true;
            address_to.SortMode = DataGridViewColumnSortMode.NotSortable;
            address_to.Width = 500;
            // 
            // groupBox15
            // 
            groupBox15.Controls.Add(textBoxDescription);
            groupBox15.Dock = DockStyle.Fill;
            groupBox15.Location = new Point(357, 3);
            groupBox15.Name = "groupBox15";
            groupBox15.Size = new Size(348, 110);
            groupBox15.TabIndex = 2;
            groupBox15.TabStop = false;
            groupBox15.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.Location = new Point(3, 17);
            textBoxDescription.MaxLength = 4000;
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(342, 90);
            textBoxDescription.TabIndex = 0;
            textBoxDescription.TextChanged += textBoxDescription_TextChanged;
            textBoxDescription.Enter += selectAll_Enter;
            // 
            // groupBox14
            // 
            groupBox14.Controls.Add(numericUpDownDebts);
            groupBox14.Controls.Add(dateTimePickerResettleDate);
            groupBox14.Controls.Add(label37);
            groupBox14.Controls.Add(label36);
            groupBox14.Controls.Add(comboBoxDocumentResidence);
            groupBox14.Controls.Add(label35);
            groupBox14.Dock = DockStyle.Fill;
            groupBox14.Location = new Point(3, 3);
            groupBox14.Name = "groupBox14";
            groupBox14.Size = new Size(348, 110);
            groupBox14.TabIndex = 1;
            groupBox14.TabStop = false;
            groupBox14.Text = "Общие сведения";
            // 
            // numericUpDownDebts
            // 
            numericUpDownDebts.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                        | AnchorStyles.Right;
            numericUpDownDebts.DecimalPlaces = 2;
            numericUpDownDebts.Location = new Point(161, 51);
            numericUpDownDebts.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownDebts.Name = "numericUpDownDebts";
            numericUpDownDebts.Size = new Size(181, 21);
            numericUpDownDebts.TabIndex = 1;
            numericUpDownDebts.ThousandsSeparator = true;
            numericUpDownDebts.ValueChanged += numericUpDownDebts_ValueChanged;
            numericUpDownDebts.Enter += selectAll_Enter;
            // 
            // dateTimePickerResettleDate
            // 
            dateTimePickerResettleDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            dateTimePickerResettleDate.Location = new Point(161, 80);
            dateTimePickerResettleDate.Name = "dateTimePickerResettleDate";
            dateTimePickerResettleDate.ShowCheckBox = true;
            dateTimePickerResettleDate.Size = new Size(181, 21);
            dateTimePickerResettleDate.TabIndex = 2;
            dateTimePickerResettleDate.ValueChanged += dateTimePickerResettleDate_ValueChanged;
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Location = new Point(14, 82);
            label37.Name = "label37";
            label37.Size = new Size(116, 15);
            label37.TabIndex = 3;
            label37.Text = "Дата переселения";
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.Location = new Point(14, 54);
            label36.Name = "label36";
            label36.Size = new Size(86, 15);
            label36.TabIndex = 4;
            label36.Text = "Задолжность";
            // 
            // comboBoxDocumentResidence
            // 
            comboBoxDocumentResidence.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                               | AnchorStyles.Right;
            comboBoxDocumentResidence.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxDocumentResidence.FormattingEnabled = true;
            comboBoxDocumentResidence.Location = new Point(161, 22);
            comboBoxDocumentResidence.Name = "comboBoxDocumentResidence";
            comboBoxDocumentResidence.Size = new Size(181, 23);
            comboBoxDocumentResidence.TabIndex = 0;
            comboBoxDocumentResidence.SelectedValueChanged += comboBoxDocumentResidence_SelectedValueChanged;
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Location = new Point(14, 25);
            label35.Name = "label35";
            label35.Size = new Size(146, 15);
            label35.TabIndex = 5;
            label35.Text = "Основание проживания";
            // 
            // ResettleProcessListViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(640, 320);
            BackColor = Color.White;
            ClientSize = new Size(714, 342);
            Controls.Add(tableLayoutPanel6);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "ResettleProcessListViewport";
            Padding = new Padding(3);
            Text = "Процесс переселения №{0}";
            tableLayoutPanel6.ResumeLayout(false);
            ((ISupportInitialize)(dataGridView)).EndInit();
            groupBox15.ResumeLayout(false);
            groupBox15.PerformLayout();
            groupBox14.ResumeLayout(false);
            groupBox14.PerformLayout();
            ((ISupportInitialize)(numericUpDownDebts)).EndInit();
            ResumeLayout(false);

        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
