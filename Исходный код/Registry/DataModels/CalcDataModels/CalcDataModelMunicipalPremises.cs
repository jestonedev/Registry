using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.DataModels.DataModels;
using Registry.Entities;
using System.Data;
using System.Globalization;

namespace Registry.DataModels.CalcDataModels
{
    class CalcDataModelMunicipalPremises : CalcDataModel
    {
        private static CalcDataModelMunicipalPremises _dataModel;        

        private const string TableName = "municipal_premises_current_funds";

        private CalcDataModelMunicipalPremises()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(DataModel.GetInstance<FundsHistoryDataModel>().Select());
            RefreshOnTableModify(DataModel.GetInstance<FundsPremisesAssocDataModel>().Select());
            RefreshOnTableModify(DataModel.GetInstance<FundsSubPremisesAssocDataModel>().Select());
            RefreshOnTableModify(DataModel.GetInstance<PremisesDataModel>().Select());
            RefreshOnTableModify(DataModel.GetInstance<SubPremisesDataModel>().Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) { Locale = CultureInfo.InvariantCulture };
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("id_premises").DataType = typeof(int);
            table.Columns.Add("id_sub_premises").DataType = typeof(int);
            table.Columns.Add("id_fund_type").DataType = typeof(int);
            table.Columns.Add("id_fund").DataType = typeof(int);           
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelMunicipalPremises");            
            // Фильтруем удаленные строки
            var fundsHistory = DataModel.GetInstance<FundsHistoryDataModel>().FilterDeletedRows();
            var fundsPremisesAssoc = DataModel.GetInstance<FundsPremisesAssocDataModel>().FilterDeletedRows();
            var fundsSubPremisesAssoc = DataModel.GetInstance<FundsSubPremisesAssocDataModel>().FilterDeletedRows();
            var premises = DataModel.GetInstance<PremisesDataModel>().FilterDeletedRows();
            var subPremises = DataModel.GetInstance<SubPremisesDataModel>().FilterDeletedRows();
            var buildings = DataModel.GetInstance<BuildingsDataModel>().FilterDeletedRows();

            // Вычисляем агрегационную информацию
            var maxIdByPremises = DataModelHelper.MaxFundIDsByObject(fundsPremisesAssoc, EntityType.Premise);
            var maxIdBySubPremises = DataModelHelper.MaxFundIDsByObject(fundsSubPremisesAssoc, EntityType.SubPremise);
            var premisesDataSet = (from buildingRow in buildings
                                   join premisesRow in premises
                                   on buildingRow.Field<int>("id_building") equals premisesRow.Field<int>("id_building")
                                   join maxIdByPremisesRow in maxIdByPremises
                                   on premisesRow.Field<int>("id_premises") equals maxIdByPremisesRow.IdObject
                                   join fundHistoryRow in fundsHistory
                                   on maxIdByPremisesRow.IdFund equals fundHistoryRow.Field<int>("id_fund")
                                   where (new object[] { 4, 5, 9 }).Contains(premisesRow.Field<int>("id_state"))
                                   select new MunicipalPremises
                                   {
                                       id_building = premisesRow.Field<int>("id_building"),
                                       id_premises = maxIdByPremisesRow.IdObject,
                                       id_sub_premises = 0,
                                       id_fund = maxIdByPremisesRow.IdFund,
                                       id_fund_type = fundHistoryRow.Field<int>("id_fund_type")
                                   }).ToList();
            var subPremisesDataSet = (from buildingRow in buildings
                                      join premisesRow in premises
                                      on buildingRow.Field<int>("id_building") equals premisesRow.Field<int>("id_building")
                                      join spRow in subPremises
                                      on premisesRow.Field<int>("id_premises") equals spRow.Field<int>("id_premises")
                                      join maxIdBySubPremisesRow in maxIdBySubPremises
                                      on spRow.Field<int>("id_sub_premises") equals maxIdBySubPremisesRow.IdObject
                                      join fundHistoryRow in fundsHistory
                                      on maxIdBySubPremisesRow.IdFund equals fundHistoryRow.Field<int>("id_fund")
                                      where (new object[] { 4, 5, 9 }).Contains(spRow.Field<int>("id_state")) &&
                                        (new object[] { 4, 5, 9 }).Contains(premisesRow.Field<int>("id_state"))
                                      select new MunicipalPremises
                                      {
                                          id_building = premisesRow.Field<int>("id_building"),
                                          id_premises = premisesRow.Field<int>("id_premises"),
                                          id_sub_premises = maxIdBySubPremisesRow.IdObject,
                                          id_fund = maxIdBySubPremisesRow.IdFund,
                                          id_fund_type = fundHistoryRow.Field<int>("id_fund_type")
                                      }).ToList();
            //var idsSubPr = subPremisesDataSet.Select(sp => sp.id_premises);
            var municipalPrWithoutInnerSubPr = premisesDataSet.Where(p => !subPremisesDataSet.Select(sp => sp.id_premises).Contains(p.id_premises));
            //var municipalPrWithoutInnerSubPr = premisesDataSet.Except(subPremisesDataSet);
            var allMunicipal = municipalPrWithoutInnerSubPr.Union(subPremisesDataSet);
            var table = InitializeTable();
            table.BeginLoadData();
            allMunicipal.ToList().ForEach(x =>
                {
                    table.Rows.Add(x.id_building, x.id_premises, x.id_sub_premises, x.id_fund_type, x.id_fund);
                });
            table.EndLoadData();
            e.Result = table;
        }
        public static CalcDataModelMunicipalPremises GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelMunicipalPremises());
        }
    }

    class MunicipalPremises
    {
        public int id_building { get; set; }
        public int? id_premises { get; set; }
        public int? id_sub_premises { get; set; }        
        public int id_fund_type { get; set; }
        public int? id_fund { get; set; }

    }
}
