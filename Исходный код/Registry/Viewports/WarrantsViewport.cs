using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using CustomControls;
using Registry.DataModels;
using Security;

namespace Registry.Viewport
{
    internal sealed class WarrantsViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel14;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_warrant;
        private DataGridViewTextBoxColumn registration_num;
        private DataGridViewDateTimeColumn registration_date;
        private DataGridViewTextBoxColumn notary;
        private DataGridViewTextBoxColumn on_behalf_of;
        private DataGridViewTextBoxColumn description;
        private GroupBox groupBox32;
        private GroupBox groupBox33;
        private Label label83;
        private Label label84;
        private Label label85;
        private Label label86;
        private Label label87;
        private Label label88;
        private TextBox textBoxWarrantRegNum;
        private TextBox textBoxWarrantNotary;
        private TextBox textBoxWarrantDistrict;
        private TextBox textBoxWarrantOnBehalfOf;
        private TextBox textBoxWarrantDescription;
        private DateTimePicker dateTimePickerWarrantDate;
        private ComboBox comboBoxWarrantDocType;
        #endregion Components

        #region Models
        WarrantsDataModel warrants = null;
        WarrantDocTypesDataModel warrant_doc_types = null;
        #endregion Models

        #region Views
        BindingSource v_warrants = null;
        BindingSource v_warrant_doc_types = null;
        #endregion Views

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;

        private WarrantsViewport()
            : this(null)
        {
        }

        public WarrantsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public WarrantsViewport(WarrantsViewport warrantsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = warrantsViewport.DynamicFilter;
            this.StaticFilter = warrantsViewport.StaticFilter;
            this.ParentRow = warrantsViewport.ParentRow;
            this.ParentType = warrantsViewport.ParentType;
        }

        private void DataBind()
        {
            comboBoxWarrantDocType.DataSource = v_warrant_doc_types;
            comboBoxWarrantDocType.ValueMember = "id_warrant_doc_type";
            comboBoxWarrantDocType.DisplayMember = "warrant_doc_type";
            comboBoxWarrantDocType.DataBindings.Clear();
            comboBoxWarrantDocType.DataBindings.Add("SelectedValue", v_warrants, "id_warrant_doc_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxWarrantRegNum.DataBindings.Clear();
            textBoxWarrantRegNum.DataBindings.Add("Text", v_warrants, "registration_num", true, DataSourceUpdateMode.Never, "");
            textBoxWarrantNotary.DataBindings.Clear();
            textBoxWarrantNotary.DataBindings.Add("Text", v_warrants, "notary", true, DataSourceUpdateMode.Never, "");
            textBoxWarrantOnBehalfOf.DataBindings.Clear();
            textBoxWarrantOnBehalfOf.DataBindings.Add("Text", v_warrants, "on_behalf_of", true, DataSourceUpdateMode.Never, "");
            textBoxWarrantDistrict.DataBindings.Clear();
            textBoxWarrantDistrict.DataBindings.Add("Text", v_warrants, "notary_district", true, DataSourceUpdateMode.Never, "");
            textBoxWarrantDescription.DataBindings.Clear();
            textBoxWarrantDescription.DataBindings.Add("Text", v_warrants, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerWarrantDate.DataBindings.Clear();
            dateTimePickerWarrantDate.DataBindings.Add("Value", v_warrants, "registration_date", true, DataSourceUpdateMode.Never, DateTime.Now);

            dataGridView.DataSource = v_warrants;
            id_warrant.DataPropertyName = "id_warrant";
            notary.DataPropertyName = "notary";
            on_behalf_of.DataPropertyName = "on_behalf_of";
            registration_num.DataPropertyName = "registration_num";
            registration_date.DataPropertyName = "registration_date";
            description.DataPropertyName = "description";
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!this.ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_warrants.Position != -1) && (WarrantFromView() != WarrantFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    MenuCallback.EditingStateUpdate();
                    dataGridView.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    MenuCallback.EditingStateUpdate();
                    dataGridView.Enabled = true;
                }
            }
        }

        private bool ChangeViewportStateTo(ViewportState state)
        {
            switch (state)
            {
                case ViewportState.ReadState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            return true;
                        case ViewportState.NewRowState:
                        case ViewportState.ModifyRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                            if (warrants.EditingNewRecord)
                                return false;
                            else
                            {
                                viewportState = ViewportState.NewRowState;
                                return true;
                            }
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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

        private void LocateWarrantBy(int id)
        {
            int Position = v_warrants.Find("id_warrant", id);
            is_editable = false;
            if (Position > 0)
                v_warrants.Position = Position;
            is_editable = true;
        }

        private void ViewportFromWarrant(Warrant warrant)
        {
            comboBoxWarrantDocType.SelectedValue = ViewportHelper.ValueOrDBNull(warrant.IdWarrantDocType);
            dateTimePickerWarrantDate.Value = ViewportHelper.ValueOrDefault(warrant.RegistrationDate);
            textBoxWarrantDescription.Text = warrant.Description;
            textBoxWarrantRegNum.Text = warrant.RegistrationNum;
            textBoxWarrantNotary.Text = warrant.Notary;
            textBoxWarrantDistrict.Text = warrant.NotaryDistrict;
            textBoxWarrantOnBehalfOf.Text = warrant.OnBehalfOf;
        }

        private Warrant WarrantFromViewport()
        {
            Warrant warrant = new Warrant();
            if (v_warrants.Position == -1)
                warrant.IdWarrant = null;
            else
                warrant.IdWarrant = ViewportHelper.ValueOrNull<int>((DataRowView)v_warrants[v_warrants.Position],"id_warrant");
            warrant.IdWarrantDocType = ViewportHelper.ValueOrNull<int>(comboBoxWarrantDocType);
            warrant.RegistrationNum = ViewportHelper.ValueOrNull(textBoxWarrantRegNum);
            warrant.OnBehalfOf = ViewportHelper.ValueOrNull(textBoxWarrantOnBehalfOf);
            warrant.Notary = ViewportHelper.ValueOrNull(textBoxWarrantNotary);
            warrant.NotaryDistrict = ViewportHelper.ValueOrNull(textBoxWarrantDistrict);
            warrant.Description = ViewportHelper.ValueOrNull(textBoxWarrantDescription);
            warrant.RegistrationDate = dateTimePickerWarrantDate.Value;
            return warrant;
        }

        private Warrant WarrantFromView()
        {
            Warrant warrant = new Warrant();
            DataRowView row = (DataRowView)v_warrants[v_warrants.Position];
            warrant.IdWarrant = ViewportHelper.ValueOrNull<int>(row, "id_warrant");
            warrant.IdWarrantDocType = ViewportHelper.ValueOrNull<int>(row, "id_warrant_doc_type");
            warrant.RegistrationNum = ViewportHelper.ValueOrNull(row, "registration_num");
            warrant.RegistrationDate = ViewportHelper.ValueOrNull<DateTime>(row, "registration_date");
            warrant.OnBehalfOf = ViewportHelper.ValueOrNull(row, "on_behalf_of");
            warrant.Notary = ViewportHelper.ValueOrNull(row, "notary");
            warrant.NotaryDistrict = ViewportHelper.ValueOrNull(row, "notary_district");
            warrant.Description = ViewportHelper.ValueOrNull(row, "description");
            return warrant;
        }

        private static void FillRowFromWarrant(Warrant warrant, DataRowView row)
        {
            row.BeginEdit();
            row["id_warrant"] = warrant.IdWarrant == null ? DBNull.Value : (object)warrant.IdWarrant;
            row["id_warrant_doc_type"] = warrant.IdWarrantDocType == null ? DBNull.Value : (object)warrant.IdWarrantDocType;
            row["registration_num"] = warrant.RegistrationNum == null ? DBNull.Value : (object)warrant.RegistrationNum;
            row["registration_date"] = warrant.RegistrationDate == null ? DBNull.Value : (object)warrant.RegistrationDate;
            row["on_behalf_of"] = warrant.OnBehalfOf == null ? DBNull.Value : (object)warrant.OnBehalfOf;
            row["notary"] = warrant.Notary == null ? DBNull.Value : (object)warrant.Notary;
            row["notary_district"] = warrant.NotaryDistrict == null ? DBNull.Value : (object)warrant.NotaryDistrict;
            row["description"] = warrant.Description == null ? DBNull.Value : (object)warrant.Description;
            row.EndEdit();
        }

        private bool ValidateWarrant(Warrant warrant)
        {
            if (warrant.IdWarrantDocType == null)
            {
                MessageBox.Show("Необходимо выбрать тип документа", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxWarrantDocType.Focus();
                return false;
            }
            if (warrant.RegistrationNum == null)
            {
                MessageBox.Show("Регистрационный номер не может быть пустым", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxWarrantRegNum.Focus();
                return false;
            }
            return true;
        }

        public override int GetRecordCount()
        {
            return v_warrants.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_warrants.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_warrants.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_warrants.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_warrants.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_warrants.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_warrants.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_warrants.Position > -1) && (v_warrants.Position < (v_warrants.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_warrants.Position > -1) && (v_warrants.Position < (v_warrants.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            warrants = WarrantsDataModel.GetInstance();
            warrant_doc_types = WarrantDocTypesDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            warrants.Select();
            warrant_doc_types.Select();

            DataSet ds = DataSetManager.DataSet;

            v_warrant_doc_types = new BindingSource();
            v_warrant_doc_types.DataMember = "warrant_doc_types";
            v_warrant_doc_types.DataSource = ds;

            v_warrants = new BindingSource();
            v_warrants.CurrentItemChanged += new EventHandler(v_warrants_CurrentItemChanged);
            v_warrants.DataMember = "warrants";
            v_warrants.DataSource = ds;
            v_warrants.Sort = "registration_date DESC";

            DataBind();
            is_editable = true;
        }

        public override bool CanInsertRecord()
        {
            return (!warrants.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            v_warrants.AddNew();
            dataGridView.Enabled = false;
            is_editable = true;
            warrants.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (v_warrants.Position != -1) && (!warrants.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            Warrant warrant = WarrantFromView();
            v_warrants.AddNew();
            dataGridView.Enabled = false;
            warrants.EditingNewRecord = true;
            ViewportFromWarrant(warrant);
            is_editable = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (WarrantsDataModel.Delete((int)((DataRowView)v_warrants.Current)["id_warrant"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_warrants[v_warrants.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanDeleteRecord()
        {
            return (v_warrants.Position > -1)
                && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            WarrantsViewport viewport = new WarrantsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_warrants.Count > 0)
                viewport.LocateWarrantBy((((DataRowView)v_warrants[v_warrants.Position])["id_warrant"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            Warrant warrant = WarrantFromViewport();
            if (!ValidateWarrant(warrant))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    int id_warrant = WarrantsDataModel.Insert(warrant);
                    if (id_warrant == -1)
                        return;
                    DataRowView newRow;
                    warrant.IdWarrant = id_warrant;
                    is_editable = false;
                    if (v_warrants.Position == -1)
                        newRow = (DataRowView)v_warrants.AddNew();
                    else
                        newRow = ((DataRowView)v_warrants[v_warrants.Position]);
                    FillRowFromWarrant(warrant, newRow);
                    warrants.EditingNewRecord = false;
                    is_editable = true;
                    break;
                case ViewportState.ModifyRowState:
                    if (warrant.IdWarrant == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись о принадлежности фонду без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (WarrantsDataModel.Update(warrant) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_warrants[v_warrants.Position]);
                    is_editable = false;
                    FillRowFromWarrant(warrant, row);
                    break;
            }
            dataGridView.Enabled = true;
            is_editable = true;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    warrants.EditingNewRecord = false;
                    if (v_warrants.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)v_warrants[v_warrants.Position]).Delete();
                    }
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            is_editable = true;
            MenuCallback.EditingStateUpdate();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                warrants.EditingNewRecord = false;
            base.Close();
        }

        void v_warrants_CurrentItemChanged(object sender, EventArgs e)
        {
            if (v_warrants.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (v_warrants.Position >= dataGridView.RowCount)
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                else
                    if (dataGridView.Rows[v_warrants.Position].Selected != true)
                        dataGridView.Rows[v_warrants.Position].Selected = true;
            if (Selected)
                MenuCallback.NavigationStateUpdate();
            if (v_warrants.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        void dateTimePickerWarrantDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxWarrantDocType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxWarrantRegNum_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxWarrantRegion_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxWarrantNotaryDistrict_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxWarrantNotary_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxWarrantDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarrantsViewport));
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.label88 = new System.Windows.Forms.Label();
            this.textBoxWarrantOnBehalfOf = new System.Windows.Forms.TextBox();
            this.label87 = new System.Windows.Forms.Label();
            this.textBoxWarrantDistrict = new System.Windows.Forms.TextBox();
            this.label86 = new System.Windows.Forms.Label();
            this.textBoxWarrantNotary = new System.Windows.Forms.TextBox();
            this.label85 = new System.Windows.Forms.Label();
            this.label84 = new System.Windows.Forms.Label();
            this.comboBoxWarrantDocType = new System.Windows.Forms.ComboBox();
            this.textBoxWarrantRegNum = new System.Windows.Forms.TextBox();
            this.dateTimePickerWarrantDate = new System.Windows.Forms.DateTimePicker();
            this.label83 = new System.Windows.Forms.Label();
            this.groupBox33 = new System.Windows.Forms.GroupBox();
            this.textBoxWarrantDescription = new System.Windows.Forms.TextBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_warrant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_date = new CustomControls.DataGridViewDateTimeColumn();
            this.notary = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.on_behalf_of = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel14.SuspendLayout();
            this.groupBox32.SuspendLayout();
            this.groupBox33.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.ColumnCount = 2;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.Controls.Add(this.groupBox32, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.groupBox33, 1, 0);
            this.tableLayoutPanel14.Controls.Add(this.dataGridView, 0, 1);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 2;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(653, 370);
            this.tableLayoutPanel14.TabIndex = 0;
            // 
            // groupBox32
            // 
            this.groupBox32.Controls.Add(this.label88);
            this.groupBox32.Controls.Add(this.textBoxWarrantOnBehalfOf);
            this.groupBox32.Controls.Add(this.label87);
            this.groupBox32.Controls.Add(this.textBoxWarrantDistrict);
            this.groupBox32.Controls.Add(this.label86);
            this.groupBox32.Controls.Add(this.textBoxWarrantNotary);
            this.groupBox32.Controls.Add(this.label85);
            this.groupBox32.Controls.Add(this.label84);
            this.groupBox32.Controls.Add(this.comboBoxWarrantDocType);
            this.groupBox32.Controls.Add(this.textBoxWarrantRegNum);
            this.groupBox32.Controls.Add(this.dateTimePickerWarrantDate);
            this.groupBox32.Controls.Add(this.label83);
            this.groupBox32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox32.Location = new System.Drawing.Point(3, 3);
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.Size = new System.Drawing.Size(320, 194);
            this.groupBox32.TabIndex = 0;
            this.groupBox32.TabStop = false;
            this.groupBox32.Text = "Основные сведения";
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Location = new System.Drawing.Point(17, 167);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(138, 15);
            this.label88.TabIndex = 51;
            this.label88.Text = "Действует в лице кого";
            // 
            // textBoxWarrantOnBehalfOf
            // 
            this.textBoxWarrantOnBehalfOf.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWarrantOnBehalfOf.Location = new System.Drawing.Point(175, 164);
            this.textBoxWarrantOnBehalfOf.MaxLength = 100;
            this.textBoxWarrantOnBehalfOf.Name = "textBoxWarrantOnBehalfOf";
            this.textBoxWarrantOnBehalfOf.Size = new System.Drawing.Size(139, 21);
            this.textBoxWarrantOnBehalfOf.TabIndex = 5;
            this.textBoxWarrantOnBehalfOf.TextChanged += new System.EventHandler(this.textBoxWarrantNotaryDistrict_TextChanged);
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Location = new System.Drawing.Point(17, 138);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(138, 15);
            this.label87.TabIndex = 49;
            this.label87.Text = "Нотариального округа";
            // 
            // textBoxWarrantDistrict
            // 
            this.textBoxWarrantDistrict.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWarrantDistrict.Location = new System.Drawing.Point(175, 135);
            this.textBoxWarrantDistrict.MaxLength = 100;
            this.textBoxWarrantDistrict.Name = "textBoxWarrantDistrict";
            this.textBoxWarrantDistrict.Size = new System.Drawing.Size(139, 21);
            this.textBoxWarrantDistrict.TabIndex = 4;
            this.textBoxWarrantDistrict.TextChanged += new System.EventHandler(this.textBoxWarrantRegion_TextChanged);
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(17, 109);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(145, 15);
            this.label86.TabIndex = 47;
            this.label86.Text = "Удостовер. нотариусом";
            // 
            // textBoxWarrantNotary
            // 
            this.textBoxWarrantNotary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWarrantNotary.Location = new System.Drawing.Point(175, 106);
            this.textBoxWarrantNotary.MaxLength = 100;
            this.textBoxWarrantNotary.Name = "textBoxWarrantNotary";
            this.textBoxWarrantNotary.Size = new System.Drawing.Size(139, 21);
            this.textBoxWarrantNotary.TabIndex = 3;
            this.textBoxWarrantNotary.TextChanged += new System.EventHandler(this.textBoxWarrantNotary_TextChanged);
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Location = new System.Drawing.Point(17, 51);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(152, 15);
            this.label85.TabIndex = 45;
            this.label85.Text = "Регистрационный номер";
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Location = new System.Drawing.Point(17, 23);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(93, 15);
            this.label84.TabIndex = 44;
            this.label84.Text = "Тип документа";
            // 
            // comboBoxWarrantDocType
            // 
            this.comboBoxWarrantDocType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxWarrantDocType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWarrantDocType.FormattingEnabled = true;
            this.comboBoxWarrantDocType.Location = new System.Drawing.Point(175, 19);
            this.comboBoxWarrantDocType.Name = "comboBoxWarrantDocType";
            this.comboBoxWarrantDocType.Size = new System.Drawing.Size(139, 23);
            this.comboBoxWarrantDocType.TabIndex = 0;
            this.comboBoxWarrantDocType.SelectedIndexChanged += new System.EventHandler(this.comboBoxWarrantDocType_SelectedIndexChanged);
            // 
            // textBoxWarrantRegNum
            // 
            this.textBoxWarrantRegNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWarrantRegNum.Location = new System.Drawing.Point(175, 48);
            this.textBoxWarrantRegNum.MaxLength = 10;
            this.textBoxWarrantRegNum.Name = "textBoxWarrantRegNum";
            this.textBoxWarrantRegNum.Size = new System.Drawing.Size(139, 21);
            this.textBoxWarrantRegNum.TabIndex = 1;
            this.textBoxWarrantRegNum.TextChanged += new System.EventHandler(this.textBoxWarrantRegNum_TextChanged);
            // 
            // dateTimePickerWarrantDate
            // 
            this.dateTimePickerWarrantDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerWarrantDate.Location = new System.Drawing.Point(175, 77);
            this.dateTimePickerWarrantDate.Name = "dateTimePickerWarrantDate";
            this.dateTimePickerWarrantDate.Size = new System.Drawing.Size(139, 21);
            this.dateTimePickerWarrantDate.TabIndex = 2;
            this.dateTimePickerWarrantDate.ValueChanged += new System.EventHandler(this.dateTimePickerWarrantDate_ValueChanged);
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Location = new System.Drawing.Point(17, 80);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(37, 15);
            this.label83.TabIndex = 41;
            this.label83.Text = "Дата";
            // 
            // groupBox33
            // 
            this.groupBox33.Controls.Add(this.textBoxWarrantDescription);
            this.groupBox33.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox33.Location = new System.Drawing.Point(329, 3);
            this.groupBox33.Name = "groupBox33";
            this.groupBox33.Size = new System.Drawing.Size(321, 194);
            this.groupBox33.TabIndex = 1;
            this.groupBox33.TabStop = false;
            this.groupBox33.Text = "Дополнительные сведения";
            // 
            // textBoxWarrantDescription
            // 
            this.textBoxWarrantDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxWarrantDescription.Location = new System.Drawing.Point(3, 17);
            this.textBoxWarrantDescription.MaxLength = 4000;
            this.textBoxWarrantDescription.Multiline = true;
            this.textBoxWarrantDescription.Name = "textBoxWarrantDescription";
            this.textBoxWarrantDescription.Size = new System.Drawing.Size(315, 174);
            this.textBoxWarrantDescription.TabIndex = 0;
            this.textBoxWarrantDescription.TextChanged += new System.EventHandler(this.textBoxWarrantDescription_TextChanged);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_warrant,
            this.registration_num,
            this.registration_date,
            this.notary,
            this.on_behalf_of,
            this.description});
            this.tableLayoutPanel14.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 203);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(647, 164);
            this.dataGridView.TabIndex = 6;
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // id_warrant
            // 
            this.id_warrant.Frozen = true;
            this.id_warrant.HeaderText = "Идентификатор доверенности";
            this.id_warrant.Name = "id_warrant";
            this.id_warrant.ReadOnly = true;
            this.id_warrant.Visible = false;
            // 
            // registration_num
            // 
            this.registration_num.HeaderText = "Регистрационный №";
            this.registration_num.MinimumWidth = 150;
            this.registration_num.Name = "registration_num";
            this.registration_num.ReadOnly = true;
            // 
            // registration_date
            // 
            this.registration_date.HeaderText = "Дата";
            this.registration_date.MinimumWidth = 150;
            this.registration_date.Name = "registration_date";
            this.registration_date.ReadOnly = true;
            // 
            // notary
            // 
            this.notary.HeaderText = "Нотариус";
            this.notary.MinimumWidth = 200;
            this.notary.Name = "notary";
            this.notary.ReadOnly = true;
            // 
            // on_behalf_of
            // 
            this.on_behalf_of.HeaderText = "В лице кого";
            this.on_behalf_of.MinimumWidth = 200;
            this.on_behalf_of.Name = "on_behalf_of";
            this.on_behalf_of.ReadOnly = true;
            // 
            // description
            // 
            this.description.FillWeight = 200F;
            this.description.HeaderText = "Примечание";
            this.description.MinimumWidth = 300;
            this.description.Name = "description";
            this.description.ReadOnly = true;
            // 
            // WarrantsViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(650, 310);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(659, 376);
            this.Controls.Add(this.tableLayoutPanel14);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WarrantsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Реестр доверенностей";
            this.tableLayoutPanel14.ResumeLayout(false);
            this.groupBox32.ResumeLayout(false);
            this.groupBox32.PerformLayout();
            this.groupBox33.ResumeLayout(false);
            this.groupBox33.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
