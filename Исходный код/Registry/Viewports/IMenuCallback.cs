using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Viewport
{
    public interface IMenuCallback
    {
        void TabsStateUpdate();
        void RegistryStateUpdate();
        void NavigationStateUpdate();
        void EditingStateUpdate();
        void RibbonTabsStateUpdate();
        void RelationsStateUpdate();
        void HousingRefBooksStateUpdate();
        void ForceCloseDetachedViewports();
        void AddViewport(Viewport viewport);
        void SwitchToViewport(Viewport viewport);
    }
}
