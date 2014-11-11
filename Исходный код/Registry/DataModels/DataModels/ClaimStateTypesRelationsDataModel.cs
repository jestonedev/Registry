using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using System.Data.Odbc;
using System.Data.Common;
using System.Globalization;

namespace Registry.DataModels
{
    public sealed class ClaimStateTypesRelationsDataModel: DataModel
    {
        private static ClaimStateTypesRelationsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM claim_state_types_relations";
        private static string deleteQuery = "DELETE FROM claim_state_types_relations WHERE id_relation = ?";
        private static string insertQuery = @"INSERT INTO claim_state_types_relations
                            (id_state_from, id_state_to)
                            VALUES (?, ?)";

        private static string tableName = "claim_state_types_relations";

        private ClaimStateTypesRelationsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {  
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_relation"] };
        }

        public static ClaimStateTypesRelationsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ClaimStateTypesRelationsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ClaimStateTypesRelationsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Insert(ClaimStateTypeRelation claimStateTypeRelation)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state_from", claimStateTypeRelation.id_state_from));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state_to", claimStateTypeRelation.id_state_to));
                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                    connection.SqlCommitTransaction();
                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return -1;
                    }
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось добавить связь между видами состояний претензионно-исковой работы в базу данных. " +
                        "Подробная ошибка: {0}", e.Message), "Ошибка",
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
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_relation", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось удалить связь между видами состояний претензионно-исковой рабоыт. Подробная ошибка: {0}",
                        e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }
    }
}
