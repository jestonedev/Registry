using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.Presenters;
using Security;
using Settings;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class ClaimStatesViewport : FormWithGridViewport
    {
        private bool _noUpdateFieldList;

        private ClaimStatesViewport()
            : this(null, null)
        {
        }

        public ClaimStatesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new ClaimStatesPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        private void RebuildFilter()
        {
            IsEditable = false;
            ((ClaimStatesPresenter)Presenter).RebuildClaimStateTypeFilter();
            //Делаем перепривязку ComboboxStateType
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
            {
                IsEditable = true; 
                return;
            }
            var idStateType = row["id_state_type"];
            // Состояние существует, но его возможные тип определить не удалось из-за изменений в ветке зависимостей типов состояний
            if ((Presenter.ViewModel["claim_state_types"].BindingSource.Find("id_state_type", idStateType) == -1) && 
                (ViewportState != ViewportState.NewRowState))
            {
                label109.ForeColor = Color.Red;
                label109.Text = @"Вид состояния (ошибка)";
                Presenter.ViewModel["claim_state_types"].BindingSource.Filter = "";
            }
            else
            {
                label109.ForeColor = SystemColors.WindowText;
                label109.Text = @"Вид состояния";
            }
            comboBoxClaimStateType.SelectedValue = row["id_state_type"];
            IsEditable = true;
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindSource(comboBoxClaimStateType, Presenter.ViewModel["claim_state_types"].BindingSource, "state_type",
                 Presenter.ViewModel["claim_state_types"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxClaimStateType, "SelectedValue", bindingSource,
                Presenter.ViewModel["claim_state_types"].PrimaryKeyFirst, DBNull.Value);
            ViewportHelper.BindProperty(textBoxExecutor, "Text", bindingSource, "executor", "");
            ViewportHelper.BindProperty(textBoxDescription, "Text", bindingSource, "description", "");
            ViewportHelper.BindProperty(dateTimePickerStartState, "Value", bindingSource, "date_start_state", DateTime.Now.Date);

            ViewportHelper.BindProperty(textBoxBksRequester, "Text", bindingSource, "bks_requester", "");

            ViewportHelper.BindProperty(dateTimePickerTransfertToLegalDepartmentDate, "Value", bindingSource, "transfert_to_legal_department_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxTransferToLegalDepartmentWho, "Text", bindingSource, "transfer_to_legal_department_who", "");
            
            ViewportHelper.BindProperty(dateTimePickerAcceptedByLegalDepartmentDate, "Value", bindingSource, "accepted_by_legal_department_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxAcceptedByLegalDepartmentWho, "Text", bindingSource, "accepted_by_legal_department_who", "");
            
            ViewportHelper.BindProperty(dateTimePickerClaimDirectionDate, "Value", bindingSource, "claim_direction_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxClaimDirectionDescription, "Text", bindingSource, "claim_direction_description", "");
            ViewportHelper.BindProperty(dateTimePickerCourtOrderDate, "Value", bindingSource, "court_order_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxCourtOrderNum, "Text", bindingSource, "court_order_num", "");
            ViewportHelper.BindProperty(dateTimePickerObtainingCourtOrderDate, "Value", bindingSource, "obtaining_court_order_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxObtainingCourtOrderDescription, "Text", bindingSource, "obtaining_court_order_description", "");
            
            ViewportHelper.BindProperty(dateTimePickerDirectionCourtOrderBailiffsDate, "Value", bindingSource, "direction_court_order_bailiffs_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxDirectionCourtOrderBailiffsDescription, "Text", bindingSource, "direction_court_order_bailiffs_description", "");
            ViewportHelper.BindProperty(dateTimePickerEnforcementProceedingStartDate, "Value", bindingSource, "enforcement_proceeding_start_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxEnforcementProceedingStartDescription, "Text", bindingSource, "enforcement_proceeding_start_description", "");
            ViewportHelper.BindProperty(dateTimePickerEnforcementProceedingEndDate, "Value", bindingSource, "enforcement_proceeding_end_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxEnforcementProceedingEndDescription, "Text", bindingSource, "enforcement_proceeding_end_description", "");
            ViewportHelper.BindProperty(dateTimePickerEnforcementProceedingTerminateDate, "Value", bindingSource, "enforcement_proceeding_terminate_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxEnforcementProceedingTerminateDescription, "Text", bindingSource, "enforcement_proceeding_terminate_description", "");
            ViewportHelper.BindProperty(dateTimePickerRepeatedDirectionCourtOrderBailiffsDate, "Value", bindingSource, "repeated_direction_court_order_bailiffs_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxRepeatedDirectionCourtOrderBailiffsDescription, "Text", bindingSource, "repeated_direction_court_order_bailiffs_description", "");
            ViewportHelper.BindProperty(dateTimePickerRepeatedEnforcementProceedingStartDate, "Value", bindingSource, "repeated_enforcement_proceeding_start_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxRepeatedEnforcementProceedingStartDescription, "Text", bindingSource, "repeated_enforcement_proceeding_start_description", "");
            ViewportHelper.BindProperty(dateTimePickerRepeatedEnforcementProceedingEndDate, "Value", bindingSource, "repeated_enforcement_proceeding_end_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxRepeatedEnforcementProceedingEndDescription, "Text", bindingSource, "repeated_enforcement_proceeding_end_description", "");
            
            ViewportHelper.BindProperty(dateTimePickerCourtOrderCancelDate, "Value", bindingSource, "court_order_cancel_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxCourtOrderCancelDescription, "Text", bindingSource, "court_order_cancel_description", "");
            ViewportHelper.BindProperty(dateTimePickerClaimCompleteDate, "Value", bindingSource, "claim_complete_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxClaimCompleteDescription, "Text", bindingSource, "claim_complete_description", "");
            ViewportHelper.BindProperty(textBoxClaimCompleteReason, "Text", bindingSource, "claim_complete_reason", "");

            DataGridView.DataSource = bindingSource;
            date_start_state.DataPropertyName = "date_start_state";
            description.DataPropertyName = "description";

            ViewportHelper.BindSource(id_state_type, Presenter.ViewModel["claim_state_types_for_grid"].BindingSource, "state_type",
                Presenter.ViewModel["claim_state_types_for_grid"].PrimaryKeyFirst);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            if (Presenter.ViewModel["general"].BindingSource.Count == 0) return;
            var row = Presenter.ViewModel["general"].CurrentRow;
            IsEditable = false;
            if (row != null && (row["transfert_to_legal_department_date"] != DBNull.Value))
                dateTimePickerTransfertToLegalDepartmentDate.Checked = true;
            else
            {
                dateTimePickerTransfertToLegalDepartmentDate.Value = DateTime.Now.Date;
                dateTimePickerTransfertToLegalDepartmentDate.Checked = false;
            }
            if (row != null && row["accepted_by_legal_department_date"] != DBNull.Value)
                dateTimePickerAcceptedByLegalDepartmentDate.Checked = true;
            else
            {
                dateTimePickerAcceptedByLegalDepartmentDate.Value = DateTime.Now.Date;
                dateTimePickerAcceptedByLegalDepartmentDate.Checked = false;
            }
            if (row != null && (row["claim_direction_date"] != DBNull.Value))
                dateTimePickerClaimDirectionDate.Checked = true;
            else
            {
                dateTimePickerClaimDirectionDate.Value = DateTime.Now.Date;
                dateTimePickerClaimDirectionDate.Checked = false;
            }
            if (row != null && (row["court_order_date"] != DBNull.Value))
                dateTimePickerCourtOrderDate.Checked = true;
            else
            {
                dateTimePickerCourtOrderDate.Value = DateTime.Now.Date;
                dateTimePickerCourtOrderDate.Checked = false;
            }
            if (row != null && (row["obtaining_court_order_date"] != DBNull.Value))
                dateTimePickerObtainingCourtOrderDate.Checked = true;
            else
            {
                dateTimePickerObtainingCourtOrderDate.Value = DateTime.Now.Date;
                dateTimePickerObtainingCourtOrderDate.Checked = false;
            }
            if (row != null && (row["direction_court_order_bailiffs_date"] != DBNull.Value))
                dateTimePickerDirectionCourtOrderBailiffsDate.Checked = true;
            else
            {
                dateTimePickerDirectionCourtOrderBailiffsDate.Value = DateTime.Now.Date;
                dateTimePickerDirectionCourtOrderBailiffsDate.Checked = false;
            }
            if (row != null && (row["enforcement_proceeding_start_date"] != DBNull.Value))
                dateTimePickerEnforcementProceedingStartDate.Checked = true;
            else
            {
                dateTimePickerEnforcementProceedingStartDate.Value = DateTime.Now.Date;
                dateTimePickerEnforcementProceedingStartDate.Checked = false;
            }
            if (row != null && (row["enforcement_proceeding_end_date"] != DBNull.Value))
                dateTimePickerEnforcementProceedingEndDate.Checked = true;
            else
            {
                dateTimePickerEnforcementProceedingEndDate.Value = DateTime.Now.Date;
                dateTimePickerEnforcementProceedingEndDate.Checked = false;
            }
            if (row != null && (row["enforcement_proceeding_terminate_date"] != DBNull.Value))
                dateTimePickerEnforcementProceedingTerminateDate.Checked = true;
            else
            {
                dateTimePickerEnforcementProceedingTerminateDate.Value = DateTime.Now.Date;
                dateTimePickerEnforcementProceedingTerminateDate.Checked = false;
            }
            if (row != null && (row["repeated_direction_court_order_bailiffs_date"] != DBNull.Value))
                dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Checked = true;
            else
            {
                dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Value = DateTime.Now.Date;
                dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Checked = false;
            }
            if (row != null && (row["repeated_enforcement_proceeding_start_date"] != DBNull.Value))
                dateTimePickerRepeatedEnforcementProceedingStartDate.Checked = true;
            else
            {
                dateTimePickerRepeatedEnforcementProceedingStartDate.Value = DateTime.Now.Date;
                dateTimePickerRepeatedEnforcementProceedingStartDate.Checked = false;
            }
            if (row != null && (row["repeated_enforcement_proceeding_end_date"] != DBNull.Value))
                dateTimePickerRepeatedEnforcementProceedingEndDate.Checked = true;
            else
            {
                dateTimePickerRepeatedEnforcementProceedingEndDate.Value = DateTime.Now.Date;
                dateTimePickerRepeatedEnforcementProceedingEndDate.Checked = false;
            }
            if (row != null && (row["court_order_cancel_date"] != DBNull.Value))
                dateTimePickerCourtOrderCancelDate.Checked = true;
            else
            {
                dateTimePickerCourtOrderCancelDate.Value = DateTime.Now.Date;
                dateTimePickerCourtOrderCancelDate.Checked = false;
            }
            if (row != null && (row["claim_complete_date"] != DBNull.Value))
                dateTimePickerClaimCompleteDate.Checked = true;
            else
            {
                dateTimePickerClaimCompleteDate.Value = DateTime.Now.Date;
                dateTimePickerClaimCompleteDate.Checked = false;
            }
            IsEditable = true;
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.ClaimsWrite))
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidateClaimState(ClaimState claimState)
        {
            if (claimState.IdStateType == null)
            {
                MessageBox.Show(@"Необходимо выбрать тип состояния претензионно-исковой работы", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxClaimStateType.Focus();
                return false;
            }
            return true;
        }

        protected override Entity EntityFromViewport()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var claimState = new ClaimState
            {
                IdState = row == null ? null : ViewportHelper.ValueOrNull<int>(row, "id_state"),
                IdStateType = ViewportHelper.ValueOrNull<int>(comboBoxClaimStateType),
                IdClaim = ViewportHelper.ValueOrNull<int>(ParentRow, "id_claim"),
                Executor = ViewportHelper.ValueOrNull(textBoxExecutor),
                Description = ViewportHelper.ValueOrNull(textBoxDescription),
                BksRequester = ViewportHelper.ValueOrNull(textBoxBksRequester),
                DateStartState = ViewportHelper.ValueOrNull(dateTimePickerStartState),
                TransfertToLegalDepartmentDate = ViewportHelper.ValueOrNull(dateTimePickerTransfertToLegalDepartmentDate),
                TransferToLegalDepartmentWho = ViewportHelper.ValueOrNull(textBoxTransferToLegalDepartmentWho),
                AcceptedByLegalDepartmentDate = ViewportHelper.ValueOrNull(dateTimePickerAcceptedByLegalDepartmentDate),
                AcceptedByLegalDepartmentWho = ViewportHelper.ValueOrNull(textBoxAcceptedByLegalDepartmentWho),
                ClaimDirectionDate = ViewportHelper.ValueOrNull(dateTimePickerClaimDirectionDate),
                ClaimDirectionDescription = ViewportHelper.ValueOrNull(textBoxClaimDirectionDescription),
                CourtOrderDate = ViewportHelper.ValueOrNull(dateTimePickerCourtOrderDate),
                CourtOrderNum = ViewportHelper.ValueOrNull(textBoxCourtOrderNum),
                ObtainingCourtOrderDate = ViewportHelper.ValueOrNull(dateTimePickerObtainingCourtOrderDate),
                ObtainingCourtOrderDescription = ViewportHelper.ValueOrNull(textBoxObtainingCourtOrderDescription),
                DirectionCourtOrderBailiffsDate = ViewportHelper.ValueOrNull(dateTimePickerDirectionCourtOrderBailiffsDate),
                DirectionCourtOrderBailiffsDescription = ViewportHelper.ValueOrNull(textBoxDirectionCourtOrderBailiffsDescription),
                EnforcementProceedingStartDate = ViewportHelper.ValueOrNull(dateTimePickerEnforcementProceedingStartDate),
                EnforcementProceedingStartDescription = ViewportHelper.ValueOrNull(textBoxEnforcementProceedingStartDescription),
                EnforcementProceedingEndDate = ViewportHelper.ValueOrNull(dateTimePickerEnforcementProceedingEndDate),
                EnforcementProceedingEndDescription = ViewportHelper.ValueOrNull(textBoxEnforcementProceedingEndDescription),
                EnforcementProceedingTerminateDate = ViewportHelper.ValueOrNull(dateTimePickerEnforcementProceedingTerminateDate),
                EnforcementProceedingTerminateDescription = ViewportHelper.ValueOrNull(textBoxEnforcementProceedingTerminateDescription),
                RepeatedDirectionCourtOrderBailiffsDate = ViewportHelper.ValueOrNull(dateTimePickerRepeatedDirectionCourtOrderBailiffsDate),
                RepeatedDirectionCourtOrderBailiffsDescription = ViewportHelper.ValueOrNull(textBoxRepeatedDirectionCourtOrderBailiffsDescription),
                RepeatedEnforcementProceedingStartDate = ViewportHelper.ValueOrNull(dateTimePickerRepeatedEnforcementProceedingStartDate),
                RepeatedEnforcementProceedingStartDescription = ViewportHelper.ValueOrNull(textBoxRepeatedEnforcementProceedingStartDescription),
                RepeatedEnforcementProceedingEndDate = ViewportHelper.ValueOrNull(dateTimePickerRepeatedEnforcementProceedingEndDate),
                RepeatedEnforcementProceedingEndDescription = ViewportHelper.ValueOrNull(textBoxRepeatedEnforcementProceedingEndDescription),
                CourtOrderCancelDate = ViewportHelper.ValueOrNull(dateTimePickerCourtOrderCancelDate),
                CourtOrderCancelDescription = ViewportHelper.ValueOrNull(textBoxCourtOrderCancelDescription),
                ClaimCompleteDate = ViewportHelper.ValueOrNull(dateTimePickerClaimCompleteDate),
                ClaimCompleteDescription = ViewportHelper.ValueOrNull(textBoxClaimCompleteDescription),
                ClaimCompleteReason = ViewportHelper.ValueOrNull(textBoxClaimCompleteReason)
            };
            return claimState;
        }

        protected override Entity EntityFromView()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return row == null ? new ClaimState() : EntityConverter<ClaimState>.FromRow(row);
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;

            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            if (ParentType == ParentTypeEnum.Claim && ParentRow != null)
                Text = string.Format(CultureInfo.InvariantCulture, "Состояния иск. работы №{0}", ParentRow["id_claim"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            DataBind();

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", ClaimStatesViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", ClaimStatesViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["claim_state_types"].DataSource, 
                "RowChanged", ClaimStateTypesViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["claim_state_types"].DataSource, 
                "RowDeleted", ClaimStateTypesViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["claim_state_types_relations"].DataSource, 
                "RowChanged", ClaimStateTypesRelationsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["claim_state_types_relations"].DataSource, 
                "RowDeleted", ClaimStateTypesRelationsViewport_RowDeleted);

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());
            
            DataChangeHandlersInit();
            
            IsEditable = true;
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void SaveRecord()
        {
            var claimState = (ClaimState) EntityFromViewport();
            if (!ValidateClaimState(claimState))
                return;
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    if (!((ClaimStatesPresenter)Presenter).InsertRecord(claimState))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((ClaimStatesPresenter)Presenter).UpdateRecord(claimState))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
            }
            UnbindedCheckBoxesUpdate();
            DataGridView.Enabled = true;
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanCopyRecord()
        {
            return false;
        }

        public override bool CanInsertRecord()
        {
            return !Presenter.ViewModel["general"].Model.EditingNewRecord && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();

            if (Presenter.ViewModel["claim_state_types"].BindingSource.Count > 0)
            {
                comboBoxClaimStateType.SelectedValue = ((DataRowView)Presenter.ViewModel["claim_state_types"].BindingSource[0])["id_state_type"];
            }
            textBoxExecutor.Text = UserDomain.Current.DisplayName;
            textBoxTransferToLegalDepartmentWho.Text = UserDomain.Current.DisplayName;
            textBoxAcceptedByLegalDepartmentWho.Text = UserDomain.Current.DisplayName;
            textBoxBksRequester.Text = UserDomain.Current.DisplayName;
            DataGridView.Enabled = false;
            IsEditable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) 
                && (ViewportState != ViewportState.NewRowState) 
                && AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            IsEditable = false;
            if (!((ClaimStatesPresenter)Presenter).DeleteRecord())
            {
                IsEditable = true;
                return;
            }
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
                    Presenter.ViewModel["general"].Model.EditingNewRecord = false;
                    var row = Presenter.ViewModel["general"].CurrentRow;
                    if (row != null)
                    {
                        IsEditable = false;
                        row.Delete();
                        if (Presenter.ViewModel["general"].CurrentRow != null)
                        {
                            DataGridView.Rows[Presenter.ViewModel["general"].BindingSource.Position].Selected = true;   
                        }
                    }
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            DataGridView.Enabled = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
        }

        protected override void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            _noUpdateFieldList = true;
            if (Presenter.ViewModel["general"].CurrentRow == null || DataGridView.RowCount == 0)
                DataGridView.ClearSelection();
            else
                if (bindingSource.Position >= DataGridView.RowCount)
                    DataGridView.Rows[DataGridView.RowCount - 1].Selected = true;
                else if (DataGridView.Rows[bindingSource.Position].Selected != true)
                    DataGridView.Rows[bindingSource.Position].Selected = true;
            var isEditable = IsEditable;
            UnbindedCheckBoxesUpdate();
            RebuildFilter();
            _noUpdateFieldList = false;
            comboBoxClaimStateType_SelectedValueChanged(this, new EventArgs());
            IsEditable = isEditable;
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void ClaimStatesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void ClaimStatesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        private void ClaimStateTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RebuildFilter();
        }

        private void ClaimStateTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RebuildFilter();
        }

        private void ClaimStateTypesRelationsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RebuildFilter();
        }

        private void ClaimStateTypesRelationsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RebuildFilter();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            textBoxDescription.Focus();
            base.OnVisibleChanged(e);
        }

        // Метод прячет поля с задержкой в 200 мс
        private void LagHideExtendedFields()
        {
            if (comboBoxClaimStateType.SelectedValue == DBNull.Value || comboBoxClaimStateType.SelectedValue == null ||
                _noUpdateFieldList)
            {
                tabControlWithoutTabs1.Visible = false;
            }
        }

        private void comboBoxClaimStateType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBoxClaimStateType.SelectedValue == DBNull.Value || comboBoxClaimStateType.SelectedValue == null || _noUpdateFieldList)
            {
                var context = SynchronizationContext.Current;
                ThreadPool.UnsafeQueueUserWorkItem(n =>
                {
                    Thread.Sleep(200);
                    context.Post(x => LagHideExtendedFields(), null);
                }, null);
                return;
            }
            var isEditabel = IsEditable;
            IsEditable = false;
            tabControlWithoutTabs1.Visible = true;
            if (!(comboBoxClaimStateType.SelectedValue is int)) return;
            switch ((int)comboBoxClaimStateType.SelectedValue)
            {
                case 1:
                    tabControlWithoutTabs1.SelectTab(tabPageRequestToBks);
                    break;
                case 2:
                    tabControlWithoutTabs1.SelectTab(tabPageToLegalDepartment);
                    break;
                case 3:
                    tabControlWithoutTabs1.SelectTab(tabPageAcceptedByLegalDepartment);
                    break;
                case 4:
                    tabControlWithoutTabs1.SelectTab(tabPagePreparingOrder);
                    break;
                case 5:
                    tabControlWithoutTabs1.SelectTab(tabPageExecutoryProcess);
                    break;
                case 6:
                    tabControlWithoutTabs1.SelectTab(tabPageCompletionClaims);
                    break;
                default:
                    tabControlWithoutTabs1.Visible = false;
                    break;
            }
            comboBoxClaimStateType.Focus();
            IsEditable = isEditabel;
        }
    }
}
