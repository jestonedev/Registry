using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;
using System.Drawing;
using Security;
using System.Globalization;

namespace Registry.Viewport
{
    internal sealed class ClaimStatesViewport: Viewport
    {
        #region Components
        TableLayoutPanel tableLayoutPanel17;
        TableLayoutPanel tableLayoutPanel18;
        GroupBox groupBox35;
        Panel panel10;
        Panel panel11;
        Label label100;
        Label label101;
        Label label105;
        Label label108;
        Label label109;
        Label label110;
        TextBox textBoxDescription;
        TextBox textBoxDocumentNumber;
        ComboBox comboBoxClaimStateType;
        DateTimePicker dateTimePickerDocDate;
        DateTimePicker dateTimePickerStartState;
        DateTimePicker dateTimePickerEndState;
        DataGridView dataGridView;
        #endregion Components

        #region Models
        ClaimStatesDataModel claim_states = null;
        ClaimStateTypesDataModel claim_state_types = null;
        ClaimStateTypesRelationsDataModel claim_state_types_relations = null;
        #endregion Models

        #region Views
        BindingSource v_claim_states = null;
        BindingSource v_claim_state_types = null;
        BindingSource v_claim_state_types_for_grid = null;
        BindingSource v_claim_state_types_relations = null;
        #endregion Views

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private DataGridViewComboBoxColumn id_state_type;
        private DataGridViewTextBoxColumn date_start_state;
        private DataGridViewTextBoxColumn date_end_state;
        private DataGridViewTextBoxColumn description;
        private bool is_editable = false;

        private ClaimStatesViewport()
            : this(null)
        {
            
        }

        public ClaimStatesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public ClaimStatesViewport(ClaimStatesViewport claimStatesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = claimStatesViewport.DynamicFilter;
            this.StaticFilter = claimStatesViewport.StaticFilter;
            this.ParentRow = claimStatesViewport.ParentRow;
            this.ParentType = claimStatesViewport.ParentType;
        }

        private void RebuildFilter()
        {
            string filter = "";
            IEnumerable<int> included_states = null;
            // Если текущая позиция - первый элемент, и количество элементов 1 то он может иметь только начальное состояние (любое)
            if ((v_claim_states.Position == 0) && (v_claim_states.Count == 1))
                included_states = DataModelHelper.ClaimStartStateTypeIds();
            else
            // Если текущая позиция - первый элемент, и количество элементов 1 то он может иметь только начальное состояние 
            // (не противоречащее следующей позиции)
            if ((v_claim_states.Position == 0) && (v_claim_states.Count > 1))
            {
                int next_claim_state_type = Convert.ToInt32(
                    ((DataRowView)v_claim_states[v_claim_states.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
                included_states = DataModelHelper.ClaimStateTypeIdsByNextStateType(next_claim_state_type);
            }
            else
            // Если текущая позиция - последний элемент, то выбрать состояние, в которое можно перейти из состояния предыдущего элемента
            if ((v_claim_states.Position != -1) && (v_claim_states.Position == (v_claim_states.Count - 1)))
            {
                int prev_claim_state_type = Convert.ToInt32(
                    ((DataRowView)v_claim_states[v_claim_states.Position - 1])["id_state_type"], CultureInfo.InvariantCulture);
                included_states = DataModelHelper.ClaimStateTypeIdsByPrevStateType(prev_claim_state_type);
            }
            else
            // Мы находимся не в конце списка и не в начале и необходимо выбрать только те состояния, в которые можно перейти с учетом окружающих состояний
            if (v_claim_states.Position != -1)
            {
                int prev_claim_state_type = 
                    Convert.ToInt32(((DataRowView)v_claim_states[v_claim_states.Position - 1])["id_state_type"], CultureInfo.InvariantCulture);
                int next_claim_state_type = 
                    Convert.ToInt32(((DataRowView)v_claim_states[v_claim_states.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
                included_states = DataModelHelper.ClaimStateTypeIdsByNextAndPrevStateTypes(next_claim_state_type, prev_claim_state_type); 
            }
            if (included_states != null)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_state_type IN (0";
                foreach (int id in included_states)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            v_claim_state_types.Filter = filter;
            //Делаем перепривязку ComboboxStateType
            if (v_claim_states.Position != -1)
            {
                object id_state_type = ((DataRowView)v_claim_states[v_claim_states.Position])["id_state_type"];
                // Состояние существует, но его возможные тип определить не удалось из-за изменений в ветке зависимостей типов состояний
                if ((v_claim_state_types.Find("id_state_type", id_state_type) == -1) && (viewportState != ViewportState.NewRowState))
                {
                    label109.ForeColor = Color.Red;
                    label109.Text = "Вид состояния (ошибка)";
                    v_claim_state_types.Filter = "";
                }
                else
                {
                    label109.ForeColor = SystemColors.WindowText;
                    label109.Text = "Вид состояния";
                }
                comboBoxClaimStateType.SelectedValue = ((DataRowView)v_claim_states[v_claim_states.Position])["id_state_type"];
            }
        }

        private void DataBind()
        {
            comboBoxClaimStateType.DataSource = v_claim_state_types;
            comboBoxClaimStateType.ValueMember = "id_state_type";
            comboBoxClaimStateType.DisplayMember = "state_type";
            comboBoxClaimStateType.DataBindings.Clear();
            comboBoxClaimStateType.DataBindings.Add("SelectedValue", v_claim_states, "id_state_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxDocumentNumber.DataBindings.Clear();
            textBoxDocumentNumber.DataBindings.Add("Text", v_claim_states, "document_num", true, DataSourceUpdateMode.Never, "");
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", v_claim_states, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDocDate.DataBindings.Clear();
            dateTimePickerDocDate.DataBindings.Add("Value", v_claim_states, "document_date", true, DataSourceUpdateMode.Never, null);
            dateTimePickerStartState.DataBindings.Clear();
            dateTimePickerStartState.DataBindings.Add("Value", v_claim_states, "date_start_state", true, DataSourceUpdateMode.Never, null);
            dateTimePickerEndState.DataBindings.Clear();
            dateTimePickerEndState.DataBindings.Add("Value", v_claim_states, "date_end_state", true, DataSourceUpdateMode.Never, null);

            dataGridView.DataSource = v_claim_states;
            id_state_type.DataSource = v_claim_state_types_for_grid;
            id_state_type.DisplayMember = "state_type";
            id_state_type.ValueMember = "id_state_type";
            id_state_type.DataPropertyName = "id_state_type";
            date_start_state.DataPropertyName = "date_start_state";
            date_end_state.DataPropertyName = "date_end_state";
            description.DataPropertyName = "description";
        }

        private void UnbindedCheckBoxesUpdate()
        {
            DataRowView row = (v_claim_states.Position >= 0) ? (DataRowView)v_claim_states[v_claim_states.Position] : null;
            if ((v_claim_states.Position >= 0) && (row["document_date"] != DBNull.Value))
                dateTimePickerDocDate.Checked = true;
            else
            {
                dateTimePickerDocDate.Value = DateTime.Now.Date;
                dateTimePickerDocDate.Checked = false;
            }
            if ((v_claim_states.Position >= 0) && (row["date_start_state"] != DBNull.Value))
                dateTimePickerStartState.Checked = true;
            else
            {
                dateTimePickerStartState.Value = DateTime.Now.Date;
                dateTimePickerStartState.Checked = false;
            }

            if ((v_claim_states.Position >= 0) && (row["date_end_state"] != DBNull.Value))
                dateTimePickerEndState.Checked = true;
            else
            {
                dateTimePickerEndState.Value = DateTime.Now.Date;
                dateTimePickerEndState.Checked = false;
            }
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!this.ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_claim_states.Position != -1) && (ClaimStateFromView() != ClaimStateFromViewport()))
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
            if (!AccessControl.HasPrivelege(Priveleges.ClaimsWrite))
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
                            if (claim_states.EditingNewRecord)
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

        private void LocateClaimStateBy(int id)
        {
            int Position = v_claim_states.Find("id_state", id);
            is_editable = false;
            if (Position > 0)
                v_claim_states.Position = Position;
            is_editable = true;
        }

        private static bool ValidateClaimState(ClaimState claimState)
        {
            if (claimState.IdStateType == null)
            {
                MessageBox.Show("Необходимо выбрать тип состояния претензионно-исковой работы", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private static void FillRowFromClaimState(ClaimState claimState, DataRowView row)
        {
            row.BeginEdit();
            row["id_state"] = ViewportHelper.ValueOrDBNull(claimState.IdState);
            row["id_claim"] = ViewportHelper.ValueOrDBNull(claimState.IdClaim);
            row["id_state_type"] = ViewportHelper.ValueOrDBNull(claimState.IdStateType);
            row["date_start_state"] = ViewportHelper.ValueOrDBNull(claimState.DateStartState);
            row["date_end_state"] = ViewportHelper.ValueOrDBNull(claimState.DateEndState);
            row["document_num"] = ViewportHelper.ValueOrDBNull(claimState.DocumentNum);
            row["document_date"] = ViewportHelper.ValueOrDBNull(claimState.DocumentDate);
            row["description"] = ViewportHelper.ValueOrDBNull(claimState.Description);
            row.EndEdit();
        }

        private void ViewportFromClaimState(ClaimState claimState)
        {
            comboBoxClaimStateType.SelectedValue = ViewportHelper.ValueOrDBNull(claimState.IdStateType);
            textBoxDocumentNumber.Text = claimState.DocumentNum;
            textBoxDescription.Text = claimState.Description;
            dateTimePickerDocDate.Value = ViewportHelper.ValueOrDefault(claimState.DocumentDate);
            dateTimePickerStartState.Value = ViewportHelper.ValueOrDefault(claimState.DateStartState);
            dateTimePickerEndState.Value = ViewportHelper.ValueOrDefault(claimState.DateEndState);
        }

        private ClaimState ClaimStateFromViewport()
        {
            ClaimState claimState = new ClaimState();
            if (v_claim_states.Position == -1)
                claimState.IdState = null;
            else
                claimState.IdState = ViewportHelper.ValueOrNull<int>((DataRowView)v_claim_states[v_claim_states.Position], "id_state");
            claimState.IdStateType = ViewportHelper.ValueOrNull<int>(comboBoxClaimStateType);
            claimState.IdClaim = ViewportHelper.ValueOrNull<int>(ParentRow, "id_claim");
            claimState.DocumentNum = ViewportHelper.ValueOrNull(textBoxDocumentNumber);
            claimState.Description = ViewportHelper.ValueOrNull(textBoxDescription);
            claimState.DateStartState = ViewportHelper.ValueOrNull(dateTimePickerStartState);
            claimState.DateEndState = ViewportHelper.ValueOrNull(dateTimePickerEndState);
            claimState.DocumentDate = ViewportHelper.ValueOrNull(dateTimePickerDocDate);
            return claimState;
        }

        private ClaimState ClaimStateFromView()
        {
            ClaimState claimState = new ClaimState();
            DataRowView row = (DataRowView)v_claim_states[v_claim_states.Position];
            claimState.IdState = ViewportHelper.ValueOrNull<int>(row, "id_state");
            claimState.IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type");
            claimState.IdClaim = ViewportHelper.ValueOrNull<int>(row, "id_claim");
            claimState.DocumentNum = ViewportHelper.ValueOrNull(row, "document_num");
            claimState.Description = ViewportHelper.ValueOrNull(row, "description");
            claimState.DateStartState = ViewportHelper.ValueOrNull<DateTime>(row, "date_start_state");
            claimState.DateEndState = ViewportHelper.ValueOrNull<DateTime>(row, "date_end_state");
            claimState.DocumentDate = ViewportHelper.ValueOrNull<DateTime>(row, "document_date");
            return claimState;
        }

        public override int GetRecordCount()
        {
            return v_claim_states.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claim_states.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claim_states.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claim_states.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_claim_states.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_claim_states.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_claim_states.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_claim_states.Position > -1) && (v_claim_states.Position < (v_claim_states.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_claim_states.Position > -1) && (v_claim_states.Position < (v_claim_states.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            dataGridView.AutoGenerateColumns = false;
            claim_states = ClaimStatesDataModel.GetInstance();
            claim_state_types = ClaimStateTypesDataModel.GetInstance();
            claim_state_types_relations = ClaimStateTypesRelationsDataModel.GetInstance();

            //Ожидаем дозагрузки, если это необходимо
            claim_states.Select();
            claim_state_types.Select();
            claim_state_types_relations.Select();

            DataSet ds = DataSetManager.DataSet;

            if (ParentType == ParentTypeEnum.Claim && ParentRow != null)
                this.Text = String.Format(CultureInfo.InvariantCulture, "Состояния иск. работы №{0}", ParentRow["id_claim"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            v_claim_state_types = new BindingSource();
            v_claim_state_types.DataMember = "claim_state_types";
            v_claim_state_types.DataSource = ds;

            v_claim_state_types_for_grid = new BindingSource();
            v_claim_state_types_for_grid.DataMember = "claim_state_types";
            v_claim_state_types_for_grid.DataSource = ds;

            v_claim_state_types_relations = new BindingSource();
            v_claim_state_types_relations.DataMember = "claim_state_types_relations";
            v_claim_state_types_relations.DataSource = ds;

            v_claim_states = new BindingSource();
            v_claim_states.CurrentItemChanged += new EventHandler(v_claim_states_CurrentItemChanged);
            v_claim_states.DataMember = "claim_states";
            v_claim_states.DataSource = ds;
            v_claim_states.Filter = StaticFilter;

            DataBind();

            claim_states.Select().RowChanged += new DataRowChangeEventHandler(ClaimStatesViewport_RowChanged);
            claim_states.Select().RowDeleted += new DataRowChangeEventHandler(ClaimStatesViewport_RowDeleted);
            claim_state_types.Select().RowChanged += new DataRowChangeEventHandler(ClaimStateTypesViewport_RowChanged);
            claim_state_types.Select().RowDeleted += new DataRowChangeEventHandler(ClaimStateTypesViewport_RowDeleted);
            claim_state_types_relations.Select().RowChanged += new DataRowChangeEventHandler(ClaimStateTypesRelationsViewport_RowChanged);
            claim_state_types_relations.Select().RowDeleted += new DataRowChangeEventHandler(ClaimStateTypesRelationsViewport_RowDeleted);
            is_editable = true;
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void SaveRecord()
        {
            ClaimState claimState = ClaimStateFromViewport();
            if (!ValidateClaimState(claimState))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    int id_state = ClaimStatesDataModel.Insert(claimState);
                    if (id_state == -1)
                        return;
                    DataRowView newRow;
                    claimState.IdState = id_state;
                    is_editable = false;
                    if (v_claim_states.Position == -1)
                        newRow = (DataRowView)v_claim_states.AddNew();
                    else
                        newRow = ((DataRowView)v_claim_states[v_claim_states.Position]);
                    FillRowFromClaimState(claimState, newRow);
                    v_claim_states.Position = v_claim_states.Count - 1;
                    claim_states.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (claimState.IdState == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись о состоянии претензионно-исковой работы без внутреннего номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (ClaimStatesDataModel.Update(claimState) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_claim_states[v_claim_states.Position]);
                    is_editable = false;
                    FillRowFromClaimState(claimState, row);
                    break;
            }
            UnbindedCheckBoxesUpdate();
            dataGridView.Enabled = true;
            is_editable = true;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanCopyRecord()
        {
            return (v_claim_states.Position != -1) && (!claim_states.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            ClaimState claimState = ClaimStateFromView();
            v_claim_states.AddNew();
            dataGridView.Enabled = false;
            claim_states.EditingNewRecord = true;
            ViewportFromClaimState(claimState);
            dateTimePickerDocDate.Checked = (claimState.DocumentDate != null);
            dateTimePickerStartState.Checked = (claimState.DateStartState != null);
            dateTimePickerEndState.Checked = (claimState.DateEndState != null);
            is_editable = true;
        }

        public override bool CanInsertRecord()
        {
            return (!claim_states.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            v_claim_states.AddNew();
            is_editable = true;
            dataGridView.Enabled = false;
            claim_states.EditingNewRecord = true; 
        }

        public override bool CanDeleteRecord()
        {
            return (v_claim_states.Position > -1) 
                && (viewportState != ViewportState.NewRowState) 
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                int stateCount = -1;
                // Мы находимся в начале списка и текущий элемент не последний
                if ((v_claim_states.Position == 0) && (v_claim_states.Count > 1))
                {
                    int next_claim_state_type = 
                        Convert.ToInt32(((DataRowView)v_claim_states[v_claim_states.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
                    stateCount = (from claim_state_types_row in DataModelHelper.FilterRows(claim_state_types.Select())
                                  where Convert.ToBoolean(claim_state_types_row.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture) &&
                                        (claim_state_types_row.Field<int>("id_state_type") == next_claim_state_type)
                                    select claim_state_types_row.Field<int>("id_state_type")).Count();
                }
                else
                    // Мы находимся не в конце списка и не в начале
                    if ((v_claim_states.Position != -1) && (v_claim_states.Position != (v_claim_states.Count - 1)))
                    {
                        int previos_claim_state_type = 
                            Convert.ToInt32(((DataRowView)v_claim_states[v_claim_states.Position - 1])["id_state_type"], CultureInfo.InvariantCulture);
                        int next_claim_state_type = 
                            Convert.ToInt32(((DataRowView)v_claim_states[v_claim_states.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
                        stateCount = (from claim_state_types_rel_row in DataModelHelper.FilterRows(claim_state_types_relations.Select())
                                           where claim_state_types_rel_row.Field<int>("id_state_from") == previos_claim_state_type &&
                                                 claim_state_types_rel_row.Field<int>("id_state_to") == next_claim_state_type
                                           select claim_state_types_rel_row.Field<int>("id_state_to")).Count();
                    }
                if (stateCount == 0)
                {
                    MessageBox.Show("Вы не можете удалить это состояние, так как это нарушит цепочку зависимости состояний претензионно-исковой работы."+
                        "Чтобы удалить данное состояние, необходимо сначала удалить все состояния после него", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (ClaimStatesDataModel.Delete((int)((DataRowView)v_claim_states.Current)["id_state"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_claim_states[v_claim_states.Position]).Delete();
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                is_editable = true;
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
                    claim_states.EditingNewRecord = false;
                    if (v_claim_states.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)v_claim_states[v_claim_states.Position]).Delete();
                        if (v_claim_states.Position != -1)
                            dataGridView.Rows[v_claim_states.Position].Selected = true;
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
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            ClaimStatesViewport viewport = new ClaimStatesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_claim_states.Count > 0)
                viewport.LocateClaimStateBy((((DataRowView)v_claim_states[v_claim_states.Position])["id_claim"] as Int32?) ?? -1);
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
                claim_states.Select().RowChanged -= new DataRowChangeEventHandler(ClaimStatesViewport_RowChanged);
                claim_states.Select().RowDeleted -= new DataRowChangeEventHandler(ClaimStatesViewport_RowDeleted);
                claim_state_types.Select().RowChanged -= new DataRowChangeEventHandler(ClaimStateTypesViewport_RowChanged);
                claim_state_types.Select().RowDeleted -= new DataRowChangeEventHandler(ClaimStateTypesViewport_RowDeleted);
                claim_state_types_relations.Select().RowChanged -= new DataRowChangeEventHandler(ClaimStateTypesRelationsViewport_RowChanged);
                claim_state_types_relations.Select().RowDeleted -= new DataRowChangeEventHandler(ClaimStateTypesRelationsViewport_RowDeleted);
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                claim_states.EditingNewRecord = false;
            claim_states.Select().RowChanged -= new DataRowChangeEventHandler(ClaimStatesViewport_RowChanged);
            claim_states.Select().RowDeleted -= new DataRowChangeEventHandler(ClaimStatesViewport_RowDeleted);
            claim_state_types.Select().RowChanged -= new DataRowChangeEventHandler(ClaimStateTypesViewport_RowChanged);
            claim_state_types.Select().RowDeleted -= new DataRowChangeEventHandler(ClaimStateTypesViewport_RowDeleted);
            claim_state_types_relations.Select().RowChanged -= new DataRowChangeEventHandler(ClaimStateTypesRelationsViewport_RowChanged);
            claim_state_types_relations.Select().RowDeleted -= new DataRowChangeEventHandler(ClaimStateTypesRelationsViewport_RowDeleted);
            base.Close();
        }

        void v_claim_states_CurrentItemChanged(object sender, EventArgs e)
        {
            if (v_claim_states.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (v_claim_states.Position >= dataGridView.RowCount)
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                else
                    if (dataGridView.Rows[v_claim_states.Position].Selected != true)
                        dataGridView.Rows[v_claim_states.Position].Selected = true;
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            RebuildFilter();
            if (v_claim_states.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        void ClaimStatesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                UnbindedCheckBoxesUpdate();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        void ClaimStatesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void ClaimStateTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RebuildFilter();
        }

        void ClaimStateTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RebuildFilter();
        }

        void ClaimStateTypesRelationsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RebuildFilter();
        }

        void ClaimStateTypesRelationsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RebuildFilter();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            base.OnVisibleChanged(e);
        }

        void dataGridViewClaimStates_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        void dateTimePickerEndState_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerStartState_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerDocDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxClaimStateDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxDocNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxClaimStateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimStatesViewport));
            this.tableLayoutPanel17 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox35 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel18 = new System.Windows.Forms.TableLayoutPanel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.textBoxDocumentNumber = new System.Windows.Forms.TextBox();
            this.label100 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label101 = new System.Windows.Forms.Label();
            this.dateTimePickerDocDate = new System.Windows.Forms.DateTimePicker();
            this.label105 = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.dateTimePickerEndState = new System.Windows.Forms.DateTimePicker();
            this.label110 = new System.Windows.Forms.Label();
            this.comboBoxClaimStateType = new System.Windows.Forms.ComboBox();
            this.dateTimePickerStartState = new System.Windows.Forms.DateTimePicker();
            this.label108 = new System.Windows.Forms.Label();
            this.label109 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_state_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.date_start_state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_end_state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel17.SuspendLayout();
            this.groupBox35.SuspendLayout();
            this.tableLayoutPanel18.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel17
            // 
            this.tableLayoutPanel17.ColumnCount = 1;
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel17.Controls.Add(this.groupBox35, 0, 0);
            this.tableLayoutPanel17.Controls.Add(this.dataGridView, 0, 1);
            this.tableLayoutPanel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel17.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel17.Name = "tableLayoutPanel17";
            this.tableLayoutPanel17.RowCount = 2;
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel17.Size = new System.Drawing.Size(703, 205);
            this.tableLayoutPanel17.TabIndex = 1;
            // 
            // groupBox35
            // 
            this.groupBox35.Controls.Add(this.tableLayoutPanel18);
            this.groupBox35.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox35.Location = new System.Drawing.Point(0, 0);
            this.groupBox35.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox35.Name = "groupBox35";
            this.groupBox35.Size = new System.Drawing.Size(703, 110);
            this.groupBox35.TabIndex = 1;
            this.groupBox35.TabStop = false;
            this.groupBox35.Text = "Общие сведения";
            // 
            // tableLayoutPanel18
            // 
            this.tableLayoutPanel18.ColumnCount = 2;
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel18.Controls.Add(this.panel10, 1, 0);
            this.tableLayoutPanel18.Controls.Add(this.panel11, 0, 0);
            this.tableLayoutPanel18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel18.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel18.Name = "tableLayoutPanel18";
            this.tableLayoutPanel18.RowCount = 1;
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 148F));
            this.tableLayoutPanel18.Size = new System.Drawing.Size(697, 90);
            this.tableLayoutPanel18.TabIndex = 0;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.textBoxDocumentNumber);
            this.panel10.Controls.Add(this.label100);
            this.panel10.Controls.Add(this.textBoxDescription);
            this.panel10.Controls.Add(this.label101);
            this.panel10.Controls.Add(this.dateTimePickerDocDate);
            this.panel10.Controls.Add(this.label105);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(348, 0);
            this.panel10.Margin = new System.Windows.Forms.Padding(0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(349, 90);
            this.panel10.TabIndex = 1;
            // 
            // textBoxDocumentNumber
            // 
            this.textBoxDocumentNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocumentNumber.Location = new System.Drawing.Point(161, 4);
            this.textBoxDocumentNumber.MaxLength = 50;
            this.textBoxDocumentNumber.Name = "textBoxDocumentNumber";
            this.textBoxDocumentNumber.Size = new System.Drawing.Size(179, 21);
            this.textBoxDocumentNumber.TabIndex = 0;
            this.textBoxDocumentNumber.TextChanged += new System.EventHandler(this.textBoxDocNumber_TextChanged);
            // 
            // label100
            // 
            this.label100.AutoSize = true;
            this.label100.Location = new System.Drawing.Point(12, 7);
            this.label100.Name = "label100";
            this.label100.Size = new System.Drawing.Size(111, 15);
            this.label100.TabIndex = 51;
            this.label100.Text = "Номер документа";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(161, 62);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(179, 21);
            this.textBoxDescription.TabIndex = 2;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxClaimStateDescription_TextChanged);
            // 
            // label101
            // 
            this.label101.AutoSize = true;
            this.label101.Location = new System.Drawing.Point(12, 65);
            this.label101.Name = "label101";
            this.label101.Size = new System.Drawing.Size(80, 15);
            this.label101.TabIndex = 49;
            this.label101.Text = "Примечание";
            // 
            // dateTimePickerDocDate
            // 
            this.dateTimePickerDocDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDocDate.Location = new System.Drawing.Point(161, 33);
            this.dateTimePickerDocDate.Name = "dateTimePickerDocDate";
            this.dateTimePickerDocDate.ShowCheckBox = true;
            this.dateTimePickerDocDate.Size = new System.Drawing.Size(179, 21);
            this.dateTimePickerDocDate.TabIndex = 1;
            this.dateTimePickerDocDate.ValueChanged += new System.EventHandler(this.dateTimePickerDocDate_ValueChanged);
            // 
            // label105
            // 
            this.label105.AutoSize = true;
            this.label105.Location = new System.Drawing.Point(12, 36);
            this.label105.Name = "label105";
            this.label105.Size = new System.Drawing.Size(102, 15);
            this.label105.TabIndex = 44;
            this.label105.Text = "Дата документа";
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.dateTimePickerEndState);
            this.panel11.Controls.Add(this.label110);
            this.panel11.Controls.Add(this.comboBoxClaimStateType);
            this.panel11.Controls.Add(this.dateTimePickerStartState);
            this.panel11.Controls.Add(this.label108);
            this.panel11.Controls.Add(this.label109);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Margin = new System.Windows.Forms.Padding(0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(348, 90);
            this.panel11.TabIndex = 0;
            // 
            // dateTimePickerEndState
            // 
            this.dateTimePickerEndState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndState.Location = new System.Drawing.Point(161, 62);
            this.dateTimePickerEndState.Name = "dateTimePickerEndState";
            this.dateTimePickerEndState.ShowCheckBox = true;
            this.dateTimePickerEndState.Size = new System.Drawing.Size(178, 21);
            this.dateTimePickerEndState.TabIndex = 2;
            this.dateTimePickerEndState.ValueChanged += new System.EventHandler(this.dateTimePickerEndState_ValueChanged);
            // 
            // label110
            // 
            this.label110.AutoSize = true;
            this.label110.Location = new System.Drawing.Point(14, 65);
            this.label110.Name = "label110";
            this.label110.Size = new System.Drawing.Size(86, 15);
            this.label110.TabIndex = 38;
            this.label110.Text = "Крайний срок";
            // 
            // comboBoxClaimStateType
            // 
            this.comboBoxClaimStateType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxClaimStateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxClaimStateType.FormattingEnabled = true;
            this.comboBoxClaimStateType.Location = new System.Drawing.Point(161, 4);
            this.comboBoxClaimStateType.Name = "comboBoxClaimStateType";
            this.comboBoxClaimStateType.Size = new System.Drawing.Size(178, 23);
            this.comboBoxClaimStateType.TabIndex = 0;
            this.comboBoxClaimStateType.SelectedIndexChanged += new System.EventHandler(this.comboBoxClaimStateType_SelectedIndexChanged);
            // 
            // dateTimePickerStartState
            // 
            this.dateTimePickerStartState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartState.Location = new System.Drawing.Point(161, 33);
            this.dateTimePickerStartState.Name = "dateTimePickerStartState";
            this.dateTimePickerStartState.ShowCheckBox = true;
            this.dateTimePickerStartState.Size = new System.Drawing.Size(178, 21);
            this.dateTimePickerStartState.TabIndex = 1;
            this.dateTimePickerStartState.ValueChanged += new System.EventHandler(this.dateTimePickerStartState_ValueChanged);
            // 
            // label108
            // 
            this.label108.AutoSize = true;
            this.label108.Location = new System.Drawing.Point(14, 36);
            this.label108.Name = "label108";
            this.label108.Size = new System.Drawing.Size(99, 15);
            this.label108.TabIndex = 31;
            this.label108.Text = "Дата установки";
            // 
            // label109
            // 
            this.label109.AutoSize = true;
            this.label109.Location = new System.Drawing.Point(14, 7);
            this.label109.Name = "label109";
            this.label109.Size = new System.Drawing.Size(93, 15);
            this.label109.TabIndex = 29;
            this.label109.Text = "Вид состояния";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_state_type,
            this.date_start_state,
            this.date_end_state,
            this.description});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 113);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(697, 89);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewClaimStates_DataError);
            // 
            // id_state_type
            // 
            this.id_state_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_state_type.HeaderText = "Вид состояния";
            this.id_state_type.MinimumWidth = 150;
            this.id_state_type.Name = "id_state_type";
            this.id_state_type.ReadOnly = true;
            this.id_state_type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.id_state_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.id_state_type.Width = 150;
            // 
            // date_start_state
            // 
            this.date_start_state.HeaderText = "Дата установки";
            this.date_start_state.MinimumWidth = 150;
            this.date_start_state.Name = "date_start_state";
            this.date_start_state.Width = 150;
            // 
            // date_end_state
            // 
            this.date_end_state.HeaderText = "Крайний срок";
            this.date_end_state.MinimumWidth = 150;
            this.date_end_state.Name = "date_end_state";
            this.date_end_state.ReadOnly = true;
            this.date_end_state.Width = 150;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Примечание";
            this.description.MinimumWidth = 200;
            this.description.Name = "description";
            this.description.ReadOnly = true;
            // 
            // ClaimStatesViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(700, 190);
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(709, 211);
            this.Controls.Add(this.tableLayoutPanel17);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClaimStatesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Состояния иск. работы №{0}";
            this.tableLayoutPanel17.ResumeLayout(false);
            this.groupBox35.ResumeLayout(false);
            this.tableLayoutPanel18.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
