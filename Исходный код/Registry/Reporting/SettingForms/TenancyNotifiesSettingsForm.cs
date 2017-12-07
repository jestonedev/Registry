using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Settings;

namespace Registry.Reporting.SettingForms
{
    public partial class TenancyNotifiesSettingsForm : Form
    {
        #region Models
        private readonly Collection<int> _checkedTenancies = new Collection<int>();
        #endregion Models

        #region Views
        private BindingSource _vTenancies;
        private BindingSource _vTenanciesAggregate;
        private BindingSource _vRentTypes;
        private BindingSource _vTenancyNotifiesMaxDate;
        #endregion Views

        private string _staticFilter = "(registration_num IS NULL OR registration_num NOT LIKE '%н')";
        private Action<TenancyNotifiesSettingsForm> _callback;
        public Collection<int> TenancyProcessIds
        {
            get
            {
                return _checkedTenancies;
            }
        }
        public TenancyNotifiesReportType ReportType { get; set; }

        public int IdExecutor
        {
            get
            {
                if (comboBoxExecutor.SelectedValue != null)
                    return (int)comboBoxExecutor.SelectedValue;
                return -1;
            }
        }

        public TenancyNotifiesSettingsForm()
        {
            InitializeComponent();

            dataGridView.AutoGenerateColumns = false;
        }

        public void InitializeData(Action<TenancyNotifiesSettingsForm> callback)
        {
            DataModel tenancies = EntityDataModel<TenancyProcess>.GetInstance();
            var rentTypes = DataModel.GetInstance<RentTypesDataModel>();
            var executors = DataModel.GetInstance<EntityDataModel<Executor>>();
            var tenanciesAggregate = CalcDataModel.GetInstance<CalcDataModelTenancyAggregated>();
            var tenancyNotifiesMaxDate = CalcDataModel.GetInstance<CalcDataModelTenancyNotifiesMaxDate>();

            //Ожидаем загрузки данных, если это необходимо
            tenancies.Select();
            rentTypes.Select();
            executors.Select();

            var ds = DataStorage.DataSet;

            _vTenancies = new BindingSource { DataMember = "tenancy_processes" };
            _vTenancies.CurrentItemChanged += v_tenancies_CurrentItemChanged;
            _vTenancies.DataSource = ds;
            var excludeProcesses = TenancyService.OldTenancyProcesses().ToList();
            if (excludeProcesses.Any())
            {
                _staticFilter += " AND id_process NOT IN (0";
                foreach (var id in excludeProcesses)
                    _staticFilter += "," + id.ToString(CultureInfo.InvariantCulture);
                _staticFilter += ") ";
            }
            RebuildFilter();
            _vTenancies.Sort = "end_date DESC";
            end_date.HeaderCell.SortGlyphDirection = SortOrder.Descending;

            _vRentTypes = new BindingSource
            {
                DataMember = "rent_types",
                DataSource = ds
            };

            var vExecutors = new BindingSource
            {
                DataMember = "executors",
                DataSource = ds,
                Filter = "is_inactive = 0"
            };

            _vTenanciesAggregate = new BindingSource { DataSource = tenanciesAggregate.Select() };

            _vTenancyNotifiesMaxDate = new BindingSource { DataSource = tenancyNotifiesMaxDate.Select() };

            tenancies.Select().RowChanged += TenancyListViewport_RowChanged;
            tenancies.Select().RowDeleted += TenancyListViewport_RowDeleted;

            comboBoxExecutor.DataSource = vExecutors;
            comboBoxExecutor.DisplayMember = "executor_name";
            comboBoxExecutor.ValueMember = "id_executor";
            var login = UserDomain.Current.sAMAccountName;
            var idExecutor = (from row in executors.FilterDeletedRows()
                              where
                                 row.Field<string>("executor_login") != null &&
                                 row.Field<string>("executor_login").ToUpperInvariant() ==
                                     "PWR\\" + login.ToUpperInvariant()
                              select row.Field<int?>("id_executor")).FirstOrDefault();
            if (idExecutor != null)
            {
                comboBoxExecutor.SelectedValue = idExecutor;
            }

            dataGridView.RowCount = _vTenancies.Count;
            tenanciesAggregate.RefreshEvent += tenancies_aggregate_RefreshEvent;
            tenancyNotifiesMaxDate.RefreshEvent += tenancy_notifies_max_date_RefreshEvent;

            typeof(Control).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, dataGridView, new object[] { true }, CultureInfo.InvariantCulture);

            _callback = callback;
        }

        private void RebuildFilter()
        {
            var filter = "";
            if (checkBoxExpired.Checked || checkBoxExpiring.Checked || 
                checkBoxWithoutRegNum.Checked || checkBoxProlongContracts.Checked)
            {
                if(checkBoxExpired.Checked && !checkBoxExpiring.Checked)
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        filter += " AND ";
                    }
                    filter += "registration_num IS NOT NULL AND end_date IS NOT NULL AND " + 
                        string.Format(CultureInfo.InvariantCulture, "end_date < '{0:yyyy-MM-dd}'", 
                        DateTime.Now.Date);
                }
                if (!checkBoxExpired.Checked && checkBoxExpiring.Checked)
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        filter += " AND ";
                    }
                    filter += "registration_num IS NOT NULL AND end_date IS NOT NULL AND " + 
                        string.Format(CultureInfo.InvariantCulture, "end_date >= '{0:yyyy-MM-dd}' AND end_date < '{1:yyyy-MM-dd}'", 
                        DateTime.Now.Date, DateTime.Now.Date.AddMonths(4));
                }
                if (checkBoxExpired.Checked && checkBoxExpiring.Checked)
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        filter += " AND ";
                    }
                    filter += "registration_num IS NOT NULL AND end_date IS NOT NULL AND " + 
                        string.Format(CultureInfo.InvariantCulture, "end_date < '{0:yyyy-MM-dd}'", 
                        DateTime.Now.Date.AddMonths(4));
                }
                if(checkBoxWithoutRegNum.Checked)
                {
                    if (!string.IsNullOrEmpty(filter))
                        filter += " OR ";
                    filter += "registration_num IS NULL";
                }
                if (checkBoxProlongContracts.Checked)
                {
                    var prolongContractsIds =
                        from row in EntityDataModel<TenancyRentPeriod>.GetInstance().FilterDeletedRows()
                        where row.Field<int?>("id_process") != null
                        select row.Field<int>("id_process");
                    if (!string.IsNullOrEmpty(filter))
                        filter += " OR ";
                    filter += "id_process IN(0" + prolongContractsIds.Select(v => v.ToString()).
                        Aggregate((v, acc) => v + "," + acc)+")";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(filter))
                {
                    filter += " AND ";
                }
                filter += "1=0";
            }
            _vTenancies.Filter = string.Format("({0}) AND ({1})", filter, _staticFilter);
            dataGridView.RowCount = _vTenancies.Count;
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
            dataGridView.RowCount = _vTenancies.Count;
            dataGridView.Refresh();
        }

        private void TenancyListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = _vTenancies.Count;
        }

        private void v_tenancies_CurrentItemChanged(object sender, EventArgs e)
        {
            if (_vTenancies.Position == -1 || dataGridView.RowCount == 0)
            {
                dataGridView.ClearSelection();
                return;
            }
            if (_vTenancies.Position >= dataGridView.RowCount)
            {
                dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                dataGridView.Rows[_vTenancies.Position].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[_vTenancies.Position].Cells[0];
            }
        }

        private void vButtonExport_Click(object sender, EventArgs e)
        {
            if (_checkedTenancies.Count == 0)
            {
                MessageBox.Show(@"Необходимо выбрать хотя бы один договор", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ReportType = TenancyNotifiesReportType.ExportAsIs;
            DialogResult = DialogResult.OK;
            if (_callback != null)
            {
                _callback(this);
            }
        }

        private void vButtonNotify_Click(object sender, EventArgs e)
        {
            if (_checkedTenancies.Count == 0)
            {
                MessageBox.Show(@"Необходимо выбрать хотя бы один договор", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            contextMenuStripNotify.Show(vButtonNotify, 0, -48);
        }

        private readonly IEnumerable<int> _tenanciesWithEmergencyEstates =
            TenancyService.TenancyProcessIDsWithEmergencyEstates().ToList();

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_vTenancies.Count <= e.RowIndex) return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    var idProcess = Convert.ToInt32(((DataRowView)_vTenancies[e.RowIndex])["id_process"], CultureInfo.InvariantCulture);
                    e.Value = _checkedTenancies.Contains(idProcess);
                    if (_tenanciesWithEmergencyEstates.Contains(idProcess))
                    {
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(255, 187, 254, 232);
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.FromArgb(255, 72, 215, 143);
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Присутствует реквизит \"Аварийное\"";
                    }
                    else
                    {
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = SystemColors.Highlight;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "";
                    }
                    break;
                case "id_process":
                    e.Value = ((DataRowView)_vTenancies[e.RowIndex])["id_process"];
                    break;
                case "registration_num":
                    e.Value = ((DataRowView)_vTenancies[e.RowIndex])["registration_num"];
                    break;
                case "registration_date":
                    DateTime registrationDate;
                    e.Value = DateTime.TryParse(((DataRowView)_vTenancies[e.RowIndex])["registration_date"].ToString(), out registrationDate) ? 
                        registrationDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
                    break;
                case "begin_date":
                    DateTime beginDate;
                    e.Value = DateTime.TryParse(((DataRowView)_vTenancies[e.RowIndex])["begin_date"].ToString(), out beginDate) ? 
                        beginDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
                    break;
                case "end_date":
                    DateTime endDate;
                    e.Value = DateTime.TryParse(((DataRowView) _vTenancies[e.RowIndex])["end_date"].ToString(), out endDate) ? 
                        endDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
                    break;
                case "notify_date":
                    var rowIndex = _vTenancyNotifiesMaxDate.Find("id_process", ((DataRowView)_vTenancies[e.RowIndex])["id_process"]);
                    if (rowIndex != -1)
                    {
                        DateTime notifyDate;
                        e.Value = DateTime.TryParse(((DataRowView)_vTenancyNotifiesMaxDate[rowIndex])["notify_date"].ToString(), out notifyDate) ? 
                            notifyDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : "";
                    }
                    break;
                case "tenant":
                    rowIndex = _vTenanciesAggregate.Find("id_process", ((DataRowView)_vTenancies[e.RowIndex])["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vTenanciesAggregate[rowIndex])["tenant"];
                    break;
                case "id_rent_type":
                    rowIndex = _vRentTypes.Find("id_rent_type", ((DataRowView)_vTenancies[e.RowIndex])["id_rent_type"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vRentTypes[rowIndex])["rent_type"];
                    break;
                case "address":
                    rowIndex = _vTenanciesAggregate.Find("id_process", ((DataRowView)_vTenancies[e.RowIndex])["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vTenanciesAggregate[rowIndex])["address"];
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
                _vTenancies.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + (way == SortOrder.Ascending ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (dataGridView.Size.Width > 1600)
            {
                if (address.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    address.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                if (address.AutoSizeMode != DataGridViewAutoSizeColumnMode.None)
                    address.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        private void dataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            var idProcess = Convert.ToInt32(((DataRowView)_vTenancies[e.RowIndex])["id_process"], CultureInfo.InvariantCulture);
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "is_checked":
                    if ((bool)e.Value && !_checkedTenancies.Contains(idProcess))
                        _checkedTenancies.Add(idProcess);
                    else
                        if (((bool)e.Value) == false && _checkedTenancies.Contains(idProcess))
                            _checkedTenancies.Remove(idProcess);
                    if (_checkedTenancies.Count == 0)
                        checkBoxCheckAll.CheckState = CheckState.Unchecked;
                    else
                    if (_checkedTenancies.Count == _vTenancies.Count)
                        checkBoxCheckAll.CheckState = CheckState.Checked;
                    else
                        checkBoxCheckAll.CheckState = CheckState.Indeterminate;
                    break;
            }
        }

        private void checkBoxExpiring_CheckedChanged(object sender, EventArgs e)
        {
            _checkedTenancies.Clear();
            checkBoxCheckAll.CheckState = CheckState.Unchecked;
            RebuildFilter();
        }

        private void checkBoxExpired_CheckedChanged(object sender, EventArgs e)
        {
            _checkedTenancies.Clear();
            checkBoxCheckAll.CheckState = CheckState.Unchecked;
            RebuildFilter();
        }

        private void checkBoxWithoutRegNum_CheckedChanged(object sender, EventArgs e)
        {
            _checkedTenancies.Clear();
            checkBoxCheckAll.CheckState = CheckState.Unchecked;
            RebuildFilter();
        }

        private void checkBoxProlongContracts_CheckedChanged(object sender, EventArgs e)
        {
            _checkedTenancies.Clear();
            checkBoxCheckAll.CheckState = CheckState.Unchecked;
            RebuildFilter();
        }

        private void checkBoxCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCheckAll.CheckState != CheckState.Checked && checkBoxCheckAll.CheckState != CheckState.Unchecked)
                return;
            _checkedTenancies.Clear();
            if (checkBoxCheckAll.CheckState == CheckState.Checked)
                for (var i = 0; i < _vTenancies.Count; i++)
                    _checkedTenancies.Add(int.Parse(((DataRowView)_vTenancies[i])["id_process"].ToString(), CultureInfo.InvariantCulture));
            dataGridView.Refresh();
        }

        private void повторноеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportType = TenancyNotifiesReportType.PrintNotifiesPrimary;
            DialogResult = DialogResult.OK;
            if (_callback != null)
            {
                _callback(this);
            }
        }

        private void повторноеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ReportType = TenancyNotifiesReportType.PrintNotifiesSecondary;
            DialogResult = DialogResult.OK;
            if (_callback != null)
            {
                _callback(this);
            }
        }

        private void ответНаОбращениеПоПродлениюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportType = TenancyNotifiesReportType.PrintNotifiesProlongContract;
            DialogResult = DialogResult.OK;
            if (_callback != null)
            {
                _callback(this);
            }
        }

        private void vButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            if (_callback != null)
            {
                _callback(this);
            }
        }

        private void TenancyNotifiesSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            if (_callback != null)
            {
                _callback(this);
            }
        }
    }

    public enum TenancyNotifiesReportType { ExportAsIs, PrintNotifiesPrimary, PrintNotifiesSecondary, PrintNotifiesProlongContract }
}
