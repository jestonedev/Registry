using Registry.CalcDataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class DataModelsCallbackUpdater
    {
        private static DataModelsCallbackUpdater instance;
        private static string query = @"SELECT id_record, `table`, id_key, field_name, field_new_value, operation_type 
                                        FROM `log` WHERE id_record > ? AND (operation_type = 'UPDATE' OR (operation_type IN ('DELETE','INSERT') AND user_name <> ?))";
        private static string initQuery = @"SELECT IFNULL(MAX(id_record), 0) AS id_record, USER() AS user_name FROM log";
        private int id_record = -1;
        private string user_name = "";
        private DataRow updRow = null;

        private DataModelsCallbackUpdater()
        {
        }

        public void Initialize()
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = initQuery;
                try
                {
                    DataTable table = connection.SqlSelectTable("table", command);
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Не удалось инициализировать DataModelCallbackUpdater", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1);
                        Application.Exit();
                    }
                    id_record = Convert.ToInt32(table.Rows[0]["id_record"].ToString(), CultureInfo.InvariantCulture);
                    user_name = table.Rows[0]["user_name"].ToString();
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
            SynchronizationContext context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                DataTable tableCacheLvl1 = new DataTable("tableCacheLvl1");
                DataTable tableCacheLvl2 = new DataTable("tableCacheLvl2");
                tableCacheLvl1.Locale = CultureInfo.InvariantCulture;
                tableCacheLvl2.Locale = CultureInfo.InvariantCulture;
                InitializeColumns(tableCacheLvl1);
                InitializeColumns(tableCacheLvl2);
                while (true)
                {
                    //Пробуем обновить модель из кэша
                    tableCacheLvl2.Clear();
                    context.Send(__ =>
                    {
                        foreach (DataRow row in tableCacheLvl1.Rows)
                        {
                            if (!UpdateModelFromRow(row))
                                tableCacheLvl2.Rows.Add(RowToCacheObject(row));
                        }
                    }, null);
                    //Переносим данные из кэша 2-го уровня в первый
                    tableCacheLvl1.Clear();
                    foreach (DataRow row in tableCacheLvl2.Rows)
                        tableCacheLvl1.Rows.Add(RowToCacheObject(row));
                    //Обновляем модель из базы
                    using (DBConnection connection = new DBConnection())
                    using (DbCommand command = DBConnection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.Parameters.Add(DBConnection.CreateParameter<int>("id_record", id_record));
                        command.Parameters.Add(DBConnection.CreateParameter<string>("user_name", user_name));
                        try
                        {
                            DataTable tableDB = connection.SqlSelectTable("tableDB", command);
                            tableDB.Locale = CultureInfo.InvariantCulture;
                            context.Send(__ =>
                            {
                                foreach (DataRow row in tableDB.Rows)
                                {
                                    if (!UpdateModelFromRow(row))
                                        tableCacheLvl1.Rows.Add(RowToCacheObject(row));
                                }
                            }, null);
                            if (tableDB.Rows.Count > 0)
                                id_record = Convert.ToInt32(tableDB.Rows[tableDB.Rows.Count - 1]["id_record"].ToString(), CultureInfo.InvariantCulture);
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
            string table = row["table"].ToString();
            int id_key = Convert.ToInt32(row["id_key"].ToString(), CultureInfo.InvariantCulture);
            string field_name = row["field_name"].ToString();
            string field_value = row["field_new_value"].ToString();
            string operation_type = row["operation_type"].ToString();
            //Если таблица не загружена, то у пользователя просто нет необходимых прав. Игнорируем ее и возвращаем true
            if (!DataSetManager.DataSet.Tables.Contains(table))
                return true;
            DataTable updTable = DataSetManager.DataSet.Tables[table];
            //Ищем строку для обновления
            if (!((updRow != null)
                && (updRow.RowState != DataRowState.Deleted)
                && (updRow.RowState != DataRowState.Detached)
                && (updRow.Table.TableName == table) 
                && (updRow.Table.PrimaryKey.Length > 0)
                && (Convert.ToInt32(updRow[updRow.Table.PrimaryKey[0].ColumnName], CultureInfo.InvariantCulture) == id_key)))
            {
                //Если строка не закэширована, или закэширована не та строка, то надо найти и закэшировать строку по имени таблицы и id_key
                updRow = updTable.Rows.Find(id_key);
            }
            //Если строка в представлении пользователя существует, но помечена как удаленная, то игнорировать ее
            if (updRow != null && (updRow.RowState == DataRowState.Deleted || updRow.RowState == DataRowState.Detached))
                return true;
            switch (operation_type)
            {
                case "INSERT":
                    //Если модель находится в режиме IsNewRecord, то вернуть false
                    if (EditingNewRecordModel(table))
                        return false;
                    //Если строки нет, то создаем новую
                    if (updRow == null)
                    {
                        updRow = updTable.NewRow();
                        updRow[updRow.Table.PrimaryKey[0].ColumnName] = id_key;
                        updRow.EndEdit();
                        updTable.Rows.Add(updRow);
                    }
                    SetValue(updRow, field_name, field_value, operation_type);
                    return true;
                case "UPDATE":
                    //Если строки нет, то игнорируем и возвращаем false, чтобы сохранить в кэш строку
                    if (updRow == null)
                        return false;
                    SetValue(updRow, field_name, field_value, operation_type);
                    return true;
                case "DELETE":
                    //Если строка не найдена, значит она уже удалена, возвращаем true
                    if (updRow == null)
                        return true;
                    updTable.Rows.Remove(updRow);
                    CalcDataModelsUpdate(table, field_name, operation_type);
                    return true;
                default:
                    return true;
            }
        }

        private static void CalcDataModelsUpdate(string table, string field_name, string operation_type)
        {
            switch (table)
            {
                case "funds_history":
                    if (CalcDataModelBuildingsCurrentFunds.HasInstance())
                        CalcDataModelBuildingsCurrentFunds.GetInstance().DefferedUpdate = true;
                    if (CalcDataModelPremisesCurrentFunds.HasInstance())
                        CalcDataModelPremisesCurrentFunds.GetInstance().DefferedUpdate = true;
                    if (CalcDataModelBuildingsPremisesFunds.HasInstance())
                        CalcDataModelBuildingsPremisesFunds.GetInstance().DefferedUpdate = true;
                    break;
                case "sub_premises":
                    if (operation_type == "DELETE" || (operation_type == "UPDATE" && (field_name == "id_premises" || field_name == "sub_premises_num")))
                        if (CalcDataModelTenancyAggregated.HasInstance())
                            CalcDataModelTenancyAggregated.GetInstance().DefferedUpdate = true;
                    break;
                case "premises":
                    if (CalcDataModelBuildingsPremisesSumArea.HasInstance())
                        CalcDataModelBuildingsPremisesSumArea.GetInstance().DefferedUpdate = true;
                    if (operation_type == "DELETE")
                    {
                        if (CalcDataModelBuildingsPremisesFunds.HasInstance())
                            CalcDataModelBuildingsPremisesFunds.GetInstance().DefferedUpdate = true;
                    }
                    if (operation_type == "DELETE" || (operation_type == "UPDATE" && (field_name == "id_buildings" || field_name == "premises_num")))
                        if (CalcDataModelTenancyAggregated.HasInstance())
                            CalcDataModelTenancyAggregated.GetInstance().DefferedUpdate = true;
                    break;
                case "buildings":
                    if (operation_type == "DELETE" || (operation_type == "UPDATE" && (field_name == "id_street" || field_name == "house")))
                        if (CalcDataModelTenancyAggregated.HasInstance())
                            CalcDataModelTenancyAggregated.GetInstance().DefferedUpdate = true;
                    break;
                case "tenancy_buildings_assoc":
                case "tenancy_premises_assoc":
                case "tenancy_sub_premises_assoc":
                    if (CalcDataModelTenancyAggregated.HasInstance())
                        CalcDataModelTenancyAggregated.GetInstance().DefferedUpdate = true;
                    break;
            }
        }

        private static void SetValue(DataRow row, string field_name, string field_value, string operation_type)
        {
            if (String.IsNullOrEmpty(field_value))
            {
                if (!row[field_name].Equals(DBNull.Value))
                {
                    row[field_name] = DBNull.Value;
                    CalcDataModelsUpdate(row.Table.TableName, field_name, operation_type);
                }
            }
            else
            {
                object value = Convert.ChangeType(field_value, row.Table.Columns[field_name].DataType, CultureInfo.InvariantCulture);
                if (!row[field_name].Equals(value))
                {
                    row[field_name] = value;
                    CalcDataModelsUpdate(row.Table.TableName, field_name, operation_type);
                }
            }
        }

        private static bool EditingNewRecordModel(string table)
        {
            switch (table)
            {
                case "buildings": 
                    return BuildingsDataModel.GetInstance().EditingNewRecord;
                case "claim_states":
                    return ClaimStatesDataModel.GetInstance().EditingNewRecord;
                case "claims":
                    return ClaimsDataModel.GetInstance().EditingNewRecord;
                case "funds_buildings_assoc":
                    return FundsHistoryDataModel.GetInstance().EditingNewRecord;
                case "funds_history":
                    return FundsHistoryDataModel.GetInstance().EditingNewRecord;
                case "funds_premises_assoc":
                    return FundsHistoryDataModel.GetInstance().EditingNewRecord;
                case "funds_sub_premises_assoc":
                    return FundsHistoryDataModel.GetInstance().EditingNewRecord;
                case "premises":
                    return PremisesDataModel.GetInstance().EditingNewRecord;
                case "tenancy_agreements":
                    return TenancyAgreementsDataModel.GetInstance().EditingNewRecord;
                case "tenancy_persons":
                    return TenancyPersonsDataModel.GetInstance().EditingNewRecord;
                case "tenancy_processes":
                    return TenancyProcessesDataModel.GetInstance().EditingNewRecord;
                case "warrants":
                    return WarrantsDataModel.GetInstance().EditingNewRecord;
                case "documents_issued_by":
                case "executors":
                case "claim_state_types":
                case "claim_state_types_relations":
                case "ownership_buildings_assoc":
                case "ownership_premises_assoc":
                case "ownership_right_types":
                case "ownership_rights":
                case "restriction_types":
                case "restrictions":
                case "restrictions_buildings_assoc":
                case "restrictions_premises_assoc":
                case "structure_types":
                case "sub_premises":
                case "tenancy_buildings_assoc":
                case "tenancy_premises_assoc":
                case "tenancy_reasons":
                case "tenancy_reason_types":
                case "tenancy_sub_premises_assoc":
                    return false;
                default:
                    throw new NotImplementedException();
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
            return new object[] {
                row["table"],
                row["id_key"],
                row["field_name"],
                row["field_new_value"],
                row["operation_type"]
            };
        }

        public static DataModelsCallbackUpdater GetInstance()
        {
            if (instance == null)
                instance = new DataModelsCallbackUpdater();
            return instance;
        }
    }
}
