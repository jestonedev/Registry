﻿using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.CalcDataModels
{
    internal sealed class CalcDataModelPremisesTenanciesInfo : CalcDataModel
    {
        private static CalcDataModelPremisesTenanciesInfo _dataModel;

        private const string TableName = "premises_tenancies_reg_number";

        private CalcDataModelPremisesTenanciesInfo()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(EntityDataModel<TenancyPremisesAssoc>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<TenancySubPremisesAssoc>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<TenancyProcess>.GetInstance().Select());
            RefreshOnTableModify(EntityDataModel<TenancyPerson>.GetInstance().Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_premises").DataType = typeof(int);
            table.Columns.Add("id_process").DataType = typeof(int);
            table.Columns.Add("registration_num").DataType = typeof(string);
            table.Columns.Add("registration_date").DataType = typeof(DateTime);
            table.Columns.Add("end_date").DataType = typeof(DateTime);
            table.Columns.Add("until_dismissal").DataType = typeof(bool);
            table.Columns.Add("residence_warrant_num").DataType = typeof(string);
            table.Columns.Add("residence_warrant_date").DataType = typeof(DateTime);
            table.Columns.Add("tenant").DataType = typeof(string);
            table.Columns.Add("object_type").DataType = typeof(int);
            return table;
        }

        protected override void Calculate(object sender, DoWorkEventArgs e)
        {
            if (e == null)
                throw new DataModelException(
                    "Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelPremisesTenanciesRegNumbers");
            // Фильтруем удаленные строки
            var tenancyPremises = EntityDataModel<TenancyPremisesAssoc>.GetInstance().FilterDeletedRows();
            var tenancySubPremises = EntityDataModel<TenancySubPremisesAssoc>.GetInstance().FilterDeletedRows();
            var subPremises = EntityDataModel<SubPremise>.GetInstance().FilterDeletedRows();
            var tenancyProcesses = EntityDataModel<TenancyProcess>.GetInstance().FilterDeletedRows();
            var tenancyPersons = EntityDataModel<TenancyPerson>.GetInstance().FilterDeletedRows();
            // Вычисляем агрегационную информацию
            var tenancyAssoc = (from tenancySubPremisesRow in tenancySubPremises
                join subPremisesRow in subPremises
                    on tenancySubPremisesRow.Field<int>("id_sub_premises") equals
                    subPremisesRow.Field<int>("id_sub_premises")
                select new
                {
                    idPremises = subPremisesRow.Field<int>("id_premises"),
                    idProcess = tenancySubPremisesRow.Field<int>("id_process"),
                    idObjectType = 2
                })
                .Union(from premisesRow in tenancyPremises 
                select new
                {
                    idPremises = premisesRow.Field<int>("id_premises"),
                    idProcess = premisesRow.Field<int>("id_process"),
                    idObjectType = 1
                });
            var tenants = from row in tenancyPersons
                          where row.Field<int?>("id_kinship") == 1 && row.Field<DateTime?>("exclude_date") == null
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
                    end_date = processRow.Field<DateTime?>("end_date"),
                    until_dismissal = processRow.Field<short?>("until_dismissal") == 1,
                    residence_warrant_num = processRow.Field<string>("residence_warrant_num"),
                    residence_warrant_date = processRow.Field<DateTime?>("residence_warrant_date"),
                    tenant = pTenantsRow != null ? pTenantsRow.tenant : null
                };
            var result = from processRow in tenancyProcessesWithTenants
                         join tenancyAssocRow in tenancyAssoc
                            on processRow.id_process equals tenancyAssocRow.idProcess
                            where processRow.registration_num == null || !processRow.registration_num.TrimEnd().EndsWith("н")
                            select new
                            {
                                tenancyAssocRow.idPremises,
                                processRow.id_process,
                                processRow.registration_num,
                                processRow.registration_date,
                                processRow.end_date,
                                processRow.until_dismissal,
                                processRow.residence_warrant_num,
                                processRow.residence_warrant_date,
                                processRow.tenant,
                                tenancyAssocRow.idObjectType,
                            };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.idPremises, x.id_process, x.registration_num, 
                    x.registration_date, x.end_date, x.until_dismissal, x.residence_warrant_num, x
                    .residence_warrant_date, x.tenant, x.idObjectType);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelPremisesTenanciesInfo GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelPremisesTenanciesInfo());
        }
    }
}
