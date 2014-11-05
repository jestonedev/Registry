using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using System.Data.Odbc;

namespace Registry.DataModels
{
    public sealed class FundsHistoryDataModel : DataModel
    {
        private static FundsHistoryDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM funds_history WHERE deleted = 0";
        private static string deleteQuery = "UPDATE funds_history SET deleted = 1 WHERE id_fund = ?";
        private static string insertQuery = @"INSERT INTO funds_history
                            (id_fund_type, protocol_number, protocol_date, 
                            include_restriction_number, include_restriction_date, include_restriction_description,
                            exclude_restriction_number, exclude_restriction_date, exclude_restriction_description, description)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE funds_history SET id_fund_type = ?,
                            protocol_number = ?, protocol_date = ?, include_restriction_number = ?, include_restriction_date = ?,
                            include_restriction_description = ?, exclude_restriction_number = ?, exclude_restriction_date = ?, 
                            exclude_restriction_description = ?, description = ? WHERE id_fund = ?";
        private static string tableName = "funds_history";
       
        public bool EditingNewRecord { get; set; }

        private FundsHistoryDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_fund"] };
        }

        public static FundsHistoryDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static FundsHistoryDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new FundsHistoryDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Insert(FundHistory fundHistory, ParentTypeEnum ParentType, int id_parent)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_fund_type", fundHistory.id_fund_type));
            command.Parameters.Add(connection.CreateParameter<string>("protocol_number", fundHistory.protocol_number));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("protocol_date", fundHistory.protocol_date));
            command.Parameters.Add(connection.CreateParameter<string>("include_restriction_number", fundHistory.include_restriction_number));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("include_restriction_date", fundHistory.include_restriction_date));
            command.Parameters.Add(connection.CreateParameter<string>("include_restriction_description", fundHistory.include_restriction_description));
            command.Parameters.Add(connection.CreateParameter<string>("exclude_restriction_number", fundHistory.exclude_restriction_number));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("exclude_restriction_date", fundHistory.exclude_restriction_date));
            command.Parameters.Add(connection.CreateParameter<string>("exclude_restriction_description", fundHistory.exclude_restriction_description));
            command.Parameters.Add(connection.CreateParameter<string>("description", fundHistory.description));

            DbCommand command_assoc = connection.CreateCommand();
            if (ParentType == ParentTypeEnum.Building)
                command_assoc.CommandText = "INSERT INTO funds_buildings_assoc (id_building, id_fund) VALUES (?, ?)";
            else
                if (ParentType == ParentTypeEnum.Premises)
                    command_assoc.CommandText = "INSERT INTO funds_premises_assoc (id_premises, id_fund) VALUES (?, ?)";
                else
                    if (ParentType == ParentTypeEnum.SubPremises)
                        command_assoc.CommandText = "INSERT INTO funds_sub_premises_assoc (id_sub_premises, id_fund) VALUES (?, ?)";
                    else
                    {
                        MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
            try
            {
                connection.SqlBeginTransaction();
                connection.SqlModifyQuery(command);
                DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                if (last_id.Rows.Count == 0)
                {
                    MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connection.SqlRollbackTransaction();
                    return -1;
                }
                command_assoc.Parameters.Add(connection.CreateParameter<int?>("id_object", id_parent));
                command_assoc.Parameters.Add(connection.CreateParameter<int?>("id_fund", Convert.ToInt32(last_id.Rows[0][0])));
                connection.SqlModifyQuery(command_assoc);
                connection.SqlCommitTransaction();
                return Convert.ToInt32(last_id.Rows[0][0]);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось добавить запись о найме в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(FundHistory fundHistory)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_fund_type", fundHistory.id_fund_type));
            command.Parameters.Add(connection.CreateParameter<string>("protocol_number", fundHistory.protocol_number));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("protocol_date", fundHistory.protocol_date));
            command.Parameters.Add(connection.CreateParameter<string>("include_restriction_number", fundHistory.include_restriction_number));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("include_restriction_date", fundHistory.include_restriction_date));
            command.Parameters.Add(connection.CreateParameter<string>("include_restriction_description", fundHistory.include_restriction_description));
            command.Parameters.Add(connection.CreateParameter<string>("exclude_restriction_number", fundHistory.exclude_restriction_number));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("exclude_restriction_date", fundHistory.exclude_restriction_date));
            command.Parameters.Add(connection.CreateParameter<string>("exclude_restriction_description", fundHistory.exclude_restriction_description));
            command.Parameters.Add(connection.CreateParameter<string>("description", fundHistory.description));
            command.Parameters.Add(connection.CreateParameter<int?>("id_fund", fundHistory.id_fund));

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить запись о найме в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_fund", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить запись о найме из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
