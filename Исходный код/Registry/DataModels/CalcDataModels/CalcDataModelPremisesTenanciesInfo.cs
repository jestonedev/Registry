using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelPremisesTenanciesInfo : CalcDataModel
    {
        private static CalcDataModelPremisesTenanciesInfo dataModel = null;

        private static string tableName = "premises_tenancies_reg_number";

        private CalcDataModelPremisesTenanciesInfo()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(tableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_premises").DataType = typeof(int);
            table.Columns.Add("registration_num").DataType = typeof(string);
            table.Columns.Add("registration_date").DataType = typeof(DateTime);
            table.Columns.Add("residence_warrant_num").DataType = typeof(string);
            table.Columns.Add("residence_warrant_date").DataType = typeof(DateTime);
            table.Columns.Add("tenant").DataType = typeof(string);
            return table;
        }

        protected override void Calculate(object sender, DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException(
                    "Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelPremisesTenanciesRegNumbers");
            var config = (CalcAsyncConfig) e.Argument;
            // Фильтруем удаленные строки
            var tenancyPremises = DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select());
            var tenancyProcesses = DataModelHelper.FilterRows(TenancyProcessesDataModel.GetInstance().Select());
            var tenancyPersons = DataModelHelper.FilterRows(TenancyPersonsDataModel.GetInstance().Select());
            // Вычисляем агрегационную информацию
            var tenants = from row in tenancyPersons
                where row.Field<int?>("id_kinship") == 1
                select new
                {
                    id_process = row.Field<int>("id_process"),
                    tenant =
                        row.Field<string>("surname") +
                        (row.Field<string>("name") != null ? " " + row.Field<string>("name") : "") +
                        (row.Field<string>("patronymic") != null ? " " + row.Field<string>("patronymic") : "")
                };
            var tenancyProcessesWithTenants = from processRow in tenancyProcesses
                join tenantRow in tenants
                    on processRow.Field<int>("id_process") equals tenantRow.id_process into pTenants
                from pTenantsRow in pTenants.DefaultIfEmpty()
                select new
                {
                    id_process = processRow.Field<int>("id_process"),
                    registration_num = processRow.Field<string>("registration_num"),
                    registration_date = processRow.Field<DateTime?>("registration_date"),
                    residence_warrant_num = processRow.Field<string>("residence_warrant_num"),
                    residence_warrant_date = processRow.Field<DateTime?>("residence_warrant_date"),
                    tenant = pTenantsRow != null ? pTenantsRow.tenant : null
                };
            var result = from processRow in tenancyProcessesWithTenants
                            join tenancyPremisesRow in tenancyPremises
                            on processRow.id_process equals tenancyPremisesRow.Field<int>("id_process")
                            select new
                            {
                                id_premises = tenancyPremisesRow.Field<int>("id_premises"),
                                processRow.registration_num,
                                processRow.registration_date,
                                processRow.residence_warrant_num,
                                processRow.residence_warrant_date,
                                processRow.tenant
                            };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_premises, x.registration_num, x.registration_date, x.residence_warrant_num, x.residence_warrant_date, x.tenant);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelPremisesTenanciesInfo GetInstance()
        {
            return dataModel ?? (dataModel = new CalcDataModelPremisesTenanciesInfo());
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
