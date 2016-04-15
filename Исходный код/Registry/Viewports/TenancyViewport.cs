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
using Registry.Entities;
using Registry.Reporting;
using Registry.Viewport.SearchForms;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyViewport: FormViewport
    {
        #region Models
        private DataModel tenancy_building_assoc;
        private DataModel tenancy_premises_assoc;
        private DataModel tenancy_sub_premises_assoc;
        private DataModel executors;
        private DataModel rent_types;
        private DataModel tenancy_agreements;
        private DataModel warrants;
        private DataModel tenancy_persons;
        private DataModel tenancy_reasons;
        private DataModel kinships;
        private CalcDataModel tenancy_premises_info;
        #endregion Models

        #region Views
        private BindingSource v_executors;
        private BindingSource v_rent_types;
        private BindingSource v_tenancy_agreements;
        private BindingSource v_warrants;
        private BindingSource v_tenancy_persons;
        private BindingSource v_tenancy_addresses;
        private BindingSource v_tenancy_reasons;
        private BindingSource v_kinships;
        #endregion Views

        //Forms
        private SearchForm stExtendedSearchForm;
        private SearchForm stSimpleSearchForm;
        private SelectWarrantForm swForm;

        private int? id_warrant;
        private bool is_copy;
        private int? id_copy_process;

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
            if (v_tenancy_addresses == null)
                return;
            v_tenancy_addresses.Filter = (GeneralBindingSource.Position >= 0 ? "id_process = 0" + ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"] : "id_process = 0");
        }

        private void RebuildStaticFilter()
        {
            IEnumerable<int> ids = null;
            if (ParentRow == null)
                return;
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    ids = DataModelHelper.TenancyProcessIDsByBuildingId(Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.Premises:
                    ids = DataModelHelper.TenancyProcessIDsByPremisesId(Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.SubPremises:
                    ids = DataModelHelper.TenancyProcessIDsBySubPremisesId(Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture));
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
                if (viewportState == ViewportState.NewRowState)
                    Text = "Новый найм";
                else
                    if (GeneralBindingSource.Position != -1)
                        Text = string.Format(CultureInfo.InvariantCulture, "Процесс найма №{0}", ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"]);
                    else
                        Text = "Процессы отсутствуют";
            }
            else
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        if (viewportState == ViewportState.NewRowState)
                            Text = string.Format(CultureInfo.InvariantCulture, "Новый найм здания №{0}", ParentRow["id_building"]);
                        else
                        if (GeneralBindingSource.Position != -1)
                            Text = string.Format(CultureInfo.InvariantCulture, "Найм №{0} здания №{1}", 
                                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], ParentRow["id_building"]);
                        else
                            Text = string.Format(CultureInfo.InvariantCulture, "Наймы здания №{0} отсутствуют", ParentRow["id_building"]);
                        break;
                    case ParentTypeEnum.Premises:
                        if (viewportState == ViewportState.NewRowState)
                            Text = string.Format(CultureInfo.InvariantCulture, "Новый найм помещения №{0}", ParentRow["id_premises"]);
                        else
                            if (GeneralBindingSource.Position != -1)
                                Text = string.Format(CultureInfo.InvariantCulture, "Найм №{0} помещения №{1}",
                                    ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_process"], ParentRow["id_premises"]);
                            else
                                Text = string.Format(CultureInfo.InvariantCulture, "Наймы помещения №{0} отсутствуют", ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.SubPremises:
                        if (viewportState == ViewportState.NewRowState)
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
                if (((DataRowView)v_tenancy_persons[i])["id_kinship"] != DBNull.Value &&
                    Convert.ToInt32(((DataRowView)v_tenancy_persons[i])["id_kinship"], CultureInfo.InvariantCulture) == 1 &&
                    ((DataRowView)v_tenancy_persons[i])["exclude_date"] == DBNull.Value)
                    dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                else
                    if (((DataRowView)v_tenancy_persons[i])["exclude_date"] != DBNull.Value)
                        dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
                    else
                        dataGridViewTenancyPersons.Rows[i].DefaultCellStyle.BackColor = Color.White;
        }

        private string WarrantStringByID(int id_warrant)
        {
            if (v_warrants.Position == -1)
                return null;
            else
            {
                var row_index = v_warrants.Find("id_warrant", id_warrant);
                if (row_index == -1)
                    return null;
                var registration_date = Convert.ToDateTime(((DataRowView)v_warrants[row_index])["registration_date"], CultureInfo.InvariantCulture);
                var registration_num = ((DataRowView)v_warrants[row_index])["registration_num"].ToString();
                return string.Format(CultureInfo.InvariantCulture, "№ {0} от {1}", 
                    registration_num, registration_date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            }
        }

        private void BindWarrantID()
        {
            if ((GeneralBindingSource.Position > -1) && ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_warrant"] != DBNull.Value)
            {
                id_warrant = Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_warrant"], CultureInfo.InvariantCulture);
                textBoxSelectedWarrant.Text =
                    WarrantStringByID(id_warrant.Value);
                vButtonWarrant.Text = "x";
            }
            else
            {
                id_warrant = null;
                textBoxSelectedWarrant.Text = "";
                vButtonWarrant.Text = "...";
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
            comboBoxRentType.DataSource = v_rent_types;
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

            dataGridViewTenancyPersons.DataSource = v_tenancy_persons;
            surname.DataPropertyName = "surname";
            name.DataPropertyName = "name";
            patronymic.DataPropertyName = "patronymic";
            date_of_birth.DataPropertyName = "date_of_birth";
            id_kinship.DataSource = v_kinships;
            id_kinship.ValueMember = "id_kinship";
            id_kinship.DisplayMember = "kinship";
            id_kinship.DataPropertyName = "id_kinship";


            dataGridViewTenancyAgreements.DataSource = v_tenancy_agreements;
            agreement_date.DataPropertyName = "agreement_date";
            agreement_content.DataPropertyName = "agreement_content";

            dataGridViewTenancyReasons.DataSource = v_tenancy_reasons;
            reason_date.DataPropertyName = "reason_date";
            reason_number.DataPropertyName = "reason_number";
            reason_prepared.DataPropertyName = "reason_prepared";

            dataGridViewTenancyAddress.DataSource = v_tenancy_addresses;
            address.DataPropertyName = "address";
            total_area.DataPropertyName = "total_area";
            living_area.DataPropertyName = "living_area";
            rent_area.DataPropertyName = "rent_area";

            textBoxProtocolNumber.DataBindings.Clear();
            textBoxProtocolNumber.DataBindings.Add("Text", GeneralBindingSource, "protocol_num", true, DataSourceUpdateMode.Never, "");

            dateTimePickerProtocolDate.DataBindings.Clear();
            dateTimePickerProtocolDate.DataBindings.Add("Value", GeneralBindingSource, "protocol_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");

            comboBoxExecutor.DataSource = v_executors;
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
            viewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidateTenancy(TenancyProcess tenancy)
        {
            if (tenancy.IdRentType == null)
            {
                MessageBox.Show("Необходимо выбрать тип найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxRentType.Focus();
                return false;
            }
            if (checkBoxContractEnable.Checked)
            {
                if (tenancy.RegistrationNum == null)
                {
                    MessageBox.Show("Не указан номер договора найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    textBoxRegistrationNumber.Focus();
                    return false;
                }
            }
            if (checkBoxProtocolEnable.Checked)
            {
                if (tenancy.ProtocolNum == null)
                {
                    MessageBox.Show("Не указан номер протокола жилищной комиссии", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    textBoxProtocolNumber.Focus();
                    return false;
                }
            }
            var tenancyFromView = (TenancyProcess) EntityFromView();
            if (tenancy.RegistrationNum != null && tenancy.RegistrationNum != tenancyFromView.RegistrationNum)
                if (DataModelHelper.TenancyProcessesDuplicateCount(tenancy) != 0 &&
                    MessageBox.Show("В базе уже имеется договор с таким номером. Все равно продолжить сохранение?", "Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return false;
            // Проверить соответствие вида найма
            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        if (!ViewportHelper.BuildingFundAndRentMatch((int)ParentRow["id_building"], tenancy.IdRentType.Value) &&
                            MessageBox.Show("Выбранный вид найма не соответствует фонду сдаваемого здания. Все равно продолжить сохранение?",
                            "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                            return false;
                        break;
                    case ParentTypeEnum.Premises:
                        if (!ViewportHelper.PremiseFundAndRentMatch((int)ParentRow["id_premises"], tenancy.IdRentType.Value))
                        {
                            if (!ViewportHelper.BuildingFundAndRentMatch((int)ParentRow["id_building"], tenancy.IdRentType.Value) &&
                            MessageBox.Show("Выбранный вид найма не соответствует фонду сдаваемого помещения. Все равно продолжить сохранение?",
                            "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                                return false;
                        }
                        break;
                    case ParentTypeEnum.SubPremises:
                        if (!ViewportHelper.SubPremiseFundAndRentMatch((int)ParentRow["id_sub_premises"], tenancy.IdRentType.Value))
                        {
                            if (!ViewportHelper.PremiseFundAndRentMatch((int)ParentRow["id_premises"], tenancy.IdRentType.Value))
                            {
                                var idBuilding = (int)DataModel.GetInstance<PremisesDataModel>().Select().Rows.Find((int)ParentRow["id_premises"])["id_building"];
                                if (!ViewportHelper.BuildingFundAndRentMatch(idBuilding, tenancy.IdRentType.Value) &&
                                    MessageBox.Show("Выбранный вид найма не соответствует фонду сдаваемой комнаты. Все равно продолжить сохранение?",
                                    "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
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
            var tenancy = new TenancyProcess();
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            tenancy.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            tenancy.IdRentType = ViewportHelper.ValueOrNull<int>(row, "id_rent_type");
            tenancy.IdWarrant = ViewportHelper.ValueOrNull<int>(row, "id_warrant");
            tenancy.IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor");
            tenancy.RegistrationNum = ViewportHelper.ValueOrNull(row, "registration_num");
            tenancy.RegistrationDate = ViewportHelper.ValueOrNull<DateTime>(row, "registration_date");
            tenancy.IssueDate = ViewportHelper.ValueOrNull<DateTime>(row, "issue_date");
            tenancy.BeginDate = ViewportHelper.ValueOrNull<DateTime>(row, "begin_date");
            tenancy.EndDate = ViewportHelper.ValueOrNull<DateTime>(row, "end_date");
            tenancy.UntilDismissal = ViewportHelper.ValueOrNull<bool>(row, "until_dismissal");     
            tenancy.ResidenceWarrantNum = ViewportHelper.ValueOrNull(row, "residence_warrant_num");
            tenancy.ResidenceWarrantDate = ViewportHelper.ValueOrNull<DateTime>(row, "residence_warrant_date");
            tenancy.ProtocolNum = ViewportHelper.ValueOrNull(row, "protocol_num");
            tenancy.ProtocolDate = ViewportHelper.ValueOrNull<DateTime>(row, "protocol_date");
            tenancy.Description = ViewportHelper.ValueOrNull(row, "description");     
            return tenancy;
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
                tenancy.IdWarrant = id_warrant;
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
            if (viewportState != ViewportState.NewRowState)
            {
                var reasons =
                    (from reason_row in DataModel.GetInstance<TenancyReasonsDataModel>().FilterDeletedRows() 
                        where reason_row.Field<int>("id_process") == (int) row["id_process"] &&
                              reason_row.Field<string>("reason_prepared").ToUpper().Contains("ОРДЕР")
                        select new
                        {
                            number = reason_row.Field<string>("reason_number"),
                            date = reason_row.Field<DateTime?>("reason_date")
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
            comboBoxRentType.SelectedValue = ViewportHelper.ValueOrDBNull(tenancy.IdRentType);
            comboBoxExecutor.SelectedValue = ViewportHelper.ValueOrDBNull(tenancy.IdExecutor);
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
                textBoxSelectedWarrant.Text = WarrantStringByID(tenancy.IdWarrant.Value);
                id_warrant = tenancy.IdWarrant;
            }
            else
            {
                textBoxSelectedWarrant.Text = "";
                id_warrant = null;
            }
        }

        private static void FillRowFromTenancy(TenancyProcess tenancy, DataRowView row)
        {
            row.BeginEdit();
            row["id_process"] = ViewportHelper.ValueOrDBNull(tenancy.IdProcess);
            row["id_rent_type"] = ViewportHelper.ValueOrDBNull(tenancy.IdRentType);
            row["id_warrant"] = ViewportHelper.ValueOrDBNull(tenancy.IdWarrant);
            row["registration_num"] = ViewportHelper.ValueOrDBNull(tenancy.RegistrationNum);
            row["registration_date"] = ViewportHelper.ValueOrDBNull(tenancy.RegistrationDate);
            row["issue_date"] = ViewportHelper.ValueOrDBNull(tenancy.IssueDate);
            row["begin_date"] = ViewportHelper.ValueOrDBNull(tenancy.BeginDate);
            row["end_date"] = ViewportHelper.ValueOrDBNull(tenancy.EndDate);
            row["until_dismissal"] = ViewportHelper.ValueOrDBNull(tenancy.UntilDismissal);
            row["residence_warrant_num"] = ViewportHelper.ValueOrDBNull(tenancy.ResidenceWarrantNum);
            row["residence_warrant_date"] = ViewportHelper.ValueOrDBNull(tenancy.ResidenceWarrantDate);
            row["protocol_num"] = ViewportHelper.ValueOrDBNull(tenancy.ProtocolNum);
            row["protocol_date"] = ViewportHelper.ValueOrDBNull(tenancy.ProtocolDate);
            row["id_executor"] = ViewportHelper.ValueOrDBNull(tenancy.IdExecutor);
            row["description"] = ViewportHelper.ValueOrDBNull(tenancy.Description);
            row.EndEdit();
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
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance<TenancyProcessesDataModel>();
            executors = DataModel.GetInstance<ExecutorsDataModel>();
            rent_types = DataModel.GetInstance<RentTypesDataModel>();
            tenancy_agreements = DataModel.GetInstance<TenancyAgreementsDataModel>();
            warrants = DataModel.GetInstance<WarrantsDataModel>();
            tenancy_persons = DataModel.GetInstance<TenancyPersonsDataModel>();
            tenancy_reasons = DataModel.GetInstance<TenancyReasonsDataModel>();
            kinships = DataModel.GetInstance<KinshipsDataModel>();
            tenancy_premises_info = CalcDataModel.GetInstance<CalcDataModelTenancyPremisesInfo>();

            //Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();
            executors.Select();
            rent_types.Select();
            tenancy_agreements.Select();
            warrants.Select();
            tenancy_persons.Select();
            tenancy_reasons.Select();
            kinships.Select();

            v_executors = new BindingSource
            {
                DataSource = DataModel.DataSet,
                DataMember = "executors",
                Filter = "is_inactive = 0"
            };

            v_rent_types = new BindingSource
            {
                DataSource = DataModel.DataSet,
                DataMember = "rent_types"
            };

            v_kinships = new BindingSource
            {
                DataSource = DataModel.DataSet,
                DataMember = "kinships"
            };

            v_warrants = new BindingSource
            {
                DataSource = DataModel.DataSet,
                DataMember = "warrants"
            };

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += v_tenancies_CurrentItemChanged;
            GeneralBindingSource.DataMember = "tenancy_processes";
            GeneralBindingSource.DataSource = DataModel.DataSet;
            RebuildStaticFilter();
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                GeneralBindingSource.Filter += " AND ";
            GeneralBindingSource.Filter += DynamicFilter;

            v_tenancy_persons = new BindingSource
            {
                DataMember = "tenancy_processes_tenancy_persons",
                DataSource = GeneralBindingSource
            };

            v_tenancy_agreements = new BindingSource
            {
                DataMember = "tenancy_processes_tenancy_agreements",
                DataSource = GeneralBindingSource
            };

            v_tenancy_reasons = new BindingSource
            {
                DataMember = "tenancy_processes_tenancy_reasons",
                DataSource = GeneralBindingSource
            };

            v_tenancy_addresses = new BindingSource {DataSource = tenancy_premises_info.Select()};

            DataBind();

            tenancy_persons.Select().RowChanged += TenancyPersons_RowChanged;
            tenancy_persons.Select().RowDeleted += TenancyPersons_RowDeleted;
            GeneralDataModel.Select().RowChanged += TenancyViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += TenancyViewport_RowDeleted;
            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        tenancy_building_assoc = DataModel.GetInstance<TenancyBuildingsAssocDataModel>();
                        tenancy_building_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_building_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    case ParentTypeEnum.Premises:
                        tenancy_premises_assoc = DataModel.GetInstance<TenancyPremisesAssocDataModel>();
                        tenancy_premises_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_premises_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    case ParentTypeEnum.SubPremises:
                        tenancy_sub_premises_assoc = DataModel.GetInstance<TenancySubPremisesAssocDataModel>();
                        tenancy_sub_premises_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_sub_premises_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
            v_tenancy_persons.ListChanged += v_persons_ListChanged;   
            tenancy_premises_info.RefreshEvent +=tenancy_premises_info_RefreshEvent;
            FiltersRebuild();
            DataChangeHandlersInit();
        }

        private void tenancy_premises_info_RefreshEvent(object sender, EventArgs e)
        {
            // Обновляем информацию по помещениям (живое обновление не реализуемо)
            if (v_tenancy_addresses != null)
            {
                v_tenancy_addresses.DataSource =
                    tenancy_premises_info.Select();
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
            is_editable = false;
            GeneralBindingSource.AddNew();
            var index = v_executors.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)v_executors[index])["id_executor"];
            is_copy = false;
            id_copy_process = null;
            is_editable = true;
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
            is_editable = false;
            var tenancy = (TenancyProcess) EntityFromView();
            GeneralBindingSource.AddNew();
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromTenancy(tenancy);
            checkBoxContractEnable.Checked = (tenancy.RegistrationDate != null) || (tenancy.RegistrationNum != null);
            checkBoxProtocolEnable.Checked = (tenancy.ProtocolDate != null);
            dateTimePickerIssueDate.Checked = (tenancy.IssueDate != null);
            dateTimePickerBeginDate.Checked = (tenancy.BeginDate != null);
            dateTimePickerEndDate.Checked = (tenancy.EndDate != null);
            var index = v_executors.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)v_executors[index])["id_executor"];
            is_copy = true;
            id_copy_process = tenancy.IdProcess;
            is_editable = true;
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
                    if (stSimpleSearchForm == null)
                        stSimpleSearchForm = new SimpleSearchTenancyForm();
                    if (stSimpleSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = stSimpleSearchForm.GetFilter();
                    break;
                case SearchFormType.ExtendedSearchForm:
                    if (stExtendedSearchForm == null)
                        stExtendedSearchForm = new ExtendedSearchTenancyForm();
                    if (stExtendedSearchForm.ShowDialog() != DialogResult.OK)
                        return;
                    DynamicFilter = stExtendedSearchForm.GetFilter();
                    break;
            }
            var Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            is_editable = false;
            GeneralBindingSource.Filter = Filter;
            is_editable = true;
        }

        public override void ClearSearch()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            GeneralBindingSource.Filter = StaticFilter;
            is_editable = true;
            DynamicFilter = "";
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот процесс найма?", "Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_process"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        is_editable = false;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                    }
                    else
                        Text = "Процессы отсутствуют";
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    is_editable = false;
                    DataBind();
                    BindWarrantID();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_copy = false;
            id_copy_process = null;
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void SaveRecord()
        {
            var tenancy = (TenancyProcess) EntityFromViewport();
            if (!ValidateTenancy(tenancy))
                return;
            var Filter = "";
            if (!string.IsNullOrEmpty(GeneralBindingSource.Filter))
                Filter += " OR ";
            else
                Filter += "(1 = 1) OR ";
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var id_process = GeneralDataModel.Insert(tenancy);
                    if (id_process == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    tenancy.IdProcess = id_process;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", tenancy.IdProcess);
                    GeneralBindingSource.Filter += Filter;
                    FillRowFromTenancy(tenancy, newRow);
                    // Если производится копирование, а не создание новой записи, то надо скопировать участников найма и нанимаемое жилье
                    if (is_copy && id_copy_process != null)
                    {
                        if (!CopyTenancyProcessRelData(id_process, id_copy_process.Value))
                            MessageBox.Show("Произошла ошибка во время копирования данных", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                    else
                        if (ParentRow != null)
                        {
                            var to = new TenancyObject();
                            to.IdProcess = id_process;
                            to.RentLivingArea = null;
                            to.RentTotalArea = null;
                            var id_assoc = -1;
                            switch (ParentType)
                            {
                                case ParentTypeEnum.Building:
                                    var tenancy_buildings = DataModel.GetInstance<TenancyBuildingsAssocDataModel>();
                                    to.IdObject = Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture);
                                    tenancy_buildings.EditingNewRecord = true;
                                    id_assoc = tenancy_buildings.Insert(to);
                                    if (id_assoc == -1)
                                        return;
                                    to.IdAssoc = id_assoc;
                                    tenancy_buildings.Select().Rows.Add(id_assoc, to.IdObject, to.IdProcess, to.RentTotalArea, to.RentLivingArea, 0);
                                    tenancy_buildings.EditingNewRecord = false;
                                    break;
                                case ParentTypeEnum.Premises:
                                    var tenancy_premises = DataModel.GetInstance<TenancyPremisesAssocDataModel>();
                                    to.IdObject = Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture);
                                    tenancy_premises.EditingNewRecord = true;
                                    id_assoc = tenancy_premises.Insert(to);
                                    if (id_assoc == -1)
                                        return;
                                    to.IdAssoc = id_assoc;
                                    tenancy_premises.Select().Rows.Add(id_assoc, to.IdObject, to.IdProcess, to.RentTotalArea, to.RentLivingArea, 0);
                                    tenancy_premises.EditingNewRecord = false;
                                    break;
                                case ParentTypeEnum.SubPremises:
                                    var tenancy_sub_premises = DataModel.GetInstance<TenancySubPremisesAssocDataModel>();
                                    to.IdObject = Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture);
                                    tenancy_sub_premises.EditingNewRecord = true;
                                    id_assoc = tenancy_sub_premises.Insert(to);
                                    if (id_assoc == -1)
                                        return;
                                    to.IdAssoc = id_assoc;
                                    tenancy_sub_premises.Select().Rows.Add(id_assoc, to.IdObject, to.IdProcess, to.RentTotalArea, 0);
                                    tenancy_sub_premises.EditingNewRecord = false;
                                    break;
                                default: throw new ViewportException("Неизвестный тип родительского объекта");
                            }
                        }
                    // Обновляем информацию по помещениям (живое обновление не реализуемо)
                    if (v_tenancy_addresses != null)
                    {
                        v_tenancy_addresses.DataSource = CalcDataModel.GetInstance<CalcDataModelTenancyPremisesInfo>();
                        FiltersRebuild();
                    }
                    GeneralDataModel.EditingNewRecord = false;
                    RebuildStaticFilter();
                    GeneralBindingSource.Position = GeneralBindingSource.Count - 1;
                    break;
                case ViewportState.ModifyRowState:
                    if (tenancy.IdProcess == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить процесс найма без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(tenancy) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    is_editable = false;
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", tenancy.IdProcess);
                    GeneralBindingSource.Filter += Filter;
                    FillRowFromTenancy(tenancy, row);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        // Метод копирует зависимые данные по процессу найма
        private bool CopyTenancyProcessRelData(int id_new_process, int id_copy_process)
        {
            var persons = from persons_row in tenancy_persons.FilterDeletedRows()
                          where persons_row.Field<int>("id_process") == id_copy_process
                          select persons_row;
            tenancy_persons.EditingNewRecord = true;
            foreach (var person_row in persons.ToList())
            {
                var person = DataRowToPerson(person_row);
                person.IdProcess = id_new_process;
                var id_person = tenancy_persons.Insert(person);
                if (id_person == -1)
                {
                    tenancy_persons.EditingNewRecord = false;
                    return false;
                }
                person.IdPerson = id_person;
                tenancy_persons.Select().Rows.Add(PersonToObjectArray(person));
            }
            tenancy_persons.EditingNewRecord = false;
            var tenancyBuildingsAssoc = DataModel.GetInstance<TenancyBuildingsAssocDataModel>();
            var buildings = from row in tenancyBuildingsAssoc.FilterDeletedRows()
                            where row.Field<int>("id_process") == id_copy_process
                            select row;
            tenancyBuildingsAssoc.EditingNewRecord = true;
            foreach (var row in buildings.ToList())
            {
                var obj = new TenancyObject();
                obj.IdObject = row.Field<int?>("id_building");
                obj.IdProcess = id_new_process;
                obj.RentLivingArea = row.Field<double?>("rent_living_area");
                obj.RentTotalArea = row.Field<double?>("rent_total_area");
                var id_assoc = tenancyBuildingsAssoc.Insert(obj);
                if (id_assoc == -1)
                {
                    tenancyBuildingsAssoc.EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = id_assoc;
                tenancyBuildingsAssoc.Select().Rows.Add(obj.IdAssoc, obj.IdObject, obj.IdProcess,
                    obj.RentTotalArea, obj.RentLivingArea);
            }
            tenancyBuildingsAssoc.EditingNewRecord = false;
            var tenancyPremisesAssoc = DataModel.GetInstance<TenancyPremisesAssocDataModel>();
            var premises = from row in tenancyPremisesAssoc.FilterDeletedRows()
                            where row.Field<int>("id_process") == id_copy_process
                           select row;
            tenancyPremisesAssoc.EditingNewRecord = true;
            foreach (var row in premises.ToList())
            {
                var obj = new TenancyObject();
                obj.IdObject = row.Field<int?>("id_premises");
                obj.IdProcess = id_new_process;
                obj.RentLivingArea = row.Field<double?>("rent_living_area");
                obj.RentTotalArea = row.Field<double?>("rent_total_area");
                var id_assoc = tenancyPremisesAssoc.Insert(obj);
                if (id_assoc == -1)
                {
                    tenancyPremisesAssoc.EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = id_assoc;
                tenancyPremisesAssoc.Select().Rows.Add(obj.IdAssoc, obj.IdObject, obj.IdProcess,
                    obj.RentTotalArea, obj.RentLivingArea);
            }
            tenancyPremisesAssoc.EditingNewRecord = false;
            var tenancySubPremisesAssoc = DataModel.GetInstance<TenancySubPremisesAssocDataModel>();
            var sub_premises = from row in tenancySubPremisesAssoc.FilterDeletedRows()
                           where row.Field<int>("id_process") == id_copy_process
                               select row;
            tenancySubPremisesAssoc.EditingNewRecord = true;
            foreach (var row in sub_premises.ToList())
            {
                var obj = new TenancyObject();
                obj.IdObject = row.Field<int?>("id_sub_premises");
                obj.IdProcess = id_new_process;
                obj.RentTotalArea = row.Field<double?>("rent_total_area");
                var id_assoc = tenancySubPremisesAssoc.Insert(obj);
                if (id_assoc == -1)
                {
                    tenancySubPremisesAssoc.EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = id_assoc;
                tenancySubPremisesAssoc.Select().Rows.Add(obj.IdAssoc, obj.IdObject, obj.IdProcess,
                    obj.RentTotalArea);
            }
            tenancySubPremisesAssoc.EditingNewRecord = false;
            return true;
        }

        private TenancyPerson DataRowToPerson(DataRow row)
        {
            var person = new TenancyPerson();
            person.IdPerson = row.Field<int?>("id_person");
            person.IdProcess = row.Field<int?>("id_process");
            person.IdKinship = row.Field<int?>("id_kinship");
            person.Surname = row.Field<string>("surname");
            person.Name = row.Field<string>("name");
            person.Patronymic = row.Field<string>("patronymic");
            person.DateOfBirth = row.Field<DateTime?>("date_of_birth");
            person.IdDocumentType = row.Field<int?>("id_document_type");
            person.DateOfDocumentIssue = row.Field<DateTime?>("date_of_document_issue");
            person.DocumentNum = row.Field<string>("document_num");
            person.DocumentSeria = row.Field<string>("document_seria");
            person.IdDocumentIssuedBy = row.Field<int?>("id_document_issued_by");
            person.RegistrationIdStreet = row.Field<string>("registration_id_street");
            person.RegistrationHouse = row.Field<string>("registration_house");
            person.RegistrationFlat = row.Field<string>("registration_flat");
            person.RegistrationRoom = row.Field<string>("registration_room");
            person.ResidenceIdStreet = row.Field<string>("residence_id_street");
            person.ResidenceHouse = row.Field<string>("residence_house");
            person.ResidenceFlat = row.Field<string>("residence_flat");
            person.ResidenceRoom = row.Field<string>("residence_room");
            person.PersonalAccount = row.Field<string>("personal_account");
            person.IncludeDate = row.Field<DateTime?>("include_date");
            person.ExcludeDate = row.Field<DateTime?>("exclude_date");
            return person;
        }

        private object[] PersonToObjectArray(TenancyPerson person)
        {
            return new object[] {
                person.IdPerson,
                person.IdProcess,
                person.IdKinship,
                person.Surname,
                person.Name,
                person.Patronymic,
                person.DateOfBirth,
                person.IdDocumentType,
                person.DateOfDocumentIssue,
                person.DocumentNum,
                person.DocumentSeria,
                person.IdDocumentIssuedBy,
                person.RegistrationIdStreet,
                person.RegistrationHouse,
                person.RegistrationFlat,
                person.RegistrationRoom,
                person.RegistrationDate,
                person.ResidenceIdStreet,
                person.ResidenceHouse,
                person.ResidenceFlat,
                person.ResidenceRoom,
                person.PersonalAccount,
                person.IncludeDate,
                person.ExcludeDate
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                GeneralBindingSource.CurrentItemChanged -= v_tenancies_CurrentItemChanged;
                v_tenancy_persons.ListChanged -= v_persons_ListChanged;
                tenancy_premises_info.RefreshEvent -= tenancy_premises_info_RefreshEvent;
                tenancy_persons.Select().RowChanged -= TenancyPersons_RowChanged;
                tenancy_persons.Select().RowDeleted -= TenancyPersons_RowDeleted;
                GeneralDataModel.Select().RowChanged -= TenancyViewport_RowChanged;
                GeneralDataModel.Select().RowDeleted -= TenancyViewport_RowDeleted;

                if (tenancy_building_assoc != null)
                {
                        tenancy_building_assoc.Select().RowChanged -= TenancyAssocViewport_RowChanged;
                        tenancy_building_assoc.Select().RowDeleted -= TenancyAssocViewport_RowDeleted;
                }
                if (tenancy_premises_assoc != null)
                {
                        tenancy_premises_assoc.Select().RowChanged -= TenancyAssocViewport_RowChanged;
                        tenancy_premises_assoc.Select().RowDeleted -= TenancyAssocViewport_RowDeleted;
                }
                if (tenancy_sub_premises_assoc != null)
                {
                        tenancy_sub_premises_assoc.Select().RowChanged -= TenancyAssocViewport_RowChanged;
                        tenancy_sub_premises_assoc.Select().RowDeleted -= TenancyAssocViewport_RowDeleted;
                }
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                GeneralDataModel.EditingNewRecord = false;
            base.ForceClose();
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
            if (GeneralBindingSource.Position == -1)
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
                    return true;
                case ReporterType.TenancyAgreementReporter:
                    return idProcess != null && (DataModelHelper.TenancyAgreementsForProcess(idProcess.Value) > 0);
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
                case ReporterType.TenancyAgreementReporter:
                    if (v_tenancy_agreements.Position == -1)
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
            var row = (DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position];
            return new Dictionary<string, string> {{"id_agreement", row["id_agreement"].ToString()}};
        }

        private bool TenancyValidForReportGenerate()
        {
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (GeneralBindingSource.Position == -1)
                return false;
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            if (!DataModelHelper.TenancyProcessHasTenant(Convert.ToInt32(row["id_process"], CultureInfo.InvariantCulture)))
            {
                MessageBox.Show("Для формирования отчетной документации необходимо указать нанимателя процесса найма", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewportHelper.ValueOrNull<DateTime>(row, "registration_date") == null || ViewportHelper.ValueOrNull(row, "registration_num") == null)
            {
                MessageBox.Show("Для формирования отчетной документации необходимо завести договор найма и указать его номер и дату регистрации", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
            BindWarrantID();
            if (GeneralBindingSource.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            viewportState = ViewportState.ReadState;
            is_editable = true;
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
            if (id_warrant != null)
            {
                id_warrant = null;
                textBoxSelectedWarrant.Text = "";
                vButtonWarrant.Text = "...";
                return;
            }
            if (swForm == null)
                swForm = new SelectWarrantForm();
            if (swForm.ShowDialog() == DialogResult.OK)
            {
                if (swForm.WarrantId != null)
                {
                    id_warrant = swForm.WarrantId.Value;
                    textBoxSelectedWarrant.Text = WarrantStringByID(swForm.WarrantId.Value);
                    vButtonWarrant.Text = "x";
                }
            }
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
    }
}
