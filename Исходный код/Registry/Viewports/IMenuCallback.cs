using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Viewport
{
    public interface IMenuCallback
    {
        void TabsStateUpdate();
        void NavigationStateUpdate();
        void EditingStateUpdate();
        void RelationsStateUpdate();
        void DocumentsStateUpdate();
        void StatusBarStateUpdate();
        void ForceCloseDetachedViewports();
        void AddViewport(Viewport viewport);
        void SwitchToPreviousViewport();
    }
}
