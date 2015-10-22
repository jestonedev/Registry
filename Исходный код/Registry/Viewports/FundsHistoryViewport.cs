using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CustomControls;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class FundsHistoryViewport : FormWithGridViewport
    {
        #region Models
        DataModel fund_types;
        DataModel fund_assoc;
        #endregion Models

        #region Views
        BindingSource v_fund_types;
        BindingSource v_fund_assoc;
        #endregion Views

        private FundsHistoryViewport()
            : this(null)
        {
        }

        public FundsHistoryViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        public FundsHistoryViewport(FundsHistoryViewport fundsHistoryViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = fundsHistoryViewport.DynamicFilter;
            StaticFilter = fundsHistoryViewport.StaticFilter;
            ParentRow = fundsHistoryViewport.ParentRow;
            ParentType = fundsHistoryViewport.ParentType;
        }

        private void RedrawDataGridRows()
        {
            if (dataGridView.Rows.Count == 0)
                return;
            var currentFundFounded = false;
            for (var i = dataGridView.Rows.Count - 1; i >= 0; i--)
                if ((((DataRowView)GeneralBindingSource[i])["exclude_restriction_date"] == DBNull.Value) && (!currentFundFounded))
                {
                    dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    currentFundFounded = true;
                }
                else
                    dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.White;
        }

        private void RebuildFilter()
        {
            var filter = "id_fund IN (0";
            foreach (var fund in v_fund_assoc)
                filter += ((DataRowView)fund)["id_fund"] + ",";
            filter = filter.TrimEnd(',');
            filter += ")";
            GeneralBindingSource.Filter = filter;
        }

        private void DataBind()
        {
            comboBoxFundType.DataSource = v_fund_types;
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
            id_fund_type.DataSource = v_fund_types;
            id_fund_type.ValueMember = "id_fund_type";
            id_fund_type.DisplayMember = "fund_type";
            protocol_date.DataPropertyName = "protocol_date";
            protocol_number.DataPropertyName = "protocol_number";
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = (GeneralBindingSource.Position >= 0) ? ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]) : null;
            checkBoxIncludeRest.Checked = (GeneralBindingSource.Position >= 0) &&
                (row["include_restriction_date"] != DBNull.Value) &&
                (row["include_restriction_number"] != DBNull.Value);
            checkBoxExcludeRest.Checked = (GeneralBindingSource.Position >= 0) &&
                (row["exclude_restriction_date"] != DBNull.Value) &&
                (row["exclude_restriction_number"] != DBNull.Value);
            if ((GeneralBindingSource.Position >= 0) &&
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
            if (!(AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))))
            {
                viewportState = ViewportState.ReadState;
                return true;
            }
            switch (state)
            {
                case ViewportState.ReadState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            return true;
                        case ViewportState.NewRowState:
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else return false;
                            if (viewportState == ViewportState.ReadState)
                                return true;
                            else
                                return false;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            if (GeneralDataModel.EditingNewRecord)
                                return false;
                            viewportState = ViewportState.NewRowState;
                            return true;
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.NewRowState);
                            else
                                return false;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            return viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.ModifyRowState);
                    }
                    break;
            }
            return false;
        }

        private void LocateFundHistoryBy(int id)
        {
            var position = GeneralBindingSource.Find("id_fund", id);
            is_editable = false;
            if (position > 0)
                GeneralBindingSource.Position = position;
            is_editable = true;
        }

        private bool ValidatePermissions()
        {
            var entity = EntityType.Unknown;
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
                else
                    if (ParentType == ParentTypeEnum.SubPremises)
                    {
                        entity = EntityType.SubPremise;
                        fieldName = "id_sub_premises";
                    }
            if (DataModelHelper.HasMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show("У вас нет прав на изменение информации об истории фондов муниципальных объектов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (DataModelHelper.HasNotMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show("У вас нет прав на изменение информации об истории фондов немуниципальных объектов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                MessageBox.Show("Необходимо задать номер реквизитов НПА по включению в фонд или отключить реквизит", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (checkBoxExcludeRest.Checked && fundHistory.ExcludeRestrictionNumber == null)
            {
                MessageBox.Show("Необходимо задать номер реквизитов НПА по исключению из фонда или отключить реквизит", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (fundHistory.IdFundType == null)
            {
                MessageBox.Show("Необходимо выбрать тип найма", "Ошибка",
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
            var fundHistory = new FundHistory();
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            fundHistory.IdFund = ViewportHelper.ValueOrNull<int>(row, "id_fund");
            fundHistory.IdFundType = ViewportHelper.ValueOrNull<int>(row, "id_fund_type");
            fundHistory.ProtocolNumber = ViewportHelper.ValueOrNull(row, "protocol_number");
            fundHistory.ProtocolDate = ViewportHelper.ValueOrNull<DateTime>(row, "protocol_date");
            fundHistory.IncludeRestrictionNumber = ViewportHelper.ValueOrNull(row, "include_restriction_number");
            fundHistory.IncludeRestrictionDate = ViewportHelper.ValueOrNull<DateTime>(row, "include_restriction_date");
            fundHistory.IncludeRestrictionDescription = ViewportHelper.ValueOrNull(row, "include_restriction_description");
            fundHistory.ExcludeRestrictionNumber = ViewportHelper.ValueOrNull(row, "exclude_restriction_number");
            fundHistory.ExcludeRestrictionDate = ViewportHelper.ValueOrNull<DateTime>(row, "exclude_restriction_date");
            fundHistory.ExcludeRestrictionDescription = ViewportHelper.ValueOrNull(row, "exclude_restriction_description");
            fundHistory.Description = ViewportHelper.ValueOrNull(row, "description");         
            return fundHistory;
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

        private static void FillRowFromFundHistory(FundHistory fundHistory, DataRowView row)
        {
            row.BeginEdit();
            row["id_fund"] = ViewportHelper.ValueOrDBNull(fundHistory.IdFund);
            row["id_fund_type"] = ViewportHelper.ValueOrDBNull(fundHistory.IdFundType);
            row["protocol_number"] = ViewportHelper.ValueOrDBNull(fundHistory.ProtocolNumber);
            row["protocol_date"] = ViewportHelper.ValueOrDBNull(fundHistory.ProtocolDate);
            row["include_restriction_number"] = ViewportHelper.ValueOrDBNull(fundHistory.IncludeRestrictionNumber);
            row["include_restriction_date"] = ViewportHelper.ValueOrDBNull(fundHistory.IncludeRestrictionDate);
            row["include_restriction_description"] = ViewportHelper.ValueOrDBNull(fundHistory.IncludeRestrictionDescription);
            row["exclude_restriction_number"] = ViewportHelper.ValueOrDBNull(fundHistory.ExcludeRestrictionNumber);
            row["exclude_restriction_date"] = ViewportHelper.ValueOrDBNull(fundHistory.ExcludeRestrictionDate);
            row["exclude_restriction_description"] = ViewportHelper.ValueOrDBNull(fundHistory.ExcludeRestrictionDescription);
            row["description"] = ViewportHelper.ValueOrDBNull(fundHistory.Description);
            row.EndEdit();
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance(DataModelType.FundsHistoryDataModel);
            fund_types = DataModel.GetInstance(DataModelType.FundTypesDataModel);

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            fund_types.Select();

            if (ParentType == ParentTypeEnum.SubPremises)
                fund_assoc = DataModel.GetInstance(DataModelType.FundsSubPremisesAssocDataModel);
            else
                if (ParentType == ParentTypeEnum.Premises)
                    fund_assoc = DataModel.GetInstance(DataModelType.FundsPremisesAssocDataModel);
                else
                    if (ParentType == ParentTypeEnum.Building)
                        fund_assoc = DataModel.GetInstance(DataModelType.FundsBuildingsAssocDataModel);
                    else
                        throw new ViewportException("Неизвестный тип родительского объекта");

            var ds = DataModel.DataSet;

            v_fund_assoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.SubPremises) && (ParentRow != null))
            {
                v_fund_assoc.DataMember = "funds_sub_premises_assoc";
                v_fund_assoc.Filter = "id_sub_premises = " + ParentRow["id_sub_premises"];
                Text = string.Format(CultureInfo.InvariantCulture, "История фонда комнаты №{0} помещения №{1}", ParentRow["sub_premises_num"],
                    ParentRow["id_premises"]);
            }
            else
                if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
                {
                    v_fund_assoc.DataMember = "funds_premises_assoc";
                    v_fund_assoc.Filter = "id_premises = " + ParentRow["id_premises"];
                    Text = string.Format(CultureInfo.InvariantCulture, "История фонда помещения №{0}", ParentRow["id_premises"]);
                }
                else
                    if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                    {
                        v_fund_assoc.DataMember = "funds_buildings_assoc";
                        v_fund_assoc.Filter = "id_building = " + ParentRow["id_building"];
                        Text = string.Format(CultureInfo.InvariantCulture, "История фонда здания №{0}", ParentRow["id_building"]);
                    }
                    else
                        throw new ViewportException("Неизвестный тип родительского объекта");
            v_fund_assoc.DataSource = ds;

            v_fund_types = new BindingSource
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
            fund_assoc.Select().RowChanged += FundAssoc_RowChanged;
            fund_assoc.Select().RowDeleted += FundAssoc_RowDeleted;
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
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
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
                        MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                            DataModel.GetInstance(DataModelType.FundsBuildingsAssocDataModel).Insert(assoc);
                            break;
                        case ParentTypeEnum.Premises:
                            DataModel.GetInstance(DataModelType.FundsPremisesAssocDataModel).Insert(assoc);
                            break;
                        case ParentTypeEnum.SubPremises:
                            DataModel.GetInstance(DataModelType.FundsSubPremisesAssocDataModel).Insert(assoc);
                            break;
                    }
                    DataRowView newRow;
                    fundHistory.IdFund = idFund;
                    is_editable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    FillRowFromFundHistory(fundHistory, newRow);
                    fund_assoc.Select().Rows.Add(idParent, idFund);
                    RebuildFilter();
                    GeneralBindingSource.Position = GeneralBindingSource.Count - 1;
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (fundHistory.IdFund == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о принадлежности фонду без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(fundHistory) == -1)
                        return;
                    is_editable = false;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    FillRowFromFundHistory(fundHistory, row);
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

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new FundsHistoryViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (GeneralBindingSource.Count > 0)
                viewport.LocateFundHistoryBy((((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])["id_fund"] as int?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                fund_assoc.Select().RowChanged -= FundAssoc_RowChanged;
                fund_assoc.Select().RowDeleted -= FundAssoc_RowDeleted;
                GeneralDataModel.Select().RowChanged -= FundsHistoryViewport_RowChanged;
                GeneralDataModel.Select().RowDeleted -= FundsHistoryViewport_RowDeleted;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                GeneralDataModel.EditingNewRecord = false;
            fund_assoc.Select().RowChanged -= FundAssoc_RowChanged;
            fund_assoc.Select().RowDeleted -= FundAssoc_RowDeleted;
            GeneralDataModel.Select().RowChanged -= FundsHistoryViewport_RowChanged;
            GeneralDataModel.Select().RowDeleted -= FundsHistoryViewport_RowDeleted;
            Close();
        }

        void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
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

        void FundsHistoryViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                UnbindedCheckBoxesUpdate();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        void FundsHistoryViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            textBoxProtocolNumber.Focus();
            base.OnVisibleChanged(e);
        }

        void comboBoxFundType_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxProtocolNumber.Enabled = ((comboBoxFundType.SelectedValue) != null &&
                (Convert.ToInt32(comboBoxFundType.SelectedValue, CultureInfo.InvariantCulture) != 1));
            dateTimePickerProtocolDate.Enabled = ((comboBoxFundType.SelectedValue) != null &&
                (Convert.ToInt32(comboBoxFundType.SelectedValue, CultureInfo.InvariantCulture) != 1));
        }

        void FundAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                RebuildFilter();
                UnbindedCheckBoxesUpdate();
                RedrawDataGridRows();
            }
        }

        void FundAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RebuildFilter();
            UnbindedCheckBoxesUpdate();
            RedrawDataGridRows();
        }

        void checkBoxExcludeRest_CheckedChanged(object sender, EventArgs e)
        {
            textBoxExcludeRestNum.Enabled = checkBoxExcludeRest.Checked;
            textBoxExcludeRestDesc.Enabled = checkBoxExcludeRest.Checked;
            dateTimePickerExcludeRestDate.Enabled = checkBoxExcludeRest.Checked;
            CheckViewportModifications();
        }

        void checkBoxIncludeRest_CheckedChanged(object sender, EventArgs e)
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
