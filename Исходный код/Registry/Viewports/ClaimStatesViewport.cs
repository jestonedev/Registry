using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

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
        private DataGridViewComboBoxColumn id_state_type;
        private DataGridViewTextBoxColumn date_start_state;
        private DataGridViewTextBoxColumn date_end_state;
        private DataGridViewTextBoxColumn description;
        #endregion Components

        #region Models
        DataModel claim_states;
        DataModel claim_state_types;
        DataModel claim_state_types_relations;
        #endregion Models

        #region Views
        BindingSource v_claim_states;
        BindingSource v_claim_state_types;
        BindingSource v_claim_state_types_for_grid;
        #endregion Views

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable;

        private ClaimStatesViewport()
            : this(null)
        {
            
        }

        public ClaimStatesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public ClaimStatesViewport(Viewport claimStatesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = claimStatesViewport.DynamicFilter;
            StaticFilter = claimStatesViewport.StaticFilter;
            ParentRow = claimStatesViewport.ParentRow;
            ParentType = claimStatesViewport.ParentType;
        }

        private void RebuildFilter()
        {
            var filter = "";
            IEnumerable<int> includedStates = null;
            // Если текущая позиция - первый элемент, и количество элементов 1 то он может иметь только начальное состояние (любое)
            if ((v_claim_states.Position == 0) && (v_claim_states.Count == 1))
                includedStates = DataModelHelper.ClaimStartStateTypeIds();
            else
            // Если текущая позиция - первый элемент, и количество элементов 1 то он может иметь только начальное состояние 
            // (не противоречащее следующей позиции)
            if ((v_claim_states.Position == 0) && (v_claim_states.Count > 1))
            {
                var nextClaimStateType = Convert.ToInt32(
                    ((DataRowView)v_claim_states[v_claim_states.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
                includedStates = DataModelHelper.ClaimStateTypeIdsByNextStateType(nextClaimStateType);
            }
            else
            // Если текущая позиция - последний элемент, то выбрать состояние, в которое можно перейти из состояния предыдущего элемента
            if ((v_claim_states.Position != -1) && (v_claim_states.Position == (v_claim_states.Count - 1)))
            {
                var prevClaimStateType = Convert.ToInt32(
                    ((DataRowView)v_claim_states[v_claim_states.Position - 1])["id_state_type"], CultureInfo.InvariantCulture);
                includedStates = DataModelHelper.ClaimStateTypeIdsByPrevStateType(prevClaimStateType);
            }
            else
            // Мы находимся не в конце списка и не в начале и необходимо выбрать только те состояния, в которые можно перейти с учетом окружающих состояний
            if (v_claim_states.Position != -1)
            {
                var prevClaimStateType = 
                    Convert.ToInt32(((DataRowView)v_claim_states[v_claim_states.Position - 1])["id_state_type"], CultureInfo.InvariantCulture);
                var nextClaimStateType = 
                    Convert.ToInt32(((DataRowView)v_claim_states[v_claim_states.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
                includedStates = DataModelHelper.ClaimStateTypeIdsByNextAndPrevStateTypes(nextClaimStateType, prevClaimStateType); 
            }
            if (includedStates != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_state_type IN (0";
                foreach (var id in includedStates)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            v_claim_state_types.Filter = filter;
            //Делаем перепривязку ComboboxStateType
            if (v_claim_states.Position != -1)
            {
                var idStateType = ((DataRowView)v_claim_states[v_claim_states.Position])["id_state_type"];
                // Состояние существует, но его возможные тип определить не удалось из-за изменений в ветке зависимостей типов состояний
                if ((v_claim_state_types.Find("id_state_type", idStateType) == -1) && (viewportState != ViewportState.NewRowState))
                {
                    label109.ForeColor = Color.Red;
                    label109.Text = @"Вид состояния (ошибка)";
                    v_claim_state_types.Filter = "";
                }
                else
                {
                    label109.ForeColor = SystemColors.WindowText;
                    label109.Text = @"Вид состояния";
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
            var row = (v_claim_states.Position >= 0) ? (DataRowView)v_claim_states[v_claim_states.Position] : null;
            if (row != null && ((v_claim_states.Position >= 0) && (row["document_date"] != DBNull.Value)))
                dateTimePickerDocDate.Checked = true;
            else
            {
                dateTimePickerDocDate.Value = DateTime.Now.Date;
                dateTimePickerDocDate.Checked = false;
            }
            if (row != null && ((v_claim_states.Position >= 0) && (row["date_start_state"] != DBNull.Value)))
                dateTimePickerStartState.Checked = true;
            else
            {
                dateTimePickerStartState.Value = DateTime.Now.Date;
                dateTimePickerStartState.Checked = false;
            }

            if (row != null && ((v_claim_states.Position >= 0) && (row["date_end_state"] != DBNull.Value)))
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
            if ((!ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_claim_states.Position != -1) && (ClaimStateFromView() != ClaimStateFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    dataGridView.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    dataGridView.Enabled = true;
                }
            }
            if (Selected)
                MenuCallback.EditingStateUpdate();
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
                                    return false;
                            }
                            return viewportState == ViewportState.ReadState;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            if (claim_states.EditingNewRecord)
                                return false;
                            viewportState = ViewportState.NewRowState;
                            return true;
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
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
                                    return false;
                            }
                            return viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.NewRowState);
                    }
                    break;
                case ViewportState.ModifyRowState: 
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
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
                                    return false;
                            }
                            return viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.ModifyRowState);
                    }
                    break;
            }
            return false;
        }

        private void LocateClaimStateBy(int id)
        {
            var position = v_claim_states.Find("id_state", id);
            is_editable = false;
            if (position > 0)
                v_claim_states.Position = position;
            is_editable = true;
        }

        private static bool ValidateClaimState(ClaimState claimState)
        {
            if (claimState.IdStateType == null)
            {
                MessageBox.Show(@"Необходимо выбрать тип состояния претензионно-исковой работы", @"Ошибка",
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
            var claimState = new ClaimState
            {
                IdState =
                    v_claim_states.Position == -1
                        ? null
                        : ViewportHelper.ValueOrNull<int>((DataRowView) v_claim_states[v_claim_states.Position],
                            "id_state"),
                IdStateType = ViewportHelper.ValueOrNull<int>(comboBoxClaimStateType),
                IdClaim = ViewportHelper.ValueOrNull<int>(ParentRow, "id_claim"),
                DocumentNum = ViewportHelper.ValueOrNull(textBoxDocumentNumber),
                Description = ViewportHelper.ValueOrNull(textBoxDescription),
                DateStartState = ViewportHelper.ValueOrNull(dateTimePickerStartState),
                DateEndState = ViewportHelper.ValueOrNull(dateTimePickerEndState),
                DocumentDate = ViewportHelper.ValueOrNull(dateTimePickerDocDate)
            };
            return claimState;
        }

        private ClaimState ClaimStateFromView()
        {
            var row = (DataRowView)v_claim_states[v_claim_states.Position];
            var claimState = new ClaimState
            {
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type"),
                IdClaim = ViewportHelper.ValueOrNull<int>(row, "id_claim"),
                DocumentNum = ViewportHelper.ValueOrNull(row, "document_num"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                DateStartState = ViewportHelper.ValueOrNull<DateTime>(row, "date_start_state"),
                DateEndState = ViewportHelper.ValueOrNull<DateTime>(row, "date_end_state"),
                DocumentDate = ViewportHelper.ValueOrNull<DateTime>(row, "document_date")
            };
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
            DockAreas = DockAreas.Document;
            dataGridView.AutoGenerateColumns = false;
            claim_states = DataModel.GetInstance(DataModelType.ClaimStatesDataModel);
            claim_state_types = DataModel.GetInstance(DataModelType.ClaimStateTypesDataModel);
            claim_state_types_relations = DataModel.GetInstance(DataModelType.ClaimStateTypesRelationsDataModel);

            //Ожидаем дозагрузки, если это необходимо
            claim_states.Select();
            claim_state_types.Select();
            claim_state_types_relations.Select();

            var ds = DataModel.DataSet;

            if (ParentType == ParentTypeEnum.Claim && ParentRow != null)
                Text = string.Format(CultureInfo.InvariantCulture, "Состояния иск. работы №{0}", ParentRow["id_claim"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            v_claim_state_types = new BindingSource
            {
                DataMember = "claim_state_types",
                DataSource = ds
            };

            v_claim_state_types_for_grid = new BindingSource
            {
                DataMember = "claim_state_types",
                DataSource = ds
            };

            new BindingSource
            {
                DataMember = "claim_state_types_relations",
                DataSource = ds
            };

            v_claim_states = new BindingSource();
            v_claim_states.CurrentItemChanged += v_claim_states_CurrentItemChanged;
            v_claim_states.DataMember = "claim_states";
            v_claim_states.DataSource = ds;
            v_claim_states.Filter = StaticFilter;

            DataBind();

            claim_states.Select().RowChanged += ClaimStatesViewport_RowChanged;
            claim_states.Select().RowDeleted += ClaimStatesViewport_RowDeleted;
            claim_state_types.Select().RowChanged += ClaimStateTypesViewport_RowChanged;
            claim_state_types.Select().RowDeleted += ClaimStateTypesViewport_RowDeleted;
            claim_state_types_relations.Select().RowChanged += ClaimStateTypesRelationsViewport_RowChanged;
            claim_state_types_relations.Select().RowDeleted += ClaimStateTypesRelationsViewport_RowDeleted;
            is_editable = true;
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void SaveRecord()
        {
            var claimState = ClaimStateFromViewport();
            if (!ValidateClaimState(claimState))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idState = claim_states.Insert(claimState);
                    if (idState == -1)
                    {
                        claim_states.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    claimState.IdState = idState;
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
                        MessageBox.Show(@"Вы пытаетесь изменить запись о состоянии претензионно-исковой работы без внутреннего номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (claim_states.Update(claimState) == -1)
                        return;
                    var row = ((DataRowView)v_claim_states[v_claim_states.Position]);
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
            var claimState = ClaimStateFromView();
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
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            var stateCount = -1;
            // Мы находимся в начале списка и текущий элемент не последний
            if ((v_claim_states.Position == 0) && (v_claim_states.Count > 1))
            {
                var nextClaimStateType = 
                    Convert.ToInt32(((DataRowView)v_claim_states[v_claim_states.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
                stateCount = (from claimStateTypesRow in claim_state_types.FilterDeletedRows()
                    where Convert.ToBoolean(claimStateTypesRow.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture) &&
                          (claimStateTypesRow.Field<int>("id_state_type") == nextClaimStateType)
                    select claimStateTypesRow.Field<int>("id_state_type")).Count();
            }
            else
            // Мы находимся не в конце списка и не в начале
                if ((v_claim_states.Position != -1) && (v_claim_states.Position != (v_claim_states.Count - 1)))
                {
                    var previosClaimStateType = 
                        Convert.ToInt32(((DataRowView)v_claim_states[v_claim_states.Position - 1])["id_state_type"], CultureInfo.InvariantCulture);
                    var nextClaimStateType = 
                        Convert.ToInt32(((DataRowView)v_claim_states[v_claim_states.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
                    stateCount = (from claimStateTypesRelRow in claim_state_types_relations.FilterDeletedRows()
                        where claimStateTypesRelRow.Field<int>("id_state_from") == previosClaimStateType &&
                              claimStateTypesRelRow.Field<int>("id_state_to") == nextClaimStateType
                        select claimStateTypesRelRow.Field<int>("id_state_to")).Count();
                }
            if (stateCount == 0)
            {
                MessageBox.Show(@"Вы не можете удалить это состояние, так как это нарушит цепочку зависимости состояний претензионно-исковой работы."+
                                @"Чтобы удалить данное состояние, необходимо сначала удалить все состояния после него", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            if (claim_states.Delete((int)((DataRowView)v_claim_states.Current)["id_state"]) == -1)
                return;
            is_editable = false;
            ((DataRowView)v_claim_states[v_claim_states.Position]).Delete();
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            is_editable = true;
            MenuCallback.ForceCloseDetachedViewports();
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
            var viewport = new ClaimStatesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_claim_states.Count > 0)
                viewport.LocateClaimStateBy((((DataRowView)v_claim_states[v_claim_states.Position])["id_claim"] as int?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                claim_states.Select().RowChanged -= ClaimStatesViewport_RowChanged;
                claim_states.Select().RowDeleted -= ClaimStatesViewport_RowDeleted;
                claim_state_types.Select().RowChanged -= ClaimStateTypesViewport_RowChanged;
                claim_state_types.Select().RowDeleted -= ClaimStateTypesViewport_RowDeleted;
                claim_state_types_relations.Select().RowChanged -= ClaimStateTypesRelationsViewport_RowChanged;
                claim_state_types_relations.Select().RowDeleted -= ClaimStateTypesRelationsViewport_RowDeleted;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                claim_states.EditingNewRecord = false;
            claim_states.Select().RowChanged -= ClaimStatesViewport_RowChanged;
            claim_states.Select().RowDeleted -= ClaimStatesViewport_RowDeleted;
            claim_state_types.Select().RowChanged -= ClaimStateTypesViewport_RowChanged;
            claim_state_types.Select().RowDeleted -= ClaimStateTypesViewport_RowDeleted;
            claim_state_types_relations.Select().RowChanged -= ClaimStateTypesRelationsViewport_RowChanged;
            claim_state_types_relations.Select().RowDeleted -= ClaimStateTypesRelationsViewport_RowDeleted;
            Close();
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
            CheckViewportModifications();
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
            dateTimePickerStartState.Focus();
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
            var resources = new ComponentResourceManager(typeof(ClaimStatesViewport));
            tableLayoutPanel17 = new TableLayoutPanel();
            groupBox35 = new GroupBox();
            tableLayoutPanel18 = new TableLayoutPanel();
            panel10 = new Panel();
            textBoxDocumentNumber = new TextBox();
            label100 = new Label();
            textBoxDescription = new TextBox();
            label101 = new Label();
            dateTimePickerDocDate = new DateTimePicker();
            label105 = new Label();
            panel11 = new Panel();
            dateTimePickerEndState = new DateTimePicker();
            label110 = new Label();
            comboBoxClaimStateType = new ComboBox();
            dateTimePickerStartState = new DateTimePicker();
            label108 = new Label();
            label109 = new Label();
            dataGridView = new DataGridView();
            id_state_type = new DataGridViewComboBoxColumn();
            date_start_state = new DataGridViewTextBoxColumn();
            date_end_state = new DataGridViewTextBoxColumn();
            description = new DataGridViewTextBoxColumn();
            tableLayoutPanel17.SuspendLayout();
            groupBox35.SuspendLayout();
            tableLayoutPanel18.SuspendLayout();
            panel10.SuspendLayout();
            panel11.SuspendLayout();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel17
            // 
            tableLayoutPanel17.ColumnCount = 1;
            tableLayoutPanel17.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel17.Controls.Add(groupBox35, 0, 0);
            tableLayoutPanel17.Controls.Add(dataGridView, 0, 1);
            tableLayoutPanel17.Dock = DockStyle.Fill;
            tableLayoutPanel17.Location = new Point(3, 3);
            tableLayoutPanel17.Name = "tableLayoutPanel17";
            tableLayoutPanel17.RowCount = 2;
            tableLayoutPanel17.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            tableLayoutPanel17.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel17.Size = new Size(703, 205);
            tableLayoutPanel17.TabIndex = 1;
            // 
            // groupBox35
            // 
            groupBox35.Controls.Add(tableLayoutPanel18);
            groupBox35.Dock = DockStyle.Fill;
            groupBox35.Location = new Point(0, 0);
            groupBox35.Margin = new Padding(0);
            groupBox35.Name = "groupBox35";
            groupBox35.Size = new Size(703, 110);
            groupBox35.TabIndex = 1;
            groupBox35.TabStop = false;
            groupBox35.Text = @"Общие сведения";
            // 
            // tableLayoutPanel18
            // 
            tableLayoutPanel18.ColumnCount = 2;
            tableLayoutPanel18.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel18.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel18.Controls.Add(panel10, 1, 0);
            tableLayoutPanel18.Controls.Add(panel11, 0, 0);
            tableLayoutPanel18.Dock = DockStyle.Fill;
            tableLayoutPanel18.Location = new Point(3, 17);
            tableLayoutPanel18.Name = "tableLayoutPanel18";
            tableLayoutPanel18.RowCount = 1;
            tableLayoutPanel18.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel18.RowStyles.Add(new RowStyle(SizeType.Absolute, 148F));
            tableLayoutPanel18.Size = new Size(697, 90);
            tableLayoutPanel18.TabIndex = 0;
            // 
            // panel10
            // 
            panel10.Controls.Add(textBoxDocumentNumber);
            panel10.Controls.Add(label100);
            panel10.Controls.Add(textBoxDescription);
            panel10.Controls.Add(label101);
            panel10.Controls.Add(dateTimePickerDocDate);
            panel10.Controls.Add(label105);
            panel10.Dock = DockStyle.Fill;
            panel10.Location = new Point(348, 0);
            panel10.Margin = new Padding(0);
            panel10.Name = "panel10";
            panel10.Size = new Size(349, 90);
            panel10.TabIndex = 1;
            // 
            // textBoxDocumentNumber
            // 
            textBoxDocumentNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            textBoxDocumentNumber.Location = new Point(161, 4);
            textBoxDocumentNumber.MaxLength = 50;
            textBoxDocumentNumber.Name = "textBoxDocumentNumber";
            textBoxDocumentNumber.Size = new Size(179, 21);
            textBoxDocumentNumber.TabIndex = 0;
            textBoxDocumentNumber.TextChanged += textBoxDocNumber_TextChanged;
            textBoxDocumentNumber.Enter += selectAll_Enter;
            // 
            // label100
            // 
            label100.AutoSize = true;
            label100.Location = new Point(12, 7);
            label100.Name = "label100";
            label100.Size = new Size(111, 15);
            label100.TabIndex = 51;
            label100.Text = @"Номер документа";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                        | AnchorStyles.Right;
            textBoxDescription.Location = new Point(161, 62);
            textBoxDescription.MaxLength = 4000;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(179, 21);
            textBoxDescription.TabIndex = 2;
            textBoxDescription.TextChanged += textBoxClaimStateDescription_TextChanged;
            textBoxDescription.Enter += selectAll_Enter;
            // 
            // label101
            // 
            label101.AutoSize = true;
            label101.Location = new Point(12, 65);
            label101.Name = "label101";
            label101.Size = new Size(80, 15);
            label101.TabIndex = 49;
            label101.Text = @"Примечание";
            // 
            // dateTimePickerDocDate
            // 
            dateTimePickerDocDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            dateTimePickerDocDate.Location = new Point(161, 33);
            dateTimePickerDocDate.Name = "dateTimePickerDocDate";
            dateTimePickerDocDate.ShowCheckBox = true;
            dateTimePickerDocDate.Size = new Size(179, 21);
            dateTimePickerDocDate.TabIndex = 1;
            dateTimePickerDocDate.ValueChanged += dateTimePickerDocDate_ValueChanged;
            // 
            // label105
            // 
            label105.AutoSize = true;
            label105.Location = new Point(12, 36);
            label105.Name = "label105";
            label105.Size = new Size(102, 15);
            label105.TabIndex = 44;
            label105.Text = @"Дата документа";
            // 
            // panel11
            // 
            panel11.Controls.Add(dateTimePickerEndState);
            panel11.Controls.Add(label110);
            panel11.Controls.Add(comboBoxClaimStateType);
            panel11.Controls.Add(dateTimePickerStartState);
            panel11.Controls.Add(label108);
            panel11.Controls.Add(label109);
            panel11.Dock = DockStyle.Fill;
            panel11.Location = new Point(0, 0);
            panel11.Margin = new Padding(0);
            panel11.Name = "panel11";
            panel11.Size = new Size(348, 90);
            panel11.TabIndex = 0;
            // 
            // dateTimePickerEndState
            // 
            dateTimePickerEndState.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            dateTimePickerEndState.Location = new Point(161, 62);
            dateTimePickerEndState.Name = "dateTimePickerEndState";
            dateTimePickerEndState.ShowCheckBox = true;
            dateTimePickerEndState.Size = new Size(178, 21);
            dateTimePickerEndState.TabIndex = 2;
            dateTimePickerEndState.ValueChanged += dateTimePickerEndState_ValueChanged;
            // 
            // label110
            // 
            label110.AutoSize = true;
            label110.Location = new Point(14, 65);
            label110.Name = "label110";
            label110.Size = new Size(86, 15);
            label110.TabIndex = 38;
            label110.Text = @"Крайний срок";
            // 
            // comboBoxClaimStateType
            // 
            comboBoxClaimStateType.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            comboBoxClaimStateType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxClaimStateType.FormattingEnabled = true;
            comboBoxClaimStateType.Location = new Point(161, 4);
            comboBoxClaimStateType.Name = "comboBoxClaimStateType";
            comboBoxClaimStateType.Size = new Size(178, 23);
            comboBoxClaimStateType.TabIndex = 0;
            comboBoxClaimStateType.SelectedIndexChanged += comboBoxClaimStateType_SelectedIndexChanged;
            // 
            // dateTimePickerStartState
            // 
            dateTimePickerStartState.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                              | AnchorStyles.Right;
            dateTimePickerStartState.Location = new Point(161, 33);
            dateTimePickerStartState.Name = "dateTimePickerStartState";
            dateTimePickerStartState.ShowCheckBox = true;
            dateTimePickerStartState.Size = new Size(178, 21);
            dateTimePickerStartState.TabIndex = 1;
            dateTimePickerStartState.ValueChanged += dateTimePickerStartState_ValueChanged;
            // 
            // label108
            // 
            label108.AutoSize = true;
            label108.Location = new Point(14, 36);
            label108.Name = "label108";
            label108.Size = new Size(99, 15);
            label108.TabIndex = 31;
            label108.Text = @"Дата установки";
            // 
            // label109
            // 
            label109.AutoSize = true;
            label109.Location = new Point(14, 7);
            label109.Name = "label109";
            label109.Size = new Size(93, 15);
            label109.TabIndex = 29;
            label109.Text = @"Вид состояния";
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_state_type, date_start_state, date_end_state, description);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 113);
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(697, 89);
            dataGridView.TabIndex = 0;
            dataGridView.DataError += dataGridViewClaimStates_DataError;
            // 
            // id_state_type
            // 
            id_state_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_state_type.HeaderText = @"Вид состояния";
            id_state_type.MinimumWidth = 150;
            id_state_type.Name = "id_state_type";
            id_state_type.ReadOnly = true;
            id_state_type.Resizable = DataGridViewTriState.True;
            id_state_type.SortMode = DataGridViewColumnSortMode.Automatic;
            id_state_type.Width = 150;
            // 
            // date_start_state
            // 
            date_start_state.HeaderText = @"Дата установки";
            date_start_state.MinimumWidth = 150;
            date_start_state.Name = "date_start_state";
            date_start_state.Width = 150;
            // 
            // date_end_state
            // 
            date_end_state.HeaderText = @"Крайний срок";
            date_end_state.MinimumWidth = 150;
            date_end_state.Name = "date_end_state";
            date_end_state.ReadOnly = true;
            date_end_state.Width = 150;
            // 
            // description
            // 
            description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            description.HeaderText = @"Примечание";
            description.MinimumWidth = 200;
            description.Name = "description";
            description.ReadOnly = true;
            // 
            // ClaimStatesViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(700, 190);
            AutoSize = true;
            BackColor = Color.White;
            ClientSize = new Size(709, 211);
            Controls.Add(tableLayoutPanel17);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "ClaimStatesViewport";
            Padding = new Padding(3);
            Text = @"Состояния иск. работы №{0}";
            tableLayoutPanel17.ResumeLayout(false);
            groupBox35.ResumeLayout(false);
            tableLayoutPanel18.ResumeLayout(false);
            panel10.ResumeLayout(false);
            panel10.PerformLayout();
            panel11.ResumeLayout(false);
            panel11.PerformLayout();
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
