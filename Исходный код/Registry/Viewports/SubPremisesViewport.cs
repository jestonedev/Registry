using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.DataModels;
using Registry.Entities;
using Registry.CalcDataModels;
using System.Text.RegularExpressions;
using CustomControls;
using Security;
using System.Globalization;
using Registry.Reporting;

namespace Registry.Viewport
{
    internal sealed class SubPremisesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        SubPremisesDataModel sub_premises = null;
        ObjectStatesDataModel object_states = null;
        DataTable snapshot_sub_premises = new DataTable("snapshot_sub_premises");
        #endregion Models

        #region Views
        BindingSource v_sub_premises = null;
        BindingSource v_object_states = null;
        BindingSource v_snapshot_sub_premises = null;
        #endregion Views
        private DataGridViewTextBoxColumn id_sub_premises;
        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn sub_premises_num;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn description;
        private DataGridViewComboBoxColumn id_state;
        private DataGridViewDateTimeColumn state_date;


        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private SubPremisesViewport()
            : this(null)
        {
        }

        public SubPremisesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshot_sub_premises.Locale = CultureInfo.InvariantCulture;
        }

        public SubPremisesViewport(SubPremisesViewport subPremisesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = subPremisesViewport.DynamicFilter;
            this.StaticFilter = subPremisesViewport.StaticFilter;
            this.ParentRow = subPremisesViewport.ParentRow;
            this.ParentType = subPremisesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            List<SubPremise> list_from_view = SubPremisesFromView();
            List<SubPremise> list_from_viewport = SubPremisesFromViewport();
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
                dataRowView["id_sub_premises"], 
                dataRowView["id_premises"], 
                dataRowView["id_state"], 
                dataRowView["sub_premises_num"], 
                dataRowView["total_area"],
                dataRowView["living_area"],
                dataRowView["description"],
                dataRowView["state_date"]
            };
        }

        private static bool ValidateSubPremises(List<SubPremise> subPremises)
        {
            foreach (SubPremise subPremise in subPremises)
            {
                if (subPremise.IdState == null)
                {
                    MessageBox.Show("Необходимо выбрать состояние помещения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.SubPremisesNum != null && subPremise.SubPremisesNum.Length > 20)
                {
                    MessageBox.Show("Длина номера комнаты не может превышать 20 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.SubPremisesNum != null && !Regex.IsMatch(subPremise.SubPremisesNum, "^([0-9]+[а-я]{0,1}|[а-я])$"))
                {
                    MessageBox.Show("Некорректно задан номер комнаты. Можно использовать только цифры и не более одной строчной буквы кириллицы", 
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.Description != null && subPremise.Description.Length > 65535)
                {
                    MessageBox.Show("Длина примечания комнаты не может превышать 65535 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SubPremise RowToSubPremise(DataRow row)
        {
            SubPremise subPremise = new SubPremise();
            subPremise.IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
            subPremise.IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises");
            subPremise.IdState = ViewportHelper.ValueOrNull<int>(row, "id_state");
            subPremise.SubPremisesNum = ViewportHelper.ValueOrNull(row, "sub_premises_num");
            subPremise.TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area");
            subPremise.LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area");
            subPremise.Description = ViewportHelper.ValueOrNull(row, "description");
            subPremise.StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date");
            return subPremise;
        }

        private List<SubPremise> SubPremisesFromViewport()
        {
            List<SubPremise> list = new List<SubPremise>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    SubPremise sp = new SubPremise();
                    DataGridViewRow row = dataGridView.Rows[i];
                    sp.IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
                    sp.IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises");
                    sp.IdState = ViewportHelper.ValueOrNull<int>(row, "id_state");
                    sp.SubPremisesNum = ViewportHelper.ValueOrNull(row, "sub_premises_num");
                    sp.TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area");
                    if (ViewportHelper.ValueOrNull<double>(row, "living_area") == 0)
                        dataGridView.Rows[i].Cells["living_area"].Value = sp.TotalArea.Value;
                    sp.LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area");
                    sp.Description = ViewportHelper.ValueOrNull(row, "description");
                    sp.StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date");
                    list.Add(sp);
                }
            }
            return list;
        }

        private List<SubPremise> SubPremisesFromView()
        {
            List<SubPremise> list = new List<SubPremise>();
            for (int i = 0; i < v_sub_premises.Count; i++)
            {
                SubPremise sp = new SubPremise();
                DataRowView row = ((DataRowView)v_sub_premises[i]);
                sp.IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises");
                sp.IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises");
                sp.IdState = ViewportHelper.ValueOrNull<int>(row, "id_state");
                sp.SubPremisesNum = ViewportHelper.ValueOrNull(row, "sub_premises_num");
                sp.TotalArea = ViewportHelper.ValueOrNull<double>(row, "total_area");
                sp.LivingArea = ViewportHelper.ValueOrNull<double>(row, "living_area");
                sp.Description = ViewportHelper.ValueOrNull(row, "description");
                sp.StateDate = ViewportHelper.ValueOrNull<DateTime>(row, "state_date");
                list.Add(sp);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_sub_premises.Count;
        }

        public override void MoveFirst()
        {
            v_snapshot_sub_premises.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_sub_premises.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_sub_premises.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_sub_premises.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_sub_premises.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_sub_premises.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_sub_premises.Position > -1) && (v_snapshot_sub_premises.Position < (v_snapshot_sub_premises.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_sub_premises.Position > -1) && (v_snapshot_sub_premises.Position < (v_snapshot_sub_premises.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            sub_premises = SubPremisesDataModel.GetInstance();
            object_states = ObjectStatesDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            sub_premises.Select();
            object_states.Select();

            v_object_states = new BindingSource();
            v_object_states.DataMember = "object_states";
            v_object_states.DataSource = DataSetManager.DataSet;

            v_sub_premises = new BindingSource();
            v_sub_premises.DataMember = "sub_premises";
            v_sub_premises.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_sub_premises.Filter += " AND ";
            v_sub_premises.Filter += DynamicFilter;
            v_sub_premises.DataSource = DataSetManager.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.Premises)
                this.Text = String.Format(CultureInfo.InvariantCulture, "Комнаты помещения №{0}", ParentRow["id_premises"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < sub_premises.Select().Columns.Count; i++)
                snapshot_sub_premises.Columns.Add(new DataColumn(sub_premises.Select().Columns[i].ColumnName, sub_premises.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_sub_premises.Count; i++)
                snapshot_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_sub_premises[i])));
            v_snapshot_sub_premises = new BindingSource();
            v_snapshot_sub_premises.DataSource = snapshot_sub_premises;
            v_snapshot_sub_premises.CurrentItemChanged += new EventHandler(v_snapshot_sub_premises_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_sub_premises;
            id_sub_premises.DataPropertyName = "id_sub_premises";
            id_premises.DataPropertyName = "id_premises";
            sub_premises_num.DataPropertyName = "sub_premises_num";
            total_area.DataPropertyName = "total_area";
            living_area.DataPropertyName = "living_area";
            description.DataPropertyName = "description";
            id_state.DataSource = v_object_states;
            id_state.ValueMember = "id_state";
            id_state.DisplayMember = "state_female";
            id_state.DataPropertyName = "id_state";
            state_date.DataPropertyName = "state_date";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView_EditingControlShowing);
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            dataGridView.DataError += dataGridView_DataError;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            sub_premises.Select().RowChanged += new DataRowChangeEventHandler(SubPremisesViewport_RowChanged);
            sub_premises.Select().RowDeleting += new DataRowChangeEventHandler(SubPremisesViewport_RowDeleting);
            sub_premises.Select().RowDeleted += SubPremisesViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return (ParentType == ParentTypeEnum.Premises) && (ParentRow != null) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            if ((ParentRow == null) || (ParentType != ParentTypeEnum.Premises))
                return;
            DataRowView row = (DataRowView)v_snapshot_sub_premises.AddNew();
            row["id_premises"] = ParentRow["id_premises"];
            row["total_area"] = 0;
            row["living_area"] = 0;
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_sub_premises.Position != -1) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_sub_premises[v_snapshot_sub_premises.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_sub_premises.Clear();
            for (int i = 0; i < v_sub_premises.Count; i++)
                snapshot_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_sub_premises[i])));
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
            sub_premises.EditingNewRecord = true;
            List<SubPremise> list = SubPremisesFromViewport();
            if (!ValidateSubPremises(list))
            {
                sync_views = true;
                sub_premises.EditingNewRecord = false;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = sub_premises.Select().Rows.Find(((SubPremise)list[i]).IdSubPremises);
                if (row == null)
                {
                    if (new int[] { 4, 5 }.Contains(list[i].IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                    {
                        MessageBox.Show("У вас нет прав на добавление в базу муниципальных жилых помещений", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    if (new int[] { 1, 3 }.Contains(list[i].IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                    {
                        MessageBox.Show("У вас нет прав на добавление в базу немуниципальных жилых помещений", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    int id_sub_premises = SubPremisesDataModel.Insert(list[i]);
                    if (id_sub_premises == -1)
                    {
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_sub_premises[i])["id_sub_premises"] = id_sub_premises;
                    sub_premises.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_sub_premises[i]));
                }
                else
                {
                    SubPremise subPremiseFromView = RowToSubPremise(row);
                    if (subPremiseFromView == list[i])
                        continue;
                    if (DataModelHelper.HasMunicipal(subPremiseFromView.IdSubPremises.Value, EntityType.SubPremise)
                        && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                    {
                        MessageBox.Show("Вы не можете изменить информацию по данной комнате, т.к. она является муниципальной",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    if (DataModelHelper.HasNotMunicipal(subPremiseFromView.IdSubPremises.Value, EntityType.SubPremise)
                        && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                    {
                        MessageBox.Show("Вы не можете изменить информацию по данной комнате, т.к. она является немуниципальной",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    if (SubPremisesDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    row["id_premises"] = list[i].IdPremises == null ? DBNull.Value : (object)list[i].IdPremises;
                    row["id_state"] = list[i].IdState == null ? DBNull.Value : (object)list[i].IdState;
                    row["sub_premises_num"] = list[i].SubPremisesNum == null ? DBNull.Value : (object)list[i].SubPremisesNum;
                    row["total_area"] = list[i].TotalArea == null ? DBNull.Value : (object)list[i].TotalArea;
                    row["living_area"] = list[i].LivingArea == null ? DBNull.Value : (object)list[i].LivingArea;
                    row["description"] = list[i].Description == null ? DBNull.Value : (object)list[i].Description;
                    row["state_date"] = list[i].StateDate == null ? DBNull.Value : (object)list[i].StateDate;
                }
            }
            list = SubPremisesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_sub_premises"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_sub_premises"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_sub_premises"].Value == list[i].IdSubPremises))
                        row_index = j;
                if (row_index == -1)
                {
                    if (DataModelHelper.HasMunicipal(list[i].IdSubPremises.Value, EntityType.SubPremise)
                        && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                    {
                        MessageBox.Show("Вы не можете удалить муниципальную комнату, т.к. не имеете на это прав",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    if (DataModelHelper.HasNotMunicipal(list[i].IdSubPremises.Value, EntityType.SubPremise)
                        && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                    {
                        MessageBox.Show("Вы не можете удалить немуниципальную комнату, т.к. не имеете на это прав",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    if (SubPremisesDataModel.Delete(list[i].IdSubPremises.Value) == -1)
                    {
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    sub_premises.Select().Rows.Find(((SubPremise)list[i]).IdSubPremises).Delete();
                }
            }
            sync_views = true;
            sub_premises.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
            CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
            CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, false);
            CalcDataModelPremiseSubPremisesSumArea.GetInstance().Refresh(EntityType.Premise,
                Int32.Parse(ParentRow["id_premises"].ToString(), CultureInfo.InvariantCulture), true);
            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Building,
                Int32.Parse(ParentRow["id_building"].ToString(), CultureInfo.InvariantCulture), true);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            SubPremisesViewport viewport = new SubPremisesViewport(this, MenuCallback);
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
                DialogResult result = MessageBox.Show("Сохранить изменения о комнатах в базу данных?", "Внимание",
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
            sub_premises.Select().RowChanged -= new DataRowChangeEventHandler(SubPremisesViewport_RowChanged);
            sub_premises.Select().RowDeleting -= new DataRowChangeEventHandler(SubPremisesViewport_RowDeleting);
            sub_premises.Select().RowDeleted -= new DataRowChangeEventHandler(SubPremisesViewport_RowDeleted);
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            sub_premises.Select().RowChanged -= new DataRowChangeEventHandler(SubPremisesViewport_RowChanged);
            sub_premises.Select().RowDeleting -= new DataRowChangeEventHandler(SubPremisesViewport_RowDeleting);
            sub_premises.Select().RowDeleted -= new DataRowChangeEventHandler(SubPremisesViewport_RowDeleted);
            base.Close();
        }

        public override bool HasAssocFundHistory()
        {
            return (v_snapshot_sub_premises.Count > 0);
        }

        public override bool HasAssocTenancies()
        {
            return (v_snapshot_sub_premises.Count > 0);
        }

        public override bool HasRegistryExcerptPremiseReport()
        {
            return true;
        }

        public override bool HasRegistryExcerptSubPremiseReport()
        {
            return (v_snapshot_sub_premises.Count > 0);
        }

        public override bool HasRegistryExcerptSubPremisesReport()
        {
            return true;
        }

        public override void RegistryExcerptPremiseReportGenerate()
        {
            Reporter reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("ids", ParentRow["id_premises"].ToString());
            arguments.Add("excerpt_type", "1");
            reporter.Run(arguments);
        }

        public override void RegistryExcerptSubPremiseReportGenerate()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Перед формированием выписки по комнате необходимо сохранить изменения в базу данных. " +
                    "Вы хотите это сделать?", "Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            if (SnapshotHasChanges())
                return;
            if (v_snapshot_sub_premises.Position == -1)
            {
                MessageBox.Show("Не выбрана комната для формирования выписки", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            Reporter reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)v_snapshot_sub_premises[v_snapshot_sub_premises.Position])["id_sub_premises"].ToString());
            arguments.Add("excerpt_type", "2");
            reporter.Run(arguments);
        }

        public override void RegistryExcerptSubPremisesReportGenerate()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Перед формированием выписки по муниципальным комнатам необходимо сохранить изменения в базу данных. " +
                    "Вы хотите это сделать?", "Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            if (SnapshotHasChanges())
                return;
            Reporter reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("ids", ParentRow["id_premises"].ToString());
            arguments.Add("excerpt_type", "3");
            reporter.Run(arguments);
        }

        public override void ShowFundHistory()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Перед открытием истории принадлежности фондам необходимо сохранить изменения в базу данных. "+
                    "Вы хотите это сделать?", "Внимание", 
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            if (SnapshotHasChanges())
                return;
            if (v_snapshot_sub_premises.Position == -1)
            {
                MessageBox.Show("Не выбрана комната для отображения истории принадлежности к фондам", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, ViewportType.FundsHistoryViewport, "id_sub_premises = " +
                Convert.ToInt32(((DataRowView)v_snapshot_sub_premises[v_snapshot_sub_premises.Position])["id_sub_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)v_snapshot_sub_premises[v_snapshot_sub_premises.Position]).Row, ParentTypeEnum.SubPremises);
        }

        public override void ShowTenancies()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Перед открытием истории найма необходимо сохранить изменения в базу данных. Вы хотите это сделать?",
                    "Внимание", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            if (SnapshotHasChanges())
                return;
            if (v_snapshot_sub_premises.Position == -1)
            {
                MessageBox.Show("Не выбрана комната для отображения истории найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, ViewportType.TenancyListViewport, "id_sub_premises = " +
                Convert.ToInt32(((DataRowView)v_snapshot_sub_premises[v_snapshot_sub_premises.Position])["id_sub_premises"], CultureInfo.InvariantCulture),
                ((DataRowView)v_snapshot_sub_premises[v_snapshot_sub_premises.Position]).Row, ParentTypeEnum.SubPremises);
        }

        void v_snapshot_sub_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
                MenuCallback.DocumentsStateUpdate();
            }
        }
        
        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "total_area":
                case "living_area":
                    double stub = 0;
                    if ((String.IsNullOrEmpty(cell.Value.ToString()) || (!Double.TryParse(cell.Value.ToString(), out stub))))
                        cell.Value = 0;
                    break;
                case "sub_premises_num":
                    if (cell.Value.ToString().Trim().Length > 20)
                        cell.ErrorText = "Длина номера комнаты не может превышать 20 символов";
                    else
                        if ((cell.Value.ToString().Trim().Length > 0) && !Regex.IsMatch(cell.Value.ToString().Trim(), "^([0-9]+[а-я]{0,1}|[а-я])$"))
                            cell.ErrorText = "Номер комнаты может содержать в себе только цифры и не более одной строчной буквы кирилицы";
                        else
                            cell.ErrorText = "";
                    break;
                case "description":
                    if (cell.Value.ToString().Trim().Length > 65535)
                        cell.ErrorText = "Длина примечания комнаты не может превышать 65535 символов";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        void SubPremisesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        void SubPremisesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_sub_premises[row_index]).Delete();
            }
        }

        void SubPremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            int row_index = v_snapshot_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
            if (row_index == -1 && v_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]) != -1)
            {
                snapshot_sub_premises.Rows.Add(new object[] { 
                        e.Row["id_sub_premises"], 
                        e.Row["id_premises"],   
                        e.Row["id_state"],                 
                        e.Row["sub_premises_num"],
                        e.Row["total_area"],
                        e.Row["living_area"],
                        e.Row["description"],
                        e.Row["state_date"]
                    });
            } else
            if (row_index != -1)
            {
                DataRowView row = ((DataRowView)v_snapshot_sub_premises[row_index]);
                row["id_premises"] = e.Row["id_premises"];
                row["id_state"] = e.Row["id_state"];
                row["sub_premises_num"] = e.Row["sub_premises_num"];
                row["total_area"] = e.Row["total_area"];
                row["living_area"] = e.Row["living_area"];
                row["description"] = e.Row["description"];
                row["state_date"] = e.Row["state_date"];
            }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView.CurrentCell.OwningColumn.Name == "total_area" || dataGridView.CurrentCell.OwningColumn.Name == "living_area")
            {
                dataGridView.EditingControl.KeyPress -= new KeyPressEventHandler(EditingControl_KeyPress);
                dataGridView.EditingControl.KeyPress += new KeyPressEventHandler(EditingControl_KeyPress);
                if (String.IsNullOrEmpty(((TextBox)e.Control).Text.Trim()))
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text = "0";
                else
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text.Substring(0, ((TextBox)e.Control).Text.Length - 3);
            } else
                if (dataGridView.CurrentCell.OwningColumn.Name == "sub_premises_num")
                {
                    dataGridView.EditingControl.KeyPress -= new KeyPressEventHandler(EditingControl_KeyPress);
                    dataGridView.EditingControl.KeyPress += new KeyPressEventHandler(EditingControl_KeyPress);
                }
                else
                    if (dataGridView.CurrentCell.OwningColumn.Name == "id_state")
                {
                    DataGridViewComboBoxEditingControl editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                    editingControl.DropDownClosed -= editingControl_DropDownClosed;
                    editingControl.DropDownClosed += editingControl_DropDownClosed;
                }
        }

        void editingControl_DropDownClosed(object sender, EventArgs e)
        {
            DataGridViewComboBoxEditingControl editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            dataGridView.CurrentCell.Value = editingControl.SelectedValue;
            dataGridView.EndEdit();
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "total_area":
                    MessageBox.Show("Значение общей площади комнаты является некорректным","Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.ThrowException = false;
                    break;
                case "living_area":
                    MessageBox.Show("Значение жилой площади комнаты является некорректным", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.ThrowException = false;
                    break;
            }
        }
        
        void EditingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView.SelectedCells[0].OwningColumn.Name == "total_area" || dataGridView.SelectedCells[0].OwningColumn.Name == "living_area")
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == (char)8))
                    e.Handled = false;
                else
                    if ((e.KeyChar == '.') || (e.KeyChar == ','))
                    {
                        e.KeyChar = ',';
                        if (((TextBox)dataGridView.EditingControl).Text.IndexOf(',') != -1)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                    else
                        e.Handled = true;
            } else
                if (dataGridView.SelectedCells[0].OwningColumn.Name == "sub_premises_num")
                {
                    if (e.KeyChar >= 'А' && e.KeyChar <= 'Я')
                        e.KeyChar = e.KeyChar.ToString().ToLower(CultureInfo.CurrentCulture)[0];
                    if (e.KeyChar == ' ')
                        e.Handled = true;
                }
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubPremisesViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_sub_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_state = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.state_date = new CustomControls.DataGridViewDateTimeColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
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
            this.id_sub_premises,
            this.id_premises,
            this.sub_premises_num,
            this.total_area,
            this.living_area,
            this.description,
            this.id_state,
            this.state_date});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(966, 333);
            this.dataGridView.TabIndex = 0;
            // 
            // id_sub_premises
            // 
            this.id_sub_premises.HeaderText = "Внутренний номер комнаты";
            this.id_sub_premises.Name = "id_sub_premises";
            this.id_sub_premises.Visible = false;
            // 
            // id_premises
            // 
            this.id_premises.HeaderText = "Внутренний номер помещения";
            this.id_premises.Name = "id_premises";
            this.id_premises.Visible = false;
            // 
            // sub_premises_num
            // 
            this.sub_premises_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.sub_premises_num.HeaderText = "Номер комнаты";
            this.sub_premises_num.MinimumWidth = 150;
            this.sub_premises_num.Name = "sub_premises_num";
            this.sub_premises_num.Width = 150;
            // 
            // total_area
            // 
            this.total_area.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            this.total_area.DefaultCellStyle = dataGridViewCellStyle2;
            this.total_area.HeaderText = "Общая площадь";
            this.total_area.MinimumWidth = 150;
            this.total_area.Name = "total_area";
            this.total_area.Width = 150;
            // 
            // living_area
            // 
            this.living_area.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.living_area.DefaultCellStyle = dataGridViewCellStyle2;
            this.living_area.HeaderText = "Жилая площадь";
            this.living_area.MinimumWidth = 150;
            this.living_area.Name = "living_area";
            this.living_area.Width = 150;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Примечание";
            this.description.MinimumWidth = 300;
            this.description.Name = "description";
            // 
            // id_state
            // 
            this.id_state.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.id_state.HeaderText = "Текущее состояние";
            this.id_state.MinimumWidth = 150;
            this.id_state.Name = "id_state";
            this.id_state.Width = 150;
            // 
            // state_date
            // 
            this.state_date.HeaderText = "Дата установки состояния";
            this.state_date.MinimumWidth = 170;
            this.state_date.Name = "state_date";
            this.state_date.Width = 170;
            // 
            // SubPremisesViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(972, 339);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SubPremisesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень комнат";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
