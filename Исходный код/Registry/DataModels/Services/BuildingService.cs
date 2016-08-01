﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.Services
{
    public sealed class BuildingService
    {
        public static IEnumerable<int> BuildingIdsBySnp(string[] snp, Func<DataRow, bool> condition)
        {
            var tenancyBuildingsAssoc = EntityDataModel<TenancyBuildingAssoc>.GetInstance().FilterDeletedRows();
            var tenancyPersons = EntityDataModel<TenancyPerson>.GetInstance().FilterDeletedRows();
            return
            (from tenancyBuildingsAssocRow in tenancyBuildingsAssoc
             join tenancyPersonsRow in tenancyPersons
             on tenancyBuildingsAssocRow.Field<int>("id_process") equals tenancyPersonsRow.Field<int>("id_process")
             where snp.Any() && tenancyPersonsRow.Field<string>("surname") != null && String.Equals(tenancyPersonsRow.Field<string>("surname"), snp[0], StringComparison.InvariantCultureIgnoreCase) &&
                    ((snp.Length < 2) || tenancyPersonsRow.Field<string>("name") != null &&
                     String.Equals(tenancyPersonsRow.Field<string>("name"), snp[1], StringComparison.InvariantCultureIgnoreCase)) &&
                    ((snp.Length < 3) || tenancyPersonsRow.Field<string>("patronymic") != null &&
                     String.Equals(tenancyPersonsRow.Field<string>("patronymic"), snp[2], StringComparison.InvariantCultureIgnoreCase)) &&
                    condition(tenancyPersonsRow)
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
                    on tenancyProcessesRow.Field<int>("id_process") equals tenancyBuildingsAssocRow.Field<int>("id_process")
                where tenancyProcessesRow.Field<string>("registration_num") == number
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
                              on subPremisesFundsRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                              where DataModelHelper.MunicipalAndUnknownObjectStates().Contains(subPremisesRow.Field<int>("id_state")) &&
                              subPremisesFundsRow.Field<int>("id_fund_type") == idFund
                              select subPremisesRow.Field<int>("id_premises");
            var buildingsIds = from premisesFundsRow in premisesFunds.AsEnumerable()
                               join premisesRow in EntityDataModel<Premise>.GetInstance().FilterDeletedRows()
                               on premisesFundsRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                               where DataModelHelper.MunicipalAndUnknownObjectStates().Contains(premisesRow.Field<int>("id_state")) &&
                                premisesFundsRow.Field<int>("id_fund_type") == idFund ||
                                (premisesFundsRow.Field<int>("id_fund_type") == 4 && premisesIds.Contains(premisesFundsRow.Field<int>("id_premises")))
                               select premisesRow.Field<int>("id_building");
            return from buildingsFundsRow in buildingsFunds.AsEnumerable()
                   join buildingsRow in DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows()
                    on buildingsFundsRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                   where DataModelHelper.MunicipalAndUnknownObjectStates().Contains(buildingsRow.Field<int>("id_state")) &&
                         buildingsFundsRow.Field<int>("id_fund_type") == idFund ||
                         (buildingsFundsRow.Field<int>("id_fund_type") == 4 && buildingsIds.Contains(buildingsFundsRow.Field<int>("id_building")))
                   select buildingsFundsRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByRegion(string region)
        {
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
            return from buildingRow in buildings
                where buildingRow.Field<string>("id_street").StartsWith(region, StringComparison.Ordinal)
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
                where buildingRow.Field<string>("house").ToUpper().Contains(house.ToUpper())
                select buildingRow.Field<int>("id_building");
        }

        public static int BuildingsDuplicateCount(Building building)
        {
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
            return (from buildingsRow in buildings
                where buildingsRow.Field<string>("id_street") == building.IdStreet &&
                      buildingsRow.Field<string>("house") == building.House &&
                      buildingsRow.Field<int>("id_building") != building.IdBuilding
                select buildingsRow).Count();
        }

        public static IEnumerable<int> DemolishedBuildingIDs()
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows();
            var ownershipBuildingdsAssoc = EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance().FilterDeletedRows();
            return from ownershipBuildingsAssocRow in ownershipBuildingdsAssoc
                join ownershipRightsRow in ownershipRights
                    on ownershipBuildingsAssocRow.Field<int>("id_ownership_right") equals ownershipRightsRow.Field<int>("id_ownership_right")
                where ownershipRightsRow.Field<int>("id_ownership_right_type") == 1
                select ownershipBuildingsAssocRow.Field<int>("id_building");
        }

        public static IEnumerable<int> EmergencyExcludedBuildingIDs()
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows();
            var ownershipBuildingdsAssoc = EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance().FilterDeletedRows();
            return from ownershipBuildingsAssocRow in ownershipBuildingdsAssoc
                join ownershipRightsRow in ownershipRights
                    on ownershipBuildingsAssocRow.Field<int>("id_ownership_right") equals ownershipRightsRow.Field<int>("id_ownership_right")
                where ownershipRightsRow.Field<int>("id_ownership_right_type") == 6
                select ownershipBuildingsAssocRow.Field<int>("id_building");
        }

        public static IEnumerable<int> BuildingIDsByOwnershipType(int idOwnershipType)
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows();
            var ownershipBuildingdsAssoc = EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance().FilterDeletedRows();
            // Если используется непользовательское ограничение "Аварийное", то не выводить здания еще и снесеные
            var demolishedBuildings = DemolishedBuildingIDs().ToList();
            var emergencyExcludedBuilding = EmergencyExcludedBuildingIDs().ToList();
            return from ownershipRightsRow in ownershipRights
                join ownershipBuildingdsAssocRow in ownershipBuildingdsAssoc
                    on ownershipRightsRow.Field<int>("id_ownership_right") equals ownershipBuildingdsAssocRow.Field<int>("id_ownership_right")
                where (idOwnershipType != 2 || !demolishedBuildings.Contains(ownershipBuildingdsAssocRow.Field<int>("id_building")) &&
                       !emergencyExcludedBuilding.Contains(ownershipBuildingdsAssocRow.Field<int>("id_building"))) &&
                      ownershipRightsRow.Field<int>("id_ownership_right_type") == idOwnershipType
                select ownershipBuildingdsAssocRow.Field<int>("id_building");
        }

        public static IEnumerable<RestrictionBuildingAssoc> BuildingIDsExcludedFromMunicipal(List<DataRow> buildingAssocDataRows)
        {
            var restrictions = EntityDataModel<Restriction>.GetInstance().FilterDeletedRows().ToList();
            var restrictionsMaxDate = from restrictionsAssocRow in buildingAssocDataRows
                join restrictionsRow in restrictions
                    on restrictionsAssocRow.Field<int>("id_restriction") equals restrictionsRow.Field<int>("id_restriction")
                where new[] { 1, 2 }.Contains(restrictionsRow.Field<int>("id_restriction_type"))
                group restrictionsRow.Field<DateTime>("date") by restrictionsAssocRow.Field<int>("id_building") into gs
                select new
                {
                    id = gs.Key,
                    date = gs.Max()
                };
            return from restrictionsRow in restrictions
                join restrictionsAssocRow in buildingAssocDataRows
                    on restrictionsRow.Field<int>("id_restriction")
                    equals restrictionsAssocRow.Field<int>("id_restriction")
                join rmdRow in restrictionsMaxDate
                    on new
                    {
                        id = restrictionsAssocRow.Field<int>("id_building"),
                        date = restrictionsRow.Field<DateTime>("date")
                    } equals
                    new
                    {
                        rmdRow.id,
                        rmdRow.date
                    }
                where restrictionsRow.Field<int>("id_restriction_type") == 2
                select new RestrictionBuildingAssoc(restrictionsAssocRow.Field<int?>("id_building"),
                    restrictionsRow.Field<int?>("id_restriction"),
                    restrictionsRow.Field<DateTime?>("date"));
        }

        public static IEnumerable<RestrictionBuildingAssoc> BuildingIDsIncludedIntoMunicipal(List<DataRow> buildingAssocDataRows)
        {
            var restrictions = EntityDataModel<Restriction>.GetInstance().FilterDeletedRows().ToList();
            var restrictionsMaxDate = from restrictionsAssocRow in buildingAssocDataRows
                join restrictionsRow in restrictions
                    on restrictionsAssocRow.Field<int>("id_restriction") equals restrictionsRow.Field<int>("id_restriction")
                where new[] { 1, 2 }.Contains(restrictionsRow.Field<int>("id_restriction_type"))
                group restrictionsRow.Field<DateTime>("date") by restrictionsAssocRow.Field<int>("id_building") into gs
                select new
                {
                    id = gs.Key,
                    date = gs.Max()
                };
            return from restrictionsRow in restrictions
                join restrictionsAssocRow in buildingAssocDataRows
                    on restrictionsRow.Field<int>("id_restriction")
                    equals restrictionsAssocRow.Field<int>("id_restriction")
                join rmdRow in restrictionsMaxDate
                    on new
                    {
                        id = restrictionsAssocRow.Field<int>("id_building"),
                        date = restrictionsRow.Field<DateTime>("date")
                    } equals
                    new
                    {
                        rmdRow.id,
                        rmdRow.date
                    }
                where restrictionsRow.Field<int>("id_restriction_type") == 1
                select new RestrictionBuildingAssoc(restrictionsAssocRow.Field<int?>("id_building"),
                    restrictionsRow.Field<int?>("id_restriction"),
                    restrictionsRow.Field<DateTime?>("date"));
        }
    }
}
