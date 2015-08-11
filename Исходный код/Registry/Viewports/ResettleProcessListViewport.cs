using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CustomControls;
using Registry.DataModels;
using System.Data;
using Registry.Entities;
using System.Drawing;
using Registry.CalcDataModels;
using Security;
using System.Globalization;
using Registry.SearchForms;

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
        ResettleProcessesDataModel resettle_processes = null;
        DocumentsResidenceDataModel documents_residence = null;
        CalcDataModelResettleAggregated resettle_aggregate = null;
        #endregion Models

        #region Views
        BindingSource v_resettle_processes = null;
        BindingSource v_documents_residence = null;
        BindingSource v_resettle_aggregate = null;
        #endregion Views

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false; 
        private SearchForm spExtendedSearchForm = null;
        private SearchForm spSimpleSearchForm = null;

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
            this.DynamicFilter = resettleProcessListViewport.DynamicFilter;
            this.StaticFilter = resettleProcessListViewport.StaticFilter;
            this.ParentRow = resettleProcessListViewport.ParentRow;
            this.ParentType = resettleProcessListViewport.ParentType;
        }

        private void LocateResettleProcessBy(int id)
        {
            int Position = v_resettle_processes.Find("id_process", id);
            is_editable = false;
            if (Position > 0)
                v_resettle_processes.Position = Position;
            is_editable = true;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
                this.Text = "Новая исковая работа";
            else
                if (v_resettle_processes.Position != -1)
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Процесс переселения №{0}",
                            ((DataRowView)v_resettle_processes[v_resettle_processes.Position])["id_process"]);
                else
                        this.Text = "Процессы переселения отсутствуют";
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
            DataRowView row = (v_resettle_processes.Position >= 0) ? (DataRowView)v_resettle_processes[v_resettle_processes.Position] : null;
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
            if ((!this.ContainsFocus) || (dataGridView.Focused))
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
            ResettleProcess resettle_process = new ResettleProcess();
            DataRowView row = (DataRowView)v_resettle_processes[v_resettle_processes.Position];
            resettle_process.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            resettle_process.Debts = ViewportHelper.ValueOrNull<decimal>(row, "debts");
            resettle_process.ResettleDate = ViewportHelper.ValueOrNull<DateTime>(row, "resettle_date");
            resettle_process.IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence");
            resettle_process.Description = ViewportHelper.ValueOrNull(row, "description");
            return resettle_process;
        }

        private ResettleProcess ResettleProcessViewport()
        {
            ResettleProcess resettle_process = new ResettleProcess();
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            dataGridView.AutoGenerateColumns = false;
            resettle_processes = ResettleProcessesDataModel.GetInstance();
            documents_residence = DocumentsResidenceDataModel.GetInstance();
            resettle_aggregate = CalcDataModelResettleAggregated.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            resettle_processes.Select();
            documents_residence.Select();

            DataSet ds = DataSetManager.DataSet;

            v_documents_residence = new BindingSource();
            v_documents_residence.DataMember = "documents_residence";
            v_documents_residence.DataSource = ds;

            v_resettle_processes = new BindingSource();
            v_resettle_processes.CurrentItemChanged += new EventHandler(v_resettle_processes_CurrentItemChanged);
            v_resettle_processes.DataMember = "resettle_processes";
            v_resettle_processes.DataSource = ds;
            v_resettle_processes.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_resettle_processes.Filter += " AND ";
            v_resettle_processes.Filter += DynamicFilter;

            v_resettle_aggregate = new BindingSource();
            v_resettle_aggregate.DataSource = resettle_aggregate.Select();

            DataBind();

            resettle_processes.Select().RowChanged += new DataRowChangeEventHandler(ResettleProcessListViewport_RowChanged);
            resettle_processes.Select().RowDeleted += new DataRowChangeEventHandler(ResettleProcessListViewport_RowDeleted);

            dataGridView.RowCount = v_resettle_processes.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridView);
            resettle_aggregate.RefreshEvent += new EventHandler<EventArgs>(resettles_aggregate_RefreshEvent);
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
            ResettleProcess resettleProcess = ResettleProcessFromView();
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
            if (!String.IsNullOrEmpty(DynamicFilter))
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
            string Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
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
            ResettleProcess resettleProcess = ResettleProcessViewport();
            if (!ValidateResettleProcess(resettleProcess))
                return;
            string Filter = "";
            if (!String.IsNullOrEmpty(v_resettle_processes.Filter))
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
                    int id_process = ResettleProcessesDataModel.Insert(resettleProcess);
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
                    Filter += String.Format(CultureInfo.CurrentCulture, "(id_process = {0})", resettleProcess.IdProcess);
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
                    DataRowView row = ((DataRowView)v_resettle_processes[v_resettle_processes.Position]);
                    is_editable = false;
                    Filter += String.Format(CultureInfo.CurrentCulture, "(id_process = {0})", resettleProcess.IdProcess);
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
            ResettleProcessListViewport viewport = new ResettleProcessListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_resettle_processes.Count > 0)
                viewport.LocateResettleProcessBy((((DataRowView)v_resettle_processes[v_resettle_processes.Position])["id_process"] as Int32?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                resettle_processes.Select().RowChanged -= new DataRowChangeEventHandler(ResettleProcessListViewport_RowChanged);
                resettle_processes.Select().RowDeleted -= new DataRowChangeEventHandler(ResettleProcessListViewport_RowDeleted);
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                resettle_processes.EditingNewRecord = false;
            resettle_processes.Select().RowChanged -= new DataRowChangeEventHandler(ResettleProcessListViewport_RowChanged);
            resettle_processes.Select().RowDeleted -= new DataRowChangeEventHandler(ResettleProcessListViewport_RowDeleted);
            base.Close();
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
            Viewport viewport = ViewportFactory.CreateViewport(MenuCallback, viewportType);
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
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_process":
                    e.Value = ((DataRowView)v_resettle_processes[e.RowIndex])["id_process"];
                    break;
                case "resettle_date":
                    e.Value = ((DataRowView)v_resettle_processes[e.RowIndex])["resettle_date"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)v_resettle_processes[e.RowIndex])["resettle_date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "resettle_persons":
                    int row_index = v_resettle_aggregate.Find("id_process", ((DataRowView)v_resettle_processes[e.RowIndex])["id_process"]);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResettleProcessListViewport));
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resettle_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resettle_persons = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address_from = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address_to = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.numericUpDownDebts = new System.Windows.Forms.NumericUpDown();
            this.dateTimePickerResettleDate = new System.Windows.Forms.DateTimePicker();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.comboBoxDocumentResidence = new System.Windows.Forms.ComboBox();
            this.label35 = new System.Windows.Forms.Label();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.groupBox15.SuspendLayout();
            this.groupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDebts)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.dataGridView, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.groupBox15, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.groupBox14, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(708, 336);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_process,
            this.resettle_date,
            this.resettle_persons,
            this.address_from,
            this.address_to});
            this.tableLayoutPanel6.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(3, 119);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(702, 214);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // id_process
            // 
            this.id_process.HeaderText = "№";
            this.id_process.MinimumWidth = 100;
            this.id_process.Name = "id_process";
            this.id_process.ReadOnly = true;
            // 
            // resettle_date
            // 
            this.resettle_date.HeaderText = "Дата переселения";
            this.resettle_date.MinimumWidth = 150;
            this.resettle_date.Name = "resettle_date";
            this.resettle_date.ReadOnly = true;
            this.resettle_date.Width = 150;
            // 
            // resettle_persons
            // 
            this.resettle_persons.HeaderText = "Участники";
            this.resettle_persons.MinimumWidth = 250;
            this.resettle_persons.Name = "resettle_persons";
            this.resettle_persons.ReadOnly = true;
            this.resettle_persons.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.resettle_persons.Width = 250;
            // 
            // address_from
            // 
            this.address_from.HeaderText = "Адрес (откуда)";
            this.address_from.MinimumWidth = 500;
            this.address_from.Name = "address_from";
            this.address_from.ReadOnly = true;
            this.address_from.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.address_from.Width = 500;
            // 
            // address_to
            // 
            this.address_to.HeaderText = "Адрес (куда)";
            this.address_to.MinimumWidth = 500;
            this.address_to.Name = "address_to";
            this.address_to.ReadOnly = true;
            this.address_to.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.address_to.Width = 500;
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.textBoxDescription);
            this.groupBox15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox15.Location = new System.Drawing.Point(357, 3);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(348, 110);
            this.groupBox15.TabIndex = 2;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(3, 17);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(342, 90);
            this.textBoxDescription.TabIndex = 0;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            this.textBoxDescription.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.numericUpDownDebts);
            this.groupBox14.Controls.Add(this.dateTimePickerResettleDate);
            this.groupBox14.Controls.Add(this.label37);
            this.groupBox14.Controls.Add(this.label36);
            this.groupBox14.Controls.Add(this.comboBoxDocumentResidence);
            this.groupBox14.Controls.Add(this.label35);
            this.groupBox14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox14.Location = new System.Drawing.Point(3, 3);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(348, 110);
            this.groupBox14.TabIndex = 1;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Общие сведения";
            // 
            // numericUpDownDebts
            // 
            this.numericUpDownDebts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownDebts.DecimalPlaces = 2;
            this.numericUpDownDebts.Location = new System.Drawing.Point(161, 51);
            this.numericUpDownDebts.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownDebts.Name = "numericUpDownDebts";
            this.numericUpDownDebts.Size = new System.Drawing.Size(181, 21);
            this.numericUpDownDebts.TabIndex = 1;
            this.numericUpDownDebts.ThousandsSeparator = true;
            this.numericUpDownDebts.ValueChanged += new System.EventHandler(this.numericUpDownDebts_ValueChanged);
            this.numericUpDownDebts.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // dateTimePickerResettleDate
            // 
            this.dateTimePickerResettleDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerResettleDate.Location = new System.Drawing.Point(161, 80);
            this.dateTimePickerResettleDate.Name = "dateTimePickerResettleDate";
            this.dateTimePickerResettleDate.ShowCheckBox = true;
            this.dateTimePickerResettleDate.Size = new System.Drawing.Size(181, 21);
            this.dateTimePickerResettleDate.TabIndex = 2;
            this.dateTimePickerResettleDate.ValueChanged += new System.EventHandler(this.dateTimePickerResettleDate_ValueChanged);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(14, 82);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(116, 15);
            this.label37.TabIndex = 3;
            this.label37.Text = "Дата переселения";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(14, 54);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(86, 15);
            this.label36.TabIndex = 4;
            this.label36.Text = "Задолжность";
            // 
            // comboBoxDocumentResidence
            // 
            this.comboBoxDocumentResidence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDocumentResidence.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDocumentResidence.FormattingEnabled = true;
            this.comboBoxDocumentResidence.Location = new System.Drawing.Point(161, 22);
            this.comboBoxDocumentResidence.Name = "comboBoxDocumentResidence";
            this.comboBoxDocumentResidence.Size = new System.Drawing.Size(181, 23);
            this.comboBoxDocumentResidence.TabIndex = 0;
            this.comboBoxDocumentResidence.SelectedValueChanged += new System.EventHandler(this.comboBoxDocumentResidence_SelectedValueChanged);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(14, 25);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(146, 15);
            this.label35.TabIndex = 5;
            this.label35.Text = "Основание проживания";
            // 
            // ResettleProcessListViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(640, 320);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(714, 342);
            this.Controls.Add(this.tableLayoutPanel6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ResettleProcessListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Процесс переселения №{0}";
            this.tableLayoutPanel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDebts)).EndInit();
            this.ResumeLayout(false);

        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
