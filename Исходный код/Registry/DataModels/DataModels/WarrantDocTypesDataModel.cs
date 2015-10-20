using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal sealed class WarrantDocTypesDataModel : DataModel
    {
        private static WarrantDocTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM warrant_doc_types";
        private const string TableName = "warrant_doc_types";

        private WarrantDocTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static WarrantDocTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new WarrantDocTypesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_warrant_doc_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_warrant_doc_type", "warrants", "id_warrant_doc_type");
        }
    }
}
