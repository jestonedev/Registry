﻿using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class OwnershipBuildingsAssocDataModel : DataModel
    {
        private static OwnershipBuildingsAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM ownership_buildings_assoc WHERE deleted = 0";
        private const string TableName = "ownership_buildings_assoc";

        private OwnershipBuildingsAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static OwnershipBuildingsAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new OwnershipBuildingsAssocDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_ownership_right"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("ownership_rights", "id_ownership_right", TableName, "id_ownership_right");
            AddRelation("buildings", "id_building", TableName, "id_building");
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = "INSERT INTO ownership_buildings_assoc (id_building, id_ownership_right) VALUES (?, ?)";;
            var ownershipRightObjectAssoc = (OwnershipRightObjectAssoc)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_building", ownershipRightObjectAssoc.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_ownership_right", ownershipRightObjectAssoc.IdOwnershipRight));
        }
    }
}
