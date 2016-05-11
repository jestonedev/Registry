using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal partial class PaymentsViewport : FormWithGridViewport
    {
        private PaymentsViewport()
            : this(null, null)
        {
        }

        public PaymentsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            DockAreas = DockAreas.Document;
            dataGridView.AutoGenerateColumns = false;
            GeneralDataModel = DataModel.GetInstance<PaymentsDataModel>();

            var title = "История лицевого счета №{0}";
            if (ParentType != ParentTypeEnum.PaymentAccount || ParentRow == null)
                throw new ViewportException("Неизвестный тип родительского объекта");
            title = string.Format(title, ParentRow["account"]);
            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataSource = ((PaymentsDataModel)GeneralDataModel).Select((int)ParentRow["id_account"]);

            Text = title;

            DataBind();
            dataGridView.RowCount = GeneralBindingSource.Count;
        }

        private void DataBind()
        {
            textBoxCRN.DataBindings.Clear();
            textBoxCRN.DataBindings.Add("Text", GeneralBindingSource, "crn", true, DataSourceUpdateMode.Never, "");
            textBoxRawAddress.DataBindings.Clear();
            textBoxRawAddress.DataBindings.Add("Text", GeneralBindingSource, "raw_address", true, DataSourceUpdateMode.Never, "");
            textBoxAddress.DataBindings.Clear();
            textBoxAddress.DataBindings.Add("Text", GeneralBindingSource, "parsed_address", true, DataSourceUpdateMode.Never, "");
            textBoxAccount.DataBindings.Clear();
            textBoxAccount.DataBindings.Add("Text", GeneralBindingSource, "account", true, DataSourceUpdateMode.Never, "");
            textBoxTenant.DataBindings.Clear();
            textBoxTenant.DataBindings.Add("Text", GeneralBindingSource, "tenant", true, DataSourceUpdateMode.Never, "");

            foreach (var keyValuePair in new Dictionary<string, NumericUpDown>
            {
                {"total_area",numericUpDownTotalArea},
                {"living_area",numericUpDownLivingArea},
                {"prescribed",numericUpDownPrescribed},
                {"balance_input",numericUpDownBalanceTotalInput},
                {"balance_tenancy",numericUpDownBalanceTenancyInput},
                {"balance_dgi",numericUpDownBalanceDGIInput},
                {"balance_input_penalties",numericUpDownPenaltiesInput},
                {"charging_total",numericUpDownChargingTotal},
                {"charging_tenancy",numericUpDownChargingTenancy},
                {"charging_dgi",numericUpDownChargingDGI},
                {"charging_penalties",numericUpDownChargingPenalties},
                {"recalc_tenancy",numericUpDownRecalcTenancy},
                {"recalc_dgi",numericUpDownRecalcDGI},
                {"recalc_penalties",numericUpDownRecalcPenalties},
                {"payment_tenancy",numericUpDownPaymentTenancy},
                {"payment_dgi",numericUpDownPaymentDGI},
                {"payment_penalties",numericUpDownPaymentPenalties},
                {"transfer_balance",numericUpDownTransferBalance},
                {"balance_output_total",numericUpDownBalanceTotalOutput},
                {"balance_output_tenancy",numericUpDownBalanceTenancyOutput},
                {"balance_output_dgi",numericUpDownBalanceDGIOutput},
                {"balance_output_penalties",numericUpDownPenaltiesOutput}
            })
            {
                keyValuePair.Value.DataBindings.Clear();
                keyValuePair.Value.DataBindings.Add("Minimum", GeneralBindingSource, keyValuePair.Key, true, DataSourceUpdateMode.Never, 0);
                keyValuePair.Value.DataBindings.Add("Maximum", GeneralBindingSource, keyValuePair.Key, true, DataSourceUpdateMode.Never, 0);
                keyValuePair.Value.DataBindings.Add("Value", GeneralBindingSource, keyValuePair.Key, true, DataSourceUpdateMode.Never, 0);
            }
            
            dateTimePickerAtDate.DataBindings.Clear();
            dateTimePickerAtDate.DataBindings.Add("Value", GeneralBindingSource, "date", true, DataSourceUpdateMode.Never, 0);
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "date":
                    e.Value = ((DateTime)((DataRowView)GeneralBindingSource[e.RowIndex])["date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                default:
                    e.Value = ((DataRowView)GeneralBindingSource[e.RowIndex])[dataGridView.Columns[e.ColumnIndex].Name];
                    break;
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                GeneralBindingSource.Position = dataGridView.SelectedRows[0].Index;
            else
                GeneralBindingSource.Position = -1;
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = way =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                GeneralBindingSource.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection ==
                             SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (GeneralBindingSource.Position == -1 || dataGridView.RowCount == 0)
                DataGridView.ClearSelection();
            else
                if (GeneralBindingSource.Position >= DataGridView.RowCount)
                {
                    DataGridView.Rows[DataGridView.RowCount - 1].Selected = true;
                    DataGridView.CurrentCell = DataGridView.CurrentCell != null ?
                        DataGridView.Rows[DataGridView.RowCount - 1].Cells[DataGridView.CurrentCell.ColumnIndex] :
                        DataGridView.Rows[DataGridView.RowCount - 1].Cells[0];
                }
                else
                {
                    DataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                    DataGridView.CurrentCell = DataGridView.CurrentCell != null ?
                        DataGridView.Rows[GeneralBindingSource.Position].Cells[DataGridView.CurrentCell.ColumnIndex] :
                        DataGridView.Rows[GeneralBindingSource.Position].Cells[0];
                }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.EditingStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (GeneralBindingSource != null)
                GeneralBindingSource.CurrentItemChanged -= GeneralBindingSource_CurrentItemChanged;
            base.OnClosing(e);
        }
    }
}
