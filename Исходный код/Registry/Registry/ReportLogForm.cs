using System;
using System.Globalization;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry
{
    internal sealed partial class ReportLogForm : DockContent
    {
        public ReportLogForm()
        {
            InitializeComponent();
            DockAreas = ((DockAreas.DockLeft | DockAreas.DockRight)
                         | DockAreas.DockTop)
                        | DockAreas.DockBottom;
        }

        public void Log(string text)
        {
            _richTextBox1.AppendText((_richTextBox1.Lines.Length == 0 ? "" : Environment.NewLine) + 
                (_richTextBox1.Lines.Length + 1).ToString(CultureInfo.InvariantCulture) + ". " + text);
        }
    }
}
