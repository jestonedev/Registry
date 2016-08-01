using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.Services
{
    public static class ResettleService
    {
        public static IEnumerable<int> ResettleProcessIdsBySnp(string[] snp)
        {
            var resettlePersons = EntityDataModel<ResettlePerson>.GetInstance().FilterDeletedRows();
            return
            (from resettlePersonsRow in resettlePersons
             where snp.Any() && resettlePersonsRow.Field<string>("surname") != null && string.Equals(resettlePersonsRow.Field<string>("surname"), snp[0], StringComparison.InvariantCultureIgnoreCase) &&
                ((snp.Length < 2) || resettlePersonsRow.Field<string>("name") != null &&
                    string.Equals(resettlePersonsRow.Field<string>("name"), snp[1], StringComparison.InvariantCultureIgnoreCase)) &&
                ((snp.Length < 3) || resettlePersonsRow.Field<string>("patronymic") != null &&
                    string.Equals(resettlePersonsRow.Field<string>("patronymic"), snp[2], StringComparison.InvariantCultureIgnoreCase))
             select resettlePersonsRow.Field<int>("id_process")).Distinct();
        }

        public static IEnumerable<int> ResettleProcessIDsByCondition(Func<DataRow, bool> condition, DataModelHelper.ConditionType conditionType, ResettleEstateObjectWay way)
        {
            var buildings = EntityDataModel<Building>.GetInstance().FilterDeletedRows().ToList();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
            var resettleBuildingsAssoc = way == ResettleEstateObjectWay.From ?
                EntityDataModel<ResettleBuildingFromAssoc>.GetInstance().FilterDeletedRows() :
                EntityDataModel<ResettleBuildingToAssoc>.GetInstance().FilterDeletedRows();
            var resettlePremisesAssoc = way == ResettleEstateObjectWay.From ?
                EntityDataModel<ResettlePremisesFromAssoc>.GetInstance().FilterDeletedRows() :
                EntityDataModel<ResettlePremisesToAssoc>.GetInstance().FilterDeletedRows();
            var resettleSubPremisesAssoc = way == ResettleEstateObjectWay.From ?
                EntityDataModel<ResettleSubPremisesFromAssoc>.GetInstance().FilterDeletedRows() :
                EntityDataModel<ResettleSubPremisesToAssoc>.GetInstance().FilterDeletedRows();
            var resettleBuildings = from resettleBuildingsRow in resettleBuildingsAssoc
                                    join buildingsRow in buildings
                                    on resettleBuildingsRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                    where
                                    (conditionType != DataModelHelper.ConditionType.PremisesCondition) && condition(buildingsRow)
                                    select resettleBuildingsRow.Field<int>("id_process");
            var resettlePremises = from resettlePremisesRow in resettlePremisesAssoc
                                   join premisesRow in premises
                                   on resettlePremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                   join buildingsRow in buildings
                                   on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                   where conditionType == DataModelHelper.ConditionType.PremisesCondition ? condition(premisesRow) : condition(buildingsRow)
                                   select resettlePremisesRow.Field<int>("id_process");
            var resettleSubPremises = from resettleSubPremisesRow in resettleSubPremisesAssoc
                                      join subPremisesRow in subPremises
                                      on resettleSubPremisesRow.Field<int>("id_sub_premises") equals subPremisesRow.Field<int>("id_sub_premises")
                                      join premisesRow in premises
                                      on subPremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                      join buildingsRow in buildings
                                      on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                      where conditionType == DataModelHelper.ConditionType.PremisesCondition ? condition(premisesRow) : condition(buildingsRow)
                                      select resettleSubPremisesRow.Field<int>("id_process");
            return resettleBuildings.Union(resettlePremises).Union(resettleSubPremises);
        }

        public static IEnumerable<int> ResettleProcessIDsByAddress(string[] addressParts, ResettleEstateObjectWay way)
        {
            var kladrStreets = DataModel.GetInstance<KladrStreetsDataModel>().FilterDeletedRows().ToList();
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows().ToList();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
            var resettleBuildingsAssoc = way == ResettleEstateObjectWay.From ?
                EntityDataModel<ResettleBuildingFromAssoc>.GetInstance().FilterDeletedRows() :
                EntityDataModel<ResettleBuildingToAssoc>.GetInstance().FilterDeletedRows();
            var resettlePremisesAssoc = way == ResettleEstateObjectWay.From ?
                EntityDataModel<ResettlePremisesFromAssoc>.GetInstance().FilterDeletedRows() :
                EntityDataModel<ResettlePremisesToAssoc>.GetInstance().FilterDeletedRows();
            var resettleSubPremisesAssoc = way == ResettleEstateObjectWay.From ?
                EntityDataModel<ResettleSubPremisesFromAssoc>.GetInstance().FilterDeletedRows() :
                EntityDataModel<ResettleSubPremisesToAssoc>.GetInstance().FilterDeletedRows();
            var resettleBuildings = from resettleBuildingsRow in resettleBuildingsAssoc
                                    join buildingsRow in buildings
                                    on resettleBuildingsRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                    join kladrRow in kladrStreets
                                    on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                                    where addressParts.Length == 1 ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                                                Contains(addressParts[0].ToUpperInvariant()) :
                                          (addressParts.Length >= 2) && kladrRow.Field<string>("street_name").ToUpperInvariant().
                                              Contains(addressParts[0].ToUpperInvariant()) && string.Equals(buildingsRow.Field<string>("house"), addressParts[1], StringComparison.InvariantCultureIgnoreCase)
                                    select resettleBuildingsRow.Field<int>("id_process");
            var resettlePremises = from resettlePremisesRow in resettlePremisesAssoc
                                   join premisesRow in premises
                                   on resettlePremisesRow.Field<int>("id_premises") equals premisesRow.Field<int>("id_premises")
                                   join buildingsRow in buildings
                                   on premisesRow.Field<int>("id_building") equals buildingsRow.Field<int>("id_building")
                                   join kladrRow in kladrStreets
                                    on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                                   where addressParts.Length == 1 ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                                       Contains(addressParts[0].ToUpperInvariant()) :
                                         addressParts.Length == 2 ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                                             Contains(addressParts[0].ToUpperInvariant()) &&
                                         string.Equals(buildingsRow.Field<string>("house"), addressParts[1], StringComparison.InvariantCultureIgnoreCase) :
                                         (addressParts.Length == 3) && kladrRow.Field<string>("street_name").ToUpperInvariant().
                                             Contains(addressParts[0].ToUpperInvariant()) && string.Equals(buildingsRow.Field<string>("house"), addressParts[1], StringComparison.InvariantCultureIgnoreCase) && string.Equals(premisesRow.Field<string>("premises_num"), addressParts[2], StringComparison.InvariantCultureIgnoreCase)
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
                                      where addressParts.Length == 1 ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                                          Contains(addressParts[0].ToUpperInvariant()) :
                                        addressParts.Length == 2 ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                                            Contains(addressParts[0].ToUpperInvariant()) &&
                                        string.Equals(buildingsRow.Field<string>("house"), addressParts[1], StringComparison.InvariantCultureIgnoreCase) :
                                        (addressParts.Length == 3) && kladrRow.Field<string>("street_name").ToUpperInvariant().
                                            Contains(addressParts[0].ToUpperInvariant()) && string.Equals(buildingsRow.Field<string>("house"), addressParts[1], StringComparison.InvariantCultureIgnoreCase) && string.Equals(premisesRow.Field<string>("premises_num"), addressParts[2], StringComparison.InvariantCultureIgnoreCase)
                                      select resettleSubPremisesRow.Field<int>("id_process");
            return resettleBuildings.Union(resettlePremises).Union(resettleSubPremises);
        }
    }
}
