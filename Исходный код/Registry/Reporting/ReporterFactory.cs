using System;
using Registry.Reporting.RegistryReporters;
using Registry.Reporting.TenancyReporters;
using System.Globalization;
using Registry.Reporting.ClaimsAndPaymentsReporters;
using Registry.Reporting.ResettleReporters;

namespace Registry.Reporting
{
    public static class ReporterFactory
    {
        public static Reporter CreateReporter(ReporterType reporterType)
        {
            switch (reporterType)
            {
                case ReporterType.RegistryShortStatisticReporter:
                    return new ShortStatisticReporter();
                case ReporterType.RegistryFullStatisticReporter:
                    return new FullStatisticReporter();
                case ReporterType.RegistryOwnershipsReporter:
                    return new OwnershipsReporter();
                case ReporterType.RegistryCommercialFundReporter:
                    return new CommercialFundReporter();
                case ReporterType.RegistrySpecialFundReporter:
                    return new SpecialFundReporter();
                case ReporterType.RegistrySocialFundReporter:
                    return new SocialFundReporter();
                case ReporterType.RegistryPremisesForOrphansReporter:
                    return new PremisesForOrphansReporter();
                case ReporterType.RegistryPremisesByExchangeReporter:
                    return new PremisesByExchangeReporter();
                case ReporterType.RegistryPremisesByDonationReporter:
                    return new PremisesByDonationReporter();
                case ReporterType.RegistryAllPremisesReporter:
                    return new AllPremisesReporter();
                case ReporterType.RegistryExcerptReporterPremise:
                case ReporterType.RegistryExcerptReporterSubPremise:
                case ReporterType.RegistryExcerptReporterAllMunSubPremises:
                    return new ExcerptReporter();
                case ReporterType.RegistryExcerptReporterBuilding:
                    return new BuildingExcerptReporter();
                case ReporterType.MultiExcerptReporter:
                    return new MultiExcerptReporter();
                case ReporterType.ClaimsStatisticReporter:
                    return new ClaimsStatisticReporter();
                case ReporterType.ClaimsStatesReporter:
                    return new ClaimsStatesReporter();
                case ReporterType.TenancyContractCommercialReporter:
                    return new TenancyContractCommercialReporter();
                case ReporterType.TenancyContractSocialReporter:
                    return new TenancyContractSocialReporter();
                case ReporterType.TenancyContractSpecial1711Reporter:
                    return new TenancyContractSpecial1711Reporter();
                case ReporterType.TenancyContractSpecial1712Reporter:
                    return new TenancyContractSpecial1712Reporter();
                case ReporterType.TenancyActToEmploymentReporter:
                    return new TenancyActEmploymentReporter(ActEmploymentType.ToEmployment);
                case ReporterType.TenancyActFromEmploymentReporter:
                    return new TenancyActEmploymentReporter(ActEmploymentType.FromEmployment);
                case ReporterType.TenancyAgreementReporter:
                    return new TenancyAgreementReporter();
                case ReporterType.TenancyStatisticReporter:
                    return new TenancyStatisticReporter();
                case ReporterType.TenancyStatisticForCoMSReporter:
                    return new TenancyStatisticForCoMSReporter();
                case ReporterType.TenancyOrderReporter:
                    return new TenancyOrderReporter();
                case ReporterType.TenancyNotifiesReporter:
                    return new TenancyNotifiesReporter();
                case ReporterType.ResettleBuildingDemolishingReporter:
                    return new ResettleBuildingDemolishingReporter();
                case ReporterType.ResettleEmergencyBuildingsReporter:
                    return new ResettleEmergencyBuildingsReporter();
                case ReporterType.ResettleFullProcessingReporter:
                    return new ResettleFullProcessingReporter();
                case ReporterType.ResettleShortProcessingReporter:
                    return new ResettleShortProcessingReporter();
                case ReporterType.ResettleTotalStatisticReporter:
                    return new ResettleTotalStatisticReporter();
                case ReporterType.ExportReporter:
                    return new ExportReporter();
                case ReporterType.RequestToBksReporter:
                    return new RequestToBksReporter();
                case ReporterType.JudicialOrderReporter:
                    return new JudicialOrderReporter();
                case ReporterType.TransferToLegalDepartmentReporter:
                    return new TransfertToLegalDepartmentReporter();
                case ReporterType.TenancyNotifyDocumentsPrepared:
                case ReporterType.TenancyNotifyIllegalResident:
                case ReporterType.TenancyNotifyNoProlongTrouble:
                case ReporterType.TenancyNotifyNoProlongCategory:
                case ReporterType.TenancyNotifyEvictionTrouble:
                case ReporterType.TenancyNotifyContractViolation:
                    return new TenancyNotifySingleDocumentReporter();
                case ReporterType.RegistryMunicipalPremisesCurrentFundsReporter:
                    return new MunicipalPremisesCurrentFundsReporter();
                case ReporterType.RegistryMunicipalPremisesReporter:
                    return new MunicipalPremisesReporter();
                case ReporterType.RegistryMunicipalBuildingsReporter:
                    return new MunicipalBuildingsReporter();
                case ReporterType.AccountsDuplicateStatistic:
                    return new AccountsDuplicateStatistic();
                case ReporterType.ClaimsCourtOrderPrepareReporter:
                    return new ClaimsCourtOrderPrepareReporter();
                case ReporterType.GisZkhReporter:
                    return new GisZkhExport();
            }
            throw new ReporterException(String.Format(CultureInfo.InvariantCulture, "В фабрику ReporterFactory передан неизвестный тип {0}", reporterType));
        }
    }
}
