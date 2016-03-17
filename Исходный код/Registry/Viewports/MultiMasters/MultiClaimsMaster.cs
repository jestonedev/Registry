using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    public sealed partial class MultiClaimsMaster : DockContent, IMultiMaster
    {
        private readonly BindingSource _claims = new BindingSource();
        private readonly IMenuCallback _menuCallback;

        public MultiClaimsMaster(IMenuCallback menuCallback)
        {
            InitializeComponent();
            DockAreas = DockAreas.DockLeft | DockAreas.DockRight
                        | DockAreas.DockTop
                        | DockAreas.DockBottom;
            _menuCallback = menuCallback;
            DataModel.GetInstance(DataModelType.ClaimsDataModel).Select();
            _claims.DataSource = DataModel.DataSet;
            _claims.DataMember = "claims";
            _claims.Filter = "0 = 1";
            dataGridView.RowCount = 0;
        }

        void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                _claims.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + (way == SortOrder.Ascending ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                _claims.Position = dataGridView.SelectedRows[0].Index;
            else
                _claims.Position = -1;
        }

        void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_claims.Count <= e.RowIndex) return;
            var row = (DataRowView)_claims[e.RowIndex];
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_claim":
                    e.Value = row["id_claim"];
                    break;
                case "id_account":
                    if (row["id_account"] == DBNull.Value) return;
                    var accountList = (from paymentsAccountRow in DataModel.GetInstance(DataModelType.PaymentsAccountsDataModel).FilterDeletedRows()
                                       where paymentsAccountRow.Field<int?>("id_account") == (int?)row["id_account"]
                                       select paymentsAccountRow).ToList();
                    if (accountList.Any())
                        e.Value = accountList.First().Field<string>("account");
                    break;
                case "start_dept_period":
                    e.Value = row["start_dept_period"] == DBNull.Value ? "" :
                        ((DateTime)row["start_dept_period"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "end_dept_period":
                    e.Value = row["end_dept_period"] == DBNull.Value ? "" :
                        ((DateTime)row["end_dept_period"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "amount_tenancy":
                    e.Value = row["amount_tenancy"];
                    break;
                case "amount_dgi":
                    e.Value = row["amount_dgi"];
                    break;
                case "at_date":
                    e.Value = row["at_date"] == DBNull.Value ? "" :
                        ((DateTime)row["at_date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "description":
                    e.Value = row["description"];
                    break;
                case "current_state":
                    if (row["id_claim"] == DBNull.Value || row["id_claim"] == null) return;
                    var idClaim = (int?)row["id_claim"];
                    var lastClaimStateMaxIds =
                        from claimStateRow in DataModel.GetInstance(DataModelType.ClaimStatesDataModel).FilterDeletedRows()
                        where claimStateRow.Field<int?>("id_claim") == idClaim
                        group claimStateRow.Field<int?>("id_state") by claimStateRow.Field<int?>("id_claim") into gs
                        select new
                        {
                            id_claim = gs.Key,
                            id_state = gs.Max()
                        };
                    var lastClaimState =
                        (from claimStateRow in
                             DataModel.GetInstance(DataModelType.ClaimStatesDataModel).FilterDeletedRows()
                         join lastClaimStateRow in lastClaimStateMaxIds
                             on claimStateRow.Field<int?>("id_state") equals lastClaimStateRow.id_state
                         join stateTypeRow in
                             DataModel.GetInstance(DataModelType.ClaimStateTypesDataModel).FilterDeletedRows()
                             on claimStateRow.Field<int?>("id_state_type") equals
                             stateTypeRow.Field<int?>("id_state_type")
                         select stateTypeRow.Field<string>("state_type")).ToList();
                    if (lastClaimState.Any())
                        e.Value = lastClaimState.First();
                    break;
            }
        }

        private void toolStripButtonAccountDeleteAll_Click(object sender, EventArgs e)
        {
            _claims.Filter = "0 = 1";
            dataGridView.RowCount = _claims.Count;
        }

        private void toolStripButtonClaimDelete_Click(object sender, EventArgs e)
        {
            if (_claims.Position < 0) return;
            if (((DataRowView)_claims[_claims.Position])["id_claim"] == DBNull.Value) return;
            var idAccount = (int)((DataRowView)_claims[_claims.Position])["id_claim"];
            _claims.Filter = string.Format("({0}) AND (id_claim <> {1})", _claims.Filter, idAccount);
            dataGridView.RowCount = _claims.Count;
            dataGridView.Refresh();
        }

        private void toolStripButtonClaimCurrent_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            IEnumerable<int> idClaim = new List<int>();
            var claimsViewport = viewport as ClaimListViewport;
            if (claimsViewport != null)
                idClaim = claimsViewport.GetCurrentIds();
            if (!idClaim.Any()) return;
            _claims.Filter = string.Format("({0}) OR (id_claim IN ({1}))", _claims.Filter, idClaim.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
            dataGridView.RowCount = _claims.Count;
        }

        private void toolStripButtonAccountsByFilter_Click(object sender, EventArgs e)
        {
            var viewport = _menuCallback.GetCurrentViewport();
            if (viewport == null)
                return;
            var filter = "";
            var claimsViewport = viewport as ClaimListViewport;
            if (claimsViewport != null)
                filter = claimsViewport.GetFilter();
            if (filter == "") filter = "1=1";
            _claims.Filter = string.Format("({0}) OR ({1})", _claims.Filter, filter);
            dataGridView.RowCount = _claims.Count;
        }

        private void toolStripButtonCreateClaimStates_Click(object sender, EventArgs e)
        {
            if (_claims.Count == 0)
                return;
            if (DataModel.GetInstance(DataModelType.ClaimStatesDataModel).EditingNewRecord)
            {
                MessageBox.Show(@"Невозможно провести массовую операцию пока форма состояний исковых работ находится в состоянии добавления новой записи. Отмените добавление новой записи или сохраните ее.",
                    @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
            var form = new MultiClaimsStateConfigForm();
            if (form.ShowDialog() == DialogResult.Cancel) return;
            if (form.IdStateType == null)
                return;
            var claimStatesDataModel = DataModel.GetInstance(DataModelType.ClaimStatesDataModel);
            var claimStatesBindingSource = new BindingSource
            {
                DataSource = claimStatesDataModel.Select()
            };
            for (var i = 0; i < _claims.Count; i++)
            {
                var row = (DataRowView)_claims[i];
                if (row["id_claim"] == DBNull.Value) continue;
                var lastStates = from stateRow in claimStatesDataModel.FilterDeletedRows()
                                 where stateRow.Field<int>("id_claim") == (int)row["id_claim"]
                                 group stateRow.Field<int?>("id_state") by stateRow.Field<int>("id_claim") into gs
                                 select new
                                 {
                                     id_claim = gs.Key,
                                     id_state = gs.Max()
                                 };
                var lastStateTypes = (from lastStateRow in lastStates
                                     join stateRow in claimStatesDataModel.FilterDeletedRows()
                                         on lastStateRow.id_state equals stateRow.Field<int?>("id_state")
                                     select new
                                     {
                                         id_claim = stateRow.Field<int>("id_claim"),
                                         id_state_type = stateRow.Field<int>("id_state_type")
                                     }).ToList();

                int? lastId = null;
                if (lastStateTypes.Any())
                    lastId = lastStateTypes.First().id_state_type;
                var localValid = lastId != null ? 
                    DataModelHelper.ClaimStateTypeIdsByPrevStateType(lastId.Value).Contains(form.IdStateType.Value) : 
                    DataModelHelper.ClaimStartStateTypeIds().Contains(form.IdStateType.Value);
                if (localValid == false)
                {
                    MessageBox.Show(string.Format(@"Невозможно произвести массовый перевод исковой работы №{0} в указанное состояние, т.к. это нарушает порядок перехода по стадиям", row["id_claim"]), 
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
            }
            toolStripProgressBarMultiOperations.Visible = true;
            toolStripProgressBarMultiOperations.Value = 0;
            toolStripProgressBarMultiOperations.Maximum = _claims.Count - 1;
            for (var i = 0; i < _claims.Count; i++)
            {
                toolStripProgressBarMultiOperations.Value = i;
                var row = (DataRowView) _claims[i];
                if (row["id_claim"] == DBNull.Value) continue;
                var claimState = new ClaimState
                {
                    IdClaim = (int)row["id_claim"],
                    IdStateType = form.IdStateType,
                    DateStartState = form.DateStartState,
                    Description = form.Description,
                    AcceptedByLegalDepartmentDate = form.AcceptedByLegalDepartmentDate,
                    AcceptedByLegalDepartmentWho = form.AcceptedByLegalDepartmentWho,
                    TransfertToLegalDepartmentDate = form.TransfertToLegalDepartmentDate,
                    TransferToLegalDepartmentWho = form.TransfertToLegalDepartmentWho
                };
                var idState = claimStatesDataModel.Insert(claimState);
                if (idState == -1)
                {
                    MessageBox.Show(@"Произошла неизвестная ошибка во время создания стадии исковых работ",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                }
                claimState.IdState = idState;
                var claimsStateRow = (DataRowView)claimStatesBindingSource.AddNew();
                if (claimsStateRow == null) break;
                claimsStateRow.BeginEdit();
                claimsStateRow["id_state"] = ViewportHelper.ValueOrDBNull(claimState.IdState);
                claimsStateRow["id_claim"] = ViewportHelper.ValueOrDBNull(claimState.IdClaim);
                claimsStateRow["id_state_type"] = ViewportHelper.ValueOrDBNull(claimState.IdStateType);
                claimsStateRow["date_start_state"] = ViewportHelper.ValueOrDBNull(claimState.DateStartState);
                claimsStateRow["description"] = ViewportHelper.ValueOrDBNull(claimState.Description);
                claimsStateRow["transfer_to_legal_department_who"] = ViewportHelper.ValueOrDBNull(claimState.TransferToLegalDepartmentWho);
                claimsStateRow["transfert_to_legal_department_date"] = ViewportHelper.ValueOrDBNull(claimState.TransfertToLegalDepartmentDate);
                claimsStateRow["accepted_by_legal_department_who"] = ViewportHelper.ValueOrDBNull(claimState.AcceptedByLegalDepartmentWho);
                claimsStateRow["accepted_by_legal_department_date"] = ViewportHelper.ValueOrDBNull(claimState.AcceptedByLegalDepartmentDate);
                claimsStateRow.EndEdit();
                Application.DoEvents();
            }
            toolStripProgressBarMultiOperations.Visible = false;
            if (toolStripProgressBarMultiOperations.Value == toolStripProgressBarMultiOperations.Maximum)
                MessageBox.Show(@"Стадия успешно добавлена", @"Сообщение", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            else
                MessageBox.Show(@"Во время добавления стадий возникли ошибки", @"Внимание", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }

        public void UpdateToolbar()
        {
            var viewport = _menuCallback.GetCurrentViewport();
            toolStripButtonClaimCurrent.Visible = false;
            toolStripButtonClaimsByFilter.Visible = false;
            if (!(viewport is ClaimListViewport)) return;
            toolStripButtonClaimCurrent.Visible = true;
            toolStripButtonClaimsByFilter.Visible = true;
            toolStripButtonCreateClaims.Enabled = AccessControl.HasPrivelege(Priveleges.ClaimsWrite);
        }

        private void toolStripButtonJudicialOrder_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < _claims.Count; i++)
            {
                var row = ((DataRowView)_claims[i]);
                var hasState =
                    (from currentRow in DataModel.GetInstance(DataModelType.ClaimStatesDataModel).FilterDeletedRows()
                        where
                            currentRow.Field<int?>("id_claim") == (int)row["id_claim"] &&
                            currentRow.Field<int?>("id_state_type") == 4
                        select currentRow).Any();
                if (hasState) continue;
                MessageBox.Show(string.Format("В претензионно-исковой работе №{0} отсутствует стадия подготовки и направления судебного приказа. Для формирования заявлений необходимо проставить стадию или убрать претензионно-исковую работу из списка мастера массовых операций", 
                    row["id_claim"]), @"Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }

            var reporter = ReporterFactory.CreateReporter(ReporterType.JudicialOrderReporter);
            var arguments = new Dictionary<string, string>();
            var filter = "";
            for (var i = 0; i < _claims.Count; i++)
            {
                var row = ((DataRowView)_claims[i]);
                if (row["id_claim"] != DBNull.Value)
                    filter += row["id_claim"] + ",";
            }
            filter = filter.TrimEnd(',');
            arguments.Add("filter", filter);
            reporter.Run(arguments);
        }
    }
}
