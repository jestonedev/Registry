using Registry.CalcDataModels;
using Registry.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels.DataModels;

namespace Registry.Reporting
{
    public partial class TenancyNotifiesSettingsForm : Form
    {
        #region Models
        private TenancyProcessesDataModel tenancies = null;
        private CalcDataModelTenancyAggregated tenancies_aggregate = null;
        private RentTypesDataModel rent_types = null;
        private ExecutorsDataModel executors = null;
        private CalcDataModelTenancyNotifiesMaxDate tenancy_notifies_max_date = null;
        private Collection<int> checked_tenancies = new Collection<int>();
        #endregion Models

        #region Views
        private BindingSource v_tenancies = null;
        private BindingSource v_tenancies_aggregate = null;
        private BindingSource v_rent_types = null;
        private BindingSource v_tenancy_notifies_max_date = null;
        private BindingSource v_executors = null;
        #endregion Views

        private string StaticFilter = "registration_num IS NOT NULL AND end_date IS NOT NULL AND registration_num NOT LIKE '%н'";

        public Collection<int> TenancyProcessIds
        {
            get
            {
                return checked_tenancies;
            }
        }
        public TenancyNotifiesReportType ReportType { get; set; }

        public int IdExecutor
        {
            get
            {
                if (comboBoxExecutor.SelectedValue != null)
                    return (int)comboBoxExecutor.SelectedValue;
                else
                    return -1;
            }
        }

        public TenancyNotifiesSettingsForm()
        {
            InitializeComponent();

            dataGridView.AutoGenerateColumns = false;
            tenancies = TenancyProcessesDataModel.GetInstance();
            rent_types = RentTypesDataModel.GetInstance();
            executors = ExecutorsDataModel.GetInstance();
            tenancies_aggregate = CalcDataModelTenancyAggregated.GetInstance();
            tenancy_notifies_max_date = CalcDataModelTenancyNotifiesMaxDate.GetInstance();

            //Ожидаем загрузки данных, если это необходимо
            tenancies.Select();
            rent_types.Select();
            executors.Select();

            DataSet ds = DataSetManager.DataSet;

            v_tenancies = new BindingSource();
            v_tenancies.DataMember = "tenancy_processes";
            v_tenancies.CurrentItemChanged += new EventHandler(v_tenancies_CurrentItemChanged);
            v_tenancies.DataSource = ds;
            IEnumerable<int> exclude_processes = DataModelHelper.OldTenancyProcesses();
            if (exclude_processes.Count() > 0)
            {
                StaticFilter += " AND id_process NOT IN (0";
                foreach (int id in exclude_processes)
                    StaticFilter += "," + id.ToString(CultureInfo.InvariantCulture);
                StaticFilter += ")";
            }   
            RebuildFilter();
            v_tenancies.Sort = "end_date DESC";
            dataGridView.Columns["end_date"].HeaderCell.SortGlyphDirection = SortOrder.Descending;

            v_rent_types = new BindingSource();
            v_rent_types.DataMember = "rent_types";
            v_rent_types.DataSource = ds;

            v_executors = new BindingSource();
            v_executors.DataMember = "executors";
            v_executors.DataSource = ds;
            v_executors.Filter = "is_inactive = 0";

            v_tenancies_aggregate = new BindingSource();
            v_tenancies_aggregate.DataSource = tenancies_aggregate.Select();

            v_tenancy_notifies_max_date = new BindingSource();
            v_tenancy_notifies_max_date.DataSource = tenancy_notifies_max_date.Select();

            tenancies.Select().RowChanged += new DataRowChangeEventHandler(TenancyListViewport_RowChanged);
            tenancies.Select().RowDeleted += new DataRowChangeEventHandler(TenancyListViewport_RowDeleted);

            comboBoxExecutor.DataSource = v_executors;
            comboBoxExecutor.DisplayMember = "executor_name";
            comboBoxExecutor.ValueMember = "id_executor";

            dataGridView.RowCount = v_tenancies.Count;
            tenancies_aggregate.RefreshEvent += new EventHandler<EventArgs>(tenancies_aggregate_RefreshEvent);
            tenancy_notifies_max_date.RefreshEvent += tenancy_notifies_max_date_RefreshEvent;

            typeof(Control).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, dataGridView, new object[] { true }, CultureInfo.InvariantCulture);
        }

        private void RebuildFilter()
        {
            string Filter = StaticFilter;
            if ((!checkBoxExpired.Checked) && (!checkBoxExpiring.Checked))
                Filter += " AND 1=0";
            else
            if (checkBoxExpired.Checked && (!checkBoxExpiring.Checked))
                Filter += String.Format(CultureInfo.InvariantCulture, " AND end_date < '{0:yyyy-MM-dd}'", DateTime.Now.Date);
            else
            if ((!checkBoxExpired.Checked) && checkBoxExpiring.Checked)
                Filter += String.Format(CultureInfo.InvariantCulture, " AND end_date >= '{0:yyyy-MM-dd}' AND end_date < '{1:yyyy-MM-dd}'", DateTime.Now.Date, DateTime.Now.Date.AddMonths(4));
            else
                Filter += String.Format(CultureInfo.InvariantCulture, " AND end_date < '{0:yyyy-MM-dd}'", DateTime.Now.Date.AddMonths(4));
            v_tenancies.Filter = Filter;
            dataGridView.RowCount = v_tenancies.Count;
            dataGridView.Refresh();
        }

        void tenancy_notifies_max_date_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        private void tenancies_aggregate_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        private void TenancyListViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.RowCount = v_tenancies.Count;
            dataGridView.Refresh();
        }

        private void TenancyListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = v_tenancies.Count;
        }

        private void v_tenancies_CurrentItemChanged(object sender, EventArgs e)
        {
            if (v_tenancies.Position == -1 || dataGridView.RowCount == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            if (v_tenancies.Position >= dataGridView.RowCount)
            {
                dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                dataGridView.Rows[v_tenancies.Position].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[v_tenancies.Position].Cells[0];
            }
        }

        private void vButtonExport_Click(object sender, EventArgs e)
        {
            if (checked_tenancies.Count == 0)
            {
                MessageBox.Show("Необходимо выбрать хотя бы один договор", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ReportType = TenancyNotifiesReportType.ExportAsIs;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void vButtonNotify_Click(object sender, EventArgs e)
        {
            if (checked_tenancies.Count == 0)
            {
                MessageBox.Show("Необходимо выбрать хотя бы один договор", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            contextMenuStripNotify.Show(vButtonNotify, 0, -48);
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_tenancies.Count <= e.RowIndex) return;
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    int id_process = Convert.ToInt32(((DataRowView)v_tenancies[e.RowIndex])["id_process"], CultureInfo.InvariantCulture);
                    e.Value = checked_tenancies.Contains(id_process);
                    break;
                case "id_process":
                    e.Value = ((DataRowView)v_tenancies[e.RowIndex])["id_process"];
                    break;
                case "registration_num":
                    e.Value = ((DataRowView)v_tenancies[e.RowIndex])["registration_num"];
                    break;
                case "registration_date":
                    DateTime registration_date;
                    if (DateTime.TryParse(((DataRowView)v_tenancies[e.RowIndex])["registration_date"].ToString(), out registration_date))
                        e.Value = registration_date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    else
                        e.Value = "";
                    break;
                case "begin_date":
                    DateTime begin_date;
                    if (DateTime.TryParse(((DataRowView)v_tenancies[e.RowIndex])["begin_date"].ToString(), out begin_date))
                        e.Value = begin_date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    else
                        e.Value = "";
                    break;
                case "end_date":
                    DateTime end_date;
                    if (DateTime.TryParse(((DataRowView)v_tenancies[e.RowIndex])["end_date"].ToString(), out end_date))
                        e.Value = end_date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    else
                        e.Value = "";
                    break;
                case "notify_date":
                    int row_index = v_tenancy_notifies_max_date.Find("id_process", ((DataRowView)v_tenancies[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                    {
                        DateTime notify_date;
                        if (DateTime.TryParse(((DataRowView)v_tenancy_notifies_max_date[row_index])["notify_date"].ToString(), out notify_date))
                            e.Value = notify_date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                        else
                            e.Value = "";
                    }
                    break;
                case "tenant":
                    row_index = v_tenancies_aggregate.Find("id_process", ((DataRowView)v_tenancies[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_tenancies_aggregate[row_index])["tenant"];
                    break;
                case "rent_type":
                    row_index = v_rent_types.Find("id_rent_type", ((DataRowView)v_tenancies[e.RowIndex])["id_rent_type"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_rent_types[row_index])["rent_type"];
                    break;
                case "address":
                    row_index = v_tenancies_aggregate.Find("id_process", ((DataRowView)v_tenancies[e.RowIndex])["id_process"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_tenancies_aggregate[row_index])["address"];
                    break;
            }
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                v_tenancies.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            if (dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                changeSortColumn(SortOrder.Descending);
            else
                changeSortColumn(SortOrder.Ascending);
            dataGridView.Refresh();
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1600)
            {
                if (dataGridView.Columns["address"].AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    dataGridView.Columns["address"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (dataGridView.Columns["address"].AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    dataGridView.Columns["address"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        private void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            int id_process = Convert.ToInt32(((DataRowView)v_tenancies[e.RowIndex])["id_process"], CultureInfo.InvariantCulture);
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if (((bool)e.Value) == true && !checked_tenancies.Contains(id_process))
                        checked_tenancies.Add(id_process);
                    else
                        if (((bool)e.Value) == false && checked_tenancies.Contains(id_process))
                            checked_tenancies.Remove(id_process);
                    if (checked_tenancies.Count == 0)
                        checkBoxCheckAll.CheckState = CheckState.Unchecked;
                    else
                    if (checked_tenancies.Count == v_tenancies.Count)
                        checkBoxCheckAll.CheckState = CheckState.Checked;
                    else
                        checkBoxCheckAll.CheckState = CheckState.Indeterminate;
                    break;
            }
        }

        private void checkBoxExpiring_CheckedChanged(object sender, EventArgs e)
        {
            checked_tenancies.Clear();
            checkBoxCheckAll.CheckState = CheckState.Unchecked;
            RebuildFilter();
        }

        private void checkBoxExpired_CheckedChanged(object sender, EventArgs e)
        {
            checked_tenancies.Clear();
            checkBoxCheckAll.CheckState = CheckState.Unchecked;
            RebuildFilter();
        }

        private void checkBoxCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCheckAll.CheckState == CheckState.Checked || checkBoxCheckAll.CheckState == CheckState.Unchecked)
            {
                checked_tenancies.Clear();
                if (checkBoxCheckAll.CheckState == CheckState.Checked)
                    for (int i = 0; i < v_tenancies.Count; i++)
                        checked_tenancies.Add(Int32.Parse(((DataRowView)v_tenancies[i])["id_process"].ToString(), CultureInfo.InvariantCulture));
                dataGridView.Refresh();
            }
        }

        private void повторноеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportType = TenancyNotifiesReportType.PrintNotifiesPrimary;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void повторноеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ReportType = TenancyNotifiesReportType.PrintNotifiesSecondary;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }

    public enum TenancyNotifiesReportType { ExportAsIs, PrintNotifiesPrimary, PrintNotifiesSecondary }
}
