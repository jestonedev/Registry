using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class ResettleProcessListPresenter: Presenter
    {
        public ResettleProcessListPresenter()
            : base(new ResettleProcessListViewModel(), new SimpleSearchResettleForm(), new ExtendedSearchResettleForm())
        {
            
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

        public bool UpdateRecord(ResettleProcess resettleProcess)
        {
            if (resettleProcess.IdProcess == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить запись о процессе переселения без внутреннего номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(resettleProcess) == -1)
                return false;
            RebuildFilterAfterSave(ViewModel["general"].BindingSource, resettleProcess.IdProcess);
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<ResettleProcess>.FillRow(resettleProcess, row);
            return true;
        }

        public bool InsertRecord(ResettleProcess resettleProcess)
        {
            var id = ViewModel["general"].Model.Insert(resettleProcess);
            if (id == -1)
            {
                return false;
            }
            resettleProcess.IdProcess = id;
            RebuildFilterAfterSave(ViewModel["general"].BindingSource, resettleProcess.IdProcess);
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<ResettleProcess>.FillRow(resettleProcess, row);
            ViewModel["general"].Model.EditingNewRecord = false;
            return true;
        }

        private void RebuildFilterAfterSave(IBindingListView bindingSource, int? idProcess)
        {
            var filter = "";
            if (!string.IsNullOrEmpty(bindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            filter += string.Format(CultureInfo.CurrentCulture, "(id_process = {0})", idProcess);
            bindingSource.Filter += filter;
        }
    }
}
