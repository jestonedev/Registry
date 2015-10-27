using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal class DataGridViewport: Viewport
    {
        protected DataGridView DataGridView;

        protected DataGridViewport(): this(null, null)
        {
        }

        protected DataGridViewport(Viewport viewport, IMenuCallback menuCallback): base(viewport, menuCallback)
        {
        }

        public sealed override bool CanMoveFirst()
        {
            return GeneralBindingSource.Position > 0;
        }

        public sealed override bool CanMovePrev()
        {
            return GeneralBindingSource.Position > 0;
        }

        public sealed override bool CanMoveNext()
        {
            return (GeneralBindingSource.Position > -1) && (GeneralBindingSource.Position < (GeneralBindingSource.Count - 1));
        }

        public sealed override bool CanMoveLast()
        {
            return (GeneralBindingSource.Position > -1) && (GeneralBindingSource.Position < (GeneralBindingSource.Count - 1));
        }

        public sealed override void MoveFirst()
        {
            GeneralBindingSource.MoveFirst();
        }

        public sealed override void MovePrev()
        {
            GeneralBindingSource.MovePrevious();
        }

        public sealed override void MoveNext()
        {
            GeneralBindingSource.MoveNext();
        }

        public sealed override void MoveLast()
        {
            GeneralBindingSource.MoveLast();
        }

        protected void GeneralBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (GeneralBindingSource.Position == -1 || DataGridView.RowCount == 0)
            {
                DataGridView.ClearSelection();
                return;
            }
            if (GeneralBindingSource.Position >= DataGridView.RowCount)
            {
                DataGridView.Rows[DataGridView.RowCount - 1].Selected = true;
                DataGridView.CurrentCell = DataGridView.CurrentCell != null ? 
                    DataGridView.Rows[DataGridView.RowCount - 1].Cells[DataGridView.CurrentCell.ColumnIndex] : 
                    DataGridView.Rows[DataGridView.RowCount - 1].Cells[0];
            }
            else
            {
                DataGridView.Rows[GeneralBindingSource.Position].Selected = true;
                DataGridView.CurrentCell = DataGridView.CurrentCell != null ? 
                    DataGridView.Rows[GeneralBindingSource.Position].Cells[DataGridView.CurrentCell.ColumnIndex] : 
                    DataGridView.Rows[GeneralBindingSource.Position].Cells[0];
            }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.EditingStateUpdate();
            MenuCallback.RelationsStateUpdate();
            MenuCallback.DocumentsStateUpdate();
        }

        public override void LocateEntityBy(string fieldName, object value)
        {
            var position = GeneralBindingSource.Find(fieldName, value);
            if (position > 0)
                GeneralBindingSource.Position = position;
        }

        public override Viewport Duplicate()
        {
            var viewport = (Viewport)Activator.CreateInstance(GetType(), this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (GeneralBindingSource.Count <= 0 || !GeneralDataModel.Select().PrimaryKey.Any()) return viewport;
            var fileName = GeneralDataModel.Select().PrimaryKey[0].ColumnName;
            viewport.LocateEntityBy(fileName,
                (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])[fileName] as int?) ?? -1);
            return viewport;
        }

        public override bool CanDuplicate()
        {
            return true;
        }
    }
}
