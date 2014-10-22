using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal sealed class ClaimStateTypesViewport: Viewport
    {
        #region Components
        TableLayoutPanel tableLayoutPanel19 = new TableLayoutPanel();
        GroupBox groupBox36 = new GroupBox();
        GroupBox groupBox37 = new GroupBox();
        DataGridView dataGridViewClaimStateTypes = new DataGridView();           
        DataGridViewTextBoxColumn field_id_state_type = new DataGridViewTextBoxColumn();
        DataGridViewCheckBoxColumn filed_is_start_state_type = new DataGridViewCheckBoxColumn();
        DataGridViewTextBoxColumn field_state_type = new DataGridViewTextBoxColumn();
        DataGridView dataGridViewClaimStateTypesFrom = new DataGridView();
        DataGridViewCheckBoxColumn field_state_type_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
        DataGridViewTextBoxColumn field_id_state_type_from = new System.Windows.Forms.DataGridViewTextBoxColumn();
        DataGridViewTextBoxColumn field_state_type_from = new System.Windows.Forms.DataGridViewTextBoxColumn();
        #endregion Components

        public ClaimStateTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageClaimStateTypes";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Виды состояний иск. работы";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        private void ConstructViewport()
        {
            this.Controls.Add(tableLayoutPanel19);
            this.SuspendLayout();
            this.tableLayoutPanel19.SuspendLayout();
            this.groupBox36.SuspendLayout();
            this.groupBox37.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypesFrom)).BeginInit();
            // 
            // tableLayoutPanel19
            // 
            this.tableLayoutPanel19.ColumnCount = 2;
            this.tableLayoutPanel19.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel19.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel19.Controls.Add(this.groupBox36, 0, 0);
            this.tableLayoutPanel19.Controls.Add(this.groupBox37, 1, 0);
            this.tableLayoutPanel19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel19.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel19.Name = "tableLayoutPanel19";
            this.tableLayoutPanel19.RowCount = 1;
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel19.Size = new System.Drawing.Size(984, 531);
            this.tableLayoutPanel19.TabIndex = 0;
            // 
            // groupBox36
            // 
            this.groupBox36.Controls.Add(this.dataGridViewClaimStateTypes);
            this.groupBox36.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox36.Location = new System.Drawing.Point(3, 3);
            this.groupBox36.Name = "groupBox36";
            this.groupBox36.Size = new System.Drawing.Size(486, 525);
            this.groupBox36.TabIndex = 0;
            this.groupBox36.TabStop = false;
            this.groupBox36.Text = "Состояния";
            // 
            // groupBox37
            // 
            this.groupBox37.Controls.Add(this.dataGridViewClaimStateTypesFrom);
            this.groupBox37.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox37.Location = new System.Drawing.Point(495, 3);
            this.groupBox37.Name = "groupBox37";
            this.groupBox37.Size = new System.Drawing.Size(486, 525);
            this.groupBox37.TabIndex = 1;
            this.groupBox37.TabStop = false;
            this.groupBox37.Text = "Разрешены переходы из";
            // 
            // dataGridViewClaimStateTypes
            // 
            this.dataGridViewClaimStateTypes.AllowUserToAddRows = false;
            this.dataGridViewClaimStateTypes.AllowUserToDeleteRows = false;
            this.dataGridViewClaimStateTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaimStateTypes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_state_type,
            this.filed_is_start_state_type,
            this.field_state_type});
            this.dataGridViewClaimStateTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaimStateTypes.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewClaimStateTypes.Name = "dataGridViewClaimStateTypes";
            this.dataGridViewClaimStateTypes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewClaimStateTypes.Size = new System.Drawing.Size(480, 506);
            this.dataGridViewClaimStateTypes.TabIndex = 0;
            // 
            // field_claim_id_state_type
            // 
            this.field_id_state_type.HeaderText = "Идентификатор состояния";
            this.field_id_state_type.Name = "field_claim_id_state_type";
            this.field_id_state_type.Visible = false;
            // 
            // filed_claim_is_start_state_type
            // 
            this.filed_is_start_state_type.HeaderText = "Начальное";
            this.filed_is_start_state_type.MinimumWidth = 70;
            this.filed_is_start_state_type.Name = "filed_claim_is_start_state_type";
            this.filed_is_start_state_type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.filed_is_start_state_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.filed_is_start_state_type.Width = 70;
            // 
            // field_claim_state_type
            // 
            this.field_state_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.field_state_type.HeaderText = "Наименование вида состояния";
            this.field_state_type.Name = "field_claim_state_type";
            // 
            // dataGridViewClaimStateTypesFrom
            // 
            this.dataGridViewClaimStateTypesFrom.AllowUserToAddRows = false;
            this.dataGridViewClaimStateTypesFrom.AllowUserToDeleteRows = false;
            this.dataGridViewClaimStateTypesFrom.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaimStateTypesFrom.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_state_type_checked,
            this.field_id_state_type_from,
            this.field_state_type_from});
            this.dataGridViewClaimStateTypesFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaimStateTypesFrom.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewClaimStateTypesFrom.Name = "dataGridViewClaimStateTypesFrom";
            this.dataGridViewClaimStateTypesFrom.Size = new System.Drawing.Size(480, 506);
            this.dataGridViewClaimStateTypesFrom.TabIndex = 0;
            // 
            // field_state_type_checked
            // 
            this.field_state_type_checked.HeaderText = "";
            this.field_state_type_checked.MinimumWidth = 70;
            this.field_state_type_checked.Name = "checked";
            this.field_state_type_checked.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.field_state_type_checked.Width = 70;
            // 
            // field_id_state_type_from
            // 
            this.field_id_state_type_from.HeaderText = "Идентификатор состояния";
            this.field_id_state_type_from.Name = "id_state_type";
            this.field_id_state_type_from.Visible = false;
            // 
            // field_state_type_from
            // 
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.field_state_type_from.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.ControlLight;
            this.field_state_type_from.DefaultCellStyle = dataGridViewCellStyle;
            this.field_state_type_from.HeaderText = "Наименование вида состояния";
            this.field_state_type_from.Name = "state_type";
            this.field_state_type_from.ReadOnly = true;


            this.tableLayoutPanel19.ResumeLayout(false);
            this.groupBox36.ResumeLayout(false);
            this.groupBox37.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypesFrom)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
