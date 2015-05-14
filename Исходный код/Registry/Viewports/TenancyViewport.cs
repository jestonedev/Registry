using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.SearchForms;
using System.Drawing;
using Registry.Reporting;
using Security;
using Registry.CalcDataModels;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class TenancyViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel9;
        private TableLayoutPanel tableLayoutPanel10;
        private GroupBox groupBoxTenancyContract;
        private GroupBox groupBoxResidenceWarrant;
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
        private CheckBox checkBoxResidenceWarrantEnable;
        private CheckBox checkBoxProtocolEnable;
        private Label label42;
        private Label label43;
        private Label label44;
        private Label label45;
        private Label label47;
        private Label label48;
        private Label label49;
        private Label label50;
        private Label label51;
        private Label label52;
        private Label label82;
        private TextBox textBoxResidenceWarrantNumber;
        private TextBox textBoxProtocolNumber;
        private TextBox textBoxRegistrationNumber;
        private TextBox textBoxSelectedWarrant = new System.Windows.Forms.TextBox();
        private DateTimePicker dateTimePickerResidenceWarrantDate;
        private DateTimePicker dateTimePickerProtocolDate;
        private DateTimePicker dateTimePickerRegistrationDate;
        private DateTimePicker dateTimePickerIssueDate;
        private DateTimePicker dateTimePickerBeginDate;
        private DateTimePicker dateTimePickerEndDate;
        private VIBlend.WinForms.Controls.vButton vButtonWarrant = new VIBlend.WinForms.Controls.vButton();
        private GroupBox groupBox31;
        private TextBox textBoxDescription;
        private GroupBox groupBox22;
        private ComboBox comboBoxExecutor;
        private Label label41;
        private ComboBox comboBoxRentType;
        private Label label46;
        private GroupBox groupBox1;
        private DataGridView dataGridViewTenancyPersons;
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        private DataGridViewTextBoxColumn date_of_birth;
        private DataGridViewComboBoxColumn id_kinship;
        private DataGridViewTextBoxColumn address;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn living_area;
        #endregion Components

        #region Models
        private TenancyProcessesDataModel tenancies = null;
        private TenancyBuildingsAssocDataModel tenancy_building_assoc = null;
        private TenancyPremisesAssocDataModel tenancy_premises_assoc = null;
        private TenancySubPremisesAssocDataModel tenancy_sub_premises_assoc = null;
        private ExecutorsDataModel executors = null;
        private RentTypesDataModel rent_types = null;
        private TenancyAgreementsDataModel tenancy_agreements = null;
        private WarrantsDataModel warrants = null;
        private TenancyPersonsDataModel tenancy_persons = null;
        private TenancyReasonsDataModel tenancy_reasons = null;
        private KinshipsDataModel kinships = null;
        #endregion Models

        #region Views
        private BindingSource v_tenancies = null;
        private BindingSource v_executors = null;
        private BindingSource v_rent_types = null;
        private BindingSource v_tenancy_agreements = null;
        private BindingSource v_warrants = null;
        private BindingSource v_tenancy_persons = null;
        private BindingSource v_tenancy_addresses = null;
        private BindingSource v_tenancy_reasons = null;
        private BindingSource v_kinships = null;
        #endregion Views

        //Forms
        private SearchForm stExtendedSearchForm = null;
        private SearchForm stSimpleSearchForm = null;
        private SelectWarrantForm swForm = null;

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;
        private int? id_warrant = null;
        private bool is_copy = false;
        private int? id_copy_process = null;

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
            this.DynamicFilter = tenancyViewport.DynamicFilter;
            this.StaticFilter = tenancyViewport.StaticFilter;
            this.ParentRow = tenancyViewport.ParentRow;
            this.ParentType = tenancyViewport.ParentType;
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
                foreach (int id in ids)
                    StaticFilter += id.ToString(CultureInfo.InvariantCulture) + ",";
                StaticFilter = StaticFilter.TrimEnd(new char[] { ',' }) + ")";
            }
            v_tenancies.Filter = StaticFilter;           
        }

        private void SetViewportCaption()
        {
            if (ParentRow == null)
            {
                if (viewportState == ViewportState.NewRowState)
                    this.Text = "Новый найм";
                else
                    if (v_tenancies.Position != -1)
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Процесс найма №{0}", ((DataRowView)v_tenancies[v_tenancies.Position])["id_process"]);
                    else
                        this.Text = "Процессы отсутствуют";
            }
            else
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        if (viewportState == ViewportState.NewRowState)
                            this.Text = String.Format(CultureInfo.InvariantCulture, "Новый найм здания №{0}", ParentRow["id_building"]);
                        else
                        if (v_tenancies.Position != -1)
                            this.Text = String.Format(CultureInfo.InvariantCulture, "Найм №{0} здания №{1}", 
                                ((DataRowView)v_tenancies[v_tenancies.Position])["id_process"], ParentRow["id_building"]);
                        else
                            this.Text = String.Format(CultureInfo.InvariantCulture, "Наймы здания №{0} отсутствуют", ParentRow["id_building"]);
                        break;
                    case ParentTypeEnum.Premises:
                        if (viewportState == ViewportState.NewRowState)
                            this.Text = String.Format(CultureInfo.InvariantCulture, "Новый найм помещения №{0}", ParentRow["id_premises"]);
                        else
                            if (v_tenancies.Position != -1)
                                this.Text = String.Format(CultureInfo.InvariantCulture, "Найм №{0} помещения №{1}",
                                    ((DataRowView)v_tenancies[v_tenancies.Position])["id_process"], ParentRow["id_premises"]);
                            else
                                this.Text = String.Format(CultureInfo.InvariantCulture, "Наймы помещения №{0} отсутствуют", ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.SubPremises:
                        if (viewportState == ViewportState.NewRowState)
                            this.Text = String.Format(CultureInfo.InvariantCulture, "Новый найм комнаты №{0}", ParentRow["id_sub_premises"]);
                        else
                            if (v_tenancies.Position != -1)
                                this.Text = String.Format(CultureInfo.InvariantCulture, "Найм №{0} комнаты №{1}",
                                    ((DataRowView)v_tenancies[v_tenancies.Position])["id_process"], ParentRow["id_sub_premises"]);
                            else
                                this.Text = String.Format(CultureInfo.InvariantCulture, "Наймы комнаты №{0} отсутствуют", ParentRow["id_sub_premises"]);
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
        }

        private void RedrawDataGridRows()
        {
            if (dataGridViewTenancyPersons.Rows.Count == 0)
                return;
            for (int i = 0; i < dataGridViewTenancyPersons.Rows.Count; i++)
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
                int row_index = v_warrants.Find("id_warrant", id_warrant);
                if (row_index == -1)
                    return null;
                DateTime registration_date = Convert.ToDateTime(((DataRowView)v_warrants[row_index])["registration_date"], CultureInfo.InvariantCulture);
                string registration_num = ((DataRowView)v_warrants[row_index])["registration_num"].ToString();
                return String.Format(CultureInfo.InvariantCulture, "№ {0} от {1}", 
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
            DataRowView row = (v_tenancies.Position >= 0) ? (DataRowView)v_tenancies[v_tenancies.Position] : null;
            checkBoxContractEnable.Checked = (v_tenancies.Position >= 0) &&
                ((row["registration_date"] != DBNull.Value) || (row["registration_num"] != DBNull.Value));
            checkBoxResidenceWarrantEnable.Checked = (v_tenancies.Position >= 0) && (row["residence_warrant_date"] != DBNull.Value);
            checkBoxProtocolEnable.Checked = (v_tenancies.Position >= 0) && (row["protocol_date"] != DBNull.Value);
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

            textBoxResidenceWarrantNumber.DataBindings.Clear();
            textBoxResidenceWarrantNumber.DataBindings.Add("Text", v_tenancies, "residence_warrant_num", true, DataSourceUpdateMode.Never, "");

            dateTimePickerResidenceWarrantDate.DataBindings.Clear();
            dateTimePickerResidenceWarrantDate.DataBindings.Add("Value", v_tenancies, "residence_warrant_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

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
                            DialogResult result = MessageBox.Show("Сохранить изменения о процессе найма в базу данных?", "Внимание",
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
                            DialogResult result = MessageBox.Show("Сохранить изменения о процессе найма в базу данных?", "Внимание",
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
                            DialogResult result = MessageBox.Show("Сохранить изменения о помещениях в базу данных?", "Внимание",
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
            if (!this.ContainsFocus)
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
            int Position = v_tenancies.Find("id_process", id);
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
            if (checkBoxResidenceWarrantEnable.Checked)
            {
                if (tenancy.ResidenceWarrantNum == null)
                {
                    MessageBox.Show("Не указан номер ордера на проживание", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    textBoxResidenceWarrantNumber.Focus();
                    return false;
                }
            }
            TenancyProcess tenancyFromView = TenancyFromView();
            if (tenancy.RegistrationNum != tenancyFromView.RegistrationNum)
                if (DataModelHelper.TenancyProcessesDuplicateCount(tenancy) != 0 &&
                    MessageBox.Show("В базе уже имеется договор с таким номером. Все равно продолжить сохранение?", "Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != System.Windows.Forms.DialogResult.Yes)
                    return false;
            // Проверить соответствие вида найма
            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        if (!ViewportHelper.BuildingFundAndRentMatch((int)ParentRow["id_building"], tenancy.IdRentType.Value) &&
                            MessageBox.Show("Выбранный вид найма не соответствует фонду сдаваемого здания. Все равно продолжить сохранение?",
                            "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != System.Windows.Forms.DialogResult.Yes)
                            return false;
                        break;
                    case ParentTypeEnum.Premises:
                        if (!ViewportHelper.PremiseFundAndRentMatch((int)ParentRow["id_premises"], tenancy.IdRentType.Value))
                        {
                            if (!ViewportHelper.BuildingFundAndRentMatch((int)ParentRow["id_building"], tenancy.IdRentType.Value) &&
                            MessageBox.Show("Выбранный вид найма не соответствует фонду сдаваемого помещения. Все равно продолжить сохранение?",
                            "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != System.Windows.Forms.DialogResult.Yes)
                                return false;
                        }
                        break;
                    case ParentTypeEnum.SubPremises:
                        if (!ViewportHelper.SubPremiseFundAndRentMatch((int)ParentRow["id_sub_premises"], tenancy.IdRentType.Value))
                        {
                            if (!ViewportHelper.PremiseFundAndRentMatch((int)ParentRow["id_premises"], tenancy.IdRentType.Value))
                            {
                                int idBuilding = (int)PremisesDataModel.GetInstance().Select().Rows.Find((int)ParentRow["id_premises"])["id_building"];
                                if (!ViewportHelper.BuildingFundAndRentMatch(idBuilding, tenancy.IdRentType.Value) &&
                                    MessageBox.Show("Выбранный вид найма не соответствует фонду сдаваемой комнаты. Все равно продолжить сохранение?",
                                    "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != System.Windows.Forms.DialogResult.Yes)
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
            TenancyProcess tenancy = new TenancyProcess();
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
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
            TenancyProcess tenancy = new TenancyProcess();
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
            if (checkBoxResidenceWarrantEnable.Checked)
            {
                tenancy.ResidenceWarrantNum = ViewportHelper.ValueOrNull(textBoxResidenceWarrantNumber);
                tenancy.ResidenceWarrantDate = dateTimePickerResidenceWarrantDate.Value.Date;
            }
            else
            {
                tenancy.ResidenceWarrantNum = null;
                tenancy.ResidenceWarrantDate = null;
            }
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
            textBoxResidenceWarrantNumber.Text = tenancy.ResidenceWarrantNum;
            dateTimePickerResidenceWarrantDate.Value = ViewportHelper.ValueOrDefault(tenancy.ResidenceWarrantDate);
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
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

            DataSet ds = DataSetManager.DataSet;

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
            v_tenancies.CurrentItemChanged += new EventHandler(v_tenancies_CurrentItemChanged);
            v_tenancies.DataMember = "tenancy_processes";
            v_tenancies.DataSource = ds;
            RebuildStaticFilter();
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
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

            tenancy_persons.Select().RowChanged += new DataRowChangeEventHandler(TenancyPersons_RowChanged);
            tenancy_persons.Select().RowDeleted += new DataRowChangeEventHandler(TenancyPersons_RowDeleted);
            tenancies.Select().RowChanged += new DataRowChangeEventHandler(TenancyViewport_RowChanged);
            tenancies.Select().RowDeleted += new DataRowChangeEventHandler(TenancyViewport_RowDeleted);
            if (ParentRow != null)
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        tenancy_building_assoc = TenancyBuildingsAssocDataModel.GetInstance();
                        tenancy_building_assoc.Select().RowChanged += new DataRowChangeEventHandler(TenancyAssocViewport_RowChanged);
                        tenancy_building_assoc.Select().RowDeleted += new DataRowChangeEventHandler(TenancyAssocViewport_RowDeleted);
                        break;
                    case ParentTypeEnum.Premises:
                        tenancy_premises_assoc = TenancyPremisesAssocDataModel.GetInstance();
                        tenancy_premises_assoc.Select().RowChanged += new DataRowChangeEventHandler(TenancyAssocViewport_RowChanged);
                        tenancy_premises_assoc.Select().RowDeleted += new DataRowChangeEventHandler(TenancyAssocViewport_RowDeleted);
                        break;
                    case ParentTypeEnum.SubPremises:
                        tenancy_sub_premises_assoc = TenancySubPremisesAssocDataModel.GetInstance();
                        tenancy_sub_premises_assoc.Select().RowChanged += new DataRowChangeEventHandler(TenancyAssocViewport_RowChanged);
                        tenancy_sub_premises_assoc.Select().RowDeleted += new DataRowChangeEventHandler(TenancyAssocViewport_RowDeleted);
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
            v_tenancy_persons.ListChanged += new System.ComponentModel.ListChangedEventHandler(v_persons_ListChanged);
            
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
            int index = v_executors.Find("executor_login", System.Security.Principal.WindowsIdentity.GetCurrent().Name);
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
            TenancyProcess tenancy = TenancyFromView();
            v_tenancies.AddNew();
            tenancies.EditingNewRecord = true;
            ViewportFromTenancy(tenancy);
            checkBoxContractEnable.Checked = (tenancy.RegistrationDate != null) || (tenancy.RegistrationNum != null);
            checkBoxResidenceWarrantEnable.Checked = (tenancy.ResidenceWarrantDate != null);
            checkBoxProtocolEnable.Checked = (tenancy.ProtocolDate != null);
            dateTimePickerIssueDate.Checked = (tenancy.IssueDate != null);
            dateTimePickerBeginDate.Checked = (tenancy.BeginDate != null);
            dateTimePickerEndDate.Checked = (tenancy.EndDate != null);
            int index = v_executors.Find("executor_login", System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            if (index != -1)
                comboBoxExecutor.SelectedValue = ((DataRowView)v_executors[index])["id_executor"];
            is_copy = true;
            id_copy_process = tenancy.IdProcess;
            is_editable = true;
        }

        public override bool SearchedRecords()
        {
            if (!String.IsNullOrEmpty(DynamicFilter))
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
            string Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
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
                        this.Text = "Процессы отсутствуют";
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
            TenancyProcess tenancy = TenancyFromViewport();
            if (!ValidateTenancy(tenancy))
                return;
            string Filter = "";
            if (!String.IsNullOrEmpty(v_tenancies.Filter))
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
                    int id_process = TenancyProcessesDataModel.Insert(tenancy);
                    if (id_process == -1)
                        return;
                    DataRowView newRow;
                    tenancy.IdProcess = id_process;
                    is_editable = false;
                    if (v_tenancies.Position == -1)
                        newRow = (DataRowView)v_tenancies.AddNew();
                    else
                        newRow = ((DataRowView)v_tenancies[v_tenancies.Position]);
                    Filter += String.Format(CultureInfo.CurrentCulture, "(id_process = {0})", tenancy.IdProcess);
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
                            TenancyObject to = new TenancyObject();
                            to.IdProcess = id_process;
                            to.RentLivingArea = null;
                            to.RentTotalArea = null;
                            int id_assoc = -1;
                            switch (ParentType)
                            {
                                case ParentTypeEnum.Building:
                                    TenancyBuildingsAssocDataModel tenancy_buildings = TenancyBuildingsAssocDataModel.GetInstance();
                                    to.IdObject = Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture);
                                    id_assoc = TenancyBuildingsAssocDataModel.Insert(to);
                                    if (id_assoc == -1)
                                        return;
                                    to.IdAssoc = id_assoc;
                                    tenancy_buildings.Select().Rows.Add(new object[] { 
                                id_assoc, to.IdObject, to.IdProcess, to.RentTotalArea, to.RentLivingArea, 0
                            });
                                    break;
                                case ParentTypeEnum.Premises:
                                    TenancyPremisesAssocDataModel tenancy_premises = TenancyPremisesAssocDataModel.GetInstance();
                                    to.IdObject = Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture);
                                    id_assoc = TenancyPremisesAssocDataModel.Insert(to);
                                    if (id_assoc == -1)
                                        return;
                                    to.IdAssoc = id_assoc;
                                    tenancy_premises.Select().Rows.Add(new object[] { 
                                id_assoc, to.IdObject, to.IdProcess, to.RentTotalArea, to.RentLivingArea, 0
                            });
                                    break;
                                case ParentTypeEnum.SubPremises:
                                    TenancySubPremisesAssocDataModel tenancy_sub_premises = TenancySubPremisesAssocDataModel.GetInstance();
                                    to.IdObject = Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture);
                                    id_assoc = TenancySubPremisesAssocDataModel.Insert(to);
                                    if (id_assoc == -1)
                                        return;
                                    to.IdAssoc = id_assoc;
                                    tenancy_sub_premises.Select().Rows.Add(new object[] { 
                                id_assoc, to.IdObject, to.IdProcess, to.RentTotalArea, 0
                            });
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
                    DataRowView row = ((DataRowView)v_tenancies[v_tenancies.Position]);
                    is_editable = false;
                    Filter += String.Format(CultureInfo.CurrentCulture, "(id_process = {0})", tenancy.IdProcess);
                    v_tenancies.Filter += Filter;
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
            var persons = from persons_row in DataModelHelper.FilterRows(TenancyPersonsDataModel.GetInstance().Select())
                          where persons_row.Field<int>("id_process") == id_copy_process
                          select persons_row;
            foreach (var person_row in persons.ToList())
            {
                TenancyPerson person = DataRowToPerson(person_row);
                person.IdProcess = id_new_process;
                int id_person = TenancyPersonsDataModel.Insert(person);
                if (id_person == -1)
                    return false;
                person.IdPerson = id_person;
                TenancyPersonsDataModel.GetInstance().Select().Rows.Add(PersonToObjectArray(person));
            }
            var buildings = from row in DataModelHelper.FilterRows(TenancyBuildingsAssocDataModel.GetInstance().Select())
                            where row.Field<int>("id_process") == id_copy_process
                            select row;
            foreach (var row in buildings.ToList())
            {
                TenancyObject obj = new TenancyObject();
                obj.IdObject = row.Field<int?>("id_building");
                obj.IdProcess = id_new_process;
                obj.RentLivingArea = row.Field<double?>("rent_living_area");
                obj.RentTotalArea = row.Field<double?>("rent_total_area");
                int id_assoc = TenancyBuildingsAssocDataModel.Insert(obj);
                if (id_assoc == -1)
                    return false;
                obj.IdAssoc = id_assoc;
                TenancyBuildingsAssocDataModel.GetInstance().Select().Rows.Add(obj.IdAssoc, obj.IdObject, obj.IdProcess,
                    obj.RentTotalArea, obj.RentLivingArea);
            }
            var premises = from row in DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select())
                            where row.Field<int>("id_process") == id_copy_process
                            select row;
            foreach (var row in premises.ToList())
            {
                TenancyObject obj = new TenancyObject();
                obj.IdObject = row.Field<int?>("id_premises");
                obj.IdProcess = id_new_process;
                obj.RentLivingArea = row.Field<double?>("rent_living_area");
                obj.RentTotalArea = row.Field<double?>("rent_total_area");
                int id_assoc = TenancyPremisesAssocDataModel.Insert(obj);
                if (id_assoc == -1)
                    return false;
                obj.IdAssoc = id_assoc;
                TenancyPremisesAssocDataModel.GetInstance().Select().Rows.Add(obj.IdAssoc, obj.IdObject, obj.IdProcess,
                    obj.RentTotalArea, obj.RentLivingArea);
            }
            var sub_premises = from row in DataModelHelper.FilterRows(TenancySubPremisesAssocDataModel.GetInstance().Select())
                           where row.Field<int>("id_process") == id_copy_process
                           select row;
            foreach (var row in sub_premises.ToList())
            {
                TenancyObject obj = new TenancyObject();
                obj.IdObject = row.Field<int?>("id_sub_premises");
                obj.IdProcess = id_new_process;
                obj.RentTotalArea = row.Field<double?>("rent_total_area");
                int id_assoc = TenancySubPremisesAssocDataModel.Insert(obj);
                if (id_assoc == -1)
                    return false;
                obj.IdAssoc = id_assoc;
                TenancySubPremisesAssocDataModel.GetInstance().Select().Rows.Add(obj.IdAssoc, obj.IdObject, obj.IdProcess,
                    obj.RentTotalArea);
            }
            return true;
        }

        private TenancyPerson DataRowToPerson(DataRow row)
        {
            TenancyPerson person = new TenancyPerson();
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
            TenancyViewport viewport = new TenancyViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_tenancies.Count > 0)
                viewport.LocateTenancyBy((((DataRowView)v_tenancies[v_tenancies.Position])["id_process"] as Int32?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                tenancy_persons.Select().RowChanged -= new DataRowChangeEventHandler(TenancyPersons_RowChanged);
                tenancy_persons.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyPersons_RowDeleted);
                tenancies.Select().RowChanged -= new DataRowChangeEventHandler(TenancyViewport_RowChanged);
                tenancies.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyViewport_RowDeleted);
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                tenancies.EditingNewRecord = false;
            tenancy_persons.Select().RowChanged -= new DataRowChangeEventHandler(TenancyPersons_RowChanged);
            tenancy_persons.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyPersons_RowDeleted);
            tenancies.Select().RowChanged -= new DataRowChangeEventHandler(TenancyViewport_RowChanged);
            tenancies.Select().RowDeleted -= new DataRowChangeEventHandler(TenancyViewport_RowDeleted);
            base.Close();
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

        public override void TenancyContract17xReportGenerate(Reporting.TenancyContractTypes tenancyContractType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (!TenancyValidForReportGenerate())
                return;
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
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
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
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
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
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
            DataRowView row = (DataRowView)v_tenancy_agreements[v_tenancy_agreements.Position];
            ReporterFactory.CreateReporter(ReporterType.TenancyAgreementReporter).Run(
                new Dictionary<string, string>() { { "id_agreement", row["id_agreement"].ToString() } });
        }

        private bool TenancyValidForReportGenerate()
        {
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (v_tenancies.Position == -1)
                return false;
            DataRowView row = (DataRowView)v_tenancies[v_tenancies.Position];
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
            base.OnVisibleChanged(e);
        }

        void v_persons_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
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

        void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
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

        void dateTimePickerResidenceWarrantDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxResidenceWarrantNumber_TextChanged(object sender, EventArgs e)
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

        void checkBoxResidenceWarrantEnable_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxResidenceWarrant.Controls)
                if (control != checkBoxResidenceWarrantEnable)
                    control.Enabled = checkBoxResidenceWarrantEnable.Checked;
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
            Viewport viewport = ShowAssocViewport(ViewportType.TenancyAgreementsViewport);
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
            Viewport viewport = ShowAssocViewport(ViewportType.TenancyPersonsViewport);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyViewport));
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox31 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.comboBoxExecutor = new System.Windows.Forms.ComboBox();
            this.label41 = new System.Windows.Forms.Label();
            this.comboBoxRentType = new System.Windows.Forms.ComboBox();
            this.label46 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridViewTenancyPersons = new System.Windows.Forms.DataGridView();
            this.surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_of_birth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_kinship = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.groupBoxTenancyContract = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label52 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDate = new System.Windows.Forms.DateTimePicker();
            this.label50 = new System.Windows.Forms.Label();
            this.dateTimePickerBeginDate = new System.Windows.Forms.DateTimePicker();
            this.label49 = new System.Windows.Forms.Label();
            this.dateTimePickerIssueDate = new System.Windows.Forms.DateTimePicker();
            this.panel5 = new System.Windows.Forms.Panel();
            this.vButtonWarrant = new VIBlend.WinForms.Controls.vButton();
            this.textBoxSelectedWarrant = new System.Windows.Forms.TextBox();
            this.label82 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.dateTimePickerRegistrationDate = new System.Windows.Forms.DateTimePicker();
            this.textBoxRegistrationNumber = new System.Windows.Forms.TextBox();
            this.label47 = new System.Windows.Forms.Label();
            this.checkBoxContractEnable = new System.Windows.Forms.CheckBox();
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.dataGridViewTenancyAgreements = new System.Windows.Forms.DataGridView();
            this.agreement_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agreement_content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.dataGridViewTenancyReasons = new System.Windows.Forms.DataGridView();
            this.reason_prepared = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reason_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reason_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxResidenceWarrant = new System.Windows.Forms.GroupBox();
            this.label44 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.textBoxResidenceWarrantNumber = new System.Windows.Forms.TextBox();
            this.dateTimePickerResidenceWarrantDate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxResidenceWarrantEnable = new System.Windows.Forms.CheckBox();
            this.groupBoxProtocol = new System.Windows.Forms.GroupBox();
            this.label45 = new System.Windows.Forms.Label();
            this.dateTimePickerProtocolDate = new System.Windows.Forms.DateTimePicker();
            this.label42 = new System.Windows.Forms.Label();
            this.textBoxProtocolNumber = new System.Windows.Forms.TextBox();
            this.checkBoxProtocolEnable = new System.Windows.Forms.CheckBox();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.dataGridViewTenancyAddress = new System.Windows.Forms.DataGridView();
            this.address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel9.SuspendLayout();
            this.groupBox31.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).BeginInit();
            this.groupBoxTenancyContract.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.groupBox25.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyAgreements)).BeginInit();
            this.groupBox24.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyReasons)).BeginInit();
            this.groupBoxResidenceWarrant.SuspendLayout();
            this.groupBoxProtocol.SuspendLayout();
            this.groupBox21.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Controls.Add(this.groupBox31, 1, 3);
            this.tableLayoutPanel9.Controls.Add(this.groupBox22, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.groupBox1, 0, 4);
            this.tableLayoutPanel9.Controls.Add(this.groupBoxTenancyContract, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.groupBox25, 1, 1);
            this.tableLayoutPanel9.Controls.Add(this.groupBox24, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.groupBoxResidenceWarrant, 0, 2);
            this.tableLayoutPanel9.Controls.Add(this.groupBoxProtocol, 1, 0);
            this.tableLayoutPanel9.Controls.Add(this.groupBox21, 0, 4);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 5;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(867, 502);
            this.tableLayoutPanel9.TabIndex = 0;
            // 
            // groupBox31
            // 
            this.groupBox31.Controls.Add(this.textBoxDescription);
            this.groupBox31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox31.Location = new System.Drawing.Point(436, 311);
            this.groupBox31.Name = "groupBox31";
            this.groupBox31.Size = new System.Drawing.Size(428, 79);
            this.groupBox31.TabIndex = 6;
            this.groupBox31.TabStop = false;
            this.groupBox31.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(3, 17);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(422, 59);
            this.textBoxDescription.TabIndex = 0;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged_1);
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.comboBoxExecutor);
            this.groupBox22.Controls.Add(this.label41);
            this.groupBox22.Controls.Add(this.comboBoxRentType);
            this.groupBox22.Controls.Add(this.label46);
            this.groupBox22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox22.Location = new System.Drawing.Point(3, 3);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(427, 79);
            this.groupBox22.TabIndex = 0;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Общие сведения";
            // 
            // comboBoxExecutor
            // 
            this.comboBoxExecutor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxExecutor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExecutor.FormattingEnabled = true;
            this.comboBoxExecutor.Location = new System.Drawing.Point(168, 48);
            this.comboBoxExecutor.Name = "comboBoxExecutor";
            this.comboBoxExecutor.Size = new System.Drawing.Size(250, 23);
            this.comboBoxExecutor.TabIndex = 1;
            this.comboBoxExecutor.SelectedValueChanged += new System.EventHandler(this.comboBoxExecutor_SelectedValueChanged);
            // 
            // label41
            // 
            this.label41.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(12, 51);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(141, 15);
            this.label41.TabIndex = 1;
            this.label41.Text = "Составитель договора";
            // 
            // comboBoxRentType
            // 
            this.comboBoxRentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRentType.FormattingEnabled = true;
            this.comboBoxRentType.Location = new System.Drawing.Point(168, 19);
            this.comboBoxRentType.Name = "comboBoxRentType";
            this.comboBoxRentType.Size = new System.Drawing.Size(250, 23);
            this.comboBoxRentType.TabIndex = 0;
            this.comboBoxRentType.SelectedValueChanged += new System.EventHandler(this.comboBoxRentType_SelectedValueChanged);
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(12, 22);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(108, 15);
            this.label46.TabIndex = 16;
            this.label46.Text = "Тип найма жилья";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridViewTenancyPersons);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(436, 396);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(428, 103);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Участники найма";
            // 
            // dataGridViewTenancyPersons
            // 
            this.dataGridViewTenancyPersons.AllowUserToAddRows = false;
            this.dataGridViewTenancyPersons.AllowUserToDeleteRows = false;
            this.dataGridViewTenancyPersons.AllowUserToResizeRows = false;
            this.dataGridViewTenancyPersons.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTenancyPersons.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTenancyPersons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTenancyPersons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.surname,
            this.name,
            this.patronymic,
            this.date_of_birth,
            this.id_kinship});
            this.dataGridViewTenancyPersons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTenancyPersons.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewTenancyPersons.MultiSelect = false;
            this.dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            this.dataGridViewTenancyPersons.ReadOnly = true;
            this.dataGridViewTenancyPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyPersons.Size = new System.Drawing.Size(422, 83);
            this.dataGridViewTenancyPersons.TabIndex = 0;
            this.dataGridViewTenancyPersons.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewTenancyPersons_CellDoubleClick);
            // 
            // surname
            // 
            this.surname.HeaderText = "Фамилия";
            this.surname.MinimumWidth = 100;
            this.surname.Name = "surname";
            this.surname.ReadOnly = true;
            // 
            // name
            // 
            this.name.HeaderText = "Имя";
            this.name.MinimumWidth = 100;
            this.name.Name = "name";
            this.name.ReadOnly = true;
            // 
            // patronymic
            // 
            this.patronymic.HeaderText = "Отчество";
            this.patronymic.MinimumWidth = 100;
            this.patronymic.Name = "patronymic";
            this.patronymic.ReadOnly = true;
            // 
            // date_of_birth
            // 
            this.date_of_birth.HeaderText = "Дата рождения";
            this.date_of_birth.MinimumWidth = 130;
            this.date_of_birth.Name = "date_of_birth";
            this.date_of_birth.ReadOnly = true;
            // 
            // id_kinship
            // 
            this.id_kinship.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_kinship.HeaderText = "Отношение/связь";
            this.id_kinship.MinimumWidth = 120;
            this.id_kinship.Name = "id_kinship";
            this.id_kinship.ReadOnly = true;
            // 
            // groupBoxTenancyContract
            // 
            this.tableLayoutPanel9.SetColumnSpan(this.groupBoxTenancyContract, 2);
            this.groupBoxTenancyContract.Controls.Add(this.tableLayoutPanel10);
            this.groupBoxTenancyContract.Controls.Add(this.checkBoxContractEnable);
            this.groupBoxTenancyContract.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxTenancyContract.Location = new System.Drawing.Point(3, 88);
            this.groupBoxTenancyContract.Name = "groupBoxTenancyContract";
            this.groupBoxTenancyContract.Size = new System.Drawing.Size(861, 109);
            this.groupBoxTenancyContract.TabIndex = 2;
            this.groupBoxTenancyContract.TabStop = false;
            this.groupBoxTenancyContract.Text = "      Договор найма";
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.panel6, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(855, 89);
            this.tableLayoutPanel10.TabIndex = 1;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label52);
            this.panel6.Controls.Add(this.label51);
            this.panel6.Controls.Add(this.dateTimePickerEndDate);
            this.panel6.Controls.Add(this.label50);
            this.panel6.Controls.Add(this.dateTimePickerBeginDate);
            this.panel6.Controls.Add(this.label49);
            this.panel6.Controls.Add(this.dateTimePickerIssueDate);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(427, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(428, 90);
            this.panel6.TabIndex = 1;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(150, 65);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(21, 15);
            this.label52.TabIndex = 28;
            this.label52.Text = "по";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(158, 36);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(13, 15);
            this.label51.TabIndex = 27;
            this.label51.Text = "с";
            // 
            // dateTimePickerEndDate
            // 
            this.dateTimePickerEndDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDate.Location = new System.Drawing.Point(174, 62);
            this.dateTimePickerEndDate.Name = "dateTimePickerEndDate";
            this.dateTimePickerEndDate.ShowCheckBox = true;
            this.dateTimePickerEndDate.Size = new System.Drawing.Size(246, 21);
            this.dateTimePickerEndDate.TabIndex = 2;
            this.dateTimePickerEndDate.ValueChanged += new System.EventHandler(this.dateTimePickerEndDate_ValueChanged);
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(15, 36);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(93, 15);
            this.label50.TabIndex = 25;
            this.label50.Text = "Срок действия";
            // 
            // dateTimePickerBeginDate
            // 
            this.dateTimePickerBeginDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerBeginDate.Location = new System.Drawing.Point(174, 33);
            this.dateTimePickerBeginDate.Name = "dateTimePickerBeginDate";
            this.dateTimePickerBeginDate.ShowCheckBox = true;
            this.dateTimePickerBeginDate.Size = new System.Drawing.Size(246, 21);
            this.dateTimePickerBeginDate.TabIndex = 1;
            this.dateTimePickerBeginDate.ValueChanged += new System.EventHandler(this.dateTimePickerBeginDate_ValueChanged);
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(15, 7);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(83, 15);
            this.label49.TabIndex = 23;
            this.label49.Text = "Дата выдачи";
            // 
            // dateTimePickerIssueDate
            // 
            this.dateTimePickerIssueDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIssueDate.Location = new System.Drawing.Point(174, 4);
            this.dateTimePickerIssueDate.Name = "dateTimePickerIssueDate";
            this.dateTimePickerIssueDate.ShowCheckBox = true;
            this.dateTimePickerIssueDate.Size = new System.Drawing.Size(246, 21);
            this.dateTimePickerIssueDate.TabIndex = 0;
            this.dateTimePickerIssueDate.ValueChanged += new System.EventHandler(this.dateTimePickerIssueDate_ValueChanged);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.vButtonWarrant);
            this.panel5.Controls.Add(this.textBoxSelectedWarrant);
            this.panel5.Controls.Add(this.label82);
            this.panel5.Controls.Add(this.label48);
            this.panel5.Controls.Add(this.dateTimePickerRegistrationDate);
            this.panel5.Controls.Add(this.textBoxRegistrationNumber);
            this.panel5.Controls.Add(this.label47);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(427, 90);
            this.panel5.TabIndex = 0;
            // 
            // vButtonWarrant
            // 
            this.vButtonWarrant.AllowAnimations = true;
            this.vButtonWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonWarrant.BackColor = System.Drawing.Color.Transparent;
            this.vButtonWarrant.Location = new System.Drawing.Point(391, 62);
            this.vButtonWarrant.Name = "vButtonWarrant";
            this.vButtonWarrant.RoundedCornersMask = ((byte)(15));
            this.vButtonWarrant.Size = new System.Drawing.Size(27, 20);
            this.vButtonWarrant.TabIndex = 24;
            this.vButtonWarrant.Text = "...";
            this.vButtonWarrant.UseVisualStyleBackColor = false;
            this.vButtonWarrant.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonWarrant.Click += new System.EventHandler(this.vButtonWarrant_Click);
            // 
            // textBoxSelectedWarrant
            // 
            this.textBoxSelectedWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSelectedWarrant.Location = new System.Drawing.Point(172, 62);
            this.textBoxSelectedWarrant.Name = "textBoxSelectedWarrant";
            this.textBoxSelectedWarrant.ReadOnly = true;
            this.textBoxSelectedWarrant.Size = new System.Drawing.Size(213, 21);
            this.textBoxSelectedWarrant.TabIndex = 2;
            this.textBoxSelectedWarrant.TextChanged += new System.EventHandler(this.textBoxSelectedWarrant_TextChanged);
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.Location = new System.Drawing.Point(14, 65);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(92, 15);
            this.label82.TabIndex = 23;
            this.label82.Text = "Доверенность";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(14, 36);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(114, 15);
            this.label48.TabIndex = 21;
            this.label48.Text = "Дата регистрации";
            // 
            // dateTimePickerRegistrationDate
            // 
            this.dateTimePickerRegistrationDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerRegistrationDate.Location = new System.Drawing.Point(172, 33);
            this.dateTimePickerRegistrationDate.Name = "dateTimePickerRegistrationDate";
            this.dateTimePickerRegistrationDate.Size = new System.Drawing.Size(246, 21);
            this.dateTimePickerRegistrationDate.TabIndex = 1;
            this.dateTimePickerRegistrationDate.ValueChanged += new System.EventHandler(this.dateTimePickerRegistrationDate_ValueChanged);
            // 
            // textBoxRegistrationNumber
            // 
            this.textBoxRegistrationNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationNumber.Location = new System.Drawing.Point(172, 4);
            this.textBoxRegistrationNumber.MaxLength = 255;
            this.textBoxRegistrationNumber.Name = "textBoxRegistrationNumber";
            this.textBoxRegistrationNumber.Size = new System.Drawing.Size(246, 21);
            this.textBoxRegistrationNumber.TabIndex = 0;
            this.textBoxRegistrationNumber.TextChanged += new System.EventHandler(this.textBoxRegistrationNumber_TextChanged);
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(14, 7);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(152, 15);
            this.label47.TabIndex = 18;
            this.label47.Text = "Регистрационный номер";
            // 
            // checkBoxContractEnable
            // 
            this.checkBoxContractEnable.AutoSize = true;
            this.checkBoxContractEnable.Checked = true;
            this.checkBoxContractEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxContractEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxContractEnable.Name = "checkBoxContractEnable";
            this.checkBoxContractEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxContractEnable.TabIndex = 0;
            this.checkBoxContractEnable.UseVisualStyleBackColor = true;
            this.checkBoxContractEnable.CheckedChanged += new System.EventHandler(this.checkBoxProcessEnable_CheckedChanged);
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.dataGridViewTenancyAgreements);
            this.groupBox25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox25.Location = new System.Drawing.Point(436, 203);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(428, 102);
            this.groupBox25.TabIndex = 4;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "Соглашения найма";
            // 
            // dataGridViewTenancyAgreements
            // 
            this.dataGridViewTenancyAgreements.AllowUserToAddRows = false;
            this.dataGridViewTenancyAgreements.AllowUserToDeleteRows = false;
            this.dataGridViewTenancyAgreements.AllowUserToResizeRows = false;
            this.dataGridViewTenancyAgreements.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTenancyAgreements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTenancyAgreements.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.agreement_date,
            this.agreement_content});
            this.dataGridViewTenancyAgreements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTenancyAgreements.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewTenancyAgreements.MultiSelect = false;
            this.dataGridViewTenancyAgreements.Name = "dataGridViewTenancyAgreements";
            this.dataGridViewTenancyAgreements.ReadOnly = true;
            this.dataGridViewTenancyAgreements.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyAgreements.Size = new System.Drawing.Size(422, 82);
            this.dataGridViewTenancyAgreements.TabIndex = 0;
            this.dataGridViewTenancyAgreements.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewTenancyAgreements_CellDoubleClick);
            // 
            // agreement_date
            // 
            this.agreement_date.HeaderText = "Дата";
            this.agreement_date.Name = "agreement_date";
            this.agreement_date.ReadOnly = true;
            // 
            // agreement_content
            // 
            this.agreement_content.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.agreement_content.HeaderText = "Содержание";
            this.agreement_content.Name = "agreement_content";
            this.agreement_content.ReadOnly = true;
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.dataGridViewTenancyReasons);
            this.groupBox24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox24.Location = new System.Drawing.Point(3, 203);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(427, 102);
            this.groupBox24.TabIndex = 3;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "Основания найма";
            // 
            // dataGridViewTenancyReasons
            // 
            this.dataGridViewTenancyReasons.AllowUserToAddRows = false;
            this.dataGridViewTenancyReasons.AllowUserToDeleteRows = false;
            this.dataGridViewTenancyReasons.AllowUserToResizeRows = false;
            this.dataGridViewTenancyReasons.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTenancyReasons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTenancyReasons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.reason_prepared,
            this.reason_number,
            this.reason_date});
            this.dataGridViewTenancyReasons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTenancyReasons.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewTenancyReasons.MultiSelect = false;
            this.dataGridViewTenancyReasons.Name = "dataGridViewTenancyReasons";
            this.dataGridViewTenancyReasons.ReadOnly = true;
            this.dataGridViewTenancyReasons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyReasons.Size = new System.Drawing.Size(421, 82);
            this.dataGridViewTenancyReasons.TabIndex = 0;
            this.dataGridViewTenancyReasons.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewTenancyReasons_CellDoubleClick);
            // 
            // reason_prepared
            // 
            this.reason_prepared.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.reason_prepared.HeaderText = "Основание";
            this.reason_prepared.Name = "reason_prepared";
            this.reason_prepared.ReadOnly = true;
            // 
            // reason_number
            // 
            this.reason_number.HeaderText = "№";
            this.reason_number.Name = "reason_number";
            this.reason_number.ReadOnly = true;
            // 
            // reason_date
            // 
            this.reason_date.HeaderText = "Дата";
            this.reason_date.Name = "reason_date";
            this.reason_date.ReadOnly = true;
            // 
            // groupBoxResidenceWarrant
            // 
            this.groupBoxResidenceWarrant.Controls.Add(this.label44);
            this.groupBoxResidenceWarrant.Controls.Add(this.label43);
            this.groupBoxResidenceWarrant.Controls.Add(this.textBoxResidenceWarrantNumber);
            this.groupBoxResidenceWarrant.Controls.Add(this.dateTimePickerResidenceWarrantDate);
            this.groupBoxResidenceWarrant.Controls.Add(this.checkBoxResidenceWarrantEnable);
            this.groupBoxResidenceWarrant.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxResidenceWarrant.Location = new System.Drawing.Point(3, 311);
            this.groupBoxResidenceWarrant.Name = "groupBoxResidenceWarrant";
            this.groupBoxResidenceWarrant.Size = new System.Drawing.Size(427, 79);
            this.groupBoxResidenceWarrant.TabIndex = 5;
            this.groupBoxResidenceWarrant.TabStop = false;
            this.groupBoxResidenceWarrant.Text = "      Ордер на проживание";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(17, 53);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(82, 15);
            this.label44.TabIndex = 16;
            this.label44.Text = "Дата ордера";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(17, 22);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(91, 15);
            this.label43.TabIndex = 14;
            this.label43.Text = "Номер ордера";
            // 
            // textBoxResidenceWarrantNumber
            // 
            this.textBoxResidenceWarrantNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceWarrantNumber.Location = new System.Drawing.Point(175, 19);
            this.textBoxResidenceWarrantNumber.MaxLength = 50;
            this.textBoxResidenceWarrantNumber.Name = "textBoxResidenceWarrantNumber";
            this.textBoxResidenceWarrantNumber.Size = new System.Drawing.Size(246, 21);
            this.textBoxResidenceWarrantNumber.TabIndex = 1;
            this.textBoxResidenceWarrantNumber.TextChanged += new System.EventHandler(this.textBoxResidenceWarrantNumber_TextChanged);
            // 
            // dateTimePickerResidenceWarrantDate
            // 
            this.dateTimePickerResidenceWarrantDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerResidenceWarrantDate.Location = new System.Drawing.Point(175, 48);
            this.dateTimePickerResidenceWarrantDate.Name = "dateTimePickerResidenceWarrantDate";
            this.dateTimePickerResidenceWarrantDate.Size = new System.Drawing.Size(246, 21);
            this.dateTimePickerResidenceWarrantDate.TabIndex = 2;
            this.dateTimePickerResidenceWarrantDate.ValueChanged += new System.EventHandler(this.dateTimePickerResidenceWarrantDate_ValueChanged);
            // 
            // checkBoxResidenceWarrantEnable
            // 
            this.checkBoxResidenceWarrantEnable.AutoSize = true;
            this.checkBoxResidenceWarrantEnable.Checked = true;
            this.checkBoxResidenceWarrantEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxResidenceWarrantEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxResidenceWarrantEnable.Name = "checkBoxResidenceWarrantEnable";
            this.checkBoxResidenceWarrantEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxResidenceWarrantEnable.TabIndex = 0;
            this.checkBoxResidenceWarrantEnable.UseVisualStyleBackColor = true;
            this.checkBoxResidenceWarrantEnable.CheckedChanged += new System.EventHandler(this.checkBoxResidenceWarrantEnable_CheckedChanged);
            // 
            // groupBoxProtocol
            // 
            this.groupBoxProtocol.Controls.Add(this.label45);
            this.groupBoxProtocol.Controls.Add(this.dateTimePickerProtocolDate);
            this.groupBoxProtocol.Controls.Add(this.label42);
            this.groupBoxProtocol.Controls.Add(this.textBoxProtocolNumber);
            this.groupBoxProtocol.Controls.Add(this.checkBoxProtocolEnable);
            this.groupBoxProtocol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxProtocol.Location = new System.Drawing.Point(436, 3);
            this.groupBoxProtocol.Name = "groupBoxProtocol";
            this.groupBoxProtocol.Size = new System.Drawing.Size(428, 79);
            this.groupBoxProtocol.TabIndex = 1;
            this.groupBoxProtocol.TabStop = false;
            this.groupBoxProtocol.Text = "      Протокол жилищной комиссии";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(12, 52);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(102, 15);
            this.label45.TabIndex = 18;
            this.label45.Text = "Дата протокола";
            // 
            // dateTimePickerProtocolDate
            // 
            this.dateTimePickerProtocolDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerProtocolDate.Location = new System.Drawing.Point(171, 48);
            this.dateTimePickerProtocolDate.Name = "dateTimePickerProtocolDate";
            this.dateTimePickerProtocolDate.Size = new System.Drawing.Size(246, 21);
            this.dateTimePickerProtocolDate.TabIndex = 2;
            this.dateTimePickerProtocolDate.ValueChanged += new System.EventHandler(this.dateTimePickerProtocolDate_ValueChanged);
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(12, 22);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(111, 15);
            this.label42.TabIndex = 12;
            this.label42.Text = "Номер протокола";
            // 
            // textBoxProtocolNumber
            // 
            this.textBoxProtocolNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProtocolNumber.Location = new System.Drawing.Point(171, 19);
            this.textBoxProtocolNumber.MaxLength = 50;
            this.textBoxProtocolNumber.Name = "textBoxProtocolNumber";
            this.textBoxProtocolNumber.Size = new System.Drawing.Size(246, 21);
            this.textBoxProtocolNumber.TabIndex = 1;
            this.textBoxProtocolNumber.TextChanged += new System.EventHandler(this.textBoxProtocolNumber_TextChanged);
            // 
            // checkBoxProtocolEnable
            // 
            this.checkBoxProtocolEnable.AutoSize = true;
            this.checkBoxProtocolEnable.Checked = true;
            this.checkBoxProtocolEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProtocolEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxProtocolEnable.Name = "checkBoxProtocolEnable";
            this.checkBoxProtocolEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxProtocolEnable.TabIndex = 0;
            this.checkBoxProtocolEnable.UseVisualStyleBackColor = true;
            this.checkBoxProtocolEnable.CheckedChanged += new System.EventHandler(this.checkBoxProtocolEnable_CheckedChanged);
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.dataGridViewTenancyAddress);
            this.groupBox21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox21.Location = new System.Drawing.Point(3, 396);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(427, 103);
            this.groupBox21.TabIndex = 7;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "Нанимаемое жилье";
            // 
            // dataGridViewTenancyAddress
            // 
            this.dataGridViewTenancyAddress.AllowUserToAddRows = false;
            this.dataGridViewTenancyAddress.AllowUserToDeleteRows = false;
            this.dataGridViewTenancyAddress.AllowUserToResizeRows = false;
            this.dataGridViewTenancyAddress.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTenancyAddress.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTenancyAddress.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTenancyAddress.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.address,
            this.total_area,
            this.living_area});
            this.dataGridViewTenancyAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTenancyAddress.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewTenancyAddress.MultiSelect = false;
            this.dataGridViewTenancyAddress.Name = "dataGridViewTenancyAddress";
            this.dataGridViewTenancyAddress.ReadOnly = true;
            this.dataGridViewTenancyAddress.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyAddress.Size = new System.Drawing.Size(421, 83);
            this.dataGridViewTenancyAddress.TabIndex = 0;
            this.dataGridViewTenancyAddress.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewTenancyAddress_CellDoubleClick);
            // 
            // address
            // 
            this.address.HeaderText = "Адрес";
            this.address.MinimumWidth = 400;
            this.address.Name = "address";
            this.address.ReadOnly = true;
            // 
            // total_area
            // 
            this.total_area.HeaderText = "Общая площадь";
            this.total_area.MinimumWidth = 150;
            this.total_area.Name = "total_area";
            this.total_area.ReadOnly = true;
            // 
            // living_area
            // 
            this.living_area.HeaderText = "Жилая площадь";
            this.living_area.MinimumWidth = 150;
            this.living_area.Name = "living_area";
            this.living_area.ReadOnly = true;
            // 
            // TenancyViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(720, 480);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(873, 508);
            this.Controls.Add(this.tableLayoutPanel9);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Процесс найма №{0}";
            this.tableLayoutPanel9.ResumeLayout(false);
            this.groupBox31.ResumeLayout(false);
            this.groupBox31.PerformLayout();
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).EndInit();
            this.groupBoxTenancyContract.ResumeLayout(false);
            this.groupBoxTenancyContract.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.groupBox25.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyAgreements)).EndInit();
            this.groupBox24.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyReasons)).EndInit();
            this.groupBoxResidenceWarrant.ResumeLayout(false);
            this.groupBoxResidenceWarrant.PerformLayout();
            this.groupBoxProtocol.ResumeLayout(false);
            this.groupBoxProtocol.PerformLayout();
            this.groupBox21.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyAddress)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
