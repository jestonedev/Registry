using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.DataModels;
using System.Collections;

namespace Registry.Viewport
{
    internal class PremisesListViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new DataGridView();
        private DataGridViewTextBoxColumn field_id_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_street = new System.Windows.Forms.DataGridViewComboBoxColumn();
        private DataGridViewComboBoxColumn field_house = new System.Windows.Forms.DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_premises_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_cadastral_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
        #endregion Components

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        //Models
        private PremisesDataModel premises = null;
        private BuildingsDataModel buildings = null;
        private KladrDataModel kladr = null;
        private PremisesTypesDataModel premises_types = null;

        //Views
        private BindingSource v_premises = null;
        private BindingSource v_buildings = null;
        private BindingSource v_premises_types = null;

        public PremisesListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPagePremises";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень помещений";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public PremisesListViewport(PremisesListViewport premisesListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = premisesListViewport.DynamicFilter;
            this.StaticFilter = premisesListViewport.StaticFilter;
            this.ParentRow = premisesListViewport.ParentRow;
            this.ParentType = premisesListViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            premises = PremisesDataModel.GetInstance();
            kladr = KladrDataModel.GetInstance();
            buildings  = BuildingsDataModel.GetInstance();
            premises_types = PremisesTypesDataModel.GetInstance();

            DataSet ds = DataSetManager.GetDataSet();

            v_premises = new BindingSource();
            v_premises.CurrentItemChanged += new EventHandler(v_premises_CurrentItemChanged);
            v_premises.DataMember = "premises";
            v_premises.DataSource = ds;
            v_premises.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_premises.Filter += " AND ";
            v_premises.Filter += DynamicFilter;
            if (ParentRow != null)
            {
                if (ParentType == ParentTypeEnum.Building)
                    Text = "Помещения здания №" + ParentRow["id_building"].ToString();
            }

            v_buildings = new BindingSource();
            v_buildings.DataMember = "buildings";
            v_buildings.DataSource = ds;

            v_premises_types = new BindingSource();
            v_premises_types.DataMember = "premises_types";
            v_premises_types.DataSource = ds;

            dataGridView.DataSource = v_premises;

            field_id_premises.DataPropertyName = "id_premises";
            field_premises_num.DataPropertyName = "premises_num";

            field_street.DataPropertyName = "id_building";
            field_street.DataSource = new StreetByBuildingDataSource(buildings, kladr);
            field_street.ValueMember = "id_building";
            field_street.DisplayMember = "street_name";

            field_house.DataSource = v_buildings;
            field_house.DataPropertyName = "id_building";
            field_house.ValueMember = "id_building";
            field_house.DisplayMember = "house";
            field_total_area.DataPropertyName = "total_area";
            field_living_area.DataPropertyName = "living_area";
            field_cadastral_num.DataPropertyName = "cadastral_num";
            field_premises_type.DataSource = v_premises_types;
            field_premises_type.DataPropertyName = "id_premises_type";
            field_premises_type.ValueMember = "id_premises_type";
            field_premises_type.DisplayMember = "premises_type";
        }

        public void LocatePremisesBy(int id)
        {
            v_premises.Position = v_premises.Find("id_premises", id);
        }

        void v_premises_CurrentItemChanged(object sender, EventArgs e)
        {
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanMoveFirst()
        {
            return v_premises.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_premises.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_premises.Position > -1) && (v_premises.Position < (v_premises.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_premises.Position > -1) && (v_premises.Position < (v_premises.Count - 1));
        }

        public override void MoveFirst()
        {
            v_premises.MoveFirst();
        }

        public override void MovePrev()
        {
            v_premises.MovePrevious();
        }

        public override void MoveNext()
        {
            v_premises.MoveNext();
        }

        public override void MoveLast()
        {
            v_premises.MoveLast();
        }

        public override bool CanDeleteRecord()
        {
            if (v_premises.Position == -1)
                return false;
            else
                return true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это помещение?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (premises.Delete((int)((DataRowView)v_premises.Current)["id_premises"]) == -1)
                    return;
                ((DataRowView)v_premises[v_premises.Position]).Delete();
                menuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            if (DynamicFilter != "")
                return true;
            else
                return false;
        }

        public override void SearchRecord()
        {
            SearchPremisesForm spForm = new SearchPremisesForm();
            if (spForm.ShowDialog() == DialogResult.OK)
            {
                DynamicFilter = spForm.GetFilter();
                v_premises.Filter = StaticFilter;
                if (StaticFilter != "" && DynamicFilter != "")
                    v_premises.Filter += " AND ";
                v_premises.Filter += DynamicFilter;
                if (DynamicFilter != "")
                    menuCallback.NavigationStateUpdate();
            }
        }

        public override void ClearSearch()
        {
            v_premises.Filter = StaticFilter;
            DynamicFilter = "";
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanOpenDetails()
        {
            if (v_premises.Position == -1)
                return false;
            else
                return true;
        }

        public override void OpenDetails()
        {
            PremisesViewport viewport = new PremisesViewport(menuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            if (!premises.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            PremisesViewport viewport = new PremisesViewport(menuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            viewport.InsertRecord();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override bool CanCopyRecord()
        {
            return (v_premises.Position != -1) && !premises.EditingNewRecord;
        }

        public override void CopyRecord()
        {
            PremisesViewport viewport = new PremisesViewport(menuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
            viewport.CopyRecord();
        }

        public override bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        private void ConstructViewport()
        {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            this.Controls.Add(this.dataGridView);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Dock = DockStyle.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_premises,
            this.field_street,
            this.field_house,
            this.field_premises_num,
            this.field_premises_type,
            this.field_total_area,
            this.field_living_area,
            this.field_cadastral_num});
            this.dataGridView.Location = new System.Drawing.Point(6, 6);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.TabIndex = 0;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.MultiSelect = false;
            // 
            // field_id_premises
            // 
            this.field_id_premises.HeaderText = "№";
            this.field_id_premises.Name = "id_premises";
            this.field_id_premises.ReadOnly = true;
            // 
            // field_street
            // 
            this.field_street.HeaderText = "Адрес";
            this.field_street.MinimumWidth = 300;
            this.field_street.Name = "id_street";
            this.field_street.ReadOnly = true;
            this.field_street.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            // 
            // field_street
            // 
            this.field_house.HeaderText = "Дом";
            this.field_house.Name = "house";
            this.field_house.ReadOnly = true;
            this.field_house.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            // 
            // field_premises_num
            // 
            this.field_premises_num.HeaderText = "Помещение";
            this.field_premises_num.Name = "premises_num";
            this.field_premises_num.ReadOnly = true;
            // 
            // field_premises_type
            // 
            this.field_premises_type.HeaderText = "Тип помещения";
            this.field_premises_type.MinimumWidth = 150;
            this.field_premises_type.Name = "premises_type";
            this.field_premises_type.ReadOnly = true;
            this.field_premises_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            // 
            // field_cadastral_num
            // 
            this.field_cadastral_num.HeaderText = "Кадастровый номер";
            this.field_cadastral_num.MinimumWidth = 150;
            this.field_cadastral_num.Name = "cadastral_num";
            this.field_cadastral_num.ReadOnly = true;
            // 
            // field_total_area
            // 
            this.field_total_area.HeaderText = "Общая площадь";
            this.field_total_area.MinimumWidth = 120;
            this.field_total_area.Name = "total_area";
            this.field_total_area.DefaultCellStyle.Format = "#0.0## м²";
            this.field_cadastral_num.ReadOnly = true;
            // 
            // field_living_area
            // 
            this.field_living_area.HeaderText = "Жилая площадь";
            this.field_living_area.MinimumWidth = 150;
            this.field_living_area.Name = "living_area";
            this.field_living_area.DefaultCellStyle.Format = "#0.0## м²";
            this.field_living_area.ReadOnly = true;

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }

        public override Viewport Duplicate()
        {
            PremisesListViewport viewport = new PremisesListViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_premises.Count > 0)
                viewport.LocatePremisesBy((((DataRowView)v_premises[v_premises.Position])["id_premises"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override bool HasAssocOwnerships()
        {
            return true;
        }

        public override bool HasAssocRestrictions()
        {
            return true;
        }

        public override bool HasAssocSubPremises()
        {
            return true;
        }

        public override bool HasFundHistory()
        {
            return true;
        }

        public override void ShowOwnerships()
        {
            //TODO
        }

        public override void ShowRestrictions()
        {
            //TODO
        }

        public override void ShowSubPremises()
        {
            //TODO
        }

        public override void ShowFundHistory()
        {
            //TODO
        }
    }
}
