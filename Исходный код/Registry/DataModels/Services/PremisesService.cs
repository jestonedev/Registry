using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.Services
{
    public static class PremisesService
    {
        public static IEnumerable<int> PremisesIdsBySnp(string[] snp, Func<DataRow, bool> condition)
        {
            var tenancyPremisesAssoc = EntityDataModel<TenancyPremisesAssoc>.GetInstance().FilterDeletedRows();
            var tenancyPersons = EntityDataModel<TenancyPerson>.GetInstance().FilterDeletedRows();
            return
            (from tenancyPremisesAssocRow in tenancyPremisesAssoc
             join tenancyPersonsRow in tenancyPersons
             on tenancyPremisesAssocRow.Field<int>("id_process") equals tenancyPersonsRow.Field<int>("id_process")
             where snp.Any() && tenancyPersonsRow.Field<string>("surname") != null && String.Equals(tenancyPersonsRow.Field<string>("surname"), snp[0], StringComparison.InvariantCultureIgnoreCase) &&
                    ((snp.Length < 2) || tenancyPersonsRow.Field<string>("name") != null &&
                     String.Equals(tenancyPersonsRow.Field<string>("name"), snp[1], StringComparison.InvariantCultureIgnoreCase)) &&
                    ((snp.Length < 3) || tenancyPersonsRow.Field<string>("patronymic") != null &&
                     String.Equals(tenancyPersonsRow.Field<string>("patronymic"), snp[2], StringComparison.InvariantCultureIgnoreCase)) &&
                    condition(tenancyPersonsRow)
             select tenancyPremisesAssocRow.Field<int>("id_premises")).Distinct();
        }

        public static IEnumerable<int> PremiseIDsByAddress(string[] addressParts)
        {
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
            var kladrStreets = DataModel.GetInstance<KladrStreetsDataModel>().FilterDeletedRows();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows();
            return from premisesRow in premises
                   join buildingRow in buildings
                       on premisesRow.Field<int>("id_building") equals buildingRow.Field<int>("id_building")
                   join kladrRow in kladrStreets
                       on buildingRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                   where addressParts.Length == 1 ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                       Contains(addressParts[0].ToUpperInvariant()) :
                       addressParts.Length == 2 ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                           Contains(addressParts[0].ToUpperInvariant()) &&
                                                   buildingRow.Field<string>("house").ToUpper().Equals(addressParts[1].ToUpper()) :
                           (addressParts.Length == 3) && kladrRow.Field<string>("street_name").ToUpperInvariant().
                               Contains(addressParts[0].ToUpperInvariant()) && buildingRow.Field<string>("house").ToUpper().Equals(addressParts[1].ToUpper()) && premisesRow.Field<string>("premises_num").ToUpperInvariant().Equals(
                                   addressParts[2].ToUpperInvariant())
                   select premisesRow.Field<int>("id_premises");
        }

        public static IEnumerable<int> PremiseIDsByRegistrationNumber(string number)
        {
            var tenancyPremisesAssoc = EntityDataModel<TenancyPremisesAssoc>.GetInstance().FilterDeletedRows();
            var tenancyProcesses = EntityDataModel<TenancyProcess>.GetInstance().FilterDeletedRows();
            return from tenancyProcessesRow in tenancyProcesses
                   join tenancyPremisesAssocRow in tenancyPremisesAssoc
                       on tenancyProcessesRow.Field<int>("id_process") equals tenancyPremisesAssocRow.Field<int>("id_process")
                   where tenancyProcessesRow.Field<string>("registration_num") == number
                   select tenancyPremisesAssocRow.Field<int>("id_premises");
        }

        public static IEnumerable<int> PremiseIDsByCurrentFund(int idFund)
        {
            // Ищем помещения указанного фонда, а также помещения, в которых присутствуют комнаты указанного фонда
            var premisesFunds = CalcDataModelPremisesCurrentFunds.GetInstance().Select();
            var subPremisesFunds = CalcDataModelSubPremisesCurrentFunds.GetInstance().Select();
            var premisesIds = from subPremisesFundsRow in subPremisesFunds.AsEnumerable()
                              join subPremisesRow in EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows()
                              on subPremisesFundsRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                              where DataModelHelper.MunicipalAndUnknownObjectStates().Contains(subPremisesRow.Field<int>("id_state")) &&
                              subPremisesFundsRow.Field<int>("id_fund_type") == idFund
                              select subPremisesRow.Field<int>("id_premises");
            return from premisesFundsRow in premisesFunds.AsEnumerable()
                   join premisesRow in EntityDataModel<Premise>.GetInstance().FilterDeletedRows()
                    on premisesFundsRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                   where DataModelHelper.MunicipalAndUnknownObjectStates().Contains(premisesRow.Field<int>("id_state")) &&
                         premisesFundsRow.Field<int>("id_fund_type") == idFund ||
                         (premisesFundsRow.Field<int>("id_fund_type") == 4 && premisesIds.Contains(premisesFundsRow.Field<int>("id_premises")))
                   select premisesFundsRow.Field<int>("id_premises");
        }

        public static int PremisesDuplicateCount(Premise premise)
        {
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows();
            return (from premisesRow in premises
                where premisesRow.Field<int>("id_building") == premise.IdBuilding &&
                      premisesRow.Field<string>("premises_num") == premise.PremisesNum &&
                      premisesRow.Field<int>("id_premises") != premise.IdPremises
                select premisesRow).Count();
        }

        public static IEnumerable<int> DemolishedPremisesIDs()
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows();
            var ownershipPremisesAssoc = EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance().FilterDeletedRows();
            return from ownershipPremisesAssocRow in ownershipPremisesAssoc
                join ownershipRightsRow in ownershipRights
                    on ownershipPremisesAssocRow.Field<int>("id_ownership_right") equals ownershipRightsRow.Field<int>("id_ownership_right")
                where ownershipRightsRow.Field<int>("id_ownership_right_type") == 1
                select ownershipPremisesAssocRow.Field<int>("id_premises");
        }

        public static IEnumerable<int> EmergencyExcludedPremisesIDs()
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows();
            var ownershipPremisesAssoc = EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance().FilterDeletedRows();
            return from ownershipPremisesAssocRow in ownershipPremisesAssoc
                join ownershipRightsRow in ownershipRights
                    on ownershipPremisesAssocRow.Field<int>("id_ownership_right") equals ownershipRightsRow.Field<int>("id_ownership_right")
                where ownershipRightsRow.Field<int>("id_ownership_right_type") == 6
                select ownershipPremisesAssocRow.Field<int>("id_premises");
        }

        public static IEnumerable<int> PremiseIDsByOwnershipType(int idOwnershipType)
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows();
            var ownershipPremisesAssoc = EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance().FilterDeletedRows();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows();
            var buildingdIds = BuildingService.BuildingIDsByOwnershipType(idOwnershipType);
            // Если используется ограничение "Аварийное", то не выводить помещения, если они или их здание имеют ограничение "Снесено"
            var demolishedPremises = DemolishedPremisesIDs().ToList();
            var emergencyExcludedPremises = EmergencyExcludedPremisesIDs().ToList();
            //Выбираются помещения с установленным ограничением и помещения, находящиеся в зданиях с установленным ограничением
            var premisesIds = from ownershipRightsRow in ownershipRights
                join ownershipPremisesAssocRow in ownershipPremisesAssoc
                    on ownershipRightsRow.Field<int>("id_ownership_right") equals ownershipPremisesAssocRow.Field<int>("id_ownership_right")
                where (idOwnershipType != 2 || !demolishedPremises.Contains(ownershipPremisesAssocRow.Field<int>("id_premises")) &&
                       !emergencyExcludedPremises.Contains(ownershipPremisesAssocRow.Field<int>("id_premises"))) &&
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

        public static IEnumerable<int> PremiseIDsByOwnershipNumber(string ownershipNumber)
        {
            var ownershipRights = EntityDataModel<OwnershipRight>.GetInstance().FilterDeletedRows();
            var ownershipPremisesAssoc = EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance().FilterDeletedRows();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var buildingIds = BuildingService.BuildingIDsByOwnershipNumber(ownershipNumber);

            return (from premisesRow in premises
                join ownershipPremisesAssocRow in ownershipPremisesAssoc
                    on premisesRow.Field<int>("id_premises") equals ownershipPremisesAssocRow.Field<int>("id_premises")
                join ownershipRightsRow in ownershipRights
                    on ownershipPremisesAssocRow.Field<int>("id_ownership_right") equals
                    ownershipRightsRow.Field<int>("id_ownership_right")
                where ownershipRightsRow.Field<string>("number") == ownershipNumber
                select premisesRow.Field<int>("id_premises")).Union(
                    from premisesRow in premises
                    join buildingId in buildingIds
                        on premisesRow.Field<int>("id_building") equals buildingId
                    select premisesRow.Field<int>("id_premises")
                );
        }

        public static IEnumerable<int> PremiseIDsByRestricitonNumber(string restrictionNumber)
        {
            var restricitons = EntityDataModel<Restriction>.GetInstance().FilterDeletedRows();
            var restrictionPremisesAssoc = EntityDataModel<RestrictionPremisesAssoc>.GetInstance().FilterDeletedRows();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var buildingIds = BuildingService.BuildingIDsByRestricitonNumber(restrictionNumber);
            return (from premisesRow in premises
                join restrictionPremisesAssocRow in restrictionPremisesAssoc
                    on premisesRow.Field<int>("id_premises") equals
                    restrictionPremisesAssocRow.Field<int>("id_premises")
                join restrictionsRow in restricitons
                    on restrictionPremisesAssocRow.Field<int>("id_restriction") equals
                    restrictionsRow.Field<int>("id_restriction")
                where restrictionsRow.Field<string>("number") == restrictionNumber
                select premisesRow.Field<int>("id_premises")).Union(
                    from premisesRow in premises
                    join buildingId in buildingIds
                        on premisesRow.Field<int>("id_building") equals buildingId
                    select premisesRow.Field<int>("id_premises")
                );
        }

        public static IEnumerable<int> PremiseIDsByRestricitonType(int idRestrictionType)
        {
            var restricitons = EntityDataModel<Restriction>.GetInstance().FilterDeletedRows();
            var restrictionPremisesAssoc = EntityDataModel<RestrictionPremisesAssoc>.GetInstance().FilterDeletedRows();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var buildingIds = BuildingService.BuildingIDsByRestricitonType(idRestrictionType);
            return (from premisesRow in premises
                join restrictionPremisesAssocRow in restrictionPremisesAssoc
                    on premisesRow.Field<int>("id_premises") equals
                    restrictionPremisesAssocRow.Field<int>("id_premises")
                join restrictionsRow in restricitons
                    on restrictionPremisesAssocRow.Field<int>("id_restriction") equals
                    restrictionsRow.Field<int>("id_restriction")
                where restrictionsRow.Field<int>("id_restriction_type") == idRestrictionType
                select premisesRow.Field<int>("id_premises")).Union(
                    from premisesRow in premises
                    join buildingId in buildingIds
                        on premisesRow.Field<int>("id_building") equals buildingId
                    select premisesRow.Field<int>("id_premises")
                );
        }

        public static IEnumerable<RestrictionPremisesAssoc> PremiseIDsExcludedFromMunicipal(List<DataRow> premisesAssocDataRows)
        {
            var restrictions = EntityDataModel<Restriction>.GetInstance().FilterDeletedRows().ToList();
            var restrictionsMaxDate = from restrictionsAssocRow in premisesAssocDataRows
                join restrictionsRow in restrictions
                    on restrictionsAssocRow.Field<int>("id_restriction") equals restrictionsRow.Field<int>("id_restriction")
                where new[] { 1, 2 }.Contains(restrictionsRow.Field<int>("id_restriction_type"))
                group restrictionsRow.Field<DateTime>("date") by restrictionsAssocRow.Field<int>("id_premises") into gs
                select new
                {
                    id = gs.Key,
                    date = gs.Max()
                };
            return from restrictionsRow in restrictions
                join restrictionsAssocRow in premisesAssocDataRows
                    on restrictionsRow.Field<int>("id_restriction")
                    equals restrictionsAssocRow.Field<int>("id_restriction")
                join rmdRow in restrictionsMaxDate
                    on new
                    {
                        id = restrictionsAssocRow.Field<int>("id_premises"),
                        date = restrictionsRow.Field<DateTime>("date")
                    } equals
                    new
                    {
                        rmdRow.id,
                        rmdRow.date
                    }
                where restrictionsRow.Field<int>("id_restriction_type") == 2
                select new RestrictionPremisesAssoc(restrictionsAssocRow.Field<int?>("id_premises"),
                    restrictionsRow.Field<int?>("id_restriction"),
                    restrictionsRow.Field<DateTime?>("date"));
        }

        public static IEnumerable<RestrictionPremisesAssoc> PremiseIDsIncludedIntoMunicipal(List<DataRow> premisesAssocDataRows)
        {
            var restrictions = EntityDataModel<Restriction>.GetInstance().FilterDeletedRows().ToList();
            var restrictionsMaxDate = from restrictionsAssocRow in premisesAssocDataRows
                join restrictionsRow in restrictions
                    on restrictionsAssocRow.Field<int>("id_restriction") equals restrictionsRow.Field<int>("id_restriction")
                where new[] { 1, 2 }.Contains(restrictionsRow.Field<int>("id_restriction_type"))
                group restrictionsRow.Field<DateTime>("date") by restrictionsAssocRow.Field<int>("id_premises") into gs
                select new
                {
                    id = gs.Key,
                    date = gs.Max()
                };
            return from restrictionsRow in restrictions
                join restrictionsAssocRow in premisesAssocDataRows
                    on restrictionsRow.Field<int>("id_restriction")
                    equals restrictionsAssocRow.Field<int>("id_restriction")
                join rmdRow in restrictionsMaxDate
                    on new
                    {
                        id = restrictionsAssocRow.Field<int>("id_premises"),
                        date = restrictionsRow.Field<DateTime>("date")
                    } equals
                    new
                    {
                        rmdRow.id,
                        rmdRow.date
                    }
                where restrictionsRow.Field<int>("id_restriction_type") == 1
                select
                    new RestrictionPremisesAssoc(restrictionsAssocRow.Field<int?>("id_premises"),
                        restrictionsRow.Field<int?>("id_restriction"),
                        restrictionsRow.Field<DateTime?>("date"));
        }
    }
}
