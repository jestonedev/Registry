namespace Registry.Viewport.ModalEditors
{
    partial class MultiClaimsStateConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiClaimsStateConfigForm));
            this.vButtonOk = new VIBlend.WinForms.Controls.vButton();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanelDetails = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlWithoutTabs1 = new CustomControls.TabControlWithoutTabs();
            this.tabPageToLegalDepartment = new System.Windows.Forms.TabPage();
            this.groupBoxTransfertToLegalDepartment = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTransferToLegalDepartmentWho = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dateTimePickerTransfertToLegalDepartmentDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageAcceptedByLegalDepartment = new System.Windows.Forms.TabPage();
            this.groupBoxAcceptedByLegalDepartment = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxAcceptedByLegalDepartmentWho = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.dateTimePickerAcceptedByLegalDepartmentDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.dateTimePickerStartState = new System.Windows.Forms.DateTimePicker();
            this.label108 = new System.Windows.Forms.Label();
            this.panel31 = new System.Windows.Forms.Panel();
            this.label30 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.comboBoxClaimStateType = new System.Windows.Forms.ComboBox();
            this.label109 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tableLayoutPanelDetails.SuspendLayout();
            this.tabControlWithoutTabs1.SuspendLayout();
            this.tabPageToLegalDepartment.SuspendLayout();
            this.groupBoxTransfertToLegalDepartment.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPageAcceptedByLegalDepartment.SuspendLayout();
            this.groupBoxAcceptedByLegalDepartment.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel31.SuspendLayout();
            this.SuspendLayout();
            // 
            // vButtonOk
            // 
            this.vButtonOk.AllowAnimations = true;
            this.vButtonOk.BackColor = System.Drawing.Color.Transparent;
            this.vButtonOk.Location = new System.Drawing.Point(326, 6);
            this.vButtonOk.Name = "vButtonOk";
            this.vButtonOk.RoundedCornersMask = ((byte)(15));
            this.vButtonOk.Size = new System.Drawing.Size(117, 35);
            this.vButtonOk.TabIndex = 2;
            this.vButtonOk.Text = "Сформировать";
            this.vButtonOk.UseVisualStyleBackColor = false;
            this.vButtonOk.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonOk.Click += new System.EventHandler(this.vButtonOk_Click);
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(449, 6);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 3;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.vButtonOk);
            this.panel1.Controls.Add(this.vButtonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 112);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(891, 48);
            this.panel1.TabIndex = 4;
            // 
            // tableLayoutPanelDetails
            // 
            this.tableLayoutPanelDetails.ColumnCount = 2;
            this.tableLayoutPanelDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDetails.Controls.Add(this.tabControlWithoutTabs1, 0, 1);
            this.tableLayoutPanelDetails.Controls.Add(this.panel11, 1, 0);
            this.tableLayoutPanelDetails.Controls.Add(this.panel31, 0, 0);
            this.tableLayoutPanelDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDetails.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelDetails.Name = "tableLayoutPanelDetails";
            this.tableLayoutPanelDetails.RowCount = 2;
            this.tableLayoutPanelDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanelDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDetails.Size = new System.Drawing.Size(891, 112);
            this.tableLayoutPanelDetails.TabIndex = 5;
            // 
            // tabControlWithoutTabs1
            // 
            this.tableLayoutPanelDetails.SetColumnSpan(this.tabControlWithoutTabs1, 2);
            this.tabControlWithoutTabs1.Controls.Add(this.tabPageToLegalDepartment);
            this.tabControlWithoutTabs1.Controls.Add(this.tabPageAcceptedByLegalDepartment);
            this.tabControlWithoutTabs1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlWithoutTabs1.Location = new System.Drawing.Point(0, 60);
            this.tabControlWithoutTabs1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlWithoutTabs1.Name = "tabControlWithoutTabs1";
            this.tabControlWithoutTabs1.SelectedIndex = 0;
            this.tabControlWithoutTabs1.Size = new System.Drawing.Size(891, 52);
            this.tabControlWithoutTabs1.TabIndex = 2;
            // 
            // tabPageToLegalDepartment
            // 
            this.tabPageToLegalDepartment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.tabPageToLegalDepartment.Controls.Add(this.groupBoxTransfertToLegalDepartment);
            this.tabPageToLegalDepartment.Location = new System.Drawing.Point(4, 22);
            this.tabPageToLegalDepartment.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageToLegalDepartment.Name = "tabPageToLegalDepartment";
            this.tabPageToLegalDepartment.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageToLegalDepartment.Size = new System.Drawing.Size(883, 26);
            this.tabPageToLegalDepartment.TabIndex = 0;
            this.tabPageToLegalDepartment.Text = "Передача дела в юр. отдел";
            // 
            // groupBoxTransfertToLegalDepartment
            // 
            this.groupBoxTransfertToLegalDepartment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.groupBoxTransfertToLegalDepartment.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxTransfertToLegalDepartment.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxTransfertToLegalDepartment.Location = new System.Drawing.Point(2, 2);
            this.groupBoxTransfertToLegalDepartment.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxTransfertToLegalDepartment.Name = "groupBoxTransfertToLegalDepartment";
            this.groupBoxTransfertToLegalDepartment.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxTransfertToLegalDepartment.Size = new System.Drawing.Size(879, 47);
            this.groupBoxTransfertToLegalDepartment.TabIndex = 0;
            this.groupBoxTransfertToLegalDepartment.TabStop = false;
            this.groupBoxTransfertToLegalDepartment.Text = "Передано в юр. отдел";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(875, 30);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textBoxTransferToLegalDepartmentWho);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(437, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(438, 30);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Кто передал";
            // 
            // textBoxTransferToLegalDepartmentWho
            // 
            this.textBoxTransferToLegalDepartmentWho.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTransferToLegalDepartmentWho.Location = new System.Drawing.Point(161, 5);
            this.textBoxTransferToLegalDepartmentWho.Name = "textBoxTransferToLegalDepartmentWho";
            this.textBoxTransferToLegalDepartmentWho.Size = new System.Drawing.Size(273, 20);
            this.textBoxTransferToLegalDepartmentWho.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dateTimePickerTransfertToLegalDepartmentDate);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(437, 30);
            this.panel3.TabIndex = 0;
            // 
            // dateTimePickerTransfertToLegalDepartmentDate
            // 
            this.dateTimePickerTransfertToLegalDepartmentDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerTransfertToLegalDepartmentDate.Location = new System.Drawing.Point(155, 5);
            this.dateTimePickerTransfertToLegalDepartmentDate.Name = "dateTimePickerTransfertToLegalDepartmentDate";
            this.dateTimePickerTransfertToLegalDepartmentDate.ShowCheckBox = true;
            this.dateTimePickerTransfertToLegalDepartmentDate.Size = new System.Drawing.Size(272, 20);
            this.dateTimePickerTransfertToLegalDepartmentDate.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Дата передачи";
            // 
            // tabPageAcceptedByLegalDepartment
            // 
            this.tabPageAcceptedByLegalDepartment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.tabPageAcceptedByLegalDepartment.Controls.Add(this.groupBoxAcceptedByLegalDepartment);
            this.tabPageAcceptedByLegalDepartment.Location = new System.Drawing.Point(4, 22);
            this.tabPageAcceptedByLegalDepartment.Name = "tabPageAcceptedByLegalDepartment";
            this.tabPageAcceptedByLegalDepartment.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageAcceptedByLegalDepartment.Size = new System.Drawing.Size(883, 49);
            this.tabPageAcceptedByLegalDepartment.TabIndex = 4;
            this.tabPageAcceptedByLegalDepartment.Text = "Принято в юр. отдел";
            // 
            // groupBoxAcceptedByLegalDepartment
            // 
            this.groupBoxAcceptedByLegalDepartment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.groupBoxAcceptedByLegalDepartment.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxAcceptedByLegalDepartment.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxAcceptedByLegalDepartment.Location = new System.Drawing.Point(2, 2);
            this.groupBoxAcceptedByLegalDepartment.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxAcceptedByLegalDepartment.Name = "groupBoxAcceptedByLegalDepartment";
            this.groupBoxAcceptedByLegalDepartment.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxAcceptedByLegalDepartment.Size = new System.Drawing.Size(879, 47);
            this.groupBoxAcceptedByLegalDepartment.TabIndex = 2;
            this.groupBoxAcceptedByLegalDepartment.TabStop = false;
            this.groupBoxAcceptedByLegalDepartment.Text = "Принято в юр. отдел";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel4, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(875, 30);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.textBoxAcceptedByLegalDepartmentWho);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(437, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(438, 30);
            this.panel4.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Кто принял";
            // 
            // textBoxAcceptedByLegalDepartmentWho
            // 
            this.textBoxAcceptedByLegalDepartmentWho.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAcceptedByLegalDepartmentWho.Location = new System.Drawing.Point(161, 5);
            this.textBoxAcceptedByLegalDepartmentWho.Name = "textBoxAcceptedByLegalDepartmentWho";
            this.textBoxAcceptedByLegalDepartmentWho.Size = new System.Drawing.Size(273, 20);
            this.textBoxAcceptedByLegalDepartmentWho.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.dateTimePickerAcceptedByLegalDepartmentDate);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(437, 30);
            this.panel5.TabIndex = 0;
            // 
            // dateTimePickerAcceptedByLegalDepartmentDate
            // 
            this.dateTimePickerAcceptedByLegalDepartmentDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAcceptedByLegalDepartmentDate.Location = new System.Drawing.Point(155, 5);
            this.dateTimePickerAcceptedByLegalDepartmentDate.Name = "dateTimePickerAcceptedByLegalDepartmentDate";
            this.dateTimePickerAcceptedByLegalDepartmentDate.ShowCheckBox = true;
            this.dateTimePickerAcceptedByLegalDepartmentDate.Size = new System.Drawing.Size(272, 20);
            this.dateTimePickerAcceptedByLegalDepartmentDate.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Дата принятия";
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.dateTimePickerStartState);
            this.panel11.Controls.Add(this.label108);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel11.Location = new System.Drawing.Point(445, 0);
            this.panel11.Margin = new System.Windows.Forms.Padding(0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(446, 60);
            this.panel11.TabIndex = 1;
            // 
            // dateTimePickerStartState
            // 
            this.dateTimePickerStartState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartState.Location = new System.Drawing.Point(161, 6);
            this.dateTimePickerStartState.Name = "dateTimePickerStartState";
            this.dateTimePickerStartState.Size = new System.Drawing.Size(275, 20);
            this.dateTimePickerStartState.TabIndex = 0;
            // 
            // label108
            // 
            this.label108.AutoSize = true;
            this.label108.Location = new System.Drawing.Point(14, 9);
            this.label108.Name = "label108";
            this.label108.Size = new System.Drawing.Size(88, 13);
            this.label108.TabIndex = 31;
            this.label108.Text = "Дата установки";
            // 
            // panel31
            // 
            this.panel31.Controls.Add(this.label30);
            this.panel31.Controls.Add(this.textBoxDescription);
            this.panel31.Controls.Add(this.comboBoxClaimStateType);
            this.panel31.Controls.Add(this.label109);
            this.panel31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel31.Location = new System.Drawing.Point(0, 0);
            this.panel31.Margin = new System.Windows.Forms.Padding(0);
            this.panel31.Name = "panel31";
            this.panel31.Size = new System.Drawing.Size(445, 60);
            this.panel31.TabIndex = 0;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(14, 36);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(70, 13);
            this.label30.TabIndex = 34;
            this.label30.Text = "Примечание";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(161, 33);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(274, 20);
            this.textBoxDescription.TabIndex = 1;
            // 
            // comboBoxClaimStateType
            // 
            this.comboBoxClaimStateType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxClaimStateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxClaimStateType.FormattingEnabled = true;
            this.comboBoxClaimStateType.Location = new System.Drawing.Point(161, 4);
            this.comboBoxClaimStateType.Name = "comboBoxClaimStateType";
            this.comboBoxClaimStateType.Size = new System.Drawing.Size(275, 21);
            this.comboBoxClaimStateType.TabIndex = 0;
            this.comboBoxClaimStateType.SelectedValueChanged += new System.EventHandler(this.comboBoxClaimStateType_SelectedValueChanged);
            // 
            // label109
            // 
            this.label109.AutoSize = true;
            this.label109.Location = new System.Drawing.Point(14, 7);
            this.label109.Name = "label109";
            this.label109.Size = new System.Drawing.Size(82, 13);
            this.label109.TabIndex = 29;
            this.label109.Text = "Вид состояния";
            // 
            // MultiClaimsStateConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(891, 160);
            this.Controls.Add(this.tableLayoutPanelDetails);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MultiClaimsStateConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Укажите стадию";
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanelDetails.ResumeLayout(false);
            this.tabControlWithoutTabs1.ResumeLayout(false);
            this.tabPageToLegalDepartment.ResumeLayout(false);
            this.groupBoxTransfertToLegalDepartment.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabPageAcceptedByLegalDepartment.ResumeLayout(false);
            this.groupBoxAcceptedByLegalDepartment.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.panel31.ResumeLayout(false);
            this.panel31.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButtonOk;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDetails;
        private CustomControls.TabControlWithoutTabs tabControlWithoutTabs1;
        private System.Windows.Forms.TabPage tabPageToLegalDepartment;
        private System.Windows.Forms.GroupBox groupBoxTransfertToLegalDepartment;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTransferToLegalDepartmentWho;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DateTimePicker dateTimePickerTransfertToLegalDepartmentDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageAcceptedByLegalDepartment;
        private System.Windows.Forms.GroupBox groupBoxAcceptedByLegalDepartment;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAcceptedByLegalDepartmentWho;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.DateTimePicker dateTimePickerAcceptedByLegalDepartmentDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.DateTimePicker dateTimePickerStartState;
        private System.Windows.Forms.Label label108;
        private System.Windows.Forms.Panel panel31;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.ComboBox comboBoxClaimStateType;
        private System.Windows.Forms.Label label109;
    }
}