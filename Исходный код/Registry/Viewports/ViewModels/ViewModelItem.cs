using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;

namespace Registry.Viewport.ViewModels
{
    public class ViewModelItem
    {
        private DataTable _dataSource;

        public DataModel Model { get; private set; }

        public DataTable DataSource
        {
            get
            {
                if (_dataSource != null)
                {
                    return _dataSource;
                }
                return _dataSource = Model.Select();
            }
        }

        public BindingSource BindingSource { get; private set; }

        public DataRowView CurrentRow
        {
            get
            {
                return BindingSource.Position > -1 && BindingSource.Count > 0
                    ? (DataRowView) BindingSource[BindingSource.Position]
                    : null;
            }
        }

        public string PrimaryKeyFirst
        {
            get { return PrimaryKeyFirstColumn != null ? PrimaryKeyFirstColumn.ColumnName : null; }
        }

        public DataColumn PrimaryKeyFirstColumn
        {
            get { return DataSource.PrimaryKey.Any() ? DataSource.PrimaryKey.First() : null; }
        }

        public ViewModelItem(DataModel model, object bindingDataSource = null, string bindingDataMember = null)
        {
            if (model == null)
            {
                throw new ViewportException("Не передана ссылка на модель");
            }
            Model = model;
            var newBindingSource = new BindingSource();
            if (bindingDataSource != null)
            {
                newBindingSource.DataSource = bindingDataSource;
            }
            if (bindingDataMember != null)
            {
                newBindingSource.DataMember = bindingDataMember;
            }
            if (bindingDataSource == null && bindingDataMember == null)
            {
                if (model is CalcDataModel)
                {
                    newBindingSource.DataSource = model.Select();
                }
                else
                {
                    newBindingSource.DataSource = DataStorage.DataSet;
                    newBindingSource.DataMember = DataSource.TableName;   
                }
            }
            BindingSource = newBindingSource;
        }

        public bool Delete(int id)
        {
            if (Model.Delete(id) == -1)
                return false;
            CurrentRow.Delete();
            return true;
        }
    }
}
