using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class RestrictionsPremisesAssocDataModel : DataModel
    {
        private static RestrictionsPremisesAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM restrictions_premises_assoc WHERE deleted = 0";
        private const string TableName = "restrictions_premises_assoc";

        private RestrictionsPremisesAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static RestrictionsPremisesAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new RestrictionsPremisesAssocDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_restriction"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("premises", "id_premises", TableName, "id_premises");
            AddRelation("restrictions", "id_restriction", TableName, "id_restriction");
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = "INSERT INTO restrictions_premises_assoc (id_premises, id_restriction) VALUES (?, ?)";
            var restrictionObject = (RestrictionObjectAssoc)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_premises", restrictionObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_restriction", restrictionObject.IdRestriction));
        }
    }
}
