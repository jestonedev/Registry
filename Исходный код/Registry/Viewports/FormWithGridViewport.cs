using System.Windows.Forms;

namespace Registry.Viewport
{
    internal class FormWithGridViewport: FormViewport
    {
        private DataGridView _dataGridView;
        protected DataGridView DataGridView
        {
            get { return _dataGridView; }
            set
            {
                _dataGridView = value;
                _dataGridView.DataError += DataGridView_DataError;
            }
        }

        protected FormWithGridViewport(): this(null, null)
        {
        }

        protected FormWithGridViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
        }

        void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        protected override void CheckViewportModifications()
        {
            if (!IsEditable)
                return;
            if (!ContainsFocus || (DataGridView.Focused))
                return;
            if ((GeneralBindingSource.Position != -1) && (!EntityFromView().Equals(EntityFromViewport())))
            {
                if (ViewportState == ViewportState.ReadState)
                {
                    ViewportState = ViewportState.ModifyRowState;
                    DataGridView.Enabled = false;
                }
            }
            else
            {
                if (ViewportState == ViewportState.ModifyRowState)
                {
                    ViewportState = ViewportState.ReadState;
                    DataGridView.Enabled = true;
                }
            }
            if (Selected)
                MenuCallback.EditingStateUpdate();
        }
    }
}
