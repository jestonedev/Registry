using System.Data;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;
using Security;

namespace Registry.Viewport.Presenters
{
    internal sealed class FundsHistoryPresenter: Presenter
    {
        public FundsHistoryPresenter(): base(new FundsHistoryViewModel(), null, null)
        {
            
        }

        public void AddAssocViewModelItem()
        {
            switch (ParentType)
            {
                case ParentTypeEnum.SubPremises:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<FundSubPremisesAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_sub_premises = " + ParentRow["id_sub_premises"];
                    break;
                case ParentTypeEnum.Premises:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<FundPremisesAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_premises = " + ParentRow["id_premises"];
                    break;
                case ParentTypeEnum.Building:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<FundBuildingAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_building = " + ParentRow["id_building"];
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
        }

        public void RebuildFilter()
        {
            var filter = "id_fund IN (0";
            foreach (var fund in ViewModel["assoc"].BindingSource)
                filter += ((DataRowView)fund)["id_fund"] + ",";
            filter = filter.TrimEnd(',');
            filter += ")";
            ViewModel["general"].BindingSource.Filter = filter;
        }

        public bool InsertRecord(FundHistory fundHistory)
        {
            var idParent =
                        (ParentType == ParentTypeEnum.SubPremises) && ParentRow != null ? (int)ParentRow["id_sub_premises"] :
                        (ParentType == ParentTypeEnum.Premises) && ParentRow != null ? (int)ParentRow["id_premises"] :
                        (ParentType == ParentTypeEnum.Building) && ParentRow != null ? (int)ParentRow["id_building"] :
                        -1;
            if (idParent == -1)
            {
                MessageBox.Show(@"Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            var idFund = ViewModel["general"].Model.Insert(fundHistory);
            if (idFund == -1)
            {
                return false;
            }
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    ViewModel["assoc"].Model.Insert(new FundBuildingAssoc(idParent, idFund));
                    break;
                case ParentTypeEnum.Premises:
                    ViewModel["assoc"].Model.Insert(new FundPremisesAssoc(idParent, idFund));
                    break;
                case ParentTypeEnum.SubPremises:
                    ViewModel["assoc"].Model.Insert(new FundSubPremisesAssoc(idParent, idFund));
                    break;
            }
            fundHistory.IdFund = idFund;
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<FundHistory>.FillRow(fundHistory, row);
            ViewModel["assoc"].DataSource.Rows.Add(idParent, idFund);
            RebuildFilter();
            ViewModel["general"].BindingSource.Position = ViewModel["general"].BindingSource.Count - 1;
            ViewModel["general"].Model.EditingNewRecord = false;
            return true;
        }

        public bool UpdateRecord(FundHistory fundHistory)
        {
            if (fundHistory.IdFund == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить запись о принадлежности фонду без внутренного номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(fundHistory) == -1)
                return false;
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<FundHistory>.FillRow(fundHistory, row);
            return true;
        }

        public bool DeleteRecord()
        {
            var row = ViewModel["general"].CurrentRow;
            var columnName = ViewModel["general"].PrimaryKeyFirst;
            if (ViewModel["general"].Model.Delete((int)row[columnName]) == -1)
                return false;
            row.Delete();
            return true;
        }

        public bool ValidatePermissions()
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
            if (OtherService.HasMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации об истории фондов муниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (OtherService.HasNotMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации об истории фондов немуниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }    
  

    }
}
