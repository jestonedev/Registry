using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.Properties;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;
using System.Linq;
using Registry.Entities.Infrastructure;
using Registry.Viewport.Presenters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport
{
    internal sealed partial class ResettlePremisesViewport: DataGridViewport
    {
        #region Models
        private DataModel _subPremises;
        #endregion Models

        #region Views
        private BindingSource _vSubPremises;
        #endregion Views

        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        //Идентификатор развернутого помещения
        private int _idExpanded = -1;

        public ResettleEstateObjectWay Way { get; set; }

        private ResettlePremisesViewport()
            : this(null, null)
        {
        }

        public ResettlePremisesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new ResettlePremisesPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            ViewportHelper.SetDoubleBuffered(DataGridView);
        }

        private bool SnapshotHasChanges()
        {
            if (((ResettlePremisesPresenter) Presenter).SnapshotHasChanges())
            {
                return true;
            }

            //Проверяем комнаты
            var listRoomsFromView = ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromView();
            var listRoomsFromViewport = ((ResettleSubPremisesDetails)dataGridView.DetailsControl).ResettleSubPremisesFromViewport();
            if (listRoomsFromView.Count != listRoomsFromViewport.Count)
                return true;
            foreach (var roomRowFromView in listRoomsFromView)
            {
                var founded = false;
                foreach (var roomRowFromViewport in listRoomsFromViewport)
                    if (roomRowFromView == roomRowFromViewport)
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        public void LocatePremisesBy(int id)
        {
            Presenter.ViewModel["general"].BindingSource.Position = Presenter.ViewModel["general"].BindingSource.Find("id_premises", id);
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;

            ((ResettlePremisesViewModel)Presenter.ViewModel).AddAssocViewModelItem(Way);

            Presenter.ViewModel["assoc"].BindingSource.Filter = StaticFilter;

            Presenter.ParentRow = ParentRow;
            Presenter.ParentType = ParentType;

            ((SnapshotedViewModel)Presenter.ViewModel).InitializeSnapshot();

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.ResettleProcess))
            {
                if (Way == ResettleEstateObjectWay.From)
                    Text = @"Помещения (из) переселения №" + ParentRow["id_process"];
                else
                    Text = @"Помещения (в) переселения №" + ParentRow["id_process"];
            }
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            DataBind();

            _subPremises = EntityDataModel<SubPremise>.GetInstance();
            _subPremises.Select();

            _vSubPremises = new BindingSource
            {
                DataSource = Presenter.ViewModel["general"].BindingSource,
                DataMember = "premises_sub_premises"
            };

            // Настраивем компонент отображения комнат
            var details = new ResettleSubPremisesDetails
            {
                v_sub_premises = _vSubPremises,
                sub_premises = _subPremises.Select(),
                StaticFilter = StaticFilter,
                ParentRow = ParentRow,
                ParentType = ParentType,
                menuCallback = MenuCallback,
                Way = Way
            };
            details.InitializeControl();
            ((DataGridViewWithDetails)DataGridView).DetailsControl = details;

            //Строим фильтр арендуемых зданий
            if (string.IsNullOrEmpty(DynamicFilter))
            {
                DynamicFilter = ((ResettlePremisesPresenter)Presenter).GetDefaultDynamicFilter();
                var defaultFilterSubPremises = details.GetDefaultDynamicFilter();
                if (!string.IsNullOrEmpty(DynamicFilter) && !string.IsNullOrEmpty(defaultFilterSubPremises))
                {
                    DynamicFilter += " OR ";
                }
                DynamicFilter += defaultFilterSubPremises;
            }
            Presenter.ViewModel["general"].BindingSource.Filter = DynamicFilter;

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, 
                "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", PremisesListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", PremisesListViewport_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowChanged", ResettlePremisesViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowDeleting", ResettlePremisesViewport_RowDeleting);

            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());
        }

        private void DataBind()
        {
            id_premises_type.DataSource = Presenter.ViewModel["premises_types"].BindingSource;
            id_premises_type.ValueMember = "id_premises_type";
            id_premises_type.DisplayMember = "premises_type";
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить это помещение?", @"Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                ((ResettlePremisesPresenter)Presenter).DeleteRecord();
                MenuCallback.ForceCloseDetachedViewports();
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
            Presenter.ViewModel["general"].BindingSource.Filter = DynamicFilter;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
        }

        public override void ClearSearch()
        {
            DataGridView.RowCount = 0;
            Presenter.ViewModel["general"].BindingSource.Filter = "";
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DynamicFilter = "";
        }

        public override bool CanOpenDetails()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) && AccessControl.HasPrivelege(Priveleges.RegistryRead);
        }

        public override void OpenDetails()
        {
            var viewport = new PremisesViewport(null, MenuCallback)
            {
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

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            ((SnapshotedViewModel)Presenter.ViewModel).LoadSnapshot();
            DataGridView.Refresh();
            ((ResettleSubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).CancelRecord();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.ResettleWrite);
        }

        public override void SaveRecord()
        {
            _syncViews = false;
            DataGridView.EndEdit();
            Presenter.ViewModel["assoc"].Model.EditingNewRecord = true;
            if (((ResettlePremisesPresenter)Presenter).ValidateResettlePremises())
            {
                ((ResettlePremisesPresenter)Presenter).SaveRecords();
                ((ResettleSubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).SaveRecord();
            }
            Presenter.ViewModel["assoc"].Model.EditingNewRecord = false;
            _syncViews = true;
            DataGridView.Invalidate();

            MenuCallback.EditingStateUpdate();
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
            return (Presenter.ViewModel["general"].CurrentRow != null) && !Presenter.ViewModel["general"].Model.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void CopyRecord()
        {
            var viewport = new PremisesViewport(null, MenuCallback)
            {
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case DialogResult.Yes:
                        SaveRecord();
                        break;
                    case DialogResult.No:
                        CancelRecord();
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }
            }
            base.OnClosing(e);
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.SubPremisesViewport,
                ViewportType.OwnershipListViewport,
                ViewportType.RestrictionListViewport,
                ViewportType.FundsHistoryViewport,
                ViewportType.TenancyListViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (Presenter.ViewModel["general"].CurrentRow != null);
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
                viewModel.CurrentRow.Row, ParentTypeEnum.Building);
        }

        private void ResettlePremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                ((ResettlePremisesPresenter)Presenter).DeleteRowByIdFromSnapshot((int)e.Row["id_premises"]);
            }
            DataGridView.Refresh();
        }

        private void ResettlePremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            ((ResettlePremisesPresenter)Presenter).InsertOrUpdateRowIntoSnapshot(e.Row);
            DataGridView.Invalidate();
        }

        private void PremisesListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void PremisesListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DataGridView.Refresh();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void dataGridView_BeforeCollapseDetails(object sender, DataGridViewDetailsEventArgs e)
        {
            DataGridView.Rows[e.RowIndex].Cells["is_checked"].Style.Alignment = DataGridViewContentAlignment.TopCenter;
        }

        private void dataGridView_BeforeExpandDetails(object sender, DataGridViewDetailsEventArgs e)
        {
            DataGridView.Rows[e.RowIndex].Cells["is_checked"].Style.Alignment = DataGridViewContentAlignment.TopCenter;
            ((ResettleSubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).CalcControlHeight();
            var width = 0;
            for (var i = 0; i < DataGridView.Columns.Count; i++)
                width += DataGridView.Columns[i].Width;
            width += DataGridView.RowHeadersWidth;
            ((ResettleSubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).SetControlWidth(width);
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            var width = 0;
            for (var i = 0; i < DataGridView.Columns.Count; i++)
                width += DataGridView.Columns[i].Width;
            width += DataGridView.RowHeadersWidth;
            ((ResettleSubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).SetControlWidth(width);
            if (DataGridView.Size.Width > 1240)
            {
                if (id_street.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (id_street.AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DataGridView.Columns[e.ColumnIndex].Name != "image") return;
            var row = (DataRowView) Presenter.ViewModel["general"].BindingSource[e.RowIndex];
            if (_idExpanded != Convert.ToInt32(row["id_premises"], CultureInfo.InvariantCulture))
            {
                DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Resource.minus;
                _idExpanded = Convert.ToInt32(row["id_premises"], CultureInfo.InvariantCulture);
                ((DataGridViewWithDetails)DataGridView).ExpandDetails(e.RowIndex);
            }
            else
            {
                DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Resource.plus;
                _idExpanded = -1;
                ((DataGridViewWithDetails)DataGridView).CollapseDetails();
            }
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView_ColumnHeaderMouseClick(sender, e);
            ((DataGridViewWithDetails)DataGridView).CollapseDetails();
            _idExpanded = -1;
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView_SelectionChanged();
            ((DataGridViewWithDetails)DataGridView).CollapseDetails();
            _idExpanded = -1;
        }

        private void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            if (bindingSource.Count <= e.RowIndex || bindingSource.Count == 0) return;
            var idPremises = Convert.ToInt32(((DataRowView)bindingSource[e.RowIndex])["id_premises"], CultureInfo.InvariantCulture);
            var snapshotedBindinsSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;
            var snapshotedDataSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotDataSource;
            var rowIndex = snapshotedBindinsSource.Find("id_premises", idPremises);
            _syncViews = false;
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (rowIndex == -1)
                        snapshotedDataSource.Rows.Add(null, idPremises, e.Value);
                    else
                        ((DataRowView)snapshotedBindinsSource[rowIndex])["is_checked"] = e.Value;
                    break;
            }
            _syncViews = true;
            MenuCallback.EditingStateUpdate();
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (Presenter.ViewModel["general"].BindingSource.Count <= e.RowIndex || Presenter.ViewModel["general"].BindingSource.Count == 0) return;
            var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[e.RowIndex];
            var idPremises = Convert.ToInt32(row["id_premises"], CultureInfo.InvariantCulture);
            var snapshotedBindinsSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;
            var rowIndex = snapshotedBindinsSource.Find("id_premises", idPremises);
            var buildingRow = Presenter.ViewModel["buildings"].DataSource.Rows.Find(row["id_building"]);
            if (buildingRow == null)
                return;
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "image":
                    e.Value = _idExpanded == idPremises ? Resource.minus : Resource.plus;
                    break;
                case "is_checked":
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)snapshotedBindinsSource[rowIndex])["is_checked"];
                    break;
                case "id_street":
                    var kladrRow = Presenter.ViewModel["kladr"].DataSource.Rows.Find(buildingRow["id_street"]);
                    string streetName = null;
                    if (kladrRow != null)
                        streetName = kladrRow["street_name"].ToString();
                    e.Value = streetName;
                    break;
                case "house":
                    e.Value = buildingRow["house"];
                    break;
                case "id_premises":
                case "premises_num":
                case "id_premises_type":
                case "cadastral_num":
                case "total_area":
                case "living_area":
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name];
                    break;
            }
        }

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DataGridView.CurrentCell is DataGridViewCheckBoxCell)
                DataGridView.EndEdit();
        }
    }
}
