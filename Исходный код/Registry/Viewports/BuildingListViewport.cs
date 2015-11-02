using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Registry.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class BuildingListViewport : DataGridViewport
    {

        #region Models
        private DataModel _kladr;
        private DataModel _objectStates;
        #endregion

        #region Views
        private BindingSource _vKladr;
        #endregion

        //Forms
        private SearchForm _sbSimpleSearchForm;
        private SearchForm _sbExtendedSearchForm;

        private BuildingListViewport()
            : this(null, null)
        {
        }

        public BuildingListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance(DataModelType.BuildingsDataModel);
            _kladr = DataModel.GetInstance(DataModelType.KladrStreetsDataModel);
            _objectStates = DataModel.GetInstance(DataModelType.ObjectStatesDataModel);
            
            // Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _kladr.Select();
            _objectStates.Select();

            var ds = DataModel.DataSet;

            GeneralBindingSource = new BindingSource {DataMember = "buildings"};
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;

            _vKladr = new BindingSource
            {
                DataMember = "kladr",
                DataSource = ds
            };

            id_street.DataSource = _vKladr;
            id_street.ValueMember = "id_street";
            id_street.DisplayMember = "street_name";

            GeneralDataModel.Select().RowChanged += BuildingListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += BuildingListViewport_RowDeleted;
            dataGridView.RowCount = GeneralBindingSource.Count;
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            return !string.IsNullOrEmpty(DynamicFilter);
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (_sbSimpleSearchForm == null)
                        _sbSimpleSearchForm = new SimpleSearchBuildingForm();
                    if (_sbSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _sbSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (_sbExtendedSearchForm == null)
                        _sbExtendedSearchForm = new ExtendedSearchBuildingForm();
                    if (_sbExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _sbExtendedSearchForm.GetFilter();
                    break;
            }
            var filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            dataGridView.RowCount = 0;
            GeneralBindingSource.Filter = filter;
            dataGridView.RowCount = GeneralBindingSource.Count;
        }

        public override void ClearSearch()
        {
            GeneralBindingSource.Filter = StaticFilter;
            dataGridView.RowCount = GeneralBindingSource.Count;
            DynamicFilter = "";
        }

        public override bool CanOpenDetails()
        {
            return GeneralBindingSource.Position != -1;
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
            if (GeneralBindingSource.Count > 0)
                viewport.LocateEntityBy("id_building", (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить это здание?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (DataModelHelper.HasMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_building"], EntityType.Building)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на удаление муниципальных жилых зданий и зданий, в которых присутствуют муниципальные помещения",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (DataModelHelper.HasNotMunicipal((int)((DataRowView)GeneralBindingSource.Current)["id_building"], EntityType.Building)
                    && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на удаление немуниципальных жилых зданий и зданий, в которых присутствуют немуниципальные помещения",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_building"]) == -1)
                    return;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) && 
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
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
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
            if (GeneralBindingSource.Count > 0)
                viewport.LocateEntityBy("id_building", (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"] as int?) ?? -1);
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool HasAssocViewport(ViewportType viewportType)
        {
            var reports = new List<ViewportType>
            {
                ViewportType.PremisesListViewport,
                ViewportType.OwnershipListViewport,
                ViewportType.RestrictionListViewport,
                ViewportType.FundsHistoryViewport,
                ViewportType.TenancyListViewport
            };
            return reports.Contains(viewportType) && (GeneralBindingSource.Position > -1);
        }
        
        public override void ShowAssocViewport(ViewportType viewportType)
        {
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание для отображения истории найма", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "id_building = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_building"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Building);
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
            var columnHeaders = dataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnHeader\":\"" + column.HeaderText + "\"}");
            var columnPatterns = dataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnPattern\":\"$column" + column.DisplayIndex + "$\"}");
            var arguments = new Dictionary<string, string>
            {
                {"type", "1"},
                {"filter", GeneralBindingSource.Filter.Trim() == "" ? "(1=1)" : GeneralBindingSource.Filter},
                {"columnHeaders", "["+columnHeaders+"]"},
                {"columnPatterns", "["+columnPatterns+"]"}
            };
            return arguments;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            GeneralDataModel.Select().RowChanged -= BuildingListViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= BuildingListViewport_RowDeleted;
            base.OnClosing(e);
        }

        void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (CanOpenDetails())
                OpenDetails();
        }

        void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        void BuildingListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.RowCount = GeneralBindingSource.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void BuildingListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = GeneralBindingSource.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            var row = ((DataRowView) GeneralBindingSource[e.RowIndex]);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_building":
                case "id_street":
                case "house":
                case "floors":
                case "living_area":
                case "cadastral_num":
                case "startup_year":
                    e.Value = row[dataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "id_state":
                    var stateRow = _objectStates.Select().Rows.Find(row["id_state"]);
                    if (stateRow != null)
                        e.Value = stateRow["state_female"];
                    break;
            }
        } 

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            var idStreetColumn = dataGridView.Columns["id_street"];
            if (idStreetColumn == null) return;
            if (dataGridView.Size.Width > 1100)
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


        internal int GetCurrentId()
        {
            if (GeneralBindingSource.Position < 0) return -1;
            if (((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_building"] != DBNull.Value)
                return (int) ((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_building"];
            return -1;
        }
    }
}
