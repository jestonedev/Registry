using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public class BuildingsAggregatedDataModel: DataModel
    {
        private static BuildingsAggregatedDataModel dataModel = null;

        private static string tableName = "buildings_aggregated"; 

        private BuildingsAggregatedDataModel()
        {        
        }

        public override DataTable Select()
        {
            DataTable buildings = BuildingsDataModel.GetInstance().Select();
            DataTable premises = PremisesDataModel.GetInstance().Select();
            DataTable funds_history = FundsHistoryDataModel.GetInstance().Select();
            DataTable funds_premises_assoc = FundsPremisesAssocDataModel.GetInstance().Select();
            var max_date_by_premises = from fund_history_row in funds_history.AsEnumerable()
                                       join funds_premises_assoc_row in funds_premises_assoc.AsEnumerable()
                                       on fund_history_row.Field<int>("id_fund") equals funds_premises_assoc_row.Field<int>("id_fund")
                                       group fund_history_row.Field<DateTime>("protocol_date") by
                                             funds_premises_assoc_row.Field<int>("id_premises") into gs
                                       select new
                                       {
                                           id_premises = gs.Key,
                                           max_date = gs.Max()
                                       };
            var not_deleted_document = from fund_history_row in funds_history.AsEnumerable()
                                       join funds_premises_assoc_row in funds_premises_assoc.AsEnumerable()
                                       on fund_history_row.Field<int>("id_fund") equals funds_premises_assoc_row.Field<int>("id_fund")
                                       select new
                                       {
                                           id_premises = funds_premises_assoc_row.Field<int>("id_premises"),
                                           id_fund = fund_history_row.Field<int>("id_fund"),
                                           id_fund_type = fund_history_row.Field<int>("id_fund_type"),
                                           protocol_num = fund_history_row.Field<string>("protocol_num"),
                                           protocol_date = fund_history_row.Field<DateTime>("protocol_date"),
                                       };
            var join_mdp_and_ndd = from mdp_row in max_date_by_premises
                                   join ndd_row in not_deleted_document
                                   on new { date = mdp_row.max_date, id_premises = mdp_row.id_premises } equals
                                      new { date = ndd_row.protocol_date, id_premises = ndd_row.id_premises }
                                   group new
                                   {
                                       id_premises = ndd_row.id_premises,
                                       id_fund = ndd_row.id_fund,
                                       id_fund_type = ndd_row.id_fund_type,
                                       protocol_num = ndd_row.protocol_num,
                                       protocol_date = ndd_row.protocol_date
                                   } by new { ndd_row.id_premises, ndd_row.protocol_date } into gs
                                   select gs.Last();
            var social_premises = from premises_row in premises.AsEnumerable()
                                  join agg_row in join_mdp_and_ndd
                                  on premises_row.Field<int>("id_premises") equals agg_row.id_premises
                                  where agg_row.id_fund_type == 1
                                  group premises_row.Field<int>("id_premises") by premises_row.Field<int>("id_building") into gs
                                  select new {
                                    id_building = gs.Key,
                                    social_premises_count = gs.Count()
                                  };
            var commercial_premises = from premises_row in premises.AsEnumerable()
                                  join agg_row in join_mdp_and_ndd
                                  on premises_row.Field<int>("id_premises") equals agg_row.id_premises
                                  where agg_row.id_fund_type == 2
                                  group premises_row.Field<int>("id_premises") by premises_row.Field<int>("id_building") into gs
                                  select new
                                  {
                                      id_building = gs.Key,
                                      commercial_premises_count = gs.Count()
                                  };
            var special_premises = from premises_row in premises.AsEnumerable()
                                  join agg_row in join_mdp_and_ndd
                                  on premises_row.Field<int>("id_premises") equals agg_row.id_premises
                                  where agg_row.id_fund_type == 3
                                  group premises_row.Field<int>("id_premises") by premises_row.Field<int>("id_building") into gs
                                  select new
                                  {
                                      id_building = gs.Key,
                                      special_premises_count = gs.Count()
                                  };
            var other_premises = from premises_row in premises.AsEnumerable()
                                  join agg_row in join_mdp_and_ndd
                                  on premises_row.Field<int>("id_premises") equals agg_row.id_premises
                                  where agg_row.id_fund_type == 4
                                  group premises_row.Field<int>("id_premises") by premises_row.Field<int>("id_building") into gs
                                  select new
                                  {
                                      id_building = gs.Key,
                                      other_premises_count = gs.Count()
                                  };
            var result = from buildings_row in buildings.AsEnumerable()
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
            DataTable buildings_aggregated = new DataTable(tableName);
            buildings_aggregated.Columns.Add("id_building").DataType = typeof(int);
            buildings_aggregated.Columns.Add("social_premises_count").DataType = typeof(int);
            buildings_aggregated.Columns.Add("commercial_premises_count").DataType = typeof(int);
            buildings_aggregated.Columns.Add("special_premises_count").DataType = typeof(int);
            buildings_aggregated.Columns.Add("other_premises_count").DataType = typeof(int);
            result.ToList().ForEach((x) =>
            {
                buildings_aggregated.Rows.Add(new object[] { 
                    x.id_building, 
                    x.social_premises_count, 
                    x.commercial_premises_count, 
                    x.special_premises_count, 
                    x.other_premises_count });
            });
            buildings_aggregated.PrimaryKey = new DataColumn[] { buildings_aggregated.Columns["id_building"] };
            return buildings_aggregated;
        }

        public static BuildingsAggregatedDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new BuildingsAggregatedDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
