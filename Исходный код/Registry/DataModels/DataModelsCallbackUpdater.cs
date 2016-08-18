using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Settings;

namespace Registry.DataModels
{
    internal sealed class DataModelsCallbackUpdater
    {
        private static DataModelsCallbackUpdater _instance;
        private const string Query = @"SELECT id_record, `table`, id_key, field_name, field_new_value, operation_type 
                                        FROM `log` WHERE id_record > ? AND (operation_type = 'UPDATE' OR (operation_type IN ('DELETE','INSERT') AND (user_name <> ? OR `table` = 'tenancy_notifies')))";
        private const string InitQuery = @"SELECT IFNULL(MAX(id_record), 0) AS id_record, USER() AS user_name FROM log";
        private int _idRecord = -1;
        private string _userName = "";
        private DataRow _updRow;

        private DataModelsCallbackUpdater()
        {
        }

        public void Initialize()
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = InitQuery;
                try
                {
                    var table = connection.SqlSelectTable("table", command);
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Не удалось инициализировать DataModelCallbackUpdater", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1);
                        Application.Exit();
                    }
                    _idRecord = Convert.ToInt32(table.Rows[0]["id_record"].ToString(), CultureInfo.InvariantCulture);
                    _userName = table.Rows[0]["user_name"].ToString();
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                            "Произошла ошибка при загрузке данных из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    Application.Exit();
                }
            }
        }

        public void Run()
        {
            var context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var tableCacheLvl1 = new DataTable("tableCacheLvl1");
                var tableCacheLvl2 = new DataTable("tableCacheLvl2");
                tableCacheLvl1.Locale = CultureInfo.InvariantCulture;
                tableCacheLvl2.Locale = CultureInfo.InvariantCulture;
                InitializeColumns(tableCacheLvl1);
                InitializeColumns(tableCacheLvl2);
                while (true)
                {
                    bool error = false;
                    //Пробуем обновить модель из кэша
                    tableCacheLvl2.Clear();
                    context.Send(__ =>
                    {
                        var workTable = tableCacheLvl1;
                        foreach (DataRow row in workTable.Rows)
                        {
                            if (!error && UpdateModelFromRow(row)) continue;
                            tableCacheLvl2.Rows.Add(RowToCacheObject(row));
                            error = true;
                        }
                    }, null);
                    //Переносим данные из кэша 2-го уровня в первый
                    tableCacheLvl1.Clear();
                    foreach (DataRow row in tableCacheLvl2.Rows)
                        tableCacheLvl1.Rows.Add(RowToCacheObject(row));
                    if (error)
                        continue;
                    //Обновляем модель из базы
                    using (var connection = new DBConnection())
                    using (var command = DBConnection.CreateCommand())
                    {
                        command.CommandText = Query;
                        command.Parameters.Add(DBConnection.CreateParameter("id_record", _idRecord));
                        command.Parameters.Add(DBConnection.CreateParameter("user_name", _userName));
                        try
                        {
                            var tableDb = connection.SqlSelectTable("tableDB", command);
                            tableDb.Locale = CultureInfo.InvariantCulture;
                            context.Send(__ =>
                            {
                                var workTable = tableDb;
                                foreach (DataRow row in workTable.Rows)
                                {
                                    if (!error && UpdateModelFromRow(row)) continue;
                                    tableCacheLvl1.Rows.Add(RowToCacheObject(row));
                                    error = true;
                                }
                            }, null);
                            if (tableDb.Rows.Count > 0)
                                _idRecord = Convert.ToInt32(tableDb.Rows[tableDb.Rows.Count - 1]["id_record"].ToString(), CultureInfo.InvariantCulture);
                        }
                        catch (OdbcException)
                        {
                            //При ошибке загрузки из БД мы просто игнорируем и не производим обновление локальной модели до тех пор, пока связь не установится
                        }
                    }
                    //Обновление делаем примерно каждые DataModelsCallbackUpdateTimeout милисекунд
                    Thread.Sleep(RegistrySettings.DataModelsCallbackUpdateTimeout);
                }
            }, null);
        }

        private bool UpdateModelFromRow(DataRow row)
        {
            var table = row["table"].ToString();
            var idKey = Convert.ToInt32(row["id_key"].ToString(), CultureInfo.InvariantCulture);
            var fieldName = row["field_name"].ToString();
            var fieldValue = row["field_new_value"].ToString();
            var operationType = row["operation_type"].ToString();
            //Если таблица не загружена, то у пользователя просто нет необходимых прав. Игнорируем ее и возвращаем true
            if (DataModel.GetLoadedInstance(table) == null)
                return true;
            var updTable = DataModel.GetLoadedInstance(table).Select();
            //Ищем строку для обновления
            if (!((_updRow != null)
                && (_updRow.RowState != DataRowState.Deleted)
                && (_updRow.RowState != DataRowState.Detached)
                && (_updRow.Table.TableName == table) 
                && (_updRow.Table.PrimaryKey.Length > 0)
                && (Convert.ToInt32(_updRow[_updRow.Table.PrimaryKey[0].ColumnName], CultureInfo.InvariantCulture) == idKey)))
            {
                //Если строка не закэширована, или закэширована не та строка, то надо найти и закэшировать строку по имени таблицы и id_key
                _updRow = updTable.Rows.Find(idKey);
            }
            //Если строка в представлении пользователя существует, но помечена как удаленная, то игнорировать ее
            if (_updRow != null && (_updRow.RowState == DataRowState.Deleted || _updRow.RowState == DataRowState.Detached))
                return true;
            switch (operationType)
            {
                case "INSERT":
                    //Если модель находится в режиме IsNewRecord, то вернуть false
                    if (DataModel.GetLoadedInstance(table).EditingNewRecord)
                        return false;
                    //Если строки нет, то создаем новую
                    if (_updRow == null)
                    {
                        _updRow = updTable.NewRow();
                        _updRow[_updRow.Table.PrimaryKey[0].ColumnName] = idKey;
                        _updRow.EndEdit();
                        updTable.Rows.Add(_updRow);
                    }
                    SetValue(_updRow, fieldName, fieldValue);
                    return true;
                case "UPDATE":
                    //Если строки нет, то игнорируем и возвращаем false, чтобы сохранить в кэш строку
                    if (_updRow == null)
                        return false;
                    //Если строка есть, но при обновлении нарушается ссылочная целостность, то
                    //текущий пользователь уже удалил строку справочника. Игнорируем и возвращаем true
                    try
                    {
                        SetValue(_updRow, fieldName, fieldValue);
                    }
                    catch (InvalidConstraintException)
                    {
                        return true;
                    }
                    return true;
                case "DELETE":
                    //Если строка не найдена, значит она уже удалена, возвращаем true
                    if (_updRow == null)
                        return true;
                    updTable.Rows.Remove(_updRow);
                    return true;
                default:
                    return true;
            }
        }

        private static void SetValue(DataRow row, string fieldName, string fieldValue)
        {
            // Если поле не найдено, то возможно оно новое в базе и надо его проигнорировать
            if (!row.Table.Columns.Contains(fieldName))
                return;
            if (string.IsNullOrEmpty(fieldValue))
            {
                if (row[fieldName].Equals(DBNull.Value)) return;
                row[fieldName] = DBNull.Value;
                row.EndEdit();
            }
            else
            {
                var value = Convert.ChangeType(fieldValue, row.Table.Columns[fieldName].DataType, CultureInfo.InvariantCulture);
                if (row[fieldName].Equals(value)) return;
                row[fieldName] = value;
                row.EndEdit();
            }
        }

        private static void InitializeColumns(DataTable table)
        {
            table.Columns.Add("table").DataType = typeof(string);
            table.Columns.Add("id_key").DataType = typeof(int);
            table.Columns.Add("field_name").DataType = typeof(string);
            table.Columns.Add("field_new_value").DataType = typeof(string);
            table.Columns.Add("operation_type").DataType = typeof(string);
        }

        private static object[] RowToCacheObject(DataRow row)
        {
            return new[] {
                row["table"],
                row["id_key"],
                row["field_name"],
                row["field_new_value"],
                row["operation_type"]
            };
        }

        public static DataModelsCallbackUpdater GetInstance()
        {
            return _instance ?? (_instance = new DataModelsCallbackUpdater());
        }
    }
}
