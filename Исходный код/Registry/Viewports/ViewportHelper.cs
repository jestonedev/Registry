using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using Registry.DataModels.CalcDataModels;
using Registry.Entities;

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
            if (string.IsNullOrEmpty(control.Text.Trim()))
                return null;
            else
                return control.Text.Trim();
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

        internal static bool BuildingFundAndRentMatch(int idBuilding, int idRentType)
        {
            var bRow = CalcDataModel.GetInstance<CalcDataModelBuildingsCurrentFunds>().Select().Rows.Find(idBuilding);
            if (bRow == null) return false;
            var idFundType = (int)bRow["id_fund_type"];
            if (idRentType == TranslateFundIdToRentId(idFundType))
                return true;
            return false;
        }

        internal static bool PremiseFundAndRentMatch(int idPremise, int idRentType)
        {
            var bRow = CalcDataModel.GetInstance<CalcDataModelPremisesCurrentFunds>().Select().Rows.Find(idPremise);
            if (bRow == null) return false;
            var idFundType = (int)bRow["id_fund_type"];
            if (idRentType == TranslateFundIdToRentId(idFundType))
                return true;
            return false;
        }

        internal static bool SubPremiseFundAndRentMatch(int idSubPremise, int idRentType)
        {
            var bRow = CalcDataModel.GetInstance<CalcDataModelSubPremisesCurrentFunds>().Select().Rows.Find(idSubPremise);
            if (bRow == null) return false;
            var idFundType = (int)bRow["id_fund_type"];
            if (idRentType == TranslateFundIdToRentId(idFundType))
                return true;
            return false;
        }

        internal static void SelectAllText(object sender)
        {
            if (sender is NumericUpDown)
            {
                var control = ((NumericUpDown) sender);
                control.Select(0, control.Text.Length);
            }
            else if (sender is TextBox)
            {
                var control = ((TextBox) sender);
                control.SelectAll();
            } else if (sender is ComboBox)
            {
                var control = ((ComboBox)sender);
                control.SelectAll();
            }
        }

        internal static TenancyPerson PersonFromRow(DataRow row)
        {
            var tenancyPerson = new TenancyPerson
            {
                IdPerson = ValueOrNull<int>(row, "id_person"),
                IdProcess = ValueOrNull<int>(row, "id_process"),
                IdKinship = ValueOrNull<int>(row, "id_kinship"),
                IdDocumentType = ValueOrNull<int>(row, "id_document_type"),
                IdDocumentIssuedBy = ValueOrNull<int>(row, "id_document_issued_by"),
                Surname = ValueOrNull(row, "surname"),
                Name = ValueOrNull(row, "name"),
                Patronymic = ValueOrNull(row, "patronymic"),
                DateOfBirth = ValueOrNull<DateTime>(row, "date_of_birth"),
                DateOfDocumentIssue = ValueOrNull<DateTime>(row, "date_of_document_issue"),
                DocumentNum = ValueOrNull(row, "document_num"),
                DocumentSeria = ValueOrNull(row, "document_seria"),
                RegistrationIdStreet = ValueOrNull(row, "registration_id_street"),
                RegistrationHouse = ValueOrNull(row, "registration_house"),
                RegistrationFlat = ValueOrNull(row, "registration_flat"),
                RegistrationRoom = ValueOrNull(row, "registration_room"),
                ResidenceIdStreet = ValueOrNull(row, "residence_id_street"),
                ResidenceHouse = ValueOrNull(row, "residence_house"),
                ResidenceFlat = ValueOrNull(row, "residence_flat"),
                ResidenceRoom = ValueOrNull(row, "residence_room"),
                PersonalAccount = ValueOrNull(row, "personal_account"),
                IncludeDate = ValueOrNull<DateTime>(row, "include_date"),
                ExcludeDate = ValueOrNull<DateTime>(row, "exclude_date")
            };
            return tenancyPerson;
        }
    }
}
