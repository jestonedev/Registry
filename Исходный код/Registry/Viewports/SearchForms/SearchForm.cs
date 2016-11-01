using System;
using System.Windows.Forms;

namespace Registry.Viewport.SearchForms
{
    public class SearchForm: Form
    {
        internal virtual string GetFilter() { return ""; }

        protected void HandleHotKeys(Control.ControlCollection controls, Action<object, EventArgs> eventClick)
        {
            foreach (Control control in controls)
            {
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            eventClick(sender, e);
                            break;
                        case Keys.Escape:
                            DialogResult = DialogResult.Cancel;
                            break;
                    }
                };
                if (control.Controls.Count > 0)
                {
                    HandleHotKeys(control.Controls, eventClick);
                }
            }
        }
    }
}
