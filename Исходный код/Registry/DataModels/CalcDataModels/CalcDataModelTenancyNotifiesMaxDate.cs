using System;
using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;

namespace Registry.DataModels.CalcDataModels
{
    internal sealed class CalcDataModelTenancyNotifiesMaxDate : CalcDataModel
    {
        private static CalcDataModelTenancyNotifiesMaxDate _dataModel;

        private const string TableName = "tenancy_notifies_max_date";

        private CalcDataModelTenancyNotifiesMaxDate()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(DataModel.GetInstance<TenancyNotifiesDataModel>().Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_process").DataType = typeof(int);
            table.Columns.Add("notify_date").DataType = typeof(DateTime);
            table.PrimaryKey = new [] { table.Columns["id_process"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelTenancyNotifiesMaxDate");
            // Фильтруем удаленные строки
            var tenancyNotifies = DataModel.GetInstance<TenancyNotifiesDataModel>().FilterDeletedRows();
            // Вычисляем агрегационную информацию
            var result = from tenancyNotifiesRow in tenancyNotifies
                         group tenancyNotifiesRow.Field<DateTime>("notify_date") by
                                             tenancyNotifiesRow.Field<int>("id_process") into gs
                         select new
                         {
                             id_process = gs.Key,
                             notify_date = gs.Max()
                         };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_process, x.notify_date);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelTenancyNotifiesMaxDate GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelTenancyNotifiesMaxDate());
        }
    }
}
