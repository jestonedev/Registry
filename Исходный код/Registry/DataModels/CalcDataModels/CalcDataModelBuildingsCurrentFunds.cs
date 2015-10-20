using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.CalcDataModels
{
    internal sealed class CalcDataModelBuildingsCurrentFunds : CalcDataModel
    {
        private static CalcDataModelBuildingsCurrentFunds _dataModel;
        private const string TableName = "buildings_current_funds";

        private CalcDataModelBuildingsCurrentFunds()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.FundsHistoryDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.FundsBuildingsAssocDataModel).Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("id_fund_type").DataType = typeof(int);
            table.PrimaryKey = new [] { table.Columns["id_building"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelBuildingsCurrentFunds");
            // Фильтруем удаленные строки
            var fundsHistory = DataModel.GetInstance(DataModelType.FundsHistoryDataModel).FilterDeletedRows();
            var fundsBuildingsAssoc = DataModel.GetInstance(DataModelType.FundsBuildingsAssocDataModel).FilterDeletedRows();
            // Вычисляем агрегационную информацию
            var maxIdByBuldings = DataModelHelper.MaxFundIDsByObject(fundsBuildingsAssoc, EntityType.Building);          
            var result = from fundHistoryRow in fundsHistory
                         join maxIdByBuildingRow in maxIdByBuldings
                                       on fundHistoryRow.Field<int>("id_fund") equals maxIdByBuildingRow.IdFund
                         select new
                         {
                             id_building = maxIdByBuildingRow.IdObject,
                             id_fund = maxIdByBuildingRow.IdFund,
                             id_fund_type = fundHistoryRow.Field<int>("id_fund_type")
                         };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_building, x.id_fund_type);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelBuildingsCurrentFunds GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelBuildingsCurrentFunds());
        }
    }
}
