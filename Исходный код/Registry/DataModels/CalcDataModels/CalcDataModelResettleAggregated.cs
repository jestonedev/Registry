using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.DataModels;
using System.Globalization;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelResettleAggregated : CalcDataModel
    {
        private static CalcDataModelResettleAggregated dataModel = null;

        private static string tableName = "resettle_aggregated";

        private CalcDataModelResettleAggregated()
        {
            Table = InitializeTable();
            Refresh(CalcDataModelFilterEnity.All, null);            
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("id_process").DataType = typeof(int);
            table.Columns.Add("resettlers").DataType = typeof(string);
            table.Columns.Add("address_from").DataType = typeof(string);
            table.Columns.Add("address_to").DataType = typeof(string);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_process"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModeTenancyAggregated");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var resettles = from resettles_row in DataModelHelper.FilterRows(ResettleProcessesDataModel.GetInstance().Select())
                            where (config.Entity == CalcDataModelFilterEnity.Resettle ?
                                  resettles_row.Field<int>("id_process") == config.IdObject : true)
                            select resettles_row;
            var resettle_persons = from resettle_persons_row in DataModelHelper.FilterRows(ResettlePersonsDataModel.GetInstance().Select())
                                   where (config.Entity == CalcDataModelFilterEnity.Tenancy ?
                                           resettle_persons_row.Field<int>("id_process") == config.IdObject : true)
                                   select resettle_persons_row;
            // Вычисляем агрегационную информацию
            var resettlers = from resettle_persons_row in resettle_persons
                             group (resettle_persons_row.Field<string>("surname") + " " + 
                                    resettle_persons_row.Field<string>("name") + " " +
                                    resettle_persons_row.Field<string>("patronymic")).Trim() by resettle_persons_row.Field<int>("id_process") into gs
                             select new
                             {
                                 id_process = gs.Key,
                                 resettlers = gs.Aggregate((a, b) => { 
                                     return a + ", " + b; 
                                 })
                             };
            var addresses_from = DataModelHelper.AggregateAddressByIdProcess(ResettleBuildingsFromAssocDataModel.GetInstance(),
                ResettlePremisesFromAssocDataModel.GetInstance(),
                ResettleSubPremisesFromAssocDataModel.GetInstance(), config.Entity == CalcDataModelFilterEnity.Resettle ? config.IdObject : null);
            var addresses_to = DataModelHelper.AggregateAddressByIdProcess(ResettleBuildingsToAssocDataModel.GetInstance(),
                ResettlePremisesToAssocDataModel.GetInstance(),
                ResettleSubPremisesToAssocDataModel.GetInstance(), config.Entity == CalcDataModelFilterEnity.Resettle ? config.IdObject : null);
            var result = from resettles_row in resettles
                         join resettlers_row in resettlers
                         on resettles_row.Field<int>("id_process") equals resettlers_row.id_process into a
                         from a_row in a.DefaultIfEmpty()
                         join addresses_from_row in addresses_from
                         on resettles_row.Field<int>("id_process") equals addresses_from_row.IdProcess into b
                         from b_row in b.DefaultIfEmpty()
                         join addresses_to_row in addresses_to
                         on resettles_row.Field<int>("id_process") equals addresses_to_row.IdProcess into c
                         from c_row in c.DefaultIfEmpty()
                         select new
                         {
                             id_process = resettles_row.Field<int>("id_process"),
                             address_to = (c_row == null) ? "" : c_row.Address,
                             address_from = (b_row == null) ? "" : b_row.Address,
                             resettlers = (a_row == null) ? "" : a_row.resettlers
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_process, 
                    x.resettlers,
                    x.address_from, 
                    x.address_to
                });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelResettleAggregated GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelResettleAggregated();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
