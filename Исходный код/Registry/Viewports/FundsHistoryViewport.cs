using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CustomControls;
using Registry.DataModels;
using System.Data;
using Registry.Entities;
using System.Drawing;
using Registry.CalcDataModels;
using Security;
using System.Globalization;

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
        #endregion Components

        #region Models
        FundsHistoryDataModel funds_history = null;
        FundTypesDataModel fund_types = null;
        DataModel fund_assoc = null;
        #endregion Models

        #region Views
        BindingSource v_funds_history = null;
        BindingSource v_fund_types = null;
        BindingSource v_fund_assoc = null;
        #endregion Views

        private ViewportState viewportState = ViewportState.ReadState;
        private DataGridViewTextBoxColumn id_fund;
        private DataGridViewTextBoxColumn protocol_number;
        private DataGridViewDateTimeColumn protocol_date;
        private DataGridViewComboBoxColumn id_fund_type;
        private bool is_editable = false;

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
            this.DynamicFilter = fundsHistoryViewport.DynamicFilter;
            this.StaticFilter = fundsHistoryViewport.StaticFilter;
            this.ParentRow = fundsHistoryViewport.ParentRow;
            this.ParentType = fundsHistoryViewport.ParentType;
        }

        private void RedrawDataGridRows()
        {
            if (dataGridView.Rows.Count == 0)
                return;
            bool currentFundFounded = false;
            for (int i = dataGridView.Rows.Count - 1; i >= 0; i--)
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
            string filter = "id_fund IN (0";
            for (int i = 0; i < v_fund_assoc.Count; i++)
                filter += ((DataRowView)v_fund_assoc[i])["id_fund"].ToString() + ",";
            filter = filter.TrimEnd(new char[] { ',' });
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
            checkBoxIncludeRest.Checked = (v_funds_history.Position >= 0) &&
                (((DataRowView)v_funds_history[v_funds_history.Position])["include_restriction_date"] != DBNull.Value);
            checkBoxExcludeRest.Checked = (v_funds_history.Position >= 0) &&
                (((DataRowView)v_funds_history[v_funds_history.Position])["exclude_restriction_date"] != DBNull.Value);
            if ((v_funds_history.Position >= 0) &&
                (((DataRowView)v_funds_history[v_funds_history.Position])["protocol_date"] != DBNull.Value))
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
            if ((!this.ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_funds_history.Position != -1) && (FundHistoryFromView() != FundHistoryFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    MenuCallback.EditingStateUpdate();
                    dataGridView.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    MenuCallback.EditingStateUpdate();
                    dataGridView.Enabled = true;
                }
            }
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
                case ViewportState.ModifyRowState: ;
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.ModifyRowState);
                            else
                                return false;
                    }
                    break;
            }
            return false;
        }

        private void LocateFundHistoryBy(int id)
        {
            int Position = v_funds_history.Find("id_fund", id);
            is_editable = false;
            if (Position > 0)
                v_funds_history.Position = Position;
            is_editable = true;
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
            FundHistory fundHistory = new FundHistory();
            if (v_funds_history.Position == -1)
                fundHistory.IdFund = null;
            else
                fundHistory.IdFund = ViewportHelper.ValueOrNull<int>((DataRowView)v_funds_history[v_funds_history.Position], "id_fund");
            fundHistory.IdFundType = ViewportHelper.ValueOrNull<int>(comboBoxFundType);
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
            FundHistory fundHistory = new FundHistory();
            DataRowView row = (DataRowView)v_funds_history[v_funds_history.Position];
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
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            funds_history = FundsHistoryDataModel.GetInstance();
            fund_types = FundTypesDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            funds_history.Select();
            fund_types.Select();

            if (ParentType == ParentTypeEnum.SubPremises)
                fund_assoc = FundsSubPremisesAssocDataModel.GetInstance();
            else
                if (ParentType == ParentTypeEnum.Premises)
                    fund_assoc = FundsPremisesAssocDataModel.GetInstance();
                else
                    if (ParentType == ParentTypeEnum.Building)
                        fund_assoc = FundsBuildingsAssocDataModel.GetInstance();
                    else
                        throw new ViewportException("Неизвестный тип родительского объекта");

            DataSet ds = DataSetManager.DataSet;

            v_fund_assoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.SubPremises) && (ParentRow != null))
            {
                v_fund_assoc.DataMember = "funds_sub_premises_assoc";
                v_fund_assoc.Filter = "id_sub_premises = " + ParentRow["id_sub_premises"].ToString();
                this.Text = String.Format(CultureInfo.InvariantCulture, "История найма комнаты №{0} помещения №{1}", ParentRow["sub_premises_num"].ToString(),
                    ParentRow["id_premises"].ToString());
            }
            else
                if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
                {
                    v_fund_assoc.DataMember = "funds_premises_assoc";
                    v_fund_assoc.Filter = "id_premises = " + ParentRow["id_premises"].ToString();
                    this.Text = String.Format(CultureInfo.InvariantCulture, "История найма помещения №{0}", ParentRow["id_premises"].ToString());
                }
                else
                    if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                    {
                        v_fund_assoc.DataMember = "funds_buildings_assoc";
                        v_fund_assoc.Filter = "id_building = " + ParentRow["id_building"].ToString();
                        this.Text = String.Format(CultureInfo.InvariantCulture, "История найма здания №{0}", ParentRow["id_building"].ToString());
                    }
                    else
                        throw new ViewportException("Неизвестный тип родительского объекта");
            v_fund_assoc.DataSource = ds;

            v_fund_types = new BindingSource();
            v_fund_types.DataMember = "fund_types";
            v_fund_types.DataSource = ds;

            v_funds_history = new BindingSource();
            v_funds_history.CurrentItemChanged += new EventHandler(v_funds_history_CurrentItemChanged);
            v_funds_history.DataMember = "funds_history";
            v_funds_history.DataSource = ds;
            //Перестраиваем фильтр v_funds_history.Filter
            RebuildFilter();

            DataBind();

            funds_history.Select().RowChanged += new DataRowChangeEventHandler(FundsHistoryViewport_RowChanged);
            funds_history.Select().RowDeleted += new DataRowChangeEventHandler(FundsHistoryViewport_RowDeleted);
            fund_assoc.Select().RowChanged += new DataRowChangeEventHandler(FundAssoc_RowChanged);
            fund_assoc.Select().RowDeleted += new DataRowChangeEventHandler(FundAssoc_RowDeleted);
            comboBoxFundType.SelectedIndexChanged += new EventHandler(comboBoxFundType_SelectedIndexChanged);
            is_editable = true;
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || (AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)));
        }

        public override void SaveRecord()
        {
            FundHistory fundHistory = FundHistoryFromViewport();
            if (!ValidateFundHistory(fundHistory))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    int id_parent =
                        ((ParentType == ParentTypeEnum.SubPremises) && ParentRow != null) ? (int)ParentRow["id_sub_premises"] :
                        ((ParentType == ParentTypeEnum.Premises) && ParentRow != null) ? (int)ParentRow["id_premises"] :
                        ((ParentType == ParentTypeEnum.Building) && ParentRow != null) ? (int)ParentRow["id_building"] :
                        -1;
                    if (id_parent == -1)
                    {
                        MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    int id_fund = FundsHistoryDataModel.Insert(fundHistory, ParentType, id_parent);
                    if (id_fund == -1)
                        return;
                    DataRowView newRow;
                    fundHistory.IdFund = id_fund;
                    is_editable = false;
                    if (v_funds_history.Position == -1)
                        newRow = (DataRowView)v_funds_history.AddNew();
                    else
                        newRow = ((DataRowView)v_funds_history[v_funds_history.Position]);
                    FillRowFromFundHistory(fundHistory, newRow);
                    funds_history.EditingNewRecord = false;
                    fund_assoc.Select().Rows.Add(new object[] { id_parent, id_fund });
                    RebuildFilter();
                    v_funds_history.Position = v_funds_history.Count - 1;
                    break;
                case ViewportState.ModifyRowState:
                    if (fundHistory.IdFund == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись о принадлежности фонду без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (FundsHistoryDataModel.Update(fundHistory) == -1)
                        return;
                    is_editable = false;
                    DataRowView row = ((DataRowView)v_funds_history[v_funds_history.Position]);
                    FillRowFromFundHistory(fundHistory, row);
                    break;
            }
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            dataGridView.Enabled = true;
            is_editable = true;
            viewportState = ViewportState.ReadState;
            if (ParentType == ParentTypeEnum.Building || ParentType == ParentTypeEnum.Premises)
                CalcDataModelBuildingsPremisesFunds.GetInstance().Refresh(EntityType.Building, (int)ParentRow["id_building"], true);
            if (ParentType == ParentTypeEnum.Building)
                CalcDataModelBuildingsCurrentFunds.GetInstance().Refresh(EntityType.Building, (int)ParentRow["id_building"], true);
            else
                if (ParentType == ParentTypeEnum.Premises)
                    CalcDataModelPremisesCurrentFunds.GetInstance().Refresh(EntityType.Premise, (int)ParentRow["id_premises"], true);
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
            is_editable = false;
            FundHistory fundHistory = FundHistoryFromView();
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
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (ValidatePermissions() == false)
                    return;
                if (FundsHistoryDataModel.Delete((int)((DataRowView)v_funds_history.Current)["id_fund"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_funds_history[v_funds_history.Position]).Delete();
                is_editable = true;
                RedrawDataGridRows();
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
                if (ParentType == ParentTypeEnum.Building || ParentType == ParentTypeEnum.Premises)
                    CalcDataModelBuildingsPremisesFunds.GetInstance().Refresh(EntityType.Building, (int)ParentRow["id_building"], true);
                if (ParentType == ParentTypeEnum.Building)
                    CalcDataModelBuildingsCurrentFunds.GetInstance().Refresh(EntityType.Building, (int)ParentRow["id_building"], true);
                else
                    if (ParentType == ParentTypeEnum.Premises)
                        CalcDataModelPremisesCurrentFunds.GetInstance().Refresh(EntityType.Premise, (int)ParentRow["id_premises"], true);
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
            FundsHistoryViewport viewport = new FundsHistoryViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_funds_history.Count > 0)
                viewport.LocateFundHistoryBy((((DataRowView)v_funds_history[v_funds_history.Position])["id_fund"] as Int32?) ?? -1);
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            else
            {
                fund_assoc.Select().RowChanged -= new DataRowChangeEventHandler(FundAssoc_RowChanged);
                fund_assoc.Select().RowDeleted -= new DataRowChangeEventHandler(FundAssoc_RowDeleted);
                funds_history.Select().RowChanged -= new DataRowChangeEventHandler(FundsHistoryViewport_RowChanged);
                funds_history.Select().RowDeleted -= new DataRowChangeEventHandler(FundsHistoryViewport_RowDeleted);
            }
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                funds_history.EditingNewRecord = false;
            fund_assoc.Select().RowChanged -= new DataRowChangeEventHandler(FundAssoc_RowChanged);
            fund_assoc.Select().RowDeleted -= new DataRowChangeEventHandler(FundAssoc_RowDeleted);
            funds_history.Select().RowChanged -= new DataRowChangeEventHandler(FundsHistoryViewport_RowChanged);
            funds_history.Select().RowDeleted -= new DataRowChangeEventHandler(FundsHistoryViewport_RowDeleted);
            base.Close();
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
            if (is_editable)
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
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
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
            this.textBoxExcludeRestNum.Enabled = checkBoxExcludeRest.Checked;
            this.textBoxExcludeRestDesc.Enabled = checkBoxExcludeRest.Checked;
            this.dateTimePickerExcludeRestDate.Enabled = checkBoxExcludeRest.Checked;
            CheckViewportModifications();
        }

        void checkBoxIncludeRest_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxIncludeRestNum.Enabled = checkBoxIncludeRest.Checked;
            this.textBoxIncludeRestDesc.Enabled = checkBoxIncludeRest.Checked;
            this.dateTimePickerIncludeRestDate.Enabled = checkBoxIncludeRest.Checked;
            CheckViewportModifications();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FundsHistoryViewport));
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.dateTimePickerProtocolDate = new System.Windows.Forms.DateTimePicker();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.textBoxProtocolNumber = new System.Windows.Forms.TextBox();
            this.comboBoxFundType = new System.Windows.Forms.ComboBox();
            this.label35 = new System.Windows.Forms.Label();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.checkBoxIncludeRest = new System.Windows.Forms.CheckBox();
            this.label31 = new System.Windows.Forms.Label();
            this.textBoxIncludeRestDesc = new System.Windows.Forms.TextBox();
            this.dateTimePickerIncludeRestDate = new System.Windows.Forms.DateTimePicker();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.textBoxIncludeRestNum = new System.Windows.Forms.TextBox();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.checkBoxExcludeRest = new System.Windows.Forms.CheckBox();
            this.label32 = new System.Windows.Forms.Label();
            this.textBoxExcludeRestDesc = new System.Windows.Forms.TextBox();
            this.dateTimePickerExcludeRestDate = new System.Windows.Forms.DateTimePicker();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.textBoxExcludeRestNum = new System.Windows.Forms.TextBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_fund = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.protocol_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.protocol_date = new CustomControls.DataGridViewDateTimeColumn();
            this.id_fund_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel8, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.dataGridView, 0, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(708, 336);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.Controls.Add(this.groupBox14, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.groupBox17, 0, 1);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(348, 224);
            this.tableLayoutPanel8.TabIndex = 1;
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.dateTimePickerProtocolDate);
            this.groupBox14.Controls.Add(this.label37);
            this.groupBox14.Controls.Add(this.label36);
            this.groupBox14.Controls.Add(this.textBoxProtocolNumber);
            this.groupBox14.Controls.Add(this.comboBoxFundType);
            this.groupBox14.Controls.Add(this.label35);
            this.groupBox14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox14.Location = new System.Drawing.Point(3, 3);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(342, 106);
            this.groupBox14.TabIndex = 0;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Общие сведения";
            // 
            // dateTimePickerProtocolDate
            // 
            this.dateTimePickerProtocolDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerProtocolDate.Enabled = false;
            this.dateTimePickerProtocolDate.Location = new System.Drawing.Point(161, 77);
            this.dateTimePickerProtocolDate.Name = "dateTimePickerProtocolDate";
            this.dateTimePickerProtocolDate.ShowCheckBox = true;
            this.dateTimePickerProtocolDate.Size = new System.Drawing.Size(175, 21);
            this.dateTimePickerProtocolDate.TabIndex = 2;
            this.dateTimePickerProtocolDate.ValueChanged += new System.EventHandler(this.dateTimePickerProtocolDate_ValueChanged);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(14, 80);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(124, 15);
            this.label37.TabIndex = 3;
            this.label37.Text = "Дата протокола ЖК";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(14, 54);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(133, 15);
            this.label36.TabIndex = 4;
            this.label36.Text = "Номер протокола ЖК";
            // 
            // textBoxProtocolNumber
            // 
            this.textBoxProtocolNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProtocolNumber.Enabled = false;
            this.textBoxProtocolNumber.Location = new System.Drawing.Point(161, 51);
            this.textBoxProtocolNumber.MaxLength = 50;
            this.textBoxProtocolNumber.Name = "textBoxProtocolNumber";
            this.textBoxProtocolNumber.Size = new System.Drawing.Size(175, 21);
            this.textBoxProtocolNumber.TabIndex = 1;
            this.textBoxProtocolNumber.TextChanged += new System.EventHandler(this.textBoxProtocolNumber_TextChanged);
            // 
            // comboBoxFundType
            // 
            this.comboBoxFundType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxFundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFundType.FormattingEnabled = true;
            this.comboBoxFundType.Location = new System.Drawing.Point(161, 24);
            this.comboBoxFundType.Name = "comboBoxFundType";
            this.comboBoxFundType.Size = new System.Drawing.Size(175, 23);
            this.comboBoxFundType.TabIndex = 0;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(14, 27);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(68, 15);
            this.label35.TabIndex = 5;
            this.label35.Text = "Тип найма";
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.textBoxDescription);
            this.groupBox17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox17.Location = new System.Drawing.Point(3, 115);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(342, 106);
            this.groupBox17.TabIndex = 1;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(3, 17);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(336, 86);
            this.textBoxDescription.TabIndex = 4;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel7.Controls.Add(this.groupBox15, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.groupBox16, 0, 1);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(357, 3);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 2;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(348, 224);
            this.tableLayoutPanel7.TabIndex = 1;
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.checkBoxIncludeRest);
            this.groupBox15.Controls.Add(this.label31);
            this.groupBox15.Controls.Add(this.textBoxIncludeRestDesc);
            this.groupBox15.Controls.Add(this.dateTimePickerIncludeRestDate);
            this.groupBox15.Controls.Add(this.label30);
            this.groupBox15.Controls.Add(this.label29);
            this.groupBox15.Controls.Add(this.textBoxIncludeRestNum);
            this.groupBox15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox15.Location = new System.Drawing.Point(3, 3);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(342, 106);
            this.groupBox15.TabIndex = 0;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "      Реквизиты НПА по включению в фонд";
            // 
            // checkBoxIncludeRest
            // 
            this.checkBoxIncludeRest.AutoSize = true;
            this.checkBoxIncludeRest.Location = new System.Drawing.Point(11, 0);
            this.checkBoxIncludeRest.Name = "checkBoxIncludeRest";
            this.checkBoxIncludeRest.Size = new System.Drawing.Size(15, 14);
            this.checkBoxIncludeRest.TabIndex = 5;
            this.checkBoxIncludeRest.UseVisualStyleBackColor = true;
            this.checkBoxIncludeRest.CheckedChanged += new System.EventHandler(this.checkBoxIncludeRest_CheckedChanged);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(8, 77);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(95, 15);
            this.label31.TabIndex = 6;
            this.label31.Text = "Наименование";
            // 
            // textBoxIncludeRestDesc
            // 
            this.textBoxIncludeRestDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludeRestDesc.Enabled = false;
            this.textBoxIncludeRestDesc.Location = new System.Drawing.Point(161, 74);
            this.textBoxIncludeRestDesc.MaxLength = 255;
            this.textBoxIncludeRestDesc.Name = "textBoxIncludeRestDesc";
            this.textBoxIncludeRestDesc.Size = new System.Drawing.Size(175, 21);
            this.textBoxIncludeRestDesc.TabIndex = 8;
            this.textBoxIncludeRestDesc.TextChanged += new System.EventHandler(this.textBoxIncludeRestDesc_TextChanged);
            // 
            // dateTimePickerIncludeRestDate
            // 
            this.dateTimePickerIncludeRestDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIncludeRestDate.Enabled = false;
            this.dateTimePickerIncludeRestDate.Location = new System.Drawing.Point(161, 48);
            this.dateTimePickerIncludeRestDate.Name = "dateTimePickerIncludeRestDate";
            this.dateTimePickerIncludeRestDate.Size = new System.Drawing.Size(175, 21);
            this.dateTimePickerIncludeRestDate.TabIndex = 7;
            this.dateTimePickerIncludeRestDate.ValueChanged += new System.EventHandler(this.dateTimePickerIncludeRestDate_ValueChanged);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(8, 52);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(101, 15);
            this.label30.TabIndex = 9;
            this.label30.Text = "Дата реквизита";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(8, 25);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(110, 15);
            this.label29.TabIndex = 10;
            this.label29.Text = "Номер реквизита";
            // 
            // textBoxIncludeRestNum
            // 
            this.textBoxIncludeRestNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludeRestNum.Enabled = false;
            this.textBoxIncludeRestNum.Location = new System.Drawing.Point(161, 22);
            this.textBoxIncludeRestNum.MaxLength = 30;
            this.textBoxIncludeRestNum.Name = "textBoxIncludeRestNum";
            this.textBoxIncludeRestNum.Size = new System.Drawing.Size(175, 21);
            this.textBoxIncludeRestNum.TabIndex = 6;
            this.textBoxIncludeRestNum.TextChanged += new System.EventHandler(this.textBoxIncludeRestNum_TextChanged);
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.checkBoxExcludeRest);
            this.groupBox16.Controls.Add(this.label32);
            this.groupBox16.Controls.Add(this.textBoxExcludeRestDesc);
            this.groupBox16.Controls.Add(this.dateTimePickerExcludeRestDate);
            this.groupBox16.Controls.Add(this.label33);
            this.groupBox16.Controls.Add(this.label34);
            this.groupBox16.Controls.Add(this.textBoxExcludeRestNum);
            this.groupBox16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox16.Location = new System.Drawing.Point(3, 115);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(342, 106);
            this.groupBox16.TabIndex = 1;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "      Реквизиты НПА по исключению из фонда";
            // 
            // checkBoxExcludeRest
            // 
            this.checkBoxExcludeRest.AutoSize = true;
            this.checkBoxExcludeRest.Location = new System.Drawing.Point(11, 0);
            this.checkBoxExcludeRest.Name = "checkBoxExcludeRest";
            this.checkBoxExcludeRest.Size = new System.Drawing.Size(15, 14);
            this.checkBoxExcludeRest.TabIndex = 9;
            this.checkBoxExcludeRest.UseVisualStyleBackColor = true;
            this.checkBoxExcludeRest.CheckedChanged += new System.EventHandler(this.checkBoxExcludeRest_CheckedChanged);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(8, 76);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(95, 15);
            this.label32.TabIndex = 10;
            this.label32.Text = "Наименование";
            // 
            // textBoxExcludeRestDesc
            // 
            this.textBoxExcludeRestDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExcludeRestDesc.Enabled = false;
            this.textBoxExcludeRestDesc.Location = new System.Drawing.Point(161, 73);
            this.textBoxExcludeRestDesc.MaxLength = 255;
            this.textBoxExcludeRestDesc.Name = "textBoxExcludeRestDesc";
            this.textBoxExcludeRestDesc.Size = new System.Drawing.Size(175, 21);
            this.textBoxExcludeRestDesc.TabIndex = 12;
            this.textBoxExcludeRestDesc.TextChanged += new System.EventHandler(this.textBoxExcludeRestDesc_TextChanged);
            // 
            // dateTimePickerExcludeRestDate
            // 
            this.dateTimePickerExcludeRestDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerExcludeRestDate.Enabled = false;
            this.dateTimePickerExcludeRestDate.Location = new System.Drawing.Point(161, 47);
            this.dateTimePickerExcludeRestDate.Name = "dateTimePickerExcludeRestDate";
            this.dateTimePickerExcludeRestDate.Size = new System.Drawing.Size(175, 21);
            this.dateTimePickerExcludeRestDate.TabIndex = 11;
            this.dateTimePickerExcludeRestDate.ValueChanged += new System.EventHandler(this.dateTimePickerExcludeRestDate_ValueChanged);
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(8, 51);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(101, 15);
            this.label33.TabIndex = 13;
            this.label33.Text = "Дата реквизита";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(8, 24);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(110, 15);
            this.label34.TabIndex = 14;
            this.label34.Text = "Номер реквизита";
            // 
            // textBoxExcludeRestNum
            // 
            this.textBoxExcludeRestNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExcludeRestNum.Enabled = false;
            this.textBoxExcludeRestNum.Location = new System.Drawing.Point(161, 21);
            this.textBoxExcludeRestNum.MaxLength = 30;
            this.textBoxExcludeRestNum.Name = "textBoxExcludeRestNum";
            this.textBoxExcludeRestNum.Size = new System.Drawing.Size(175, 21);
            this.textBoxExcludeRestNum.TabIndex = 10;
            this.textBoxExcludeRestNum.TextChanged += new System.EventHandler(this.textBoxExcludeRestNum_TextChanged);
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
            this.id_fund,
            this.protocol_number,
            this.protocol_date,
            this.id_fund_type});
            this.tableLayoutPanel6.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 233);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(702, 100);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // id_fund
            // 
            this.id_fund.HeaderText = "Идентификатор фонда";
            this.id_fund.Name = "id_fund";
            this.id_fund.ReadOnly = true;
            this.id_fund.Visible = false;
            // 
            // protocol_number
            // 
            this.protocol_number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.protocol_number.HeaderText = "Номер протокола";
            this.protocol_number.MinimumWidth = 150;
            this.protocol_number.Name = "protocol_number";
            this.protocol_number.ReadOnly = true;
            this.protocol_number.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.protocol_number.Width = 150;
            // 
            // protocol_date
            // 
            this.protocol_date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.protocol_date.HeaderText = "Дата протокола";
            this.protocol_date.MinimumWidth = 150;
            this.protocol_date.Name = "protocol_date";
            this.protocol_date.ReadOnly = true;
            this.protocol_date.Width = 150;
            // 
            // id_fund_type
            // 
            this.id_fund_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.id_fund_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_fund_type.HeaderText = "Наименование";
            this.id_fund_type.MinimumWidth = 250;
            this.id_fund_type.Name = "id_fund_type";
            this.id_fund_type.ReadOnly = true;
            // 
            // FundsHistoryViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(640, 320);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(714, 342);
            this.Controls.Add(this.tableLayoutPanel6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FundsHistoryViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "История найма";
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

    }
}
