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
    public sealed class SubPremisesDataModel : DataModel
    {
        private static SubPremisesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM sub_premises WHERE deleted = 0";
        private static string deleteQuery = "UPDATE sub_premises SET deleted = 1 WHERE id_sub_premises = ?";
        private static string insertQuery = @"INSERT INTO sub_premises
                            (id_premises, id_state, sub_premises_num, total_area, description)
                            VALUES (?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE sub_premises SET id_premises = ?, id_state = ?, sub_premises_num = ?, 
                            total_area = ?, description = ? WHERE id_sub_premises = ?";

        private static string tableName = "sub_premises";

        private SubPremisesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_sub_premises"] };
            Table.Columns["total_area"].DefaultValue = 0;
            Table.Columns["deleted"].DefaultValue = 0;
            Table.Columns["id_state"].DefaultValue = 1;
        }

        public static SubPremisesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SubPremisesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new SubPremisesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_sub_premises", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось удалить комнату из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SubPremise subPremise)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (subPremise == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект комнаты", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises", subPremise.IdPremises));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state", subPremise.IdState));
                command.Parameters.Add(DBConnection.CreateParameter<string>("sub_premises_num", subPremise.SubPremisesNum));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("total_area", subPremise.TotalArea));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", subPremise.Description));
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
                        "Не удалось добавить комнату в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SubPremise subPremise)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (subPremise == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект комнаты", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises", subPremise.IdPremises));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state", subPremise.IdState));
                command.Parameters.Add(DBConnection.CreateParameter<string>("sub_premises_num", subPremise.SubPremisesNum));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("total_area", subPremise.TotalArea));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", subPremise.Description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_sub_premises", subPremise.IdSubPremises));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить комнату в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
