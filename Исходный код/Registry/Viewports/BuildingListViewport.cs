using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport
{
    internal class BuildingListViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_building = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_street = new DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_house = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_floors = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_living_area = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_cadastral_num = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_startup_year = new DataGridViewTextBoxColumn();
        #endregion Components

        //Models
        private BuildingsDataModel buildings = null;
        private KladrDataModel kladr = null;

        //Views
        private BindingSource v_buildings = null;
        private BindingSource v_kladr = null;

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        public BuildingListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageBuildings";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень зданий";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public BuildingListViewport(BuildingListViewport buildingListViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = buildingListViewport.DynamicFilter;
            this.StaticFilter = buildingListViewport.StaticFilter;
            this.ParentRow = buildingListViewport.ParentRow;
            this.ParentType = buildingListViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            buildings = BuildingsDataModel.GetInstance();
            kladr = KladrDataModel.GetInstance();

            DataSet ds = DataSetManager.GetDataSet();

            v_buildings = new BindingSource();
            v_buildings.DataMember = "buildings";
            v_buildings.CurrentItemChanged += new EventHandler(v_buildings_CurrentItemChanged);
            v_buildings.DataSource = ds;
            v_buildings.Filter = StaticFilter;
            if (StaticFilter != "" && DynamicFilter != "")
                v_buildings.Filter += " AND ";
            v_buildings.Filter += DynamicFilter;

            v_kladr = new BindingSource();
            v_kladr.DataMember = "kladr";
            v_kladr.DataSource = ds;

            dataGridView.DataSource = v_buildings;

            field_id_building.DataPropertyName = "id_building";
            field_id_street.DataSource = v_kladr;
            field_id_street.DataPropertyName = "id_street";
            field_id_street.ValueMember = "id_street";
            field_id_street.DisplayMember = "street_name";
            field_house.DataPropertyName = "house";
            field_floors.DataPropertyName = "floors";
            field_living_area.DataPropertyName = "living_area";
            field_cadastral_num.DataPropertyName = "cadastral_num";
            field_startup_year.DataPropertyName = "startup_year";

            dataGridView.DoubleClick += new EventHandler(dataGridView_DoubleClick);
        }

        public void LocateBuildingBy(int id)
        {
            v_buildings.Position = v_buildings.Find("id_building", id);
        }

        void v_buildings_CurrentItemChanged(object sender, EventArgs e)
        {
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanMoveFirst()
        {
            return v_buildings.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_buildings.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_buildings.Position > -1) && (v_buildings.Position < (v_buildings.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_buildings.Position > -1) && (v_buildings.Position < (v_buildings.Count - 1));
        }

        public override void MoveFirst()
        {
            v_buildings.MoveFirst();
        }

        public override void MovePrev()
        {
            v_buildings.MovePrevious();
        }

        public override void MoveNext()
        {
            v_buildings.MoveNext();
        }

        public override void MoveLast()
        {
            v_buildings.MoveLast();
        }

        public override bool CanDeleteRecord()
        {
            if (v_buildings.Position == -1)
                return false;
            else
                return true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это здание?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (buildings.Delete((int)((DataRowView)v_buildings.Current)["id_building"]) == -1)
                    return;
                ((DataRowView)v_buildings[v_buildings.Position]).Delete();
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
            SearchBuildingForm sbForm = new SearchBuildingForm();
            if (sbForm.ShowDialog() == DialogResult.OK)
            {
                DynamicFilter = sbForm.GetFilter();
                v_buildings.Filter = StaticFilter;
                if (StaticFilter != "" && DynamicFilter != "")
                    v_buildings.Filter += " AND ";
                v_buildings.Filter += DynamicFilter;
                if (DynamicFilter != "")
                    menuCallback.NavigationStateUpdate();
            }
        }

        public override void ClearSearch()
        {
            v_buildings.Filter = StaticFilter;
            DynamicFilter = "";
            menuCallback.NavigationStateUpdate();
        }

        public override bool CanOpenDetails()
        {
            if (v_buildings.Position == -1)
                return false;
            else
                return true;
        }

        void dataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (CanOpenDetails())
                OpenDetails();
        }

        public override void OpenDetails()
        {
            BuildingViewport viewport = new BuildingViewport(menuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as Int32?) ?? -1);
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override bool HasAssocPremises()
        {
            return (v_buildings.Position > -1);
        }

        public override bool HasAssocOwnerships()
        {
            return true;
        }

        public override bool HasAssocRestrictions()
        {
            return true;
        }

        public override bool HasFundHistory()
        {
            return true;
        }

        public override void ShowPremises()
        {
            PremisesListViewport viewport = new PremisesListViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowOwnerships()
        {
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения ограничений", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            OwnershipListViewport viewport = new OwnershipListViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowRestrictions()
        {
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения реквизитов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            RestrictionListViewport viewport = new RestrictionListViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override void ShowFundHistory()
        {
            if (v_buildings.Position == -1)
            {
                MessageBox.Show("Не выбрано здание для отображения истории найма", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FundsHistoryViewport viewport = new FundsHistoryViewport(menuCallback);
            viewport.StaticFilter = "id_building = " + Convert.ToInt32(((DataRowView)v_buildings[v_buildings.Position])["id_building"]);
            viewport.ParentRow = ((DataRowView)v_buildings[v_buildings.Position]).Row;
            viewport.ParentType = ParentTypeEnum.Building;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
        }

        public override bool CanInsertRecord()
        {
            if (!buildings.EditingNewRecord)
                return true;
            else
                return false;
        }

        public override void InsertRecord()
        {
            BuildingViewport viewport = new BuildingViewport(menuCallback);
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
            if ((v_buildings.Position != -1) && (!buildings.EditingNewRecord))
                return true;
            else
                return false;
        }

        public override void CopyRecord()
        {
            BuildingViewport viewport = new BuildingViewport(menuCallback);
            viewport.StaticFilter = StaticFilter;
            viewport.DynamicFilter = DynamicFilter;
            viewport.ParentRow = ParentRow;
            viewport.ParentType = ParentType;
            if (viewport.CanLoadData())
                viewport.LoadData();
            else
                return;
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as Int32?) ?? -1);
            menuCallback.AddViewport(viewport);
            menuCallback.SwitchToViewport(viewport);
            viewport.CopyRecord();
        }

        private void ConstructViewport()
        {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).BeginInit();
            this.Controls.Add(dataGridView);
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                field_id_building,
                field_id_street,
                field_house,
                field_floors,
                field_living_area,
                field_cadastral_num,
                field_startup_year});
            dataGridView.Location = new System.Drawing.Point(6, 6);
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView.TabIndex = 0;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.MultiSelect = false;
            // 
            // field_id_building
            // 
            field_id_building.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            field_id_building.HeaderText = "№";
            field_id_building.Name = "id_building";
            field_id_building.ReadOnly = true;
            // 
            // field_id_street
            // 
            field_id_street.HeaderText = "Адрес";
            field_id_street.MinimumWidth = 300;
            field_id_street.Name = "id_street";
            field_id_street.ReadOnly = true;
            field_id_street.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            // 
            // field_house
            // 
            field_house.HeaderText = "Дом";
            field_house.Name = "house";
            field_house.ReadOnly = true;
            // 
            // field_floors
            // 
            field_floors.HeaderText = "Этажность";
            field_floors.Name = "floors";
            field_floors.ReadOnly = true;
            // 
            // field_living_area
            // 
            field_living_area.HeaderText = "Жилая площадь";
            field_living_area.MinimumWidth = 150;
            field_living_area.Name = "living_area";
            field_living_area.ReadOnly = true;
            field_living_area.DefaultCellStyle.Format = "#0.0## м²";
            // 
            // field_cadastral_num
            // 
            field_cadastral_num.HeaderText = "Кадастровый номер";
            field_cadastral_num.MinimumWidth = 150;
            field_cadastral_num.Name = "cadastral_num";
            field_cadastral_num.ReadOnly = true;
            // 
            // field_startup_year
            // 
            field_startup_year.HeaderText = "Год ввода в эксплуатацию";
            field_startup_year.MinimumWidth = 200;
            field_startup_year.Name = "startup_year";
            field_startup_year.ReadOnly = true;

            ((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
        }

        public override Viewport Duplicate()
        {
            BuildingListViewport viewport = new BuildingListViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_buildings.Count > 0)
                viewport.LocateBuildingBy((((DataRowView)v_buildings[v_buildings.Position])["id_building"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool CanDuplicate()
        {
            return true;
        }
    }
}
