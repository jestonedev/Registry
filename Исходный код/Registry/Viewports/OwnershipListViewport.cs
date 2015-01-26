using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CustomControls;
using Registry.DataModels;
using Registry.Entities;
using Security;
using System.Globalization;
using Registry.CalcDataModels;

namespace Registry.Viewport
{
    internal sealed class OwnershipListViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        OwnershipsRightsDataModel ownership_rights = null;
        OwnershipRightTypesDataModel ownerships_rights_types = null;
        DataModel ownership_assoc = null;
        DataTable snapshot_ownerships_rights = new DataTable("snapshot_ownerships_rights");
        #endregion Models

        #region Views
        BindingSource v_ownership_rights = null;
        BindingSource v_ownership_right_types = null;
        BindingSource v_ownership_assoc = null;
        BindingSource v_snapshot_ownerships_rights = null;
        #endregion Views
        private DataGridViewTextBoxColumn id_ownership_right;
        private DataGridViewTextBoxColumn number;
        private DataGridViewDateTimeColumn date;
        private DataGridViewTextBoxColumn description;
        private DataGridViewComboBoxColumn id_ownership_right_type;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private OwnershipListViewport()
            : this(null)
        {
        }

        public OwnershipListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            InitializeComponent();
            snapshot_ownerships_rights.Locale = CultureInfo.InvariantCulture;
        }

        public OwnershipListViewport(OwnershipListViewport ownershipListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = ownershipListViewport.DynamicFilter;
            this.StaticFilter = ownershipListViewport.StaticFilter;
            this.ParentRow = ownershipListViewport.ParentRow;
            this.ParentType = ownershipListViewport.ParentType;
        }

        private void RebuildFilter()
        {
            string ownershipFilter = "id_ownership_right IN (0";
            for (int i = 0; i < v_ownership_assoc.Count; i++)
                ownershipFilter += ((DataRowView)v_ownership_assoc[i])["id_ownership_right"].ToString() + ",";
            ownershipFilter = ownershipFilter.TrimEnd(new char[] { ',' });
            ownershipFilter += ")";
            v_ownership_rights.Filter = ownershipFilter;
        }

        private bool SnapshotHasChanges()
        {
            List<OwnershipRight> list_from_view = OwnershipRightsFromView();
            List<OwnershipRight> list_from_viewport = OwnershipRightsFromViewport();
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
                dataRowView["id_ownership_right"], 
                dataRowView["id_ownership_right_type"], 
                dataRowView["number"], 
                dataRowView["date"], 
                dataRowView["description"]
            };
        }

        private bool ValidatePermissions()
        {
            EntityType entity = EntityType.Unknown;
            string fieldName = null;
            if (ParentType == ParentTypeEnum.Building)
            {
                entity = EntityType.Building;
                fieldName = "id_building";
            }
            else
                if (ParentType == ParentTypeEnum.Premises)
                {
                    entity = EntityType.Premise;
                    fieldName = "id_premises";
                }
            if (DataModelHelper.HasMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("У вас нет прав на изменение информации об ограничениях муниципальных объектов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.HasNotMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show("У вас нет прав на изменение информации об ограничениях немуниципальных объектов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private bool ValidateViewportData(List<OwnershipRight> list)
        {
            if (ValidatePermissions() == false)
                return false;
            foreach (OwnershipRight ownershipRight in list)
            {
                if (ownershipRight.Number != null && ownershipRight.Number.Length > 10)
                {
                    MessageBox.Show("Номер основания не может привышать 10 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.Date == null)
                {
                    MessageBox.Show("Не заполнена дата начала действия ограничения, установленного в отношении муниципальной собственности",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.Description != null && ownershipRight.Description.Length > 255)
                {
                    MessageBox.Show("Длина наименования ограничения не может превышать 255 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.IdOwnershipRightType == null)
                {
                    MessageBox.Show("Не выбран тип ограничения, установленного в отношении муниципальной собственности", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static OwnershipRight RowToOwnershipRight(DataRow row)
        {
            OwnershipRight ownershipRight = new OwnershipRight();
            ownershipRight.IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right");
            ownershipRight.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
            ownershipRight.Number = ViewportHelper.ValueOrNull(row, "number");
            ownershipRight.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
            ownershipRight.Description = ViewportHelper.ValueOrNull(row, "description");
            return ownershipRight;
        }

        private List<OwnershipRight> OwnershipRightsFromViewport()
        {
            List<OwnershipRight> list = new List<OwnershipRight>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    OwnershipRight or = new OwnershipRight();
                    DataGridViewRow row = dataGridView.Rows[i];
                    or.IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right");
                    or.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                    or.Number = ViewportHelper.ValueOrNull(row, "number");
                    or.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
                    or.Description = ViewportHelper.ValueOrNull(row, "description");
                    list.Add(or);
                }
            }
            return list;
        }

        private List<OwnershipRight> OwnershipRightsFromView()
        {
            List<OwnershipRight> list = new List<OwnershipRight>();
            for (int i = 0; i < v_ownership_rights.Count; i++)
            {
                OwnershipRight or = new OwnershipRight();
                DataRowView row = ((DataRowView)v_ownership_rights[i]);
                or.IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right");
                or.IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type");
                or.Number = ViewportHelper.ValueOrNull(row, "number");
                or.Date = ViewportHelper.ValueOrNull<DateTime>(row, "date");
                or.Description = ViewportHelper.ValueOrNull(row, "description");
                list.Add(or);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_ownerships_rights.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            ownership_rights = OwnershipsRightsDataModel.GetInstance();
            ownerships_rights_types = OwnershipRightTypesDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            ownership_rights.Select();
            ownerships_rights_types.Select();
            if (ParentType == ParentTypeEnum.Premises)
                ownership_assoc = OwnershipPremisesAssocDataModel.GetInstance();
            else
                if (ParentType == ParentTypeEnum.Building)
                    ownership_assoc = OwnershipBuildingsAssocDataModel.GetInstance();
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");
            ownership_assoc.Select();

            v_ownership_assoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
            {
                v_ownership_assoc.DataMember = "ownership_premises_assoc";
                v_ownership_assoc.Filter = "id_premises = " + ParentRow["id_premises"].ToString();
                this.Text = String.Format(CultureInfo.InvariantCulture, "Ограничения помещения №{0}", ParentRow["id_premises"].ToString());
            }
            else
                if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                {
                    v_ownership_assoc.DataMember = "ownership_buildings_assoc";
                    v_ownership_assoc.Filter = "id_building = " + ParentRow["id_building"].ToString();
                    this.Text = String.Format(CultureInfo.InvariantCulture, "Ограничения здания №{0}", ParentRow["id_building"].ToString());
                }
                else
                    throw new ViewportException("Неизвестный тип родительского объекта");
            v_ownership_assoc.DataSource = DataSetManager.DataSet;

            v_ownership_rights = new BindingSource();
            v_ownership_rights.DataMember = "ownership_rights";
            v_ownership_rights.DataSource = DataSetManager.DataSet;
            //Перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();

            v_ownership_right_types = new BindingSource();
            v_ownership_right_types.DataMember = "ownership_right_types";
            v_ownership_right_types.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < ownership_rights.Select().Columns.Count; i++)
                snapshot_ownerships_rights.Columns.Add(new DataColumn(ownership_rights.Select().Columns[i].ColumnName, 
                    ownership_rights.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_ownership_rights.Count; i++)
                snapshot_ownerships_rights.Rows.Add(DataRowViewToArray(((DataRowView)v_ownership_rights[i])));
            v_snapshot_ownerships_rights = new BindingSource();
            v_snapshot_ownerships_rights.DataSource = snapshot_ownerships_rights;
            v_snapshot_ownerships_rights.CurrentItemChanged += new EventHandler(v_snapshot_ownerships_rights_CurrentItemChanged);
            snapshot_ownerships_rights.RowChanged += snapshot_ownerships_rights_RowChanged;
            snapshot_ownerships_rights.RowDeleted += snapshot_ownerships_rights_RowDeleted;

            dataGridView.DataSource = v_snapshot_ownerships_rights;

            id_ownership_right.DataPropertyName = "id_ownership_right";
            id_ownership_right_type.DataSource = v_ownership_right_types;
            id_ownership_right_type.ValueMember = "id_ownership_right_type";
            id_ownership_right_type.DisplayMember = "ownership_right_type";
            id_ownership_right_type.DataPropertyName = "id_ownership_right_type";
            number.DataPropertyName = "number";
            date.DataPropertyName = "date";
            description.DataPropertyName = "description";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);

            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            ownership_rights.Select().RowChanged += new DataRowChangeEventHandler(OwnershipListViewport_RowChanged);
            ownership_rights.Select().RowDeleting += new DataRowChangeEventHandler(OwnershipListViewport_RowDeleting);
            ownership_assoc.Select().RowChanged += new DataRowChangeEventHandler(OwnershipAssoc_RowChanged);
            ownership_assoc.Select().RowDeleted += new DataRowChangeEventHandler(OwnershipAssoc_RowDeleted);
        }
        
        public override void MoveFirst()
        {
            v_snapshot_ownerships_rights.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_ownerships_rights.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_ownerships_rights.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_ownerships_rights.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_ownerships_rights.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_ownerships_rights.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_ownerships_rights.Position > -1) && (v_snapshot_ownerships_rights.Position < (v_snapshot_ownerships_rights.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_ownerships_rights.Position > -1) && (v_snapshot_ownerships_rights.Position < (v_snapshot_ownerships_rights.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_ownerships_rights.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_ownerships_rights.Position != -1) && 
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_ownerships_rights[v_snapshot_ownerships_rights.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_ownerships_rights.Clear();
            for (int i = 0; i < v_ownership_rights.Count; i++)
                snapshot_ownerships_rights.Rows.Add(DataRowViewToArray(((DataRowView)v_ownership_rights[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && 
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<OwnershipRight> list = OwnershipRightsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = ownership_rights.Select().Rows.Find(((OwnershipRight)list[i]).IdOwnershipRight);
                if (row == null)
                {
                    int id_parent = ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] :
                        ((ParentType == ParentTypeEnum.Building) && ParentRow != null) ? (int)ParentRow["id_building"] :
                        -1;
                    if (id_parent == -1)
                    {
                        MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору", 
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    int id_ownership_right = OwnershipsRightsDataModel.Insert(list[i], ParentType, id_parent);
                    if (id_ownership_right == -1)
                    {
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    ((DataRowView)v_snapshot_ownerships_rights[i])["id_ownership_right"] = id_ownership_right;
                    ownership_rights.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_ownerships_rights[i]));
                    ownership_assoc.Select().Rows.Add(new object[] { id_parent, id_ownership_right });
                }
                else
                {
                    if (RowToOwnershipRight(row) == list[i])
                        continue;
                    if (OwnershipsRightsDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    row["id_ownership_right_type"] = list[i].IdOwnershipRightType == null ? DBNull.Value : (object)list[i].IdOwnershipRightType;
                    row["number"] = list[i].Number == null ? DBNull.Value : (object)list[i].Number;
                    row["date"] = list[i].Date == null ? DBNull.Value : (object)list[i].Date;
                    row["description"] = list[i].Description == null ? DBNull.Value : (object)list[i].Description;
                }
            }
            list = OwnershipRightsFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_ownership_right"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_ownership_right"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_ownership_right"].Value == list[i].IdOwnershipRight))
                        row_index = j;
                if (row_index == -1)
                {
                    if (OwnershipsRightsDataModel.Delete(list[i].IdOwnershipRight.Value) == -1)
                    {
                        sync_views = true;
                        RebuildFilter();
                        return;
                    }
                    ownership_rights.Select().Rows.Find(((OwnershipRight)list[i]).IdOwnershipRight).Delete();
                    RebuildFilter();
                }
            }
            RebuildFilter();
            sync_views = true;
            MenuCallback.EditingStateUpdate();
            if (ParentType == ParentTypeEnum.Premises || ParentType == ParentTypeEnum.Building)
                CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                    Int32.Parse(ParentRow["id_building"].ToString(), CultureInfo.InvariantCulture), true);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            OwnershipListViewport viewport = new OwnershipListViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения об ограничениях в базу данных?", "Внимание",
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
            ownership_rights.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipListViewport_RowChanged);
            ownership_rights.Select().RowDeleting -= new DataRowChangeEventHandler(OwnershipListViewport_RowDeleting);
            ownership_assoc.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipAssoc_RowChanged);
            ownership_assoc.Select().RowDeleted -= new DataRowChangeEventHandler(OwnershipAssoc_RowDeleted);
        }

        public override void ForceClose()
        {
            ownership_rights.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipListViewport_RowChanged);
            ownership_rights.Select().RowDeleting -= new DataRowChangeEventHandler(OwnershipListViewport_RowDeleting);
            ownership_assoc.Select().RowChanged -= new DataRowChangeEventHandler(OwnershipAssoc_RowChanged);
            ownership_assoc.Select().RowDeleted -= new DataRowChangeEventHandler(OwnershipAssoc_RowDeleted);
            base.Close();
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "number":
                    if (cell.Value.ToString().Trim().Length > 10)
                        cell.ErrorText = "Длина номера ограничения не может превышать 10 символов";
                    else
                        cell.ErrorText = "";
                    break;
                case "date":
                    if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                        cell.ErrorText = "Не заполнена дата начала действия ограничения, установленного в отношении муниципальной собственности";
                    else
                        cell.ErrorText = "";
                    break;
                case "description":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина наименования ограничения не может превышать 255 символов";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        void snapshot_ownerships_rights_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void snapshot_ownerships_rights_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add && Selected)
            {
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
            }
        }

        void OwnershipAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            //Если добавлена новая ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();
            //Если в модели есть запись, а в снапшоте нет, то добавляем в снапшот
            if (e.Row["id_ownership_right"] == DBNull.Value)
                return;
            int row_index = v_ownership_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
            if (row_index == -1)
                return;
            DataRowView row = (DataRowView)v_ownership_rights[row_index];
            if ((v_snapshot_ownerships_rights.Find("id_ownership_right", e.Row["id_ownership_right"]) == -1) && (row_index != -1))
            {
                snapshot_ownerships_rights.Rows.Add(new object[] { 
                            row["id_ownership_right"], 
                            row["id_ownership_right_type"],
                            row["number"],
                            row["date"],
                            row["description"]
                        });
            }
        }

        void OwnershipAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            //Если удалена ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            if (e.Action == DataRowAction.Delete)
                RebuildFilter();
        }

        void OwnershipListViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_ownerships_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_ownerships_rights[row_index]).Delete();
            }
        }

        void OwnershipListViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_ownerships_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_ownerships_rights[row_index]);
                    row["id_ownership_right_type"] = e.Row["id_ownership_right_type"];
                    row["number"] = e.Row["number"];
                    row["date"] = e.Row["date"];
                    row["description"] = e.Row["description"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    //Если строка имеется в текущем контексте оригинального представления, то добавить его и в snapshot, 
                    //иначе - объект не принадлежит текущему родителю
                    int row_index = v_ownership_rights.Find("id_ownership_right", e.Row["id_ownership_right"]);
                    if (row_index != -1)
                        snapshot_ownerships_rights.Rows.Add(new object[] { 
                            e.Row["id_ownership_right"], 
                            e.Row["id_ownership_right_type"],
                            e.Row["number"],
                            e.Row["date"],
                            e.Row["description"]
                        });
                }
        }

        void v_snapshot_ownerships_rights_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OwnershipListViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_ownership_right = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new CustomControls.DataGridViewDateTimeColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_ownership_right_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
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
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_ownership_right,
            this.number,
            this.date,
            this.description,
            this.id_ownership_right_type});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(819, 328);
            this.dataGridView.TabIndex = 2;
            // 
            // id_ownership_right
            // 
            this.id_ownership_right.HeaderText = "Идентификатор основания";
            this.id_ownership_right.Name = "id_ownership_right";
            this.id_ownership_right.Visible = false;
            // 
            // number
            // 
            this.number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.number.HeaderText = "Номер";
            this.number.MinimumWidth = 150;
            this.number.Name = "number";
            this.number.Width = 150;
            // 
            // date
            // 
            this.date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.date.HeaderText = "Дата";
            this.date.MinimumWidth = 150;
            this.date.Name = "date";
            this.date.Width = 150;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Наименование";
            this.description.MinimumWidth = 300;
            this.description.Name = "description";
            // 
            // id_ownership_right_type
            // 
            this.id_ownership_right_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.id_ownership_right_type.HeaderText = "Тип ограничения";
            this.id_ownership_right_type.MinimumWidth = 150;
            this.id_ownership_right_type.Name = "id_ownership_right_type";
            this.id_ownership_right_type.Width = 150;
            // 
            // OwnershipListViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(825, 334);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OwnershipListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Ограничения";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
