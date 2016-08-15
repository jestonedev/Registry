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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarrantsViewport));
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.label88 = new System.Windows.Forms.Label();
            this.textBoxWarrantOnBehalfOf = new System.Windows.Forms.TextBox();
            this.label87 = new System.Windows.Forms.Label();
            this.textBoxWarrantDistrict = new System.Windows.Forms.TextBox();
            this.label86 = new System.Windows.Forms.Label();
            this.textBoxWarrantNotary = new System.Windows.Forms.TextBox();
            this.label85 = new System.Windows.Forms.Label();
            this.label84 = new System.Windows.Forms.Label();
            this.comboBoxWarrantDocType = new System.Windows.Forms.ComboBox();
            this.textBoxWarrantRegNum = new System.Windows.Forms.TextBox();
            this.dateTimePickerWarrantDate = new System.Windows.Forms.DateTimePicker();
            this.label83 = new System.Windows.Forms.Label();
            this.groupBox33 = new System.Windows.Forms.GroupBox();
            this.textBoxWarrantDescription = new System.Windows.Forms.TextBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_warrant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_date = new CustomControls.DataGridViewDateTimeColumn();
            this.notary = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.on_behalf_of = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel14.SuspendLayout();
            this.groupBox32.SuspendLayout();
            this.groupBox33.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.ColumnCount = 2;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.Controls.Add(this.groupBox32, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.groupBox33, 1, 0);
            this.tableLayoutPanel14.Controls.Add(this.dataGridView, 0, 1);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 2;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(653, 370);
            this.tableLayoutPanel14.TabIndex = 0;
            // 
            // groupBox32
            // 
            this.groupBox32.Controls.Add(this.label88);
            this.groupBox32.Controls.Add(this.textBoxWarrantOnBehalfOf);
            this.groupBox32.Controls.Add(this.label87);
            this.groupBox32.Controls.Add(this.textBoxWarrantDistrict);
            this.groupBox32.Controls.Add(this.label86);
            this.groupBox32.Controls.Add(this.textBoxWarrantNotary);
            this.groupBox32.Controls.Add(this.label85);
            this.groupBox32.Controls.Add(this.label84);
            this.groupBox32.Controls.Add(this.comboBoxWarrantDocType);
            this.groupBox32.Controls.Add(this.textBoxWarrantRegNum);
            this.groupBox32.Controls.Add(this.dateTimePickerWarrantDate);
            this.groupBox32.Controls.Add(this.label83);
            this.groupBox32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox32.Location = new System.Drawing.Point(3, 3);
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.Size = new System.Drawing.Size(320, 194);
            this.groupBox32.TabIndex = 1;
            this.groupBox32.TabStop = false;
            this.groupBox32.Text = "Основные сведения";
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Location = new System.Drawing.Point(17, 167);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(138, 15);
            this.label88.TabIndex = 51;
            this.label88.Text = "Действует в лице кого";
            // 
            // textBoxWarrantOnBehalfOf
            // 
            this.textBoxWarrantOnBehalfOf.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWarrantOnBehalfOf.Location = new System.Drawing.Point(175, 164);
            this.textBoxWarrantOnBehalfOf.MaxLength = 100;
            this.textBoxWarrantOnBehalfOf.Name = "textBoxWarrantOnBehalfOf";
            this.textBoxWarrantOnBehalfOf.Size = new System.Drawing.Size(139, 21);
            this.textBoxWarrantOnBehalfOf.TabIndex = 5;
            this.textBoxWarrantOnBehalfOf.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Location = new System.Drawing.Point(17, 138);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(138, 15);
            this.label87.TabIndex = 49;
            this.label87.Text = "Нотариального округа";
            // 
            // textBoxWarrantDistrict
            // 
            this.textBoxWarrantDistrict.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWarrantDistrict.Location = new System.Drawing.Point(175, 135);
            this.textBoxWarrantDistrict.MaxLength = 100;
            this.textBoxWarrantDistrict.Name = "textBoxWarrantDistrict";
            this.textBoxWarrantDistrict.Size = new System.Drawing.Size(139, 21);
            this.textBoxWarrantDistrict.TabIndex = 4;
            this.textBoxWarrantDistrict.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(17, 109);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(145, 15);
            this.label86.TabIndex = 47;
            this.label86.Text = "Удостовер. нотариусом";
            // 
            // textBoxWarrantNotary
            // 
            this.textBoxWarrantNotary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWarrantNotary.Location = new System.Drawing.Point(175, 106);
            this.textBoxWarrantNotary.MaxLength = 100;
            this.textBoxWarrantNotary.Name = "textBoxWarrantNotary";
            this.textBoxWarrantNotary.Size = new System.Drawing.Size(139, 21);
            this.textBoxWarrantNotary.TabIndex = 3;
            this.textBoxWarrantNotary.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Location = new System.Drawing.Point(17, 51);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(152, 15);
            this.label85.TabIndex = 45;
            this.label85.Text = "Регистрационный номер";
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Location = new System.Drawing.Point(17, 23);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(93, 15);
            this.label84.TabIndex = 44;
            this.label84.Text = "Тип документа";
            // 
            // comboBoxWarrantDocType
            // 
            this.comboBoxWarrantDocType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxWarrantDocType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWarrantDocType.FormattingEnabled = true;
            this.comboBoxWarrantDocType.Location = new System.Drawing.Point(175, 19);
            this.comboBoxWarrantDocType.Name = "comboBoxWarrantDocType";
            this.comboBoxWarrantDocType.Size = new System.Drawing.Size(139, 23);
            this.comboBoxWarrantDocType.TabIndex = 0;
            // 
            // textBoxWarrantRegNum
            // 
            this.textBoxWarrantRegNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWarrantRegNum.Location = new System.Drawing.Point(175, 48);
            this.textBoxWarrantRegNum.MaxLength = 10;
            this.textBoxWarrantRegNum.Name = "textBoxWarrantRegNum";
            this.textBoxWarrantRegNum.Size = new System.Drawing.Size(139, 21);
            this.textBoxWarrantRegNum.TabIndex = 1;
            this.textBoxWarrantRegNum.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // dateTimePickerWarrantDate
            // 
            this.dateTimePickerWarrantDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerWarrantDate.Location = new System.Drawing.Point(175, 77);
            this.dateTimePickerWarrantDate.Name = "dateTimePickerWarrantDate";
            this.dateTimePickerWarrantDate.Size = new System.Drawing.Size(139, 21);
            this.dateTimePickerWarrantDate.TabIndex = 2;
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Location = new System.Drawing.Point(17, 80);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(37, 15);
            this.label83.TabIndex = 41;
            this.label83.Text = "Дата";
            // 
            // groupBox33
            // 
            this.groupBox33.Controls.Add(this.textBoxWarrantDescription);
            this.groupBox33.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox33.Location = new System.Drawing.Point(329, 3);
            this.groupBox33.Name = "groupBox33";
            this.groupBox33.Size = new System.Drawing.Size(321, 194);
            this.groupBox33.TabIndex = 2;
            this.groupBox33.TabStop = false;
            this.groupBox33.Text = "Дополнительные сведения";
            // 
            // textBoxWarrantDescription
            // 
            this.textBoxWarrantDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxWarrantDescription.Location = new System.Drawing.Point(3, 17);
            this.textBoxWarrantDescription.MaxLength = 4000;
            this.textBoxWarrantDescription.Multiline = true;
            this.textBoxWarrantDescription.Name = "textBoxWarrantDescription";
            this.textBoxWarrantDescription.Size = new System.Drawing.Size(315, 174);
            this.textBoxWarrantDescription.TabIndex = 0;
            this.textBoxWarrantDescription.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_warrant,
            this.registration_num,
            this.registration_date,
            this.notary,
            this.on_behalf_of,
            this.description});
            this.tableLayoutPanel14.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 203);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(647, 164);
            this.dataGridView.TabIndex = 0;
            // 
            // id_warrant
            // 
            this.id_warrant.Frozen = true;
            this.id_warrant.HeaderText = "Идентификатор доверенности";
            this.id_warrant.Name = "id_warrant";
            this.id_warrant.ReadOnly = true;
            this.id_warrant.Visible = false;
            // 
            // registration_num
            // 
            this.registration_num.HeaderText = "Регистрационный №";
            this.registration_num.MinimumWidth = 150;
            this.registration_num.Name = "registration_num";
            this.registration_num.ReadOnly = true;
            // 
            // registration_date
            // 
            this.registration_date.HeaderText = "Дата";
            this.registration_date.MinimumWidth = 150;
            this.registration_date.Name = "registration_date";
            this.registration_date.ReadOnly = true;
            this.registration_date.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // notary
            // 
            this.notary.HeaderText = "Нотариус";
            this.notary.MinimumWidth = 200;
            this.notary.Name = "notary";
            this.notary.ReadOnly = true;
            // 
            // on_behalf_of
            // 
            this.on_behalf_of.HeaderText = "В лице кого";
            this.on_behalf_of.MinimumWidth = 200;
            this.on_behalf_of.Name = "on_behalf_of";
            this.on_behalf_of.ReadOnly = true;
            // 
            // description
            // 
            this.description.FillWeight = 200F;
            this.description.HeaderText = "Примечание";
            this.description.MinimumWidth = 300;
            this.description.Name = "description";
            this.description.ReadOnly = true;
            // 
            // WarrantsViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(650, 310);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(659, 376);
            this.Controls.Add(this.tableLayoutPanel14);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WarrantsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Реестр доверенностей";
            this.tableLayoutPanel14.ResumeLayout(false);
            this.groupBox32.ResumeLayout(false);
            this.groupBox32.PerformLayout();
            this.groupBox33.ResumeLayout(false);
            this.groupBox33.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private DataGridViewTextBoxColumn id_warrant;
        private DataGridViewTextBoxColumn registration_num;
        private DataGridViewDateTimeColumn registration_date;
        private DataGridViewTextBoxColumn notary;
        private DataGridViewTextBoxColumn on_behalf_of;
        private DataGridViewTextBoxColumn description;
    }
}
