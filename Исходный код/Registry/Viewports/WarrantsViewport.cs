using System;
using System.Data;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class WarrantsViewport : FormWithGridViewport
    {

        #region Models
        private DataModel _warrantDocTypes;
        #endregion Models

        #region Views

        private BindingSource _vWarrantDocTypes;
        #endregion Views

        private WarrantsViewport()
            : this(null, null)
        {
        }

        public WarrantsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            DataGridView = dataGridView;
        }

        private void DataBind()
        {
            comboBoxWarrantDocType.DataSource = _vWarrantDocTypes;
            comboBoxWarrantDocType.ValueMember = "id_warrant_doc_type";
            comboBoxWarrantDocType.DisplayMember = "warrant_doc_type";
            comboBoxWarrantDocType.DataBindings.Clear();
            comboBoxWarrantDocType.DataBindings.Add("SelectedValue", GeneralBindingSource, "id_warrant_doc_type", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxWarrantRegNum.DataBindings.Clear();
            textBoxWarrantRegNum.DataBindings.Add("Text", GeneralBindingSource, "registration_num", true, DataSourceUpdateMode.Never, "");
            textBoxWarrantNotary.DataBindings.Clear();
            textBoxWarrantNotary.DataBindings.Add("Text", GeneralBindingSource, "notary", true, DataSourceUpdateMode.Never, "");
            textBoxWarrantOnBehalfOf.DataBindings.Clear();
            textBoxWarrantOnBehalfOf.DataBindings.Add("Text", GeneralBindingSource, "on_behalf_of", true, DataSourceUpdateMode.Never, "");
            textBoxWarrantDistrict.DataBindings.Clear();
            textBoxWarrantDistrict.DataBindings.Add("Text", GeneralBindingSource, "notary_district", true, DataSourceUpdateMode.Never, "");
            textBoxWarrantDescription.DataBindings.Clear();
            textBoxWarrantDescription.DataBindings.Add("Text", GeneralBindingSource, "description", true, DataSourceUpdateMode.Never, "");
            dateTimePickerWarrantDate.DataBindings.Clear();
            dateTimePickerWarrantDate.DataBindings.Add("Value", GeneralBindingSource, "registration_date", true, DataSourceUpdateMode.Never, DateTime.Now);

            dataGridView.DataSource = GeneralBindingSource;
            id_warrant.DataPropertyName = "id_warrant";
            notary.DataPropertyName = "notary";
            on_behalf_of.DataPropertyName = "on_behalf_of";
            registration_num.DataPropertyName = "registration_num";
            registration_date.DataPropertyName = "registration_date";
            description.DataPropertyName = "description";
        }

        protected override bool ChangeViewportStateTo(ViewportState state)
        {
            if (AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite))
                return base.ChangeViewportStateTo(state);
            ViewportState = ViewportState.ReadState;
            return true;
        }

        private void ViewportFromWarrant(Warrant warrant)
        {
            comboBoxWarrantDocType.SelectedValue = ViewportHelper.ValueOrDbNull(warrant.IdWarrantDocType);
            dateTimePickerWarrantDate.Value = ViewportHelper.ValueOrDefault(warrant.RegistrationDate);
            textBoxWarrantDescription.Text = warrant.Description;
            textBoxWarrantRegNum.Text = warrant.RegistrationNum;
            textBoxWarrantNotary.Text = warrant.Notary;
            textBoxWarrantDistrict.Text = warrant.NotaryDistrict;
            textBoxWarrantOnBehalfOf.Text = warrant.OnBehalfOf;
        }

        protected override Entity EntityFromViewport()
        {
            var warrant = new Warrant();
            if (GeneralBindingSource.Position == -1)
                warrant.IdWarrant = null;
            else
                warrant.IdWarrant = ViewportHelper.ValueOrNull<int>((DataRowView)GeneralBindingSource[GeneralBindingSource.Position],"id_warrant");
            warrant.IdWarrantDocType = ViewportHelper.ValueOrNull<int>(comboBoxWarrantDocType);
            warrant.RegistrationNum = ViewportHelper.ValueOrNull(textBoxWarrantRegNum);
            warrant.OnBehalfOf = ViewportHelper.ValueOrNull(textBoxWarrantOnBehalfOf);
            warrant.Notary = ViewportHelper.ValueOrNull(textBoxWarrantNotary);
            warrant.NotaryDistrict = ViewportHelper.ValueOrNull(textBoxWarrantDistrict);
            warrant.Description = ViewportHelper.ValueOrNull(textBoxWarrantDescription);
            warrant.RegistrationDate = dateTimePickerWarrantDate.Value;
            return warrant;
        }

        protected override Entity EntityFromView()
        {
            var row = (DataRowView)GeneralBindingSource[GeneralBindingSource.Position];
            return WarrantConverter.FromRow(row);
        }

        private bool ValidateWarrant(Warrant warrant)
        {
            if (warrant.IdWarrantDocType == null)
            {
                MessageBox.Show(@"Необходимо выбрать тип документа", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxWarrantDocType.Focus();
                return false;
            }
            if (warrant.RegistrationNum == null)
            {
                MessageBox.Show(@"Регистрационный номер не может быть пустым", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxWarrantRegNum.Focus();
                return false;
            }
            return true;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = EntityDataModel<Warrant>.GetInstance();
            _warrantDocTypes = DataModel.GetInstance<WarrantDocTypesDataModel>();

            // Ожидаем дозагрузки, если это необходимо
            GeneralDataModel.Select();
            _warrantDocTypes.Select();

            var ds = DataModel.DataSet;

            _vWarrantDocTypes = new BindingSource
            {
                DataMember = "warrant_doc_types",
                DataSource = ds
            };

            GeneralBindingSource = new BindingSource();
            AddEventHandler<EventArgs>(GeneralBindingSource, "CurrentItemChanged", v_warrants_CurrentItemChanged);
            GeneralBindingSource.DataMember = "warrants";
            GeneralBindingSource.DataSource = ds;
            GeneralBindingSource.Sort = "registration_date DESC";
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", WarrantsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", WarrantsViewport_RowDeleted);

            DataBind();
            IsEditable = true;

            DataChangeHandlersInit();
        }

        public override bool CanInsertRecord()
        {
            return (!GeneralDataModel.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            IsEditable = true;
            GeneralDataModel.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (GeneralBindingSource.Position != -1) && (!GeneralDataModel.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            var warrant = (Warrant) EntityFromView();
            GeneralBindingSource.AddNew();
            dataGridView.Enabled = false;
            GeneralDataModel.EditingNewRecord = true;
            ViewportFromWarrant(warrant);
            IsEditable = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (GeneralDataModel.Delete((int)((DataRowView)GeneralBindingSource.Current)["id_warrant"]) == -1)
                    return;
                IsEditable = false;
                ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                IsEditable = true;
                ViewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralBindingSource.Position > -1)
                && (ViewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override bool CanCancelRecord()
        {
            return (ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState);
        }

        public override bool CanSaveRecord()
        {
            return ((ViewportState == ViewportState.NewRowState) || (ViewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            var warrant = (Warrant) EntityFromViewport();
            if (!ValidateWarrant(warrant))
                return;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var id_warrant = GeneralDataModel.Insert(warrant);
                    if (id_warrant == -1)
                    {
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    DataRowView newRow;
                    warrant.IdWarrant = id_warrant;
                    IsEditable = false;
                    if (GeneralBindingSource.Position == -1)
                        newRow = (DataRowView)GeneralBindingSource.AddNew();
                    else
                        newRow = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    WarrantConverter.FillRow(warrant, newRow);
                    IsEditable = true;
                    GeneralDataModel.EditingNewRecord = false;
                    break;
                case ViewportState.ModifyRowState:
                    if (warrant.IdWarrant == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о доверенности без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (GeneralDataModel.Update(warrant) == -1)
                        return;
                    var row = ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]);
                    IsEditable = false;
                    WarrantConverter.FillRow(warrant, row);
                    break;
            }
            dataGridView.Enabled = true;
            IsEditable = true;
            ViewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
        }

        public override void CancelRecord()
        {
            switch (ViewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    GeneralDataModel.EditingNewRecord = false;
                    if (GeneralBindingSource.Position != -1)
                    {
                        IsEditable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)GeneralBindingSource[GeneralBindingSource.Position]).Delete();
                    }
                    ViewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    IsEditable = false;
                    DataBind();
                    ViewportState = ViewportState.ReadState;
                    break;
            }
            IsEditable = true;
            MenuCallback.EditingStateUpdate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            textBoxWarrantRegNum.Focus();
            base.OnVisibleChanged(e);
        }

        private void WarrantsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void WarrantsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void v_warrants_CurrentItemChanged(object sender, EventArgs e)
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
            if (GeneralBindingSource.Position == -1)
                return;
            if (ViewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            ViewportState = ViewportState.ReadState;
            IsEditable = true;
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
