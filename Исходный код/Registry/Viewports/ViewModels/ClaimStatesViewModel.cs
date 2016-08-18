using System.Collections.Generic;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal class ClaimStatesViewModel: ViewModel
    {
        public ClaimStatesViewModel() : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<ClaimState>.GetInstance())},
            {"claim_state_types", new ViewModelItem(EntityDataModel<ClaimStateType>.GetInstance())},
            {"claim_state_types_for_grid", new ViewModelItem(EntityDataModel<ClaimStateType>.GetInstance())},
            {"claim_state_types_relations", new ViewModelItem(EntityDataModel<ClaimStateTypeRelation>.GetInstance())}
        })
        {
        }
    }
}
