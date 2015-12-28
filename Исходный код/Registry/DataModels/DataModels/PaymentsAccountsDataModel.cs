using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    public class PaymentsAccountsDataModel: DataModel
    {
        private static PaymentsAccountsDataModel _dataModel;
        private const string SelectQuery = @"SELECT pa.id_account,
                  pa.crn,
                  pa.raw_address,
                  CONCAT(vks.street_name, ', д. ', b.house, ', кв. ', v.premises) AS parsed_address,
                  pa.account,
                  pa.tenant,
                  pm.date,
                  pm.total_area,
                  pm.living_area,
                  pm.prescribed,
                  pm.balance_input,
                  pm.balance_tenancy,
                  pm.balance_dgi,
                  pm.charging_tenancy,
                  pm.charging_dgi,
                  pm.charging_total,
                  pm.recalc_tenancy,
                  pm.recalc_dgi,
                  pm.payment_tenancy,
                  pm.payment_dgi,
                  pm.transfer_balance,
                  pm.balance_output_total,
                  pm.balance_output_tenancy,
                  pm.balance_output_dgi
                FROM (
                SELECT v.id_account, p.id_building, GROUP_CONCAT(
                    CONCAT(p.premises_num,
                    IF(v.sub_premises IS NOT NULL,CONCAT(' ком. ',v.sub_premises),'')) SEPARATOR ', ') AS premises
                FROM (
                SELECT paspa.id_account, sp.id_premises, GROUP_CONCAT(sp.sub_premises_num SEPARATOR ', ') AS sub_premises
                FROM payments_account_sub_premises_assoc paspa
                  INNER JOIN sub_premises sp ON paspa.id_sub_premises = sp.id_sub_premises
                GROUP BY paspa.id_account, sp.id_premises
                UNION ALL
                SELECT papa.id_account, papa.id_premises, NULL
                FROM payments_account_premises_assoc papa) v 
                  INNER JOIN premises p ON p.id_premises = v.id_premises
                GROUP BY v.id_account, p.id_building) v
                  INNER JOIN buildings b ON b.id_building = v.id_building
                  INNER JOIN v_kladr_streets vks ON b.id_street = vks.id_street
                  RIGHT JOIN payments_accounts pa ON v.id_account = pa.id_account
                  LEFT JOIN (SELECT p.*
                FROM payments p
                  INNER JOIN (
                SELECT p.id_account, MAX(p.date) AS date
                FROM payments p
                GROUP BY p.id_account) v ON p.id_account = v.id_account AND p.date = v.date
                GROUP BY p.id_account) pm ON pa.id_account = pm.id_account";
        private const string TableName = "payments_accounts";

        private PaymentsAccountsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static PaymentsAccountsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new PaymentsAccountsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_account", "claims", "id_account");
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["id_account"] };
        }

        public static IEnumerable<int> GetAccountIdsByPremiseFilter(string whereStatement)
        {
            using(var connection = new DBConnection())
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
                command.CommandText =@"SELECT id_account
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
    }
}
