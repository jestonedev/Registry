﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Data;

namespace Registry.Viewport
{
    internal static class ViewportHelper
    {
        internal static void SetDoubleBuffered(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, control, new object[] { true });
        }

        internal static DateTime ValueOrDefault(DateTime? value)
        {
            if (value != null)
                return value.Value;
            else
                return DateTime.Now.Date;
        }

        internal static double ValueOrDefault(double? value)
        {
            if (value != null)
                return value.Value;
            else
                return 0;
        }

        internal static int ValueOrDefault(int? value)
        {
            if (value != null)
                return value.Value;
            else
                return 0;
        }

        internal static bool ValueOrDefault(bool? value)
        {
            if (value != null)
                return value.Value;
            else
                return false;
        }

        internal static decimal ValueOrDefault(decimal? value)
        {
            if (value != null)
                return value.Value;
            else
                return 0;
        }

        internal static string ValueOrNull(TextBox control)
        {
            if (control.Text.Trim() == "")
                return null;
            else
                return control.Text.Trim().Trim();
        }

        internal static T? ValueOrNull<T>(ComboBox control) where T: struct
        {
            if (control.SelectedValue == null)
                return null;
            else
                return (T?)control.SelectedValue;
        }

        internal static string ValueOrNull(ComboBox control)
        {
            if (control.SelectedValue == null)
                return null;
            else
                return control.SelectedValue.ToString().Trim();
        }

        internal static DateTime? ValueOrNull(DateTimePicker control)
        {
            if (control.Checked)
                return control.Value;
            else
                return null;
        }

        internal static T? ValueOrNull<T>(DataRowView row, string property) where T : struct
        {
            if (row[property] is DBNull)
                return null;
            else
                return (T?)Convert.ChangeType(row[property], typeof(T));
        }

        internal static string ValueOrNull(DataRowView row, string property)
        {
            if (row[property] is DBNull)
                return null;
            else
                return row[property].ToString().Trim();
        }

        internal static T? ValueOrNull<T>(DataRow row, string property) where T : struct
        {
            if (row[property] is DBNull)
                return null;
            else
                return (T?)Convert.ChangeType(row[property], typeof(T));
        }

        internal static string ValueOrNull(DataRow row, string property)
        {
            if (row[property] is DBNull)
                return null;
            else
                return row[property].ToString().Trim();
        }

        internal static T? ValueOrNull<T>(DataGridViewRow row, string property) where T : struct
        {
            if (row.Cells[property].Value is DBNull)
                return null;
            else
                return (T?)Convert.ChangeType(row.Cells[property].Value, typeof(T));
        }

        internal static string ValueOrNull(DataGridViewRow row, string property)
        {
            if (row.Cells[property].Value is DBNull)
                return null;
            else
                return row.Cells[property].Value.ToString().Trim();
        }

        internal static object ValueOrDBNull(object value)
        {
            return value == null ? DBNull.Value : value;
        }
    }
}
