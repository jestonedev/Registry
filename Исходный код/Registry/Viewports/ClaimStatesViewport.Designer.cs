using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class ClaimStatesViewport
    {
        #region Components
        TableLayoutPanel tableLayoutPanelAll;
        TableLayoutPanel tableLayoutPanelDetails;
        GroupBox groupBox35;
        Panel panel10;
        Panel panel11;
        Label label108;
        Label label109;
        ComboBox comboBoxClaimStateType;
        DateTimePicker dateTimePickerStartState;
        DataGridView dataGridView;
        #endregion Components

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimStatesViewport));
            this.tableLayoutPanelAll = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox35 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelDetails = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlWithoutTabs1 = new CustomControls.TabControlWithoutTabs();
            this.tabPageToLegalDepartment = new System.Windows.Forms.TabPage();
            this.groupBoxTransfertToLegalDepartment = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTransferToLegalDepartmentWho = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dateTimePickerTransfertToLegalDepartmentDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPagePreparingOrder = new System.Windows.Forms.TabPage();
            this.groupBoxObtainingCourtOrder = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxObtainingCourtOrderDescription = new System.Windows.Forms.TextBox();
            this.panel12 = new System.Windows.Forms.Panel();
            this.dateTimePickerObtainingCourtOrderDate = new System.Windows.Forms.DateTimePicker();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBoxCourtOrder = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCourtOrderNum = new System.Windows.Forms.TextBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.dateTimePickerCourtOrderDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBoxClaimDirectedToCourt = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxClaimDirectionDescription = new System.Windows.Forms.TextBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.dateTimePickerClaimDirectionDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPageExecutoryProcess = new System.Windows.Forms.TabPage();
            this.groupBoxRepeatedEnforcementProceedingEnd = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.panel21 = new System.Windows.Forms.Panel();
            this.label19 = new System.Windows.Forms.Label();
            this.textBoxRepeatedEnforcementProceedingEndDescription = new System.Windows.Forms.TextBox();
            this.panel22 = new System.Windows.Forms.Panel();
            this.dateTimePickerRepeatedEnforcementProceedingEndDate = new System.Windows.Forms.DateTimePicker();
            this.label20 = new System.Windows.Forms.Label();
            this.groupBoxRepeatedEnforcementProceedingStart = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.panel23 = new System.Windows.Forms.Panel();
            this.label21 = new System.Windows.Forms.Label();
            this.textBoxRepeatedEnforcementProceedingStartDescription = new System.Windows.Forms.TextBox();
            this.panel24 = new System.Windows.Forms.Panel();
            this.dateTimePickerRepeatedEnforcementProceedingStartDate = new System.Windows.Forms.DateTimePicker();
            this.label22 = new System.Windows.Forms.Label();
            this.groupBoxRepeatedDirectionCourtOrderBailiffs = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.panel19 = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.textBoxRepeatedDirectionCourtOrderBailiffsDescription = new System.Windows.Forms.TextBox();
            this.panel20 = new System.Windows.Forms.Panel();
            this.dateTimePickerRepeatedDirectionCourtOrderBailiffsDate = new System.Windows.Forms.DateTimePicker();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBoxEnforcementProceedingTerminate = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.panel25 = new System.Windows.Forms.Panel();
            this.label23 = new System.Windows.Forms.Label();
            this.textBoxEnforcementProceedingTerminateDescription = new System.Windows.Forms.TextBox();
            this.panel26 = new System.Windows.Forms.Panel();
            this.dateTimePickerEnforcementProceedingTerminateDate = new System.Windows.Forms.DateTimePicker();
            this.label24 = new System.Windows.Forms.Label();
            this.groupBoxEnforcementProceedingEnd = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.panel17 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxEnforcementProceedingEndDescription = new System.Windows.Forms.TextBox();
            this.panel18 = new System.Windows.Forms.Panel();
            this.dateTimePickerEnforcementProceedingEndDate = new System.Windows.Forms.DateTimePicker();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBoxEnforcementProceedingStart = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.panel15 = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxEnforcementProceedingStartDescription = new System.Windows.Forms.TextBox();
            this.panel16 = new System.Windows.Forms.Panel();
            this.dateTimePickerEnforcementProceedingStartDate = new System.Windows.Forms.DateTimePicker();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBoxDirectionCourtOrderBailiffs = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxDirectionCourtOrderBailiffsDescription = new System.Windows.Forms.TextBox();
            this.panel14 = new System.Windows.Forms.Panel();
            this.dateTimePickerDirectionCourtOrderBailiffsDate = new System.Windows.Forms.DateTimePicker();
            this.label12 = new System.Windows.Forms.Label();
            this.tabPageCompletionClaims = new System.Windows.Forms.TabPage();
            this.groupBoxClaimComplete = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.panel29 = new System.Windows.Forms.Panel();
            this.label27 = new System.Windows.Forms.Label();
            this.textBoxClaimCompleteDescription = new System.Windows.Forms.TextBox();
            this.panel30 = new System.Windows.Forms.Panel();
            this.label29 = new System.Windows.Forms.Label();
            this.textBoxClaimCompleteReason = new System.Windows.Forms.TextBox();
            this.dateTimePickerClaimCompleteDate = new System.Windows.Forms.DateTimePicker();
            this.label28 = new System.Windows.Forms.Label();
            this.groupBoxCourtOrderCancel = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel13 = new System.Windows.Forms.TableLayoutPanel();
            this.panel27 = new System.Windows.Forms.Panel();
            this.label25 = new System.Windows.Forms.Label();
            this.textBoxCourtOrderCancelDescription = new System.Windows.Forms.TextBox();
            this.panel28 = new System.Windows.Forms.Panel();
            this.dateTimePickerCourtOrderCancelDate = new System.Windows.Forms.DateTimePicker();
            this.label26 = new System.Windows.Forms.Label();
            this.tabPageAcceptedByLegalDepartment = new System.Windows.Forms.TabPage();
            this.panel10 = new System.Windows.Forms.Panel();
            this.dateTimePickerStartState = new System.Windows.Forms.DateTimePicker();
            this.label108 = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.label30 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.comboBoxClaimStateType = new System.Windows.Forms.ComboBox();
            this.label109 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_state_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.date_start_state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxAcceptedByLegalDepartment = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxAcceptedByLegalDepartmentWho = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dateTimePickerAcceptedByLegalDepartmentDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanelAll.SuspendLayout();
            this.groupBox35.SuspendLayout();
            this.tableLayoutPanelDetails.SuspendLayout();
            this.tabControlWithoutTabs1.SuspendLayout();
            this.tabPageToLegalDepartment.SuspendLayout();
            this.groupBoxTransfertToLegalDepartment.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPagePreparingOrder.SuspendLayout();
            this.groupBoxObtainingCourtOrder.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel12.SuspendLayout();
            this.groupBoxCourtOrder.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.groupBoxClaimDirectedToCourt.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.tabPageExecutoryProcess.SuspendLayout();
            this.groupBoxRepeatedEnforcementProceedingEnd.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.panel21.SuspendLayout();
            this.panel22.SuspendLayout();
            this.groupBoxRepeatedEnforcementProceedingStart.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.panel23.SuspendLayout();
            this.panel24.SuspendLayout();
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.panel19.SuspendLayout();
            this.panel20.SuspendLayout();
            this.groupBoxEnforcementProceedingTerminate.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.panel25.SuspendLayout();
            this.panel26.SuspendLayout();
            this.groupBoxEnforcementProceedingEnd.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.panel17.SuspendLayout();
            this.panel18.SuspendLayout();
            this.groupBoxEnforcementProceedingStart.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.panel15.SuspendLayout();
            this.panel16.SuspendLayout();
            this.groupBoxDirectionCourtOrderBailiffs.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel14.SuspendLayout();
            this.tabPageCompletionClaims.SuspendLayout();
            this.groupBoxClaimComplete.SuspendLayout();
            this.tableLayoutPanel14.SuspendLayout();
            this.panel29.SuspendLayout();
            this.panel30.SuspendLayout();
            this.groupBoxCourtOrderCancel.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            this.panel27.SuspendLayout();
            this.panel28.SuspendLayout();
            this.tabPageAcceptedByLegalDepartment.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.groupBoxAcceptedByLegalDepartment.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelAll
            // 
            this.tableLayoutPanelAll.ColumnCount = 1;
            this.tableLayoutPanelAll.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelAll.Controls.Add(this.groupBox35, 0, 1);
            this.tableLayoutPanelAll.Controls.Add(this.dataGridView, 0, 0);
            this.tableLayoutPanelAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelAll.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelAll.Name = "tableLayoutPanelAll";
            this.tableLayoutPanelAll.RowCount = 2;
            this.tableLayoutPanelAll.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelAll.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 412F));
            this.tableLayoutPanelAll.Size = new System.Drawing.Size(1002, 565);
            this.tableLayoutPanelAll.TabIndex = 1;
            // 
            // groupBox35
            // 
            this.groupBox35.Controls.Add(this.tableLayoutPanelDetails);
            this.groupBox35.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox35.Location = new System.Drawing.Point(0, 153);
            this.groupBox35.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox35.Name = "groupBox35";
            this.groupBox35.Size = new System.Drawing.Size(1002, 412);
            this.groupBox35.TabIndex = 0;
            this.groupBox35.TabStop = false;
            this.groupBox35.Text = "Общие сведения";
            // 
            // tableLayoutPanelDetails
            // 
            this.tableLayoutPanelDetails.ColumnCount = 2;
            this.tableLayoutPanelDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDetails.Controls.Add(this.tabControlWithoutTabs1, 0, 1);
            this.tableLayoutPanelDetails.Controls.Add(this.panel10, 1, 0);
            this.tableLayoutPanelDetails.Controls.Add(this.panel11, 0, 0);
            this.tableLayoutPanelDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDetails.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanelDetails.Name = "tableLayoutPanelDetails";
            this.tableLayoutPanelDetails.RowCount = 2;
            this.tableLayoutPanelDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanelDetails.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDetails.Size = new System.Drawing.Size(996, 392);
            this.tableLayoutPanelDetails.TabIndex = 0;
            // 
            // tabControlWithoutTabs1
            // 
            this.tableLayoutPanelDetails.SetColumnSpan(this.tabControlWithoutTabs1, 2);
            this.tabControlWithoutTabs1.Controls.Add(this.tabPageToLegalDepartment);
            this.tabControlWithoutTabs1.Controls.Add(this.tabPageAcceptedByLegalDepartment);
            this.tabControlWithoutTabs1.Controls.Add(this.tabPagePreparingOrder);
            this.tabControlWithoutTabs1.Controls.Add(this.tabPageExecutoryProcess);
            this.tabControlWithoutTabs1.Controls.Add(this.tabPageCompletionClaims);
            this.tabControlWithoutTabs1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlWithoutTabs1.Location = new System.Drawing.Point(0, 60);
            this.tabControlWithoutTabs1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlWithoutTabs1.Name = "tabControlWithoutTabs1";
            this.tabControlWithoutTabs1.SelectedIndex = 0;
            this.tabControlWithoutTabs1.Size = new System.Drawing.Size(996, 356);
            this.tabControlWithoutTabs1.TabIndex = 2;
            // 
            // tabPageToLegalDepartment
            // 
            this.tabPageToLegalDepartment.Controls.Add(this.groupBoxTransfertToLegalDepartment);
            this.tabPageToLegalDepartment.Location = new System.Drawing.Point(4, 24);
            this.tabPageToLegalDepartment.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageToLegalDepartment.Name = "tabPageToLegalDepartment";
            this.tabPageToLegalDepartment.Size = new System.Drawing.Size(988, 328);
            this.tabPageToLegalDepartment.TabIndex = 0;
            this.tabPageToLegalDepartment.Text = "Передача дела в юр. отдел";
            this.tabPageToLegalDepartment.UseVisualStyleBackColor = true;
            // 
            // groupBoxTransfertToLegalDepartment
            // 
            this.groupBoxTransfertToLegalDepartment.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxTransfertToLegalDepartment.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxTransfertToLegalDepartment.Location = new System.Drawing.Point(0, 0);
            this.groupBoxTransfertToLegalDepartment.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxTransfertToLegalDepartment.Name = "groupBoxTransfertToLegalDepartment";
            this.groupBoxTransfertToLegalDepartment.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxTransfertToLegalDepartment.Size = new System.Drawing.Size(988, 47);
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
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textBoxTransferToLegalDepartmentWho);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(494, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(494, 33);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 15);
            this.label2.TabIndex = 32;
            this.label2.Text = "Кто передал";
            // 
            // textBoxTransferToLegalDepartmentWho
            // 
            this.textBoxTransferToLegalDepartmentWho.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTransferToLegalDepartmentWho.Location = new System.Drawing.Point(161, 6);
            this.textBoxTransferToLegalDepartmentWho.Name = "textBoxTransferToLegalDepartmentWho";
            this.textBoxTransferToLegalDepartmentWho.Size = new System.Drawing.Size(327, 21);
            this.textBoxTransferToLegalDepartmentWho.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dateTimePickerTransfertToLegalDepartmentDate);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 33);
            this.panel1.TabIndex = 0;
            // 
            // dateTimePickerTransfertToLegalDepartmentDate
            // 
            this.dateTimePickerTransfertToLegalDepartmentDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerTransfertToLegalDepartmentDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerTransfertToLegalDepartmentDate.Name = "dateTimePickerTransfertToLegalDepartmentDate";
            this.dateTimePickerTransfertToLegalDepartmentDate.ShowCheckBox = true;
            this.dateTimePickerTransfertToLegalDepartmentDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerTransfertToLegalDepartmentDate.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 15);
            this.label1.TabIndex = 33;
            this.label1.Text = "Дата передачи";
            // 
            // tabPagePreparingOrder
            // 
            this.tabPagePreparingOrder.Controls.Add(this.groupBoxObtainingCourtOrder);
            this.tabPagePreparingOrder.Controls.Add(this.groupBoxCourtOrder);
            this.tabPagePreparingOrder.Controls.Add(this.groupBoxClaimDirectedToCourt);
            this.tabPagePreparingOrder.Location = new System.Drawing.Point(4, 24);
            this.tabPagePreparingOrder.Margin = new System.Windows.Forms.Padding(0);
            this.tabPagePreparingOrder.Name = "tabPagePreparingOrder";
            this.tabPagePreparingOrder.Size = new System.Drawing.Size(988, 328);
            this.tabPagePreparingOrder.TabIndex = 1;
            this.tabPagePreparingOrder.Text = "Подготовка и получение судебного приказа";
            this.tabPagePreparingOrder.UseVisualStyleBackColor = true;
            // 
            // groupBoxObtainingCourtOrder
            // 
            this.groupBoxObtainingCourtOrder.Controls.Add(this.tableLayoutPanel5);
            this.groupBoxObtainingCourtOrder.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxObtainingCourtOrder.Location = new System.Drawing.Point(0, 94);
            this.groupBoxObtainingCourtOrder.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxObtainingCourtOrder.Name = "groupBoxObtainingCourtOrder";
            this.groupBoxObtainingCourtOrder.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxObtainingCourtOrder.Size = new System.Drawing.Size(988, 47);
            this.groupBoxObtainingCourtOrder.TabIndex = 3;
            this.groupBoxObtainingCourtOrder.TabStop = false;
            this.groupBoxObtainingCourtOrder.Text = "Получение судебного приказа";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.panel9, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.panel12, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.label9);
            this.panel9.Controls.Add(this.textBoxObtainingCourtOrderDescription);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(494, 0);
            this.panel9.Margin = new System.Windows.Forms.Padding(0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(494, 33);
            this.panel9.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 15);
            this.label9.TabIndex = 32;
            this.label9.Text = "Примечание";
            // 
            // textBoxObtainingCourtOrderDescription
            // 
            this.textBoxObtainingCourtOrderDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxObtainingCourtOrderDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxObtainingCourtOrderDescription.Name = "textBoxObtainingCourtOrderDescription";
            this.textBoxObtainingCourtOrderDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxObtainingCourtOrderDescription.TabIndex = 0;
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.dateTimePickerObtainingCourtOrderDate);
            this.panel12.Controls.Add(this.label10);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel12.Location = new System.Drawing.Point(0, 0);
            this.panel12.Margin = new System.Windows.Forms.Padding(0);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(494, 33);
            this.panel12.TabIndex = 0;
            // 
            // dateTimePickerObtainingCourtOrderDate
            // 
            this.dateTimePickerObtainingCourtOrderDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerObtainingCourtOrderDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerObtainingCourtOrderDate.Name = "dateTimePickerObtainingCourtOrderDate";
            this.dateTimePickerObtainingCourtOrderDate.ShowCheckBox = true;
            this.dateTimePickerObtainingCourtOrderDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerObtainingCourtOrderDate.TabIndex = 32;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(119, 15);
            this.label10.TabIndex = 33;
            this.label10.Text = "Дата получения с/п";
            // 
            // groupBoxCourtOrder
            // 
            this.groupBoxCourtOrder.Controls.Add(this.tableLayoutPanel4);
            this.groupBoxCourtOrder.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxCourtOrder.Location = new System.Drawing.Point(0, 47);
            this.groupBoxCourtOrder.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxCourtOrder.Name = "groupBoxCourtOrder";
            this.groupBoxCourtOrder.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxCourtOrder.Size = new System.Drawing.Size(988, 47);
            this.groupBoxCourtOrder.TabIndex = 2;
            this.groupBoxCourtOrder.TabStop = false;
            this.groupBoxCourtOrder.Text = "Вынесение судебного приказа";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.panel7, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.panel8, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label7);
            this.panel7.Controls.Add(this.textBoxCourtOrderNum);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(494, 0);
            this.panel7.Margin = new System.Windows.Forms.Padding(0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(494, 33);
            this.panel7.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 15);
            this.label7.TabIndex = 32;
            this.label7.Text = "Номер с/п";
            // 
            // textBoxCourtOrderNum
            // 
            this.textBoxCourtOrderNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCourtOrderNum.Location = new System.Drawing.Point(161, 6);
            this.textBoxCourtOrderNum.Name = "textBoxCourtOrderNum";
            this.textBoxCourtOrderNum.Size = new System.Drawing.Size(327, 21);
            this.textBoxCourtOrderNum.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.dateTimePickerCourtOrderDate);
            this.panel8.Controls.Add(this.label8);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Margin = new System.Windows.Forms.Padding(0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(494, 33);
            this.panel8.TabIndex = 0;
            // 
            // dateTimePickerCourtOrderDate
            // 
            this.dateTimePickerCourtOrderDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerCourtOrderDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerCourtOrderDate.Name = "dateTimePickerCourtOrderDate";
            this.dateTimePickerCourtOrderDate.ShowCheckBox = true;
            this.dateTimePickerCourtOrderDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerCourtOrderDate.TabIndex = 32;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(123, 15);
            this.label8.TabIndex = 33;
            this.label8.Text = "Дата вынесения с/п";
            // 
            // groupBoxClaimDirectedToCourt
            // 
            this.groupBoxClaimDirectedToCourt.Controls.Add(this.tableLayoutPanel3);
            this.groupBoxClaimDirectedToCourt.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxClaimDirectedToCourt.Location = new System.Drawing.Point(0, 0);
            this.groupBoxClaimDirectedToCourt.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxClaimDirectedToCourt.Name = "groupBoxClaimDirectedToCourt";
            this.groupBoxClaimDirectedToCourt.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxClaimDirectedToCourt.Size = new System.Drawing.Size(988, 47);
            this.groupBoxClaimDirectedToCourt.TabIndex = 1;
            this.groupBoxClaimDirectedToCourt.TabStop = false;
            this.groupBoxClaimDirectedToCourt.Text = "Направлено исковое заявление в суд";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.panel5, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.panel6, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.textBoxClaimDirectionDescription);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(494, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(494, 33);
            this.panel5.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 15);
            this.label5.TabIndex = 32;
            this.label5.Text = "Примечание";
            // 
            // textBoxClaimDirectionDescription
            // 
            this.textBoxClaimDirectionDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxClaimDirectionDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxClaimDirectionDescription.Name = "textBoxClaimDirectionDescription";
            this.textBoxClaimDirectionDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxClaimDirectionDescription.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.dateTimePickerClaimDirectionDate);
            this.panel6.Controls.Add(this.label6);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(494, 33);
            this.panel6.TabIndex = 0;
            // 
            // dateTimePickerClaimDirectionDate
            // 
            this.dateTimePickerClaimDirectionDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerClaimDirectionDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerClaimDirectionDate.Name = "dateTimePickerClaimDirectionDate";
            this.dateTimePickerClaimDirectionDate.ShowCheckBox = true;
            this.dateTimePickerClaimDirectionDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerClaimDirectionDate.TabIndex = 32;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 15);
            this.label6.TabIndex = 33;
            this.label6.Text = "Дата направления";
            // 
            // tabPageExecutoryProcess
            // 
            this.tabPageExecutoryProcess.Controls.Add(this.groupBoxRepeatedEnforcementProceedingEnd);
            this.tabPageExecutoryProcess.Controls.Add(this.groupBoxRepeatedEnforcementProceedingStart);
            this.tabPageExecutoryProcess.Controls.Add(this.groupBoxRepeatedDirectionCourtOrderBailiffs);
            this.tabPageExecutoryProcess.Controls.Add(this.groupBoxEnforcementProceedingTerminate);
            this.tabPageExecutoryProcess.Controls.Add(this.groupBoxEnforcementProceedingEnd);
            this.tabPageExecutoryProcess.Controls.Add(this.groupBoxEnforcementProceedingStart);
            this.tabPageExecutoryProcess.Controls.Add(this.groupBoxDirectionCourtOrderBailiffs);
            this.tabPageExecutoryProcess.Location = new System.Drawing.Point(4, 24);
            this.tabPageExecutoryProcess.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageExecutoryProcess.Name = "tabPageExecutoryProcess";
            this.tabPageExecutoryProcess.Size = new System.Drawing.Size(988, 328);
            this.tabPageExecutoryProcess.TabIndex = 2;
            this.tabPageExecutoryProcess.Text = "Исполнительное производство";
            this.tabPageExecutoryProcess.UseVisualStyleBackColor = true;
            // 
            // groupBoxRepeatedEnforcementProceedingEnd
            // 
            this.groupBoxRepeatedEnforcementProceedingEnd.Controls.Add(this.tableLayoutPanel10);
            this.groupBoxRepeatedEnforcementProceedingEnd.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxRepeatedEnforcementProceedingEnd.Location = new System.Drawing.Point(0, 282);
            this.groupBoxRepeatedEnforcementProceedingEnd.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxRepeatedEnforcementProceedingEnd.Name = "groupBoxRepeatedEnforcementProceedingEnd";
            this.groupBoxRepeatedEnforcementProceedingEnd.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxRepeatedEnforcementProceedingEnd.Size = new System.Drawing.Size(988, 47);
            this.groupBoxRepeatedEnforcementProceedingEnd.TabIndex = 6;
            this.groupBoxRepeatedEnforcementProceedingEnd.TabStop = false;
            this.groupBoxRepeatedEnforcementProceedingEnd.Text = "Повторное постановление об окончании исполнительного производства";
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.panel21, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.panel22, 0, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel10.TabIndex = 0;
            // 
            // panel21
            // 
            this.panel21.Controls.Add(this.label19);
            this.panel21.Controls.Add(this.textBoxRepeatedEnforcementProceedingEndDescription);
            this.panel21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel21.Location = new System.Drawing.Point(494, 0);
            this.panel21.Margin = new System.Windows.Forms.Padding(0);
            this.panel21.Name = "panel21";
            this.panel21.Size = new System.Drawing.Size(494, 33);
            this.panel21.TabIndex = 1;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(14, 9);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(80, 15);
            this.label19.TabIndex = 32;
            this.label19.Text = "Примечание";
            // 
            // textBoxRepeatedEnforcementProceedingEndDescription
            // 
            this.textBoxRepeatedEnforcementProceedingEndDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRepeatedEnforcementProceedingEndDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxRepeatedEnforcementProceedingEndDescription.Name = "textBoxRepeatedEnforcementProceedingEndDescription";
            this.textBoxRepeatedEnforcementProceedingEndDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxRepeatedEnforcementProceedingEndDescription.TabIndex = 0;
            // 
            // panel22
            // 
            this.panel22.Controls.Add(this.dateTimePickerRepeatedEnforcementProceedingEndDate);
            this.panel22.Controls.Add(this.label20);
            this.panel22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel22.Location = new System.Drawing.Point(0, 0);
            this.panel22.Margin = new System.Windows.Forms.Padding(0);
            this.panel22.Name = "panel22";
            this.panel22.Size = new System.Drawing.Size(494, 33);
            this.panel22.TabIndex = 0;
            // 
            // dateTimePickerRepeatedEnforcementProceedingEndDate
            // 
            this.dateTimePickerRepeatedEnforcementProceedingEndDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerRepeatedEnforcementProceedingEndDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerRepeatedEnforcementProceedingEndDate.Name = "dateTimePickerRepeatedEnforcementProceedingEndDate";
            this.dateTimePickerRepeatedEnforcementProceedingEndDate.ShowCheckBox = true;
            this.dateTimePickerRepeatedEnforcementProceedingEndDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerRepeatedEnforcementProceedingEndDate.TabIndex = 32;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 9);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(118, 15);
            this.label20.TabIndex = 33;
            this.label20.Text = "Дата прекращения";
            // 
            // groupBoxRepeatedEnforcementProceedingStart
            // 
            this.groupBoxRepeatedEnforcementProceedingStart.Controls.Add(this.tableLayoutPanel11);
            this.groupBoxRepeatedEnforcementProceedingStart.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxRepeatedEnforcementProceedingStart.Location = new System.Drawing.Point(0, 235);
            this.groupBoxRepeatedEnforcementProceedingStart.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxRepeatedEnforcementProceedingStart.Name = "groupBoxRepeatedEnforcementProceedingStart";
            this.groupBoxRepeatedEnforcementProceedingStart.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxRepeatedEnforcementProceedingStart.Size = new System.Drawing.Size(988, 47);
            this.groupBoxRepeatedEnforcementProceedingStart.TabIndex = 5;
            this.groupBoxRepeatedEnforcementProceedingStart.TabStop = false;
            this.groupBoxRepeatedEnforcementProceedingStart.Text = "Повтороное постановление о возбуждении исполнительного производства";
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.ColumnCount = 2;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel11.Controls.Add(this.panel23, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.panel24, 0, 0);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel11.TabIndex = 0;
            // 
            // panel23
            // 
            this.panel23.Controls.Add(this.label21);
            this.panel23.Controls.Add(this.textBoxRepeatedEnforcementProceedingStartDescription);
            this.panel23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel23.Location = new System.Drawing.Point(494, 0);
            this.panel23.Margin = new System.Windows.Forms.Padding(0);
            this.panel23.Name = "panel23";
            this.panel23.Size = new System.Drawing.Size(494, 33);
            this.panel23.TabIndex = 1;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(14, 9);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(80, 15);
            this.label21.TabIndex = 32;
            this.label21.Text = "Примечание";
            // 
            // textBoxRepeatedEnforcementProceedingStartDescription
            // 
            this.textBoxRepeatedEnforcementProceedingStartDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRepeatedEnforcementProceedingStartDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxRepeatedEnforcementProceedingStartDescription.Name = "textBoxRepeatedEnforcementProceedingStartDescription";
            this.textBoxRepeatedEnforcementProceedingStartDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxRepeatedEnforcementProceedingStartDescription.TabIndex = 0;
            // 
            // panel24
            // 
            this.panel24.Controls.Add(this.dateTimePickerRepeatedEnforcementProceedingStartDate);
            this.panel24.Controls.Add(this.label22);
            this.panel24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel24.Location = new System.Drawing.Point(0, 0);
            this.panel24.Margin = new System.Windows.Forms.Padding(0);
            this.panel24.Name = "panel24";
            this.panel24.Size = new System.Drawing.Size(494, 33);
            this.panel24.TabIndex = 0;
            // 
            // dateTimePickerRepeatedEnforcementProceedingStartDate
            // 
            this.dateTimePickerRepeatedEnforcementProceedingStartDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerRepeatedEnforcementProceedingStartDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerRepeatedEnforcementProceedingStartDate.Name = "dateTimePickerRepeatedEnforcementProceedingStartDate";
            this.dateTimePickerRepeatedEnforcementProceedingStartDate.ShowCheckBox = true;
            this.dateTimePickerRepeatedEnforcementProceedingStartDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerRepeatedEnforcementProceedingStartDate.TabIndex = 32;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(10, 9);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(116, 15);
            this.label22.TabIndex = 33;
            this.label22.Text = "Дата возбуждения";
            // 
            // groupBoxRepeatedDirectionCourtOrderBailiffs
            // 
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.Controls.Add(this.tableLayoutPanel9);
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.Location = new System.Drawing.Point(0, 188);
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.Name = "groupBoxRepeatedDirectionCourtOrderBailiffs";
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.Size = new System.Drawing.Size(988, 47);
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.TabIndex = 4;
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.TabStop = false;
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.Text = "Повторное направление с/п приставам";
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Controls.Add(this.panel19, 1, 0);
            this.tableLayoutPanel9.Controls.Add(this.panel20, 0, 0);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 1;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel9.TabIndex = 0;
            // 
            // panel19
            // 
            this.panel19.Controls.Add(this.label17);
            this.panel19.Controls.Add(this.textBoxRepeatedDirectionCourtOrderBailiffsDescription);
            this.panel19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel19.Location = new System.Drawing.Point(494, 0);
            this.panel19.Margin = new System.Windows.Forms.Padding(0);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(494, 33);
            this.panel19.TabIndex = 1;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(14, 9);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(80, 15);
            this.label17.TabIndex = 32;
            this.label17.Text = "Примечание";
            // 
            // textBoxRepeatedDirectionCourtOrderBailiffsDescription
            // 
            this.textBoxRepeatedDirectionCourtOrderBailiffsDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRepeatedDirectionCourtOrderBailiffsDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxRepeatedDirectionCourtOrderBailiffsDescription.Name = "textBoxRepeatedDirectionCourtOrderBailiffsDescription";
            this.textBoxRepeatedDirectionCourtOrderBailiffsDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxRepeatedDirectionCourtOrderBailiffsDescription.TabIndex = 0;
            // 
            // panel20
            // 
            this.panel20.Controls.Add(this.dateTimePickerRepeatedDirectionCourtOrderBailiffsDate);
            this.panel20.Controls.Add(this.label18);
            this.panel20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel20.Location = new System.Drawing.Point(0, 0);
            this.panel20.Margin = new System.Windows.Forms.Padding(0);
            this.panel20.Name = "panel20";
            this.panel20.Size = new System.Drawing.Size(494, 33);
            this.panel20.TabIndex = 0;
            // 
            // dateTimePickerRepeatedDirectionCourtOrderBailiffsDate
            // 
            this.dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Name = "dateTimePickerRepeatedDirectionCourtOrderBailiffsDate";
            this.dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.ShowCheckBox = true;
            this.dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.TabIndex = 32;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 9);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(117, 15);
            this.label18.TabIndex = 33;
            this.label18.Text = "Дата направления";
            // 
            // groupBoxEnforcementProceedingTerminate
            // 
            this.groupBoxEnforcementProceedingTerminate.Controls.Add(this.tableLayoutPanel12);
            this.groupBoxEnforcementProceedingTerminate.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxEnforcementProceedingTerminate.Location = new System.Drawing.Point(0, 141);
            this.groupBoxEnforcementProceedingTerminate.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxEnforcementProceedingTerminate.Name = "groupBoxEnforcementProceedingTerminate";
            this.groupBoxEnforcementProceedingTerminate.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxEnforcementProceedingTerminate.Size = new System.Drawing.Size(988, 47);
            this.groupBoxEnforcementProceedingTerminate.TabIndex = 3;
            this.groupBoxEnforcementProceedingTerminate.TabStop = false;
            this.groupBoxEnforcementProceedingTerminate.Text = "Постановление о прекращении исполнительного производства";
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 2;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.Controls.Add(this.panel25, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.panel26, 0, 0);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 1;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel12.TabIndex = 0;
            // 
            // panel25
            // 
            this.panel25.Controls.Add(this.label23);
            this.panel25.Controls.Add(this.textBoxEnforcementProceedingTerminateDescription);
            this.panel25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel25.Location = new System.Drawing.Point(494, 0);
            this.panel25.Margin = new System.Windows.Forms.Padding(0);
            this.panel25.Name = "panel25";
            this.panel25.Size = new System.Drawing.Size(494, 33);
            this.panel25.TabIndex = 1;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(14, 9);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(80, 15);
            this.label23.TabIndex = 32;
            this.label23.Text = "Примечание";
            // 
            // textBoxEnforcementProceedingTerminateDescription
            // 
            this.textBoxEnforcementProceedingTerminateDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEnforcementProceedingTerminateDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxEnforcementProceedingTerminateDescription.Name = "textBoxEnforcementProceedingTerminateDescription";
            this.textBoxEnforcementProceedingTerminateDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxEnforcementProceedingTerminateDescription.TabIndex = 0;
            // 
            // panel26
            // 
            this.panel26.Controls.Add(this.dateTimePickerEnforcementProceedingTerminateDate);
            this.panel26.Controls.Add(this.label24);
            this.panel26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel26.Location = new System.Drawing.Point(0, 0);
            this.panel26.Margin = new System.Windows.Forms.Padding(0);
            this.panel26.Name = "panel26";
            this.panel26.Size = new System.Drawing.Size(494, 33);
            this.panel26.TabIndex = 0;
            // 
            // dateTimePickerEnforcementProceedingTerminateDate
            // 
            this.dateTimePickerEnforcementProceedingTerminateDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEnforcementProceedingTerminateDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerEnforcementProceedingTerminateDate.Name = "dateTimePickerEnforcementProceedingTerminateDate";
            this.dateTimePickerEnforcementProceedingTerminateDate.ShowCheckBox = true;
            this.dateTimePickerEnforcementProceedingTerminateDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerEnforcementProceedingTerminateDate.TabIndex = 32;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(10, 9);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(118, 15);
            this.label24.TabIndex = 33;
            this.label24.Text = "Дата прекращения";
            // 
            // groupBoxEnforcementProceedingEnd
            // 
            this.groupBoxEnforcementProceedingEnd.Controls.Add(this.tableLayoutPanel8);
            this.groupBoxEnforcementProceedingEnd.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxEnforcementProceedingEnd.Location = new System.Drawing.Point(0, 94);
            this.groupBoxEnforcementProceedingEnd.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxEnforcementProceedingEnd.Name = "groupBoxEnforcementProceedingEnd";
            this.groupBoxEnforcementProceedingEnd.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxEnforcementProceedingEnd.Size = new System.Drawing.Size(988, 47);
            this.groupBoxEnforcementProceedingEnd.TabIndex = 2;
            this.groupBoxEnforcementProceedingEnd.TabStop = false;
            this.groupBoxEnforcementProceedingEnd.Text = "Постановление об окончании исполнительного производства";
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Controls.Add(this.panel17, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.panel18, 0, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel8.TabIndex = 0;
            // 
            // panel17
            // 
            this.panel17.Controls.Add(this.label15);
            this.panel17.Controls.Add(this.textBoxEnforcementProceedingEndDescription);
            this.panel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel17.Location = new System.Drawing.Point(494, 0);
            this.panel17.Margin = new System.Windows.Forms.Padding(0);
            this.panel17.Name = "panel17";
            this.panel17.Size = new System.Drawing.Size(494, 33);
            this.panel17.TabIndex = 1;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 9);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(80, 15);
            this.label15.TabIndex = 32;
            this.label15.Text = "Примечание";
            // 
            // textBoxEnforcementProceedingEndDescription
            // 
            this.textBoxEnforcementProceedingEndDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEnforcementProceedingEndDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxEnforcementProceedingEndDescription.Name = "textBoxEnforcementProceedingEndDescription";
            this.textBoxEnforcementProceedingEndDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxEnforcementProceedingEndDescription.TabIndex = 0;
            // 
            // panel18
            // 
            this.panel18.Controls.Add(this.dateTimePickerEnforcementProceedingEndDate);
            this.panel18.Controls.Add(this.label16);
            this.panel18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel18.Location = new System.Drawing.Point(0, 0);
            this.panel18.Margin = new System.Windows.Forms.Padding(0);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(494, 33);
            this.panel18.TabIndex = 0;
            // 
            // dateTimePickerEnforcementProceedingEndDate
            // 
            this.dateTimePickerEnforcementProceedingEndDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEnforcementProceedingEndDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerEnforcementProceedingEndDate.Name = "dateTimePickerEnforcementProceedingEndDate";
            this.dateTimePickerEnforcementProceedingEndDate.ShowCheckBox = true;
            this.dateTimePickerEnforcementProceedingEndDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerEnforcementProceedingEndDate.TabIndex = 32;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(10, 9);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(101, 15);
            this.label16.TabIndex = 33;
            this.label16.Text = "Дата окончания";
            // 
            // groupBoxEnforcementProceedingStart
            // 
            this.groupBoxEnforcementProceedingStart.Controls.Add(this.tableLayoutPanel7);
            this.groupBoxEnforcementProceedingStart.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxEnforcementProceedingStart.Location = new System.Drawing.Point(0, 47);
            this.groupBoxEnforcementProceedingStart.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxEnforcementProceedingStart.Name = "groupBoxEnforcementProceedingStart";
            this.groupBoxEnforcementProceedingStart.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxEnforcementProceedingStart.Size = new System.Drawing.Size(988, 47);
            this.groupBoxEnforcementProceedingStart.TabIndex = 1;
            this.groupBoxEnforcementProceedingStart.TabStop = false;
            this.groupBoxEnforcementProceedingStart.Text = "Постановление о возбуждении исполнительного производства";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.panel15, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.panel16, 0, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // panel15
            // 
            this.panel15.Controls.Add(this.label13);
            this.panel15.Controls.Add(this.textBoxEnforcementProceedingStartDescription);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel15.Location = new System.Drawing.Point(494, 0);
            this.panel15.Margin = new System.Windows.Forms.Padding(0);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(494, 33);
            this.panel15.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(14, 9);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(80, 15);
            this.label13.TabIndex = 32;
            this.label13.Text = "Примечание";
            // 
            // textBoxEnforcementProceedingStartDescription
            // 
            this.textBoxEnforcementProceedingStartDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEnforcementProceedingStartDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxEnforcementProceedingStartDescription.Name = "textBoxEnforcementProceedingStartDescription";
            this.textBoxEnforcementProceedingStartDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxEnforcementProceedingStartDescription.TabIndex = 0;
            // 
            // panel16
            // 
            this.panel16.Controls.Add(this.dateTimePickerEnforcementProceedingStartDate);
            this.panel16.Controls.Add(this.label14);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel16.Location = new System.Drawing.Point(0, 0);
            this.panel16.Margin = new System.Windows.Forms.Padding(0);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(494, 33);
            this.panel16.TabIndex = 0;
            // 
            // dateTimePickerEnforcementProceedingStartDate
            // 
            this.dateTimePickerEnforcementProceedingStartDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEnforcementProceedingStartDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerEnforcementProceedingStartDate.Name = "dateTimePickerEnforcementProceedingStartDate";
            this.dateTimePickerEnforcementProceedingStartDate.ShowCheckBox = true;
            this.dateTimePickerEnforcementProceedingStartDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerEnforcementProceedingStartDate.TabIndex = 32;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 9);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(116, 15);
            this.label14.TabIndex = 33;
            this.label14.Text = "Дата возбуждения";
            // 
            // groupBoxDirectionCourtOrderBailiffs
            // 
            this.groupBoxDirectionCourtOrderBailiffs.Controls.Add(this.tableLayoutPanel6);
            this.groupBoxDirectionCourtOrderBailiffs.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxDirectionCourtOrderBailiffs.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDirectionCourtOrderBailiffs.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxDirectionCourtOrderBailiffs.Name = "groupBoxDirectionCourtOrderBailiffs";
            this.groupBoxDirectionCourtOrderBailiffs.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxDirectionCourtOrderBailiffs.Size = new System.Drawing.Size(988, 47);
            this.groupBoxDirectionCourtOrderBailiffs.TabIndex = 0;
            this.groupBoxDirectionCourtOrderBailiffs.TabStop = false;
            this.groupBoxDirectionCourtOrderBailiffs.Text = "Направлено с/п приставам";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.panel13, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.panel14, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.label11);
            this.panel13.Controls.Add(this.textBoxDirectionCourtOrderBailiffsDescription);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel13.Location = new System.Drawing.Point(494, 0);
            this.panel13.Margin = new System.Windows.Forms.Padding(0);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(494, 33);
            this.panel13.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 15);
            this.label11.TabIndex = 32;
            this.label11.Text = "Примечание";
            // 
            // textBoxDirectionCourtOrderBailiffsDescription
            // 
            this.textBoxDirectionCourtOrderBailiffsDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDirectionCourtOrderBailiffsDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxDirectionCourtOrderBailiffsDescription.Name = "textBoxDirectionCourtOrderBailiffsDescription";
            this.textBoxDirectionCourtOrderBailiffsDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxDirectionCourtOrderBailiffsDescription.TabIndex = 0;
            // 
            // panel14
            // 
            this.panel14.Controls.Add(this.dateTimePickerDirectionCourtOrderBailiffsDate);
            this.panel14.Controls.Add(this.label12);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel14.Location = new System.Drawing.Point(0, 0);
            this.panel14.Margin = new System.Windows.Forms.Padding(0);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(494, 33);
            this.panel14.TabIndex = 0;
            // 
            // dateTimePickerDirectionCourtOrderBailiffsDate
            // 
            this.dateTimePickerDirectionCourtOrderBailiffsDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDirectionCourtOrderBailiffsDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerDirectionCourtOrderBailiffsDate.Name = "dateTimePickerDirectionCourtOrderBailiffsDate";
            this.dateTimePickerDirectionCourtOrderBailiffsDate.ShowCheckBox = true;
            this.dateTimePickerDirectionCourtOrderBailiffsDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerDirectionCourtOrderBailiffsDate.TabIndex = 32;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(117, 15);
            this.label12.TabIndex = 33;
            this.label12.Text = "Дата направления";
            // 
            // tabPageCompletionClaims
            // 
            this.tabPageCompletionClaims.Controls.Add(this.groupBoxClaimComplete);
            this.tabPageCompletionClaims.Controls.Add(this.groupBoxCourtOrderCancel);
            this.tabPageCompletionClaims.Location = new System.Drawing.Point(4, 24);
            this.tabPageCompletionClaims.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageCompletionClaims.Name = "tabPageCompletionClaims";
            this.tabPageCompletionClaims.Size = new System.Drawing.Size(988, 328);
            this.tabPageCompletionClaims.TabIndex = 3;
            this.tabPageCompletionClaims.Text = "Завершение претензионной работы";
            this.tabPageCompletionClaims.UseVisualStyleBackColor = true;
            // 
            // groupBoxClaimComplete
            // 
            this.groupBoxClaimComplete.Controls.Add(this.tableLayoutPanel14);
            this.groupBoxClaimComplete.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxClaimComplete.Location = new System.Drawing.Point(0, 47);
            this.groupBoxClaimComplete.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxClaimComplete.Name = "groupBoxClaimComplete";
            this.groupBoxClaimComplete.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxClaimComplete.Size = new System.Drawing.Size(988, 77);
            this.groupBoxClaimComplete.TabIndex = 4;
            this.groupBoxClaimComplete.TabStop = false;
            this.groupBoxClaimComplete.Text = "Завершено";
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.ColumnCount = 2;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.Controls.Add(this.panel29, 1, 0);
            this.tableLayoutPanel14.Controls.Add(this.panel30, 0, 0);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 1;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(988, 63);
            this.tableLayoutPanel14.TabIndex = 0;
            // 
            // panel29
            // 
            this.panel29.Controls.Add(this.label27);
            this.panel29.Controls.Add(this.textBoxClaimCompleteDescription);
            this.panel29.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel29.Location = new System.Drawing.Point(494, 0);
            this.panel29.Margin = new System.Windows.Forms.Padding(0);
            this.panel29.Name = "panel29";
            this.panel29.Size = new System.Drawing.Size(494, 63);
            this.panel29.TabIndex = 1;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(14, 9);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(80, 15);
            this.label27.TabIndex = 32;
            this.label27.Text = "Примечание";
            // 
            // textBoxClaimCompleteDescription
            // 
            this.textBoxClaimCompleteDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxClaimCompleteDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxClaimCompleteDescription.Name = "textBoxClaimCompleteDescription";
            this.textBoxClaimCompleteDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxClaimCompleteDescription.TabIndex = 0;
            // 
            // panel30
            // 
            this.panel30.Controls.Add(this.label29);
            this.panel30.Controls.Add(this.textBoxClaimCompleteReason);
            this.panel30.Controls.Add(this.dateTimePickerClaimCompleteDate);
            this.panel30.Controls.Add(this.label28);
            this.panel30.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel30.Location = new System.Drawing.Point(0, 0);
            this.panel30.Margin = new System.Windows.Forms.Padding(0);
            this.panel30.Name = "panel30";
            this.panel30.Size = new System.Drawing.Size(494, 63);
            this.panel30.TabIndex = 0;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(11, 37);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(57, 15);
            this.label29.TabIndex = 35;
            this.label29.Text = "Причина";
            // 
            // textBoxClaimCompleteReason
            // 
            this.textBoxClaimCompleteReason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxClaimCompleteReason.Location = new System.Drawing.Point(158, 34);
            this.textBoxClaimCompleteReason.Name = "textBoxClaimCompleteReason";
            this.textBoxClaimCompleteReason.Size = new System.Drawing.Size(327, 21);
            this.textBoxClaimCompleteReason.TabIndex = 34;
            // 
            // dateTimePickerClaimCompleteDate
            // 
            this.dateTimePickerClaimCompleteDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerClaimCompleteDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerClaimCompleteDate.Name = "dateTimePickerClaimCompleteDate";
            this.dateTimePickerClaimCompleteDate.ShowCheckBox = true;
            this.dateTimePickerClaimCompleteDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerClaimCompleteDate.TabIndex = 32;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(10, 9);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(111, 15);
            this.label28.TabIndex = 33;
            this.label28.Text = "Дата завершения";
            // 
            // groupBoxCourtOrderCancel
            // 
            this.groupBoxCourtOrderCancel.Controls.Add(this.tableLayoutPanel13);
            this.groupBoxCourtOrderCancel.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxCourtOrderCancel.Location = new System.Drawing.Point(0, 0);
            this.groupBoxCourtOrderCancel.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxCourtOrderCancel.Name = "groupBoxCourtOrderCancel";
            this.groupBoxCourtOrderCancel.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxCourtOrderCancel.Size = new System.Drawing.Size(988, 47);
            this.groupBoxCourtOrderCancel.TabIndex = 3;
            this.groupBoxCourtOrderCancel.TabStop = false;
            this.groupBoxCourtOrderCancel.Text = "Определение об отмене с/п";
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.ColumnCount = 2;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel13.Controls.Add(this.panel27, 1, 0);
            this.tableLayoutPanel13.Controls.Add(this.panel28, 0, 0);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 1;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel13.TabIndex = 0;
            // 
            // panel27
            // 
            this.panel27.Controls.Add(this.label25);
            this.panel27.Controls.Add(this.textBoxCourtOrderCancelDescription);
            this.panel27.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel27.Location = new System.Drawing.Point(494, 0);
            this.panel27.Margin = new System.Windows.Forms.Padding(0);
            this.panel27.Name = "panel27";
            this.panel27.Size = new System.Drawing.Size(494, 33);
            this.panel27.TabIndex = 1;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(14, 9);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(80, 15);
            this.label25.TabIndex = 32;
            this.label25.Text = "Примечание";
            // 
            // textBoxCourtOrderCancelDescription
            // 
            this.textBoxCourtOrderCancelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCourtOrderCancelDescription.Location = new System.Drawing.Point(161, 6);
            this.textBoxCourtOrderCancelDescription.Name = "textBoxCourtOrderCancelDescription";
            this.textBoxCourtOrderCancelDescription.Size = new System.Drawing.Size(327, 21);
            this.textBoxCourtOrderCancelDescription.TabIndex = 0;
            // 
            // panel28
            // 
            this.panel28.Controls.Add(this.dateTimePickerCourtOrderCancelDate);
            this.panel28.Controls.Add(this.label26);
            this.panel28.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel28.Location = new System.Drawing.Point(0, 0);
            this.panel28.Margin = new System.Windows.Forms.Padding(0);
            this.panel28.Name = "panel28";
            this.panel28.Size = new System.Drawing.Size(494, 33);
            this.panel28.TabIndex = 0;
            // 
            // dateTimePickerCourtOrderCancelDate
            // 
            this.dateTimePickerCourtOrderCancelDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerCourtOrderCancelDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerCourtOrderCancelDate.Name = "dateTimePickerCourtOrderCancelDate";
            this.dateTimePickerCourtOrderCancelDate.ShowCheckBox = true;
            this.dateTimePickerCourtOrderCancelDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerCourtOrderCancelDate.TabIndex = 32;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(10, 9);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(86, 15);
            this.label26.TabIndex = 33;
            this.label26.Text = "Дата отмены";
            // 
            // tabPageAcceptedByLegalDepartment
            // 
            this.tabPageAcceptedByLegalDepartment.Controls.Add(this.groupBoxAcceptedByLegalDepartment);
            this.tabPageAcceptedByLegalDepartment.Location = new System.Drawing.Point(4, 24);
            this.tabPageAcceptedByLegalDepartment.Name = "tabPageAcceptedByLegalDepartment";
            this.tabPageAcceptedByLegalDepartment.Size = new System.Drawing.Size(988, 328);
            this.tabPageAcceptedByLegalDepartment.TabIndex = 4;
            this.tabPageAcceptedByLegalDepartment.Text = "Принято в юр. отдел";
            this.tabPageAcceptedByLegalDepartment.UseVisualStyleBackColor = true;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.dateTimePickerStartState);
            this.panel10.Controls.Add(this.label108);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(498, 0);
            this.panel10.Margin = new System.Windows.Forms.Padding(0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(498, 60);
            this.panel10.TabIndex = 1;
            // 
            // dateTimePickerStartState
            // 
            this.dateTimePickerStartState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartState.Location = new System.Drawing.Point(161, 6);
            this.dateTimePickerStartState.Name = "dateTimePickerStartState";
            this.dateTimePickerStartState.Size = new System.Drawing.Size(327, 21);
            this.dateTimePickerStartState.TabIndex = 0;
            // 
            // label108
            // 
            this.label108.AutoSize = true;
            this.label108.Location = new System.Drawing.Point(14, 9);
            this.label108.Name = "label108";
            this.label108.Size = new System.Drawing.Size(99, 15);
            this.label108.TabIndex = 31;
            this.label108.Text = "Дата установки";
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.label30);
            this.panel11.Controls.Add(this.textBoxDescription);
            this.panel11.Controls.Add(this.comboBoxClaimStateType);
            this.panel11.Controls.Add(this.label109);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Margin = new System.Windows.Forms.Padding(0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(498, 60);
            this.panel11.TabIndex = 0;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(14, 36);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(80, 15);
            this.label30.TabIndex = 34;
            this.label30.Text = "Примечание";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(161, 33);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(327, 21);
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
            this.comboBoxClaimStateType.Size = new System.Drawing.Size(328, 23);
            this.comboBoxClaimStateType.TabIndex = 0;
            this.comboBoxClaimStateType.SelectedValueChanged += new System.EventHandler(this.comboBoxClaimStateType_SelectedValueChanged);
            // 
            // label109
            // 
            this.label109.AutoSize = true;
            this.label109.Location = new System.Drawing.Point(14, 7);
            this.label109.Name = "label109";
            this.label109.Size = new System.Drawing.Size(93, 15);
            this.label109.TabIndex = 29;
            this.label109.Text = "Вид состояния";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_state_type,
            this.date_start_state,
            this.description});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(996, 147);
            this.dataGridView.TabIndex = 1;
            // 
            // id_state_type
            // 
            this.id_state_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_state_type.HeaderText = "Вид состояния";
            this.id_state_type.MinimumWidth = 150;
            this.id_state_type.Name = "id_state_type";
            this.id_state_type.ReadOnly = true;
            this.id_state_type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.id_state_type.Width = 150;
            // 
            // date_start_state
            // 
            this.date_start_state.HeaderText = "Дата установки";
            this.date_start_state.MinimumWidth = 150;
            this.date_start_state.Name = "date_start_state";
            this.date_start_state.ReadOnly = true;
            this.date_start_state.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.date_start_state.Width = 150;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Примечание";
            this.description.MinimumWidth = 200;
            this.description.Name = "description";
            this.description.ReadOnly = true;
            this.description.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // groupBoxAcceptedByLegalDepartment
            // 
            this.groupBoxAcceptedByLegalDepartment.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxAcceptedByLegalDepartment.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxAcceptedByLegalDepartment.Location = new System.Drawing.Point(0, 0);
            this.groupBoxAcceptedByLegalDepartment.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxAcceptedByLegalDepartment.Name = "groupBoxAcceptedByLegalDepartment";
            this.groupBoxAcceptedByLegalDepartment.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxAcceptedByLegalDepartment.Size = new System.Drawing.Size(988, 47);
            this.groupBoxAcceptedByLegalDepartment.TabIndex = 2;
            this.groupBoxAcceptedByLegalDepartment.TabStop = false;
            this.groupBoxAcceptedByLegalDepartment.Text = "Принято в юр. отдел";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel4, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 14);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(988, 33);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.textBoxAcceptedByLegalDepartmentWho);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(494, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(494, 33);
            this.panel3.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 15);
            this.label3.TabIndex = 32;
            this.label3.Text = "Кто принял";
            // 
            // textBoxAcceptedByLegalDepartmentWho
            // 
            this.textBoxAcceptedByLegalDepartmentWho.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAcceptedByLegalDepartmentWho.Location = new System.Drawing.Point(161, 6);
            this.textBoxAcceptedByLegalDepartmentWho.Name = "textBoxAcceptedByLegalDepartmentWho";
            this.textBoxAcceptedByLegalDepartmentWho.Size = new System.Drawing.Size(327, 21);
            this.textBoxAcceptedByLegalDepartmentWho.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dateTimePickerAcceptedByLegalDepartmentDate);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(494, 33);
            this.panel4.TabIndex = 0;
            // 
            // dateTimePickerAcceptedByLegalDepartmentDate
            // 
            this.dateTimePickerAcceptedByLegalDepartmentDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAcceptedByLegalDepartmentDate.Location = new System.Drawing.Point(157, 6);
            this.dateTimePickerAcceptedByLegalDepartmentDate.Name = "dateTimePickerAcceptedByLegalDepartmentDate";
            this.dateTimePickerAcceptedByLegalDepartmentDate.ShowCheckBox = true;
            this.dateTimePickerAcceptedByLegalDepartmentDate.Size = new System.Drawing.Size(328, 21);
            this.dateTimePickerAcceptedByLegalDepartmentDate.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 15);
            this.label4.TabIndex = 33;
            this.label4.Text = "Дата принятия";
            // 
            // ClaimStatesViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(700, 190);
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 571);
            this.Controls.Add(this.tableLayoutPanelAll);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClaimStatesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Состояния иск. работы №{0}";
            this.tableLayoutPanelAll.ResumeLayout(false);
            this.groupBox35.ResumeLayout(false);
            this.tableLayoutPanelDetails.ResumeLayout(false);
            this.tabControlWithoutTabs1.ResumeLayout(false);
            this.tabPageToLegalDepartment.ResumeLayout(false);
            this.groupBoxTransfertToLegalDepartment.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPagePreparingOrder.ResumeLayout(false);
            this.groupBoxObtainingCourtOrder.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.groupBoxCourtOrder.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.groupBoxClaimDirectedToCourt.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.tabPageExecutoryProcess.ResumeLayout(false);
            this.groupBoxRepeatedEnforcementProceedingEnd.ResumeLayout(false);
            this.tableLayoutPanel10.ResumeLayout(false);
            this.panel21.ResumeLayout(false);
            this.panel21.PerformLayout();
            this.panel22.ResumeLayout(false);
            this.panel22.PerformLayout();
            this.groupBoxRepeatedEnforcementProceedingStart.ResumeLayout(false);
            this.tableLayoutPanel11.ResumeLayout(false);
            this.panel23.ResumeLayout(false);
            this.panel23.PerformLayout();
            this.panel24.ResumeLayout(false);
            this.panel24.PerformLayout();
            this.groupBoxRepeatedDirectionCourtOrderBailiffs.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            this.panel19.ResumeLayout(false);
            this.panel19.PerformLayout();
            this.panel20.ResumeLayout(false);
            this.panel20.PerformLayout();
            this.groupBoxEnforcementProceedingTerminate.ResumeLayout(false);
            this.tableLayoutPanel12.ResumeLayout(false);
            this.panel25.ResumeLayout(false);
            this.panel25.PerformLayout();
            this.panel26.ResumeLayout(false);
            this.panel26.PerformLayout();
            this.groupBoxEnforcementProceedingEnd.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.panel17.ResumeLayout(false);
            this.panel17.PerformLayout();
            this.panel18.ResumeLayout(false);
            this.panel18.PerformLayout();
            this.groupBoxEnforcementProceedingStart.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            this.panel16.ResumeLayout(false);
            this.panel16.PerformLayout();
            this.groupBoxDirectionCourtOrderBailiffs.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel13.PerformLayout();
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            this.tabPageCompletionClaims.ResumeLayout(false);
            this.groupBoxClaimComplete.ResumeLayout(false);
            this.tableLayoutPanel14.ResumeLayout(false);
            this.panel29.ResumeLayout(false);
            this.panel29.PerformLayout();
            this.panel30.ResumeLayout(false);
            this.panel30.PerformLayout();
            this.groupBoxCourtOrderCancel.ResumeLayout(false);
            this.tableLayoutPanel13.ResumeLayout(false);
            this.panel27.ResumeLayout(false);
            this.panel27.PerformLayout();
            this.panel28.ResumeLayout(false);
            this.panel28.PerformLayout();
            this.tabPageAcceptedByLegalDepartment.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupBoxAcceptedByLegalDepartment.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        private CustomControls.TabControlWithoutTabs tabControlWithoutTabs1;
        private TabPage tabPageToLegalDepartment;
        private GroupBox groupBoxTransfertToLegalDepartment;
        private TabPage tabPagePreparingOrder;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel2;
        private Label label2;
        private TextBox textBoxTransferToLegalDepartmentWho;
        private Panel panel1;
        private DateTimePicker dateTimePickerTransfertToLegalDepartmentDate;
        private Label label1;
        private GroupBox groupBoxObtainingCourtOrder;
        private TableLayoutPanel tableLayoutPanel5;
        private Panel panel9;
        private Label label9;
        private TextBox textBoxObtainingCourtOrderDescription;
        private Panel panel12;
        private DateTimePicker dateTimePickerObtainingCourtOrderDate;
        private Label label10;
        private GroupBox groupBoxCourtOrder;
        private TableLayoutPanel tableLayoutPanel4;
        private Panel panel7;
        private Label label7;
        private TextBox textBoxCourtOrderNum;
        private Panel panel8;
        private DateTimePicker dateTimePickerCourtOrderDate;
        private Label label8;
        private GroupBox groupBoxClaimDirectedToCourt;
        private TableLayoutPanel tableLayoutPanel3;
        private Panel panel5;
        private Label label5;
        private TextBox textBoxClaimDirectionDescription;
        private Panel panel6;
        private DateTimePicker dateTimePickerClaimDirectionDate;
        private Label label6;
        private TabPage tabPageExecutoryProcess;
        private GroupBox groupBoxRepeatedEnforcementProceedingEnd;
        private TableLayoutPanel tableLayoutPanel10;
        private Panel panel21;
        private Label label19;
        private TextBox textBoxRepeatedEnforcementProceedingEndDescription;
        private Panel panel22;
        private DateTimePicker dateTimePickerRepeatedEnforcementProceedingEndDate;
        private Label label20;
        private GroupBox groupBoxRepeatedEnforcementProceedingStart;
        private TableLayoutPanel tableLayoutPanel11;
        private Panel panel23;
        private Label label21;
        private TextBox textBoxRepeatedEnforcementProceedingStartDescription;
        private Panel panel24;
        private DateTimePicker dateTimePickerRepeatedEnforcementProceedingStartDate;
        private Label label22;
        private GroupBox groupBoxRepeatedDirectionCourtOrderBailiffs;
        private TableLayoutPanel tableLayoutPanel9;
        private Panel panel19;
        private Label label17;
        private TextBox textBoxRepeatedDirectionCourtOrderBailiffsDescription;
        private Panel panel20;
        private DateTimePicker dateTimePickerRepeatedDirectionCourtOrderBailiffsDate;
        private Label label18;
        private GroupBox groupBoxEnforcementProceedingTerminate;
        private TableLayoutPanel tableLayoutPanel12;
        private Panel panel25;
        private Label label23;
        private TextBox textBoxEnforcementProceedingTerminateDescription;
        private Panel panel26;
        private DateTimePicker dateTimePickerEnforcementProceedingTerminateDate;
        private Label label24;
        private GroupBox groupBoxEnforcementProceedingEnd;
        private TableLayoutPanel tableLayoutPanel8;
        private Panel panel17;
        private Label label15;
        private TextBox textBoxEnforcementProceedingEndDescription;
        private Panel panel18;
        private DateTimePicker dateTimePickerEnforcementProceedingEndDate;
        private Label label16;
        private GroupBox groupBoxEnforcementProceedingStart;
        private TableLayoutPanel tableLayoutPanel7;
        private Panel panel15;
        private Label label13;
        private TextBox textBoxEnforcementProceedingStartDescription;
        private Panel panel16;
        private DateTimePicker dateTimePickerEnforcementProceedingStartDate;
        private Label label14;
        private GroupBox groupBoxDirectionCourtOrderBailiffs;
        private TableLayoutPanel tableLayoutPanel6;
        private Panel panel13;
        private Label label11;
        private TextBox textBoxDirectionCourtOrderBailiffsDescription;
        private Panel panel14;
        private DateTimePicker dateTimePickerDirectionCourtOrderBailiffsDate;
        private Label label12;
        private TabPage tabPageCompletionClaims;
        private GroupBox groupBoxClaimComplete;
        private TableLayoutPanel tableLayoutPanel14;
        private Panel panel29;
        private Label label27;
        private TextBox textBoxClaimCompleteDescription;
        private Panel panel30;
        private DateTimePicker dateTimePickerClaimCompleteDate;
        private Label label28;
        private GroupBox groupBoxCourtOrderCancel;
        private TableLayoutPanel tableLayoutPanel13;
        private Panel panel27;
        private Label label25;
        private TextBox textBoxCourtOrderCancelDescription;
        private Panel panel28;
        private DateTimePicker dateTimePickerCourtOrderCancelDate;
        private Label label26;
        private Label label29;
        private TextBox textBoxClaimCompleteReason;
        private Label label30;
        private TextBox textBoxDescription;
        private DataGridViewComboBoxColumn id_state_type;
        private DataGridViewTextBoxColumn date_start_state;
        private DataGridViewTextBoxColumn description;
        private TabPage tabPageAcceptedByLegalDepartment;
        private GroupBox groupBoxAcceptedByLegalDepartment;
        private TableLayoutPanel tableLayoutPanel2;
        private Panel panel3;
        private Label label3;
        private TextBox textBoxAcceptedByLegalDepartmentWho;
        private Panel panel4;
        private DateTimePicker dateTimePickerAcceptedByLegalDepartmentDate;
        private Label label4;
    }
}
