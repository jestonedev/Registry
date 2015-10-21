using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry
{
    internal partial class ReportLogForm
    {
        private RichTextBox _richTextBox1;

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(ReportLogForm));
            this._richTextBox1 = new RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this._richTextBox1.BackColor = Color.White;
            this._richTextBox1.BorderStyle = BorderStyle.None;
            this._richTextBox1.Dock = DockStyle.Fill;
            this._richTextBox1.ForeColor = Color.Black;
            this._richTextBox1.Location = new Point(0, 0);
            this._richTextBox1.Name = "_richTextBox1";
            this._richTextBox1.ReadOnly = true;
            this._richTextBox1.Size = new Size(572, 71);
            this._richTextBox1.TabIndex = 0;
            this._richTextBox1.Text = "";
            // 
            // ReportLogForm
            // 
            this.ClientSize = new Size(572, 71);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this._richTextBox1);
            this.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = Color.Black;
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportLogForm";
            this.TabText = "Лог отчетов";
            this.Text = "Лог отчетов";
            this.ResumeLayout(false);
        }

    }
}
