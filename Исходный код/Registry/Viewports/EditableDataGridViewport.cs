using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport
{
    internal class EditableDataGridViewport: Viewport
    {
        protected DataTable GeneralSnapshot;
        protected BindingSource GeneralSnapshotBindingSource;

        //Флаг разрешения синхронизации snapshot и original моделей
        protected bool sync_views = true;

        protected EditableDataGridViewport(): this(null, null)
        {
        }

        protected EditableDataGridViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
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

        protected virtual bool SnapshotHasChanges()
        {
            var listFromView = EntitiesListFromView();
            var listFromViewport = EntitiesListFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            foreach (var documentFromView in listFromView)
            {
                var founded = false;
                foreach (var documentFromViewport in listFromViewport)
                    if (documentFromView.Equals(documentFromViewport))
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        protected virtual List<Entity> EntitiesListFromView()
        {
            return new List<Entity>();
        }

        protected virtual List<Entity> EntitiesListFromViewport()
        {
            return new List<Entity>();
        }
    }
}
