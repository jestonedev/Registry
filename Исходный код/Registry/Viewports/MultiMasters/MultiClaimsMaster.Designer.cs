using System.Windows.Forms;

namespace Registry.Viewport.MultiMasters
{
    internal partial class MultiClaimsMaster
    {

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiClaimsMaster));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonClaimCurrent = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClaimsByFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClaimDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClaimDeleteAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCreateClaims = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeptPeriod = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonJudicialOrder = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonToLegalDepartment = new System.Windows.Forms.ToolStripButton();
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
            this.amount_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStripButtonRequestToBks = new System.Windows.Forms.ToolStripButton();
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
            this.toolStripButtonClaimCurrent,
            this.toolStripButtonClaimsByFilter,
            this.toolStripSeparator1,
            this.toolStripButtonClaimDelete,
            this.toolStripButtonClaimDeleteAll,
            this.toolStripSeparator2,
            this.toolStripButtonCreateClaims,
            this.toolStripButtonDeptPeriod,
            this.toolStripButtonJudicialOrder,
            this.toolStripButtonRequestToBks,
            this.toolStripButtonToLegalDepartment,
            this.toolStripProgressBarMultiOperations,
            this.toolStripSeparator3,
            this.toolStripLabelRowCount});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(608, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonClaimCurrent
            // 
            this.toolStripButtonClaimCurrent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClaimCurrent.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClaimCurrent.Image")));
            this.toolStripButtonClaimCurrent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClaimCurrent.Name = "toolStripButtonClaimCurrent";
            this.toolStripButtonClaimCurrent.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonClaimCurrent.Text = "Добавить выбранные претензионно-исковые работы";
            this.toolStripButtonClaimCurrent.Click += new System.EventHandler(this.toolStripButtonClaimCurrent_Click);
            // 
            // toolStripButtonClaimsByFilter
            // 
            this.toolStripButtonClaimsByFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClaimsByFilter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClaimsByFilter.Image")));
            this.toolStripButtonClaimsByFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClaimsByFilter.Name = "toolStripButtonClaimsByFilter";
            this.toolStripButtonClaimsByFilter.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonClaimsByFilter.Text = "Добавить все отфильтрованные претензионно-исковые работы";
            this.toolStripButtonClaimsByFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonClaimsByFilter.Click += new System.EventHandler(this.toolStripButtonAccountsByFilter_Click);
            // 
            // toolStripButtonClaimDelete
            // 
            this.toolStripButtonClaimDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClaimDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClaimDelete.Image")));
            this.toolStripButtonClaimDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClaimDelete.Name = "toolStripButtonClaimDelete";
            this.toolStripButtonClaimDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonClaimDelete.Text = "Удалить текущую претензионно-исковую работу";
            this.toolStripButtonClaimDelete.Click += new System.EventHandler(this.toolStripButtonClaimDelete_Click);
            // 
            // toolStripButtonClaimDeleteAll
            // 
            this.toolStripButtonClaimDeleteAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClaimDeleteAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClaimDeleteAll.Image")));
            this.toolStripButtonClaimDeleteAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClaimDeleteAll.Name = "toolStripButtonClaimDeleteAll";
            this.toolStripButtonClaimDeleteAll.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonClaimDeleteAll.Text = "Удалить все лицензионно-исковые работы";
            this.toolStripButtonClaimDeleteAll.Click += new System.EventHandler(this.toolStripButtonAccountDeleteAll_Click);
            // 
            // toolStripButtonCreateClaims
            // 
            this.toolStripButtonCreateClaims.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCreateClaims.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCreateClaims.Image")));
            this.toolStripButtonCreateClaims.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreateClaims.Name = "toolStripButtonCreateClaims";
            this.toolStripButtonCreateClaims.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCreateClaims.Text = "Создать стадию претензионно-исковой работы";
            this.toolStripButtonCreateClaims.Click += new System.EventHandler(this.toolStripButtonCreateClaimStates_Click);
            // 
            // toolStripButtonDeptPeriod
            // 
            this.toolStripButtonDeptPeriod.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeptPeriod.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeptPeriod.Image")));
            this.toolStripButtonDeptPeriod.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeptPeriod.Name = "toolStripButtonDeptPeriod";
            this.toolStripButtonDeptPeriod.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDeptPeriod.Text = "Установить период предъявления";
            this.toolStripButtonDeptPeriod.Click += new System.EventHandler(this.toolStripButtonDeptPeriod_Click);
            // 
            // toolStripButtonJudicialOrder
            // 
            this.toolStripButtonJudicialOrder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonJudicialOrder.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonJudicialOrder.Image")));
            this.toolStripButtonJudicialOrder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonJudicialOrder.Name = "toolStripButtonJudicialOrder";
            this.toolStripButtonJudicialOrder.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonJudicialOrder.Text = "Сформировать заявление о выдаче судебного приказа";
            this.toolStripButtonJudicialOrder.Click += new System.EventHandler(this.toolStripButtonJudicialOrder_Click);
            // 
            // toolStripButtonToLegalDepartment
            // 
            this.toolStripButtonToLegalDepartment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonToLegalDepartment.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonToLegalDepartment.Image")));
            this.toolStripButtonToLegalDepartment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonToLegalDepartment.Name = "toolStripButtonToLegalDepartment";
            this.toolStripButtonToLegalDepartment.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonToLegalDepartment.Text = "Передача в юридический отдел";
            this.toolStripButtonToLegalDepartment.Click += new System.EventHandler(this.toolStripButtonToLegalDepartment_Click);
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
            dataGridViewCellStyle5.Format = "#0.0# руб.";
            this.amount_tenancy.DefaultCellStyle = dataGridViewCellStyle5;
            this.amount_tenancy.HeaderText = "Сумма долга найм";
            this.amount_tenancy.MinimumWidth = 200;
            this.amount_tenancy.Name = "amount_tenancy";
            this.amount_tenancy.ReadOnly = true;
            this.amount_tenancy.Width = 200;
            // 
            // amount_dgi
            // 
            dataGridViewCellStyle6.Format = "#0.0# руб.";
            this.amount_dgi.DefaultCellStyle = dataGridViewCellStyle6;
            this.amount_dgi.HeaderText = "Сумма долга ДГИ";
            this.amount_dgi.MinimumWidth = 200;
            this.amount_dgi.Name = "amount_dgi";
            this.amount_dgi.ReadOnly = true;
            this.amount_dgi.Width = 200;
            // 
            // amount_penalties
            // 
            this.amount_penalties.HeaderText = "Сумма долга пени";
            this.amount_penalties.MinimumWidth = 200;
            this.amount_penalties.Name = "amount_penalties";
            this.amount_penalties.ReadOnly = true;
            this.amount_penalties.Width = 200;
            // 
            // toolStripButtonRequestToBks
            // 
            this.toolStripButtonRequestToBks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRequestToBks.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRequestToBks.Image")));
            this.toolStripButtonRequestToBks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRequestToBks.Name = "toolStripButtonRequestToBks";
            this.toolStripButtonRequestToBks.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRequestToBks.Text = "Сформировать запрос в БКС";
            this.toolStripButtonRequestToBks.Click += new System.EventHandler(this.toolStripButtonRequestToBks_Click);
            // 
            // MultiClaimsMaster
            // 
            this.ClientSize = new System.Drawing.Size(608, 157);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MultiClaimsMaster";
            this.TabText = "Мастер массовых операций над претензионно-исковыми работами";
            this.Text = "Мастер массовых операций над претензионно-исковыми работами";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private ToolStripButton toolStripButtonClaimCurrent;
        private ToolStripButton toolStripButtonClaimsByFilter;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonClaimDelete;
        private ToolStripButton toolStripButtonClaimDeleteAll;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonCreateClaims;
        private ToolStrip toolStrip1;
        private ToolStripProgressBar toolStripProgressBarMultiOperations;
        private DataGridView dataGridView;
        private ToolStripButton toolStripButtonJudicialOrder;
        private ToolStripButton toolStripButtonDeptPeriod;
        private ToolStripButton toolStripButtonToLegalDepartment;
        private DataGridViewTextBoxColumn id_claim;
        private DataGridViewTextBoxColumn id_account;
        private DataGridViewTextBoxColumn at_date;
        private DataGridViewTextBoxColumn current_state;
        private DataGridViewTextBoxColumn start_dept_period;
        private DataGridViewTextBoxColumn end_dept_period;
        private DataGridViewTextBoxColumn amount_tenancy;
        private DataGridViewTextBoxColumn amount_dgi;
        private DataGridViewTextBoxColumn amount_penalties;
        private ToolStripLabel toolStripLabelRowCount;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolStripButtonRequestToBks;

    }
}
