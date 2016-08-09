﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.Presenters;
using Security;
using WeifenLuo.WinFormsUI.Docking;
using MessageBox = System.Windows.Forms.MessageBox;
using SystemColors = System.Drawing.SystemColors;

namespace Registry.Viewport
{
    internal sealed partial class BuildingListViewport : DataGridViewport
    {
        public BuildingListViewport(Viewport viewport = null, IMenuCallback menuCallback = null)
            : base(viewport, menuCallback, new BuildingListPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
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

            id_street.DataSource = Presenter.ViewModel["kladr"].DataSource;
            id_street.ValueMember = Presenter.ViewModel["kladr"].PrimaryKeyFirst;
            id_street.DisplayMember = "street_name";

            id_structure_type.DataSource = Presenter.ViewModel["structure_types"].DataSource;
            id_structure_type.ValueMember = Presenter.ViewModel["structure_types"].PrimaryKeyFirst;
            id_structure_type.DisplayMember = "structure_type";         

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", GeneralDataSource_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", GeneralDataSource_RowDeleted);
            AddEventHandler<EventArgs>(Presenter.ViewModel["municipal_premises"].Model, "RefreshEvent", _municipalPremises_RefreshEvent);
            
            var ownershipRightsAssoc = EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance().Select();
            AddEventHandler<DataRowChangeEventArgs>(ownershipRightsAssoc, "RowChanged", BuildingsOwnershipChanged);
            AddEventHandler<DataRowChangeEventArgs>(ownershipRightsAssoc, "RowDeleted", BuildingsOwnershipChanged);

            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());
        }

        private void _municipalPremises_RefreshEvent(object sender, EventArgs e)
        {
            if (DataGridView.Columns["mun_area"] != null)
            {
                DataGridView.InvalidateColumn(DataGridView.Columns["mun_area"].Index);
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
            var viewport = new BuildingViewport(null, MenuCallback)
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

        public override bool CanDeleteRecord()
        {
            return Presenter.ViewModel["general"].CurrentRow != null &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить это здание?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            if (((BuildingListPresenter) Presenter).DeleteRecord())
            {
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanInsertRecord()
        {
            return !GeneralDataModel.EditingNewRecord && 
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            var viewport = new BuildingViewport(null, MenuCallback)
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
            var viewport = new BuildingViewport(null, MenuCallback)
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
                ViewportType.PremisesListViewport,
                ViewportType.OwnershipListViewport,
                ViewportType.RestrictionListViewport,
                ViewportType.FundsHistoryViewport,
                ViewportType.TenancyListViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && Presenter.ViewModel["general"].CurrentRow != null;
        }
        
        public override void ShowAssocViewport<T>()
        {
            var viewModel = Presenter.ViewModel["general"];
            if (viewModel.CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано здание", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, viewModel.PrimaryKeyFirst + " = " +
                Convert.ToInt32(viewModel.CurrentRow[viewModel.PrimaryKeyFirst], CultureInfo.InvariantCulture), 
                viewModel.CurrentRow.Row, ParentTypeEnum.Building);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            var reports = new List<ReporterType>
            {
                ReporterType.ExportReporter
            };
            return reports.Contains(reporterType);
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            var reporter = ReporterFactory.CreateReporter(reporterType);
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.ExportReporter:
                    arguments = ExportReportArguments();
                    break;
            }
            reporter.Run(arguments);
        }

        private Dictionary<string,string> ExportReportArguments()
        {
            var columnHeaders = DataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnHeader\":\"" + column.HeaderText + "\"}");
            var columnPatterns = DataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnPattern\":\"$column" + column.DisplayIndex + "$\"}");
            var arguments = new Dictionary<string, string>
            {
                {"type", "1"},
                {"filter", 
                    Presenter.ViewModel["general"].BindingSource.Filter.Trim() == "" ? "(1=1)" : 
                    Presenter.ViewModel["general"].BindingSource.Filter},
                {"columnHeaders", "["+columnHeaders+",{\"columnHeader\":\"Дополнительные сведения\"}]"},
                {"columnPatterns", "["+columnPatterns+",{\"columnPattern\":\"$description$\"}]"}
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

        private IEnumerable<int> _demolishedBuildings = BuildingService.DemolishedBuildingIDs().ToList();

        private void BuildingsOwnershipChanged(object sender, DataRowChangeEventArgs dataRowChangeEventArgs)
        {
            _demolishedBuildings = BuildingService.DemolishedBuildingIDs().ToList();
            DataGridView.Refresh();
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (Presenter.ViewModel["general"].BindingSource.Count <= e.RowIndex) return;
            var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[e.RowIndex];
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_building":
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name];
                    var style = DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style;
                    if (_demolishedBuildings.Contains((int)row[Presenter.ViewModel["general"].PrimaryKeyFirst]))
                    {
                        style.BackColor = Color.Red;
                        style.SelectionBackColor = Color.DarkRed;
                    }
                    else
                    {
                        style.BackColor = Color.White;
                        style.SelectionBackColor = SystemColors.Highlight;
                    }
                    break;
                case "id_street":
                case "house":
                case "floors":
                case "living_area":
                case "cadastral_num":
                case "startup_year":
                case "id_structure_type":
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "id_state":
                    var stateRow = Presenter.ViewModel["object_states"].DataSource.Rows.Find(row["id_state"]);
                    if (stateRow != null)
                        e.Value = stateRow["state_female"];
                    break;
                case "mun_area":
                    e.Value = Convert.ToDecimal(Presenter.ViewModel["municipal_premises"].DataSource.AsEnumerable().
                        Where(s => s.Field<int>("id_building") == (int?)row["id_building"]).Sum(m => m.Field<double>("total_area")));
                    break;
            }
        } 

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            var idStreetColumn = DataGridView.Columns["id_street"];
            if (idStreetColumn == null) return;
            if (DataGridView.Size.Width > 1010)
            {
                if (idStreetColumn.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    idStreetColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (idStreetColumn.AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    idStreetColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }
    }
}
