using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.Presenters;
using Registry.Viewport.ViewModels;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class SubPremisesViewport: EditableDataGridViewport
    {
        private SubPremisesViewport()
            : this(null, null)
        {
        }

        public SubPremisesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new SubPremisesPresenter())
        {
            InitializeComponent(); 
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        protected override List<Entity> EntitiesListFromViewport()
        {
            return ((SubPremisesPresenter)Presenter).EntitiesListFromSnapshot();
        }

        protected override List<Entity> EntitiesListFromView()
        {
            return ((SubPremisesPresenter)Presenter).EntitiesListFromView();
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;            
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            ((SnapshotedViewModel)Presenter.ViewModel).InitializeSnapshot();
            GeneralSnapshot = ((SnapshotedViewModel) Presenter.ViewModel).SnapshotDataSource;
            GeneralSnapshotBindingSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;

            if (ParentRow != null && ParentType == ParentTypeEnum.Premises)
                Text = string.Format(CultureInfo.InvariantCulture, "Комнаты помещения №{0}", ParentRow["id_premises"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            DataBind();

            AddEventHandler<EventArgs>(((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource, 
                "CurrentItemChanged", v_snapshot_sub_premises_CurrentItemChanged);
            //Синхронизация данных исходные->текущие
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", SubPremisesViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleting", SubPremisesViewport_RowDeleting);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", SubPremisesViewport_RowDeleted);

            v_snapshot_sub_premises_CurrentItemChanged(null, new EventArgs());
        }

        private void DataBind()
        {
            dataGridView.DataSource = ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource;
            id_sub_premises.DataPropertyName = "id_sub_premises";
            id_premises.DataPropertyName = "id_premises";
            sub_premises_num.DataPropertyName = "sub_premises_num";
            total_area.DataPropertyName = "total_area";
            living_area.DataPropertyName = "living_area";
            description.DataPropertyName = "description";

            state_date.DataPropertyName = "state_date";
            cadastral_num.DataPropertyName = "cadastral_num";
            cadastral_cost.DataPropertyName = "cadastral_cost";
            balance_cost.DataPropertyName = "balance_cost";
            account.DataPropertyName = "account";
            ViewportHelper.BindSource(id_state, Presenter.ViewModel["object_states"].BindingSource, "state_female",
                Presenter.ViewModel["object_states"].PrimaryKeyFirst);
        }

        public override bool CanInsertRecord()
        {
            return AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) ||
                   AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal);
        }

        public override void InsertRecord()
        {
            ((SubPremisesPresenter)Presenter).InsertRecordIntoSnapshot((int)ParentRow["id_premises"]);
        }

        public override bool CanDeleteRecord()
        {
            return (((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource.Position != -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void DeleteRecord()
        {
            ((SubPremisesPresenter)Presenter).DeleteCurrentRecordFromSnapshot();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            ((SnapshotedViewModel)Presenter.ViewModel).LoadSnapshot();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void SaveRecord()
        {
            SyncViews = false;
            dataGridView.EndEdit();
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            if (((SubPremisesPresenter)Presenter).ValidateSubPremisesInSnapshot())
            {
                ((SubPremisesPresenter)Presenter).SaveRecords();
                MenuCallback.EditingStateUpdate();
            }
            Presenter.ViewModel["general"].Model.EditingNewRecord = false;
            SyncViews = true;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new SubPremisesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.FundsHistoryViewport,
                ViewportType.TenancyListViewport,
                ViewportType.PaymentsAccountsViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (Presenter.ViewModel["general"].CurrentRow != null);
        }

        public override void ShowAssocViewport<T>()
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Перед открытием связных объектов необходимо сохранить изменения в базу данных. " +
                    @"Вы хотите это сделать?", @"Внимание",
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
                        return;
                }
            }
            var snapshotBindingSource = ((SnapshotedViewModel) Presenter.ViewModel).SnapshotBindingSource;
            if (snapshotBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрана комната для отображения истории принадлежности к фондам", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback,  "id_sub_premises = " +
                Convert.ToInt32(((DataRowView)snapshotBindingSource[snapshotBindingSource.Position])["id_sub_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)snapshotBindingSource[snapshotBindingSource.Position]).Row, ParentTypeEnum.SubPremises);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            switch (reporterType)
            {
                case  ReporterType.RegistryExcerptReporterSubPremise:
                    return ((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource.Count > 0;
                case  ReporterType.RegistryExcerptReporterPremise:
                case ReporterType.RegistryExcerptReporterAllMunSubPremises:
                    return true;
            }
            return false;
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Перед открытием истории принадлежности фондам необходимо сохранить изменения в базу данных. " +
                    @"Вы хотите это сделать?", @"Внимание",
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
                        return;
                }
            }
            if (((SnapshotedViewModel)Presenter.ViewModel).SnapshotBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрана комната для отображения истории принадлежности к фондам", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.RegistryExcerptReporterSubPremise:
                    arguments = RegistryExcerptReporterSubPremiseArguments();
                    break;
                case ReporterType.RegistryExcerptReporterPremise:
                    arguments = RegistryExcerptPremiseReportArguments();
                    break;
                case ReporterType.RegistryExcerptReporterAllMunSubPremises:
                    arguments = RegistryExcerptReporterAllMunSubPremisesArguments();
                    break;
            }
            MenuCallback.RunReport(reporterType, arguments);
        }

        private Dictionary<string, string> RegistryExcerptPremiseReportArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", ParentRow["id_premises"].ToString()},
                {"excerpt_type", "1"}
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptReporterSubPremiseArguments()
        {
            var snapshotBindingSource = ((SnapshotedViewModel) Presenter.ViewModel).SnapshotBindingSource;
            var arguments = new Dictionary<string, string>
            {
                {
                    "ids",
                    ((DataRowView) snapshotBindingSource[snapshotBindingSource.Position])["id_sub_premises"].ToString()
                },
                {"excerpt_type", "2"}
            };
            return arguments;
        }

        private Dictionary<string, string> RegistryExcerptReporterAllMunSubPremisesArguments()
        {
            var arguments = new Dictionary<string, string>
            {
                {"ids", ParentRow["id_premises"].ToString()},
                {"excerpt_type", "3"}
            };
            return arguments;
        }

        private void v_snapshot_sub_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
            MenuCallback.DocumentsStateUpdate();
        }

        private void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.Value == null)
            {
                return;
            }
            switch (cell.OwningColumn.Name)
            {
                case "total_area":
                case "living_area":
                case "cadastral_cost":
                case "balance_cost":
                    double stub;
                    if (string.IsNullOrEmpty(cell.Value.ToString()) || !double.TryParse(cell.Value.ToString(), out stub))
                        cell.Value = 0;
                    break;
                case "sub_premises_num":
                    if (cell.Value.ToString().Trim().Length > 20)
                        cell.ErrorText = "Длина номера комнаты не может превышать 20 символов";
                    else
                        if ((cell.Value.ToString().Trim().Length > 0) && !Regex.IsMatch(cell.Value.ToString().Trim(), "^([0-9]+[а-я]{0,1}|[а-я])$"))
                            cell.ErrorText = "Номер комнаты может содержать в себе только цифры и не более одной строчной буквы кирилицы";
                        else
                            cell.ErrorText = "";
                    break;
                case "description":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 65535 ? 
                        "Длина примечания комнаты не может превышать 65535 символов" : "";
                    break;
            }
        }

        private void SubPremisesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            MenuCallback.ForceCloseDetachedViewports();
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void SubPremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            ((SubPremisesPresenter) Presenter).DeleteRowByIdFromSnapshot((int) e.Row["id_sub_premises"]);
        }

        private void SubPremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            ((SubPremisesPresenter) Presenter).InsertOrUpdateRowIntoSnapshot(e.Row);
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
            }
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.SelectedCells.Count > 0 && 
                new[] { "total_area", "living_area", "cadastral_cost", "balance_cost" }.Contains(dataGridView.SelectedCells[0].OwningColumn.Name))
            {
                dataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
                dataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
                var textBox = (TextBox) e.Control;
                textBox.Text = string.IsNullOrEmpty(textBox.Text.Trim()) ? 
                    @"0" : 
                    textBox.Text.Substring(0, textBox.Text.Trim().IndexOf(" ", StringComparison.Ordinal));
            } else
                if (dataGridView.CurrentCell.OwningColumn.Name == "sub_premises_num")
                {
                    dataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
                    dataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
                }
                else
                    if (dataGridView.CurrentCell.OwningColumn.Name == "id_state")
                {
                    var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                    if (editingControl == null) return;
                    editingControl.DropDownClosed -= editingControl_DropDownClosed;
                    editingControl.DropDownClosed += editingControl_DropDownClosed;
                }
        }

        private void editingControl_DropDownClosed(object sender, EventArgs e)
        {
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            if (editingControl != null) dataGridView.CurrentCell.Value = editingControl.SelectedValue;
            dataGridView.EndEdit();
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "total_area":
                    MessageBox.Show(@"Значение общей площади комнаты является некорректным",@"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.ThrowException = false;
                    break;
                case "living_area":
                    MessageBox.Show(@"Значение жилой площади комнаты является некорректным", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.ThrowException = false;
                    break;
                case "cadastral_cost":
                    MessageBox.Show(@"Значение кадастровой стоимости комнаты является некорректным", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.ThrowException = false;
                    break;
                case "balance_cost":
                    MessageBox.Show(@"Значение балансовой стоимости комнаты является некорректным", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.ThrowException = false;
                    break;
            }
        }

        private void EditingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView.SelectedCells.Count > 0 && 
                new[] { "total_area", "living_area", "cadastral_cost", "balance_cost" }.Contains(dataGridView.SelectedCells[0].OwningColumn.Name))
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == (char)8))
                    e.Handled = false;
                else
                    if ((e.KeyChar == '.') || (e.KeyChar == ','))
                    {
                        e.KeyChar = ',';
                        e.Handled = ((TextBox)dataGridView.EditingControl).Text.IndexOf(',') != -1;
                    }
                    else
                        e.Handled = true;
            } else
                if (dataGridView.SelectedCells[0].OwningColumn.Name == "sub_premises_num")
                {
                    if (e.KeyChar >= 'А' && e.KeyChar <= 'Я')
                        e.KeyChar = e.KeyChar.ToString().ToLower(CultureInfo.CurrentCulture)[0];
                    if (e.KeyChar == ' ')
                        e.Handled = true;
                }
        }
    }
}
