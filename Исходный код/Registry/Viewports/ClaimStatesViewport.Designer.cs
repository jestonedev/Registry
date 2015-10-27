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
        TableLayoutPanel tableLayoutPanel17;
        TableLayoutPanel tableLayoutPanel18;
        GroupBox groupBox35;
        Panel panel10;
        Panel panel11;
        Label label100;
        Label label101;
        Label label105;
        Label label108;
        Label label109;
        Label label110;
        TextBox textBoxDescription;
        TextBox textBoxDocumentNumber;
        ComboBox comboBoxClaimStateType;
        DateTimePicker dateTimePickerDocDate;
        DateTimePicker dateTimePickerStartState;
        DateTimePicker dateTimePickerEndState;
        DataGridView dataGridView;
        private DataGridViewComboBoxColumn id_state_type;
        private DataGridViewTextBoxColumn date_start_state;
        private DataGridViewTextBoxColumn date_end_state;
        private DataGridViewTextBoxColumn description;
        #endregion Components

        private void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(ClaimStatesViewport));
            tableLayoutPanel17 = new TableLayoutPanel();
            groupBox35 = new GroupBox();
            tableLayoutPanel18 = new TableLayoutPanel();
            panel10 = new Panel();
            textBoxDocumentNumber = new TextBox();
            label100 = new Label();
            textBoxDescription = new TextBox();
            label101 = new Label();
            dateTimePickerDocDate = new DateTimePicker();
            label105 = new Label();
            panel11 = new Panel();
            dateTimePickerEndState = new DateTimePicker();
            label110 = new Label();
            comboBoxClaimStateType = new ComboBox();
            dateTimePickerStartState = new DateTimePicker();
            label108 = new Label();
            label109 = new Label();
            dataGridView = new DataGridView();
            id_state_type = new DataGridViewComboBoxColumn();
            date_start_state = new DataGridViewTextBoxColumn();
            date_end_state = new DataGridViewTextBoxColumn();
            description = new DataGridViewTextBoxColumn();
            tableLayoutPanel17.SuspendLayout();
            groupBox35.SuspendLayout();
            tableLayoutPanel18.SuspendLayout();
            panel10.SuspendLayout();
            panel11.SuspendLayout();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel17
            // 
            tableLayoutPanel17.ColumnCount = 1;
            tableLayoutPanel17.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel17.Controls.Add(groupBox35, 0, 0);
            tableLayoutPanel17.Controls.Add(dataGridView, 0, 1);
            tableLayoutPanel17.Dock = DockStyle.Fill;
            tableLayoutPanel17.Location = new Point(3, 3);
            tableLayoutPanel17.Name = "tableLayoutPanel17";
            tableLayoutPanel17.RowCount = 2;
            tableLayoutPanel17.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            tableLayoutPanel17.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel17.Size = new Size(703, 205);
            tableLayoutPanel17.TabIndex = 1;
            // 
            // groupBox35
            // 
            groupBox35.Controls.Add(tableLayoutPanel18);
            groupBox35.Dock = DockStyle.Fill;
            groupBox35.Location = new Point(0, 0);
            groupBox35.Margin = new Padding(0);
            groupBox35.Name = "groupBox35";
            groupBox35.Size = new Size(703, 110);
            groupBox35.TabIndex = 1;
            groupBox35.TabStop = false;
            groupBox35.Text = @"Общие сведения";
            // 
            // tableLayoutPanel18
            // 
            tableLayoutPanel18.ColumnCount = 2;
            tableLayoutPanel18.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel18.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel18.Controls.Add(panel10, 1, 0);
            tableLayoutPanel18.Controls.Add(panel11, 0, 0);
            tableLayoutPanel18.Dock = DockStyle.Fill;
            tableLayoutPanel18.Location = new Point(3, 17);
            tableLayoutPanel18.Name = "tableLayoutPanel18";
            tableLayoutPanel18.RowCount = 1;
            tableLayoutPanel18.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel18.RowStyles.Add(new RowStyle(SizeType.Absolute, 148F));
            tableLayoutPanel18.Size = new Size(697, 90);
            tableLayoutPanel18.TabIndex = 0;
            // 
            // panel10
            // 
            panel10.Controls.Add(textBoxDocumentNumber);
            panel10.Controls.Add(label100);
            panel10.Controls.Add(textBoxDescription);
            panel10.Controls.Add(label101);
            panel10.Controls.Add(dateTimePickerDocDate);
            panel10.Controls.Add(label105);
            panel10.Dock = DockStyle.Fill;
            panel10.Location = new Point(348, 0);
            panel10.Margin = new Padding(0);
            panel10.Name = "panel10";
            panel10.Size = new Size(349, 90);
            panel10.TabIndex = 1;
            // 
            // textBoxDocumentNumber
            // 
            textBoxDocumentNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                           | AnchorStyles.Right;
            textBoxDocumentNumber.Location = new Point(161, 4);
            textBoxDocumentNumber.MaxLength = 50;
            textBoxDocumentNumber.Name = "textBoxDocumentNumber";
            textBoxDocumentNumber.Size = new Size(179, 21);
            textBoxDocumentNumber.TabIndex = 0;
            // 
            // label100
            // 
            label100.AutoSize = true;
            label100.Location = new Point(12, 7);
            label100.Name = "label100";
            label100.Size = new Size(111, 15);
            label100.TabIndex = 51;
            label100.Text = @"Номер документа";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                        | AnchorStyles.Right;
            textBoxDescription.Location = new Point(161, 62);
            textBoxDescription.MaxLength = 4000;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(179, 21);
            textBoxDescription.TabIndex = 2;
            // 
            // label101
            // 
            label101.AutoSize = true;
            label101.Location = new Point(12, 65);
            label101.Name = "label101";
            label101.Size = new Size(80, 15);
            label101.TabIndex = 49;
            label101.Text = @"Примечание";
            // 
            // dateTimePickerDocDate
            // 
            dateTimePickerDocDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                           | AnchorStyles.Right;
            dateTimePickerDocDate.Location = new Point(161, 33);
            dateTimePickerDocDate.Name = "dateTimePickerDocDate";
            dateTimePickerDocDate.ShowCheckBox = true;
            dateTimePickerDocDate.Size = new Size(179, 21);
            dateTimePickerDocDate.TabIndex = 1;
            // 
            // label105
            // 
            label105.AutoSize = true;
            label105.Location = new Point(12, 36);
            label105.Name = "label105";
            label105.Size = new Size(102, 15);
            label105.TabIndex = 44;
            label105.Text = @"Дата документа";
            // 
            // panel11
            // 
            panel11.Controls.Add(dateTimePickerEndState);
            panel11.Controls.Add(label110);
            panel11.Controls.Add(comboBoxClaimStateType);
            panel11.Controls.Add(dateTimePickerStartState);
            panel11.Controls.Add(label108);
            panel11.Controls.Add(label109);
            panel11.Dock = DockStyle.Fill;
            panel11.Location = new Point(0, 0);
            panel11.Margin = new Padding(0);
            panel11.Name = "panel11";
            panel11.Size = new Size(348, 90);
            panel11.TabIndex = 0;
            // 
            // dateTimePickerEndState
            // 
            dateTimePickerEndState.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                            | AnchorStyles.Right;
            dateTimePickerEndState.Location = new Point(161, 62);
            dateTimePickerEndState.Name = "dateTimePickerEndState";
            dateTimePickerEndState.ShowCheckBox = true;
            dateTimePickerEndState.Size = new Size(178, 21);
            dateTimePickerEndState.TabIndex = 2;
            // 
            // label110
            // 
            label110.AutoSize = true;
            label110.Location = new Point(14, 65);
            label110.Name = "label110";
            label110.Size = new Size(86, 15);
            label110.TabIndex = 38;
            label110.Text = @"Крайний срок";
            // 
            // comboBoxClaimStateType
            // 
            comboBoxClaimStateType.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                            | AnchorStyles.Right;
            comboBoxClaimStateType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxClaimStateType.FormattingEnabled = true;
            comboBoxClaimStateType.Location = new Point(161, 4);
            comboBoxClaimStateType.Name = "comboBoxClaimStateType";
            comboBoxClaimStateType.Size = new Size(178, 23);
            comboBoxClaimStateType.TabIndex = 0;
            // 
            // dateTimePickerStartState
            // 
            dateTimePickerStartState.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                              | AnchorStyles.Right;
            dateTimePickerStartState.Location = new Point(161, 33);
            dateTimePickerStartState.Name = "dateTimePickerStartState";
            dateTimePickerStartState.ShowCheckBox = true;
            dateTimePickerStartState.Size = new Size(178, 21);
            dateTimePickerStartState.TabIndex = 1;
            // 
            // label108
            // 
            label108.AutoSize = true;
            label108.Location = new Point(14, 36);
            label108.Name = "label108";
            label108.Size = new Size(99, 15);
            label108.TabIndex = 31;
            label108.Text = @"Дата установки";
            // 
            // label109
            // 
            label109.AutoSize = true;
            label109.Location = new Point(14, 7);
            label109.Name = "label109";
            label109.Size = new Size(93, 15);
            label109.TabIndex = 29;
            label109.Text = @"Вид состояния";
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_state_type, date_start_state, date_end_state, description);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 113);
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(697, 89);
            dataGridView.TabIndex = 0;
            // 
            // id_state_type
            // 
            id_state_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_state_type.HeaderText = @"Вид состояния";
            id_state_type.MinimumWidth = 150;
            id_state_type.Name = "id_state_type";
            id_state_type.ReadOnly = true;
            id_state_type.Resizable = DataGridViewTriState.True;
            id_state_type.SortMode = DataGridViewColumnSortMode.Automatic;
            id_state_type.Width = 150;
            // 
            // date_start_state
            // 
            date_start_state.HeaderText = @"Дата установки";
            date_start_state.MinimumWidth = 150;
            date_start_state.Name = "date_start_state";
            date_start_state.Width = 150;
            // 
            // date_end_state
            // 
            date_end_state.HeaderText = @"Крайний срок";
            date_end_state.MinimumWidth = 150;
            date_end_state.Name = "date_end_state";
            date_end_state.ReadOnly = true;
            date_end_state.Width = 150;
            // 
            // description
            // 
            description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            description.HeaderText = @"Примечание";
            description.MinimumWidth = 200;
            description.Name = "description";
            description.ReadOnly = true;
            // 
            // ClaimStatesViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(700, 190);
            AutoSize = true;
            BackColor = Color.White;
            ClientSize = new Size(709, 211);
            Controls.Add(tableLayoutPanel17);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "ClaimStatesViewport";
            Padding = new Padding(3);
            Text = @"Состояния иск. работы №{0}";
            tableLayoutPanel17.ResumeLayout(false);
            groupBox35.ResumeLayout(false);
            tableLayoutPanel18.ResumeLayout(false);
            panel10.ResumeLayout(false);
            panel10.PerformLayout();
            panel11.ResumeLayout(false);
            panel11.PerformLayout();
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
