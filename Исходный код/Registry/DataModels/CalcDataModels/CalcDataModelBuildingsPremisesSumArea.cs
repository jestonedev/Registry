using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.DataModels;
using System.Data;
using System.Globalization;

namespace Registry.CalcDataModels
{

    public sealed class CalcDataModelBuildingsPremisesSumArea : CalcDataModel
    {
        private static CalcDataModelBuildingsPremisesSumArea dataModel = null;

        private static string tableName = "buildings_premises_sum_area";

        private CalcDataModelBuildingsPremisesSumArea()
            : base()
        {
            Table = InitializeTable();
            Refresh(CalcDataModelFilterEnity.All, null);
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.CurrentCulture;
            table.Columns.Add("id_building").DataType = typeof(int);
            table.Columns.Add("sum_area").DataType = typeof(double);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_building"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelBuildingsPremisesSumArea");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var buildings = from buildings_row in DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select())
                            where (config.Entity == CalcDataModelFilterEnity.Building ? buildings_row.Field<int>("id_building") == config.IdObject :
                                   config.Entity == CalcDataModelFilterEnity.All ? true : false)
                            select buildings_row;
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            // Вычисляем агрегационную информацию
            var result = from buildings_row in buildings
                         join premises_row in premises
                         on buildings_row.Field<int>("id_building") equals premises_row.Field<int>("id_building")
                         group premises_row.Field<double>("total_area") by buildings_row.Field<int>("id_building") into gs
                         select new
                         {
                             id_building = gs.Key,
                             sum_area = gs.Sum()
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_building, 
                    x.sum_area });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelBuildingsPremisesSumArea GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelBuildingsPremisesSumArea();
            return dataModel;
        }
    }
}
