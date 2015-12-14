using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal class SelectableHeadHousingDepDataModel: DataModel
    {
        private static SelectableHeadHousingDepDataModel _dataModel;
        private const string SelectQuery = @"SELECT s.id_record, CONCAT(s.surname, ' ', s.`name`, 
                                              IF(s.patronymic IS NULL, '', CONCAT(' ', s.patronymic))) AS snp
                                            FROM selectable_head_housing_dep s
                                            WHERE s.deleted = 0";
        private const string TableName = "selectable_head_housing_dep";

        private SelectableHeadHousingDepDataModel()
            : base(null, 1, SelectQuery, TableName)
        {
        }

        public static SelectableHeadHousingDepDataModel GetInstance()
        {
            return _dataModel ?? (_dataModel = new SelectableHeadHousingDepDataModel());
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_record"] };
        }
    }
}
