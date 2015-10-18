using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.DataModels;
using System.Globalization;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelBuildingsCurrentFunds : CalcDataModel
    {
        private static CalcDataModelBuildingsCurrentFunds dataModel = null;

        private static string tableName = "buildings_current_funds";

        private CalcDataModelBuildingsCurrentFunds(): base()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("id_fund_type").DataType = typeof(int);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_building"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelBuildingsCurrentFunds");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var funds_history = DataModelHelper.FilterRows(FundsHistoryDataModel.GetInstance().Select());
            var funds_buildings_assoc = DataModelHelper.FilterRows(FundsBuildingsAssocDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            // Вычисляем агрегационную информацию
            var max_id_by_buldings = DataModelHelper.MaxFundIDsByObject(funds_buildings_assoc, EntityType.Building);          
            var result = from fund_history_row in funds_history
                         join max_id_by_building_row in max_id_by_buldings
                                       on fund_history_row.Field<int>("id_fund") equals max_id_by_building_row.IdFund
                         select new
                         {
                             id_building = max_id_by_building_row.IdObject,
                             id_fund = max_id_by_building_row.IdFund,
                             id_fund_type = fund_history_row.Field<int>("id_fund_type")
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_building, 
                    x.id_fund_type });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelBuildingsCurrentFunds GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelBuildingsCurrentFunds();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
