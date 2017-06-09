using System.Windows.Forms;

namespace Registry.Viewport.MultiMasters
{
    internal partial class MultiPaymentAccountsMaster
    {

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiPaymentAccountsMaster));
            this.toolStripButtonAccountCurrent = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAccountsByFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAccountDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAccountDeleteAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCreateClaims = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRequestToBks = new System.Windows.Forms.ToolStripButton();
            this.toolStripProgressBarMultiOperations = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelRowCount = new System.Windows.Forms.ToolStripLabel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.balance_input_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_padun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_pkk = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_padun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_pkk = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transfer_balance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recalc_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recalc_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recalc_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recalc_padun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recalc_pkk = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_padun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_pkk = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_padun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_output_pkk = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripButtonAccountCurrent
            // 
            this.toolStripButtonAccountCurrent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAccountCurrent.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAccountCurrent.Image")));
            this.toolStripButtonAccountCurrent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAccountCurrent.Name = "toolStripButtonAccountCurrent";
            this.toolStripButtonAccountCurrent.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAccountCurrent.Text = "Добавить выбранные лицевые счета";
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
            this.toolStripButtonRequestToBks,
            this.toolStripButtonExport,
            this.toolStripProgressBarMultiOperations,
            this.toolStripSeparator3,
            this.toolStripLabelRowCount});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(608, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
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
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.date,
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
            this.balance_input_penalties,
            this.balance_dgi,
            this.balance_padun,
            this.balance_pkk,
            this.charging_total,
            this.charging_tenancy,
            this.charging_penalties,
            this.charging_dgi,
            this.charging_padun,
            this.charging_pkk,
            this.transfer_balance,
            this.recalc_tenancy,
            this.recalc_penalties,
            this.recalc_dgi,
            this.recalc_padun,
            this.recalc_pkk,
            this.payment_tenancy,
            this.payment_penalties,
            this.payment_dgi,
            this.payment_padun,
            this.payment_pkk,
            this.balance_output_total,
            this.balance_output_tenancy,
            this.balance_output_penalties,
            this.balance_output_dgi,
            this.balance_output_padun,
            this.balance_output_pkk});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 25);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(608, 132);
            this.dataGridView.TabIndex = 7;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView_RowsAdded);
            this.dataGridView.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView_RowsRemoved);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // date
            // 
            this.date.Frozen = true;
            this.date.HeaderText = "Состояние на дату";
            this.date.Name = "date";
            this.date.ReadOnly = true;
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
            // balance_input_penalties
            // 
            this.balance_input_penalties.HeaderText = "Пени (вх.)";
            this.balance_input_penalties.Name = "balance_input_penalties";
            this.balance_input_penalties.ReadOnly = true;
            // 
            // balance_dgi
            // 
            this.balance_dgi.HeaderText = "Сальдо вх. ДГИ";
            this.balance_dgi.Name = "balance_dgi";
            this.balance_dgi.ReadOnly = true;
            // 
            // balance_padun
            // 
            this.balance_padun.HeaderText = "Сальдо вх. Падун";
            this.balance_padun.Name = "balance_padun";
            this.balance_padun.ReadOnly = true;
            // 
            // balance_pkk
            // 
            this.balance_pkk.HeaderText = "Сальдо вх. ПКК";
            this.balance_pkk.Name = "balance_pkk";
            this.balance_pkk.ReadOnly = true;
            // 
            // charging_total
            // 
            this.charging_total.HeaderText = "Начисление итого";
            this.charging_total.Name = "charging_total";
            this.charging_total.ReadOnly = true;
            // 
            // charging_tenancy
            // 
            this.charging_tenancy.HeaderText = "Начисление найм";
            this.charging_tenancy.Name = "charging_tenancy";
            this.charging_tenancy.ReadOnly = true;
            // 
            // charging_penalties
            // 
            this.charging_penalties.HeaderText = "Начисление пени";
            this.charging_penalties.Name = "charging_penalties";
            this.charging_penalties.ReadOnly = true;
            // 
            // charging_dgi
            // 
            this.charging_dgi.HeaderText = "Начисление ДГИ";
            this.charging_dgi.Name = "charging_dgi";
            this.charging_dgi.ReadOnly = true;
            // 
            // charging_padun
            // 
            this.charging_padun.HeaderText = "Начисление Падун";
            this.charging_padun.Name = "charging_padun";
            this.charging_padun.ReadOnly = true;
            // 
            // charging_pkk
            // 
            this.charging_pkk.HeaderText = "Начисление ПКК";
            this.charging_pkk.Name = "charging_pkk";
            this.charging_pkk.ReadOnly = true;
            // 
            // transfer_balance
            // 
            this.transfer_balance.HeaderText = "Перенос сальдо";
            this.transfer_balance.Name = "transfer_balance";
            this.transfer_balance.ReadOnly = true;
            // 
            // recalc_tenancy
            // 
            this.recalc_tenancy.HeaderText = "Перерасчет найм";
            this.recalc_tenancy.Name = "recalc_tenancy";
            this.recalc_tenancy.ReadOnly = true;
            // 
            // recalc_penalties
            // 
            this.recalc_penalties.HeaderText = "Перерасчет пени";
            this.recalc_penalties.Name = "recalc_penalties";
            this.recalc_penalties.ReadOnly = true;
            // 
            // recalc_dgi
            // 
            this.recalc_dgi.HeaderText = "Перерасчет ДГИ";
            this.recalc_dgi.Name = "recalc_dgi";
            this.recalc_dgi.ReadOnly = true;
            // 
            // recalc_padun
            // 
            this.recalc_padun.HeaderText = "Перерасчет Падун";
            this.recalc_padun.Name = "recalc_padun";
            this.recalc_padun.ReadOnly = true;
            // 
            // recalc_pkk
            // 
            this.recalc_pkk.HeaderText = "Перерасчет ПКК";
            this.recalc_pkk.Name = "recalc_pkk";
            this.recalc_pkk.ReadOnly = true;
            // 
            // payment_tenancy
            // 
            this.payment_tenancy.HeaderText = "Оплата найм";
            this.payment_tenancy.Name = "payment_tenancy";
            this.payment_tenancy.ReadOnly = true;
            // 
            // payment_penalties
            // 
            this.payment_penalties.HeaderText = "Оплата пени";
            this.payment_penalties.Name = "payment_penalties";
            this.payment_penalties.ReadOnly = true;
            // 
            // payment_dgi
            // 
            this.payment_dgi.HeaderText = "Оплата ДГИ";
            this.payment_dgi.Name = "payment_dgi";
            this.payment_dgi.ReadOnly = true;
            // 
            // payment_padun
            // 
            this.payment_padun.HeaderText = "Оплата Падун";
            this.payment_padun.Name = "payment_padun";
            this.payment_padun.ReadOnly = true;
            // 
            // payment_pkk
            // 
            this.payment_pkk.HeaderText = "Оплата ПКК";
            this.payment_pkk.Name = "payment_pkk";
            this.payment_pkk.ReadOnly = true;
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
            // balance_output_penalties
            // 
            this.balance_output_penalties.HeaderText = "Пени исх.";
            this.balance_output_penalties.Name = "balance_output_penalties";
            this.balance_output_penalties.ReadOnly = true;
            // 
            // balance_output_dgi
            // 
            this.balance_output_dgi.HeaderText = "Сальдо исх. ДГИ";
            this.balance_output_dgi.Name = "balance_output_dgi";
            this.balance_output_dgi.ReadOnly = true;
            // 
            // balance_output_padun
            // 
            this.balance_output_padun.HeaderText = "Сальдо исх. Падун";
            this.balance_output_padun.Name = "balance_output_padun";
            this.balance_output_padun.ReadOnly = true;
            // 
            // balance_output_pkk
            // 
            this.balance_output_pkk.HeaderText = "Сальдо исх. ПКК";
            this.balance_output_pkk.Name = "balance_output_pkk";
            this.balance_output_pkk.ReadOnly = true;
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonExport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExport.Image")));
            this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonExport.Text = "Экспорт";
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
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
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private ToolStripButton toolStripButtonAccountCurrent;
        private ToolStripButton toolStripButtonAccountsByFilter;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonAccountDelete;
        private ToolStripButton toolStripButtonAccountDeleteAll;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonCreateClaims;
        private ToolStrip toolStrip1;
        private ToolStripProgressBar toolStripProgressBarMultiOperations;
        private ToolStripButton toolStripButtonRequestToBks;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripLabel toolStripLabelRowCount;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn date;
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
        private DataGridViewTextBoxColumn balance_input_penalties;
        private DataGridViewTextBoxColumn balance_dgi;
        private DataGridViewTextBoxColumn balance_padun;
        private DataGridViewTextBoxColumn balance_pkk;
        private DataGridViewTextBoxColumn charging_total;
        private DataGridViewTextBoxColumn charging_tenancy;
        private DataGridViewTextBoxColumn charging_penalties;
        private DataGridViewTextBoxColumn charging_dgi;
        private DataGridViewTextBoxColumn charging_padun;
        private DataGridViewTextBoxColumn charging_pkk;
        private DataGridViewTextBoxColumn transfer_balance;
        private DataGridViewTextBoxColumn recalc_tenancy;
        private DataGridViewTextBoxColumn recalc_penalties;
        private DataGridViewTextBoxColumn recalc_dgi;
        private DataGridViewTextBoxColumn recalc_padun;
        private DataGridViewTextBoxColumn recalc_pkk;
        private DataGridViewTextBoxColumn payment_tenancy;
        private DataGridViewTextBoxColumn payment_penalties;
        private DataGridViewTextBoxColumn payment_dgi;
        private DataGridViewTextBoxColumn payment_padun;
        private DataGridViewTextBoxColumn payment_pkk;
        private DataGridViewTextBoxColumn balance_output_total;
        private DataGridViewTextBoxColumn balance_output_tenancy;
        private DataGridViewTextBoxColumn balance_output_penalties;
        private DataGridViewTextBoxColumn balance_output_dgi;
        private DataGridViewTextBoxColumn balance_output_padun;
        private DataGridViewTextBoxColumn balance_output_pkk;
        private ToolStripButton toolStripButtonExport;

    }
}
