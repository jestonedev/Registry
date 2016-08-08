using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Declensions.Unicode;
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

namespace Registry.Viewport
{
    internal sealed partial class TenancyAgreementsViewport : FormWithGridViewport
    {
        #region Modeles

        private DataModel _tenancyPersonsExclude;
        private DataModel _executors;
        private DataModel _warrants;
        private DataModel _kinships;
        #endregion Modeles
        
        #region Views

        private BindingSource _vPersonsExclude;
        private BindingSource _vPersonsChangeTenant;
        private BindingSource _vTenantChangeTenant;
        private BindingSource _vExecutors;
        private BindingSource _vWarrants;
        private BindingSource _vKinships;
        #endregion Views

        //Forms
        private SelectWarrantForm _swForm;
        private int? _idWarrant;
        private bool _isFirstVisible = true;   // первое отображение формы

        // Auto modify properties
        private readonly List<TenancyPerson> _excludePersons = new List<TenancyPerson>(); 
        private readonly List<TenancyPerson> _includePersons = new List<TenancyPerson>();

        private enum ExtModificationTypes
        {
            CommercialProlong,
            SpecialProlong,
            ChangeTenant
        }

        private sealed class ExtModificationParameter
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        private readonly List<Dictionary<ExtModificationTypes, List<ExtModificationParameter>>> _modifications = 
            new List<Dictionary<ExtModificationTypes, List<ExtModificationParameter>>>();

        private TenancyAgreementsViewport()
            : this(null, null)
        {
        }

        public TenancyAgreementsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        private void RedrawDataGridTenancyPersonsRows()
        {
            if (dataGridViewTenancyPersons.Rows.Count == 0)
                return;
            for (var i = 0; i < dataGridViewTenancyPersons.Rows.Count; i++)
            {
                if (_vPersonsExclude.Count <= i) break;
                if (((DataRowView)_vPersonsExclude[i])["id_kinship"] != DBNull.Value &&
                    Convert.ToInt32(((DataRowView)_vPersonsExclude[i])["id_kinship"], CultureInfo.InvariantCulture) ==
                    1 &&
                    ((DataRowView)_vPersonsExclude[i])["exclude_date"] == DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else if (((DataRowView)_vPersonsExclude[i])["exclude_date"] != DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
                else
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }

            if (dataGridViewChangeTenant.Rows.Count == 0)
                return;
            for (var i = 0; i < dataGridViewChangeTenant.Rows.Count; i++)
            {
                if (_vPersonsChangeTenant.Count <= i) break;
                if (((DataRowView) _vPersonsChangeTenant[i])["id_kinship"] != DBNull.Value &&
                    Convert.ToInt32(((DataRowView) _vPersonsChangeTenant[i])["id_kinship"], CultureInfo.InvariantCulture) ==
                    1 &&
                    ((DataRowView) _vPersonsChangeTenant[i])["exclude_date"] == DBNull.Value)
                    dataGridViewChangeTenant.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else if (((DataRowView) _vPersonsChangeTenant[i])["exclude_date"] != DBNull.Value)
                    dataGridViewChangeTenant.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
                else
                    dataGridViewChangeTenant.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void DataBind()
        {
            comboBoxExecutor.DataSource = _vExecutors;
            comboBoxExecutor.ValueMember = "id_executor";
            comboBoxExecutor.DisplayMember = "executor_name";
            comboBoxExecutor.DataBindings.Clear();
            comboBoxExecutor.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_executor", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxIncludeKinship.DataSource = _vKinships;
            comboBoxIncludeKinship.ValueMember = "id_kinship";
            comboBoxIncludeKinship.DisplayMember = "kinship";

            textBoxAgreementContent.DataBindings.Clear();
            textBoxAgreementContent.DataBindings.Add("Text", GeneralBindingSource, "agreement_content", true, DataSourceUpdateMode.Never, "");

            dateTimePickerAgreementDate.DataBindings.Clear();
            dateTimePickerAgreementDate.DataBindings.Add("Value", GeneralBindingSource, "agreement_date", true, DataSourceUpdateMode.Never, DateTime.Now);

            dataGridViewTenancyPersons.DataSource = _vPersonsExclude;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            date_of_birth.DataPropertyName = "date_of_birth";

            dataGridView.DataSource = GeneralBindingSource;
            id_agreement.DataPropertyName = "id_agreement";
            agreement_date.DataPropertyName = "agreement_date";
            agreement_content.DataPropertyName = "agreement_content";

            comboboxTenantChangeKinship.DataSource = _vKinships;
            comboboxTenantChangeKinship.DisplayMember = "kinship";
            comboboxTenantChangeKinship.ValueMember = "id_kinship";

            UpdateTenantChangeTab();

            dataGridViewChangeTenant.DataSource = _vPersonsChangeTenant;
            surnameChangeTenant.DataPropertyName = "surname";
            nameChangeTenant.DataPropertyName = "name";
            patronymicChangeTenant.DataPropertyName = "patronymic";
            dateofbirthChangeTenant.DataPropertyName = "date_of_birth";
        }

        private void UpdateTenantChangeTab()
        {
            if (_vTenantChangeTenant.Count > 0)
            {
                var row = (DataRowView)_vTenantChangeTenant[0];
                textBoxChangeTenantChangeFIO.Text = row["surname"] + @" " + row["name"] + @" " + row["patronymic"];
            }
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.TenancyWrite)) return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private string WarrantStringById(int idWarrant)
        {
            if (_vWarrants.Position == -1)
                return null;
            var rowIndex = _vWarrants.Find("id_warrant", idWarrant);
            if (rowIndex == -1)
                return null;
            var registrationDate = Convert.ToDateTime(((DataRowView)_vWarrants[rowIndex])["registration_date"], CultureInfo.InvariantCulture);
            var registrationNum = ((DataRowView)_vWarrants[rowIndex])["registration_num"].ToString();
            return string.Format(CultureInfo.InvariantCulture, "№ {0} от {1}",
                registrationNum, registrationDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
        }

        private void BindWarrantId()
        {
            if ((GeneralBindingSource.Position > -1) && ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_warrant"] != DBNull.Value)
            {
                _idWarrant = Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_warrant"], CultureInfo.InvariantCulture);
                textBoxAgreementWarrant.Text =
                    WarrantStringById(_idWarrant.Value);
                vButtonSelectWarrant.Text = @"x";
            }
            else
            {
                _idWarrant = null;
                textBoxAgreementWarrant.Text = "";
                vButtonSelectWarrant.Text = @"...";
            }
        }

        private void ViewportFromTenancyAgreement(TenancyAgreement tenancyAgreement)
        {
            comboBoxExecutor.SelectedValue = ViewportHelper.ValueOrDbNull(tenancyAgreement.IdExecutor);
            dateTimePickerAgreementDate.Value = ViewportHelper.ValueOrDefault(tenancyAgreement.AgreementDate);
            textBoxAgreementContent.Text = tenancyAgreement.AgreementContent;
            if (tenancyAgreement.IdWarrant != null)
            {
                textBoxAgreementWarrant.Text = WarrantStringById(tenancyAgreement.IdWarrant.Value);
                _idWarrant = tenancyAgreement.IdWarrant;
            }
            else
            {
                textBoxAgreementWarrant.Text = "";
                _idWarrant = null;
            }
        }

        protected override Entity EntityFromViewport()
        {
            var tenancyAgreement = new TenancyAgreement
            {
                IdAgreement = GeneralBindingSource.Position == -1
                    ? null
                    : ViewportHelper.ValueOrNull<int>((DataRowView) GeneralBindingSource[GeneralBindingSource.Position],
                        "id_agreement")
            };
            if (ParentType == ParentTypeEnum.Tenancy && ParentRow != null)
                tenancyAgreement.IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process");
            else
                tenancyAgreement.IdProcess = null;
            tenancyAgreement.IdExecutor = ViewportHelper.ValueOrNull<int>(comboBoxExecutor);
            tenancyAgreement.IdWarrant = _idWarrant;
            tenancyAgreement.AgreementContent = textBoxAgreementContent.Text;
            tenancyAgreement.AgreementDate = ViewportHelper.ValueOrNull(dateTimePickerAgreementDate);
            return tenancyAgreement;
        }

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return EntityConverter<TenancyAgreement>.FromRow(row);
        }

        private bool ValidateAgreement(TenancyAgreement tenancyAgreement)
        {
            if (tenancyAgreement.IdExecutor != null) return true;
            MessageBox.Show(@"Необходимо выбрать исполнителя", @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            comboBoxExecutor.Focus();
            return false;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            dataGridViewTenancyPersons.AutoGenerateColumns = false;
            dataGridViewChangeTenant.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = EntityDataModel<TenancyAgreement>.GetInstance();
            _tenancyPersonsExclude = EntityDataModel<TenancyPerson>.GetInstance();
            _executors = DataModel.GetInstance<EntityDataModel<Executor>>();
            _warrants = EntityDataModel<Warrant>.GetInstance();
            _kinships = DataModel.GetInstance<KinshipsDataModel>();

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            _tenancyPersonsExclude.Select();
            _executors.Select();
            _warrants.Select();
            _kinships.Select();

            var ds = DataStorage.DataSet;

            if ((ParentType == ParentTypeEnum.Tenancy) && (ParentRow != null))
                Text = string.Format(CultureInfo.InvariantCulture, "Соглашения найма №{0}", ParentRow["id_process"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            _vPersonsExclude = new BindingSource
            {
                DataMember = "tenancy_persons",
                Filter = StaticFilter,
                DataSource = ds
            };

            _vPersonsChangeTenant = new BindingSource
            {
                DataMember = "tenancy_persons",
                Filter = StaticFilter + " AND (id_kinship <> 1 OR exclude_date is not null)",
                DataSource = ds
            };

            _vTenantChangeTenant = new BindingSource
            {
                DataMember = "tenancy_persons",
                Filter = StaticFilter + " AND (id_kinship = 1 AND exclude_date is null)",
                DataSource = ds
            };

            _vExecutors = new BindingSource
            {
                DataMember = "executors",
                DataSource = ds,
                Filter = "is_inactive = 0"
            };

            _vWarrants = new BindingSource
            {
                DataMember = "warrants",
                DataSource = ds
            };

            _vKinships = new BindingSource
            {
                DataMember = "kinships",
                DataSource = ds
            };

            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);
            GeneralBindingSource.DataMember = "tenancy_agreements";
            GeneralBindingSource.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;
            GeneralBindingSource.DataSource = ds;
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", TenancyAgreementsViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", TenancyAgreementsViewport_RowChanged);

            DataBind();

            AddEventHandler<DataRowChangeEventArgs>(_tenancyPersonsExclude.Select(), "RowDeleted", TenancyPersonsViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(_tenancyPersonsExclude.Select(), "RowChanged", TenancyPersonsViewport_RowChanged);

            IsEditable = true;
            DataChangeHandlersInit();
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите это соглашение?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_agreement"]) == -1)
                return;
            IsEditable = false;
            ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            MenuCallback.ForceCloseDetachedViewports();
        }

        public override bool CanCancelRecord()
        {
            return (ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (ViewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        IsEditable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        if (GeneralBindingSource.Position != -1)
                            dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                    }
                    else
                        Text = @"Соглашения отсутствуют";
                    ViewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    IsEditable = false;
                    DataBind();
                    BindWarrantId();
                    ViewportState = ViewportState.ReadState;
                    break;
            }
            IsEditable = true;
            ClearModifyState();
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState))
                 && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            var tenancyAgreement = (TenancyAgreement) EntityFromViewport();
            if (!ValidateAgreement(tenancyAgreement))
                return;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                case ViewportState.NewRowState:
                    var idAgreement = GeneralDataModel.Insert(tenancyAgreement);
                    if (idAgreement == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    tenancyAgreement.IdAgreement = idAgreement;
                    IsEditable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    EntityConverter<TenancyAgreement>.FillRow(tenancyAgreement, newRow);
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (tenancyAgreement.IdAgreement == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить соглашение без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(tenancyAgreement) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    IsEditable = false;
                    EntityConverter<TenancyAgreement>.FillRow(tenancyAgreement, row);
                    break;
            }
            ViewportState = ViewportState.ReadState;
            dataGridView.Enabled = true;
            IsEditable = true;
            MenuCallback.EditingStateUpdate();
            
            // Обновление участников найма после сохранения соглашения
            if (_includePersons.Count > 0)
            {
                new TenancyAgreementOnSavePersonManager(_includePersons,
                    TenancyAgreementOnSavePersonManager.PersonsOperationType.IncludePersons).ShowDialog();
            }
            if (_excludePersons.Count > 0)
            {
                new TenancyAgreementOnSavePersonManager(_excludePersons,
                    TenancyAgreementOnSavePersonManager.PersonsOperationType.ExcludePersons).ShowDialog();
            }
            // Дополнительные обновления
            ExtModificationsExecute(_modifications);
            ClearModifyState();
        }

        private void ExtModificationsExecute(List<Dictionary<ExtModificationTypes, List<ExtModificationParameter>>> modifications)
        {
            foreach (var modification in modifications)
            {
                foreach (var modificationPair in modification)
                {
                    switch (modificationPair.Key)
                    {
                            case ExtModificationTypes.ChangeTenant:
                                ChangeTenant(modificationPair.Value);
                                break;
                            case ExtModificationTypes.CommercialProlong:
                            case ExtModificationTypes.SpecialProlong:
                                Prolong(modificationPair.Value);
                                break;
                    }
                }
            }
        }

        private void Prolong(List<ExtModificationParameter> parameters)
        {
            var beginDate = parameters.Where(v => v.Name == "ProlongFrom").Select(v => (DateTime?)v.Value).FirstOrDefault();
            var endDate = parameters.Where(v => v.Name == "ProlongTo").Select(v => (DateTime?)v.Value).FirstOrDefault();
            var untilDismissal = parameters.Where(v => v.Name == "UntilDismissal").Select(v => (bool?)v.Value).FirstOrDefault() ?? false;

            var rentPeriod = new TenancyRentPeriod
            {
                IdProcess = (int)ParentRow["id_process"],
                BeginDate = ViewportHelper.ValueOrNull<DateTime>(ParentRow, "begin_date"),
                EndDate = ViewportHelper.ValueOrNull<DateTime>(ParentRow, "end_date"),
                UntilDismissal = ViewportHelper.ValueOrNull<bool>(ParentRow, "until_dismissal"),
            };
            var rentPeriods = EntityDataModel<TenancyRentPeriod>.GetInstance();
            rentPeriods.EditingNewRecord = true;
            var idRentPeriod = rentPeriods.Insert(rentPeriod);
            if (idRentPeriod == -1) return;
            rentPeriod.IdRentPeriod = idRentPeriod;
            rentPeriods.Select().Rows.Add(idRentPeriod, rentPeriod.IdProcess, rentPeriod.BeginDate, rentPeriod.EndDate, rentPeriod.UntilDismissal);
            rentPeriods.EditingNewRecord = false;

            var tenancyProcess = EntityConverter<TenancyProcess>.FromRow(ParentRow);
            tenancyProcess.BeginDate = beginDate;
            tenancyProcess.EndDate = endDate;
            tenancyProcess.UntilDismissal = untilDismissal;
            var tenancyProcesses = EntityDataModel<TenancyProcess>.GetInstance();
            tenancyProcesses.EditingNewRecord = true;
            if (tenancyProcesses.Update(tenancyProcess) == -1)
            {
                return;
            }
            ParentRow["begin_date"] = ViewportHelper.ValueOrDbNull(beginDate);
            ParentRow["end_date"] = ViewportHelper.ValueOrDbNull(endDate);
            ParentRow["until_dismissal"] = untilDismissal;
            tenancyProcesses.EditingNewRecord = false;
        }

        private void ChangeTenant(List<ExtModificationParameter> parameters)
        {
            var idOldTenant = parameters.Where(v => v.Name == "IdOldTenant").Select(v => (int?)v.Value).FirstOrDefault();
            var idKinshipOldTenant = parameters.Where(v => v.Name == "IdKinshipOldTenant").Select(v => (int?)v.Value).FirstOrDefault();
            var idNewTenant = parameters.Where(v => v.Name == "IdNewTenant").Select(v => (int?)v.Value).FirstOrDefault();
            var excludeTenant = parameters.Where(v => v.Name == "ExcludeTenant").Select(v => (bool?)v.Value).FirstOrDefault();
            if (idOldTenant == null || idNewTenant == null) return;
            var oldTenantRow =
                EntityDataModel<TenancyPerson>.GetInstance()
                    .FilterDeletedRows().FirstOrDefault(v => v.Field<int>("id_person") == idOldTenant.Value);
            var newTenantRow =
                EntityDataModel<TenancyPerson>.GetInstance()
                    .FilterDeletedRows().FirstOrDefault(v => v.Field<int>("id_person") == idNewTenant.Value);
            var oldTenant = oldTenantRow != null ? EntityConverter<TenancyPerson>.FromRow(oldTenantRow) : null;
            var newTenant = newTenantRow != null ? EntityConverter<TenancyPerson>.FromRow(newTenantRow) : null;

            if (oldTenant == null || newTenant == null)
            {
                MessageBox.Show(@"Произошла непредвиденная ошибка при изменении данных об участниках найма",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            if (excludeTenant == true)
            {
                using (var form = new PersonExcludeDateForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        oldTenant.ExcludeDate = form.ExcludeDate;
                    }
                }
            }
            else
            {
                oldTenant.IdKinship = idKinshipOldTenant;
            }
            var affected = EntityDataModel<TenancyPerson>.GetInstance().Update(oldTenant);
            if (affected == -1)
            {
                return;
            }
            oldTenantRow.BeginEdit();
            oldTenantRow["id_kinship"] = ViewportHelper.ValueOrDbNull(oldTenant.IdKinship);
            oldTenantRow["exclude_date"] = ViewportHelper.ValueOrDbNull(oldTenant.ExcludeDate);
            oldTenantRow.EndEdit();

            newTenant.IdKinship = 1;
            newTenant.ExcludeDate = null;
            affected = EntityDataModel<TenancyPerson>.GetInstance().Update(newTenant);
            if (affected == -1)
            {
                return;
            }
            newTenantRow.BeginEdit();
            newTenantRow["id_kinship"] = ViewportHelper.ValueOrDbNull(newTenant.IdKinship);
            newTenantRow["exclude_date"] = ViewportHelper.ValueOrDbNull(newTenant.ExcludeDate);
            newTenantRow.EndEdit();
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            var index = _vExecutors.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)_vExecutors[index])["id_executor"];
            if (ParentRow != null && ParentType == ParentTypeEnum.Tenancy)
                textBoxAgreementContent.Text = string.Format(CultureInfo.InvariantCulture, "1.1. По настоящему Соглашению Стороны по договору № {0} от {1} договорились:",
                    ParentRow["registration_num"],
                    ParentRow["registration_date"] != DBNull.Value ?
                        Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "");
            IsEditable = true;
            GeneralDataModel.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            var tenancyAgreement = (TenancyAgreement) EntityFromView();
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromTenancyAgreement(tenancyAgreement);
            IsEditable = true;
        }

        public override bool HasReport(ReporterType reporterType)
        {
            if (GeneralBindingSource.Position == -1)
                return false;
            var idAgreement = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_agreement"] != DBNull.Value
                ? (int?)Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_agreement"],
                    CultureInfo.InvariantCulture) : null;
            switch (reporterType)
            {
                case ReporterType.TenancyAgreementReporter:
                case ReporterType.TenancyNotifyContractAgreement:
                    return idAgreement != null;
            }
            return false;
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate())
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбрано соглашение для печати",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            switch (reporterType)
            {
                case ReporterType.TenancyAgreementReporter:
                    ReporterFactory.CreateReporter(reporterType).Run(new Dictionary<string, string> { { "id_agreement", row["id_agreement"].ToString() } });
                    break;
                case ReporterType.TenancyNotifyContractAgreement:
                    var arguments = new Dictionary<string, string>
                    {
                        {"id_process", row["id_process"].ToString()},
                        {"report_type", "2"}
                    };
                    ReporterFactory.CreateReporter(reporterType).Run(arguments);
                    break;
            }
        }

        private bool TenancyValidForReportGenerate()
        {
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (ParentType != ParentTypeEnum.Tenancy)
            {
                MessageBox.Show(@"Некорректный родительский объект",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (!TenancyService.TenancyProcessHasTenant(Convert.ToInt32(ParentRow["id_process"], CultureInfo.InvariantCulture)))
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо указать нанимателя процесса найма", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewportHelper.ValueOrNull<DateTime>(ParentRow, "registration_date") == null || ViewportHelper.ValueOrNull(ParentRow, "registration_num") == null)
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо завести договор найма и указать его номер и дату регистрации", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridTenancyPersonsRows();
            if (_isFirstVisible)
            {
                _isFirstVisible = false;
                if (GeneralBindingSource.Count == 0)
                    InsertRecord();
            }
            vButtonSelectWarrant.Focus();
            base.OnVisibleChanged(e);
        }

        private void vButtonSelectWarrant_Click(object sender, EventArgs e)
        {
            if (_idWarrant != null)
            {
                _idWarrant = null;
                textBoxAgreementWarrant.Text = "";
                vButtonSelectWarrant.Text = @"...";
                return;
            }
            if (_swForm == null)
                _swForm = new SelectWarrantForm();
            if (_swForm.ShowDialog() == DialogResult.OK)
            {
                if (_swForm.WarrantId != null)
                {
                    _idWarrant = _swForm.WarrantId.Value;
                    textBoxAgreementWarrant.Text = WarrantStringById(_swForm.WarrantId.Value);
                    vButtonSelectWarrant.Text = @"x";
                }
            }
        }

        private void TenancyAgreementsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        private void TenancyAgreementsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void TenancyPersonsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridTenancyPersonsRows();
            UpdateTenantChangeTab();
        }

        private void TenancyPersonsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridTenancyPersonsRows();
            UpdateTenantChangeTab();
        }

        private void vButtonTerminatePaste_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxTerminateAgreement.Text.Trim()))
            {
                MessageBox.Show(@"Не указана причина расторжения договора", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainPoint.Focus();
                return;
            }
            ClearModifyState();
            textBoxAgreementContent.Text =
                string.Format(CultureInfo.InvariantCulture,
                    "1.1. По настоящему Соглашению Стороны договорились расторгнуть с {3} договор № {0} от {1} {4} найма жилого помещения (далее - договор) по {2}.\r\n" +
                    "1.2. Обязательства, возникшие из указанного договора до момента расторжения, подлежат исполнению в соответствии с указанным договором. Стороны не имеют взаимных претензий по исполнению условий договора № {0} от {1}.",
                    ParentRow["registration_num"],
                    ParentRow["registration_date"] != DBNull.Value ?
                        Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "",
                        textBoxTerminateAgreement.Text.StartsWith("по ") ? textBoxTerminateAgreement.Text.Substring(3).Trim() : textBoxTerminateAgreement.Text.Trim(),
                    dateTimePickerTerminateDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    DataModel.GetInstance<RentTypesDataModel>().Select().Rows.Find(ParentRow["id_rent_type"])["rent_type_genetive"]);
        }

        private void vButtonExplainPaste_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxExplainPoint.Text.Trim()) && string.IsNullOrEmpty(textBoxExplainGeneralPoint.Text.Trim()))
            {
                MessageBox.Show(@"Не указан ни номер пункта, ни номер подпункта", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainPoint.Focus();
                return;
            }
            if (string.IsNullOrEmpty(textBoxExplainContent.Text.Trim()))
            {
                MessageBox.Show(@"Содержание изложения не может быть пустым", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExplainContent.Focus();
                return;
            }
            var contentList = textBoxAgreementContent.Lines.ToList();
            var headersCount = 0;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B"))
                    headersCount++;
            }
            var headerIndex = -1;
            var lastPointIndex = -1;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B.*изложить"))
                {
                    headerIndex = i;
                }
                else
                    if (headerIndex != -1 && Regex.IsMatch(contentList[i],
                        "^(\u200B.*из пункта .+ исключить|\u200B.*пункт .+ дополнить|\u200B.*изложить|\u200B.*расторгнуть|\u200B.*считать.+нанимателем)"))
                    {
                        lastPointIndex = i;
                        break;
                    }
            }
            string point;
            if (!string.IsNullOrEmpty(textBoxExplainPoint.Text.Trim()) &&
                !string.IsNullOrEmpty(textBoxExplainGeneralPoint.Text.Trim()))
            {
                point = string.Format("подпункт {0} пункта {1}", textBoxExplainPoint.Text.Trim(), textBoxExplainGeneralPoint.Text.Trim());
            } else if (!string.IsNullOrEmpty(textBoxExplainPoint.Text.Trim()))
            {
                point = string.Format("подпункт {0}", textBoxExplainPoint.Text.Trim());
            }
            else
            {
                point = string.Format("пункт {0}", textBoxExplainGeneralPoint.Text.Trim());
            }
            var element = string.Format(CultureInfo.InvariantCulture, "{0}. {1}", point, textBoxExplainContent.Text.Trim());
            if (headerIndex == -1)
            {
                contentList.Add(string.Format("\u200B{0}) изложить в новой редакции:", ++headersCount));
            }
            if (lastPointIndex == -1)
                contentList.Add(element);
            else
                contentList.Insert(lastPointIndex, element);
            textBoxAgreementContent.Lines = contentList.ToArray();
        }

        private void vButtonIncludePaste_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxIncludeSNP.Text.Trim()))
            {
                MessageBox.Show(@"Поле ФИО не может быть пустым", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxIncludeSNP.Focus();
                return;
            }
            if (comboBoxIncludeKinship.SelectedValue == null)
            {
                MessageBox.Show(@"Не выбрана родственная связь", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxIncludeKinship.Focus();
                return;
            }
            var contentList = textBoxAgreementContent.Lines.ToList();
            var headersCount = 0;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B"))
                    headersCount++;
            }
            var headerIndex = -1;
            var lastPointIndex = -1;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], string.Format("^\u200B.*пункт {0} договора дополнить", 
                    textBoxGeneralIncludePoint.Text)))
                {
                    headerIndex = i;
                }
                else
                    if (headerIndex != -1 && Regex.IsMatch(contentList[i],
                        "^(\u200B.*из пункта .+ договора исключить|\u200B.*пункт .+ договора дополнить|\u200B.*изложить|\u200B.*расторгнуть|\u200B.*считать.+нанимателем)"))
                    {
                        lastPointIndex = i;
                        break;
                    }
            }

            var kinship = ((DataRowView)comboBoxIncludeKinship.SelectedItem)["kinship"].ToString();
            var element = string.Format(CultureInfo.InvariantCulture, "«{0}. {1}, {2} - {3} г.р.»;", textBoxIncludePoint.Text,
                textBoxIncludeSNP.Text.Trim(),
                kinship,
                dateTimePickerIncludeDateOfBirth.Value.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)); 
            var snp = textBoxIncludeSNP.Text.Trim();
            string sSurname, sName, sPatronymic;
            Declension.GetSNM(snp, out sSurname, out sName, out sPatronymic);
            if (kinship == "наниматель")
            {
                var gender = Declension.GetGender(sPatronymic);
                contentList.Add(string.Format("\u200B{4}) считать по договору № {0} от {1} нанимателем - «{2} - {3} г.р.»;",
                    ParentRow["registration_num"],
                    ParentRow["registration_date"] != DBNull.Value ?
                        Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "",
                        gender == Gender.NotDefind ? snp :
                            Declension.GetSNPDeclension(snp, gender, DeclensionCase.Vinit),
                        dateTimePickerIncludeDateOfBirth.Value.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                        ++headersCount));
            }
            else
            {
                if (headerIndex == -1)
                {
                    contentList.Add(string.Format("\u200B{2}) пункт {0} договора дополнить подпунктом {1} следующего содержания:",
                        textBoxGeneralIncludePoint.Text, textBoxIncludePoint.Text, ++headersCount));
                }
                if (lastPointIndex == -1)
                    contentList.Add(element);
                else
                    contentList.Insert(lastPointIndex, element);
            }
            textBoxAgreementContent.Lines = contentList.ToArray();
            _includePersons.Add(new TenancyPerson
            {
                IdProcess = (int?)ParentRow["id_process"],
                Surname = sSurname,
                Name = sName,
                Patronymic = sPatronymic,
                DateOfBirth = dateTimePickerIncludeDateOfBirth.Value.Date,
                IdKinship = (int?)comboBoxIncludeKinship.SelectedValue
            });
        }

        private void vButtonExcludePaste_Click(object sender, EventArgs e)
        {
            if (_vPersonsExclude.Position == -1)
            {
                MessageBox.Show(@"Не выбран участник найма", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var contentList = textBoxAgreementContent.Lines.ToList();
            var headerIndex = -1;
            var lastPointIndex = -1;
            var headersCount = 0;
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], "^\u200B"))
                    headersCount++;
            }
            for (var i = 0; i < contentList.Count; i++)
            {
                if (Regex.IsMatch(contentList[i], string.Format("^\u200B.*из пункта {0} договора исключить",
                    textBoxGeneralExcludePoint.Text)))
                {
                    headerIndex = i;
                }
                else
                    if (headerIndex != -1 && Regex.IsMatch(contentList[i],
                        "^(\u200B.*из пункта .+ договора исключить|\u200B.*пункт .+ договора дополнить|\u200B.*изложить|\u200B.*расторгнуть|\u200B.*считать.+нанимателем)"))
                    {
                        lastPointIndex = i;
                        break;
                    }
            }
            var tenancyPerson = ((DataRowView)_vPersonsExclude[_vPersonsExclude.Position]);

            var kinship = tenancyPerson["id_kinship"] != DBNull.Value ?
                ((DataRowView)_vKinships[_vKinships.Find("id_kinship", tenancyPerson["id_kinship"])])["kinship"].ToString() : "";
            var element = string.Format(CultureInfo.InvariantCulture, "«{0}. {1} {2} {3} - {4}, {5} г.р.»;", textBoxExcludePoint.Text,
                tenancyPerson["surname"],
                tenancyPerson["name"],
                tenancyPerson["patronymic"],
                kinship,
                tenancyPerson["date_of_birth"] != DBNull.Value ?
                    Convert.ToDateTime(tenancyPerson["date_of_birth"], CultureInfo.InvariantCulture).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "");
            if (headerIndex == -1)
            {
                contentList.Add(string.Format("\u200B{2}) из пункта {0} договора исключить подпункт {1} следующего содержания:",
                    textBoxGeneralExcludePoint.Text, textBoxExcludePoint.Text, ++headersCount));
            }
            if (lastPointIndex == -1)
                contentList.Add(element);
            else
                contentList.Insert(lastPointIndex, element);
            textBoxAgreementContent.Lines = contentList.ToArray();
            _excludePersons.Add(new TenancyPerson
            {
                IdProcess = (int?)ParentRow["id_process"],
                IdPerson = (int?)tenancyPerson["id_person"],
                Surname = tenancyPerson["surname"].ToString(),
                Name = tenancyPerson["name"].ToString(),
                Patronymic = tenancyPerson["patronymic"].ToString(),
                DateOfBirth = (DateTime?)(tenancyPerson["date_of_birth"] == DBNull.Value ? null : tenancyPerson["date_of_birth"]),
                IdKinship = (int?)tenancyPerson["id_kinship"]
            });
        }

        private void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (GeneralBindingSource.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (GeneralBindingSource.Position >= dataGridView.RowCount)
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                else
                    if (dataGridView.Rows[GeneralBindingSource.Position].Selected != true)
                        dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            BindWarrantId();
            if (GeneralBindingSource.Position == -1)
                return;
            if (ViewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            ViewportState = ViewportState.ReadState;
            IsEditable = true;
        }

        private void vButtonChangeTenancy_Click(object sender, EventArgs e)
        {
            if (_vTenantChangeTenant.Count == 0 || _vPersonsChangeTenant.Position == -1)
            {
                MessageBox.Show(@"Не выбран текущий наниматель", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_vPersonsChangeTenant.Position == -1)
            {
                MessageBox.Show(@"Не выбран новый наниматель", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (comboboxTenantChangeKinship.SelectedValue == null && !checkBoxExcludeTenant.Checked)
            {
                MessageBox.Show(@"Не выбрано новое родственное отношение текущего нанимателя или его исключение из найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if ((comboboxTenantChangeKinship.SelectedValue != null && (int)comboboxTenantChangeKinship.SelectedValue == 1) && !checkBoxExcludeTenant.Checked)
            {
                MessageBox.Show(@"Нельзя исключаемому нанимателю указать новую родственную связь «наниматель». Выберите другую родственную связь", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            ClearModifyState();
            var regDateStr = ParentRow["registration_date"] != DBNull.Value
                ? Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture)
                .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
            var registrationNumber = ParentRow["registration_num"];

            textBoxAgreementContent.Text = string.Format(CultureInfo.InvariantCulture,
                "1.1. По настоящему Соглашению Стороны по договору № {0} от {1}, ",
                regDateStr, registrationNumber);
            
            // Исключаем старого нанимателя
            var oldTenantRow = (DataRowView)_vTenantChangeTenant[0];      
            var snp = oldTenantRow["surname"] + @" " + oldTenantRow["name"] + @" " + oldTenantRow["patronymic"];
            var sPatronymic = oldTenantRow["patronymic"].ToString(); 
            var gender = Declension.GetGender(sPatronymic);
            textBoxAgreementContent.Text += string.Format("в связи c ________________________________________ нанимателя «{0}», договорились:",
                gender == Gender.NotDefind ? snp : Declension.GetSNPDeclension(snp, gender, DeclensionCase.Rodit));
            
            // Включаем нового нанимателя
            var newTenantRow = ((DataRowView) _vPersonsChangeTenant[_vPersonsChangeTenant.Position]);
            var newTenantSurname = (string)newTenantRow["surname"];
            var newTenantName = (string)newTenantRow["name"];
            var newTenantPatronymic = (string)newTenantRow["patronymic"];
            var newTenantSnp = newTenantSurname + " " + newTenantName + " " + newTenantPatronymic;
            gender = Declension.GetGender(newTenantPatronymic);
            textBoxAgreementContent.Text += Environment.NewLine +
                string.Format("\u200B1) считать стороной по договору - нанимателем - «{0}, {1}»;",
                gender == Gender.NotDefind ? newTenantSnp : Declension.GetSNPDeclension(newTenantSnp, gender, DeclensionCase.Vinit),
                newTenantRow["date_of_birth"] != DBNull.Value ? ((DateTime)newTenantRow["date_of_birth"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "");


            _modifications.Add(new Dictionary<ExtModificationTypes, List<ExtModificationParameter>>
            {
                { ExtModificationTypes.ChangeTenant, new List<ExtModificationParameter>
                    {
                        new ExtModificationParameter
                        {
                            Name = "ExcludeTenant",
                            Value = checkBoxExcludeTenant.Checked
                        },
                        new ExtModificationParameter
                        {
                            Name = "IdNewTenant",
                            Value = (int)newTenantRow["id_person"]
                        },
                        new ExtModificationParameter
                        {
                            Name = "IdOldTenant",
                            Value = (int?)oldTenantRow["id_person"]
                        },
                        new ExtModificationParameter
                        {
                            Name = "IdKinshipOldTenant",
                            Value = !checkBoxExcludeTenant.Checked && comboboxTenantChangeKinship.SelectedValue != null ? 
                                    (int?)comboboxTenantChangeKinship.SelectedValue : null
                        }
                    } 
                }
            });
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
                return false;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        private void vButtonPaymentInsert_Click(object sender, EventArgs e)
        {
            var add = "";
            var pointCount = textBoxAgreementContent.Lines.Count(v => Regex.IsMatch(v, "^\u200B?[0-9]+\\)"));
            switch ((int)ParentRow["id_rent_type"]) 
            {
                case 3 :
                    add = "\u200B" + (pointCount + 1) + @") изложить подпункт ""з"" пункта ___ в следующей редакции:
«вносить плату:
• за пользование жилым помещением (плата за наем) на расчетный счет Наймодателя в размере, установленном муниципальным правовым актом города Братска в соответствии с Жилищным кодексом Российской Федерации; 
• за содержание и ремонт жилого помещения лицам, осуществляющим соответствующий вид деятельности, в размере установленном муниципальным правовым актом города Братска в соответствии с Жилищным кодексом Российской Федерации;
• за коммунальные услуги лицам, осуществляющим соответствующий вид деятельности, в соответствии с тарифами, установленными в соответствии законодательством Российской Федерации.
Плата за жилое помещение и коммунальные услуги вносится ежемесячно до десятого числа месяца, следующего за истекшим месяцем, если иной срок не установлен договором управления многоквартирным домом либо решением общего собрания членов товарищества собственников жилья, жилищного кооператива или иного специализированного потребительского кооператива, созданного в целях удовлетворения потребностей граждан в жилье в соответствии с федеральным законом о таком кооперативе.
Плата за жилое помещение и коммунальные услуги вносится Нанимателем на основании платежных документов.
В случае невнесения в установленный срок платы за жилое помещение и (или) коммунальные услуги Наниматель уплачивает Наймодателю пени в размере, установленном Жилищным кодексом Российской Федерации, что не освобождает Нанимателя от уплаты причитающихся платежей».";                     
                    break;
                case 1:
                case 2:
                    add = "\u200B" + (pointCount + 1) + @") главу ___ изложить в следующей редакции:
Наниматель вносит плату:
• за пользование жилым помещением (плата за наем) на расчетный счет Наймодателя в размере, установленном решением органа местного самоуправления в соответствии с Жилищным кодексом Российской Федерации;
• за содержание и ремонт жилого помещения лицам, осуществляющим соответствующий вид деятельности, в размере установленном решением органа местного самоуправления, в соответствии с Жилищным кодексом Российской Федерации;
• за коммунальные услуги лицам, осуществляющим соответствующий вид деятельности, в соответствии с тарифами, установленными в соответствии законодательством Российской Федерации.
Плата за жилое помещение и коммунальные услуги вносится ежемесячно до десятого числа месяца, следующего за истекшим месяцем, если иной срок не установлен договором управления многоквартирным домом либо решением общего собрания членов товарищества собственников жилья, жилищного кооператива или иного специализированного потребительского кооператива, созданного в целях удовлетворения потребностей граждан в жилье в соответствии с федеральным законом о таком кооперативе.
Плата за жилое помещение и коммунальные услуги вносится Нанимателем на основании платежных документов.
Наймодатель вправе в одностороннем порядке изменить размер платы за пользование жилым помещением (плата за наем) и за содержание и ремонт жилого помещения в случае внесения изменения в муниципальный правовой акт города Братска, устанавливающего размер платы за жилое помещение.";
                    break;
            }
            textBoxAgreementContent.Text += textBoxAgreementContent.Text.EndsWith("\n") ? add : Environment.NewLine + add;                                       
        }

        private void checkBoxExcludeTenant_CheckStateChanged(object sender, EventArgs e)
        {
            comboboxTenantChangeKinship.Enabled = !checkBoxExcludeTenant.Checked;
        }

        private void vButtonProlongCommercial_Click(object sender, EventArgs e)
        {
            ClearModifyState();
            var regDateStr = ParentRow["registration_date"] != DBNull.Value
                ? Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture)
                .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
            var registrationNumber = ParentRow["registration_num"];

            var rentPeriodStr = "";
            if (dateTimePickerCommercialProlongFrom.Checked)
            {
                rentPeriodStr += "с " + dateTimePickerCommercialProlongFrom.Value.ToString("dd.MM.yyy", CultureInfo.InvariantCulture);
            }
            if (checkBoxCommercialProlongUntilDismissal.Checked)
            {
                if (!string.IsNullOrEmpty(rentPeriodStr))
                    rentPeriodStr += " ";
                rentPeriodStr += "на период трудовых отношений";
            }
            else if (dateTimePickerCommercialProlongTo.Checked)
            {
                if (!string.IsNullOrEmpty(rentPeriodStr))
                    rentPeriodStr += " ";
                rentPeriodStr += "по " + dateTimePickerCommercialProlongTo.Value.ToString("dd.MM.yyy", CultureInfo.InvariantCulture);
            }
            if (!checkBoxCommercialProlongUntilDismissal.Checked)
            {
                rentPeriodStr = "на период " + rentPeriodStr;
            }

            textBoxAgreementContent.Text = string.Format(@"1.1. По настоящему Соглашению на основании личного заявления нанимателя от {3}. Стороны договорились:"+
                "\r\n\u200B1) продлить срок действия  договора  № {0} от {1} {2}."+
                "\r\n\u200B2) пункт {4} исключить.",
                registrationNumber, regDateStr, rentPeriodStr,
                dateTimePickerCommercialProlongRequest.Value.ToString("dd.MM.yyyy"), textBoxCommercialProlongGeneralPoint.Text);

            _modifications.Add(new Dictionary<ExtModificationTypes, List<ExtModificationParameter>>
            {
                { ExtModificationTypes.CommercialProlong, new List<ExtModificationParameter>
                    {
                        new ExtModificationParameter
                        {
                            Name = "ProlongFrom",
                            Value = ViewportHelper.ValueOrNull(dateTimePickerCommercialProlongFrom)
                        },
                        new ExtModificationParameter
                        {
                            Name = "ProlongTo",
                            Value = ViewportHelper.ValueOrNull(dateTimePickerCommercialProlongTo)
                        },
                        new ExtModificationParameter
                        {
                            Name = "UntilDismissal",
                            Value = checkBoxCommercialProlongUntilDismissal.Checked
                        }
                    } 
                }
            });
        }

        private void vButtonProlongSpecial_Click(object sender, EventArgs e)
        {
            ClearModifyState();
            var regDateStr = ParentRow["registration_date"] != DBNull.Value
                ? Convert.ToDateTime(ParentRow["registration_date"], CultureInfo.InvariantCulture)
                .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
            var registrationNumber = ParentRow["registration_num"];
            var rentPeriodStr = "";
            if (dateTimePickerSpecialProlongFrom.Checked)
            {
                rentPeriodStr += "с " + dateTimePickerSpecialProlongFrom.Value.ToString("dd.MM.yyy", CultureInfo.InvariantCulture);
            }
            if (checkBoxSpecialProlongUntilDismissal.Checked)
            {
                if (!string.IsNullOrEmpty(rentPeriodStr))
                    rentPeriodStr += " ";
                rentPeriodStr += "на период трудовых отношений";
            }
            else if (dateTimePickerSpecialProlongTo.Checked)
            {
                if (!string.IsNullOrEmpty(rentPeriodStr))
                    rentPeriodStr += " ";
                rentPeriodStr += "по " + dateTimePickerSpecialProlongTo.Value.ToString("dd.MM.yyy", CultureInfo.InvariantCulture);
            }

            textBoxAgreementContent.Text = string.Format(@"1.1. По настоящему Соглашению Стороны по договору № {0} от {1} договорились:"+
                "\r\n\u200B1) изложить в новой редакции:" +
                "\r\nподпункт {2} пункта {3}: \"Срок найма жилого помещения устанавливается {4}\".",
                regDateStr, registrationNumber, 
                textBoxSpecialProlongPoint.Text, textBoxSpecialProlongGeneralPoint.Text, rentPeriodStr);

            _modifications.Add(new Dictionary<ExtModificationTypes, List<ExtModificationParameter>>
            {
                { ExtModificationTypes.SpecialProlong, new List<ExtModificationParameter>
                    {
                        new ExtModificationParameter
                        {
                            Name = "ProlongFrom",
                            Value = ViewportHelper.ValueOrNull(dateTimePickerSpecialProlongFrom)
                        },
                        new ExtModificationParameter
                        {
                            Name = "ProlongTo",
                            Value = ViewportHelper.ValueOrNull(dateTimePickerSpecialProlongTo)
                        },
                        new ExtModificationParameter
                        {
                            Name = "UntilDismissal",
                            Value = checkBoxSpecialProlongUntilDismissal.Checked
                        }
                    } 
                }
            });
        }

        private void ClearModifyState()
        {
            _includePersons.Clear();
            _excludePersons.Clear();
            _modifications.Clear();
        }

        private void checkBoxSpecialProlongUntilDismissal_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerSpecialProlongTo.Enabled = !checkBoxSpecialProlongUntilDismissal.Checked;
        }

        private void checkBoxCommercialProlongUntilDismissal_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerCommercialProlongTo.Enabled = !checkBoxCommercialProlongUntilDismissal.Checked;
        }
    }
}
