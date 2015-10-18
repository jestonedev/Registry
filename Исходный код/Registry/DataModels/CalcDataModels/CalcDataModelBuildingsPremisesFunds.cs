using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using Registry.DataModels;
using System.Globalization;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelBuildingsPremisesFunds : CalcDataModel
    {
        private static CalcDataModelBuildingsPremisesFunds dataModel = null;

        private static string tableName = "buildings_premises_funds";

        private CalcDataModelBuildingsPremisesFunds()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("social_premises_count").DataType = typeof(int);
            table.Columns.Add("commercial_premises_count").DataType = typeof(int);
            table.Columns.Add("special_premises_count").DataType = typeof(int);
            table.Columns.Add("other_premises_count").DataType = typeof(int);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_building"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelBuildingsPremisesFunds");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            var funds_history = DataModelHelper.FilterRows(FundsHistoryDataModel.GetInstance().Select());
            var funds_premises_assoc = DataModelHelper.FilterRows(FundsPremisesAssocDataModel.GetInstance().Select());
            // Вычисляем агрегационную информацию
            var max_id_by_premises = DataModelHelper.MaxFundIDsByObject(funds_premises_assoc, EntityType.Premise);  
            var current_funds = from fund_history_row in funds_history
                                    join max_id_by_premises_row in max_id_by_premises
                                       on fund_history_row.Field<int>("id_fund") equals max_id_by_premises_row.IdFund
                                    select new
                                    {
                                        id_premises = max_id_by_premises_row.IdObject,
                                        id_fund = max_id_by_premises_row.IdFund,
                                        id_fund_type = fund_history_row.Field<int>("id_fund_type"),
                                    };
            var social_premises = from premises_row in premises
                                  join agg_row in current_funds
                                  on premises_row.Field<int>("id_premises") equals agg_row.id_premises
                                  where agg_row.id_fund_type == 1
                                  group premises_row.Field<int>("id_premises") by premises_row.Field<int>("id_building") into gs
                                  select new
                                  {
                                      id_building = gs.Key,
                                      social_premises_count = gs.Count()
                                  };
            var commercial_premises = from premises_row in premises
                                      join agg_row in current_funds
                                      on premises_row.Field<int>("id_premises") equals agg_row.id_premises
                                      where agg_row.id_fund_type == 2
                                      group premises_row.Field<int>("id_premises") by premises_row.Field<int>("id_building") into gs
                                      select new
                                      {
                                          id_building = gs.Key,
                                          commercial_premises_count = gs.Count()
                                      };
            var special_premises = from premises_row in premises
                                   join agg_row in current_funds
                                   on premises_row.Field<int>("id_premises") equals agg_row.id_premises
                                   where agg_row.id_fund_type == 3
                                   group premises_row.Field<int>("id_premises") by premises_row.Field<int>("id_building") into gs
                                   select new
                                   {
                                       id_building = gs.Key,
                                       special_premises_count = gs.Count()
                                   };
            var other_premises = from premises_row in premises
                                 join agg_row in current_funds
                                 on premises_row.Field<int>("id_premises") equals agg_row.id_premises
                                 where agg_row.id_fund_type == 4
                                 group premises_row.Field<int>("id_premises") by premises_row.Field<int>("id_building") into gs
                                 select new
                                 {
                                     id_building = gs.Key,
                                     other_premises_count = gs.Count()
                                 };
            var result = from buildings_row in buildings
                         join sp_row in social_premises on buildings_row.Field<int>("id_building") equals sp_row.id_building into sp
                         from buildings_row_sp in sp.DefaultIfEmpty()
                         join cp_row in commercial_premises on buildings_row.Field<int>("id_building") equals cp_row.id_building into cp
                         from buildings_row_cp in cp.DefaultIfEmpty()
                         join spp_row in special_premises on buildings_row.Field<int>("id_building") equals spp_row.id_building into spp
                         from buildings_row_spp in spp.DefaultIfEmpty()
                         join o_row in other_premises on buildings_row.Field<int>("id_building") equals o_row.id_building into opp
                         from buildings_row_opp in opp.DefaultIfEmpty()
                         select new
                         {
                             id_building = buildings_row.Field<int>("id_building"),
                             social_premises_count = (buildings_row_sp == null) ? 0 : buildings_row_sp.social_premises_count,
                             commercial_premises_count = (buildings_row_cp == null) ? 0 : buildings_row_cp.commercial_premises_count,
                             special_premises_count = (buildings_row_spp == null) ? 0 : buildings_row_spp.special_premises_count,
                             other_premises_count = (buildings_row_opp == null) ? 0 : buildings_row_opp.other_premises_count
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_building, 
                    x.social_premises_count, 
                    x.commercial_premises_count, 
                    x.special_premises_count, 
                    x.other_premises_count });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelBuildingsPremisesFunds GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelBuildingsPremisesFunds();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
