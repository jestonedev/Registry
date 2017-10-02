using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport.MultiMasters
{
    internal sealed partial class MultiTenanciesMaster : DockContent, IMultiMaster
    {
        private readonly BindingSource _tenancies = new BindingSource();
        private readonly BindingSource _tenancyAggregated = new BindingSource();
        private readonly BindingSource _tenancyRentTypes = new BindingSource();
        private readonly IMenuCallback _menuCallback;

        public MultiTenanciesMaster(IMenuCallback menuCallback)
        {
            InitializeComponent();
            DockAreas = DockAreas.DockLeft | DockAreas.DockRight
                        | DockAreas.DockTop
                        | DockAreas.DockBottom;
            _menuCallback = menuCallback;
            DataModel.GetInstance<EntityDataModel<TenancyProcess>>().Select();
            _tenancies.DataSource = DataStorage.DataSet;
            _tenancies.DataMember = "tenancy_processes";
            _tenancies.Filter = "0 = 1";

            DataModel.GetInstance<RentTypesDataModel>().Select();
            _tenancies.DataSource = DataStorage.DataSet;
            _tenancies.DataMember = "rent_types";

            _tenancyAggregated.DataSource = CalcDataModel.GetInstance<CalcDataModelTenancyAggregated>().Select();
            dataGridView.RowCount = 0;
        }

        void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                _tenancies.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + (way == SortOrder.Ascending ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                _tenancies.Position = dataGridView.SelectedRows[0].Index;
            else
                _tenancies.Position = -1;
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var bindingSource = _tenancies;
            if (bindingSource.Count <= e.RowIndex) return;
            var row = ((DataRowView)bindingSource[e.RowIndex]);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_process":
                    e.Value = row["id_process"];
                    break;
                case "registration_num":
                    e.Value = row["registration_num"];
                    break;
                case "registration_date":
                    var date = row["registration_date"];
                    e.Value = date is DateTime ? ((DateTime)date).ToString("dd.MM.yyyy") : null;
                    break;
                case "end_date":
                    var date2 = row["end_date"];
                    e.Value = date2 is DateTime ? ((DateTime)date2).ToString("dd.MM.yyyy") : null;
                    break;
                case "residence_warrant_num":
                    e.Value = row["residence_warrant_num"];
                    break;
                case "tenant":
                    var rowIndex = _tenancyAggregated.Find("id_process", row["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_tenancyAggregated[rowIndex])["tenant"];
                    break;
                case "rent_type":
                    rowIndex = _tenancyRentTypes.Find("id_rent_type", row["id_rent_type"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_tenancyRentTypes[rowIndex])["rent_type"];
                    break;
                case "address":
                    rowIndex = _tenancyAggregated.Find("id_process", row["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_tenancyAggregated[rowIndex])["address"];
                    break;
                case "payment":
                    var paymentRows =
                        from paymentRow in CalcDataModel.GetInstance<CalcDataModelTenancyPaymentsInfo>().FilterDeletedRows()
                        where paymentRow.Field<int?>("id_process") == (int?)row["id_process"]
                        select paymentRow.Field<decimal>("payment");
                    e.Value = paymentRows.Sum(r => r);
                    break;
            }
        }

        private void toolStripButtonAccountDeleteAll_Click(object sender, EventArgs e)
        {
            _tenancies.Filter = "0 = 1";
            dataGridView.RowCount = _tenancies.Count;
        }

        private void toolStripButtonTenancyDelete_Click(object sender, EventArgs e)
        {
            if (_tenancies.Position < 0) return;
            var row = (DataRowView) _tenancies[_tenancies.Position];
            if (row["id_process"] == DBNull.Value) return;
            var idProcess = (int)row["id_process"];
            _tenancies.Filter = string.Format("({0}) AND (id_process <> {1})", _tenancies.Filter, idProcess);
            dataGridView.RowCount = _tenancies.Count;
            dataGridView.Refresh();
        }

        private void toolStripButtonTenancyCurrent_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            IEnumerable<int> idProcesses = new List<int>();
            var tenancyListViewport = viewport as TenancyListViewport;
            if (tenancyListViewport != null)
            {
                idProcesses = tenancyListViewport.GetCurrentIds();
            }
            else
            {
                var tenancyViewport = viewport as TenancyViewport;
                if (tenancyViewport != null)
                {
                    idProcesses = tenancyViewport.GetCurrentIds();
                }
            }
            if (!idProcesses.Any()) return;
            _tenancies.Filter = string.Format("({0}) OR (id_process IN ({1}))", _tenancies.Filter, 
                idProcesses.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
            dataGridView.RowCount = _tenancies.Count;
        }

        private void toolStripButtonTenanciesByFilter_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            var filter = "";
            var claimsViewport = viewport as ClaimListViewport;
            if (claimsViewport != null)
                filter = claimsViewport.GetFilter();
            if (filter == "") filter = "1=1";
            _tenancies.Filter = string.Format("({0}) OR ({1})", _tenancies.Filter, filter);
            dataGridView.RowCount = _tenancies.Count;
        }

        private void toolStripButtonRequestMvd_Click(object sender, EventArgs e)
        {
            
        }

        public void UpdateToolbar()
        {
            var viewport = _menuCallback.GetCurrentViewport();
            toolStripButtonTenancyCurrent.Visible = false;
            toolStripButtonTenanciesByFilter.Visible = false;
            if (!(viewport is TenancyListViewport) && !(viewport is TenancyViewport)) return;
            toolStripButtonTenancyCurrent.Visible = true;
            toolStripButtonTenanciesByFilter.Visible = true;
            toolStripButtonRequestMvd.Enabled = AccessControl.HasPrivelege(Priveleges.TenancyRead);
            toolStripButtonRequestMvdNew.Enabled = AccessControl.HasPrivelege(Priveleges.TenancyRead);
        }

        private void toolStripButtonRequestMvdNew_Click(object sender, EventArgs e)
        {
            
        }

        private void dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            RowCountChanged();
        }

        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            RowCountChanged();
        }

        private void RowCountChanged()
        {
            toolStripLabelRowCount.Text = string.Format("Всего записей в мастере: {0}", dataGridView.RowCount);
        }
    }
}
