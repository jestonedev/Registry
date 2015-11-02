using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Reporting;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    public sealed partial class MultiExcerptsMaster : DockContent
    {
        private readonly BindingSource _premises = new BindingSource();
        private readonly IMenuCallback _menuCallback;

        public MultiExcerptsMaster(IMenuCallback menuCallback)
        {
            InitializeComponent();
            DockAreas = ((DockAreas.DockLeft | DockAreas.DockRight)
                         | DockAreas.DockTop)
                        | DockAreas.DockBottom;
            _menuCallback = menuCallback;
            DataModel.GetInstance(DataModelType.PremisesDataModel).Select();
            _premises.DataSource = DataModel.DataSet;
            _premises.DataMember = "premises";
            _premises.Filter = "0 = 1";
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
                _premises.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
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
                _premises.Position = dataGridView.SelectedRows[0].Index;
            else
                _premises.Position = -1;
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_premises.Count <= e.RowIndex) return;
            var row = ((DataRowView)_premises[e.RowIndex]);
            var buildingRow = DataModel.GetInstance(DataModelType.BuildingsDataModel).Select().Rows.Find(row["id_building"]);
            if (buildingRow == null)
                return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_premises":
                    e.Value = row["id_premises"];
                    break;
                case "id_street":
                    var kladrRow = DataModel.GetInstance(DataModelType.KladrStreetsDataModel).Select().Rows.Find(buildingRow["id_street"]);
                    string streetName = null;
                    if (kladrRow != null)
                        streetName = kladrRow["street_name"].ToString();
                    e.Value = streetName;
                    break;
                case "house":
                    e.Value = buildingRow["house"];
                    break;
                case "premises_num":
                    e.Value = row["premises_num"];
                    break;
            }
        }

        private void toolStripButtonBuildingAllPremises_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            var idBuilding = -1;
            var buildingListViewport = viewport as BuildingListViewport;
            if (buildingListViewport != null)
                idBuilding = buildingListViewport.GetCurrentId();
            var buildingsViewport = viewport as BuildingViewport;
            if (buildingsViewport != null)
                idBuilding = buildingsViewport.GetCurrentId();
            if (idBuilding == -1) return;
            _premises.Filter = string.Format("({0}) OR id_building = {1}", _premises.Filter, idBuilding);
            dataGridView.RowCount = _premises.Count;
        }

        private void toolStripButtonBuildingMunPremises_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            var idBuilding = -1;
            var buildingListViewport = viewport as BuildingListViewport;
            if (buildingListViewport != null)
                idBuilding = buildingListViewport.GetCurrentId();
            var buildingsViewport = viewport as BuildingViewport;
            if (buildingsViewport != null)
                idBuilding = buildingsViewport.GetCurrentId();
            if (idBuilding == -1) return;
            _premises.Filter = string.Format("({0}) OR (id_building = {1} AND id_state IN (4,5,9))", _premises.Filter, idBuilding);
            dataGridView.RowCount = _premises.Count;
        }

        private void toolStripButtonPremisesDeleteAll_Click(object sender, EventArgs e)
        {
            _premises.Filter = "0 = 1";
            dataGridView.RowCount = _premises.Count;
        }

        private void toolStripButtonPremisesDelete_Click(object sender, EventArgs e)
        {
            if (_premises.Position < 0) return;
            if (((DataRowView) _premises[_premises.Position])["id_premises"] == DBNull.Value) return;
            var idPremises = (int)((DataRowView) _premises[_premises.Position])["id_premises"];
            _premises.Filter = string.Format("({0}) AND (id_premises <> {1})", _premises.Filter, idPremises);
            dataGridView.RowCount = _premises.Count;
            dataGridView.Refresh();
        }

        private void toolStripButtonPremisesCurrent_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            var idPremises = -1;
            var premisesListViewport = viewport as PremisesListViewport;
            if (premisesListViewport != null)
                idPremises = premisesListViewport.GetCurrentId();
            var premisesViewport = viewport as PremisesViewport;
            if (premisesViewport != null)
                idPremises = premisesViewport.GetCurrentId();
            if (idPremises == -1) return;
            _premises.Filter = string.Format("({0}) OR (id_premises = {1})", _premises.Filter, idPremises);
            dataGridView.RowCount = _premises.Count;
        }

        private void toolStripButtonPremisesByFilter_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            var filter = "";
            var premisesListViewport = viewport as PremisesListViewport;
            if (premisesListViewport != null)
                filter = premisesListViewport.GetFilter();
            var premisesViewport = viewport as PremisesViewport;
            if (premisesViewport != null)
                filter = premisesViewport.GetFilter();
            if (filter == "") return;
            _premises.Filter = string.Format("({0}) OR ({1})", _premises.Filter, filter);
            dataGridView.RowCount = _premises.Count;
        }

        private void toolStripButtonGenerateExcerpt_Click(object sender, EventArgs e)
        {
            var reporter = ReporterFactory.CreateReporter(ReporterType.MultiExcerptReporter);
            var arguments = new Dictionary<string, string>();
            var filter = "";
            for (var i = 0; i < _premises.Count; i++)
            {
                var row = ((DataRowView) _premises[i]);
                if (row["id_premises"] != DBNull.Value)
                    filter += row["id_premises"] + ",";
            }
            filter = filter.TrimEnd(',');
            arguments.Add("filter", filter);
            reporter.Run(arguments);
        }

        public void UpdateToolbar()
        {
            var viewport = _menuCallback.GetCurrentViewport();
            toolStripButtonPremisesCurrent.Visible = false;
            toolStripButtonPremisesByFilter.Visible = false;
            toolStripButtonBuildingAllPremises.Visible = false;
            toolStripButtonBuildingMunPremises.Visible = false;
            if (viewport is BuildingViewport || viewport is BuildingListViewport)
            {
                toolStripButtonBuildingAllPremises.Visible = true;
                toolStripButtonBuildingMunPremises.Visible = true;
            }
            if (!(viewport is PremisesViewport) && !(viewport is PremisesListViewport)) return;
            toolStripButtonPremisesCurrent.Visible = true;
            toolStripButtonPremisesByFilter.Visible = true;
        }
    }
}
