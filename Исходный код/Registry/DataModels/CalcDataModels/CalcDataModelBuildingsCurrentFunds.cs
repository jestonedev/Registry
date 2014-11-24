using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.DataModels;
using System.Globalization;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelBuildingsCurrentFunds : CalcDataModel
    {
        private static CalcDataModelBuildingsCurrentFunds dataModel = null;

        private static string tableName = "buildings_current_funds";

        private CalcDataModelBuildingsCurrentFunds(): base()
        {
            Table = InitializeTable();
            Refresh(CalcDataModelFilterEnity.All, null);
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
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelBuildingsCurrentFunds");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var funds_history = DataModelHelper.FilterRows(FundsHistoryDataModel.GetInstance().Select());
            var funds_buildings_assoc = from funds_buildings_assoc_row in DataModelHelper.FilterRows(FundsBuildingsAssocDataModel.GetInstance().Select())
                                        where (config.Entity == CalcDataModelFilterEnity.Building ? funds_buildings_assoc_row.Field<int>("id_building") == config.IdObject :
                                               config.Entity == CalcDataModelFilterEnity.All ? true : false)
                                        select funds_buildings_assoc_row;
            // Вычисляем агрегационную информацию
            var max_id_by_buldings = from funds_buildings_assoc_row in funds_buildings_assoc
                                     join fund_history_row in funds_history
                                     on funds_buildings_assoc_row.Field<int>("id_fund") equals fund_history_row.Field<int>("id_fund")
                                     where fund_history_row.Field<DateTime?>("exclude_restriction_date") == null
                                     group funds_buildings_assoc_row.Field<int>("id_fund") by
                                             funds_buildings_assoc_row.Field<int>("id_building") into gs
                                     select new
                                     {
                                         id_building = gs.Key,
                                         id_fund = gs.Max()
                                     };
            var result = from fund_history_row in funds_history
                         join max_id_by_building_row in max_id_by_buldings
                                       on fund_history_row.Field<int>("id_fund") equals max_id_by_building_row.id_fund
                         select new
                         {
                             id_building = max_id_by_building_row.id_building,
                             id_fund = max_id_by_building_row.id_fund,
                             id_fund_type = fund_history_row.Field<int>("id_fund_type"),
                             protocol_number = fund_history_row.Field<string>("protocol_number"),
                             protocol_date = fund_history_row.Field<DateTime?>("protocol_date"),
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
    }
}
