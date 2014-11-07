using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;

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
        DataGridViewTextBoxColumn id_claim;
        DataGridViewTextBoxColumn date_of_transfer;
        DataGridViewTextBoxColumn amount_of_debt_rent;
        DataGridViewTextBoxColumn amount_of_debt_fine;
        DataGridViewTextBoxColumn at_date;
        DataGridViewTextBoxColumn description;
        #endregion Components

        //Modeles
        ClaimsDataModel claims = null;

        //Views
        BindingSource v_claims = null;

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;

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
            this.DynamicFilter = claimListViewport.DynamicFilter;
            this.StaticFilter = claimListViewport.StaticFilter;
            this.ParentRow = claimListViewport.ParentRow;
            this.ParentType = claimListViewport.ParentType;
        }

        private void LocateClaimBy(int id)
        {
            int Position = v_claims.Find("id_claim", id);
            is_editable = false;
            if (Position > 0)
                v_claims.Position = Position;
            is_editable = true;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                {
                    this.Text = String.Format("Новая исковая работа найма №{0}", ParentRow["id_process"]);
                }
                else
                    this.Text = "Новая исковая работа";
            }
            else
                if (v_claims.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                        this.Text = String.Format("Исковая работа №{0} найма №{1}",
                            ((DataRowView)v_claims[v_claims.Position])["id_claim"], ParentRow["id_process"]);
                    else
                        this.Text = String.Format("Исковая работа №{0}", ((DataRowView)v_claims[v_claims.Position])["id_claim"]);
                }
                else
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                        this.Text = String.Format("Исковые работы в найме №{0} отсутствуют", ParentRow["id_process"]);
                    else
                        this.Text = "Исковые работы отсутствуют";
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
            DataRowView row = (v_claims.Position >= 0) ? (DataRowView)v_claims[v_claims.Position] : null;
            if ((v_claims.Position >= 0) && (row["date_of_transfer"] != DBNull.Value))
                dateTimePickerDateOfTransfer.Checked = true;
            else
            {
                dateTimePickerDateOfTransfer.Value = DateTime.Now.Date;
                dateTimePickerDateOfTransfer.Checked = false;
            }
            if ((v_claims.Position >= 0) && (row["at_date"] != DBNull.Value))
                dateTimePickerAtDate.Checked = true;
            else
            {
                dateTimePickerAtDate.Value = DateTime.Now.Date;
                dateTimePickerAtDate.Checked = false;
            }
            if ((v_claims.Position >= 0) && (row["start_dept_period"] != DBNull.Value))
                dateTimePickerStartDeptPeriod.Checked = true;
            else
            {
                dateTimePickerStartDeptPeriod.Value = DateTime.Now.Date;
                dateTimePickerStartDeptPeriod.Checked = false;
            }
            if ((v_claims.Position >= 0) && (row["end_dept_period"] != DBNull.Value))
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
            if ((!this.ContainsFocus) || (dataGridViewClaims.Focused))
                return;
            if ((v_claims.Position != -1) && (ClaimFromView() != ClaimFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    menuCallback.EditingStateUpdate();
                    dataGridViewClaims.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    menuCallback.EditingStateUpdate();
                    dataGridViewClaims.Enabled = true;
                }
            }
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
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
                            if (claims.EditingNewRecord)
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
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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

        private bool ValidateClaim(Claim claim)
        {
            if (claim.id_process == null)
            {
                MessageBox.Show("Необходимо задать внутренний номер процесса найм. Если вы видите это сообщение, обратитесь к системному администратору",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            var tenancy_rows = from tenancy_row in TenancyProcessesDataModel.GetInstance().Select().AsEnumerable()
                               where tenancy_row.Field<int>("id_process") == claim.id_process
                               select tenancy_row;
            if (tenancy_rows.Count() == 0)
            {
                MessageBox.Show("Процесса найма с указаннным внутренним номером не существует",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDownProcessID.Focus();
                return false;
            }
            return true;
        }

        private Claim ClaimFromView()
        {
            Claim claim = new Claim();
            DataRowView row = (DataRowView)v_claims[v_claims.Position];
            claim.id_claim = ViewportHelper.ValueOrNull<int>(row, "id_claim");
            claim.id_process = ViewportHelper.ValueOrNull<int>(row, "id_process");
            claim.amount_of_debt_rent = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_debt_rent");
            claim.amount_of_debt_fine = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_debt_fine");
            claim.amount_of_rent = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_rent");
            claim.amount_of_fine = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_fine");
            claim.amount_of_rent_recover = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_rent_recover");
            claim.amount_of_fine_recover = ViewportHelper.ValueOrNull<decimal>(row, "amount_of_fine_recover");
            claim.date_of_transfer = ViewportHelper.ValueOrNull<DateTime>(row, "date_of_transfer");
            claim.at_date = ViewportHelper.ValueOrNull<DateTime>(row, "at_date");
            claim.start_dept_period = ViewportHelper.ValueOrNull<DateTime>(row, "start_dept_period");
            claim.end_dept_period = ViewportHelper.ValueOrNull<DateTime>(row, "end_dept_period");
            claim.description = ViewportHelper.ValueOrNull(row, "description");
            return claim;
        }

        private Claim ClaimFromViewport()
        {
            Claim claim = new Claim();
            if ((v_claims.Position == -1) || ((DataRowView)v_claims[v_claims.Position])["id_claim"] is DBNull)
                claim.id_claim = null;
            else
                claim.id_claim = Convert.ToInt32(((DataRowView)v_claims[v_claims.Position])["id_claim"]);
            claim.id_process = Convert.ToInt32(numericUpDownProcessID.Value);
            claim.amount_of_debt_rent = numericUpDownAmountOfDebtRent.Value;
            claim.amount_of_debt_fine = numericUpDownAmountOfDebtFine.Value;
            claim.amount_of_rent = numericUpDownAmountOfRent.Value;
            claim.amount_of_fine = numericUpDownAmountOfFine.Value;
            claim.amount_of_rent_recover = numericUpDownAmountOfRentRecover.Value;
            claim.amount_of_fine_recover = numericUpDownAmountOfFineRecover.Value;
            claim.date_of_transfer = ViewportHelper.ValueOrNull(dateTimePickerDateOfTransfer);
            claim.at_date = ViewportHelper.ValueOrNull(dateTimePickerAtDate);
            claim.start_dept_period = ViewportHelper.ValueOrNull(dateTimePickerStartDeptPeriod);
            claim.end_dept_period = ViewportHelper.ValueOrNull(dateTimePickerEndDeptPeriod);
            claim.description = ViewportHelper.ValueOrNull(textBoxDescription);
            return claim;
        }

        private void ViewportFromClaim(Claim claim)
        {
            numericUpDownAmountOfDebtFine.Value = ViewportHelper.ValueOrDefault(claim.amount_of_debt_fine);
            numericUpDownAmountOfDebtRent.Value = ViewportHelper.ValueOrDefault(claim.amount_of_debt_rent);
            numericUpDownAmountOfFine.Value = ViewportHelper.ValueOrDefault(claim.amount_of_fine);
            numericUpDownAmountOfRent.Value = ViewportHelper.ValueOrDefault(claim.amount_of_rent);
            numericUpDownAmountOfFineRecover.Value = ViewportHelper.ValueOrDefault(claim.amount_of_fine_recover);
            numericUpDownAmountOfRentRecover.Value = ViewportHelper.ValueOrDefault(claim.amount_of_rent_recover);
            dateTimePickerDateOfTransfer.Value = ViewportHelper.ValueOrDefault(claim.date_of_transfer);
            dateTimePickerAtDate.Value = ViewportHelper.ValueOrDefault(claim.at_date);
            dateTimePickerStartDeptPeriod.Value = ViewportHelper.ValueOrDefault(claim.start_dept_period);
            dateTimePickerEndDeptPeriod.Value = ViewportHelper.ValueOrDefault(claim.end_dept_period);
            textBoxDescription.Text = claim.description;
        }

        private void FillRowFromClaim(Claim claim, DataRowView row)
        {
            row.BeginEdit();
            row["id_claim"] = ViewportHelper.ValueOrDBNull(claim.id_claim);
            row["id_process"] = ViewportHelper.ValueOrDBNull(claim.id_process);
            row["date_of_transfer"] = ViewportHelper.ValueOrDBNull(claim.date_of_transfer);
            row["at_date"] = ViewportHelper.ValueOrDBNull(claim.at_date);
            row["start_dept_period"] = ViewportHelper.ValueOrDBNull(claim.start_dept_period);
            row["end_dept_period"] = ViewportHelper.ValueOrDBNull(claim.end_dept_period);
            row["amount_of_debt_rent"] = ViewportHelper.ValueOrDBNull(claim.amount_of_debt_rent);
            row["amount_of_debt_fine"] = ViewportHelper.ValueOrDBNull(claim.amount_of_debt_fine);
            row["amount_of_rent"] = ViewportHelper.ValueOrDBNull(claim.amount_of_rent);
            row["amount_of_fine"] = ViewportHelper.ValueOrDBNull(claim.amount_of_fine);
            row["amount_of_rent_recover"] = ViewportHelper.ValueOrDBNull(claim.amount_of_rent_recover);
            row["amount_of_fine_recover"] = ViewportHelper.ValueOrDBNull(claim.amount_of_fine_recover);
            row["description"] = ViewportHelper.ValueOrDBNull(claim.description);
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            dataGridViewClaims.AutoGenerateColumns = false;
            claims = ClaimsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            claims.Select();

            DataSet ds = DataSetManager.GetDataSet();

            v_claims = new BindingSource();
            v_claims.CurrentItemChanged += new EventHandler(v_claims_CurrentItemChanged);
            v_claims.DataMember = "claims";
            v_claims.DataSource = ds;
            v_claims.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_claims.Filter += " AND ";
            v_claims.Filter += DynamicFilter;

            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                numericUpDownProcessID.Enabled = false;

            DataBind();

            claims.Select().RowChanged += new DataRowChangeEventHandler(ClaimListViewport_RowChanged);
            claims.Select().RowDeleted += new DataRowChangeEventHandler(ClaimListViewport_RowDeleted);

            dataGridViewClaims.RowCount = v_claims.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridViewClaims);
            is_editable = true;
        }

        public override bool CanInsertRecord()
        {
            if ((viewportState == ViewportState.ReadState || viewportState == ViewportState.ModifyRowState) && !claims.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridViewClaims.RowCount = dataGridViewClaims.RowCount + 1;
            DataRowView row = (DataRowView)v_claims.AddNew();
            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                numericUpDownProcessID.Value = (int)ParentRow["id_process"];
            is_editable = true;
            dataGridViewClaims.Enabled = false;
            claims.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return ((v_claims.Position != -1) && (!claims.EditingNewRecord));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            Claim claim = ClaimFromView();
            dataGridViewClaims.RowCount = dataGridViewClaims.RowCount + 1;
            DataRowView row = (DataRowView)v_claims.AddNew();
            dataGridViewClaims.Enabled = false;
            claims.EditingNewRecord = true;
            ViewportFromClaim(claim);
            dateTimePickerDateOfTransfer.Checked = (claim.date_of_transfer != null);
            dateTimePickerAtDate.Checked = (claim.at_date != null);
            dateTimePickerStartDeptPeriod.Checked = (claim.start_dept_period != null);
            dateTimePickerEndDeptPeriod.Checked = (claim.end_dept_period != null);
            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                numericUpDownProcessID.Value = (int)ParentRow["id_process"];
            is_editable = true;
        }

        public override bool CanDeleteRecord()
        {
            if ((v_claims.Position == -1) || (viewportState == ViewportState.NewRowState))
                return false;
            else
                return true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (claims.Delete((int)((DataRowView)v_claims.Current)["id_claim"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_claims[v_claims.Position]).Delete();
                is_editable = true;
                menuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanSaveRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override void SaveRecord()
        {
            Claim claim = ClaimFromViewport();
            if (!ValidateClaim(claim))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ViewportState.NewRowState:
                    int id_claim = claims.Insert(claim);
                    if (id_claim == -1)
                        return;
                    DataRowView newRow;
                    claim.id_claim = id_claim;
                    is_editable = false;
                    if (v_claims.Position == -1)
                        newRow = (DataRowView)v_claims.AddNew();
                    else
                        newRow = ((DataRowView)v_claims[v_claims.Position]);
                    FillRowFromClaim(claim, newRow);
                    claims.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (claim.id_claim == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись о претензионно-исковой работе без внутреннего номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (claims.Update(claim) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_claims[v_claims.Position]);
                    is_editable = false;
                    FillRowFromClaim(claim, row);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            dataGridViewClaims.Enabled = true;
            is_editable = true;
            viewportState = ViewportState.ReadState;
            menuCallback.EditingStateUpdate();
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
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridViewClaims.Enabled = true;
                    is_editable = false;
                    DataBind();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            menuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            ClaimListViewport viewport = new ClaimListViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_claims.Count > 0)
                viewport.LocateClaimBy((((DataRowView)v_claims[v_claims.Position])["id_claim"] as Int32?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                claims.Select().RowChanged -= new DataRowChangeEventHandler(ClaimListViewport_RowChanged);
                claims.Select().RowDeleted -= new DataRowChangeEventHandler(ClaimListViewport_RowDeleted);
            }
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                claims.EditingNewRecord = false;
            claims.Select().RowChanged -= new DataRowChangeEventHandler(ClaimListViewport_RowChanged);
            claims.Select().RowDeleted -= new DataRowChangeEventHandler(ClaimListViewport_RowDeleted);
            base.Close();
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
                MessageBox.Show("Не выбрана протензионно-исковая работа для отображения ее состояний", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ShowAssocViewport(menuCallback, ViewportType.ClaimStatesViewport,
                "id_claim = " + Convert.ToInt32(((DataRowView)v_claims[v_claims.Position])["id_claim"]),
                ((DataRowView)v_claims[v_claims.Position]).Row, ParentTypeEnum.Claim);
        }

        void v_claims_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            if (v_claims.Position == -1 || dataGridViewClaims.RowCount == 0)
            {
                dataGridViewClaims.ClearSelection();
                return;
            }
            if (v_claims.Position >= dataGridViewClaims.RowCount)
            {
                dataGridViewClaims.Rows[dataGridViewClaims.RowCount - 1].Selected = true;
                dataGridViewClaims.CurrentCell = dataGridViewClaims.Rows[dataGridViewClaims.RowCount - 1].Cells[0];
            }
            else
            {
                dataGridViewClaims.Rows[v_claims.Position].Selected = true;
                dataGridViewClaims.CurrentCell = dataGridViewClaims.Rows[v_claims.Position].Cells[0];
            }
            if (Selected)
                menuCallback.NavigationStateUpdate();
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
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridViewClaims.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                v_claims.Sort = dataGridViewClaims.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridViewClaims.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            if (dataGridViewClaims.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                changeSortColumn(SortOrder.Descending);
            else
                changeSortColumn(SortOrder.Ascending);
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
            switch (this.dataGridViewClaims.Columns[e.ColumnIndex].Name)
            {
                case "id_claim":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["id_claim"];
                    break;
                case "date_of_transfer":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["date_of_transfer"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)v_claims[e.RowIndex])["date_of_transfer"]).ToString("dd.MM.yyyy");
                    break;
                case "amount_of_debt_rent":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["amount_of_debt_rent"];
                    break;
                case "amount_of_debt_fine":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["amount_of_debt_fine"];
                    break;
                case "at_date":
                    e.Value = ((DataRowView)v_claims[e.RowIndex])["at_date"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)v_claims[e.RowIndex])["at_date"]).ToString("dd.MM.yyyy");
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
            }
        }

        void ClaimListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridViewClaims.Refresh();
            dataGridViewClaims.RowCount = v_claims.Count;
            UnbindedCheckBoxesUpdate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            base.OnVisibleChanged(e);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimListViewport));
            this.tableLayoutPanel15 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox34 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel16 = new System.Windows.Forms.TableLayoutPanel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.numericUpDownAmountOfDebtRent = new System.Windows.Forms.NumericUpDown();
            this.label102 = new System.Windows.Forms.Label();
            this.numericUpDownAmountOfFine = new System.Windows.Forms.NumericUpDown();
            this.label93 = new System.Windows.Forms.Label();
            this.numericUpDownAmountOfRent = new System.Windows.Forms.NumericUpDown();
            this.label92 = new System.Windows.Forms.Label();
            this.dateTimePickerAtDate = new System.Windows.Forms.DateTimePicker();
            this.label91 = new System.Windows.Forms.Label();
            this.dateTimePickerDateOfTransfer = new System.Windows.Forms.DateTimePicker();
            this.label90 = new System.Windows.Forms.Label();
            this.numericUpDownAmountOfDebtFine = new System.Windows.Forms.NumericUpDown();
            this.label89 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.numericUpDownProcessID = new System.Windows.Forms.NumericUpDown();
            this.label103 = new System.Windows.Forms.Label();
            this.numericUpDownAmountOfFineRecover = new System.Windows.Forms.NumericUpDown();
            this.label95 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.numericUpDownAmountOfRentRecover = new System.Windows.Forms.NumericUpDown();
            this.label99 = new System.Windows.Forms.Label();
            this.label94 = new System.Windows.Forms.Label();
            this.label98 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDeptPeriod = new System.Windows.Forms.DateTimePicker();
            this.label97 = new System.Windows.Forms.Label();
            this.dateTimePickerStartDeptPeriod = new System.Windows.Forms.DateTimePicker();
            this.label96 = new System.Windows.Forms.Label();
            this.dataGridViewClaims = new System.Windows.Forms.DataGridView();
            this.id_claim = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_of_transfer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_of_debt_rent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_of_debt_fine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.at_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel15.SuspendLayout();
            this.groupBox34.SuspendLayout();
            this.tableLayoutPanel16.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtRent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtFine)).BeginInit();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProcessID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFineRecover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRentRecover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel15
            // 
            this.tableLayoutPanel15.ColumnCount = 1;
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel15.Controls.Add(this.groupBox34, 0, 0);
            this.tableLayoutPanel15.Controls.Add(this.dataGridViewClaims, 0, 1);
            this.tableLayoutPanel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel15.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel15.Name = "tableLayoutPanel15";
            this.tableLayoutPanel15.RowCount = 2;
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 196F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel15.Size = new System.Drawing.Size(949, 491);
            this.tableLayoutPanel15.TabIndex = 0;
            // 
            // groupBox34
            // 
            this.groupBox34.Controls.Add(this.tableLayoutPanel16);
            this.groupBox34.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox34.Location = new System.Drawing.Point(0, 0);
            this.groupBox34.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox34.Name = "groupBox34";
            this.groupBox34.Size = new System.Drawing.Size(949, 196);
            this.groupBox34.TabIndex = 0;
            this.groupBox34.TabStop = false;
            this.groupBox34.Text = "Общие сведения";
            // 
            // tableLayoutPanel16
            // 
            this.tableLayoutPanel16.ColumnCount = 2;
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel16.Controls.Add(this.panel8, 0, 0);
            this.tableLayoutPanel16.Controls.Add(this.panel9, 1, 0);
            this.tableLayoutPanel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel16.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel16.Name = "tableLayoutPanel16";
            this.tableLayoutPanel16.RowCount = 1;
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            this.tableLayoutPanel16.Size = new System.Drawing.Size(943, 177);
            this.tableLayoutPanel16.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.numericUpDownAmountOfDebtRent);
            this.panel8.Controls.Add(this.label102);
            this.panel8.Controls.Add(this.numericUpDownAmountOfFine);
            this.panel8.Controls.Add(this.label93);
            this.panel8.Controls.Add(this.numericUpDownAmountOfRent);
            this.panel8.Controls.Add(this.label92);
            this.panel8.Controls.Add(this.dateTimePickerAtDate);
            this.panel8.Controls.Add(this.label91);
            this.panel8.Controls.Add(this.dateTimePickerDateOfTransfer);
            this.panel8.Controls.Add(this.label90);
            this.panel8.Controls.Add(this.numericUpDownAmountOfDebtFine);
            this.panel8.Controls.Add(this.label89);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Margin = new System.Windows.Forms.Padding(0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(471, 177);
            this.panel8.TabIndex = 0;
            // 
            // numericUpDownAmountOfDebtRent
            // 
            this.numericUpDownAmountOfDebtRent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfDebtRent.DecimalPlaces = 2;
            this.numericUpDownAmountOfDebtRent.Location = new System.Drawing.Point(161, 33);
            this.numericUpDownAmountOfDebtRent.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfDebtRent.Name = "numericUpDownAmountOfDebtRent";
            this.numericUpDownAmountOfDebtRent.Size = new System.Drawing.Size(301, 20);
            this.numericUpDownAmountOfDebtRent.TabIndex = 1;
            this.numericUpDownAmountOfDebtRent.ThousandsSeparator = true;
            this.numericUpDownAmountOfDebtRent.ValueChanged += new System.EventHandler(this.numericUpDownAmountOfDebtRent_ValueChanged);
            // 
            // label102
            // 
            this.label102.AutoSize = true;
            this.label102.Location = new System.Drawing.Point(14, 35);
            this.label102.Name = "label102";
            this.label102.Size = new System.Drawing.Size(91, 13);
            this.label102.TabIndex = 37;
            this.label102.Text = "Сумма долга АП";
            // 
            // numericUpDownAmountOfFine
            // 
            this.numericUpDownAmountOfFine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfFine.DecimalPlaces = 2;
            this.numericUpDownAmountOfFine.Location = new System.Drawing.Point(161, 149);
            this.numericUpDownAmountOfFine.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfFine.Name = "numericUpDownAmountOfFine";
            this.numericUpDownAmountOfFine.Size = new System.Drawing.Size(301, 20);
            this.numericUpDownAmountOfFine.TabIndex = 5;
            this.numericUpDownAmountOfFine.ThousandsSeparator = true;
            this.numericUpDownAmountOfFine.ValueChanged += new System.EventHandler(this.numericUpDownAmountOfFine_ValueChanged);
            // 
            // label93
            // 
            this.label93.AutoSize = true;
            this.label93.Location = new System.Drawing.Point(14, 151);
            this.label93.Name = "label93";
            this.label93.Size = new System.Drawing.Size(109, 13);
            this.label93.TabIndex = 35;
            this.label93.Text = "Сумма пени по иску";
            // 
            // numericUpDownAmountOfRent
            // 
            this.numericUpDownAmountOfRent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfRent.DecimalPlaces = 2;
            this.numericUpDownAmountOfRent.Location = new System.Drawing.Point(161, 120);
            this.numericUpDownAmountOfRent.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfRent.Name = "numericUpDownAmountOfRent";
            this.numericUpDownAmountOfRent.Size = new System.Drawing.Size(301, 20);
            this.numericUpDownAmountOfRent.TabIndex = 4;
            this.numericUpDownAmountOfRent.ThousandsSeparator = true;
            this.numericUpDownAmountOfRent.ValueChanged += new System.EventHandler(this.numericUpDownAmountOfRent_ValueChanged);
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Location = new System.Drawing.Point(14, 122);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(100, 13);
            this.label92.TabIndex = 33;
            this.label92.Text = "Сумма АП по иску";
            // 
            // dateTimePickerAtDate
            // 
            this.dateTimePickerAtDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAtDate.Location = new System.Drawing.Point(161, 91);
            this.dateTimePickerAtDate.Name = "dateTimePickerAtDate";
            this.dateTimePickerAtDate.ShowCheckBox = true;
            this.dateTimePickerAtDate.Size = new System.Drawing.Size(301, 20);
            this.dateTimePickerAtDate.TabIndex = 3;
            this.dateTimePickerAtDate.ValueChanged += new System.EventHandler(this.dateTimePickerAtDate_ValueChanged);
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(14, 94);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(46, 13);
            this.label91.TabIndex = 31;
            this.label91.Text = "На дату";
            // 
            // dateTimePickerDateOfTransfer
            // 
            this.dateTimePickerDateOfTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfTransfer.Location = new System.Drawing.Point(161, 4);
            this.dateTimePickerDateOfTransfer.Name = "dateTimePickerDateOfTransfer";
            this.dateTimePickerDateOfTransfer.ShowCheckBox = true;
            this.dateTimePickerDateOfTransfer.Size = new System.Drawing.Size(301, 20);
            this.dateTimePickerDateOfTransfer.TabIndex = 0;
            this.dateTimePickerDateOfTransfer.ValueChanged += new System.EventHandler(this.dateTimePickerDateOfTransfer_ValueChanged);
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(14, 7);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(83, 13);
            this.label90.TabIndex = 29;
            this.label90.Text = "Дата передачи";
            // 
            // numericUpDownAmountOfDebtFine
            // 
            this.numericUpDownAmountOfDebtFine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfDebtFine.DecimalPlaces = 2;
            this.numericUpDownAmountOfDebtFine.Location = new System.Drawing.Point(161, 62);
            this.numericUpDownAmountOfDebtFine.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfDebtFine.Name = "numericUpDownAmountOfDebtFine";
            this.numericUpDownAmountOfDebtFine.Size = new System.Drawing.Size(301, 20);
            this.numericUpDownAmountOfDebtFine.TabIndex = 2;
            this.numericUpDownAmountOfDebtFine.ThousandsSeparator = true;
            this.numericUpDownAmountOfDebtFine.ValueChanged += new System.EventHandler(this.numericUpDownAmountOfDebtFine_ValueChanged);
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Location = new System.Drawing.Point(14, 64);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(100, 13);
            this.label89.TabIndex = 24;
            this.label89.Text = "Сумма долга пени";
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.numericUpDownProcessID);
            this.panel9.Controls.Add(this.label103);
            this.panel9.Controls.Add(this.numericUpDownAmountOfFineRecover);
            this.panel9.Controls.Add(this.label95);
            this.panel9.Controls.Add(this.textBoxDescription);
            this.panel9.Controls.Add(this.numericUpDownAmountOfRentRecover);
            this.panel9.Controls.Add(this.label99);
            this.panel9.Controls.Add(this.label94);
            this.panel9.Controls.Add(this.label98);
            this.panel9.Controls.Add(this.dateTimePickerEndDeptPeriod);
            this.panel9.Controls.Add(this.label97);
            this.panel9.Controls.Add(this.dateTimePickerStartDeptPeriod);
            this.panel9.Controls.Add(this.label96);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(471, 0);
            this.panel9.Margin = new System.Windows.Forms.Padding(0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(472, 177);
            this.panel9.TabIndex = 1;
            // 
            // numericUpDownProcessID
            // 
            this.numericUpDownProcessID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownProcessID.Location = new System.Drawing.Point(159, 4);
            this.numericUpDownProcessID.Maximum = new decimal(new int[] {
            1410065407,
            0,
            0,
            0});
            this.numericUpDownProcessID.Name = "numericUpDownProcessID";
            this.numericUpDownProcessID.Size = new System.Drawing.Size(302, 20);
            this.numericUpDownProcessID.TabIndex = 0;
            this.numericUpDownProcessID.ValueChanged += new System.EventHandler(this.numericUpDownProcessID_ValueChanged);
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(14, 6);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(86, 13);
            this.label103.TabIndex = 51;
            this.label103.Text = "Процесс найма";
            // 
            // numericUpDownAmountOfFineRecover
            // 
            this.numericUpDownAmountOfFineRecover.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfFineRecover.DecimalPlaces = 2;
            this.numericUpDownAmountOfFineRecover.Location = new System.Drawing.Point(161, 62);
            this.numericUpDownAmountOfFineRecover.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfFineRecover.Name = "numericUpDownAmountOfFineRecover";
            this.numericUpDownAmountOfFineRecover.Size = new System.Drawing.Size(302, 20);
            this.numericUpDownAmountOfFineRecover.TabIndex = 8;
            this.numericUpDownAmountOfFineRecover.ThousandsSeparator = true;
            this.numericUpDownAmountOfFineRecover.ValueChanged += new System.EventHandler(this.numericUpDownAmountOfFineRecover_ValueChanged);
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(14, 64);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(138, 13);
            this.label95.TabIndex = 39;
            this.label95.Text = "Сумма пени к взысканию";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(161, 149);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(302, 20);
            this.textBoxDescription.TabIndex = 11;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // numericUpDownAmountOfRentRecover
            // 
            this.numericUpDownAmountOfRentRecover.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfRentRecover.DecimalPlaces = 2;
            this.numericUpDownAmountOfRentRecover.Location = new System.Drawing.Point(161, 33);
            this.numericUpDownAmountOfRentRecover.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfRentRecover.Name = "numericUpDownAmountOfRentRecover";
            this.numericUpDownAmountOfRentRecover.Size = new System.Drawing.Size(302, 20);
            this.numericUpDownAmountOfRentRecover.TabIndex = 7;
            this.numericUpDownAmountOfRentRecover.ThousandsSeparator = true;
            this.numericUpDownAmountOfRentRecover.ValueChanged += new System.EventHandler(this.numericUpDownAmountOfRentRecover_ValueChanged);
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(14, 151);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(70, 13);
            this.label99.TabIndex = 49;
            this.label99.Text = "Примечание";
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Location = new System.Drawing.Point(14, 35);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(129, 13);
            this.label94.TabIndex = 37;
            this.label94.Text = "Сумма АП к взысканию";
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Location = new System.Drawing.Point(136, 122);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(19, 13);
            this.label98.TabIndex = 48;
            this.label98.Text = "по";
            // 
            // dateTimePickerEndDeptPeriod
            // 
            this.dateTimePickerEndDeptPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDeptPeriod.Location = new System.Drawing.Point(161, 120);
            this.dateTimePickerEndDeptPeriod.Name = "dateTimePickerEndDeptPeriod";
            this.dateTimePickerEndDeptPeriod.ShowCheckBox = true;
            this.dateTimePickerEndDeptPeriod.Size = new System.Drawing.Size(302, 20);
            this.dateTimePickerEndDeptPeriod.TabIndex = 10;
            this.dateTimePickerEndDeptPeriod.ValueChanged += new System.EventHandler(this.dateTimePickerEndDeptPeriod_ValueChanged);
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Location = new System.Drawing.Point(142, 93);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(13, 13);
            this.label97.TabIndex = 46;
            this.label97.Text = "с";
            // 
            // dateTimePickerStartDeptPeriod
            // 
            this.dateTimePickerStartDeptPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartDeptPeriod.Location = new System.Drawing.Point(161, 91);
            this.dateTimePickerStartDeptPeriod.Name = "dateTimePickerStartDeptPeriod";
            this.dateTimePickerStartDeptPeriod.ShowCheckBox = true;
            this.dateTimePickerStartDeptPeriod.Size = new System.Drawing.Size(302, 20);
            this.dateTimePickerStartDeptPeriod.TabIndex = 9;
            this.dateTimePickerStartDeptPeriod.ValueChanged += new System.EventHandler(this.dateTimePickerStartDeptPeriod_ValueChanged);
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Location = new System.Drawing.Point(14, 93);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(115, 13);
            this.label96.TabIndex = 44;
            this.label96.Text = "Период задолжности";
            // 
            // dataGridViewClaims
            // 
            this.dataGridViewClaims.AllowUserToAddRows = false;
            this.dataGridViewClaims.AllowUserToDeleteRows = false;
            this.dataGridViewClaims.AllowUserToResizeRows = false;
            this.dataGridViewClaims.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewClaims.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewClaims.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaims.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_claim,
            this.date_of_transfer,
            this.amount_of_debt_rent,
            this.amount_of_debt_fine,
            this.at_date,
            this.description});
            this.dataGridViewClaims.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaims.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewClaims.Location = new System.Drawing.Point(3, 199);
            this.dataGridViewClaims.MultiSelect = false;
            this.dataGridViewClaims.Name = "dataGridViewClaims";
            this.dataGridViewClaims.ReadOnly = true;
            this.dataGridViewClaims.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewClaims.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewClaims.ShowCellToolTips = false;
            this.dataGridViewClaims.Size = new System.Drawing.Size(943, 289);
            this.dataGridViewClaims.TabIndex = 1;
            this.dataGridViewClaims.VirtualMode = true;
            this.dataGridViewClaims.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridViewClaims_CellValueNeeded);
            this.dataGridViewClaims.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewClaims_ColumnHeaderMouseClick);
            this.dataGridViewClaims.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewClaims_DataError);
            this.dataGridViewClaims.SelectionChanged += new System.EventHandler(this.dataGridViewClaims_SelectionChanged);
            // 
            // id_claim
            // 
            this.id_claim.HeaderText = "№";
            this.id_claim.Name = "id_claim";
            this.id_claim.ReadOnly = true;
            // 
            // date_of_transfer
            // 
            this.date_of_transfer.HeaderText = "Дата передачи";
            this.date_of_transfer.MinimumWidth = 150;
            this.date_of_transfer.Name = "date_of_transfer";
            this.date_of_transfer.ReadOnly = true;
            this.date_of_transfer.Width = 150;
            // 
            // amount_of_debt_rent
            // 
            dataGridViewCellStyle1.Format = "#0.0# руб.";
            this.amount_of_debt_rent.DefaultCellStyle = dataGridViewCellStyle1;
            this.amount_of_debt_rent.HeaderText = "Сумма долга АП";
            this.amount_of_debt_rent.MinimumWidth = 150;
            this.amount_of_debt_rent.Name = "amount_of_debt_rent";
            this.amount_of_debt_rent.ReadOnly = true;
            this.amount_of_debt_rent.Width = 150;
            // 
            // amount_of_debt_fine
            // 
            dataGridViewCellStyle2.Format = "#0.0# руб.";
            this.amount_of_debt_fine.DefaultCellStyle = dataGridViewCellStyle2;
            this.amount_of_debt_fine.HeaderText = "Сумма долга пени";
            this.amount_of_debt_fine.MinimumWidth = 150;
            this.amount_of_debt_fine.Name = "amount_of_debt_fine";
            this.amount_of_debt_fine.ReadOnly = true;
            this.amount_of_debt_fine.Width = 150;
            // 
            // at_date
            // 
            this.at_date.HeaderText = "На дату";
            this.at_date.MinimumWidth = 150;
            this.at_date.Name = "at_date";
            this.at_date.ReadOnly = true;
            this.at_date.Width = 150;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Примечание";
            this.description.MinimumWidth = 200;
            this.description.Name = "description";
            this.description.ReadOnly = true;
            // 
            // ClaimListViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(620, 300);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(955, 497);
            this.Controls.Add(this.tableLayoutPanel15);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClaimListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Исковая работа №{0}";
            this.tableLayoutPanel15.ResumeLayout(false);
            this.groupBox34.ResumeLayout(false);
            this.tableLayoutPanel16.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtRent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtFine)).EndInit();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProcessID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFineRecover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRentRecover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
