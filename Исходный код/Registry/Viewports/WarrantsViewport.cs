using System;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.Presenters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class WarrantsViewport : FormWithGridViewport
    {

        private WarrantsViewport()
            : this(null, null)
        {
        }

        public WarrantsViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback, new WarrantsPresenter())
        {
            InitializeComponent();
            DataGridView = dataGridView;
            DataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
        }

        private void DataBind()
        {
            var bindingSource = Presenter.ViewModel["general"].BindingSource;
            ViewportHelper.BindSource(comboBoxWarrantDocType, Presenter.ViewModel["warrant_doc_types"].BindingSource, "warrant_doc_type",
                 Presenter.ViewModel["warrant_doc_types"].PrimaryKeyFirst);
            ViewportHelper.BindProperty(comboBoxWarrantDocType, "SelectedValue", bindingSource,
                Presenter.ViewModel["warrant_doc_types"].PrimaryKeyFirst, DBNull.Value);

            ViewportHelper.BindProperty(textBoxWarrantRegNum, "Text", bindingSource, "registration_num", "");
            ViewportHelper.BindProperty(textBoxWarrantNotary, "Text", bindingSource, "notary", "");
            ViewportHelper.BindProperty(textBoxWarrantOnBehalfOf, "Text", bindingSource, "on_behalf_of", "");
            ViewportHelper.BindProperty(textBoxWarrantDistrict, "Text", bindingSource, "notary_district", "");
            ViewportHelper.BindProperty(textBoxWarrantDescription, "Text", bindingSource, "description", "");
            ViewportHelper.BindProperty(dateTimePickerWarrantDate, "Value", bindingSource, "registration_date", DateTime.Now.Date);
            
            DataGridView.DataSource = bindingSource;
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
            var row = Presenter.ViewModel["general"].CurrentRow;
            var warrant = new Warrant
            {
                IdWarrant = row == null ? null : ViewportHelper.ValueOrNull<int>(row, "id_warrant"),
                IdWarrantDocType = ViewportHelper.ValueOrNull<int>(comboBoxWarrantDocType),
                RegistrationNum = ViewportHelper.ValueOrNull(textBoxWarrantRegNum),
                OnBehalfOf = ViewportHelper.ValueOrNull(textBoxWarrantOnBehalfOf),
                Notary = ViewportHelper.ValueOrNull(textBoxWarrantNotary),
                NotaryDistrict = ViewportHelper.ValueOrNull(textBoxWarrantDistrict),
                Description = ViewportHelper.ValueOrNull(textBoxWarrantDescription),
                RegistrationDate = ViewportHelper.ValueOrNull(dateTimePickerWarrantDate)
            };
            return warrant;
        }

        protected override Entity EntityFromView()
        {
            var row = Presenter.ViewModel["general"].CurrentRow;
            return row == null ? new Warrant() : EntityConverter<Warrant>.FromRow(row);
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
            GeneralDataModel = Presenter.ViewModel["general"].Model;
            GeneralBindingSource = Presenter.ViewModel["general"].BindingSource;

            DataBind();

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].BindingSource, "CurrentItemChanged", GeneralBindingSource_CurrentItemChanged);

            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowChanged", WarrantsViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(Presenter.ViewModel["general"].DataSource, "RowDeleted", WarrantsViewport_RowDeleted);

            AddEventHandler<EventArgs>(Presenter.ViewModel["general"].DataSource.Constraints, 
                "CollectionChanged", (s,e) => DataBind());

            DataChangeHandlersInit();

            GeneralBindingSource_CurrentItemChanged(null, new EventArgs());

            IsEditable = true;
        }

        public override bool CanInsertRecord()
        {
            return !Presenter.ViewModel["general"].Model.EditingNewRecord && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            GeneralDataModel.EditingNewRecord = true;
            GeneralBindingSource.AddNew();
            DataGridView.Enabled = false;
            IsEditable = true;
        }

        public override bool CanCopyRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null) && !Presenter.ViewModel["general"].Model.EditingNewRecord
                && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            IsEditable = false;
            var warrant = (Warrant)EntityFromView();
            Presenter.ViewModel["general"].Model.EditingNewRecord = true;
            Presenter.ViewModel["general"].BindingSource.AddNew();
            ViewportFromWarrant(warrant);
            DataGridView.Enabled = false;
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
                if (!((WarrantsPresenter)Presenter).DeleteRecord())
                {
                    IsEditable = true;
                    return;
                }
                IsEditable = true;
                ViewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanDeleteRecord()
        {
            return (Presenter.ViewModel["general"].CurrentRow != null)
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
            var warrant = (Warrant)EntityFromViewport();
            if (!ValidateWarrant(warrant))
                return;
            IsEditable = false;
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    if (!((WarrantsPresenter)Presenter).InsertRecord(warrant))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
                case ViewportState.ModifyRowState:
                    if (!((WarrantsPresenter)Presenter).UpdateRecord(warrant))
                    {
                        IsEditable = true;
                        return;
                    }
                    break;
            }
            DataGridView.Enabled = true;
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
                    Presenter.ViewModel["general"].Model.EditingNewRecord = false;
                    var row = Presenter.ViewModel["general"].CurrentRow;
                    if (row != null)
                    {
                        IsEditable = false;
                        row.Delete();
                    }
                    break;
                case ViewportState.ModifyRowState:
                    IsEditable = false;
                    DataBind();
                    break;
            }
            ViewportState = ViewportState.ReadState;
            IsEditable = true;
            DataGridView.Enabled = true;
            MenuCallback.EditingStateUpdate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            textBoxWarrantRegNum.Focus();
            base.OnVisibleChanged(e);
        }

        private void WarrantsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void WarrantsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
            CheckViewportModifications();
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
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void selectAll_Enter(object sender, EventArgs e)
        {
            ViewportHelper.SelectAllText(sender);
        }
    }
}
