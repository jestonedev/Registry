using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.DataModels;
using System.Data;
using System.Globalization;
using Registry.Entities;

namespace Registry.CalcDataModels
{

    public sealed class CalcDataModelPremiseSubPremisesSumArea : CalcDataModel
    {
        private static CalcDataModelPremiseSubPremisesSumArea dataModel = null;

        private static string tableName = "premise_sub_premises_sum_area";

        private CalcDataModelPremiseSubPremisesSumArea()
            : base()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("id_premises").DataType = typeof(int);
            table.Columns.Add("sum_area").DataType = typeof(double);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_premises"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelPremiseSubPremisesSumArea");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
            // Вычисляем агрегационную информацию
            var result = from premises_row in premises
                         join sub_premises_row in sub_premises
                         on premises_row.Field<int>("id_premises") equals sub_premises_row.Field<int>("id_premises")
                         where new int[] {4, 5, 9}.Contains(sub_premises_row.Field<int>("id_state"))
                         group sub_premises_row.Field<double>("total_area") by premises_row.Field<int>("id_premises") into gs
                         select new
                         {
                             id_premises = gs.Key,
                             sum_area = gs.Sum()
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_premises, 
                    x.sum_area });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelPremiseSubPremisesSumArea GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelPremiseSubPremisesSumArea();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
