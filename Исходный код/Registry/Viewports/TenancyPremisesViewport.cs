using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.Presenters;
using Registry.Viewport.Properties;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyPremisesViewport: DataGridViewport
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

        private TenancyPremisesViewport()
            : this(null, null)
        {
        }

        public TenancyPremisesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new TenancyPremisesPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            ViewportHelper.SetDoubleBuffered(DataGridView);
        }

        private bool SnapshotHasChanges()
        {
            if (((TenancyPremisesPresenter)Presenter).SnapshotHasChanges())
            {
                return true;
            }

            //Проверяем комнаты
            var listRoomsFromView = ((TenancySubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).TenancySubPremisesFromView();
            var listRoomsFromViewport = ((TenancySubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).TenancySubPremisesFromViewport();
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;

            Presenter.ViewModel["tenancy_premises_assoc"].BindingSource.Filter = StaticFilter;

            Presenter.ParentRow = ParentRow;
            Presenter.ParentType = ParentType;

            ((SnapshotedViewModel)Presenter.ViewModel).InitializeSnapshot();

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                Text = @"Помещения найма №" + ParentRow["id_process"];
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
            var details = new TenancySubPremisesDetails
            {
                v_sub_premises = _vSubPremises,
                sub_premises = _subPremises.Select(),
                StaticFilter = StaticFilter,
                ParentRow = ParentRow,
                ParentType = ParentType,
                menuCallback = MenuCallback
            };
            details.InitializeControl();
            ((DataGridViewWithDetails)DataGridView).DetailsControl = details;

            //Строим фильтр арендуемых квартир и комнат во время первой загрузки
            if (string.IsNullOrEmpty(DynamicFilter))
            {
                DynamicFilter = ((TenancyPremisesPresenter)Presenter).GetDefaultDynamicFilter();
                var defaultFilterSubPremises = details.GetDefaultDynamicFilter();
                if (!string.IsNullOrEmpty(DynamicFilter) && !string.IsNullOrEmpty(defaultFilterSubPremises))
                {
                    DynamicFilter += " OR ";
                }
                DynamicFilter += defaultFilterSubPremises;
            }
            Presenter.ViewModel["general"].BindingSource.Filter = DynamicFilter;

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", PremisesListViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", PremisesListViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["tenancy_premises_assoc"].DataSource, 
                "RowChanged", TenancyPremisesViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["tenancy_premises_assoc"].DataSource, 
                "RowDeleting", TenancyPremisesViewport_RowDeleting);

            AddEventHandler<EventArgs>(Presenter.ViewModel["premises_current_funds"].Model, "RefreshEvent", premises_funds_RefreshEvent);

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
                ((TenancyPremisesPresenter)Presenter).DeleteRecord();
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
            ((TenancySubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).CancelRecord();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            _syncViews = false;
            DataGridView.EndEdit();
            Presenter.ViewModel["tenancy_premises_assoc"].Model.EditingNewRecord = true;
            if (((TenancyPremisesPresenter)Presenter).ValidateTenancyPremises())
            {
                ((TenancyPremisesPresenter)Presenter).SaveRecords();
                ((TenancySubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).SaveRecord();
            }
            Presenter.ViewModel["tenancy_premises_assoc"].Model.EditingNewRecord = false;
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
                viewModel.CurrentRow.Row, ParentTypeEnum.Premises);
        }

        private void premises_funds_RefreshEvent(object sender, EventArgs e)
        {
            DataGridView.Refresh();
        }

        private void TenancyPremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                ((TenancyPremisesPresenter)Presenter).DeleteRowByIdFromSnapshot((int)e.Row["id_premises"]);
            }
            DataGridView.Refresh();
        }

        private void TenancyPremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            ((TenancyPremisesPresenter)Presenter).InsertOrUpdateRowIntoSnapshot(e.Row);
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
            ((TenancySubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).CalcControlHeight();
            var width = 0;
            for (var i = 0; i < DataGridView.Columns.Count; i++)
                width += DataGridView.Columns[i].Width;
            width += DataGridView.RowHeadersWidth;
            ((TenancySubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).SetControlWidth(width);
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            var width = 0;
            for (var i = 0; i < DataGridView.Columns.Count; i++)
                width += DataGridView.Columns[i].Width;
            width += DataGridView.RowHeadersWidth;
            ((TenancySubPremisesDetails)((DataGridViewWithDetails)DataGridView).DetailsControl).SetControlWidth(width);
            if (DataGridView.Size.Width > 1650)
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
            var row = (DataRowView)Presenter.ViewModel["general"].BindingSource[e.RowIndex];
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
                        snapshotedDataSource.Rows.Add(null, idPremises, e.Value, null);
                    else
                        ((DataRowView)snapshotedBindinsSource[rowIndex])["is_checked"] = e.Value;
                    break;
                case "rent_total_area":
                case "rent_living_area":
                    double value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (rowIndex == -1)
                        snapshotedDataSource.Rows.Add(null, idPremises, false, value > 0 ? (object)value : DBNull.Value, DBNull.Value);
                    else
                        ((DataRowView)snapshotedBindinsSource[rowIndex])[DataGridView.Columns[e.ColumnIndex].Name] = value > 0 ? (object)value : DBNull.Value;
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
                case "rent_total_area":
                case "rent_living_area":
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)snapshotedBindinsSource[rowIndex])[DataGridView.Columns[e.ColumnIndex].Name];
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
                case "total_area":
                case "cadastral_num":
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "id_state":
                    var stateRow = Presenter.ViewModel["object_states"].DataSource.Rows.Find(row["id_state"]);
                    if (stateRow != null)
                        e.Value = stateRow["state_female"];
                    break;
                case "current_fund":
                    if (DataModelHelper.MunicipalAndUnknownObjectStates().ToList().Contains((int)row["id_state"]))
                    {
                        var fundRow = Presenter.ViewModel["premises_current_funds"].DataSource.Rows.Find(row["id_premises"]);
                        if (fundRow != null)
                            e.Value = Presenter.ViewModel["fund_types"].DataSource.Rows.Find(fundRow["id_fund_type"])["fund_type"];
                    }
                    break;
            }
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if ((DataGridView.CurrentCell.OwningColumn.Name != "rent_total_area") &&
                (DataGridView.CurrentCell.OwningColumn.Name != "rent_living_area")) return;
            DataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
            DataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
            if (string.IsNullOrEmpty(((TextBox)e.Control).Text.Trim()))
                ((TextBox)e.Control).Text = ((TextBox)e.Control).Text = @"0";
            else
                ((TextBox)e.Control).Text = ((TextBox)e.Control).Text.Substring(0, ((TextBox)e.Control).Text.Length - 3);
        }

        private void EditingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((DataGridView.CurrentCell.OwningColumn.Name == "rent_total_area") ||
                (DataGridView.CurrentCell.OwningColumn.Name == "rent_living_area"))
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == (char)8))
                    e.Handled = false;
                else
                    if ((e.KeyChar == '.') || (e.KeyChar == ','))
                    {
                        e.KeyChar = ',';
                        e.Handled = ((TextBox)DataGridView.EditingControl).Text.IndexOf(',') != -1;
                    }
                    else
                        e.Handled = true;
            }
        }

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DataGridView.CurrentCell is DataGridViewCheckBoxCell)
                DataGridView.EndEdit();
        }
    }
}
