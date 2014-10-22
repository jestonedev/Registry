using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Viewport
{
    public class ViewportFactory
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
                case ViewportType.RestrictionTypeListViewport: 
                    return new RestrictionTypeListViewport(menuCallback);
                case ViewportType.StructureTypeListViewport: 
                    return new StructureTypeListViewport(menuCallback);
                case ViewportType.TenancyListViewport:
                    return new TenancyListViewport(menuCallback);
                case ViewportType.WarrantsViewport:
                    return new WarrantsViewport(menuCallback);
                case ViewportType.ReasonTypesViewport:
                    return new ReasonTypesViewport(menuCallback);
                case ViewportType.ExecutorsViewport:
                    return new ExecutorsViewport(menuCallback);
                case ViewportType.DocumentIssuedByViewport:
                    return new DocumentIssuedByViewport(menuCallback);
                case ViewportType.Claims:
                    return new ClaimListViewport(menuCallback);
                case ViewportType.ClaimStateTypes:
                    return new ClaimStateTypesViewport(menuCallback);
            }
            throw new ViewportException(
                String.Format("В фабрику ViewportFactory передан неизвестный тип {0}", viewportType.ToString()));
        }
    }
}
