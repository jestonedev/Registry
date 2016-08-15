using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;

namespace Registry.Viewport.ViewModels
{
    internal sealed class DocumentIssuedByViewModel: SnapshotedViewModel
    {
        public DocumentIssuedByViewModel()
            : base(new Dictionary<string, ViewModelItem>
            {
                { "general", new ViewModelItem(EntityDataModel<DocumentIssuedBy>.GetInstance()) }
            })
        {      
        }

        public override void InitializeSnapshot()
        {
            SnapshotDataSource = new DataTable("snapshot_documents_issued_by")
            {
                Locale = CultureInfo.InvariantCulture
            };
            //Инициируем колонки snapshot-модели
            for (var i = 0; i < this["general"].DataSource.Columns.Count; i++)
                SnapshotDataSource.Columns.Add(new DataColumn(this["general"].DataSource.Columns[i].ColumnName,
                    this["general"].DataSource.Columns[i].DataType));
            LoadSnapshot();
            SnapshotBindingSource = new BindingSource { DataSource = SnapshotDataSource };
        }

        public override void LoadSnapshot()
        {
            SnapshotDataSource.Clear();
            //Загружаем данные snapshot-модели из original-view
            foreach (var row in this["general"].BindingSource)
                SnapshotDataSource.Rows.Add(EntityConverter<DocumentIssuedBy>.ToArray((DataRowView)row));
        }
    }
}
