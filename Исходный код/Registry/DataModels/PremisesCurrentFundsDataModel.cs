using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Registry.DataModels
{
    public class PremisesCurrentFundsDataModel : DataModel
    {
        private static PremisesCurrentFundsDataModel dataModel = null;

        private static string tableName = "premises_current_funds";

        public static PremisesCurrentFundsDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new PremisesCurrentFundsDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }

        public override DataTable Select()
        {
            DataTable premises = PremisesDataModel.GetInstance().Select();
            DataTable funds_history = FundsHistoryDataModel.GetInstance().Select();
            DataTable funds_premsies_assoc = FundsPremisesAssocDataModel.GetInstance().Select();
            var max_date_by_premises = from fund_history_row in funds_history.AsEnumerable()
                                        join funds_premsies_assoc_row in funds_premsies_assoc.AsEnumerable()
                                        on fund_history_row.Field<int>("id_fund") equals funds_premsies_assoc_row.Field<int>("id_fund")
                                        group fund_history_row.Field<DateTime>("protocol_date") by
                                             funds_premsies_assoc_row.Field<int>("id_premises") into gs
                                        select new
                                        {
                                            id_premises = gs.Key,
                                            max_date = gs.Max()
                                        };
            var not_deleted_document = from fund_history_row in funds_history.AsEnumerable()
                                       join funds_premsies_assoc_row in funds_premsies_assoc.AsEnumerable()
                                       on fund_history_row.Field<int>("id_fund") equals funds_premsies_assoc_row.Field<int>("id_fund")
                                       select new
                                       {
                                           id_premises = funds_premsies_assoc_row.Field<int>("id_premises"),
                                           id_fund = fund_history_row.Field<int>("id_fund"),
                                           id_fund_type = fund_history_row.Field<int>("id_fund_type"),
                                           protocol_num = fund_history_row.Field<string>("protocol_num"),
                                           protocol_date = fund_history_row.Field<DateTime>("protocol_date"),
                                       };
            var result = from mdp_row in max_date_by_premises
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
            DataTable premises_current_funds = new DataTable(tableName);
            premises_current_funds.Columns.Add("id_premises").DataType = typeof(int);
            premises_current_funds.Columns.Add("id_fund").DataType = typeof(int);
            result.ToList().ForEach((x) =>
            {
                premises_current_funds.Rows.Add(new object[] { 
                    x.id_premises, 
                    x.id_fund });
            });
            premises_current_funds.PrimaryKey = new DataColumn[] { premises_current_funds.Columns["id_premises"] };
            return premises_current_funds;
        }
    }
}
