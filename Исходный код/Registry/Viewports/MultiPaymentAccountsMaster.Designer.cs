using System.Windows.Forms;

namespace Registry.Viewport
{
    public partial class MultiPaymentAccountsMaster
    {

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiPaymentAccountsMaster));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.crn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.raw_address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parsed_address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prescribed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_input = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recalc_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recalc_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transfer_balance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStripButtonAccountCurrent = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAccountsByFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAccountDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAccountDeleteAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCreateClaims = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripProgressBarMultiOperations = new System.Windows.Forms.ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.date,
            this.id_account,
            this.crn,
            this.raw_address,
            this.parsed_address,
            this.account,
            this.tenant,
            this.total_area,
            this.living_area,
            this.prescribed,
            this.balance_input,
            this.balance_tenancy,
            this.balance_dgi,
            this.charging_tenancy,
            this.charging_dgi,
            this.charging_total,
            this.recalc_tenancy,
            this.recalc_dgi,
            this.payment_tenancy,
            this.payment_dgi,
            this.transfer_balance,
            this.balance_output_total,
            this.balance_output_tenancy,
            this.balance_output_dgi});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 25);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(608, 132);
            this.dataGridView.TabIndex = 7;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // date
            // 
            this.date.HeaderText = "Состояние на дату";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            // 
            // id_account
            // 
            this.id_account.HeaderText = "№";
            this.id_account.Name = "id_account";
            this.id_account.ReadOnly = true;
            this.id_account.Visible = false;
            // 
            // crn
            // 
            this.crn.HeaderText = "СРН";
            this.crn.Name = "crn";
            this.crn.ReadOnly = true;
            // 
            // raw_address
            // 
            this.raw_address.HeaderText = "Адрес по БКС";
            this.raw_address.MinimumWidth = 300;
            this.raw_address.Name = "raw_address";
            this.raw_address.ReadOnly = true;
            this.raw_address.Width = 300;
            // 
            // parsed_address
            // 
            this.parsed_address.HeaderText = "Адрес в реестре ЖФ";
            this.parsed_address.MinimumWidth = 350;
            this.parsed_address.Name = "parsed_address";
            this.parsed_address.ReadOnly = true;
            this.parsed_address.Width = 350;
            // 
            // account
            // 
            this.account.HeaderText = "Лицевой счет";
            this.account.MinimumWidth = 150;
            this.account.Name = "account";
            this.account.ReadOnly = true;
            this.account.Width = 150;
            // 
            // tenant
            // 
            this.tenant.HeaderText = "Наниматель";
            this.tenant.MinimumWidth = 150;
            this.tenant.Name = "tenant";
            this.tenant.ReadOnly = true;
            this.tenant.Width = 150;
            // 
            // total_area
            // 
            this.total_area.HeaderText = "Общая площадь";
            this.total_area.Name = "total_area";
            this.total_area.ReadOnly = true;
            // 
            // living_area
            // 
            this.living_area.HeaderText = "Жилая площадь";
            this.living_area.Name = "living_area";
            this.living_area.ReadOnly = true;
            // 
            // prescribed
            // 
            this.prescribed.HeaderText = "Прописано";
            this.prescribed.Name = "prescribed";
            this.prescribed.ReadOnly = true;
            // 
            // balance_input
            // 
            this.balance_input.HeaderText = "Сальдо вх.";
            this.balance_input.Name = "balance_input";
            this.balance_input.ReadOnly = true;
            // 
            // balance_tenancy
            // 
            this.balance_tenancy.HeaderText = "Сальдо вх. найм";
            this.balance_tenancy.Name = "balance_tenancy";
            this.balance_tenancy.ReadOnly = true;
            // 
            // balance_dgi
            // 
            this.balance_dgi.HeaderText = "Сальдо вх. ДГИ";
            this.balance_dgi.Name = "balance_dgi";
            this.balance_dgi.ReadOnly = true;
            // 
            // charging_tenancy
            // 
            this.charging_tenancy.HeaderText = "Начислено найм";
            this.charging_tenancy.Name = "charging_tenancy";
            this.charging_tenancy.ReadOnly = true;
            // 
            // charging_dgi
            // 
            this.charging_dgi.HeaderText = "Начислено ДГИ";
            this.charging_dgi.Name = "charging_dgi";
            this.charging_dgi.ReadOnly = true;
            // 
            // charging_total
            // 
            this.charging_total.HeaderText = "Начислено итого";
            this.charging_total.Name = "charging_total";
            this.charging_total.ReadOnly = true;
            // 
            // recalc_tenancy
            // 
            this.recalc_tenancy.HeaderText = "Перерасчет найм";
            this.recalc_tenancy.Name = "recalc_tenancy";
            this.recalc_tenancy.ReadOnly = true;
            // 
            // recalc_dgi
            // 
            this.recalc_dgi.HeaderText = "Перерасчет ДГИ";
            this.recalc_dgi.Name = "recalc_dgi";
            this.recalc_dgi.ReadOnly = true;
            // 
            // payment_tenancy
            // 
            this.payment_tenancy.HeaderText = "Оплата найм";
            this.payment_tenancy.Name = "payment_tenancy";
            this.payment_tenancy.ReadOnly = true;
            // 
            // payment_dgi
            // 
            this.payment_dgi.HeaderText = "Оплата ДГИ";
            this.payment_dgi.Name = "payment_dgi";
            this.payment_dgi.ReadOnly = true;
            // 
            // transfer_balance
            // 
            this.transfer_balance.HeaderText = "Перенос сальдо";
            this.transfer_balance.Name = "transfer_balance";
            this.transfer_balance.ReadOnly = true;
            // 
            // balance_output_total
            // 
            this.balance_output_total.HeaderText = "Сальдо исх.";
            this.balance_output_total.Name = "balance_output_total";
            this.balance_output_total.ReadOnly = true;
            // 
            // balance_output_tenancy
            // 
            this.balance_output_tenancy.HeaderText = "Сальдо исх. найм";
            this.balance_output_tenancy.Name = "balance_output_tenancy";
            this.balance_output_tenancy.ReadOnly = true;
            // 
            // balance_output_dgi
            // 
            this.balance_output_dgi.HeaderText = "Сальдо исх. ДГИ";
            this.balance_output_dgi.Name = "balance_output_dgi";
            this.balance_output_dgi.ReadOnly = true;
            // 
            // toolStripButtonAccountCurrent
            // 
            this.toolStripButtonAccountCurrent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAccountCurrent.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAccountCurrent.Image")));
            this.toolStripButtonAccountCurrent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAccountCurrent.Name = "toolStripButtonAccountCurrent";
            this.toolStripButtonAccountCurrent.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAccountCurrent.Text = "Добавить текущий лицевой счет";
            this.toolStripButtonAccountCurrent.Click += new System.EventHandler(this.toolStripButtonAccountCurrent_Click);
            // 
            // toolStripButtonAccountsByFilter
            // 
            this.toolStripButtonAccountsByFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAccountsByFilter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAccountsByFilter.Image")));
            this.toolStripButtonAccountsByFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAccountsByFilter.Name = "toolStripButtonAccountsByFilter";
            this.toolStripButtonAccountsByFilter.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAccountsByFilter.Text = "Добавить все отфильтрованные лицевые счета";
            this.toolStripButtonAccountsByFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonAccountsByFilter.Click += new System.EventHandler(this.toolStripButtonAccountsByFilter_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonAccountDelete
            // 
            this.toolStripButtonAccountDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAccountDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAccountDelete.Image")));
            this.toolStripButtonAccountDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAccountDelete.Name = "toolStripButtonAccountDelete";
            this.toolStripButtonAccountDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAccountDelete.Text = "Удалить текущий лицевой счет";
            this.toolStripButtonAccountDelete.Click += new System.EventHandler(this.toolStripButtonAccountDelete_Click);
            // 
            // toolStripButtonAccountDeleteAll
            // 
            this.toolStripButtonAccountDeleteAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAccountDeleteAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAccountDeleteAll.Image")));
            this.toolStripButtonAccountDeleteAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAccountDeleteAll.Name = "toolStripButtonAccountDeleteAll";
            this.toolStripButtonAccountDeleteAll.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAccountDeleteAll.Text = "Удалить все лицевые счета";
            this.toolStripButtonAccountDeleteAll.Click += new System.EventHandler(this.toolStripButtonAccountDeleteAll_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCreateClaims
            // 
            this.toolStripButtonCreateClaims.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCreateClaims.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCreateClaims.Image")));
            this.toolStripButtonCreateClaims.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreateClaims.Name = "toolStripButtonCreateClaims";
            this.toolStripButtonCreateClaims.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCreateClaims.Text = "Создать претензионно-исковую работу по всем выбранным счетам";
            this.toolStripButtonCreateClaims.Click += new System.EventHandler(this.toolStripButtonCreateClaims_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAccountCurrent,
            this.toolStripButtonAccountsByFilter,
            this.toolStripSeparator1,
            this.toolStripButtonAccountDelete,
            this.toolStripButtonAccountDeleteAll,
            this.toolStripSeparator2,
            this.toolStripButtonCreateClaims,
            this.toolStripProgressBarMultiOperations});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(608, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripProgressBarMultiOperations
            // 
            this.toolStripProgressBarMultiOperations.Name = "toolStripProgressBarMultiOperations";
            this.toolStripProgressBarMultiOperations.Size = new System.Drawing.Size(100, 22);
            this.toolStripProgressBarMultiOperations.Visible = false;
            // 
            // MultiPaymentAccountsMaster
            // 
            this.ClientSize = new System.Drawing.Size(608, 157);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MultiPaymentAccountsMaster";
            this.TabText = "Мастер массовых операций над лицевыми счетами";
            this.Text = "Мастер массовых операций над лицевыми счетами";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn date;
        private DataGridViewTextBoxColumn id_account;
        private DataGridViewTextBoxColumn crn;
        private DataGridViewTextBoxColumn raw_address;
        private DataGridViewTextBoxColumn parsed_address;
        private DataGridViewTextBoxColumn account;
        private DataGridViewTextBoxColumn tenant;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn prescribed;
        private DataGridViewTextBoxColumn balance_input;
        private DataGridViewTextBoxColumn balance_tenancy;
        private DataGridViewTextBoxColumn balance_dgi;
        private DataGridViewTextBoxColumn charging_tenancy;
        private DataGridViewTextBoxColumn charging_dgi;
        private DataGridViewTextBoxColumn charging_total;
        private DataGridViewTextBoxColumn recalc_tenancy;
        private DataGridViewTextBoxColumn recalc_dgi;
        private DataGridViewTextBoxColumn payment_tenancy;
        private DataGridViewTextBoxColumn payment_dgi;
        private DataGridViewTextBoxColumn transfer_balance;
        private DataGridViewTextBoxColumn balance_output_total;
        private DataGridViewTextBoxColumn balance_output_tenancy;
        private DataGridViewTextBoxColumn balance_output_dgi;
        private ToolStripButton toolStripButtonAccountCurrent;
        private ToolStripButton toolStripButtonAccountsByFilter;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonAccountDelete;
        private ToolStripButton toolStripButtonAccountDeleteAll;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonCreateClaims;
        private ToolStrip toolStrip1;
        private ToolStripProgressBar toolStripProgressBarMultiOperations;

    }
}
