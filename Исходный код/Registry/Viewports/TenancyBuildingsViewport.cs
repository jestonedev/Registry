using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Windows.Forms;
using Registry.Entities.Infrastructure;
using Registry.Viewport.Presenters;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyBuildingsViewport : DataGridViewport
    {
        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private TenancyBuildingsViewport()
            : this(null, null)
        {
        }

        public TenancyBuildingsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new TenancyBuildingsPresenter())
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
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;
            Presenter.ViewModel["tenancy_buildings_assoc"].BindingSource.Filter = StaticFilter;
            Presenter.ParentRow = ParentRow;
            Presenter.ParentType = ParentType;

            ((SnapshotedViewModel)Presenter.ViewModel).InitializeSnapshot();

            if ((ParentRow != null) && (ParentType == ParentTypeEnum.Tenancy))
                Text = @"Здания найма №" + ParentRow["id_process"];
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Строим фильтр арендуемых зданий
            if (string.IsNullOrEmpty(DynamicFilter))
            {
                DynamicFilter = ((TenancyBuildingsPresenter) Presenter).GetDefaultDynamicFilter();
            }
            Presenter.ViewModel["general"].BindingSource.Filter = DynamicFilter;

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", BuildingsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", BuildingsViewport_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["tenancy_buildings_assoc"].DataSource, 
                "RowChanged", TenancyBuildingsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["tenancy_buildings_assoc"].DataSource, 
                "RowDeleting", TenancyBuildingsViewport_RowDeleting);

            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить это здание?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                ((TenancyBuildingsPresenter) Presenter).DeleteRecord();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanSearchRecord()
        {
            return true;
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
            var viewport = new BuildingViewport(null, MenuCallback)
            {
                StaticFilter = "",
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
            var viewport = new BuildingViewport(null, MenuCallback)
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
            var viewport = new BuildingViewport(null, MenuCallback)
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

        public override bool CanCancelRecord()
        {
            return ((TenancyBuildingsPresenter)Presenter).SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            ((SnapshotedViewModel)Presenter.ViewModel).LoadSnapshot();
            DataGridView.Refresh();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return ((TenancyBuildingsPresenter)Presenter).SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            _syncViews = false;
            DataGridView.EndEdit();
            Presenter.ViewModel["tenancy_buildings_assoc"].Model.EditingNewRecord = true;
            if (((TenancyBuildingsPresenter)Presenter).ValidateTenancyBuildings())
            {
                ((TenancyBuildingsPresenter)Presenter).SaveRecords();
                MenuCallback.EditingStateUpdate();
            }
            Presenter.ViewModel["tenancy_buildings_assoc"].Model.EditingNewRecord = false;
            _syncViews = true;
            DataGridView.Invalidate();
            MenuCallback.EditingStateUpdate();
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
            return reports.Any(v => v.ToString() == typeof(T).Name) && (Presenter.ViewModel["general"].CurrentRow != null);
        }

        public override void ShowAssocViewport<T>()
        {
            var viewModel = Presenter.ViewModel["general"];
            if (viewModel.CurrentRow == null)
            {
                MessageBox.Show(@"Не выбрано здание для отображения связных объектов", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, viewModel.PrimaryKeyFirst + " = " +
                Convert.ToInt32(viewModel.CurrentRow[viewModel.PrimaryKeyFirst], CultureInfo.InvariantCulture),
                viewModel.CurrentRow.Row, ParentTypeEnum.Building);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (((TenancyBuildingsPresenter)Presenter).SnapshotHasChanges())
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

        private void TenancyBuildingsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            if (e.Action == DataRowAction.Delete)
            {
                ((TenancyBuildingsPresenter)Presenter).DeleteRowByIdFromSnapshot((int)e.Row["id_building"]);
            }
            DataGridView.Invalidate();
        }

        private void TenancyBuildingsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Row["id_process"] == DBNull.Value || 
                Convert.ToInt32(e.Row["id_process"], CultureInfo.InvariantCulture) != Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture))
                return;
            ((TenancyBuildingsPresenter)Presenter).InsertOrUpdateRowIntoSnapshot(e.Row);
            DataGridView.Invalidate();
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView_ColumnHeaderMouseClick(sender, e);
        }

        private void BuildingsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            DataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void BuildingsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                DataGridView.Refresh();
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView_SelectionChanged();
        }

        private void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            var idBuilding = Convert.ToInt32(((DataRowView)bindingSource[e.RowIndex])["id_building"], CultureInfo.InvariantCulture);
            var snapshotedBindinsSource = ((SnapshotedViewModel) Presenter.ViewModel).SnapshotBindingSource;
            var snapshotedDataSource = ((SnapshotedViewModel) Presenter.ViewModel).SnapshotDataSource;
            var rowIndex = snapshotedBindinsSource.Find("id_building", idBuilding);
            _syncViews = false;
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (rowIndex == -1)
                        snapshotedDataSource.Rows.Add(null, idBuilding, e.Value, null);
                    else
                        ((DataRowView)snapshotedBindinsSource[rowIndex])["is_checked"] = e.Value;
                    break;
                case "rent_total_area":
                case "rent_living_area":
                    double value = 0;
                    if (e.Value != null)
                        double.TryParse(e.Value.ToString(), out value);
                    if (rowIndex == -1)
                        snapshotedDataSource.Rows.Add(null, idBuilding, false, value > 0 ? (object)value : DBNull.Value, DBNull.Value);
                    else
                        ((DataRowView)snapshotedBindinsSource[rowIndex])[DataGridView.Columns[e.ColumnIndex].Name] = value > 0 ?
                            (object)value : DBNull.Value;
                    break;
            }
            _syncViews = true;
            MenuCallback.EditingStateUpdate();
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (Presenter.ViewModel["general"].BindingSource.Count <= e.RowIndex || Presenter.ViewModel["general"].BindingSource.Count == 0) return;
            var row = (DataRowView) Presenter.ViewModel["general"].BindingSource[e.RowIndex];
            var idBuilding = Convert.ToInt32(row["id_building"], CultureInfo.InvariantCulture);
            var snapshotedBindinsSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;
            var rowIndex = snapshotedBindinsSource.Find("id_building", idBuilding);
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                case "rent_total_area":
                case "rent_living_area":
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)snapshotedBindinsSource[rowIndex])[DataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "id_building":
                case "house":
                case "floors":
                case "living_area":
                case "cadastral_num":
                case "startup_year":
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name];
                    break;
                case "id_street":
                    var streetRow = Presenter.ViewModel["kladr"].DataSource.Rows.Find(row["id_street"]);
                    if (streetRow != null)
                        e.Value = streetRow["street_name"];
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
            if ((DataGridView.CurrentCell.OwningColumn.Name != "rent_total_area") &&
                (DataGridView.CurrentCell.OwningColumn.Name != "rent_living_area")) return;
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

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (DataGridView.Size.Width > 1260)
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

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DataGridView.CurrentCell is DataGridViewCheckBoxCell)
                DataGridView.EndEdit();
        }
    }
}
