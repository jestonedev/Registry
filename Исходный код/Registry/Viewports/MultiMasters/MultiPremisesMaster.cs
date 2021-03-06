﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ModalEditors;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport.MultiMasters
{
    internal sealed partial class MultiPremisesMaster : DockContent, IMultiMaster
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
            _premisesDataModel = EntityDataModel<Premise>.GetInstance();
            _premisesDataModel.Select();
            _premises.DataSource = DataStorage.DataSet;
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
            var buildingRow = DataModel.GetInstance<EntityDataModel<Building>>().Select().Rows.Find(row["id_building"]);
            if (buildingRow == null)
                return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "house":
                    e.Value = buildingRow[dataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "id_premises":
                case "premises_num":
                    e.Value = row[dataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "id_street":
                    var kladrRow = DataModel.GetInstance<KladrStreetsDataModel>().Select().Rows.Find(buildingRow["id_street"]);
                    string streetName = null;
                    if (kladrRow != null)
                        streetName = kladrRow["street_name"].ToString();
                    e.Value = streetName;
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

            var munPremises = from row in EntityDataModel<Premise>.GetInstance().FilterDeletedRows()
                where DataModelHelper.MunicipalObjectStates().Contains(row.Field<int?>("id_state") ?? 0) && row.Field<int?>("id_building") == idBuilding
                select row.Field<int>("id_premises");

            _premises.Filter = string.Format("({0}) OR (id_premises IN ({1}))",
                _premises.Filter, munPremises.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
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
            IEnumerable<int> idPremises = new List<int>();
            var premisesListViewport = viewport as PremisesListViewport;
            if (premisesListViewport != null)
                idPremises = premisesListViewport.GetCurrentIds();
            var premisesViewport = viewport as PremisesViewport;
            if (premisesViewport != null)
                idPremises = new List<int> {premisesViewport.GetCurrentId()};
            if (!idPremises.Any()) return;
            _premises.Filter = string.Format("({0}) OR (id_premises IN ({1}))", _premises.Filter,
                idPremises.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
            dataGridView.RowCount = _premises.Count;
        }

        private void toolStripButtonPremisesByFilter_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            IEnumerable<int> idPremises = new List<int>();
            var premisesListViewport = viewport as PremisesListViewport;
            if (premisesListViewport != null)
                idPremises = premisesListViewport.GetFilteredIds();
            var premisesViewport = viewport as PremisesViewport;
            if (premisesViewport != null)
                idPremises = premisesViewport.GetFilteredIds();
            if (!idPremises.Any()) return;
            _premises.Filter = string.Format("({0}) OR (id_premises IN ({1}))", _premises.Filter,
                idPremises.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
            dataGridView.RowCount = _premises.Count;
        }

        private void toolStripButtonGenerateExcerpt_Click(object sender, EventArgs e)
        {
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
            _menuCallback.RunReport(ReporterType.MultiExcerptReporter, arguments);
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
            if (OtherService.HasMunicipal(idPremises, EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации о реквизитах НПА муниципальных объектов. "+
                    @"Удалите из списка на массовую операцию все муниципальные объекты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (OtherService.HasNotMunicipal(idPremises, EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации о реквизитах НПА немуниципальных объектов"+
                    @"Удалите из списка на массовую операцию все немуниципальные объекты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private bool ValidatePermissionsAll()
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
                    return false;
                }
            }
            return true;
        }

        private void toolStripButtonRestrictions_Click(object sender, EventArgs e)
        {
            var restrictions = EntityDataModel<Restriction>.GetInstance();
            var restrictionsAssoc = EntityDataModel<RestrictionPremisesAssoc>.GetInstance();
            if (restrictions.EditingNewRecord || restrictionsAssoc.EditingNewRecord)
            {
                MessageBox.Show(@"Форма реквизитов уже находится в режиме добавления новой записи. "+
                    @"Просмотрите все вкладки и отмените добавление новой записи перед тем, как воспользоваться мастером.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (!ValidatePermissionsAll())
            {
                return;
            }
            using (var form = new RestrictionsEditorMultiMaster())
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                toolStripProgressBarMultiOperations.Value = 0;
                toolStripProgressBarMultiOperations.Maximum = _premises.Count - 1;
                toolStripProgressBarMultiOperations.Visible = true;
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
                        DateStateReg = form.DateStateReg,
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
                    restrictionsAssoc.Insert(new RestrictionPremisesAssoc(idPremises, idRestriction, null));
                    restrictions.EditingNewRecord = true;
                    restrictions.Select().Rows.Add(idRestriction, restriction.IdRestrictionType, restriction.Number, restriction.Date, restriction.Description, restriction.DateStateReg);
                    restrictionsAssoc.Select().Rows.Add(idPremises, idRestriction);
                    restrictions.EditingNewRecord = false;
                    toolStripProgressBarMultiOperations.Value = i;
                }
                MessageBox.Show(@"Массовое проставление реквизитов успешно завершено. ",
                    @"Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                toolStripProgressBarMultiOperations.Visible = false;
            }
        }

        private void toolStripButtonOwnerships_Click(object sender, EventArgs e)
        {
            var ownershipsRights = EntityDataModel<OwnershipRight>.GetInstance();
            var ownershipsRightsAssoc = EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance();
            if (ownershipsRights.EditingNewRecord || ownershipsRightsAssoc.EditingNewRecord)
            {
                MessageBox.Show(@"Форма ограничений уже находится в режиме добавления новой записи. " +
                    @"Просмотрите все вкладки и отмените добавление новой записи перед тем, как воспользоваться мастером.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (!ValidatePermissionsAll())
            {
                return;
            }
            using (var form = new OwnershipsEditorMultiMaster())
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                toolStripProgressBarMultiOperations.Value = 0;
                toolStripProgressBarMultiOperations.Maximum = _premises.Count - 1;
                toolStripProgressBarMultiOperations.Visible = true;
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

                    var assoc = new OwnershipRightPremisesAssoc(idPremises, idOwnershipRight);
                    ownershipsRightsAssoc.Insert(assoc);
                    ownershipsRights.EditingNewRecord = true;
                    ownershipsRights.Select().Rows.Add(idOwnershipRight, ownershipRight.IdOwnershipRightType,
                        ownershipRight.Number, ownershipRight.Date, ownershipRight.Description);
                    ownershipsRightsAssoc.Select().Rows.Add(idPremises, idOwnershipRight);
                    ownershipsRights.EditingNewRecord = false;
                    toolStripProgressBarMultiOperations.Value = i;
                }
                MessageBox.Show(@"Массовое проставление ограничений успешно завершено. ",
                    @"Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                toolStripProgressBarMultiOperations.Visible = false;
            }
        }

        private void toolStripButtonObjectStates_Click(object sender, EventArgs e)
        {
            if (!ValidatePermissionsAll())
            {
                return;
            }
            using (var form = new ObjectStateEditor())
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                toolStripProgressBarMultiOperations.Value = 0;
                toolStripProgressBarMultiOperations.Maximum = _premises.Count - 1;
                toolStripProgressBarMultiOperations.Visible = true;
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
                    var premise = EntityConverter<Premise>.FromRow((DataRowView)_premises[i]);
                    premise.IdState = form.IdObjectState;
                    if (_premisesDataModel.Update(premise) == -1)
                    {
                        return;
                    }
                    ((DataRowView) _premises[i])["id_state"] = form.IdObjectState;
                    toolStripProgressBarMultiOperations.Value = i;
                }
                MessageBox.Show(@"Массовое проставление текущих состояний помещений успешно завершено. ",
                    @"Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                toolStripProgressBarMultiOperations.Visible = false;
            }
        }

        private void toolStripButtonRegDate_Click(object sender, EventArgs e)
        {
            if (!ValidatePermissionsAll())
            {
                return;
            }
            using (var form = new RegDateEditor())
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                toolStripProgressBarMultiOperations.Value = 0;
                toolStripProgressBarMultiOperations.Maximum = _premises.Count - 1;
                toolStripProgressBarMultiOperations.Visible = true;
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
                    var premise = EntityConverter<Premise>.FromRow((DataRowView)_premises[i]);
                    premise.RegDate = form.RegDate;
                    if (_premisesDataModel.Update(premise) == -1)
                    {
                        return;
                    }
                    ((DataRowView)_premises[i])["reg_date"] = form.RegDate;
                    toolStripProgressBarMultiOperations.Value = i;
                }
                dataGridView.Refresh();
                MessageBox.Show(@"Массовое проставление даты включения в РМИ помещений успешно завершено. ",
                    @"Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                toolStripProgressBarMultiOperations.Visible = false;
            }
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

        private void toolStripButtonExport_Click(object sender, EventArgs e)
        {
            string columnHeaders;
            string columnPatterns;
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                columnHeaders = "{\"columnHeader\":\"№ по реестру\"},{\"columnHeader\":\"Адрес\"},{\"columnHeader\":\"Дом\"},"+
                    "{\"columnHeader\":\"Пом.\"},{\"columnHeader\":\"Тип помещения\"},{\"columnHeader\":\"Кадастровый номер\"},"+
                    "{\"columnHeader\":\"Общая площадь\"},{\"columnHeader\":\"Текущее состояние\"},{\"columnHeader\":\"Текущий фонд\"},"+
                    "{\"columnHeader\":\"№ договора найма\"},{\"columnHeader\":\"Дата регистрации договора\"},"+
                    "{\"columnHeader\":\"Дата окончания договора\"},{\"columnHeader\":\"№ ордера найма\"},{\"columnHeader\":\"Дата ордера найма\"},"+
                    "{\"columnHeader\":\"Наниматель\"},{\"columnHeader\":\"Размер платы\"},{\"columnHeader\":\"Номер и дата включения в фонд\"},"+
                    "{\"columnHeader\":\"Дополнительные сведения\"}";
                columnPatterns = "{\"columnPattern\":\"$column0$\"},{\"columnPattern\":\"$column1$\"},{\"columnPattern\":\"$column2$\"}," +
                    "{\"columnPattern\":\"$column3$\"},{\"columnPattern\":\"$column4$\"},{\"columnPattern\":\"$column5$\"}," +
                    "{\"columnPattern\":\"$column6$\"},{\"columnPattern\":\"$column8$\"},{\"columnPattern\":\"$column9$\"}," +
                    "{\"columnPattern\":\"$column10$\"},{\"columnPattern\":\"$column11$\"}," +
                    "{\"columnPattern\":\"$column12$\"},{\"columnPattern\":\"$column13$\"},{\"columnPattern\":\"$column14$\"}," +
                    "{\"columnPattern\":\"$column15$\"},{\"columnPattern\":\"$column16$\"},{\"columnPattern\":\"$fund_info$\"}," +
                    "{\"columnPattern\":\"$description$\"}";
            }
            else
            {
                columnHeaders = "{\"columnHeader\":\"№ по реестру\"},{\"columnHeader\":\"Адрес\"},{\"columnHeader\":\"Дом\"}," +
                    "{\"columnHeader\":\"Пом.\"},{\"columnHeader\":\"Тип помещения\"},{\"columnHeader\":\"Кадастровый номер\"}," +
                    "{\"columnHeader\":\"Общая площадь\"},{\"columnHeader\":\"Текущее состояние\"},{\"columnHeader\":\"Текущий фонд\"}," +
                    "{\"columnHeader\":\"Дополнительные сведения\"}";
                columnPatterns = "{\"columnPattern\":\"$column0$\"},{\"columnPattern\":\"$column1$\"},{\"columnPattern\":\"$column2$\"}," +
                    "{\"columnPattern\":\"$column3$\"},{\"columnPattern\":\"$column4$\"},{\"columnPattern\":\"$column5$\"}," +
                    "{\"columnPattern\":\"$column6$\"},{\"columnPattern\":\"$column8$\"},{\"columnPattern\":\"$fund_info$\"}," +
                    "{\"columnPattern\":\"$description$\"}";
            }
            var filter = _premises.Filter ?? "";   
            var arguments = new Dictionary<string, string>
            {
                {"type", "2"},
                {"filter", filter.Trim() == "" ? "(1=1)" : filter },
                {"columnHeaders", "["+columnHeaders+"]"},
                {"columnPatterns", "["+columnPatterns+"]"}
            };
            _menuCallback.RunReport(ReporterType.ExportReporter, arguments);
        }
    }
}
