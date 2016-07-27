using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.CalcDataModels
{
    internal sealed class CalcDataModelBuildingsPremisesFunds : CalcDataModel
    {
        private static CalcDataModelBuildingsPremisesFunds _dataModel;

        private const string TableName = "buildings_premises_funds";

        private CalcDataModelBuildingsPremisesFunds()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(DataModel.GetInstance<EntityDataModel<Building>>().Select());
            RefreshOnTableModify(EntityDataModel<Premise>.GetInstance().Select());
            RefreshOnTableModify(DataModel.GetInstance<EntityDataModel<FundHistory>>().Select());
            RefreshOnTableModify(DataModel.GetInstance<FundsPremisesAssocDataModel>().Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("social_premises_count").DataType = typeof(int);
            table.Columns.Add("commercial_premises_count").DataType = typeof(int);
            table.Columns.Add("special_premises_count").DataType = typeof(int);
            table.Columns.Add("other_premises_count").DataType = typeof(int);
            table.PrimaryKey = new [] { table.Columns["id_building"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelBuildingsPremisesFunds");
            // Фильтруем удаленные строки
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows();
            var fundsHistory = DataModel.GetInstance<EntityDataModel<FundHistory>>().FilterDeletedRows();
            var fundsPremisesAssoc = DataModel.GetInstance<FundsPremisesAssocDataModel>().FilterDeletedRows();

            // Вычисляем агрегационную информацию
            var maxIdByPremises = DataModelHelper.MaxFundIDsByObject(fundsPremisesAssoc, EntityType.Premise);  
            var currentFunds = from fundHistoryRow in fundsHistory
                                    join maxIdByPremisesRow in maxIdByPremises
                                       on fundHistoryRow.Field<int>("id_fund") equals maxIdByPremisesRow.IdFund
                                    select new
                                    {
                                        id_premises = maxIdByPremisesRow.IdObject,
                                        id_fund = maxIdByPremisesRow.IdFund,
                                        id_fund_type = fundHistoryRow.Field<int>("id_fund_type"),
                                    };
            var socialPremises = from premisesRow in premises
                                  join aggRow in currentFunds
                                  on premisesRow.Field<int>("id_premises") equals aggRow.id_premises
                                  where aggRow.id_fund_type == 1
                                  group premisesRow.Field<int>("id_premises") by premisesRow.Field<int>("id_building") into gs
                                  select new
                                  {
                                      id_building = gs.Key,
                                      social_premises_count = gs.Count()
                                  };
            var commercialPremises = from premisesRow in premises
                                      join aggRow in currentFunds
                                      on premisesRow.Field<int>("id_premises") equals aggRow.id_premises
                                      where aggRow.id_fund_type == 2
                                      group premisesRow.Field<int>("id_premises") by premisesRow.Field<int>("id_building") into gs
                                      select new
                                      {
                                          id_building = gs.Key,
                                          commercial_premises_count = gs.Count()
                                      };
            var specialPremises = from premisesRow in premises
                                   join aggRow in currentFunds
                                   on premisesRow.Field<int>("id_premises") equals aggRow.id_premises
                                   where aggRow.id_fund_type == 3
                                   group premisesRow.Field<int>("id_premises") by premisesRow.Field<int>("id_building") into gs
                                   select new
                                   {
                                       id_building = gs.Key,
                                       special_premises_count = gs.Count()
                                   };
            var otherPremises = from premisesRow in premises
                                 join aggRow in currentFunds
                                 on premisesRow.Field<int>("id_premises") equals aggRow.id_premises
                                 where aggRow.id_fund_type == 4
                                 group premisesRow.Field<int>("id_premises") by premisesRow.Field<int>("id_building") into gs
                                 select new
                                 {
                                     id_building = gs.Key,
                                     other_premises_count = gs.Count()
                                 };
            var result = from buildingsRow in buildings
                         join spRow in socialPremises on buildingsRow.Field<int>("id_building") equals spRow.id_building into sp
                         from buildingsRowSp in sp.DefaultIfEmpty()
                         join cpRow in commercialPremises on buildingsRow.Field<int>("id_building") equals cpRow.id_building into cp
                         from buildingsRowCp in cp.DefaultIfEmpty()
                         join sppRow in specialPremises on buildingsRow.Field<int>("id_building") equals sppRow.id_building into spp
                         from buildingsRowSpp in spp.DefaultIfEmpty()
                         join oRow in otherPremises on buildingsRow.Field<int>("id_building") equals oRow.id_building into opp
                         from buildingsRowOpp in opp.DefaultIfEmpty()
                         select new
                         {
                             id_building = buildingsRow.Field<int>("id_building"),
                             social_premises_count = (buildingsRowSp == null) ? 0 : buildingsRowSp.social_premises_count,
                             commercial_premises_count = (buildingsRowCp == null) ? 0 : buildingsRowCp.commercial_premises_count,
                             special_premises_count = (buildingsRowSpp == null) ? 0 : buildingsRowSpp.special_premises_count,
                             other_premises_count = (buildingsRowOpp == null) ? 0 : buildingsRowOpp.other_premises_count
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_building, x.social_premises_count, x.commercial_premises_count, 
                    x.special_premises_count, x.other_premises_count);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelBuildingsPremisesFunds GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelBuildingsPremisesFunds());
        }
    }
}
