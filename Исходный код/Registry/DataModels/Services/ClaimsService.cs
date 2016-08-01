using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.Services
{
    public static class ClaimsService
    {
        public static IEnumerable<int> ClaimStateTypeIdsByNextStateType(int nextStateType)
        {
            var claimStateTypes = DataModel.GetInstance<EntityDataModel<ClaimStateType>>().FilterDeletedRows();
            var claimStateTypeRelations = DataModel.GetInstance<EntityDataModel<ClaimStateTypeRelation>>().FilterDeletedRows();
            return from claimStateTypesRow in claimStateTypes
                   join claimStateTypesRelRow in claimStateTypeRelations
                       on claimStateTypesRow.Field<int>("id_state_type") equals claimStateTypesRelRow.Field<int>("id_state_from")
                   where Convert.ToBoolean(claimStateTypesRow.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture) &&
                         (claimStateTypesRelRow.Field<int>("id_state_to") == nextStateType)
                   select claimStateTypesRow.Field<int>("id_state_type");
        }

        public static IEnumerable<int> ClaimStateTypeIdsByPrevStateType(int prevStateType)
        {
            var claimStateTypes = DataModel.GetInstance<EntityDataModel<ClaimStateType>>().FilterDeletedRows();
            var claimStateTypeRelations = DataModel.GetInstance<EntityDataModel<ClaimStateTypeRelation>>().FilterDeletedRows();
            return from claimStateTypesRow in claimStateTypes
                   join claimStateTypesRelRow in claimStateTypeRelations
                       on claimStateTypesRow.Field<int>("id_state_type") equals claimStateTypesRelRow.Field<int>("id_state_from")
                   where claimStateTypesRelRow.Field<int>("id_state_from") == prevStateType
                   select claimStateTypesRelRow.Field<int>("id_state_to");
        }

        public static IEnumerable<int> ClaimStateTypeIdsByNextAndPrevStateTypes(int nextStateType, int prevStateType)
        {
            var claimStateTypeRelations = DataModel.GetInstance<EntityDataModel<ClaimStateTypeRelation>>().FilterDeletedRows().ToList();
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
            var claimStateTypes = DataModel.GetInstance<EntityDataModel<ClaimStateType>>().FilterDeletedRows();
            return from claimStateTypesRow in claimStateTypes
                   where Convert.ToBoolean(claimStateTypesRow.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture)
                   select claimStateTypesRow.Field<int>("id_state_type");
        }
    }
}
