using System;
using System.Windows.Forms;

namespace CustomControls
{
    public class DataGridViewDateTimeColumn: DataGridViewColumn
    {
        public DataGridViewDateTimeColumn()
            : base(new DataGridViewDateTimeCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a DateTimeCell. 
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(DataGridViewDateTimeCell)))
                {
                    throw new InvalidCastException("Must be a DateTimeCell");
                }
                base.CellTemplate = value;
            }
        }
    }
}
