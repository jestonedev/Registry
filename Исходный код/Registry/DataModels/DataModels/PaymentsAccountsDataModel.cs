using System;

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
                  pm.tenant,
                  pm.date,
                  vplcd.charging_date AS charging_date,
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
                GROUP BY p.id_account) pm ON pa.id_account = pm.id_account
                  LEFT JOIN v_payments_last_charging_date vplcd ON pa.id_account = vplcd.id_account";
        private const string TableName = "payments_accounts";

        private PaymentsAccountsDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static PaymentsAccountsDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new PaymentsAccountsDataModel(afterLoadHandler));
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_account", "claims", "id_account");
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["id_account"] };
        }
    }
}
