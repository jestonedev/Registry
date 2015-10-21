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
    internal partial class WarrantsViewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel14;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_warrant;
        private DataGridViewTextBoxColumn registration_num;
        private DataGridViewDateTimeColumn registration_date;
        private DataGridViewTextBoxColumn notary;
        private DataGridViewTextBoxColumn on_behalf_of;
        private DataGridViewTextBoxColumn description;
        private GroupBox groupBox32;
        private GroupBox groupBox33;
        private Label label83;
        private Label label84;
        private Label label85;
        private Label label86;
        private Label label87;
        private Label label88;
        private TextBox textBoxWarrantRegNum;
        private TextBox textBoxWarrantNotary;
        private TextBox textBoxWarrantDistrict;
        private TextBox textBoxWarrantOnBehalfOf;
        private TextBox textBoxWarrantDescription;
        private DateTimePicker dateTimePickerWarrantDate;
        private ComboBox comboBoxWarrantDocType;
        #endregion Components


        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(WarrantsViewport));
            tableLayoutPanel14 = new TableLayoutPanel();
            groupBox32 = new GroupBox();
            label88 = new Label();
            textBoxWarrantOnBehalfOf = new TextBox();
            label87 = new Label();
            textBoxWarrantDistrict = new TextBox();
            label86 = new Label();
            textBoxWarrantNotary = new TextBox();
            label85 = new Label();
            label84 = new Label();
            comboBoxWarrantDocType = new ComboBox();
            textBoxWarrantRegNum = new TextBox();
            dateTimePickerWarrantDate = new DateTimePicker();
            label83 = new Label();
            groupBox33 = new GroupBox();
            textBoxWarrantDescription = new TextBox();
            dataGridView = new DataGridView();
            id_warrant = new DataGridViewTextBoxColumn();
            registration_num = new DataGridViewTextBoxColumn();
            registration_date = new DataGridViewDateTimeColumn();
            notary = new DataGridViewTextBoxColumn();
            on_behalf_of = new DataGridViewTextBoxColumn();
            description = new DataGridViewTextBoxColumn();
            tableLayoutPanel14.SuspendLayout();
            groupBox32.SuspendLayout();
            groupBox33.SuspendLayout();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel14
            // 
            tableLayoutPanel14.ColumnCount = 2;
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel14.Controls.Add(groupBox32, 0, 0);
            tableLayoutPanel14.Controls.Add(groupBox33, 1, 0);
            tableLayoutPanel14.Controls.Add(dataGridView, 0, 1);
            tableLayoutPanel14.Dock = DockStyle.Fill;
            tableLayoutPanel14.Location = new Point(3, 3);
            tableLayoutPanel14.Name = "tableLayoutPanel14";
            tableLayoutPanel14.RowCount = 2;
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel14.Size = new Size(653, 370);
            tableLayoutPanel14.TabIndex = 0;
            // 
            // groupBox32
            // 
            groupBox32.Controls.Add(label88);
            groupBox32.Controls.Add(textBoxWarrantOnBehalfOf);
            groupBox32.Controls.Add(label87);
            groupBox32.Controls.Add(textBoxWarrantDistrict);
            groupBox32.Controls.Add(label86);
            groupBox32.Controls.Add(textBoxWarrantNotary);
            groupBox32.Controls.Add(label85);
            groupBox32.Controls.Add(label84);
            groupBox32.Controls.Add(comboBoxWarrantDocType);
            groupBox32.Controls.Add(textBoxWarrantRegNum);
            groupBox32.Controls.Add(dateTimePickerWarrantDate);
            groupBox32.Controls.Add(label83);
            groupBox32.Dock = DockStyle.Fill;
            groupBox32.Location = new Point(3, 3);
            groupBox32.Name = "groupBox32";
            groupBox32.Size = new Size(320, 194);
            groupBox32.TabIndex = 1;
            groupBox32.TabStop = false;
            groupBox32.Text = "Основные сведения";
            // 
            // label88
            // 
            label88.AutoSize = true;
            label88.Location = new Point(17, 167);
            label88.Name = "label88";
            label88.Size = new Size(138, 15);
            label88.TabIndex = 51;
            label88.Text = "Действует в лице кого";
            // 
            // textBoxWarrantOnBehalfOf
            // 
            textBoxWarrantOnBehalfOf.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                              | AnchorStyles.Right;
            textBoxWarrantOnBehalfOf.Location = new Point(175, 164);
            textBoxWarrantOnBehalfOf.MaxLength = 100;
            textBoxWarrantOnBehalfOf.Name = "textBoxWarrantOnBehalfOf";
            textBoxWarrantOnBehalfOf.Size = new Size(139, 21);
            textBoxWarrantOnBehalfOf.TabIndex = 5;
            textBoxWarrantOnBehalfOf.Enter += selectAll_Enter;
            // 
            // label87
            // 
            label87.AutoSize = true;
            label87.Location = new Point(17, 138);
            label87.Name = "label87";
            label87.Size = new Size(138, 15);
            label87.TabIndex = 49;
            label87.Text = "Нотариального округа";
            // 
            // textBoxWarrantDistrict
            // 
            textBoxWarrantDistrict.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                            | AnchorStyles.Right;
            textBoxWarrantDistrict.Location = new Point(175, 135);
            textBoxWarrantDistrict.MaxLength = 100;
            textBoxWarrantDistrict.Name = "textBoxWarrantDistrict";
            textBoxWarrantDistrict.Size = new Size(139, 21);
            textBoxWarrantDistrict.TabIndex = 4;
            textBoxWarrantDistrict.Enter += selectAll_Enter;
            // 
            // label86
            // 
            label86.AutoSize = true;
            label86.Location = new Point(17, 109);
            label86.Name = "label86";
            label86.Size = new Size(145, 15);
            label86.TabIndex = 47;
            label86.Text = "Удостовер. нотариусом";
            // 
            // textBoxWarrantNotary
            // 
            textBoxWarrantNotary.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                          | AnchorStyles.Right;
            textBoxWarrantNotary.Location = new Point(175, 106);
            textBoxWarrantNotary.MaxLength = 100;
            textBoxWarrantNotary.Name = "textBoxWarrantNotary";
            textBoxWarrantNotary.Size = new Size(139, 21);
            textBoxWarrantNotary.TabIndex = 3;
            textBoxWarrantNotary.Enter += selectAll_Enter;
            // 
            // label85
            // 
            label85.AutoSize = true;
            label85.Location = new Point(17, 51);
            label85.Name = "label85";
            label85.Size = new Size(152, 15);
            label85.TabIndex = 45;
            label85.Text = "Регистрационный номер";
            // 
            // label84
            // 
            label84.AutoSize = true;
            label84.Location = new Point(17, 23);
            label84.Name = "label84";
            label84.Size = new Size(93, 15);
            label84.TabIndex = 44;
            label84.Text = "Тип документа";
            // 
            // comboBoxWarrantDocType
            // 
            comboBoxWarrantDocType.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                            | AnchorStyles.Right;
            comboBoxWarrantDocType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxWarrantDocType.FormattingEnabled = true;
            comboBoxWarrantDocType.Location = new Point(175, 19);
            comboBoxWarrantDocType.Name = "comboBoxWarrantDocType";
            comboBoxWarrantDocType.Size = new Size(139, 23);
            comboBoxWarrantDocType.TabIndex = 0;
            // 
            // textBoxWarrantRegNum
            // 
            textBoxWarrantRegNum.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                          | AnchorStyles.Right;
            textBoxWarrantRegNum.Location = new Point(175, 48);
            textBoxWarrantRegNum.MaxLength = 10;
            textBoxWarrantRegNum.Name = "textBoxWarrantRegNum";
            textBoxWarrantRegNum.Size = new Size(139, 21);
            textBoxWarrantRegNum.TabIndex = 1;
            textBoxWarrantRegNum.Enter += selectAll_Enter;
            // 
            // dateTimePickerWarrantDate
            // 
            dateTimePickerWarrantDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                               | AnchorStyles.Right;
            dateTimePickerWarrantDate.Location = new Point(175, 77);
            dateTimePickerWarrantDate.Name = "dateTimePickerWarrantDate";
            dateTimePickerWarrantDate.Size = new Size(139, 21);
            dateTimePickerWarrantDate.TabIndex = 2;
            // 
            // label83
            // 
            label83.AutoSize = true;
            label83.Location = new Point(17, 80);
            label83.Name = "label83";
            label83.Size = new Size(37, 15);
            label83.TabIndex = 41;
            label83.Text = "Дата";
            // 
            // groupBox33
            // 
            groupBox33.Controls.Add(textBoxWarrantDescription);
            groupBox33.Dock = DockStyle.Fill;
            groupBox33.Location = new Point(329, 3);
            groupBox33.Name = "groupBox33";
            groupBox33.Size = new Size(321, 194);
            groupBox33.TabIndex = 2;
            groupBox33.TabStop = false;
            groupBox33.Text = "Дополнительные сведения";
            // 
            // textBoxWarrantDescription
            // 
            textBoxWarrantDescription.Dock = DockStyle.Fill;
            textBoxWarrantDescription.Location = new Point(3, 17);
            textBoxWarrantDescription.MaxLength = 4000;
            textBoxWarrantDescription.Multiline = true;
            textBoxWarrantDescription.Name = "textBoxWarrantDescription";
            textBoxWarrantDescription.Size = new Size(315, 174);
            textBoxWarrantDescription.TabIndex = 0;
            textBoxWarrantDescription.Enter += selectAll_Enter;
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.Fixed3D;
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
            dataGridView.Columns.AddRange(id_warrant, registration_num, registration_date, notary, on_behalf_of, description);
            tableLayoutPanel14.SetColumnSpan(dataGridView, 2);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 203);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(647, 164);
            dataGridView.TabIndex = 0;
            dataGridView.DataError += dataGridView_DataError;
            // 
            // id_warrant
            // 
            id_warrant.Frozen = true;
            id_warrant.HeaderText = "Идентификатор доверенности";
            id_warrant.Name = "id_warrant";
            id_warrant.ReadOnly = true;
            id_warrant.Visible = false;
            // 
            // registration_num
            // 
            registration_num.HeaderText = "Регистрационный №";
            registration_num.MinimumWidth = 150;
            registration_num.Name = "registration_num";
            registration_num.ReadOnly = true;
            // 
            // registration_date
            // 
            registration_date.HeaderText = "Дата";
            registration_date.MinimumWidth = 150;
            registration_date.Name = "registration_date";
            registration_date.ReadOnly = true;
            // 
            // notary
            // 
            notary.HeaderText = "Нотариус";
            notary.MinimumWidth = 200;
            notary.Name = "notary";
            notary.ReadOnly = true;
            // 
            // on_behalf_of
            // 
            on_behalf_of.HeaderText = "В лице кого";
            on_behalf_of.MinimumWidth = 200;
            on_behalf_of.Name = "on_behalf_of";
            on_behalf_of.ReadOnly = true;
            // 
            // description
            // 
            description.FillWeight = 200F;
            description.HeaderText = "Примечание";
            description.MinimumWidth = 300;
            description.Name = "description";
            description.ReadOnly = true;
            // 
            // WarrantsViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(650, 310);
            BackColor = Color.White;
            ClientSize = new Size(659, 376);
            Controls.Add(tableLayoutPanel14);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "WarrantsViewport";
            Padding = new Padding(3);
            Text = "Реестр доверенностей";
            tableLayoutPanel14.ResumeLayout(false);
            groupBox32.ResumeLayout(false);
            groupBox32.PerformLayout();
            groupBox33.ResumeLayout(false);
            groupBox33.PerformLayout();
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
