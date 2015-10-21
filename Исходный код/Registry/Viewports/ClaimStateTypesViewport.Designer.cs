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
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(ClaimStateTypesViewport));
            tableLayoutPanel19 = new TableLayoutPanel();
            groupBox36 = new GroupBox();
            dataGridViewClaimStateTypes = new DataGridView();
            id_state_type = new DataGridViewTextBoxColumn();
            is_start_state_type = new DataGridViewCheckBoxColumn();
            state_type = new DataGridViewTextBoxColumn();
            groupBox37 = new GroupBox();
            dataGridViewClaimStateTypesFrom = new DataGridView();
            state_type_checked = new DataGridViewCheckBoxColumn();
            id_relation = new DataGridViewTextBoxColumn();
            id_state_type_from = new DataGridViewTextBoxColumn();
            state_type_from = new DataGridViewTextBoxColumn();
            tableLayoutPanel19.SuspendLayout();
            groupBox36.SuspendLayout();
            ((ISupportInitialize)(dataGridViewClaimStateTypes)).BeginInit();
            groupBox37.SuspendLayout();
            ((ISupportInitialize)(dataGridViewClaimStateTypesFrom)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel19
            // 
            tableLayoutPanel19.ColumnCount = 2;
            tableLayoutPanel19.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel19.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel19.Controls.Add(groupBox36, 0, 0);
            tableLayoutPanel19.Controls.Add(groupBox37, 1, 0);
            tableLayoutPanel19.Dock = DockStyle.Fill;
            tableLayoutPanel19.Location = new Point(3, 3);
            tableLayoutPanel19.Name = "tableLayoutPanel19";
            tableLayoutPanel19.RowCount = 1;
            tableLayoutPanel19.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel19.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel19.Size = new Size(707, 427);
            tableLayoutPanel19.TabIndex = 0;
            // 
            // groupBox36
            // 
            groupBox36.Controls.Add(dataGridViewClaimStateTypes);
            groupBox36.Dock = DockStyle.Fill;
            groupBox36.Location = new Point(3, 3);
            groupBox36.Name = "groupBox36";
            groupBox36.Size = new Size(347, 421);
            groupBox36.TabIndex = 0;
            groupBox36.TabStop = false;
            groupBox36.Text = @"Состояния";
            // 
            // dataGridViewClaimStateTypes
            // 
            dataGridViewClaimStateTypes.AllowUserToAddRows = false;
            dataGridViewClaimStateTypes.AllowUserToDeleteRows = false;
            dataGridViewClaimStateTypes.AllowUserToResizeRows = false;
            dataGridViewClaimStateTypes.BackgroundColor = Color.White;
            dataGridViewClaimStateTypes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewClaimStateTypes.Columns.AddRange(id_state_type, is_start_state_type, state_type);
            dataGridViewClaimStateTypes.Dock = DockStyle.Fill;
            dataGridViewClaimStateTypes.Location = new Point(3, 17);
            dataGridViewClaimStateTypes.Name = "dataGridViewClaimStateTypes";
            dataGridViewClaimStateTypes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewClaimStateTypes.Size = new Size(341, 401);
            dataGridViewClaimStateTypes.TabIndex = 0;
            dataGridViewClaimStateTypes.CellValidated += dataGridViewClaimStateTypes_CellValidated;
            // 
            // id_state_type
            // 
            id_state_type.HeaderText = @"Идентификатор состояния";
            id_state_type.Name = "id_state_type";
            id_state_type.Visible = false;
            // 
            // is_start_state_type
            // 
            is_start_state_type.HeaderText = @"Начальное";
            is_start_state_type.MinimumWidth = 70;
            is_start_state_type.Name = "is_start_state_type";
            is_start_state_type.Resizable = DataGridViewTriState.True;
            is_start_state_type.SortMode = DataGridViewColumnSortMode.Automatic;
            is_start_state_type.Width = 70;
            // 
            // state_type
            // 
            state_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            state_type.HeaderText = @"Наименование вида состояния";
            state_type.Name = "state_type";
            state_type.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // groupBox37
            // 
            groupBox37.Controls.Add(dataGridViewClaimStateTypesFrom);
            groupBox37.Dock = DockStyle.Fill;
            groupBox37.Location = new Point(356, 3);
            groupBox37.Name = "groupBox37";
            groupBox37.Size = new Size(348, 421);
            groupBox37.TabIndex = 1;
            groupBox37.TabStop = false;
            groupBox37.Text = @"Разрешены переходы из";
            // 
            // dataGridViewClaimStateTypesFrom
            // 
            dataGridViewClaimStateTypesFrom.AllowUserToAddRows = false;
            dataGridViewClaimStateTypesFrom.AllowUserToDeleteRows = false;
            dataGridViewClaimStateTypesFrom.AllowUserToResizeRows = false;
            dataGridViewClaimStateTypesFrom.BackgroundColor = Color.White;
            dataGridViewClaimStateTypesFrom.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewClaimStateTypesFrom.Columns.AddRange(state_type_checked, id_relation, id_state_type_from, state_type_from);
            dataGridViewClaimStateTypesFrom.Dock = DockStyle.Fill;
            dataGridViewClaimStateTypesFrom.Location = new Point(3, 17);
            dataGridViewClaimStateTypesFrom.Name = "dataGridViewClaimStateTypesFrom";
            dataGridViewClaimStateTypesFrom.Size = new Size(342, 401);
            dataGridViewClaimStateTypesFrom.TabIndex = 0;
            dataGridViewClaimStateTypesFrom.VirtualMode = true;
            dataGridViewClaimStateTypesFrom.CellValueNeeded += dataGridViewClaimStateTypesFrom_CellValueNeeded;
            dataGridViewClaimStateTypesFrom.CellValuePushed += dataGridViewClaimStateTypesFrom_CellValuePushed;
            dataGridViewClaimStateTypesFrom.ColumnHeaderMouseClick += dataGridViewClaimStateTypesFrom_ColumnHeaderMouseClick;
            dataGridViewClaimStateTypesFrom.SelectionChanged += dataGridViewClaimStateTypesFrom_SelectionChanged;
            // 
            // state_type_checked
            // 
            state_type_checked.HeaderText = "";
            state_type_checked.MinimumWidth = 70;
            state_type_checked.Name = "state_type_checked";
            state_type_checked.Resizable = DataGridViewTriState.False;
            state_type_checked.Width = 70;
            // 
            // id_relation
            // 
            id_relation.HeaderText = @"Идентификатор отношения";
            id_relation.Name = "id_relation";
            id_relation.Visible = false;
            // 
            // id_state_type_from
            // 
            id_state_type_from.HeaderText = @"Идентификатор состояния";
            id_state_type_from.Name = "id_state_type_from";
            id_state_type_from.Visible = false;
            // 
            // state_type_from
            // 
            state_type_from.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(224, 224, 224);
            state_type_from.DefaultCellStyle = dataGridViewCellStyle1;
            state_type_from.HeaderText = @"Наименование вида состояния";
            state_type_from.Name = "state_type_from";
            state_type_from.ReadOnly = true;
            // 
            // ClaimStateTypesViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(713, 433);
            Controls.Add(tableLayoutPanel19);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "ClaimStateTypesViewport";
            Padding = new Padding(3);
            Text = @"Виды состояний иск. работы";
            tableLayoutPanel19.ResumeLayout(false);
            groupBox36.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewClaimStateTypes)).EndInit();
            groupBox37.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewClaimStateTypesFrom)).EndInit();
            ResumeLayout(false);

        }
    }
}
