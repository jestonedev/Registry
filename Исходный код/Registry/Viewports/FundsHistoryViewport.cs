using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class FundsHistoryViewport : FormWithGridViewport
    {
        #region Models

        private DataModel _fundTypes;
        private DataModel _fundAssoc;
        #endregion Models

        #region Views

        private BindingSource _vFundTypes;
        private BindingSource _vFundAssoc;
        #endregion Views

        private FundsHistoryViewport()
            : this(null, null)
        {
        }

        public FundsHistoryViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        private void RedrawDataGridRows()
        {
            if (dataGridView.Rows.Count == 0)
                return;
            var currentFundFounded = false;
            for (var i = dataGridView.Rows.Count - 1; i >= 0; i--)
            {
                if ((GeneralBindingSource.Count - 1) >= i && 
                    (((DataRowView) GeneralBindingSource[i])["exclude_restriction_date"] == DBNull.Value) &&
                    (!currentFundFounded))
                {
                    dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    currentFundFounded = true;
                }
                else
                    dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void RebuildFilter()
        {
            var filter = "id_fund IN (0";
            foreach (var fund in _vFundAssoc)
                filter += ((DataRowView)fund)["id_fund"] + ",";
            filter = filter.TrimEnd(',');
            filter += ")";
            GeneralBindingSource.Filter = filter;
        }

        private void DataBind()
        {
            comboBoxFundType.DataSource = _vFundTypes;
            comboBoxFundType.ValueMember = "id_fund_type";
            comboBoxFundType.DisplayMember = "fund_type";
            comboBoxFundType.DataBindings.Clear();
            comboBoxFundType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_fund_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxProtocolNumber.DataBindings.Clear();
            textBoxProtocolNumber.DataBindings.Add("Text", GeneralBindingSource, "protocol_number", true, DataSourceUpdateMode.Never, "");
            dateTimePickerProtocolDate.DataBindings.Clear();
            dateTimePickerProtocolDate.DataBindings.Add("Value", GeneralBindingSource, "protocol_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
            textBoxIncludeRestNum.DataBindings.Clear();
            textBoxIncludeRestNum.DataBindings.Add("Text", GeneralBindingSource, "include_restriction_number", true, DataSourceUpdateMode.Never, "");
            textBoxIncludeRestDesc.DataBindings.Clear();
            textBoxIncludeRestDesc.DataBindings.Add("Text", GeneralBindingSource, "include_restriction_description", true, DataSourceUpdateMode.Never, "");
            textBoxExcludeRestNum.DataBindings.Clear();
            textBoxExcludeRestNum.DataBindings.Add("Text", GeneralBindingSource, "exclude_restriction_number", true, DataSourceUpdateMode.Never, "");
            textBoxExcludeRestDesc.DataBindings.Clear();
            textBoxExcludeRestDesc.DataBindings.Add("Text", GeneralBindingSource, "exclude_restriction_description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerIncludeRestDate.DataBindings.Clear();
            dateTimePickerIncludeRestDate.DataBindings.Add("Value", GeneralBindingSource, "include_restriction_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);
            dateTimePickerExcludeRestDate.DataBindings.Clear();
            dateTimePickerExcludeRestDate.DataBindings.Add("Value", GeneralBindingSource, "exclude_restriction_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            dataGridView.DataSource = GeneralBindingSource;
            id_fund.DataPropertyName = "id_fund";
            id_fund_type.DataPropertyName = "id_fund_type";
            id_fund_type.DataSource = _vFundTypes;
            id_fund_type.ValueMember = "id_fund_type";
            id_fund_type.DisplayMember = "fund_type";
            protocol_date.DataPropertyName = "protocol_date";
            protocol_number.DataPropertyName = "protocol_number";
        }

        private void UnbindedCheckBoxesUpdate()
        {
            if (GeneralBindingSource.Count == 0) return;
            var row = GeneralBindingSource.Position >= 0 ? ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]) : null;
            checkBoxIncludeRest.Checked = (GeneralBindingSource.Position >= 0) && (row != null) &&
                (row["include_restriction_date"] != DBNull.Value) &&
                (row["include_restriction_number"] != DBNull.Value);
            checkBoxExcludeRest.Checked = (GeneralBindingSource.Position >= 0) && (row != null) &&
                (row["exclude_restriction_date"] != DBNull.Value) &&
                (row["exclude_restriction_number"] != DBNull.Value);
            if ((GeneralBindingSource.Position >= 0) && (row != null) &&
                (row["protocol_date"] != DBNull.Value))
                dateTimePickerProtocolDate.Checked = true;
            else
            {
                dateTimePickerProtocolDate.Value = DateTime.Now.Date;
                dateTimePickerProtocolDate.Checked = false;
            }
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) ||
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)))
                return base.ChangeViewportStateTo(state);
            viewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidatePermissions()
        {
            EntityType entity;
            string fieldName;
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    entity = EntityType.Building;
                    fieldName = "id_building";
                    break;
                case ParentTypeEnum.Premises:
                    entity = EntityType.Premise;
                    fieldName = "id_premises";
                    break;
                case ParentTypeEnum.SubPremises:
                    entity = EntityType.SubPremise;
                    fieldName = "id_sub_premises";
                    break;
                default:
                    MessageBox.Show(@"Неизвестный тип родительского объекта",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
            }
            if (DataModelHelper.HasMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации об истории фондов муниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.HasNotMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации об истории фондов немуниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private bool ValidateFundHistory(FundHistory fundHistory)
        {
            if (ValidatePermissions() == false)
                return false;
            if (checkBoxIncludeRest.Checked && fundHistory.IncludeRestrictionNumber == null)
            {
                MessageBox.Show(@"Необходимо задать номер реквизитов НПА по включению в фонд или отключить реквизит", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (checkBoxExcludeRest.Checked && fundHistory.ExcludeRestrictionNumber == null)
            {
                MessageBox.Show(@"Необходимо задать номер реквизитов НПА по исключению из фонда или отключить реквизит", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (fundHistory.IdFundType == null)
            {
                MessageBox.Show(@"Необходимо выбрать тип найма", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        protected override Entity EntityFromViewport()
        {
            var fundHistory = new FundHistory
            {
                IdFund = GeneralBindingSource.Position == -1
                    ? null
                    : ViewportHelper.ValueOrNull<int>((DataRowView) GeneralBindingSource[GeneralBindingSource.Position], "id_fund"),
                IdFundType = ViewportHelper.ValueOrNull<int>(comboBoxFundType)
            };
            if (fundHistory.IdFundType == null || fundHistory.IdFundType == 1)
            {
                fundHistory.ProtocolNumber = null;
                fundHistory.ProtocolDate = null;
            }
            else
            {
                fundHistory.ProtocolNumber = ViewportHelper.ValueOrNull(textBoxProtocolNumber);
                fundHistory.ProtocolDate = ViewportHelper.ValueOrNull(dateTimePickerProtocolDate);
            }
            fundHistory.Description = ViewportHelper.ValueOrNull(textBoxDescription);
            if (checkBoxIncludeRest.Checked)
            {
                fundHistory.IncludeRestrictionNumber = ViewportHelper.ValueOrNull(textBoxIncludeRestNum);
                fundHistory.IncludeRestrictionDescription = ViewportHelper.ValueOrNull(textBoxIncludeRestDesc);
                fundHistory.IncludeRestrictionDate = dateTimePickerIncludeRestDate.Value;
            }
            else
            {
                fundHistory.IncludeRestrictionDate = null;
                fundHistory.IncludeRestrictionDescription = null;
                fundHistory.IncludeRestrictionNumber = null;
            }
            if (checkBoxExcludeRest.Checked)
            {
                fundHistory.ExcludeRestrictionNumber = ViewportHelper.ValueOrNull(textBoxExcludeRestNum);
                fundHistory.ExcludeRestrictionDescription = ViewportHelper.ValueOrNull(textBoxExcludeRestDesc);
                fundHistory.ExcludeRestrictionDate = dateTimePickerExcludeRestDate.Value;
            }
            else
            {
                fundHistory.ExcludeRestrictionDate = null;
                fundHistory.ExcludeRestrictionDescription = null;
                fundHistory.ExcludeRestrictionNumber = null;
            }      
            return fundHistory;
        }

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return FundHistoryConverter.FromRow(row);
        }

        private void ViewportFromFundHistory(FundHistory fundHistory)
        {
            comboBoxFundType.SelectedValue = ViewportHelper.ValueOrDBNull(fundHistory.IdFundType);
            dateTimePickerProtocolDate.Value = ViewportHelper.ValueOrDefault(fundHistory.ProtocolDate);
            dateTimePickerIncludeRestDate.Value = ViewportHelper.ValueOrDefault(fundHistory.IncludeRestrictionDate);
            dateTimePickerExcludeRestDate.Value = ViewportHelper.ValueOrDefault(fundHistory.ExcludeRestrictionDate);
            textBoxProtocolNumber.Text = fundHistory.ProtocolNumber;
            textBoxIncludeRestNum.Text = fundHistory.IncludeRestrictionNumber;
            textBoxIncludeRestDesc.Text = fundHistory.IncludeRestrictionDescription;
            textBoxExcludeRestNum.Text = fundHistory.ExcludeRestrictionNumber;
            textBoxExcludeRestDesc.Text = fundHistory.ExcludeRestrictionDescription;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance<FundsHistoryDataModel>();
            _fundTypes = DataModel.GetInstance<FundTypesDataModel>();

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            _fundTypes.Select();

            if (ParentType == ParentTypeEnum.SubPremises)
                _fundAssoc = DataModel.GetInstance<FundsSubPremisesAssocDataModel>();
            else
                if (ParentType == ParentTypeEnum.Premises)
                    _fundAssoc = DataModel.GetInstance<FundsPremisesAssocDataModel>();
                else
                    if (ParentType == ParentTypeEnum.Building)
                        _fundAssoc = DataModel.GetInstance<FundsBuildingsAssocDataModel>();
                    else
                        throw new ViewportException("Неизвестный тип родительского объекта");

            var ds = DataModel.DataSet;

            _vFundAssoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.SubPremises) && (ParentRow != null))
            {
                _vFundAssoc.DataMember = "funds_sub_premises_assoc";
                _vFundAssoc.Filter = "id_sub_premises = " + ParentRow["id_sub_premises"];
                Text = string.Format(CultureInfo.InvariantCulture, "История фонда комнаты №{0} помещения №{1}", ParentRow["sub_premises_num"],
                    ParentRow["id_premises"]);
            }
            else
                if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
                {
                    _vFundAssoc.DataMember = "funds_premises_assoc";
                    _vFundAssoc.Filter = "id_premises = " + ParentRow["id_premises"];
                    Text = string.Format(CultureInfo.InvariantCulture, "История фонда помещения №{0}", ParentRow["id_premises"]);
                }
                else
                    if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                    {
                        _vFundAssoc.DataMember = "funds_buildings_assoc";
                        _vFundAssoc.Filter = "id_building = " + ParentRow["id_building"];
                        Text = string.Format(CultureInfo.InvariantCulture, "История фонда здания №{0}", ParentRow["id_building"]);
                    }
                    else
                        throw new ViewportException("Неизвестный тип родительского объекта");
            _vFundAssoc.DataSource = ds;

            _vFundTypes = new BindingSource
            {
                DataMember = "fund_types",
                DataSource = ds
            };

            GeneralBindingSource = new BindingSource();
            GeneralBindingSource.CurrentItemChanged += GeneralBindingSource_CurrentItemChanged;
            GeneralBindingSource.DataMember = "funds_history";
            GeneralBindingSource.DataSource = ds;
            //Перестраиваем фильтр GeneralBindingSource.Filter
            RebuildFilter();

            DataBind();

            GeneralDataModel.Select().RowChanged += FundsHistoryViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted += FundsHistoryViewport_RowDeleted;
            _fundAssoc.Select().RowChanged += FundAssoc_RowChanged;
            _fundAssoc.Select().RowDeleted += FundAssoc_RowDeleted;
            comboBoxFundType.SelectedIndexChanged += comboBoxFundType_SelectedIndexChanged;
            is_editable = true;
            DataChangeHandlersInit();
            if (GeneralBindingSource.Count == 0)
                InsertRecord();
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            var fundHistory = (FundHistory) EntityFromViewport();
            if (!ValidateFundHistory(fundHistory))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idParent =
                        ((ParentType == ParentTypeEnum.SubPremises) && ParentRow != null) ? (int)ParentRow["id_sub_premises"] :
                        ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] :
                        ((ParentType == ParentTypeEnum.Building) && ParentRow != null) ? (int)ParentRow["id_building"] :
                        -1;
                    if (idParent == -1)
                    {
                        MessageBox.Show(@"Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                            @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    var idFund = GeneralDataModel.Insert(fundHistory);
                    if (idFund == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    var assoc = new FundObjectAssoc(idParent, idFund);
                    switch (ParentType)
                    {
                        case ParentTypeEnum.Building:
                            DataModel.GetInstance<FundsBuildingsAssocDataModel>().Insert(assoc);
                            break;
                        case ParentTypeEnum.Premises:
                            DataModel.GetInstance<FundsPremisesAssocDataModel>().Insert(assoc);
                            break;
                        case ParentTypeEnum.SubPremises:
                            DataModel.GetInstance<FundsSubPremisesAssocDataModel>().Insert(assoc);
                            break;
                    }
                    DataRowView newRow;
                    fundHistory.IdFund = idFund;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    FundHistoryConverter.FillRow(fundHistory, newRow);
                    _fundAssoc.Select().Rows.Add(idParent, idFund);
                    RebuildFilter();
                    GeneralBindingSource.Position = GeneralBindingSource.Count - 1;
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (fundHistory.IdFund == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о принадлежности фонду без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(fundHistory) == -1)
                        return;
                    is_editable = false;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    FundHistoryConverter.FillRow(fundHistory, row);
                    break;
            }
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            dataGridView.Enabled = true;
            is_editable = true;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanCopyRecord()
        {
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            comboBoxFundType.Focus();
            is_editable = false;
            var fundHistory = (FundHistory) EntityFromView();
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromFundHistory(fundHistory);
            checkBoxIncludeRest.Checked = (fundHistory.IncludeRestrictionDate != null);
            checkBoxExcludeRest.Checked = (fundHistory.ExcludeRestrictionDate != null);
            is_editable = true;
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            comboBoxFundType.Focus();
            is_editable = false;
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            is_editable = true;
            GeneralDataModel.EditingNewRecord = true;
            UnbindedCheckBoxesUpdate();
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (viewportState != ViewportState.NewRowState) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (ValidatePermissions() == false)
                    return;
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_fund"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                is_editable = true;
                RedrawDataGridRows();
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                        RedrawDataGridRows();
                        if (GeneralBindingSource.Position != -1)
                            dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                    }
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            MenuCallback.EditingStateUpdate();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                GeneralBindingSource.CurrentItemChanged -= GeneralBindingSource_CurrentItemChanged;
                GeneralDataModel.Select().RowChanged -= FundsHistoryViewport_RowChanged;
                GeneralDataModel.Select().RowDeleted -= FundsHistoryViewport_RowDeleted;
                _fundAssoc.Select().RowChanged -= FundAssoc_RowChanged;
                _fundAssoc.Select().RowDeleted -= FundAssoc_RowDeleted;
                comboBoxFundType.SelectedIndexChanged -= comboBoxFundType_SelectedIndexChanged;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                GeneralDataModel.EditingNewRecord = false;
            Close();
        }

        private void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (GeneralBindingSource.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (GeneralBindingSource.Position >= dataGridView.RowCount)
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                else
                    if (dataGridView.Rows[GeneralBindingSource.Position].Selected != true)
                        dataGridView.Rows[GeneralBindingSource.Position].Selected = true;
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            if (GeneralBindingSource.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        private void FundsHistoryViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                UnbindedCheckBoxesUpdate();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        private void FundsHistoryViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            textBoxProtocolNumber.Focus();
            base.OnVisibleChanged(e);
        }

        private void comboBoxFundType_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxProtocolNumber.Enabled = comboBoxFundType.SelectedValue != null &&
                                            (Convert.ToInt32(comboBoxFundType.SelectedValue, CultureInfo.InvariantCulture) != 1);
            dateTimePickerProtocolDate.Enabled = comboBoxFundType.SelectedValue != null &&
                                                 (Convert.ToInt32(comboBoxFundType.SelectedValue, CultureInfo.InvariantCulture) != 1);
        }

        private void FundAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            RebuildFilter();
            UnbindedCheckBoxesUpdate();
            RedrawDataGridRows();
        }

        private void FundAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RebuildFilter();
            UnbindedCheckBoxesUpdate();
            RedrawDataGridRows();
        }

        private void checkBoxExcludeRest_CheckedChanged(object sender, EventArgs e)
        {
            textBoxExcludeRestNum.Enabled = checkBoxExcludeRest.Checked;
            textBoxExcludeRestDesc.Enabled = checkBoxExcludeRest.Checked;
            dateTimePickerExcludeRestDate.Enabled = checkBoxExcludeRest.Checked;
            CheckViewportModifications();
        }

        private void checkBoxIncludeRest_CheckedChanged(object sender, EventArgs e)
        {
            textBoxIncludeRestNum.Enabled = checkBoxIncludeRest.Checked;
            textBoxIncludeRestDesc.Enabled = checkBoxIncludeRest.Checked;
            dateTimePickerIncludeRestDate.Enabled = checkBoxIncludeRest.Checked;
            CheckViewportModifications();
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

    }
}
