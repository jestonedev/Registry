using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.DataModels;
using System.Globalization;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelPremisesCurrentFunds : CalcDataModel
    {
        private static CalcDataModelPremisesCurrentFunds dataModel = null;

        private static string tableName = "premises_current_funds";

        private CalcDataModelPremisesCurrentFunds()
        {
            Table = InitializeTable();
            Refresh(CalcDataModelFilterEnity.All, null);
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("id_premises").DataType = typeof(int);
            table.Columns.Add("id_fund_type").DataType = typeof(int);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_premises"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelPremisesCurrentFunds");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var funds_history = DataModelHelper.FilterRows(FundsHistoryDataModel.GetInstance().Select());
            var funds_premises_assoc = from funds_premises_assoc_row in DataModelHelper.FilterRows(FundsPremisesAssocDataModel.GetInstance().Select())
                                       where (config.Entity == CalcDataModelFilterEnity.Premise ? funds_premises_assoc_row.Field<int>("id_premises") == config.IdObject :
                                               config.Entity == CalcDataModelFilterEnity.All ? true : false)
                                        select funds_premises_assoc_row;

            // Вычисляем агрегационную информацию
            var max_id_by_premises = from funds_premises_assoc_row in funds_premises_assoc
                                     join fund_history_row in funds_history
                                        on funds_premises_assoc_row.Field<int>("id_fund") equals fund_history_row.Field<int>("id_fund")
                                     where fund_history_row.Field<DateTime?>("exclude_restriction_date") == null
                                     group funds_premises_assoc_row.Field<int>("id_fund") by
                                             funds_premises_assoc_row.Field<int>("id_premises") into gs
                                     select new
                                     {
                                         id_premises = gs.Key,
                                         id_fund = gs.Max()
                                     };
            var result = from fund_history_row in funds_history
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
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_premises, 
                    x.id_fund_type });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }
        
        public static CalcDataModelPremisesCurrentFunds GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelPremisesCurrentFunds();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
