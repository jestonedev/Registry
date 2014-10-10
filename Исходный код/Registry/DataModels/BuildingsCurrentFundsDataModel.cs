using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Registry.DataModels
{
    public sealed class BuildingsCurrentFundsDataModel : DataModel
    {
        private static BuildingsCurrentFundsDataModel dataModel = null;

        private static string tableName = "buildings_current_funds"; 

        public static BuildingsCurrentFundsDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new BuildingsCurrentFundsDataModel();
            return dataModel;
        }

        public override DataTable Select()
        {
            DataTable buildings = BuildingsDataModel.GetInstance().Select();
            DataTable funds_history = FundsHistoryDataModel.GetInstance().Select();
            DataTable funds_buildings_assoc = FundsBuildingsAssocDataModel.GetInstance().Select();
            var max_id_by_buldings = from funds_buildings_assoc_row in funds_buildings_assoc.AsEnumerable()
                                        join fund_history_row in funds_history.AsEnumerable()
                                        on funds_buildings_assoc_row.Field<int>("id_fund") equals fund_history_row.Field<int>("id_fund")
                                     where fund_history_row.Field<DateTime?>("exclude_restriction_date") == null
                                     group funds_buildings_assoc_row.Field<int>("id_fund") by
                                             funds_buildings_assoc_row.Field<int>("id_building") into gs
                                     select new
                                     {
                                         id_building = gs.Key,
                                         id_fund = gs.Max()
                                     };
            var result = from fund_history_row in funds_history.AsEnumerable()
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
            DataTable buildings_current_funds = new DataTable(tableName);
            buildings_current_funds.Columns.Add("id_building").DataType = typeof(int);
            buildings_current_funds.Columns.Add("id_fund_type").DataType = typeof(int);
            result.ToList().ForEach((x) =>
            {
                buildings_current_funds.Rows.Add(new object[] { 
                    x.id_building, 
                    x.id_fund_type });
            });
            buildings_current_funds.PrimaryKey = new DataColumn[] { buildings_current_funds.Columns["id_building"] };
            return buildings_current_funds;
        }
    }
}
