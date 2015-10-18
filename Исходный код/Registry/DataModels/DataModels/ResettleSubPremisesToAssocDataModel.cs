using Registry.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels.DataModels;

namespace Registry.DataModels
{
    public sealed class ResettleSubPremisesToAssocDataModel: DataModel
    {
        private static ResettleSubPremisesToAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM resettle_sub_premises_to_assoc WHERE deleted = 0";
        private static string deleteQuery = "UPDATE resettle_sub_premises_to_assoc SET deleted = 1 WHERE id_assoc = ?";
        private static string insertQuery = @"INSERT INTO resettle_sub_premises_to_assoc (id_sub_premises, id_process) VALUES (?,?)";
        private static string tableName = "resettle_sub_premises_to_assoc";

        private ResettleSubPremisesToAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_assoc"] };
        }

        public static ResettleSubPremisesToAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ResettleSubPremisesToAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ResettleSubPremisesToAssocDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Insert(ResettleObject resettleObject)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (resettleObject == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на сущность объекта переселения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("id_sub_premises", resettleObject.IdObject));
                command.Parameters.Add(DBConnection.CreateParameter("id_process", resettleObject.IdProcess));
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
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.InvariantCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                        "Не удалось добавить связь комнаты с процессом переселения в базу данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                        "Не удалось удалить связь комнаты с процесссом переселения из базы данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
