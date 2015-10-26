
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport
{
    internal class FormViewport: Viewport
    {
        protected bool is_editable;
        protected ViewportState viewportState = ViewportState.ReadState;

        protected FormViewport(): this(null)
        {
        }

        protected FormViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
        }

        public sealed override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            GeneralBindingSource.MoveFirst();
            is_editable = true;
        }

        public sealed override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            GeneralBindingSource.MoveLast();
            is_editable = true;
        }

        public sealed override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            GeneralBindingSource.MoveNext();
            is_editable = true;
        }

        public sealed override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            GeneralBindingSource.MovePrevious();
            is_editable = true;
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

        protected virtual bool ChangeViewportStateTo(ViewportState state)
        {
            viewportState = ViewportState.ReadState;
            return true;
        }

        protected virtual void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if (!ContainsFocus)
                return;
            if ((GeneralBindingSource.Position != -1) && (!EntityFromView().Equals(EntityFromViewport())))
            {
                if (viewportState == ViewportState.ReadState)
                    viewportState = ViewportState.ModifyRowState;
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                    viewportState = ViewportState.ReadState;
            }
            if (Selected)
                MenuCallback.EditingStateUpdate();
        }

        protected virtual Entity EntityFromView()
        {
            return new Entity();
        }

        protected virtual Entity EntityFromViewport()
        {
            return new Entity();
        }

        protected virtual void DataChangeHandlersInit(Control parent = null)
        {
            if (parent == null)
                parent = this;
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox)
                {
                    (control as TextBox).TextChanged += (sender, args) => CheckViewportModifications();
                    (control as TextBox).Enter += (sender, args) => ViewportHelper.SelectAllText(sender);
                }
                if (control is DateTimePicker)
                    (control as DateTimePicker).ValueChanged += (sender, args) => CheckViewportModifications();
                if (control is NumericUpDown)
                {
                    (control as NumericUpDown).ValueChanged += (sender, args) => CheckViewportModifications();
                    (control as NumericUpDown).Enter += (sender, args) => ViewportHelper.SelectAllText(sender);
                }
                if (control is ComboBox)
                {
                    (control as ComboBox).SelectedValueChanged += (sender, args) => CheckViewportModifications();
                    (control as ComboBox).Enter += (sender, args) => ViewportHelper.SelectAllText(sender);
                }
                if (control is CheckBox)
                    (control as CheckBox).CheckedChanged += (sender, args) => CheckViewportModifications();
                if (control.Controls.Count > 0)
                    DataChangeHandlersInit(control);
            }
        }
    }
}
