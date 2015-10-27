using System;
using System.Globalization;

namespace Registry.Viewport
{
    public static class ViewportFactory
    {
        public static Viewport CreateViewport(IMenuCallback menuCallback, ViewportType viewportType)
        {
            switch (viewportType)
            {
                case ViewportType.BuildingListViewport:
                    return new BuildingListViewport(null, menuCallback);
                case ViewportType.BuildingViewport:
                    return new BuildingViewport(null, menuCallback);
                case ViewportType.OwnershipListViewport:
                    return new OwnershipListViewport(null, menuCallback);
                case ViewportType.OwnershipTypeListViewport:
                    return new OwnershipTypeListViewport(null, menuCallback);
                case ViewportType.PremisesListViewport: 
                    return new PremisesListViewport(null, menuCallback);
                case ViewportType.PremisesViewport:
                    return new PremisesViewport(null, menuCallback);
                case ViewportType.RestrictionListViewport:
                    return new RestrictionListViewport(null, menuCallback);
                case ViewportType.SubPremisesViewport:
                    return new SubPremisesViewport(null, menuCallback);
                case ViewportType.RestrictionTypeListViewport:
                    return new RestrictionTypeListViewport(null, menuCallback);
                case ViewportType.StructureTypeListViewport:
                    return new StructureTypeListViewport(null, menuCallback);
                case ViewportType.TenancyListViewport:
                    return new TenancyListViewport(null, menuCallback);
                case ViewportType.TenancyPersonsViewport:
                    return new TenancyPersonsViewport(null, menuCallback);
                case ViewportType.TenancyBuildingsViewport:
                    return new TenancyBuildingsViewport(null, menuCallback);
                case ViewportType.TenancyPremisesViewport:
                    return new TenancyPremisesViewport(null, menuCallback);
                case ViewportType.TenancyReasonsViewport:
                    return new TenancyReasonsViewport(null, menuCallback);
                case ViewportType.TenancyAgreementsViewport:
                    return new TenancyAgreementsViewport(null, menuCallback);
                case ViewportType.WarrantsViewport:
                    return new WarrantsViewport(null, menuCallback);
                case ViewportType.TenancyReasonTypesViewport:
                    return new TenancyReasonTypesViewport(null, menuCallback);
                case ViewportType.ExecutorsViewport:
                    return new ExecutorsViewport(null, menuCallback);
                case ViewportType.DocumentIssuedByViewport:
                    return new DocumentIssuedByViewport(null, menuCallback);
                case ViewportType.ClaimListViewport:
                    return new ClaimListViewport(null, menuCallback);
                case ViewportType.ClaimStatesViewport:
                    return new ClaimStatesViewport(null, menuCallback);
                case ViewportType.ClaimStateTypesViewport:
                    return new ClaimStateTypesViewport(null, menuCallback);
                case ViewportType.FundsHistoryViewport:
                    return new FundsHistoryViewport(null, menuCallback);
                case ViewportType.DocumentsResidenceViewport:
                    return new DocumentsResidenceViewport(null, menuCallback);
                case ViewportType.ResettleProcessListViewport:
                    return new ResettleProcessListViewport(null, menuCallback);
                case ViewportType.ResettlePersonsViewport:
                    return new ResettlePersonsViewport(null, menuCallback);
                case ViewportType.ResettleFromBuildingsViewport:
                case ViewportType.ResettleToBuildingsViewport:
                    return new ResettleBuildingsViewport(null, menuCallback);
                case ViewportType.ResettleFromPremisesViewport:
                case ViewportType.ResettleToPremisesViewport:
                    return new ResettlePremisesViewport(null, menuCallback);
            }
            throw new ViewportException(
                string.Format(CultureInfo.InvariantCulture, "В фабрику ViewportFactory передан неизвестный тип {0}", viewportType));
        }
    }
}
