using System;
using System.Windows.Forms;
using Registry.Viewport.Presenters;

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

        protected FormWithGridViewport(Viewport viewport, IMenuCallback menuCallback, Presenter presenter = null)
            : base(viewport, menuCallback, presenter)
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

        public void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in DataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = DataGridView.Columns[e.ColumnIndex].Name + " " +
                    (way == SortOrder.Ascending ? "ASC" : "DESC");
                DataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(DataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            DataGridView.Refresh();
        }

        public void DataGridView_SelectionChanged()
        {
            if (DataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = DataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        protected virtual void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (GeneralBindingSource.Position == -1 || DataGridView.RowCount == 0)
            {
                DataGridView.ClearSelection();
                return;
            }
            if (GeneralBindingSource.Position >= DataGridView.RowCount)
            {
                DataGridView.Rows[DataGridView.RowCount - 1].Selected = true;
                var dataGridViewColumn = DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                if (dataGridViewColumn != null)
                    DataGridView.CurrentCell = DataGridView.CurrentCell != null ?
                        DataGridView.Rows[DataGridView.RowCount - 1].Cells[DataGridView.CurrentCell.ColumnIndex] :
                        DataGridView.Rows[DataGridView.RowCount - 1].Cells[dataGridViewColumn.Index];
            }
            else
            {
                DataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                var dataGridViewColumn = DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                if (dataGridViewColumn != null)
                    DataGridView.CurrentCell = DataGridView.CurrentCell != null ?
                        DataGridView.Rows[GeneralBindingSource.Position].Cells[DataGridView.CurrentCell.ColumnIndex] :
                        DataGridView.Rows[GeneralBindingSource.Position].Cells[dataGridViewColumn.Index];
            }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
            MenuCallback.DocumentsStateUpdate();
        }
    }
}
