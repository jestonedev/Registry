using System.Data;

namespace Registry.DataModels.DataModels
{
    public class PaymentsPremiseHistoryDataModel: DataModel
    {
        private const string SelectQuery = @"SELECT DISTINCT pm.id_account,
                   pm.crn,
                   pm.raw_address,
                   pm.db_address AS parsed_address,
                   pm.account,
                   pm.tenant,
                   pm.date,
                   pm.total_area,
                   pm.living_area,
                   pm.prescribed,
                   pm.balance_input,
                   pm.balance_tenancy,
                   pm.balance_dgi,
                   pm.balance_input_penalties,
                   pm.charging_tenancy,
                   pm.charging_dgi,
                   pm.charging_total,
                   pm.charging_penalties,
                   pm.recalc_tenancy,
                   pm.recalc_dgi,
                   pm.recalc_penalties,
                   pm.payment_tenancy,
                   pm.payment_dgi,
                   pm.payment_penalties,
                   pm.transfer_balance,
                   pm.balance_output_total,
                   pm.balance_output_tenancy,
                   pm.balance_output_dgi,
                   pm.balance_output_penalties
            FROM (
            SELECT vpa.db_address, pa.crn, pa.raw_address, pa.account, pm.*
             FROM v_payments_address vpa
               RIGHT JOIN payments_accounts pa ON vpa.id_account = pa.id_account
               INNER JOIN payments pm ON pa.id_account = pm.id_account
               INNER JOIN (SELECT pa1.raw_address FROM payments_accounts pa1 WHERE pa1.id_account = {0}) pa1 ON
                  pa.raw_address = pa1.raw_address
            UNION ALL
            SELECT vpa.db_address, pa.crn, pa.raw_address, pa.account, pm.*
             FROM v_payments_address vpa
               RIGHT JOIN payments_accounts pa ON vpa.id_account = pa.id_account
               INNER JOIN payments pm ON pa.id_account = pm.id_account
               INNER JOIN (SELECT vpa1.db_address FROM v_payments_address vpa1 WHERE vpa1.id_account = {0}) vpa1 ON
                  vpa.db_address =vpa1.db_address) pm
            ORDER BY date";
        private const string TableName = "payments";

        public override DataTable Select()
        {
            return null;
        }

        private PaymentsPremiseHistoryDataModel()
        {        
        }

        public static PaymentsPremiseHistoryDataModel GetInstance()
        {
            return new PaymentsPremiseHistoryDataModel();
        }

        public DataTable Select(int idAccount)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format(SelectQuery, idAccount);
                return connection.SqlSelectTable(TableName, command);
            }   
        }
    }
}
