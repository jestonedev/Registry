using System.Windows.Forms;

namespace Registry.Viewport
{
    public partial class MultiExcerptsMaster
    {

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiExcerptsMaster));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonBuildingAllPremises = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonBuildingMunPremises = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPremisesCurrent = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPremisesByFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPremisesDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPremisesDeleteAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonGenerateExcerpt = new System.Windows.Forms.ToolStripButton();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_street = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.house = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonBuildingAllPremises,
            this.toolStripButtonBuildingMunPremises,
            this.toolStripButtonPremisesCurrent,
            this.toolStripButtonPremisesByFilter,
            this.toolStripSeparator1,
            this.toolStripButtonPremisesDelete,
            this.toolStripButtonPremisesDeleteAll,
            this.toolStripSeparator2,
            this.toolStripButtonGenerateExcerpt});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(608, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonBuildingAllPremises
            // 
            this.toolStripButtonBuildingAllPremises.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBuildingAllPremises.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBuildingAllPremises.Image")));
            this.toolStripButtonBuildingAllPremises.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBuildingAllPremises.Name = "toolStripButtonBuildingAllPremises";
            this.toolStripButtonBuildingAllPremises.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBuildingAllPremises.Text = "Добавить все помещения здания";
            this.toolStripButtonBuildingAllPremises.Click += new System.EventHandler(this.toolStripButtonBuildingAllPremises_Click);
            // 
            // toolStripButtonBuildingMunPremises
            // 
            this.toolStripButtonBuildingMunPremises.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBuildingMunPremises.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBuildingMunPremises.Image")));
            this.toolStripButtonBuildingMunPremises.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBuildingMunPremises.Name = "toolStripButtonBuildingMunPremises";
            this.toolStripButtonBuildingMunPremises.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBuildingMunPremises.Text = "Добавить муниципальные помещения здания";
            this.toolStripButtonBuildingMunPremises.Click += new System.EventHandler(this.toolStripButtonBuildingMunPremises_Click);
            // 
            // toolStripButtonPremisesCurrent
            // 
            this.toolStripButtonPremisesCurrent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPremisesCurrent.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPremisesCurrent.Image")));
            this.toolStripButtonPremisesCurrent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPremisesCurrent.Name = "toolStripButtonPremisesCurrent";
            this.toolStripButtonPremisesCurrent.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPremisesCurrent.Text = "Добавить текущее помещение";
            this.toolStripButtonPremisesCurrent.Click += new System.EventHandler(this.toolStripButtonPremisesCurrent_Click);
            // 
            // toolStripButtonPremisesByFilter
            // 
            this.toolStripButtonPremisesByFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPremisesByFilter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPremisesByFilter.Image")));
            this.toolStripButtonPremisesByFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPremisesByFilter.Name = "toolStripButtonPremisesByFilter";
            this.toolStripButtonPremisesByFilter.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPremisesByFilter.Text = "Добавить все отфильтрованные помещения";
            this.toolStripButtonPremisesByFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonPremisesByFilter.Click += new System.EventHandler(this.toolStripButtonPremisesByFilter_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonPremisesDelete
            // 
            this.toolStripButtonPremisesDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPremisesDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPremisesDelete.Image")));
            this.toolStripButtonPremisesDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPremisesDelete.Name = "toolStripButtonPremisesDelete";
            this.toolStripButtonPremisesDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPremisesDelete.Text = "Удалить текущее помещение";
            this.toolStripButtonPremisesDelete.Click += new System.EventHandler(this.toolStripButtonPremisesDelete_Click);
            // 
            // toolStripButtonPremisesDeleteAll
            // 
            this.toolStripButtonPremisesDeleteAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPremisesDeleteAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPremisesDeleteAll.Image")));
            this.toolStripButtonPremisesDeleteAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPremisesDeleteAll.Name = "toolStripButtonPremisesDeleteAll";
            this.toolStripButtonPremisesDeleteAll.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPremisesDeleteAll.Text = "Удалить все помещения";
            this.toolStripButtonPremisesDeleteAll.Click += new System.EventHandler(this.toolStripButtonPremisesDeleteAll_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonGenerateExcerpt
            // 
            this.toolStripButtonGenerateExcerpt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGenerateExcerpt.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonGenerateExcerpt.Image")));
            this.toolStripButtonGenerateExcerpt.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGenerateExcerpt.Name = "toolStripButtonGenerateExcerpt";
            this.toolStripButtonGenerateExcerpt.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonGenerateExcerpt.Text = "Сформировать выписку";
            this.toolStripButtonGenerateExcerpt.Click += new System.EventHandler(this.toolStripButtonGenerateExcerpt_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_premises,
            this.id_street,
            this.house,
            this.premises_num});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 25);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(608, 132);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // id_premises
            // 
            this.id_premises.HeaderText = "№";
            this.id_premises.MinimumWidth = 50;
            this.id_premises.Name = "id_premises";
            this.id_premises.ReadOnly = true;
            this.id_premises.Width = 50;
            // 
            // id_street
            // 
            this.id_street.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.id_street.HeaderText = "Улица";
            this.id_street.MinimumWidth = 300;
            this.id_street.Name = "id_street";
            this.id_street.ReadOnly = true;
            // 
            // house
            // 
            this.house.HeaderText = "Дом";
            this.house.MinimumWidth = 50;
            this.house.Name = "house";
            this.house.ReadOnly = true;
            this.house.Width = 50;
            // 
            // premises_num
            // 
            this.premises_num.HeaderText = "Квартира";
            this.premises_num.MinimumWidth = 75;
            this.premises_num.Name = "premises_num";
            this.premises_num.ReadOnly = true;
            this.premises_num.Width = 75;
            // 
            // MultiExcerptsMaster
            // 
            this.ClientSize = new System.Drawing.Size(608, 157);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MultiExcerptsMaster";
            this.TabText = "Мастер формирования массовой выписки";
            this.Text = "Мастер формирования массовой выписки";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonBuildingAllPremises;
        private ToolStripButton toolStripButtonBuildingMunPremises;
        private ToolStripButton toolStripButtonPremisesCurrent;
        private ToolStripButton toolStripButtonPremisesByFilter;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonPremisesDelete;
        private ToolStripButton toolStripButtonPremisesDeleteAll;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonGenerateExcerpt;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn premises_num;

    }
}
