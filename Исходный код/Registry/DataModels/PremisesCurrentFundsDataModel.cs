using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Registry.DataModels
{
    public sealed class PremisesCurrentFundsDataModel : DataModel
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
            DataTable funds_premises_assoc = FundsPremisesAssocDataModel.GetInstance().Select();
            var max_id_by_premises = from funds_premises_assoc_row in funds_premises_assoc.AsEnumerable()
                                     group funds_premises_assoc_row.Field<int>("id_fund") by
                                             funds_premises_assoc_row.Field<int>("id_premises") into gs
                                     select new
                                     {
                                         id_premises = gs.Key,
                                         id_fund = gs.Max()
                                     };
            var result = from fund_history_row in funds_history.AsEnumerable()
                                    join max_id_by_premises_row in max_id_by_premises
                                       on fund_history_row.Field<int>("id_fund") equals max_id_by_premises_row.id_fund
                                    select new
                                    {
                                        id_premises = max_id_by_premises_row.id_premises,
                                        id_fund = max_id_by_premises_row.id_fund,
                                        id_fund_type = fund_history_row.Field<int>("id_fund_type"),
                                        protocol_number = fund_history_row.Field<string>("protocol_number"),
                                        protocol_date = fund_history_row.Field<DateTime?>("protocol_date"),
                                    };
            DataTable premises_current_funds = new DataTable(tableName);
            premises_current_funds.Columns.Add("id_premises").DataType = typeof(int);
            premises_current_funds.Columns.Add("id_fund_type").DataType = typeof(int);
            result.ToList().ForEach((x) =>
            {
                premises_current_funds.Rows.Add(new object[] { 
                    x.id_premises, 
                    x.id_fund_type });
            });
            premises_current_funds.PrimaryKey = new DataColumn[] { premises_current_funds.Columns["id_premises"] };
            return premises_current_funds;
        }
    }
}
