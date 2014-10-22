using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using System.Data.Odbc;

namespace Registry.DataModels
{
    public sealed class PremisesDataModel : DataModel
    {
        private static PremisesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM premises WHERE deleted = 0";
        private static string deleteQuery = "UPDATE premises SET deleted = 1 WHERE id_premises = ?";
        private static string insertQuery = @"INSERT INTO premises
                            (id_building, id_state, premises_num, total_area
                             , living_area, num_beds, id_premises_type, id_premises_kind
                             , floor, for_orphans, accepted_by_exchange
                             , accepted_by_donation, accepted_by_other, cadastral_num, cadastral_cost
                             , balance_cost, description)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE premises SET id_building = ?, id_state = ?, premises_num = ?, 
                            total_area = ?, living_area = ?, num_beds = ?, id_premises_type = ?,
                            id_premises_kind = ?, floor = ?, for_orphans = ?, accepted_by_exchange = ?, 
                            accepted_by_donation = ?, accepted_by_other = ?, cadastral_num = ?, cadastral_cost = ?, 
                            balance_cost = ?, description = ? WHERE id_premises = ?";
        private static string tableName = "premises";
        
        public bool EditingNewRecord { get; set; }

        private PremisesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;            
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_premise"] };
            table.Columns["id_state"].DefaultValue = 1;
            table.Columns["living_area"].DefaultValue = 0;
            table.Columns["total_area"].DefaultValue = 0;
            table.Columns["num_beds"].DefaultValue = 0;
            table.Columns["id_premises_type"].DefaultValue = 1;
            table.Columns["id_premises_kind"].DefaultValue = 1;
            table.Columns["floor"].DefaultValue = 0;
            table.Columns["for_orphans"].DefaultValue = false;
            table.Columns["accepted_by_exchange"].DefaultValue = false;
            table.Columns["accepted_by_donation"].DefaultValue = false;
            table.Columns["accepted_by_other"].DefaultValue = false;
            table.Columns["cadastral_cost"].DefaultValue = 0;
            table.Columns["balance_cost"].DefaultValue = 0;
        }

        public static PremisesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static PremisesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new PremisesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_premises", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить помещение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Premise premise)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_building", premise.id_building));
            command.Parameters.Add(connection.CreateParameter<int?>("id_state", premise.id_state));
            command.Parameters.Add(connection.CreateParameter<string>("premises_num", premise.premises_num));
            command.Parameters.Add(connection.CreateParameter<double?>("total_area", premise.total_area));
            command.Parameters.Add(connection.CreateParameter<double?>("living_area", premise.living_area));
            command.Parameters.Add(connection.CreateParameter<short?>("num_beds", premise.num_beds));
            command.Parameters.Add(connection.CreateParameter<int?>("id_premises_type", premise.id_premises_type));
            command.Parameters.Add(connection.CreateParameter<int?>("id_premises_kind", premise.id_premises_kind));
            command.Parameters.Add(connection.CreateParameter<short?>("floor", premise.floor));
            command.Parameters.Add(connection.CreateParameter<bool?>("for_orphans", premise.for_orphans));
            command.Parameters.Add(connection.CreateParameter<bool?>("accepted_by_exchange", premise.accepted_by_exchange));
            command.Parameters.Add(connection.CreateParameter<bool?>("accepted_by_donation", premise.accepted_by_donation));
            command.Parameters.Add(connection.CreateParameter<bool?>("accepted_by_other", premise.accepted_by_other));
            command.Parameters.Add(connection.CreateParameter<string>("cadastral_num", premise.cadastral_num));

            DbParameter cadastral_cost = connection.CreateParameter<decimal?>("cadastral_cost", premise.cadastral_cost);
            cadastral_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)cadastral_cost).Scale = 2;
            ((IDbDataParameter)cadastral_cost).Precision = 12;
            command.Parameters.Add(cadastral_cost);

            DbParameter balance_cost = connection.CreateParameter<decimal?>("balance_cost", premise.balance_cost);
            balance_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)balance_cost).Scale = 2;
            ((IDbDataParameter)balance_cost).Precision = 12;
            command.Parameters.Add(balance_cost);

            command.Parameters.Add(connection.CreateParameter<string>("description", premise.description));
            command.Parameters.Add(connection.CreateParameter<int?>("id_premises", premise.id_premises));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось изменить данные о помещении. Подробная ошибка: {0}", e.Message), "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(Premise premise)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_building", premise.id_building));
            command.Parameters.Add(connection.CreateParameter<int?>("id_state", premise.id_state));
            command.Parameters.Add(connection.CreateParameter<string>("premises_num", premise.premises_num));
            command.Parameters.Add(connection.CreateParameter<double?>("total_area", premise.total_area));
            command.Parameters.Add(connection.CreateParameter<double?>("living_area", premise.living_area));
            command.Parameters.Add(connection.CreateParameter<short?>("num_beds", premise.num_beds));
            command.Parameters.Add(connection.CreateParameter<int?>("id_premises_type", premise.id_premises_type));
            command.Parameters.Add(connection.CreateParameter<int?>("id_premises_kind", premise.id_premises_kind));
            command.Parameters.Add(connection.CreateParameter<short?>("floor", premise.floor));
            command.Parameters.Add(connection.CreateParameter<bool?>("for_orphans", premise.for_orphans));
            command.Parameters.Add(connection.CreateParameter<bool?>("accepted_by_exchange", premise.accepted_by_exchange));
            command.Parameters.Add(connection.CreateParameter<bool?>("accepted_by_donation", premise.accepted_by_donation));
            command.Parameters.Add(connection.CreateParameter<bool?>("accepted_by_other", premise.accepted_by_other));
            command.Parameters.Add(connection.CreateParameter<string>("cadastral_num", premise.cadastral_num));

            DbParameter cadastral_cost = connection.CreateParameter<decimal?>("cadastral_cost", premise.cadastral_cost);
            cadastral_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)cadastral_cost).Scale = 2;
            ((IDbDataParameter)cadastral_cost).Precision = 12;
            command.Parameters.Add(cadastral_cost);

            DbParameter balance_cost = connection.CreateParameter<decimal?>("balance_cost", premise.balance_cost);
            balance_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)balance_cost).Scale = 2;
            ((IDbDataParameter)balance_cost).Precision = 12;
            command.Parameters.Add(balance_cost);

            command.Parameters.Add(connection.CreateParameter<string>("description", premise.description));

            try
            {
                connection.SqlBeginTransaction();
                connection.SqlModifyQuery(command);
                DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                connection.SqlCommitTransaction();
                if (last_id.Rows.Count == 0)
                {
                    MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
                return Convert.ToInt32(last_id.Rows[0][0]);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось добавить помещение в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
