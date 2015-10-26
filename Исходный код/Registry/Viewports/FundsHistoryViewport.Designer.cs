using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CustomControls;

namespace Registry.Viewport
{
    internal partial class FundsHistoryViewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanel7;
        private TableLayoutPanel tableLayoutPanel8;
        private GroupBox groupBox14;
        private GroupBox groupBox15;
        private GroupBox groupBox16;
        private GroupBox groupBox17;
        private Label label29;
        private Label label30;
        private Label label31;
        private Label label32;
        private Label label33;
        private Label label34;
        private Label label35;
        private Label label36;
        private Label label37;
        private DateTimePicker dateTimePickerProtocolDate;
        private TextBox textBoxProtocolNumber;
        private ComboBox comboBoxFundType;
        private DataGridView dataGridView;
        private CheckBox checkBoxIncludeRest;
        private CheckBox checkBoxExcludeRest;
        private TextBox textBoxIncludeRestDesc;
        private TextBox textBoxExcludeRestDesc;
        private TextBox textBoxIncludeRestNum;
        private TextBox textBoxExcludeRestNum;
        private DateTimePicker dateTimePickerIncludeRestDate;
        private DateTimePicker dateTimePickerExcludeRestDate;
        private TextBox textBoxDescription;
        private DataGridViewTextBoxColumn id_fund;
        private DataGridViewTextBoxColumn protocol_number;
        private DataGridViewDateTimeColumn protocol_date;
        private DataGridViewComboBoxColumn id_fund_type;
        #endregion Components



        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(FundsHistoryViewport));
            tableLayoutPanel6 = new TableLayoutPanel();
            tableLayoutPanel8 = new TableLayoutPanel();
            groupBox14 = new GroupBox();
            dateTimePickerProtocolDate = new DateTimePicker();
            label37 = new Label();
            label36 = new Label();
            textBoxProtocolNumber = new TextBox();
            comboBoxFundType = new ComboBox();
            label35 = new Label();
            groupBox17 = new GroupBox();
            textBoxDescription = new TextBox();
            tableLayoutPanel7 = new TableLayoutPanel();
            groupBox15 = new GroupBox();
            checkBoxIncludeRest = new CheckBox();
            label31 = new Label();
            textBoxIncludeRestDesc = new TextBox();
            dateTimePickerIncludeRestDate = new DateTimePicker();
            label30 = new Label();
            label29 = new Label();
            textBoxIncludeRestNum = new TextBox();
            groupBox16 = new GroupBox();
            checkBoxExcludeRest = new CheckBox();
            label32 = new Label();
            textBoxExcludeRestDesc = new TextBox();
            dateTimePickerExcludeRestDate = new DateTimePicker();
            label33 = new Label();
            label34 = new Label();
            textBoxExcludeRestNum = new TextBox();
            dataGridView = new DataGridView();
            id_fund = new DataGridViewTextBoxColumn();
            protocol_number = new DataGridViewTextBoxColumn();
            protocol_date = new DataGridViewDateTimeColumn();
            id_fund_type = new DataGridViewComboBoxColumn();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            groupBox14.SuspendLayout();
            groupBox17.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            groupBox15.SuspendLayout();
            groupBox16.SuspendLayout();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Controls.Add(tableLayoutPanel8, 0, 0);
            tableLayoutPanel6.Controls.Add(tableLayoutPanel7, 1, 0);
            tableLayoutPanel6.Controls.Add(dataGridView, 0, 1);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Absolute, 230F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.Size = new Size(708, 336);
            tableLayoutPanel6.TabIndex = 0;
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.ColumnCount = 1;
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel8.Controls.Add(groupBox14, 0, 0);
            tableLayoutPanel8.Controls.Add(groupBox17, 0, 1);
            tableLayoutPanel8.Dock = DockStyle.Fill;
            tableLayoutPanel8.Location = new Point(3, 3);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 2;
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.Size = new Size(348, 224);
            tableLayoutPanel8.TabIndex = 1;
            // 
            // groupBox14
            // 
            groupBox14.Controls.Add(dateTimePickerProtocolDate);
            groupBox14.Controls.Add(label37);
            groupBox14.Controls.Add(label36);
            groupBox14.Controls.Add(textBoxProtocolNumber);
            groupBox14.Controls.Add(comboBoxFundType);
            groupBox14.Controls.Add(label35);
            groupBox14.Dock = DockStyle.Fill;
            groupBox14.Location = new Point(3, 3);
            groupBox14.Name = "groupBox14";
            groupBox14.Size = new Size(342, 106);
            groupBox14.TabIndex = 0;
            groupBox14.TabStop = false;
            groupBox14.Text = @"Общие сведения";
            // 
            // dateTimePickerProtocolDate
            // 
            dateTimePickerProtocolDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                | AnchorStyles.Right;
            dateTimePickerProtocolDate.Enabled = false;
            dateTimePickerProtocolDate.Location = new Point(161, 77);
            dateTimePickerProtocolDate.Name = "dateTimePickerProtocolDate";
            dateTimePickerProtocolDate.ShowCheckBox = true;
            dateTimePickerProtocolDate.Size = new Size(175, 21);
            dateTimePickerProtocolDate.TabIndex = 2;
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Location = new Point(14, 80);
            label37.Name = "label37";
            label37.Size = new Size(124, 15);
            label37.TabIndex = 3;
            label37.Text = @"Дата протокола ЖК";
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.Location = new Point(14, 54);
            label36.Name = "label36";
            label36.Size = new Size(133, 15);
            label36.TabIndex = 4;
            label36.Text = @"Номер протокола ЖК";
            // 
            // textBoxProtocolNumber
            // 
            textBoxProtocolNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                           | AnchorStyles.Right;
            textBoxProtocolNumber.Enabled = false;
            textBoxProtocolNumber.Location = new Point(161, 51);
            textBoxProtocolNumber.MaxLength = 50;
            textBoxProtocolNumber.Name = "textBoxProtocolNumber";
            textBoxProtocolNumber.Size = new Size(175, 21);
            textBoxProtocolNumber.TabIndex = 1;
            textBoxProtocolNumber.Enter += selectAll_Enter;
            // 
            // comboBoxFundType
            // 
            comboBoxFundType.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                      | AnchorStyles.Right;
            comboBoxFundType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFundType.FormattingEnabled = true;
            comboBoxFundType.Location = new Point(161, 24);
            comboBoxFundType.Name = "comboBoxFundType";
            comboBoxFundType.Size = new Size(175, 23);
            comboBoxFundType.TabIndex = 0;
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Location = new Point(14, 27);
            label35.Name = "label35";
            label35.Size = new Size(68, 15);
            label35.TabIndex = 5;
            label35.Text = @"Тип найма";
            // 
            // groupBox17
            // 
            groupBox17.Controls.Add(textBoxDescription);
            groupBox17.Dock = DockStyle.Fill;
            groupBox17.Location = new Point(3, 115);
            groupBox17.Name = "groupBox17";
            groupBox17.Size = new Size(342, 106);
            groupBox17.TabIndex = 1;
            groupBox17.TabStop = false;
            groupBox17.Text = @"Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.Location = new Point(3, 17);
            textBoxDescription.MaxLength = 4000;
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(336, 86);
            textBoxDescription.TabIndex = 4;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 1;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel7.Controls.Add(groupBox15, 0, 0);
            tableLayoutPanel7.Controls.Add(groupBox16, 0, 1);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(357, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 2;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.Size = new Size(348, 224);
            tableLayoutPanel7.TabIndex = 1;
            // 
            // groupBox15
            // 
            groupBox15.Controls.Add(checkBoxIncludeRest);
            groupBox15.Controls.Add(label31);
            groupBox15.Controls.Add(textBoxIncludeRestDesc);
            groupBox15.Controls.Add(dateTimePickerIncludeRestDate);
            groupBox15.Controls.Add(label30);
            groupBox15.Controls.Add(label29);
            groupBox15.Controls.Add(textBoxIncludeRestNum);
            groupBox15.Dock = DockStyle.Fill;
            groupBox15.Location = new Point(3, 3);
            groupBox15.Name = "groupBox15";
            groupBox15.Size = new Size(342, 106);
            groupBox15.TabIndex = 0;
            groupBox15.TabStop = false;
            groupBox15.Text = @"      Реквизиты НПА по включению в фонд";
            // 
            // checkBoxIncludeRest
            // 
            checkBoxIncludeRest.AutoSize = true;
            checkBoxIncludeRest.Location = new Point(11, 0);
            checkBoxIncludeRest.Name = "checkBoxIncludeRest";
            checkBoxIncludeRest.Size = new Size(15, 14);
            checkBoxIncludeRest.TabIndex = 5;
            checkBoxIncludeRest.UseVisualStyleBackColor = true;
            checkBoxIncludeRest.CheckedChanged += checkBoxIncludeRest_CheckedChanged;
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Location = new Point(8, 77);
            label31.Name = "label31";
            label31.Size = new Size(95, 15);
            label31.TabIndex = 6;
            label31.Text = @"Наименование";
            // 
            // textBoxIncludeRestDesc
            // 
            textBoxIncludeRestDesc.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                            | AnchorStyles.Right;
            textBoxIncludeRestDesc.Enabled = false;
            textBoxIncludeRestDesc.Location = new Point(161, 74);
            textBoxIncludeRestDesc.MaxLength = 255;
            textBoxIncludeRestDesc.Name = "textBoxIncludeRestDesc";
            textBoxIncludeRestDesc.Size = new Size(175, 21);
            textBoxIncludeRestDesc.TabIndex = 8;
            textBoxIncludeRestDesc.Enter += selectAll_Enter;
            // 
            // dateTimePickerIncludeRestDate
            // 
            dateTimePickerIncludeRestDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                   | AnchorStyles.Right;
            dateTimePickerIncludeRestDate.Enabled = false;
            dateTimePickerIncludeRestDate.Location = new Point(161, 48);
            dateTimePickerIncludeRestDate.Name = "dateTimePickerIncludeRestDate";
            dateTimePickerIncludeRestDate.Size = new Size(175, 21);
            dateTimePickerIncludeRestDate.TabIndex = 7;
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Location = new Point(8, 52);
            label30.Name = "label30";
            label30.Size = new Size(101, 15);
            label30.TabIndex = 9;
            label30.Text = @"Дата реквизита";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(8, 25);
            label29.Name = "label29";
            label29.Size = new Size(110, 15);
            label29.TabIndex = 10;
            label29.Text = @"Номер реквизита";
            // 
            // textBoxIncludeRestNum
            // 
            textBoxIncludeRestNum.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                           | AnchorStyles.Right;
            textBoxIncludeRestNum.Enabled = false;
            textBoxIncludeRestNum.Location = new Point(161, 22);
            textBoxIncludeRestNum.MaxLength = 30;
            textBoxIncludeRestNum.Name = "textBoxIncludeRestNum";
            textBoxIncludeRestNum.Size = new Size(175, 21);
            textBoxIncludeRestNum.TabIndex = 6;
            textBoxIncludeRestNum.Enter += selectAll_Enter;
            // 
            // groupBox16
            // 
            groupBox16.Controls.Add(checkBoxExcludeRest);
            groupBox16.Controls.Add(label32);
            groupBox16.Controls.Add(textBoxExcludeRestDesc);
            groupBox16.Controls.Add(dateTimePickerExcludeRestDate);
            groupBox16.Controls.Add(label33);
            groupBox16.Controls.Add(label34);
            groupBox16.Controls.Add(textBoxExcludeRestNum);
            groupBox16.Dock = DockStyle.Fill;
            groupBox16.Location = new Point(3, 115);
            groupBox16.Name = "groupBox16";
            groupBox16.Size = new Size(342, 106);
            groupBox16.TabIndex = 1;
            groupBox16.TabStop = false;
            groupBox16.Text = @"      Реквизиты НПА по исключению из фонда";
            // 
            // checkBoxExcludeRest
            // 
            checkBoxExcludeRest.AutoSize = true;
            checkBoxExcludeRest.Location = new Point(11, 0);
            checkBoxExcludeRest.Name = "checkBoxExcludeRest";
            checkBoxExcludeRest.Size = new Size(15, 14);
            checkBoxExcludeRest.TabIndex = 9;
            checkBoxExcludeRest.UseVisualStyleBackColor = true;
            checkBoxExcludeRest.CheckedChanged += checkBoxExcludeRest_CheckedChanged;
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Location = new Point(8, 76);
            label32.Name = "label32";
            label32.Size = new Size(95, 15);
            label32.TabIndex = 10;
            label32.Text = @"Наименование";
            // 
            // textBoxExcludeRestDesc
            // 
            textBoxExcludeRestDesc.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                            | AnchorStyles.Right;
            textBoxExcludeRestDesc.Enabled = false;
            textBoxExcludeRestDesc.Location = new Point(161, 73);
            textBoxExcludeRestDesc.MaxLength = 255;
            textBoxExcludeRestDesc.Name = "textBoxExcludeRestDesc";
            textBoxExcludeRestDesc.Size = new Size(175, 21);
            textBoxExcludeRestDesc.TabIndex = 12;
            textBoxExcludeRestDesc.Enter += selectAll_Enter;
            // 
            // dateTimePickerExcludeRestDate
            // 
            dateTimePickerExcludeRestDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                   | AnchorStyles.Right;
            dateTimePickerExcludeRestDate.Enabled = false;
            dateTimePickerExcludeRestDate.Location = new Point(161, 47);
            dateTimePickerExcludeRestDate.Name = "dateTimePickerExcludeRestDate";
            dateTimePickerExcludeRestDate.Size = new Size(175, 21);
            dateTimePickerExcludeRestDate.TabIndex = 11;
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Location = new Point(8, 51);
            label33.Name = "label33";
            label33.Size = new Size(101, 15);
            label33.TabIndex = 13;
            label33.Text = @"Дата реквизита";
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Location = new Point(8, 24);
            label34.Name = "label34";
            label34.Size = new Size(110, 15);
            label34.TabIndex = 14;
            label34.Text = @"Номер реквизита";
            // 
            // textBoxExcludeRestNum
            // 
            textBoxExcludeRestNum.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                           | AnchorStyles.Right;
            textBoxExcludeRestNum.Enabled = false;
            textBoxExcludeRestNum.Location = new Point(161, 21);
            textBoxExcludeRestNum.MaxLength = 30;
            textBoxExcludeRestNum.Name = "textBoxExcludeRestNum";
            textBoxExcludeRestNum.Size = new Size(175, 21);
            textBoxExcludeRestNum.TabIndex = 10;
            textBoxExcludeRestNum.Enter += selectAll_Enter;
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_fund, protocol_number, protocol_date, id_fund_type);
            tableLayoutPanel6.SetColumnSpan(dataGridView, 2);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 233);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(702, 100);
            dataGridView.TabIndex = 0;
            dataGridView.DataError += dataGridView_DataError;
            // 
            // id_fund
            // 
            id_fund.HeaderText = @"Идентификатор фонда";
            id_fund.Name = "id_fund";
            id_fund.ReadOnly = true;
            id_fund.Visible = false;
            // 
            // protocol_number
            // 
            protocol_number.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            protocol_number.HeaderText = @"Номер протокола";
            protocol_number.MinimumWidth = 150;
            protocol_number.Name = "protocol_number";
            protocol_number.ReadOnly = true;
            protocol_number.SortMode = DataGridViewColumnSortMode.NotSortable;
            protocol_number.Width = 150;
            // 
            // protocol_date
            // 
            protocol_date.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            protocol_date.HeaderText = @"Дата протокола";
            protocol_date.MinimumWidth = 150;
            protocol_date.Name = "protocol_date";
            protocol_date.ReadOnly = true;
            protocol_date.Width = 150;
            // 
            // id_fund_type
            // 
            id_fund_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            id_fund_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_fund_type.HeaderText = @"Наименование";
            id_fund_type.MinimumWidth = 250;
            id_fund_type.Name = "id_fund_type";
            id_fund_type.ReadOnly = true;
            // 
            // FundsHistoryViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(640, 320);
            BackColor = Color.White;
            ClientSize = new Size(714, 342);
            Controls.Add(tableLayoutPanel6);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "FundsHistoryViewport";
            Padding = new Padding(3);
            Text = @"История фонда";
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel8.ResumeLayout(false);
            groupBox14.ResumeLayout(false);
            groupBox14.PerformLayout();
            groupBox17.ResumeLayout(false);
            groupBox17.PerformLayout();
            tableLayoutPanel7.ResumeLayout(false);
            groupBox15.ResumeLayout(false);
            groupBox15.PerformLayout();
            groupBox16.ResumeLayout(false);
            groupBox16.PerformLayout();
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
