using System.Collections.Generic;
using Registry.Reporting;

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
        void RunReport(ReporterType reporterType, Dictionary<string, string> arguments = null);
        Viewport GetCurrentViewport();
    }
}
