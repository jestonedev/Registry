using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.DataModels;
using System.Globalization;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelSubPremisesCurrentFunds : CalcDataModel
    {
        private static CalcDataModelSubPremisesCurrentFunds dataModel = null;

        private static string tableName = "sub_premises_current_funds";

        private CalcDataModelSubPremisesCurrentFunds()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("id_sub_premises").DataType = typeof(int);
            table.Columns.Add("id_fund_type").DataType = typeof(int);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_sub_premises"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelSubPremisesCurrentFunds");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var funds_history = DataModelHelper.FilterRows(FundsHistoryDataModel.GetInstance().Select());
            var funds_sub_premises_assoc = DataModelHelper.FilterRows(FundsSubPremisesAssocDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            // Вычисляем агрегационную информацию
            var max_id_by_sub_premises = DataModelHelper.MaxFundIDsByObject(funds_sub_premises_assoc, EntityType.SubPremise); 
            var result = from fund_history_row in funds_history
                         join max_id_by_sub_premises_row in max_id_by_sub_premises
                            on fund_history_row.Field<int>("id_fund") equals max_id_by_sub_premises_row.IdFund
                         select new
                         {
                             id_sub_premises = max_id_by_sub_premises_row.IdObject,
                             id_fund = max_id_by_sub_premises_row.IdFund,
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
                    x.id_sub_premises, 
                    x.id_fund_type });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelSubPremisesCurrentFunds GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelSubPremisesCurrentFunds();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
