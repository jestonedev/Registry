using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    public class PaymentsDataModel: DataModel
    {
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
                                          INNER JOIN payments pm ON pa.id_account = pm.id_account
                                          WHERE pa.id_account = {0}
                                          ORDER BY pm.date";
        private const string TableName = "payments";

        public override DataTable Select()
        {
            return null;
        }

        private PaymentsDataModel()
        {        
        }

        public static PaymentsDataModel GetInstance()
        {
            return new PaymentsDataModel();
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
