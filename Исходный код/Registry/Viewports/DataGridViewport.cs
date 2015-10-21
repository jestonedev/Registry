using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Viewport
{
    internal class DataGridViewport: Viewport
    {
        protected DataGridViewport(): this(null)
        {
        }

        protected DataGridViewport(IMenuCallback menuCallback): base(menuCallback)
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
    }
}
