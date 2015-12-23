using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Registry.Entities;
using Registry.DataModels.CalcDataModels;
using Settings;

namespace Registry.DataModels.DataModels
{
    public class DataModel
    {
        private static readonly DataSet dataSet = new DataSet();
        public static DataSet DataSet { get { return dataSet; } }

        private DataTable _table;
        private DataModelLoadState _dmLoadState = DataModelLoadState.BeforeLoad;
        private DataModelLoadSyncType _dmLoadType = DataModelLoadSyncType.Syncronize; // По умолчанию загрузка синхронная

        public DataModelLoadState DmLoadState { get { return _dmLoadState; } set { _dmLoadState = value; } }
        public DataModelLoadSyncType DmLoadType { get { return _dmLoadType; } set { _dmLoadType = value; } }
        protected DataTable Table { get { return _table; } set { _table = value; } }

        private static readonly object LockObj = new object();
        // Не больше MaxDBConnectionCount потоков одновременно делают запросы к БД
        private static readonly Semaphore DbAccessSemaphore = new Semaphore(RegistrySettings.MaxDbConnectionCount, RegistrySettings.MaxDbConnectionCount);
        public bool EditingNewRecord { get; set; }

        protected DataModel()
        {
        }

        protected DataModel(ToolStripProgressBar progressBar, int incrementor, string selectQuery, string tableName)
        {
            var context = SynchronizationContext.Current;
            DmLoadType = DataModelLoadSyncType.Asyncronize;
            ThreadPool.QueueUserWorkItem(progress =>
            {
                try
                {
                    DmLoadState = DataModelLoadState.Loading;
                    using (var connection = new DBConnection())
                    using (var command = DBConnection.CreateCommand())
                    {
                        command.CommandText = selectQuery;
                        DbAccessSemaphore.WaitOne();
                        Interlocked.Exchange(ref _table, connection.SqlSelectTable(tableName, command));
                    }
                    DbAccessSemaphore.Release();
                    ConfigureTable();
                    lock (LockObj)
                    {
                        if (!dataSet.Tables.Contains(Table.TableName))
                        {
                            Table.ExtendedProperties.Add("model", this);
                            dataSet.Tables.Add(Table);
                            ConfigureRelations();
                        }
                    }
                    DmLoadState = DataModelLoadState.SuccessLoad;
                    if (progress != null)
                    {
                        context.Post(_ => {
                            progressBar.Value += incrementor;
                            if (progressBar.Value != progressBar.Maximum) return;
                            progressBar.Visible = false;
                            //Если мы загрузили все данные, то запускаем CallbackUpdater
                            DataModelsCallbackUpdater.GetInstance().Run();
                            CalcDataModel.RunRefreshWalker();
                        }, null);
                    }
                }
                catch (OdbcException e)
                {
                    lock (LockObj)
                    {
                        MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                            "Произошла ошибка при загрузке данных из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        DmLoadState = DataModelLoadState.ErrorLoad;
                        Application.Exit();
                    }
                }
                catch (DataModelException e)
                {
                    MessageBox.Show(e.Message, "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    DmLoadState = DataModelLoadState.ErrorLoad;
                }
            }, progressBar); 
        }

        public static DataModel GetInstance(DataModelType dataModelType)
        {
            return GetInstance(null, 0, dataModelType);
        }

        public static DataModel GetInstance(ToolStripProgressBar progressBar, int incrementor, DataModelType dataModelType)
        {
            switch (dataModelType)
            {
                case DataModelType.BuildingsDataModel:
                    return BuildingsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ClaimsDataModel:
                    return ClaimsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ClaimStatesDataModel:
                    return ClaimStatesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ClaimStateTypesDataModel:
                    return ClaimStateTypesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ClaimStateTypesRelationsDataModel:
                    return ClaimStateTypesRelationsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.DocumentsIssuedByDataModel:
                    return DocumentsIssuedByDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.DocumentsResidenceDataModel:
                    return DocumentsResidenceDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.DocumentTypesDataModel:
                    return DocumentTypesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ExecutorsDataModel:
                    return ExecutorsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.FundsBuildingsAssocDataModel:
                    return FundsBuildingsAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.FundsHistoryDataModel:
                    return FundsHistoryDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.FundsPremisesAssocDataModel:
                    return FundsPremisesAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.FundsSubPremisesAssocDataModel:
                    return FundsSubPremisesAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.FundTypesDataModel:
                    return FundTypesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.KinshipsDataModel:
                    return KinshipsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.KladrRegionsDataModel:
                    return KladrRegionsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.KladrStreetsDataModel:
                    return KladrStreetsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ObjectStatesDataModel:
                    return ObjectStatesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.OwnershipBuildingsAssocDataModel:
                    return OwnershipBuildingsAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.OwnershipPremisesAssocDataModel:
                    return OwnershipPremisesAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.OwnershipRightTypesDataModel:
                    return OwnershipRightTypesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.OwnershipsRightsDataModel:
                    return OwnershipsRightsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.PremisesDataModel:
                    return PremisesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.PremisesKindsDataModel:
                    return PremisesKindsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.PremisesTypesDataModel:
                    return PremisesTypesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.RentTypesDataModel:
                    return RentTypesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ResettleBuildingsFromAssocDataModel:
                    return ResettleBuildingsFromAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ResettleBuildingsToAssocDataModel:
                    return ResettleBuildingsToAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ResettlePersonsDataModel:
                    return ResettlePersonsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ResettlePremisesFromAssocDataModel:
                    return ResettlePremisesFromAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ResettlePremisesToAssocDataModel:
                    return ResettlePremisesToAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ResettleProcessesDataModel:
                    return ResettleProcessesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ResettleSubPremisesFromAssocDataModel:
                    return ResettleSubPremisesFromAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.ResettleSubPremisesToAssocDataModel:
                    return ResettleSubPremisesToAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.RestrictionsBuildingsAssocDataModel:
                    return RestrictionsBuildingsAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.RestrictionsDataModel:
                    return RestrictionsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.RestrictionsPremisesAssocDataModel:
                    return RestrictionsPremisesAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.RestrictionTypesDataModel:
                    return RestrictionTypesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.StructureTypesDataModel:
                    return StructureTypesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.SubPremisesDataModel:
                    return SubPremisesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.TenancyAgreementsDataModel:
                    return TenancyAgreementsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.TenancyBuildingsAssocDataModel:
                    return TenancyBuildingsAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.TenancyNotifiesDataModel:
                    return TenancyNotifiesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.TenancyPersonsDataModel:
                    return TenancyPersonsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.TenancyPremisesAssocDataModel:
                    return TenancyPremisesAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.TenancyProcessesDataModel:
                    return TenancyProcessesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.TenancyReasonsDataModel:
                    return TenancyReasonsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.TenancyReasonTypesDataModel:
                    return TenancyReasonTypesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.TenancySubPremisesAssocDataModel:
                    return TenancySubPremisesAssocDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.WarrantDocTypesDataModel:
                    return WarrantDocTypesDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.WarrantsDataModel:
                    return WarrantsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.PaymentsAccountsDataModel:
                    return PaymentsAccountsDataModel.GetInstance(progressBar, incrementor);
                case DataModelType.PaymentsDataModel:
                    return PaymentsDataModel.GetInstance();
                case DataModelType.SelectableHeadHousingDepDataModel:
                    return SelectableSigners.GetInstance();
                default:
                    throw new DataModelException("Неизвестный тип модели");
            }
        }

        public static DataModel GetLoadedInstance(string tableName)
        {
            DataModel dm = null;
            lock (LockObj)
            {
                if (dataSet.Tables.Contains(tableName))
                    dm = (DataModel)dataSet.Tables[tableName].ExtendedProperties["model"];
            }
            return dm;
        }

        protected static void AddRelation(string masterTableName, string masterColumnName, string slaveTableName,
            string slaveColumnName)
        {
            if (!dataSet.Tables.Contains(masterTableName)) return;
            if (!dataSet.Tables.Contains(slaveTableName)) return;
            if (dataSet.Relations.Contains(masterTableName + "_" + slaveTableName)) return;
            var relation = new DataRelation(masterTableName + "_" + slaveTableName,
                dataSet.Tables[masterTableName].Columns[masterColumnName],
                dataSet.Tables[slaveTableName].Columns[slaveColumnName], true);
            dataSet.Relations.Add(relation);
        }

        protected virtual void ConfigureRelations()
        {
            
        }

        protected virtual void ConfigureTable()
        {
        }

        public virtual DataTable Select()
        {
            if (DmLoadType == DataModelLoadSyncType.Syncronize)
                return Table;
            while (DmLoadState != DataModelLoadState.SuccessLoad)
            {
                if (DmLoadState == DataModelLoadState.ErrorLoad)
                {
                    lock (LockObj)
                    {
                        MessageBox.Show("Произошла ошибка при загрузке данных из базы данных. Дальнейшая работа приложения невозможна", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        Application.Exit();
                        return null;
                    }
                }
                Application.DoEvents();
            }
            return Table;
        }

        public virtual int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                ConfigureDeleteCommand(command, id);
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                        "Не удалось удалить объект из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        protected virtual void ConfigureDeleteCommand(DbCommand command, int id)
        {
        }

        public virtual int Update(Entity entity)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                if (entity == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                ConfigureUpdateCommand(command, entity);
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                        "Не удалось изменить данные о здание. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        protected virtual void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
        }

        public virtual int Insert(Entity entity)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            using (var lastIdCommand = DBConnection.CreateCommand())
            {
                if (entity == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }

                lastIdCommand.CommandText = "SELECT LAST_INSERT_ID()";
                ConfigureInsertCommand(command, entity);
                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    var lastId = connection.SqlSelectTable("last_id", lastIdCommand);
                    connection.SqlCommitTransaction();
                    if (lastId.Rows.Count != 0) 
                        return Convert.ToInt32(lastId.Rows[0][0], CultureInfo.InvariantCulture);
                    MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture,
                        "Не удалось добавить объект в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        protected virtual void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
        }

        public IEnumerable<DataRow> FilterDeletedRows()
        {
            return from tableRow in Select().AsEnumerable()
                   where (tableRow.RowState != DataRowState.Deleted) &&
                         (tableRow.RowState != DataRowState.Detached)
                   select tableRow;
        }    
    }
}
