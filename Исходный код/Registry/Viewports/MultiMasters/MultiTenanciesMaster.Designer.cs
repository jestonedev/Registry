using System.Windows.Forms;

namespace Registry.Viewport.MultiMasters
{
    internal partial class MultiTenanciesMaster
    {

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiTenanciesMaster));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonTenancyCurrent = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTenanciesByFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTenancyDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTenancyDeleteAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRequestMvd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRequestMvdNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExportReasonsForGisZkh = new System.Windows.Forms.ToolStripButton();
            this.toolStripProgressBarMultiOperations = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelRowCount = new System.Windows.Forms.ToolStripLabel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.end_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.residence_warrant_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rent_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStripButtonGisZkhExport = new System.Windows.Forms.ToolStripButton();
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
            this.toolStripButtonExportReasonsForGisZkh,
            this.toolStripButtonGisZkhExport,
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
            // toolStripButtonExportReasonsForGisZkh
            // 
            this.toolStripButtonExportReasonsForGisZkh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonExportReasonsForGisZkh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExportReasonsForGisZkh.Image")));
            this.toolStripButtonExportReasonsForGisZkh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExportReasonsForGisZkh.Name = "toolStripButtonExportReasonsForGisZkh";
            this.toolStripButtonExportReasonsForGisZkh.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonExportReasonsForGisZkh.Text = "Экспорт файлов-оснований найма для ГИС ЖКХ";
            this.toolStripButtonExportReasonsForGisZkh.Click += new System.EventHandler(this.toolStripButtonExportReasonsForGisZkh_Click);
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
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_process,
            this.registration_num,
            this.registration_date,
            this.end_date,
            this.residence_warrant_num,
            this.tenant,
            this.rent_type,
            this.address,
            this.payment});
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
            // id_process
            // 
            this.id_process.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.id_process.HeaderText = "№";
            this.id_process.MinimumWidth = 60;
            this.id_process.Name = "id_process";
            this.id_process.ReadOnly = true;
            this.id_process.Width = 60;
            // 
            // registration_num
            // 
            this.registration_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.registration_num.HeaderText = "№ договора";
            this.registration_num.MinimumWidth = 90;
            this.registration_num.Name = "registration_num";
            this.registration_num.ReadOnly = true;
            this.registration_num.Width = 90;
            // 
            // registration_date
            // 
            this.registration_date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.registration_date.HeaderText = "Дата регистрации договора";
            this.registration_date.MinimumWidth = 90;
            this.registration_date.Name = "registration_date";
            this.registration_date.ReadOnly = true;
            this.registration_date.Width = 90;
            // 
            // end_date
            // 
            this.end_date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.end_date.HeaderText = "Дата окончания договора";
            this.end_date.MinimumWidth = 90;
            this.end_date.Name = "end_date";
            this.end_date.ReadOnly = true;
            this.end_date.Width = 90;
            // 
            // residence_warrant_num
            // 
            this.residence_warrant_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.residence_warrant_num.HeaderText = "№ ордера / другого документа-основания";
            this.residence_warrant_num.MinimumWidth = 150;
            this.residence_warrant_num.Name = "residence_warrant_num";
            this.residence_warrant_num.ReadOnly = true;
            this.residence_warrant_num.Width = 150;
            // 
            // tenant
            // 
            this.tenant.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.tenant.HeaderText = "Наниматель";
            this.tenant.MinimumWidth = 250;
            this.tenant.Name = "tenant";
            this.tenant.ReadOnly = true;
            this.tenant.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.tenant.Width = 250;
            // 
            // rent_type
            // 
            this.rent_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.rent_type.HeaderText = "Тип найма";
            this.rent_type.MinimumWidth = 140;
            this.rent_type.Name = "rent_type";
            this.rent_type.ReadOnly = true;
            this.rent_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.rent_type.Width = 140;
            // 
            // address
            // 
            this.address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.address.HeaderText = "Нанимаемое жилье";
            this.address.MinimumWidth = 400;
            this.address.Name = "address";
            this.address.ReadOnly = true;
            this.address.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.address.Width = 400;
            // 
            // payment
            // 
            this.payment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.Format = "#0.## руб\\.";
            dataGridViewCellStyle6.NullValue = null;
            this.payment.DefaultCellStyle = dataGridViewCellStyle6;
            this.payment.HeaderText = "Размер платы";
            this.payment.MinimumWidth = 150;
            this.payment.Name = "payment";
            this.payment.ReadOnly = true;
            this.payment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.payment.Width = 150;
            // 
            // toolStripButtonGisZkhExport
            // 
            this.toolStripButtonGisZkhExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGisZkhExport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonGisZkhExport.Image")));
            this.toolStripButtonGisZkhExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGisZkhExport.Name = "toolStripButtonGisZkhExport";
            this.toolStripButtonGisZkhExport.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonGisZkhExport.Text = "Экспорт для ГИС \"ЖКХ\"";
            this.toolStripButtonGisZkhExport.Click += new System.EventHandler(this.toolStripButtonGisZkhExport_Click);
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
        private ToolStripButton toolStripButtonRequestMvdNew;
        private ToolStripLabel toolStripLabelRowCount;
        private ToolStripSeparator toolStripSeparator3;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewTextBoxColumn registration_num;
        private DataGridViewTextBoxColumn registration_date;
        private DataGridViewTextBoxColumn end_date;
        private DataGridViewTextBoxColumn residence_warrant_num;
        private DataGridViewTextBoxColumn tenant;
        private DataGridViewTextBoxColumn rent_type;
        private DataGridViewTextBoxColumn address;
        private DataGridViewTextBoxColumn payment;
        private ToolStripButton toolStripButtonExportReasonsForGisZkh;
        private ToolStripButton toolStripButtonGisZkhExport;

    }
}
