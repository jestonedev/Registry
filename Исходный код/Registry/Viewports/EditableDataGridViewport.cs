using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal class EditableDataGridViewport: Viewport
    {
        protected DataTable GeneralSnapshot;
        protected BindingSource GeneralSnapshotBindingSource;

        //Флаг разрешения синхронизации snapshot и original моделей
        protected bool sync_views = true;

        protected EditableDataGridViewport(): this(null)
        {
        }

        protected EditableDataGridViewport(IMenuCallback menuCallback): base(menuCallback)
        {
        }

        public sealed override void MoveFirst()
        {
            GeneralSnapshotBindingSource.MoveFirst();
        }

        public sealed override void MoveLast()
        {
            GeneralSnapshotBindingSource.MoveLast();
        }

        public sealed override void MoveNext()
        {
            GeneralSnapshotBindingSource.MoveNext();
        }

        public sealed override void MovePrev()
        {
            GeneralSnapshotBindingSource.MovePrevious();
        }

        public sealed override bool CanMoveFirst()
        {
            return GeneralSnapshotBindingSource.Position > 0;
        }

        public sealed override bool CanMovePrev()
        {
            return GeneralSnapshotBindingSource.Position > 0;
        }

        public sealed override bool CanMoveNext()
        {
            return (GeneralSnapshotBindingSource.Position > -1) && (GeneralSnapshotBindingSource.Position < (GeneralSnapshotBindingSource.Count - 1));
        }

        public sealed override bool CanMoveLast()
        {
            return (GeneralSnapshotBindingSource.Position > -1) && (GeneralSnapshotBindingSource.Position < (GeneralSnapshotBindingSource.Count - 1));
        }

        public sealed override int GetRecordCount()
        {
            return GeneralSnapshotBindingSource.Count;
        }
    }
}
