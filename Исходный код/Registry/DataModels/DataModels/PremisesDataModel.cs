using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using System.Data.Odbc;
using System.Globalization;

namespace Registry.DataModels
{
    public sealed class PremisesDataModel : DataModel
    {
        private static PremisesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM premises WHERE deleted = 0";
        private static string deleteQuery = "UPDATE premises SET deleted = 1 WHERE id_premises = ?";
        private static string insertQuery = @"INSERT INTO premises
                            (id_building, id_state, id_premises_kind, id_premises_type, premises_num, floor
                             , num_rooms, num_beds, total_area, living_area, height
                             , cadastral_num, cadastral_cost
                             , balance_cost, description)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE premises SET id_building = ?, id_state = ?, id_premises_kind = ?, 
                            id_premises_type = ?,premises_num = ?, floor = ?, num_rooms = ?, num_beds = ?, 
                            total_area = ?, living_area = ?, height = ?, 
                            cadastral_num = ?, cadastral_cost = ?, 
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
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_premise"] };
            Table.Columns["id_state"].DefaultValue = 1;
            Table.Columns["living_area"].DefaultValue = 0;
            Table.Columns["total_area"].DefaultValue = 0;
            Table.Columns["height"].DefaultValue = 0;
            Table.Columns["num_rooms"].DefaultValue = 0;
            Table.Columns["num_beds"].DefaultValue = 0;
            Table.Columns["id_premises_type"].DefaultValue = 1;
            Table.Columns["id_premises_kind"].DefaultValue = 1;
            Table.Columns["floor"].DefaultValue = 0;
            Table.Columns["cadastral_cost"].DefaultValue = 0;
            Table.Columns["balance_cost"].DefaultValue = 0;
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

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось удалить помещение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(Premise premise)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (premise == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект помещения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_building", premise.IdBuilding));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state", premise.IdState));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises_kind", premise.IdPremisesKind));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises_type", premise.IdPremisesType));
                command.Parameters.Add(DBConnection.CreateParameter<string>("premises_num", premise.PremisesNum));
                command.Parameters.Add(DBConnection.CreateParameter<short?>("floor", premise.Floor));
                command.Parameters.Add(DBConnection.CreateParameter<short?>("num_rooms", premise.NumRooms));
                command.Parameters.Add(DBConnection.CreateParameter<short?>("num_beds", premise.NumBeds));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("total_area", premise.TotalArea));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("living_area", premise.LivingArea));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("height", premise.Height));
                command.Parameters.Add(DBConnection.CreateParameter<string>("cadastral_num", premise.CadastralNum));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("cadastral_cost", premise.CadastralCost));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("balance_cost", premise.BalanceCost));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", premise.Description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises", premise.IdPremises));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить данные о помещении. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(Premise premise)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (premise == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект помещения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_building", premise.IdBuilding));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state", premise.IdState));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises_kind", premise.IdPremisesKind));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises_type", premise.IdPremisesType));
                command.Parameters.Add(DBConnection.CreateParameter<string>("premises_num", premise.PremisesNum));
                command.Parameters.Add(DBConnection.CreateParameter<short?>("floor", premise.Floor));
                command.Parameters.Add(DBConnection.CreateParameter<short?>("num_rooms", premise.NumRooms));
                command.Parameters.Add(DBConnection.CreateParameter<short?>("num_beds", premise.NumBeds));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("total_area", premise.TotalArea));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("living_area", premise.LivingArea));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("height", premise.Height));
                command.Parameters.Add(DBConnection.CreateParameter<string>("cadastral_num", premise.CadastralNum));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("cadastral_cost", premise.CadastralCost));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("balance_cost", premise.BalanceCost));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", premise.Description));

                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                    connection.SqlCommitTransaction();
                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return -1;
                    }
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось добавить помещение в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
