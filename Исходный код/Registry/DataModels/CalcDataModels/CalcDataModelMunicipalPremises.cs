using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.DataModels.DataModels;
using Registry.Entities;
using System.Data;
using System.Globalization;
using Registry.DataModels.Services;

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
            RefreshOnTableModify(DataModel.GetInstance<EntityDataModel<FundHistory>>().Select());
            RefreshOnTableModify(EntityDataModel<FundPremisesAssoc>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<FundSubPremisesAssoc>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<Premise>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<SubPremise>.GetInstance().Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) { Locale = CultureInfo.InvariantCulture };
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("id_premises").DataType = typeof(int);
            table.Columns.Add("id_sub_premises").DataType = typeof(int);
            table.Columns.Add("id_fund_type").DataType = typeof(int);
            table.Columns.Add("id_fund").DataType = typeof(int);
            table.Columns.Add("total_area").DataType = typeof(double);
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelMunicipalPremises");            
            // Фильтруем удаленные строки
            var fundsHistory = DataModel.GetInstance<EntityDataModel<FundHistory>>().FilterDeletedRows().ToList();
            var fundsPremisesAssoc = EntityDataModel<FundPremisesAssoc>.GetInstance().FilterDeletedRows().ToList();
            var fundsSubPremisesAssoc = EntityDataModel<FundSubPremisesAssoc>.GetInstance().FilterDeletedRows().ToList();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows().ToList();

            // Вычисляем агрегационную информацию
            var fundInfoPremises =
                (from fundRow in OtherService.MaxFundIDsByPremisesId(fundsPremisesAssoc)
                join fundsHistoryRow in fundsHistory
                    on fundRow.IdFund equals fundsHistoryRow.Field<int?>("id_fund")
                select new
                {
                    id_premises = fundRow.IdPremises,
                    id_fund = fundRow.IdFund,
                    id_fund_type = fundsHistoryRow.Field<int?>("id_fund_type")
                }).ToList();
            var fundInfoSubPremises =
                 (from fundRow in OtherService.MaxFundIDsBySubPremiseId(fundsSubPremisesAssoc)
                 join fundsHistoryRow in fundsHistory
                     on fundRow.IdFund equals fundsHistoryRow.Field<int?>("id_fund")
                 select new
                 {
                     id_sub_premises = fundRow.IdSubPremises,
                     id_fund = fundRow.IdFund,
                     id_fund_type = fundsHistoryRow.Field<int?>("id_fund_type")
                 }).ToList();

            var premisesDataSet = (from premisesRow in premises
                join fundInfoRow in fundInfoPremises
                    on premisesRow.Field<int>("id_premises") equals fundInfoRow.id_premises into fi
                from fiRow in fi.DefaultIfEmpty()
                where new object[] {4, 5, 9}.Contains(premisesRow.Field<int>("id_state"))
                select new MunicipalPremises
                {
                    id_building = premisesRow.Field<int>("id_building"),
                    id_premises = premisesRow.Field<int>("id_premises"),
                    id_sub_premises = 0,
                    id_fund = fiRow == null ? null : fiRow.id_fund,
                    id_fund_type = premisesRow.Field<int>("id_state") == 5 || fiRow == null
                        ? 0
                        : fiRow.id_fund_type.Value,
                    total_area = premisesRow.Field<double>("total_area")
                }).ToList();

            // выбираем муниц. комнаты, принадлежащие какому-либо фонду, а также имеющие состояния: 4, 5 или 9
            var subPremisesDataSet = (from premisesRow in premises
                join spRow in subPremises
                    on premisesRow.Field<int>("id_premises") equals spRow.Field<int>("id_premises")
                join fundInfoRow in fundInfoSubPremises
                    on spRow.Field<int>("id_sub_premises") equals fundInfoRow.id_sub_premises into fi
                from fiRow in fi.DefaultIfEmpty()
                join fundInfoPremiseRow in fundInfoPremises
                    on premisesRow.Field<int>("id_premises") equals fundInfoPremiseRow.id_premises into fip
                from fipRow in fip.DefaultIfEmpty()
                where new object[] {4, 5, 9}.Contains(spRow.Field<int>("id_state"))
                select new MunicipalPremises
                {
                    id_building = premisesRow.Field<int>("id_building"),
                    id_premises = premisesRow.Field<int>("id_premises"),
                    id_sub_premises = spRow.Field<int>("id_sub_premises"),
                    id_fund = fiRow == null ? null : fiRow.id_fund,
                    id_fund_type = spRow.Field<int>("id_state") == 5 || (fiRow == null && fipRow == null)
                        ? 0
                        : (fiRow == null ? fipRow.id_fund_type.Value : fiRow.id_fund_type.Value),
                    total_area = spRow.Field<double>("total_area")
                }).ToList();

            var allMunicipal =
                subPremisesDataSet.Union(
                    premisesDataSet.Where(p => subPremisesDataSet.All(sp => sp.id_premises != p.id_premises)));
                   
            var table = InitializeTable();
            table.BeginLoadData();
            allMunicipal.ToList().ForEach(x =>
                {
                    table.Rows.Add(x.id_building, x.id_premises, x.id_sub_premises, x.id_fund_type, x.id_fund, x.total_area);
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
        public double total_area { get; set; }

    }
}
