using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Entities.Infrastructure;

namespace Registry.DataModels.Services
{
    public sealed class OtherService
    {
        public static IEnumerable<int> ObjectIdsByStates(EntityType entity, int[] states)
        {
            if (entity == EntityType.Building)
            {
                var buildings = EntityDataModel<Building>.GetInstance().FilterDeletedRows();
                var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows();
                var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
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
                var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows();
                var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
                var result = from premisesRow in premises
                             join subPremisesRow in subPremises
                             on premisesRow.Field<int>("id_premises") equals subPremisesRow.Field<int>("id_premises") into ps
                             from psRow in ps.DefaultIfEmpty()
                             where psRow != null && states.Contains(psRow.Field<int>("id_state"))
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
                        var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
                        var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
                        var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
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
                        var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows();
                        var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
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
                        var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
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
            return HasObjectState(id, entity, DataModelHelper.MunicipalObjectStates().ToArray());
        }

        public static bool HasNotMunicipal(int id, EntityType entity)
        {
            return HasObjectState(id, entity, DataModelHelper.NonMunicipalAndUnknownObjectStates().ToArray());
        }

        public static IEnumerable<FundBuildingAssoc> MaxFundIDsByBuildingId(IEnumerable<DataRow> objectAssocDataRows)
        {
            var fundsHistory = DataModel.GetInstance<EntityDataModel<FundHistory>>().FilterDeletedRows();
            return from assocRow in objectAssocDataRows
                   join fundHistoryRow in fundsHistory
                      on assocRow.Field<int>("id_fund") equals fundHistoryRow.Field<int>("id_fund")
                   where fundHistoryRow.Field<DateTime?>("exclude_restriction_date") == null
                   group assocRow.Field<int>("id_fund") by
                           assocRow.Field<int>("id_building") into gs
                   select new FundBuildingAssoc(gs.Key, gs.Max());
        }

        public static IEnumerable<FundPremisesAssoc> MaxFundIDsByPremisesId(IEnumerable<DataRow> objectAssocDataRows)
        {
            var fundsHistory = DataModel.GetInstance<EntityDataModel<FundHistory>>().FilterDeletedRows();
            return from assocRow in objectAssocDataRows
                   join fundHistoryRow in fundsHistory
                       on assocRow.Field<int>("id_fund") equals fundHistoryRow.Field<int>("id_fund")
                   where fundHistoryRow.Field<DateTime?>("exclude_restriction_date") == null
                   group assocRow.Field<int>("id_fund") by
                           assocRow.Field<int>("id_premises") into gs
                   select new FundPremisesAssoc(gs.Key, gs.Max());
        }

        public static IEnumerable<FundSubPremisesAssoc> MaxFundIDsBySubPremiseId(IEnumerable<DataRow> objectAssocDataRows)
        {
            var fundsHistory = DataModel.GetInstance<EntityDataModel<FundHistory>>().FilterDeletedRows();
            return from assocRow in objectAssocDataRows
                   join fundHistoryRow in fundsHistory
                      on assocRow.Field<int>("id_fund") equals fundHistoryRow.Field<int>("id_fund")
                   where fundHistoryRow.Field<DateTime?>("exclude_restriction_date") == null
                   group assocRow.Field<int>("id_fund") by
                           assocRow.Field<int>("id_sub_premises") into gs
                   select new FundSubPremisesAssoc(gs.Key, gs.Max());
        }
    }
}
