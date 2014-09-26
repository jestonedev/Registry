using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Registry.DataModels
{
    public class BuildingsCurrentFundsDataModel: DataModel
    {
        private static BuildingsCurrentFundsDataModel dataModel = null;

        private static string tableName = "buildings_current_funds"; 

        public static BuildingsCurrentFundsDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new BuildingsCurrentFundsDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }

        public override DataTable Select()
        {
            DataTable buildings = BuildingsDataModel.GetInstance().Select();
            DataTable funds_history = FundsHistoryDataModel.GetInstance().Select();
            DataTable funds_buildings_assoc = FundsBuildingsAssocDataModel.GetInstance().Select();
            var max_date_by_buildings = from fund_history_row in funds_history.AsEnumerable()
                                        join funds_buildings_assoc_row in funds_buildings_assoc.AsEnumerable()
                                        on fund_history_row.Field<int>("id_fund") equals funds_buildings_assoc_row.Field<int>("id_fund")
                                        group fund_history_row.Field<DateTime>("protocol_date") by
                                             funds_buildings_assoc_row.Field<int>("id_building") into gs
                                        select new
                                        {
                                           id_building = gs.Key,
                                           max_date = gs.Max()
                                        };
            var not_deleted_document = from fund_history_row in funds_history.AsEnumerable()
                                       join funds_buildings_assoc_row in funds_buildings_assoc.AsEnumerable()
                                       on fund_history_row.Field<int>("id_fund") equals funds_buildings_assoc_row.Field<int>("id_fund")
                                       select new
                                       {
                                           id_building = funds_buildings_assoc_row.Field<int>("id_building"),
                                           id_fund = fund_history_row.Field<int>("id_fund"),
                                           id_fund_type = fund_history_row.Field<int>("id_fund_type"),
                                           protocol_num = fund_history_row.Field<string>("protocol_num"),
                                           protocol_date = fund_history_row.Field<DateTime>("protocol_date"),
                                       };
            var result = from mdp_row in max_date_by_buildings
                         join ndd_row in not_deleted_document
                         on new { date = mdp_row.max_date, id_building = mdp_row.id_building } equals
                             new { date = ndd_row.protocol_date, id_building = ndd_row.id_building }
                         group new
                         {
                             id_building = ndd_row.id_building,
                             id_fund = ndd_row.id_fund,
                             id_fund_type = ndd_row.id_fund_type,
                             protocol_num = ndd_row.protocol_num,
                             protocol_date = ndd_row.protocol_date
                         } by new { ndd_row.id_building, ndd_row.protocol_date } into gs
                         select gs.Last();
            DataTable buildings_current_funds = new DataTable(tableName);
            buildings_current_funds.Columns.Add("id_building").DataType = typeof(int);
            buildings_current_funds.Columns.Add("id_fund").DataType = typeof(int);
            result.ToList().ForEach((x) =>
            {
                buildings_current_funds.Rows.Add(new object[] { 
                    x.id_building, 
                    x.id_fund });
            });
            buildings_current_funds.PrimaryKey = new DataColumn[] { buildings_current_funds.Columns["id_building"] };
            return buildings_current_funds;
        }
    }
}
