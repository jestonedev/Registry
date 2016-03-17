using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Registry.Viewport.ModalEditors;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    public sealed partial class MultiPremisesMaster : DockContent, IMultiMaster
    {
        private readonly BindingSource _premises = new BindingSource();
        private readonly DataModel _premisesDataModel;
        private readonly IMenuCallback _menuCallback;

        public MultiPremisesMaster(IMenuCallback menuCallback)
        {
            InitializeComponent();
            DockAreas = ((DockAreas.DockLeft | DockAreas.DockRight)
                         | DockAreas.DockTop)
                        | DockAreas.DockBottom;
            _menuCallback = menuCallback;
            _premisesDataModel = DataModel.GetInstance(DataModelType.PremisesDataModel);
            _premisesDataModel.Select();
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
            if (filter == "") filter = "1=1";
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

        private static bool ValidatePermissions(int idPremises)
        {
            if (DataModelHelper.HasMunicipal(idPremises, EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации о реквизитах НПА муниципальных объектов. "+
                    @"Удалите из списка на массовую операцию все муниципальные объекты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.HasNotMunicipal(idPremises, EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации о реквизитах НПА немуниципальных объектов"+
                    @"Удалите из списка на массовую операцию все немуниципальные объекты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }
        
        private void toolStripButtonRestrictions_Click(object sender, EventArgs e)
        {
            var restrictions = DataModel.GetInstance(DataModelType.RestrictionsDataModel);
            var restrictionsAssoc = DataModel.GetInstance(DataModelType.RestrictionsPremisesAssocDataModel);
            if (restrictions.EditingNewRecord || restrictionsAssoc.EditingNewRecord)
            {
                MessageBox.Show(@"Форма реквизитов уже находится в режиме добавления новой записи. "+
                    @"Просмотрите все вкладки и отмените добавление новой записи перед тем, как воспользоваться мастером.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            for (var i = 0; i < _premises.Count; i++)
            {
                int? idPremises = null;
                if (((DataRowView)_premises[i])["id_premises"] != DBNull.Value)
                {
                    idPremises = (int)((DataRowView)_premises[i])["id_premises"];
                }
                if (idPremises == null)
                {
                    continue;
                }
                if (!ValidatePermissions(idPremises.Value))
                {
                    return;
                }
            }
            using (var form = new RestrictionsEditorMultiMaster())
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = _premises.Count - 1;
                toolStripProgressBar1.Visible = true;
                for (var i = 0; i < _premises.Count; i++)
                {
                    int? idPremises = null;
                    if (((DataRowView)_premises[i])["id_premises"] != DBNull.Value)
                    {
                        idPremises = (int)((DataRowView)_premises[i])["id_premises"];
                    }
                    if (idPremises == null)
                    {
                        continue;
                    }
                    var restriction = new Restriction
                    {
                        IdRestrictionType = form.IdRestrictionType,
                        Date = form.Date,
                        Number = form.RestrictionNumber,
                        Description = form.RestrictionDescription
                    };
                    var idRestriction = restrictions.Insert(restriction);
                    if (idRestriction == -1)
                    {
                        MessageBox.Show(
                            string.Format(
                                "Для помещения с реестровым номером {0} и всех следующих в списке после него не был проставлен реквизит. ",
                                idPremises),
                            @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        break;
                    }
                    
                    var assoc = new RestrictionObjectAssoc(idPremises, idRestriction, null);
                    restrictionsAssoc.Insert(assoc);
                    restrictions.EditingNewRecord = true;
                    restrictions.Select().Rows.Add(idRestriction, restriction.IdRestrictionType, restriction.Number, restriction.Date, restriction.Description);
                    restrictionsAssoc.Select().Rows.Add(idPremises, idRestriction);
                    restrictions.EditingNewRecord = false;
                    toolStripProgressBar1.Value = i;
                }
                MessageBox.Show(@"Массовое проставление реквизитов успешно завершено. ",
                    @"Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                toolStripProgressBar1.Visible = false;
            }
        }

        private void toolStripButtonOwnerships_Click(object sender, EventArgs e)
        {
            var ownershipsRights = DataModel.GetInstance(DataModelType.OwnershipsRightsDataModel);
            var ownershipsRightsAssoc = DataModel.GetInstance(DataModelType.OwnershipPremisesAssocDataModel);
            if (ownershipsRights.EditingNewRecord || ownershipsRightsAssoc.EditingNewRecord)
            {
                MessageBox.Show(@"Форма ограничений уже находится в режиме добавления новой записи. " +
                    @"Просмотрите все вкладки и отмените добавление новой записи перед тем, как воспользоваться мастером.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            for (var i = 0; i < _premises.Count; i++)
            {
                int? idPremises = null;
                if (((DataRowView)_premises[i])["id_premises"] != DBNull.Value)
                {
                    idPremises = (int)((DataRowView)_premises[i])["id_premises"];
                }
                if (idPremises == null)
                {
                    continue;
                }
                if (!ValidatePermissions(idPremises.Value))
                {
                    return;
                }
            }
            using (var form = new OwnershipsEditorMultiMaster())
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = _premises.Count - 1;
                toolStripProgressBar1.Visible = true;
                for (var i = 0; i < _premises.Count; i++)
                {
                    int? idPremises = null;
                    if (((DataRowView)_premises[i])["id_premises"] != DBNull.Value)
                    {
                        idPremises = (int)((DataRowView)_premises[i])["id_premises"];
                    }
                    if (idPremises == null)
                    {
                        continue;
                    }
                    var ownershipRight = new OwnershipRight
                    {
                        IdOwnershipRightType = form.IdOwnershipType,
                        Date = form.OwnershipDate,
                        Number = form.OwnershipNumber,
                        Description = form.OwnershipDescription
                    };
                    var idOwnershipRight = ownershipsRights.Insert(ownershipRight);
                    if (idOwnershipRight == -1)
                    {
                        MessageBox.Show(
                            string.Format("Для помещения с реестровым номером {0} и всех следующих в списке после него не было проставлено ограничение. ", idPremises),
                            @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        break;
                    }

                    var assoc = new OwnershipRightObjectAssoc(idPremises, idOwnershipRight);
                    ownershipsRightsAssoc.Insert(assoc);
                    ownershipsRights.EditingNewRecord = true;
                    ownershipsRights.Select().Rows.Add(idOwnershipRight, ownershipRight.IdOwnershipRightType,
                        ownershipRight.Number, ownershipRight.Date, ownershipRight.Description);
                    ownershipsRightsAssoc.Select().Rows.Add(idPremises, idOwnershipRight);
                    ownershipsRights.EditingNewRecord = false;
                    toolStripProgressBar1.Value = i;
                }
                MessageBox.Show(@"Массовое проставление ограничений успешно завершено. ",
                    @"Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                toolStripProgressBar1.Visible = false;
            }
        }

        private void toolStripButtonObjectStates_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < _premises.Count; i++)
            {
                int? idPremises = null;
                if (((DataRowView)_premises[i])["id_premises"] != DBNull.Value)
                {
                    idPremises = (int)((DataRowView)_premises[i])["id_premises"];
                }
                if (idPremises == null)
                {
                    continue;
                }
                if (!ValidatePermissions(idPremises.Value))
                {
                    return;
                }
            }
            using (var form = new ObjectStateEditor())
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = _premises.Count - 1;
                toolStripProgressBar1.Visible = true;
                for (var i = 0; i < _premises.Count; i++)
                {
                    int? idPremises = null;
                    if (((DataRowView)_premises[i])["id_premises"] != DBNull.Value)
                    {
                        idPremises = (int)((DataRowView)_premises[i])["id_premises"];
                    }
                    if (idPremises == null)
                    {
                        continue;
                    }
                    var premise = PremiseFromRow((DataRowView) _premises[i]);
                    premise.IdState = form.IdObjectState;
                    if (_premisesDataModel.Update(premise) == -1)
                    {
                        return;
                    }
                    ((DataRowView) _premises[i])["id_state"] = form.IdObjectState;
                    toolStripProgressBar1.Value = i;
                }
                MessageBox.Show(@"Массовое проставление текущих состояний помещений успешно завершено. ",
                    @"Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                toolStripProgressBar1.Visible = false;
            }
        }

        private Premise PremiseFromRow(DataRowView row)
        {
            var premise = new Premise
            {
                IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                IdBuilding = ViewportHelper.ValueOrNull<int>(row, "id_building"),
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                PremisesNum = ViewportHelper.ValueOrNull(row, "premises_num"),
                LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area"),
                TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area"),
                Height = ViewportHelper.ValueOrNull<double>(row, "height"),
                NumRooms = ViewportHelper.ValueOrNull<short>(row, "num_rooms"),
                NumBeds = ViewportHelper.ValueOrNull<short>(row, "num_beds"),
                IdPremisesType = ViewportHelper.ValueOrNull<int>(row, "id_premises_type"),
                IdPremisesKind = ViewportHelper.ValueOrNull<int>(row, "id_premises_kind"),
                Floor = ViewportHelper.ValueOrNull<short>(row, "floor"),
                CadastralNum = ViewportHelper.ValueOrNull(row, "cadastral_num"),
                CadastralCost = ViewportHelper.ValueOrNull<decimal>(row, "cadastral_cost"),
                BalanceCost = ViewportHelper.ValueOrNull<decimal>(row, "balance_cost"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                IsMemorial = ViewportHelper.ValueOrNull<bool>(row, "is_memorial"),
                Account = ViewportHelper.ValueOrNull(row, "account"),
                RegDate = ViewportHelper.ValueOrNull<DateTime>(row, "reg_date"),
                StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date")
            };
            return premise;
        }
    }
}
