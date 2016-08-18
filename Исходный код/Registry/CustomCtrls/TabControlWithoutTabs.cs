using System;
using System.Windows.Forms;

namespace CustomControls
{
    public class TabControlWithoutTabs: TabControl
    {
        private const int TCM_ADJUSTRECT = 0x1328;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == TCM_ADJUSTRECT && !DesignMode)
            {
                m.Result = (IntPtr)1;
                return;
            }
            base.WndProc(ref m);
        }
    }
}
