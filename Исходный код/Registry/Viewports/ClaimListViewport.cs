using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ClaimListViewport : FormWithGridViewport
    {
        private ClaimListViewport()
            : this(null)
        {
        }

        public ClaimListViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridViewClaims;
        }

        public ClaimListViewport(ClaimListViewport claimListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = claimListViewport.DynamicFilter;
            StaticFilter = claimListViewport.StaticFilter;
            ParentRow = claimListViewport.ParentRow;
            ParentType = claimListViewport.ParentType;
        }

        private void LocateClaimBy(int id)
        {
            var position = GeneralBindingSource.Find("id_claim", id);
            is_editable = false;
            if (position > 0)
                GeneralBindingSource.Position = position;
            is_editable = true;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                {
                    Text = string.Format(CultureInfo.InvariantCulture, "Новая исковая работа найма №{0}", ParentRow["id_process"]);
                }
                else
                    Text = @"Новая исковая работа";
            }
            else
                if (GeneralBindingSource.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                        Text = string.Format(CultureInfo.InvariantCulture, "Исковая работа №{0} найма №{1}",
                            ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"], ParentRow["id_process"]);
                    else
                        Text = string.Format(CultureInfo.InvariantCulture, "Исковая работа №{0}", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"]);
                }
                else
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                        Text = string.Format(CultureInfo.InvariantCulture, "Исковые работы в найме №{0} отсутствуют", ParentRow["id_process"]);
                    else
                        Text = @"Исковые работы отсутствуют";
                }
        }

        private void DataBind()
        {
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDateOfTransfer.DataBindings.Clear();
            dateTimePickerDateOfTransfer.DataBindings.Add("Value", GeneralBindingSource, "date_of_transfer", true, DataSourceUpdateMode.Never, null);
            dateTimePickerAtDate.DataBindings.Clear();
            dateTimePickerAtDate.DataBindings.Add("Value", GeneralBindingSource, "at_date", true, DataSourceUpdateMode.Never, null);
            dateTimePickerStartDeptPeriod.DataBindings.Clear();
            dateTimePickerStartDeptPeriod.DataBindings.Add("Value", GeneralBindingSource, "start_dept_period", true, DataSourceUpdateMode.Never, null);
            dateTimePickerEndDeptPeriod.DataBindings.Clear();
            dateTimePickerEndDeptPeriod.DataBindings.Add("Value", GeneralBindingSource, "end_dept_period", true, DataSourceUpdateMode.Never, null);
            numericUpDownProcessID.DataBindings.Clear();
            numericUpDownProcessID.DataBindings.Add("Value", GeneralBindingSource, "id_process", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfDebtFine.DataBindings.Clear();
            numericUpDownAmountOfDebtFine.DataBindings.Add("Value", GeneralBindingSource, "amount_of_debt_fine", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfDebtRent.DataBindings.Clear();
            numericUpDownAmountOfDebtRent.DataBindings.Add("Value", GeneralBindingSource, "amount_of_debt_rent", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfFine.DataBindings.Clear();
            numericUpDownAmountOfFine.DataBindings.Add("Value", GeneralBindingSource, "amount_of_fine", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfRent.DataBindings.Clear();
            numericUpDownAmountOfRent.DataBindings.Add("Value", GeneralBindingSource, "amount_of_rent", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfFineRecover.DataBindings.Clear();
            numericUpDownAmountOfFineRecover.DataBindings.Add("Value", GeneralBindingSource, "amount_of_fine_recover", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfRentRecover.DataBindings.Clear();
            numericUpDownAmountOfRentRecover.DataBindings.Add("Value", GeneralBindingSource, "amount_of_rent_recover", true, DataSourceUpdateMode.Never, 0);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = (GeneralBindingSource.Position >= 0) ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["date_of_transfer"] != DBNull.Value)))
                dateTimePickerDateOfTransfer.Checked = true;
            else
            {
                dateTimePickerDateOfTransfer.Value = DateTime.Now.Date;
                dateTimePickerDateOfTransfer.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["at_date"] != DBNull.Value)))
                dateTimePickerAtDate.Checked = true;
            else
            {
                dateTimePickerAtDate.Value = DateTime.Now.Date;
                dateTimePickerAtDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["start_dept_period"] != DBNull.Value)))
                dateTimePickerStartDeptPeriod.Checked = true;
            else
            {
                dateTimePickerStartDeptPeriod.Value = DateTime.Now.Date;
                dateTimePickerStartDeptPeriod.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["end_dept_period"] != DBNull.Value)))
                dateTimePickerEndDeptPeriod.Checked = true;
            else
            {
                dateTimePickerEndDeptPeriod.Value = DateTime.Now.Date;
                dateTimePickerEndDeptPeriod.Checked = false;
            }
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
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
                            var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
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
                                    return false;
                            }
                            return viewportState == ViewportState.ReadState;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            if (GeneralDataModel.EditingNewRecord)
                                return false;
                            viewportState = ViewportState.NewRowState;
                            return true;
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
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
                                    return false;
                            }
                            return viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.NewRowState);
                    }
                    break;
                case ViewportState.ModifyRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
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
                                    return false;
                            }
                            return viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.ModifyRowState);
                    }
                    break;
            }
            return false;
        }

        private bool ValidateClaim(Claim claim)
        {
            if (claim.IdProcess == null)
            {
                MessageBox.Show(@"Необходимо задать внутренний номер процесса найм. Если вы видите это сообщение, обратитесь к системному администратору",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            var tenancyRows = from tenancyRow in DataModel.GetInstance(DataModelType.TenancyProcessesDataModel).FilterDeletedRows()
                               where tenancyRow.Field<int>("id_process") == claim.IdProcess
                               select tenancyRow;
            if (tenancyRows.Any()) return true;
            MessageBox.Show(@"Процесса найма с указаннным внутренним номером не существует",
                @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            numericUpDownProcessID.Focus();
            return false;
        }

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            var claim = new Claim
            {
                IdClaim = ViewportHelper.ValueOrNull<int>(row, "id_claim"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                AmountOfDebtRent = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_debt_rent"),
                AmountOfDebtFine = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_debt_fine"),
                AmountOfRent = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_rent"),
                AmountOfFine = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_fine"),
                AmountOfRentRecover = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_rent_recover"),
                AmountOfFineRecover = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_fine_recover"),
                DateOfTransfer = ViewportHelper.ValueOrNull<DateTime>(row, "date_of_transfer"),
                AtDate = ViewportHelper.ValueOrNull<DateTime>(row, "at_date"),
                StartDeptPeriod = ViewportHelper.ValueOrNull<DateTime>(row, "start_dept_period"),
                EndDeptPeriod = ViewportHelper.ValueOrNull<DateTime>(row, "end_dept_period"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
            return claim;
        }

        protected override Entity EntityFromViewport()
        {
            var claim = new Claim();
            if ((GeneralBindingSource.Position == -1) || ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"] is DBNull)
                claim.IdClaim = null;
            else
                claim.IdClaim = Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"], CultureInfo.InvariantCulture);
            claim.IdProcess = Convert.ToInt32(numericUpDownProcessID.Value);
            claim.AmountOfDebtRent = numericUpDownAmountOfDebtRent.Value;
            claim.AmountOfDebtFine = numericUpDownAmountOfDebtFine.Value;
            claim.AmountOfRent = numericUpDownAmountOfRent.Value;
            claim.AmountOfFine = numericUpDownAmountOfFine.Value;
            claim.AmountOfRentRecover = numericUpDownAmountOfRentRecover.Value;
            claim.AmountOfFineRecover = numericUpDownAmountOfFineRecover.Value;
            claim.DateOfTransfer = ViewportHelper.ValueOrNull(dateTimePickerDateOfTransfer);
            claim.AtDate = ViewportHelper.ValueOrNull(dateTimePickerAtDate);
            claim.StartDeptPeriod = ViewportHelper.ValueOrNull(dateTimePickerStartDeptPeriod);
            claim.EndDeptPeriod = ViewportHelper.ValueOrNull(dateTimePickerEndDeptPeriod);
            claim.Description = ViewportHelper.ValueOrNull(textBoxDescription);
            return claim;
        }

        private void ViewportFromClaim(Claim claim)
        {
            numericUpDownProcessID.Value = ViewportHelper.ValueOrDefault(claim.IdProcess);
            numericUpDownAmountOfDebtFine.Value = ViewportHelper.ValueOrDefault(claim.AmountOfDebtFine);
            numericUpDownAmountOfDebtRent.Value = ViewportHelper.ValueOrDefault(claim.AmountOfDebtRent);
            numericUpDownAmountOfFine.Value = ViewportHelper.ValueOrDefault(claim.AmountOfFine);
            numericUpDownAmountOfRent.Value = ViewportHelper.ValueOrDefault(claim.AmountOfRent);
            numericUpDownAmountOfFineRecover.Value = ViewportHelper.ValueOrDefault(claim.AmountOfFineRecover);
            numericUpDownAmountOfRentRecover.Value = ViewportHelper.ValueOrDefault(claim.AmountOfRentRecover);
            dateTimePickerDateOfTransfer.Value = ViewportHelper.ValueOrDefault(claim.DateOfTransfer);
            dateTimePickerAtDate.Value = ViewportHelper.ValueOrDefault(claim.AtDate);
            dateTimePickerStartDeptPeriod.Value = ViewportHelper.ValueOrDefault(claim.StartDeptPeriod);
            dateTimePickerEndDeptPeriod.Value = ViewportHelper.ValueOrDefault(claim.EndDeptPeriod);
            textBoxDescription.Text = claim.Description;
        }

        private static void FillRowFromClaim(Claim claim, DataRowView row)
        {
            row.BeginEdit();
            row["id_claim"] = ViewportHelper.ValueOrDBNull(claim.IdClaim);
            row["id_process"] = ViewportHelper.ValueOrDBNull(claim.IdProcess);
            row["date_of_transfer"] = ViewportHelper.ValueOrDBNull(claim.DateOfTransfer);
            row["at_date"] = ViewportHelper.ValueOrDBNull(claim.AtDate);
            row["start_dept_period"] = ViewportHelper.ValueOrDBNull(claim.StartDeptPeriod);
            row["end_dept_period"] = ViewportHelper.ValueOrDBNull(claim.EndDeptPeriod);
            row["amount_of_debt_rent"] = ViewportHelper.ValueOrDBNull(claim.AmountOfDebtRent);
            row["amount_of_debt_fine"] = ViewportHelper.ValueOrDBNull(claim.AmountOfDebtFine);
            row["amount_of_rent"] = ViewportHelper.ValueOrDBNull(claim.AmountOfRent);
            row["amount_of_fine"] = ViewportHelper.ValueOrDBNull(claim.AmountOfFine);
            row["amount_of_rent_recover"] = ViewportHelper.ValueOrDBNull(claim.AmountOfRentRecover);
            row["amount_of_fine_recover"] = ViewportHelper.ValueOrDBNull(claim.AmountOfFineRecover);
            row["description"] = ViewportHelper.ValueOrDBNull(claim.Description);
            row.EndEdit();
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            DockAreas = DockAreas.Document;
            dataGridViewClaims.AutoGenerateColumns = false;
            GeneralDataModel = DataModel.GetInstance(DataModelType.ClaimsDataModel);

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataMember = "claims";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;

            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                numericUpDownProcessID.Enabled = false;

            DataBind();

            GeneralDataModel.Select().RowChanged += ClaimListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += ClaimListViewport_RowDeleted;

            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridViewClaims);
            is_editable = true;
            DataChangeHandlersInit();
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridViewClaims.RowCount = dataGridViewClaims.RowCount + 1;
            GeneralBindingSource.AddNew();
            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                numericUpDownProcessID.Value = (int)ParentRow["id_process"];
            is_editable = true;
            dataGridViewClaims.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord) 
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var claim = (Claim)EntityFromView();
            dataGridViewClaims.RowCount = dataGridViewClaims.RowCount + 1;
            GeneralBindingSource.AddNew();
            dataGridViewClaims.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromClaim(claim);
            dateTimePickerDateOfTransfer.Checked = (claim.DateOfTransfer != null);
            dateTimePickerAtDate.Checked = (claim.AtDate != null);
            dateTimePickerStartDeptPeriod.Checked = (claim.StartDeptPeriod != null);
            dateTimePickerEndDeptPeriod.Checked = (claim.EndDeptPeriod != null);
            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                numericUpDownProcessID.Value = (int)ParentRow["id_process"];
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_claim"]) == -1)
                return;
            is_editable = false;
            ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
            is_editable = true;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            MenuCallback.ForceCloseDetachedViewports();
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void SaveRecord()
        {
            var claim = (Claim) EntityFromViewport();
            if (!ValidateClaim(claim))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idClaim = GeneralDataModel.Insert(claim);
                    if (idClaim == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    claim.IdClaim = idClaim;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    FillRowFromClaim(claim, newRow);
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (claim.IdClaim == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о претензионно-исковой работе без внутреннего номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(claim) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    is_editable = false;
                    FillRowFromClaim(claim, row);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            dataGridViewClaims.Enabled = true;
            is_editable = true;
            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
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
                        dataGridViewClaims.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        dataGridViewClaims.RowCount = dataGridViewClaims.RowCount - 1;
                        if (GeneralBindingSource.Position != -1)
                            dataGridViewClaims.Rows[GeneralBindingSource.Position].Selected = true;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    dataGridViewClaims.Enabled = true;
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
            var viewport = new ClaimListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (GeneralBindingSource.Count > 0)
                viewport.LocateClaimBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"] as int?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                GeneralDataModel.Select().RowChanged -= ClaimListViewport_RowChanged;
                GeneralDataModel.Select().RowDeleted -= ClaimListViewport_RowDeleted;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                GeneralDataModel.EditingNewRecord = false;
            GeneralDataModel.Select().RowChanged -= ClaimListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= ClaimListViewport_RowDeleted;
            Close();
        }

        public override bool HasAssocViewport(ViewportType viewportType)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.ClaimStatesViewport
            };
            return reports.Contains(viewportType) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport(ViewportType viewportType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрана протензионно-исковая работа для отображения ее состояний", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_claim = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row, ParentTypeEnum.Claim);
        }

        void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            if (GeneralBindingSource.Position == -1 || dataGridViewClaims.RowCount == 0)
                dataGridViewClaims.ClearSelection();
            else
                if (GeneralBindingSource.Position >= dataGridViewClaims.RowCount)
				{
                    dataGridViewClaims.Rows[dataGridViewClaims.RowCount - 1].Selected = true;
					dataGridViewClaims.CurrentCell = dataGridViewClaims.Rows[dataGridViewClaims.RowCount - 1].
						Cells[1];
				}
            else
                    if (dataGridViewClaims.Rows[GeneralBindingSource.Position].Selected != true)
					{
                        dataGridViewClaims.Rows[GeneralBindingSource.Position].Selected = true;
						dataGridViewClaims.CurrentCell = dataGridViewClaims.Rows[GeneralBindingSource.Position].
							Cells[1];
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
            dataGridViewClaims.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        void dataGridViewClaims_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewClaims.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in dataGridViewClaims.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = dataGridViewClaims.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridViewClaims.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridViewClaims.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection ==
                             SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridViewClaims.Refresh();
        }

        void dataGridViewClaims_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewClaims.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridViewClaims.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        void dataGridViewClaims_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            switch (dataGridViewClaims.Columns[e.ColumnIndex].Name)
            {
                case "id_claim":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["id_claim"];
                    break;
                case "date_of_transfer":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["date_of_transfer"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)GeneralBindingSource[e.RowIndex])["date_of_transfer"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "amount_of_debt_rent":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["amount_of_debt_rent"];
                    break;
                case "amount_of_debt_fine":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["amount_of_debt_fine"];
                    break;
                case "at_date":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["at_date"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)GeneralBindingSource[e.RowIndex])["at_date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "description":
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])["description"];
                    break;
            }
        }

        void dataGridViewClaims_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        void ClaimListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                dataGridViewClaims.RowCount = GeneralBindingSource.Count;
                dataGridViewClaims.Refresh();
                UnbindedCheckBoxesUpdate();
                MenuCallback.ForceCloseDetachedViewports();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        void ClaimListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridViewClaims.Refresh();
            dataGridViewClaims.RowCount = GeneralBindingSource.Count;
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            base.OnVisibleChanged(e);
        }
    }
}
