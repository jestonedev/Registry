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
            table.RowDeleted += new System.Data.DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, System.Data.DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
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

        public int Insert(Entities.FundHistory fundHistory, ParentTypeEnum ParentType, int id_parent)
        {
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            DbParameter id_fund_type = connection.CreateParameter();
            id_fund_type.ParameterName = "id_ownership_right_type";
            id_fund_type.Value = (fundHistory.id_fund_type == null) ? DBNull.Value : (Object)fundHistory.id_fund_type;
            command.Parameters.Add(id_fund_type);

            DbParameter protocol_number = connection.CreateParameter();
            protocol_number.ParameterName = "protocol_number";
            protocol_number.Value = (fundHistory.protocol_number == null) ? DBNull.Value : (Object)fundHistory.protocol_number;
            command.Parameters.Add(protocol_number);

            DbParameter protocol_date = connection.CreateParameter();
            protocol_date.ParameterName = "protocol_date";
            protocol_date.Value = (fundHistory.protocol_date == null) ? DBNull.Value : (Object)fundHistory.protocol_date;
            command.Parameters.Add(protocol_date);

            DbParameter include_restriction_number = connection.CreateParameter();
            include_restriction_number.ParameterName = "include_restriction_number";
            include_restriction_number.Value = (fundHistory.include_restriction_number == null) ? DBNull.Value : (Object)fundHistory.include_restriction_number;
            command.Parameters.Add(include_restriction_number);

            DbParameter include_restriction_date = connection.CreateParameter();
            include_restriction_date.ParameterName = "include_restriction_date";
            include_restriction_date.Value = (fundHistory.include_restriction_date == null) ? DBNull.Value : (Object)fundHistory.include_restriction_date;
            command.Parameters.Add(include_restriction_date);

            DbParameter include_restriction_description = connection.CreateParameter();
            include_restriction_description.ParameterName = "include_restriction_description";
            include_restriction_description.Value = (fundHistory.include_restriction_description == null) ? DBNull.Value : 
                (Object)fundHistory.include_restriction_description;
            command.Parameters.Add(include_restriction_description);

            DbParameter exclude_restriction_number = connection.CreateParameter();
            exclude_restriction_number.ParameterName = "exclude_restriction_number";
            exclude_restriction_number.Value = (fundHistory.exclude_restriction_number == null) ? DBNull.Value : (Object)fundHistory.exclude_restriction_number;
            command.Parameters.Add(exclude_restriction_number);

            DbParameter exclude_restriction_date = connection.CreateParameter();
            exclude_restriction_date.ParameterName = "exclude_restriction_date";
            exclude_restriction_date.Value = (fundHistory.exclude_restriction_date == null) ? DBNull.Value : (Object)fundHistory.exclude_restriction_date;
            command.Parameters.Add(exclude_restriction_date);

            DbParameter exclude_restriction_description = connection.CreateParameter();
            exclude_restriction_description.ParameterName = "exclude_restriction_description";
            exclude_restriction_description.Value = (fundHistory.exclude_restriction_description == null) ? DBNull.Value : 
                (Object)fundHistory.exclude_restriction_description;
            command.Parameters.Add(exclude_restriction_description);

            DbParameter description = connection.CreateParameter();
            description.ParameterName = "description";
            description.Value = (fundHistory.description == null) ? DBNull.Value : (Object)fundHistory.description;
            command.Parameters.Add(description);

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

                DbParameter id_object = connection.CreateParameter();
                id_object.ParameterName = "id_object";
                id_object.Value = id_parent;
                command_assoc.Parameters.Add(id_object);

                DbParameter id_fund = connection.CreateParameter();
                id_fund.ParameterName = "id_fund";
                id_fund.Value = last_id.Rows[0][0];
                command_assoc.Parameters.Add(id_fund);

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

        public int Update(Entities.FundHistory fundHistory)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            DbParameter id_fund_type = connection.CreateParameter();
            id_fund_type.ParameterName = "id_ownership_right_type";
            id_fund_type.Value = (fundHistory.id_fund_type == null) ? DBNull.Value : (Object)fundHistory.id_fund_type;
            command.Parameters.Add(id_fund_type);

            DbParameter protocol_number = connection.CreateParameter();
            protocol_number.ParameterName = "protocol_number";
            protocol_number.Value = (fundHistory.protocol_number == null) ? DBNull.Value : (Object)fundHistory.protocol_number;
            command.Parameters.Add(protocol_number);

            DbParameter protocol_date = connection.CreateParameter();
            protocol_date.ParameterName = "protocol_date";
            protocol_date.Value = (fundHistory.protocol_date == null) ? DBNull.Value : (Object)fundHistory.protocol_date;
            command.Parameters.Add(protocol_date);

            DbParameter include_restriction_number = connection.CreateParameter();
            include_restriction_number.ParameterName = "include_restriction_number";
            include_restriction_number.Value = (fundHistory.include_restriction_number == null) ? DBNull.Value : (Object)fundHistory.include_restriction_number;
            command.Parameters.Add(include_restriction_number);

            DbParameter include_restriction_date = connection.CreateParameter();
            include_restriction_date.ParameterName = "include_restriction_date";
            include_restriction_date.Value = (fundHistory.include_restriction_date == null) ? DBNull.Value : (Object)fundHistory.include_restriction_date;
            command.Parameters.Add(include_restriction_date);

            DbParameter include_restriction_description = connection.CreateParameter();
            include_restriction_description.ParameterName = "include_restriction_description";
            include_restriction_description.Value = (fundHistory.include_restriction_description == null) ? DBNull.Value :
                (Object)fundHistory.include_restriction_description;
            command.Parameters.Add(include_restriction_description);

            DbParameter exclude_restriction_number = connection.CreateParameter();
            exclude_restriction_number.ParameterName = "exclude_restriction_number";
            exclude_restriction_number.Value = (fundHistory.exclude_restriction_number == null) ? DBNull.Value : (Object)fundHistory.exclude_restriction_number;
            command.Parameters.Add(exclude_restriction_number);

            DbParameter exclude_restriction_date = connection.CreateParameter();
            exclude_restriction_date.ParameterName = "exclude_restriction_date";
            exclude_restriction_date.Value = (fundHistory.exclude_restriction_date == null) ? DBNull.Value : (Object)fundHistory.exclude_restriction_date;
            command.Parameters.Add(exclude_restriction_date);

            DbParameter exclude_restriction_description = connection.CreateParameter();
            exclude_restriction_description.ParameterName = "exclude_restriction_description";
            exclude_restriction_description.Value = (fundHistory.exclude_restriction_description == null) ? DBNull.Value :
                (Object)fundHistory.exclude_restriction_description;
            command.Parameters.Add(exclude_restriction_description);

            DbParameter description = connection.CreateParameter();
            description.ParameterName = "description";
            description.Value = (fundHistory.description == null) ? DBNull.Value : (Object)fundHistory.description;
            command.Parameters.Add(description);

            DbParameter id_fund = connection.CreateParameter();
            id_fund.ParameterName = "id_fund";
            id_fund.Value = (fundHistory.id_fund == null) ? DBNull.Value : (Object)fundHistory.id_fund;
            command.Parameters.Add(id_fund);

            DbCommand command_assoc = connection.CreateCommand();

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
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            DbParameter id_fund = connection.CreateParameter();
            id_fund.ParameterName = "id_fund";
            id_fund.Value = id;
            command.Parameters.Add(id_fund);
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
