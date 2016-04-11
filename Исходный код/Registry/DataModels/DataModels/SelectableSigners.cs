using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    //public class SelectableSigners: DataModel
    public class SelectableSigners : DataModel
    {
        private static SelectableSigners _dataModel;
        private const string SelectQuery = @"SELECT s.id_record, CONCAT(s.surname, ' ', s.`name`, 
                                              IF(s.patronymic IS NULL, '', CONCAT(' ', s.patronymic))) AS snp, id_signer_group
                                            FROM selectable_signers s
                                            WHERE s.deleted = 0";
        private const string TableName = "selectable_head_housing_dep";

        private SelectableSigners()
            : base(null, 1, SelectQuery, TableName)
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
