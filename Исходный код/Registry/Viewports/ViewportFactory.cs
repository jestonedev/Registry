using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Viewport
{
    public static class ViewportFactory
    {
        public static Viewport CreateViewport(IMenuCallback menuCallback, ViewportType viewportType)
        {
            switch (viewportType)
            {
                case ViewportType.BuildingListViewport:
                    return new BuildingListViewport(menuCallback);
                case ViewportType.BuildingViewport:
                    return new BuildingViewport(menuCallback);
                case ViewportType.OwnershipListViewport: 
                    return new OwnershipListViewport(menuCallback);
                case ViewportType.OwnershipTypeListViewport: 
                    return new OwnershipTypeListViewport(menuCallback);
                case ViewportType.PremisesListViewport: 
                    return new PremisesListViewport(menuCallback);
                case ViewportType.PremisesViewport:
                    return new PremisesViewport(menuCallback);
                case ViewportType.RestrictionListViewport: 
                    return new RestrictionListViewport(menuCallback);
                case ViewportType.SubPremisesViewport:
                    return new SubPremisesViewport(menuCallback);
                case ViewportType.RestrictionTypeListViewport: 
                    return new RestrictionTypeListViewport(menuCallback);
                case ViewportType.StructureTypeListViewport: 
                    return new StructureTypeListViewport(menuCallback);
                case ViewportType.TenancyListViewport:
                    return new TenancyListViewport(menuCallback);
                case ViewportType.TenancyPersonsViewport:
                    return new TenancyPersonsViewport(menuCallback);
                case ViewportType.TenancyBuildingsViewport:
                    return new TenancyBuildingsViewport(menuCallback);
                case ViewportType.TenancyPremisesViewport:
                    return new TenancyPremisesViewport(menuCallback);
                case ViewportType.TenancyReasonsViewport:
                    return new TenancyReasonsViewport(menuCallback);
                case ViewportType.TenancyAgreementsViewport:
                    return new TenancyAgreementsViewport(menuCallback);
                case ViewportType.WarrantsViewport:
                    return new WarrantsViewport(menuCallback);
                case ViewportType.TenancyReasonTypesViewport:
                    return new TenancyReasonTypesViewport(menuCallback);
                case ViewportType.ExecutorsViewport:
                    return new ExecutorsViewport(menuCallback);
                case ViewportType.DocumentIssuedByViewport:
                    return new DocumentIssuedByViewport(menuCallback);
                case ViewportType.ClaimListViewport:
                    return new ClaimListViewport(menuCallback);
                case ViewportType.ClaimStatesViewport:
                    return new ClaimStatesViewport(menuCallback);
                case ViewportType.ClaimStateTypesViewport:
                    return new ClaimStateTypesViewport(menuCallback);
                case ViewportType.FundsHistoryViewport:
                    return new FundsHistoryViewport(menuCallback);
            }
            throw new ViewportException(
                String.Format("В фабрику ViewportFactory передан неизвестный тип {0}", viewportType.ToString()));
        }
    }
}
