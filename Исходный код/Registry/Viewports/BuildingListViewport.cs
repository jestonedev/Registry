using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;
using MessageBox = System.Windows.Forms.MessageBox;
using SystemColors = System.Drawing.SystemColors;

namespace Registry.Viewport
{
    internal sealed partial class BuildingListViewport : DataGridViewport
    {

        #region Models
        private DataModel _kladr;
        private DataModel _objectStates;
        private CalcDataModel _municipalPremises;
        #endregion

        #region Views
        private BindingSource _vKladr;
        private BindingSource _vStructureType;
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
            GeneralDataModel = DataModel.GetInstance<BuildingsDataModel>();
            _kladr = DataModel.GetInstance<KladrStreetsDataModel>();
            _objectStates = DataModel.GetInstance<ObjectStatesDataModel>();
            _municipalPremises = CalcDataModel.GetInstance<CalcDataModelMunicipalPremises>();
            
            // Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _kladr.Select();
            _objectStates.Select();
            _municipalPremises.Select();

            var ds = DataModel.DataSet;
            
            GeneralBindingSource = new BindingSource {DataMember = "buildings"};

            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

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

            _vStructureType = new BindingSource
            {
                DataMember = "structure_types",
                DataSource = ds
            };
            
            id_structure_type.DataSource = _vStructureType;
            id_structure_type.ValueMember = "id_structure_type";
            id_structure_type.DisplayMember = "structure_type";


            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", BuildingListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", BuildingListViewport_RowDeleted);
            AddEventHandler<EventArgs>(_municipalPremises, "RefreshEvent", _municipalPremises_RefreshEvent);
            AddEventHandler<DataRowChangeEventArgs>(DataModel.GetInstance<OwnershipBuildingsAssocDataModel>().Select(),
                "RowChanged", BuildingsOwnershipChanged);
            AddEventHandler<DataRowChangeEventArgs>(DataModel.GetInstance<OwnershipBuildingsAssocDataModel>().Select(),
                "RowDeleted", BuildingsOwnershipChanged);

            dataGridView.RowCount = GeneralBindingSource.Count;
            ViewportHelper.SetDoubleBuffered(dataGridView);
        }

        void _municipalPremises_RefreshEvent(object sender, EventArgs e)
        {
            if (dataGridView.Columns["mun_area"] != null)
            {
                dataGridView.InvalidateColumn(dataGridView.Columns["mun_area"].Index);
            }
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
            return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
        }
        
        public override void ShowAssocViewport<T>()
        {
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано здание для отображения истории найма", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback,
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

        private void BuildingListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.RowCount = GeneralBindingSource.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void BuildingListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = GeneralBindingSource.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        private IEnumerable<int> _demolishedBuildings = DataModelHelper.DemolishedBuildingIDs().ToList();

        private void BuildingsOwnershipChanged(object sender, DataRowChangeEventArgs dataRowChangeEventArgs)
        {
            _demolishedBuildings = DataModelHelper.DemolishedBuildingIDs().ToList();
            dataGridView.Refresh();
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            var row = ((DataRowView) GeneralBindingSource[e.RowIndex]);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_building":
                    e.Value = row[dataGridView.Columns[e.ColumnIndex].Name];
                    if (_demolishedBuildings.Contains((int)row["id_building"]))
                    {
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.DarkRed;
                    }
                    else
                    {
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = SystemColors.Highlight;
                    }
                    break;
                case "id_street":
                case "house":
                case "floors":
                case "living_area":
                case "cadastral_num":
                case "startup_year":
                case "id_premises_type":
                case "id_structure_type":
                    e.Value = row[dataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "id_state":
                    var stateRow = _objectStates.Select().Rows.Find(row["id_state"]);
                    if (stateRow != null)
                        e.Value = stateRow["state_female"];
                    break;
                case "mun_area":
                    e.Value = Convert.ToDecimal(_municipalPremises.Select().AsEnumerable().
                        Where(s => s.Field<int>("id_building") == (int?)row["id_building"]).Sum(m => m.Field<double>("total_area")));
                    break;
            }
        } 

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            var idStreetColumn = dataGridView.Columns["id_street"];
            if (idStreetColumn == null) return;
            if (dataGridView.Size.Width > 1010)
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
