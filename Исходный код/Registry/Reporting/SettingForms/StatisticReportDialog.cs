using System.Windows.Forms;

namespace Registry.Reporting.SettingForms
{
    public partial class StatisticReportDialog : Form
    {
        public int? StatisticReportType { get; private set; }

        public StatisticReportDialog()
        {
            InitializeComponent();
        }

        public StatisticReportDialog(ReporterType reporterType)
        {
            InitializeComponent();
            switch (reporterType)
            {
                case ReporterType.RegistryMunicipalPremisesReporter:
                    break;
                case ReporterType.RegistryMunicipalBuildingsReporter:
                    vButtonAllObjects.Text = @"Все здания";
                    vButtonMunicipalObjects.Text = @"Муниципальные здания";
                    break;
                default:
                    throw new ReporterException(@"Неподдерживаемый тип объекта недвижимости в классе StatisticReportDialog");
            }
        }

        private void vButtonAllObjects_Click(object sender, System.EventArgs e)
        {
            StatisticReportType = 1;
            DialogResult = DialogResult.OK;
        }

        private void vButtonMunicipalObjects_Click(object sender, System.EventArgs e)
        {
            StatisticReportType = 2;
            DialogResult = DialogResult.OK;
        }
    }
}
