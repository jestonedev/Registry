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

namespace Registry.Viewport
{
    internal sealed class FundsHistoryViewport : Viewport
    {
        #region Components

        private TableLayoutPanel tableLayoutPanel6 = new TableLayoutPanel();
        private TableLayoutPanel tableLayoutPanel7 = new TableLayoutPanel();
        private TableLayoutPanel tableLayoutPanel8 = new TableLayoutPanel();
        private GroupBox groupBox14 = new System.Windows.Forms.GroupBox();
        private GroupBox groupBox15 = new System.Windows.Forms.GroupBox();
        private GroupBox groupBox16 = new System.Windows.Forms.GroupBox();
        private GroupBox groupBox17 = new System.Windows.Forms.GroupBox();
        private Label label29 = new Label();
        private Label label30 = new Label();
        private Label label31 = new Label();
        private Label label32 = new Label();
        private Label label33 = new Label();
        private Label label34 = new Label();
        private Label label35 = new Label();
        private Label label36 = new Label();
        private Label label37 = new Label();
        private DateTimePicker dateTimePickerProtocolDate = new DateTimePicker();
        private TextBox textBoxProtocolNumber = new TextBox();
        private ComboBox comboBoxFundType = new ComboBox();

        private DataGridView dataGridView = new DataGridView();
        private DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
        private DataGridViewTextBoxColumn field_id_fund = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_fund_type = new DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_protocol_number = new DataGridViewTextBoxColumn();
        private DateGridViewDateTimeColumn field_protocol_date = new DateGridViewDateTimeColumn();
        private CheckBox checkBoxIncludeRest = new CheckBox();
        private CheckBox checkBoxExcludeRest = new CheckBox();
        private TextBox textBoxIncludeRestDesc = new TextBox();
        private TextBox textBoxExcludeRestDesc = new TextBox();
        private TextBox textBoxIncludeRestNum = new TextBox();
        private TextBox textBoxExcludeRestNum = new TextBox();
        private DateTimePicker dateTimePickerIncludeRestDate = new DateTimePicker();
        private DateTimePicker dateTimePickerExcludeRestDate = new DateTimePicker();
        private TextBox textBoxDescription = new TextBox();
        #endregion Components

        //Modeles
        FundsHistoryDataModel funds_history = null;
        FundTypesDataModel fund_types = null;
        DataModel fund_assoc = null;

        //Views
        BindingSource v_funds_history = null;
        BindingSource v_fund_types = null;
        BindingSource v_fund_assoc = null;

        private ViewportState viewportState = ViewportState.ReadState;
        private bool is_editable = false;

        public FundsHistoryViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageFundsHistory";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "История найма";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public FundsHistoryViewport(FundsHistoryViewport fundsHistoryViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = fundsHistoryViewport.DynamicFilter;
            this.StaticFilter = fundsHistoryViewport.StaticFilter;
            this.ParentRow = fundsHistoryViewport.ParentRow;
            this.ParentType = fundsHistoryViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
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

            DataSet ds = DataSetManager.GetDataSet();

            v_fund_assoc = new BindingSource();
            if ((ParentType == ParentTypeEnum.SubPremises) && (ParentRow != null))
            {
                v_fund_assoc.DataMember = "funds_sub_premises_assoc";
                v_fund_assoc.Filter = "id_sub_premises = " + ParentRow["id_sub_premises"].ToString();
                this.Text = String.Format("История найма комнаты №{0} помещения №{1}", ParentRow["sub_premises_num"].ToString(), 
                    ParentRow["id_premises"].ToString());
            } else
                if ((ParentType == ParentTypeEnum.Premises) && (ParentRow != null))
                {
                    v_fund_assoc.DataMember = "funds_premises_assoc";
                    v_fund_assoc.Filter = "id_premises = " + ParentRow["id_premises"].ToString();
                    this.Text = String.Format("История найма помещения №{0}", ParentRow["id_premises"].ToString());
                }
                else
                    if ((ParentType == ParentTypeEnum.Building) && (ParentRow != null))
                    {
                        v_fund_assoc.DataMember = "funds_buildings_assoc";
                        v_fund_assoc.Filter = "id_building = " + ParentRow["id_building"].ToString();
                        this.Text = String.Format("История найма здания №{0}", ParentRow["id_building"].ToString());
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
            textBoxProtocolNumber.TextChanged += new EventHandler(textBoxProtocolNumber_TextChanged);
            dateTimePickerProtocolDate.ValueChanged += new EventHandler(dateTimePickerProtocolDate_ValueChanged);
            textBoxDescription.TextChanged += new EventHandler(textBoxDescription_TextChanged);
            textBoxIncludeRestNum.TextChanged += new EventHandler(textBoxIncludeRestNum_TextChanged);
            textBoxIncludeRestDesc.TextChanged += new EventHandler(textBoxIncludeRestDesc_TextChanged);
            dateTimePickerIncludeRestDate.ValueChanged += new EventHandler(dateTimePickerIncludeRestDate_ValueChanged);
            textBoxExcludeRestNum.TextChanged += new EventHandler(textBoxExcludeRestNum_TextChanged);
            textBoxExcludeRestDesc.TextChanged += new EventHandler(textBoxExcludeRestDesc_TextChanged);
            dateTimePickerExcludeRestDate.ValueChanged += new EventHandler(dateTimePickerExcludeRestDate_ValueChanged);
            dataGridView.DataError += new DataGridViewDataErrorEventHandler(dataGridView_DataError);
        }

        void FundsHistoryViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            GroupEnableCheckBoxesUpdate();
        }

        void FundsHistoryViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            GroupEnableCheckBoxesUpdate();            
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RedrawDataGridRows();
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
            textBoxProtocolNumber.Enabled = ((comboBoxFundType.SelectedValue) != null && (Convert.ToInt32(comboBoxFundType.SelectedValue) != 1));
            dateTimePickerProtocolDate.Enabled = ((comboBoxFundType.SelectedValue) != null && (Convert.ToInt32(comboBoxFundType.SelectedValue) != 1));
            CheckViewportModifications();
        }

        void FundAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                RebuildFilter();
                GroupEnableCheckBoxesUpdate();
                RedrawDataGridRows();
            }
        }

        void FundAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
            {
                RebuildFilter();
                GroupEnableCheckBoxesUpdate();
                RedrawDataGridRows();
            }
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
            dateTimePickerProtocolDate.DataBindings.Add("Value", v_funds_history, "protocol_date", true, DataSourceUpdateMode.Never, DateTime.Now);
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
            dateTimePickerIncludeRestDate.DataBindings.Add("Value", v_funds_history, "include_restriction_date", true, DataSourceUpdateMode.Never, DateTime.Now);
            dateTimePickerExcludeRestDate.DataBindings.Clear();
            dateTimePickerExcludeRestDate.DataBindings.Add("Value", v_funds_history, "exclude_restriction_date", true, DataSourceUpdateMode.Never, DateTime.Now);

            dataGridView.DataSource = v_funds_history;
            field_id_fund.DataPropertyName = "id_fund";
            field_id_fund_type.DataPropertyName = "id_fund_type";
            field_id_fund_type.DataSource = v_fund_types;
            field_id_fund_type.ValueMember = "id_fund_type";
            field_id_fund_type.DisplayMember = "fund_type";
            field_protocol_date.DataPropertyName = "protocol_date";
            field_protocol_number.DataPropertyName = "protocol_number";
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
            menuCallback.EditingStateUpdate();
        }

        public override void Close()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            fund_assoc.Select().RowChanged -= new DataRowChangeEventHandler(FundAssoc_RowChanged);
            fund_assoc.Select().RowDeleted -= new DataRowChangeEventHandler(FundAssoc_RowDeleted); 
            funds_history.Select().RowChanged -= new DataRowChangeEventHandler(FundsHistoryViewport_RowChanged);
            funds_history.Select().RowDeleted -= new DataRowChangeEventHandler(FundsHistoryViewport_RowDeleted);
            base.Close();
        }

        public override bool CanSaveRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanCopyRecord()
        {
            return ((v_funds_history.Position != -1) && (!funds_history.EditingNewRecord));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return; 
            FundHistory fundHistory = FundHistoryFromView();
            DataRowView row = (DataRowView)v_funds_history.AddNew();
            dataGridView.Enabled = false;
            funds_history.EditingNewRecord = true;
            ViewportFromFundHistory(fundHistory);
        }

        private void ViewportFromFundHistory(FundHistory fundHistory)
        {
            if (fundHistory.id_fund_type != null)
                comboBoxFundType.SelectedValue = fundHistory.id_fund_type;
            else
                comboBoxFundType.SelectedValue = DBNull.Value;
            textBoxProtocolNumber.Text = fundHistory.protocol_number;
            if (fundHistory.protocol_date != null)
                dateTimePickerProtocolDate.Value = fundHistory.protocol_date.Value;
            else
                dateTimePickerProtocolDate.Value = DateTime.Now;
            textBoxIncludeRestNum.Text = fundHistory.include_restriction_number;
            textBoxIncludeRestDesc.Text = fundHistory.include_restriction_description;
            if (fundHistory.include_restriction_date != null)
                dateTimePickerIncludeRestDate.Value = fundHistory.include_restriction_date.Value;
            else
                dateTimePickerIncludeRestDate.Value = DateTime.Now;
            textBoxExcludeRestNum.Text = fundHistory.exclude_restriction_number;
            textBoxExcludeRestDesc.Text = fundHistory.exclude_restriction_description;
            if (fundHistory.exclude_restriction_date != null)
                dateTimePickerExcludeRestDate.Value = fundHistory.exclude_restriction_date.Value;
            else
                dateTimePickerExcludeRestDate.Value = DateTime.Now;
            textBoxDescription.Text = fundHistory.description;
        }

        private FundHistory FundHistoryFromViewport()
        {
            FundHistory fundHistory = new FundHistory();
            if ((v_funds_history.Position == -1) || ((DataRowView)v_funds_history[v_funds_history.Position])["id_fund"] is DBNull)
                fundHistory.id_fund = null;
            else
                fundHistory.id_fund = Convert.ToInt32(((DataRowView)v_funds_history[v_funds_history.Position])["id_fund"]);
            if (comboBoxFundType.SelectedValue == null)
                fundHistory.id_fund_type = null;
            else
                fundHistory.id_fund_type = Convert.ToInt32(comboBoxFundType.SelectedValue);
            if (fundHistory.id_fund_type == null || fundHistory.id_fund_type == 1)
            {
                fundHistory.protocol_number = null;
                fundHistory.protocol_date = null;
            }
            else
            {
                if (textBoxProtocolNumber.Text.Trim() == "")
                    fundHistory.protocol_number = null;
                else
                    fundHistory.protocol_number = textBoxProtocolNumber.Text.Trim();
                fundHistory.protocol_date = dateTimePickerProtocolDate.Value;
            }
            if (textBoxDescription.Text.Trim() == "")
                fundHistory.description = null;
            else
                fundHistory.description = textBoxDescription.Text.Trim();
            if (checkBoxIncludeRest.Checked)
            {
                if (textBoxIncludeRestNum.Text.Trim() == "")
                    fundHistory.include_restriction_number = null;
                else
                    fundHistory.include_restriction_number = textBoxIncludeRestNum.Text.Trim();
                if (textBoxIncludeRestDesc.Text.Trim() == "")
                    fundHistory.include_restriction_description = null;
                else
                    fundHistory.include_restriction_description = textBoxIncludeRestDesc.Text.Trim();
                fundHistory.include_restriction_date = dateTimePickerIncludeRestDate.Value;
            }
            else
            {
                fundHistory.include_restriction_date = null;
                fundHistory.include_restriction_description = null;
                fundHistory.include_restriction_number = null;
            }
            if (checkBoxExcludeRest.Checked)
            {
                if (textBoxExcludeRestNum.Text.Trim() == "")
                    fundHistory.exclude_restriction_number = null;
                else
                    fundHistory.exclude_restriction_number = textBoxExcludeRestNum.Text.Trim();
                if (textBoxExcludeRestDesc.Text.Trim() == "")
                    fundHistory.exclude_restriction_description = null;
                else
                    fundHistory.exclude_restriction_description = textBoxExcludeRestDesc.Text.Trim();
                fundHistory.exclude_restriction_date = dateTimePickerExcludeRestDate.Value;
            }
            else
            {
                fundHistory.exclude_restriction_date = null;
                fundHistory.exclude_restriction_description = null;
                fundHistory.exclude_restriction_number = null;
            }      
            return fundHistory;
        }

        private FundHistory FundHistoryFromView()
        {
            FundHistory fundHistory = new FundHistory();
            DataRowView row = (DataRowView)v_funds_history[v_funds_history.Position];
            if (row["id_fund"] is DBNull)
                fundHistory.id_fund = null;
            else
                fundHistory.id_fund = Convert.ToInt32(row["id_fund"]);
            if (row["id_fund_type"] is DBNull)
                fundHistory.id_fund_type = null;
            else
                fundHistory.id_fund_type =  Convert.ToInt32(row["id_fund_type"]);
            if (row["protocol_number"] is DBNull)
                fundHistory.protocol_number = null;
            else
                fundHistory.protocol_number = row["protocol_number"].ToString();
            if (row["protocol_date"] is DBNull)
                fundHistory.protocol_date = null;
            else
                fundHistory.protocol_date = Convert.ToDateTime(row["protocol_date"]);
            if (row["include_restriction_number"] is DBNull)
                fundHistory.include_restriction_number = null;
            else
                fundHistory.include_restriction_number = row["include_restriction_number"].ToString();
            if (row["include_restriction_date"] is DBNull)
                fundHistory.include_restriction_date = null;
            else
                fundHistory.include_restriction_date = Convert.ToDateTime(row["include_restriction_date"]);
            if (row["include_restriction_description"] is DBNull)
                fundHistory.include_restriction_description = null;
            else
                fundHistory.include_restriction_description = row["include_restriction_description"].ToString();
            if (row["exclude_restriction_number"] is DBNull)
                fundHistory.exclude_restriction_number = null;
            else
                fundHistory.exclude_restriction_number = row["exclude_restriction_number"].ToString();
            if (row["exclude_restriction_date"] is DBNull)
                fundHistory.exclude_restriction_date = null;
            else
                fundHistory.exclude_restriction_date = Convert.ToDateTime(row["exclude_restriction_date"]);
            if (row["exclude_restriction_description"] is DBNull)
                fundHistory.exclude_restriction_description = null;
            else
                fundHistory.exclude_restriction_description = row["exclude_restriction_description"].ToString();
            if (row["description"] is DBNull)
                fundHistory.description = null;
            else
                fundHistory.description = row["description"].ToString();
            return fundHistory;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_funds_history.MoveFirst();
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_funds_history.MoveLast();
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_funds_history.MoveNext();
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_funds_history.MovePrevious();
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

        public override bool CanInsertRecord()
        {
            if ((viewportState == ViewportState.ReadState || viewportState == ViewportState.ModifyRowState) && !funds_history.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            DataRowView row = (DataRowView)v_funds_history.AddNew();
            dataGridView.Enabled = false;
            funds_history.EditingNewRecord = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (funds_history.Delete((int)((DataRowView)v_funds_history.Current)["id_fund"]) == -1)
                    return;
                ((DataRowView)v_funds_history[v_funds_history.Position]).Delete();
                RedrawDataGridRows();
                menuCallback.ForceCloseDetachedViewports();
                CalcDataModelBuildingsPremisesFunds.GetInstance().Refresh(CalcDataModelFilterEnity.Building, (int)ParentRow["id_building"]);
                if (ParentType == ParentTypeEnum.Building)
                    CalcDataModelBuildingsCurrentFunds.GetInstance().Refresh(CalcDataModelFilterEnity.Building, (int)ParentRow["id_building"]);
                else
                    if (ParentType == ParentTypeEnum.Premises)
                        CalcDataModelPremisesCurrentFunds.GetInstance().Refresh(CalcDataModelFilterEnity.Premise, (int)ParentRow["id_premises"]);
            }
        }

        public override bool CanDeleteRecord()
        {
            if ((v_funds_history.Position == -1) || (viewportState == ViewportState.NewRowState))
                return false;
            else
                return true;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            FundsHistoryViewport viewport = new FundsHistoryViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_funds_history.Count > 0)
                viewport.LocateFundHistoryBy((((DataRowView)v_funds_history[v_funds_history.Position])["id_fund"] as Int32?) ?? -1);
            return viewport;
        }

        private void LocateFundHistoryBy(int id)
        {
            int Position = v_funds_history.Find("id_fund", id);
            if (Position > 0)
                v_funds_history.Position = Position;
        }

        void v_funds_history_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate(); 
            dataGridView.Enabled = true;
            GroupEnableCheckBoxesUpdate();
            if (v_funds_history.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        private void GroupEnableCheckBoxesUpdate()
        {
            checkBoxIncludeRest.Checked = (v_funds_history.Position >= 0) &&
                (((DataRowView)v_funds_history[v_funds_history.Position])["include_restriction_date"] != DBNull.Value);
            checkBoxExcludeRest.Checked = (v_funds_history.Position >= 0) &&
                (((DataRowView)v_funds_history[v_funds_history.Position])["exclude_restriction_date"] != DBNull.Value);
        }

        public override int GetRecordCount()
        {
            return v_funds_history.Count;
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        private bool ValidateFundHistory(FundHistory fundHistory)
        {
            if (checkBoxIncludeRest.Checked && fundHistory.include_restriction_number == null)
            {
                MessageBox.Show("Необходимо задать номер реквизитов НПА по включению в фонд или отключить реквизит", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (checkBoxExcludeRest.Checked && fundHistory.exclude_restriction_number == null)
            {
                MessageBox.Show("Необходимо задать номер реквизитов НПА по исключению из фонда или отключить реквизит", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (textBoxProtocolNumber.Enabled && textBoxProtocolNumber.Text.Trim() == "")
            {
                MessageBox.Show("Необходимо задать номер протокола жилищной комиссии", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (fundHistory.id_fund_type == null)
            {
                MessageBox.Show("Необходимо выбрать тип найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
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
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int id_fund = funds_history.Insert(fundHistory, ParentType, id_parent);
                    if (id_fund == -1)
                        return;
                    DataRowView newRow;
                    if (v_funds_history.Position == -1)
                        newRow = (DataRowView)v_funds_history.AddNew();
                    else
                        newRow = ((DataRowView)v_funds_history[v_funds_history.Position]);
                    fundHistory.id_fund = id_fund;
                    FillRowFromFundHistory(fundHistory, newRow);
                    funds_history.EditingNewRecord = false;
                    is_editable = true;
                    fund_assoc.Select().Rows.Add(new object[] { id_parent, id_fund });
                    RebuildFilter();
                    v_funds_history.Position = v_funds_history.Count - 1;
                    break;
                case ViewportState.ModifyRowState:
                    if (fundHistory.id_fund == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись о принадлежности фонду без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (funds_history.Update(fundHistory) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_funds_history[v_funds_history.Position]);
                    FillRowFromFundHistory(fundHistory, row);
                    break;
            }
            RedrawDataGridRows();
            GroupEnableCheckBoxesUpdate();
            dataGridView.Enabled = true;
            viewportState = ViewportState.ReadState;
            CalcDataModelBuildingsPremisesFunds.GetInstance().Refresh(CalcDataModelFilterEnity.Building, (int)ParentRow["id_building"]);
            if (ParentType == ParentTypeEnum.Building)
                CalcDataModelBuildingsCurrentFunds.GetInstance().Refresh(CalcDataModelFilterEnity.Building, (int)ParentRow["id_building"]);
            else
                if (ParentType == ParentTypeEnum.Premises)
                    CalcDataModelPremisesCurrentFunds.GetInstance().Refresh(CalcDataModelFilterEnity.Premise, (int)ParentRow["id_premises"]);
        }

        private void FillRowFromFundHistory(FundHistory fundHistory, DataRowView row)
        {
            row.BeginEdit();
            row["id_fund"] = fundHistory.id_fund == null ? DBNull.Value : (object)fundHistory.id_fund;
            row["id_fund_type"] = fundHistory.id_fund_type == null ? DBNull.Value : 
                (object)fundHistory.id_fund_type;
            row["protocol_number"] = fundHistory.protocol_number == null ? DBNull.Value : 
                (object)fundHistory.protocol_number;
            row["protocol_date"] = fundHistory.protocol_date == null ? DBNull.Value : 
                (object)fundHistory.protocol_date;
            row["include_restriction_number"] = fundHistory.include_restriction_number == null ? DBNull.Value : 
                (object)fundHistory.include_restriction_number;
            row["include_restriction_date"] = fundHistory.include_restriction_date == null ? DBNull.Value : 
                (object)fundHistory.include_restriction_date;
            row["include_restriction_description"] = 
                fundHistory.include_restriction_description == null ? DBNull.Value : (object)fundHistory.include_restriction_description;
            row["exclude_restriction_number"] = fundHistory.exclude_restriction_number == null ? DBNull.Value : 
                (object)fundHistory.exclude_restriction_number;
            row["exclude_restriction_date"] = fundHistory.exclude_restriction_date == null ? DBNull.Value :
                (object)fundHistory.exclude_restriction_date;
            row["exclude_restriction_description"] =
                fundHistory.exclude_restriction_description == null ? DBNull.Value : 
                (object)fundHistory.exclude_restriction_description;
            row["description"] = fundHistory.description == null ? DBNull.Value : (object)fundHistory.description;
            row.EndEdit();
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    viewportState = ViewportState.ReadState;
                    funds_history.EditingNewRecord = false;
                    if (v_funds_history.Position != -1)
                    {
                        dataGridView.Enabled = true;
                        ((DataRowView)v_funds_history[v_funds_history.Position]).Delete();
                        RedrawDataGridRows();
                    }
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    GroupEnableCheckBoxesUpdate();
                    is_editable = true;
                    viewportState = ViewportState.ReadState;
                    break;
            }
        }

        bool ChangeViewportStateTo(ViewportState state)
        {
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
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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

        public override bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        private void ConstructViewport()
        {
            this.SuspendLayout();
            this.Controls.Add(this.tableLayoutPanel6);

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.groupBox17.SuspendLayout();
            
            this.Controls.Add(this.dataGridView);
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
            this.tableLayoutPanel6.Size = new System.Drawing.Size(984, 531);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel7.Controls.Add(this.groupBox15, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.groupBox16, 0, 1);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 2;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
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
            this.tableLayoutPanel8.Size = new System.Drawing.Size(486, 216);
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
            this.groupBox14.Size = new System.Drawing.Size(480, 102);
            this.groupBox14.TabStop = false;
            this.groupBox14.TabIndex = 0;
            this.groupBox14.Text = "Общие сведения";
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
            this.groupBox15.Size = new System.Drawing.Size(480, 102);
            this.groupBox15.TabStop = false;
            this.groupBox14.TabIndex = 2;
            this.groupBox15.Text = "      Реквизиты НПА по включению в фонд";
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
            this.groupBox16.Location = new System.Drawing.Point(3, 111);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(480, 102);
            this.groupBox16.TabStop = false;
            this.groupBox14.TabIndex = 3;
            this.groupBox16.Text = "      Реквизиты НПА по исключению из фонда";
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.textBoxDescription);
            this.groupBox17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox17.Location = new System.Drawing.Point(3, 111);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(480, 102);
            this.groupBox17.TabStop = false;
            this.groupBox14.TabIndex = 1;
            this.groupBox17.Text = "Дополнительные сведения";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(8, 25);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(97, 13);
            this.label29.Text = "Номер реквизита";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(8, 52);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(89, 13);
            this.label30.Text = "Дата реквизита";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(8, 77);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(83, 13);
            this.label31.Text = "Наименование";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(8, 76);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(83, 13);
            this.label32.Text = "Наименование";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(8, 51);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(89, 13);
            this.label33.Text = "Дата реквизита";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(8, 24);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(97, 13);
            this.label34.Text = "Номер реквизита";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(14, 27);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(61, 13);
            this.label35.Text = "Тип найма";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(14, 54);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(118, 13);
            this.label36.Text = "Номер протокола ЖК";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(14, 80);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(110, 13);
            this.label37.Text = "Дата протокола ЖК";
            // 
            // dateTimePickerProtocolDate
            // 
            this.dateTimePickerProtocolDate.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerProtocolDate.Location = new System.Drawing.Point(161, 77);
            this.dateTimePickerProtocolDate.Name = "dateTimePickerProtocolDate";
            this.dateTimePickerProtocolDate.Size = new System.Drawing.Size(313, 20);
            this.dateTimePickerProtocolDate.TabIndex = 2;
            this.dateTimePickerProtocolDate.Enabled = false;
            // 
            // textBoxProtocolNumber
            // 
            this.textBoxProtocolNumber.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProtocolNumber.Location = new System.Drawing.Point(161, 51);
            this.textBoxProtocolNumber.Name = "textBoxProtocolNumber";
            this.textBoxProtocolNumber.Size = new System.Drawing.Size(313, 20);
            this.textBoxProtocolNumber.TabIndex = 1;
            this.textBoxProtocolNumber.Enabled = false;
            this.textBoxProtocolNumber.MaxLength = 50;
            // 
            // comboBoxFundType
            // 
            this.comboBoxFundType.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxFundType.FormattingEnabled = true;
            this.comboBoxFundType.Location = new System.Drawing.Point(161, 24);
            this.comboBoxFundType.Name = "comboBoxFundType";
            this.comboBoxFundType.Size = new System.Drawing.Size(313, 21);
            this.comboBoxFundType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxFundType.TabIndex = 0;
            // 
            // checkBoxIncludeRest
            // 
            this.checkBoxIncludeRest.AutoSize = true;
            this.checkBoxIncludeRest.Location = new System.Drawing.Point(11, 0);
            this.checkBoxIncludeRest.Name = "checkBoxIncludeRest";
            this.checkBoxIncludeRest.Size = new System.Drawing.Size(15, 14);
            this.checkBoxIncludeRest.TabIndex = 5;
            this.checkBoxIncludeRest.UseVisualStyleBackColor = true;
            this.checkBoxIncludeRest.CheckedChanged += new EventHandler(checkBoxIncludeRest_CheckedChanged);
            // 
            // checkBoxExcludeRest
            // 
            this.checkBoxExcludeRest.AutoSize = true;
            this.checkBoxExcludeRest.Location = new System.Drawing.Point(11, 0);
            this.checkBoxExcludeRest.Name = "checkBoxExcludeRest";
            this.checkBoxExcludeRest.Size = new System.Drawing.Size(15, 14);
            this.checkBoxExcludeRest.TabIndex = 9;
            this.checkBoxExcludeRest.UseVisualStyleBackColor = true;
            this.checkBoxExcludeRest.CheckedChanged += new EventHandler(checkBoxExcludeRest_CheckedChanged);
            // 
            // textBoxIncludeRestNum
            // 
            this.textBoxIncludeRestNum.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludeRestNum.Location = new System.Drawing.Point(155, 22);
            this.textBoxIncludeRestNum.Name = "textBoxIncludeRestNum";
            this.textBoxIncludeRestNum.Size = new System.Drawing.Size(319, 20);
            this.textBoxIncludeRestNum.TabIndex = 6;
            this.textBoxIncludeRestNum.Enabled = false;
            this.textBoxIncludeRestNum.MaxLength = 30;
            // 
            // textBoxExcludeRestNum
            // 
            this.textBoxExcludeRestNum.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExcludeRestNum.Location = new System.Drawing.Point(155, 21);
            this.textBoxExcludeRestNum.Name = "textBoxExcludeRestNum";
            this.textBoxExcludeRestNum.Size = new System.Drawing.Size(319, 20);
            this.textBoxExcludeRestNum.TabIndex = 10;
            this.textBoxExcludeRestNum.Enabled = false;
            this.textBoxExcludeRestNum.MaxLength = 30;
            // 
            // textBoxIncludeRestDesc
            // 
            this.textBoxIncludeRestDesc.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludeRestDesc.Location = new System.Drawing.Point(155, 74);
            this.textBoxIncludeRestDesc.Name = "textBoxIncludeRestDesc";
            this.textBoxIncludeRestDesc.Size = new System.Drawing.Size(319, 20);
            this.textBoxIncludeRestDesc.TabIndex = 8;
            this.textBoxIncludeRestDesc.Enabled = false;
            this.textBoxIncludeRestDesc.MaxLength = 255;
            // 
            // textBoxExcludeRestDesc
            // 
            this.textBoxExcludeRestDesc.Anchor =
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExcludeRestDesc.Location = new System.Drawing.Point(155, 73);
            this.textBoxExcludeRestDesc.Name = "textBoxExcludeRestDesc";
            this.textBoxExcludeRestDesc.Size = new System.Drawing.Size(319, 20);
            this.textBoxExcludeRestDesc.TabIndex = 12;
            this.textBoxExcludeRestDesc.Enabled = false;
            this.textBoxExcludeRestDesc.MaxLength = 255;
            // 
            // dateTimePickerIncludeRestDate
            // 
            this.dateTimePickerIncludeRestDate.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIncludeRestDate.Location = new System.Drawing.Point(155, 48);
            this.dateTimePickerIncludeRestDate.Name = "dateTimePickerIncludeRestDate";
            this.dateTimePickerIncludeRestDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerIncludeRestDate.TabIndex = 7;
            this.dateTimePickerIncludeRestDate.Enabled = false;
            // 
            // dateTimePickerExcludeRestDate
            // 
            this.dateTimePickerExcludeRestDate.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerExcludeRestDate.Location = new System.Drawing.Point(155, 47);
            this.dateTimePickerExcludeRestDate.Name = "dateTimePickerExcludeRestDate";
            this.dateTimePickerExcludeRestDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerExcludeRestDate.TabIndex = 11;
            this.dateTimePickerExcludeRestDate.Enabled = false;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom)));
            this.textBoxDescription.Location = new System.Drawing.Point(7, 19);
            this.textBoxDescription.MaxLength = 255;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBox2";
            this.textBoxDescription.Size = new System.Drawing.Size(468, 74);
            this.textBoxDescription.TabIndex = 4;
            this.textBoxDescription.MaxLength = 255;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F,
                System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_fund,
            this.field_protocol_number,
            this.field_protocol_date,
            this.field_id_fund_type});
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.TabIndex = 13;
            this.tableLayoutPanel6.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.ScrollBars = ScrollBars.Both;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.AllowUserToResizeRows = false;
            // 
            // field_id_fund
            // 
            this.field_id_fund.HeaderText = "Идентификатор фонда";
            this.field_id_fund.Name = "id_fund";
            this.field_id_fund.Visible = false;
            this.field_id_fund.ReadOnly = true;
            // 
            // field_protocol_number
            // 
            this.field_protocol_number.HeaderText = "Номер протокола";
            this.field_protocol_number.Name = "protocol_num";
            this.field_protocol_number.ReadOnly = true;
            this.field_protocol_number.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // field_protocol_date
            // 
            this.field_protocol_date.HeaderText = "Дата протокола";
            this.field_protocol_date.Name = "protocol_date";
            this.field_protocol_date.ReadOnly = true;
            // 
            // field_id_fund_type
            // 
            this.field_id_fund_type.HeaderText = "Наименование";
            this.field_id_fund_type.Name = "id_fund_type";
            this.field_id_fund_type.ReadOnly = true;
            this.field_id_fund_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupBox17.ResumeLayout(false);
            this.groupBox16.ResumeLayout(false);
            this.groupBox15.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.ResumeLayout(false);
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
    }
}
