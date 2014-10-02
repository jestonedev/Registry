using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Registry.DataModels
{
    public class DataSetManager
    {
        private static DataSet ds = new DataSet();

        public static void AddModel(DataModel model)
        {
            DataTable table = model.Select();
            if (!ds.Tables.Contains(table.TableName))
                ds.Tables.Add(table);
            RebuildRelations();
        }

        private static void RebuildRelations()
        {
            DataSetManager.AddRelation("kladr", "id_street", "buildings", "id_street", true);
            DataSetManager.AddRelation("structure_types", "id_structure_type", "buildings", "id_structure_type", true);
            DataSetManager.AddRelation("fund_types", "id_fund_type", "funds_history", "id_fund_type", true);
            DataSetManager.AddRelation("funds_history", "id_fund", "funds_buildings_assoc", "id_fund", true);
            DataSetManager.AddRelation("funds_history", "id_fund", "funds_premises_assoc", "id_fund", true);
            DataSetManager.AddRelation("restriction_types", "id_restriction_type", "restrictions", "id_restriction_type", true);
            DataSetManager.AddRelation("restrictions", "id_restriction", "restrictions_buildings_assoc", "id_restriction", true);
            DataSetManager.AddRelation("restrictions", "id_restriction", "restrictions_premises_assoc", "id_restriction", true);
            DataSetManager.AddRelation("ownership_right_types", "id_ownership_right_type", "ownership_rights", "id_ownership_right_type", true);
            DataSetManager.AddRelation("ownership_rights", "id_ownership_right", "ownership_buildings_assoc", "id_ownership_right", true);
            DataSetManager.AddRelation("ownership_rights", "id_ownership_right", "ownership_premises_assoc", "id_ownership_right", true);
            DataSetManager.AddRelation("buildings", "id_building", "restrictions_buildings_assoc", "id_building", true);
            DataSetManager.AddRelation("buildings", "id_building", "premises", "id_building", true);
            DataSetManager.AddRelation("buildings", "id_building", "ownership_buildings_assoc", "id_building", true);
            DataSetManager.AddRelation("buildings", "id_building", "funds_buildings_assoc", "id_building", true);
            DataSetManager.AddRelation("premises", "id_premises", "restrictions_premises_assoc", "id_premises", true);
            DataSetManager.AddRelation("premises", "id_premises", "ownership_premises_assoc", "id_premises", true);
            DataSetManager.AddRelation("premises", "id_premises", "funds_premises_assoc", "id_premises", true);
            DataSetManager.AddRelation("premises", "id_premises", "sub_premises", "id_premises", true);
            DataSetManager.AddRelation("premises_types", "id_premises_type", "premises", "id_premises_type", true);
            DataSetManager.AddRelation("premises_kinds", "id_premises_kind", "premises", "id_premises_kind", true);
        }

        private static void AddRelation(string master_table_name, string master_column_name, string slave_table_name, 
            string slave_column_name, bool create_constraints)
        {
            if (!ds.Tables.Contains(master_table_name))
                return;
            if (!ds.Tables.Contains(slave_table_name))
                return;
            if (!ds.Relations.Contains(master_table_name+"_"+slave_table_name))
            {
                DataRelation relation = new DataRelation(master_table_name + "_" + slave_table_name, 
                    ds.Tables[master_table_name].Columns[master_column_name], 
                    ds.Tables[slave_table_name].Columns[slave_column_name], create_constraints);
                ds.Relations.Add(relation);
            }
        }

        public static DataSet GetDataSet()
        {
            return ds;
        }
    }
}
