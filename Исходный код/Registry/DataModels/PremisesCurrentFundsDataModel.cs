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

        public event EventHandler<EventArgs> RefreshEvent;

        private PremisesCurrentFundsDataModel()
        {
            table = new DataTable(tableName);
            table.Columns.Add("id_premises").DataType = typeof(int);
            table.Columns.Add("id_fund_type").DataType = typeof(int);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_premises"] };
            Refresh();
        }

        public void Refresh()
        {
            DataTable premises = PremisesDataModel.GetInstance().Select();
            DataTable funds_history = FundsHistoryDataModel.GetInstance().Select();
            DataTable funds_premises_assoc = FundsPremisesAssocDataModel.GetInstance().Select();
            var max_id_by_premises = from funds_premises_assoc_row in funds_premises_assoc.AsEnumerable()
                                     join fund_history_row in funds_history.AsEnumerable()
                                        on funds_premises_assoc_row.Field<int>("id_fund") equals fund_history_row.Field<int>("id_fund")
                                     where fund_history_row.Field<DateTime?>("exclude_restriction_date") == null
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
            table.Clear();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_premises, 
                    x.id_fund_type });
            });
            table.EndLoadData();
            if (RefreshEvent != null)
                RefreshEvent(this, new EventArgs());
        }
        
        public static PremisesCurrentFundsDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new PremisesCurrentFundsDataModel();
            return dataModel;
        }
    }
}
