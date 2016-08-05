using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using Registry.DataModels.CalcDataModels;

namespace Registry.Viewport
{
    internal static class ViewportHelper
    {
        internal static void SetDoubleBuffered(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, control, new object[] { true }, CultureInfo.InvariantCulture);
        }

        internal static DateTime ValueOrDefault(DateTime? value)
        {
            return value ?? DateTime.Now.Date;
        }

        internal static double ValueOrDefault(double? value)
        {
            return value ?? 0;
        }

        internal static int ValueOrDefault(int? value)
        {
            return value ?? 0;
        }

        internal static decimal ValueOrDefault(decimal? value)
        {
            return value ?? 0;
        }

        internal static bool ValueOrDefault(bool? value)
        {
            return value != null && value.Value;
        }

        internal static string ValueOrNull(TextBox control)
        {
            return string.IsNullOrEmpty(control.Text.Trim()) ? null : control.Text.Trim();
        }

        internal static T? ValueOrNull<T>(ComboBox control) where T: struct
        {
            return (T?) control.SelectedValue;
        }

        internal static string ValueOrNull(ComboBox control)
        {
            return control.SelectedValue == null ? null : control.SelectedValue.ToString().Trim();
        }

        internal static DateTime? ValueOrNull(DateTimePicker control)
        {
            if (control.Checked)
                return control.Value.Date;
            return null;
        }

        internal static T? ValueOrNull<T>(DataRowView row, string property) where T : struct
        {
            if (row[property] is DBNull)
                return null;
            return (T?)Convert.ChangeType(row[property], typeof(T), CultureInfo.InvariantCulture);
        }

        internal static string ValueOrNull(DataRowView row, string property)
        {
            if (row[property] is DBNull)
                return null;
            return row[property].ToString().Trim();
        }

        internal static T? ValueOrNull<T>(DataRow row, string property) where T : struct
        {
            if (row[property] is DBNull)
                return null;
            return (T?)Convert.ChangeType(row[property], typeof(T), CultureInfo.InvariantCulture);
        }

        internal static string ValueOrNull(DataRow row, string property)
        {
            if (row[property] is DBNull)
                return null;
            return row[property].ToString().Trim();
        }

        internal static T? ValueOrNull<T>(DataGridViewRow row, string property) where T : struct
        {
            if (row.Cells[property].Value is DBNull)
                return null;
            if (row.Cells[property].Value == null)
                return null;
            return (T?)Convert.ChangeType(row.Cells[property].Value, typeof(T), CultureInfo.InvariantCulture);
        }

        internal static string ValueOrNull(DataGridViewRow row, string property)
        {
            if (row.Cells[property].Value is DBNull)
                return null;
            return row.Cells[property].Value == null ? null : row.Cells[property].Value.ToString().Trim();
        }

        internal static object ValueOrDbNull(object value)
        {
            return value ?? DBNull.Value;
        }

        internal static void SelectAllText(object sender)
        {
            if (sender is NumericUpDown)
            {
                var control = (NumericUpDown) sender;
                control.Select(0, control.Text.Length);
            }
            else if (sender is TextBox)
            {
                var control = (TextBox) sender;
                control.SelectAll();
            } else if (sender is ComboBox)
            {
                var control = (ComboBox)sender;
                control.SelectAll();
            }
        }


        internal static void BindProperty(IBindableComponent control, string controlProperty,
            BindingSource dataSource, string dataSourceProperty, object defaultValue)
        {
            control.DataBindings.Clear();
            control.DataBindings.Add(controlProperty, dataSource, dataSourceProperty, true, DataSourceUpdateMode.Never, defaultValue);
        }

        internal static void BindSource(ComboBox comboBox, BindingSource dataSource, string displayMember, string valueMember)
        {
            comboBox.DataSource = dataSource;
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;
        }

        internal static void BindSource(DataGridViewComboBoxColumn comboBox, BindingSource dataSource, string displayMember, string valueMember, string dataPropertyName = null)
        {
            comboBox.DataSource = dataSource;
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;
            comboBox.DataPropertyName = dataPropertyName ?? valueMember;
        }
    }
}
