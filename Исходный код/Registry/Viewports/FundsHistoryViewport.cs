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
    internal sealed class FundsHistoryViewport : Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanel7;
        private TableLayoutPanel tableLayoutPanel8;
        private GroupBox groupBox14;
        private GroupBox groupBox15;
        private GroupBox groupBox16;
        private GroupBox groupBox17;
        private Label label29;
        private Label label30;
        private Label label31;
        private Label label32;
        private Label label33;
        private Label label34;
        private Label label35;
        private Label label36;
        private Label label37;
        private DateTimePicker dateTimePickerProtocolDate;
        private TextBox textBoxProtocolNumber;
        private ComboBox comboBoxFundType;
        private DataGridView dataGridView;
        private CheckBox checkBoxIncludeRest;
        private CheckBox checkBoxExcludeRest;
        private TextBox textBoxIncludeRestDesc;
        private TextBox textBoxExcludeRestDesc;
        private TextBox textBoxIncludeRestNum;
        private TextBox textBoxExcludeRestNum;
        private DateTimePicker dateTimePickerIncludeRestDate;
        private DateTimePicker dateTimePickerExcludeRestDate;
        private TextBox textBoxDescription;
        private DataGridViewTextBoxColumn id_fund;
        private DataGridViewTextBoxColumn protocol_number;
        private DataGridViewDateTimeColumn protocol_date;
        private DataGridViewComboBoxColumn id_fund_type;
        #endregion Components

        #region Models
        DataModel funds_history;
        DataModel fund_types;
        DataModel fund_assoc;
        #endregion Models

        #region Views
        BindingSource v_funds_history;
        BindingSource v_fund_types;
        BindingSource v_fund_assoc;
        #endregion Views

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable;

        private FundsHistoryViewport()
            : this(null)
        {
        }

        public FundsHistoryViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
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
                if ((((DataRowView)v_funds_history[i])["exclude_restriction_date"] == DBNull.Value) && (!currentFundFounded))
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
            v_funds_history.Filter = filter;
        }

        private void DataBind()
        {
            comboBoxFundType.DataSource = v_fund_types;
            comboBoxFundType.ValueMember = "id_fund_type";
            comboBoxFundType.DisplayMember = "fund_type";
            comboBoxFundType.DataBindings.Clear();
            comboBoxFundType.DataBindings.Add("SelectedValue", v_funds_history, "id_fund_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxProtocolNumber.DataBindings.Clear();
            textBoxProtocolNumber.DataBindings.Add("Text", v_funds_history, "protocol_number", true, DataSourceUpdateMode.Never, "");
            dateTimePickerProtocolDate.DataBindings.Clear();
            dateTimePickerProtocolDate.DataBindings.Add("Value", v_funds_history, "protocol_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", v_funds_history, "description", true, DataSourceUpdateMode.Never, "");
            textBoxIncludeRestNum.DataBindings.Clear();
            textBoxIncludeRestNum.DataBindings.Add("Text", v_funds_history, "include_restriction_number", true, DataSourceUpdateMode.Never, "");
            textBoxIncludeRestDesc.DataBindings.Clear();
            textBoxIncludeRestDesc.DataBindings.Add("Text", v_funds_history, "include_restriction_description", true, DataSourceUpdateMode.Never, "");
            textBoxExcludeRestNum.DataBindings.Clear();
            textBoxExcludeRestNum.DataBindings.Add("Text", v_funds_history, "exclude_restriction_number", true, DataSourceUpdateMode.Never, "");
            textBoxExcludeRestDesc.DataBindings.Clear();
            textBoxExcludeRestDesc.DataBindings.Add("Text", v_funds_history, "exclude_restriction_description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerIncludeRestDate.DataBindings.Clear();
            dateTimePickerIncludeRestDate.DataBindings.Add("Value", v_funds_history, "include_restriction_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);
            dateTimePickerExcludeRestDate.DataBindings.Clear();
            dateTimePickerExcludeRestDate.DataBindings.Add("Value", v_funds_history, "exclude_restriction_date", true, DataSourceUpdateMode.Never, DateTime.Now.Date);

            dataGridView.DataSource = v_funds_history;
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
            var row = (v_funds_history.Position >= 0) ? ((DataRowView)v_funds_history[v_funds_history.Position]) : null;
            checkBoxIncludeRest.Checked = (v_funds_history.Position >= 0) &&
                (row["include_restriction_date"] != DBNull.Value) &&
                (row["include_restriction_number"] != DBNull.Value);
            checkBoxExcludeRest.Checked = (v_funds_history.Position >= 0) &&
                (row["exclude_restriction_date"] != DBNull.Value) &&
                (row["exclude_restriction_number"] != DBNull.Value);
            if ((v_funds_history.Position >= 0) &&
                (row["protocol_date"] != DBNull.Value))
                dateTimePickerProtocolDate.Checked = true;
            else
            {
                dateTimePickerProtocolDate.Value = DateTime.Now.Date;
                dateTimePickerProtocolDate.Checked = false;
            }
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_funds_history.Position != -1) && (FundHistoryFromView() != FundHistoryFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    dataGridView.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    dataGridView.Enabled = true;
                }
            }
            if (Selected)
                MenuCallback.EditingStateUpdate();
        }

        private bool ChangeViewportStateTo(ViewportState state)
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
                            if (funds_history.EditingNewRecord)
                                return false;
                            else
                            {
                                viewportState = ViewportState.NewRowState;
                                return true;
                            }
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
            var position = v_funds_history.Find("id_fund", id);
            is_editable = false;
            if (position > 0)
                v_funds_history.Position = position;
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

        private FundHistory FundHistoryFromViewport()
        {
            var fundHistory = new FundHistory
            {
                IdFund = v_funds_history.Position == -1
                    ? null
                    : ViewportHelper.ValueOrNull<int>((DataRowView) v_funds_history[v_funds_history.Position], "id_fund"),
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

        private FundHistory FundHistoryFromView()
        {
            var fundHistory = new FundHistory();
            var row = (DataRowView)v_funds_history[v_funds_history.Position];
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

        public override int GetRecordCount()
        {
            return v_funds_history.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_funds_history.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_funds_history.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_funds_history.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_funds_history.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_funds_history.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_funds_history.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_funds_history.Position > -1) && (v_funds_history.Position < (v_funds_history.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_funds_history.Position > -1) && (v_funds_history.Position < (v_funds_history.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            funds_history = DataModel.GetInstance(DataModelType.FundsHistoryDataModel);
            fund_types = DataModel.GetInstance(DataModelType.FundTypesDataModel);

            // Ожидаем дозагрузки, если это необходимо
            funds_history.Select();
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

            v_funds_history = new BindingSource();
            v_funds_history.CurrentItemChanged += v_funds_history_CurrentItemChanged;
            v_funds_history.DataMember = "funds_history";
            v_funds_history.DataSource = ds;
            //Перестраиваем фильтр v_funds_history.Filter
            RebuildFilter();

            DataBind();

            funds_history.Select().RowChanged += FundsHistoryViewport_RowChanged;
            funds_history.Select().RowDeleted += FundsHistoryViewport_RowDeleted;
            fund_assoc.Select().RowChanged += FundAssoc_RowChanged;
            fund_assoc.Select().RowDeleted += FundAssoc_RowDeleted;
            comboBoxFundType.SelectedIndexChanged += comboBoxFundType_SelectedIndexChanged;
            is_editable = true;
            if (v_funds_history.Count == 0)
                InsertRecord();
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            var fundHistory = FundHistoryFromViewport();
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
                    var idFund = funds_history.Insert(fundHistory);
                    if (idFund == -1)
                    {
                        funds_history.EditingNewRecord = false;
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
                    if (v_funds_history.Position == -1)
                        newRow = (DataRowView)v_funds_history.AddNew();
                    else
                        newRow = ((DataRowView)v_funds_history[v_funds_history.Position]);
                    FillRowFromFundHistory(fundHistory, newRow);
                    fund_assoc.Select().Rows.Add(idParent, idFund);
                    RebuildFilter();
                    v_funds_history.Position = v_funds_history.Count - 1;
                    funds_history.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (fundHistory.IdFund == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о принадлежности фонду без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (funds_history.Update(fundHistory) == -1)
                        return;
                    is_editable = false;
                    var row = ((DataRowView)v_funds_history[v_funds_history.Position]);
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
            return (v_funds_history.Position != -1) && (!funds_history.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            comboBoxFundType.Focus();
            is_editable = false;
            var fundHistory = FundHistoryFromView();
            v_funds_history.AddNew();
            dataGridView.Enabled = false;
            funds_history.EditingNewRecord = true;
            ViewportFromFundHistory(fundHistory);
            checkBoxIncludeRest.Checked = (fundHistory.IncludeRestrictionDate != null);
            checkBoxExcludeRest.Checked = (fundHistory.ExcludeRestrictionDate != null);
            is_editable = true;
        }

        public override bool CanInsertRecord()
        {
            return (!funds_history.EditingNewRecord) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            comboBoxFundType.Focus();
            is_editable = false;
            v_funds_history.AddNew();
            dataGridView.Enabled = false;
            is_editable = true;
            funds_history.EditingNewRecord = true;
            UnbindedCheckBoxesUpdate();
        }

        public override bool CanDeleteRecord()
        {
            return (v_funds_history.Position > -1)
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
                if (funds_history.Delete((int)((DataRowView)v_funds_history.Current)["id_fund"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_funds_history[v_funds_history.Position]).Delete();
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
                    funds_history.EditingNewRecord = false;
                    if (v_funds_history.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)v_funds_history[v_funds_history.Position]).Delete();
                        RedrawDataGridRows();
                        if (v_funds_history.Position != -1)
                            dataGridView.Rows[v_funds_history.Position].Selected = true;
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
            if (v_funds_history.Count > 0)
                viewport.LocateFundHistoryBy((((DataRowView)v_funds_history[v_funds_history.Position])["id_fund"] as int?) ?? -1);
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
                funds_history.Select().RowChanged -= FundsHistoryViewport_RowChanged;
                funds_history.Select().RowDeleted -= FundsHistoryViewport_RowDeleted;
            }
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                funds_history.EditingNewRecord = false;
            fund_assoc.Select().RowChanged -= FundAssoc_RowChanged;
            fund_assoc.Select().RowDeleted -= FundAssoc_RowDeleted;
            funds_history.Select().RowChanged -= FundsHistoryViewport_RowChanged;
            funds_history.Select().RowDeleted -= FundsHistoryViewport_RowDeleted;
            Close();
        }

        void v_funds_history_CurrentItemChanged(object sender, EventArgs e)
        {
            if (v_funds_history.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (v_funds_history.Position >= dataGridView.RowCount)
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                else
                    if (dataGridView.Rows[v_funds_history.Position].Selected != true)
                        dataGridView.Rows[v_funds_history.Position].Selected = true;
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            if (v_funds_history.Position == -1)
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

        void dateTimePickerExcludeRestDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxExcludeRestDesc_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxExcludeRestNum_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerIncludeRestDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxIncludeRestDesc_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxIncludeRestNum_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dateTimePickerProtocolDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void textBoxProtocolNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void comboBoxFundType_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxProtocolNumber.Enabled = ((comboBoxFundType.SelectedValue) != null &&
                (Convert.ToInt32(comboBoxFundType.SelectedValue, CultureInfo.InvariantCulture) != 1));
            dateTimePickerProtocolDate.Enabled = ((comboBoxFundType.SelectedValue) != null &&
                (Convert.ToInt32(comboBoxFundType.SelectedValue, CultureInfo.InvariantCulture) != 1));
            CheckViewportModifications();
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

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(FundsHistoryViewport));
            tableLayoutPanel6 = new TableLayoutPanel();
            tableLayoutPanel8 = new TableLayoutPanel();
            groupBox14 = new GroupBox();
            dateTimePickerProtocolDate = new DateTimePicker();
            label37 = new Label();
            label36 = new Label();
            textBoxProtocolNumber = new TextBox();
            comboBoxFundType = new ComboBox();
            label35 = new Label();
            groupBox17 = new GroupBox();
            textBoxDescription = new TextBox();
            tableLayoutPanel7 = new TableLayoutPanel();
            groupBox15 = new GroupBox();
            checkBoxIncludeRest = new CheckBox();
            label31 = new Label();
            textBoxIncludeRestDesc = new TextBox();
            dateTimePickerIncludeRestDate = new DateTimePicker();
            label30 = new Label();
            label29 = new Label();
            textBoxIncludeRestNum = new TextBox();
            groupBox16 = new GroupBox();
            checkBoxExcludeRest = new CheckBox();
            label32 = new Label();
            textBoxExcludeRestDesc = new TextBox();
            dateTimePickerExcludeRestDate = new DateTimePicker();
            label33 = new Label();
            label34 = new Label();
            textBoxExcludeRestNum = new TextBox();
            dataGridView = new DataGridView();
            id_fund = new DataGridViewTextBoxColumn();
            protocol_number = new DataGridViewTextBoxColumn();
            protocol_date = new DataGridViewDateTimeColumn();
            id_fund_type = new DataGridViewComboBoxColumn();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            groupBox14.SuspendLayout();
            groupBox17.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            groupBox15.SuspendLayout();
            groupBox16.SuspendLayout();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Controls.Add(tableLayoutPanel8, 0, 0);
            tableLayoutPanel6.Controls.Add(tableLayoutPanel7, 1, 0);
            tableLayoutPanel6.Controls.Add(dataGridView, 0, 1);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Absolute, 230F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.Size = new Size(708, 336);
            tableLayoutPanel6.TabIndex = 0;
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.ColumnCount = 1;
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel8.Controls.Add(groupBox14, 0, 0);
            tableLayoutPanel8.Controls.Add(groupBox17, 0, 1);
            tableLayoutPanel8.Dock = DockStyle.Fill;
            tableLayoutPanel8.Location = new Point(3, 3);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 2;
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.Size = new Size(348, 224);
            tableLayoutPanel8.TabIndex = 1;
            // 
            // groupBox14
            // 
            groupBox14.Controls.Add(dateTimePickerProtocolDate);
            groupBox14.Controls.Add(label37);
            groupBox14.Controls.Add(label36);
            groupBox14.Controls.Add(textBoxProtocolNumber);
            groupBox14.Controls.Add(comboBoxFundType);
            groupBox14.Controls.Add(label35);
            groupBox14.Dock = DockStyle.Fill;
            groupBox14.Location = new Point(3, 3);
            groupBox14.Name = "groupBox14";
            groupBox14.Size = new Size(342, 106);
            groupBox14.TabIndex = 0;
            groupBox14.TabStop = false;
            groupBox14.Text = @"Общие сведения";
            // 
            // dateTimePickerProtocolDate
            // 
            dateTimePickerProtocolDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                | AnchorStyles.Right;
            dateTimePickerProtocolDate.Enabled = false;
            dateTimePickerProtocolDate.Location = new Point(161, 77);
            dateTimePickerProtocolDate.Name = "dateTimePickerProtocolDate";
            dateTimePickerProtocolDate.ShowCheckBox = true;
            dateTimePickerProtocolDate.Size = new Size(175, 21);
            dateTimePickerProtocolDate.TabIndex = 2;
            dateTimePickerProtocolDate.ValueChanged += dateTimePickerProtocolDate_ValueChanged;
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Location = new Point(14, 80);
            label37.Name = "label37";
            label37.Size = new Size(124, 15);
            label37.TabIndex = 3;
            label37.Text = @"Дата протокола ЖК";
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.Location = new Point(14, 54);
            label36.Name = "label36";
            label36.Size = new Size(133, 15);
            label36.TabIndex = 4;
            label36.Text = @"Номер протокола ЖК";
            // 
            // textBoxProtocolNumber
            // 
            textBoxProtocolNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            textBoxProtocolNumber.Enabled = false;
            textBoxProtocolNumber.Location = new Point(161, 51);
            textBoxProtocolNumber.MaxLength = 50;
            textBoxProtocolNumber.Name = "textBoxProtocolNumber";
            textBoxProtocolNumber.Size = new Size(175, 21);
            textBoxProtocolNumber.TabIndex = 1;
            textBoxProtocolNumber.TextChanged += textBoxProtocolNumber_TextChanged;
            textBoxProtocolNumber.Enter += selectAll_Enter;
            // 
            // comboBoxFundType
            // 
            comboBoxFundType.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                      | AnchorStyles.Right;
            comboBoxFundType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFundType.FormattingEnabled = true;
            comboBoxFundType.Location = new Point(161, 24);
            comboBoxFundType.Name = "comboBoxFundType";
            comboBoxFundType.Size = new Size(175, 23);
            comboBoxFundType.TabIndex = 0;
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Location = new Point(14, 27);
            label35.Name = "label35";
            label35.Size = new Size(68, 15);
            label35.TabIndex = 5;
            label35.Text = @"Тип найма";
            // 
            // groupBox17
            // 
            groupBox17.Controls.Add(textBoxDescription);
            groupBox17.Dock = DockStyle.Fill;
            groupBox17.Location = new Point(3, 115);
            groupBox17.Name = "groupBox17";
            groupBox17.Size = new Size(342, 106);
            groupBox17.TabIndex = 1;
            groupBox17.TabStop = false;
            groupBox17.Text = @"Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.Location = new Point(3, 17);
            textBoxDescription.MaxLength = 4000;
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(336, 86);
            textBoxDescription.TabIndex = 4;
            textBoxDescription.TextChanged += textBoxDescription_TextChanged;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 1;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel7.Controls.Add(groupBox15, 0, 0);
            tableLayoutPanel7.Controls.Add(groupBox16, 0, 1);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(357, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 2;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.Size = new Size(348, 224);
            tableLayoutPanel7.TabIndex = 1;
            // 
            // groupBox15
            // 
            groupBox15.Controls.Add(checkBoxIncludeRest);
            groupBox15.Controls.Add(label31);
            groupBox15.Controls.Add(textBoxIncludeRestDesc);
            groupBox15.Controls.Add(dateTimePickerIncludeRestDate);
            groupBox15.Controls.Add(label30);
            groupBox15.Controls.Add(label29);
            groupBox15.Controls.Add(textBoxIncludeRestNum);
            groupBox15.Dock = DockStyle.Fill;
            groupBox15.Location = new Point(3, 3);
            groupBox15.Name = "groupBox15";
            groupBox15.Size = new Size(342, 106);
            groupBox15.TabIndex = 0;
            groupBox15.TabStop = false;
            groupBox15.Text = @"      Реквизиты НПА по включению в фонд";
            // 
            // checkBoxIncludeRest
            // 
            checkBoxIncludeRest.AutoSize = true;
            checkBoxIncludeRest.Location = new Point(11, 0);
            checkBoxIncludeRest.Name = "checkBoxIncludeRest";
            checkBoxIncludeRest.Size = new Size(15, 14);
            checkBoxIncludeRest.TabIndex = 5;
            checkBoxIncludeRest.UseVisualStyleBackColor = true;
            checkBoxIncludeRest.CheckedChanged += checkBoxIncludeRest_CheckedChanged;
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Location = new Point(8, 77);
            label31.Name = "label31";
            label31.Size = new Size(95, 15);
            label31.TabIndex = 6;
            label31.Text = @"Наименование";
            // 
            // textBoxIncludeRestDesc
            // 
            textBoxIncludeRestDesc.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            textBoxIncludeRestDesc.Enabled = false;
            textBoxIncludeRestDesc.Location = new Point(161, 74);
            textBoxIncludeRestDesc.MaxLength = 255;
            textBoxIncludeRestDesc.Name = "textBoxIncludeRestDesc";
            textBoxIncludeRestDesc.Size = new Size(175, 21);
            textBoxIncludeRestDesc.TabIndex = 8;
            textBoxIncludeRestDesc.TextChanged += textBoxIncludeRestDesc_TextChanged;
            textBoxIncludeRestDesc.Enter += selectAll_Enter;
            // 
            // dateTimePickerIncludeRestDate
            // 
            dateTimePickerIncludeRestDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                   | AnchorStyles.Right;
            dateTimePickerIncludeRestDate.Enabled = false;
            dateTimePickerIncludeRestDate.Location = new Point(161, 48);
            dateTimePickerIncludeRestDate.Name = "dateTimePickerIncludeRestDate";
            dateTimePickerIncludeRestDate.Size = new Size(175, 21);
            dateTimePickerIncludeRestDate.TabIndex = 7;
            dateTimePickerIncludeRestDate.ValueChanged += dateTimePickerIncludeRestDate_ValueChanged;
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Location = new Point(8, 52);
            label30.Name = "label30";
            label30.Size = new Size(101, 15);
            label30.TabIndex = 9;
            label30.Text = @"Дата реквизита";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(8, 25);
            label29.Name = "label29";
            label29.Size = new Size(110, 15);
            label29.TabIndex = 10;
            label29.Text = @"Номер реквизита";
            // 
            // textBoxIncludeRestNum
            // 
            textBoxIncludeRestNum.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            textBoxIncludeRestNum.Enabled = false;
            textBoxIncludeRestNum.Location = new Point(161, 22);
            textBoxIncludeRestNum.MaxLength = 30;
            textBoxIncludeRestNum.Name = "textBoxIncludeRestNum";
            textBoxIncludeRestNum.Size = new Size(175, 21);
            textBoxIncludeRestNum.TabIndex = 6;
            textBoxIncludeRestNum.TextChanged += textBoxIncludeRestNum_TextChanged;
            textBoxIncludeRestNum.Enter += selectAll_Enter;
            // 
            // groupBox16
            // 
            groupBox16.Controls.Add(checkBoxExcludeRest);
            groupBox16.Controls.Add(label32);
            groupBox16.Controls.Add(textBoxExcludeRestDesc);
            groupBox16.Controls.Add(dateTimePickerExcludeRestDate);
            groupBox16.Controls.Add(label33);
            groupBox16.Controls.Add(label34);
            groupBox16.Controls.Add(textBoxExcludeRestNum);
            groupBox16.Dock = DockStyle.Fill;
            groupBox16.Location = new Point(3, 115);
            groupBox16.Name = "groupBox16";
            groupBox16.Size = new Size(342, 106);
            groupBox16.TabIndex = 1;
            groupBox16.TabStop = false;
            groupBox16.Text = @"      Реквизиты НПА по исключению из фонда";
            // 
            // checkBoxExcludeRest
            // 
            checkBoxExcludeRest.AutoSize = true;
            checkBoxExcludeRest.Location = new Point(11, 0);
            checkBoxExcludeRest.Name = "checkBoxExcludeRest";
            checkBoxExcludeRest.Size = new Size(15, 14);
            checkBoxExcludeRest.TabIndex = 9;
            checkBoxExcludeRest.UseVisualStyleBackColor = true;
            checkBoxExcludeRest.CheckedChanged += checkBoxExcludeRest_CheckedChanged;
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Location = new Point(8, 76);
            label32.Name = "label32";
            label32.Size = new Size(95, 15);
            label32.TabIndex = 10;
            label32.Text = @"Наименование";
            // 
            // textBoxExcludeRestDesc
            // 
            textBoxExcludeRestDesc.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                            | AnchorStyles.Right;
            textBoxExcludeRestDesc.Enabled = false;
            textBoxExcludeRestDesc.Location = new Point(161, 73);
            textBoxExcludeRestDesc.MaxLength = 255;
            textBoxExcludeRestDesc.Name = "textBoxExcludeRestDesc";
            textBoxExcludeRestDesc.Size = new Size(175, 21);
            textBoxExcludeRestDesc.TabIndex = 12;
            textBoxExcludeRestDesc.TextChanged += textBoxExcludeRestDesc_TextChanged;
            textBoxExcludeRestDesc.Enter += selectAll_Enter;
            // 
            // dateTimePickerExcludeRestDate
            // 
            dateTimePickerExcludeRestDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                                   | AnchorStyles.Right;
            dateTimePickerExcludeRestDate.Enabled = false;
            dateTimePickerExcludeRestDate.Location = new Point(161, 47);
            dateTimePickerExcludeRestDate.Name = "dateTimePickerExcludeRestDate";
            dateTimePickerExcludeRestDate.Size = new Size(175, 21);
            dateTimePickerExcludeRestDate.TabIndex = 11;
            dateTimePickerExcludeRestDate.ValueChanged += dateTimePickerExcludeRestDate_ValueChanged;
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Location = new Point(8, 51);
            label33.Name = "label33";
            label33.Size = new Size(101, 15);
            label33.TabIndex = 13;
            label33.Text = @"Дата реквизита";
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Location = new Point(8, 24);
            label34.Name = "label34";
            label34.Size = new Size(110, 15);
            label34.TabIndex = 14;
            label34.Text = @"Номер реквизита";
            // 
            // textBoxExcludeRestNum
            // 
            textBoxExcludeRestNum.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                           | AnchorStyles.Right;
            textBoxExcludeRestNum.Enabled = false;
            textBoxExcludeRestNum.Location = new Point(161, 21);
            textBoxExcludeRestNum.MaxLength = 30;
            textBoxExcludeRestNum.Name = "textBoxExcludeRestNum";
            textBoxExcludeRestNum.Size = new Size(175, 21);
            textBoxExcludeRestNum.TabIndex = 10;
            textBoxExcludeRestNum.TextChanged += textBoxExcludeRestNum_TextChanged;
            textBoxExcludeRestNum.Enter += selectAll_Enter;
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            dataGridView.Columns.AddRange(id_fund, protocol_number, protocol_date, id_fund_type);
            tableLayoutPanel6.SetColumnSpan(dataGridView, 2);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 233);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(702, 100);
            dataGridView.TabIndex = 0;
            dataGridView.DataError += dataGridView_DataError;
            // 
            // id_fund
            // 
            id_fund.HeaderText = @"Идентификатор фонда";
            id_fund.Name = "id_fund";
            id_fund.ReadOnly = true;
            id_fund.Visible = false;
            // 
            // protocol_number
            // 
            protocol_number.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            protocol_number.HeaderText = @"Номер протокола";
            protocol_number.MinimumWidth = 150;
            protocol_number.Name = "protocol_number";
            protocol_number.ReadOnly = true;
            protocol_number.SortMode = DataGridViewColumnSortMode.NotSortable;
            protocol_number.Width = 150;
            // 
            // protocol_date
            // 
            protocol_date.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            protocol_date.HeaderText = @"Дата протокола";
            protocol_date.MinimumWidth = 150;
            protocol_date.Name = "protocol_date";
            protocol_date.ReadOnly = true;
            protocol_date.Width = 150;
            // 
            // id_fund_type
            // 
            id_fund_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            id_fund_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_fund_type.HeaderText = @"Наименование";
            id_fund_type.MinimumWidth = 250;
            id_fund_type.Name = "id_fund_type";
            id_fund_type.ReadOnly = true;
            // 
            // FundsHistoryViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(640, 320);
            BackColor = Color.White;
            ClientSize = new Size(714, 342);
            Controls.Add(tableLayoutPanel6);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "FundsHistoryViewport";
            Padding = new Padding(3);
            Text = @"История фонда";
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel8.ResumeLayout(false);
            groupBox14.ResumeLayout(false);
            groupBox14.PerformLayout();
            groupBox17.ResumeLayout(false);
            groupBox17.PerformLayout();
            tableLayoutPanel7.ResumeLayout(false);
            groupBox15.ResumeLayout(false);
            groupBox15.PerformLayout();
            groupBox16.ResumeLayout(false);
            groupBox16.PerformLayout();
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }

    }
}
