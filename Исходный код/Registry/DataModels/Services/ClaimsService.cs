using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services.ServiceModels;
using Registry.Entities;

namespace Registry.DataModels.Services
{
    public static class ClaimsService
    {
        public static IEnumerable<int> ClaimStateTypeIdsByNextStateType(int nextStateType)
        {
            var claimStateTypes = EntityDataModel<ClaimStateType>.GetInstance().FilterDeletedRows();
            var claimStateTypeRelations = EntityDataModel<ClaimStateTypeRelation>.GetInstance().FilterDeletedRows();
            return from claimStateTypesRow in claimStateTypes
                   join claimStateTypesRelRow in claimStateTypeRelations
                       on claimStateTypesRow.Field<int>("id_state_type") equals claimStateTypesRelRow.Field<int?>("id_state_from")
                   where Convert.ToBoolean(claimStateTypesRow.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture) &&
                         (claimStateTypesRelRow.Field<int?>("id_state_to") == nextStateType)
                   select claimStateTypesRow.Field<int>("id_state_type");
        }

        public static IEnumerable<int> ClaimStateTypeIdsByPrevStateType(int prevStateType)
        {
            var claimStateTypes = EntityDataModel<ClaimStateType>.GetInstance().FilterDeletedRows();
            var claimStateTypeRelations = EntityDataModel<ClaimStateTypeRelation>.GetInstance().FilterDeletedRows();
            return from claimStateTypesRow in claimStateTypes
                   join claimStateTypesRelRow in claimStateTypeRelations
                       on claimStateTypesRow.Field<int>("id_state_type") equals claimStateTypesRelRow.Field<int?>("id_state_from")
                   where claimStateTypesRelRow.Field<int?>("id_state_from") == prevStateType &&
                            claimStateTypesRelRow.Field<int?>("id_state_to") != null
                   select claimStateTypesRelRow.Field<int>("id_state_to");
        }

        public static IEnumerable<int> ClaimStateTypeIdsByNextAndPrevStateTypes(int nextStateType, int prevStateType)
        {
            var claimStateTypeRelations = EntityDataModel<ClaimStateTypeRelation>.GetInstance().FilterDeletedRows().ToList();
            var fromStates = from claimStateTypesRelRow in claimStateTypeRelations
                             where claimStateTypesRelRow.Field<int?>("id_state_from") == prevStateType &&
                                   claimStateTypesRelRow.Field<int?>("id_state_to") != null
                             select claimStateTypesRelRow.Field<int>("id_state_to");
            var toStates = from claimStateTypesRelRow in claimStateTypeRelations
                           where claimStateTypesRelRow.Field<int?>("id_state_to") == nextStateType &&
                                 claimStateTypesRelRow.Field<int?>("id_state_from") != null
                           select claimStateTypesRelRow.Field<int>("id_state_from");
            return fromStates.Intersect(toStates);
        }

        public static IEnumerable<int> ClaimStartStateTypeIds()
        {
            var claimStateTypes = EntityDataModel<ClaimStateType>.GetInstance().FilterDeletedRows();
            return from claimStateTypesRow in claimStateTypes
                   where Convert.ToBoolean(claimStateTypesRow.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture)
                   select claimStateTypesRow.Field<int>("id_state_type");
        }

        public static IEnumerable<int> ClaimIdsByStateCondition(Func<DataRow, bool> condition)
        {
            var claimStates = EntityDataModel<ClaimState>.GetInstance().FilterDeletedRows();
            return (from claimStatesRow in claimStates
                    where condition(claimStatesRow) && claimStatesRow.Field<int?>("id_claim") != null
                    select claimStatesRow.Field<int>("id_claim")).Distinct();
        }

        public static IEnumerable<ClaimPaymentAccountInfo> NotCompletedClaimsPaymentAccountsInfo()
        {
            var claimsDataModel = DataModel.GetInstance<EntityDataModel<Claim>>();
            var claimStatesDataModel = DataModel.GetInstance<EntityDataModel<ClaimState>>();
            var claimStatesTypeDataModel = DataModel.GetInstance<EntityDataModel<ClaimStateType>>();
            var lastStates = from stateRow in claimStatesDataModel.FilterDeletedRows()
                             group stateRow.Field<int?>("id_state") by stateRow.Field<int?>("id_claim") into gs
                             select new
                             {
                                 id_claim = gs.Key,
                                 id_state = gs.Max()
                             };
            var lastStateTypes = from lastStateRow in lastStates
                                  join stateRow in claimStatesDataModel.FilterDeletedRows()
                                      on lastStateRow.id_state equals stateRow.Field<int?>("id_state")
                                  join stateTypeRow in claimStatesTypeDataModel.FilterDeletedRows()
                                      on stateRow.Field<int?>("id_state_type") equals stateTypeRow.Field<int?>("id_state_type")
                                  where stateRow.Field<int?>("id_claim") != null &&
                                  stateRow.Field<int?>("id_state_type") != null
                                  select new
                                  {
                                      id_claim = stateRow.Field<int>("id_claim"),
                                      id_state_type = stateRow.Field<int>("id_state_type"),
                                      state_type = stateTypeRow.Field<string>("state_type") != null ? 
                                        stateTypeRow.Field<string>("state_type").ToLowerInvariant() : null
                                  };
            return from lastStateTypeRow in lastStateTypes
                                      join claimsRow in claimsDataModel.FilterDeletedRows()
                                          on lastStateTypeRow.id_claim equals claimsRow.Field<int>("id_claim")
                                      join accountRow in DataModel.GetInstance<PaymentsAccountsDataModel>().FilterDeletedRows()
                                          on claimsRow.Field<int?>("id_account") equals accountRow.Field<int?>("id_account")
                                      where ClaimStateTypeIdsByPrevStateType(lastStateTypeRow.id_state_type).Any()
                                        && accountRow.Field<int?>("id_account") != null &&
                                        claimsRow.Field<int?>("id_claim") != null
                                      select new ClaimPaymentAccountInfo
                                      {
                                          IdClaim = claimsRow.Field<int>("id_claim"),
                                          StartDeptPeriod = claimsRow.Field<DateTime?>("start_dept_period"),
                                          EndDeptPeriod = claimsRow.Field<DateTime?>("end_dept_period"),
                                          Amount = claimsRow.Field<decimal>("amount_tenancy")+
                                          claimsRow.Field<decimal>("amount_dgi") +
                                          claimsRow.Field<decimal>("amount_penalties"),
                                          IdAccount = accountRow.Field<int>("id_account"),
                                          Account = accountRow.Field<string>("account"),
                                          RawAddress = accountRow.Field<string>("raw_address"),
                                          ParsedAddress = accountRow.Field<string>("parsed_address"),
                                          Tenant = accountRow.Field<string>("tenant"),
                                          StateType = lastStateTypeRow.state_type
                                      };
        }
    }
}
