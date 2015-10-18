using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using Registry.CalcDataModels;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Registry.SearchForms;
using Security;
using VIBlend.Utilities;
using VIBlend.WinForms.Controls;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class TenancyViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel9;
        private TableLayoutPanel tableLayoutPanel10;
        private GroupBox groupBoxTenancyContract;
        private GroupBox groupBoxProtocol;
        private GroupBox groupBox21;
        private GroupBox groupBox24;
        private GroupBox groupBox25;
        private Panel panel5;
        private Panel panel6;
        private DataGridView dataGridViewTenancyAddress;
        private DataGridView dataGridViewTenancyReasons;
        private DataGridViewTextBoxColumn reason_prepared;
        private DataGridViewTextBoxColumn reason_number;
        private DataGridViewTextBoxColumn reason_date;
        private DataGridView dataGridViewTenancyAgreements;
        private DataGridViewTextBoxColumn agreement_date;
        private DataGridViewTextBoxColumn agreement_content;
        private CheckBox checkBoxContractEnable;
        private CheckBox checkBoxProtocolEnable;
        private Label label42;
        private Label label45;
        private Label label47;
        private Label label48;
        private Label label49;
        private Label label50;
        private Label label51;
        private Label label52;
        private Label label82;
        private TextBox textBoxProtocolNumber;
        private TextBox textBoxRegistrationNumber;
        private TextBox textBoxSelectedWarrant = new TextBox();
        private DateTimePicker dateTimePickerProtocolDate;
        private DateTimePicker dateTimePickerRegistrationDate;
        private DateTimePicker dateTimePickerIssueDate;
        private DateTimePicker dateTimePickerBeginDate;
        private DateTimePicker dateTimePickerEndDate;
        private vButton vButtonWarrant = new vButton();
        private GroupBox groupBox22;
        private ComboBox comboBoxExecutor;
        private Label label41;
        private ComboBox comboBoxRentType;
        private Label label46;
        #endregion Components

        #region Models
        private TenancyProcessesDataModel tenancies;
        private TenancyBuildingsAssocDataModel tenancy_building_assoc;
        private TenancyPremisesAssocDataModel tenancy_premises_assoc;
        private TenancySubPremisesAssocDataModel tenancy_sub_premises_assoc;
        private ExecutorsDataModel executors;
        private RentTypesDataModel rent_types;
        private TenancyAgreementsDataModel tenancy_agreements;
        private WarrantsDataModel warrants;
        private TenancyPersonsDataModel tenancy_persons;
        private TenancyReasonsDataModel tenancy_reasons;
        private KinshipsDataModel kinships;
        #endregion Models

        #region Views
        private BindingSource v_tenancies;
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

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable;
        private int? id_warrant;
        private bool is_copy;
        private DataGridViewTextBoxColumn address;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn rent_area;
        private GroupBox groupBox31;
        private TextBox textBoxDescription;
        private GroupBox groupBox1;
        private DataGridView dataGridViewTenancyPersons;
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        private DataGridViewTextBoxColumn date_of_birth;
        private DataGridViewComboBoxColumn id_kinship;
        private int? id_copy_process;

        private TenancyViewport()
            : this(null)
        {
        }

        public TenancyViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public TenancyViewport(TenancyViewport tenancyViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = tenancyViewport.DynamicFilter;
            StaticFilter = tenancyViewport.StaticFilter;
            ParentRow = tenancyViewport.ParentRow;
            ParentType = tenancyViewport.ParentType;
        }

        private void FiltersRebuild()
        {
            if (v_tenancy_addresses == null)
                return;
            v_tenancy_addresses.Filter = (v_tenancies.Position >= 0 ? "id_process = 0" + ((DataRowView)v_tenancies[v_tenancies.Position])["id_process"] : "id_process = 0");
        }

        private void RebuildStaticFilter()
        {
            IEnumerable<int> ids = null;
            if (ParentRow == null)
                return;
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    ids = DataModelHelper.TenancyProcessIDsByBuildingID(Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.Premises:
                    ids = DataModelHelper.TenancyProcessIDsByPremisesID(Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.SubPremises:
                    ids = DataModelHelper.TenancyProcessIDsBySubPremisesID(Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture));
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
            v_tenancies.Filter = StaticFilter;           
        }

        private void SetViewportCaption()
        {
            if (ParentRow == null)
            {
                if (viewportState == ViewportState.NewRowState)
                    Text = "Новый найм";
                else
                    if (v_tenancies.Position != -1)
                        Text = string.Format(CultureInfo.InvariantCulture, "Процесс найма №{0}", ((DataRowView)v_tenancies[v_tenancies.Position])["id_process"]);
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
                        if (v_tenancies.Position != -1)
                            Text = string.Format(CultureInfo.InvariantCulture, "Найм №{0} здания №{1}", 
                                ((DataRowView)v_tenancies[v_tenancies.Position])["id_process"], ParentRow["id_building"]);
                        else
                            Text = string.Format(CultureInfo.InvariantCulture, "Наймы здания №{0} отсутствуют", ParentRow["id_building"]);
                        break;
                    case ParentTypeEnum.Premises:
                        if (viewportState == ViewportState.NewRowState)
                            Text = string.Format(CultureInfo.InvariantCulture, "Новый найм помещения №{0}", ParentRow["id_premises"]);
                        else
                            if (v_tenancies.Position != -1)
                                Text = string.Format(CultureInfo.InvariantCulture, "Найм №{0} помещения №{1}",
                                    ((DataRowView)v_tenancies[v_tenancies.Position])["id_process"], ParentRow["id_premises"]);
                            else
                                Text = string.Format(CultureInfo.InvariantCulture, "Наймы помещения №{0} отсутствуют", ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.SubPremises:
                        if (viewportState == ViewportState.NewRowState)
                            Text = string.Format(CultureInfo.InvariantCulture, "Новый найм комнаты №{0}", ParentRow["id_sub_premises"]);
                        else
                            if (v_tenancies.Position != -1)
                                Text = string.Format(CultureInfo.InvariantCulture, "Найм №{0} комнаты №{1}",
                                    ((DataRowView)v_tenancies[v_tenancies.Position])["id_process"], ParentRow["id_sub_premises"]);
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
            if ((v_tenancies.Position > -1) && ((DataRowView)v_tenancies[v_tenancies.Position])["id_warrant"] != DBNull.Value)
            {
                id_warrant = Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_warrant"], CultureInfo.InvariantCulture);
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
            var row = (v_tenancies.Position >= 0) ? (DataRowView)v_tenancies[v_tenancies.Position] : null;
            checkBoxContractEnable.Checked = (v_tenancies.Position >= 0) &&
                (row["registration_date"] != DBNull.Value) && (row["registration_num"] != DBNull.Value);
            checkBoxProtocolEnable.Checked = (v_tenancies.Position >= 0) && (row["protocol_date"] != DBNull.Value) && (row["protocol_num"] != DBNull.Value);
            if ((v_tenancies.Position >= 0) && (row["issue_date"] != DBNull.Value))
                dateTimePickerIssueDate.Checked = true;
            else
            {
                dateTimePickerIssueDate.Value = DateTime.Now.Date;
                dateTimePickerIssueDate.Checked = false;
            }
            if ((v_tenancies.Position >= 0) && (row["begin_date"] != DBNull.Value))
                dateTimePickerBeginDate.Checked = true;
            else
            {
                dateTimePickerBeginDate.Value = DateTime.Now.Date;
                dateTimePickerBeginDate.Checked = false;
            }
            if ((v_tenancies.Position >= 0) && (row["end_date"] != DBNull.Value))
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
            comboBoxRentType.DataBindings.Add("SelectedValue", v_tenancies, "id_rent_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxRegistrationNumber.DataBindings.Clear();
            textBoxRegistrationNumber.DataBindings.Add("Text", v_tenancies, "registration_num", true, DataSourceUpdateMode.Never, "");

            dateTimePickerRegistrationDate.DataBindings.Clear();
            dateTimePickerRegistrationDate.DataBindings.Add("Value", v_tenancies, "registration_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            dateTimePickerIssueDate.DataBindings.Clear();
            dateTimePickerIssueDate.DataBindings.Add("Value", v_tenancies, "issue_date", true, DataSourceUpdateMode.Never, null);

            dateTimePickerBeginDate.DataBindings.Clear();
            dateTimePickerBeginDate.DataBindings.Add("Value", v_tenancies, "begin_date", true, DataSourceUpdateMode.Never, null);

            dateTimePickerEndDate.DataBindings.Clear();
            dateTimePickerEndDate.DataBindings.Add("Value", v_tenancies, "end_date", true, DataSourceUpdateMode.Never, null);

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
            textBoxProtocolNumber.DataBindings.Add("Text", v_tenancies, "protocol_num", true, DataSourceUpdateMode.Never, "");

            dateTimePickerProtocolDate.DataBindings.Clear();
            dateTimePickerProtocolDate.DataBindings.Add("Value", v_tenancies, "protocol_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", v_tenancies, "description", true, DataSourceUpdateMode.Never, "");

            comboBoxExecutor.DataSource = v_executors;
            comboBoxExecutor.ValueMember = "id_executor";
            comboBoxExecutor.DisplayMember = "executor_name";
            comboBoxExecutor.DataBindings.Clear();
            comboBoxExecutor.DataBindings.Add("SelectedValue", v_tenancies, "id_executor", true, DataSourceUpdateMode.Never, DBNull.Value);
        }

        private bool ChangeViewportStateTo(ViewportState state)
        {
            if (!AccessControl.HasPrivelege(Priveleges.TenancyWrite))
            {
                viewportState = ViewportState.ReadState;
                return true;
            }
            switch (state)
            {
                case ViewportState.ReadState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            return true;
                        case ViewportState.NewRowState:
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show("Сохранить изменения о процессе найма в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else return false;
                            if (viewportState == ViewportState.ReadState)
                                return true;
                            else
                                return false;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            if (tenancies.EditingNewRecord)
                                return false;
                            else
                            {
                                viewportState = ViewportState.NewRowState;
                                return true;
                            }
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show("Сохранить изменения о процессе найма в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.NewRowState);
                            else
                                return false;
                    }
                    break;
                case ViewportState.ModifyRowState: ;
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            var result = MessageBox.Show("Сохранить изменения о помещениях в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.ModifyRowState);
                            else
                                return false;
                    }
                    break;
            }
            return false;
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if (!ContainsFocus)
                return;
            if ((v_tenancies.Position != -1) && (TenancyFromView() != TenancyFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                    viewportState = ViewportState.ModifyRowState;
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                    viewportState = ViewportState.ReadState;
            }
            if (Selected)
                MenuCallback.EditingStateUpdate();
        }

        internal void LocateTenancyBy(int id)
        {
            var Position = v_tenancies.Find("id_process", id);
            is_editable = false;
            if (Position > 0)
                v_tenancies.Position = Position;
            is_editable = true;
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
            var tenancyFromView = TenancyFromView();
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
                                var idBuilding = (int)PremisesDataModel.GetInstance().Select().Rows.Find((int)ParentRow["id_premises"])["id_building"];
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

        private TenancyProcess TenancyFromView()
        {
            var tenancy = new TenancyProcess();
            var row = (DataRowView)v_tenancies[v_tenancies.Position];
            tenancy.IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            tenancy.IdRentType = ViewportHelper.ValueOrNull<int>(row, "id_rent_type");
            tenancy.IdWarrant = ViewportHelper.ValueOrNull<int>(row, "id_warrant");
            tenancy.IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor");
            tenancy.RegistrationNum = ViewportHelper.ValueOrNull(row, "registration_num");
            tenancy.RegistrationDate = ViewportHelper.ValueOrNull<DateTime>(row, "registration_date");
            tenancy.IssueDate = ViewportHelper.ValueOrNull<DateTime>(row, "issue_date");
            tenancy.BeginDate = ViewportHelper.ValueOrNull<DateTime>(row, "begin_date");
            tenancy.EndDate = ViewportHelper.ValueOrNull<DateTime>(row, "end_date");
            tenancy.ResidenceWarrantNum = ViewportHelper.ValueOrNull(row, "residence_warrant_num");
            tenancy.ResidenceWarrantDate = ViewportHelper.ValueOrNull<DateTime>(row, "residence_warrant_date");
            tenancy.ProtocolNum = ViewportHelper.ValueOrNull(row, "protocol_num");
            tenancy.ProtocolDate = ViewportHelper.ValueOrNull<DateTime>(row, "protocol_date");
            tenancy.Description = ViewportHelper.ValueOrNull(row, "description");     
            return tenancy;
        }

        private TenancyProcess TenancyFromViewport()
        {
            var tenancy = new TenancyProcess();
            if (v_tenancies.Position == -1)
                tenancy.IdProcess = null;
            else
                tenancy.IdProcess = ViewportHelper.ValueOrNull<int>((DataRowView)v_tenancies[v_tenancies.Position], "id_process"); 
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
                tenancy.EndDate = ViewportHelper.ValueOrNull(dateTimePickerEndDate);
            }
            else
            {
                tenancy.IdWarrant = null;
                tenancy.RegistrationNum = null;
                tenancy.RegistrationDate = null;
                tenancy.IssueDate = null;
                tenancy.BeginDate = null;
                tenancy.EndDate = null;
            }
            // Отклики из прошлого, раньше была возможность менять ордер на вкладке процесса найма, убрано из-за плохой согласованности с основаниями найма
            var row = (DataRowView)v_tenancies[v_tenancies.Position];
            if (viewportState != ViewportState.NewRowState)
            {
                var reasons =
                    (from reason_row in DataModelHelper.FilterRows(TenancyReasonsDataModel.GetInstance().Select())
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
            dateTimePickerEndDate.Checked = (tenancy.EndDate != null);
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
            row["residence_warrant_num"] = ViewportHelper.ValueOrDBNull(tenancy.ResidenceWarrantNum);
            row["residence_warrant_date"] = ViewportHelper.ValueOrDBNull(tenancy.ResidenceWarrantDate);
            row["protocol_num"] = ViewportHelper.ValueOrDBNull(tenancy.ProtocolNum);
            row["protocol_date"] = ViewportHelper.ValueOrDBNull(tenancy.ProtocolDate);
            row["id_executor"] = ViewportHelper.ValueOrDBNull(tenancy.IdExecutor);
            row["description"] = ViewportHelper.ValueOrDBNull(tenancy.Description);
            row.EndEdit();
        }

        public override int GetRecordCount()
        {
            return v_tenancies.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancies.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancies.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancies.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancies.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_tenancies.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_tenancies.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_tenancies.Position > -1) && (v_tenancies.Position < (v_tenancies.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_tenancies.Position > -1) && (v_tenancies.Position < (v_tenancies.Count - 1));
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
            tenancies = TenancyProcessesDataModel.GetInstance();
            executors = ExecutorsDataModel.GetInstance();
            rent_types = RentTypesDataModel.GetInstance();
            tenancy_agreements = TenancyAgreementsDataModel.GetInstance();
            warrants = WarrantsDataModel.GetInstance();
            tenancy_persons = TenancyPersonsDataModel.GetInstance();
            tenancy_reasons = TenancyReasonsDataModel.GetInstance();
            kinships = KinshipsDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            tenancies.Select();
            executors.Select();
            rent_types.Select();
            tenancy_agreements.Select();
            warrants.Select();
            tenancy_persons.Select();
            tenancy_reasons.Select();
            kinships.Select();

            var ds = DataSetManager.DataSet;

            v_executors = new BindingSource();
            v_executors.DataMember = "executors";
            v_executors.DataSource = ds;
            v_executors.Filter = "is_inactive = 0";

            v_rent_types = new BindingSource();
            v_rent_types.DataMember = "rent_types";
            v_rent_types.DataSource = ds;

            v_kinships = new BindingSource();
            v_kinships.DataMember = "kinships";
            v_kinships.DataSource = ds;

            v_warrants = new BindingSource();
            v_warrants.DataMember = "warrants";
            v_warrants.DataSource = ds;

            v_tenancies = new BindingSource();
            v_tenancies.CurrentItemChanged += v_tenancies_CurrentItemChanged;
            v_tenancies.DataMember = "tenancy_processes";
            v_tenancies.DataSource = ds;
            RebuildStaticFilter();
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_tenancies.Filter += " AND ";
            v_tenancies.Filter += DynamicFilter;

            v_tenancy_persons = new BindingSource();
            v_tenancy_persons.DataMember = "tenancy_processes_tenancy_persons";
            v_tenancy_persons.DataSource = v_tenancies;

            v_tenancy_agreements = new BindingSource();
            v_tenancy_agreements.DataMember = "tenancy_processes_tenancy_agreements";
            v_tenancy_agreements.DataSource = v_tenancies;

            v_tenancy_reasons = new BindingSource();
            v_tenancy_reasons.DataMember = "tenancy_processes_tenancy_reasons";
            v_tenancy_reasons.DataSource = v_tenancies;

            v_tenancy_addresses = new BindingSource();
            v_tenancy_addresses.DataSource = CalcDataModelTenancyPremisesInfo.GetInstance().Select();

            DataBind();

            tenancy_persons.Select().RowChanged += TenancyPersons_RowChanged;
            tenancy_persons.Select().RowDeleted += TenancyPersons_RowDeleted;
            tenancies.Select().RowChanged += TenancyViewport_RowChanged;
            tenancies.Select().RowDeleted += TenancyViewport_RowDeleted;
            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        tenancy_building_assoc = TenancyBuildingsAssocDataModel.GetInstance();
                        tenancy_building_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_building_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    case ParentTypeEnum.Premises:
                        tenancy_premises_assoc = TenancyPremisesAssocDataModel.GetInstance();
                        tenancy_premises_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_premises_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    case ParentTypeEnum.SubPremises:
                        tenancy_sub_premises_assoc = TenancySubPremisesAssocDataModel.GetInstance();
                        tenancy_sub_premises_assoc.Select().RowChanged += TenancyAssocViewport_RowChanged;
                        tenancy_sub_premises_assoc.Select().RowDeleted += TenancyAssocViewport_RowDeleted;
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
            v_tenancy_persons.ListChanged += v_persons_ListChanged;
            
            FiltersRebuild();
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool CanInsertRecord()
        {
            return (!tenancies.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            v_tenancies.AddNew();
            var index = v_executors.Find("executor_login", WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)v_executors[index])["id_executor"];
            is_copy = false;
            id_copy_process = null;
            is_editable = true;
            tenancies.EditingNewRecord = true;
            UnbindedCheckBoxesUpdate();
        }

        public override bool CanCopyRecord()
        {
            return (v_tenancies.Position != -1) && (!tenancies.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var tenancy = TenancyFromView();
            v_tenancies.AddNew();
            tenancies.EditingNewRecord = true;
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
            v_tenancies.Filter = Filter;
            is_editable = true;
        }

        public override void ClearSearch()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_tenancies.Filter = StaticFilter;
            is_editable = true;
            DynamicFilter = "";
        }

        public override bool CanDeleteRecord()
        {
            return (v_tenancies.Position > -1)
                && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот процесс найма?", "Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (TenancyProcessesDataModel.Delete((int)((DataRowView)v_tenancies.Current)["id_process"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_tenancies[v_tenancies.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
                if (CalcDataModelPremisesTenanciesInfo.HasInstance())
                    CalcDataModelPremisesTenanciesInfo.GetInstance().Refresh(EntityType.Unknown, null, true);
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
                    tenancies.EditingNewRecord = false;
                    if (v_tenancies.Position != -1)
                    {
                        is_editable = false;
                        ((DataRowView)v_tenancies[v_tenancies.Position]).Delete();
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
            var tenancy = TenancyFromViewport();
            if (!ValidateTenancy(tenancy))
                return;
            var Filter = "";
            if (!string.IsNullOrEmpty(v_tenancies.Filter))
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
                    var id_process = TenancyProcessesDataModel.Insert(tenancy);
                    if (id_process == -1)
                    {
                        tenancies.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    tenancy.IdProcess = id_process;
                    is_editable = false;
                    if (v_tenancies.Position == -1)
                        newRow = (DataRowView)v_tenancies.AddNew();
                    else
                        newRow = ((DataRowView)v_tenancies[v_tenancies.Position]);
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", tenancy.IdProcess);
                    v_tenancies.Filter += Filter;
                    FillRowFromTenancy(tenancy, newRow);
                    // Если производится копирование, а не создание новой записи, то надо скопировать участников найма и нанимаемое жилье
                    if (is_copy && id_copy_process != null)
                    {
                        if (!CopyTenancyProcessRelData(id_process, id_copy_process.Value))
                            MessageBox.Show("Произошла ошибка во время копирования данных", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.TenancyProcess, id_process, false);
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
                                    var tenancy_buildings = TenancyBuildingsAssocDataModel.GetInstance();
                                    to.IdObject = Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture);
                                    tenancy_buildings.EditingNewRecord = true;
                                    id_assoc = TenancyBuildingsAssocDataModel.Insert(to);
                                    if (id_assoc == -1)
                                        return;
                                    to.IdAssoc = id_assoc;
                                    tenancy_buildings.Select().Rows.Add(id_assoc, to.IdObject, to.IdProcess, to.RentTotalArea, to.RentLivingArea, 0);
                                    tenancy_buildings.EditingNewRecord = false;
                                    break;
                                case ParentTypeEnum.Premises:
                                    var tenancy_premises = TenancyPremisesAssocDataModel.GetInstance();
                                    to.IdObject = Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture);
                                    tenancy_premises.EditingNewRecord = true;
                                    id_assoc = TenancyPremisesAssocDataModel.Insert(to);
                                    if (id_assoc == -1)
                                        return;
                                    to.IdAssoc = id_assoc;
                                    tenancy_premises.Select().Rows.Add(id_assoc, to.IdObject, to.IdProcess, to.RentTotalArea, to.RentLivingArea, 0);
                                    tenancy_premises.EditingNewRecord = false;
                                    break;
                                case ParentTypeEnum.SubPremises:
                                    var tenancy_sub_premises = TenancySubPremisesAssocDataModel.GetInstance();
                                    to.IdObject = Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture);
                                    tenancy_sub_premises.EditingNewRecord = true;
                                    id_assoc = TenancySubPremisesAssocDataModel.Insert(to);
                                    if (id_assoc == -1)
                                        return;
                                    to.IdAssoc = id_assoc;
                                    tenancy_sub_premises.Select().Rows.Add(id_assoc, to.IdObject, to.IdProcess, to.RentTotalArea, 0);
                                    tenancy_sub_premises.EditingNewRecord = false;
                                    break;
                                default: throw new ViewportException("Неизвестный тип родительского объекта");
                            }
                            CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.TenancyProcess, id_process, true);
                        }
                    // Обновляем информацию по помещениям (живое обновление не реализуемо)
                    if (v_tenancy_addresses != null)
                    {
                        v_tenancy_addresses.DataSource = CalcDataModelTenancyPremisesInfo.GetInstance().Select();
                        FiltersRebuild();
                    }
                    tenancies.EditingNewRecord = false;
                    RebuildStaticFilter();
                    v_tenancies.Position = v_tenancies.Count - 1;
                    break;
                case ViewportState.ModifyRowState:
                    if (tenancy.IdProcess == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить процесс найма без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (TenancyProcessesDataModel.Update(tenancy) == -1)
                        return;
                    var row = ((DataRowView)v_tenancies[v_tenancies.Position]);
                    is_editable = false;
                    Filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", tenancy.IdProcess);
                    v_tenancies.Filter += Filter;
                    FillRowFromTenancy(tenancy, row);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            if (CalcDataModelPremisesTenanciesInfo.HasInstance())
                CalcDataModelPremisesTenanciesInfo.GetInstance().Refresh(EntityType.Unknown, null, true);
        }

        // Метод копирует зависимые данные по процессу найма
        private bool CopyTenancyProcessRelData(int id_new_process, int id_copy_process)
        {
            var persons = from persons_row in DataModelHelper.FilterRows(TenancyPersonsDataModel.GetInstance().Select())
                          where persons_row.Field<int>("id_process") == id_copy_process
                          select persons_row;
            TenancyPersonsDataModel.GetInstance().EditingNewRecord = true;
            foreach (var person_row in persons.ToList())
            {
                var person = DataRowToPerson(person_row);
                person.IdProcess = id_new_process;
                var id_person = TenancyPersonsDataModel.Insert(person);
                if (id_person == -1)
                {
                    TenancyPersonsDataModel.GetInstance().EditingNewRecord = false;
                    return false;
                }
                person.IdPerson = id_person;
                TenancyPersonsDataModel.GetInstance().Select().Rows.Add(PersonToObjectArray(person));
            }
            TenancyPersonsDataModel.GetInstance().EditingNewRecord = false;
            var buildings = from row in DataModelHelper.FilterRows(TenancyBuildingsAssocDataModel.GetInstance().Select())
                            where row.Field<int>("id_process") == id_copy_process
                            select row;
            TenancyBuildingsAssocDataModel.GetInstance().EditingNewRecord = true;
            foreach (var row in buildings.ToList())
            {
                var obj = new TenancyObject();
                obj.IdObject = row.Field<int?>("id_building");
                obj.IdProcess = id_new_process;
                obj.RentLivingArea = row.Field<double?>("rent_living_area");
                obj.RentTotalArea = row.Field<double?>("rent_total_area");
                var id_assoc = TenancyBuildingsAssocDataModel.Insert(obj);
                if (id_assoc == -1)
                {
                    TenancyBuildingsAssocDataModel.GetInstance().EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = id_assoc;
                TenancyBuildingsAssocDataModel.GetInstance().Select().Rows.Add(obj.IdAssoc, obj.IdObject, obj.IdProcess,
                    obj.RentTotalArea, obj.RentLivingArea);
            }
            TenancyBuildingsAssocDataModel.GetInstance().EditingNewRecord = false;
            var premises = from row in DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select())
                            where row.Field<int>("id_process") == id_copy_process
                           select row;
            TenancyPremisesAssocDataModel.GetInstance().EditingNewRecord = true;
            foreach (var row in premises.ToList())
            {
                var obj = new TenancyObject();
                obj.IdObject = row.Field<int?>("id_premises");
                obj.IdProcess = id_new_process;
                obj.RentLivingArea = row.Field<double?>("rent_living_area");
                obj.RentTotalArea = row.Field<double?>("rent_total_area");
                var id_assoc = TenancyPremisesAssocDataModel.Insert(obj);
                if (id_assoc == -1)
                {
                    TenancyPremisesAssocDataModel.GetInstance().EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = id_assoc;
                TenancyPremisesAssocDataModel.GetInstance().Select().Rows.Add(obj.IdAssoc, obj.IdObject, obj.IdProcess,
                    obj.RentTotalArea, obj.RentLivingArea);
            }
            TenancyPremisesAssocDataModel.GetInstance().EditingNewRecord = false;
            var sub_premises = from row in DataModelHelper.FilterRows(TenancySubPremisesAssocDataModel.GetInstance().Select())
                           where row.Field<int>("id_process") == id_copy_process
                               select row;
            TenancySubPremisesAssocDataModel.GetInstance().EditingNewRecord = true;
            foreach (var row in sub_premises.ToList())
            {
                var obj = new TenancyObject();
                obj.IdObject = row.Field<int?>("id_sub_premises");
                obj.IdProcess = id_new_process;
                obj.RentTotalArea = row.Field<double?>("rent_total_area");
                var id_assoc = TenancySubPremisesAssocDataModel.Insert(obj);
                if (id_assoc == -1)
                {
                    TenancySubPremisesAssocDataModel.GetInstance().EditingNewRecord = false;
                    return false;
                }
                obj.IdAssoc = id_assoc;
                TenancySubPremisesAssocDataModel.GetInstance().Select().Rows.Add(obj.IdAssoc, obj.IdObject, obj.IdProcess,
                    obj.RentTotalArea);
            }
            TenancySubPremisesAssocDataModel.GetInstance().EditingNewRecord = false;
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
                person.ResidenceIdStreet,
                person.ResidenceHouse,
                person.ResidenceFlat,
                person.ResidenceRoom,
                person.PersonalAccount,
                person.IncludeDate,
                person.ExcludeDate
            };
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new TenancyViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_process"] as int?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                tenancy_persons.Select().RowChanged -= TenancyPersons_RowChanged;
                tenancy_persons.Select().RowDeleted -= TenancyPersons_RowDeleted;
                tenancies.Select().RowChanged -= TenancyViewport_RowChanged;
                tenancies.Select().RowDeleted -= TenancyViewport_RowDeleted;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                tenancies.EditingNewRecord = false;
            tenancy_persons.Select().RowChanged -= TenancyPersons_RowChanged;
            tenancy_persons.Select().RowDeleted -= TenancyPersons_RowDeleted;
            tenancies.Select().RowChanged -= TenancyViewport_RowChanged;
            tenancies.Select().RowDeleted -= TenancyViewport_RowDeleted;
            Close();
        }

        public override bool HasAssocTenancyPersons()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocTenancyReasons()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocTenancyObjects()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasAssocTenancyAgreements()
        {
            return (v_tenancies.Position > -1);
        }

        public override void ShowTenancyPersons()
        {
            ShowAssocViewport(ViewportType.TenancyPersonsViewport);
        }

        public override void ShowTenancyReasons()
        {
            ShowAssocViewport(ViewportType.TenancyReasonsViewport);
        }

        public override void ShowTenancyAgreements()
        {
            ShowAssocViewport(ViewportType.TenancyAgreementsViewport);
        }

        public override void ShowTenancyBuildings()
        {
            ShowAssocViewport(ViewportType.TenancyBuildingsViewport);
        }

        public override void ShowTenancyPremises()
        {
            ShowAssocViewport(ViewportType.TenancyPremisesViewport);
        }

        public override void ShowClaims()
        {
            ShowAssocViewport(ViewportType.ClaimListViewport);
        }

        private Viewport ShowAssocViewport(ViewportType viewportType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return null;
            if (v_tenancies.Position == -1)
            {
                MessageBox.Show("Не выбран процесс найма", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return null;
            }
            return ShowAssocViewport(MenuCallback, viewportType,
                "id_process = " + Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_process"], CultureInfo.InvariantCulture),
                ((DataRowView)v_tenancies[v_tenancies.Position]).Row,
                ParentTypeEnum.Tenancy);
        }

        public override bool HasTenancyContract17xReport()
        {
            return (v_tenancies.Position > -1) && (((DataRowView)v_tenancies[v_tenancies.Position])["id_rent_type"] != DBNull.Value) &&
                Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_rent_type"], CultureInfo.InvariantCulture) == 2;
        }

        public override bool HasTenancyContractReport()
        {
            return (v_tenancies.Position > -1) && (((DataRowView)v_tenancies[v_tenancies.Position])["id_rent_type"] != DBNull.Value) && 
                Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_rent_type"], CultureInfo.InvariantCulture) != 2;
        }

        public override bool HasTenancyActReport()
        {
            return (v_tenancies.Position > -1);
        }

        public override bool HasTenancyAgreementReport()
        {
            return (v_tenancies.Position > -1) && (((DataRowView)v_tenancies[v_tenancies.Position])["id_process"] != DBNull.Value) &&
                (DataModelHelper.TenancyAgreementsForProcess(
                    Convert.ToInt32(((DataRowView)v_tenancies[v_tenancies.Position])["id_process"], CultureInfo.InvariantCulture)) > 0);
        }

        public override void TenancyContract17xReportGenerate(TenancyContractTypes tenancyContractType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate())
                return;
            var row = (DataRowView)v_tenancies[v_tenancies.Position];
            if (ViewportHelper.ValueOrNull<int>(row, "id_rent_type") != 2)
            {
                MessageBox.Show("Для формирования договора по формам 1711 и 1712 необходимо, чтобы тип найма был - специализированный", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (tenancyContractType == TenancyContractTypes.SpecialContract1711Form)
                ReporterFactory.CreateReporter(ReporterType.TenancyContractSpecial1711Reporter).
                    Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
            else
                if (tenancyContractType == TenancyContractTypes.SpecialContract1712Form)
                    ReporterFactory.CreateReporter(ReporterType.TenancyContractSpecial1712Reporter).
                        Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
        }

        public override void TenancyContractReportGenerate()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate())
                return;
            var row = (DataRowView)v_tenancies[v_tenancies.Position];
            if (ViewportHelper.ValueOrNull<int>(row, "id_rent_type") == 2)
                MessageBox.Show("Для формирования договора специализированного найма необходимо выбрать форму договора: 1711 или 1712", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            else
                if (ViewportHelper.ValueOrNull<int>(row, "id_rent_type") == 1)
                    ReporterFactory.CreateReporter(ReporterType.TenancyContractCommercialReporter).
                        Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
                else
                    if (ViewportHelper.ValueOrNull<int>(row, "id_rent_type") == 3)
                        ReporterFactory.CreateReporter(ReporterType.TenancyContractSocialReporter).
                            Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
        }

        public override void TenancyActReportGenerate()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate())
                return;
            var row = (DataRowView)v_tenancies[v_tenancies.Position];
            ReporterFactory.CreateReporter(ReporterType.TenancyActReporter).
                Run(new Dictionary<string, string>() { { "id_process", row["id_process"].ToString() } });
        }

        public override void TenancyAgreementReportGenerate()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate())
                return;
            if (v_tenancy_agreements.Position == -1)
            {
                MessageBox.Show("Не выбрано соглашение для печати",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var row = (DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position];
            ReporterFactory.CreateReporter(ReporterType.TenancyAgreementReporter).Run(
                new Dictionary<string, string>() { { "id_agreement", row["id_agreement"].ToString() } });
        }

        private bool TenancyValidForReportGenerate()
        {
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (v_tenancies.Position == -1)
                return false;
            var row = (DataRowView)v_tenancies[v_tenancies.Position];
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
            // Обновляем информацию по помещениям (живое обновление не реализуемо)
            if (v_tenancy_addresses != null)
            {
                v_tenancy_addresses.DataSource = CalcDataModelTenancyPremisesInfo.GetInstance().Select();
                FiltersRebuild();
            }
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
            if (v_tenancies.Position == -1)
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

        void textBoxSelectedWarrant_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
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

        void comboBoxExecutor_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerProtocolDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxProtocolNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void textBoxDescription_TextChanged_1(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerEndDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerBeginDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerIssueDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerRegistrationDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxRegistrationNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxRentType_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void checkBoxProtocolEnable_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxProtocol.Controls)
                if (control != checkBoxProtocolEnable)
                    control.Enabled = checkBoxProtocolEnable.Checked;
            CheckViewportModifications();
        }

        void checkBoxProcessEnable_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxTenancyContract.Controls)
                if (control != checkBoxContractEnable)
                    control.Enabled = checkBoxContractEnable.Checked;
            CheckViewportModifications();
        }

        private void dataGridViewTenancyAgreements_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!HasAssocTenancyReasons())
                return;
            if (e.RowIndex == -1)
                return;
            var viewport = ShowAssocViewport(ViewportType.TenancyAgreementsViewport);
            if (viewport != null)
                ((TenancyAgreementsViewport)viewport).LocateAgreementBy(
                    Convert.ToInt32(((DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position])["id_agreement"], CultureInfo.InvariantCulture));    
        }

        private void dataGridViewTenancyPersons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!HasAssocTenancyReasons())
                return;
            if (e.RowIndex == -1)
                return;
            var viewport = ShowAssocViewport(ViewportType.TenancyPersonsViewport);
            if (viewport != null)
                ((TenancyPersonsViewport)viewport).LocatePersonBy(
                    Convert.ToInt32(((DataRowView)v_tenancy_persons[v_tenancy_persons.Position])["id_person"], CultureInfo.InvariantCulture)); 
        }

        private void dataGridViewTenancyAddress_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocTenancyObjects())
                ShowAssocViewport(ViewportType.TenancyPremisesViewport);
        }

        private void dataGridViewTenancyReasons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (HasAssocTenancyReasons())
                ShowAssocViewport(ViewportType.TenancyReasonsViewport);
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var dataGridViewCellStyle3 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyViewport));
            tableLayoutPanel9 = new TableLayoutPanel();
            groupBox31 = new GroupBox();
            textBoxDescription = new TextBox();
            groupBox22 = new GroupBox();
            comboBoxExecutor = new ComboBox();
            label41 = new Label();
            comboBoxRentType = new ComboBox();
            label46 = new Label();
            groupBox1 = new GroupBox();
            dataGridViewTenancyPersons = new DataGridView();
            surname = new DataGridViewTextBoxColumn();
            name = new DataGridViewTextBoxColumn();
            patronymic = new DataGridViewTextBoxColumn();
            date_of_birth = new DataGridViewTextBoxColumn();
            id_kinship = new DataGridViewComboBoxColumn();
            groupBoxTenancyContract = new GroupBox();
            tableLayoutPanel10 = new TableLayoutPanel();
            panel6 = new Panel();
            label52 = new Label();
            label51 = new Label();
            dateTimePickerEndDate = new DateTimePicker();
            label50 = new Label();
            dateTimePickerBeginDate = new DateTimePicker();
            label49 = new Label();
            dateTimePickerIssueDate = new DateTimePicker();
            panel5 = new Panel();
            vButtonWarrant = new vButton();
            textBoxSelectedWarrant = new TextBox();
            label82 = new Label();
            label48 = new Label();
            dateTimePickerRegistrationDate = new DateTimePicker();
            textBoxRegistrationNumber = new TextBox();
            label47 = new Label();
            checkBoxContractEnable = new CheckBox();
            groupBox25 = new GroupBox();
            dataGridViewTenancyAgreements = new DataGridView();
            agreement_date = new DataGridViewTextBoxColumn();
            agreement_content = new DataGridViewTextBoxColumn();
            groupBox24 = new GroupBox();
            dataGridViewTenancyReasons = new DataGridView();
            reason_prepared = new DataGridViewTextBoxColumn();
            reason_number = new DataGridViewTextBoxColumn();
            reason_date = new DataGridViewTextBoxColumn();
            groupBoxProtocol = new GroupBox();
            label45 = new Label();
            dateTimePickerProtocolDate = new DateTimePicker();
            label42 = new Label();
            textBoxProtocolNumber = new TextBox();
            checkBoxProtocolEnable = new CheckBox();
            groupBox21 = new GroupBox();
            dataGridViewTenancyAddress = new DataGridView();
            address = new DataGridViewTextBoxColumn();
            total_area = new DataGridViewTextBoxColumn();
            living_area = new DataGridViewTextBoxColumn();
            rent_area = new DataGridViewTextBoxColumn();
            tableLayoutPanel9.SuspendLayout();
            groupBox31.SuspendLayout();
            groupBox22.SuspendLayout();
            groupBox1.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyPersons)).BeginInit();
            groupBoxTenancyContract.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            panel6.SuspendLayout();
            panel5.SuspendLayout();
            groupBox25.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyAgreements)).BeginInit();
            groupBox24.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyReasons)).BeginInit();
            groupBoxProtocol.SuspendLayout();
            groupBox21.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyAddress)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel9
            // 
            tableLayoutPanel9.ColumnCount = 2;
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.Controls.Add(groupBox31, 0, 3);
            tableLayoutPanel9.Controls.Add(groupBox22, 0, 0);
            tableLayoutPanel9.Controls.Add(groupBox1, 1, 3);
            tableLayoutPanel9.Controls.Add(groupBoxTenancyContract, 0, 1);
            tableLayoutPanel9.Controls.Add(groupBox25, 1, 2);
            tableLayoutPanel9.Controls.Add(groupBox24, 0, 2);
            tableLayoutPanel9.Controls.Add(groupBoxProtocol, 1, 0);
            tableLayoutPanel9.Controls.Add(groupBox21, 0, 4);
            tableLayoutPanel9.Dock = DockStyle.Fill;
            tableLayoutPanel9.Location = new Point(3, 3);
            tableLayoutPanel9.Name = "tableLayoutPanel9";
            tableLayoutPanel9.RowCount = 5;
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F));
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Absolute, 115F));
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F));
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.RowStyles.Add(new RowStyle());
            tableLayoutPanel9.Size = new Size(867, 502);
            tableLayoutPanel9.TabIndex = 0;
            // 
            // groupBox31
            // 
            groupBox31.Controls.Add(textBoxDescription);
            groupBox31.Dock = DockStyle.Fill;
            groupBox31.Location = new Point(3, 311);
            groupBox31.Name = "groupBox31";
            groupBox31.Size = new Size(427, 79);
            groupBox31.TabIndex = 6;
            groupBox31.TabStop = false;
            groupBox31.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.Location = new Point(3, 17);
            textBoxDescription.MaxLength = 4000;
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(421, 59);
            textBoxDescription.TabIndex = 0;
            textBoxDescription.TextChanged += textBoxDescription_TextChanged_1;
            textBoxDescription.Enter += selectAll_Enter;
            // 
            // groupBox22
            // 
            groupBox22.Controls.Add(comboBoxExecutor);
            groupBox22.Controls.Add(label41);
            groupBox22.Controls.Add(comboBoxRentType);
            groupBox22.Controls.Add(label46);
            groupBox22.Dock = DockStyle.Fill;
            groupBox22.Location = new Point(3, 3);
            groupBox22.Name = "groupBox22";
            groupBox22.Size = new Size(427, 79);
            groupBox22.TabIndex = 0;
            groupBox22.TabStop = false;
            groupBox22.Text = "Общие сведения";
            // 
            // comboBoxExecutor
            // 
            comboBoxExecutor.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                      | AnchorStyles.Right;
            comboBoxExecutor.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxExecutor.FormattingEnabled = true;
            comboBoxExecutor.Location = new Point(168, 48);
            comboBoxExecutor.Name = "comboBoxExecutor";
            comboBoxExecutor.Size = new Size(250, 23);
            comboBoxExecutor.TabIndex = 1;
            comboBoxExecutor.SelectedValueChanged += comboBoxExecutor_SelectedValueChanged;
            // 
            // label41
            // 
            label41.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                             | AnchorStyles.Right;
            label41.AutoSize = true;
            label41.Location = new Point(12, 51);
            label41.Name = "label41";
            label41.Size = new Size(141, 15);
            label41.TabIndex = 1;
            label41.Text = "Составитель договора";
            // 
            // comboBoxRentType
            // 
            comboBoxRentType.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                      | AnchorStyles.Right;
            comboBoxRentType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxRentType.FormattingEnabled = true;
            comboBoxRentType.Location = new Point(168, 19);
            comboBoxRentType.Name = "comboBoxRentType";
            comboBoxRentType.Size = new Size(250, 23);
            comboBoxRentType.TabIndex = 0;
            comboBoxRentType.SelectedValueChanged += comboBoxRentType_SelectedValueChanged;
            // 
            // label46
            // 
            label46.AutoSize = true;
            label46.Location = new Point(12, 22);
            label46.Name = "label46";
            label46.Size = new Size(108, 15);
            label46.TabIndex = 16;
            label46.Text = "Тип найма жилья";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(dataGridViewTenancyPersons);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(436, 311);
            groupBox1.Name = "groupBox1";
            tableLayoutPanel9.SetRowSpan(groupBox1, 2);
            groupBox1.Size = new Size(428, 188);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Участники найма";
            // 
            // dataGridViewTenancyPersons
            // 
            dataGridViewTenancyPersons.AllowUserToAddRows = false;
            dataGridViewTenancyPersons.AllowUserToDeleteRows = false;
            dataGridViewTenancyPersons.AllowUserToResizeRows = false;
            dataGridViewTenancyPersons.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTenancyPersons.BackgroundColor = Color.White;
            dataGridViewTenancyPersons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTenancyPersons.Columns.AddRange(surname, name, patronymic, date_of_birth, id_kinship);
            dataGridViewTenancyPersons.Dock = DockStyle.Fill;
            dataGridViewTenancyPersons.Location = new Point(3, 17);
            dataGridViewTenancyPersons.MultiSelect = false;
            dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            dataGridViewTenancyPersons.ReadOnly = true;
            dataGridViewTenancyPersons.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyPersons.Size = new Size(422, 168);
            dataGridViewTenancyPersons.TabIndex = 0;
            dataGridViewTenancyPersons.CellDoubleClick += dataGridViewTenancyPersons_CellDoubleClick;
            // 
            // surname
            // 
            surname.HeaderText = "Фамилия";
            surname.MinimumWidth = 100;
            surname.Name = "surname";
            surname.ReadOnly = true;
            // 
            // name
            // 
            name.HeaderText = "Имя";
            name.MinimumWidth = 100;
            name.Name = "name";
            name.ReadOnly = true;
            // 
            // patronymic
            // 
            patronymic.HeaderText = "Отчество";
            patronymic.MinimumWidth = 100;
            patronymic.Name = "patronymic";
            patronymic.ReadOnly = true;
            // 
            // date_of_birth
            // 
            date_of_birth.HeaderText = "Дата рождения";
            date_of_birth.MinimumWidth = 130;
            date_of_birth.Name = "date_of_birth";
            date_of_birth.ReadOnly = true;
            // 
            // id_kinship
            // 
            id_kinship.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_kinship.HeaderText = "Отношение/связь";
            id_kinship.MinimumWidth = 120;
            id_kinship.Name = "id_kinship";
            id_kinship.ReadOnly = true;
            // 
            // groupBoxTenancyContract
            // 
            tableLayoutPanel9.SetColumnSpan(groupBoxTenancyContract, 2);
            groupBoxTenancyContract.Controls.Add(tableLayoutPanel10);
            groupBoxTenancyContract.Controls.Add(checkBoxContractEnable);
            groupBoxTenancyContract.Dock = DockStyle.Fill;
            groupBoxTenancyContract.Location = new Point(3, 88);
            groupBoxTenancyContract.Name = "groupBoxTenancyContract";
            groupBoxTenancyContract.Size = new Size(861, 109);
            groupBoxTenancyContract.TabIndex = 2;
            groupBoxTenancyContract.TabStop = false;
            groupBoxTenancyContract.Text = "      Договор найма";
            // 
            // tableLayoutPanel10
            // 
            tableLayoutPanel10.ColumnCount = 2;
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Controls.Add(panel6, 1, 0);
            tableLayoutPanel10.Controls.Add(panel5, 0, 0);
            tableLayoutPanel10.Dock = DockStyle.Fill;
            tableLayoutPanel10.Location = new Point(3, 17);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            tableLayoutPanel10.RowCount = 1;
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            tableLayoutPanel10.Size = new Size(855, 89);
            tableLayoutPanel10.TabIndex = 1;
            // 
            // panel6
            // 
            panel6.Controls.Add(label52);
            panel6.Controls.Add(label51);
            panel6.Controls.Add(dateTimePickerEndDate);
            panel6.Controls.Add(label50);
            panel6.Controls.Add(dateTimePickerBeginDate);
            panel6.Controls.Add(label49);
            panel6.Controls.Add(dateTimePickerIssueDate);
            panel6.Dock = DockStyle.Fill;
            panel6.Location = new Point(427, 0);
            panel6.Margin = new Padding(0);
            panel6.Name = "panel6";
            panel6.Size = new Size(428, 90);
            panel6.TabIndex = 1;
            // 
            // label52
            // 
            label52.AutoSize = true;
            label52.Location = new Point(150, 65);
            label52.Name = "label52";
            label52.Size = new Size(21, 15);
            label52.TabIndex = 28;
            label52.Text = "по";
            // 
            // label51
            // 
            label51.AutoSize = true;
            label51.Location = new Point(158, 36);
            label51.Name = "label51";
            label51.Size = new Size(13, 15);
            label51.TabIndex = 27;
            label51.Text = "с";
            // 
            // dateTimePickerEndDate
            // 
            dateTimePickerEndDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            dateTimePickerEndDate.Location = new Point(174, 62);
            dateTimePickerEndDate.Name = "dateTimePickerEndDate";
            dateTimePickerEndDate.ShowCheckBox = true;
            dateTimePickerEndDate.Size = new Size(246, 21);
            dateTimePickerEndDate.TabIndex = 2;
            dateTimePickerEndDate.ValueChanged += dateTimePickerEndDate_ValueChanged;
            // 
            // label50
            // 
            label50.AutoSize = true;
            label50.Location = new Point(15, 36);
            label50.Name = "label50";
            label50.Size = new Size(93, 15);
            label50.TabIndex = 25;
            label50.Text = "Срок действия";
            // 
            // dateTimePickerBeginDate
            // 
            dateTimePickerBeginDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            dateTimePickerBeginDate.Location = new Point(174, 33);
            dateTimePickerBeginDate.Name = "dateTimePickerBeginDate";
            dateTimePickerBeginDate.ShowCheckBox = true;
            dateTimePickerBeginDate.Size = new Size(246, 21);
            dateTimePickerBeginDate.TabIndex = 1;
            dateTimePickerBeginDate.ValueChanged += dateTimePickerBeginDate_ValueChanged;
            // 
            // label49
            // 
            label49.AutoSize = true;
            label49.Location = new Point(15, 7);
            label49.Name = "label49";
            label49.Size = new Size(83, 15);
            label49.TabIndex = 23;
            label49.Text = "Дата выдачи";
            // 
            // dateTimePickerIssueDate
            // 
            dateTimePickerIssueDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                             | AnchorStyles.Right;
            dateTimePickerIssueDate.Location = new Point(174, 4);
            dateTimePickerIssueDate.Name = "dateTimePickerIssueDate";
            dateTimePickerIssueDate.ShowCheckBox = true;
            dateTimePickerIssueDate.Size = new Size(246, 21);
            dateTimePickerIssueDate.TabIndex = 0;
            dateTimePickerIssueDate.ValueChanged += dateTimePickerIssueDate_ValueChanged;
            // 
            // panel5
            // 
            panel5.Controls.Add(vButtonWarrant);
            panel5.Controls.Add(textBoxSelectedWarrant);
            panel5.Controls.Add(label82);
            panel5.Controls.Add(label48);
            panel5.Controls.Add(dateTimePickerRegistrationDate);
            panel5.Controls.Add(textBoxRegistrationNumber);
            panel5.Controls.Add(label47);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(0, 0);
            panel5.Margin = new Padding(0);
            panel5.Name = "panel5";
            panel5.Size = new Size(427, 90);
            panel5.TabIndex = 0;
            // 
            // vButtonWarrant
            // 
            vButtonWarrant.AllowAnimations = true;
            vButtonWarrant.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonWarrant.BackColor = Color.Transparent;
            vButtonWarrant.Location = new Point(391, 62);
            vButtonWarrant.Name = "vButtonWarrant";
            vButtonWarrant.RoundedCornersMask = 15;
            vButtonWarrant.Size = new Size(27, 20);
            vButtonWarrant.TabIndex = 24;
            vButtonWarrant.Text = "...";
            vButtonWarrant.UseVisualStyleBackColor = false;
            vButtonWarrant.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonWarrant.Click += vButtonWarrant_Click;
            // 
            // textBoxSelectedWarrant
            // 
            textBoxSelectedWarrant.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            textBoxSelectedWarrant.Location = new Point(172, 62);
            textBoxSelectedWarrant.Name = "textBoxSelectedWarrant";
            textBoxSelectedWarrant.ReadOnly = true;
            textBoxSelectedWarrant.Size = new Size(213, 21);
            textBoxSelectedWarrant.TabIndex = 2;
            textBoxSelectedWarrant.TextChanged += textBoxSelectedWarrant_TextChanged;
            // 
            // label82
            // 
            label82.AutoSize = true;
            label82.Location = new Point(14, 65);
            label82.Name = "label82";
            label82.Size = new Size(92, 15);
            label82.TabIndex = 23;
            label82.Text = "Доверенность";
            // 
            // label48
            // 
            label48.AutoSize = true;
            label48.Location = new Point(14, 36);
            label48.Name = "label48";
            label48.Size = new Size(114, 15);
            label48.TabIndex = 21;
            label48.Text = "Дата регистрации";
            // 
            // dateTimePickerRegistrationDate
            // 
            dateTimePickerRegistrationDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                    | AnchorStyles.Right;
            dateTimePickerRegistrationDate.Location = new Point(172, 33);
            dateTimePickerRegistrationDate.Name = "dateTimePickerRegistrationDate";
            dateTimePickerRegistrationDate.Size = new Size(246, 21);
            dateTimePickerRegistrationDate.TabIndex = 1;
            dateTimePickerRegistrationDate.ValueChanged += dateTimePickerRegistrationDate_ValueChanged;
            // 
            // textBoxRegistrationNumber
            // 
            textBoxRegistrationNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                               | AnchorStyles.Right;
            textBoxRegistrationNumber.Location = new Point(172, 4);
            textBoxRegistrationNumber.MaxLength = 255;
            textBoxRegistrationNumber.Name = "textBoxRegistrationNumber";
            textBoxRegistrationNumber.Size = new Size(246, 21);
            textBoxRegistrationNumber.TabIndex = 0;
            textBoxRegistrationNumber.TextChanged += textBoxRegistrationNumber_TextChanged;
            textBoxRegistrationNumber.Enter += selectAll_Enter;
            // 
            // label47
            // 
            label47.AutoSize = true;
            label47.Location = new Point(14, 7);
            label47.Name = "label47";
            label47.Size = new Size(152, 15);
            label47.TabIndex = 18;
            label47.Text = "Регистрационный номер";
            // 
            // checkBoxContractEnable
            // 
            checkBoxContractEnable.AutoSize = true;
            checkBoxContractEnable.Checked = true;
            checkBoxContractEnable.CheckState = CheckState.Checked;
            checkBoxContractEnable.Location = new Point(11, 0);
            checkBoxContractEnable.Name = "checkBoxContractEnable";
            checkBoxContractEnable.Size = new Size(15, 14);
            checkBoxContractEnable.TabIndex = 0;
            checkBoxContractEnable.UseVisualStyleBackColor = true;
            checkBoxContractEnable.CheckedChanged += checkBoxProcessEnable_CheckedChanged;
            // 
            // groupBox25
            // 
            groupBox25.Controls.Add(dataGridViewTenancyAgreements);
            groupBox25.Dock = DockStyle.Fill;
            groupBox25.Location = new Point(436, 203);
            groupBox25.Name = "groupBox25";
            groupBox25.Size = new Size(428, 102);
            groupBox25.TabIndex = 4;
            groupBox25.TabStop = false;
            groupBox25.Text = "Соглашения найма";
            // 
            // dataGridViewTenancyAgreements
            // 
            dataGridViewTenancyAgreements.AllowUserToAddRows = false;
            dataGridViewTenancyAgreements.AllowUserToDeleteRows = false;
            dataGridViewTenancyAgreements.AllowUserToResizeRows = false;
            dataGridViewTenancyAgreements.BackgroundColor = Color.White;
            dataGridViewTenancyAgreements.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTenancyAgreements.Columns.AddRange(agreement_date, agreement_content);
            dataGridViewTenancyAgreements.Dock = DockStyle.Fill;
            dataGridViewTenancyAgreements.Location = new Point(3, 17);
            dataGridViewTenancyAgreements.MultiSelect = false;
            dataGridViewTenancyAgreements.Name = "dataGridViewTenancyAgreements";
            dataGridViewTenancyAgreements.ReadOnly = true;
            dataGridViewTenancyAgreements.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyAgreements.Size = new Size(422, 82);
            dataGridViewTenancyAgreements.TabIndex = 0;
            dataGridViewTenancyAgreements.CellDoubleClick += dataGridViewTenancyAgreements_CellDoubleClick;
            // 
            // agreement_date
            // 
            agreement_date.HeaderText = "Дата";
            agreement_date.Name = "agreement_date";
            agreement_date.ReadOnly = true;
            // 
            // agreement_content
            // 
            agreement_content.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            agreement_content.HeaderText = "Содержание";
            agreement_content.Name = "agreement_content";
            agreement_content.ReadOnly = true;
            // 
            // groupBox24
            // 
            groupBox24.Controls.Add(dataGridViewTenancyReasons);
            groupBox24.Dock = DockStyle.Fill;
            groupBox24.Location = new Point(3, 203);
            groupBox24.Name = "groupBox24";
            groupBox24.Size = new Size(427, 102);
            groupBox24.TabIndex = 3;
            groupBox24.TabStop = false;
            groupBox24.Text = "Основания найма";
            // 
            // dataGridViewTenancyReasons
            // 
            dataGridViewTenancyReasons.AllowUserToAddRows = false;
            dataGridViewTenancyReasons.AllowUserToDeleteRows = false;
            dataGridViewTenancyReasons.AllowUserToResizeRows = false;
            dataGridViewTenancyReasons.BackgroundColor = Color.White;
            dataGridViewTenancyReasons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTenancyReasons.Columns.AddRange(reason_prepared, reason_number, reason_date);
            dataGridViewTenancyReasons.Dock = DockStyle.Fill;
            dataGridViewTenancyReasons.Location = new Point(3, 17);
            dataGridViewTenancyReasons.MultiSelect = false;
            dataGridViewTenancyReasons.Name = "dataGridViewTenancyReasons";
            dataGridViewTenancyReasons.ReadOnly = true;
            dataGridViewTenancyReasons.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyReasons.Size = new Size(421, 82);
            dataGridViewTenancyReasons.TabIndex = 0;
            dataGridViewTenancyReasons.CellDoubleClick += dataGridViewTenancyReasons_CellDoubleClick;
            // 
            // reason_prepared
            // 
            reason_prepared.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            reason_prepared.HeaderText = "Основание";
            reason_prepared.Name = "reason_prepared";
            reason_prepared.ReadOnly = true;
            // 
            // reason_number
            // 
            reason_number.HeaderText = "№";
            reason_number.Name = "reason_number";
            reason_number.ReadOnly = true;
            // 
            // reason_date
            // 
            reason_date.HeaderText = "Дата";
            reason_date.Name = "reason_date";
            reason_date.ReadOnly = true;
            // 
            // groupBoxProtocol
            // 
            groupBoxProtocol.Controls.Add(label45);
            groupBoxProtocol.Controls.Add(dateTimePickerProtocolDate);
            groupBoxProtocol.Controls.Add(label42);
            groupBoxProtocol.Controls.Add(textBoxProtocolNumber);
            groupBoxProtocol.Controls.Add(checkBoxProtocolEnable);
            groupBoxProtocol.Dock = DockStyle.Fill;
            groupBoxProtocol.Location = new Point(436, 3);
            groupBoxProtocol.Name = "groupBoxProtocol";
            groupBoxProtocol.Size = new Size(428, 79);
            groupBoxProtocol.TabIndex = 1;
            groupBoxProtocol.TabStop = false;
            groupBoxProtocol.Text = "      Протокол жилищной комиссии";
            // 
            // label45
            // 
            label45.AutoSize = true;
            label45.Location = new Point(12, 52);
            label45.Name = "label45";
            label45.Size = new Size(102, 15);
            label45.TabIndex = 18;
            label45.Text = "Дата протокола";
            // 
            // dateTimePickerProtocolDate
            // 
            dateTimePickerProtocolDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            dateTimePickerProtocolDate.Location = new Point(171, 48);
            dateTimePickerProtocolDate.Name = "dateTimePickerProtocolDate";
            dateTimePickerProtocolDate.Size = new Size(246, 21);
            dateTimePickerProtocolDate.TabIndex = 2;
            dateTimePickerProtocolDate.ValueChanged += dateTimePickerProtocolDate_ValueChanged;
            // 
            // label42
            // 
            label42.AutoSize = true;
            label42.Location = new Point(12, 22);
            label42.Name = "label42";
            label42.Size = new Size(111, 15);
            label42.TabIndex = 12;
            label42.Text = "Номер протокола";
            // 
            // textBoxProtocolNumber
            // 
            textBoxProtocolNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            textBoxProtocolNumber.Location = new Point(171, 19);
            textBoxProtocolNumber.MaxLength = 50;
            textBoxProtocolNumber.Name = "textBoxProtocolNumber";
            textBoxProtocolNumber.Size = new Size(246, 21);
            textBoxProtocolNumber.TabIndex = 1;
            textBoxProtocolNumber.TextChanged += textBoxProtocolNumber_TextChanged;
            textBoxProtocolNumber.Enter += selectAll_Enter;
            // 
            // checkBoxProtocolEnable
            // 
            checkBoxProtocolEnable.AutoSize = true;
            checkBoxProtocolEnable.Checked = true;
            checkBoxProtocolEnable.CheckState = CheckState.Checked;
            checkBoxProtocolEnable.Location = new Point(11, 0);
            checkBoxProtocolEnable.Name = "checkBoxProtocolEnable";
            checkBoxProtocolEnable.Size = new Size(15, 14);
            checkBoxProtocolEnable.TabIndex = 0;
            checkBoxProtocolEnable.UseVisualStyleBackColor = true;
            checkBoxProtocolEnable.CheckedChanged += checkBoxProtocolEnable_CheckedChanged;
            // 
            // groupBox21
            // 
            groupBox21.Controls.Add(dataGridViewTenancyAddress);
            groupBox21.Dock = DockStyle.Fill;
            groupBox21.Location = new Point(3, 396);
            groupBox21.Name = "groupBox21";
            groupBox21.Size = new Size(427, 103);
            groupBox21.TabIndex = 7;
            groupBox21.TabStop = false;
            groupBox21.Text = "Нанимаемое жилье";
            // 
            // dataGridViewTenancyAddress
            // 
            dataGridViewTenancyAddress.AllowUserToAddRows = false;
            dataGridViewTenancyAddress.AllowUserToDeleteRows = false;
            dataGridViewTenancyAddress.AllowUserToResizeRows = false;
            dataGridViewTenancyAddress.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTenancyAddress.BackgroundColor = Color.White;
            dataGridViewTenancyAddress.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTenancyAddress.Columns.AddRange(address, total_area, living_area, rent_area);
            dataGridViewTenancyAddress.Dock = DockStyle.Fill;
            dataGridViewTenancyAddress.Location = new Point(3, 17);
            dataGridViewTenancyAddress.MultiSelect = false;
            dataGridViewTenancyAddress.Name = "dataGridViewTenancyAddress";
            dataGridViewTenancyAddress.ReadOnly = true;
            dataGridViewTenancyAddress.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyAddress.Size = new Size(421, 83);
            dataGridViewTenancyAddress.TabIndex = 0;
            dataGridViewTenancyAddress.CellDoubleClick += dataGridViewTenancyAddress_CellDoubleClick;
            // 
            // address
            // 
            address.HeaderText = "Адрес";
            address.MinimumWidth = 400;
            address.Name = "address";
            address.ReadOnly = true;
            // 
            // total_area
            // 
            dataGridViewCellStyle1.Format = "#0.0## м²";
            total_area.DefaultCellStyle = dataGridViewCellStyle1;
            total_area.HeaderText = "Общая площадь";
            total_area.MinimumWidth = 150;
            total_area.Name = "total_area";
            total_area.ReadOnly = true;
            // 
            // living_area
            // 
            dataGridViewCellStyle2.Format = "#0.0## м²";
            living_area.DefaultCellStyle = dataGridViewCellStyle2;
            living_area.HeaderText = "Жилая площадь";
            living_area.MinimumWidth = 150;
            living_area.Name = "living_area";
            living_area.ReadOnly = true;
            // 
            // rent_area
            // 
            dataGridViewCellStyle3.Format = "#0.0## м²";
            rent_area.DefaultCellStyle = dataGridViewCellStyle3;
            rent_area.HeaderText = "Площадь койко-места";
            rent_area.MinimumWidth = 200;
            rent_area.Name = "rent_area";
            rent_area.ReadOnly = true;
            // 
            // TenancyViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(720, 480);
            BackColor = Color.White;
            ClientSize = new Size(873, 508);
            Controls.Add(tableLayoutPanel9);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyViewport";
            Padding = new Padding(3);
            Text = "Процесс найма №{0}";
            tableLayoutPanel9.ResumeLayout(false);
            groupBox31.ResumeLayout(false);
            groupBox31.PerformLayout();
            groupBox22.ResumeLayout(false);
            groupBox22.PerformLayout();
            groupBox1.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewTenancyPersons)).EndInit();
            groupBoxTenancyContract.ResumeLayout(false);
            groupBoxTenancyContract.PerformLayout();
            tableLayoutPanel10.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            groupBox25.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewTenancyAgreements)).EndInit();
            groupBox24.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewTenancyReasons)).EndInit();
            groupBoxProtocol.ResumeLayout(false);
            groupBoxProtocol.PerformLayout();
            groupBox21.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewTenancyAddress)).EndInit();
            ResumeLayout(false);

        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
