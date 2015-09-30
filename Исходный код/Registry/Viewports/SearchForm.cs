using System.Windows.Forms;

namespace Registry.SearchForms
{
    public class SearchForm: Form
    {
        internal virtual string GetFilter() { return ""; }
    }
}
