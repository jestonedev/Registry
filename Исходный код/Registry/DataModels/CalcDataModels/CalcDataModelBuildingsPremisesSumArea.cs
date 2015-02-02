using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.DataModels;
using System.Data;
using System.Globalization;
using Registry.Entities;

namespace Registry.CalcDataModels
{

    public sealed class CalcDataModelBuildingsPremisesSumArea : CalcDataModel
    {
        private static CalcDataModelBuildingsPremisesSumArea dataModel = null;

        private static string tableName = "buildings_premises_sum_area";

        private CalcDataModelBuildingsPremisesSumArea()
            : base()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("sum_area").DataType = typeof(double);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_building"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelBuildingsPremisesSumArea");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
            var restrictions_buildigns_assoc = DataModelHelper.FilterRows(RestrictionsBuildingsAssocDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            var restrictions_premises_assoc = DataModelHelper.FilterRows(RestrictionsPremisesAssocDataModel.GetInstance().Select());
            // Вычисляем агрегационную информацию
            var sub_premises_sum_area = from premises_row in premises
                                        join sub_premises_row in sub_premises
                                        on premises_row.Field<int>("id_premises") equals sub_premises_row.Field<int>("id_premises")
                                        where new int[] { 4, 5 }.Contains(sub_premises_row.Field<int>("id_state"))
                                        group sub_premises_row.Field<double>("total_area") by premises_row.Field<int>("id_premises") into gs
                                        select new
                                        {
                                            id_premises = gs.Key,
                                            sum_area = gs.Sum()
                                        };
            // Определяем снесеные здания и снесеные (на всякий случай) квартиры
            var demolished_buildings = DataModelHelper.DemolishedBuildingIDs();
            var demolished_premises = DataModelHelper.DemolishedPremisesIDs();
            // Определяем исключенные здания и квартиры из муниципальной собственности
            var buildings_exclude_from_municipal = DataModelHelper.ObjectIDsExcludedFromMunicipal(restrictions_buildigns_assoc, EntityType.Building);
            var buildings_included_into_municipal = DataModelHelper.ObjectIDsIncludedIntoMunicipal(restrictions_buildigns_assoc, EntityType.Building);
            var premises_exclude_from_municipal = DataModelHelper.ObjectIDsExcludedFromMunicipal(restrictions_premises_assoc, EntityType.Premise);
            var premises_included_into_municipal = DataModelHelper.ObjectIDsIncludedIntoMunicipal(restrictions_premises_assoc, EntityType.Premise);
            // Возвращаем сумму площадей только муниципальных помещений
            var result = from premises_row in premises
                         join sub_premises_sum_area_row in sub_premises_sum_area
                         on premises_row.Field<int>("id_premises") equals sub_premises_sum_area_row.id_premises into spsar
                         from spsar_row in spsar.DefaultIfEmpty()
                         join demolished_buildings_id in demolished_buildings
                         on premises_row.Field<int>("id_building") equals demolished_buildings_id into dbr
                         from dbr_row in dbr.DefaultIfEmpty()
                         join demolished_premises_id in demolished_premises
                         on premises_row.Field<int>("id_premises") equals demolished_premises_id into dpr
                         from dpr_row in dpr.DefaultIfEmpty()
                         join buildings_exclude_from_municipal_row in buildings_exclude_from_municipal
                         on premises_row.Field<int>("id_building") equals buildings_exclude_from_municipal_row.IdObject into befmr
                         from befmr_row in befmr.DefaultIfEmpty()
                         join premises_exclude_from_municipal_row in premises_exclude_from_municipal
                         on premises_row.Field<int>("id_premises") equals premises_exclude_from_municipal_row.IdObject into pefmr
                         from pefmr_row in pefmr.DefaultIfEmpty()
                         join buildings_included_into_municipal_row in buildings_included_into_municipal
                         on premises_row.Field<int>("id_building") equals buildings_included_into_municipal_row.IdObject into biimr
                         from biimr_row in biimr.DefaultIfEmpty()
                         join premises_included_into_municipal_row in premises_included_into_municipal
                         on premises_row.Field<int>("id_premises") equals premises_included_into_municipal_row.IdObject into piimr
                         from piimr_row in piimr.DefaultIfEmpty()
                         where dbr_row == 0 && dpr_row == 0 &&
                               (befmr_row == null || (piimr_row != null && befmr_row.date <= piimr_row.date)) && 
                               (pefmr_row == null || (biimr_row != null && pefmr_row.date <= biimr_row.date))
                         group new int[] { 4, 5 }.Contains(premises_row.Field<int>("id_state")) ? 
                                premises_row.Field<double>("total_area") :
                                premises_row.Field<int>("id_state") == 1 ?
                                (spsar_row == null ? 0 : spsar_row.sum_area) : 0 
                                by premises_row.Field<int>("id_building") into gs
                         select new
                         {
                             id_building = gs.Key,
                             sum_area = gs.Sum()
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_building, 
                    x.sum_area });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelBuildingsPremisesSumArea GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelBuildingsPremisesSumArea();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
