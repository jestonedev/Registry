namespace Registry.Viewport
{
    partial class TenancyFiles
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimFiles));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_file = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.file_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.display_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vButtonOpenFile = new VIBlend.WinForms.Controls.vButton();
            this.vButtonDeleteFile = new VIBlend.WinForms.Controls.vButton();
            this.vButtonAddFile = new VIBlend.WinForms.Controls.vButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_file,
            this.file_name,
            this.display_name});
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(623, 327);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            // 
            // id_file
            // 
            this.id_file.HeaderText = "IdFile";
            this.id_file.Name = "id_file";
            this.id_file.ReadOnly = true;
            this.id_file.Visible = false;
            // 
            // file_name
            // 
            this.file_name.HeaderText = "FileName";
            this.file_name.Name = "file_name";
            this.file_name.ReadOnly = true;
            this.file_name.Visible = false;
            // 
            // display_name
            // 
            this.display_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.display_name.HeaderText = "Имя файла";
            this.display_name.Name = "display_name";
            this.display_name.ReadOnly = true;
            // 
            // vButtonOpenFile
            // 
            this.vButtonOpenFile.AllowAnimations = true;
            this.vButtonOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonOpenFile.BackColor = System.Drawing.Color.Transparent;
            this.vButtonOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("vButtonOpenFile.Image")));
            this.vButtonOpenFile.Location = new System.Drawing.Point(629, 66);
            this.vButtonOpenFile.Name = "vButtonOpenFile";
            this.vButtonOpenFile.RoundedCornersMask = ((byte)(15));
            this.vButtonOpenFile.Size = new System.Drawing.Size(32, 25);
            this.vButtonOpenFile.TabIndex = 5;
            this.vButtonOpenFile.UseVisualStyleBackColor = false;
            this.vButtonOpenFile.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonOpenFile.Click += new System.EventHandler(this.vButtonOpenFile_Click);
            // 
            // vButtonDeleteFile
            // 
            this.vButtonDeleteFile.AllowAnimations = true;
            this.vButtonDeleteFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonDeleteFile.BackColor = System.Drawing.Color.Transparent;
            this.vButtonDeleteFile.Image = ((System.Drawing.Image)(resources.GetObject("vButtonDeleteFile.Image")));
            this.vButtonDeleteFile.Location = new System.Drawing.Point(629, 39);
            this.vButtonDeleteFile.Name = "vButtonDeleteFile";
            this.vButtonDeleteFile.RoundedCornersMask = ((byte)(15));
            this.vButtonDeleteFile.Size = new System.Drawing.Size(32, 25);
            this.vButtonDeleteFile.TabIndex = 4;
            this.vButtonDeleteFile.UseVisualStyleBackColor = false;
            this.vButtonDeleteFile.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonDeleteFile.Click += new System.EventHandler(this.vButtonDeleteFile_Click);
            // 
            // vButtonAddFile
            // 
            this.vButtonAddFile.AllowAnimations = true;
            this.vButtonAddFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonAddFile.BackColor = System.Drawing.Color.Transparent;
            this.vButtonAddFile.Image = ((System.Drawing.Image)(resources.GetObject("vButtonAddFile.Image")));
            this.vButtonAddFile.Location = new System.Drawing.Point(629, 12);
            this.vButtonAddFile.Name = "vButtonAddFile";
            this.vButtonAddFile.RoundedCornersMask = ((byte)(15));
            this.vButtonAddFile.Size = new System.Drawing.Size(32, 25);
            this.vButtonAddFile.TabIndex = 3;
            this.vButtonAddFile.UseVisualStyleBackColor = false;
            this.vButtonAddFile.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonAddFile.Click += new System.EventHandler(this.vButtonAddFile_Click);
            // 
            // ClaimFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(664, 327);
            this.Controls.Add(this.vButtonOpenFile);
            this.Controls.Add(this.vButtonDeleteFile);
            this.Controls.Add(this.vButtonAddFile);
            this.Controls.Add(this.dataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ClaimFiles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Прикрепленные файлы процесса найма №{0}";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private VIBlend.WinForms.Controls.vButton vButtonOpenFile;
        private VIBlend.WinForms.Controls.vButton vButtonDeleteFile;
        private VIBlend.WinForms.Controls.vButton vButtonAddFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn id_file;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn display_name;
    }
}