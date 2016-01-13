using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;

namespace Registry.DataModels.CalcDataModels
{
    internal sealed class CalcDataModelLastClaimStates : CalcDataModel
    {
        private static CalcDataModelLastClaimStates _dataModel;

        private const string TableName = "last_claim_states";

        private CalcDataModelLastClaimStates()
        {
            Table = InitializeTable();
            Refresh();      
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.ClaimStatesDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.ClaimsDataModel).Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_claim").DataType = typeof(int);
            table.Columns.Add("id_state_type").DataType = typeof(int);
            table.Columns.Add("state_type").DataType = typeof(string);
            table.Columns.Add("date_start_state").DataType = typeof(DateTime);
            table.PrimaryKey = new[] { table.Columns["id_claim"] };
            return table;
        }

        protected override void Calculate(object sender, DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModeTenancyAggregated");
            // Фильтруем удаленные строки
            var claimStates = DataModel.GetInstance(DataModelType.ClaimStatesDataModel).FilterDeletedRows();
            // Вычисляем агрегационную информацию
            var lastClaimStateMaxIds =
                        from claimStateRow in claimStates
                        group claimStateRow.Field<int?>("id_state") by claimStateRow.Field<int?>("id_claim") into gs
                        select new
                        {
                            id_claim = gs.Key,
                            id_state = gs.Max()
                        };
            var result =
                from claimStateRow in claimStates
                join lastClaimStateRow in lastClaimStateMaxIds
                    on claimStateRow.Field<int?>("id_state") equals lastClaimStateRow.id_state
                join stateTypeRow in
                    DataModel.GetInstance(DataModelType.ClaimStateTypesDataModel).FilterDeletedRows()
                    on claimStateRow.Field<int?>("id_state_type") equals
                    stateTypeRow.Field<int?>("id_state_type")
                select new
                {
                    id_claim = claimStateRow.Field<int>("id_claim"),
                    id_state_type = stateTypeRow.Field<int>("id_state_type"),
                    state_type = stateTypeRow.Field<string>("state_type"),
                    date_start_state = claimStateRow.Field<DateTime?>("date_start_state")
                };


            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_claim, x.id_state_type, x.state_type, x.date_start_state);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelLastClaimStates GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelLastClaimStates());
        }
    }
}
