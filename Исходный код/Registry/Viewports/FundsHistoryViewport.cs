using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.Presenters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class FundsHistoryViewport : FormWithGridViewport
    {

        private FundsHistoryViewport()
            : this(null, null)
        {
        }

        public FundsHistoryViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new FundsHistoryPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        private void RedrawDataGridRows()
        {
            if (DataGridView.Rows.Count == 0)
                return;
            var currentFundFounded = false;
            for (var i = DataGridView.Rows.Count - 1; i >= 0; i--)
            {
                if (Presenter.ViewModel["general"].BindingSource.Count - 1 >= i &&
                    (((DataRowView)Presenter.ViewModel["general"].BindingSource[i])["exclude_restriction_date"] == DBNull.Value) &&
                    !currentFundFounded)
                {
                    DataGridView.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    currentFundFounded = true;
                }
                else
                    DataGridView.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindSource(comboBoxFundType, Presenter.ViewModel["fund_types"].BindingSource, "fund_type",
                 Presenter.ViewModel["fund_types"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxFundType, "SelectedValue", bindingSource,
                Presenter.ViewModel["fund_types"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindProperty(textBoxProtocolNumber, "Text", bindingSource, "protocol_number", "");
            ViewportHelper.BindProperty(dateTimePickerProtocolDate, "Value", bindingSource, "protocol_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(textBoxDescription, "Text", bindingSource, "description", "");
            ViewportHelper.BindProperty(textBoxIncludeRestNum, "Text", bindingSource, "include_restriction_number", "");
            ViewportHelper.BindProperty(textBoxIncludeRestDesc, "Text", bindingSource, "include_restriction_description", "");
            ViewportHelper.BindProperty(textBoxExcludeRestNum, "Text", bindingSource, "exclude_restriction_number", "");
            ViewportHelper.BindProperty(textBoxExcludeRestDesc, "Text", bindingSource, "exclude_restriction_description", "");
            ViewportHelper.BindProperty(dateTimePickerIncludeRestDate, "Value", bindingSource, "include_restriction_date", DateTime.Now.Date);
            ViewportHelper.BindProperty(dateTimePickerExcludeRestDate, "Value", bindingSource, "exclude_restriction_date", DateTime.Now.Date);

            DataGridView.DataSource = bindingSource;
            id_fund.DataPropertyName = "id_fund";
            protocol_date.DataPropertyName = "protocol_date";
            protocol_number.DataPropertyName = "protocol_number";
            ViewportHelper.BindSource(id_fund_type, Presenter.ViewModel["fund_types"].BindingSource, "fund_type",
                Presenter.ViewModel["fund_types"].PrimaryKeyFirst);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            IsEditable = false;
            checkBoxIncludeRest.Checked = row != null &&
                                          (row["include_restriction_date"] != DBNull.Value) &&
                                          (row["include_restriction_number"] != DBNull.Value);
            checkBoxExcludeRest.Checked = row != null &&
                                          (row["exclude_restriction_date"] != DBNull.Value) &&
                                          (row["exclude_restriction_number"] != DBNull.Value);
            if (row != null && row["protocol_date"] != DBNull.Value)
            {
                dateTimePickerProtocolDate.Checked = true;   
            }
            else
            {
                dateTimePickerProtocolDate.Value = DateTime.Now.Date;
                dateTimePickerProtocolDate.Checked = false;
            }
            IsEditable = true;
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) ||
                AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private bool ValidateFundHistory(FundHistory fundHistory)
        {
            if (((FundsHistoryPresenter)Presenter).ValidatePermissions() == false)
                return false;
            if (checkBoxIncludeRest.Checked && fundHistory.IncludeRestrictionNumber == null)
            {
                MessageBox.Show(@"Необходимо задать номер реквизитов НПА по включению в фонд или отключить реквизит", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxIncludeRestNum.Focus();
                return false;
            }
            if (checkBoxExcludeRest.Checked && fundHistory.ExcludeRestrictionNumber == null)
            {
                MessageBox.Show(@"Необходимо задать номер реквизитов НПА по исключению из фонда или отключить реквизит", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxExcludeRestNum.Focus();
                return false;
            }
            if (fundHistory.IdFundType == null)
            {
                MessageBox.Show(@"Необходимо выбрать тип найма", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxFundType.Focus();
                return false;
            }
            return true;
        }

        protected override Entity EntityFromViewport()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            var fundHistory = new FundHistory
            {
                IdFund = row == null ? null : ViewportHelper.ValueOrNull<int>(row, "id_fund"),
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
            return EntityConverter<FundHistory>.FromRow(Presenter.ViewModel["general"].CurrentRow);
        }

        private void ViewportFromFundHistory(FundHistory fundHistory)
        {
            comboBoxFundType.SelectedValue = ViewportHelper.ValueOrDbNull(fundHistory.IdFundType);
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
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;
            Presenter.ParentRow = ParentRow;
            Presenter.ParentType = ParentType;

            ((FundsHistoryPresenter)Presenter).AddAssocViewModelItem();

            if (ParentRow == null)
            {
                throw new ViewportException("Не указан родительский объект");
            }

            switch (ParentType)
            {
                case ParentTypeEnum.SubPremises:
                    Text = string.Format(CultureInfo.InvariantCulture, "История фонда комнаты №{0} помещения №{1}", 
                        ParentRow["sub_premises_num"], ParentRow["id_premises"]);
                    break;
                case ParentTypeEnum.Premises:
                    Text = string.Format(CultureInfo.InvariantCulture, "История фонда помещения №{0}", ParentRow["id_premises"]);
                    break;
                case ParentTypeEnum.Building:
                    Text = string.Format(CultureInfo.InvariantCulture, "История фонда здания №{0}", ParentRow["id_building"]);
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }

            //Перестраиваем фильтр GeneralBindingSource.Filter
            ((FundsHistoryPresenter)Presenter).RebuildFilter();

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", FundsHistoryViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", FundsHistoryViewport_RowDeleted);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowChanged", FundAssoc_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["assoc"].DataSource, "RowDeleted", FundAssoc_RowDeleted);
            AddEventHandler<EventArgs>(comboBoxFundType, "SelectedIndexChanged", comboBoxFundType_SelectedIndexChanged);

            DataBind();

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());

            DataChangeHandlersInit();

            IsEditable = true;
            if (Presenter.ViewModel["general"].BindingSource.Count == 0)
                InsertRecord();
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState)) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void SaveRecord()
        {
            var fundHistory = (FundHistory) EntityFromViewport();
            if (!ValidateFundHistory(fundHistory))
                return;
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    if (!((FundsHistoryPresenter) Presenter).InsertRecord(fundHistory))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((FundsHistoryPresenter) Presenter).UpdateRecord(fundHistory))
                    {
                        IsEditable = true; 
                        return;
                    }
                    break;
            }
            RedrawDataGridRows();
            UnbindedCheckBoxesUpdate();
            DataGridView.Enabled = true;
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanCopyRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) && !Presenter.ViewModel["general"].Model.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            var fundHistory = (FundHistory)EntityFromView();
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            ViewportFromFundHistory(fundHistory);
            DataGridView.Enabled = false;
            checkBoxIncludeRest.Checked = fundHistory.IncludeRestrictionDate != null;
            checkBoxExcludeRest.Checked = fundHistory.ExcludeRestrictionDate != null;
            comboBoxFundType.Focus();
            IsEditable = true;
        }

        public override bool CanInsertRecord()
        {
            return !Presenter.ViewModel["general"].Model.EditingNewRecord &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            comboBoxFundType.Focus();
            IsEditable = false;
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            DataGridView.Enabled = false;
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null)
                && (ViewportState != ViewportState.NewRowState) &&
                (AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal) || AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal));
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;
            if (((FundsHistoryPresenter)Presenter).ValidatePermissions() == false)
                return;
            IsEditable = false;
            if (!((FundsHistoryPresenter) Presenter).DeleteRecord())
            {
                IsEditable = true;
                return;
            }
            IsEditable = true;
            RedrawDataGridRows();
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            MenuCallback.ForceCloseDetachedViewports();
        }

        public override bool CanCancelRecord()
        {
            return (ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState);
        }

        public override void CancelRecord()
        {
            switch (ViewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    Presenter.ViewModel["general"].Model.EditingNewRecord = false;
                    var row = Presenter.ViewModel["general"].CurrentRow;
                    if (row != null)
                    {
                        IsEditable = false;
                        row.Delete();
                        RedrawDataGridRows();
                        if (Presenter.ViewModel["general"].CurrentRow != null)
                        {
                            DataGridView.Rows[Presenter.ViewModel["general"].BindingSource.Position].Selected = true;
                        }
                    }
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            IsEditable = true;
            DataGridView.Enabled = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
        }

        protected override void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            if (Presenter.ViewModel["general"].CurrentRow == null || DataGridView.RowCount == 0)
                DataGridView.ClearSelection();
            else
                if (bindingSource.Position >= DataGridView.RowCount)
                    DataGridView.Rows[DataGridView.RowCount - 1].Selected = true;
                else if (DataGridView.Rows[bindingSource.Position].Selected != true)
                    DataGridView.Rows[bindingSource.Position].Selected = true;

            var isEditable = IsEditable;
            UnbindedCheckBoxesUpdate();
            IsEditable = isEditable;

            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void FundsHistoryViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
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
            int selectedValue;
            if (comboBoxFundType.SelectedValue != null && int.TryParse(comboBoxFundType.SelectedValue.ToString(), out selectedValue))
            {
                textBoxProtocolNumber.Enabled = dateTimePickerProtocolDate.Enabled = selectedValue != 1;
            }
            else
            {
                textBoxProtocolNumber.Enabled = dateTimePickerProtocolDate.Enabled = false;
            }
        }

        private void FundAssoc_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            ((FundsHistoryPresenter)Presenter).RebuildFilter();
            UnbindedCheckBoxesUpdate();
            RedrawDataGridRows();
        }

        private void FundAssoc_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            ((FundsHistoryPresenter)Presenter).RebuildFilter();
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
