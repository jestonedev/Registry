using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels
{
    public static class DataModelHelper
    {
        public static IEnumerable<int> Intersect(IEnumerable<int> list1, IEnumerable<int> list2)
        {
            if (list1 != null && list2 != null)
                return list1.Intersect(list2).ToList();
            return list1 ?? list2;
        }

        public static IEnumerable<int> BuildingIdsBySnp(string[] snp, Func<DataRow, bool> condition)
        {
            var tenancyBuildingsAssoc =
                DataModel.GetInstance(DataModelType.TenancyBuildingsAssocDataModel).FilterDeletedRows();
            var tenancyPersons = DataModel.GetInstance(DataModelType.TenancyPersonsDataModel).FilterDeletedRows();
            return
            (from tenancyBuildingsAssocRow in tenancyBuildingsAssoc
             join tenancyPersonsRow in tenancyPersons
             on tenancyBuildingsAssocRow.Field<int>("id_process") equals tenancyPersonsRow.Field<int>("id_process")
             where ((snp.Any()) && (tenancyPersonsRow.Field<string>("surname") != null &&
                     string.Equals(tenancyPersonsRow.Field<string>("surname"), snp[0], StringComparison.InvariantCultureIgnoreCase))) &&
                    ((snp.Count() < 2) || tenancyPersonsRow.Field<string>("name") != null &&
                     string.Equals(tenancyPersonsRow.Field<string>("name"), snp[1], StringComparison.InvariantCultureIgnoreCase) ) &&
                    ((snp.Count() < 3) || tenancyPersonsRow.Field<string>("patronymic") != null &&
                     string.Equals(tenancyPersonsRow.Field<string>("patronymic"), snp[2], StringComparison.InvariantCultureIgnoreCase)) &&
                    condition(tenancyPersonsRow)
             select tenancyBuildingsAssocRow.Field<int>("id_building")).Distinct();
        }

        public static IEnumerable<int> PremisesIdsBySnp(string[] snp, Func<DataRow, bool> condition)
        {
            var tenancyPremisesAssoc = DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).FilterDeletedRows();
            var tenancyPersons = DataModel.GetInstance(DataModelType.TenancyPersonsDataModel).FilterDeletedRows();
            return
            (from tenancyPremisesAssocRow in tenancyPremisesAssoc
             join tenancyPersonsRow in tenancyPersons
             on tenancyPremisesAssocRow.Field<int>("id_process") equals tenancyPersonsRow.Field<int>("id_process")
             where ((snp.Any()) && (tenancyPersonsRow.Field<string>("surname") != null &&
                     string.Equals(tenancyPersonsRow.Field<string>("surname"), snp[0], StringComparison.InvariantCultureIgnoreCase))) &&
                    ((snp.Count() < 2) || tenancyPersonsRow.Field<string>("name") != null &&
                     string.Equals(tenancyPersonsRow.Field<string>("name"), snp[1], StringComparison.InvariantCultureIgnoreCase)) &&
                    ((snp.Count() < 3) || tenancyPersonsRow.Field<string>("patronymic") != null &&
                     string.Equals(tenancyPersonsRow.Field<string>("patronymic"), snp[2], StringComparison.InvariantCultureIgnoreCase)) &&
                    condition(tenancyPersonsRow)
             select tenancyPremisesAssocRow.Field<int>("id_premises")).Distinct();
        }

        public static IEnumerable<int> TenancyProcessIdsBySnp(string[] snp, Func<DataRow, bool> condition)
        {
            var tenancyPersons = DataModel.GetInstance(DataModelType.TenancyPersonsDataModel).FilterDeletedRows();

            return
            (from tenancyPersonsRow in tenancyPersons
             where ((snp.Any()) && (tenancyPersonsRow.Field<string>("surname") != null &&
                    string.Equals(tenancyPersonsRow.Field<string>("surname"), snp[0], StringComparison.InvariantCultureIgnoreCase))) &&
                ((snp.Count() < 2) || tenancyPersonsRow.Field<string>("name") != null &&
                    string.Equals(tenancyPersonsRow.Field<string>("name"), snp[1], StringComparison.InvariantCultureIgnoreCase)) &&
                ((snp.Count() < 3) || tenancyPersonsRow.Field<string>("patronymic") != null &&
                    string.Equals(tenancyPersonsRow.Field<string>("patronymic"), snp[2], StringComparison.InvariantCultureIgnoreCase)) &&
                condition(tenancyPersonsRow)
             select tenancyPersonsRow.Field<int>("id_process")).Distinct();
        }

        public static IEnumerable<int> ResettleProcessIdsBySnp(string[] snp)
        {
            var resettlePersons = DataModel.GetInstance(DataModelType.ResettlePersonsDataModel).FilterDeletedRows();
            return
            (from resettlePersonsRow in resettlePersons
             where ((snp.Any()) && (resettlePersonsRow.Field<string>("surname") != null &&
                    string.Equals(resettlePersonsRow.Field<string>("surname"), snp[0], StringComparison.InvariantCultureIgnoreCase))) &&
                ((snp.Count() < 2) || resettlePersonsRow.Field<string>("name") != null &&
                    string.Equals(resettlePersonsRow.Field<string>("name"), snp[1], StringComparison.InvariantCultureIgnoreCase)) &&
                ((snp.Count() < 3) || resettlePersonsRow.Field<string>("patronymic") != null &&
                    string.Equals(resettlePersonsRow.Field<string>("patronymic"), snp[2], StringComparison.InvariantCultureIgnoreCase))
             select resettlePersonsRow.Field<int>("id_process")).Distinct();
        }

        public static IEnumerable<int> BuildingIDsByAddress(string[] addressParts)
        {
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            var kladrStreets = DataModel.GetInstance(DataModelType.KladrStreetsDataModel).FilterDeletedRows();
            return (from buildingRow in buildings
                    join kladrRow in kladrStreets
                    on buildingRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                    where (addressParts.Count() == 1) ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                                Contains(addressParts[0].ToUpperInvariant()) :
                            (addressParts.Count() >= 2) && (kladrRow.Field<string>("street_name").ToUpperInvariant().
                                Contains(addressParts[0].ToUpperInvariant()) &&
                            buildingRow.Field<string>("house").ToUpper().Contains(addressParts[1].ToUpper()))
                    select buildingRow.Field<int>("id_building"));
        }

        public static IEnumerable<int> PremiseIDsByAddress(string[] addressParts)
        {
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            var kladrStreets = DataModel.GetInstance(DataModelType.KladrStreetsDataModel).FilterDeletedRows();
            var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
            return (from premisesRow in premises
                    join buildingRow in buildings
                    on premisesRow.Field<int>("id_building") equals buildingRow.Field<int>("id_building")
                    join kladrRow in kladrStreets
                    on buildingRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                    where (addressParts.Count() == 1) ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                                Contains(addressParts[0].ToUpperInvariant()) :
                            (addressParts.Count() == 2) ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                                Contains(addressParts[0].ToUpperInvariant()) &&
                            buildingRow.Field<string>("house").ToUpper().Contains(addressParts[1].ToUpper()) :
                            (addressParts.Count() == 3) && (kladrRow.Field<string>("street_name").ToUpperInvariant().
                                Contains(addressParts[0].ToUpperInvariant()) &&
                            buildingRow.Field<string>("house").ToUpper().Contains(addressParts[1].ToUpper()) &&
                            premisesRow.Field<string>("premises_num").ToUpperInvariant().Contains(
                                addressParts[2].ToUpperInvariant()))
                    select premisesRow.Field<int>("id_premises"));
        }

        public static IEnumerable<int> TenancyProcessIDsByAddress(string[] addressParts)
        {
            var kladrStreets = DataModel.GetInstance(DataModelType.KladrStreetsDataModel).FilterDeletedRows();
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
            var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
            var tenancyBuildingsAssoc = DataModel.GetInstance(DataModelType.TenancyBuildingsAssocDataModel).FilterDeletedRows();
            var tenancyPremisesAssoc = DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).FilterDeletedRows();
            var tenancySubPremisesAssoc = DataModel.GetInstance(DataModelType.TenancySubPremisesAssocDataModel).FilterDeletedRows();
            var tenancyBuildings = from tenancyBuildingsRow in tenancyBuildingsAssoc
                                    join buildingsRow in buildings
                                    on tenancyBuildingsRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                    join kladrRow in kladrStreets
                                    on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                                    where (addressParts.Count() == 1) ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant()) :
                                          (addressParts.Count() >= 2) && ((kladrRow.Field<string>("street_name").ToUpperInvariant().
                                              Contains(addressParts[0].ToUpperInvariant())) &&
                                        buildingsRow.Field<string>("house").ToUpper().Contains(addressParts[1].ToUpper()))
                                    select tenancyBuildingsRow.Field<int>("id_process");
            var tenancyPremises = from tenancyPremisesRow in tenancyPremisesAssoc
                                   join premisesRow in premises
                                   on tenancyPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                   join buildingsRow in buildings
                                   on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                   join kladrRow in kladrStreets
                                    on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                                   where (addressParts.Count() == 1) ? (kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) :
                                         (addressParts.Count() == 2) ? (kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         buildingsRow.Field<string>("house").ToUpper().Contains(addressParts[1].ToUpper()) :
                                         (addressParts.Count() == 3) && ((kladrRow.Field<string>("street_name").ToUpperInvariant().
                                             Contains(addressParts[0].ToUpperInvariant())) &&
                                        buildingsRow.Field<string>("house").ToUpper().Contains(addressParts[1].ToUpper()) &&
                                        (premisesRow.Field<string>("premises_num").ToUpperInvariant().Contains(
                                            addressParts[2].ToUpperInvariant())))
                                   select tenancyPremisesRow.Field<int>("id_process");
            var tenancySubPremises = from tenancySubPremisesRow in tenancySubPremisesAssoc
                                       join subPremisesRow in subPremises
                                       on tenancySubPremisesRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                                       join premisesRow in premises
                                       on subPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                       join buildingsRow in buildings
                                       on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                       join kladrRow in kladrStreets
                                       on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                                       where (addressParts.Count() == 1) ? (kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) :
                                         (addressParts.Count() == 2) ? (kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         buildingsRow.Field<string>("house").ToUpper().Contains(addressParts[1].ToUpper()) :
                                         (addressParts.Count() == 3) && ((kladrRow.Field<string>("street_name").ToUpperInvariant().
                                             Contains(addressParts[0].ToUpperInvariant())) &&
                                        buildingsRow.Field<string>("house").ToUpper().Contains(addressParts[1].ToUpper()) &&
                                        premisesRow.Field<string>("premises_num").ToUpper().Contains(addressParts[2].ToUpper()))
                                       select tenancySubPremisesRow.Field<int>("id_process");
            return tenancyBuildings.Union(tenancyPremises).Union(tenancySubPremises);
        }

        public static IEnumerable<int> ResettleProcessIDsByAddress(string[] addressParts, ResettleEstateObjectWay way)
        {
            var kladrStreets = DataModel.GetInstance(DataModelType.KladrStreetsDataModel).FilterDeletedRows();
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
            var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
            var resettleBuildingsAssoc = way == ResettleEstateObjectWay.From ? 
                DataModel.GetInstance(DataModelType.ResettleBuildingsFromAssocDataModel).FilterDeletedRows() :
                DataModel.GetInstance(DataModelType.ResettleBuildingsToAssocDataModel).FilterDeletedRows();
            var resettlePremisesAssoc = way == ResettleEstateObjectWay.From ?
                DataModel.GetInstance(DataModelType.ResettlePremisesFromAssocDataModel).FilterDeletedRows() :
                DataModel.GetInstance(DataModelType.ResettlePremisesToAssocDataModel).FilterDeletedRows();
            var resettleSubPremisesAssoc = way == ResettleEstateObjectWay.From ?
                DataModel.GetInstance(DataModelType.ResettleSubPremisesFromAssocDataModel).FilterDeletedRows() :
                DataModel.GetInstance(DataModelType.ResettleSubPremisesToAssocDataModel).FilterDeletedRows();
            var resettleBuildings = from resettleBuildingsRow in resettleBuildingsAssoc
                                    join buildingsRow in buildings
                                    on resettleBuildingsRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                    join kladrRow in kladrStreets
                                    on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                                    where (addressParts.Count() == 1) ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant()) :
                                          (addressParts.Count() >= 2) && ((kladrRow.Field<string>("street_name").ToUpperInvariant().
                                              Contains(addressParts[0].ToUpperInvariant())) &&
                                        (string.Equals(buildingsRow.Field<string>("house"), addressParts[1], StringComparison.InvariantCultureIgnoreCase)))
                                    select resettleBuildingsRow.Field<int>("id_process");
            var resettlePremises = from resettlePremisesRow in resettlePremisesAssoc
                                   join premisesRow in premises
                                   on resettlePremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                   join buildingsRow in buildings
                                   on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                   join kladrRow in kladrStreets
                                    on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                                   where (addressParts.Count() == 1) ? (kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) :
                                         (addressParts.Count() == 2) ? (kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         (string.Equals(buildingsRow.Field<string>("house"), addressParts[1], StringComparison.InvariantCultureIgnoreCase)) :
                                         (addressParts.Count() == 3) && ((kladrRow.Field<string>("street_name").ToUpperInvariant().
                                             Contains(addressParts[0].ToUpperInvariant())) &&
                                        (string.Equals(buildingsRow.Field<string>("house"), addressParts[1], StringComparison.InvariantCultureIgnoreCase)) &&
                                        (string.Equals(premisesRow.Field<string>("premises_num"), addressParts[2], StringComparison.InvariantCultureIgnoreCase)))
                                   select resettlePremisesRow.Field<int>("id_process");
            var resettleSubPremises = from resettleSubPremisesRow in resettleSubPremisesAssoc
                                       join subPremisesRow in subPremises
                                       on resettleSubPremisesRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                                       join premisesRow in premises
                                       on subPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                       join buildingsRow in buildings
                                       on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                       join kladrRow in kladrStreets
                                       on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                                       where (addressParts.Count() == 1) ? (kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) :
                                         (addressParts.Count() == 2) ? (kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant())) &&
                                         (string.Equals(buildingsRow.Field<string>("house"), addressParts[1], StringComparison.InvariantCultureIgnoreCase)) :
                                         (addressParts.Count() == 3) && ((kladrRow.Field<string>("street_name").ToUpperInvariant().
                                             Contains(addressParts[0].ToUpperInvariant())) &&
                                        (string.Equals(buildingsRow.Field<string>("house"), addressParts[1], StringComparison.InvariantCultureIgnoreCase)) &&
                                        (string.Equals(premisesRow.Field<string>("premises_num"), addressParts[2], StringComparison.InvariantCultureIgnoreCase)))
                                       select resettleSubPremisesRow.Field<int>("id_process");
            return resettleBuildings.Union(resettlePremises).Union(resettleSubPremises);
        }

        public enum ConditionType { BuildingCondition, PremisesCondition };

        public static IEnumerable<int> TenancyProcessIDsByCondition(Func<DataRow, bool> condition, ConditionType conditionType)
        {
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
            var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
            var tenancyBuildingsAssoc = DataModel.GetInstance(DataModelType.TenancyBuildingsAssocDataModel).FilterDeletedRows();
            var tenancyPremisesAssoc = DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).FilterDeletedRows();
            var tenancySubPremisesAssoc = DataModel.GetInstance(DataModelType.TenancySubPremisesAssocDataModel).FilterDeletedRows();
            var tenancyBuildings = from tenancyBuildingsRow in tenancyBuildingsAssoc
                                    join buildingsRow in buildings
                                    on tenancyBuildingsRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                    where
                                    (conditionType != ConditionType.PremisesCondition) && condition(buildingsRow)
                                    select tenancyBuildingsRow.Field<int>("id_process");
            var tenancyPremises = from tenancyPremisesRow in tenancyPremisesAssoc
                                   join premisesRow in premises
                                   on tenancyPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                   join buildingsRow in buildings
                                   on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                   where (conditionType == ConditionType.PremisesCondition) ? condition(premisesRow) : condition(buildingsRow)
                                   select tenancyPremisesRow.Field<int>("id_process");
            var tenancySubPremises = from tenancySubPremisesRow in tenancySubPremisesAssoc
                                       join subPremisesRow in subPremises
                                       on tenancySubPremisesRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                                       join premisesRow in premises
                                       on subPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                       join buildingsRow in buildings
                                       on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                       where (conditionType == ConditionType.PremisesCondition) ? condition(premisesRow) : condition(buildingsRow)
                                       select tenancySubPremisesRow.Field<int>("id_process");
            return tenancyBuildings.Union(tenancyPremises).Union(tenancySubPremises);
        }

        public static IEnumerable<int> ResettleProcessIDsByCondition(Func<DataRow, bool> condition, ConditionType conditionType, ResettleEstateObjectWay way)
        {
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
            var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
            var resettleBuildingsAssoc = way == ResettleEstateObjectWay.From ?
                DataModel.GetInstance(DataModelType.ResettleBuildingsFromAssocDataModel).FilterDeletedRows() :
                DataModel.GetInstance(DataModelType.ResettleBuildingsToAssocDataModel).FilterDeletedRows();
            var resettlePremisesAssoc = way == ResettleEstateObjectWay.From ?
                DataModel.GetInstance(DataModelType.ResettlePremisesFromAssocDataModel).FilterDeletedRows() :
                DataModel.GetInstance(DataModelType.ResettlePremisesToAssocDataModel).FilterDeletedRows();
            var resettleSubPremisesAssoc = way == ResettleEstateObjectWay.From ?
                DataModel.GetInstance(DataModelType.ResettleSubPremisesFromAssocDataModel).FilterDeletedRows() :
                DataModel.GetInstance(DataModelType.ResettleSubPremisesToAssocDataModel).FilterDeletedRows();
            var resettleBuildings = from resettleBuildingsRow in resettleBuildingsAssoc
                                    join buildingsRow in buildings
                                    on resettleBuildingsRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                    where
                                    (conditionType != ConditionType.PremisesCondition) && condition(buildingsRow)
                                    select resettleBuildingsRow.Field<int>("id_process");
            var resettlePremises = from resettlePremisesRow in resettlePremisesAssoc
                                   join premisesRow in premises
                                   on resettlePremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                   join buildingsRow in buildings
                                   on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                   where (conditionType == ConditionType.PremisesCondition) ? condition(premisesRow) : condition(buildingsRow)
                                   select resettlePremisesRow.Field<int>("id_process");
            var resettleSubPremises = from resettleSubPremisesRow in resettleSubPremisesAssoc
                                       join subPremisesRow in subPremises
                                       on resettleSubPremisesRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                                       join premisesRow in premises
                                       on subPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                       join buildingsRow in buildings
                                       on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                       where (conditionType == ConditionType.PremisesCondition) ? condition(premisesRow) : condition(buildingsRow)
                                       select resettleSubPremisesRow.Field<int>("id_process");
            return resettleBuildings.Union(resettlePremises).Union(resettleSubPremises);
        }

        public static IEnumerable<int> TenancyProcessIDsByBuildingId(int id)
        {
            var tenancyBuildingsAssoc = DataModel.GetInstance(DataModelType.TenancyBuildingsAssocDataModel).FilterDeletedRows();
            var tenancyBuildings = from tenancyBuildingsRow in tenancyBuildingsAssoc
                                    where tenancyBuildingsRow.Field<int?>("id_building") == id
                                    select tenancyBuildingsRow.Field<int>("id_process");
            return tenancyBuildings;
        }

        public static IEnumerable<int> TenancyProcessIDsByPremisesId(int id)
        {
            var tenancyPremisesAssoc = DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).FilterDeletedRows();
            var tenancyPremises = from tenancyPremisesRow in tenancyPremisesAssoc
                                    where tenancyPremisesRow.Field<int?>("id_premises") == id
                                    select tenancyPremisesRow.Field<int>("id_process");
            return tenancyPremises;
        }

        public static IEnumerable<int> TenancyProcessIDsBySubPremisesId(int id)
        {
            var tenancySubPremisesAssoc = DataModel.GetInstance(DataModelType.TenancySubPremisesAssocDataModel).FilterDeletedRows();
            var tenancySubPremises = from tenancySubPremisesRow in tenancySubPremisesAssoc
                                   where tenancySubPremisesRow.Field<int?>("id_sub_premises") == id
                                   select tenancySubPremisesRow.Field<int>("id_process");
            return tenancySubPremises;
        }

        public static IEnumerable<int> BuildingIDsByRegistrationNumber(string number)
        {
            var tenancyBuildingsAssoc = DataModel.GetInstance(DataModelType.TenancyBuildingsAssocDataModel).FilterDeletedRows();
            var tenancyProcesses = DataModel.GetInstance(DataModelType.TenancyProcessesDataModel).FilterDeletedRows();
            return (from tenancyProcessesRow in tenancyProcesses
                    join tenancyBuildingsAssocRow in tenancyBuildingsAssoc
                    on tenancyProcessesRow.Field<int>("id_process") equals tenancyBuildingsAssocRow.Field<int>("id_process")
                    where tenancyProcessesRow.Field<string>("registration_num") == number
                    select tenancyBuildingsAssocRow.Field<int>("id_building"));
        }

        public static IEnumerable<int> PremiseIDsByRegistrationNumber(string number)
        {
            var tenancyPremisesAssoc = DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).FilterDeletedRows();
            var tenancyProcesses = DataModel.GetInstance(DataModelType.TenancyProcessesDataModel).FilterDeletedRows();
            return (from tenancyProcessesRow in tenancyProcesses
                    join tenancyPremisesAssocRow in tenancyPremisesAssoc
                    on tenancyProcessesRow.Field<int>("id_process") equals tenancyPremisesAssocRow.Field<int>("id_process")
                    where tenancyProcessesRow.Field<string>("registration_num") == number
                    select tenancyPremisesAssocRow.Field<int>("id_premises"));
        }

        public static IEnumerable<int> BuildingIDsByCurrentFund(int idFund)
        {
            // Ищем здания указанного фонда, а также здания, в которых присутствуют помещения и комнаты указанного фонда
            var buildingsFunds = CalcDataModelBuildingsCurrentFunds.GetInstance().Select();
            var premisesFunds = CalcDataModelPremisesCurrentFunds.GetInstance().Select();
            var subPremisesFunds = CalcDataModelSubPremisesCurrentFunds.GetInstance().Select();
            var premisesIds = from subPremisesFundsRow in subPremisesFunds.AsEnumerable()
                              join subPremisesRow in DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows()
                              on subPremisesFundsRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                              where new [] { 1, 4, 5, 9 }.Contains(subPremisesRow.Field<int>("id_state")) &&
                              subPremisesFundsRow.Field<int>("id_fund_type") == idFund
                              select subPremisesRow.Field<int>("id_premises");
            var buildingsIds = from premisesFundsRow in premisesFunds.AsEnumerable()
                               join premisesRow in DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows()
                               on premisesFundsRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                               where new [] { 1, 4, 5, 9 }.Contains(premisesRow.Field<int>("id_state")) &&
                                premisesFundsRow.Field<int>("id_fund_type") == idFund ||
                                (premisesFundsRow.Field<int>("id_fund_type") == 4 && premisesIds.Contains(premisesFundsRow.Field<int>("id_premises")))
                               select premisesRow.Field<int>("id_building");
            return (from buildingsFundsRow in buildingsFunds.AsEnumerable()
                    join buildingsRow in DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows()
                              on buildingsFundsRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                    where new [] { 1, 4, 5, 9 }.Contains(buildingsRow.Field<int>("id_state")) && 
                    buildingsFundsRow.Field<int>("id_fund_type") == idFund ||
                                   (buildingsFundsRow.Field<int>("id_fund_type") == 4 && buildingsIds.Contains(buildingsFundsRow.Field<int>("id_building")))
                    select buildingsFundsRow.Field<int>("id_building"));
        }

        public static IEnumerable<int> PremiseIDsByCurrentFund(int idFund)
        {
            // Ищем помещения указанного фонда, а также помещения, в которых присутствуют комнаты указанного фонда
            var premisesFunds = CalcDataModelPremisesCurrentFunds.GetInstance().Select();
            var subPremisesFunds = CalcDataModelSubPremisesCurrentFunds.GetInstance().Select();
            var premisesIds = from subPremisesFundsRow in subPremisesFunds.AsEnumerable()
                              join subPremisesRow in DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows()
                              on subPremisesFundsRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                              where new [] { 1, 4, 5, 9 }.Contains(subPremisesRow.Field<int>("id_state")) &&
                              subPremisesFundsRow.Field<int>("id_fund_type") == idFund
                              select subPremisesRow.Field<int>("id_premises");
            return (from premisesFundsRow in premisesFunds.AsEnumerable()
                    join premisesRow in DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows()
                               on premisesFundsRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                    where new [] { 1, 4, 5, 9 }.Contains(premisesRow.Field<int>("id_state")) && 
                        premisesFundsRow.Field<int>("id_fund_type") == idFund ||
                        (premisesFundsRow.Field<int>("id_fund_type") == 4 && premisesIds.Contains(premisesFundsRow.Field<int>("id_premises")))
                    select premisesFundsRow.Field<int>("id_premises"));
        }

        public static IEnumerable<int> BuildingIDsByRegion(string region)
        {
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            return (from buildingRow in buildings
                    where (buildingRow.Field<string>("id_street").StartsWith(region, StringComparison.Ordinal))
                    select buildingRow.Field<int>("id_building"));
        }

        public static IEnumerable<int> BuildingIDsByStreet(string street)
        {
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            return (from buildingRow in buildings
                    where (buildingRow.Field<string>("id_street") == street)
                    select buildingRow.Field<int>("id_building"));
        }

        public static IEnumerable<int> BuildingIDsByHouse(string house)
        {
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            return (from buildingRow in buildings
                    where (buildingRow.Field<string>("house").ToUpper().Contains(house.ToUpper()))
                    select buildingRow.Field<int>("id_building"));
        }

        public static IEnumerable<int> ClaimStateTypeIdsByNextStateType(int nextStateType)
        {
            var claimStateTypes = DataModel.GetInstance(DataModelType.ClaimStateTypesDataModel).FilterDeletedRows();
            var claimStateTypeRelations = DataModel.GetInstance(DataModelType.ClaimStateTypesRelationsDataModel).FilterDeletedRows();
            return (from claimStateTypesRow in claimStateTypes
                    join claimStateTypesRelRow in claimStateTypeRelations
                    on claimStateTypesRow.Field<int>("id_state_type") equals claimStateTypesRelRow.Field<int>("id_state_from")
                    where Convert.ToBoolean(claimStateTypesRow.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture) &&
                        (claimStateTypesRelRow.Field<int>("id_state_to") == nextStateType)
                    select claimStateTypesRow.Field<int>("id_state_type"));
        }

        public static IEnumerable<int> ClaimStateTypeIdsByPrevStateType(int prevStateType)
        {
            var claimStateTypes = DataModel.GetInstance(DataModelType.ClaimStateTypesDataModel).FilterDeletedRows();
            var claimStateTypeRelations = DataModel.GetInstance(DataModelType.ClaimStateTypesRelationsDataModel).FilterDeletedRows();
            return (from claimStateTypesRow in claimStateTypes
                    join claimStateTypesRelRow in claimStateTypeRelations
                    on claimStateTypesRow.Field<int>("id_state_type") equals claimStateTypesRelRow.Field<int>("id_state_from")
                    where claimStateTypesRelRow.Field<int>("id_state_from") == prevStateType
                    select claimStateTypesRelRow.Field<int>("id_state_to"));
        }

        public static IEnumerable<int> ClaimStateTypeIdsByNextAndPrevStateTypes(int nextStateType, int prevStateType)
        {
            var claimStateTypeRelations = DataModel.GetInstance(DataModelType.ClaimStateTypesRelationsDataModel).FilterDeletedRows();
            var fromStates = from claimStateTypesRelRow in claimStateTypeRelations
                              where claimStateTypesRelRow.Field<int>("id_state_from") == prevStateType
                              select claimStateTypesRelRow.Field<int>("id_state_to");
            var toStates = from claimStateTypesRelRow in claimStateTypeRelations
                            where claimStateTypesRelRow.Field<int>("id_state_to") == nextStateType
                            select claimStateTypesRelRow.Field<int>("id_state_from");
            return fromStates.Intersect(toStates);
        }

        public static IEnumerable<int> ClaimStartStateTypeIds()
        {
            var claimStateTypes = DataModel.GetInstance(DataModelType.ClaimStateTypesDataModel).FilterDeletedRows();
            return (from claimStateTypesRow in claimStateTypes
                    where Convert.ToBoolean(claimStateTypesRow.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture)
                    select claimStateTypesRow.Field<int>("id_state_type"));
        }
    
        public static bool TenancyProcessHasTenant(int idProcess)
        {
            var persons = DataModel.GetInstance(DataModelType.TenancyPersonsDataModel).FilterDeletedRows();
            return (from personsRow in persons
                    where (personsRow.Field<int?>("id_kinship") == 1) && (personsRow.Field<int>("id_process") == idProcess
                        && (personsRow.Field<DateTime?>("exclude_date") == null))
                    select personsRow).Any();
        }

        public static int TenancyAgreementsForProcess(int idProcess)
        {
            var agreements = DataModel.GetInstance(DataModelType.TenancyAgreementsDataModel).FilterDeletedRows();
            return (from agreementsRow in agreements
                    where agreementsRow.Field<int>("id_process") == idProcess
                    select agreementsRow).Count();
        }   

        public static int BuildingsDuplicateCount(Building building)
        {
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            return (from buildingsRow in buildings
                    where buildingsRow.Field<string>("id_street") == building.IdStreet &&
                          buildingsRow.Field<string>("house") == building.House &&
                          buildingsRow.Field<int>("id_building") != building.IdBuilding
                    select buildingsRow).Count();
        }

        public static int PremisesDuplicateCount(Premise premise)
        {
            var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
            return (from premisesRow in premises
                    where premisesRow.Field<int>("id_building") == premise.IdBuilding &&
                          premisesRow.Field<string>("premises_num") == premise.PremisesNum &&
                          premisesRow.Field<int>("id_premises") != premise.IdPremises
                    select premisesRow).Count();
        }

        public static int TenancyProcessesDuplicateCount(TenancyProcess process)
        {
            var tenancyProcesses = DataModel.GetInstance(DataModelType.TenancyProcessesDataModel).FilterDeletedRows();
            return (from tenancyProcessesRow in tenancyProcesses
                    where tenancyProcessesRow.Field<string>("registration_num") == process.RegistrationNum &&
                          tenancyProcessesRow.Field<int>("id_process") != process.IdProcess
                    select tenancyProcessesRow).Count();
        }

        public static IEnumerable<int> ObjectIdsByStates(EntityType entity, int[] states)
        {
            if (entity == EntityType.Building)
            {
                var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
                var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
                var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
                var result = from buildingsRow in buildings
                             join premisesRow in premises
                             on buildingsRow.Field<int>("id_building") equals premisesRow.Field<int>("id_building") into bp
                             from bpRow in bp.DefaultIfEmpty()
                             join subPremisesRow in subPremises
                             on bpRow.Field<int?>("id_premises") equals subPremisesRow.Field<int?>("id_premises") into ps
                             from psRow in ps.DefaultIfEmpty()
                             where (bpRow != null && states.Contains(bpRow.Field<int>("id_state"))) ||
                                   (psRow != null && states.Contains(psRow.Field<int>("id_state")))
                             select buildingsRow.Field<int>("id_building");
                return result;
            }
            else
            {
                var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
                var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
                var result = from premisesRow in premises
                             join subPremisesRow in subPremises
                             on premisesRow.Field<int>("id_premises") equals subPremisesRow.Field<int>("id_premises") into ps
                             from psRow in ps.DefaultIfEmpty()
                             where (psRow != null && states.Contains(psRow.Field<int>("id_state")))
                             select premisesRow.Field<int>("id_premises");
                return result;
            }
        }

        private static bool HasObjectState(int id, EntityType entity, int[] states)
        {
            switch (entity)
            {
                case EntityType.Building:
                {
                    var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
                    var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
                    var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
                    var mBuilding = (from buildingRow in buildings
                        where buildingRow.Field<int>("id_building") == id &&
                              states.Contains(buildingRow.Field<int>("id_state"))
                        select buildingRow).Any();
                    var mPremises = (from premisesRow in premises
                        where premisesRow.Field<int>("id_building") == id &&
                              states.Contains(premisesRow.Field<int>("id_state"))
                        select premisesRow).Any();
                    var mSubPremises = (from premisesRow in premises
                        join subPremisesRow in subPremises
                            on premisesRow.Field<int>("id_premises") equals subPremisesRow.Field<int>("id_premises")
                        where premisesRow.Field<int>("id_building") == id &&
                              states.Contains(subPremisesRow.Field<int>("id_state"))
                        select subPremisesRow).Any();
                    return mBuilding || mPremises || mSubPremises;
                }
                case EntityType.Premise:
                {
                    var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
                    var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
                    var mPremises = (from premisesRow in premises
                        where premisesRow.Field<int>("id_premises") == id &&
                              states.Contains(premisesRow.Field<int>("id_state"))
                        select premisesRow).Any();
                    var mSubPremises = (from subPremisesRow in subPremises
                        where subPremisesRow.Field<int>("id_premises") == id &&
                              states.Contains(subPremisesRow.Field<int>("id_state"))
                        select subPremisesRow).Any();
                    return mPremises || mSubPremises;
                }
                case EntityType.SubPremise:
                {
                    var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
                    return (from subPremisesRow in subPremises
                        where subPremisesRow.Field<int>("id_sub_premises") == id &&
                              states.Contains(subPremisesRow.Field<int>("id_state"))
                        select subPremisesRow).Any();
                }
                default:
                    return false;
            }
        }

        public static bool HasMunicipal(int id, EntityType entity)
        {
            return HasObjectState(id, entity, new [] { 4, 5, 9 });
        }

        public static bool HasNotMunicipal(int id, EntityType entity)
        {
            return HasObjectState(id, entity, new [] { 1, 3 });
        }

        public static IEnumerable<int> DemolishedBuildingIDs()
        {
            var ownershipRights = DataModel.GetInstance(DataModelType.OwnershipsRightsDataModel).FilterDeletedRows();
            var ownershipBuildingdsAssoc = DataModel.GetInstance(DataModelType.OwnershipBuildingsAssocDataModel).FilterDeletedRows();
            return from ownershipBuildingsAssocRow in ownershipBuildingdsAssoc
                   join ownershipRightsRow in ownershipRights
                   on ownershipBuildingsAssocRow.Field<int>("id_ownership_right") equals ownershipRightsRow.Field<int>("id_ownership_right")
                   where ownershipRightsRow.Field<int>("id_ownership_right_type") == 1
                   select ownershipBuildingsAssocRow.Field<int>("id_building");
        }

        public static IEnumerable<int> DemolishedPremisesIDs()
        {
            var ownershipRights = DataModel.GetInstance(DataModelType.OwnershipsRightsDataModel).FilterDeletedRows();
            var ownershipPremisesAssoc = DataModel.GetInstance(DataModelType.OwnershipPremisesAssocDataModel).FilterDeletedRows();
            return from ownershipPremisesAssocRow in ownershipPremisesAssoc
                   join ownershipRightsRow in ownershipRights
                   on ownershipPremisesAssocRow.Field<int>("id_ownership_right") equals ownershipRightsRow.Field<int>("id_ownership_right")
                   where ownershipRightsRow.Field<int>("id_ownership_right_type") == 1
                   select ownershipPremisesAssocRow.Field<int>("id_premises");
        }

        public static IEnumerable<int> BuildingIDsByOwnershipType(int idOwnershipType)
        {
            var ownershipRights = DataModel.GetInstance(DataModelType.OwnershipsRightsDataModel).FilterDeletedRows();
            var ownershipBuildingdsAssoc = DataModel.GetInstance(DataModelType.OwnershipBuildingsAssocDataModel).FilterDeletedRows();
            // Если используется непользовательское ограничение "Аварийное", то не выводить здания еще и снесеные
            var demolishedBuildings = DemolishedBuildingIDs();
            return from ownershipRightsRow in ownershipRights
                   join ownershipBuildingdsAssocRow in ownershipBuildingdsAssoc
                   on ownershipRightsRow.Field<int>("id_ownership_right") equals ownershipBuildingdsAssocRow.Field<int>("id_ownership_right")
                   where (idOwnershipType != 2 || !demolishedBuildings.Contains(ownershipBuildingdsAssocRow.Field<int>("id_building"))) && 
                        ownershipRightsRow.Field<int>("id_ownership_right_type") == idOwnershipType
                   select ownershipBuildingdsAssocRow.Field<int>("id_building");
        }

        public static IEnumerable<int> PremiseIDsByOwnershipType(int idOwnershipType)
        {
            var ownershipRights = DataModel.GetInstance(DataModelType.OwnershipsRightsDataModel).FilterDeletedRows();
            var ownershipPremisesAssoc = DataModel.GetInstance(DataModelType.OwnershipPremisesAssocDataModel).FilterDeletedRows();
            var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
            var buildingdIds = BuildingIDsByOwnershipType(idOwnershipType);
            // Если используется ограничение "Аварийное", то не выводить помещения, если они или их здание имеют ограничение "Снесено"
            var demolishedPremises = DemolishedPremisesIDs();
            //Выбираются помещения с установленным ограничением и помещения, находящиеся в зданиях с установленным ограничением
            var premisesIds = from ownershipRightsRow in ownershipRights
                                   join ownershipPremisesAssocRow in ownershipPremisesAssoc
                                   on ownershipRightsRow.Field<int>("id_ownership_right") equals ownershipPremisesAssocRow.Field<int>("id_ownership_right")
                                   where (idOwnershipType != 2 || !demolishedPremises.Contains(ownershipPremisesAssocRow.Field<int>("id_premises"))) &&
                                        ownershipRightsRow.Field<int>("id_ownership_right_type") == idOwnershipType
                                   select ownershipPremisesAssocRow.Field<int>("id_premises");


            return
                from premisesRow in premises
                join buildingId in buildingdIds on premisesRow.Field<int>("id_building") equals buildingId into bids
                from bid in bids.DefaultIfEmpty()
                join premisesId in premisesIds on premisesRow.Field<int>("id_premises") equals premisesId into pids
                from pid in pids.DefaultIfEmpty()
                where bid != 0 || pid != 0
                select premisesRow.Field<int>("id_premises");
        }

        /// <summary>
        /// Объекты недвижимости, исключенные из муниципальной собственности
        /// </summary>
        /// <param name="objectAssocDataRows">Список строк из ассоциативной модели реквизитов</param>
        /// <param name="entity">Тип ассоциативной модели</param>
        /// <returns>Перечень объектов</returns>
        public static IEnumerable<RestrictionObjectAssoc> ObjectIDsExcludedFromMunicipal(IEnumerable<DataRow> objectAssocDataRows, EntityType entity)
        {
            var restrictions = DataModel.GetInstance(DataModelType.RestrictionsDataModel).FilterDeletedRows();
            string fieldName = null;
            switch (entity)
            {
                case EntityType.Premise:
                    fieldName = "id_premises";
                    break;
                case EntityType.Building:
                    fieldName = "id_building";
                    break;
                case EntityType.SubPremise:
                    fieldName = "id_sub_premises";
                    break;
            }
            var restrictionsMaxDate = from restrictionsAssocRow in objectAssocDataRows
                                                 join restrictionsRow in restrictions
                                                 on restrictionsAssocRow.Field<int>("id_restriction") equals restrictionsRow.Field<int>("id_restriction")
                                                 where new[] { 1, 2 }.Contains(restrictionsRow.Field<int>("id_restriction_type"))
                                                 group restrictionsRow.Field<DateTime>("date") by restrictionsAssocRow.Field<int>(fieldName) into gs
                                                 select new
                                                 {
                                                     id = gs.Key,
                                                     date = gs.Max()
                                                 };
            var excludeFromMunicipal = from restrictionsRow in restrictions
                                         join restrictionsAssocRow in objectAssocDataRows
                                         on restrictionsRow.Field<int>("id_restriction")
                                         equals restrictionsAssocRow.Field<int>("id_restriction")
                                         join rmdRow in restrictionsMaxDate
                                         on new
                                         {
                                             id = restrictionsAssocRow.Field<int>(fieldName),
                                             date = restrictionsRow.Field<DateTime>("date")
                                         } equals
                                         new
                                         {
                                             rmdRow.id, rmdRow.date
                                         }
                                         where restrictionsRow.Field<int>("id_restriction_type") == 2
                                         select new RestrictionObjectAssoc(restrictionsAssocRow.Field<int?>(fieldName), restrictionsRow.Field<int?>("id_restriction"), restrictionsRow.Field<DateTime?>("date"));
            return excludeFromMunicipal;
        }

        /// <summary>
        /// Объекты недвижимости, включенные в муниципальную собственность
        /// </summary>
        /// <param name="objectAssocDataRows">Список строк из ассоциативной модели реквизитов</param>
        /// <param name="entity">Тип ассоциативной модели</param>
        /// <returns>Перечень объектов</returns>
        public static IEnumerable<RestrictionObjectAssoc> ObjectIDsIncludedIntoMunicipal(IEnumerable<DataRow> objectAssocDataRows, EntityType entity)
        {
            var restrictions = DataModel.GetInstance(DataModelType.RestrictionsDataModel).FilterDeletedRows();
            string fieldName = null;
            switch (entity)
            {
                case EntityType.Premise:
                    fieldName = "id_premises";
                    break;
                case EntityType.Building:
                    fieldName = "id_building";
                    break;
                case EntityType.SubPremise:
                    fieldName = "id_sub_premises";
                    break;
            }
            if (fieldName == null)
                throw new DataModelException("Не указано название поля идентификатора в методе ObjectIDsIncludedIntoMunicipal");
            var restrictionsMaxDate = from restrictionsAssocRow in objectAssocDataRows
                                        join restrictionsRow in restrictions
                                        on restrictionsAssocRow.Field<int>("id_restriction") equals restrictionsRow.Field<int>("id_restriction")
                                        where new [] { 1, 2 }.Contains(restrictionsRow.Field<int>("id_restriction_type"))
                                        group restrictionsRow.Field<DateTime>("date") by restrictionsAssocRow.Field<int>(fieldName) into gs
                                        select new
                                        {
                                            id = gs.Key,
                                            date = gs.Max()
                                        };
            var includedIntoMunicipal = from restrictionsRow in restrictions
                                         join restrictionsAssocRow in objectAssocDataRows
                                         on restrictionsRow.Field<int>("id_restriction")
                                         equals restrictionsAssocRow.Field<int>("id_restriction")
                                         join rmdRow in restrictionsMaxDate
                                         on new
                                         {
                                             id = restrictionsAssocRow.Field<int>(fieldName),
                                             date = restrictionsRow.Field<DateTime>("date")
                                         } equals
                                         new
                                         {
                                             rmdRow.id, rmdRow.date
                                         }
                                         where restrictionsRow.Field<int>("id_restriction_type") == 1
                                         select new RestrictionObjectAssoc(restrictionsAssocRow.Field<int?>(fieldName), restrictionsRow.Field<int?>("id_restriction"), restrictionsRow.Field<DateTime?>("date"));
            return includedIntoMunicipal;
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
            switch (entity)
            {
                case EntityType.Premise:
                    fieldName = "id_premises";
                    break;
                case EntityType.Building:
                    fieldName = "id_building";
                    break;
                case EntityType.SubPremise:
                    fieldName = "id_sub_premises";
                    break;
            }
            if (fieldName == null)
                throw new DataModelException("Не указано название поля идентификатора в методе ObjectIDsIncludedIntoMunicipal");
            var fundsHistory = DataModel.GetInstance(DataModelType.FundsHistoryDataModel).FilterDeletedRows();
            var maxIdByObject = from assocRow in objectAssocDataRows
                                     join fundHistoryRow in fundsHistory
                                        on assocRow.Field<int>("id_fund") equals fundHistoryRow.Field<int>("id_fund")
                                     where fundHistoryRow.Field<DateTime?>("exclude_restriction_date") == null
                                     group assocRow.Field<int>("id_fund") by
                                             assocRow.Field<int>(fieldName) into gs
                                     select new FundObjectAssoc(gs.Key, gs.Max());
            return maxIdByObject;
        }

        /// <summary>
        /// Агрегатор адреса для различных процессов: найма, приватизации, переселения и т.д.
        /// </summary>
        /// <param name="assocBuildings">Ассоциативная модель зданий</param>
        /// <param name="assocPremises">Ассоциативная модель помещений</param>
        /// <param name="assocSubPremises">Ассоциативная модель комнат</param>
        /// <returns>Возвращает перечисление вида id_process, address</returns>
        public static IEnumerable<AggregatedAddress> AggregateAddressByIdProcess(IEnumerable<DataRow> assocBuildings, IEnumerable<DataRow> assocPremises, IEnumerable<DataRow> assocSubPremises)
        {
            if (assocBuildings == null || assocPremises == null || assocSubPremises == null)
                throw new DataModelException("Не переданы все ссылки на ассоциативные модели");
            var kladrStreet = DataModel.GetInstance(DataModelType.KladrStreetsDataModel).FilterDeletedRows();
            var buildings = DataModel.GetInstance(DataModelType.BuildingsDataModel).FilterDeletedRows();
            var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
            var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
            var aSubPremisesGc = from assocSubPremisesRow in assocSubPremises
                                    join subPremisesRow in subPremises
                                    on assocSubPremisesRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                                    join premisesRow in premises.AsEnumerable()
                                    on subPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                    group subPremisesRow.Field<string>("sub_premises_num") by
                                        new
                                        {
                                            id_process = assocSubPremisesRow.Field<int>("id_process"),
                                            id_building = premisesRow.Field<int>("id_building"),
                                            id_premises = subPremisesRow.Field<int>("id_premises"),
                                            id_premises_type = premisesRow.Field<int>("id_premises_type"),
                                            premises_num = premisesRow.Field<string>("premises_num")
                                        } into gs
                                    select new
                                    {
                                        gs.Key.id_process, gs.Key.id_building, gs.Key.id_premises,
                                        result_str = (gs.Key.id_premises_type == 2 ? " ком. " : (gs.Key.id_premises_type == 4 ? " пом. " : " кв. ")) + 
                                            gs.Key.premises_num + ((gs.Any()) ?
                                        (" ком. " + gs.Aggregate((a, b) => a + ", " + b)) : "")
                                    };
            var aPremises = from assocPremisesRow in assocPremises
                             join premisesRow in premises
                             on assocPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                             select new
                             {
                                 id_process = assocPremisesRow.Field<int>("id_process"),
                                 id_building = premisesRow.Field<int>("id_building"),
                                 id_premises = assocPremisesRow.Field<int>("id_premises"),
                                 result_str = (premisesRow.Field<int>("id_premises_type") == 2 ? " ком. " :
                                    (premisesRow.Field<int>("id_premises_type") == 4 ? " пом. " : " кв. ")) + premisesRow.Field<string>("premises_num")
                             };
            var aBuildings = from assocBuildingsRow in assocBuildings
                              select new
                              {
                                  id_process = assocBuildingsRow.Field<int>("id_process"),
                                  id_building = assocBuildingsRow.Field<int>("id_building"),
                                  result_str = ""
                              };
            var assocPremisesGc = from assocPremisesRow in aPremises.Union(aSubPremisesGc)
                                      join premisesRow in premises
                                      on assocPremisesRow.id_premises equals premisesRow.Field<int>("id_premises")
                                      group
                                          assocPremisesRow.result_str
                                          by new
                                          {
                                              assocPremisesRow.id_process,
                                              assocPremisesRow.id_building
                                          } into gs
                                      select new
                                      {
                                          gs.Key.id_process, gs.Key.id_building,
                                          result_str = gs.Aggregate((a, b) => a + ", " + b.Trim())
                                      };
            var addresses = from aBuildingsRow in aBuildings.Union(assocPremisesGc)
                            join buildingsRow in buildings
                            on aBuildingsRow.id_building equals buildingsRow.Field<int>("id_building")
                            join kladrRow in kladrStreet
                            on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                            group
                               kladrRow.Field<string>("street_name") + ", дом " + buildingsRow.Field<string>("house") + aBuildingsRow.result_str
                            by aBuildingsRow.id_process into gs
                            select new AggregatedAddress(gs.Key, gs.Aggregate((a, b) => a + ", " + b.Trim()));
            return addresses;
        }
    
        /// <summary>
        /// Возвращает список идентификаторов старых процессов найма
        /// </summary>
        /// <returns>Список идентификаторов</returns>
        public static IEnumerable<int> OldTenancyProcesses()
        {
            // Собираем строку проверки уникальности сдаваемой группы помещений. Выглядеть эта строка будет примерно подобным образом з1з12п23п122к32 и т.п.
            var assocSubPremises = from assocSubPremisesRow in DataModel.GetInstance(DataModelType.TenancySubPremisesAssocDataModel).FilterDeletedRows()
                                     group assocSubPremisesRow.Field<int>("id_sub_premises").ToString(CultureInfo.InvariantCulture)
                                     by assocSubPremisesRow.Field<int>("id_process") into gs
                                     select new
                                     {
                                         id_process = gs.Key,
                                         value = gs.Count() > 1 ? gs.OrderBy(val => val).Aggregate((str1, str2) => 'к'+str1+'к'+str2) : 'к'+gs.First()
                                     };
            var assocPremises = from assocPremisesRow in DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).FilterDeletedRows()
                                     group assocPremisesRow.Field<int>("id_premises").ToString(CultureInfo.InvariantCulture)
                                     by assocPremisesRow.Field<int>("id_process") into gs
                                     select new
                                     {
                                         id_process = gs.Key,
                                         value = gs.Count() > 1 ? gs.OrderBy(val => val).Aggregate((str1, str2) => 'п' + str1 + 'п' + str2) : 'п' + gs.First()
                                     };
            var assocBuildings = from assocBuildingsRow in DataModel.GetInstance(DataModelType.TenancyBuildingsAssocDataModel).FilterDeletedRows()
                                  group assocBuildingsRow.Field<int>("id_building").ToString(CultureInfo.InvariantCulture)
                                 by assocBuildingsRow.Field<int>("id_process") into gs
                                 select new
                                 {
                                     id_process = gs.Key,
                                     value = gs.Count() > 1 ? gs.OrderBy(val => val).Aggregate((str1, str2) => 'з' + str1 + 'з' + str2) : 'з' + gs.First()
                                 };
            var assocObjects = assocSubPremises.Union(assocPremises).Union(assocBuildings);
            var identStrings = from assocObjectsRow in assocObjects
                                group assocObjectsRow.value by assocObjectsRow.id_process into gs
                                select new
                                {
                                    id_process = gs.Key,
                                    value = gs.Count() > 1 ? gs.OrderBy(val => val).Aggregate((str1, str2) => str1 + str2) : gs.First()
                                };
            var moreOneIdentStrings = from identRow in identStrings
                                         group identRow.id_process by identRow.value into gs
                                         where gs.Count() > 1
                                         select gs.Key;
            var duplicateProcesses = from identRow in identStrings
                                        join tenancyRow in DataModel.GetInstance(DataModelType.TenancyProcessesDataModel).FilterDeletedRows()
                                        on identRow.id_process equals tenancyRow.Field<int>("id_process")
                                        join moreOneIdentStringsRow in moreOneIdentStrings
                                        on identRow.value equals moreOneIdentStringsRow
                                        where tenancyRow.Field<string>("registration_num") != null
                                        select new
                                        {
                                            id_process = tenancyRow.Field<int>("id_process"),
                                            ident_value = identRow.value,
                                            registration_date = tenancyRow.Field<DateTime?>("registration_date")
                                        };
            var maxDates = from row in duplicateProcesses
                            group row.registration_date by row.ident_value into gs
                            select new
                            {
                                ident_value = gs.Key,
                                max_date = gs.Max()
                            };
            var maxProcessIds = from processRow in duplicateProcesses
                                  join maxDatesRow in maxDates
                                  on new {processRow.ident_value, date = processRow.registration_date } equals
                                  new {maxDatesRow.ident_value, date = maxDatesRow.max_date }
                                  select processRow.id_process;
            var duplicateProcessIds = from row in duplicateProcesses
                                        select row.id_process;
            return duplicateProcessIds.Except(maxProcessIds);
        }
    }
}
