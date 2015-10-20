using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CustomControls;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

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
        DataModel warrants;
        DataModel warrant_doc_types;
        #endregion Models

        #region Views
        BindingSource v_warrants;
        BindingSource v_warrant_doc_types;
        #endregion Views

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable;

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
            DynamicFilter = warrantsViewport.DynamicFilter;
            StaticFilter = warrantsViewport.StaticFilter;
            ParentRow = warrantsViewport.ParentRow;
            ParentType = warrantsViewport.ParentType;
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
            if ((!ContainsFocus) || (dataGridView.Focused))
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
            if (!AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite))
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
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
            var Position = v_warrants.Find("id_warrant", id);
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
            var warrant = new Warrant();
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
            var warrant = new Warrant();
            var row = (DataRowView)v_warrants[v_warrants.Position];
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
            row["id_warrant"] = ViewportHelper.ValueOrDBNull(warrant.IdWarrant);
            row["id_warrant_doc_type"] = ViewportHelper.ValueOrDBNull(warrant.IdWarrantDocType);
            row["registration_num"] = ViewportHelper.ValueOrDBNull(warrant.RegistrationNum);
            row["registration_date"] = ViewportHelper.ValueOrDBNull(warrant.RegistrationDate);
            row["on_behalf_of"] = ViewportHelper.ValueOrDBNull(warrant.OnBehalfOf);
            row["notary"] = ViewportHelper.ValueOrDBNull(warrant.Notary);
            row["notary_district"] = ViewportHelper.ValueOrDBNull(warrant.NotaryDistrict);
            row["description"] = ViewportHelper.ValueOrDBNull(warrant.Description);
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
            DockAreas = DockAreas.Document;
            warrants = DataModel.GetInstance(DataModelType.WarrantsDataModel);
            warrant_doc_types = DataModel.GetInstance(DataModelType.WarrantDocTypesDataModel);

            // Ожидаем дозагрузки, если это необходимо
            warrants.Select();
            warrant_doc_types.Select();

            var ds = DataModel.DataSet;

            v_warrant_doc_types = new BindingSource();
            v_warrant_doc_types.DataMember = "warrant_doc_types";
            v_warrant_doc_types.DataSource = ds;

            v_warrants = new BindingSource();
            v_warrants.CurrentItemChanged += v_warrants_CurrentItemChanged;
            v_warrants.DataMember = "warrants";
            v_warrants.DataSource = ds;
            v_warrants.Sort = "registration_date DESC";
            warrants.Select().RowChanged += WarrantsViewport_RowChanged;
            warrants.Select().RowDeleted += WarrantsViewport_RowDeleted;

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
            var warrant = WarrantFromView();
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
                if (warrants.Delete((int)((DataRowView)v_warrants.Current)["id_warrant"]) == -1)
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
            var viewport = new WarrantsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_warrants.Count > 0)
                viewport.LocateWarrantBy((((DataRowView)v_warrants[v_warrants.Position])["id_warrant"] as int?) ?? -1);
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
            var warrant = WarrantFromViewport();
            if (!ValidateWarrant(warrant))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var id_warrant = warrants.Insert(warrant);
                    if (id_warrant == -1)
                    {
                        warrants.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    warrant.IdWarrant = id_warrant;
                    is_editable = false;
                    if (v_warrants.Position == -1)
                        newRow = (DataRowView)v_warrants.AddNew();
                    else
                        newRow = ((DataRowView)v_warrants[v_warrants.Position]);
                    FillRowFromWarrant(warrant, newRow);
                    is_editable = true;
                    warrants.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (warrant.IdWarrant == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись о доверенности без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (warrants.Update(warrant) == -1)
                        return;
                    var row = ((DataRowView)v_warrants[v_warrants.Position]);
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            warrants.Select().RowChanged -= WarrantsViewport_RowChanged;
            warrants.Select().RowDeleted -= WarrantsViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                warrants.EditingNewRecord = false;
            warrants.Select().RowChanged -= WarrantsViewport_RowChanged;
            warrants.Select().RowDeleted -= WarrantsViewport_RowDeleted;
            Close();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            textBoxWarrantRegNum.Focus();
            base.OnVisibleChanged(e);
        }

        void WarrantsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void WarrantsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
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
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
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
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(WarrantsViewport));
            tableLayoutPanel14 = new TableLayoutPanel();
            groupBox32 = new GroupBox();
            label88 = new Label();
            textBoxWarrantOnBehalfOf = new TextBox();
            label87 = new Label();
            textBoxWarrantDistrict = new TextBox();
            label86 = new Label();
            textBoxWarrantNotary = new TextBox();
            label85 = new Label();
            label84 = new Label();
            comboBoxWarrantDocType = new ComboBox();
            textBoxWarrantRegNum = new TextBox();
            dateTimePickerWarrantDate = new DateTimePicker();
            label83 = new Label();
            groupBox33 = new GroupBox();
            textBoxWarrantDescription = new TextBox();
            dataGridView = new DataGridView();
            id_warrant = new DataGridViewTextBoxColumn();
            registration_num = new DataGridViewTextBoxColumn();
            registration_date = new DataGridViewDateTimeColumn();
            notary = new DataGridViewTextBoxColumn();
            on_behalf_of = new DataGridViewTextBoxColumn();
            description = new DataGridViewTextBoxColumn();
            tableLayoutPanel14.SuspendLayout();
            groupBox32.SuspendLayout();
            groupBox33.SuspendLayout();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel14
            // 
            tableLayoutPanel14.ColumnCount = 2;
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel14.Controls.Add(groupBox32, 0, 0);
            tableLayoutPanel14.Controls.Add(groupBox33, 1, 0);
            tableLayoutPanel14.Controls.Add(dataGridView, 0, 1);
            tableLayoutPanel14.Dock = DockStyle.Fill;
            tableLayoutPanel14.Location = new Point(3, 3);
            tableLayoutPanel14.Name = "tableLayoutPanel14";
            tableLayoutPanel14.RowCount = 2;
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel14.Size = new Size(653, 370);
            tableLayoutPanel14.TabIndex = 0;
            // 
            // groupBox32
            // 
            groupBox32.Controls.Add(label88);
            groupBox32.Controls.Add(textBoxWarrantOnBehalfOf);
            groupBox32.Controls.Add(label87);
            groupBox32.Controls.Add(textBoxWarrantDistrict);
            groupBox32.Controls.Add(label86);
            groupBox32.Controls.Add(textBoxWarrantNotary);
            groupBox32.Controls.Add(label85);
            groupBox32.Controls.Add(label84);
            groupBox32.Controls.Add(comboBoxWarrantDocType);
            groupBox32.Controls.Add(textBoxWarrantRegNum);
            groupBox32.Controls.Add(dateTimePickerWarrantDate);
            groupBox32.Controls.Add(label83);
            groupBox32.Dock = DockStyle.Fill;
            groupBox32.Location = new Point(3, 3);
            groupBox32.Name = "groupBox32";
            groupBox32.Size = new Size(320, 194);
            groupBox32.TabIndex = 1;
            groupBox32.TabStop = false;
            groupBox32.Text = "Основные сведения";
            // 
            // label88
            // 
            label88.AutoSize = true;
            label88.Location = new Point(17, 167);
            label88.Name = "label88";
            label88.Size = new Size(138, 15);
            label88.TabIndex = 51;
            label88.Text = "Действует в лице кого";
            // 
            // textBoxWarrantOnBehalfOf
            // 
            textBoxWarrantOnBehalfOf.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                              | AnchorStyles.Right;
            textBoxWarrantOnBehalfOf.Location = new Point(175, 164);
            textBoxWarrantOnBehalfOf.MaxLength = 100;
            textBoxWarrantOnBehalfOf.Name = "textBoxWarrantOnBehalfOf";
            textBoxWarrantOnBehalfOf.Size = new Size(139, 21);
            textBoxWarrantOnBehalfOf.TabIndex = 5;
            textBoxWarrantOnBehalfOf.TextChanged += textBoxWarrantNotaryDistrict_TextChanged;
            textBoxWarrantOnBehalfOf.Enter += selectAll_Enter;
            // 
            // label87
            // 
            label87.AutoSize = true;
            label87.Location = new Point(17, 138);
            label87.Name = "label87";
            label87.Size = new Size(138, 15);
            label87.TabIndex = 49;
            label87.Text = "Нотариального округа";
            // 
            // textBoxWarrantDistrict
            // 
            textBoxWarrantDistrict.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            textBoxWarrantDistrict.Location = new Point(175, 135);
            textBoxWarrantDistrict.MaxLength = 100;
            textBoxWarrantDistrict.Name = "textBoxWarrantDistrict";
            textBoxWarrantDistrict.Size = new Size(139, 21);
            textBoxWarrantDistrict.TabIndex = 4;
            textBoxWarrantDistrict.TextChanged += textBoxWarrantRegion_TextChanged;
            textBoxWarrantDistrict.Enter += selectAll_Enter;
            // 
            // label86
            // 
            label86.AutoSize = true;
            label86.Location = new Point(17, 109);
            label86.Name = "label86";
            label86.Size = new Size(145, 15);
            label86.TabIndex = 47;
            label86.Text = "Удостовер. нотариусом";
            // 
            // textBoxWarrantNotary
            // 
            textBoxWarrantNotary.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                          | AnchorStyles.Right;
            textBoxWarrantNotary.Location = new Point(175, 106);
            textBoxWarrantNotary.MaxLength = 100;
            textBoxWarrantNotary.Name = "textBoxWarrantNotary";
            textBoxWarrantNotary.Size = new Size(139, 21);
            textBoxWarrantNotary.TabIndex = 3;
            textBoxWarrantNotary.TextChanged += textBoxWarrantNotary_TextChanged;
            textBoxWarrantNotary.Enter += selectAll_Enter;
            // 
            // label85
            // 
            label85.AutoSize = true;
            label85.Location = new Point(17, 51);
            label85.Name = "label85";
            label85.Size = new Size(152, 15);
            label85.TabIndex = 45;
            label85.Text = "Регистрационный номер";
            // 
            // label84
            // 
            label84.AutoSize = true;
            label84.Location = new Point(17, 23);
            label84.Name = "label84";
            label84.Size = new Size(93, 15);
            label84.TabIndex = 44;
            label84.Text = "Тип документа";
            // 
            // comboBoxWarrantDocType
            // 
            comboBoxWarrantDocType.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            comboBoxWarrantDocType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxWarrantDocType.FormattingEnabled = true;
            comboBoxWarrantDocType.Location = new Point(175, 19);
            comboBoxWarrantDocType.Name = "comboBoxWarrantDocType";
            comboBoxWarrantDocType.Size = new Size(139, 23);
            comboBoxWarrantDocType.TabIndex = 0;
            comboBoxWarrantDocType.SelectedIndexChanged += comboBoxWarrantDocType_SelectedIndexChanged;
            // 
            // textBoxWarrantRegNum
            // 
            textBoxWarrantRegNum.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                          | AnchorStyles.Right;
            textBoxWarrantRegNum.Location = new Point(175, 48);
            textBoxWarrantRegNum.MaxLength = 10;
            textBoxWarrantRegNum.Name = "textBoxWarrantRegNum";
            textBoxWarrantRegNum.Size = new Size(139, 21);
            textBoxWarrantRegNum.TabIndex = 1;
            textBoxWarrantRegNum.TextChanged += textBoxWarrantRegNum_TextChanged;
            textBoxWarrantRegNum.Enter += selectAll_Enter;
            // 
            // dateTimePickerWarrantDate
            // 
            dateTimePickerWarrantDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                               | AnchorStyles.Right;
            dateTimePickerWarrantDate.Location = new Point(175, 77);
            dateTimePickerWarrantDate.Name = "dateTimePickerWarrantDate";
            dateTimePickerWarrantDate.Size = new Size(139, 21);
            dateTimePickerWarrantDate.TabIndex = 2;
            dateTimePickerWarrantDate.ValueChanged += dateTimePickerWarrantDate_ValueChanged;
            // 
            // label83
            // 
            label83.AutoSize = true;
            label83.Location = new Point(17, 80);
            label83.Name = "label83";
            label83.Size = new Size(37, 15);
            label83.TabIndex = 41;
            label83.Text = "Дата";
            // 
            // groupBox33
            // 
            groupBox33.Controls.Add(textBoxWarrantDescription);
            groupBox33.Dock = DockStyle.Fill;
            groupBox33.Location = new Point(329, 3);
            groupBox33.Name = "groupBox33";
            groupBox33.Size = new Size(321, 194);
            groupBox33.TabIndex = 2;
            groupBox33.TabStop = false;
            groupBox33.Text = "Дополнительные сведения";
            // 
            // textBoxWarrantDescription
            // 
            textBoxWarrantDescription.Dock = DockStyle.Fill;
            textBoxWarrantDescription.Location = new Point(3, 17);
            textBoxWarrantDescription.MaxLength = 4000;
            textBoxWarrantDescription.Multiline = true;
            textBoxWarrantDescription.Name = "textBoxWarrantDescription";
            textBoxWarrantDescription.Size = new Size(315, 174);
            textBoxWarrantDescription.TabIndex = 0;
            textBoxWarrantDescription.TextChanged += textBoxWarrantDescription_TextChanged;
            textBoxWarrantDescription.Enter += selectAll_Enter;
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_warrant, registration_num, registration_date, notary, on_behalf_of, description);
            tableLayoutPanel14.SetColumnSpan(dataGridView, 2);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 203);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(647, 164);
            dataGridView.TabIndex = 0;
            dataGridView.DataError += dataGridView_DataError;
            // 
            // id_warrant
            // 
            id_warrant.Frozen = true;
            id_warrant.HeaderText = "Идентификатор доверенности";
            id_warrant.Name = "id_warrant";
            id_warrant.ReadOnly = true;
            id_warrant.Visible = false;
            // 
            // registration_num
            // 
            registration_num.HeaderText = "Регистрационный №";
            registration_num.MinimumWidth = 150;
            registration_num.Name = "registration_num";
            registration_num.ReadOnly = true;
            // 
            // registration_date
            // 
            registration_date.HeaderText = "Дата";
            registration_date.MinimumWidth = 150;
            registration_date.Name = "registration_date";
            registration_date.ReadOnly = true;
            // 
            // notary
            // 
            notary.HeaderText = "Нотариус";
            notary.MinimumWidth = 200;
            notary.Name = "notary";
            notary.ReadOnly = true;
            // 
            // on_behalf_of
            // 
            on_behalf_of.HeaderText = "В лице кого";
            on_behalf_of.MinimumWidth = 200;
            on_behalf_of.Name = "on_behalf_of";
            on_behalf_of.ReadOnly = true;
            // 
            // description
            // 
            description.FillWeight = 200F;
            description.HeaderText = "Примечание";
            description.MinimumWidth = 300;
            description.Name = "description";
            description.ReadOnly = true;
            // 
            // WarrantsViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(650, 310);
            BackColor = Color.White;
            ClientSize = new Size(659, 376);
            Controls.Add(tableLayoutPanel14);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "WarrantsViewport";
            Padding = new Padding(3);
            Text = "Реестр доверенностей";
            tableLayoutPanel14.ResumeLayout(false);
            groupBox32.ResumeLayout(false);
            groupBox32.PerformLayout();
            groupBox33.ResumeLayout(false);
            groupBox33.PerformLayout();
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
