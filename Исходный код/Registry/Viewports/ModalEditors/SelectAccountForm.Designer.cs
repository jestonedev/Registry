namespace Registry.Viewport.ModalEditors
{
    partial class SelectAccountForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectAccountForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSave = new VIBlend.WinForms.Controls.vButton();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charging_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.vButtonCancel);
            this.panel1.Controls.Add(this.vButtonSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 208);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(784, 54);
            this.panel1.TabIndex = 10;
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(397, 9);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 2;
            this.vButtonCancel.Text = "Не сохранять";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSave
            // 
            this.vButtonSave.AllowAnimations = true;
            this.vButtonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonSave.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSave.Location = new System.Drawing.Point(274, 9);
            this.vButtonSave.Name = "vButtonSave";
            this.vButtonSave.RoundedCornersMask = ((byte)(15));
            this.vButtonSave.Size = new System.Drawing.Size(117, 35);
            this.vButtonSave.TabIndex = 1;
            this.vButtonSave.Text = "Сохранить";
            this.vButtonSave.UseVisualStyleBackColor = false;
            this.vButtonSave.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSave.Click += new System.EventHandler(this.vButtonSave_Click);
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
            this.charging_date,
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
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(784, 208);
            this.dataGridView.TabIndex = 11;
            this.dataGridView.VisibleChanged += new System.EventHandler(this.dataGridView_VisibleChanged);
            // 
            // date
            // 
            this.date.HeaderText = "Состояние на дату";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 130;
            // 
            // charging_date
            // 
            this.charging_date.HeaderText = "Дата последнего начисления";
            this.charging_date.Name = "charging_date";
            this.charging_date.ReadOnly = true;
            this.charging_date.Width = 130;
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
            // SelectAccountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(784, 262);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectAccountForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выберите необходимый лицевой счет";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private VIBlend.WinForms.Controls.vButton vButtonSave;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn charging_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn id_account;
        private System.Windows.Forms.DataGridViewTextBoxColumn crn;
        private System.Windows.Forms.DataGridViewTextBoxColumn raw_address;
        private System.Windows.Forms.DataGridViewTextBoxColumn parsed_address;
        private System.Windows.Forms.DataGridViewTextBoxColumn account;
        private System.Windows.Forms.DataGridViewTextBoxColumn tenant;
        private System.Windows.Forms.DataGridViewTextBoxColumn total_area;
        private System.Windows.Forms.DataGridViewTextBoxColumn living_area;
        private System.Windows.Forms.DataGridViewTextBoxColumn prescribed;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_input;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_tenancy;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_dgi;
        private System.Windows.Forms.DataGridViewTextBoxColumn charging_tenancy;
        private System.Windows.Forms.DataGridViewTextBoxColumn charging_dgi;
        private System.Windows.Forms.DataGridViewTextBoxColumn charging_total;
        private System.Windows.Forms.DataGridViewTextBoxColumn recalc_tenancy;
        private System.Windows.Forms.DataGridViewTextBoxColumn recalc_dgi;
        private System.Windows.Forms.DataGridViewTextBoxColumn payment_tenancy;
        private System.Windows.Forms.DataGridViewTextBoxColumn payment_dgi;
        private System.Windows.Forms.DataGridViewTextBoxColumn transfer_balance;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_output_total;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_output_tenancy;
        private System.Windows.Forms.DataGridViewTextBoxColumn balance_output_dgi;

    }
}