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
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_fund"] };
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
                if (fundHistory == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект истории фонда", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_fund_type", fundHistory.IdFundType));
                command.Parameters.Add(DBConnection.CreateParameter<string>("protocol_number", fundHistory.ProtocolNumber));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("protocol_date", fundHistory.ProtocolDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("include_restriction_number", fundHistory.IncludeRestrictionNumber));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("include_restriction_date", fundHistory.IncludeRestrictionDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("include_restriction_description", fundHistory.IncludeRestrictionDescription));
                command.Parameters.Add(DBConnection.CreateParameter<string>("exclude_restriction_number", fundHistory.ExcludeRestrictionNumber));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("exclude_restriction_date", fundHistory.ExcludeRestrictionDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("exclude_restriction_description", fundHistory.ExcludeRestrictionDescription));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", fundHistory.Description));

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
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                if (fundHistory == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект истории фонда", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_fund_type", fundHistory.IdFundType));
                command.Parameters.Add(DBConnection.CreateParameter<string>("protocol_number", fundHistory.ProtocolNumber));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("protocol_date", fundHistory.ProtocolDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("include_restriction_number", fundHistory.IncludeRestrictionNumber));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("include_restriction_date", fundHistory.IncludeRestrictionDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("include_restriction_description", fundHistory.IncludeRestrictionDescription));
                command.Parameters.Add(DBConnection.CreateParameter<string>("exclude_restriction_number", fundHistory.ExcludeRestrictionNumber));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("exclude_restriction_date", fundHistory.ExcludeRestrictionDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("exclude_restriction_description", fundHistory.ExcludeRestrictionDescription));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", fundHistory.Description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_fund", fundHistory.IdFund));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить запись о найме в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
