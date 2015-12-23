using System.Windows.Forms;

namespace Registry.Viewport.SearchForms
{
    public class SearchForm: Form
    {
        internal virtual string GetFilter() { return ""; }
    }
}
