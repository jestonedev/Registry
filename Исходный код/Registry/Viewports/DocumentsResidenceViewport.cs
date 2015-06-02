using Registry.DataModels;
using Registry.Entities;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal sealed class DocumentsResidenceViewport: Viewport
    {  
        #region Components
        private System.Windows.Forms.DataGridViewTextBoxColumn id_document_residence;
        private System.Windows.Forms.DataGridViewTextBoxColumn document_residence;
        private System.Windows.Forms.DataGridView dataGridView;
        #endregion Components

        #region Models
        DocumentsResidenceDataModel documents_residence = null;
        DataTable snapshot_documents_residence = new DataTable("snapshot_documents_residence");
        #endregion Models

        #region Views
        BindingSource v_documents_residence = null;
        BindingSource v_snapshot_documents_residence = null;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private DocumentsResidenceViewport()
            : this(null)
        {
        }

        public DocumentsResidenceViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshot_documents_residence.Locale = CultureInfo.InvariantCulture;
        }

        public DocumentsResidenceViewport(DocumentsResidenceViewport documentResidenceViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = documentResidenceViewport.DynamicFilter;
            this.StaticFilter = documentResidenceViewport.StaticFilter;
            this.ParentRow = documentResidenceViewport.ParentRow;
            this.ParentType = documentResidenceViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            List<DocumentResidence> list_from_view = DocumentsIssuedByFromView();
            List<DocumentResidence> list_from_viewport = DocumentsIssuedByFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            bool founded = false;
            for (int i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (int j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_document_residence"], 
                dataRowView["document_residence"]
            };
        }

        private static bool ValidateViewportData(List<DocumentResidence> list)
        {
            foreach (DocumentResidence DocumentResidence in list)
            {
                if (DocumentResidence.DocumentResidenceName == null)
                {
                    MessageBox.Show("Наименование документа-основания на проживание не может быть пустым",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (DocumentResidence.DocumentResidenceName != null && DocumentResidence.DocumentResidenceName.Length > 255)
                {
                    MessageBox.Show("Длина наименования документа-основания на проживание не может превышать 255 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static DocumentResidence RowToDocumentResidence(DataRow row)
        {
            DocumentResidence documentResidence = new DocumentResidence();
            documentResidence.IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence");
            documentResidence.DocumentResidenceName = ViewportHelper.ValueOrNull(row, "document_residence");
            return documentResidence;
        }

        private List<DocumentResidence> DocumentsIssuedByFromViewport()
        {
            List<DocumentResidence> list = new List<DocumentResidence>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    DocumentResidence dib = new DocumentResidence();
                    DataGridViewRow row = dataGridView.Rows[i];
                    dib.IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence");
                    dib.DocumentResidenceName = ViewportHelper.ValueOrNull(row, "document_residence");
                    list.Add(dib);
                }
            }
            return list;
        }

        private List<DocumentResidence> DocumentsIssuedByFromView()
        {
            List<DocumentResidence> list = new List<DocumentResidence>();
            for (int i = 0; i < v_documents_residence.Count; i++)
            {
                DocumentResidence dib = new DocumentResidence();
                DataRowView row = ((DataRowView)v_documents_residence[i]);
                dib.IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence");
                dib.DocumentResidenceName = ViewportHelper.ValueOrNull(row, "document_residence");
                list.Add(dib);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_documents_residence.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            documents_residence = DocumentsResidenceDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            documents_residence.Select();

            v_documents_residence = new BindingSource();
            v_documents_residence.DataMember = "documents_residence";
            v_documents_residence.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < documents_residence.Select().Columns.Count; i++)
                snapshot_documents_residence.Columns.Add(new DataColumn(
                    documents_residence.Select().Columns[i].ColumnName, documents_residence.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_documents_residence.Count; i++)
                snapshot_documents_residence.Rows.Add(DataRowViewToArray(((DataRowView)v_documents_residence[i])));
            v_snapshot_documents_residence = new BindingSource();
            v_snapshot_documents_residence.DataSource = snapshot_documents_residence;
            v_snapshot_documents_residence.CurrentItemChanged += new EventHandler(v_snapshot_documents_issued_by_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_documents_residence;
            id_document_residence.DataPropertyName = "id_document_residence";
            document_residence.DataPropertyName = "document_residence";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            documents_residence.Select().RowChanged += new DataRowChangeEventHandler(DocumentResidenceViewport_RowChanged);
            documents_residence.Select().RowDeleting += new DataRowChangeEventHandler(DocumentResidenceViewport_RowDeleting);
            documents_residence.Select().RowDeleted += DocumentResidenceViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            v_snapshot_documents_residence.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_documents_residence.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_documents_residence.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_documents_residence.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_documents_residence.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_documents_residence.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_documents_residence.Position > -1) && (v_snapshot_documents_residence.Position < (v_snapshot_documents_residence.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_documents_residence.Position > -1) && (v_snapshot_documents_residence.Position < (v_snapshot_documents_residence.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.ResettleDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_documents_residence.AddNew();
            row.EndEdit();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
            }
            documents_residence.Select().RowChanged -= new DataRowChangeEventHandler(DocumentResidenceViewport_RowChanged);
            documents_residence.Select().RowDeleting -= new DataRowChangeEventHandler(DocumentResidenceViewport_RowDeleting);
            documents_residence.Select().RowDeleted -= new DataRowChangeEventHandler(DocumentResidenceViewport_RowDeleted);
            base.OnClosing(e);
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_documents_residence.Position != -1) && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_documents_residence[v_snapshot_documents_residence.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_documents_residence.Clear();
            for (int i = 0; i < v_documents_residence.Count; i++)
                snapshot_documents_residence.Rows.Add(DataRowViewToArray(((DataRowView)v_documents_residence[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            documents_residence.EditingNewRecord = true;
            List<DocumentResidence> list = DocumentsIssuedByFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                documents_residence.EditingNewRecord = false;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = documents_residence.Select().Rows.Find(((DocumentResidence)list[i]).IdDocumentResidence);
                if (row == null)
                {
                    int id_document_residence = DocumentsResidenceDataModel.Insert(list[i]);
                    if (id_document_residence == -1)
                    {
                        sync_views = true;
                        documents_residence.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_documents_residence[i])["id_document_residence"] = id_document_residence;
                    documents_residence.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_documents_residence[i]));
                }
                else
                {

                    if (RowToDocumentResidence(row) == list[i])
                        continue;
                    if (DocumentsResidenceDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        documents_residence.EditingNewRecord = false;
                        return;
                    }
                    row["document_residence"] = list[i].DocumentResidenceName == null ? DBNull.Value : (object)list[i].DocumentResidenceName;
                }
            }
            list = DocumentsIssuedByFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_document_residence"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_document_residence"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_document_residence"].Value == list[i].IdDocumentResidence))
                        row_index = j;
                if (row_index == -1)
                {
                    if (DocumentsResidenceDataModel.Delete(list[i].IdDocumentResidence.Value) == -1)
                    {
                        sync_views = true;
                        documents_residence.EditingNewRecord = false;
                        return;
                    }
                    documents_residence.Select().Rows.Find(((DocumentResidence)list[i]).IdDocumentResidence).Delete();
                }
            }
            sync_views = true;
            documents_residence.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            DocumentsResidenceViewport viewport = new DocumentsResidenceViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        void DocumentResidenceViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void DocumentResidenceViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_documents_residence.Find("id_document_residence", e.Row["id_document_residence"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_documents_residence[row_index]).Delete();
            }
        }

        void DocumentResidenceViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            int row_index = v_snapshot_documents_residence.Find("id_document_residence", e.Row["id_document_residence"]);
            if (row_index == -1 && v_documents_residence.Find("id_document_residence", e.Row["id_document_residence"]) != -1)
            {
                snapshot_documents_residence.Rows.Add(new object[] { 
                        e.Row["id_document_residence"], 
                        e.Row["document_residence"]
                    });
            }
            else
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_documents_residence[row_index]);
                    row["document_residence"] = e.Row["document_residence"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshot_documents_issued_by_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "document_residence":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина наименования документа-основания на проживание не может превышать 255 символов";
                    else
                        if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование документа-основания на проживание не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentsResidenceViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_document_residence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.document_residence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_document_residence,
            this.document_residence});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(671, 463);
            this.dataGridView.TabIndex = 9;
            // 
            // id_document_residence
            // 
            this.id_document_residence.Frozen = true;
            this.id_document_residence.HeaderText = "Идентификатор органа";
            this.id_document_residence.Name = "id_document_residence";
            this.id_document_residence.ReadOnly = true;
            this.id_document_residence.Visible = false;
            // 
            // document_residence
            // 
            this.document_residence.HeaderText = "Наименование";
            this.document_residence.MinimumWidth = 100;
            this.document_residence.Name = "document_residence";
            // 
            // DocumentsResidenceViewport
            // 
            this.ClientSize = new System.Drawing.Size(671, 463);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DocumentsResidenceViewport";
            this.Text = "Виды документов на проживание";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
