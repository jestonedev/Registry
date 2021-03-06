﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.Services;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.Presenters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class PremisesListViewport : DataGridViewport
    {

        private PremisesListViewport()
            : this(null, null)
        {
        }

        public PremisesListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new PremisesListPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;

            ViewportHelper.SetDoubleBuffered(DataGridView);
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;
            GeneralDataModel = Presenter.ViewModel["general"].Model;

            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        Text = @"Помещения здания №" + ParentRow["id_building"];
                        break;
                    case ParentTypeEnum.PaymentAccount:
                        Text = string.Format("Помещения по лицевому счету №{0}", ParentRow["account"]);
                        break;
                }
            }

            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                var registrationNumColumn = new DataGridViewTextBoxColumn
                {
                    Name = "registration_num",
                    HeaderText = @"№ договора найма",
                    Width = 90,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var registrationDateColumn = new DataGridViewTextBoxColumn
                {
                    Name = "registration_date",
                    HeaderText = @"Дата регистрации договора",
                    Width = 90,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var endDateColumn = new DataGridViewTextBoxColumn
                {
                    Name = "end_date",
                    HeaderText = @"Дата окончания договора",
                    Width = 90,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var residenceWarrantNumColumn = new DataGridViewTextBoxColumn
                {
                    Name = "residence_warrant_num",
                    HeaderText = @"№ ордера / другого документа-основания",
                    Width = 150,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var residenceWarrantDateColumn = new DataGridViewTextBoxColumn
                {
                    Name = "residence_warrant_date",
                    HeaderText = @"Дата ордера / другого документа-основания",
                    Width = 150,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var tenantColumn = new DataGridViewTextBoxColumn
                {
                    Name = "tenant",
                    HeaderText = @"Наниматель",
                    Width = 250,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                var paymentColumn = new DataGridViewTextBoxColumn
                {
                    Name = "payment",
                    HeaderText = @"Размер платы",
                    Width = 150,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    DefaultCellStyle = {Format = "#0.## руб\\."}
                };
                DataGridView.Columns.Add(registrationNumColumn);
                DataGridView.Columns.Add(registrationDateColumn);
                DataGridView.Columns.Add(endDateColumn);
                DataGridView.Columns.Add(residenceWarrantNumColumn);
                DataGridView.Columns.Add(residenceWarrantDateColumn);
                DataGridView.Columns.Add(tenantColumn);
                DataGridView.Columns.Add(paymentColumn);
                AddEventHandler<EventArgs>(Presenter.ViewModel["premises_current_funds"].Model, "RefreshEvent",
                    (s, e) =>
                    {
                        _idPremises = int.MinValue;
                        DataGridView.Refresh();
                    });
            }

            id_premises_type.DataSource = Presenter.ViewModel["premises_types"].BindingSource;
            id_premises_type.ValueMember = Presenter.ViewModel["premises_types"].PrimaryKeyFirst;
            id_premises_type.DisplayMember = "premises_type";

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged",
                (s, e) => {
                    _idPremises = int.MinValue;
                    GeneralDataSource_RowChanged(s, e);
                });
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted",
                (s, e) => {
                    _idPremises = int.MinValue;
                    GeneralDataSource_RowDeleted(s, e);
                });
            AddEventHandler<EventArgs>(Presenter.ViewModel["premises_current_funds"].Model, "RefreshEvent", (s, e) => DataGridView.Refresh());
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                AddEventHandler<EventArgs>(Presenter.ViewModel["tenancy_payments_info"].Model, "RefreshEvent",
                    (s, e) => DataGridView.Refresh());
                AddEventHandler<EventArgs>(Presenter.ViewModel["premises_tenancies_info"].Model, "RefreshEvent",
                    (s, e) => DataGridView.Refresh());
            }

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["ownership_buildings_assoc"].DataSource, "RowChanged", BuildingsOwnershipChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["ownership_buildings_assoc"].DataSource, "RowDeleted", BuildingsOwnershipChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["ownership_premises_assoc"].DataSource, "RowChanged", PremisesOwnershipChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["ownership_premises_assoc"].DataSource, "RowDeleted", PremisesOwnershipChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["ownership_rights"].DataSource, "RowChanged", OwnershipChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["ownership_rights"].DataSource, "RowDeleted", OwnershipChanged);

            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());
        }

        private void OwnershipChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Change) return;
            _demolishedBuildings = BuildingService.DemolishedBuildingIDs().ToList();
            _demolishedPremises = PremisesService.DemolishedPremisesIDs().ToList();
            DataGridView.Refresh();
        }

        private void BuildingsOwnershipChanged(object sender, DataRowChangeEventArgs dataRowChangeEventArgs)
        {
            _demolishedBuildings = BuildingService.DemolishedBuildingIDs().ToList();
            DataGridView.Refresh();
        }

        private void PremisesOwnershipChanged(object sender, DataRowChangeEventArgs dataRowChangeEventArgs)
        {
            _demolishedPremises = PremisesService.DemolishedPremisesIDs().ToList();
            DataGridView.Refresh();
        }

        public override bool CanDeleteRecord()
        {
            return Presenter.ViewModel["general"].CurrentRow != null &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            var hasResettles = ((PremisesListPresenter)Presenter).HasResettles();
            var hasTenancies = ((PremisesListPresenter)Presenter).HasTenancies();
            if (hasResettles || hasTenancies)
            {
                if (MessageBox.Show(@"К помещению или одной из его комнат привязаны процессы" +
                    (hasTenancies ? " найма" : "") +
                    (hasTenancies && hasResettles ? " и" : "") +
                    (hasResettles ? " переселения" : "") +
                    @". Вы действительно хотите удалить это помещение?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return;
            }
            else
            if (MessageBox.Show(@"Вы действительно хотите удалить это помещение?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            if (((PremisesListPresenter) Presenter).DeleteRecord())
            {
                MenuCallback.ForceCloseDetachedViewports();   
            }
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool CanOpenDetails()
        {
            return Presenter.ViewModel["general"].CurrentRow != null;
        }

        public override void OpenDetails()
        {
            var viewport = new PremisesViewport(null, MenuCallback)
            {
                StaticFilter = StaticFilter,
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (Presenter.ViewModel["general"].BindingSource.Count > 0)
                viewport.LocateEntityBy(Presenter.ViewModel["general"].PrimaryKeyFirst,
                    Presenter.ViewModel["general"].CurrentRow[Presenter.ViewModel["general"].PrimaryKeyFirst] as int? ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return !Presenter.ViewModel["general"].Model.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void InsertRecord()
        {
            var viewport = new PremisesViewport(null, MenuCallback)
            {
                StaticFilter = StaticFilter,
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            viewport.InsertRecord();
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanCopyRecord()
        {
            return Presenter.ViewModel["general"].CurrentRow != null && !Presenter.ViewModel["general"].Model.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void CopyRecord()
        {
            var viewport = new PremisesViewport(null, MenuCallback)
            {
                StaticFilter = StaticFilter,
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            var viewModel = Presenter.ViewModel["general"];
            if (viewModel.CurrentRow != null)
            {
                viewport.LocateEntityBy(viewModel.PrimaryKeyFirst, viewModel.CurrentRow[viewModel.PrimaryKeyFirst] as int? ?? -1);
            }
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.SubPremisesViewport,
                ViewportType.OwnershipListViewport,
                ViewportType.RestrictionListViewport,
                ViewportType.FundsHistoryViewport,
                ViewportType.TenancyListViewport,
                ViewportType.PaymentsAccountsViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && Presenter.ViewModel["general"].CurrentRow != null;
        }

        public override void ShowAssocViewport<T>()
        {
            var viewModel = Presenter.ViewModel["general"];
            if (viewModel.CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано помещение", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, viewModel.PrimaryKeyFirst + " = " +
                Convert.ToInt32(viewModel.CurrentRow[viewModel.PrimaryKeyFirst], CultureInfo.InvariantCulture),
                viewModel.CurrentRow.Row, ParentTypeEnum.Premises);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            var reports = new List<ReporterType>
            {
                ReporterType.ExportReporter,
                ReporterType.TenancyHistoryReporter,
                ReporterType.RegistryExcerptReporterPremise,
                ReporterType.RegistryExcerptReporterAllMunSubPremises
            };
            return reports.Contains(reporterType);
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.ExportReporter:
                    arguments = ExportReportArguments();
                    break;
                case ReporterType.RegistryExcerptReporterPremise:
                    arguments = RegistryExcerptPremiseReportArguments();
                    break;
                case ReporterType.RegistryExcerptReporterAllMunSubPremises:
                    arguments = RegistryExcerptReporterAllMunSubPremisesArguments();
                    break;
                case ReporterType.TenancyHistoryReporter:
                    arguments = TenancyHistoryReportArguments();
                    break;
            }
            MenuCallback.RunReport(reporterType, arguments);
        }

        private Dictionary<string, string> TenancyHistoryReportArguments()
        {
            var filter = Presenter.ViewModel["general"].BindingSource.Filter ?? "";
            var arguments = new Dictionary<string, string>
            {
                {"filter", filter.Trim() == "" ? "(1=1)" : filter }
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptPremiseReportArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", Presenter.ViewModel["general"].CurrentRow[Presenter.ViewModel["general"].PrimaryKeyFirst].ToString()},
                {"excerpt_type", "1"}
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptReporterAllMunSubPremisesArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", Presenter.ViewModel["general"].CurrentRow[Presenter.ViewModel["general"].PrimaryKeyFirst].ToString()},
                {"excerpt_type", "3"}
            };
            return arguments;
        }

        private Dictionary<string, string> ExportReportArguments()
        {
            var columnHeaders = DataGridView.Columns.Cast<DataGridViewColumn>().Where(c => c.Name != "rooms_area").
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnHeader\":\"" + column.HeaderText + "\"}");
            var columnPatterns = DataGridView.Columns.Cast<DataGridViewColumn>().Where(c => c.Name != "rooms_area").
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnPattern\":\"$column" + column.DisplayIndex + "$\"}");
            var filter = Presenter.ViewModel["general"].BindingSource.Filter ?? "";
            var arguments = new Dictionary<string, string>
            {
                {"type", "2"},
                {"filter", filter.Trim() == "" ? "(1=1)" : filter },
                {
                    "columnHeaders", "["+columnHeaders+",{\"columnHeader\":\"Номер и дата включения в фонд\"},{\"columnHeader\":\"Дополнительные сведения\"}"+
                    ",{\"columnHeader\":\"Основание на включение в АФ здания\"},{\"columnHeader\":\"Основание на включение в АФ помещения\"}]"
                },
                {
                    "columnPatterns", "["+columnPatterns+",{\"columnPattern\":\"$fund_info$\"},{\"columnPattern\":\"$description$\"}"+
                    ",{\"columnPattern\":\"$b_emergency$\"},{\"columnPattern\":\"$p_emergency$\"}]"
                }
            };
            return arguments;
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (CanOpenDetails())
                OpenDetails();
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView_ColumnHeaderMouseClick(sender, e);
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView_SelectionChanged();
        }

        private int _idPremises = int.MinValue;
        private IEnumerable<DataRow> _tenancyInfoRows;
        private IEnumerable<int> _demolishedBuildings = BuildingService.DemolishedBuildingIDs().ToList();
        private readonly IEnumerable<int> _emergencyBuildings = BuildingService.EmergencyBuildingIDs().ToList();
        private IEnumerable<int> _demolishedPremises = PremisesService.DemolishedPremisesIDs().ToList();
        private readonly IEnumerable<int> _emergencyPremises = PremisesService.EmergencyPremisesIDs().ToList();
        private readonly IEnumerable<int> _emergencyExcludedPremises = PremisesService.EmergencyExcludedPremisesIDs().ToList();

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (Presenter.ViewModel["general"].BindingSource.Count <= e.RowIndex) return;
            var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[e.RowIndex];
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_premises":
                    e.Value = row["id_premises"];
                    if (_demolishedBuildings.Contains((int) row["id_building"]) ||
                        _demolishedPremises.Contains((int) row["id_premises"]))
                    {
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(255, 254, 220, 220);
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.FromArgb(255, 215, 72, 72);
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Присутствует реквизит \"Снесено\"";
                    } else
                    if (_emergencyBuildings.Contains((int) row["id_building"]) || (
                        _emergencyPremises.Contains((int) row["id_premises"]) &&
                        !_emergencyExcludedPremises.Contains((int) row["id_premises"])
                        ))
                    {
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(255, 187, 254, 232);
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.FromArgb(255, 72, 215, 143);
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Присутствует реквизит \"Аварийное\"";
                    }
                    else
                    {
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = SystemColors.Highlight;
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "";
                    }
                    break;
                case "id_street":
                    var buildingRow = Presenter.ViewModel["buildings"].DataSource.Rows.Find(
                        row[Presenter.ViewModel["buildings"].PrimaryKeyFirst]);
                    if (buildingRow == null)
                        return;
                    var kladrRow = Presenter.ViewModel["kladr"].DataSource.Rows.Find(
                        buildingRow[Presenter.ViewModel["kladr"].PrimaryKeyFirst]);
                    string streetName = null;
                    if (kladrRow != null)
                        streetName = kladrRow["street_name"].ToString();
                    e.Value = streetName;
                    break;
                case "house":
                    buildingRow = Presenter.ViewModel["buildings"].DataSource.Rows.Find(row["id_building"]);
                    if (buildingRow == null)
                        return;
                    e.Value = buildingRow["house"];
                    break;
                case "premises_num":
                case "id_premises_type":
                case "cadastral_num":
                case "total_area":
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "rooms_area":
                    var subPremises =
                        (from subPremisesRow in Presenter.ViewModel["sub_premises"].Model.FilterDeletedRows()
                            where subPremisesRow.Field<int?>("id_premises") == (int?) row["id_premises"] &&
                                    new List<int?> {4, 5, 9, 11, 12}.Contains((int?) subPremisesRow["id_state"])
                            select subPremisesRow).ToList();
                    if (subPremises.Any())
                    e.Value = subPremises.Select(sp => sp["total_area"].ToString()).Aggregate((v, acc) => v + " / " + acc);         
                    break;
                case "id_state":
                    var stateRow = Presenter.ViewModel["object_states"].DataSource.Rows.Find(
                        row[Presenter.ViewModel["object_states"].PrimaryKeyFirst]);
                    if (stateRow != null)
                        e.Value = stateRow["state_neutral"];
                    break;
                case "current_fund":
                    if (DataModelHelper.MunicipalAndUnknownObjectStates().ToList().Contains((int)row["id_state"]))
                    {
                        var fundRow = Presenter.ViewModel["premises_current_funds"].DataSource.Rows.Find(
                            row[Presenter.ViewModel["general"].PrimaryKeyFirst]);
                        if (fundRow != null)
                            e.Value = Presenter.ViewModel["fund_types"].DataSource.Rows.Find(
                                fundRow[Presenter.ViewModel["fund_types"].PrimaryKeyFirst])["fund_type"];
                    }
                    break;
                case "registration_num":
                case "registration_date":
                case "residence_warrant_num":
                case "residence_warrant_date":
                case "tenant":
                case "end_date":
                case "payment":
                    if ((int)row["id_premises"] != _idPremises || 
                        _tenancyInfoRows.Any(entry => entry.RowState == DataRowState.Deleted || entry.RowState == DataRowState.Detached))
                    {   
                        _tenancyInfoRows =
                            (from tenancyInfoRow in Presenter.ViewModel["premises_tenancies_info"].Model.FilterDeletedRows()
                            where tenancyInfoRow.Field<int>("id_premises") == (int?) row["id_premises"]
                            orderby tenancyInfoRow.Field<DateTime?>("registration_date") ?? 
                                DateTime.Now descending 
                            select tenancyInfoRow).ToList();
                        _idPremises = (int)row["id_premises"];
                    }
                    if (_tenancyInfoRows == null || !_tenancyInfoRows.Any())
                    {                        
                        break;
                    }
                        
                    var tenancyRow = _tenancyInfoRows.First();


                    switch (DataGridView.Columns[e.ColumnIndex].Name)
                    {
                        case "registration_date":
                        case "residence_warrant_date":
                        case "end_date":
                            var date = tenancyRow.Field<DateTime?>(DataGridView.Columns[e.ColumnIndex].Name);
                            e.Value = date != null ? date.Value.ToString("dd.MM.yyyy") : null;
                            if (DataGridView.Columns[e.ColumnIndex].Name == "end_date" && tenancyRow.Field<bool?>("until_dismissal") == true)
                                e.Value = "на период ТО";
                            break;
                        case "registration_num":
                        case "residence_warrant_num":
                        case "tenant":
                                e.Value = tenancyRow.Field<string>(DataGridView.Columns[e.ColumnIndex].Name);
                            break;
                        case "payment":
                            var idProcess = (int?)_tenancyInfoRows.First()["id_process"];
                            var paymentRows = from paymentRow in Presenter.ViewModel["tenancy_payments_info"].Model.FilterDeletedRows()
                                              where paymentRow.Field<int?>("id_process") == idProcess
                                              select paymentRow.Field<decimal>("payment");
                            var payment = paymentRows.Sum(r => r);
                            e.Value = payment;
                            break;
                    }
                    if (tenancyRow.Field<int?>("object_type") == 2)
                    {
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(255, 187, 254, 232);
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.FromArgb(255, 72, 215, 143);
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Текущий найм одной из комнат помещения";
                    }
                    else
                    {
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = SystemColors.Highlight;
                        DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "";
                    }
                    break;
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if ((AccessControl.HasPrivelege(Priveleges.TenancyRead) && DataGridView.Size.Width > 1795) ||
                (!AccessControl.HasPrivelege(Priveleges.TenancyRead) && DataGridView.Size.Width > 955))
            {
                premises_num.Frozen = false;
                house.Frozen = false;
                id_street.Frozen = false;
                id_premises.Frozen = false;
                if (id_street.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                premises_num.Frozen = true;
                house.Frozen = true;
                id_street.Frozen = true;
                id_premises.Frozen = true;
                if (id_street.AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        internal IEnumerable<int> GetCurrentIds()
        {
            var ids = new List<int>();
            if (Presenter.ViewModel["general"].BindingSource.Position < 0) return ids;
            for (var i = 0; i < DataGridView.SelectedRows.Count; i++)
            {
                var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[DataGridView.SelectedRows[i].Index];
                if (row["id_premises"] != DBNull.Value)
                    ids.Add((int)row["id_premises"]);
            }
            return ids;
        }

        internal IEnumerable<int> GetFilteredIds()
        {
            var ids = new List<int>();
            if (Presenter.ViewModel["general"].BindingSource.Position < 0) return ids;
            for (var i = 0; i < Presenter.ViewModel["general"].BindingSource.Count; i++)
            {
                var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[i];
                if (row["id_premises"] != DBNull.Value)
                    ids.Add((int)row["id_premises"]);
            }
            return ids;
        }
    }
}
