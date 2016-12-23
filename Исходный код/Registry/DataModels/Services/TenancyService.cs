using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.Services
{
    public static class TenancyService
    {
        public static IEnumerable<int> TenancyProcessIdsBySnp(string[] snp, Func<DataRow, bool> condition)
        {
            var tenancyPersons = EntityDataModel<TenancyPerson>.GetInstance().FilterDeletedRows();

            return
                (from tenancyPersonsRow in tenancyPersons
                    where
                        snp.Any() && tenancyPersonsRow.Field<string>("surname") != null &&
                        string.Equals(tenancyPersonsRow.Field<string>("surname"), snp[0],
                            StringComparison.InvariantCultureIgnoreCase) &&
                        ((snp.Length < 2) || tenancyPersonsRow.Field<string>("name") != null &&
                         string.Equals(tenancyPersonsRow.Field<string>("name"), snp[1],
                             StringComparison.InvariantCultureIgnoreCase)) &&
                        ((snp.Length < 3) || tenancyPersonsRow.Field<string>("patronymic") != null &&
                         string.Equals(tenancyPersonsRow.Field<string>("patronymic"), snp[2],
                             StringComparison.InvariantCultureIgnoreCase)) &&
                        condition(tenancyPersonsRow) && tenancyPersonsRow.Field<int?>("id_process") != null
                    select tenancyPersonsRow.Field<int>("id_process")).Distinct();
        }

        public static IEnumerable<int> TenancyProcessIDsByAddress(string[] addressParts)
        {
            var kladrStreets = DataModel.GetInstance<KladrStreetsDataModel>().FilterDeletedRows().ToList();
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows().ToList();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
            var tenancyBuildingsAssoc = EntityDataModel<TenancyBuildingAssoc>.GetInstance().FilterDeletedRows();
            var tenancyPremisesAssoc = EntityDataModel<TenancyPremisesAssoc>.GetInstance().FilterDeletedRows();
            var tenancySubPremisesAssoc = EntityDataModel<TenancySubPremisesAssoc>.GetInstance().FilterDeletedRows();
            var tenancyBuildings = from tenancyBuildingsRow in tenancyBuildingsAssoc
                join buildingsRow in buildings
                    on tenancyBuildingsRow.Field<int?>("id_building") equals buildingsRow.Field<int>("id_building")
                join kladrRow in kladrStreets
                    on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                where buildingsRow.Field<string>("house") != null && tenancyBuildingsRow.Field<int?>("id_process") != null &&
                    (addressParts.Length == 1
                    ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                        Contains(addressParts[0].ToUpperInvariant())
                    : (addressParts.Length >= 2) && kladrRow.Field<string>("street_name").ToUpperInvariant().
                        Contains(addressParts[0].ToUpperInvariant()) &&
                      buildingsRow.Field<string>("house").ToUpper().Equals(addressParts[1].ToUpper()))
                select tenancyBuildingsRow.Field<int>("id_process");
            var tenancyPremises = from tenancyPremisesRow in tenancyPremisesAssoc
                join premisesRow in premises
                    on tenancyPremisesRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                join buildingsRow in buildings
                    on premisesRow.Field<int?>("id_building") equals buildingsRow.Field<int>("id_building")
                join kladrRow in kladrStreets
                    on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                where buildingsRow.Field<string>("house") != null && premisesRow.Field<string>("premises_num") != null &&
                    tenancyPremisesRow.Field<int?>("id_process") != null &&
                    (addressParts.Length == 1 ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                        Contains(addressParts[0].ToUpperInvariant()) : addressParts.Length == 2
                        ? kladrRow.Field<string>("street_name").ToUpperInvariant().Contains(addressParts[0].ToUpperInvariant()) &&
                          buildingsRow.Field<string>("house").ToUpper().Equals(addressParts[1].ToUpper())
                        : (addressParts.Length == 3) && 
                          kladrRow.Field<string>("street_name").ToUpperInvariant().Contains(addressParts[0].ToUpperInvariant()) &&
                          buildingsRow.Field<string>("house").ToUpper().Equals(addressParts[1].ToUpper()) &&
                          premisesRow.Field<string>("premises_num").ToUpperInvariant().Equals(addressParts[2].ToUpperInvariant()))
                select tenancyPremisesRow.Field<int>("id_process");
            var tenancySubPremises = from tenancySubPremisesRow in tenancySubPremisesAssoc
                join subPremisesRow in subPremises
                    on tenancySubPremisesRow.Field<int?>("id_sub_premises") equals
                    subPremisesRow.Field<int>("id_sub_premises")
                join premisesRow in premises
                    on subPremisesRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                join buildingsRow in buildings
                    on premisesRow.Field<int?>("id_building") equals buildingsRow.Field<int>("id_building")
                join kladrRow in kladrStreets
                    on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                where buildingsRow.Field<string>("house") != null && premisesRow.Field<string>("premises_num") != null &&
                    tenancySubPremisesRow.Field<int?>("id_process") != null &&
                    (addressParts.Length == 1
                    ? kladrRow.Field<string>("street_name").ToUpperInvariant().Contains(addressParts[0].ToUpperInvariant())
                    : addressParts.Length == 2
                        ? kladrRow.Field<string>("street_name").ToUpperInvariant().
                            Contains(addressParts[0].ToUpperInvariant()) &&
                          buildingsRow.Field<string>("house").ToUpper().Equals(addressParts[1].ToUpper())
                        : (addressParts.Length == 3) && kladrRow.Field<string>("street_name").ToUpperInvariant().
                            Contains(addressParts[0].ToUpperInvariant()) &&
                          buildingsRow.Field<string>("house").ToUpper().Equals(addressParts[1].ToUpper()) &&
                          premisesRow.Field<string>("premises_num").ToUpper().Equals(addressParts[2].ToUpper()))
                select tenancySubPremisesRow.Field<int>("id_process");
            return tenancyBuildings.Union(tenancyPremises).Union(tenancySubPremises);
        }

        public static IEnumerable<int> TenancyProcessIDsByCondition(Func<DataRow, bool> condition,
            DataModelHelper.ConditionType conditionType)
        {
            var buildings = EntityDataModel<Building>.GetInstance().FilterDeletedRows().ToList();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
            var tenancyBuildingsAssoc = EntityDataModel<TenancyBuildingAssoc>.GetInstance().FilterDeletedRows();
            var tenancyPremisesAssoc = EntityDataModel<TenancyPremisesAssoc>.GetInstance().FilterDeletedRows();
            var tenancySubPremisesAssoc = EntityDataModel<TenancySubPremisesAssoc>.GetInstance().FilterDeletedRows();
            var tenancyBuildings = from tenancyBuildingsRow in tenancyBuildingsAssoc
                join buildingsRow in buildings
                    on tenancyBuildingsRow.Field<int?>("id_building") equals buildingsRow.Field<int>("id_building")
                where (conditionType != DataModelHelper.ConditionType.PremisesCondition) && condition(buildingsRow) &&
                       tenancyBuildingsRow.Field<int?>("id_process") != null
                select tenancyBuildingsRow.Field<int>("id_process");
            var tenancyPremises = from tenancyPremisesRow in tenancyPremisesAssoc
                join premisesRow in premises
                    on tenancyPremisesRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                join buildingsRow in buildings
                    on premisesRow.Field<int?>("id_building") equals buildingsRow.Field<int>("id_building")
                where
                    (conditionType == DataModelHelper.ConditionType.PremisesCondition
                        ? condition(premisesRow)
                        : condition(buildingsRow)) && tenancyPremisesRow.Field<int?>("id_process") != null
                select tenancyPremisesRow.Field<int>("id_process");
            var tenancySubPremises = from tenancySubPremisesRow in tenancySubPremisesAssoc
                join subPremisesRow in subPremises
                    on tenancySubPremisesRow.Field<int?>("id_sub_premises") equals
                    subPremisesRow.Field<int>("id_sub_premises")
                join premisesRow in premises
                    on subPremisesRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                join buildingsRow in buildings
                    on premisesRow.Field<int?>("id_building") equals buildingsRow.Field<int>("id_building")
                where
                    (conditionType == DataModelHelper.ConditionType.PremisesCondition
                        ? condition(premisesRow)
                        : condition(buildingsRow)) && tenancySubPremisesRow.Field<int?>("id_process") != null
                select tenancySubPremisesRow.Field<int>("id_process");
            return tenancyBuildings.Union(tenancyPremises).Union(tenancySubPremises);
        }

        public static IEnumerable<int> TenancyProcessIDsByBuildingId(int id)
        {
            var tenancyBuildingsAssoc = EntityDataModel<TenancyBuildingAssoc>.GetInstance().FilterDeletedRows();
            var tenancyBuildings = from tenancyBuildingsRow in tenancyBuildingsAssoc
                where tenancyBuildingsRow.Field<int?>("id_building") == id && 
                      tenancyBuildingsRow.Field<int?>("id_process") != null
                select tenancyBuildingsRow.Field<int>("id_process");
            return tenancyBuildings;
        }

        public static IEnumerable<int> TenancyProcessIDsByPremisesId(int id)
        {
            var tenancyPremisesAssoc = EntityDataModel<TenancyPremisesAssoc>.GetInstance().FilterDeletedRows();
            var tenancyPremises = from tenancyPremisesRow in tenancyPremisesAssoc
                where tenancyPremisesRow.Field<int?>("id_premises") == id &&
                      tenancyPremisesRow.Field<int?>("id_process") != null
                select tenancyPremisesRow.Field<int>("id_process");
            return tenancyPremises;
        }

        public static IEnumerable<int> TenancyProcessIDsBySubPremisesId(int id)
        {
            var tenancySubPremisesAssoc = EntityDataModel<TenancySubPremisesAssoc>.GetInstance().FilterDeletedRows();
            var tenancySubPremises = from tenancySubPremisesRow in tenancySubPremisesAssoc
                where tenancySubPremisesRow.Field<int?>("id_sub_premises") == id &&
                      tenancySubPremisesRow.Field<int?>("id_process") != null
                select tenancySubPremisesRow.Field<int>("id_process");
            return tenancySubPremises;
        }

        public static bool TenancyProcessHasTenant(int idProcess)
        {
            var persons = EntityDataModel<TenancyPerson>.GetInstance().FilterDeletedRows();
            return (from personsRow in persons
                where
                    (personsRow.Field<int?>("id_kinship") == 1) && personsRow.Field<int?>("id_process") == idProcess &&
                    (personsRow.Field<DateTime?>("exclude_date") == null)
                select personsRow).Any();
        }

        public static int TenancyAgreementsForProcess(int idProcess)
        {
            var agreements = EntityDataModel<TenancyAgreement>.GetInstance().FilterDeletedRows();
            return (from agreementsRow in agreements
                where agreementsRow.Field<int?>("id_process") == idProcess
                select agreementsRow).Count();
        }

        public static int TenancyProcessesDuplicateCount(TenancyProcess process)
        {
            var tenancyProcesses = EntityDataModel<TenancyProcess>.GetInstance().FilterDeletedRows();
            return (from tenancyProcessesRow in tenancyProcesses
                where tenancyProcessesRow.Field<string>("registration_num") == process.RegistrationNum &&
                      tenancyProcessesRow.Field<int>("id_process") != process.IdProcess
                select tenancyProcessesRow).Count();
        }

        public static IEnumerable<int> OldTenancyProcesses()
        {
            // Собираем строку проверки уникальности сдаваемой группы помещений. Выглядеть эта строка будет примерно подобным образом з1з12п23п122к32 и т.п.
            var assocSubPremises =
                from assocSubPremisesRow in EntityDataModel<TenancySubPremisesAssoc>.GetInstance().FilterDeletedRows()
                where assocSubPremisesRow.Field<int?>("id_sub_premises") != null &&
                      assocSubPremisesRow.Field<int?>("id_process") != null
                group assocSubPremisesRow.Field<int>("id_sub_premises").ToString(CultureInfo.InvariantCulture)
                    by assocSubPremisesRow.Field<int>("id_process")
                into gs
                select new
                {
                    id_process = gs.Key,
                    value =
                        gs.Count() > 1
                            ? gs.OrderBy(val => val).Aggregate((str1, str2) => 'к' + str1 + 'к' + str2)
                            : 'к' + gs.First()
                };
            var assocPremises =
                from assocPremisesRow in EntityDataModel<TenancyPremisesAssoc>.GetInstance().FilterDeletedRows()
                where assocPremisesRow.Field<int?>("id_premises") != null &&
                      assocPremisesRow.Field<int?>("id_process") != null
                group assocPremisesRow.Field<int>("id_premises").ToString(CultureInfo.InvariantCulture)
                    by assocPremisesRow.Field<int>("id_process")
                into gs
                select new
                {
                    id_process = gs.Key,
                    value =
                        gs.Count() > 1
                            ? gs.OrderBy(val => val).Aggregate((str1, str2) => 'п' + str1 + 'п' + str2)
                            : 'п' + gs.First()
                };
            var assocBuildings =
                from assocBuildingsRow in EntityDataModel<TenancyBuildingAssoc>.GetInstance().FilterDeletedRows()
                where assocBuildingsRow.Field<int?>("id_building") != null &&
                      assocBuildingsRow.Field<int?>("id_process") != null
                group assocBuildingsRow.Field<int>("id_building").ToString(CultureInfo.InvariantCulture)
                    by assocBuildingsRow.Field<int>("id_process")
                into gs
                select new
                {
                    id_process = gs.Key,
                    value =
                        gs.Count() > 1
                            ? gs.OrderBy(val => val).Aggregate((str1, str2) => 'з' + str1 + 'з' + str2)
                            : 'з' + gs.First()
                };
            var assocObjects = assocSubPremises.Union(assocPremises).Union(assocBuildings);
            var identStrings = (from assocObjectsRow in assocObjects
                group assocObjectsRow.value by assocObjectsRow.id_process
                into gs
                select new
                {
                    id_process = gs.Key,
                    value = gs.Count() > 1 ? gs.OrderBy(val => val).Aggregate((str1, str2) => str1 + str2) : gs.First()
                }).ToList();
            var moreOneIdentStrings = from identRow in identStrings
                group identRow.id_process by identRow.value
                into gs
                where gs.Count() > 1
                select gs.Key;
            var duplicateProcesses = (from identRow in identStrings
                join tenancyRow in EntityDataModel<TenancyProcess>.GetInstance().FilterDeletedRows()
                    on identRow.id_process equals tenancyRow.Field<int>("id_process")
                join moreOneIdentStringsRow in moreOneIdentStrings
                    on identRow.value equals moreOneIdentStringsRow
                                      where tenancyRow.Field<string>("registration_num") != null && !tenancyRow.Field<string>("registration_num").EndsWith("н")
                select new
                {
                    id_process = tenancyRow.Field<int>("id_process"),
                    ident_value = identRow.value,
                    registration_date = tenancyRow.Field<DateTime?>("registration_date")
                }).ToList();
            var maxDates = from row in duplicateProcesses
                group row.registration_date by row.ident_value
                into gs
                select new
                {
                    ident_value = gs.Key,
                    max_date = gs.Max()
                };
            var maxProcessIds = from processRow in duplicateProcesses
                join maxDatesRow in maxDates
                    on new {processRow.ident_value, date = processRow.registration_date} equals
                    new {maxDatesRow.ident_value, date = maxDatesRow.max_date}
                select processRow.id_process;
            var duplicateProcessIds = from row in duplicateProcesses
                select row.id_process;
            return duplicateProcessIds.Except(maxProcessIds);
        }

        public static IEnumerable<AggregatedAddress> AggregateAddressByIdProcess(IEnumerable<DataRow> assocBuildings,
            IEnumerable<DataRow> assocPremises, IEnumerable<DataRow> assocSubPremises)
        {
            if (assocBuildings == null || assocPremises == null || assocSubPremises == null)
                throw new DataModelException("Не переданы все ссылки на ассоциативные модели");
            var kladrStreet = DataModel.GetInstance<KladrStreetsDataModel>().FilterDeletedRows();
            var buildings = DataModel.GetInstance<EntityDataModel<Building>>().FilterDeletedRows();
            var premises = EntityDataModel<Premise>.GetInstance().FilterDeletedRows().ToList();
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
            var aSubPremisesGc = from assocSubPremisesRow in assocSubPremises
                join subPremisesRow in subPremises
                    on assocSubPremisesRow.Field<int?>("id_sub_premises") equals
                    subPremisesRow.Field<int>("id_sub_premises")
                join premisesRow in premises.AsEnumerable()
                    on subPremisesRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                where assocSubPremisesRow.Field<int?>("id_process") != null &&
                      premisesRow.Field<int?>("id_building") != null &&
                      subPremisesRow.Field<int?>("id_premises") != null &&
                      premisesRow.Field<int?>("id_premises_type") != null
                group subPremisesRow.Field<string>("sub_premises_num") by
                    new
                    {
                        id_process = assocSubPremisesRow.Field<int>("id_process"),
                        id_building = premisesRow.Field<int>("id_building"),
                        id_premises = subPremisesRow.Field<int>("id_premises"),
                        id_premises_type = premisesRow.Field<int>("id_premises_type"),
                        premises_num = premisesRow.Field<string>("premises_num")
                    }
                into gs
                select new
                {
                    gs.Key.id_process,
                    gs.Key.id_building,
                    gs.Key.id_premises,
                    result_str =
                        (gs.Key.id_premises_type == 2 ? " ком. " : (gs.Key.id_premises_type == 4 ? " пом. " : " кв. ")) +
                        gs.Key.premises_num + (gs.Any()
                            ? " ком. " + gs.Aggregate((a, b) => a + ", " + b)
                            : "")
                };
            var aPremises = from assocPremisesRow in assocPremises
                join premisesRow in premises
                    on assocPremisesRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                  where assocPremisesRow.Field<int?>("id_process") != null &&
                      premisesRow.Field<int?>("id_building") != null &&
                      assocPremisesRow.Field<int?>("id_premises") != null &&
                      premisesRow.Field<int?>("id_premises_type") != null
                select new
                {
                    id_process = assocPremisesRow.Field<int>("id_process"),
                    id_building = premisesRow.Field<int>("id_building"),
                    id_premises = assocPremisesRow.Field<int>("id_premises"),
                    result_str = (premisesRow.Field<int>("id_premises_type") == 2
                        ? " ком. "
                        : (premisesRow.Field<int>("id_premises_type") == 4 ? " пом. " : " кв. ")) +
                                 premisesRow.Field<string>("premises_num")
                };
            var aBuildings = from assocBuildingsRow in assocBuildings
                             where assocBuildingsRow.Field<int?>("id_process") != null &&
                                   assocBuildingsRow.Field<int?>("id_building") != null
                select new
                {
                    id_process = assocBuildingsRow.Field<int>("id_process"),
                    id_building = assocBuildingsRow.Field<int>("id_building"),
                    result_str = ""
                };
            var assocPremisesGc = from assocPremisesRow in aPremises.Union(aSubPremisesGc)
                join premisesRow in premises
                    on assocPremisesRow.id_premises equals premisesRow.Field<int>("id_premises")
                group
                    assocPremisesRow.result_str
                    by new
                    {
                        assocPremisesRow.id_process,
                        assocPremisesRow.id_building
                    }
                into gs
                select new
                {
                    gs.Key.id_process,
                    gs.Key.id_building,
                    result_str = gs.Aggregate((a, b) => a + ", " + b.Trim())
                };
            var addresses = from aBuildingsRow in aBuildings.Union(assocPremisesGc)
                join buildingsRow in buildings
                    on aBuildingsRow.id_building equals buildingsRow.Field<int>("id_building")
                join kladrRow in kladrStreet
                    on buildingsRow.Field<string>("id_street") equals kladrRow.Field<string>("id_street")
                group
                    kladrRow.Field<string>("street_name") + ", дом " + buildingsRow.Field<string>("house") +
                    aBuildingsRow.result_str
                    by aBuildingsRow.id_process
                into gs
                select new AggregatedAddress(gs.Key, gs.Aggregate((a, b) => a + ", " + b.Trim()));
            return addresses;
        }

        public static IEnumerable<TenancyPerson> GetTenancyPersonsFromMsp(int idProcess)
        {
            var premisesAssoc = EntityDataModel<TenancyPremisesAssoc>.GetInstance();
            var premises = EntityDataModel<Premise>.GetInstance();
            var buildings = EntityDataModel<Building>.GetInstance();
            var streets = DataModel.GetInstance<KladrStreetsDataModel>();
            var tenancyPersons = new List<TenancyPerson>();
            var currentPremise = (from premisesAssocRow in premisesAssoc.FilterDeletedRows()
                join premisesRow in premises.FilterDeletedRows()
                    on premisesAssocRow.Field<int?>("id_premises") equals premisesRow.Field<int>("id_premises")
                join buildingsRow in buildings.FilterDeletedRows()
                    on premisesRow.Field<int?>("id_building") equals buildingsRow.Field<int>("id_building")
                join streetsRow in streets.FilterDeletedRows()
                    on buildingsRow.Field<string>("id_street") equals streetsRow.Field<string>("id_street")
                where premisesAssocRow.Field<int?>("id_process") == idProcess
                select new
                {
                    premises_num = premisesRow.Field<string>("premises_num"),
                    house = buildingsRow.Field<string>("house"),
                    street_name = streetsRow.Field<string>("street_name")
                }).ToList();
            if (currentPremise.Count == 0)
            {
                MessageBox.Show(
                    @"Нельзя импортировать данные из МСП, т.к. на процесс найма не завязано ни одного помещения",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return tenancyPersons;
            }
            using (var connection = new SqlConnection(Settings.RegistrySettings.MspConnectionString))
            {
                var command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = @"
                        SELECT v.family, name, father, v.birth, v.reg_date
                        FROM (
                        SELECT 
                          CASE SUBSTRING(ts.base_month,1,3) 
                          WHEN 'Jan' THEN SUBSTRING(ts.base_month,4,4)+'01'
                          WHEN 'Feb' THEN SUBSTRING(ts.base_month,4,4)+'02'
                          WHEN 'Mar' THEN SUBSTRING(ts.base_month,4,4)+'03'
                          WHEN 'Apr' THEN SUBSTRING(ts.base_month,4,4)+'04'
                          WHEN 'May' THEN SUBSTRING(ts.base_month,4,4)+'05'
                          WHEN 'Jun' THEN SUBSTRING(ts.base_month,4,4)+'06'
                          WHEN 'Jul' THEN SUBSTRING(ts.base_month,4,4)+'07'
                          WHEN 'Aug' THEN SUBSTRING(ts.base_month,4,4)+'08'
                          WHEN 'Sep' THEN SUBSTRING(ts.base_month,4,4)+'09'
                          WHEN 'Oct' THEN SUBSTRING(ts.base_month,4,4)+'10'
                          WHEN 'Nov' THEN SUBSTRING(ts.base_month,4,4)+'11'
                          WHEN 'Dec' THEN SUBSTRING(ts.base_month,4,4)+'12'
                          ELSE ts.base_month
                          END AS mon, family, name, father, birth, c AS reg_date
                        FROM dbo.t_bks ts
                        WHERE ts.street = @street AND house = @house AND RTRIM(LTRIM(flat)) = @flat) v
                        WHERE mon = (
                          SELECT 
                          MAX(CASE SUBSTRING(ts.base_month,1,3) 
                          WHEN 'Jan' THEN SUBSTRING(ts.base_month,4,4)+'01'
                          WHEN 'Feb' THEN SUBSTRING(ts.base_month,4,4)+'02'
                          WHEN 'Mar' THEN SUBSTRING(ts.base_month,4,4)+'03'
                          WHEN 'Apr' THEN SUBSTRING(ts.base_month,4,4)+'04'
                          WHEN 'May' THEN SUBSTRING(ts.base_month,4,4)+'05'
                          WHEN 'Jun' THEN SUBSTRING(ts.base_month,4,4)+'06'
                          WHEN 'Jul' THEN SUBSTRING(ts.base_month,4,4)+'07'
                          WHEN 'Aug' THEN SUBSTRING(ts.base_month,4,4)+'08'
                          WHEN 'Sep' THEN SUBSTRING(ts.base_month,4,4)+'09'
                          WHEN 'Oct' THEN SUBSTRING(ts.base_month,4,4)+'10'
                          WHEN 'Nov' THEN SUBSTRING(ts.base_month,4,4)+'11'
                          WHEN 'Dec' THEN SUBSTRING(ts.base_month,4,4)+'12'
                          ELSE ts.base_month
                          END) AS mon
                          FROM dbo.t_bks ts
                          WHERE ts.street = @street AND house = @house AND RTRIM(LTRIM(flat)) = @flat)
                            AND mon = (
                                SELECT 
                                MAX(CASE SUBSTRING(ts.base_month,1,3) 
                                WHEN 'Jan' THEN SUBSTRING(ts.base_month,4,4)+'01'
                                WHEN 'Feb' THEN SUBSTRING(ts.base_month,4,4)+'02'
                                WHEN 'Mar' THEN SUBSTRING(ts.base_month,4,4)+'03'
                                WHEN 'Apr' THEN SUBSTRING(ts.base_month,4,4)+'04'
                                WHEN 'May' THEN SUBSTRING(ts.base_month,4,4)+'05'
                                WHEN 'Jun' THEN SUBSTRING(ts.base_month,4,4)+'06'
                                WHEN 'Jul' THEN SUBSTRING(ts.base_month,4,4)+'07'
                                WHEN 'Aug' THEN SUBSTRING(ts.base_month,4,4)+'08'
                                WHEN 'Sep' THEN SUBSTRING(ts.base_month,4,4)+'09'
                                WHEN 'Oct' THEN SUBSTRING(ts.base_month,4,4)+'10'
                                WHEN 'Nov' THEN SUBSTRING(ts.base_month,4,4)+'11'
                                WHEN 'Dec' THEN SUBSTRING(ts.base_month,4,4)+'12'
                                ELSE ts.base_month
                                END) AS mon
                                FROM dbo.t_bks ts
                              )
                        ORDER BY mon;"
                };
                var streetName = currentPremise.First().street_name;
                var prefixes = new[]
                {
                    "пер. ", "ул. ", "пр-кт. ", "б-р. ",
                    "гск. ", "проезд. ", "пл-ка. ", "туп. "
                };
                var streetParts = streetName.Split(prefixes, StringSplitOptions.RemoveEmptyEntries);
                streetName = streetParts[streetParts.Length - 1];
                var street = new SqlParameter("street", SqlDbType.NVarChar) {Value = streetName};
                command.Parameters.Add(street);

                var house = new SqlParameter("house", SqlDbType.NVarChar) {Value = currentPremise.First().house};
                command.Parameters.Add(house);

                var flat = new SqlParameter("flat", SqlDbType.NVarChar)
                {
                    Value = currentPremise.First().premises_num
                };
                command.Parameters.Add(flat);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var surnameValue = GetValue(() => reader.GetString(0));
                        var nameValue = GetValue(() => reader.GetString(1));
                        var patronymicValue = GetValue(() => reader.GetString(2));
                        DateTime? birthValue = GetValue(() => reader.GetDateTime(3));
                        DateTime? regDateValue = GetValue(() => reader.GetDateTime(4));
                        var person = new TenancyPerson
                        {
                            Surname = surnameValue,
                            Name = nameValue,
                            Patronymic = patronymicValue,
                            DateOfBirth = birthValue,
                            IdProcess = idProcess,
                            IdKinship = 64,
                            IdDocumentType = 255,
                            RegistrationDate = regDateValue
                        };
                        tenancyPersons.Add(person);
                    }
                    return tenancyPersons;
                }
                catch (SqlException err)
                {
                    MessageBox.Show(string.Format("Ошибка подключения к базе данных. Подробнее: {0}", err.Message),
                        @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return tenancyPersons;
                }
            }
        }

        private static T GetValue<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (SqlNullValueException)
            {
                // Ignore
            }
            catch (InvalidCastException)
            {
                // Ignore
            }
            return default(T);
        }
    }
}
