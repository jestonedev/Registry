using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class ClaimStateTypesViewport
    {
        #region Components
        TableLayoutPanel tableLayoutPanel19;
        GroupBox groupBox36;
        GroupBox groupBox37;
        DataGridView dataGridViewClaimStateTypes;
        DataGridViewTextBoxColumn id_state_type;
        DataGridViewCheckBoxColumn is_start_state_type;
        DataGridViewTextBoxColumn state_type;
        DataGridView dataGridViewClaimStateTypesFrom;
        private DataGridViewCheckBoxColumn state_type_checked;
        private DataGridViewTextBoxColumn id_relation;
        private DataGridViewTextBoxColumn id_state_type_from;
        private DataGridViewTextBoxColumn state_type_from;
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimStateTypesViewport));
            this.tableLayoutPanel19 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox36 = new System.Windows.Forms.GroupBox();
            this.dataGridViewClaimStateTypes = new System.Windows.Forms.DataGridView();
            this.id_state_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.is_start_state_type = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.state_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox37 = new System.Windows.Forms.GroupBox();
            this.dataGridViewClaimStateTypesFrom = new System.Windows.Forms.DataGridView();
            this.state_type_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.id_relation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_state_type_from = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.state_type_from = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel19.SuspendLayout();
            this.groupBox36.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypes)).BeginInit();
            this.groupBox37.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypesFrom)).BeginInit();
            this.SuspendLayout();
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
            this.tableLayoutPanel19.Size = new System.Drawing.Size(707, 427);
            this.tableLayoutPanel19.TabIndex = 0;
            // 
            // groupBox36
            // 
            this.groupBox36.Controls.Add(this.dataGridViewClaimStateTypes);
            this.groupBox36.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox36.Location = new System.Drawing.Point(3, 3);
            this.groupBox36.Name = "groupBox36";
            this.groupBox36.Size = new System.Drawing.Size(347, 421);
            this.groupBox36.TabIndex = 0;
            this.groupBox36.TabStop = false;
            this.groupBox36.Text = "Состояния";
            // 
            // dataGridViewClaimStateTypes
            // 
            this.dataGridViewClaimStateTypes.AllowUserToAddRows = false;
            this.dataGridViewClaimStateTypes.AllowUserToDeleteRows = false;
            this.dataGridViewClaimStateTypes.AllowUserToResizeRows = false;
            this.dataGridViewClaimStateTypes.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewClaimStateTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaimStateTypes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_state_type,
            this.is_start_state_type,
            this.state_type});
            this.dataGridViewClaimStateTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaimStateTypes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewClaimStateTypes.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewClaimStateTypes.Name = "dataGridViewClaimStateTypes";
            this.dataGridViewClaimStateTypes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewClaimStateTypes.Size = new System.Drawing.Size(341, 401);
            this.dataGridViewClaimStateTypes.TabIndex = 0;
            this.dataGridViewClaimStateTypes.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewClaimStateTypes_CellValidated);
            // 
            // id_state_type
            // 
            this.id_state_type.HeaderText = "Идентификатор состояния";
            this.id_state_type.Name = "id_state_type";
            this.id_state_type.Visible = false;
            // 
            // is_start_state_type
            // 
            this.is_start_state_type.HeaderText = "Начальное";
            this.is_start_state_type.MinimumWidth = 70;
            this.is_start_state_type.Name = "is_start_state_type";
            this.is_start_state_type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.is_start_state_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.is_start_state_type.Width = 70;
            // 
            // state_type
            // 
            this.state_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.state_type.HeaderText = "Наименование вида состояния";
            this.state_type.Name = "state_type";
            this.state_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // groupBox37
            // 
            this.groupBox37.Controls.Add(this.dataGridViewClaimStateTypesFrom);
            this.groupBox37.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox37.Location = new System.Drawing.Point(356, 3);
            this.groupBox37.Name = "groupBox37";
            this.groupBox37.Size = new System.Drawing.Size(348, 421);
            this.groupBox37.TabIndex = 1;
            this.groupBox37.TabStop = false;
            this.groupBox37.Text = "Разрешены переходы из";
            // 
            // dataGridViewClaimStateTypesFrom
            // 
            this.dataGridViewClaimStateTypesFrom.AllowUserToAddRows = false;
            this.dataGridViewClaimStateTypesFrom.AllowUserToDeleteRows = false;
            this.dataGridViewClaimStateTypesFrom.AllowUserToResizeRows = false;
            this.dataGridViewClaimStateTypesFrom.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewClaimStateTypesFrom.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaimStateTypesFrom.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.state_type_checked,
            this.id_relation,
            this.id_state_type_from,
            this.state_type_from});
            this.dataGridViewClaimStateTypesFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaimStateTypesFrom.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewClaimStateTypesFrom.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewClaimStateTypesFrom.Name = "dataGridViewClaimStateTypesFrom";
            this.dataGridViewClaimStateTypesFrom.Size = new System.Drawing.Size(342, 401);
            this.dataGridViewClaimStateTypesFrom.TabIndex = 0;
            this.dataGridViewClaimStateTypesFrom.VirtualMode = true;
            this.dataGridViewClaimStateTypesFrom.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridViewClaimStateTypesFrom_CellValueNeeded);
            this.dataGridViewClaimStateTypesFrom.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridViewClaimStateTypesFrom_CellValuePushed);
            this.dataGridViewClaimStateTypesFrom.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewClaimStateTypesFrom_ColumnHeaderMouseClick);
            this.dataGridViewClaimStateTypesFrom.SelectionChanged += new System.EventHandler(this.dataGridViewClaimStateTypesFrom_SelectionChanged);
            // 
            // state_type_checked
            // 
            this.state_type_checked.HeaderText = "";
            this.state_type_checked.MinimumWidth = 70;
            this.state_type_checked.Name = "state_type_checked";
            this.state_type_checked.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.state_type_checked.Width = 70;
            // 
            // id_relation
            // 
            this.id_relation.HeaderText = "Идентификатор отношения";
            this.id_relation.Name = "id_relation";
            this.id_relation.Visible = false;
            // 
            // id_state_type_from
            // 
            this.id_state_type_from.HeaderText = "Идентификатор состояния";
            this.id_state_type_from.Name = "id_state_type_from";
            this.id_state_type_from.Visible = false;
            // 
            // state_type_from
            // 
            this.state_type_from.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.state_type_from.DefaultCellStyle = dataGridViewCellStyle1;
            this.state_type_from.HeaderText = "Наименование вида состояния";
            this.state_type_from.Name = "state_type_from";
            this.state_type_from.ReadOnly = true;
            // 
            // ClaimStateTypesViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(713, 433);
            this.Controls.Add(this.tableLayoutPanel19);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClaimStateTypesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Виды состояний иск. работы";
            this.tableLayoutPanel19.ResumeLayout(false);
            this.groupBox36.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypes)).EndInit();
            this.groupBox37.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaimStateTypesFrom)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
