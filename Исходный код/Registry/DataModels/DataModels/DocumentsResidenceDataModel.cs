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
    public sealed class DocumentsResidenceDataModel : DataModel
    {
        private static DocumentsResidenceDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM documents_residence WHERE deleted <> 1";
        private static string deleteQuery = "UPDATE documents_residence SET deleted = 1 WHERE id_document_residence = ?";
        private static string insertQuery = @"INSERT INTO documents_residence
                            (document_residence) VALUES (?)";
        private static string updateQuery = @"UPDATE documents_residence SET document_residence = ? WHERE id_document_residence = ?";
        private static string tableName = "documents_residence";

        private DocumentsResidenceDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_document_residence"] };
        }

        public static DocumentsResidenceDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static DocumentsResidenceDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new DocumentsResidenceDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Insert(DocumentResidence documentResidence)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (documentResidence == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на документ-основание на проживание", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_residence", documentResidence.DocumentResidenceName));
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
                    connection.SqlCommitTransaction();

                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.InvariantCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                        "Не удалось добавить запись о документе-основании на проживание. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(DocumentResidence documentResidence)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (documentResidence == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на документ-основание на проживание", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_residence", documentResidence.DocumentResidenceName));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_residence", documentResidence.IdDocumentResidence));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                        "Не удалось изменить запись в базе данных о документе-основании на проживание" +
                        ". Подробная ошибка: {0}", e.Message), "Ошибка",
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
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_residence", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                        "Не удалось удалить документ-основание на проживание. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
