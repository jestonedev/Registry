using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.Reporting.RegistryReporters;
using Registry.Reporting.ClaimsReporters;
using Registry.Reporting.TenancyReporters;
using System.Globalization;
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
                case ReporterType.RegistryMunicipalPremisesReporter:
                    return new MunicipalPremisesReporter();
                case ReporterType.RegistryExcerptReporter:
                    return new ExcerptReporter();
                case ReporterType.ClaimsStatisticReporter:
                    return new ClaimsStatisticReporter();
                case ReporterType.TenancyContractCommercialReporter:
                    return new TenancyContractCommercialReporter();
                case ReporterType.TenancyContractSocialReporter:
                    return new TenancyContractSocialReporter();
                case ReporterType.TenancyContractSpecial1711Reporter:
                    return new TenancyContractSpecial1711Reporter();
                case ReporterType.TenancyContractSpecial1712Reporter:
                    return new TenancyContractSpecial1712Reporter();
                case ReporterType.TenancyActReporter:
                    return new TenancyActReporter();
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
            }
            throw new ReporterException(
                String.Format(CultureInfo.InvariantCulture, "В фабрику ReporterFactory передан неизвестный тип {0}", reporterType.ToString()));
        }
    }
}
