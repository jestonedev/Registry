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
    internal sealed partial class ClaimStatesViewport: FormViewport
    {
        #region Models
        DataModel claim_state_types;
        DataModel claim_state_types_relations;
        #endregion Models

        #region Views
        BindingSource v_claim_state_types;
        BindingSource v_claim_state_types_for_grid;
        #endregion Views

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
            if ((GeneralBindingSource.Position == 0) && (GeneralBindingSource.Count == 1))
                includedStates = DataModelHelper.ClaimStartStateTypeIds();
            else
            // Если текущая позиция - первый элемент, и количество элементов 1 то он может иметь только начальное состояние 
            // (не противоречащее следующей позиции)
            if ((GeneralBindingSource.Position == 0) && (GeneralBindingSource.Count > 1))
            {
                var nextClaimStateType = Convert.ToInt32(
                    ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
                includedStates = DataModelHelper.ClaimStateTypeIdsByNextStateType(nextClaimStateType);
            }
            else
            // Если текущая позиция - последний элемент, то выбрать состояние, в которое можно перейти из состояния предыдущего элемента
            if ((GeneralBindingSource.Position != -1) && (GeneralBindingSource.Position == (GeneralBindingSource.Count - 1)))
            {
                var prevClaimStateType = Convert.ToInt32(
                    ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position - 1])["id_state_type"], CultureInfo.InvariantCulture);
                includedStates = DataModelHelper.ClaimStateTypeIdsByPrevStateType(prevClaimStateType);
            }
            else
            // Мы находимся не в конце списка и не в начале и необходимо выбрать только те состояния, в которые можно перейти с учетом окружающих состояний
            if (GeneralBindingSource.Position != -1)
            {
                var prevClaimStateType = 
                    Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position - 1])["id_state_type"], CultureInfo.InvariantCulture);
                var nextClaimStateType = 
                    Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
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
            if (GeneralBindingSource.Position != -1)
            {
                var idStateType = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state_type"];
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
                comboBoxClaimStateType.SelectedValue = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_state_type"];
            }
        }

        private void DataBind()
        {
            comboBoxClaimStateType.DataSource = v_claim_state_types;
            comboBoxClaimStateType.ValueMember = "id_state_type";
            comboBoxClaimStateType.DisplayMember = "state_type";
            comboBoxClaimStateType.DataBindings.Clear();
            comboBoxClaimStateType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_state_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxDocumentNumber.DataBindings.Clear();
            textBoxDocumentNumber.DataBindings.Add("Text", GeneralBindingSource, "document_num", true, DataSourceUpdateMode.Never, "");
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerDocDate.DataBindings.Clear();
            dateTimePickerDocDate.DataBindings.Add("Value", GeneralBindingSource, "document_date", true, DataSourceUpdateMode.Never, null);
            dateTimePickerStartState.DataBindings.Clear();
            dateTimePickerStartState.DataBindings.Add("Value", GeneralBindingSource, "date_start_state", true, DataSourceUpdateMode.Never, null);
            dateTimePickerEndState.DataBindings.Clear();
            dateTimePickerEndState.DataBindings.Add("Value", GeneralBindingSource, "date_end_state", true, DataSourceUpdateMode.Never, null);

            dataGridView.DataSource = GeneralBindingSource;
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
            var row = (GeneralBindingSource.Position >= 0) ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["document_date"] != DBNull.Value)))
                dateTimePickerDocDate.Checked = true;
            else
            {
                dateTimePickerDocDate.Value = DateTime.Now.Date;
                dateTimePickerDocDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["date_start_state"] != DBNull.Value)))
                dateTimePickerStartState.Checked = true;
            else
            {
                dateTimePickerStartState.Value = DateTime.Now.Date;
                dateTimePickerStartState.Checked = false;
            }

            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["date_end_state"] != DBNull.Value)))
                dateTimePickerEndState.Checked = true;
            else
            {
                dateTimePickerEndState.Value = DateTime.Now.Date;
                dateTimePickerEndState.Checked = false;
            }
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
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
                            if (GeneralDataModel.EditingNewRecord)
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
            var position = GeneralBindingSource.Find("id_state", id);
            is_editable = false;
            if (position > 0)
                GeneralBindingSource.Position = position;
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

        protected override Entity EntityFromViewport()
        {
            var claimState = new ClaimState
            {
                IdState =
                    GeneralBindingSource.Position == -1 ? null : ViewportHelper.ValueOrNull<int>(
                    (DataRowView) GeneralBindingSource[GeneralBindingSource.Position], "id_state"),
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

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            DockAreas = DockAreas.Document;
            dataGridView.AutoGenerateColumns = false;
            GeneralDataModel = DataModel.GetInstance(DataModelType.ClaimStatesDataModel);
            claim_state_types = DataModel.GetInstance(DataModelType.ClaimStateTypesDataModel);
            claim_state_types_relations = DataModel.GetInstance(DataModelType.ClaimStateTypesRelationsDataModel);

            //Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
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

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += v_claim_states_CurrentItemChanged;
            GeneralBindingSource.DataMember = "claim_states";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Filter = StaticFilter;

            DataBind();

            GeneralDataModel.Select().RowChanged += ClaimStatesViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += ClaimStatesViewport_RowDeleted;
            claim_state_types.Select().RowChanged += ClaimStateTypesViewport_RowChanged;
            claim_state_types.Select().RowDeleted += ClaimStateTypesViewport_RowDeleted;
            claim_state_types_relations.Select().RowChanged += ClaimStateTypesRelationsViewport_RowChanged;
            claim_state_types_relations.Select().RowDeleted += ClaimStateTypesRelationsViewport_RowDeleted;
            is_editable = true;
            DataChangeHandlersInit();
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void SaveRecord()
        {
            var claimState = (ClaimState) EntityFromViewport();
            if (!ValidateClaimState(claimState))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idState = GeneralDataModel.Insert(claimState);
                    if (idState == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    claimState.IdState = idState;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    FillRowFromClaimState(claimState, newRow);
                    GeneralBindingSource.Position = GeneralBindingSource.Count - 1;
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (claimState.IdState == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о состоянии претензионно-исковой работы без внутреннего номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(claimState) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
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
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            var claimState = (ClaimState) EntityFromView();
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromClaimState(claimState);
            dateTimePickerDocDate.Checked = (claimState.DocumentDate != null);
            dateTimePickerStartState.Checked = (claimState.DateStartState != null);
            dateTimePickerEndState.Checked = (claimState.DateEndState != null);
            is_editable = true;
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            GeneralBindingSource.AddNew();
            is_editable = true;
            dataGridView.Enabled = false;
            GeneralDataModel.EditingNewRecord = true; 
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1) 
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
            if ((GeneralBindingSource.Position == 0) && (GeneralBindingSource.Count > 1))
            {
                var nextClaimStateType = 
                    Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
                stateCount = (from claimStateTypesRow in claim_state_types.FilterDeletedRows()
                    where Convert.ToBoolean(claimStateTypesRow.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture) &&
                          (claimStateTypesRow.Field<int>("id_state_type") == nextClaimStateType)
                    select claimStateTypesRow.Field<int>("id_state_type")).Count();
            }
            else
            // Мы находимся не в конце списка и не в начале
                if ((GeneralBindingSource.Position != -1) && (GeneralBindingSource.Position != (GeneralBindingSource.Count - 1)))
                {
                    var previosClaimStateType = 
                        Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position - 1])["id_state_type"], CultureInfo.InvariantCulture);
                    var nextClaimStateType = 
                        Convert.ToInt32(((DataRowView)GeneralBindingSource[GeneralBindingSource.Position + 1])["id_state_type"], CultureInfo.InvariantCulture);
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
            if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_state"]) == -1)
                return;
            is_editable = false;
            ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
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
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        if (GeneralBindingSource.Position != -1)
                            dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
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
            if (GeneralBindingSource.Count > 0)
                viewport.LocateClaimStateBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_claim"] as int?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                GeneralDataModel.Select().RowChanged -= ClaimStatesViewport_RowChanged;
                GeneralDataModel.Select().RowDeleted -= ClaimStatesViewport_RowDeleted;
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
                GeneralDataModel.EditingNewRecord = false;
            GeneralDataModel.Select().RowChanged -= ClaimStatesViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= ClaimStatesViewport_RowDeleted;
            claim_state_types.Select().RowChanged -= ClaimStateTypesViewport_RowChanged;
            claim_state_types.Select().RowDeleted -= ClaimStateTypesViewport_RowDeleted;
            claim_state_types_relations.Select().RowChanged -= ClaimStateTypesRelationsViewport_RowChanged;
            claim_state_types_relations.Select().RowDeleted -= ClaimStateTypesRelationsViewport_RowDeleted;
            Close();
        }

        void v_claim_states_CurrentItemChanged(object sender, EventArgs e)
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
            UnbindedCheckBoxesUpdate();
            RebuildFilter();
            if (GeneralBindingSource.Position == -1)
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
    }
}
