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

        public static void AddTable(DataTable table)
        {
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
            DataSetManager.AddRelation("funds_history", "id_fund", "funds_sub_premises_assoc", "id_fund", true);
            DataSetManager.AddRelation("restriction_types", "id_restriction_type", "restrictions", "id_restriction_type", true);
            DataSetManager.AddRelation("restrictions", "id_restriction", "restrictions_buildings_assoc", "id_restriction", true);
            DataSetManager.AddRelation("restrictions", "id_restriction", "restrictions_premises_assoc", "id_restriction", true);
            DataSetManager.AddRelation("ownership_right_types", "id_ownership_right_type", 
                "ownership_rights", "id_ownership_right_type", true);
            DataSetManager.AddRelation("ownership_rights", "id_ownership_right", 
                "ownership_buildings_assoc", "id_ownership_right", true);
            DataSetManager.AddRelation("ownership_rights", "id_ownership_right", 
                "ownership_premises_assoc", "id_ownership_right", true);
            DataSetManager.AddRelation("buildings", "id_building", "restrictions_buildings_assoc", "id_building", true);
            DataSetManager.AddRelation("buildings", "id_building", "premises", "id_building", true);
            DataSetManager.AddRelation("buildings", "id_building", "ownership_buildings_assoc", "id_building", true);
            DataSetManager.AddRelation("buildings", "id_building", "funds_buildings_assoc", "id_building", true);
            DataSetManager.AddRelation("building", "id_building", "tenancy_buildings_assoc", "id_building", true);
            DataSetManager.AddRelation("premises", "id_premises", "restrictions_premises_assoc", "id_premises", true);
            DataSetManager.AddRelation("premises", "id_premises", "ownership_premises_assoc", "id_premises", true);
            DataSetManager.AddRelation("premises", "id_premises", "funds_premises_assoc", "id_premises", true);
            DataSetManager.AddRelation("premises", "id_premises", "sub_premises", "id_premises", true);
            DataSetManager.AddRelation("premises", "id_premises", "tenancy_premises_assoc", "id_premises", true);
            DataSetManager.AddRelation("premises_types", "id_premises_type", "premises", "id_premises_type", true);
            DataSetManager.AddRelation("premises_kinds", "id_premises_kind", "premises", "id_premises_kind", true);
            DataSetManager.AddRelation("states", "id_state", "buildings", "id_state", true);
            DataSetManager.AddRelation("states", "id_state", "premises", "id_state", true);
            DataSetManager.AddRelation("states", "id_state", "sub_premises", "id_state", true);
            DataSetManager.AddRelation("sub_premises", "id_sub_premises", "funds_sub_premises_assoc", "id_sub_premises", true);
            DataSetManager.AddRelation("sub_premises", "id_sub_premises", "tenancy_sub_premises_assoc", "id_sub_premises", true);
            DataSetManager.AddRelation("tenancty_contracts", "id_contract", "tenancy_buildings_assoc", "id_contract", true);
            DataSetManager.AddRelation("tenancty_contracts", "id_contract", "tenancy_premises_assoc", "id_contract", true);
            DataSetManager.AddRelation("tenancty_contracts", "id_contract", "tenancy_sub_premises_assoc", "id_contract", true);
            DataSetManager.AddRelation("tenancty_contracts", "id_contract", "contract_reasons", "id_contract", true);
            DataSetManager.AddRelation("tenancty_contracts", "id_contract", "persons", "id_contract", true);
            DataSetManager.AddRelation("tenancty_contracts", "id_contract", "agreements", "id_contract", true);
            DataSetManager.AddRelation("rent_types", "id_rent_type", "tenancty_contracts", "id_rent_type", true);
            DataSetManager.AddRelation("reason_types", "id_reason_type", "contract_reasons", "id_reason_type", true);
            DataSetManager.AddRelation("executors", "id_executor", "tenancty_contracts", "id_executor", true);
            DataSetManager.AddRelation("executors", "id_executor", "agreements", "id_executor", true);
            DataSetManager.AddRelation("warrants", "id_warrant", "tenancty_contracts", "id_warrant", true);
            DataSetManager.AddRelation("warrants", "id_warrant", "agreement", "id_warrant", true);
            DataSetManager.AddRelation("warrant_doc_types", "id_warrant_doc_type", "warrants", "id_warrant_doc_type", true);
            DataSetManager.AddRelation("kinships", "id_kinship", "persons", "id_kinship", true);
            DataSetManager.AddRelation("document_types", "id_document_type", "persons", "id_document_type", true);
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
