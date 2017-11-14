using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.DataModels.CalcDataModels
{
    public sealed class CalcDataModelJudgeInfo: CalcDataModel
    {
         private static CalcDataModelJudgeInfo _dataModel;

        private const string TableName = "judge_info";

        private CalcDataModelJudgeInfo()
        {
            Table = InitializeTable();
            Refresh();
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_judge").DataType = typeof(int);
            table.Columns.Add("judge").DataType = typeof(string);
            table.PrimaryKey = new[] { table.Columns["id_judge"] };
            return table;
        }

        protected override void Calculate(object sender, DoWorkEventArgs e)
        {
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModeTenancyAggregated");
            // Фильтруем удаленные строки
            var judges = DataModel.GetInstance<EntityDataModel<Judge>>().FilterDeletedRows().ToList();
            // Вычисляем агрегационную информацию
            var result =
                from row in judges
                where row.Field<short>("is_inactive") == 0
                select new
                {
                    id_judge = row.Field<int>("id_judge"),
                    judge = row.Field<string>("snp") + string.Format(" (участок №{0})", row.Field<int>("num_district"))
                };

            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_judge, x.judge);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelJudgeInfo GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelJudgeInfo());
        }
    }
}
