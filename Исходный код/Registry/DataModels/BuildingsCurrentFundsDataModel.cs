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

        public event EventHandler<EventArgs> RefreshEvent;

        private BuildingsCurrentFundsDataModel()
        {
            table = new DataTable(tableName);
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("id_fund_type").DataType = typeof(int);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_building"] };
            Refresh();
        }

        public void Refresh()
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
            table.Clear();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_building, 
                    x.id_fund_type });
            });
            table.EndLoadData();
            if (RefreshEvent != null)
                RefreshEvent(this, new EventArgs());
        }

        public static BuildingsCurrentFundsDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new BuildingsCurrentFundsDataModel();
            return dataModel;
        }
    }
}
