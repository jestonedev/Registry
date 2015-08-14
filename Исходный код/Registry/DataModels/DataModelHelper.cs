using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.CalcDataModels;
using System.Globalization;
using Registry.Entities;

namespace Registry.DataModels
{
    public static class DataModelHelper
    {
        public static IEnumerable<DataRow> FilterRows(DataTable table)
        {
            return from table_row in table.AsEnumerable()
                   where (table_row.RowState != DataRowState.Deleted) &&
                         (table_row.RowState != DataRowState.Detached)
                   select table_row;
        }

        public static IEnumerable<DataRow> FilterRows(DataTable table, EntityType entity, int? idObject)
        {
            return from row in FilterRows(table)
                   where entity == EntityType.Unknown ? true :
                         entity == EntityType.Building ? row.Field<int?>("id_building") == idObject :
                         entity == EntityType.Premise ? row.Field<int?>("id_premises") == idObject :
                         entity == EntityType.SubPremise ? row.Field<int?>("id_sub_premises") == idObject :
                         entity == EntityType.TenancyProcess ? row.Field<int?>("id_process") == idObject : 
                         entity == EntityType.ResettleProcess ? row.Field<int?>("id_process") == idObject : false
                   select row;
        }

        public static IEnumerable<int> Intersect(IEnumerable<int> list1, IEnumerable<int> list2)
        {
            if (list1 != null && list2 != null)
                return list1.Intersect(list2).ToList();
            else
                if (list1 != null)
                    return list1;
                else
                    return list2;
        }

        public static IEnumerable<int> BuildingIDsBySNP(string[] snp, Func<DataRow, bool> condition)
        {
            var tenancy_buildings_assoc = DataModelHelper.FilterRows(TenancyBuildingsAssocDataModel.GetInstance().Select());
            var tenancy_persons = DataModelHelper.FilterRows(TenancyPersonsDataModel.GetInstance().Select());
            return
            (from tenancy_buildings_assoc_row in tenancy_buildings_assoc
             join tenancy_persons_row in tenancy_persons
             on tenancy_buildings_assoc_row.Field<int>("id_process") equals tenancy_persons_row.Field<int>("id_process")
             where ((snp.Count() == 1) ? tenancy_persons_row.Field<string>("surname") != null &&
                                         tenancy_persons_row.Field<string>("surname").ToUpperInvariant() ==
                                            snp[0].ToUpperInvariant() :
                    (snp.Count() == 2) ? tenancy_persons_row.Field<string>("surname") != null &&
                                         tenancy_persons_row.Field<string>("surname").ToUpperInvariant() ==
                                            snp[0].ToUpperInvariant() &&
                                         tenancy_persons_row.Field<string>("name") != null &&
                                         tenancy_persons_row.Field<string>("name").ToUpperInvariant() ==
                                            snp[1].ToUpperInvariant() :
                    (snp.Count() == 3) ? tenancy_persons_row.Field<string>("surname") != null &&
                                         tenancy_persons_row.Field<string>("surname").ToUpperInvariant() ==
                                            snp[0].ToUpperInvariant() &&
                                         tenancy_persons_row.Field<string>("name") != null &&
                                         tenancy_persons_row.Field<string>("name").ToUpperInvariant() ==
                                            snp[1].ToUpperInvariant() &&
                                         tenancy_persons_row.Field<string>("patronymic") != null &&
                                         tenancy_persons_row.Field<string>("patronymic").ToUpperInvariant() ==
                                            snp[2].ToUpperInvariant() : false) && condition(tenancy_persons_row)
             select tenancy_buildings_assoc_row.Field<int>("id_building")).Distinct();
        }

        public static IEnumerable<int> PremisesIDsBySNP(string[] snp, Func<DataRow, bool> condition)
        {
            var tenancy_premises_assoc = DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select());
            var tenancy_persons = DataModelHelper.FilterRows(TenancyPersonsDataModel.GetInstance().Select());
            return
            (from tenancy_premises_assoc_row in tenancy_premises_assoc
             join tenancy_persons_row in tenancy_persons
             on tenancy_premises_assoc_row.Field<int>("id_process") equals tenancy_persons_row.Field<int>("id_process")
             where ((snp.Count() == 1) ? tenancy_persons_row.Field<string>("surname") != null &&
                                         tenancy_persons_row.Field<string>("surname").ToUpperInvariant() == 
                                            snp[0].ToUpperInvariant() :
                    (snp.Count() == 2) ? tenancy_persons_row.Field<string>("surname") != null && 
                                         tenancy_persons_row.Field<string>("surname").ToUpperInvariant() == 
                                            snp[0].ToUpperInvariant() &&
                                         tenancy_persons_row.Field<string>("name") != null &&
                                         tenancy_persons_row.Field<string>("name").ToUpperInvariant() == 
                                            snp[1].ToUpperInvariant() :
                    (snp.Count() == 3) ? tenancy_persons_row.Field<string>("surname") != null && 
                                         tenancy_persons_row.Field<string>("surname").ToUpperInvariant() == 
                                            snp[0].ToUpperInvariant() &&
                                         tenancy_persons_row.Field<string>("name") != null &&
                                         tenancy_persons_row.Field<string>("name").ToUpperInvariant() == 
                                            snp[1].ToUpperInvariant() &&
                                         tenancy_persons_row.Field<string>("patronymic") != null &&
                                         tenancy_persons_row.Field<string>("patronymic").ToUpperInvariant() == 
                                            snp[2].ToUpperInvariant() : false) && condition(tenancy_persons_row)
             select tenancy_premises_assoc_row.Field<int>("id_premises")).Distinct();
        }

        public static IEnumerable<int> TenancyProcessIDsBySNP(string[] snp, Func<DataRow, bool> condition)
        {
            var tenancy_persons = DataModelHelper.FilterRows(TenancyPersonsDataModel.GetInstance().Select());
            return
            (from tenancy_persons_row in tenancy_persons
             where ((snp.Count() == 1) ? tenancy_persons_row.Field<string>("surname") != null &&
                                         tenancy_persons_row.Field<string>("surname").ToUpperInvariant() == 
                                                snp[0].ToUpperInvariant() :
                    (snp.Count() == 2) ? tenancy_persons_row.Field<string>("surname") != null && 
                                         tenancy_persons_row.Field<string>("surname").ToUpperInvariant() ==
                                                snp[0].ToUpperInvariant() &&
                                         tenancy_persons_row.Field<string>("name") != null &&
                                         tenancy_persons_row.Field<string>("name").ToUpperInvariant() == 
                                                snp[1].ToUpperInvariant() :
                    (snp.Count() == 3) ? tenancy_persons_row.Field<string>("surname") != null &&
                                         tenancy_persons_row.Field<string>("surname").ToUpperInvariant() == 
                                                snp[0].ToUpperInvariant() &&
                                         tenancy_persons_row.Field<string>("name") != null &&
                                         tenancy_persons_row.Field<string>("name").ToUpperInvariant() == 
                                                snp[1].ToUpperInvariant() &&
                                         tenancy_persons_row.Field<string>("patronymic") != null &&
                                         tenancy_persons_row.Field<string>("patronymic").ToUpperInvariant() == 
                                                snp[2].ToUpperInvariant() : false) && condition(tenancy_persons_row)
             select tenancy_persons_row.Field<int>("id_process")).Distinct();
        }

        public static IEnumerable<int> ResettleProcessIDsBySNP(string[] snp)
        {
            var resettle_persons = DataModelHelper.FilterRows(ResettlePersonsDataModel.GetInstance().Select());
            return
            (from resettle_persons_row in resettle_persons
             where ((snp.Count() == 1) ? resettle_persons_row.Field<string>("surname") != null && 
                                         resettle_persons_row.Field<string>("surname").ToUpperInvariant() ==
                                                snp[0].ToUpperInvariant() :
                    (snp.Count() == 2) ? resettle_persons_row.Field<string>("surname") != null && 
                                         resettle_persons_row.Field<string>("surname").ToUpperInvariant() ==
                                                snp[0].ToUpperInvariant() &&
                                         resettle_persons_row.Field<string>("name") != null && 
                                         resettle_persons_row.Field<string>("name").ToUpperInvariant() ==
                                                snp[1].ToUpperInvariant() :
                    (snp.Count() == 3) ? resettle_persons_row.Field<string>("surname") != null && 
                                         resettle_persons_row.Field<string>("surname").ToUpperInvariant() ==
                                                snp[0].ToUpperInvariant() &&
                                         resettle_persons_row.Field<string>("name") != null && 
                                         resettle_persons_row.Field<string>("name").ToUpperInvariant() ==
                                                snp[1].ToUpperInvariant() &&
                                         resettle_persons_row.Field<string>("patronymic") != null && 
                                         resettle_persons_row.Field<string>("patronymic").ToUpperInvariant() ==
                                                snp[2].ToUpperInvariant() : false)
             select resettle_persons_row.Field<int>("id_process")).Distinct();
        }

        public static IEnumerable<int> BuildingIDsByAddress(string[] addressParts)
        {
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            var kladr_streets = DataModelHelper.FilterRows(KladrStreetsDataModel.GetInstance().Select());
            return (from building_row in buildings
                    join kladr_row in kladr_streets
                    on building_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                    where (addressParts.Count() == 1) ? kladr_row.Field<string>("street_name").ToUpperInvariant().
                                Contains(addressParts[0].ToUpperInvariant()) :
                            (addressParts.Count() >= 2) ? kladr_row.Field<string>("street_name").ToUpperInvariant().
                                Contains(addressParts[0].ToUpperInvariant()) &&
                            building_row.Field<string>("house").ToUpperInvariant() == 
                                addressParts[1].ToUpperInvariant() : false
                    select building_row.Field<int>("id_building"));
        }

        public static IEnumerable<int> PremiseIDsByAddress(string[] addressParts)
        {
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            var kladr_streets = DataModelHelper.FilterRows(KladrStreetsDataModel.GetInstance().Select());
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            return (from premises_row in premises
                    join building_row in buildings
                    on premises_row.Field<int>("id_building") equals building_row.Field<int>("id_building")
                    join kladr_row in kladr_streets
                    on building_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                    where (addressParts.Count() == 1) ? kladr_row.Field<string>("street_name").ToUpperInvariant().
                                Contains(addressParts[0].ToUpperInvariant()) :
                            (addressParts.Count() == 2) ? kladr_row.Field<string>("street_name").ToUpperInvariant().
                                Contains(addressParts[0].ToUpperInvariant()) &&
                            building_row.Field<string>("house").ToUpperInvariant() == 
                                addressParts[1].ToUpperInvariant() :
                            (addressParts.Count() == 3) ? kladr_row.Field<string>("street_name").ToUpperInvariant().
                                Contains(addressParts[0].ToUpperInvariant()) &&
                            building_row.Field<string>("house").ToUpperInvariant() == 
                                addressParts[1].ToUpperInvariant() &&
                            premises_row.Field<string>("premises_num").ToUpperInvariant().Contains(
                                addressParts[2].ToUpperInvariant()) : false
                    select premises_row.Field<int>("id_premises"));
        }

        public static IEnumerable<int> TenancyProcessIDsByAddress(string[] addressParts)
        {
            var kladr_streets = DataModelHelper.FilterRows(KladrStreetsDataModel.GetInstance().Select());
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
            var tenancy_buildings_assoc = DataModelHelper.FilterRows(TenancyBuildingsAssocDataModel.GetInstance().Select());
            var tenancy_premises_assoc = DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select());
            var tenancy_sub_premises_assoc = DataModelHelper.FilterRows(TenancySubPremisesAssocDataModel.GetInstance().Select());
            var tenancy_buildings = from tenancy_buildings_row in tenancy_buildings_assoc
                                    join buildings_row in buildings
                                    on tenancy_buildings_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                    join kladr_row in kladr_streets
                                    on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                                    where (addressParts.Count() == 1) ? kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant()) :
                                          (addressParts.Count() >= 2) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                          (buildings_row.Field<string>("house").ToUpperInvariant() == 
                                                addressParts[1].ToUpperInvariant()) : false
                                    select tenancy_buildings_row.Field<int>("id_process");
            var tenancy_premises = from tenancy_premises_row in tenancy_premises_assoc
                                   join premises_row in premises
                                   on tenancy_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                   join buildings_row in buildings
                                   on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                   join kladr_row in kladr_streets
                                    on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                                   where (addressParts.Count() == 1) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) :
                                         (addressParts.Count() == 2) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         (buildings_row.Field<string>("house").ToUpperInvariant() == 
                                                addressParts[1].ToUpperInvariant()) :
                                         (addressParts.Count() == 3) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         (buildings_row.Field<string>("house").ToUpperInvariant() == 
                                                addressParts[1].ToUpperInvariant()) &&
                                         (premises_row.Field<string>("premises_num").ToUpperInvariant().Contains(
                                                addressParts[2].ToUpperInvariant())) : false
                                   select tenancy_premises_row.Field<int>("id_process");
            var tenancy_sub_premises = from tenancy_sub_premises_row in tenancy_sub_premises_assoc
                                       join sub_premises_row in sub_premises
                                       on tenancy_sub_premises_row.Field<int>("id_sub_premises") equals sub_premises_row.Field<int>("id_sub_premises")
                                       join premises_row in premises
                                       on sub_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                       join buildings_row in buildings
                                       on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                       join kladr_row in kladr_streets
                                       on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                                       where (addressParts.Count() == 1) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) :
                                         (addressParts.Count() == 2) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         (buildings_row.Field<string>("house").ToUpperInvariant() == 
                                                addressParts[1].ToUpperInvariant()) :
                                         (addressParts.Count() == 3) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         (buildings_row.Field<string>("house").ToUpperInvariant() == 
                                                addressParts[1].ToUpperInvariant()) &&
                                         (premises_row.Field<string>("premises_num").ToUpperInvariant() == 
                                                addressParts[2].ToUpperInvariant()) : false
                                       select tenancy_sub_premises_row.Field<int>("id_process");
            return tenancy_buildings.Union(tenancy_premises).Union(tenancy_sub_premises);
        }

        public static IEnumerable<int> ResettleProcessIDsByAddress(string[] addressParts, ResettleEstateObjectWay way)
        {
            var kladr_streets = DataModelHelper.FilterRows(KladrStreetsDataModel.GetInstance().Select());
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
            var resettle_buildings_assoc = way == ResettleEstateObjectWay.From ? 
                DataModelHelper.FilterRows(ResettleBuildingsFromAssocDataModel.GetInstance().Select()) :
                DataModelHelper.FilterRows(ResettleBuildingsToAssocDataModel.GetInstance().Select());
            var resettle_premises_assoc = way == ResettleEstateObjectWay.From ?
                DataModelHelper.FilterRows(ResettlePremisesFromAssocDataModel.GetInstance().Select()) :
                DataModelHelper.FilterRows(ResettlePremisesToAssocDataModel.GetInstance().Select());
            var resettle_sub_premises_assoc = way == ResettleEstateObjectWay.From ?
                DataModelHelper.FilterRows(ResettleSubPremisesFromAssocDataModel.GetInstance().Select()) :
                DataModelHelper.FilterRows(ResettleSubPremisesToAssocDataModel.GetInstance().Select());
            var resettle_buildings = from resettle_buildings_row in resettle_buildings_assoc
                                    join buildings_row in buildings
                                    on resettle_buildings_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                    join kladr_row in kladr_streets
                                    on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                                    where (addressParts.Count() == 1) ? kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant()) :
                                          (addressParts.Count() >= 2) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                          (buildings_row.Field<string>("house").ToUpperInvariant() ==
                                                addressParts[1].ToUpperInvariant()) : false
                                    select resettle_buildings_row.Field<int>("id_process");
            var resettle_premises = from resettle_premises_row in resettle_premises_assoc
                                   join premises_row in premises
                                   on resettle_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                   join buildings_row in buildings
                                   on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                   join kladr_row in kladr_streets
                                    on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                                   where (addressParts.Count() == 1) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) :
                                         (addressParts.Count() == 2) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         (buildings_row.Field<string>("house").ToUpperInvariant() ==
                                                addressParts[1].ToUpperInvariant()) :
                                         (addressParts.Count() == 3) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         (buildings_row.Field<string>("house").ToUpperInvariant() ==
                                                addressParts[1].ToUpperInvariant()) &&
                                         (premises_row.Field<string>("premises_num").ToUpperInvariant() ==
                                                addressParts[2].ToUpperInvariant()) : false
                                   select resettle_premises_row.Field<int>("id_process");
            var resettle_sub_premises = from resettle_sub_premises_row in resettle_sub_premises_assoc
                                       join sub_premises_row in sub_premises
                                       on resettle_sub_premises_row.Field<int>("id_sub_premises") equals sub_premises_row.Field<int>("id_sub_premises")
                                       join premises_row in premises
                                       on sub_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                       join buildings_row in buildings
                                       on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                       join kladr_row in kladr_streets
                                       on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                                       where (addressParts.Count() == 1) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) :
                                         (addressParts.Count() == 2) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         (buildings_row.Field<string>("house").ToUpperInvariant() ==
                                                addressParts[1].ToUpperInvariant()) :
                                         (addressParts.Count() == 3) ? (kladr_row.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         (buildings_row.Field<string>("house").ToUpperInvariant() ==
                                                addressParts[1].ToUpperInvariant()) &&
                                         (premises_row.Field<string>("premises_num").ToUpperInvariant() ==
                                                addressParts[2].ToUpperInvariant()) : false
                                       select resettle_sub_premises_row.Field<int>("id_process");
            return resettle_buildings.Union(resettle_premises).Union(resettle_sub_premises);
        }

        public enum ConditionType { BuildingCondition, PremisesCondition };

        public static IEnumerable<int> TenancyProcessIDsByCondition(Func<DataRow, bool> condition, ConditionType conditionType)
        {
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
            var tenancy_buildings_assoc = DataModelHelper.FilterRows(TenancyBuildingsAssocDataModel.GetInstance().Select());
            var tenancy_premises_assoc = DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select());
            var tenancy_sub_premises_assoc = DataModelHelper.FilterRows(TenancySubPremisesAssocDataModel.GetInstance().Select());
            var tenancy_buildings = from tenancy_buildings_row in tenancy_buildings_assoc
                                    join buildings_row in buildings
                                    on tenancy_buildings_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                    where
                                    (conditionType == ConditionType.PremisesCondition) ? false : condition(buildings_row)
                                    select tenancy_buildings_row.Field<int>("id_process");
            var tenancy_premises = from tenancy_premises_row in tenancy_premises_assoc
                                   join premises_row in premises
                                   on tenancy_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                   join buildings_row in buildings
                                   on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                   where (conditionType == ConditionType.PremisesCondition) ? condition(premises_row) : condition(buildings_row)
                                   select tenancy_premises_row.Field<int>("id_process");
            var tenancy_sub_premises = from tenancy_sub_premises_row in tenancy_sub_premises_assoc
                                       join sub_premises_row in sub_premises
                                       on tenancy_sub_premises_row.Field<int>("id_sub_premises") equals sub_premises_row.Field<int>("id_sub_premises")
                                       join premises_row in premises
                                       on sub_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                       join buildings_row in buildings
                                       on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                       where (conditionType == ConditionType.PremisesCondition) ? condition(premises_row) : condition(buildings_row)
                                       select tenancy_sub_premises_row.Field<int>("id_process");
            return tenancy_buildings.Union(tenancy_premises).Union(tenancy_sub_premises);
        }

        public static IEnumerable<int> ResettleProcessIDsByCondition(Func<DataRow, bool> condition, ConditionType conditionType, ResettleEstateObjectWay way)
        {
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
            var resettle_buildings_assoc = way == ResettleEstateObjectWay.From ?
                DataModelHelper.FilterRows(ResettleBuildingsFromAssocDataModel.GetInstance().Select()) :
                DataModelHelper.FilterRows(ResettleBuildingsToAssocDataModel.GetInstance().Select());
            var resettle_premises_assoc = way == ResettleEstateObjectWay.From ?
                DataModelHelper.FilterRows(ResettlePremisesFromAssocDataModel.GetInstance().Select()) :
                DataModelHelper.FilterRows(ResettlePremisesToAssocDataModel.GetInstance().Select());
            var resettle_sub_premises_assoc = way == ResettleEstateObjectWay.From ?
                DataModelHelper.FilterRows(ResettleSubPremisesFromAssocDataModel.GetInstance().Select()) :
                DataModelHelper.FilterRows(ResettleSubPremisesToAssocDataModel.GetInstance().Select());
            var resettle_buildings = from resettle_buildings_row in resettle_buildings_assoc
                                    join buildings_row in buildings
                                    on resettle_buildings_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                    where
                                    (conditionType == ConditionType.PremisesCondition) ? false : condition(buildings_row)
                                    select resettle_buildings_row.Field<int>("id_process");
            var resettle_premises = from resettle_premises_row in resettle_premises_assoc
                                   join premises_row in premises
                                   on resettle_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                   join buildings_row in buildings
                                   on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                   where (conditionType == ConditionType.PremisesCondition) ? condition(premises_row) : condition(buildings_row)
                                   select resettle_premises_row.Field<int>("id_process");
            var resettle_sub_premises = from resettle_sub_premises_row in resettle_sub_premises_assoc
                                       join sub_premises_row in sub_premises
                                       on resettle_sub_premises_row.Field<int>("id_sub_premises") equals sub_premises_row.Field<int>("id_sub_premises")
                                       join premises_row in premises
                                       on sub_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                       join buildings_row in buildings
                                       on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                       where (conditionType == ConditionType.PremisesCondition) ? condition(premises_row) : condition(buildings_row)
                                       select resettle_sub_premises_row.Field<int>("id_process");
            return resettle_buildings.Union(resettle_premises).Union(resettle_sub_premises);
        }

        public static IEnumerable<int> TenancyProcessIDsByBuildingID(int id)
        {
            var tenancy_buildings_assoc = DataModelHelper.FilterRows(TenancyBuildingsAssocDataModel.GetInstance().Select());
            var tenancy_buildings = from tenancy_buildings_row in tenancy_buildings_assoc
                                    where tenancy_buildings_row.Field<int?>("id_building") == id
                                    select tenancy_buildings_row.Field<int>("id_process");
            return tenancy_buildings;
        }

        public static IEnumerable<int> TenancyProcessIDsByPremisesID(int id)
        {
            var tenancy_premises_assoc = DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select());
            var tenancy_premises = from tenancy_premises_row in tenancy_premises_assoc
                                    where tenancy_premises_row.Field<int?>("id_premises") == id
                                    select tenancy_premises_row.Field<int>("id_process");
            return tenancy_premises;
        }

        public static IEnumerable<int> TenancyProcessIDsBySubPremisesID(int id)
        {
            var tenancy_sub_premises_assoc = DataModelHelper.FilterRows(TenancySubPremisesAssocDataModel.GetInstance().Select());
            var tenancy_sub_premises = from tenancy_sub_premises_row in tenancy_sub_premises_assoc
                                   where tenancy_sub_premises_row.Field<int?>("id_sub_premises") == id
                                   select tenancy_sub_premises_row.Field<int>("id_process");
            return tenancy_sub_premises;
        }

        public static IEnumerable<int> BuildingIDsByRegistrationNumber(string number)
        {
            var tenancy_buildings_assoc = DataModelHelper.FilterRows(TenancyBuildingsAssocDataModel.GetInstance().Select());
            var tenancy_processes = DataModelHelper.FilterRows(TenancyProcessesDataModel.GetInstance().Select());
            return (from tenancy_processes_row in tenancy_processes
                    join tenancy_buildings_assoc_row in tenancy_buildings_assoc
                    on tenancy_processes_row.Field<int>("id_process") equals tenancy_buildings_assoc_row.Field<int>("id_process")
                    where tenancy_processes_row.Field<string>("registration_num") == number
                    select tenancy_buildings_assoc_row.Field<int>("id_building"));
        }

        public static IEnumerable<int> PremiseIDsByRegistrationNumber(string number)
        {
            var tenancy_premises_assoc = DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select());
            var tenancy_processes = DataModelHelper.FilterRows(TenancyProcessesDataModel.GetInstance().Select());
            return (from tenancy_processes_row in tenancy_processes
                    join tenancy_premises_assoc_row in tenancy_premises_assoc
                    on tenancy_processes_row.Field<int>("id_process") equals tenancy_premises_assoc_row.Field<int>("id_process")
                    where tenancy_processes_row.Field<string>("registration_num") == number
                    select tenancy_premises_assoc_row.Field<int>("id_premises"));
        }

        public static IEnumerable<int> BuildingIDsByCurrentFund(int idFund)
        {
            // Ищем здания указанного фонда, а также здания, в которых присутствуют помещения и комнаты указанного фонда
            DataTable buildingsFunds = CalcDataModelBuildingsCurrentFunds.GetInstance().Select();
            DataTable premisesFunds = CalcDataModelPremisesCurrentFunds.GetInstance().Select();
            DataTable subPremisesFunds = CalcDataModelSubPremisesCurrentFunds.GetInstance().Select();
            var premisesIds = from subPremisesFunds_row in subPremisesFunds.AsEnumerable()
                              join subPremises_row in DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select())
                              on subPremisesFunds_row.Field<int>("id_sub_premises") equals subPremises_row.Field<int>("id_sub_premises")
                              where new int[] { 1, 4, 5 }.Contains(subPremises_row.Field<int>("id_state")) &&
                              subPremisesFunds_row.Field<int>("id_fund_type") == idFund
                              select subPremises_row.Field<int>("id_premises");
            var buildingsIds = from premisesFunds_row in premisesFunds.AsEnumerable()
                               join premises_row in DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select())
                               on premisesFunds_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                               where new int[] { 1, 4, 5 }.Contains(premises_row.Field<int>("id_state")) &&
                                premisesFunds_row.Field<int>("id_fund_type") == idFund ||
                                (premisesFunds_row.Field<int>("id_fund_type") == 4 && premisesIds.Contains(premisesFunds_row.Field<int>("id_premises")))
                               select premises_row.Field<int>("id_building");
            return (from buildingsFunds_row in buildingsFunds.AsEnumerable()
                    join buildings_row in DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select())
                              on buildingsFunds_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                    where new int[] { 1, 4, 5 }.Contains(buildings_row.Field<int>("id_state")) && 
                    buildingsFunds_row.Field<int>("id_fund_type") == idFund ||
                                   (buildingsFunds_row.Field<int>("id_fund_type") == 4 && buildingsIds.Contains(buildingsFunds_row.Field<int>("id_building")))
                    select buildingsFunds_row.Field<int>("id_building"));
        }

        public static IEnumerable<int> PremiseIDsByCurrentFund(int idFund)
        {
            // Ищем помещения указанного фонда, а также помещения, в которых присутствуют комнаты указанного фонда
            DataTable premisesFunds = CalcDataModelPremisesCurrentFunds.GetInstance().Select();
            DataTable subPremisesFunds = CalcDataModelSubPremisesCurrentFunds.GetInstance().Select();
            var premisesIds = from subPremisesFunds_row in subPremisesFunds.AsEnumerable()
                              join subPremises_row in DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select())
                              on subPremisesFunds_row.Field<int>("id_sub_premises") equals subPremises_row.Field<int>("id_sub_premises")
                              where new int[] { 1, 4, 5 }.Contains(subPremises_row.Field<int>("id_state")) &&
                              subPremisesFunds_row.Field<int>("id_fund_type") == idFund
                              select subPremises_row.Field<int>("id_premises");
            return (from premisesFunds_row in premisesFunds.AsEnumerable()
                    join premises_row in DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select())
                               on premisesFunds_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                    where new int[] { 1, 4, 5 }.Contains(premises_row.Field<int>("id_state")) && 
                        premisesFunds_row.Field<int>("id_fund_type") == idFund ||
                        (premisesFunds_row.Field<int>("id_fund_type") == 4 && premisesIds.Contains(premisesFunds_row.Field<int>("id_premises")))
                    select premisesFunds_row.Field<int>("id_premises"));
        }

        public static IEnumerable<int> BuildingIDsByRegion(string region)
        {
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            return (from building_row in buildings
                    where (building_row.Field<string>("id_street").StartsWith(region, StringComparison.Ordinal))
                    select building_row.Field<int>("id_building"));
        }

        public static IEnumerable<int> BuildingIDsByStreet(string street)
        {
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            return (from building_row in buildings
                    where (building_row.Field<string>("id_street") == street)
                    select building_row.Field<int>("id_building"));
        }

        public static IEnumerable<int> BuildingIDsByHouse(string house)
        {
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            return (from building_row in buildings
                    where (building_row.Field<string>("house") == house)
                    select building_row.Field<int>("id_building"));
        }

        public static IEnumerable<int> ClaimStateTypeIdsByNextStateType(int nextStateType)
        {
            var claim_state_types = DataModelHelper.FilterRows(ClaimStateTypesDataModel.GetInstance().Select());
            var claim_state_type_relations = DataModelHelper.FilterRows(ClaimStateTypesRelationsDataModel.GetInstance().Select());
            return (from claim_state_types_row in claim_state_types
                    join claim_state_types_rel_row in claim_state_type_relations
                    on claim_state_types_row.Field<int>("id_state_type") equals claim_state_types_rel_row.Field<int>("id_state_from")
                    where Convert.ToBoolean(claim_state_types_row.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture) &&
                        (claim_state_types_rel_row.Field<int>("id_state_to") == nextStateType)
                    select claim_state_types_row.Field<int>("id_state_type"));
        }

        public static IEnumerable<int> ClaimStateTypeIdsByPrevStateType(int prevStateType)
        {
            var claim_state_types = DataModelHelper.FilterRows(ClaimStateTypesDataModel.GetInstance().Select());
            var claim_state_type_relations = DataModelHelper.FilterRows(ClaimStateTypesRelationsDataModel.GetInstance().Select());
            return (from claim_state_types_row in claim_state_types
                    join claim_state_types_rel_row in claim_state_type_relations
                    on claim_state_types_row.Field<int>("id_state_type") equals claim_state_types_rel_row.Field<int>("id_state_from")
                    where claim_state_types_rel_row.Field<int>("id_state_from") == prevStateType
                    select claim_state_types_rel_row.Field<int>("id_state_to"));
        }

        public static IEnumerable<int> ClaimStateTypeIdsByNextAndPrevStateTypes(int nextStateType, int prevStateType)
        {
            var claim_state_type_relations = DataModelHelper.FilterRows(ClaimStateTypesRelationsDataModel.GetInstance().Select());
            var from_states = from claim_state_types_rel_row in claim_state_type_relations
                              where claim_state_types_rel_row.Field<int>("id_state_from") == prevStateType
                              select claim_state_types_rel_row.Field<int>("id_state_to");
            var to_states = from claim_state_types_rel_row in claim_state_type_relations
                            where claim_state_types_rel_row.Field<int>("id_state_to") == nextStateType
                            select claim_state_types_rel_row.Field<int>("id_state_from");
            return from_states.Intersect(to_states);
        }

        public static IEnumerable<int> ClaimStartStateTypeIds()
        {
            var claim_state_types = DataModelHelper.FilterRows(ClaimStateTypesDataModel.GetInstance().Select());
            return (from claim_state_types_row in claim_state_types
                    where Convert.ToBoolean(claim_state_types_row.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture)
                    select claim_state_types_row.Field<int>("id_state_type"));
        }
    
        public static bool TenancyProcessHasTenant(int idProcess)
        {
            var persons = DataModelHelper.FilterRows(TenancyPersonsDataModel.GetInstance().Select());
            return (from persons_row in persons
                    where (persons_row.Field<int?>("id_kinship") == 1) && (persons_row.Field<int>("id_process") == idProcess
                        && (persons_row.Field<DateTime?>("exclude_date") == null))
                    select persons_row).Count() > 0;
        }

        public static int TenancyAgreementsForProcess(int idProcess)
        {
            var agreements = DataModelHelper.FilterRows(TenancyAgreementsDataModel.GetInstance().Select());
            return (from agreements_row in agreements
                    where agreements_row.Field<int>("id_process") == idProcess
                    select agreements_row).Count();
        }   

        public static int BuildingsDuplicateCount(Building building)
        {
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            return (from buildings_row in buildings
                    where buildings_row.Field<string>("id_street") == building.IdStreet &&
                          buildings_row.Field<string>("house") == building.House &&
                          buildings_row.Field<int>("id_building") != building.IdBuilding
                    select buildings_row).Count();
        }

        public static int PremisesDuplicateCount(Premise premise)
        {
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            return (from premises_row in premises
                    where premises_row.Field<int>("id_building") == premise.IdBuilding &&
                          premises_row.Field<string>("premises_num") == premise.PremisesNum &&
                          premises_row.Field<int>("id_premises") != premise.IdPremises
                    select premises_row).Count();
        }

        public static int TenancyProcessesDuplicateCount(TenancyProcess process)
        {
            var tenancy_processes = DataModelHelper.FilterRows(TenancyProcessesDataModel.GetInstance().Select());
            return (from tenancy_processes_row in tenancy_processes
                    where tenancy_processes_row.Field<string>("registration_num") == process.RegistrationNum &&
                          tenancy_processes_row.Field<int>("id_process") != process.IdProcess
                    select tenancy_processes_row).Count();
        }

        public static IEnumerable<int> ObjectIdsByStates(EntityType entity, int[] states)
        {
            if (entity == EntityType.Building)
            {
                var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
                var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
                var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
                var result = from buildings_row in buildings
                             join premises_row in premises
                             on buildings_row.Field<int>("id_building") equals premises_row.Field<int>("id_building") into bp
                             from bp_row in bp.DefaultIfEmpty()
                             join sub_premises_row in sub_premises
                             on bp_row.Field<int?>("id_premises") equals sub_premises_row.Field<int?>("id_premises") into ps
                             from ps_row in ps.DefaultIfEmpty()
                             where (bp_row != null && states.Contains(bp_row.Field<int>("id_state"))) ||
                                   (ps_row != null && states.Contains(ps_row.Field<int>("id_state")))
                             select buildings_row.Field<int>("id_building");
                return result;
            }
            else
            {
                var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
                var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
                var result = from premises_row in premises
                             join sub_premises_row in sub_premises
                             on premises_row.Field<int>("id_premises") equals sub_premises_row.Field<int>("id_premises") into ps
                             from ps_row in ps.DefaultIfEmpty()
                             where (ps_row != null && states.Contains(ps_row.Field<int>("id_state")))
                             select premises_row.Field<int>("id_premises");
                return result;
            }
        }

        private static bool HasObjectState(int id, EntityType entity, int[] states)
        {
            if (entity == EntityType.Building)
            {
                var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select(), entity, id);
                var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select(), entity, id);
                var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
                bool m_building = (from building_row in buildings
                                   where building_row.Field<int>("id_building") == id &&
                                   states.Contains(building_row.Field<int>("id_state"))
                                   select building_row).Count() > 0;
                bool m_premises = (from premises_row in premises
                                   where premises_row.Field<int>("id_building") == id &&
                                   states.Contains(premises_row.Field<int>("id_state"))
                                   select premises_row).Count() > 0;
                bool m_sub_premises = (from premises_row in premises
                                       join sub_premises_row in sub_premises
                                       on premises_row.Field<int>("id_premises") equals sub_premises_row.Field<int>("id_premises")
                                       where premises_row.Field<int>("id_building") == id &&
                                       states.Contains(sub_premises_row.Field<int>("id_state"))
                                       select sub_premises_row).Count() > 0;
                return m_building || m_premises || m_sub_premises;
            }
            else
                if (entity == EntityType.Premise)
                {
                    var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select(), entity, id);
                    var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
                    bool m_premises = (from premises_row in premises
                                       where premises_row.Field<int>("id_premises") == id &&
                                       states.Contains(premises_row.Field<int>("id_state"))
                                       select premises_row).Count() > 0;
                    bool m_sub_premises = (from sub_premises_row in sub_premises
                                           where sub_premises_row.Field<int>("id_premises") == id &&
                                           states.Contains(sub_premises_row.Field<int>("id_state"))
                                           select sub_premises_row).Count() > 0;
                    return m_premises || m_sub_premises;
                }
                else
                    if (entity == EntityType.SubPremise)
                    {
                        var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select(), entity, id);
                        return (from sub_premises_row in sub_premises
                                where sub_premises_row.Field<int>("id_sub_premises") == id &&
                                states.Contains(sub_premises_row.Field<int>("id_state"))
                                select sub_premises_row).Count() > 0;
                    }
                    else
                        return false;
        }

        public static bool HasMunicipal(int id, EntityType entity)
        {
            return HasObjectState(id, entity, new int[] { 4, 5 });
        }

        public static bool HasNotMunicipal(int id, EntityType entity)
        {

            return HasObjectState(id, entity, new int[] { 1, 3 });
        }

        public static IEnumerable<int> DemolishedBuildingIDs()
        {
            var ownership_rights = DataModelHelper.FilterRows(OwnershipsRightsDataModel.GetInstance().Select());
            var ownership_buildingds_assoc = DataModelHelper.FilterRows(OwnershipBuildingsAssocDataModel.GetInstance().Select());
            return from ownership_buildings_assoc_row in ownership_buildingds_assoc
                   join ownership_rights_row in ownership_rights
                   on ownership_buildings_assoc_row.Field<int>("id_ownership_right") equals ownership_rights_row.Field<int>("id_ownership_right")
                   where ownership_rights_row.Field<int>("id_ownership_right_type") == 1
                   select ownership_buildings_assoc_row.Field<int>("id_building");
        }

        public static IEnumerable<int> DemolishedPremisesIDs()
        {
            var ownership_rights = DataModelHelper.FilterRows(OwnershipsRightsDataModel.GetInstance().Select());
            var ownership_premises_assoc = DataModelHelper.FilterRows(OwnershipPremisesAssocDataModel.GetInstance().Select());
            return from ownership_premises_assoc_row in ownership_premises_assoc
                   join ownership_rights_row in ownership_rights
                   on ownership_premises_assoc_row.Field<int>("id_ownership_right") equals ownership_rights_row.Field<int>("id_ownership_right")
                   where ownership_rights_row.Field<int>("id_ownership_right_type") == 1
                   select ownership_premises_assoc_row.Field<int>("id_premises");
        }

        public static IEnumerable<int> BuildingIDsByOwnershipType(int idOwnershipType)
        {
            var ownership_rights = DataModelHelper.FilterRows(OwnershipsRightsDataModel.GetInstance().Select());
            var ownership_buildingds_assoc = DataModelHelper.FilterRows(OwnershipBuildingsAssocDataModel.GetInstance().Select());
            // Если используется непользовательское ограничение "Аварийное", то не выводить здания еще и снесеные
            var demolished_buildings = DemolishedBuildingIDs();
            return from ownership_rights_row in ownership_rights
                   join ownership_buildingds_assoc_row in ownership_buildingds_assoc
                   on ownership_rights_row.Field<int>("id_ownership_right") equals ownership_buildingds_assoc_row.Field<int>("id_ownership_right")
                   where (idOwnershipType == 2 ? !demolished_buildings.Contains(ownership_buildingds_assoc_row.Field<int>("id_building")) : true) && 
                        ownership_rights_row.Field<int>("id_ownership_right_type") == idOwnershipType
                   select ownership_buildingds_assoc_row.Field<int>("id_building");
        }

        public static IEnumerable<int> PremiseIDsByOwnershipType(int idOwnershipType)
        {
            var ownership_rights = DataModelHelper.FilterRows(OwnershipsRightsDataModel.GetInstance().Select());
            var ownership_premises_assoc = DataModelHelper.FilterRows(OwnershipPremisesAssocDataModel.GetInstance().Select());
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            IEnumerable<int> buildingdIds = BuildingIDsByOwnershipType(idOwnershipType);
            // Если используется непользовательское ограничение "Аварийное", то не выводить помещения еще и снесеные
            var demolished_premises = DemolishedPremisesIDs();
            //Выбираются помещения с установленным ограничением и помещения, находящиеся в зданиях с установленным ограничением
            return from ownership_rights_row in ownership_rights
                   join ownership_premises_assoc_row in ownership_premises_assoc
                   on ownership_rights_row.Field<int>("id_ownership_right") equals ownership_premises_assoc_row.Field<int>("id_ownership_right")
                   join premises_row in premises
                   on ownership_premises_assoc_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                   where (idOwnershipType == 2 ? !demolished_premises.Contains(ownership_premises_assoc_row.Field<int>("id_premises")) : true) &&
                         (ownership_rights_row.Field<int>("id_ownership_right_type") == idOwnershipType) || 
                          buildingdIds.Contains(premises_row.Field<int>("id_building"))
                   select ownership_premises_assoc_row.Field<int>("id_premises");
        }

        /// <summary>
        /// Объекты недвижимости, исключенные из муниципальной собственности
        /// </summary>
        /// <param name="objectAssocDataRows">Список строк из ассоциативной модели реквизитов</param>
        /// <param name="entity">Тип ассоциативной модели</param>
        /// <returns>Перечень объектов</returns>
        public static IEnumerable<RestrictionObjectAssoc> ObjectIDsExcludedFromMunicipal(IEnumerable<DataRow> objectAssocDataRows, EntityType entity)
        {
            var restrictions = DataModelHelper.FilterRows(RestrictionsDataModel.GetInstance().Select());
            string fieldName = null;
            if (entity == EntityType.Premise)
                fieldName = "id_premises";
            else
                if (entity == EntityType.Building)
                    fieldName = "id_building";
                else
                    if (entity == EntityType.SubPremise)
                        fieldName = "id_sub_premises";
            var restrictions_max_date = from restrictions_assoc_row in objectAssocDataRows
                                                 join restrictions_row in restrictions
                                                 on restrictions_assoc_row.Field<int>("id_restriction") equals restrictions_row.Field<int>("id_restriction")
                                                 where new int[] { 1, 2 }.Contains(restrictions_row.Field<int>("id_restriction_type"))
                                                 group restrictions_row.Field<DateTime>("date") by restrictions_assoc_row.Field<int>(fieldName) into gs
                                                 select new
                                                 {
                                                     id = gs.Key,
                                                     date = gs.Max()
                                                 };
            var exclude_from_municipal = from restrictions_row in restrictions
                                         join restrictions__assoc_row in objectAssocDataRows
                                         on restrictions_row.Field<int>("id_restriction")
                                         equals restrictions__assoc_row.Field<int>("id_restriction")
                                         join rmd_row in restrictions_max_date
                                         on new
                                         {
                                             id = restrictions__assoc_row.Field<int>(fieldName),
                                             date = restrictions_row.Field<DateTime>("date")
                                         } equals
                                         new
                                         {
                                             id = rmd_row.id,
                                             date = rmd_row.date
                                         }
                                         where restrictions_row.Field<int>("id_restriction_type") == 2
                                         select new RestrictionObjectAssoc(restrictions__assoc_row.Field<int?>(fieldName), restrictions_row.Field<int?>("id_restriction"), restrictions_row.Field<DateTime?>("date"));
            return exclude_from_municipal;
        }

        /// <summary>
        /// Объекты недвижимости, включенные в муниципальную собственность
        /// </summary>
        /// <param name="objectAssocDataRows">Список строк из ассоциативной модели реквизитов</param>
        /// <param name="entity">Тип ассоциативной модели</param>
        /// <returns>Перечень объектов</returns>
        public static IEnumerable<RestrictionObjectAssoc> ObjectIDsIncludedIntoMunicipal(IEnumerable<DataRow> objectAssocDataRows, EntityType entity)
        {
            var restrictions = DataModelHelper.FilterRows(RestrictionsDataModel.GetInstance().Select());
            string fieldName = null;
            if (entity == EntityType.Premise)
                fieldName = "id_premises";
            else
                if (entity == EntityType.Building)
                    fieldName = "id_building";
                else
                    if (entity == EntityType.SubPremise)
                        fieldName = "id_sub_premises";
            var restrictions_max_date = from restrictions_assoc_row in objectAssocDataRows
                                        join restrictions_row in restrictions
                                        on restrictions_assoc_row.Field<int>("id_restriction") equals restrictions_row.Field<int>("id_restriction")
                                        where new int[] { 1, 2 }.Contains(restrictions_row.Field<int>("id_restriction_type"))
                                        group restrictions_row.Field<DateTime>("date") by restrictions_assoc_row.Field<int>(fieldName) into gs
                                        select new
                                        {
                                            id = gs.Key,
                                            date = gs.Max()
                                        };
            var included_into_municipal = from restrictions_row in restrictions
                                         join restrictions__assoc_row in objectAssocDataRows
                                         on restrictions_row.Field<int>("id_restriction")
                                         equals restrictions__assoc_row.Field<int>("id_restriction")
                                         join rmd_row in restrictions_max_date
                                         on new
                                         {
                                             id = restrictions__assoc_row.Field<int>(fieldName),
                                             date = restrictions_row.Field<DateTime>("date")
                                         } equals
                                         new
                                         {
                                             id = rmd_row.id,
                                             date = rmd_row.date
                                         }
                                         where restrictions_row.Field<int>("id_restriction_type") == 1
                                         select new RestrictionObjectAssoc(restrictions__assoc_row.Field<int?>(fieldName), restrictions_row.Field<int?>("id_restriction"), restrictions_row.Field<DateTime?>("date"));
            return included_into_municipal;
        }

        /// <summary>
        /// Максимальные идентификаторы фонда объектов недвижимости
        /// </summary>
        /// <param name="objectAssocDataRows">Список строк из ассоциативной модели фондов</param>
        /// <param name="entity">Тип ассоциативной модели</param>
        /// <returns>Возвращает максимальные id_fund, не имеющий реквизита исключения</returns>
        public static IEnumerable<FundObjectAssoc> MaxFundIDsByObject(IEnumerable<DataRow> objectAssocDataRows, EntityType entity)
        {
            string fieldName = null;
            if (entity == EntityType.Premise)
                fieldName = "id_premises";
            else
                if (entity == EntityType.Building)
                    fieldName = "id_building";
                else
                    if (entity == EntityType.SubPremise)
                        fieldName = "id_sub_premises";
            var funds_history = DataModelHelper.FilterRows(FundsHistoryDataModel.GetInstance().Select());
            var max_id_by_object = from assoc_row in objectAssocDataRows
                                     join fund_history_row in funds_history
                                        on assoc_row.Field<int>("id_fund") equals fund_history_row.Field<int>("id_fund")
                                     where fund_history_row.Field<DateTime?>("exclude_restriction_date") == null
                                     group assoc_row.Field<int>("id_fund") by
                                             assoc_row.Field<int>(fieldName) into gs
                                     select new FundObjectAssoc(gs.Key, gs.Max());
            return max_id_by_object;
        }

        /// <summary>
        /// Агрегатор адреса для различных процессов: найма, приватизации, переселения и т.д.
        /// </summary>
        /// <param name="buildingDataModel">Ассоциативная модель зданий</param>
        /// <param name="premisesAssocDataModel">Ассоциативная модель помещений</param>
        /// <param name="subPremisesAssocDataModel">Ассоциативная модель комнат</param>
        /// <returns>Возвращает перечисление вида id_process, address</returns>
        public static IEnumerable<AggregatedAddress> AggregateAddressByIdProcess(DataModel buildingAssocDataModel, DataModel premisesAssocDataModel, DataModel subPremisesAssocDataModel, int? idProcess)
        {
            if (buildingAssocDataModel == null || premisesAssocDataModel == null || subPremisesAssocDataModel == null)
                throw new DataModelException("Не переданы все ссылки на ассоциативные модели");
            var assoc_sub_premises = from assoc_sub_premises_row in DataModelHelper.FilterRows(subPremisesAssocDataModel.Select())
                                       where (idProcess != null ?
                                            assoc_sub_premises_row.Field<int>("id_process") == idProcess : true)
                                       select assoc_sub_premises_row;
            var assoc_premises = from assoc_premises_row in DataModelHelper.FilterRows(premisesAssocDataModel.Select())
                                   where (idProcess != null ?
                                          assoc_premises_row.Field<int>("id_process") == idProcess : true)
                                   select assoc_premises_row;
            var assoc_buildings = from assoc_buildings_row in DataModelHelper.FilterRows(buildingAssocDataModel.Select())
                                    where (idProcess != null ?
                                           assoc_buildings_row.Field<int>("id_process") == idProcess : true)
                                  select assoc_buildings_row;
            var kladr_street = DataModelHelper.FilterRows(KladrStreetsDataModel.GetInstance().Select());
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
            var a_sub_premises_gc = from assoc_sub_premises_row in assoc_sub_premises
                                    join sub_premises_row in sub_premises
                                    on assoc_sub_premises_row.Field<int>("id_sub_premises") equals sub_premises_row.Field<int>("id_sub_premises")
                                    join premises_row in premises.AsEnumerable()
                                    on sub_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                    group sub_premises_row.Field<string>("sub_premises_num") by
                                        new
                                        {
                                            id_process = assoc_sub_premises_row.Field<int>("id_process"),
                                            id_building = premises_row.Field<int>("id_building"),
                                            id_premises = sub_premises_row.Field<int>("id_premises"),
                                            id_premises_type = premises_row.Field<int>("id_premises_type"),
                                            premises_num = premises_row.Field<string>("premises_num")
                                        } into gs
                                    select new
                                    {
                                        id_process = gs.Key.id_process,
                                        id_building = gs.Key.id_building,
                                        id_premises = gs.Key.id_premises,
                                        result_str = (gs.Key.id_premises_type == 2 ? " ком. " : (gs.Key.id_premises_type == 4 ? " пом. " : " кв. ")) + 
                                            gs.Key.premises_num + ((gs.Count() > 0) ?
                                        (" ком. " + gs.Aggregate((a, b) =>
                                        {
                                            return a + ", " + b;
                                        })) : "")
                                    };
            var a_premises = from assoc_premises_row in assoc_premises
                             join premises_row in premises
                             on assoc_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                             select new
                             {
                                 id_process = assoc_premises_row.Field<int>("id_process"),
                                 id_building = premises_row.Field<int>("id_building"),
                                 id_premises = assoc_premises_row.Field<int>("id_premises"),
                                 result_str = (premises_row.Field<int>("id_premises_type") == 2 ? " ком. " :
                                    (premises_row.Field<int>("id_premises_type") == 4 ? " пом. " : " кв. ")) + premises_row.Field<string>("premises_num")
                             };
            var a_buildings = from assoc_buildings_row in assoc_buildings
                              select new
                              {
                                  id_process = assoc_buildings_row.Field<int>("id_process"),
                                  id_building = assoc_buildings_row.Field<int>("id_building"),
                                  result_str = ""
                              };
            var assoc_premises_gc = from assoc_premises_row in a_premises.Union(a_sub_premises_gc)
                                      join premises_row in premises
                                      on assoc_premises_row.id_premises equals premises_row.Field<int>("id_premises")
                                      group
                                          assoc_premises_row.result_str
                                          by new
                                          {
                                              assoc_premises_row.id_process,
                                              assoc_premises_row.id_building
                                          } into gs
                                      select new
                                      {
                                          id_process = gs.Key.id_process,
                                          id_building = gs.Key.id_building,
                                          result_str = gs.Aggregate((a, b) =>
                                          {
                                              return a + ", " + b.Trim();
                                          })
                                      };
            var addresses = from a_buildings_row in a_buildings.Union(assoc_premises_gc)
                            join buildings_row in buildings
                            on a_buildings_row.id_building equals buildings_row.Field<int>("id_building")
                            join kladr_row in kladr_street
                            on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                            group
                               kladr_row.Field<string>("street_name") + ", дом " + buildings_row.Field<string>("house") + a_buildings_row.result_str
                            by a_buildings_row.id_process into gs
                            select new AggregatedAddress(gs.Key, gs.Aggregate((a, b) =>
                                {
                                    return a + ", " + b.Trim();
                                }));
            return addresses;
        }
    
        /// <summary>
        /// Возвращает список идентификаторов старых процессов найма
        /// </summary>
        /// <returns>Список идентификаторов</returns>
        public static IEnumerable<int> OldTenancyProcesses()
        {
            // Собираем строку проверки уникальности сдаваемой группы помещений. Выглядеть эта строка будет примерно подобным образом з1з12п23п122к32 и т.п.
            var assoc_sub_premises = from assoc_sub_premises_row in DataModelHelper.FilterRows(TenancySubPremisesAssocDataModel.GetInstance().Select())
                                     group assoc_sub_premises_row.Field<int>("id_sub_premises").ToString(CultureInfo.InvariantCulture)
                                     by assoc_sub_premises_row.Field<int>("id_process") into gs
                                     select new
                                     {
                                         id_process = gs.Key,
                                         value = gs.Count() > 1 ? gs.OrderBy(val => val).Aggregate((str1, str2) => { return 'к'+str1+'к'+str2; }) : 'к'+gs.First()
                                     };
            var assoc_premises = from assoc_premises_row in DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select())
                                     group assoc_premises_row.Field<int>("id_premises").ToString(CultureInfo.InvariantCulture)
                                     by assoc_premises_row.Field<int>("id_process") into gs
                                     select new
                                     {
                                         id_process = gs.Key,
                                         value = gs.Count() > 1 ? gs.OrderBy(val => val).Aggregate((str1, str2) => { return 'п' + str1 + 'п' + str2; }) : 'п' + gs.First()
                                     };
            var assoc_buildings = from assoc_buildings_row in DataModelHelper.FilterRows(TenancyBuildingsAssocDataModel.GetInstance().Select())
                                  group assoc_buildings_row.Field<int>("id_building").ToString(CultureInfo.InvariantCulture)
                                 by assoc_buildings_row.Field<int>("id_process") into gs
                                 select new
                                 {
                                     id_process = gs.Key,
                                     value = gs.Count() > 1 ? gs.OrderBy(val => val).Aggregate((str1, str2) => { return 'з' + str1 + 'з' + str2; }) : 'з' + gs.First()
                                 };
            var assoc_objects = assoc_sub_premises.Union(assoc_premises).Union(assoc_buildings);
            var ident_strings = from assoc_objects_row in assoc_objects
                                group assoc_objects_row.value by assoc_objects_row.id_process into gs
                                select new
                                {
                                    id_process = gs.Key,
                                    value = gs.Count() > 1 ? gs.OrderBy(val => val).Aggregate((str1, str2) => { return str1 + str2; }) : gs.First()
                                };
            var more_one_ident_strings = from ident_row in ident_strings
                                         group ident_row.id_process by ident_row.value into gs
                                         where gs.Count() > 1
                                         select gs.Key;
            var duplicate_processes = from ident_row in ident_strings
                                        join tenancy_row in DataModelHelper.FilterRows(TenancyProcessesDataModel.GetInstance().Select())
                                        on ident_row.id_process equals tenancy_row.Field<int>("id_process")
                                        join more_one_ident_strings_row in more_one_ident_strings
                                        on ident_row.value equals more_one_ident_strings_row
                                        where tenancy_row.Field<string>("registration_num") != null
                                        select new
                                        {
                                            id_process = tenancy_row.Field<int>("id_process"),
                                            ident_value = ident_row.value,
                                            registration_date = tenancy_row.Field<DateTime?>("registration_date")
                                        };
            var max_dates = from row in duplicate_processes
                            group row.registration_date by row.ident_value into gs
                            select new
                            {
                                ident_value = gs.Key,
                                max_date = gs.Max()
                            };
            var max_process_ids = from process_row in duplicate_processes
                                  join max_dates_row in max_dates
                                  on new { ident_value = process_row.ident_value, date = process_row.registration_date } equals
                                  new { ident_value = max_dates_row.ident_value, date = max_dates_row.max_date }
                                  select process_row.id_process;
            var duplicate_process_ids = from row in duplicate_processes
                                        select row.id_process;
            return duplicate_process_ids.Except(max_process_ids);
        }
    }
}
