using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.Viewport.Presenters;
using Registry.Viewport.SearchForms;

namespace Registry.Viewport
{
    internal class DataGridViewport: Viewport
    {
        protected DataGridView DataGridView;

        protected DataGridViewport():this(null,null)
        {
        }

        protected DataGridViewport(Viewport viewport, IMenuCallback menuCallback, Presenter presenter = null): 
            base(viewport, menuCallback, presenter)
        {
        }

        public sealed override bool CanMoveFirst()
        {
            return GeneralBindingSource.Position > 0;
        }

        public sealed override bool CanMovePrev()
        {
            return GeneralBindingSource.Position > 0;
        }

        public sealed override bool CanMoveNext()
        {
            return (GeneralBindingSource.Position > -1) && (GeneralBindingSource.Position < (GeneralBindingSource.Count - 1));
        }

        public sealed override bool CanMoveLast()
        {
            return (GeneralBindingSource.Position > -1) && (GeneralBindingSource.Position < (GeneralBindingSource.Count - 1));
        }

        public sealed override void MoveFirst()
        {
            GeneralBindingSource.MoveFirst();
        }

        public sealed override void MovePrev()
        {
            GeneralBindingSource.MovePrevious();
        }

        public sealed override void MoveNext()
        {
            GeneralBindingSource.MoveNext();
        }

        public sealed override void MoveLast()
        {
            GeneralBindingSource.MoveLast();
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
                DataGridView.CurrentCell = DataGridView.CurrentCell != null ? 
                    DataGridView.Rows[DataGridView.RowCount - 1].Cells[DataGridView.CurrentCell.ColumnIndex] : 
                    DataGridView.Rows[DataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                DataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                DataGridView.CurrentCell = DataGridView.CurrentCell != null ? 
                    DataGridView.Rows[GeneralBindingSource.Position].Cells[DataGridView.CurrentCell.ColumnIndex] : 
                    DataGridView.Rows[GeneralBindingSource.Position].Cells[0];
            }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
            MenuCallback.DocumentsStateUpdate();
        }

        protected void GeneralDataSource_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                DataGridView.Refresh();
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        protected void GeneralDataSource_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        public override void LocateEntityBy(string fieldName, object value)
        {
            var position = GeneralBindingSource.Find(fieldName, value);
            if (position > 0)
                GeneralBindingSource.Position = position;
        }

        public int GetCurrentId()
        {
            return Presenter.GetCurrentId();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = (Viewport)Activator.CreateInstance(GetType(), this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (GeneralBindingSource.Count <= 0 || !GeneralDataModel.Select().PrimaryKey.Any()) return viewport;
            var fileName = GeneralDataModel.Select().PrimaryKey[0].ColumnName;
            viewport.LocateEntityBy(fileName,
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])[fileName] as int? ?? -1);
            return viewport;
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (Presenter.SimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = Presenter.SimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (Presenter.ExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = Presenter.ExtendedSearchForm.GetFilter();
                    break;
            }
            DataGridView.RowCount = 0;
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
        }

        public override void ClearSearch()
        {
            DataGridView.RowCount = 0;
            Presenter.ViewModel["general"].BindingSource.Filter = StaticFilter;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DynamicFilter = "";
        }

        public override bool SearchedRecords()
        {
            return !string.IsNullOrEmpty(DynamicFilter);
        }

        public void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in DataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                Presenter.ViewModel["general"].BindingSource.Sort = DataGridView.Columns[e.ColumnIndex].Name + " " + 
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
                Presenter.ViewModel["general"].BindingSource.Position = DataGridView.SelectedRows[0].Index;
            else
                Presenter.ViewModel["general"].BindingSource.Position = -1;
        }
    }
}
