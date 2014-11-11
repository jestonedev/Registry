using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Globalization;

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

        public static int Insert(FundHistory fundHistory, ParentTypeEnum parentType, int idParent)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand command_assoc = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;

                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_fund_type", fundHistory.id_fund_type));
                command.Parameters.Add(DBConnection.CreateParameter<string>("protocol_number", fundHistory.protocol_number));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("protocol_date", fundHistory.protocol_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("include_restriction_number", fundHistory.include_restriction_number));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("include_restriction_date", fundHistory.include_restriction_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("include_restriction_description", fundHistory.include_restriction_description));
                command.Parameters.Add(DBConnection.CreateParameter<string>("exclude_restriction_number", fundHistory.exclude_restriction_number));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("exclude_restriction_date", fundHistory.exclude_restriction_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("exclude_restriction_description", fundHistory.exclude_restriction_description));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", fundHistory.description));

                if (parentType == ParentTypeEnum.Building)
                    command_assoc.CommandText = "INSERT INTO funds_buildings_assoc (id_building, id_fund) VALUES (?, ?)";
                else
                    if (parentType == ParentTypeEnum.Premises)
                        command_assoc.CommandText = "INSERT INTO funds_premises_assoc (id_premises, id_fund) VALUES (?, ?)";
                    else
                        if (parentType == ParentTypeEnum.SubPremises)
                            command_assoc.CommandText = "INSERT INTO funds_sub_premises_assoc (id_sub_premises, id_fund) VALUES (?, ?)";
                        else
                        {
                            MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            return -1;
                        }
                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        connection.SqlRollbackTransaction();
                        return -1;
                    }
                    command_assoc.Parameters.Add(DBConnection.CreateParameter<int?>("id_object", idParent));
                    command_assoc.Parameters.Add(DBConnection.CreateParameter<int?>("id_fund", 
                        Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture)));
                    connection.SqlModifyQuery(command_assoc);
                    connection.SqlCommitTransaction();
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось добавить запись о найме в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Update(FundHistory fundHistory)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_fund_type", fundHistory.id_fund_type));
                command.Parameters.Add(DBConnection.CreateParameter<string>("protocol_number", fundHistory.protocol_number));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("protocol_date", fundHistory.protocol_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("include_restriction_number", fundHistory.include_restriction_number));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("include_restriction_date", fundHistory.include_restriction_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("include_restriction_description", fundHistory.include_restriction_description));
                command.Parameters.Add(DBConnection.CreateParameter<string>("exclude_restriction_number", fundHistory.exclude_restriction_number));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("exclude_restriction_date", fundHistory.exclude_restriction_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("exclude_restriction_description", fundHistory.exclude_restriction_description));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", fundHistory.description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_fund", fundHistory.id_fund));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить запись о найме в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_fund", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось удалить запись о найме из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }
    }
}
