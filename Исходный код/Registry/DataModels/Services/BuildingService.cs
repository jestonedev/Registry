using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.Services
{
    public static class BuildingService
    {
        public static IEnumerable<int> BuildingIdsBySnp(string[] snp, Func<DataRow, bool> condition)
        {
            var tenancyBuildingsAssoc = EntityDataModel<TenancyBuildingAssoc>.GetInstance().FilterDeletedRows();
            var tenancyPersons = EntityDataModel<TenancyPerson>.GetInstance().FilterDeletedRows();
            return
            (from tenancyBuildingsAssocRow in tenancyBuildingsAssoc
             join tenancyPersonsRow in tenancyPersons
             on tenancyBuildingsAssocRow.Field<int?>("id_process") equals tenancyPersonsRow.Field<int?>("id_process")
             where snp.Any() && tenancyPersonsRow.Field<string>("surname") != null && 
                    string.Equals(tenancyPersonsRow.Field<string>("surname"), snp[0], StringComparison.InvariantCultureIgnoreCase) &&
                    ((snp.Length < 2) || tenancyPersonsRow.Field<string>("name") != null &&
                     string.Equals(tenancyPersonsRow.Field<string>("name"), snp[1], StringComparison.InvariantCultureIgnoreCase)) &&
                    ((snp.Length < 3) || tenancyPersonsRow.Field<string>("patronymic") != null &&
                     string.Equals(tenancyPersonsRow.Field<string>("patronymic"), snp[2], StringComparison.InvariantCultureIgnoreCase)) &&
                    condition(tenancyPersonsRow) && tenancyBuildingsAssocRow.Field<int?>("id_building") != null
             select tenancyBuildingsAssocRow.Field<int>("id_building")).Distinct();
        }

        public static IEnumerable<int> BuildingIDsByAddress(string[] addressParts)
        {
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
            var kladrStreets = DataModel.GetInstance<KladrStreetsDataModel>().FilterDeletedRows();
            return from buildingRow in buildings
                join kladrRow in kladrStreets
                    on buildingRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                where addressParts.Length == 1 ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                    Contains(addressParts[0].ToUpperInvariant()) :
                    (addressParts.Length >= 2) && kladrRow.Field<string>("street_name").ToUpperInvariant().
                        Contains(addressParts[0].ToUpperInvariant()) && buildingRow.Field<string>("house").ToUpper().Equals(addressParts[1].ToUpper())
                select buildingRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByRegistrationNumber(string number)
        {
            var tenancyBuildingsAssoc = EntityDataModel<TenancyBuildingAssoc>.GetInstance().FilterDeletedRows();
            var tenancyProcesses = EntityDataModel<TenancyProcess>.GetInstance().FilterDeletedRows();
            return from tenancyProcessesRow in tenancyProcesses
                join tenancyBuildingsAssocRow in tenancyBuildingsAssoc
                    on tenancyProcessesRow.Field<int?>("id_process") equals tenancyBuildingsAssocRow.Field<int?>("id_process")
                where tenancyProcessesRow.Field<string>("registration_num") == number &&
                      tenancyBuildingsAssocRow.Field<int?>("id_building") != null
                select tenancyBuildingsAssocRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByCurrentFund(int idFund)
        {
            // Ищем здания указанного фонда, а также здания, в которых присутствуют помещения и комнаты указанного фонда
            var buildingsFunds = CalcDataModelBuildingsCurrentFunds.GetInstance().Select();
            var premisesFunds = CalcDataModelPremisesCurrentFunds.GetInstance().Select();
            var subPremisesFunds = CalcDataModelSubPremisesCurrentFunds.GetInstance().Select();
            var premisesIds = from subPremisesFundsRow in subPremisesFunds.AsEnumerable()
                              join subPremisesRow in EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows()
                              on subPremisesFundsRow.Field<int?>("id_sub_premises") equals subPremisesRow.Field<int?>("id_sub_premises")
                              where DataModelHelper.MunicipalAndUnknownObjectStates().Contains(subPremisesRow.Field<int?>("id_state") ?? 0) &&
                              subPremisesFundsRow.Field<int?>("id_fund_type") == idFund &&
                              subPremisesRow.Field<int?>("id_premises") != null
                              select subPremisesRow.Field<int>("id_premises");
            var buildingsIds = from premisesFundsRow in premisesFunds.AsEnumerable()
                               join premisesRow in EntityDataModel<Premise>.GetInstance().FilterDeletedRows()
                               on premisesFundsRow.Field<int?>("id_premises") equals premisesRow.Field<int?>("id_premises")
                               where DataModelHelper.MunicipalAndUnknownObjectStates().Contains(premisesRow.Field<int?>("id_state") ?? 0) &&
                                premisesFundsRow.Field<int?>("id_fund_type") == idFund ||
                                premisesFundsRow.Field<int?>("id_fund_type") == 4 && premisesIds.Contains(premisesFundsRow.Field<int?>("id_premises") ?? 0) &&
                                premisesRow.Field<int?>("id_building") != null
                               select premisesRow.Field<int>("id_building");
            return from buildingsFundsRow in buildingsFunds.AsEnumerable()
                   join buildingsRow in DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows()
                    on buildingsFundsRow.Field<int?>("id_building") equals buildingsRow.Field<int?>("id_building")
                   where DataModelHelper.MunicipalAndUnknownObjectStates().Contains(buildingsRow.Field<int?>("id_state") ?? 0) &&
                         buildingsFundsRow.Field<int?>("id_fund_type") == idFund ||
                         buildingsFundsRow.Field<int?>("id_fund_type") == 4 && buildingsIds.Contains(buildingsFundsRow.Field<int?>("id_building") ?? 0) &&
                         buildingsFundsRow.Field<int?>("id_building") != null
                   select buildingsFundsRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByRegion(string region)
        {
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
            return from buildingRow in buildings
                where buildingRow.Field<string>("id_street") != null && buildingRow.Field<string>("id_street").StartsWith(region, StringComparison.Ordinal)
                select buildingRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByStreet(string street)
        {
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
            return from buildingRow in buildings
                where buildingRow.Field<string>("id_street") == street
                select buildingRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByHouse(string house)
        {
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
            return from buildingRow in buildings
                where buildingRow.Field<string>("house") != null && 
                      buildingRow.Field<string>("house").ToUpper().Contains(house.ToUpper())
                select buildingRow.Field<int>("id_building");
        }

        public static int BuildingsDuplicateCount(Building building)
        {
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
            return (from buildingsRow in buildings
                where buildingsRow.Field<string>("id_street") == building.IdStreet &&
                      buildingsRow.Field<string>("house") == building.House &&
                      buildingsRow.Field<int?>("id_building") != building.IdBuilding
                select buildingsRow).Count();
        }

        public static IEnumerable<int> DemolishedBuildingIDs()
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows();
            var ownershipBuildingdsAssoc = EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance().FilterDeletedRows();
            return from ownershipBuildingsAssocRow in ownershipBuildingdsAssoc
                join ownershipRightsRow in ownershipRights
                    on ownershipBuildingsAssocRow.Field<int?>("id_ownership_right") equals ownershipRightsRow.Field<int>("id_ownership_right")
                where ownershipRightsRow.Field<int?>("id_ownership_right_type") == 1
                select ownershipBuildingsAssocRow.Field<int>("id_building");
        }

        public static IEnumerable<int> EmergencyBuildingIDs()
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows();
            var ownershipBuildingdsAssoc = EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance().FilterDeletedRows();
            return from ownershipBuildingsAssocRow in ownershipBuildingdsAssoc
                   join ownershipRightsRow in ownershipRights
                       on ownershipBuildingsAssocRow.Field<int?>("id_ownership_right") equals ownershipRightsRow.Field<int>("id_ownership_right")
                   where ownershipRightsRow.Field<int?>("id_ownership_right_type") == 2
                   select ownershipBuildingsAssocRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByOwnershipType(int idOwnershipType)
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows().ToList();
            var ownershipBuildingdsAssoc = EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance().FilterDeletedRows().ToList();
            // Если используется непользовательское ограничение "Аварийное", то не выводить здания еще и снесеные
            var demolishedBuildings = DemolishedBuildingIDs().Distinct().ToList();
            if (idOwnershipType == 2)
            {
                var emergencyExcludedBuildings = from ownershipBuildingsAssocRow in ownershipBuildingdsAssoc
                    join ownershipRightsRow in ownershipRights
                        on ownershipBuildingsAssocRow.Field<int?>("id_ownership_right") equals
                        ownershipRightsRow.Field<int>("id_ownership_right")
                    where ownershipRightsRow.Field<int?>("id_ownership_right_type") == 6 &&
                          ownershipBuildingsAssocRow.Field<int?>("id_building") != null &&
                          ownershipRightsRow.Field<DateTime?>("date") != null
                    select
                        new
                        {
                            id_building = ownershipBuildingsAssocRow.Field<int>("id_building"),
                            date = ownershipRightsRow.Field<DateTime>("date")
                        };

                return from ownershipRightsRow in ownershipRights
                    join ownershipBuildingdsAssocRow in ownershipBuildingdsAssoc
                        on ownershipRightsRow.Field<int>("id_ownership_right") equals
                        ownershipBuildingdsAssocRow.Field<int?>("id_ownership_right")
                    join demolishedBuildingsRow in demolishedBuildings
                        on ownershipBuildingdsAssocRow.Field<int?>("id_building") equals demolishedBuildingsRow into dBuildings
                    from dBuildingsRow in dBuildings.DefaultIfEmpty()
                    join emergencyExcludedBuildingRow in emergencyExcludedBuildings
                        on ownershipBuildingdsAssocRow.Field<int?>("id_building") equals emergencyExcludedBuildingRow.id_building into eeBuildings
                    from eeBuildingsRow in eeBuildings.DefaultIfEmpty()
                       where ownershipRightsRow.Field<int?>("id_ownership_right_type") == 2 &&
                            dBuildingsRow == 0 && (eeBuildingsRow == null || 
                            (ownershipRightsRow.Field<DateTime?>("date") != null && eeBuildingsRow.date < ownershipRightsRow.Field<DateTime>("date"))) &&
                            ownershipBuildingdsAssocRow.Field<int?>("id_building") != null
                    select ownershipBuildingdsAssocRow.Field<int>("id_building");
            }
            return from ownershipRightsRow in ownershipRights
                join ownershipBuildingdsAssocRow in ownershipBuildingdsAssoc
                    on ownershipRightsRow.Field<int>("id_ownership_right") equals ownershipBuildingdsAssocRow.Field<int?>("id_ownership_right")
                where ownershipRightsRow.Field<int?>("id_ownership_right_type") == idOwnershipType &&
                    ownershipBuildingdsAssocRow.Field<int?>("id_building") != null
                select ownershipBuildingdsAssocRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByOwnershipNumber(string ownershipNumber)
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows();
            var ownershipBuildingsAssoc = EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance().FilterDeletedRows();
            var buildings = EntityDataModel<Building>.GetInstance().FilterDeletedRows();
            return from buildingsRow in buildings
                   join ownershipBuildingsAssocRow in ownershipBuildingsAssoc
                   on buildingsRow.Field<int>("id_building") equals ownershipBuildingsAssocRow.Field<int?>("id_building")
                   join ownershipRightsRow in ownershipRights
                   on ownershipBuildingsAssocRow.Field<int?>("id_ownership_right") equals ownershipRightsRow.Field<int>("id_ownership_right")
                   where ownershipRightsRow.Field<string>("number") == ownershipNumber
                   select buildingsRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByRestricitonNumber(string restrictionNumber)
        {
            var restricitons = EntityDataModel<Restriction>.GetInstance().FilterDeletedRows();
            var restrictionBuildingsAssoc = EntityDataModel<RestrictionBuildingAssoc>.GetInstance().FilterDeletedRows();
            var buildings = EntityDataModel<Building>.GetInstance().FilterDeletedRows();
            return from buildingsRow in buildings
                   join restrictionBuildingsAssocRow in restrictionBuildingsAssoc
                   on buildingsRow.Field<int>("id_building") equals restrictionBuildingsAssocRow.Field<int?>("id_building")
                   join restrictionsRow in restricitons
                   on restrictionBuildingsAssocRow.Field<int?>("id_restriction") equals restrictionsRow.Field<int>("id_restriction")
                   where restrictionsRow.Field<string>("number") == restrictionNumber
                   select buildingsRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByRestricitonType(int idRestrictionType)
        {
            var restricitons = EntityDataModel<Restriction>.GetInstance().FilterDeletedRows();
            var restrictionBuildingsAssoc = EntityDataModel<RestrictionBuildingAssoc>.GetInstance().FilterDeletedRows();
            var buildings = EntityDataModel<Building>.GetInstance().FilterDeletedRows();
            return from buildingsRow in buildings
                   join restrictionBuildingsAssocRow in restrictionBuildingsAssoc
                   on buildingsRow.Field<int>("id_building") equals restrictionBuildingsAssocRow.Field<int?>("id_building")
                   join restrictionsRow in restricitons
                   on restrictionBuildingsAssocRow.Field<int?>("id_restriction") equals restrictionsRow.Field<int>("id_restriction")
                   where restrictionsRow.Field<int?>("id_restriction_type") == idRestrictionType
                   select buildingsRow.Field<int>("id_building");
        }

        public static IEnumerable<RestrictionBuildingAssoc> BuildingIDsExcludedFromMunicipal(List<DataRow> buildingAssocDataRows)
        {
            var restrictions = EntityDataModel<Restriction>.GetInstance().FilterDeletedRows().ToList();
            var restrictionsMaxDate = from restrictionsAssocRow in buildingAssocDataRows
                join restrictionsRow in restrictions
                    on restrictionsAssocRow.Field<int?>("id_restriction") equals restrictionsRow.Field<int>("id_restriction")
                where new[] { 1, 2 }.Contains(restrictionsRow.Field<int?>("id_restriction_type") ?? 0) &&
                    restrictionsRow.Field<DateTime?>("date") != null && restrictionsAssocRow.Field<int?>("id_building") != null
                group restrictionsRow.Field<DateTime>("date") by restrictionsAssocRow.Field<int>("id_building") into gs
                select new
                {
                    id = gs.Key,
                    date = gs.Max()
                };
            return from restrictionsRow in restrictions
                join restrictionsAssocRow in buildingAssocDataRows
                    on restrictionsRow.Field<int>("id_restriction")
                    equals restrictionsAssocRow.Field<int?>("id_restriction")
                join rmdRow in restrictionsMaxDate
                    on new
                    {
                        id = restrictionsAssocRow.Field<int?>("id_building"),
                        date = restrictionsRow.Field<DateTime?>("date")
                    } equals
                    new
                    {
                        id = (int?)rmdRow.id,
                        date = (DateTime?)rmdRow.date
                    }
                where restrictionsRow.Field<int?>("id_restriction_type") == 2
                select new RestrictionBuildingAssoc(restrictionsAssocRow.Field<int?>("id_building"),
                    restrictionsRow.Field<int?>("id_restriction"),
                    restrictionsRow.Field<DateTime?>("date"));
        }

        public static IEnumerable<RestrictionBuildingAssoc> BuildingIDsIncludedIntoMunicipal(List<DataRow> buildingAssocDataRows)
        {
            var restrictions = EntityDataModel<Restriction>.GetInstance().FilterDeletedRows().ToList();
            var restrictionsMaxDate = from restrictionsAssocRow in buildingAssocDataRows
                join restrictionsRow in restrictions
                    on restrictionsAssocRow.Field<int?>("id_restriction") equals restrictionsRow.Field<int>("id_restriction")
                where new[] { 1, 2 }.Contains(restrictionsRow.Field<int?>("id_restriction_type") ?? 0) &&
                    restrictionsRow.Field<DateTime?>("date") != null && restrictionsAssocRow.Field<int?>("id_building") != null
                group restrictionsRow.Field<DateTime>("date") by restrictionsAssocRow.Field<int>("id_building") into gs
                select new
                {
                    id = gs.Key,
                    date = gs.Max()
                };
            return from restrictionsRow in restrictions
                join restrictionsAssocRow in buildingAssocDataRows
                    on restrictionsRow.Field<int>("id_restriction")
                    equals restrictionsAssocRow.Field<int?>("id_restriction")
                join rmdRow in restrictionsMaxDate
                    on new
                    {
                        id = restrictionsAssocRow.Field<int?>("id_building"),
                        date = restrictionsRow.Field<DateTime?>("date")
                    } equals
                    new
                    {
                        id = (int?)rmdRow.id,
                        date = (DateTime?)rmdRow.date
                    }
                where restrictionsRow.Field<int?>("id_restriction_type") == 1
                select new RestrictionBuildingAssoc(restrictionsAssocRow.Field<int?>("id_building"),
                    restrictionsRow.Field<int?>("id_restriction"),
                    restrictionsRow.Field<DateTime?>("date"));
        }

        public static int GetRentCategory(DataRow buildingRow, bool isEmergency)
        {
            if (isEmergency)
                return 4;
            if (buildingRow.Field<short?>("improvement") == 1 && buildingRow.Field<short?>("floors") <= 6)
                return 1;
            if (buildingRow.Field<short?>("improvement") == 1 && buildingRow.Field<short?>("floors") > 6)
                return 2;
            if (buildingRow.Field<short?>("improvement") != 1 && buildingRow.Field<int?>("id_structure_type") == 5)
                return 3;
            return -1;
        }

        public static decimal GetRentCoefficient(int rentCategory)
        {
            switch (rentCategory)
            {
                case 1:
                    return 6.07m;
                case 2:
                    return 8.39m;
                case 3:
                    return 0.69m;
                case 4:
                    return 0.36m;
            }
            return 0;
        }

        public static bool HasTenancies(int idBuilding)
        {
            var buildingsTenancies = EntityDataModel<TenancyBuildingAssoc>.GetInstance().FilterDeletedRows();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var premisesTenancies = EntityDataModel<TenancyPremisesAssoc>.GetInstance().FilterDeletedRows();
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
            var subPremisesTenancies = EntityDataModel<TenancySubPremisesAssoc>.GetInstance().FilterDeletedRows();
            return (from tBuildingRow in buildingsTenancies
                    where tBuildingRow.Field<int?>("id_building") == idBuilding
                    select tBuildingRow).Any() ||
                   (from tPremiseRow in premisesTenancies
                    join premisesRow in premises
                    on tPremiseRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                    where premisesRow.Field<int?>("id_building") == idBuilding
                    select tPremiseRow).Any() ||
                   (from tSubPremisesRow in subPremisesTenancies
                    join subPremisesRow in subPremises
                    on tSubPremisesRow.Field<int?>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                    join premisesRow in premises
                    on subPremisesRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                    where premisesRow.Field<int?>("id_building") == idBuilding
                    select tSubPremisesRow).Any();
        }

        public static bool HasResettles(int idBuilding)
        {
            var buildingsResettleFrom = EntityDataModel<ResettleBuildingFromAssoc>.GetInstance().FilterDeletedRows();
            var buildingsResettleTo = EntityDataModel<ResettleBuildingToAssoc>.GetInstance().FilterDeletedRows();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var premisesResettleFrom = EntityDataModel<ResettlePremisesFromAssoc>.GetInstance().FilterDeletedRows();
            var premisesResettleTo = EntityDataModel<ResettlePremisesToAssoc>.GetInstance().FilterDeletedRows();
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
            var subPremisesResettleFrom = EntityDataModel<ResettleSubPremisesFromAssoc>.GetInstance().FilterDeletedRows();
            var subPremisesResettleTo = EntityDataModel<ResettleSubPremisesToAssoc>.GetInstance().FilterDeletedRows();
            return (from rBuildingRow in buildingsResettleFrom.Union(buildingsResettleTo)
                    where rBuildingRow.Field<int?>("id_building") == idBuilding
                    select rBuildingRow).Any() ||
                   (from rPremiseRow in premisesResettleFrom.Union(premisesResettleTo)
                    join premisesRow in premises
                    on rPremiseRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                    where premisesRow.Field<int?>("id_building") == idBuilding
                    select rPremiseRow).Any() ||
                   (from rSubPremisesRow in subPremisesResettleFrom.Union(subPremisesResettleTo)
                    join subPremisesRow in subPremises
                    on rSubPremisesRow.Field<int?>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                    join premisesRow in premises
                    on subPremisesRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                    where premisesRow.Field<int?>("id_building") == idBuilding
                    select rSubPremisesRow).Any();

        }
    }
}
