using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;

namespace Registry.DataModels
{
    public sealed class DocumentsIssuedByDataModel: DataModel
    {
        private static DocumentsIssuedByDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM documents_issued_by";
        private static string deleteQuery = "DELETE FROM documents_issued_by WHERE id_document_issued_by = ?";
        private static string insertQuery = @"INSERT INTO documents_issued_by
                            (document_issued_by) VALUES (?)";
        private static string updateQuery = @"UPDATE documents_issued_by SET document_issued_by = ? WHERE id_document_issued_by = ?";
        private static string tableName = "documents_issued_by";

        private DocumentsIssuedByDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_document_issued_by"] };
        }

        public static DocumentsIssuedByDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static DocumentsIssuedByDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new DocumentsIssuedByDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Insert(DocumentIssuedBy documentIssuedBy)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;

                command.Parameters.Add(DBConnection.CreateParameter<string>("document_issued_by", documentIssuedBy.document_issued_by));

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
                    connection.SqlCommitTransaction();

                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось добавить запись об органе, выдающем документы, удостоверяющие личность. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Update(Entities.DocumentIssuedBy documentIssuedBy)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;

                command.Parameters.Add(DBConnection.CreateParameter<string>("document_issued_by", documentIssuedBy.document_issued_by));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_issued_by", documentIssuedBy.id_document_issued_by));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить запись в базе данных об органе, выдающем документы, удостоверяющие личность" +
                        ". Подробная ошибка: {0}", e.Message), "Ошибка", 
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
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_issued_by", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось удалить орган, выдающий документы, удостоверяющие личность, из базы данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }
    }
}
