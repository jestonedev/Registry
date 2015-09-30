using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class ClaimListViewport: Viewport
    {
        #region Components      
        TableLayoutPanel tableLayoutPanel15;
        TableLayoutPanel tableLayoutPanel16;
        Panel panel8;
        Panel panel9;
        GroupBox groupBox34;
        NumericUpDown numericUpDownAmountOfDebtFine;
        NumericUpDown numericUpDownAmountOfDebtRent;
        NumericUpDown numericUpDownAmountOfFine;
        NumericUpDown numericUpDownAmountOfRent;
        NumericUpDown numericUpDownAmountOfFineRecover;
        NumericUpDown numericUpDownAmountOfRentRecover;
        NumericUpDown numericUpDownProcessID;
        Label label89;
        Label label90;
        Label label91;
        Label label92;
        Label label93;
        Label label94;
        Label label95;
        Label label96;
        Label label97;
        Label label98;
        Label label99;
        Label label102;
        Label label103;
        TextBox textBoxDescription;
        DateTimePicker dateTimePickerStartDeptPeriod;
        DateTimePicker dateTimePickerEndDeptPeriod;
        DateTimePicker dateTimePickerAtDate;
        DateTimePicker dateTimePickerDateOfTransfer;
        DataGridView dataGridViewClaims;
        private DataGridViewTextBoxColumn id_claim;
        private DataGridViewTextBoxColumn date_of_transfer;
        private DataGridViewTextBoxColumn amount_of_debt_rent;
        private DataGridViewTextBoxColumn amount_of_debt_fine;
        private DataGridViewTextBoxColumn at_date;
        private DataGridViewTextBoxColumn description;
        #endregion Components

        //Modeles
        ClaimsDataModel claims;

        //Views
        BindingSource v_claims;

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable;

        private ClaimListViewport()
            : this(null)
        {
        }

        public ClaimListViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
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
            var position = v_claims.Find("id_claim", id);
            is_editable = false;
            if (position > 0)
                v_claims.Position = position;
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
                if (v_claims.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                        Text = string.Format(CultureInfo.InvariantCulture, "Исковая работа №{0} найма №{1}",
                            ((DataRowView)v_claims[v_claims.Position])["id_claim"], ParentRow["id_process"]);
                    else
                        Text = string.Format(CultureInfo.InvariantCulture, "Исковая работа №{0}", ((DataRowView)v_claims[v_claims.Position])["id_claim"]);
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
            textBoxDescription.DataBindings.Add("Text", v_claims, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDateOfTransfer.DataBindings.Clear();
            dateTimePickerDateOfTransfer.DataBindings.Add("Value", v_claims, "date_of_transfer", true, DataSourceUpdateMode.Never, null);
            dateTimePickerAtDate.DataBindings.Clear();
            dateTimePickerAtDate.DataBindings.Add("Value", v_claims, "at_date", true, DataSourceUpdateMode.Never, null);
            dateTimePickerStartDeptPeriod.DataBindings.Clear();
            dateTimePickerStartDeptPeriod.DataBindings.Add("Value", v_claims, "start_dept_period", true, DataSourceUpdateMode.Never, null);
            dateTimePickerEndDeptPeriod.DataBindings.Clear();
            dateTimePickerEndDeptPeriod.DataBindings.Add("Value", v_claims, "end_dept_period", true, DataSourceUpdateMode.Never, null);
            numericUpDownProcessID.DataBindings.Clear();
            numericUpDownProcessID.DataBindings.Add("Value", v_claims, "id_process", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfDebtFine.DataBindings.Clear();
            numericUpDownAmountOfDebtFine.DataBindings.Add("Value", v_claims, "amount_of_debt_fine", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfDebtRent.DataBindings.Clear();
            numericUpDownAmountOfDebtRent.DataBindings.Add("Value", v_claims, "amount_of_debt_rent", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfFine.DataBindings.Clear();
            numericUpDownAmountOfFine.DataBindings.Add("Value", v_claims, "amount_of_fine", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfRent.DataBindings.Clear();
            numericUpDownAmountOfRent.DataBindings.Add("Value", v_claims, "amount_of_rent", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfFineRecover.DataBindings.Clear();
            numericUpDownAmountOfFineRecover.DataBindings.Add("Value", v_claims, "amount_of_fine_recover", true, DataSourceUpdateMode.Never, 0);
            numericUpDownAmountOfRentRecover.DataBindings.Clear();
            numericUpDownAmountOfRentRecover.DataBindings.Add("Value", v_claims, "amount_of_rent_recover", true, DataSourceUpdateMode.Never, 0);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = (v_claims.Position >= 0) ? (DataRowView)v_claims[v_claims.Position] : null;
            if (row != null && ((v_claims.Position >= 0) && (row["date_of_transfer"] != DBNull.Value)))
                dateTimePickerDateOfTransfer.Checked = true;
            else
            {
                dateTimePickerDateOfTransfer.Value = DateTime.Now.Date;
                dateTimePickerDateOfTransfer.Checked = false;
            }
            if (row != null && ((v_claims.Position >= 0) && (row["at_date"] != DBNull.Value)))
                dateTimePickerAtDate.Checked = true;
            else
            {
                dateTimePickerAtDate.Value = DateTime.Now.Date;
                dateTimePickerAtDate.Checked = false;
            }
            if (row != null && ((v_claims.Position >= 0) && (row["start_dept_period"] != DBNull.Value)))
                dateTimePickerStartDeptPeriod.Checked = true;
            else
            {
                dateTimePickerStartDeptPeriod.Value = DateTime.Now.Date;
                dateTimePickerStartDeptPeriod.Checked = false;
            }
            if (row != null && ((v_claims.Position >= 0) && (row["end_dept_period"] != DBNull.Value)))
                dateTimePickerEndDeptPeriod.Checked = true;
            else
            {
                dateTimePickerEndDeptPeriod.Value = DateTime.Now.Date;
                dateTimePickerEndDeptPeriod.Checked = false;
            }
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!ContainsFocus) || (dataGridViewClaims.Focused))
                return;
            if ((v_claims.Position != -1) && (ClaimFromView() != ClaimFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    dataGridViewClaims.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    dataGridViewClaims.Enabled = true;
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
                            if (claims.EditingNewRecord)
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
            var tenancyRows = from tenancyRow in DataModelHelper.FilterRows(TenancyProcessesDataModel.GetInstance().Select())
                               where tenancyRow.Field<int>("id_process") == claim.IdProcess
                               select tenancyRow;
            if (tenancyRows.Any()) return true;
            MessageBox.Show(@"Процесса найма с указаннным внутренним номером не существует",
                @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            numericUpDownProcessID.Focus();
            return false;
        }

        private Claim ClaimFromView()
        {
            var claim = new Claim();
            var row = (DataRowView)v_claims[v_claims.Position];
            claim.IdClaim = ViewportHelper.ValueOrNull<int>(row, "id_claim");
            claim.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            claim.AmountOfDebtRent = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_debt_rent");
            claim.AmountOfDebtFine = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_debt_fine");
            claim.AmountOfRent = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_rent");
            claim.AmountOfFine = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_fine");
            claim.AmountOfRentRecover = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_rent_recover");
            claim.AmountOfFineRecover = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_fine_recover");
            claim.DateOfTransfer = ViewportHelper.ValueOrNull<DateTime>(row, "date_of_transfer");
            claim.AtDate = ViewportHelper.ValueOrNull<DateTime>(row, "at_date");
            claim.StartDeptPeriod = ViewportHelper.ValueOrNull<DateTime>(row, "start_dept_period");
            claim.EndDeptPeriod = ViewportHelper.ValueOrNull<DateTime>(row, "end_dept_period");
            claim.Description = ViewportHelper.ValueOrNull(row, "description");
            return claim;
        }

        private Claim ClaimFromViewport()
        {
            var claim = new Claim();
            if ((v_claims.Position == -1) || ((DataRowView)v_claims[v_claims.Position])["id_claim"] is DBNull)
                claim.IdClaim = null;
            else
                claim.IdClaim = Convert.ToInt32(((DataRowView)v_claims[v_claims.Position])["id_claim"], CultureInfo.InvariantCulture);
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

        public override int GetRecordCount()
        {
            return v_claims.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claims.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claims.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claims.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claims.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_claims.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_claims.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_claims.Position > -1) && (v_claims.Position < (v_claims.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_claims.Position > -1) && (v_claims.Position < (v_claims.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            DockAreas = DockAreas.Document;
            dataGridViewClaims.AutoGenerateColumns = false;
            claims = ClaimsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            claims.Select();

            var ds = DataSetManager.DataSet;

            v_claims = new BindingSource();
            v_claims.CurrentItemChanged += v_claims_CurrentItemChanged;
            v_claims.DataMember = "claims";
            v_claims.DataSource = ds;
            v_claims.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_claims.Filter += " AND ";
            v_claims.Filter += DynamicFilter;

            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                numericUpDownProcessID.Enabled = false;

            DataBind();

            claims.Select().RowChanged += ClaimListViewport_RowChanged;
            claims.Select().RowDeleted += ClaimListViewport_RowDeleted;

            dataGridViewClaims.RowCount = v_claims.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridViewClaims);
            is_editable = true;
        }

        public override bool CanInsertRecord()
        {
            return (!claims.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridViewClaims.RowCount = dataGridViewClaims.RowCount + 1;
            v_claims.AddNew();
            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                numericUpDownProcessID.Value = (int)ParentRow["id_process"];
            is_editable = true;
            dataGridViewClaims.Enabled = false;
            claims.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (v_claims.Position != -1) && (!claims.EditingNewRecord) 
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var claim = ClaimFromView();
            dataGridViewClaims.RowCount = dataGridViewClaims.RowCount + 1;
            v_claims.AddNew();
            dataGridViewClaims.Enabled = false;
            claims.EditingNewRecord = true;
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
            return (v_claims.Position > -1) && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            if (ClaimsDataModel.Delete((int)((DataRowView)v_claims.Current)["id_claim"]) == -1)
                return;
            is_editable = false;
            ((DataRowView)v_claims[v_claims.Position]).Delete();
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
            var claim = ClaimFromViewport();
            if (!ValidateClaim(claim))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idClaim = ClaimsDataModel.Insert(claim);
                    if (idClaim == -1)
                    {
                        claims.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    claim.IdClaim = idClaim;
                    is_editable = false;
                    if (v_claims.Position == -1)
                        newRow = (DataRowView)v_claims.AddNew();
                    else
                        newRow = ((DataRowView)v_claims[v_claims.Position]);
                    FillRowFromClaim(claim, newRow);
                    claims.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (claim.IdClaim == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о претензионно-исковой работе без внутреннего номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (ClaimsDataModel.Update(claim) == -1)
                        return;
                    var row = ((DataRowView)v_claims[v_claims.Position]);
                    is_editable = false;
                    FillRowFromClaim(claim, row);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            dataGridViewClaims.Enabled = true;
            is_editable = true;
            dataGridViewClaims.RowCount = v_claims.Count;
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
                    claims.EditingNewRecord = false;
                    if (v_claims.Position != -1)
                    {
                        is_editable = false;
                        dataGridViewClaims.Enabled = true;
                        ((DataRowView)v_claims[v_claims.Position]).Delete();
                        dataGridViewClaims.RowCount = dataGridViewClaims.RowCount - 1;
                        if (v_claims.Position != -1)
                            dataGridViewClaims.Rows[v_claims.Position].Selected = true;
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
            if (v_claims.Count > 0)
                viewport.LocateClaimBy((((DataRowView)v_claims[v_claims.Position])["id_claim"] as int?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                claims.Select().RowChanged -= ClaimListViewport_RowChanged;
                claims.Select().RowDeleted -= ClaimListViewport_RowDeleted;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                claims.EditingNewRecord = false;
            claims.Select().RowChanged -= ClaimListViewport_RowChanged;
            claims.Select().RowDeleted -= ClaimListViewport_RowDeleted;
            Close();
        }

        public override bool HasAssocClaimStates()
        {
            return (v_claims.Position > -1);
        }

        public override void ShowClaimStates()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_claims.Position == -1)
            {
                MessageBox.Show(@"Не выбрана протензионно-исковая работа для отображения ее состояний", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, ViewportType.ClaimStatesViewport,
                "id_claim = " + Convert.ToInt32(((DataRowView)v_claims[v_claims.Position])["id_claim"], CultureInfo.InvariantCulture),
                ((DataRowView)v_claims[v_claims.Position]).Row, ParentTypeEnum.Claim);
        }

        void v_claims_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            if (v_claims.Position == -1 || dataGridViewClaims.RowCount == 0)
                dataGridViewClaims.ClearSelection();
            else
                if (v_claims.Position >= dataGridViewClaims.RowCount)
				{
                    dataGridViewClaims.Rows[dataGridViewClaims.RowCount - 1].Selected = true;
					dataGridViewClaims.CurrentCell = dataGridViewClaims.Rows[dataGridViewClaims.RowCount - 1].
						Cells[1];
				}
            else
                    if (dataGridViewClaims.Rows[v_claims.Position].Selected != true)
					{
                        dataGridViewClaims.Rows[v_claims.Position].Selected = true;
						dataGridViewClaims.CurrentCell = dataGridViewClaims.Rows[v_claims.Position].
							Cells[1];
					}
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            if (v_claims.Position == -1)
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
                v_claims.Sort = dataGridViewClaims.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
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
                v_claims.Position = dataGridViewClaims.SelectedRows[0].Index;
            else
                v_claims.Position = -1;
        }

        void dataGridViewClaims_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_claims.Count <= e.RowIndex) return;
            switch (dataGridViewClaims.Columns[e.ColumnIndex].Name)
            {
                case "id_claim":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["id_claim"];
                    break;
                case "date_of_transfer":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["date_of_transfer"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)v_claims[e.RowIndex])["date_of_transfer"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "amount_of_debt_rent":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["amount_of_debt_rent"];
                    break;
                case "amount_of_debt_fine":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["amount_of_debt_fine"];
                    break;
                case "at_date":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["at_date"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)v_claims[e.RowIndex])["at_date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "description":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["description"];
                    break;
            }
        }

        void numericUpDownProcessID_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownAmountOfRentRecover_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownAmountOfFineRecover_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownAmountOfRent_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownAmountOfFine_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownAmountOfDebtRent_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void numericUpDownAmountOfDebtFine_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerEndDeptPeriod_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerStartDeptPeriod_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerAtDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerDateOfTransfer_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dataGridViewClaims_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        void ClaimListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                dataGridViewClaims.RowCount = v_claims.Count;
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
            dataGridViewClaims.RowCount = v_claims.Count;
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

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(ClaimListViewport));
            tableLayoutPanel15 = new TableLayoutPanel();
            groupBox34 = new GroupBox();
            tableLayoutPanel16 = new TableLayoutPanel();
            panel8 = new Panel();
            numericUpDownAmountOfDebtRent = new NumericUpDown();
            label102 = new Label();
            numericUpDownAmountOfFine = new NumericUpDown();
            label93 = new Label();
            numericUpDownAmountOfRent = new NumericUpDown();
            label92 = new Label();
            dateTimePickerAtDate = new DateTimePicker();
            label91 = new Label();
            dateTimePickerDateOfTransfer = new DateTimePicker();
            label90 = new Label();
            numericUpDownAmountOfDebtFine = new NumericUpDown();
            label89 = new Label();
            panel9 = new Panel();
            numericUpDownProcessID = new NumericUpDown();
            label103 = new Label();
            numericUpDownAmountOfFineRecover = new NumericUpDown();
            label95 = new Label();
            textBoxDescription = new TextBox();
            numericUpDownAmountOfRentRecover = new NumericUpDown();
            label99 = new Label();
            label94 = new Label();
            label98 = new Label();
            dateTimePickerEndDeptPeriod = new DateTimePicker();
            label97 = new Label();
            dateTimePickerStartDeptPeriod = new DateTimePicker();
            label96 = new Label();
            dataGridViewClaims = new DataGridView();
            id_claim = new DataGridViewTextBoxColumn();
            date_of_transfer = new DataGridViewTextBoxColumn();
            amount_of_debt_rent = new DataGridViewTextBoxColumn();
            amount_of_debt_fine = new DataGridViewTextBoxColumn();
            at_date = new DataGridViewTextBoxColumn();
            description = new DataGridViewTextBoxColumn();
            tableLayoutPanel15.SuspendLayout();
            groupBox34.SuspendLayout();
            tableLayoutPanel16.SuspendLayout();
            panel8.SuspendLayout();
            ((ISupportInitialize)(numericUpDownAmountOfDebtRent)).BeginInit();
            ((ISupportInitialize)(numericUpDownAmountOfFine)).BeginInit();
            ((ISupportInitialize)(numericUpDownAmountOfRent)).BeginInit();
            ((ISupportInitialize)(numericUpDownAmountOfDebtFine)).BeginInit();
            panel9.SuspendLayout();
            ((ISupportInitialize)(numericUpDownProcessID)).BeginInit();
            ((ISupportInitialize)(numericUpDownAmountOfFineRecover)).BeginInit();
            ((ISupportInitialize)(numericUpDownAmountOfRentRecover)).BeginInit();
            ((ISupportInitialize)(dataGridViewClaims)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel15
            // 
            tableLayoutPanel15.ColumnCount = 1;
            tableLayoutPanel15.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel15.Controls.Add(groupBox34, 0, 0);
            tableLayoutPanel15.Controls.Add(dataGridViewClaims, 0, 1);
            tableLayoutPanel15.Dock = DockStyle.Fill;
            tableLayoutPanel15.Location = new Point(3, 3);
            tableLayoutPanel15.Name = "tableLayoutPanel15";
            tableLayoutPanel15.RowCount = 2;
            tableLayoutPanel15.RowStyles.Add(new RowStyle(SizeType.Absolute, 196F));
            tableLayoutPanel15.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel15.Size = new Size(773, 491);
            tableLayoutPanel15.TabIndex = 0;
            // 
            // groupBox34
            // 
            groupBox34.Controls.Add(tableLayoutPanel16);
            groupBox34.Dock = DockStyle.Fill;
            groupBox34.Location = new Point(0, 0);
            groupBox34.Margin = new Padding(0);
            groupBox34.Name = "groupBox34";
            groupBox34.Size = new Size(773, 196);
            groupBox34.TabIndex = 1;
            groupBox34.TabStop = false;
            groupBox34.Text = @"Общие сведения";
            // 
            // tableLayoutPanel16
            // 
            tableLayoutPanel16.ColumnCount = 2;
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel16.Controls.Add(panel8, 0, 0);
            tableLayoutPanel16.Controls.Add(panel9, 1, 0);
            tableLayoutPanel16.Dock = DockStyle.Fill;
            tableLayoutPanel16.Location = new Point(3, 17);
            tableLayoutPanel16.Name = "tableLayoutPanel16";
            tableLayoutPanel16.RowCount = 1;
            tableLayoutPanel16.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel16.RowStyles.Add(new RowStyle(SizeType.Absolute, 155F));
            tableLayoutPanel16.Size = new Size(767, 176);
            tableLayoutPanel16.TabIndex = 0;
            // 
            // panel8
            // 
            panel8.Controls.Add(numericUpDownAmountOfDebtRent);
            panel8.Controls.Add(label102);
            panel8.Controls.Add(numericUpDownAmountOfFine);
            panel8.Controls.Add(label93);
            panel8.Controls.Add(numericUpDownAmountOfRent);
            panel8.Controls.Add(label92);
            panel8.Controls.Add(dateTimePickerAtDate);
            panel8.Controls.Add(label91);
            panel8.Controls.Add(dateTimePickerDateOfTransfer);
            panel8.Controls.Add(label90);
            panel8.Controls.Add(numericUpDownAmountOfDebtFine);
            panel8.Controls.Add(label89);
            panel8.Dock = DockStyle.Fill;
            panel8.Location = new Point(0, 0);
            panel8.Margin = new Padding(0);
            panel8.Name = "panel8";
            panel8.Size = new Size(383, 176);
            panel8.TabIndex = 0;
            // 
            // numericUpDownAmountOfDebtRent
            // 
            numericUpDownAmountOfDebtRent.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                   | AnchorStyles.Right;
            numericUpDownAmountOfDebtRent.DecimalPlaces = 2;
            numericUpDownAmountOfDebtRent.Location = new Point(172, 33);
            numericUpDownAmountOfDebtRent.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownAmountOfDebtRent.Name = "numericUpDownAmountOfDebtRent";
            numericUpDownAmountOfDebtRent.Size = new Size(202, 21);
            numericUpDownAmountOfDebtRent.TabIndex = 1;
            numericUpDownAmountOfDebtRent.ThousandsSeparator = true;
            numericUpDownAmountOfDebtRent.ValueChanged += numericUpDownAmountOfDebtRent_ValueChanged;
            numericUpDownAmountOfDebtRent.Enter += selectAll_Enter;
            // 
            // label102
            // 
            label102.AutoSize = true;
            label102.Location = new Point(14, 35);
            label102.Name = "label102";
            label102.Size = new Size(100, 15);
            label102.TabIndex = 37;
            label102.Text = @"Сумма долга АП";
            // 
            // numericUpDownAmountOfFine
            // 
            numericUpDownAmountOfFine.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                               | AnchorStyles.Right;
            numericUpDownAmountOfFine.DecimalPlaces = 2;
            numericUpDownAmountOfFine.Location = new Point(172, 149);
            numericUpDownAmountOfFine.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownAmountOfFine.Name = "numericUpDownAmountOfFine";
            numericUpDownAmountOfFine.Size = new Size(202, 21);
            numericUpDownAmountOfFine.TabIndex = 5;
            numericUpDownAmountOfFine.ThousandsSeparator = true;
            numericUpDownAmountOfFine.ValueChanged += numericUpDownAmountOfFine_ValueChanged;
            numericUpDownAmountOfFine.Enter += selectAll_Enter;
            // 
            // label93
            // 
            label93.AutoSize = true;
            label93.Location = new Point(14, 151);
            label93.Name = "label93";
            label93.Size = new Size(120, 15);
            label93.TabIndex = 35;
            label93.Text = @"Сумма пени по иску";
            // 
            // numericUpDownAmountOfRent
            // 
            numericUpDownAmountOfRent.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                               | AnchorStyles.Right;
            numericUpDownAmountOfRent.DecimalPlaces = 2;
            numericUpDownAmountOfRent.Location = new Point(172, 120);
            numericUpDownAmountOfRent.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownAmountOfRent.Name = "numericUpDownAmountOfRent";
            numericUpDownAmountOfRent.Size = new Size(202, 21);
            numericUpDownAmountOfRent.TabIndex = 4;
            numericUpDownAmountOfRent.ThousandsSeparator = true;
            numericUpDownAmountOfRent.ValueChanged += numericUpDownAmountOfRent_ValueChanged;
            numericUpDownAmountOfRent.Enter += selectAll_Enter;
            // 
            // label92
            // 
            label92.AutoSize = true;
            label92.Location = new Point(14, 122);
            label92.Name = "label92";
            label92.Size = new Size(108, 15);
            label92.TabIndex = 33;
            label92.Text = @"Сумма АП по иску";
            // 
            // dateTimePickerAtDate
            // 
            dateTimePickerAtDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                          | AnchorStyles.Right;
            dateTimePickerAtDate.Location = new Point(172, 91);
            dateTimePickerAtDate.Name = "dateTimePickerAtDate";
            dateTimePickerAtDate.ShowCheckBox = true;
            dateTimePickerAtDate.Size = new Size(202, 21);
            dateTimePickerAtDate.TabIndex = 3;
            dateTimePickerAtDate.ValueChanged += dateTimePickerAtDate_ValueChanged;
            // 
            // label91
            // 
            label91.AutoSize = true;
            label91.Location = new Point(14, 94);
            label91.Name = "label91";
            label91.Size = new Size(52, 15);
            label91.TabIndex = 31;
            label91.Text = @"На дату";
            // 
            // dateTimePickerDateOfTransfer
            // 
            dateTimePickerDateOfTransfer.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                  | AnchorStyles.Right;
            dateTimePickerDateOfTransfer.Location = new Point(172, 4);
            dateTimePickerDateOfTransfer.Name = "dateTimePickerDateOfTransfer";
            dateTimePickerDateOfTransfer.ShowCheckBox = true;
            dateTimePickerDateOfTransfer.Size = new Size(202, 21);
            dateTimePickerDateOfTransfer.TabIndex = 0;
            dateTimePickerDateOfTransfer.ValueChanged += dateTimePickerDateOfTransfer_ValueChanged;
            // 
            // label90
            // 
            label90.AutoSize = true;
            label90.Location = new Point(14, 7);
            label90.Name = "label90";
            label90.Size = new Size(95, 15);
            label90.TabIndex = 29;
            label90.Text = @"Дата передачи";
            // 
            // numericUpDownAmountOfDebtFine
            // 
            numericUpDownAmountOfDebtFine.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                   | AnchorStyles.Right;
            numericUpDownAmountOfDebtFine.DecimalPlaces = 2;
            numericUpDownAmountOfDebtFine.Location = new Point(172, 62);
            numericUpDownAmountOfDebtFine.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownAmountOfDebtFine.Name = "numericUpDownAmountOfDebtFine";
            numericUpDownAmountOfDebtFine.Size = new Size(202, 21);
            numericUpDownAmountOfDebtFine.TabIndex = 2;
            numericUpDownAmountOfDebtFine.ThousandsSeparator = true;
            numericUpDownAmountOfDebtFine.ValueChanged += numericUpDownAmountOfDebtFine_ValueChanged;
            numericUpDownAmountOfDebtFine.Enter += selectAll_Enter;
            // 
            // label89
            // 
            label89.AutoSize = true;
            label89.Location = new Point(14, 64);
            label89.Name = "label89";
            label89.Size = new Size(112, 15);
            label89.TabIndex = 24;
            label89.Text = @"Сумма долга пени";
            // 
            // panel9
            // 
            panel9.Controls.Add(numericUpDownProcessID);
            panel9.Controls.Add(label103);
            panel9.Controls.Add(numericUpDownAmountOfFineRecover);
            panel9.Controls.Add(label95);
            panel9.Controls.Add(textBoxDescription);
            panel9.Controls.Add(numericUpDownAmountOfRentRecover);
            panel9.Controls.Add(label99);
            panel9.Controls.Add(label94);
            panel9.Controls.Add(label98);
            panel9.Controls.Add(dateTimePickerEndDeptPeriod);
            panel9.Controls.Add(label97);
            panel9.Controls.Add(dateTimePickerStartDeptPeriod);
            panel9.Controls.Add(label96);
            panel9.Dock = DockStyle.Fill;
            panel9.Location = new Point(383, 0);
            panel9.Margin = new Padding(0);
            panel9.Name = "panel9";
            panel9.Size = new Size(384, 176);
            panel9.TabIndex = 1;
            // 
            // numericUpDownProcessID
            // 
            numericUpDownProcessID.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            numericUpDownProcessID.Location = new Point(172, 4);
            numericUpDownProcessID.Maximum = new decimal(new[] {
            1410065407,
            0,
            0,
            0});
            numericUpDownProcessID.Name = "numericUpDownProcessID";
            numericUpDownProcessID.Size = new Size(202, 21);
            numericUpDownProcessID.TabIndex = 0;
            numericUpDownProcessID.ValueChanged += numericUpDownProcessID_ValueChanged;
            numericUpDownProcessID.Enter += selectAll_Enter;
            // 
            // label103
            // 
            label103.AutoSize = true;
            label103.Location = new Point(14, 6);
            label103.Name = "label103";
            label103.Size = new Size(96, 15);
            label103.TabIndex = 51;
            label103.Text = @"Процесс найма";
            // 
            // numericUpDownAmountOfFineRecover
            // 
            numericUpDownAmountOfFineRecover.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                      | AnchorStyles.Right;
            numericUpDownAmountOfFineRecover.DecimalPlaces = 2;
            numericUpDownAmountOfFineRecover.Location = new Point(172, 62);
            numericUpDownAmountOfFineRecover.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownAmountOfFineRecover.Name = "numericUpDownAmountOfFineRecover";
            numericUpDownAmountOfFineRecover.Size = new Size(202, 21);
            numericUpDownAmountOfFineRecover.TabIndex = 8;
            numericUpDownAmountOfFineRecover.ThousandsSeparator = true;
            numericUpDownAmountOfFineRecover.ValueChanged += numericUpDownAmountOfFineRecover_ValueChanged;
            numericUpDownAmountOfFineRecover.Enter += selectAll_Enter;
            // 
            // label95
            // 
            label95.AutoSize = true;
            label95.Location = new Point(14, 64);
            label95.Name = "label95";
            label95.Size = new Size(152, 15);
            label95.TabIndex = 39;
            label95.Text = @"Сумма пени к взысканию";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                        | AnchorStyles.Right;
            textBoxDescription.Location = new Point(172, 149);
            textBoxDescription.MaxLength = 4000;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(202, 21);
            textBoxDescription.TabIndex = 11;
            textBoxDescription.TextChanged += textBoxDescription_TextChanged;
            textBoxDescription.Enter += selectAll_Enter;
            // 
            // numericUpDownAmountOfRentRecover
            // 
            numericUpDownAmountOfRentRecover.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                      | AnchorStyles.Right;
            numericUpDownAmountOfRentRecover.DecimalPlaces = 2;
            numericUpDownAmountOfRentRecover.Location = new Point(172, 33);
            numericUpDownAmountOfRentRecover.Maximum = new decimal(new[] {
            1410065407,
            2,
            0,
            0});
            numericUpDownAmountOfRentRecover.Name = "numericUpDownAmountOfRentRecover";
            numericUpDownAmountOfRentRecover.Size = new Size(202, 21);
            numericUpDownAmountOfRentRecover.TabIndex = 7;
            numericUpDownAmountOfRentRecover.ThousandsSeparator = true;
            numericUpDownAmountOfRentRecover.ValueChanged += numericUpDownAmountOfRentRecover_ValueChanged;
            numericUpDownAmountOfRentRecover.Enter += selectAll_Enter;
            // 
            // label99
            // 
            label99.AutoSize = true;
            label99.Location = new Point(14, 151);
            label99.Name = "label99";
            label99.Size = new Size(80, 15);
            label99.TabIndex = 49;
            label99.Text = @"Примечание";
            // 
            // label94
            // 
            label94.AutoSize = true;
            label94.Location = new Point(14, 35);
            label94.Name = "label94";
            label94.Size = new Size(140, 15);
            label94.TabIndex = 37;
            label94.Text = @"Сумма АП к взысканию";
            // 
            // label98
            // 
            label98.AutoSize = true;
            label98.Location = new Point(145, 122);
            label98.Name = "label98";
            label98.Size = new Size(21, 15);
            label98.TabIndex = 48;
            label98.Text = @"по";
            // 
            // dateTimePickerEndDeptPeriod
            // 
            dateTimePickerEndDeptPeriod.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                 | AnchorStyles.Right;
            dateTimePickerEndDeptPeriod.Location = new Point(172, 120);
            dateTimePickerEndDeptPeriod.Name = "dateTimePickerEndDeptPeriod";
            dateTimePickerEndDeptPeriod.ShowCheckBox = true;
            dateTimePickerEndDeptPeriod.Size = new Size(202, 21);
            dateTimePickerEndDeptPeriod.TabIndex = 10;
            dateTimePickerEndDeptPeriod.ValueChanged += dateTimePickerEndDeptPeriod_ValueChanged;
            // 
            // label97
            // 
            label97.AutoSize = true;
            label97.Location = new Point(151, 93);
            label97.Name = "label97";
            label97.Size = new Size(13, 15);
            label97.TabIndex = 46;
            label97.Text = @"с";
            // 
            // dateTimePickerStartDeptPeriod
            // 
            dateTimePickerStartDeptPeriod.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                   | AnchorStyles.Right;
            dateTimePickerStartDeptPeriod.Location = new Point(172, 91);
            dateTimePickerStartDeptPeriod.Name = "dateTimePickerStartDeptPeriod";
            dateTimePickerStartDeptPeriod.ShowCheckBox = true;
            dateTimePickerStartDeptPeriod.Size = new Size(202, 21);
            dateTimePickerStartDeptPeriod.TabIndex = 9;
            dateTimePickerStartDeptPeriod.ValueChanged += dateTimePickerStartDeptPeriod_ValueChanged;
            // 
            // label96
            // 
            label96.AutoSize = true;
            label96.Location = new Point(14, 93);
            label96.Name = "label96";
            label96.Size = new Size(131, 15);
            label96.TabIndex = 44;
            label96.Text = @"Период задолжности";
            // 
            // dataGridViewClaims
            // 
            dataGridViewClaims.AllowUserToAddRows = false;
            dataGridViewClaims.AllowUserToDeleteRows = false;
            dataGridViewClaims.AllowUserToResizeRows = false;
            dataGridViewClaims.BackgroundColor = Color.White;
            dataGridViewClaims.BorderStyle = BorderStyle.None;
            dataGridViewClaims.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewClaims.Columns.AddRange(id_claim, date_of_transfer, amount_of_debt_rent, amount_of_debt_fine, at_date, description);
            dataGridViewClaims.Dock = DockStyle.Fill;
            dataGridViewClaims.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridViewClaims.Location = new Point(3, 199);
            dataGridViewClaims.MultiSelect = false;
            dataGridViewClaims.Name = "dataGridViewClaims";
            dataGridViewClaims.ReadOnly = true;
            dataGridViewClaims.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewClaims.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewClaims.ShowCellToolTips = false;
            dataGridViewClaims.Size = new Size(767, 289);
            dataGridViewClaims.TabIndex = 0;
            dataGridViewClaims.VirtualMode = true;
            dataGridViewClaims.CellValueNeeded += dataGridViewClaims_CellValueNeeded;
            dataGridViewClaims.ColumnHeaderMouseClick += dataGridViewClaims_ColumnHeaderMouseClick;
            dataGridViewClaims.DataError += dataGridViewClaims_DataError;
            dataGridViewClaims.SelectionChanged += dataGridViewClaims_SelectionChanged;
            // 
            // id_claim
            // 
            id_claim.HeaderText = @"№";
            id_claim.MinimumWidth = 50;
            id_claim.Name = "id_claim";
            id_claim.ReadOnly = true;
            id_claim.Width = 50;
            // 
            // date_of_transfer
            // 
            date_of_transfer.HeaderText = @"Дата передачи";
            date_of_transfer.MinimumWidth = 150;
            date_of_transfer.Name = "date_of_transfer";
            date_of_transfer.ReadOnly = true;
            date_of_transfer.Width = 150;
            // 
            // amount_of_debt_rent
            // 
            dataGridViewCellStyle1.Format = "#0.0# руб.";
            amount_of_debt_rent.DefaultCellStyle = dataGridViewCellStyle1;
            amount_of_debt_rent.HeaderText = @"Сумма долга АП";
            amount_of_debt_rent.MinimumWidth = 150;
            amount_of_debt_rent.Name = "amount_of_debt_rent";
            amount_of_debt_rent.ReadOnly = true;
            amount_of_debt_rent.Width = 150;
            // 
            // amount_of_debt_fine
            // 
            dataGridViewCellStyle2.Format = "#0.0# руб.";
            amount_of_debt_fine.DefaultCellStyle = dataGridViewCellStyle2;
            amount_of_debt_fine.HeaderText = @"Сумма долга пени";
            amount_of_debt_fine.MinimumWidth = 150;
            amount_of_debt_fine.Name = "amount_of_debt_fine";
            amount_of_debt_fine.ReadOnly = true;
            amount_of_debt_fine.Width = 150;
            // 
            // at_date
            // 
            at_date.HeaderText = @"На дату";
            at_date.MinimumWidth = 150;
            at_date.Name = "at_date";
            at_date.ReadOnly = true;
            at_date.Width = 150;
            // 
            // description
            // 
            description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            description.HeaderText = @"Примечание";
            description.MinimumWidth = 200;
            description.Name = "description";
            description.ReadOnly = true;
            // 
            // ClaimListViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(680, 300);
            BackColor = Color.White;
            ClientSize = new Size(779, 497);
            Controls.Add(tableLayoutPanel15);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "ClaimListViewport";
            Padding = new Padding(3);
            Text = @"Исковая работа №{0}";
            tableLayoutPanel15.ResumeLayout(false);
            groupBox34.ResumeLayout(false);
            tableLayoutPanel16.ResumeLayout(false);
            panel8.ResumeLayout(false);
            panel8.PerformLayout();
            ((ISupportInitialize)(numericUpDownAmountOfDebtRent)).EndInit();
            ((ISupportInitialize)(numericUpDownAmountOfFine)).EndInit();
            ((ISupportInitialize)(numericUpDownAmountOfRent)).EndInit();
            ((ISupportInitialize)(numericUpDownAmountOfDebtFine)).EndInit();
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            ((ISupportInitialize)(numericUpDownProcessID)).EndInit();
            ((ISupportInitialize)(numericUpDownAmountOfFineRecover)).EndInit();
            ((ISupportInitialize)(numericUpDownAmountOfRentRecover)).EndInit();
            ((ISupportInitialize)(dataGridViewClaims)).EndInit();
            ResumeLayout(false);

        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
