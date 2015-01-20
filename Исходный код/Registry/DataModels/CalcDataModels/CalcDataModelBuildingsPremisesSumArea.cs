using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.DataModels;
using System.Data;
using System.Globalization;

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
            Refresh(CalcDataModelFilterEnity.All, null);
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
            var buildings = from buildings_row in DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select())
                            where (config.Entity == CalcDataModelFilterEnity.Building ? buildings_row.Field<int>("id_building") == config.IdObject :
                                   config.Entity == CalcDataModelFilterEnity.All ? true : false)
                            select buildings_row;
            var premises = from premises_row in DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select())
                           where (config.Entity == CalcDataModelFilterEnity.Building ? premises_row.Field<int>("id_building") == config.IdObject :
                                  config.Entity == CalcDataModelFilterEnity.All ? true : false)
                           select premises_row;
            var sub_premises_sum_area = DataModelHelper.FilterRows(CalcDataModelPremiseSubPremisesSumArea.GetInstance().Select());
            var ownership_buildings_assoc = from ownership_buildigns_assoc_row in DataModelHelper.FilterRows(OwnershipBuildingsAssocDataModel.GetInstance().Select())
                                            where (config.Entity == CalcDataModelFilterEnity.Building ? 
                                                   ownership_buildigns_assoc_row.Field<int>("id_building") == config.IdObject :
                                                   config.Entity == CalcDataModelFilterEnity.All ? true : false)
                                            select ownership_buildigns_assoc_row;
            var ownership_premises_assoc = from premises_row in premises
                                           join ownership_premises_assoc_row in DataModelHelper.FilterRows(OwnershipPremisesAssocDataModel.GetInstance().Select())
                                           on premises_row.Field<int>("id_premises") equals ownership_premises_assoc_row.Field<int>("id_premises")
                                           where (config.Entity == CalcDataModelFilterEnity.Building ?
                                                  premises_row.Field<int>("id_building") == config.IdObject :
                                                  config.Entity == CalcDataModelFilterEnity.All ? true : false)
                                           select ownership_premises_assoc_row;
            var restrictions_buildigns_assoc = from restrictions_buildigns_assoc_row in DataModelHelper.FilterRows(RestrictionsBuildingsAssocDataModel.GetInstance().Select())
                                               where (config.Entity == CalcDataModelFilterEnity.Building ?
                                                      restrictions_buildigns_assoc_row.Field<int>("id_building") == config.IdObject :
                                                      config.Entity == CalcDataModelFilterEnity.All ? true : false)
                                               select restrictions_buildigns_assoc_row;
            var restrictions_premises_assoc = from premises_row in premises
                                              join restrictions_premises_assoc_row in DataModelHelper.FilterRows(RestrictionsPremisesAssocDataModel.GetInstance().Select())
                                              on premises_row.Field<int>("id_premises") equals restrictions_premises_assoc_row.Field<int>("id_premises")
                                              where (config.Entity == CalcDataModelFilterEnity.Building ?
                                                     premises_row.Field<int>("id_building") == config.IdObject :
                                                     config.Entity == CalcDataModelFilterEnity.All ? true : false)
                                              select restrictions_premises_assoc_row;
            var ownership_rights = DataModelHelper.FilterRows(OwnershipsRightsDataModel.GetInstance().Select());
            var restrictions = DataModelHelper.FilterRows(RestrictionsDataModel.GetInstance().Select());
            // Вычисляем агрегационную информацию
            // Определяем снесеные здания и снесеные (на всякий случай) квартиры
            var demolished_buildings = from ownership_buildings_assoc_row in ownership_buildings_assoc
                                       join ownership_rights_row in ownership_rights
                                       on ownership_buildings_assoc_row.Field<int>("id_ownership_right") equals ownership_rights_row.Field<int>("id_ownership_right")
                                       where ownership_rights_row.Field<int>("id_ownership_right_type") == 1
                                       select ownership_buildings_assoc_row;
            var demolished_premises = from ownership_premises_assoc_row in ownership_premises_assoc
                                      join ownership_rights_row in ownership_rights
                                      on ownership_premises_assoc_row.Field<int>("id_ownership_right") equals ownership_rights_row.Field<int>("id_ownership_right")
                                      where ownership_rights_row.Field<int>("id_ownership_right_type") == 1
                                      select ownership_premises_assoc_row;
            // Определяем исключенные здания и квартиры из муниципальной собственности
            var building_restrictions_max_date = from restrictions_buildings_assoc_row in restrictions_buildigns_assoc
                                                 join restrictions_row in restrictions
                                                 on restrictions_buildings_assoc_row.Field<int>("id_restriction") equals restrictions_row.Field<int>("id_restriction")
                                                 where new int[] { 1, 2 }.Contains(restrictions_row.Field<int>("id_restriction_type"))
                                                 group restrictions_row.Field<DateTime>("date") by restrictions_buildings_assoc_row.Field<int>("id_building") into gs
                                                 select new
                                                 {
                                                     id_building = gs.Key,
                                                     date = gs.Max()
                                                 };
            var premises_restrictions_max_date = from restrictions_premises_assoc_row in restrictions_premises_assoc
                                                 join restrictions_row in restrictions
                                                 on restrictions_premises_assoc_row.Field<int>("id_restriction") equals restrictions_row.Field<int>("id_restriction")
                                                 where new int[] { 1, 2 }.Contains(restrictions_row.Field<int>("id_restriction_type"))
                                                 group restrictions_row.Field<DateTime>("date") by restrictions_premises_assoc_row.Field<int>("id_premises") into gs
                                                 select new
                                                 {
                                                     id_premises = gs.Key,
                                                     date = gs.Max()
                                                 };
            var buildings_exclude_from_municipal = from restrictions_row in restrictions
                                                   join restrictions_buildings_assoc_row in restrictions_buildigns_assoc
                                                   on restrictions_row.Field<int>("id_restriction")
                                                   equals restrictions_buildings_assoc_row.Field<int>("id_restriction")
                                                   join building_rmd_row in building_restrictions_max_date
                                                   on new
                                                   {
                                                       id_building = restrictions_buildings_assoc_row.Field<int>("id_building"),
                                                       date = restrictions_row.Field<DateTime>("date")
                                                   } equals
                                                   new
                                                   {
                                                       id_building = building_rmd_row.id_building,
                                                       date = building_rmd_row.date
                                                   }
                                                   where restrictions_row.Field<int>("id_restriction_type") == 2
                                                   select restrictions_buildings_assoc_row;
            var premises_exclude_from_municipal = from restrictions_row in restrictions
                                                  join restrictions_premises_assoc_row in restrictions_premises_assoc
                                                  on restrictions_row.Field<int>("id_restriction")
                                                  equals restrictions_premises_assoc_row.Field<int>("id_restriction")
                                                  join premises_rmd_row in premises_restrictions_max_date
                                                  on new
                                                  {
                                                      id_premises = restrictions_premises_assoc_row.Field<int>("id_premises"),
                                                      date = restrictions_row.Field<DateTime>("date")
                                                  } equals
                                                  new
                                                  {
                                                      id_premises = premises_rmd_row.id_premises,
                                                      date = premises_rmd_row.date
                                                  }
                                                  where restrictions_row.Field<int>("id_restriction_type") == 2
                                                  select restrictions_premises_assoc_row;
            // Возвращаем сумму площадей только муниципальных помещений
            var result = from premises_row in premises
                         join sub_premises_sum_area_row in sub_premises_sum_area
                         on premises_row.Field<int>("id_premises") equals sub_premises_sum_area_row.Field<int>("id_premises") into spsar
                         from spsar_row in spsar.DefaultIfEmpty()
                         join demolished_buildings_row in demolished_buildings
                         on premises_row.Field<int>("id_building") equals demolished_buildings_row.Field<int>("id_building") into dbr
                         from dbr_row in dbr.DefaultIfEmpty()
                         join demolished_premises_row in demolished_premises
                         on premises_row.Field<int>("id_premises") equals demolished_premises_row.Field<int>("id_premises") into dpr
                         from dpr_row in dpr.DefaultIfEmpty()
                         join buildings_exclude_from_municipal_row in buildings_exclude_from_municipal
                         on premises_row.Field<int>("id_building") equals buildings_exclude_from_municipal_row.Field<int>("id_building") into befmr
                         from befmr_row in befmr.DefaultIfEmpty()
                         join premises_exclude_from_municipal_row in premises_exclude_from_municipal
                         on premises_row.Field<int>("id_premises") equals premises_exclude_from_municipal_row.Field<int>("id_premises") into pefmr
                         from pefmr_row in pefmr.DefaultIfEmpty()
                         where dbr_row == null && dpr_row == null && (befmr_row == null || premises_row.Field<int>("id_state") == 4) && pefmr_row == null
                         group new int[] { 4, 5 }.Contains(premises_row.Field<int>("id_state")) ? 
                                premises_row.Field<double>("total_area") :
                                premises_row.Field<int>("id_state") == 1 ?
                                (spsar_row == null ? 0 : spsar_row.Field<double>("sum_area")) : 0 
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
