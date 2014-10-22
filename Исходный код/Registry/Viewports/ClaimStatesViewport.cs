using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal sealed class ClaimStatesViewport: Viewport
    {
        #region Components
        TableLayoutPanel tableLayoutPanel17 = new TableLayoutPanel();
        TableLayoutPanel tableLayoutPanel18 = new TableLayoutPanel();
        GroupBox groupBox35 = new GroupBox();
        Panel panel10 = new Panel();
        Panel panel11 = new Panel();
        Label label100 = new Label();
        Label label101 = new Label();
        Label label105 = new Label();
        Label label108 = new Label();
        Label label109 = new Label();
        Label label110 = new Label();
        TextBox textBoxClaimStateDescription = new TextBox();
        TextBox textBoxDocNumber = new TextBox();
        ComboBox comboBoxClaimStateType = new ComboBox();
        DateTimePicker dateTimePickerDocDate = new DateTimePicker();
        DateTimePicker dateTimePickerStartState = new DateTimePicker();
        DateTimePicker dateTimePickerEndState = new DateTimePicker();
        DataGridView dataGridViewClaimStates = new DataGridView();            
        DataGridViewComboBoxColumn field_id_state_type = new DataGridViewComboBoxColumn();
        DataGridViewTextBoxColumn field_date_start_state = new DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn field_date_end_state = new DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn field_description = new DataGridViewTextBoxColumn();
        #endregion Components

        public ClaimStatesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageClaimStates";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Состояния иск. работы №{0}";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        private void ConstructViewport()
        {
            this.Controls.Add(tableLayoutPanel17);
            this.SuspendLayout();
            this.tableLayoutPanel17.SuspendLayout();
            this.tableLayoutPanel18.SuspendLayout();
            this.groupBox35.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStates)).BeginInit();
            // 
            // tableLayoutPanel17
            // 
            this.tableLayoutPanel17.ColumnCount = 1;
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel17.Controls.Add(this.groupBox35, 0, 0);
            this.tableLayoutPanel17.Controls.Add(this.dataGridViewClaimStates, 0, 1);
            this.tableLayoutPanel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel17.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel17.Name = "tableLayoutPanel17";
            this.tableLayoutPanel17.RowCount = 2;
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel17.Size = new System.Drawing.Size(984, 531);
            this.tableLayoutPanel17.TabIndex = 1;
            // 
            // groupBox35
            // 
            this.groupBox35.Controls.Add(this.tableLayoutPanel18);
            this.groupBox35.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox35.Location = new System.Drawing.Point(0, 0);
            this.groupBox35.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox35.Name = "groupBox35";
            this.groupBox35.Size = new System.Drawing.Size(984, 110);
            this.groupBox35.TabIndex = 0;
            this.groupBox35.TabStop = false;
            this.groupBox35.Text = "Общие сведения";
            // 
            // tableLayoutPanel18
            // 
            this.tableLayoutPanel18.ColumnCount = 2;
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel18.Controls.Add(this.panel10, 1, 0);
            this.tableLayoutPanel18.Controls.Add(this.panel11, 0, 0);
            this.tableLayoutPanel18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel18.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel18.Name = "tableLayoutPanel18";
            this.tableLayoutPanel18.RowCount = 1;
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 148F));
            this.tableLayoutPanel18.Size = new System.Drawing.Size(978, 91);
            this.tableLayoutPanel18.TabIndex = 0;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.textBoxDocNumber);
            this.panel10.Controls.Add(this.label100);
            this.panel10.Controls.Add(this.textBoxClaimStateDescription);
            this.panel10.Controls.Add(this.label101);
            this.panel10.Controls.Add(this.dateTimePickerDocDate);
            this.panel10.Controls.Add(this.label105);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(489, 0);
            this.panel10.Margin = new System.Windows.Forms.Padding(0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(489, 91);
            this.panel10.TabIndex = 1;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.dateTimePickerEndState);
            this.panel11.Controls.Add(this.label110);
            this.panel11.Controls.Add(this.comboBoxClaimStateType);
            this.panel11.Controls.Add(this.dateTimePickerStartState);
            this.panel11.Controls.Add(this.label108);
            this.panel11.Controls.Add(this.label109);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Margin = new System.Windows.Forms.Padding(0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(489, 91);
            this.panel11.TabIndex = 0;
            // 
            // label100
            // 
            this.label100.AutoSize = true;
            this.label100.Location = new System.Drawing.Point(12, 7);
            this.label100.Name = "label100";
            this.label100.Size = new System.Drawing.Size(98, 13);
            this.label100.TabIndex = 51;
            this.label100.Text = "Номер документа";
            // 
            // label101
            // 
            this.label101.AutoSize = true;
            this.label101.Location = new System.Drawing.Point(12, 65);
            this.label101.Name = "label101";
            this.label101.Size = new System.Drawing.Size(70, 13);
            this.label101.TabIndex = 49;
            this.label101.Text = "Примечание";
            // 
            // label105
            // 
            this.label105.AutoSize = true;
            this.label105.Location = new System.Drawing.Point(12, 36);
            this.label105.Name = "label105";
            this.label105.Size = new System.Drawing.Size(90, 13);
            this.label105.TabIndex = 44;
            this.label105.Text = "Дата документа";
            // 
            // label108
            // 
            this.label108.AutoSize = true;
            this.label108.Location = new System.Drawing.Point(14, 36);
            this.label108.Name = "label108";
            this.label108.Size = new System.Drawing.Size(88, 13);
            this.label108.TabIndex = 31;
            this.label108.Text = "Дата установки";
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
            // label110
            // 
            this.label110.AutoSize = true;
            this.label110.Location = new System.Drawing.Point(14, 65);
            this.label110.Name = "label110";
            this.label110.Size = new System.Drawing.Size(77, 13);
            this.label110.TabIndex = 38;
            this.label110.Text = "Крайний срок";
            // 
            // textBoxDocNumber
            // 
            this.textBoxDocNumber.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocNumber.Location = new System.Drawing.Point(161, 4);
            this.textBoxDocNumber.MaxLength = 50;
            this.textBoxDocNumber.Name = "textBoxDocNumber";
            this.textBoxDocNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxDocNumber.TabIndex = 0;
            // 
            // textBoxClaimStateDescription
            // 
            this.textBoxClaimStateDescription.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxClaimStateDescription.Location = new System.Drawing.Point(161, 62);
            this.textBoxClaimStateDescription.MaxLength = 4000;
            this.textBoxClaimStateDescription.Name = "textBoxClaimStateDescription";
            this.textBoxClaimStateDescription.Size = new System.Drawing.Size(319, 20);
            this.textBoxClaimStateDescription.TabIndex = 2;
            // 
            // comboBoxClaimStateType
            // 
            this.comboBoxClaimStateType.Anchor =
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxClaimStateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxClaimStateType.FormattingEnabled = true;
            this.comboBoxClaimStateType.Location = new System.Drawing.Point(161, 4);
            this.comboBoxClaimStateType.Name = "comboBoxClaimStateType";
            this.comboBoxClaimStateType.Size = new System.Drawing.Size(319, 21);
            this.comboBoxClaimStateType.TabIndex = 0;
            // 
            // dateTimePickerDocDate
            // 
            this.dateTimePickerDocDate.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDocDate.Location = new System.Drawing.Point(161, 33);
            this.dateTimePickerDocDate.Name = "dateTimePickerDocDate";
            this.dateTimePickerDocDate.ShowCheckBox = true;
            this.dateTimePickerDocDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerDocDate.TabIndex = 1;
            // 
            // dateTimePickerStartState
            // 
            this.dateTimePickerStartState.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartState.Location = new System.Drawing.Point(161, 33);
            this.dateTimePickerStartState.Name = "dateTimePickerStartState";
            this.dateTimePickerStartState.ShowCheckBox = true;
            this.dateTimePickerStartState.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerStartState.TabIndex = 1;
            // 
            // dataGridViewClaimStates
            // 
            this.dataGridViewClaimStates.AllowUserToAddRows = false;
            this.dataGridViewClaimStates.AllowUserToDeleteRows = false;
            this.dataGridViewClaimStates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaimStates.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_state_type,
            this.field_date_start_state,
            this.field_date_end_state,
            this.field_description});
            this.dataGridViewClaimStates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaimStates.Location = new System.Drawing.Point(3, 113);
            this.dataGridViewClaimStates.Name = "dataGridViewClaimStates";
            this.dataGridViewClaimStates.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewClaimStates.Size = new System.Drawing.Size(978, 415);
            this.dataGridViewClaimStates.TabIndex = 1;
            // 
            // dateTimePickerEndState
            // 
            this.dateTimePickerEndState.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndState.Location = new System.Drawing.Point(161, 62);
            this.dateTimePickerEndState.Name = "dateTimePickerEndState";
            this.dateTimePickerEndState.ShowCheckBox = true;
            this.dateTimePickerEndState.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerEndState.TabIndex = 2;
            // 
            // field_id_state_type
            // 
            this.field_id_state_type.HeaderText = "Вид состояния";
            this.field_id_state_type.MinimumWidth = 150;
            this.field_id_state_type.Name = "field_id_state_type";
            this.field_id_state_type.ReadOnly = true;
            this.field_id_state_type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.field_id_state_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.field_id_state_type.Width = 150;
            // 
            // field_date_start_state
            // 
            this.field_date_start_state.HeaderText = "Дата установки";
            this.field_date_start_state.MinimumWidth = 150;
            this.field_date_start_state.Name = "field_date_start_state";
            this.field_date_start_state.Width = 150;
            // 
            // field_date_end_state
            // 
            this.field_date_end_state.HeaderText = "Крайний срок";
            this.field_date_end_state.MinimumWidth = 150;
            this.field_date_end_state.Name = "field_date_end_state";
            this.field_date_end_state.ReadOnly = true;
            this.field_date_end_state.Width = 150;
            // 
            // field_description
            // 
            this.field_description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.field_description.HeaderText = "Примечание";
            this.field_description.MinimumWidth = 200;
            this.field_description.Name = "dataGridViewTextBoxColumn6";
            this.field_description.ReadOnly = true;

            this.tableLayoutPanel17.ResumeLayout(false);
            this.tableLayoutPanel18.ResumeLayout(false);
            this.groupBox35.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStates)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
