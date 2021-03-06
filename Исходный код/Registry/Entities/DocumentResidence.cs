﻿using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "resettle_processes")]
    [DataTable(Name = "documents_residence", HasDeletedMark = true)]
    public sealed class DocumentResidence : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdDocumentResidence { get; set; }

         [DataColumn(Name = "document_residence")]
        public string DocumentResidenceName { get; set; }
    }
}
