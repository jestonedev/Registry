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
        TableLayoutPanel tableLayoutPanel15 = new TableLayoutPanel();
        TableLayoutPanel tableLayoutPanel16 = new TableLayoutPanel();
        Panel panel8 = new Panel();
        Panel panel9 = new Panel();
        GroupBox groupBox34 = new GroupBox();
        NumericUpDown numericUpDownAmountOfDebtFine = new NumericUpDown();
        NumericUpDown numericUpDownAmountOfDebtRent = new NumericUpDown();
        NumericUpDown numericUpDownAmountOfFine = new NumericUpDown();
        NumericUpDown numericUpDownAmountOfRent = new NumericUpDown();
        NumericUpDown numericUpDownAmountOfFineRecover = new NumericUpDown();
        NumericUpDown numericUpDownAmountOfRentRecover = new NumericUpDown();
        Label label89 = new Label();
        Label label90 = new Label();
        Label label91 = new Label();
        Label label92 = new Label();
        Label label93 = new Label();
        Label label94 = new Label();
        Label label95 = new Label();
        Label label96 = new Label();
        Label label97 = new Label();
        Label label98 = new Label();
        Label label99 = new Label();
        Label label102 = new Label();
        TextBox textBoxDescription = new TextBox();
        DateTimePicker dateTimePickerStartDeptPeriod = new DateTimePicker();
        DateTimePicker dateTimePickerEndDeptPeriod = new DateTimePicker();
        DateTimePicker dateTimePickerAtDate = new DateTimePicker();
        DateTimePicker dateTimePickerDateOfTransfer = new DateTimePicker();
        DataGridView dataGridViewClaims = new DataGridView();
        DataGridViewTextBoxColumn field_id_claim = new DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn field_date_of_transfer = new DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn field_amount_of_debt_rent = new DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn field_amount_of_debt_fine = new DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn field_at_date = new DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn field_description = new DataGridViewTextBoxColumn();
        #endregion Components

        //Modeles
        ClaimsDataModel claims = null;

        //Views
        BindingSource v_claims = null;

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;
        private int? contract_id = null;

        public ClaimListViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageClaims";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Исковая работа №{0}";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public ClaimListViewport(ClaimListViewport claimListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = claimListViewport.DynamicFilter;
            this.StaticFilter = claimListViewport.StaticFilter;
            this.ParentRow = claimListViewport.ParentRow;
            this.ParentType = claimListViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            claims = ClaimsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            claims.Select();

            DataSet ds = DataSetManager.GetDataSet();

            v_claims = new BindingSource();
            v_claims.CurrentItemChanged += new EventHandler(v_claims_CurrentItemChanged);
            v_claims.DataMember = "claims";
            v_claims.DataSource = ds;

            DataBind();

            claims.Select().RowChanged += new DataRowChangeEventHandler(ClaimListViewport_RowChanged);
            claims.Select().RowDeleted += new DataRowChangeEventHandler(ClaimListViewport_RowDeleted);

            dateTimePickerDateOfTransfer.ValueChanged += new EventHandler(dateTimePickerDateOfTransfer_ValueChanged);
            dateTimePickerAtDate.ValueChanged += new EventHandler(dateTimePickerAtDate_ValueChanged);
            dateTimePickerStartDeptPeriod.ValueChanged += new EventHandler(dateTimePickerStartDeptPeriod_ValueChanged);
            dateTimePickerEndDeptPeriod.ValueChanged += new EventHandler(dateTimePickerEndDeptPeriod_ValueChanged);
            numericUpDownAmountOfDebtFine.ValueChanged += new EventHandler(numericUpDownAmountOfDebtFine_ValueChanged);
            numericUpDownAmountOfDebtRent.ValueChanged += new EventHandler(numericUpDownAmountOfDebtRent_ValueChanged);
            numericUpDownAmountOfFine.ValueChanged += new EventHandler(numericUpDownAmountOfFine_ValueChanged);
            numericUpDownAmountOfRent.ValueChanged += new EventHandler(numericUpDownAmountOfRent_ValueChanged);
            numericUpDownAmountOfFineRecover.ValueChanged += new EventHandler(numericUpDownAmountOfFineRecover_ValueChanged);
            numericUpDownAmountOfRentRecover.ValueChanged += new EventHandler(numericUpDownAmountOfRentRecover_ValueChanged);
            textBoxDescription.TextChanged += new EventHandler(textBoxDescription_TextChanged);
            dataGridViewClaims.DataError += new DataGridViewDataErrorEventHandler(dataGridViewClaims_DataError);
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
            UnbindedCheckBoxesUpdate();
        }

        void ClaimListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UnbindedCheckBoxesUpdate();
        }

        private void UnbindedCheckBoxesUpdate()
        {
            throw new NotImplementedException();
        }

        private void DataBind()
        {
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", v_claims, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDateOfTransfer.DataBindings.Clear();
            dateTimePickerDateOfTransfer.DataBindings.Add("Value", v_claims, "date_of_transfer", true, DataSourceUpdateMode.Never, DateTime.Now);
            dateTimePickerAtDate.DataBindings.Clear();
            dateTimePickerAtDate.DataBindings.Add("Value", v_claims, "at_date", true, DataSourceUpdateMode.Never, DateTime.Now);
            dateTimePickerStartDeptPeriod.DataBindings.Clear();
            dateTimePickerStartDeptPeriod.DataBindings.Add("Value", v_claims, "start_dept_period", true, DataSourceUpdateMode.Never, DateTime.Now);
            dateTimePickerEndDeptPeriod.DataBindings.Clear();
            dateTimePickerEndDeptPeriod.DataBindings.Add("Value", v_claims, "end_dept_period", true, DataSourceUpdateMode.Never, DateTime.Now);
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
            numericUpDownAmountOfFineRecover.DataBindings.Clear();
            numericUpDownAmountOfRentRecover.DataBindings.Add("Value", v_claims, "amount_of_rent_recover", true, DataSourceUpdateMode.Never, 0);
            dataGridViewClaims.DataSource = v_claims;
            field_id_claim.DataPropertyName = "id_claim";
            field_date_of_transfer.DataPropertyName = "date_of_transfer";
            field_amount_of_debt_fine.DataPropertyName = "amount_of_debt_fine";
            field_amount_of_debt_rent.DataPropertyName = "amount_of_debt_rent";
            field_at_date.DataPropertyName = "at_date";
            field_description.DataPropertyName = "description";
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
            menuCallback.EditingStateUpdate();
        }

        public override void Close()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            claims.Select().RowChanged -= new DataRowChangeEventHandler(ClaimListViewport_RowChanged);
            claims.Select().RowDeleted -= new DataRowChangeEventHandler(ClaimListViewport_RowDeleted);
            base.Close();
        }

        public override bool CanSaveRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanCopyRecord()
        {
            return ((v_claims.Position != -1) && (!claims.EditingNewRecord));
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claims.MoveFirst();
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claims.MoveLast();
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claims.MoveNext();
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claims.MovePrevious();
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

        private bool ChangeViewportStateTo(ViewportState viewportState)
        {
            throw new NotImplementedException();
        }

        private Claim ClaimFromView()
        {
            throw new NotImplementedException();
        }

        private Claim ClaimFromViewport()
        {
            throw new NotImplementedException();
        }

        private void ViewportFromClaim(Claim fundHistory)
        {
            throw new NotImplementedException();
        }

        void v_claims_CurrentItemChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ConstructViewport()
        {
            this.Controls.Add(tableLayoutPanel15);
            this.SuspendLayout();
            this.tableLayoutPanel15.SuspendLayout();
            this.tableLayoutPanel16.SuspendLayout();
            this.groupBox34.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtRent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtFine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFineRecover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRentRecover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).BeginInit();
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
            this.tableLayoutPanel15.Size = new System.Drawing.Size(984, 531);
            this.tableLayoutPanel15.TabIndex = 0;
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
            this.tableLayoutPanel16.Size = new System.Drawing.Size(978, 177);
            this.tableLayoutPanel16.TabIndex = 0;
            // 
            // groupBox34
            // 
            this.groupBox34.Controls.Add(this.tableLayoutPanel16);
            this.groupBox34.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox34.Location = new System.Drawing.Point(0, 0);
            this.groupBox34.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox34.Name = "groupBox34";
            this.groupBox34.Size = new System.Drawing.Size(984, 196);
            this.groupBox34.TabIndex = 0;
            this.groupBox34.TabStop = false;
            this.groupBox34.Text = "Общие сведения";
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
            this.panel8.Size = new System.Drawing.Size(489, 177);
            this.panel8.TabIndex = 0;
            // 
            // panel9
            // 
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
            this.panel9.Location = new System.Drawing.Point(489, 0);
            this.panel9.Margin = new System.Windows.Forms.Padding(0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(489, 177);
            this.panel9.TabIndex = 1;
            // 
            // numericUpDownAmountOfDebtRent
            // 
            this.numericUpDownAmountOfDebtRent.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfDebtRent.DecimalPlaces = 2;
            this.numericUpDownAmountOfDebtRent.Location = new System.Drawing.Point(161, 33);
            this.numericUpDownAmountOfDebtRent.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfDebtRent.Name = "numericUpDownAmountOfDebtRent";
            this.numericUpDownAmountOfDebtRent.Size = new System.Drawing.Size(319, 20);
            this.numericUpDownAmountOfDebtRent.TabIndex = 36;
            this.numericUpDownAmountOfDebtRent.ThousandsSeparator = true;
            // 
            // numericUpDownAmountOfFineRecover
            // 
            this.numericUpDownAmountOfFineRecover.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfFineRecover.DecimalPlaces = 2;
            this.numericUpDownAmountOfFineRecover.Location = new System.Drawing.Point(161, 33);
            this.numericUpDownAmountOfFineRecover.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfFineRecover.Name = "numericUpDownAmountOfFineRecover";
            this.numericUpDownAmountOfFineRecover.Size = new System.Drawing.Size(319, 20);
            this.numericUpDownAmountOfFineRecover.TabIndex = 1;
            this.numericUpDownAmountOfFineRecover.ThousandsSeparator = true;
            // 
            // numericUpDownAmountOfRentRecover
            // 
            this.numericUpDownAmountOfRentRecover.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfRentRecover.DecimalPlaces = 2;
            this.numericUpDownAmountOfRentRecover.Location = new System.Drawing.Point(161, 4);
            this.numericUpDownAmountOfRentRecover.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfRentRecover.Name = "numericUpDownAmountOfRentRecover";
            this.numericUpDownAmountOfRentRecover.Size = new System.Drawing.Size(319, 20);
            this.numericUpDownAmountOfRentRecover.TabIndex = 0;
            this.numericUpDownAmountOfRentRecover.ThousandsSeparator = true;
            // 
            // numericUpDownAmountOfFine
            // 
            this.numericUpDownAmountOfFine.Anchor =
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfFine.DecimalPlaces = 2;
            this.numericUpDownAmountOfFine.Location = new System.Drawing.Point(161, 149);
            this.numericUpDownAmountOfFine.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfFine.Name = "numericUpDown3AmountOfFine";
            this.numericUpDownAmountOfFine.Size = new System.Drawing.Size(319, 20);
            this.numericUpDownAmountOfFine.TabIndex = 4;
            this.numericUpDownAmountOfFine.ThousandsSeparator = true;
            // 
            // numericUpDownAmountOfRent
            // 
            this.numericUpDownAmountOfRent.Anchor =
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfRent.DecimalPlaces = 2;
            this.numericUpDownAmountOfRent.Location = new System.Drawing.Point(161, 120);
            this.numericUpDownAmountOfRent.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfRent.Name = "numericUpDownAmountOfRent";
            this.numericUpDownAmountOfRent.Size = new System.Drawing.Size(319, 20);
            this.numericUpDownAmountOfRent.TabIndex = 3;
            this.numericUpDownAmountOfRent.ThousandsSeparator = true;
            // 
            // numericUpDownAmountOfDebtFine
            // 
            this.numericUpDownAmountOfDebtFine.Anchor =
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfDebtFine.DecimalPlaces = 2;
            this.numericUpDownAmountOfDebtFine.Location = new System.Drawing.Point(161, 62);
            this.numericUpDownAmountOfDebtFine.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownAmountOfDebtFine.Name = "numericUpDownAmountOfDebtFine";
            this.numericUpDownAmountOfDebtFine.Size = new System.Drawing.Size(319, 20);
            this.numericUpDownAmountOfDebtFine.TabIndex = 1;
            this.numericUpDownAmountOfDebtFine.ThousandsSeparator = true;
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
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(14, 7);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(83, 13);
            this.label90.TabIndex = 29;
            this.label90.Text = "Дата передачи";
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
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Location = new System.Drawing.Point(14, 122);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(100, 13);
            this.label92.TabIndex = 33;
            this.label92.Text = "Сумма АП по иску";
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
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Location = new System.Drawing.Point(14, 6);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(129, 13);
            this.label94.TabIndex = 37;
            this.label94.Text = "Сумма АП к взысканию";
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(14, 35);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(138, 13);
            this.label95.TabIndex = 39;
            this.label95.Text = "Сумма пени к взысканию";
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Location = new System.Drawing.Point(12, 65);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(115, 13);
            this.label96.TabIndex = 44;
            this.label96.Text = "Период задолжности";
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Location = new System.Drawing.Point(142, 65);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(13, 13);
            this.label97.TabIndex = 46;
            this.label97.Text = "с";
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Location = new System.Drawing.Point(136, 93);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(19, 13);
            this.label98.TabIndex = 48;
            this.label98.Text = "по";
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(12, 123);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(70, 13);
            this.label99.TabIndex = 49;
            this.label99.Text = "Примечание";
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
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(161, 120);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxClaimDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(319, 49);
            this.textBoxDescription.TabIndex = 4;
            // 
            // dateTimePickerStartDeptPeriod
            // 
            this.dateTimePickerStartDeptPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartDeptPeriod.Location = new System.Drawing.Point(161, 62);
            this.dateTimePickerStartDeptPeriod.Name = "dateTimePickerStartDeptPeriod";
            this.dateTimePickerStartDeptPeriod.ShowCheckBox = true;
            this.dateTimePickerStartDeptPeriod.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerStartDeptPeriod.TabIndex = 2;
            // 
            // dateTimePickerEndDatePeriod
            // 
            this.dateTimePickerEndDeptPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDeptPeriod.Location = new System.Drawing.Point(161, 91);
            this.dateTimePickerEndDeptPeriod.Name = "dateTimePickerEndDatePeriod";
            this.dateTimePickerEndDeptPeriod.ShowCheckBox = true;
            this.dateTimePickerEndDeptPeriod.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerEndDeptPeriod.TabIndex = 3;
            // 
            // dateTimePickerAtDate
            // 
            this.dateTimePickerAtDate.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAtDate.Location = new System.Drawing.Point(161, 91);
            this.dateTimePickerAtDate.Name = "dateTimePickerAtDate";
            this.dateTimePickerAtDate.ShowCheckBox = true;
            this.dateTimePickerAtDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerAtDate.TabIndex = 2;
            // 
            // dateTimePickerDateOfTransfer
            // 
            this.dateTimePickerDateOfTransfer.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfTransfer.Location = new System.Drawing.Point(161, 4);
            this.dateTimePickerDateOfTransfer.Name = "dateTimePickerDateOfTransfer";
            this.dateTimePickerDateOfTransfer.ShowCheckBox = true;
            this.dateTimePickerDateOfTransfer.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerDateOfTransfer.TabIndex = 0;
            // 
            // dataGridViewClaims
            // 
            this.dataGridViewClaims.AllowUserToAddRows = false;
            this.dataGridViewClaims.AllowUserToDeleteRows = false;
            this.dataGridViewClaims.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaims.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_claim,
            this.field_date_of_transfer,
            this.field_amount_of_debt_rent,
            this.field_amount_of_debt_fine,
            this.field_at_date,
            this.field_description});
            this.dataGridViewClaims.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaims.Location = new System.Drawing.Point(3, 199);
            this.dataGridViewClaims.Name = "dataGridViewClaims";
            this.dataGridViewClaims.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewClaims.Size = new System.Drawing.Size(978, 329);
            this.dataGridViewClaims.TabIndex = 1;
            // 
            // field_id_claim
            // 
            this.field_id_claim.HeaderText = "№";
            this.field_id_claim.Name = "field_id_claim";
            this.field_id_claim.ReadOnly = true;
            // 
            // field_date_of_transfer
            // 
            this.field_date_of_transfer.HeaderText = "Дата передачи";
            this.field_date_of_transfer.MinimumWidth = 150;
            this.field_date_of_transfer.Name = "field_date_of_transfer";
            this.field_date_of_transfer.ReadOnly = true;
            this.field_date_of_transfer.Width = 150;
            // 
            // field_amount_of_debt_rent
            // 
            this.field_amount_of_debt_rent.HeaderText = "Сумма долга АП";
            this.field_amount_of_debt_rent.MinimumWidth = 150;
            this.field_amount_of_debt_rent.Name = "field_amount_of_debt_rent";
            this.field_amount_of_debt_rent.Width = 150;
            // 
            // field_amount_of_debt_fine
            // 
            this.field_amount_of_debt_fine.HeaderText = "Сумма долга пени";
            this.field_amount_of_debt_fine.MinimumWidth = 150;
            this.field_amount_of_debt_fine.Name = "field_amount_of_debt_fine";
            this.field_amount_of_debt_fine.ReadOnly = true;
            this.field_amount_of_debt_fine.Width = 150;
            // 
            // field_at_date
            // 
            this.field_at_date.HeaderText = "На дату";
            this.field_at_date.MinimumWidth = 150;
            this.field_at_date.Name = "field_at_date";
            this.field_at_date.ReadOnly = true;
            this.field_at_date.Width = 150;
            // 
            // field_description
            // 
            this.field_description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.field_description.HeaderText = "Примечание";
            this.field_description.MinimumWidth = 200;
            this.field_description.Name = "field_description";
            this.field_description.ReadOnly = true;

            this.ResumeLayout(false);
            this.tableLayoutPanel15.ResumeLayout(false);
            this.tableLayoutPanel16.ResumeLayout(false);
            this.groupBox34.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtRent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtFine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFineRecover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRentRecover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).EndInit();
        }
    }
}
