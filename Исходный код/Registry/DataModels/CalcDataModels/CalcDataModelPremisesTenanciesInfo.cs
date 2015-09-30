using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Registry.DataModels;
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
            table.Columns.Add("tenancy_info").DataType = typeof(string);
            table.PrimaryKey = new[] { table.Columns["id_premises"] };
            return table;
        }

        protected override void Calculate(object sender, DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelPremisesTenanciesRegNumbers");
            var config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var tenancyPremises = DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select());
            var tenancyProcesses = DataModelHelper.FilterRows(TenancyProcessesDataModel.GetInstance().Select());
            // Вычисляем агрегационную информацию
            var regNumbers = from tenancyPremisesRow in tenancyPremises
                         join tenancyProcessesRow in tenancyProcesses
                            on tenancyPremisesRow.Field<int>("id_process") equals tenancyProcessesRow.Field<int>("id_process")
                         where tenancyProcessesRow.Field<string>("registration_num") != null
                        group tenancyProcessesRow.Field<string>("registration_num") by tenancyPremisesRow.Field<int>("id_premises") into gs
                         select new
                         {
                             id_premises = gs.Key,
                             tenancy_info = "Договоры: "+gs.Aggregate((acc, val) => val == "" ? acc : acc + ", " + val)
                         };
            var orders = from tenancyPremisesRow in tenancyPremises
                             join tenancyProcessesRow in tenancyProcesses
                                on tenancyPremisesRow.Field<int>("id_process") equals tenancyProcessesRow.Field<int>("id_process")
                         where tenancyProcessesRow.Field<string>("residence_warrant_num") != null
                         group tenancyProcessesRow.Field<string>("residence_warrant_num") by tenancyPremisesRow.Field<int>("id_premises") into gs
                             select new
                             {
                                 id_premises = gs.Key,
                                 tenancy_info = "Ордера: " + gs.Aggregate((acc, val) => val == "" ? acc : acc + ", " + val)
                             };
            var result = from row in regNumbers.Union(orders)
                group row.tenancy_info by row.id_premises
                into gs
                select new
                {
                    id_premises = gs.Key,
                    tenancy_info = gs.Aggregate((acc, val) => acc + "; " + val)
                };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_premises, x.tenancy_info);
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
