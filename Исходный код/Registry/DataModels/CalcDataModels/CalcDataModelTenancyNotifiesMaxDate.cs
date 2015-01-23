using Registry.CalcDataModels;
using Registry.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Registry.Entities;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelTenancyNotifiesMaxDate: CalcDataModel
    {
        private static CalcDataModelTenancyNotifiesMaxDate dataModel = null;

        private static string tableName = "tenancy_notifies_max_date";

        private CalcDataModelTenancyNotifiesMaxDate()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("id_process").DataType = typeof(int);
            table.Columns.Add("notify_date").DataType = typeof(DateTime);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_process"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelTenancyNotifiesMaxDate");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var tenancy_notifies = DataModelHelper.FilterRows(TenancyNotifiesDataModel.GetInstance().Select(), config.Entity, config.IdObject);    
            // Вычисляем агрегационную информацию
            var result = from tenancy_notifies_row in tenancy_notifies
                         group tenancy_notifies_row.Field<DateTime>("notify_date") by
                                             tenancy_notifies_row.Field<int>("id_process") into gs
                         select new
                         {
                             id_process = gs.Key,
                             notify_date = gs.Max()
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_process, 
                    x.notify_date });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelTenancyNotifiesMaxDate GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelTenancyNotifiesMaxDate();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
