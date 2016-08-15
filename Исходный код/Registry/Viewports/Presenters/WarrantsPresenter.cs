using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class WarrantsPresenter: Presenter
    {
        public WarrantsPresenter() : base(new WarrantsViewModel(), null, null)
        {
        }

        internal bool DeleteRecord()
        {
            var row = ViewModel["general"].CurrentRow;
            var columnName = ViewModel["general"].PrimaryKeyFirst;
            if (ViewModel["general"].Model.Delete((int)row[columnName]) == -1)
                return false;
            row.Delete();
            return true;
        }

        internal bool InsertRecord(Warrant warrant)
        {
            var idWarrant = ViewModel["general"].Model.Insert(warrant);
            if (idWarrant == -1)
            {
                return false;
            }
            warrant.IdWarrant = idWarrant;
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<Warrant>.FillRow(warrant, row);
            ViewModel["general"].Model.EditingNewRecord = false;
            return true;
        }

        internal bool UpdateRecord(Warrant warrant)
        {
            if (warrant.IdWarrant == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить запись о доверенности без внутренного номера. " +
                             @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                             MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(warrant) == -1)
                return false;
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<Warrant>.FillRow(warrant, row);
            return true;
        }
    }
}
