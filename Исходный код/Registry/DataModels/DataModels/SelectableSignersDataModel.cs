namespace Registry.DataModels.DataModels
{
    public class SelectableSignersDataModel : DataModel
    {
        private static SelectableSignersDataModel _dataModel;
        private const string SelectQuery = @"SELECT s.id_record, CONCAT(s.surname, ' ', s.`name`, 
                                              IF(s.patronymic IS NULL, '', CONCAT(' ', s.patronymic))) AS snp, id_signer_group
                                            FROM selectable_signers s
                                            WHERE s.deleted = 0";
        private const string TableName = "selectable_head_housing_dep";

        private SelectableSignersDataModel()
            : base(SelectQuery, TableName, null)
        {
        }

        public static SelectableSignersDataModel GetInstance()
        {
            return _dataModel ?? (_dataModel = new SelectableSignersDataModel());
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_record"] };
        }
    }
}
