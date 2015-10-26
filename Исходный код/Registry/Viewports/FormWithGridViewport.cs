
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport
{
    internal class FormWithGridViewport: FormViewport
    {
        protected DataGridView DataGridView;

        protected FormWithGridViewport(): this(null)
        {
        }

        protected FormWithGridViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
        }

        protected override void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if (!ContainsFocus || (DataGridView.Focused))
                return;
            if ((GeneralBindingSource.Position != -1) && (!EntityFromView().Equals(EntityFromViewport())))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    DataGridView.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    DataGridView.Enabled = true;
                }
            }
            if (Selected)
                MenuCallback.EditingStateUpdate();
        }
    }
}
