using Registry.Viewport.Presenters;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ClaimCourtOrdersViewport : FormWithGridViewport
    {
        public ClaimCourtOrdersViewport()
            : this(null, null)
        {
        }

        public ClaimCourtOrdersViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new ClaimCourtOrdersPresenter())
        {
            InitializeComponent();
            dataGridViewClaimPersons.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }
    }
}
