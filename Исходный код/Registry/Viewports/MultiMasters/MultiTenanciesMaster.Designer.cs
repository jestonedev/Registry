using System.Windows.Forms;

namespace Registry.Viewport.MultiMasters
{
    internal partial class MultiTenanciesMaster
    {

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiTenanciesMaster));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonTenancyCurrent = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTenanciesByFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTenancyDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTenancyDeleteAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRequestMvd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRequestMvdNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripProgressBarMultiOperations = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelRowCount = new System.Windows.Forms.ToolStripLabel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_claim = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.at_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.current_state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.start_dept_period = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.end_dept_period = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_padun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_pkk = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonTenancyCurrent,
            this.toolStripButtonTenanciesByFilter,
            this.toolStripSeparator1,
            this.toolStripButtonTenancyDelete,
            this.toolStripButtonTenancyDeleteAll,
            this.toolStripSeparator2,
            this.toolStripButtonRequestMvd,
            this.toolStripButtonRequestMvdNew,
            this.toolStripProgressBarMultiOperations,
            this.toolStripSeparator3,
            this.toolStripLabelRowCount});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(608, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonTenancyCurrent
            // 
            this.toolStripButtonTenancyCurrent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTenancyCurrent.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTenancyCurrent.Image")));
            this.toolStripButtonTenancyCurrent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTenancyCurrent.Name = "toolStripButtonTenancyCurrent";
            this.toolStripButtonTenancyCurrent.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTenancyCurrent.Text = "Добавить выбранные претензионно-исковые работы";
            this.toolStripButtonTenancyCurrent.Click += new System.EventHandler(this.toolStripButtonTenancyCurrent_Click);
            // 
            // toolStripButtonTenanciesByFilter
            // 
            this.toolStripButtonTenanciesByFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTenanciesByFilter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTenanciesByFilter.Image")));
            this.toolStripButtonTenanciesByFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTenanciesByFilter.Name = "toolStripButtonTenanciesByFilter";
            this.toolStripButtonTenanciesByFilter.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTenanciesByFilter.Text = "Добавить все отфильтрованные претензионно-исковые работы";
            this.toolStripButtonTenanciesByFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonTenanciesByFilter.Click += new System.EventHandler(this.toolStripButtonTenanciesByFilter_Click);
            // 
            // toolStripButtonTenancyDelete
            // 
            this.toolStripButtonTenancyDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTenancyDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTenancyDelete.Image")));
            this.toolStripButtonTenancyDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTenancyDelete.Name = "toolStripButtonTenancyDelete";
            this.toolStripButtonTenancyDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTenancyDelete.Text = "Удалить текущую претензионно-исковую работу";
            this.toolStripButtonTenancyDelete.Click += new System.EventHandler(this.toolStripButtonTenancyDelete_Click);
            // 
            // toolStripButtonTenancyDeleteAll
            // 
            this.toolStripButtonTenancyDeleteAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTenancyDeleteAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTenancyDeleteAll.Image")));
            this.toolStripButtonTenancyDeleteAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTenancyDeleteAll.Name = "toolStripButtonTenancyDeleteAll";
            this.toolStripButtonTenancyDeleteAll.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTenancyDeleteAll.Text = "Удалить все лицензионно-исковые работы";
            this.toolStripButtonTenancyDeleteAll.Click += new System.EventHandler(this.toolStripButtonAccountDeleteAll_Click);
            // 
            // toolStripButtonRequestMvd
            // 
            this.toolStripButtonRequestMvd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRequestMvd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRequestMvd.Image")));
            this.toolStripButtonRequestMvd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRequestMvd.Name = "toolStripButtonRequestMvd";
            this.toolStripButtonRequestMvd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRequestMvd.Text = "Сформировать запрос в МВД";
            this.toolStripButtonRequestMvd.Click += new System.EventHandler(this.toolStripButtonRequestMvd_Click);
            // 
            // toolStripButtonRequestMvdNew
            // 
            this.toolStripButtonRequestMvdNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRequestMvdNew.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRequestMvdNew.Image")));
            this.toolStripButtonRequestMvdNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRequestMvdNew.Name = "toolStripButtonRequestMvdNew";
            this.toolStripButtonRequestMvdNew.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRequestMvdNew.Text = "Сформировать запрос в МВД (новый шаблон)";
            this.toolStripButtonRequestMvdNew.Click += new System.EventHandler(this.toolStripButtonRequestMvdNew_Click);
            // 
            // toolStripProgressBarMultiOperations
            // 
            this.toolStripProgressBarMultiOperations.Name = "toolStripProgressBarMultiOperations";
            this.toolStripProgressBarMultiOperations.Size = new System.Drawing.Size(100, 22);
            this.toolStripProgressBarMultiOperations.Visible = false;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelRowCount
            // 
            this.toolStripLabelRowCount.Name = "toolStripLabelRowCount";
            this.toolStripLabelRowCount.Size = new System.Drawing.Size(154, 22);
            this.toolStripLabelRowCount.Text = "Всего записей в мастере: 0";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_claim,
            this.id_account,
            this.at_date,
            this.current_state,
            this.start_dept_period,
            this.end_dept_period,
            this.amount_tenancy,
            this.amount_dgi,
            this.amount_padun,
            this.amount_pkk,
            this.amount_penalties});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(0, 25);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(608, 132);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView_RowsAdded);
            this.dataGridView.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView_RowsRemoved);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // id_claim
            // 
            this.id_claim.HeaderText = "№";
            this.id_claim.MinimumWidth = 50;
            this.id_claim.Name = "id_claim";
            this.id_claim.ReadOnly = true;
            this.id_claim.Width = 50;
            // 
            // id_account
            // 
            this.id_account.HeaderText = "Лицевой счет";
            this.id_account.MinimumWidth = 150;
            this.id_account.Name = "id_account";
            this.id_account.ReadOnly = true;
            this.id_account.Width = 150;
            // 
            // at_date
            // 
            this.at_date.HeaderText = "Дата формирования";
            this.at_date.MinimumWidth = 170;
            this.at_date.Name = "at_date";
            this.at_date.ReadOnly = true;
            this.at_date.Width = 170;
            // 
            // current_state
            // 
            this.current_state.HeaderText = "Текущее состояние";
            this.current_state.MinimumWidth = 150;
            this.current_state.Name = "current_state";
            this.current_state.ReadOnly = true;
            this.current_state.Width = 150;
            // 
            // start_dept_period
            // 
            this.start_dept_period.HeaderText = "Период с";
            this.start_dept_period.MinimumWidth = 150;
            this.start_dept_period.Name = "start_dept_period";
            this.start_dept_period.ReadOnly = true;
            this.start_dept_period.Width = 150;
            // 
            // end_dept_period
            // 
            this.end_dept_period.HeaderText = "Период по";
            this.end_dept_period.MinimumWidth = 150;
            this.end_dept_period.Name = "end_dept_period";
            this.end_dept_period.ReadOnly = true;
            this.end_dept_period.Width = 150;
            // 
            // amount_tenancy
            // 
            dataGridViewCellStyle3.Format = "#0.0# руб.";
            this.amount_tenancy.DefaultCellStyle = dataGridViewCellStyle3;
            this.amount_tenancy.HeaderText = "Сумма долга найм";
            this.amount_tenancy.MinimumWidth = 110;
            this.amount_tenancy.Name = "amount_tenancy";
            this.amount_tenancy.ReadOnly = true;
            this.amount_tenancy.Width = 110;
            // 
            // amount_dgi
            // 
            dataGridViewCellStyle4.Format = "#0.0# руб.";
            this.amount_dgi.DefaultCellStyle = dataGridViewCellStyle4;
            this.amount_dgi.HeaderText = "Сумма долга ДГИ";
            this.amount_dgi.MinimumWidth = 110;
            this.amount_dgi.Name = "amount_dgi";
            this.amount_dgi.ReadOnly = true;
            this.amount_dgi.Width = 110;
            // 
            // amount_padun
            // 
            this.amount_padun.HeaderText = "Сумма долга Падун";
            this.amount_padun.MinimumWidth = 110;
            this.amount_padun.Name = "amount_padun";
            this.amount_padun.ReadOnly = true;
            this.amount_padun.Width = 110;
            // 
            // amount_pkk
            // 
            this.amount_pkk.HeaderText = "Сумма долга ПКК";
            this.amount_pkk.MinimumWidth = 110;
            this.amount_pkk.Name = "amount_pkk";
            this.amount_pkk.ReadOnly = true;
            this.amount_pkk.Width = 110;
            // 
            // amount_penalties
            // 
            this.amount_penalties.HeaderText = "Сумма долга пени";
            this.amount_penalties.MinimumWidth = 110;
            this.amount_penalties.Name = "amount_penalties";
            this.amount_penalties.ReadOnly = true;
            this.amount_penalties.Width = 110;
            // 
            // MultiTenanciesMaster
            // 
            this.ClientSize = new System.Drawing.Size(608, 157);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MultiTenanciesMaster";
            this.TabText = "Мастер массовых операций над процессами найма";
            this.Text = "Мастер массовых операций над процессами найма";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private ToolStripButton toolStripButtonTenancyCurrent;
        private ToolStripButton toolStripButtonTenanciesByFilter;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonTenancyDelete;
        private ToolStripButton toolStripButtonTenancyDeleteAll;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonRequestMvd;
        private ToolStrip toolStrip1;
        private ToolStripProgressBar toolStripProgressBarMultiOperations;
        private DataGridView dataGridView;
        private ToolStripButton toolStripButtonRequestMvdNew;
        private ToolStripLabel toolStripLabelRowCount;
        private ToolStripSeparator toolStripSeparator3;
        private DataGridViewTextBoxColumn id_claim;
        private DataGridViewTextBoxColumn id_account;
        private DataGridViewTextBoxColumn at_date;
        private DataGridViewTextBoxColumn current_state;
        private DataGridViewTextBoxColumn start_dept_period;
        private DataGridViewTextBoxColumn end_dept_period;
        private DataGridViewTextBoxColumn amount_tenancy;
        private DataGridViewTextBoxColumn amount_dgi;
        private DataGridViewTextBoxColumn amount_padun;
        private DataGridViewTextBoxColumn amount_pkk;
        private DataGridViewTextBoxColumn amount_penalties;

    }
}
