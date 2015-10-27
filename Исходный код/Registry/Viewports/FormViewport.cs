
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport
{
    internal class FormViewport: Viewport
    {
        protected bool is_editable;
        protected ViewportState viewportState = ViewportState.ReadState;

        protected FormViewport(): this(null, null)
        {
        }

        protected FormViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
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
            switch (state)
            {
                case ViewportState.ReadState:
                    return SetReadState();
                case ViewportState.NewRowState:
                    return SetNewRowState();
                case ViewportState.ModifyRowState:
                    return SetModifyRowState();
            }
            return false;
        }

        private bool SetReadState()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    return true;
                case ViewportState.NewRowState:
                case ViewportState.ModifyRowState:
                    var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    switch (result)
                    {
                        case DialogResult.Yes:
                            SaveRecord();
                            break;
                        case DialogResult.No:
                            CancelRecord();
                            break;
                        default:
                            return false;
                    }
                    return viewportState == ViewportState.ReadState;
            }
            return false;
        }

        private bool SetModifyRowState()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    viewportState = ViewportState.ModifyRowState;
                    return true;
                case ViewportState.ModifyRowState:
                    return true;
                case ViewportState.NewRowState:
                    var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    switch (result)
                    {
                        case DialogResult.Yes:
                            SaveRecord();
                            break;
                        case DialogResult.No:
                            CancelRecord();
                            break;
                        default:
                            return false;
                    }
                    return viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.ModifyRowState);
            }
            return false;
        }

        private bool SetNewRowState()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    if (GeneralDataModel.EditingNewRecord)
                        return false;
                    viewportState = ViewportState.NewRowState;
                    return true;
                case ViewportState.NewRowState:
                    return true;
                case ViewportState.ModifyRowState:
                    var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    switch (result)
                    {
                        case DialogResult.Yes:
                            SaveRecord();
                            break;
                        case DialogResult.No:
                            CancelRecord();
                            break;
                        default:
                            return false;
                    }
                    return viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.NewRowState);
            }
            return false;
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
                if (control is NumericUpDown)
                {
                    (control as NumericUpDown).ValueChanged += (sender, args) => CheckViewportModifications();
                    (control as NumericUpDown).Enter += (sender, args) => ViewportHelper.SelectAllText(sender);
                } else
                if (control is DateTimePicker)
                    (control as DateTimePicker).ValueChanged += (sender, args) => CheckViewportModifications();
                else
                if (control is ComboBox)
                {
                    (control as ComboBox).SelectedValueChanged += (sender, args) => CheckViewportModifications();
                    (control as ComboBox).Enter += (sender, args) => ViewportHelper.SelectAllText(sender);
                }
                else
                if (control is CheckBox)
                    (control as CheckBox).CheckedChanged += (sender, args) => CheckViewportModifications();
                else
                if (control is TextBox && !string.IsNullOrEmpty(control.Name))
                {
                    (control as TextBox).TextChanged += (sender, args) => CheckViewportModifications();
                    (control as TextBox).Enter += (sender, args) => ViewportHelper.SelectAllText(sender);
                }
                if (control.Controls.Count > 0)
                    DataChangeHandlersInit(control);
            }
        }

        public override void LocateEntityBy(string fieldName, object value)
        {
            var position = GeneralBindingSource.Find(fieldName, value);
            is_editable = false;
            if (position > 0)
                GeneralBindingSource.Position = position;
            is_editable = true;
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
