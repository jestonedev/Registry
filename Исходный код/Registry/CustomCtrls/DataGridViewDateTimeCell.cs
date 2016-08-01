using System;
using System.Windows.Forms;

namespace CustomControls
{
    public class DataGridViewDateTimeCell : DataGridViewTextBoxCell
    {

        public DataGridViewDateTimeCell()
        {
            // Use the short date format. 
            Style.Format = "d";
        }

        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value. 
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);
            var ctl =
                DataGridView.EditingControl as DateTimeEditingControl;
            // Use the default row value when Value property is null. 
            DateTime stub;
            try
            {
                if ((Value == null) || !DateTime.TryParse(Value.ToString(), out stub))
                {
                    if (DefaultNewRowValue == null) return;
                    if (ctl != null) ctl.Value = (DateTime) DefaultNewRowValue;
                }
                else
                {
                    if (ctl != null) ctl.Value = (DateTime) Value;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        public override Type EditType
        {
            get
            {
                // Return the type of the editing control that CalendarCell uses. 
                return typeof(DateTimeEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                // Return the type of the value that CalendarCell contains. 
                return typeof(DateTime);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // Use the current date and time as the default value. 
                return DateTime.Now;
            }
        }
    }
}
