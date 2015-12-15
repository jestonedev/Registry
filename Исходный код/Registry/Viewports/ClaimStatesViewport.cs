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
    internal sealed partial class ClaimStatesViewport : FormWithGridViewport
    {
        #region Models
        DataModel claim_state_types;
        DataModel claim_state_types_relations;
        #endregion Models

        #region Views
        BindingSource v_claim_state_types;
        BindingSource v_claim_state_types_for_grid;
        #endregion Views

        private bool noUpdateFieldList = false;

        private ClaimStatesViewport()
            : this(null, null)
        {
        }

        public ClaimStatesViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
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
            comboBoxClaimStateType.ValueMember = "id_state_type";
            comboBoxClaimStateType.DisplayMember = "state_type";
            comboBoxClaimStateType.DataSource = v_claim_state_types;
            comboBoxClaimStateType.DataBindings.Clear();
            comboBoxClaimStateType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_state_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerStartState.DataBindings.Clear();
            dateTimePickerStartState.DataBindings.Add("Value", GeneralBindingSource, "date_start_state", true, DataSourceUpdateMode.Never, null);

            dataGridView.DataSource = GeneralBindingSource;
            id_state_type.DataSource = v_claim_state_types_for_grid;
            id_state_type.DisplayMember = "state_type";
            id_state_type.ValueMember = "id_state_type";
            id_state_type.DataPropertyName = "id_state_type";
            date_start_state.DataPropertyName = "date_start_state";
            description.DataPropertyName = "description";

            dateTimePickerTransfertToLegalDepartmentDate.DataBindings.Clear();
            dateTimePickerTransfertToLegalDepartmentDate.DataBindings.Add("Value", GeneralBindingSource, "transfert_to_legal_department_date",true, DataSourceUpdateMode.Never, null);
            textBoxTransferToLegalDepartmentWho.DataBindings.Clear();
            textBoxTransferToLegalDepartmentWho.DataBindings.Add("Text", GeneralBindingSource, "transfer_to_legal_department_who",true, DataSourceUpdateMode.Never, "");
            dateTimePickerAcceptedByLegalDepartmentDate.DataBindings.Clear();
            dateTimePickerAcceptedByLegalDepartmentDate.DataBindings.Add("Value", GeneralBindingSource, "accepted_by_legal_department_date",true, DataSourceUpdateMode.Never, null);
            textBoxAcceptedByLegalDepartmentWho.DataBindings.Clear();
            textBoxAcceptedByLegalDepartmentWho.DataBindings.Add("Text", GeneralBindingSource, "accepted_by_legal_department_who", true, DataSourceUpdateMode.Never, "");
            dateTimePickerClaimDirectionDate.DataBindings.Clear();
            dateTimePickerClaimDirectionDate.DataBindings.Add("Value", GeneralBindingSource, "claim_direction_date",true, DataSourceUpdateMode.Never, null);
            textBoxClaimDirectionDescription.DataBindings.Clear();
            textBoxClaimDirectionDescription.DataBindings.Add("Text", GeneralBindingSource, "claim_direction_description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerCourtOrderDate.DataBindings.Clear();
            dateTimePickerCourtOrderDate.DataBindings.Add("Value", GeneralBindingSource, "court_order_date",true, DataSourceUpdateMode.Never, null);
            textBoxCourtOrderNum.DataBindings.Clear();
            textBoxCourtOrderNum.DataBindings.Add("Text", GeneralBindingSource, "court_order_num", true, DataSourceUpdateMode.Never, "");
            dateTimePickerObtainingCourtOrderDate.DataBindings.Clear();
            dateTimePickerObtainingCourtOrderDate.DataBindings.Add("Value", GeneralBindingSource, "obtaining_court_order_date",true, DataSourceUpdateMode.Never, null);
            textBoxObtainingCourtOrderDescription.DataBindings.Clear();
            textBoxObtainingCourtOrderDescription.DataBindings.Add("Text", GeneralBindingSource, "obtaining_court_order_description", true, DataSourceUpdateMode.Never, "");

            dateTimePickerDirectionCourtOrderBailiffsDate.DataBindings.Clear();
            dateTimePickerDirectionCourtOrderBailiffsDate.DataBindings.Add("Value", GeneralBindingSource, "direction_court_order_bailiffs_date",true, DataSourceUpdateMode.Never, null);
            textBoxDirectionCourtOrderBailiffsDescription.DataBindings.Clear();
            textBoxDirectionCourtOrderBailiffsDescription.DataBindings.Add("Text", GeneralBindingSource, "direction_court_order_bailiffs_description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerEnforcementProceedingStartDate.DataBindings.Clear();
            dateTimePickerEnforcementProceedingStartDate.DataBindings.Add("Value", GeneralBindingSource, "enforcement_proceeding_start_date",true, DataSourceUpdateMode.Never, null);
            textBoxEnforcementProceedingStartDescription.DataBindings.Clear();
            textBoxEnforcementProceedingStartDescription.DataBindings.Add("Text", GeneralBindingSource, "enforcement_proceeding_start_description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerEnforcementProceedingEndDate.DataBindings.Clear();
            dateTimePickerEnforcementProceedingEndDate.DataBindings.Add("Value", GeneralBindingSource, "enforcement_proceeding_end_date",true, DataSourceUpdateMode.Never, null);
            textBoxEnforcementProceedingEndDescription.DataBindings.Clear();
            textBoxEnforcementProceedingEndDescription.DataBindings.Add("Text", GeneralBindingSource, "enforcement_proceeding_end_description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerEnforcementProceedingTerminateDate.DataBindings.Clear();
            dateTimePickerEnforcementProceedingTerminateDate.DataBindings.Add("Value", GeneralBindingSource, "enforcement_proceeding_terminate_date",true, DataSourceUpdateMode.Never, null);
            textBoxEnforcementProceedingTerminateDescription.DataBindings.Clear();
            textBoxEnforcementProceedingTerminateDescription.DataBindings.Add("Text", GeneralBindingSource, "enforcement_proceeding_terminate_description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.DataBindings.Clear();
            dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.DataBindings.Add("Value", GeneralBindingSource, "repeated_direction_court_order_bailiffs_date",true, DataSourceUpdateMode.Never, null);
            textBoxRepeatedDirectionCourtOrderBailiffsDescription.DataBindings.Clear();
            textBoxRepeatedDirectionCourtOrderBailiffsDescription.DataBindings.Add("Text", GeneralBindingSource, "repeated_direction_court_order_bailiffs_description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerRepeatedEnforcementProceedingStartDate.DataBindings.Clear();
            dateTimePickerRepeatedEnforcementProceedingStartDate.DataBindings.Add("Value", GeneralBindingSource, "repeated_enforcement_proceeding_start_date",true, DataSourceUpdateMode.Never, null);
            textBoxRepeatedEnforcementProceedingStartDescription.DataBindings.Clear();
            textBoxRepeatedEnforcementProceedingStartDescription.DataBindings.Add("Text", GeneralBindingSource, "repeated_enforcement_proceeding_start_description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerRepeatedEnforcementProceedingEndDate.DataBindings.Clear();
            dateTimePickerRepeatedEnforcementProceedingEndDate.DataBindings.Add("Value", GeneralBindingSource, "repeated_enforcement_proceeding_end_date",true, DataSourceUpdateMode.Never, null);
            textBoxRepeatedEnforcementProceedingEndDescription.DataBindings.Clear();
            textBoxRepeatedEnforcementProceedingEndDescription.DataBindings.Add("Text", GeneralBindingSource, "repeated_enforcement_proceeding_end_description", true, DataSourceUpdateMode.Never, "");

            dateTimePickerCourtOrderCancelDate.DataBindings.Clear();
            dateTimePickerCourtOrderCancelDate.DataBindings.Add("Value", GeneralBindingSource, "court_order_cancel_date",true, DataSourceUpdateMode.Never, null);
            textBoxCourtOrderCancelDescription.DataBindings.Clear();
            textBoxCourtOrderCancelDescription.DataBindings.Add("Text", GeneralBindingSource, "court_order_cancel_description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerClaimCompleteDate.DataBindings.Clear();
            dateTimePickerClaimCompleteDate.DataBindings.Add("Value", GeneralBindingSource, "claim_complete_date",true, DataSourceUpdateMode.Never, null);
            textBoxClaimCompleteDescription.DataBindings.Clear();
            textBoxClaimCompleteDescription.DataBindings.Add("Text", GeneralBindingSource, "claim_complete_description", true, DataSourceUpdateMode.Never, "");
            textBoxClaimCompleteReason.DataBindings.Clear();
            textBoxClaimCompleteReason.DataBindings.Add("Text", GeneralBindingSource, "claim_complete_reason", true, DataSourceUpdateMode.Never, "");
        }

        private void UnbindedCheckBoxesUpdate()
        {
            if (GeneralBindingSource.Count == 0) return;
            var row = GeneralBindingSource.Position >= 0 ? (DataRowView)GeneralBindingSource[GeneralBindingSource.Position] : null;
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["date_start_state"] != DBNull.Value)))
                dateTimePickerStartState.Checked = true;
            else
            {
                dateTimePickerStartState.Value = DateTime.Now.Date;
                dateTimePickerStartState.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["transfert_to_legal_department_date"] != DBNull.Value)))
                dateTimePickerTransfertToLegalDepartmentDate.Checked = true;
            else
            {
                dateTimePickerTransfertToLegalDepartmentDate.Value = DateTime.Now.Date;
                dateTimePickerTransfertToLegalDepartmentDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["accepted_by_legal_department_date"] != DBNull.Value)))
                dateTimePickerAcceptedByLegalDepartmentDate.Checked = true;
            else
            {
                dateTimePickerAcceptedByLegalDepartmentDate.Value = DateTime.Now.Date;
                dateTimePickerAcceptedByLegalDepartmentDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["claim_direction_date"] != DBNull.Value)))
                dateTimePickerClaimDirectionDate.Checked = true;
            else
            {
                dateTimePickerClaimDirectionDate.Value = DateTime.Now.Date;
                dateTimePickerClaimDirectionDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["court_order_date"] != DBNull.Value)))
                dateTimePickerCourtOrderDate.Checked = true;
            else
            {
                dateTimePickerCourtOrderDate.Value = DateTime.Now.Date;
                dateTimePickerCourtOrderDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["obtaining_court_order_date"] != DBNull.Value)))
                dateTimePickerObtainingCourtOrderDate.Checked = true;
            else
            {
                dateTimePickerObtainingCourtOrderDate.Value = DateTime.Now.Date;
                dateTimePickerObtainingCourtOrderDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["direction_court_order_bailiffs_date"] != DBNull.Value)))
                dateTimePickerDirectionCourtOrderBailiffsDate.Checked = true;
            else
            {
                dateTimePickerDirectionCourtOrderBailiffsDate.Value = DateTime.Now.Date;
                dateTimePickerDirectionCourtOrderBailiffsDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["enforcement_proceeding_start_date"] != DBNull.Value)))
                dateTimePickerEnforcementProceedingStartDate.Checked = true;
            else
            {
                dateTimePickerEnforcementProceedingStartDate.Value = DateTime.Now.Date;
                dateTimePickerEnforcementProceedingStartDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["enforcement_proceeding_end_date"] != DBNull.Value)))
                dateTimePickerEnforcementProceedingEndDate.Checked = true;
            else
            {
                dateTimePickerEnforcementProceedingEndDate.Value = DateTime.Now.Date;
                dateTimePickerEnforcementProceedingEndDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["enforcement_proceeding_terminate_date"] != DBNull.Value)))
                dateTimePickerEnforcementProceedingTerminateDate.Checked = true;
            else
            {
                dateTimePickerEnforcementProceedingTerminateDate.Value = DateTime.Now.Date;
                dateTimePickerEnforcementProceedingTerminateDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["repeated_direction_court_order_bailiffs_date"] != DBNull.Value)))
                dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Checked = true;
            else
            {
                dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Value = DateTime.Now.Date;
                dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["repeated_enforcement_proceeding_start_date"] != DBNull.Value)))
                dateTimePickerRepeatedEnforcementProceedingStartDate.Checked = true;
            else
            {
                dateTimePickerRepeatedEnforcementProceedingStartDate.Value = DateTime.Now.Date;
                dateTimePickerRepeatedEnforcementProceedingStartDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["repeated_enforcement_proceeding_end_date"] != DBNull.Value)))
                dateTimePickerRepeatedEnforcementProceedingEndDate.Checked = true;
            else
            {
                dateTimePickerRepeatedEnforcementProceedingEndDate.Value = DateTime.Now.Date;
                dateTimePickerRepeatedEnforcementProceedingEndDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["court_order_cancel_date"] != DBNull.Value)))
                dateTimePickerCourtOrderCancelDate.Checked = true;
            else
            {
                dateTimePickerCourtOrderCancelDate.Value = DateTime.Now.Date;
                dateTimePickerCourtOrderCancelDate.Checked = false;
            }
            if (row != null && ((GeneralBindingSource.Position >= 0) && (row["claim_complete_date"] != DBNull.Value)))
                dateTimePickerClaimCompleteDate.Checked = true;
            else
            {
                dateTimePickerClaimCompleteDate.Value = DateTime.Now.Date;
                dateTimePickerClaimCompleteDate.Checked = false;
            }
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.ClaimsWrite))
                return base.ChangeViewportStateTo(state);
            viewportState = ViewportState.ReadState;
            return true;
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
            row["description"] = ViewportHelper.ValueOrDBNull(claimState.Description);

            row["transfert_to_legal_department_date"] = ViewportHelper.ValueOrDBNull(claimState.TransfertToLegalDepartmentDate);
            row["transfer_to_legal_department_who"] = ViewportHelper.ValueOrDBNull(claimState.TransferToLegalDepartmentWho);
            row["accepted_by_legal_department_date"] = ViewportHelper.ValueOrDBNull(claimState.AcceptedByLegalDepartmentDate);
            row["accepted_by_legal_department_who"] = ViewportHelper.ValueOrDBNull(claimState.AcceptedByLegalDepartmentWho);

            row["claim_direction_date"] = ViewportHelper.ValueOrDBNull(claimState.ClaimDirectionDate);
            row["claim_direction_description"] = ViewportHelper.ValueOrDBNull(claimState.ClaimDirectionDescription);
            row["court_order_date"] = ViewportHelper.ValueOrDBNull(claimState.CourtOrderDate);
            row["court_order_num"] = ViewportHelper.ValueOrDBNull(claimState.CourtOrderNum);
            row["obtaining_court_order_date"] = ViewportHelper.ValueOrDBNull(claimState.ObtainingCourtOrderDate);
            row["obtaining_court_order_description"] = ViewportHelper.ValueOrDBNull(claimState.ObtainingCourtOrderDescription);

            row["direction_court_order_bailiffs_date"] = ViewportHelper.ValueOrDBNull(claimState.DirectionCourtOrderBailiffsDate);
            row["direction_court_order_bailiffs_description"] = ViewportHelper.ValueOrDBNull(claimState.DirectionCourtOrderBailiffsDescription);
            row["enforcement_proceeding_start_date"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingStartDate);
            row["enforcement_proceeding_start_description"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingStartDescription);
            row["enforcement_proceeding_end_date"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingEndDate);
            row["enforcement_proceeding_end_description"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingEndDescription);
            row["enforcement_proceeding_terminate_date"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingTerminateDate);
            row["enforcement_proceeding_terminate_description"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingTerminateDescription);
            row["repeated_direction_court_order_bailiffs_date"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedDirectionCourtOrderBailiffsDate);
            row["repeated_direction_court_order_bailiffs_description"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedDirectionCourtOrderBailiffsDescription);
            row["repeated_enforcement_proceeding_start_date"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedEnforcementProceedingStartDate);
            row["repeated_enforcement_proceeding_start_description"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedEnforcementProceedingStartDescription);
            row["repeated_enforcement_proceeding_end_date"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedEnforcementProceedingEndDate);
            row["repeated_enforcement_proceeding_end_description"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedEnforcementProceedingEndDescription);

            row["court_order_cancel_date"] = ViewportHelper.ValueOrDBNull(claimState.CourtOrderCancelDate);
            row["court_order_cancel_description"] = ViewportHelper.ValueOrDBNull(claimState.CourtOrderCancelDescription);
            row["claim_complete_date"] = ViewportHelper.ValueOrDBNull(claimState.ClaimCompleteDate);
            row["claim_complete_description"] = ViewportHelper.ValueOrDBNull(claimState.ClaimCompleteDescription);
            row["claim_complete_reason"] = ViewportHelper.ValueOrDBNull(claimState.ClaimCompleteReason);
            row.EndEdit();
        }

        private void ViewportFromClaimState(ClaimState claimState)
        {
            comboBoxClaimStateType.SelectedValue = ViewportHelper.ValueOrDBNull(claimState.IdStateType);
            textBoxDescription.Text = claimState.Description;
            dateTimePickerStartState.Value = ViewportHelper.ValueOrDefault(claimState.DateStartState);

            dateTimePickerTransfertToLegalDepartmentDate.Value = ViewportHelper.ValueOrDefault(claimState.TransfertToLegalDepartmentDate);
            textBoxTransferToLegalDepartmentWho.Text = claimState.TransferToLegalDepartmentWho;
            dateTimePickerAcceptedByLegalDepartmentDate.Value = ViewportHelper.ValueOrDefault(claimState.AcceptedByLegalDepartmentDate);
            textBoxAcceptedByLegalDepartmentWho.Text = claimState.AcceptedByLegalDepartmentWho;

            dateTimePickerClaimDirectionDate.Value = ViewportHelper.ValueOrDefault(claimState.ClaimDirectionDate);
            textBoxClaimDirectionDescription.Text = claimState.ClaimDirectionDescription;
            dateTimePickerClaimDirectionDate.Value = ViewportHelper.ValueOrDefault(claimState.CourtOrderDate);
            textBoxCourtOrderNum.Text = claimState.CourtOrderNum;
            dateTimePickerObtainingCourtOrderDate.Value = ViewportHelper.ValueOrDefault(claimState.ObtainingCourtOrderDate);
            textBoxObtainingCourtOrderDescription.Text = claimState.ObtainingCourtOrderDescription;

            dateTimePickerDirectionCourtOrderBailiffsDate.Value = ViewportHelper.ValueOrDefault(claimState.DirectionCourtOrderBailiffsDate);
            textBoxDirectionCourtOrderBailiffsDescription.Text = claimState.DirectionCourtOrderBailiffsDescription;
            dateTimePickerEnforcementProceedingStartDate.Value = ViewportHelper.ValueOrDefault(claimState.EnforcementProceedingStartDate);
            textBoxEnforcementProceedingStartDescription.Text = claimState.EnforcementProceedingStartDescription;
            dateTimePickerEnforcementProceedingEndDate.Value = ViewportHelper.ValueOrDefault(claimState.EnforcementProceedingEndDate);
            textBoxEnforcementProceedingEndDescription.Text = claimState.EnforcementProceedingEndDescription;
            dateTimePickerEnforcementProceedingTerminateDate.Value = ViewportHelper.ValueOrDefault(claimState.EnforcementProceedingTerminateDate);
            textBoxEnforcementProceedingTerminateDescription.Text = claimState.EnforcementProceedingTerminateDescription;
            dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Value = ViewportHelper.ValueOrDefault(claimState.RepeatedDirectionCourtOrderBailiffsDate);
            textBoxRepeatedDirectionCourtOrderBailiffsDescription.Text = claimState.RepeatedDirectionCourtOrderBailiffsDescription;
            dateTimePickerRepeatedEnforcementProceedingStartDate.Value = ViewportHelper.ValueOrDefault(claimState.RepeatedEnforcementProceedingStartDate);
            textBoxRepeatedEnforcementProceedingStartDescription.Text = claimState.RepeatedEnforcementProceedingStartDescription;
            dateTimePickerRepeatedEnforcementProceedingEndDate.Value = ViewportHelper.ValueOrDefault(claimState.RepeatedEnforcementProceedingEndDate);
            textBoxRepeatedEnforcementProceedingEndDescription.Text = claimState.RepeatedEnforcementProceedingEndDescription;

            dateTimePickerCourtOrderCancelDate.Value = ViewportHelper.ValueOrDefault(claimState.CourtOrderCancelDate);
            textBoxObtainingCourtOrderDescription.Text = claimState.CourtOrderCancelDescription;
            dateTimePickerClaimCompleteDate.Value = ViewportHelper.ValueOrDefault(claimState.ClaimCompleteDate);
            textBoxClaimCompleteDescription.Text = claimState.ClaimCompleteDescription;
            textBoxClaimCompleteReason.Text = claimState.ClaimCompleteReason;
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
                Description = ViewportHelper.ValueOrNull(textBoxDescription),
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
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            var claimState = new ClaimState
            {
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type"),
                IdClaim = ViewportHelper.ValueOrNull<int>(row, "id_claim"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                DateStartState = ViewportHelper.ValueOrNull<DateTime>(row, "date_start_state"),

                TransfertToLegalDepartmentDate = ViewportHelper.ValueOrNull<DateTime>(row, "transfert_to_legal_department_date"),
                TransferToLegalDepartmentWho = ViewportHelper.ValueOrNull(row, "transfer_to_legal_department_who"),
                AcceptedByLegalDepartmentDate = ViewportHelper.ValueOrNull<DateTime>(row, "accepted_by_legal_department_date"),
                AcceptedByLegalDepartmentWho = ViewportHelper.ValueOrNull(row, "accepted_by_legal_department_who"),

                ClaimDirectionDate = ViewportHelper.ValueOrNull<DateTime>(row, "claim_direction_date"),
                ClaimDirectionDescription = ViewportHelper.ValueOrNull(row, "claim_direction_description"),
                CourtOrderDate = ViewportHelper.ValueOrNull<DateTime>(row, "court_order_date"),
                CourtOrderNum = ViewportHelper.ValueOrNull(row, "court_order_num"),
                ObtainingCourtOrderDate = ViewportHelper.ValueOrNull<DateTime>(row, "obtaining_court_order_date"),
                ObtainingCourtOrderDescription = ViewportHelper.ValueOrNull(row, "obtaining_court_order_description"),

                DirectionCourtOrderBailiffsDate = ViewportHelper.ValueOrNull<DateTime>(row, "direction_court_order_bailiffs_date"),
                DirectionCourtOrderBailiffsDescription = ViewportHelper.ValueOrNull(row, "direction_court_order_bailiffs_description"),
                EnforcementProceedingStartDate = ViewportHelper.ValueOrNull<DateTime>(row, "enforcement_proceeding_start_date"),
                EnforcementProceedingStartDescription = ViewportHelper.ValueOrNull(row, "enforcement_proceeding_start_description"),
                EnforcementProceedingEndDate = ViewportHelper.ValueOrNull<DateTime>(row, "enforcement_proceeding_end_date"),
                EnforcementProceedingEndDescription = ViewportHelper.ValueOrNull(row, "enforcement_proceeding_end_description"),
                EnforcementProceedingTerminateDate = ViewportHelper.ValueOrNull<DateTime>(row, "enforcement_proceeding_terminate_date"),
                EnforcementProceedingTerminateDescription = ViewportHelper.ValueOrNull(row, "enforcement_proceeding_terminate_description"),
                RepeatedDirectionCourtOrderBailiffsDate = ViewportHelper.ValueOrNull<DateTime>(row, "repeated_direction_court_order_bailiffs_date"),
                RepeatedDirectionCourtOrderBailiffsDescription = ViewportHelper.ValueOrNull(row, "repeated_direction_court_order_bailiffs_description"),
                RepeatedEnforcementProceedingStartDate = ViewportHelper.ValueOrNull<DateTime>(row, "repeated_enforcement_proceeding_start_date"),
                RepeatedEnforcementProceedingStartDescription = ViewportHelper.ValueOrNull(row, "repeated_enforcement_proceeding_start_description"),
                RepeatedEnforcementProceedingEndDate = ViewportHelper.ValueOrNull<DateTime>(row, "repeated_enforcement_proceeding_end_date"),
                RepeatedEnforcementProceedingEndDescription = ViewportHelper.ValueOrNull(row, "repeated_enforcement_proceeding_end_description"),

                CourtOrderCancelDate = ViewportHelper.ValueOrNull<DateTime>(row, "court_order_cancel_date"),
                CourtOrderCancelDescription = ViewportHelper.ValueOrNull(row, "court_order_cancel_description"),
                ClaimCompleteDate = ViewportHelper.ValueOrNull<DateTime>(row, "claim_complete_date"),
                ClaimCompleteDescription = ViewportHelper.ValueOrNull(row, "claim_complete_description"),
                ClaimCompleteReason = ViewportHelper.ValueOrNull(row, "claim_complete_reason"),
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
            dateTimePickerStartState.Checked = (claimState.DateStartState != null);

            dateTimePickerTransfertToLegalDepartmentDate.Checked = claimState.TransfertToLegalDepartmentDate != null;
            dateTimePickerAcceptedByLegalDepartmentDate.Checked = claimState.AcceptedByLegalDepartmentDate != null;
            dateTimePickerClaimDirectionDate.Checked = claimState.ClaimDirectionDate != null;
            dateTimePickerCourtOrderDate.Checked = claimState.CourtOrderDate != null;
            dateTimePickerObtainingCourtOrderDate.Checked = claimState.ObtainingCourtOrderDate != null;
            dateTimePickerDirectionCourtOrderBailiffsDate.Checked = claimState.DirectionCourtOrderBailiffsDate != null;
            dateTimePickerEnforcementProceedingStartDate.Checked = claimState.EnforcementProceedingStartDate != null;
            dateTimePickerEnforcementProceedingEndDate.Checked = claimState.EnforcementProceedingEndDate != null;
            dateTimePickerEnforcementProceedingTerminateDate.Checked = claimState.EnforcementProceedingTerminateDate != null;
            dateTimePickerRepeatedDirectionCourtOrderBailiffsDate.Checked = claimState.RepeatedDirectionCourtOrderBailiffsDate != null;
            dateTimePickerRepeatedEnforcementProceedingStartDate.Checked = claimState.RepeatedEnforcementProceedingStartDate != null;
            dateTimePickerRepeatedEnforcementProceedingEndDate.Checked = claimState.RepeatedEnforcementProceedingEndDate != null;
            dateTimePickerCourtOrderCancelDate.Checked = claimState.CourtOrderCancelDate != null;
            dateTimePickerClaimCompleteDate.Checked = claimState.ClaimCompleteDate != null;

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
            if (v_claim_state_types.Count > 0)
                comboBoxClaimStateType.SelectedValue = ((DataRowView)v_claim_state_types[0])["id_state_type"];
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                GeneralBindingSource.CurrentItemChanged -= v_claim_states_CurrentItemChanged;
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
            Close();
        }

        void v_claim_states_CurrentItemChanged(object sender, EventArgs e)
        {
            noUpdateFieldList = true;
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
            noUpdateFieldList = false;
            comboBoxClaimStateType_SelectedValueChanged(this, new EventArgs());
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

        private void comboBoxClaimStateType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBoxClaimStateType.SelectedValue == DBNull.Value || comboBoxClaimStateType.SelectedValue == null || noUpdateFieldList)
            {  
                tableLayoutPanelAll.RowStyles[0].Height = 90F;
                tabControlWithoutTabs1.Visible = false;
                return;
            }
            tabControlWithoutTabs1.Visible = true;
            switch ((int)comboBoxClaimStateType.SelectedValue)
            {
                case 1:
                    tableLayoutPanelAll.RowStyles[0].Height = groupBoxTransfertToLegalDepartment.Height +
                                                              groupBoxAcceptedByLegalDepartment.Height + 90;
                    tabControlWithoutTabs1.SelectTab(tabPageToLegalDepartment);
                    break;
                case 2:
                    tableLayoutPanelAll.RowStyles[0].Height = groupBoxClaimDirectedToCourt.Height +
                                                              groupBoxCourtOrder.Height + groupBoxObtainingCourtOrder.Height + 90;
                    tabControlWithoutTabs1.SelectTab(tabPagePreparingOrder);
                    break;
                case 3:
                    tableLayoutPanelAll.RowStyles[0].Height =
                        groupBoxDirectionCourtOrderBailiffs.Height + groupBoxEnforcementProceedingStart.Height +
                        groupBoxEnforcementProceedingEnd.Height + groupBoxEnforcementProceedingTerminate.Height +
                        groupBoxRepeatedEnforcementProceedingStart.Height +
                        groupBoxRepeatedEnforcementProceedingEnd.Height +
                        groupBoxRepeatedDirectionCourtOrderBailiffs.Height + 90;
                    tabControlWithoutTabs1.SelectTab(tabPageExecutoryProcess);
                    break;
                case 4:
                    tableLayoutPanelAll.RowStyles[0].Height = groupBoxCourtOrderCancel.Height + groupBoxClaimComplete.Height + 90;
                    tabControlWithoutTabs1.SelectTab(tabPageCompletionClaims);
                    break;
                default:
                    tableLayoutPanelAll.RowStyles[0].Height = 90F;
                    tabControlWithoutTabs1.Visible = false;
                    break;
            }
        }
    }
}
