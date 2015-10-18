using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Globalization;
using Registry.DataModels.DataModels;

namespace Registry.DataModels
{
    public static class DataSetManager
    {
        private static DataSet dataSet = new DataSet();

        public static DataSet DataSet { get { return dataSet; }}

        public static void AddModel(DataModel model)
        {
            if (model == null)
                throw new DataModelException("DataSetManager: Не передана ссылка на модель данных");
            DataTable table = model.Select();
            if (!dataSet.Tables.Contains(table.TableName))
                dataSet.Tables.Add(table);
            RebuildRelations();
        }

        public static void AddTable(DataTable table)
        {
            if (table == null)
                throw new DataModelException("DataSetManager: Не передана ссылка на таблицу");
            if (!dataSet.Tables.Contains(table.TableName))
                dataSet.Tables.Add(table);
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
            DataSetManager.AddRelation("object_states", "id_state", "buildings", "id_state", true);
            DataSetManager.AddRelation("object_states", "id_state", "premises", "id_state", true);
            DataSetManager.AddRelation("object_states", "id_state", "sub_premises", "id_state", true);
            DataSetManager.AddRelation("sub_premises", "id_sub_premises", "funds_sub_premises_assoc", "id_sub_premises", true);
            DataSetManager.AddRelation("sub_premises", "id_sub_premises", "tenancy_sub_premises_assoc", "id_sub_premises", true);
            DataSetManager.AddRelation("tenancy_processes", "id_process", "tenancy_buildings_assoc", "id_process", true);
            DataSetManager.AddRelation("tenancy_processes", "id_process", "tenancy_premises_assoc", "id_process", true);
            DataSetManager.AddRelation("tenancy_processes", "id_process", "tenancy_sub_premises_assoc", "id_process", true);
            DataSetManager.AddRelation("tenancy_processes", "id_process", "tenancy_reasons", "id_process", true);
            DataSetManager.AddRelation("tenancy_processes", "id_process", "tenancy_persons", "id_process", true);
            DataSetManager.AddRelation("tenancy_processes", "id_process", "tenancy_agreements", "id_process", true);
            DataSetManager.AddRelation("tenancy_processes", "id_process", "claims", "id_process", true);
            DataSetManager.AddRelation("tenancy_processes", "id_process", "tenancy_notifies", "id_process", true);
            DataSetManager.AddRelation("rent_types", "id_rent_type", "tenancy_processes", "id_rent_type", true);
            DataSetManager.AddRelation("tenancy_reason_types", "id_reason_type", "tenancy_reasons", "id_reason_type", true);
            DataSetManager.AddRelation("executors", "id_executor", "tenancy_processes", "id_executor", true);
            DataSetManager.AddRelation("executors", "id_executor", "tenancy_agreements", "id_executor", true);
            DataSetManager.AddRelation("warrants", "id_warrant", "tenancy_processes", "id_warrant", true);
            DataSetManager.AddRelation("warrants", "id_warrant", "tenancy_agreements", "id_warrant", true);
            DataSetManager.AddRelation("warrant_doc_types", "id_warrant_doc_type", "warrants", "id_warrant_doc_type", true);
            DataSetManager.AddRelation("kinships", "id_kinship", "tenancy_persons", "id_kinship", true);
            DataSetManager.AddRelation("document_types", "id_document_type", "tenancy_persons", "id_document_type", true);
            DataSetManager.AddRelation("documents_issued_by", "id_document_issued_by", "tenancy_persons", "id_document_issued_by", true);
            DataSetManager.AddRelation("claims", "id_claim", "claim_states", "id_claim", true);
            DataSetManager.AddRelation("claim_state_types", "id_state_type", "claim_states", "id_state_type", true);
            DataSetManager.AddRelation("claim_state_types", "id_state_type", "claim_state_types_relations", "id_state_from", true);
            DataSetManager.AddRelation("claim_state_types", "id_state_type", "claim_state_types_relations", "id_state_to", true);
            DataSetManager.AddRelation("documents_residence", "id_document_residence", "resettle_processes", "id_document_residence", true);
            DataSetManager.AddRelation("resettle_processes", "id_process", "resettle_buildings_from_assoc", "id_process", true);
            DataSetManager.AddRelation("resettle_processes", "id_process", "resettle_buildings_to_assoc", "id_process", true);
            DataSetManager.AddRelation("resettle_processes", "id_process", "resettle_premises_from_assoc", "id_process", true);
            DataSetManager.AddRelation("resettle_processes", "id_process", "resettle_premises_to_assoc", "id_process", true);
            DataSetManager.AddRelation("resettle_processes", "id_process", "resettle_sub_premises_from_assoc", "id_process", true);
            DataSetManager.AddRelation("resettle_processes", "id_process", "resettle_sub_premises_to_assoc", "id_process", true);
            DataSetManager.AddRelation("resettle_processes", "id_process", "resettle_persons", "id_process", true);
        }

        private static void AddRelation(string master_table_name, string master_column_name, string slave_table_name, 
            string slave_column_name, bool create_constraints)
        {
            if (!dataSet.Tables.Contains(master_table_name))
                return;
            if (!dataSet.Tables.Contains(slave_table_name))
                return;
            if (!dataSet.Relations.Contains(master_table_name+"_"+slave_table_name))
            {
                DataRelation relation = new DataRelation(master_table_name + "_" + slave_table_name, 
                    dataSet.Tables[master_table_name].Columns[master_column_name], 
                    dataSet.Tables[slave_table_name].Columns[slave_column_name], create_constraints);
                dataSet.Relations.Add(relation);
            }
        }
    }
}
