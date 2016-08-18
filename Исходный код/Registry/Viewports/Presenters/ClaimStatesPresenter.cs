using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal class ClaimStatesPresenter: Presenter
    {
        public ClaimStatesPresenter(): base(new ClaimStatesViewModel(), null, null)
        {
            
        }

        public void RebuildClaimStateTypeFilter()
        {
            var filter = "";
            IEnumerable<int> includedStates = null;
            var bindingSource = ViewModel["general"].BindingSource;
            // Если текущая позиция - первый элемент, и количество элементов 1 то он может иметь только начальное состояние (любое)
            if ((bindingSource.Position == 0) && (bindingSource.Count == 1))
                includedStates = ClaimsService.ClaimStartStateTypeIds();
            else
                // Если текущая позиция - первый элемент, и количество элементов больше 1 то он может иметь только начальное состояние 
                // (не противоречащее следующей позиции)
                if ((bindingSource.Position == 0) && (bindingSource.Count > 1))
                {
                    var nextRow = (DataRowView)bindingSource[bindingSource.Position + 1];
                    var nextClaimStateType = Convert.ToInt32(nextRow["id_state_type"], CultureInfo.InvariantCulture);
                    includedStates = ClaimsService.ClaimStateTypeIdsByNextStateType(nextClaimStateType);
                }
                else
                    // Если текущая позиция - последний элемент, то выбрать состояние, в которое можно перейти из состояния предыдущего элемента
                    if ((bindingSource.Position != -1) && (bindingSource.Position == bindingSource.Count - 1))
                    {
                        var prevRow = (DataRowView)bindingSource[bindingSource.Position - 1];
                        var prevClaimStateType = Convert.ToInt32(prevRow["id_state_type"], CultureInfo.InvariantCulture);
                        includedStates = ClaimsService.ClaimStateTypeIdsByPrevStateType(prevClaimStateType);
                    }
                    else
                        // Мы находимся не в конце списка и не в начале и необходимо выбрать только те состояния, в которые можно перейти с учетом окружающих состояний
                        if (bindingSource.Position != -1)
                        {
                            var prevRow = (DataRowView)bindingSource[bindingSource.Position - 1];
                            var nextRow = (DataRowView)bindingSource[bindingSource.Position + 1];
                            var prevClaimStateType = Convert.ToInt32(prevRow["id_state_type"], CultureInfo.InvariantCulture);
                            var nextClaimStateType = Convert.ToInt32(nextRow["id_state_type"], CultureInfo.InvariantCulture);
                            includedStates = ClaimsService.ClaimStateTypeIdsByNextAndPrevStateTypes(nextClaimStateType, prevClaimStateType);
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
            ViewModel["claim_state_types"].BindingSource.Filter = filter;
        }

        public bool InsertRecord(ClaimState claimState)
        {
            var idState = ViewModel["general"].Model.Insert(claimState);
            if (idState == -1)
            {
                return false;
            }
            claimState.IdState = idState;
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<ClaimState>.FillRow(claimState, row);
            ViewModel["general"].Model.EditingNewRecord = false;
            return true;
        }

        public bool UpdateRecord(ClaimState claimState)
        {
            if (claimState.IdState == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить запись о состоянии претензионно-исковой работы без внутреннего номера. " +
                             @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                             MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(claimState) == -1)
                return false;
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<ClaimState>.FillRow(claimState, row);
            return true;
        }

        public bool DeleteRecord()
        {
            var stateCount = -1;
            var bindingSource = ViewModel["general"].BindingSource;
            // Мы находимся в начале списка и текущий элемент не последний
            if ((bindingSource.Position == 0) && (bindingSource.Count > 1))
            {
                var nextRow = (DataRowView) bindingSource[bindingSource.Position + 1];
                var nextClaimStateType = Convert.ToInt32(nextRow["id_state_type"], CultureInfo.InvariantCulture);
                stateCount = (from claimStateTypesRow in ViewModel["claim_state_types"].Model.FilterDeletedRows()
                              where Convert.ToBoolean(claimStateTypesRow.Field<object>("is_start_state_type"), CultureInfo.InvariantCulture) &&
                                    (claimStateTypesRow.Field<int>("id_state_type") == nextClaimStateType)
                              select claimStateTypesRow.Field<int>("id_state_type")).Count();
            }
            else
                // Мы находимся не в конце списка и не в начале
                if ((bindingSource.Position != -1) && (bindingSource.Position != bindingSource.Count - 1))
                {
                    var nextRow = (DataRowView)bindingSource[bindingSource.Position + 1];
                    var prevRow = (DataRowView)bindingSource[bindingSource.Position - 1];
                    var previosClaimStateType = Convert.ToInt32(prevRow["id_state_type"], CultureInfo.InvariantCulture);
                    var nextClaimStateType = Convert.ToInt32(nextRow["id_state_type"], CultureInfo.InvariantCulture);
                    stateCount = (from claimStateTypesRelRow in ViewModel["claim_state_types_relations"].Model.FilterDeletedRows()
                                  where claimStateTypesRelRow.Field<int>("id_state_from") == previosClaimStateType &&
                                        claimStateTypesRelRow.Field<int>("id_state_to") == nextClaimStateType
                                  select claimStateTypesRelRow.Field<int>("id_state_to")).Count();
                }
            if (stateCount == 0)
            {
                MessageBox.Show(@"Вы не можете удалить это состояние, так как это нарушит цепочку зависимости состояний претензионно-исковой работы." +
                                @"Чтобы удалить данное состояние, необходимо сначала удалить все состояния после него", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            var row = ViewModel["general"].CurrentRow;
            var columnName = ViewModel["general"].PrimaryKeyFirst;
            if (ViewModel["general"].Model.Delete((int)row[columnName]) == -1)
                return false;
            row.Delete();
            return true;
        }
    }
}
