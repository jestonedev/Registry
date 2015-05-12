using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Data;
using System.Globalization;
using Registry.CalcDataModels;

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
            if (String.IsNullOrEmpty(control.Text.Trim()))
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
                return control.Value.Date;
            else
                return null;
        }

        internal static T? ValueOrNull<T>(DataRowView row, string property) where T : struct
        {
            if (row[property] is DBNull)
                return null;
            else
                return (T?)Convert.ChangeType(row[property], typeof(T), CultureInfo.InvariantCulture);
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
                return (T?)Convert.ChangeType(row[property], typeof(T), CultureInfo.InvariantCulture);
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
            if (row.Cells[property].Value == null)
                return null;
            else
                return (T?)Convert.ChangeType(row.Cells[property].Value, typeof(T), CultureInfo.InvariantCulture);
        }

        internal static string ValueOrNull(DataGridViewRow row, string property)
        {
            if (row.Cells[property].Value is DBNull)
                return null;
            else
            if (row.Cells[property].Value == null)
                return null;
            else
                return row.Cells[property].Value.ToString().Trim();
        }

        internal static object ValueOrDBNull(object value)
        {
            return value == null ? DBNull.Value : value;
        }

        internal static int TranslateFundIdToRentId(int fundId)
        {
            switch (fundId)
            {
                case 1: return 3;
                case 2: return 1;
                case 3: return 2;
                default: return 0;
            }
        }

        internal static bool BuildingRentAndFundMatch(int idBuilding, int idRentType)
        {
            DataRow bRow = CalcDataModelBuildingsCurrentFunds.GetInstance().Select().Rows.Find(idBuilding);
            if (bRow != null)
            {
                int idFundType = (int)bRow["id_fund_type"];
                if (idRentType == ViewportHelper.TranslateFundIdToRentId(idFundType))
                    return true;
            }
            return false;
        }

        internal static bool PremiseRentAndFundMatch(int idPremise, int idRentType)
        {
            DataRow bRow = CalcDataModelPremisesCurrentFunds.GetInstance().Select().Rows.Find(idPremise);
            if (bRow != null)
            {
                int idFundType = (int)bRow["id_fund_type"];
                if (idRentType == ViewportHelper.TranslateFundIdToRentId(idFundType))
                    return true;
            }
            return false;
        }

        internal static bool SubPremiseRentAndFundMatch(int idSubPremise, int idRentType)
        {
            DataRow bRow = CalcDataModelSubPremisesCurrentFunds.GetInstance().Select().Rows.Find(idSubPremise);
            if (bRow != null)
            {
                int idFundType = (int)bRow["id_fund_type"];
                if (idRentType == ViewportHelper.TranslateFundIdToRentId(idFundType))
                    return true;
            }
            return false;
        }
    }
}
