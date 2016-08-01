using System;
using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;

namespace Registry.DataModels.CalcDataModels
{
    internal sealed class CalcDataModelPremisesCurrentFunds : CalcDataModel
    {
        private static CalcDataModelPremisesCurrentFunds _dataModel;

        private const string TableName = "premises_current_funds";

        private CalcDataModelPremisesCurrentFunds()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(DataModel.GetInstance<EntityDataModel<FundHistory>>().Select());
            RefreshOnTableModify(EntityDataModel<FundPremisesAssoc>.GetInstance().Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_premises").DataType = typeof(int);
            table.Columns.Add("id_fund_type").DataType = typeof(int);
            table.PrimaryKey = new[] { table.Columns["id_premises"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelPremisesCurrentFunds");
            // Фильтруем удаленные строки
            var fundsHistory = EntityDataModel<FundHistory>.GetInstance().FilterDeletedRows();
            var fundsPremisesAssoc = EntityDataModel<FundPremisesAssoc>.GetInstance().FilterDeletedRows();
            
            // Вычисляем агрегационную информацию
            var maxIdByPremises = OtherService.MaxFundIDsByPremisesId(fundsPremisesAssoc); 
            var result = from fundHistoryRow in fundsHistory
                         join maxIdByPremisesRow in maxIdByPremises
                            on fundHistoryRow.Field<int>("id_fund") equals maxIdByPremisesRow.IdFund
                         select new
                         {
                             id_premises = maxIdByPremisesRow.IdPremises,
                             id_fund = maxIdByPremisesRow.IdFund,
                             id_fund_type = fundHistoryRow.Field<int>("id_fund_type"),
                             protocol_number = fundHistoryRow.Field<string>("protocol_number"),
                             protocol_date = fundHistoryRow.Field<DateTime?>("protocol_date"),
                         };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_premises, x.id_fund_type);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }
        
        public static CalcDataModelPremisesCurrentFunds GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelPremisesCurrentFunds());
        }
    }
}
