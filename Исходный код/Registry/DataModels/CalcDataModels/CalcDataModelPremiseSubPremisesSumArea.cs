using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;

namespace Registry.DataModels.CalcDataModels
{

    internal sealed class CalcDataModelPremiseSubPremisesSumArea : CalcDataModel
    {
        private static CalcDataModelPremiseSubPremisesSumArea _dataModel;

        private const string TableName = "premise_sub_premises_sum_area";

        private CalcDataModelPremiseSubPremisesSumArea()
        {
            Table = InitializeTable();
            Refresh();
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.PremisesDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.SubPremisesDataModel).Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_premises").DataType = typeof(int);
            table.Columns.Add("sum_area").DataType = typeof(double);
            table.PrimaryKey = new [] { table.Columns["id_premises"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelPremiseSubPremisesSumArea");
            // Фильтруем удаленные строки
            var premises = DataModel.GetInstance(DataModelType.PremisesDataModel).FilterDeletedRows();
            var subPremises = DataModel.GetInstance(DataModelType.SubPremisesDataModel).FilterDeletedRows();
            // Вычисляем агрегационную информацию
            var result = from premisesRow in premises
                         join subPremisesRow in subPremises
                         on premisesRow.Field<int>("id_premises") equals subPremisesRow.Field<int>("id_premises")
                         where new [] {4, 5, 9, 11}.Contains(subPremisesRow.Field<int>("id_state"))
                         group subPremisesRow.Field<double>("total_area") by premisesRow.Field<int>("id_premises") into gs
                         select new
                         {
                             id_premises = gs.Key,
                             sum_area = gs.Sum()
                         };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_premises, x.sum_area);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelPremiseSubPremisesSumArea GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelPremiseSubPremisesSumArea());
        }
    }
}
