using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Registry.DataModels.Services
{
    public sealed class PaymentService
    {
        public static IEnumerable<int> GetAccountIdsByPremiseFilter(string whereStatement)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format("SELECT * FROM payments_account_premises_assoc WHERE {0}", whereStatement);
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsBySubPremiseFilter(string whereStatement)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format("SELECT * FROM payments_account_sub_premises_assoc WHERE {0}", whereStatement);
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsByStreet(string idStreet)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = @"SELECT id_account
                                    FROM payments_accounts pa
                                    WHERE pa.id_account IN (
                                      SELECT v.id_account
                                      FROM
                                      (SELECT sp.id_premises, paspa.id_account
                                      FROM payments_account_sub_premises_assoc paspa
                                      INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                                      UNION ALL
                                      SELECT papa.id_premises, papa.id_account
                                      FROM payments_account_premises_assoc papa) v
                                      INNER JOIN premises p ON p.id_premises = v.id_premises
                                      INNER JOIN buildings b ON p.id_building = b.id_building
                                      WHERE b.id_street = ?)";
                command.Parameters.Add(DBConnection.CreateParameter("id_street", idStreet));
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsByHouse(string house)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = @"SELECT id_account
                                    FROM payments_accounts pa
                                    WHERE pa.id_account IN (
                                      SELECT v.id_account
                                      FROM
                                      (SELECT sp.id_premises, paspa.id_account
                                      FROM payments_account_sub_premises_assoc paspa
                                      INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                                      UNION ALL
                                      SELECT papa.id_premises, papa.id_account
                                      FROM payments_account_premises_assoc papa) v
                                      INNER JOIN premises p ON p.id_premises = v.id_premises
                                      INNER JOIN buildings b ON p.id_building = b.id_building
                                      WHERE b.house = ?)";
                command.Parameters.Add(DBConnection.CreateParameter("house", house));
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsByPremiseNumber(string premiseNumber)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = @"SELECT id_account
                                    FROM payments_accounts pa
                                    WHERE pa.id_account IN (
                                      SELECT v.id_account
                                      FROM
                                      (SELECT sp.id_premises, paspa.id_account
                                      FROM payments_account_sub_premises_assoc paspa
                                      INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                                      UNION ALL
                                      SELECT papa.id_premises, papa.id_account
                                      FROM payments_account_premises_assoc papa) v
                                      INNER JOIN premises p ON p.id_premises = v.id_premises
                                      WHERE p.premises_num = ?)";
                command.Parameters.Add(DBConnection.CreateParameter("premises_num", premiseNumber));
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account"));
            }
        }

        public static IEnumerable<int> GetAccountIdsByPaymentFilter(string whereStatement)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format("SELECT id_account FROM payments WHERE {0}", whereStatement);
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_account")).Distinct();
            }
        }

        public static IEnumerable<int> GetPremisesIdsByAccountFilter(string whereStatement)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format(@"SELECT v.id_premises, v.object_type
                    FROM (
                    SELECT papa.id_premises, 1 as object_type, pa.id_account
                    FROM payments_accounts pa
                        INNER JOIN payments_account_premises_assoc papa ON pa.id_account = papa.id_account
                    UNION ALL
                    SELECT sp.id_premises, 2, pa.id_account
                    FROM payments_accounts pa
                        INNER JOIN payments_account_sub_premises_assoc paspa ON pa.id_account = paspa.id_account
                        INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises) v
                    WHERE {0}", whereStatement);
                return connection.SqlSelectTable("ids", command).AsEnumerable().Select(row => row.Field<int>("id_premises"));
            }
        }

        public static DataTable GetBalanceInfoOnDate(IEnumerable<int> idAccounts, int year, int month)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText =
                    string.Format(@"SELECT p.id_account, p.balance_output_tenancy, p.balance_output_dgi, p.balance_output_penalties
                        FROM payments p
                        WHERE p.id_account IN (0{0}) AND MONTH(p.date) = ? AND YEAR(p.date) = ?",
                        idAccounts.Select(v => v.ToString()).Aggregate(
                            (acc, v) => acc + "," + v));
                command.Parameters.Add(DBConnection.CreateParameter("month", month));
                command.Parameters.Add(DBConnection.CreateParameter("year", year));
                return connection.SqlSelectTable("balance_info", command);
            }
        }
    }
}
