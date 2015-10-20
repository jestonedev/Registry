using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CustomControls;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Reporting;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class SubPremisesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        DataModel sub_premises;
        DataModel object_states;
        DataTable snapshot_sub_premises = new DataTable("snapshot_sub_premises");
        #endregion Models

        #region Views
        BindingSource v_sub_premises;
        BindingSource v_object_states;
        BindingSource v_snapshot_sub_premises;
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
            DynamicFilter = subPremisesViewport.DynamicFilter;
            StaticFilter = subPremisesViewport.StaticFilter;
            ParentRow = subPremisesViewport.ParentRow;
            ParentType = subPremisesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = SubPremisesFromView();
            var list_from_viewport = SubPremisesFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            var founded = false;
            for (var i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (var j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
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
            foreach (var subPremise in subPremises)
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
            var subPremise = new SubPremise();
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
            var list = new List<SubPremise>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var sp = new SubPremise();
                    var row = dataGridView.Rows[i];
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
            var list = new List<SubPremise>();
            for (var i = 0; i < v_sub_premises.Count; i++)
            {
                var sp = new SubPremise();
                var row = ((DataRowView)v_sub_premises[i]);
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
            DockAreas = DockAreas.Document;
            sub_premises = DataModel.GetInstance(DataModelType.SubPremisesDataModel);
            object_states = DataModel.GetInstance(DataModelType.ObjectStatesDataModel);
            // Дожидаемся дозагрузки данных, если это необходимо
            sub_premises.Select();
            object_states.Select();

            v_object_states = new BindingSource();
            v_object_states.DataMember = "object_states";
            v_object_states.DataSource = DataModel.DataSet;

            v_sub_premises = new BindingSource();
            v_sub_premises.DataMember = "sub_premises";
            v_sub_premises.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                v_sub_premises.Filter += " AND ";
            v_sub_premises.Filter += DynamicFilter;
            v_sub_premises.DataSource = DataModel.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.Premises)
                Text = string.Format(CultureInfo.InvariantCulture, "Комнаты помещения №{0}", ParentRow["id_premises"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < sub_premises.Select().Columns.Count; i++)
                snapshot_sub_premises.Columns.Add(new DataColumn(sub_premises.Select().Columns[i].ColumnName, sub_premises.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_sub_premises.Count; i++)
                snapshot_sub_premises.Rows.Add(DataRowViewToArray(((DataRowView)v_sub_premises[i])));
            v_snapshot_sub_premises = new BindingSource();
            v_snapshot_sub_premises.DataSource = snapshot_sub_premises;
            v_snapshot_sub_premises.CurrentItemChanged += v_snapshot_sub_premises_CurrentItemChanged;

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
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            dataGridView.CellValidated += dataGridView_CellValidated;
            dataGridView.DataError += dataGridView_DataError;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            sub_premises.Select().RowChanged += SubPremisesViewport_RowChanged;
            sub_premises.Select().RowDeleting += SubPremisesViewport_RowDeleting;
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
            var row = (DataRowView)v_snapshot_sub_premises.AddNew();
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
            for (var i = 0; i < v_sub_premises.Count; i++)
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
            var list = SubPremisesFromViewport();
            if (!ValidateSubPremises(list))
            {
                sync_views = true;
                sub_premises.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = sub_premises.Select().Rows.Find(list[i].IdSubPremises);
                if (row == null)
                {
                    if (new[] { 4, 5, 9 }.Contains(list[i].IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                    {
                        MessageBox.Show("У вас нет прав на добавление в базу муниципальных жилых помещений", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    if (new[] { 1, 3, 6, 7, 8 }.Contains(list[i].IdState.Value) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                    {
                        MessageBox.Show("У вас нет прав на добавление в базу немуниципальных жилых помещений", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    var id_sub_premises = sub_premises.Insert(list[i]);
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
                    var subPremiseFromView = RowToSubPremise(row);
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
                    if (sub_premises.Update(list[i]) == -1)
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
            for (var i = 0; i < list.Count; i++)
            {
                var row_index = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_sub_premises"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_sub_premises"].Value.ToString()) &&
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
                    if (sub_premises.Delete(list[i].IdSubPremises.Value) == -1)
                    {
                        sync_views = true;
                        sub_premises.EditingNewRecord = false;
                        return;
                    }
                    sub_premises.Select().Rows.Find(list[i].IdSubPremises).Delete();
                }
            }
            sync_views = true;
            sub_premises.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new SubPremisesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show("Сохранить изменения о комнатах в базу данных?", "Внимание",
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
            sub_premises.Select().RowChanged -= SubPremisesViewport_RowChanged;
            sub_premises.Select().RowDeleting -= SubPremisesViewport_RowDeleting;
            sub_premises.Select().RowDeleted -= SubPremisesViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            sub_premises.Select().RowChanged -= SubPremisesViewport_RowChanged;
            sub_premises.Select().RowDeleting -= SubPremisesViewport_RowDeleting;
            sub_premises.Select().RowDeleted -= SubPremisesViewport_RowDeleted;
            Close();
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
            var reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            var arguments = new Dictionary<string, string>();
            arguments.Add("ids", ParentRow["id_premises"].ToString());
            arguments.Add("excerpt_type", "1");
            reporter.Run(arguments);
        }

        public override void RegistryExcerptSubPremiseReportGenerate()
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show("Перед формированием выписки по комнате необходимо сохранить изменения в базу данных. " +
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
            var reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            var arguments = new Dictionary<string, string>();
            arguments.Add("ids", ((DataRowView)v_snapshot_sub_premises[v_snapshot_sub_premises.Position])["id_sub_premises"].ToString());
            arguments.Add("excerpt_type", "2");
            reporter.Run(arguments);
        }

        public override void RegistryExcerptSubPremisesReportGenerate()
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show("Перед формированием выписки по муниципальным комнатам необходимо сохранить изменения в базу данных. " +
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
            var reporter = ReporterFactory.CreateReporter(ReporterType.RegistryExcerptReporter);
            var arguments = new Dictionary<string, string>();
            arguments.Add("ids", ParentRow["id_premises"].ToString());
            arguments.Add("excerpt_type", "3");
            reporter.Run(arguments);
        }

        public override void ShowFundHistory()
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show("Перед открытием истории принадлежности фондам необходимо сохранить изменения в базу данных. "+
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
                var result = MessageBox.Show("Перед открытием истории найма необходимо сохранить изменения в базу данных. Вы хотите это сделать?",
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
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "total_area":
                case "living_area":
                    double stub = 0;
                    if ((string.IsNullOrEmpty(cell.Value.ToString()) || (!double.TryParse(cell.Value.ToString(), out stub))))
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
                var row_index = v_snapshot_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_sub_premises[row_index]).Delete();
            }
        }

        void SubPremisesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = v_snapshot_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]);
            if (row_index == -1 && v_sub_premises.Find("id_sub_premises", e.Row["id_sub_premises"]) != -1)
            {
                snapshot_sub_premises.Rows.Add(e.Row["id_sub_premises"], e.Row["id_premises"], e.Row["id_state"], e.Row["sub_premises_num"], e.Row["total_area"], e.Row["living_area"], e.Row["description"], e.Row["state_date"]);
            } else
            if (row_index != -1)
            {
                var row = ((DataRowView)v_snapshot_sub_premises[row_index]);
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
                dataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
                dataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
                if (string.IsNullOrEmpty(((TextBox)e.Control).Text.Trim()))
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text = "0";
                else
                    ((TextBox)e.Control).Text = ((TextBox)e.Control).Text.Substring(0, ((TextBox)e.Control).Text.Length - 3);
            } else
                if (dataGridView.CurrentCell.OwningColumn.Name == "sub_premises_num")
                {
                    dataGridView.EditingControl.KeyPress -= EditingControl_KeyPress;
                    dataGridView.EditingControl.KeyPress += EditingControl_KeyPress;
                }
                else
                    if (dataGridView.CurrentCell.OwningColumn.Name == "id_state")
                {
                    var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                    editingControl.DropDownClosed -= editingControl_DropDownClosed;
                    editingControl.DropDownClosed += editingControl_DropDownClosed;
                }
        }

        void editingControl_DropDownClosed(object sender, EventArgs e)
        {
            var editingControl = dataGridView.EditingControl as DataGridViewComboBoxEditingControl;
            dataGridView.CurrentCell.Value = editingControl.SelectedValue;
            dataGridView.EndEdit();
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
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
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(SubPremisesViewport));
            dataGridView = new DataGridView();
            id_sub_premises = new DataGridViewTextBoxColumn();
            id_premises = new DataGridViewTextBoxColumn();
            sub_premises_num = new DataGridViewTextBoxColumn();
            total_area = new DataGridViewTextBoxColumn();
            living_area = new DataGridViewTextBoxColumn();
            description = new DataGridViewTextBoxColumn();
            id_state = new DataGridViewComboBoxColumn();
            state_date = new DataGridViewDateTimeColumn();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_sub_premises, id_premises, sub_premises_num, total_area, living_area, description, id_state, state_date);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(966, 333);
            dataGridView.TabIndex = 0;
            // 
            // id_sub_premises
            // 
            id_sub_premises.HeaderText = "Внутренний номер комнаты";
            id_sub_premises.Name = "id_sub_premises";
            id_sub_premises.Visible = false;
            // 
            // id_premises
            // 
            id_premises.HeaderText = "Внутренний номер помещения";
            id_premises.Name = "id_premises";
            id_premises.Visible = false;
            // 
            // sub_premises_num
            // 
            sub_premises_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            sub_premises_num.HeaderText = "Номер комнаты";
            sub_premises_num.MinimumWidth = 150;
            sub_premises_num.Name = "sub_premises_num";
            sub_premises_num.Width = 150;
            // 
            // total_area
            // 
            total_area.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            total_area.DefaultCellStyle = dataGridViewCellStyle2;
            total_area.HeaderText = "Общая площадь";
            total_area.MinimumWidth = 150;
            total_area.Name = "total_area";
            total_area.Width = 150;
            // 
            // living_area
            // 
            living_area.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            living_area.DefaultCellStyle = dataGridViewCellStyle2;
            living_area.HeaderText = "Жилая площадь";
            living_area.MinimumWidth = 150;
            living_area.Name = "living_area";
            living_area.Width = 150;
            // 
            // description
            // 
            description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            description.HeaderText = "Примечание";
            description.MinimumWidth = 300;
            description.Name = "description";
            // 
            // id_state
            // 
            id_state.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            id_state.HeaderText = "Текущее состояние";
            id_state.MinimumWidth = 150;
            id_state.Name = "id_state";
            id_state.Width = 150;
            // 
            // state_date
            // 
            state_date.HeaderText = "Дата установки состояния";
            state_date.MinimumWidth = 170;
            state_date.Name = "state_date";
            state_date.Width = 170;
            // 
            // SubPremisesViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(972, 339);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "SubPremisesViewport";
            Padding = new Padding(3);
            Text = "Перечень комнат";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
