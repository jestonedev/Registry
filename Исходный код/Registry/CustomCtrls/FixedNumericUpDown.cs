using System.Windows.Forms;

namespace CustomControls
{
    public class FixedNumericUpDown : NumericUpDown
    {
        public override void DownButton()
        {
            if (ReadOnly)
                return;
            base.DownButton();
        }

        public override void UpButton()
        {
            if (ReadOnly)
                return;
            base.UpButton();
        }
    }
}
