﻿using System;
using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;

namespace Registry.DataModels.CalcDataModels
{
    internal sealed class CalcDataModelSubPremisesCurrentFunds : CalcDataModel
    {
        private static CalcDataModelSubPremisesCurrentFunds _dataModel;

        private const string TableName = "sub_premises_current_funds";

        private CalcDataModelSubPremisesCurrentFunds()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(EntityDataModel<FundHistory>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<FundSubPremisesAssoc>.GetInstance().Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_sub_premises").DataType = typeof(int);
            table.Columns.Add("id_fund_type").DataType = typeof(int);
            table.PrimaryKey = new [] { table.Columns["id_sub_premises"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelSubPremisesCurrentFunds");
            // Фильтруем удаленные строки
            var fundsHistory = DataModel.GetInstance<EntityDataModel<FundHistory>>().FilterDeletedRows();
            var fundsSubPremisesAssoc = EntityDataModel<FundSubPremisesAssoc>.GetInstance().FilterDeletedRows();

            // Вычисляем агрегационную информацию
            var maxIdBySubPremises = OtherService.MaxFundIDsBySubPremiseId(fundsSubPremisesAssoc); 
            var result = from fundHistoryRow in fundsHistory
                         join maxIdBySubPremisesRow in maxIdBySubPremises
                            on fundHistoryRow.Field<int>("id_fund") equals maxIdBySubPremisesRow.IdFund
                         select new
                         {
                             id_sub_premises = maxIdBySubPremisesRow.IdSubPremises,
                             id_fund = maxIdBySubPremisesRow.IdFund,
                             id_fund_type = fundHistoryRow.Field<int>("id_fund_type"),
                             protocol_number = fundHistoryRow.Field<string>("protocol_number"),
                             protocol_date = fundHistoryRow.Field<DateTime?>("protocol_date"),
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_sub_premises, x.id_fund_type);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelSubPremisesCurrentFunds GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelSubPremisesCurrentFunds());
        }
    }
}
