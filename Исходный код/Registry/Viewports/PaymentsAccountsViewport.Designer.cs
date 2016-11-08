using CustomControls;

namespace Registry.Viewport
{
    partial class PaymentsAccountsViewport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaymentsAccountsViewport));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxGeneralInfo = new System.Windows.Forms.GroupBox();
            this.textBoxTenant = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.numericUpDownPrescribed = new FixedNumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownLivingArea = new FixedNumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownTotalArea = new FixedNumericUpDown();
            this.label92 = new System.Windows.Forms.Label();
            this.dateTimePickerAtDate = new System.Windows.Forms.DateTimePicker();
            this.label91 = new System.Windows.Forms.Label();
            this.textBoxAccount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxRawAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCRN = new System.Windows.Forms.TextBox();
            this.label99 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.numericUpDownPaymentPenalties = new FixedNumericUpDown();
            this.numericUpDownPaymentDGI = new FixedNumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.numericUpDownPaymentTenancy = new FixedNumericUpDown();
            this.label24 = new System.Windows.Forms.Label();
            this.groupBoxChargings = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.numericUpDownChargingPenalties = new FixedNumericUpDown();
            this.numericUpDownChargingDGI = new FixedNumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.numericUpDownChargingTotal = new FixedNumericUpDown();
            this.numericUpDownChargingTenancy = new FixedNumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.groupBoxRecalcs = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.numericUpDownRecalcPenalties = new FixedNumericUpDown();
            this.numericUpDownTransferBalance = new FixedNumericUpDown();
            this.numericUpDownRecalcDGI = new FixedNumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.numericUpDownRecalcTenancy = new FixedNumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.groupBoxBalanceOutput = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.numericUpDownPenaltiesOutput = new FixedNumericUpDown();
            this.numericUpDownBalanceDGIOutput = new FixedNumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceTotalOutput = new FixedNumericUpDown();
            this.numericUpDownBalanceTenancyOutput = new FixedNumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.groupBoxBalanceInput = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.numericUpDownPenaltiesInput = new FixedNumericUpDown();
            this.numericUpDownBalanceDGIInput = new FixedNumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceTotalInput = new FixedNumericUpDown();
            this.numericUpDownBalanceTenancyInput = new FixedNumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.crn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.raw_address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parsed_address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prescribed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_input = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_input_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recalc_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recalc_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recalc_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transfer_balance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxGeneralInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrescribed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentPenalties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentDGI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentTenancy)).BeginInit();
            this.groupBoxChargings.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingPenalties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingDGI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingTenancy)).BeginInit();
            this.groupBoxRecalcs.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcPenalties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferBalance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcDGI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcTenancy)).BeginInit();
            this.groupBoxBalanceOutput.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPenaltiesOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceDGIOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceTotalOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceTenancyOutput)).BeginInit();
            this.groupBoxBalanceInput.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPenaltiesInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceDGIInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceTotalInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceTenancyInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBoxGeneralInfo, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxChargings, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxRecalcs, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxBalanceOutput, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxBalanceInput, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1008, 730);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBoxGeneralInfo
            // 
            this.groupBoxGeneralInfo.Controls.Add(this.textBoxTenant);
            this.groupBoxGeneralInfo.Controls.Add(this.label18);
            this.groupBoxGeneralInfo.Controls.Add(this.numericUpDownPrescribed);
            this.groupBoxGeneralInfo.Controls.Add(this.label5);
            this.groupBoxGeneralInfo.Controls.Add(this.numericUpDownLivingArea);
            this.groupBoxGeneralInfo.Controls.Add(this.label4);
            this.groupBoxGeneralInfo.Controls.Add(this.numericUpDownTotalArea);
            this.groupBoxGeneralInfo.Controls.Add(this.label92);
            this.groupBoxGeneralInfo.Controls.Add(this.dateTimePickerAtDate);
            this.groupBoxGeneralInfo.Controls.Add(this.label91);
            this.groupBoxGeneralInfo.Controls.Add(this.textBoxAccount);
            this.groupBoxGeneralInfo.Controls.Add(this.label3);
            this.groupBoxGeneralInfo.Controls.Add(this.textBoxAddress);
            this.groupBoxGeneralInfo.Controls.Add(this.label2);
            this.groupBoxGeneralInfo.Controls.Add(this.textBoxRawAddress);
            this.groupBoxGeneralInfo.Controls.Add(this.label1);
            this.groupBoxGeneralInfo.Controls.Add(this.textBoxCRN);
            this.groupBoxGeneralInfo.Controls.Add(this.label99);
            this.groupBoxGeneralInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxGeneralInfo.Location = new System.Drawing.Point(3, 3);
            this.groupBoxGeneralInfo.Name = "groupBoxGeneralInfo";
            this.tableLayoutPanel1.SetRowSpan(this.groupBoxGeneralInfo, 5);
            this.groupBoxGeneralInfo.Size = new System.Drawing.Size(498, 319);
            this.groupBoxGeneralInfo.TabIndex = 0;
            this.groupBoxGeneralInfo.TabStop = false;
            this.groupBoxGeneralInfo.Text = "Основная информация";
            // 
            // textBoxTenant
            // 
            this.textBoxTenant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTenant.Location = new System.Drawing.Point(151, 123);
            this.textBoxTenant.MaxLength = 4000;
            this.textBoxTenant.Name = "textBoxTenant";
            this.textBoxTenant.Size = new System.Drawing.Size(339, 20);
            this.textBoxTenant.TabIndex = 4;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 127);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(70, 13);
            this.label18.TabIndex = 67;
            this.label18.Text = "Наниматель";
            // 
            // numericUpDownPrescribed
            // 
            this.numericUpDownPrescribed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownPrescribed.Location = new System.Drawing.Point(151, 205);
            this.numericUpDownPrescribed.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownPrescribed.Name = "numericUpDownPrescribed";
            this.numericUpDownPrescribed.Size = new System.Drawing.Size(339, 20);
            this.numericUpDownPrescribed.TabIndex = 7;
            this.numericUpDownPrescribed.ThousandsSeparator = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 207);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 65;
            this.label5.Text = "Прописано";
            // 
            // numericUpDownLivingArea
            // 
            this.numericUpDownLivingArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownLivingArea.DecimalPlaces = 3;
            this.numericUpDownLivingArea.Location = new System.Drawing.Point(151, 177);
            this.numericUpDownLivingArea.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownLivingArea.Name = "numericUpDownLivingArea";
            this.numericUpDownLivingArea.Size = new System.Drawing.Size(339, 20);
            this.numericUpDownLivingArea.TabIndex = 6;
            this.numericUpDownLivingArea.ThousandsSeparator = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 179);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 63;
            this.label4.Text = "Жилая площадь";
            // 
            // numericUpDownTotalArea
            // 
            this.numericUpDownTotalArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownTotalArea.DecimalPlaces = 3;
            this.numericUpDownTotalArea.Location = new System.Drawing.Point(151, 149);
            this.numericUpDownTotalArea.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownTotalArea.Name = "numericUpDownTotalArea";
            this.numericUpDownTotalArea.Size = new System.Drawing.Size(339, 20);
            this.numericUpDownTotalArea.TabIndex = 5;
            this.numericUpDownTotalArea.ThousandsSeparator = true;
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Location = new System.Drawing.Point(9, 151);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(90, 13);
            this.label92.TabIndex = 61;
            this.label92.Text = "Общая площадь";
            // 
            // dateTimePickerAtDate
            // 
            this.dateTimePickerAtDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAtDate.Enabled = false;
            this.dateTimePickerAtDate.Location = new System.Drawing.Point(151, 232);
            this.dateTimePickerAtDate.Name = "dateTimePickerAtDate";
            this.dateTimePickerAtDate.Size = new System.Drawing.Size(339, 20);
            this.dateTimePickerAtDate.TabIndex = 8;
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(9, 235);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(132, 13);
            this.label91.TabIndex = 60;
            this.label91.Text = "Состояние счета на дату";
            // 
            // textBoxAccount
            // 
            this.textBoxAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAccount.Location = new System.Drawing.Point(151, 98);
            this.textBoxAccount.MaxLength = 4000;
            this.textBoxAccount.Name = "textBoxAccount";
            this.textBoxAccount.Size = new System.Drawing.Size(339, 20);
            this.textBoxAccount.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 57;
            this.label3.Text = "Лицевой счет";
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAddress.Location = new System.Drawing.Point(151, 71);
            this.textBoxAddress.MaxLength = 4000;
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(339, 20);
            this.textBoxAddress.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 55;
            this.label2.Text = "Адрес в реестр ЖФ";
            // 
            // textBoxRawAddress
            // 
            this.textBoxRawAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRawAddress.Location = new System.Drawing.Point(151, 45);
            this.textBoxRawAddress.MaxLength = 4000;
            this.textBoxRawAddress.Name = "textBoxRawAddress";
            this.textBoxRawAddress.Size = new System.Drawing.Size(339, 20);
            this.textBoxRawAddress.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 53;
            this.label1.Text = "Адрес по БКС";
            // 
            // textBoxCRN
            // 
            this.textBoxCRN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCRN.Location = new System.Drawing.Point(151, 19);
            this.textBoxCRN.MaxLength = 4000;
            this.textBoxCRN.Name = "textBoxCRN";
            this.textBoxCRN.Size = new System.Drawing.Size(339, 20);
            this.textBoxCRN.TabIndex = 0;
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(9, 22);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(29, 13);
            this.label99.TabIndex = 51;
            this.label99.Text = "СРН";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel5);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(506, 197);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(500, 61);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Оплата (последний месяц)";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel5.Controls.Add(this.numericUpDownPaymentPenalties, 2, 1);
            this.tableLayoutPanel5.Controls.Add(this.numericUpDownPaymentDGI, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.label20, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.label19, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.numericUpDownPaymentTenancy, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.label24, 2, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(496, 44);
            this.tableLayoutPanel5.TabIndex = 71;
            // 
            // numericUpDownPaymentPenalties
            // 
            this.numericUpDownPaymentPenalties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownPaymentPenalties.DecimalPlaces = 2;
            this.numericUpDownPaymentPenalties.Location = new System.Drawing.Point(332, 16);
            this.numericUpDownPaymentPenalties.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownPaymentPenalties.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownPaymentPenalties.Name = "numericUpDownPaymentPenalties";
            this.numericUpDownPaymentPenalties.Size = new System.Drawing.Size(160, 20);
            this.numericUpDownPaymentPenalties.TabIndex = 2;
            this.numericUpDownPaymentPenalties.ThousandsSeparator = true;
            // 
            // numericUpDownPaymentDGI
            // 
            this.numericUpDownPaymentDGI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownPaymentDGI.DecimalPlaces = 2;
            this.numericUpDownPaymentDGI.Location = new System.Drawing.Point(168, 16);
            this.numericUpDownPaymentDGI.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownPaymentDGI.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownPaymentDGI.Name = "numericUpDownPaymentDGI";
            this.numericUpDownPaymentDGI.Size = new System.Drawing.Size(160, 20);
            this.numericUpDownPaymentDGI.TabIndex = 1;
            this.numericUpDownPaymentDGI.ThousandsSeparator = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(4, 2);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(73, 12);
            this.label20.TabIndex = 77;
            this.label20.Text = "Оплата найм";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(168, 2);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(70, 12);
            this.label19.TabIndex = 79;
            this.label19.Text = "Оплата ДГИ";
            // 
            // numericUpDownPaymentTenancy
            // 
            this.numericUpDownPaymentTenancy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownPaymentTenancy.DecimalPlaces = 2;
            this.numericUpDownPaymentTenancy.Location = new System.Drawing.Point(4, 16);
            this.numericUpDownPaymentTenancy.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownPaymentTenancy.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownPaymentTenancy.Name = "numericUpDownPaymentTenancy";
            this.numericUpDownPaymentTenancy.Size = new System.Drawing.Size(160, 20);
            this.numericUpDownPaymentTenancy.TabIndex = 0;
            this.numericUpDownPaymentTenancy.ThousandsSeparator = true;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(332, 2);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(33, 12);
            this.label24.TabIndex = 80;
            this.label24.Text = "Пени";
            // 
            // groupBoxChargings
            // 
            this.groupBoxChargings.Controls.Add(this.tableLayoutPanel3);
            this.groupBoxChargings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxChargings.Location = new System.Drawing.Point(506, 67);
            this.groupBoxChargings.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxChargings.Name = "groupBoxChargings";
            this.groupBoxChargings.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxChargings.Size = new System.Drawing.Size(500, 61);
            this.groupBoxChargings.TabIndex = 2;
            this.groupBoxChargings.TabStop = false;
            this.groupBoxChargings.Text = "Начисления (последний месяц)";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.Controls.Add(this.numericUpDownChargingPenalties, 3, 1);
            this.tableLayoutPanel3.Controls.Add(this.numericUpDownChargingDGI, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.label16, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label17, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.numericUpDownChargingTotal, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.numericUpDownChargingTenancy, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label15, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label22, 3, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(496, 44);
            this.tableLayoutPanel3.TabIndex = 69;
            // 
            // numericUpDownChargingPenalties
            // 
            this.numericUpDownChargingPenalties.DecimalPlaces = 2;
            this.numericUpDownChargingPenalties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDownChargingPenalties.Location = new System.Drawing.Point(373, 16);
            this.numericUpDownChargingPenalties.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownChargingPenalties.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownChargingPenalties.Name = "numericUpDownChargingPenalties";
            this.numericUpDownChargingPenalties.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownChargingPenalties.TabIndex = 3;
            this.numericUpDownChargingPenalties.ThousandsSeparator = true;
            // 
            // numericUpDownChargingDGI
            // 
            this.numericUpDownChargingDGI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownChargingDGI.DecimalPlaces = 2;
            this.numericUpDownChargingDGI.Location = new System.Drawing.Point(250, 16);
            this.numericUpDownChargingDGI.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownChargingDGI.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownChargingDGI.Name = "numericUpDownChargingDGI";
            this.numericUpDownChargingDGI.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownChargingDGI.TabIndex = 2;
            this.numericUpDownChargingDGI.ThousandsSeparator = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 2);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(99, 12);
            this.label16.TabIndex = 79;
            this.label16.Text = "Начисление итого";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(250, 2);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(94, 12);
            this.label17.TabIndex = 83;
            this.label17.Text = "Начисление ДГИ";
            // 
            // numericUpDownChargingTotal
            // 
            this.numericUpDownChargingTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownChargingTotal.DecimalPlaces = 2;
            this.numericUpDownChargingTotal.Location = new System.Drawing.Point(4, 16);
            this.numericUpDownChargingTotal.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownChargingTotal.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownChargingTotal.Name = "numericUpDownChargingTotal";
            this.numericUpDownChargingTotal.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownChargingTotal.TabIndex = 0;
            this.numericUpDownChargingTotal.ThousandsSeparator = true;
            // 
            // numericUpDownChargingTenancy
            // 
            this.numericUpDownChargingTenancy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownChargingTenancy.DecimalPlaces = 2;
            this.numericUpDownChargingTenancy.Location = new System.Drawing.Point(127, 16);
            this.numericUpDownChargingTenancy.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownChargingTenancy.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownChargingTenancy.Name = "numericUpDownChargingTenancy";
            this.numericUpDownChargingTenancy.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownChargingTenancy.TabIndex = 1;
            this.numericUpDownChargingTenancy.ThousandsSeparator = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(127, 2);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(97, 12);
            this.label15.TabIndex = 81;
            this.label15.Text = "Начисление найм";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(373, 2);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(33, 12);
            this.label22.TabIndex = 84;
            this.label22.Text = "Пени";
            // 
            // groupBoxRecalcs
            // 
            this.groupBoxRecalcs.Controls.Add(this.tableLayoutPanel4);
            this.groupBoxRecalcs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRecalcs.Location = new System.Drawing.Point(506, 132);
            this.groupBoxRecalcs.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxRecalcs.Name = "groupBoxRecalcs";
            this.groupBoxRecalcs.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxRecalcs.Size = new System.Drawing.Size(500, 61);
            this.groupBoxRecalcs.TabIndex = 3;
            this.groupBoxRecalcs.TabStop = false;
            this.groupBoxRecalcs.Text = "Разовый перерасчет и перенос сальдо (последний месяц)";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.Controls.Add(this.numericUpDownRecalcPenalties, 2, 1);
            this.tableLayoutPanel4.Controls.Add(this.numericUpDownTransferBalance, 3, 1);
            this.tableLayoutPanel4.Controls.Add(this.numericUpDownRecalcDGI, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label13, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.numericUpDownRecalcTenancy, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.label14, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label12, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.label23, 2, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(496, 44);
            this.tableLayoutPanel4.TabIndex = 70;
            // 
            // numericUpDownRecalcPenalties
            // 
            this.numericUpDownRecalcPenalties.DecimalPlaces = 2;
            this.numericUpDownRecalcPenalties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDownRecalcPenalties.Location = new System.Drawing.Point(250, 16);
            this.numericUpDownRecalcPenalties.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownRecalcPenalties.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownRecalcPenalties.Name = "numericUpDownRecalcPenalties";
            this.numericUpDownRecalcPenalties.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownRecalcPenalties.TabIndex = 2;
            this.numericUpDownRecalcPenalties.ThousandsSeparator = true;
            // 
            // numericUpDownTransferBalance
            // 
            this.numericUpDownTransferBalance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownTransferBalance.DecimalPlaces = 2;
            this.numericUpDownTransferBalance.Location = new System.Drawing.Point(373, 16);
            this.numericUpDownTransferBalance.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownTransferBalance.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownTransferBalance.Name = "numericUpDownTransferBalance";
            this.numericUpDownTransferBalance.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownTransferBalance.TabIndex = 3;
            this.numericUpDownTransferBalance.ThousandsSeparator = true;
            // 
            // numericUpDownRecalcDGI
            // 
            this.numericUpDownRecalcDGI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownRecalcDGI.DecimalPlaces = 2;
            this.numericUpDownRecalcDGI.Location = new System.Drawing.Point(127, 16);
            this.numericUpDownRecalcDGI.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownRecalcDGI.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownRecalcDGI.Name = "numericUpDownRecalcDGI";
            this.numericUpDownRecalcDGI.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownRecalcDGI.TabIndex = 1;
            this.numericUpDownRecalcDGI.ThousandsSeparator = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 2);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(96, 12);
            this.label13.TabIndex = 77;
            this.label13.Text = "Перерасчет найм";
            // 
            // numericUpDownRecalcTenancy
            // 
            this.numericUpDownRecalcTenancy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownRecalcTenancy.DecimalPlaces = 2;
            this.numericUpDownRecalcTenancy.Location = new System.Drawing.Point(4, 16);
            this.numericUpDownRecalcTenancy.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownRecalcTenancy.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownRecalcTenancy.Name = "numericUpDownRecalcTenancy";
            this.numericUpDownRecalcTenancy.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownRecalcTenancy.TabIndex = 0;
            this.numericUpDownRecalcTenancy.ThousandsSeparator = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(127, 2);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(93, 12);
            this.label14.TabIndex = 79;
            this.label14.Text = "Перерасчет ДГИ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(373, 2);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(90, 12);
            this.label12.TabIndex = 75;
            this.label12.Text = "Перенос сальдо";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(250, 2);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(100, 12);
            this.label23.TabIndex = 85;
            this.label23.Text = "Пени (перерасчет)";
            // 
            // groupBoxBalanceOutput
            // 
            this.groupBoxBalanceOutput.Controls.Add(this.tableLayoutPanel6);
            this.groupBoxBalanceOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxBalanceOutput.Location = new System.Drawing.Point(506, 262);
            this.groupBoxBalanceOutput.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxBalanceOutput.Name = "groupBoxBalanceOutput";
            this.groupBoxBalanceOutput.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxBalanceOutput.Size = new System.Drawing.Size(500, 61);
            this.groupBoxBalanceOutput.TabIndex = 5;
            this.groupBoxBalanceOutput.TabStop = false;
            this.groupBoxBalanceOutput.Text = "Исходящее сальдо (последний месяц)";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 4;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel6.Controls.Add(this.numericUpDownPenaltiesOutput, 3, 1);
            this.tableLayoutPanel6.Controls.Add(this.numericUpDownBalanceDGIOutput, 2, 1);
            this.tableLayoutPanel6.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.label9, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.numericUpDownBalanceTotalOutput, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.numericUpDownBalanceTenancyOutput, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.label10, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.label25, 3, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(496, 44);
            this.tableLayoutPanel6.TabIndex = 72;
            // 
            // numericUpDownPenaltiesOutput
            // 
            this.numericUpDownPenaltiesOutput.DecimalPlaces = 2;
            this.numericUpDownPenaltiesOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDownPenaltiesOutput.Location = new System.Drawing.Point(373, 16);
            this.numericUpDownPenaltiesOutput.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownPenaltiesOutput.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownPenaltiesOutput.Name = "numericUpDownPenaltiesOutput";
            this.numericUpDownPenaltiesOutput.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownPenaltiesOutput.TabIndex = 3;
            this.numericUpDownPenaltiesOutput.ThousandsSeparator = true;
            // 
            // numericUpDownBalanceDGIOutput
            // 
            this.numericUpDownBalanceDGIOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownBalanceDGIOutput.DecimalPlaces = 2;
            this.numericUpDownBalanceDGIOutput.Location = new System.Drawing.Point(250, 16);
            this.numericUpDownBalanceDGIOutput.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownBalanceDGIOutput.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceDGIOutput.Name = "numericUpDownBalanceDGIOutput";
            this.numericUpDownBalanceDGIOutput.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownBalanceDGIOutput.TabIndex = 2;
            this.numericUpDownBalanceDGIOutput.ThousandsSeparator = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 2);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(103, 12);
            this.label11.TabIndex = 69;
            this.label11.Text = "Сальдо исходящее";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(250, 2);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 12);
            this.label9.TabIndex = 73;
            this.label9.Text = "Сальдо ДГИ";
            // 
            // numericUpDownBalanceTotalOutput
            // 
            this.numericUpDownBalanceTotalOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownBalanceTotalOutput.DecimalPlaces = 2;
            this.numericUpDownBalanceTotalOutput.Location = new System.Drawing.Point(4, 16);
            this.numericUpDownBalanceTotalOutput.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownBalanceTotalOutput.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceTotalOutput.Name = "numericUpDownBalanceTotalOutput";
            this.numericUpDownBalanceTotalOutput.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownBalanceTotalOutput.TabIndex = 0;
            this.numericUpDownBalanceTotalOutput.ThousandsSeparator = true;
            // 
            // numericUpDownBalanceTenancyOutput
            // 
            this.numericUpDownBalanceTenancyOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownBalanceTenancyOutput.DecimalPlaces = 2;
            this.numericUpDownBalanceTenancyOutput.Location = new System.Drawing.Point(127, 16);
            this.numericUpDownBalanceTenancyOutput.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownBalanceTenancyOutput.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceTenancyOutput.Name = "numericUpDownBalanceTenancyOutput";
            this.numericUpDownBalanceTenancyOutput.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownBalanceTenancyOutput.TabIndex = 1;
            this.numericUpDownBalanceTenancyOutput.ThousandsSeparator = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(127, 2);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 12);
            this.label10.TabIndex = 71;
            this.label10.Text = "Сальдо найм";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(373, 2);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(33, 12);
            this.label25.TabIndex = 81;
            this.label25.Text = "Пени";
            // 
            // groupBoxBalanceInput
            // 
            this.groupBoxBalanceInput.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxBalanceInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxBalanceInput.Location = new System.Drawing.Point(506, 2);
            this.groupBoxBalanceInput.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxBalanceInput.Name = "groupBoxBalanceInput";
            this.groupBoxBalanceInput.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxBalanceInput.Size = new System.Drawing.Size(500, 61);
            this.groupBoxBalanceInput.TabIndex = 1;
            this.groupBoxBalanceInput.TabStop = false;
            this.groupBoxBalanceInput.Text = "Входящее сальдо (последний месяц)";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.numericUpDownPenaltiesInput, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.numericUpDownBalanceDGIInput, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.numericUpDownBalanceTotalInput, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.numericUpDownBalanceTenancyInput, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label8, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label7, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label21, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(496, 44);
            this.tableLayoutPanel2.TabIndex = 68;
            // 
            // numericUpDownPenaltiesInput
            // 
            this.numericUpDownPenaltiesInput.DecimalPlaces = 2;
            this.numericUpDownPenaltiesInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDownPenaltiesInput.Location = new System.Drawing.Point(373, 16);
            this.numericUpDownPenaltiesInput.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownPenaltiesInput.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownPenaltiesInput.Name = "numericUpDownPenaltiesInput";
            this.numericUpDownPenaltiesInput.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownPenaltiesInput.TabIndex = 3;
            this.numericUpDownPenaltiesInput.ThousandsSeparator = true;
            // 
            // numericUpDownBalanceDGIInput
            // 
            this.numericUpDownBalanceDGIInput.DecimalPlaces = 2;
            this.numericUpDownBalanceDGIInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDownBalanceDGIInput.Location = new System.Drawing.Point(250, 16);
            this.numericUpDownBalanceDGIInput.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownBalanceDGIInput.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceDGIInput.Name = "numericUpDownBalanceDGIInput";
            this.numericUpDownBalanceDGIInput.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownBalanceDGIInput.TabIndex = 2;
            this.numericUpDownBalanceDGIInput.ThousandsSeparator = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 2);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 12);
            this.label6.TabIndex = 63;
            this.label6.Text = "Сальдо входящее";
            // 
            // numericUpDownBalanceTotalInput
            // 
            this.numericUpDownBalanceTotalInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownBalanceTotalInput.DecimalPlaces = 2;
            this.numericUpDownBalanceTotalInput.Location = new System.Drawing.Point(4, 16);
            this.numericUpDownBalanceTotalInput.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownBalanceTotalInput.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceTotalInput.Name = "numericUpDownBalanceTotalInput";
            this.numericUpDownBalanceTotalInput.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownBalanceTotalInput.TabIndex = 0;
            this.numericUpDownBalanceTotalInput.ThousandsSeparator = true;
            // 
            // numericUpDownBalanceTenancyInput
            // 
            this.numericUpDownBalanceTenancyInput.DecimalPlaces = 2;
            this.numericUpDownBalanceTenancyInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDownBalanceTenancyInput.Location = new System.Drawing.Point(127, 16);
            this.numericUpDownBalanceTenancyInput.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownBalanceTenancyInput.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceTenancyInput.Name = "numericUpDownBalanceTenancyInput";
            this.numericUpDownBalanceTenancyInput.Size = new System.Drawing.Size(119, 20);
            this.numericUpDownBalanceTenancyInput.TabIndex = 1;
            this.numericUpDownBalanceTenancyInput.ThousandsSeparator = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(250, 2);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 12);
            this.label8.TabIndex = 67;
            this.label8.Text = "Сальдо ДГИ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(127, 2);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 12);
            this.label7.TabIndex = 65;
            this.label7.Text = "Сальдо найм";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(373, 2);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(33, 12);
            this.label21.TabIndex = 68;
            this.label21.Text = "Пени";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.date,
            this.crn,
            this.raw_address,
            this.parsed_address,
            this.account,
            this.tenant,
            this.total_area,
            this.living_area,
            this.prescribed,
            this.balance_input,
            this.balance_tenancy,
            this.balance_dgi,
            this.balance_input_penalties,
            this.charging_tenancy,
            this.charging_dgi,
            this.charging_total,
            this.charging_penalties,
            this.recalc_tenancy,
            this.recalc_dgi,
            this.recalc_penalties,
            this.payment_tenancy,
            this.payment_dgi,
            this.payment_penalties,
            this.transfer_balance,
            this.balance_output_total,
            this.balance_output_tenancy,
            this.balance_output_dgi,
            this.balance_output_penalties});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(2, 327);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(1004, 401);
            this.dataGridView.TabIndex = 6;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // date
            // 
            this.date.HeaderText = "Состояние на дату";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            // 
            // crn
            // 
            this.crn.HeaderText = "СРН";
            this.crn.Name = "crn";
            this.crn.ReadOnly = true;
            // 
            // raw_address
            // 
            this.raw_address.HeaderText = "Адрес по БКС";
            this.raw_address.MinimumWidth = 300;
            this.raw_address.Name = "raw_address";
            this.raw_address.ReadOnly = true;
            this.raw_address.Width = 300;
            // 
            // parsed_address
            // 
            this.parsed_address.HeaderText = "Адрес в реестре ЖФ";
            this.parsed_address.MinimumWidth = 350;
            this.parsed_address.Name = "parsed_address";
            this.parsed_address.ReadOnly = true;
            this.parsed_address.Width = 350;
            // 
            // account
            // 
            this.account.HeaderText = "Лицевой счет";
            this.account.MinimumWidth = 150;
            this.account.Name = "account";
            this.account.ReadOnly = true;
            this.account.Width = 150;
            // 
            // tenant
            // 
            this.tenant.HeaderText = "Наниматель";
            this.tenant.MinimumWidth = 150;
            this.tenant.Name = "tenant";
            this.tenant.ReadOnly = true;
            this.tenant.Width = 150;
            // 
            // total_area
            // 
            this.total_area.HeaderText = "Общая площадь";
            this.total_area.Name = "total_area";
            this.total_area.ReadOnly = true;
            // 
            // living_area
            // 
            this.living_area.HeaderText = "Жилая площадь";
            this.living_area.Name = "living_area";
            this.living_area.ReadOnly = true;
            // 
            // prescribed
            // 
            this.prescribed.HeaderText = "Прописано";
            this.prescribed.Name = "prescribed";
            this.prescribed.ReadOnly = true;
            // 
            // balance_input
            // 
            this.balance_input.HeaderText = "Сальдо вх.";
            this.balance_input.Name = "balance_input";
            this.balance_input.ReadOnly = true;
            // 
            // balance_tenancy
            // 
            this.balance_tenancy.HeaderText = "Сальдо вх. найм";
            this.balance_tenancy.Name = "balance_tenancy";
            this.balance_tenancy.ReadOnly = true;
            // 
            // balance_dgi
            // 
            this.balance_dgi.HeaderText = "Сальдо вх. ДГИ";
            this.balance_dgi.Name = "balance_dgi";
            this.balance_dgi.ReadOnly = true;
            // 
            // balance_input_penalties
            // 
            this.balance_input_penalties.HeaderText = "Пени (вх.)";
            this.balance_input_penalties.Name = "balance_input_penalties";
            this.balance_input_penalties.ReadOnly = true;
            // 
            // charging_tenancy
            // 
            this.charging_tenancy.HeaderText = "Начислено найм";
            this.charging_tenancy.Name = "charging_tenancy";
            this.charging_tenancy.ReadOnly = true;
            // 
            // charging_dgi
            // 
            this.charging_dgi.HeaderText = "Начислено ДГИ";
            this.charging_dgi.Name = "charging_dgi";
            this.charging_dgi.ReadOnly = true;
            // 
            // charging_total
            // 
            this.charging_total.HeaderText = "Начислено итого";
            this.charging_total.Name = "charging_total";
            this.charging_total.ReadOnly = true;
            // 
            // charging_penalties
            // 
            this.charging_penalties.HeaderText = "Пени (начислено)";
            this.charging_penalties.Name = "charging_penalties";
            this.charging_penalties.ReadOnly = true;
            // 
            // recalc_tenancy
            // 
            this.recalc_tenancy.HeaderText = "Перерасчет найм";
            this.recalc_tenancy.Name = "recalc_tenancy";
            this.recalc_tenancy.ReadOnly = true;
            // 
            // recalc_dgi
            // 
            this.recalc_dgi.HeaderText = "Перерасчет ДГИ";
            this.recalc_dgi.Name = "recalc_dgi";
            this.recalc_dgi.ReadOnly = true;
            // 
            // recalc_penalties
            // 
            this.recalc_penalties.HeaderText = "Пени (перерасчет)";
            this.recalc_penalties.Name = "recalc_penalties";
            this.recalc_penalties.ReadOnly = true;
            // 
            // payment_tenancy
            // 
            this.payment_tenancy.HeaderText = "Оплата найм";
            this.payment_tenancy.Name = "payment_tenancy";
            this.payment_tenancy.ReadOnly = true;
            // 
            // payment_dgi
            // 
            this.payment_dgi.HeaderText = "Оплата ДГИ";
            this.payment_dgi.Name = "payment_dgi";
            this.payment_dgi.ReadOnly = true;
            // 
            // payment_penalties
            // 
            this.payment_penalties.HeaderText = "Пени (оплата)";
            this.payment_penalties.Name = "payment_penalties";
            this.payment_penalties.ReadOnly = true;
            // 
            // transfer_balance
            // 
            this.transfer_balance.HeaderText = "Перенос сальдо";
            this.transfer_balance.Name = "transfer_balance";
            this.transfer_balance.ReadOnly = true;
            // 
            // balance_output_total
            // 
            this.balance_output_total.HeaderText = "Сальдо исх.";
            this.balance_output_total.Name = "balance_output_total";
            this.balance_output_total.ReadOnly = true;
            // 
            // balance_output_tenancy
            // 
            this.balance_output_tenancy.HeaderText = "Сальдо исх. найм";
            this.balance_output_tenancy.Name = "balance_output_tenancy";
            this.balance_output_tenancy.ReadOnly = true;
            // 
            // balance_output_dgi
            // 
            this.balance_output_dgi.HeaderText = "Сальдо исх. ДГИ";
            this.balance_output_dgi.Name = "balance_output_dgi";
            this.balance_output_dgi.ReadOnly = true;
            // 
            // balance_output_penalties
            // 
            this.balance_output_penalties.HeaderText = "Пени (исх.)";
            this.balance_output_penalties.Name = "balance_output_penalties";
            this.balance_output_penalties.ReadOnly = true;
            // 
            // PaymentsAccountsViewport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(860, 600);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PaymentsAccountsViewport";
            this.Text = "Лицевые счета";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBoxGeneralInfo.ResumeLayout(false);
            this.groupBoxGeneralInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrescribed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentPenalties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentDGI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentTenancy)).EndInit();
            this.groupBoxChargings.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingPenalties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingDGI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingTenancy)).EndInit();
            this.groupBoxRecalcs.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcPenalties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferBalance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcDGI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcTenancy)).EndInit();
            this.groupBoxBalanceOutput.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPenaltiesOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceDGIOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceTotalOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceTenancyOutput)).EndInit();
            this.groupBoxBalanceInput.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPenaltiesInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceDGIInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceTotalInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceTenancyInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBoxRecalcs;
        private System.Windows.Forms.GroupBox groupBoxBalanceOutput;
        private System.Windows.Forms.GroupBox groupBoxBalanceInput;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.GroupBox groupBoxChargings;
        private FixedNumericUpDown numericUpDownBalanceDGIInput;
        private System.Windows.Forms.Label label8;
        private FixedNumericUpDown numericUpDownBalanceTenancyInput;
        private System.Windows.Forms.Label label7;
        private FixedNumericUpDown numericUpDownBalanceTotalInput;
        private System.Windows.Forms.Label label6;
        private FixedNumericUpDown numericUpDownBalanceDGIOutput;
        private System.Windows.Forms.Label label9;
        private FixedNumericUpDown numericUpDownBalanceTenancyOutput;
        private System.Windows.Forms.Label label10;
        private FixedNumericUpDown numericUpDownBalanceTotalOutput;
        private System.Windows.Forms.Label label11;
        private FixedNumericUpDown numericUpDownRecalcDGI;
        private System.Windows.Forms.Label label14;
        private FixedNumericUpDown numericUpDownRecalcTenancy;
        private System.Windows.Forms.Label label13;
        private FixedNumericUpDown numericUpDownTransferBalance;
        private System.Windows.Forms.Label label12;
        private FixedNumericUpDown numericUpDownChargingDGI;
        private System.Windows.Forms.Label label17;
        private FixedNumericUpDown numericUpDownChargingTenancy;
        private System.Windows.Forms.Label label15;
        private FixedNumericUpDown numericUpDownChargingTotal;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox1;
        private FixedNumericUpDown numericUpDownPaymentDGI;
        private System.Windows.Forms.Label label19;
        private FixedNumericUpDown numericUpDownPaymentTenancy;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private FixedNumericUpDown numericUpDownPaymentPenalties;
        private System.Windows.Forms.Label label24;
        private FixedNumericUpDown numericUpDownChargingPenalties;
        private System.Windows.Forms.Label label22;
        private FixedNumericUpDown numericUpDownRecalcPenalties;
        private System.Windows.Forms.Label label23;
        private FixedNumericUpDown numericUpDownPenaltiesInput;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.GroupBox groupBoxGeneralInfo;
        private System.Windows.Forms.TextBox textBoxTenant;
        private System.Windows.Forms.Label label18;
        private FixedNumericUpDown numericUpDownPrescribed;
        private System.Windows.Forms.Label label5;
        private FixedNumericUpDown numericUpDownLivingArea;
        private System.Windows.Forms.Label label4;
        private FixedNumericUpDown numericUpDownTotalArea;
        private System.Windows.Forms.Label label92;
        private System.Windows.Forms.DateTimePicker dateTimePickerAtDate;
        private System.Windows.Forms.Label label91;
        private System.Windows.Forms.TextBox textBoxAccount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxRawAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCRN;
        private System.Windows.Forms.Label label99;
        private FixedNumericUpDown numericUpDownPenaltiesOutput;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn crn;
        private System.Windows.Forms.DataGridViewTextBoxColumn raw_address;
        private System.Windows.Forms.DataGridViewTextBoxColumn parsed_address;
        private System.Windows.Forms.DataGridViewTextBoxColumn account;
        private System.Windows.Forms.DataGridViewTextBoxColumn tenant;
        private System.Windows.Forms.DataGridViewTextBoxColumn total_area;
        private System.Windows.Forms.DataGridViewTextBoxColumn living_area;
        private System.Windows.Forms.DataGridViewTextBoxColumn prescribed;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_input;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_tenancy;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_dgi;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_input_penalties;
        private System.Windows.Forms.DataGridViewTextBoxColumn charging_tenancy;
        private System.Windows.Forms.DataGridViewTextBoxColumn charging_dgi;
        private System.Windows.Forms.DataGridViewTextBoxColumn charging_total;
        private System.Windows.Forms.DataGridViewTextBoxColumn charging_penalties;
        private System.Windows.Forms.DataGridViewTextBoxColumn recalc_tenancy;
        private System.Windows.Forms.DataGridViewTextBoxColumn recalc_dgi;
        private System.Windows.Forms.DataGridViewTextBoxColumn recalc_penalties;
        private System.Windows.Forms.DataGridViewTextBoxColumn payment_tenancy;
        private System.Windows.Forms.DataGridViewTextBoxColumn payment_dgi;
        private System.Windows.Forms.DataGridViewTextBoxColumn payment_penalties;
        private System.Windows.Forms.DataGridViewTextBoxColumn transfer_balance;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_output_total;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_output_tenancy;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_output_dgi;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_output_penalties;
    }
}