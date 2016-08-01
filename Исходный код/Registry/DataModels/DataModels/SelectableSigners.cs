namespace Registry.DataModels.DataModels
{
    internal class SelectableSigners : DataModel
    {
        private static SelectableSigners _dataModel;
        private const string SelectQuery = @"SELECT s.id_record, CONCAT(s.surname, ' ', s.`name`, 
                                              IF(s.patronymic IS NULL, '', CONCAT(' ', s.patronymic))) AS snp, id_signer_group
                                            FROM selectable_signers s
                                            WHERE s.deleted = 0";
        private const string TableName = "selectable_head_housing_dep";

        private SelectableSigners()
            : base(SelectQuery, TableName, null)
        {
        }

        public static SelectableSigners GetInstance()
        {
            return _dataModel ?? (_dataModel = new SelectableSigners());
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_record"] };
        }
    }
}
