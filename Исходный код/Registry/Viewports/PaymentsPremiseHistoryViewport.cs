using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities.Infrastructure;
using WeifenLuo.WinFormsUI.Docking;
using System.Drawing;

namespace Registry.Viewport
{
    internal partial class PaymentsPremiseHistoryViewport : FormWithGridViewport
    {
        private PaymentsPremiseHistoryViewport()
            : this(null, null)
        {
        }

        public PaymentsPremiseHistoryViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = DataModel.GetInstance<PaymentsPremiseHistoryDataModel>();

            var title = "История по помещению \"{0}\"";
            if (ParentType != ParentTypeEnum.PaymentAccount || ParentRow == null)
                throw new ViewportException("Неизвестный тип родительского объекта");
            title = string.Format(title, ParentRow["raw_address"]);
            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);
            GeneralBindingSource.DataSource = ((PaymentsPremiseHistoryDataModel)GeneralDataModel).Select((int)ParentRow["id_account"]);

            Text = title;

            DataBind();
            DataGridView.RowCount = GeneralBindingSource.Count;
        }

        private void DataBind()
        {
            var bindingSource = GeneralBindingSource;
            ViewportHelper.BindProperty(textBoxCRN, "Text", bindingSource, "crn", "");
            ViewportHelper.BindProperty(textBoxRawAddress, "Text", bindingSource, "raw_address", "");
            ViewportHelper.BindProperty(textBoxAddress, "Text", bindingSource, "parsed_address", "");
            ViewportHelper.BindProperty(textBoxAccount, "Text", bindingSource, "account", "");
            ViewportHelper.BindProperty(textBoxTenant, "Text", bindingSource, "tenant", "");

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
                keyValuePair.Value.DataBindings.Add("Value", bindingSource, keyValuePair.Key, true, DataSourceUpdateMode.Never, 0);
            }

            ViewportHelper.BindProperty(dateTimePickerAtDate, "Value", bindingSource, "date", DateTime.Now.Date);
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (GeneralBindingSource.Count <= e.RowIndex) return;
            var row = (DataRowView)GeneralBindingSource[e.RowIndex];
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "date":
                    e.Value = ((DateTime)row["date"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                default:
                    if (ParentType == ParentTypeEnum.PaymentAccount &&
                        (int) ParentRow["id_account"] == (int) row["id_account"])
                    {
                        DataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                        DataGridView.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.Green;
                    }
                    else
                    {
                        DataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                        DataGridView.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                    }
                    e.Value = row[DataGridView.Columns[e.ColumnIndex].Name];
                    break;
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView_SelectionChanged();
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView_ColumnHeaderMouseClick(sender, e);
        }
    }
}
