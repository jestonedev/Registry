using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ModalEditors;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyViewport: FormViewport
    {
        #region Models
        private DataModel _tenancyBuildingAssoc;
        private DataModel _tenancyPremisesAssoc;
        private DataModel _tenancySubPremisesAssoc;
        private DataModel _executors;
        private DataModel _rentTypes;
        private DataModel _tenancyAgreements;
        private DataModel _warrants;
        private DataModel _tenancyPersons;
        private DataModel _tenancyReasons;
        private DataModel _kinships;
        private DataModel _rentPeriods;
        private CalcDataModel _tenancyPremisesInfo;
        #endregion Models

        #region Views
        private BindingSource _vExecutors;
        private BindingSource _vRentTypes;
        private BindingSource _vTenancyAgreements;
        private BindingSource _vWarrants;
        private BindingSource _vTenancyPersons;
        private BindingSource _vTenancyAddresses;
        private BindingSource _vTenancyReasons;
        private BindingSource _vKinships;
        private BindingSource _vRentPeriods;
        #endregion Views

        //Forms
        private SearchForm _stExtendedSearchForm;
        private SearchForm _stSimpleSearchForm;
        private SelectWarrantForm _swForm;

        private int? _idWarrant;
        private bool _isCopy;
        private int? _idCopyProcess;

        private TenancyViewport()
            : this(null, null)
        {
        }

        public TenancyViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
        }

        private void FiltersRebuild()
        {
            if (_vTenancyAddresses == null)
                return;
            _vTenancyAddresses.Filter = (GeneralBindingSource.Position >= 0 ? "id_process = 0" + ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"] : "id_process = 0");
        }
       
        private void RebuildStaticFilter()
        {
            IEnumerable<int> ids;
            if (ParentRow == null)
                return;
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    ids = TenancyService.TenancyProcessIDsByBuildingId(Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.Premises:
                    ids = TenancyService.TenancyProcessIDsByPremisesId(Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.SubPremises:
                    ids = TenancyService.TenancyProcessIDsBySubPremisesId(Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture));
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
            if (ids != null)
            {
                StaticFilter = "id_process IN (0";
                foreach (var id in ids)
                    StaticFilter += id.ToString(CultureInfo.InvariantCulture) + ",";
                StaticFilter = StaticFilter.TrimEnd(',') + ")";
            }
            GeneralBindingSource.Filter = StaticFilter;           
        }

        private void SetViewportCaption()
        {
            if (ParentRow == null)
            {
                if (ViewportState == ViewportState.NewRowState)
                    Text = @"Новый найм";
                else
                    if (GeneralBindingSource.Position != -1)
                        Text = string.Format(CultureInfo.InvariantCulture, "Процесс найма №{0}", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"]);
                    else
                        Text = @"Процессы отсутствуют";
            }
            else
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        if (ViewportState == ViewportState.NewRowState)
                            Text = string.Format(CultureInfo.InvariantCulture, "Новый найм здания №{0}", ParentRow["id_building"]);
                        else
                        if (GeneralBindingSource.Position != -1)
                            Text = string.Format(CultureInfo.InvariantCulture, "Найм №{0} здания №{1}", 
                                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], ParentRow["id_building"]);
                        else
                            Text = string.Format(CultureInfo.InvariantCulture, "Наймы здания №{0} отсутствуют", ParentRow["id_building"]);
                        break;
                    case ParentTypeEnum.Premises:
                        if (ViewportState == ViewportState.NewRowState)
                            Text = string.Format(CultureInfo.InvariantCulture, "Новый найм помещения №{0}", ParentRow["id_premises"]);
                        else
                            if (GeneralBindingSource.Position != -1)
                                Text = string.Format(CultureInfo.InvariantCulture, "Найм №{0} помещения №{1}",
                                    ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], ParentRow["id_premises"]);
                            else
                                Text = string.Format(CultureInfo.InvariantCulture, "Наймы помещения №{0} отсутствуют", ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.SubPremises:
                        if (ViewportState == ViewportState.NewRowState)
                            Text = string.Format(CultureInfo.InvariantCulture, "Новый найм комнаты №{0}", ParentRow["id_sub_premises"]);
                        else
                            if (GeneralBindingSource.Position != -1)
                                Text = string.Format(CultureInfo.InvariantCulture, "Найм №{0} комнаты №{1}",
                                    ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], ParentRow["id_sub_premises"]);
                            else
                                Text = string.Format(CultureInfo.InvariantCulture, "Наймы комнаты №{0} отсутствуют", ParentRow["id_sub_premises"]);
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
        }

        private void RedrawDataGridRows()
        {
            if (dataGridViewTenancyPersons.Rows.Count == 0)
                return;
            for (var i = 0; i < dataGridViewTenancyPersons.Rows.Count; i++)
                if (((DataRowView)_vTenancyPersons[i])["id_kinship"] != DBNull.Value &&
                    Convert.ToInt32(((DataRowView)_vTenancyPersons[i])["id_kinship"], CultureInfo.InvariantCulture) == 1 &&
                    ((DataRowView)_vTenancyPersons[i])["exclude_date"] == DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else
                    if (((DataRowView)_vTenancyPersons[i])["exclude_date"] != DBNull.Value)
                        dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
                    else
                        dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.White;
        }

        private string WarrantStringById(int idWarrant)
        {
            if (_vWarrants.Position == -1)
                return null;
            else
            {
                var rowIndex = _vWarrants.Find("id_warrant", idWarrant);
                if (rowIndex == -1)
                    return null;
                var registrationDate = Convert.ToDateTime(((DataRowView)_vWarrants[rowIndex])["registration_date"], CultureInfo.InvariantCulture);
                var registrationNum = ((DataRowView)_vWarrants[rowIndex])["registration_num"].ToString();
                return string.Format(CultureInfo.InvariantCulture, "№ {0} от {1}", 
                    registrationNum, registrationDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            }
        }

        private void BindWarrantId()
        {
            if ((GeneralBindingSource.Position > -1) && ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_warrant"] != DBNull.Value)
            {
                _idWarrant = Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_warrant"], CultureInfo.InvariantCulture);
                textBoxSelectedWarrant.Text =
                    WarrantStringById(_idWarrant.Value);
                vButtonWarrant.Text = @"x";
            }
            else
            {
                _idWarrant = null;
                textBoxSelectedWarrant.Text = "";
                vButtonWarrant.Text = @"...";
            }
        }

        private void UnbindedCheckBoxesUpdate()
        {
            if (GeneralBindingSource.Count == 0) return;
            var row = (GeneralBindingSource.Position >= 0) ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            checkBoxContractEnable.Checked = (GeneralBindingSource.Position >= 0) && (row != null) &&
                (row["registration_date"] != DBNull.Value) && (row["registration_num"] != DBNull.Value);
            checkBoxProtocolEnable.Checked = (GeneralBindingSource.Position >= 0) && (row != null) && (row["protocol_date"] != DBNull.Value) && (row["protocol_num"] != DBNull.Value);
            if ((GeneralBindingSource.Position >= 0) && (row != null) && (row["issue_date"] != DBNull.Value))
                dateTimePickerIssueDate.Checked = true;
            else
            {
                dateTimePickerIssueDate.Value = DateTime.Now.Date;
                dateTimePickerIssueDate.Checked = false;
            }
            if ((GeneralBindingSource.Position >= 0) && (row != null) && (row["begin_date"] != DBNull.Value))
                dateTimePickerBeginDate.Checked = true;
            else
            {
                dateTimePickerBeginDate.Value = DateTime.Now.Date;
                dateTimePickerBeginDate.Checked = false;
            }
            if ((GeneralBindingSource.Position >= 0) && (row != null) && (row["end_date"] != DBNull.Value))
                dateTimePickerEndDate.Checked = true;
            else
            {
                dateTimePickerEndDate.Value = DateTime.Now.Date;
                dateTimePickerEndDate.Checked = false;
            }
        }

        private void DataBind()
        {
            comboBoxRentType.DataSource = _vRentTypes;
            comboBoxRentType.ValueMember = "id_rent_type";
            comboBoxRentType.DisplayMember = "rent_type";
            comboBoxRentType.DataBindings.Clear();
            comboBoxRentType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_rent_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxRegistrationNumber.DataBindings.Clear();
            textBoxRegistrationNumber.DataBindings.Add("Text", GeneralBindingSource, "registration_num", true, DataSourceUpdateMode.Never, "");

            dateTimePickerRegistrationDate.DataBindings.Clear();
            dateTimePickerRegistrationDate.DataBindings.Add("Value", GeneralBindingSource, "registration_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            dateTimePickerIssueDate.DataBindings.Clear();
            dateTimePickerIssueDate.DataBindings.Add("Value", GeneralBindingSource, "issue_date", true, DataSourceUpdateMode.Never, null);

            dateTimePickerBeginDate.DataBindings.Clear();
            dateTimePickerBeginDate.DataBindings.Add("Value", GeneralBindingSource, "begin_date", true, DataSourceUpdateMode.Never, null);

            dateTimePickerEndDate.DataBindings.Clear();
            dateTimePickerEndDate.DataBindings.Add("Value", GeneralBindingSource, "end_date", true, DataSourceUpdateMode.Never, null);

            dataGridViewTenancyPersons.DataSource = _vTenancyPersons;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            date_of_birth.DataPropertyName = "date_of_birth";
            id_kinship.DataSource = _vKinships;
            id_kinship.ValueMember = "id_kinship";
            id_kinship.DisplayMember = "kinship";
            id_kinship.DataPropertyName = "id_kinship";

            dataGridViewTenancyAgreements.DataSource = _vTenancyAgreements;
            agreement_date.DataPropertyName = "agreement_date";
            agreement_content.DataPropertyName = "agreement_content";

            dataGridViewTenancyReasons.DataSource = _vTenancyReasons;
            reason_date.DataPropertyName = "reason_date";
            reason_number.DataPropertyName = "reason_number";
            reason_prepared.DataPropertyName = "reason_prepared";

            dataGridViewTenancyAddress.DataSource = _vTenancyAddresses;
            address.DataPropertyName = "address";
            total_area.DataPropertyName = "total_area";
            living_area.DataPropertyName = "living_area";
            rent_area.DataPropertyName = "rent_area";

            dataGridViewRentPeriods.DataSource = _vRentPeriods;
            rent_periods_begin_date.DataPropertyName = "begin_date";
            rent_periods_end_date.DataPropertyName = "end_date";
            rent_periods_until_dismissal.DataPropertyName = "until_dismissal";

            textBoxProtocolNumber.DataBindings.Clear();
            textBoxProtocolNumber.DataBindings.Add("Text", GeneralBindingSource, "protocol_num", true, DataSourceUpdateMode.Never, "");

            dateTimePickerProtocolDate.DataBindings.Clear();
            dateTimePickerProtocolDate.DataBindings.Add("Value", GeneralBindingSource, "protocol_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");

            comboBoxExecutor.DataSource = _vExecutors;
            comboBoxExecutor.ValueMember = "id_executor";
            comboBoxExecutor.DisplayMember = "executor_name";
            comboBoxExecutor.DataBindings.Clear();
            comboBoxExecutor.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_executor", true, DataSourceUpdateMode.Never, DBNull.Value);

            checkBoxUntilDismissal.DataBindings.Clear();
            checkBoxUntilDismissal.DataBindings.Add("Checked", GeneralBindingSource, "until_dismissal", true,
                DataSourceUpdateMode.Never, DBNull.Value);
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.TenancyWrite)) 
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidateTenancy(TenancyProcess tenancy)
        {
            if (tenancy.IdRentType == null)
            {
                MessageBox.Show(@"Необходимо выбрать тип найма", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxRentType.Focus();
                return false;
            }
            if (checkBoxContractEnable.Checked)
            {
                if (tenancy.RegistrationNum == null)
                {
                    MessageBox.Show(@"Не указан номер договора найма", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    textBoxRegistrationNumber.Focus();
                    return false;
                }
            }
            if (checkBoxProtocolEnable.Checked)
            {
                if (tenancy.ProtocolNum == null)
                {
                    MessageBox.Show(@"Не указан номер протокола жилищной комиссии", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    textBoxProtocolNumber.Focus();
                    return false;
                }
            }
            var tenancyFromView = (TenancyProcess) EntityFromView();
            if (tenancy.RegistrationNum != null && tenancy.RegistrationNum != tenancyFromView.RegistrationNum)
                if (TenancyService.TenancyProcessesDuplicateCount(tenancy) != 0 &&
                    MessageBox.Show(@"В базе уже имеется договор с таким номером. Все равно продолжить сохранение?", @"Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
            // Проверить соответствие вида найма
            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        if (!ViewportHelper.BuildingFundAndRentMatch((int)ParentRow["id_building"], tenancy.IdRentType.Value) &&
                            MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемого здания. Все равно продолжить сохранение?",
                            @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                            return false;
                        break;
                    case ParentTypeEnum.Premises:
                        if (!ViewportHelper.PremiseFundAndRentMatch((int)ParentRow["id_premises"], tenancy.IdRentType.Value))
                        {
                            if (!ViewportHelper.BuildingFundAndRentMatch((int)ParentRow["id_building"], tenancy.IdRentType.Value) &&
                            MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемого помещения. Все равно продолжить сохранение?",
                            @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                                return false;
                        }
                        break;
                    case ParentTypeEnum.SubPremises:
                        if (!ViewportHelper.SubPremiseFundAndRentMatch((int)ParentRow["id_sub_premises"], tenancy.IdRentType.Value))
                        {
                            if (!ViewportHelper.PremiseFundAndRentMatch((int)ParentRow["id_premises"], tenancy.IdRentType.Value))
                            {
                                var idBuilding = (int)EntityDataModel<Premise>.GetInstance().Select().Rows.Find((int)ParentRow["id_premises"])["id_building"];
                                if (!ViewportHelper.BuildingFundAndRentMatch(idBuilding, tenancy.IdRentType.Value) &&
                                    MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемой комнаты. Все равно продолжить сохранение?",
                                    @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                                        return false;
                            }
                        }
                        break;
                }
            }
            return true;
        }

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return EntityConverter<TenancyProcess>.FromRow(row);
        }

        protected override Entity EntityFromViewport()
        {
            var tenancy = new TenancyProcess();
            if (GeneralBindingSource.Position == -1)
                tenancy.IdProcess = null;
            else
                tenancy.IdProcess = ViewportHelper.ValueOrNull<int>((DataRowView)GeneralBindingSource[GeneralBindingSource.Position], "id_process"); 
            tenancy.IdRentType = ViewportHelper.ValueOrNull<int>(comboBoxRentType);
            tenancy.IdExecutor = ViewportHelper.ValueOrNull<int>(comboBoxExecutor);
            tenancy.Description = ViewportHelper.ValueOrNull(textBoxDescription);
            if (checkBoxContractEnable.Checked)
            {
                tenancy.IdWarrant = _idWarrant;
                tenancy.RegistrationNum = ViewportHelper.ValueOrNull(textBoxRegistrationNumber);
                tenancy.RegistrationDate = dateTimePickerRegistrationDate.Value.Date;
                tenancy.IssueDate = ViewportHelper.ValueOrNull(dateTimePickerIssueDate);
                tenancy.BeginDate = ViewportHelper.ValueOrNull(dateTimePickerBeginDate);
                if (checkBoxUntilDismissal.Checked)
                {
                    tenancy.EndDate = null;
                    tenancy.UntilDismissal = true;
                }
                else
                {
                    tenancy.EndDate = ViewportHelper.ValueOrNull(dateTimePickerEndDate);
                    tenancy.UntilDismissal = false;
                }
            }
            else
            {
                tenancy.IdWarrant = null;
                tenancy.RegistrationNum = null;
                tenancy.RegistrationDate = null;
                tenancy.IssueDate = null;
                tenancy.BeginDate = null;
                tenancy.EndDate = null;
                tenancy.UntilDismissal = false;
            }
            // Отклики из прошлого, раньше была возможность менять ордер на вкладке процесса найма, убрано из-за плохой согласованности с основаниями найма
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            if (ViewportState != ViewportState.NewRowState)
            {
                var reasons =
                    (from reasonRow in EntityDataModel<TenancyReason>.GetInstance().FilterDeletedRows() 
                        where reasonRow.Field<int>("id_process") == (int) row["id_process"] &&
                              reasonRow.Field<string>("reason_prepared").ToUpper().Contains("ОРДЕР")
                        select new
                        {
                            number = reasonRow.Field<string>("reason_number"),
                            date = reasonRow.Field<DateTime?>("reason_date")
                        });

                var reasonsList = reasons.ToList();
                if (reasonsList.Any())
                {
                    tenancy.ResidenceWarrantNum = reasonsList.First().number;
                    tenancy.ResidenceWarrantDate = reasonsList.First().date;
                }
            }
            //
            if (checkBoxProtocolEnable.Checked)
            {
                tenancy.ProtocolNum = ViewportHelper.ValueOrNull(textBoxProtocolNumber);
                tenancy.ProtocolDate = dateTimePickerProtocolDate.Value.Date;
            }
            else
            {
                tenancy.ProtocolNum = null;
                tenancy.ProtocolDate = null;
            }
            return tenancy;
        }

        private void ViewportFromTenancy(TenancyProcess tenancy)
        {
            comboBoxRentType.SelectedValue = ViewportHelper.ValueOrDbNull(tenancy.IdRentType);
            comboBoxExecutor.SelectedValue = ViewportHelper.ValueOrDbNull(tenancy.IdExecutor);
            textBoxRegistrationNumber.Text = tenancy.RegistrationNum;
            dateTimePickerRegistrationDate.Value = ViewportHelper.ValueOrDefault(tenancy.RegistrationDate);
            dateTimePickerIssueDate.Value = ViewportHelper.ValueOrDefault(tenancy.IssueDate);
            dateTimePickerIssueDate.Checked = (tenancy.IssueDate != null);
            dateTimePickerBeginDate.Value = ViewportHelper.ValueOrDefault(tenancy.BeginDate);
            dateTimePickerBeginDate.Checked = (tenancy.BeginDate != null);
            dateTimePickerEndDate.Value = ViewportHelper.ValueOrDefault(tenancy.EndDate);
            dateTimePickerEndDate.Checked = (tenancy.EndDate != null) && (tenancy.UntilDismissal != true);
            checkBoxUntilDismissal.Checked = tenancy.UntilDismissal != null && tenancy.UntilDismissal.Value;
            textBoxProtocolNumber.Text = tenancy.ProtocolNum;
            dateTimePickerProtocolDate.Value = ViewportHelper.ValueOrDefault(tenancy.ProtocolDate);
            textBoxDescription.Text = tenancy.Description;
            if (tenancy.IdWarrant != null)
            {
                textBoxSelectedWarrant.Text = WarrantStringById(tenancy.IdWarrant.Value);
                _idWarrant = tenancy.IdWarrant;
            }
            else
            {
                textBoxSelectedWarrant.Text = "";
                _idWarrant = null;
            }
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridViewTenancyAgreements.AutoGenerateColumns = false;
            dataGridViewTenancyAddress.AutoGenerateColumns = false;
            dataGridViewTenancyPersons.AutoGenerateColumns = false;
            dataGridViewTenancyReasons.AutoGenerateColumns = false;
            dataGridViewRentPeriods.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = EntityDataModel<TenancyProcess>.GetInstance();
            _executors = DataModel.GetInstance<EntityDataModel<Executor>>();
            _rentTypes = DataModel.GetInstance<RentTypesDataModel>();
            _tenancyAgreements = EntityDataModel<TenancyAgreement>.GetInstance();
            _warrants = EntityDataModel<Warrant>.GetInstance();
            _tenancyPersons = EntityDataModel<TenancyPerson>.GetInstance();
            _tenancyReasons = EntityDataModel<TenancyReason>.GetInstance();
            _kinships = DataModel.GetInstance<KinshipsDataModel>();
            _rentPeriods = EntityDataModel<TenancyRentPeriod>.GetInstance();
            _tenancyPremisesInfo = CalcDataModel.GetInstance<CalcDataModelTenancyPremisesInfo>();

            //Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            _executors.Select();
            _rentTypes.Select();
            _tenancyAgreements.Select();
            _warrants.Select();
            _tenancyPersons.Select();
            _tenancyReasons.Select();
            _kinships.Select();
            _rentPeriods.Select();

            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", v_tenancies_CurrentItemChanged);
            GeneralBindingSource.DataMember = "tenancy_processes";
            GeneralBindingSource.DataSource = DataStorage.DataSet;
            RebuildStaticFilter();
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;

            _vExecutors = new BindingSource
            {
                DataSource = DataStorage.DataSet,
                DataMember = "executors",
                Filter = "is_inactive = 0"
            };

            _vRentTypes = new BindingSource
            {
                DataSource = DataStorage.DataSet,
                DataMember = "rent_types"
            };

            _vKinships = new BindingSource
            {
                DataSource = DataStorage.DataSet,
                DataMember = "kinships"
            };

            _vWarrants = new BindingSource
            {
                DataSource = DataStorage.DataSet,
                DataMember = "warrants"
            };

            _vTenancyPersons = new BindingSource
            {
                DataMember = "tenancy_processes_tenancy_persons",
                DataSource = GeneralBindingSource
            };

            _vTenancyAgreements = new BindingSource
            {
                DataMember = "tenancy_processes_tenancy_agreements",
                DataSource = GeneralBindingSource
            };

            _vRentPeriods = new BindingSource
            {
                DataMember = "tenancy_processes_tenancy_rent_periods_history",
                DataSource = GeneralBindingSource,
                Sort = "begin_date DESC"
            };

            _vTenancyReasons = new BindingSource
            {
                DataMember = "tenancy_processes_tenancy_reasons",
                DataSource = GeneralBindingSource
            };

            _vTenancyAddresses = new BindingSource
            {
                DataSource = _tenancyPremisesInfo.Select()
            };

            DataBind();

            AddEventHandler<DataRowChangeEventArgs>(_tenancyPersons.Select(), "RowChanged", TenancyPersons_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(_tenancyPersons.Select(), "RowDeleted", TenancyPersons_RowDeleted);

            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", TenancyViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", TenancyViewport_RowDeleted);

            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        _tenancyBuildingAssoc = EntityDataModel<TenancyBuildingAssoc>.GetInstance();
                        AddEventHandler<DataRowChangeEventArgs>(_tenancyBuildingAssoc.Select(), "RowChanged", TenancyAssocViewport_RowChanged);
                        AddEventHandler<DataRowChangeEventArgs>(_tenancyBuildingAssoc.Select(), "RowDeleted", TenancyAssocViewport_RowDeleted);
                        break;
                    case ParentTypeEnum.Premises:
                        _tenancyPremisesAssoc = EntityDataModel<TenancyPremisesAssoc>.GetInstance();
                        AddEventHandler<DataRowChangeEventArgs>(_tenancyPremisesAssoc.Select(), "RowChanged", TenancyAssocViewport_RowChanged);
                        AddEventHandler<DataRowChangeEventArgs>(_tenancyPremisesAssoc.Select(), "RowDeleted", TenancyAssocViewport_RowDeleted);
                        break;
                    case ParentTypeEnum.SubPremises:
                        _tenancySubPremisesAssoc = EntityDataModel<TenancySubPremisesAssoc>.GetInstance();
                        AddEventHandler<DataRowChangeEventArgs>(_tenancySubPremisesAssoc.Select(), "RowChanged", TenancyAssocViewport_RowChanged);
                        AddEventHandler<DataRowChangeEventArgs>(_tenancySubPremisesAssoc.Select(), "RowDeleted", TenancyAssocViewport_RowDeleted);
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }

            AddEventHandler<ListChangedEventArgs>(_vTenancyPersons, "ListChanged", v_persons_ListChanged);

            AddEventHandler<EventArgs>(_tenancyPremisesInfo, "RefreshEvent", tenancy_premises_info_RefreshEvent);

            FiltersRebuild();
            DataChangeHandlersInit();
        }

        private void tenancy_premises_info_RefreshEvent(object sender, EventArgs e)
        {
            // Обновляем информацию по помещениям (живое обновление не реализуемо)
            if (_vTenancyAddresses != null)
            {
                _vTenancyAddresses.DataSource =
                    _tenancyPremisesInfo.Select();
                FiltersRebuild();
            }
        }

        public override bool CanSearchRecord()
        {
            return true;
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
            var index = _vExecutors.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)_vExecutors[index])["id_executor"];
            _isCopy = false;
            _idCopyProcess = null;
            IsEditable = true;
            GeneralDataModel.EditingNewRecord = true;
            UnbindedCheckBoxesUpdate();
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
            var tenancy = (TenancyProcess) EntityFromView();
            GeneralBindingSource.AddNew();
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromTenancy(tenancy);
            checkBoxContractEnable.Checked = (tenancy.RegistrationDate != null) || (tenancy.RegistrationNum != null);
            checkBoxProtocolEnable.Checked = (tenancy.ProtocolDate != null);
            dateTimePickerIssueDate.Checked = (tenancy.IssueDate != null);
            dateTimePickerBeginDate.Checked = (tenancy.BeginDate != null);
            dateTimePickerEndDate.Checked = (tenancy.EndDate != null);
            var index = _vExecutors.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)_vExecutors[index])["id_executor"];
            _isCopy = true;
            _idCopyProcess = tenancy.IdProcess;
            IsEditable = true;
        }

        public override bool SearchedRecords()
        {
            if (!string.IsNullOrEmpty(DynamicFilter))
                return true;
            else
                return false;
        }

        public override void SearchRecord(SearchFormType searchFormType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            switch (searchFormType)
            {
                case SearchFormType.SimpleSearchForm:
                    if (_stSimpleSearchForm == null)
                        _stSimpleSearchForm = new SimpleSearchTenancyForm();
                    if (_stSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _stSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (_stExtendedSearchForm == null)
                        _stExtendedSearchForm = new ExtendedSearchTenancyForm();
                    if (_stExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = _stExtendedSearchForm.GetFilter();
                    break;
            }
            var filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            IsEditable = false;
            GeneralBindingSource.Filter = filter;
            IsEditable = true;
        }

        public override void ClearSearch()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            IsEditable = false;
            GeneralBindingSource.Filter = StaticFilter;
            IsEditable = true;
            DynamicFilter = "";
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить этот процесс найма?", @"Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_process"]) == -1)
                    return;
                IsEditable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                IsEditable = true;
                ViewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
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
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                    }
                    else
                        Text = @"Процессы отсутствуют";
                    ViewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    BindWarrantId();
                    ViewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            _isCopy = false;
            _idCopyProcess = null;
            IsEditable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            var tenancy = (TenancyProcess) EntityFromViewport();
            if (!ValidateTenancy(tenancy))
                return;
            var filter = "";
            if (!string.IsNullOrEmpty(GeneralBindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idProcess = GeneralDataModel.Insert(tenancy);
                    if (idProcess == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    tenancy.IdProcess = idProcess;
                    IsEditable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", tenancy.IdProcess);
                    GeneralBindingSource.Filter += filter;
                    EntityConverter<TenancyProcess>.FillRow(tenancy, newRow);
                    // Если производится копирование, а не создание новой записи, то надо скопировать участников найма и нанимаемое жилье
                    if (_isCopy && _idCopyProcess != null)
                    {
                        if (!CopyTenancyProcessRelData(idProcess, _idCopyProcess.Value))
                            MessageBox.Show(@"Произошла ошибка во время копирования данных", @"Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                    else
                        if (ParentRow != null)
                        {
                            int idAssoc;
                            switch (ParentType)
                            {
                                case ParentTypeEnum.Building:
                                    var tenancyBuildings = EntityDataModel<TenancyBuildingAssoc>.GetInstance();
                                    var tBuilding = new TenancyBuildingAssoc
                                    {
                                        IdProcess = idProcess,
                                        IdBuilding = Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture),
                                        RentLivingArea = null,
                                        RentTotalArea = null
                                    };
                                    tenancyBuildings.EditingNewRecord = true;
                                    idAssoc = tenancyBuildings.Insert(tBuilding);
                                    if (idAssoc == -1)
                                        return;
                                    tBuilding.IdAssoc = idAssoc;
                                    tenancyBuildings.Select().Rows.Add(idAssoc, tBuilding.IdBuilding,
                                        tBuilding.IdProcess, tBuilding.RentTotalArea, tBuilding.RentLivingArea, 0);
                                    tenancyBuildings.EditingNewRecord = false;
                                    break;
                                case ParentTypeEnum.Premises:
                                    var tenancyPremises = EntityDataModel<TenancyPremisesAssoc>.GetInstance();
                                    var tPremises = new TenancyPremisesAssoc
                                    {
                                        IdProcess = idProcess,
                                        IdPremises = Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture),
                                        RentLivingArea = null,
                                        RentTotalArea = null
                                    };
                                    tenancyPremises.EditingNewRecord = true;
                                    idAssoc = tenancyPremises.Insert(tPremises);
                                    if (idAssoc == -1)
                                        return;
                                    tPremises.IdAssoc = idAssoc;
                                    tenancyPremises.Select().Rows.Add(idAssoc, tPremises.IdPremises,
                                        tPremises.IdProcess, tPremises.RentTotalArea, tPremises.RentLivingArea, 0);
                                    tenancyPremises.EditingNewRecord = false;
                                    break;
                                case ParentTypeEnum.SubPremises:
                                    var tenancySubPremises = EntityDataModel<TenancySubPremisesAssoc>.GetInstance();
                                    var tSubPremises = new TenancySubPremisesAssoc
                                    {
                                        IdProcess = idProcess,
                                        IdSubPremises = Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture),
                                        RentTotalArea = null
                                    };
                                    tenancySubPremises.EditingNewRecord = true;
                                    idAssoc = tenancySubPremises.Insert(tSubPremises);
                                    if (idAssoc == -1)
                                        return;
                                    tSubPremises.IdAssoc = idAssoc;
                                    tenancySubPremises.Select().Rows.Add(idAssoc, tSubPremises.IdSubPremises,
                                        tSubPremises.IdProcess, tSubPremises.RentTotalArea, 0);
                                    tenancySubPremises.EditingNewRecord = false;
                                    break;
                                default: throw new ViewportException("Неизвестный тип родительского объекта");
                            }
                        }
                    // Обновляем информацию по помещениям (живое обновление не реализуемо)
                    if (_vTenancyAddresses != null)
                    {
                        _vTenancyAddresses.DataSource = CalcDataModel.GetInstance<CalcDataModelTenancyPremisesInfo>();
                        FiltersRebuild();
                    }
                    GeneralDataModel.EditingNewRecord = false;
                    RebuildStaticFilter();
                    GeneralBindingSource.Position = GeneralBindingSource.Count - 1;
                    break;
                case ViewportState.ModifyRowState:
                    if (tenancy.IdProcess == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить процесс найма без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(tenancy) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    IsEditable = false;
                    filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", tenancy.IdProcess);
                    GeneralBindingSource.Filter += filter;
                    EntityConverter<TenancyProcess>.FillRow(tenancy, row);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        // Метод копирует зависимые данные по процессу найма
        private bool CopyTenancyProcessRelData(int idNewProcess, int idCopyProcess)
        {
            var persons = from personsRow in _tenancyPersons.FilterDeletedRows()
                          where personsRow.Field<int>("id_process") == idCopyProcess
                          select personsRow;
            _tenancyPersons.EditingNewRecord = true;
            foreach (var personRow in persons.ToList())
            {
                var person = EntityConverter<TenancyPerson>.FromRow(personRow);
                person.IdProcess = idNewProcess;
                var idPerson = _tenancyPersons.Insert(person);
                if (idPerson == -1)
                {
                    _tenancyPersons.EditingNewRecord = false;
                    return false;
                }
                person.IdPerson = idPerson;
                var personBinding = new BindingSource
                {
                    DataSource = DataStorage.DataSet,
                    DataMember = "tenancy_persons"
                };
                var newPersonRow = (DataRowView)personBinding.AddNew();
                EntityConverter<TenancyPerson>.FillRow(person, newPersonRow);
            }
            _tenancyPersons.EditingNewRecord = false;
            var tenancyBuildingsAssoc = EntityDataModel<TenancyBuildingAssoc>.GetInstance();
            var buildings = from row in tenancyBuildingsAssoc.FilterDeletedRows()
                            where row.Field<int>("id_process") == idCopyProcess
                            select row;
            tenancyBuildingsAssoc.EditingNewRecord = true;
            foreach (var row in buildings.ToList())
            {
                var obj = new TenancyBuildingAssoc
                {
                    IdBuilding = row.Field<int?>("id_building"),
                    IdProcess = idNewProcess,
                    RentLivingArea = row.Field<double?>("rent_living_area"),
                    RentTotalArea = row.Field<double?>("rent_total_area")
                };
                var idAssoc = tenancyBuildingsAssoc.Insert(obj);
                if (idAssoc == -1)
                {
                    tenancyBuildingsAssoc.EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = idAssoc;
                tenancyBuildingsAssoc.Select().Rows.Add(obj.IdAssoc, obj.IdBuilding, obj.IdProcess,
                    obj.RentTotalArea, obj.RentLivingArea);
            }
            tenancyBuildingsAssoc.EditingNewRecord = false;
            var tenancyPremisesAssoc = EntityDataModel<TenancyPremisesAssoc>.GetInstance();
            var premises = from row in tenancyPremisesAssoc.FilterDeletedRows()
                            where row.Field<int>("id_process") == idCopyProcess
                           select row;
            tenancyPremisesAssoc.EditingNewRecord = true;
            foreach (var row in premises.ToList())
            {
                var obj = new TenancyPremisesAssoc
                {
                    IdPremises = row.Field<int?>("id_premises"),
                    IdProcess = idNewProcess,
                    RentLivingArea = row.Field<double?>("rent_living_area"),
                    RentTotalArea = row.Field<double?>("rent_total_area")
                };
                var idAssoc = tenancyPremisesAssoc.Insert(obj);
                if (idAssoc == -1)
                {
                    tenancyPremisesAssoc.EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = idAssoc;
                tenancyPremisesAssoc.Select().Rows.Add(obj.IdAssoc, obj.IdPremises, obj.IdProcess,
                    obj.RentTotalArea, obj.RentLivingArea);
            }
            tenancyPremisesAssoc.EditingNewRecord = false;
            var tenancySubPremisesAssoc = EntityDataModel<TenancySubPremisesAssoc>.GetInstance();
            var subPremises = from row in tenancySubPremisesAssoc.FilterDeletedRows()
                           where row.Field<int>("id_process") == idCopyProcess
                               select row;
            tenancySubPremisesAssoc.EditingNewRecord = true;
            foreach (var row in subPremises.ToList())
            {
                var obj = new TenancySubPremisesAssoc
                {
                    IdSubPremises = row.Field<int?>("id_sub_premises"),
                    IdProcess = idNewProcess,
                    RentTotalArea = row.Field<double?>("rent_total_area")
                };
                var idAssoc = tenancySubPremisesAssoc.Insert(obj);
                if (idAssoc == -1)
                {
                    tenancySubPremisesAssoc.EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = idAssoc;
                tenancySubPremisesAssoc.Select().Rows.Add(obj.IdAssoc, obj.IdSubPremises, obj.IdProcess,
                    obj.RentTotalArea);
            }
            tenancySubPremisesAssoc.EditingNewRecord = false;
            return true;
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.TenancyPersonsViewport,
                ViewportType.TenancyReasonsViewport,
                ViewportType.TenancyBuildingsViewport,
                ViewportType.TenancyPremisesViewport,
                ViewportType.TenancyAgreementsViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && (GeneralBindingSource.Position > -1);
        }

        public override void ShowAssocViewport<T>()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбран процесс найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, 
                "id_process = " + Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], CultureInfo.InvariantCulture),
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Row,
                ParentTypeEnum.Tenancy);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            if (GeneralBindingSource.Position == -1 || GeneralBindingSource.Count <= GeneralBindingSource.Position)
                return false;
            var idProcess = ((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_process"] != DBNull.Value
                ? (int?)Convert.ToInt32(((DataRowView) GeneralBindingSource[GeneralBindingSource.Position])["id_process"],
                    CultureInfo.InvariantCulture) : null;
            var idRentType = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_rent_type"] != DBNull.Value
                ? (int?)Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_rent_type"],
                    CultureInfo.InvariantCulture) : null;
            switch (reporterType)
            {
                case ReporterType.TenancyContractCommercialReporter:
                    return idProcess != null && idRentType == 1;
                case ReporterType.TenancyContractSocialReporter:
                    return idProcess != null && idRentType == 3;
                case ReporterType.TenancyContractSpecial1711Reporter:
                case ReporterType.TenancyContractSpecial1712Reporter:
                    return idProcess != null && idRentType == 2;
                case ReporterType.TenancyActReporter:
                case ReporterType.TenancyNotifyContractAgreement:
                    return idProcess != null;
                case ReporterType.TenancyAgreementReporter:
                    return idProcess != null && (TenancyService.TenancyAgreementsForProcess(idProcess.Value) > 0);
            }
            return false;
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate())
                return;
            var reporter = ReporterFactory.CreateReporter(reporterType);
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.TenancyContractCommercialReporter:
                case ReporterType.TenancyContractSocialReporter:
                case ReporterType.TenancyContractSpecial1711Reporter:
                case ReporterType.TenancyContractSpecial1712Reporter:
                    arguments = TenancyContractReporterArguments();
                    break;
                case ReporterType.TenancyActReporter:
                    arguments = TenancyActReporterArguments();
                    break;
                case ReporterType.TenancyNotifyContractAgreement:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "1");
                    break;
                case ReporterType.TenancyAgreementReporter:
                    if (_vTenancyAgreements.Position == -1)
                    {
                        MessageBox.Show(@"Не выбрано соглашение для печати",
                            @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    arguments = TenancyAgreementReporterArguments();
                    break;
            }
            reporter.Run(arguments);
        }
        private Dictionary<string, string> TenancyContractReporterArguments()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return new Dictionary<string, string> { { "id_process", row["id_process"].ToString() } };
        }

        private Dictionary<string, string> TenancyActReporterArguments()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return new Dictionary<string, string> {{"id_process", row["id_process"].ToString()}};
        }

        private Dictionary<string, string> TenancyAgreementReporterArguments()
        {
            var row = (DataRowView)_vTenancyAgreements[_vTenancyAgreements.Position];
            return new Dictionary<string, string> {{"id_agreement", row["id_agreement"].ToString()}};
        }

        private bool TenancyValidForReportGenerate()
        {
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (GeneralBindingSource.Position == -1)
                return false;
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            if (!TenancyService.TenancyProcessHasTenant(Convert.ToInt32(row["id_process"], CultureInfo.InvariantCulture)))
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо указать нанимателя процесса найма", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewportHelper.ValueOrNull<DateTime>(row, "registration_date") == null || ViewportHelper.ValueOrNull(row, "registration_num") == null)
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо завести договор найма и указать его номер и дату регистрации", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            checkBoxProtocolEnable.Focus();
            base.OnVisibleChanged(e);
        }

        void v_persons_ListChanged(object sender, ListChangedEventArgs e)
        {
            RedrawDataGridRows();
        }

        void v_tenancies_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            FiltersRebuild();
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
                MenuCallback.DocumentsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            BindWarrantId();
            if (GeneralBindingSource.Position == -1)
                return;
            if (ViewportState == ViewportState.NewRowState)
                return;
            ViewportState = ViewportState.ReadState;
            IsEditable = true;
        }

        void TenancyViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
                UnbindedCheckBoxesUpdate();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        void TenancyViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        void TenancyPersons_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                RedrawDataGridRows();
            }
        }

        void TenancyPersons_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RedrawDataGridRows();
        }

        private void TenancyAssocViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RebuildStaticFilter();
        }

        private void TenancyAssocViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RebuildStaticFilter();
        }

        void vButtonWarrant_Click(object sender, EventArgs e)
        {
            if (_idWarrant != null)
            {
                _idWarrant = null;
                textBoxSelectedWarrant.Text = "";
                vButtonWarrant.Text = @"...";
                return;
            }
            if (_swForm == null)
                _swForm = new SelectWarrantForm();
            if (_swForm.ShowDialog() != DialogResult.OK) return;
            if (_swForm.WarrantId == null) return;
            _idWarrant = _swForm.WarrantId.Value;
            textBoxSelectedWarrant.Text = WarrantStringById(_swForm.WarrantId.Value);
            vButtonWarrant.Text = @"x";
        }

        void checkBoxProtocolEnable_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxProtocol.Controls)
                if (control != checkBoxProtocolEnable)
                    control.Enabled = checkBoxProtocolEnable.Checked;
        }

        void checkBoxProcessEnable_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxTenancyContract.Controls)
                if (control != checkBoxContractEnable)
                    control.Enabled = checkBoxContractEnable.Checked;
        }

        private void dataGridViewTenancyAgreements_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!HasAssocViewport<TenancyAgreementsViewport>())
                return;
            if (e.RowIndex == -1)
                return;
            ShowAssocViewport<TenancyAgreementsViewport>();
        }

        private void dataGridViewTenancyPersons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!HasAssocViewport<TenancyPersonsViewport>())
                return;
            if (e.RowIndex == -1)
                return;
            ShowAssocViewport<TenancyPersonsViewport>();
        }

        private void dataGridViewTenancyAddress_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocViewport<TenancyPremisesViewport>())
                ShowAssocViewport<TenancyPremisesViewport>();
        }

        private void dataGridViewTenancyReasons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocViewport<TenancyReasonsViewport>())
                ShowAssocViewport<TenancyReasonsViewport>();
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

        private void checkBoxUntilDismissal_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerEndDate.Enabled = !checkBoxUntilDismissal.Checked;
        }

        private void vButtonRentPeriodAdd_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбран процесс найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = (DataRowView) GeneralBindingSource[GeneralBindingSource.Position];
            using (var form = new RentPeriodsEditor())
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                var rentPeriod = new TenancyRentPeriod
                {
                    IdProcess = (int) row["id_process"],
                    BeginDate = form.BeginDate,
                    EndDate = form.EndDate,
                    UntilDismissal = form.UntilDismissal
                };
                _rentPeriods.EditingNewRecord = true;
                var idRentPeriod = _rentPeriods.Insert(rentPeriod);
                if (idRentPeriod == -1) return;
                rentPeriod.IdRentPeriod = idRentPeriod;
                _rentPeriods.Select().Rows.Add(idRentPeriod, rentPeriod.IdProcess, rentPeriod.BeginDate, rentPeriod.EndDate, rentPeriod.UntilDismissal);
                _rentPeriods.EditingNewRecord = false;
            }
        }

        private void vButtonRentPeriodDelete_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбран процесс найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (_vRentPeriods.Position == -1)
            {
                MessageBox.Show(@"Не выбран период найма для удаления", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (MessageBox.Show(@"Вы уверены, что хотите удалить этот период найма?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var idRentPeriod = (int)((DataRowView)_vRentPeriods[_vRentPeriods.Position])["id_rent_period"];
            if (_rentPeriods.Delete(idRentPeriod) == -1)
                return;
            _rentPeriods.Select().Rows.Find(idRentPeriod).Delete();
        }

        private void vButtonSwapRentPeriod_Click(object sender, EventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (GeneralBindingSource.Position == -1)
            {
                MessageBox.Show(@"Не выбран процесс найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            var rentPeriod = new TenancyRentPeriod
            {
                IdProcess = (int)row["id_process"],
                BeginDate = ViewportHelper.ValueOrNull<DateTime>(row, "begin_date"),
                EndDate = ViewportHelper.ValueOrNull<DateTime>(row, "end_date"),
                UntilDismissal = ViewportHelper.ValueOrNull<bool>(row, "until_dismissal"),
            };
            _rentPeriods.EditingNewRecord = true;
            var idRentPeriod = _rentPeriods.Insert(rentPeriod);
            if (idRentPeriod == -1) return;
            rentPeriod.IdRentPeriod = idRentPeriod;
            _rentPeriods.Select().Rows.Add(idRentPeriod, rentPeriod.IdProcess, rentPeriod.BeginDate, rentPeriod.EndDate, rentPeriod.UntilDismissal);
            _rentPeriods.EditingNewRecord = false;

            dateTimePickerBeginDate.Value = dateTimePickerEndDate.Value;
            dateTimePickerBeginDate.Checked = dateTimePickerEndDate.Checked;
            dateTimePickerEndDate.Checked = false;

            dateTimePickerEndDate.Focus();
        }
    }
}
