using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public class PremisesDataModel: DataModel
    {
        private static PremisesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM premises WHERE deleted = 0";
        private static string deleteQuery = "UPDATE premises SET deleted = 1 WHERE id_premises = ?";
        private static string insertQuery = @"INSERT INTO premises
                            (id_building, premises_num, total_area
                             , living_area, num_beds, id_premises_type, id_premises_kind
                             , floor, for_orphans, accepted_by_exchange
                             , accepted_by_donation, accepted_by_other, cadastral_num, cadastral_cost
                             , balance_cost, description)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE premises SET id_building = ?, premises_num = ?, 
                            total_area = ?, living_area = ?, num_beds = ?, id_premises_type = ?,
                            id_premises_kind = ?, floor = ?, for_orphans = ?, accepted_by_exchange = ?, 
                            accepted_by_donation = ?, accepted_by_other = ?, cadastral_num = ?, cadastral_cost = ?, 
                            balance_cost = ?, description = ? WHERE id_premises = ?";
        private static string tableName = "premises";
        
        public bool EditingNewRecord { get; set; }

        private PremisesDataModel()
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_premise"] };
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
            table.RowDeleted += new DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }

        public static PremisesDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new PremisesDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }

        public int Delete(int id)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            DbParameter id_premises = connection.CreateParameter();
            id_premises.ParameterName = "id_premises";
            id_premises.Value = id;
            command.Parameters.Add(id_premises);
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить помещение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Premise premise)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            DbParameter id_building = connection.CreateParameter();
            id_building.ParameterName = "id_building";
            id_building.Value = premise.id_building;
            command.Parameters.Add(id_building);

            DbParameter premises_num = connection.CreateParameter();
            premises_num.ParameterName = "premises_num";
            premises_num.Value = (premise.premises_num == null) ? DBNull.Value : (Object)premise.premises_num;
            command.Parameters.Add(premises_num);

            DbParameter total_area = connection.CreateParameter();
            total_area.ParameterName = "total_area";
            total_area.Value = premise.total_area;
            command.Parameters.Add(total_area);

            DbParameter living_area = connection.CreateParameter();
            living_area.ParameterName = "living_area";
            living_area.Value = premise.living_area;
            command.Parameters.Add(living_area);

            DbParameter num_beds = connection.CreateParameter();
            num_beds.ParameterName = "num_beds";
            num_beds.Value = premise.num_beds;
            command.Parameters.Add(num_beds);

            DbParameter id_premises_type = connection.CreateParameter();
            id_premises_type.ParameterName = "id_premises_type";
            id_premises_type.Value = premise.id_premises_type;
            command.Parameters.Add(id_premises_type);

            DbParameter id_premises_kind = connection.CreateParameter();
            id_premises_kind.ParameterName = "id_premises_kind";
            id_premises_kind.Value = premise.id_premises_kind;
            command.Parameters.Add(id_premises_kind);

            DbParameter floor = connection.CreateParameter();
            floor.ParameterName = "floor";
            floor.Value = premise.floor;
            command.Parameters.Add(floor);

            DbParameter for_orphans = connection.CreateParameter();
            for_orphans.ParameterName = "for_orphans";
            for_orphans.Value = premise.for_orphans;
            command.Parameters.Add(for_orphans);

            DbParameter accepted_by_exchange = connection.CreateParameter();
            accepted_by_exchange.ParameterName = "accepted_by_exchange";
            accepted_by_exchange.Value = premise.accepted_by_exchange;
            command.Parameters.Add(accepted_by_exchange);

            DbParameter accepted_by_donation = connection.CreateParameter();
            accepted_by_donation.ParameterName = "accepted_by_donation";
            accepted_by_donation.Value = premise.accepted_by_donation;
            command.Parameters.Add(accepted_by_donation);

            DbParameter accepted_by_other = connection.CreateParameter();
            accepted_by_other.ParameterName = "accepted_by_other";
            accepted_by_other.Value = premise.accepted_by_other;
            command.Parameters.Add(accepted_by_other);

            DbParameter cadastral_num = connection.CreateParameter();
            cadastral_num.ParameterName = "cadastral_num";
            cadastral_num.Value = (premise.cadastral_num == null) ? DBNull.Value : (Object)premise.cadastral_num;
            command.Parameters.Add(cadastral_num);

            DbParameter cadastral_cost = connection.CreateParameter();
            cadastral_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)cadastral_cost).Scale = 2;
            ((IDbDataParameter)cadastral_cost).Precision = 12;
            cadastral_cost.ParameterName = "cadastral_cost";
            cadastral_cost.Value = premise.cadastral_cost;
            command.Parameters.Add(cadastral_cost);

            DbParameter balance_cost = connection.CreateParameter();
            balance_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)balance_cost).Scale = 2;
            ((IDbDataParameter)balance_cost).Precision = 12;
            balance_cost.ParameterName = "balance_cost";
            balance_cost.Value = premise.balance_cost;
            command.Parameters.Add(balance_cost);

            DbParameter description = connection.CreateParameter();
            description.ParameterName = "description";
            description.Value = (premise.description == null) ? DBNull.Value : (Object)premise.description;
            command.Parameters.Add(description);

            DbParameter id_premise = connection.CreateParameter();
            id_premise.ParameterName = "id_premises";
            id_premise.Value = premise.id_premises;
            command.Parameters.Add(id_premise);

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(String.Format("Не удалось изменить данные о помещении. Подробная ошибка: {0}", e.Message), "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(Premise premise)
        {
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            DbParameter id_building = connection.CreateParameter();
            id_building.ParameterName = "id_building";
            id_building.Value = premise.id_building;
            command.Parameters.Add(id_building);

            DbParameter premises_num = connection.CreateParameter();
            premises_num.ParameterName = "premises_num";
            premises_num.Value = (premise.premises_num == null) ? DBNull.Value : (Object)premise.premises_num;
            command.Parameters.Add(premises_num);

            DbParameter total_area = connection.CreateParameter();
            total_area.ParameterName = "total_area";
            total_area.Value = premise.total_area;
            command.Parameters.Add(total_area);

            DbParameter living_area = connection.CreateParameter();
            living_area.ParameterName = "living_area";
            living_area.Value = premise.living_area;
            command.Parameters.Add(living_area);

            DbParameter num_beds = connection.CreateParameter();
            num_beds.ParameterName = "num_beds";
            num_beds.Value = premise.num_beds;
            command.Parameters.Add(num_beds);

            DbParameter id_premises_type = connection.CreateParameter();
            id_premises_type.ParameterName = "id_premises_type";
            id_premises_type.Value = premise.id_premises_type;
            command.Parameters.Add(id_premises_type);

            DbParameter id_premises_kind = connection.CreateParameter();
            id_premises_kind.ParameterName = "id_premises_kind";
            id_premises_kind.Value = premise.id_premises_kind;
            command.Parameters.Add(id_premises_kind);

            DbParameter floor = connection.CreateParameter();
            floor.ParameterName = "floor";
            floor.Value = premise.floor;
            command.Parameters.Add(floor);

            DbParameter for_orphans = connection.CreateParameter();
            for_orphans.ParameterName = "for_orphans";
            for_orphans.Value = premise.for_orphans;
            command.Parameters.Add(for_orphans);

            DbParameter accepted_by_exchange = connection.CreateParameter();
            accepted_by_exchange.ParameterName = "accepted_by_exchange";
            accepted_by_exchange.Value = premise.accepted_by_exchange;
            command.Parameters.Add(accepted_by_exchange);

            DbParameter accepted_by_donation = connection.CreateParameter();
            accepted_by_donation.ParameterName = "accepted_by_donation";
            accepted_by_donation.Value = premise.accepted_by_donation;
            command.Parameters.Add(accepted_by_donation);

            DbParameter accepted_by_other = connection.CreateParameter();
            accepted_by_other.ParameterName = "accepted_by_other";
            accepted_by_other.Value = premise.accepted_by_other;
            command.Parameters.Add(accepted_by_other);

            DbParameter cadastral_num = connection.CreateParameter();
            cadastral_num.ParameterName = "cadastral_num";
            cadastral_num.Value = (premise.cadastral_num == null) ? DBNull.Value : (Object)premise.cadastral_num;
            command.Parameters.Add(cadastral_num);

            DbParameter cadastral_cost = connection.CreateParameter();
            cadastral_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)cadastral_cost).Scale = 2;
            ((IDbDataParameter)cadastral_cost).Precision = 12;
            cadastral_cost.ParameterName = "cadastral_cost";
            cadastral_cost.Value = premise.cadastral_cost;
            command.Parameters.Add(cadastral_cost);

            DbParameter balance_cost = connection.CreateParameter();
            balance_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)balance_cost).Scale = 2;
            ((IDbDataParameter)balance_cost).Precision = 12;
            balance_cost.ParameterName = "balance_cost";
            balance_cost.Value = premise.balance_cost;
            command.Parameters.Add(balance_cost);

            DbParameter description = connection.CreateParameter();
            description.ParameterName = "description";
            description.Value = (premise.description == null) ? DBNull.Value : (Object)premise.description;
            command.Parameters.Add(description);

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
            catch (InvalidOperationException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось добавить помещение в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
