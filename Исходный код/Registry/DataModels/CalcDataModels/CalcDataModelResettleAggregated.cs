using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;

namespace Registry.DataModels.CalcDataModels
{
    internal sealed class CalcDataModelResettleAggregated : CalcDataModel
    {
        private static CalcDataModelResettleAggregated _dataModel;

        private const string TableName = "resettle_aggregated";

        private CalcDataModelResettleAggregated()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.ResettleProcessesDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.ResettlePersonsDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.ResettleBuildingsFromAssocDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.ResettlePremisesFromAssocDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.ResettleSubPremisesFromAssocDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.ResettleBuildingsToAssocDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.ResettlePremisesToAssocDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.ResettleSubPremisesToAssocDataModel).Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_process").DataType = typeof(int);
            table.Columns.Add("resettlers").DataType = typeof(string);
            table.Columns.Add("address_from").DataType = typeof(string);
            table.Columns.Add("address_to").DataType = typeof(string);
            table.PrimaryKey = new [] { table.Columns["id_process"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModeTenancyAggregated");
            // Фильтруем удаленные строки
            var resettles = DataModel.GetInstance(DataModelType.ResettleProcessesDataModel).FilterDeletedRows();
            var resettlePersons = DataModel.GetInstance(DataModelType.ResettlePersonsDataModel).FilterDeletedRows();

            // Вычисляем агрегационную информацию
            var resettlers = from resettlePersonsRow in resettlePersons
                             group (resettlePersonsRow.Field<string>("surname") + " " + 
                                    resettlePersonsRow.Field<string>("name") + " " +
                                    resettlePersonsRow.Field<string>("patronymic")).Trim() by resettlePersonsRow.Field<int>("id_process") into gs
                             select new
                             {
                                 id_process = gs.Key,
                                 resettlers = gs.Aggregate((a, b) => a + ", " + b)
                             };
            var addressesFrom = DataModelHelper.AggregateAddressByIdProcess(
                DataModel.GetInstance(DataModelType.ResettleBuildingsFromAssocDataModel).FilterDeletedRows(),
                DataModel.GetInstance(DataModelType.ResettlePremisesFromAssocDataModel).FilterDeletedRows(),
                DataModel.GetInstance(DataModelType.ResettleSubPremisesFromAssocDataModel).FilterDeletedRows());
            var addressesTo = DataModelHelper.AggregateAddressByIdProcess(
                DataModel.GetInstance(DataModelType.ResettleBuildingsToAssocDataModel).FilterDeletedRows(),
                DataModel.GetInstance(DataModelType.ResettlePremisesToAssocDataModel).FilterDeletedRows(),
                DataModel.GetInstance(DataModelType.ResettleSubPremisesToAssocDataModel).FilterDeletedRows());
            var result = from resettlesRow in resettles
                         join resettlersRow in resettlers
                         on resettlesRow.Field<int>("id_process") equals resettlersRow.id_process into a
                         from aRow in a.DefaultIfEmpty()
                         join addressesFromRow in addressesFrom
                         on resettlesRow.Field<int>("id_process") equals addressesFromRow.IdProcess into b
                         from bRow in b.DefaultIfEmpty()
                         join addressesToRow in addressesTo
                         on resettlesRow.Field<int>("id_process") equals addressesToRow.IdProcess into c
                         from cRow in c.DefaultIfEmpty()
                         select new
                         {
                             id_process = resettlesRow.Field<int>("id_process"),
                             address_to = (cRow == null) ? "" : cRow.Address,
                             address_from = (bRow == null) ? "" : bRow.Address,
                             resettlers = (aRow == null) ? "" : aRow.resettlers
                         };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_process, x.resettlers, x.address_from, x.address_to);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelResettleAggregated GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelResettleAggregated());
        }
    }
}
