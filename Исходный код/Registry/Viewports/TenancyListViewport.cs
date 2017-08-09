using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.Services;
using Registry.Entities.Infrastructure;
using Registry.Reporting;
using Registry.Viewport.Presenters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class TenancyListViewport: DataGridViewport
    {

        private TenancyListViewport()
            : this(null, null)
        {
        }

        public TenancyListViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new TenancyListPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            ViewportHelper.SetDoubleBuffered(DataGridView);
        }

        private void SetViewportCaption()
        {
            if (ParentRow == null)
                Text = @"Процессы найма жилья";
            else
            {
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        Text = string.Format(CultureInfo.InvariantCulture, "Найм здания №{0}", ParentRow["id_building"]);
                        break;
                    case ParentTypeEnum.Premises:
                        Text = string.Format(CultureInfo.InvariantCulture, "Найм помещения №{0}", ParentRow["id_premises"]);
                        break;
                    case ParentTypeEnum.SubPremises:
                        Text = string.Format(CultureInfo.InvariantCulture, "Найм комнаты №{0}", ParentRow["id_sub_premises"]);
                        break;
                    default: throw new ViewportException("Неизвестный тип родительского объекта");
                }
            }
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;

            Presenter.ParentRow = ParentRow;
            Presenter.ParentType = ParentType;

            ((TenancyListPresenter)Presenter).AddAssocViewModelItem();
            StaticFilter = ((TenancyListPresenter) Presenter).GetStaticFilter();
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);

            SetViewportCaption();

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", GeneralDataSource_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", GeneralDataSource_RowDeleted);

            if (ParentRow != null)
            {
                AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowChanged",
                    TenancyAssocViewport_RowChanged);
                AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowDeleted",
                    TenancyAssocViewport_RowDeleted);
            }

            AddEventHandler<EventArgs>(Presenter.ViewModel["tenancy_aggregated"].Model, "RefreshEvent", tenancies_aggregate_RefreshEvent);
            AddEventHandler<EventArgs>(Presenter.ViewModel["tenancy_payments_info"].Model, "RefreshEvent", tenancy_payments_info_RefreshEvent);

            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count; 
            
            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());
        }

        private void tenancy_payments_info_RefreshEvent(object sender, EventArgs e)
        {
            DataGridView.Refresh();
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить этот процесс найма жилья?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            if (((TenancyListPresenter)Presenter).DeleteRecord())
            {
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool CanOpenDetails()
        {
            return Presenter.ViewModel["general"].CurrentRow != null;
        }

        public override void OpenDetails()
        {
            var viewport = new TenancyViewport(null, MenuCallback)
            {
                StaticFilter = StaticFilter,
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (Presenter.ViewModel["general"].BindingSource.Count > 0)
                viewport.LocateEntityBy(Presenter.ViewModel["general"].PrimaryKeyFirst,
                    Presenter.ViewModel["general"].CurrentRow[Presenter.ViewModel["general"].PrimaryKeyFirst] as int? ?? -1);
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            return !Presenter.ViewModel["general"].Model.EditingNewRecord && AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void InsertRecord()
        {
            var viewport = new TenancyViewport(null, MenuCallback)
            {
                StaticFilter = StaticFilter,
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            viewport.InsertRecord();
            MenuCallback.AddViewport(viewport);
        }

        public override bool CanCopyRecord()
        {
            return Presenter.ViewModel["general"].CurrentRow != null && !Presenter.ViewModel["general"].Model.EditingNewRecord && 
                AccessControl.HasPrivelege(Priveleges.TenancyWrite);
        }

        public override void CopyRecord()
        {
            var viewport = new TenancyViewport(null, MenuCallback)
            {
                StaticFilter = StaticFilter,
                DynamicFilter = DynamicFilter,
                ParentRow = ParentRow,
                ParentType = ParentType
            };
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            var viewModel = Presenter.ViewModel["general"];
            if (viewModel.CurrentRow != null)
            {
                viewport.LocateEntityBy(viewModel.PrimaryKeyFirst, viewModel.CurrentRow[viewModel.PrimaryKeyFirst] as int? ?? -1);
            }
            MenuCallback.AddViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool HasAssocViewport<T>()
        {
            var reports = new List<ViewportType>
            {
                ViewportType.TenancyPersonsViewport,
                ViewportType.TenancyReasonsViewport,
                ViewportType.TenancyBuildingsViewport,
                ViewportType.TenancyPremisesViewport,
                ViewportType.TenancyAgreementsViewport
            };
            return reports.Any(v => v.ToString() == typeof(T).Name) && Presenter.ViewModel["general"].CurrentRow != null;
        }

        public override void ShowAssocViewport<T>()
        {
            var viewModel = Presenter.ViewModel["general"];
            if (viewModel.CurrentRow == null)
            {
                MessageBox.Show(@"Не выбран процесс найма", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport<T>(MenuCallback, viewModel.PrimaryKeyFirst + " = " +
                Convert.ToInt32(viewModel.CurrentRow[viewModel.PrimaryKeyFirst], CultureInfo.InvariantCulture),
                viewModel.CurrentRow.Row, ParentTypeEnum.Tenancy);
        }

        public override bool HasReport(ReporterType reporterType)
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
                return false;
            var idProcess = ViewportHelper.ValueOrNull<int>(row, "id_process");
            var idRentType = ViewportHelper.ValueOrNull<int>(row, "id_rent_type");
            switch (reporterType)
            {
                case ReporterType.ExportReporter:
                    return true;
                case ReporterType.TenancyContractCommercialReporter:
                    return idProcess != null && idRentType == 1;
                case ReporterType.TenancyContractSocialReporter:
                    return idProcess != null && idRentType == 3;
                case ReporterType.TenancyContractSpecial1711Reporter:
                case ReporterType.TenancyContractSpecial1712Reporter:
                    return idProcess != null && idRentType == 2;
                case ReporterType.TenancyActToEmploymentReporter:
                case ReporterType.TenancyActFromEmploymentReporter:
                case ReporterType.TenancyNotifyDocumentsPrepared:
                case ReporterType.TenancyNotifyIllegalResident:
                case ReporterType.TenancyNotifyNoProlongTrouble:
                case ReporterType.TenancyNotifyNoProlongCategory:
                case ReporterType.TenancyNotifyEvictionTrouble:
                case ReporterType.TenancyNotifyContractViolation:
                case ReporterType.RequestToMvdReporter:
                case ReporterType.DistrictCommitteePreContractReporter:
                    return idProcess != null;
                case ReporterType.TenancyAgreementReporter:
                    return idProcess != null && (TenancyService.TenancyAgreementsCountForProcess(idProcess.Value) > 0);
            }
            return false;
        }

        public override void GenerateReport(ReporterType reporterType)
        {
            if (!TenancyValidForReportGenerate(reporterType))
                return;
            var arguments = new Dictionary<string, string>();
            switch (reporterType)
            {
                case ReporterType.ExportReporter:
                    arguments = ExportReporterArguments();
                    break;
                case ReporterType.TenancyContractCommercialReporter:
                case ReporterType.TenancyContractSocialReporter:
                case ReporterType.TenancyContractSpecial1711Reporter:
                case ReporterType.TenancyContractSpecial1712Reporter:
                case ReporterType.DistrictCommitteePreContractReporter:
                case ReporterType.RequestToMvdReporter:
                    arguments = TenancyContractReporterArguments();
                    break;
                case ReporterType.TenancyNotifyDocumentsPrepared:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "1");
                    break;
                case ReporterType.TenancyNotifyContractViolation:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "3");  // Report type 2 used into TenancyAgreementsViewport
                    break;
                case ReporterType.TenancyNotifyIllegalResident:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "4");
                    break;
                case ReporterType.TenancyNotifyNoProlongTrouble:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "5");
                    break;
                case ReporterType.TenancyNotifyNoProlongCategory:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "6");
                    break;
                case ReporterType.TenancyNotifyEvictionTrouble:
                    arguments = TenancyContractReporterArguments();
                    arguments.Add("report_type", "7");
                    break;
                case ReporterType.TenancyActToEmploymentReporter:
                case ReporterType.TenancyActFromEmploymentReporter:
                    arguments = TenancyActReporterArguments();
                    break;
                case ReporterType.TenancyAgreementReporter:
                    arguments = new Dictionary<string, string> {
                        {
                            "id_agreement", 
                            ((TenancyListPresenter)Presenter).TenancyAgreementLastIdForProcess().ToString()
                        }};
                    break;
            }
            MenuCallback.RunReport(reporterType, arguments);
        }

        private Dictionary<string, string> ExportReporterArguments()
        {
            var columnHeaders = DataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnHeader\":\"" + column.HeaderText + "\"}");
            var columnPatterns = DataGridView.Columns.Cast<DataGridViewColumn>().
                Aggregate("", (current, column) => current + (current == "" ? "" : ",") + "{\"columnPattern\":\"$column" + column.DisplayIndex + "$\"}");
            var arguments = new Dictionary<string, string>
            {
                {"type", "3"},
                {"filter", Presenter.ViewModel["general"].BindingSource.Filter.Trim() == "" ? "(1=1)" : Presenter.ViewModel["general"].BindingSource.Filter},
                {"columnHeaders", "["+columnHeaders+",{\"columnHeader\":\"Дополнительные сведения\"}]"},
                {"columnPatterns", "["+columnPatterns+",{\"columnPattern\":\"$description$\"}]"}
            };
            return arguments;
        }

        private Dictionary<string, string> TenancyContractReporterArguments()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return new Dictionary<string, string> {{"id_process", row["id_process"].ToString()}}; 
        }

        private Dictionary<string, string> TenancyActReporterArguments()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return new Dictionary<string, string> { { "id_process", row["id_process"].ToString() } }; 
        }

        private bool TenancyValidForReportGenerate(ReporterType reporterType)
        {
            if (reporterType == ReporterType.ExportReporter)
            {
                return true;
            }
            var row = Presenter.ViewModel["general"].CurrentRow;
            if (row == null)
                return false;
            if (reporterType == ReporterType.RequestToMvdReporter ||
                reporterType == ReporterType.TenancyNotifyIllegalResident ||
                reporterType == ReporterType.DistrictCommitteePreContractReporter)
            {
                return true;
            }
            //Проверить наличие нанимателя (и только одного) и наличия номера и даты договора найма
            if (!TenancyService.TenancyProcessHasTenant(Convert.ToInt32(row["id_process"], CultureInfo.InvariantCulture)))
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо указать нанимателя процесса найма",@"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewportHelper.ValueOrNull<DateTime>(row, "registration_date") == null || ViewportHelper.ValueOrNull(row, "registration_num") == null)
            {
                MessageBox.Show(@"Для формирования отчетной документации необходимо завести договор найма и указать его номер и дату регистрации",@"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (CanOpenDetails())
                OpenDetails();
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView_SelectionChanged();
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            if (bindingSource.Count <= e.RowIndex) return;
            var row = ((DataRowView)bindingSource[e.RowIndex]);
            switch (DataGridView.Columns[e.ColumnIndex].Name)
            {
                case "id_process":
                    e.Value = row["id_process"];
                    break;
                case "registration_num":
                    e.Value = row["registration_num"];
                    break;
                case "registration_date":
                    var date = row["registration_date"];
                    e.Value = date is DateTime ? ((DateTime)date).ToString("dd.MM.yyyy") : null;
                    break;
                case "end_date":
                    var date2 = row["end_date"];
                    e.Value = date2 is DateTime ? ((DateTime)date2).ToString("dd.MM.yyyy") : null;
                    break;
                case "residence_warrant_num":
                    e.Value = row["residence_warrant_num"];
                    break;
                case "tenant":
                    var rowIndex = Presenter.ViewModel["tenancy_aggregated"].BindingSource.Find("id_process", row["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)Presenter.ViewModel["tenancy_aggregated"].BindingSource[rowIndex])["tenant"];
                    break;
                case "rent_type":
                    rowIndex = Presenter.ViewModel["rent_types"].BindingSource.Find("id_rent_type", row["id_rent_type"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)Presenter.ViewModel["rent_types"].BindingSource[rowIndex])["rent_type"];
                    break;
                case "address":
                    rowIndex = Presenter.ViewModel["tenancy_aggregated"].BindingSource.Find("id_process", row["id_process"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)Presenter.ViewModel["tenancy_aggregated"].BindingSource[rowIndex])["address"];
                    break;
                case "payment":
                    var paymentRows =
                        from paymentRow in Presenter.ViewModel["tenancy_payments_info"].Model.FilterDeletedRows()
                        where paymentRow.Field<int?>("id_process") == (int?) row["id_process"]
                        select paymentRow.Field<decimal>("payment");
                    e.Value = paymentRows.Sum(r => r);
                    break;
            }
        }

        private void tenancies_aggregate_RefreshEvent(object sender, EventArgs e)
        {
            DataGridView.Refresh();
        }

        private void TenancyAssocViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            StaticFilter = ((TenancyListPresenter) Presenter).GetStaticFilter();
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
        }

        private void TenancyAssocViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            StaticFilter = ((TenancyListPresenter)Presenter).GetStaticFilter();
            Presenter.SetGeneralBindingSourceFilter(StaticFilter, DynamicFilter);
            DataGridView.RowCount = Presenter.ViewModel["general"].BindingSource.Count;
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            if (DataGridView.Size.Width > 1360)
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

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView_ColumnHeaderMouseClick(sender, e);
        }
    }
}
