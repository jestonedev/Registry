using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.Services
{
    public sealed class SubPremisesService
    {
        public static bool HasTenancies(int idSubPremise)
        {
            var subPremisesTenancies = EntityDataModel<TenancySubPremisesAssoc>.GetInstance().FilterDeletedRows();
            return (from tSubPremiseRow in subPremisesTenancies
                    where tSubPremiseRow.Field<int?>("id_sub_premises") == idSubPremise
                    select tSubPremiseRow).Any();
        }

        public static bool HasResettles(int idSubPremise)
        {
            var subPremisesResettleFrom = EntityDataModel<ResettleSubPremisesFromAssoc>.GetInstance().FilterDeletedRows();
            var subPremisesResettleTo = EntityDataModel<ResettleSubPremisesToAssoc>.GetInstance().FilterDeletedRows();
            return (from rSubPremiseRow in subPremisesResettleFrom.Union(subPremisesResettleTo)
                    where rSubPremiseRow.Field<int?>("id_sub_premises") == idSubPremise
                    select rSubPremiseRow).Any();
        }

        internal static IEnumerable<int> GetPremisesIdsBySubPremiseStates(IEnumerable<int> stateIds)
        {
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
            return from row in subPremises
                   where row.Field<int?>("id_premises") != null && 
                   row.Field<int?>("id_state") != null &&
                   stateIds.Contains(row.Field<int>("id_state"))
                select row.Field<int>("id_premises");
        }
    }
}
