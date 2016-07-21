using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.CalcDataModels
{

    internal sealed class CalcDataModelBuildingsPremisesSumArea : CalcDataModel
    {
        private static CalcDataModelBuildingsPremisesSumArea _dataModel;

        private const string TableName = "buildings_premises_sum_area";

        private CalcDataModelBuildingsPremisesSumArea()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(DataModel.GetInstance<PremisesDataModel>().Select());
            RefreshOnTableModify(DataModel.GetInstance<SubPremisesDataModel>().Select());
            RefreshOnTableModify(DataModel.GetInstance<RestrictionsBuildingsAssocDataModel>().Select());
            RefreshOnTableModify(DataModel.GetInstance<RestrictionsPremisesAssocDataModel>().Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("sum_area").DataType = typeof(double);
            table.Columns.Add("mun_premises_count").DataType = typeof(int);            
            table.PrimaryKey = new [] { table.Columns["id_building"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelBuildingsPremisesSumArea");
            // Фильтруем удаленные строки
            var premises = DataModel.GetInstance<PremisesDataModel>().FilterDeletedRows();
            var subPremises = DataModel.GetInstance<SubPremisesDataModel>().FilterDeletedRows();
            var restrictionsBuildignsAssoc = DataModel.GetInstance<RestrictionsBuildingsAssocDataModel>().FilterDeletedRows();
            var restrictionsPremisesAssoc = DataModel.GetInstance<RestrictionsPremisesAssocDataModel>().FilterDeletedRows(); 
            
            // Вычисляем агрегационную информацию
            var subPremisesSumArea = from premisesRow in premises
                                        join subPremisesRow in subPremises
                                        on premisesRow.Field<int>("id_premises") equals subPremisesRow.Field<int>("id_premises")
                                        where DataModelHelper.MunicipalObjectStates().Contains(subPremisesRow.Field<int>("id_state"))
                                        group subPremisesRow.Field<double>("total_area") by premisesRow.Field<int>("id_premises") into gs
                                        select new
                                        {
                                            id_premises = gs.Key,
                                            sum_area = gs.Sum()
                                        };
            // Определяем снесеные здания и снесеные (на всякий случай) квартиры
            var demolishedBuildings = DataModelHelper.DemolishedBuildingIDs();
            var demolishedPremises = DataModelHelper.DemolishedPremisesIDs();
            // Определяем исключенные здания и квартиры из муниципальной собственности
            var buildingsExcludeFromMunicipal = DataModelHelper.ObjectIDsExcludedFromMunicipal(restrictionsBuildignsAssoc, EntityType.Building);
            var buildingsIncludedIntoMunicipal = DataModelHelper.ObjectIDsIncludedIntoMunicipal(restrictionsBuildignsAssoc, EntityType.Building);
            var premisesExcludeFromMunicipal = DataModelHelper.ObjectIDsExcludedFromMunicipal(restrictionsPremisesAssoc, EntityType.Premise);
            var premisesIncludedIntoMunicipal = DataModelHelper.ObjectIDsIncludedIntoMunicipal(restrictionsPremisesAssoc, EntityType.Premise);
            // Возвращаем сумму площадей только муниципальных помещений
            var result = from premisesRow in premises
                         join subPremisesSumAreaRow in subPremisesSumArea
                         on premisesRow.Field<int>("id_premises") equals subPremisesSumAreaRow.id_premises into spsar
                         from spsarRow in spsar.DefaultIfEmpty()
                         join demolishedBuildingsId in demolishedBuildings
                         on premisesRow.Field<int>("id_building") equals demolishedBuildingsId into dbr
                         from dbrRow in dbr.DefaultIfEmpty()
                         join demolishedPremisesId in demolishedPremises
                         on premisesRow.Field<int>("id_premises") equals demolishedPremisesId into dpr
                         from dprRow in dpr.DefaultIfEmpty()
                         join buildingsExcludeFromMunicipalRow in buildingsExcludeFromMunicipal
                         on premisesRow.Field<int>("id_building") equals buildingsExcludeFromMunicipalRow.IdObject into befmr
                         from befmrRow in befmr.DefaultIfEmpty()
                         join premisesExcludeFromMunicipalRow in premisesExcludeFromMunicipal
                         on premisesRow.Field<int>("id_premises") equals premisesExcludeFromMunicipalRow.IdObject into pefmr
                         from pefmrRow in pefmr.DefaultIfEmpty()
                         join buildingsIncludedIntoMunicipalRow in buildingsIncludedIntoMunicipal
                         on premisesRow.Field<int>("id_building") equals buildingsIncludedIntoMunicipalRow.IdObject into biimr
                         from biimrRow in biimr.DefaultIfEmpty()
                         join premisesIncludedIntoMunicipalRow in premisesIncludedIntoMunicipal
                         on premisesRow.Field<int>("id_premises") equals premisesIncludedIntoMunicipalRow.IdObject into piimr
                         from piimrRow in piimr.DefaultIfEmpty()
                         where dbrRow == 0 && dprRow == 0 &&
                               (befmrRow == null || (piimrRow != null && befmrRow.Date <= piimrRow.Date)) && 
                               (pefmrRow == null || (biimrRow != null && pefmrRow.Date <= biimrRow.Date))
                         group DataModelHelper.MunicipalObjectStates().Contains(premisesRow.Field<int>("id_state")) ? 
                                premisesRow.Field<double>("total_area") :
                                premisesRow.Field<int>("id_state") == 1 ?
                                (spsarRow == null ? 0 : spsarRow.sum_area) : 0 
                                by premisesRow.Field<int>("id_building") into gs
                         select new
                         {
                             id_building = gs.Key,
                             sum_area = gs.Sum(),
                             mun_premises_count = gs.Count()
                         };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_building, x.sum_area, x.mun_premises_count);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelBuildingsPremisesSumArea GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelBuildingsPremisesSumArea());
        }
    }
}
