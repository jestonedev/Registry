using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.Presenters;

namespace Registry.Viewport
{
    internal class FormViewport: Viewport
    {
        protected bool IsEditable;
        protected ViewportState ViewportState = ViewportState.ReadState;

        protected FormViewport(): this(null, null)
        {
        }

        protected FormViewport(Viewport viewport, IMenuCallback menuCallback, Presenter presenter = null)
            : base(viewport, menuCallback, presenter)
        {
        }

        public sealed override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            IsEditable = false;
            GeneralBindingSource.MoveFirst();
            IsEditable = true;
        }

        public sealed override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            IsEditable = false;
            GeneralBindingSource.MoveLast();
            IsEditable = true;
        }

        public sealed override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            IsEditable = false;
            GeneralBindingSource.MoveNext();
            IsEditable = true;
        }

        public sealed override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            IsEditable = false;
            GeneralBindingSource.MovePrevious();
            IsEditable = true;
        }

        public sealed override bool CanMoveFirst()
        {
            return GeneralBindingSource != null && GeneralBindingSource.Position > 0;
        }

        public sealed override bool CanMovePrev()
        {
            return GeneralBindingSource != null && GeneralBindingSource.Position > 0;
        }

        public sealed override bool CanMoveNext()
        {
            return GeneralBindingSource != null && (GeneralBindingSource.Position > -1) && (GeneralBindingSource.Position < (GeneralBindingSource.Count - 1));
        }

        public sealed override bool CanMoveLast()
        {
            return GeneralBindingSource != null && (GeneralBindingSource.Position > -1) && (GeneralBindingSource.Position < (GeneralBindingSource.Count - 1));
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
            switch (ViewportState)
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
                    return ViewportState == ViewportState.ReadState;
            }
            return false;
        }

        private bool SetModifyRowState()
        {
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    ViewportState = ViewportState.ModifyRowState;
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
                    return ViewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.ModifyRowState);
            }
            return false;
        }

        private bool SetNewRowState()
        {
            switch (ViewportState)
            {
                case ViewportState.ReadState:
                    if (GeneralDataModel.EditingNewRecord)
                        return false;
                    ViewportState = ViewportState.NewRowState;
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
                    return ViewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.NewRowState);
            }
            return false;
        }

        protected virtual void CheckViewportModifications()
        {
            if (!IsEditable)
                return;
            if (!ContainsFocus)
                return;
            if ((GeneralBindingSource.Position != -1) && (!EntityFromView().Equals(EntityFromViewport())))
            {
                if (ViewportState == ViewportState.ReadState)
                    ViewportState = ViewportState.ModifyRowState;
            }
            else
            {
                if (ViewportState == ViewportState.ModifyRowState)
                    ViewportState = ViewportState.ReadState;
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
                    var numUpDown = control as NumericUpDown;
                    numUpDown.ValueChanged += (sender, args) => CheckViewportModifications();
                    numUpDown.Enter += (sender, args) => ViewportHelper.SelectAllText(sender);
                    numUpDown.Leave += (sender, args) =>
                    {
                        if (numUpDown.Text != "") return;
                        numUpDown.Text = @"0";
                        numUpDown.Value = 0;
                    };
                    numUpDown.MouseWheel += (sender, args) =>
                    {
                        OnMouseWheel(args);
                        ((HandledMouseEventArgs)args).Handled = true;
                    };
                } else
                if (control is DateTimePicker)
                    (control as DateTimePicker).ValueChanged += (sender, args) => CheckViewportModifications();
                else
                if (control is ComboBox)
                {
                    (control as ComboBox).SelectedValueChanged += (sender, args) => CheckViewportModifications();
                    (control as ComboBox).Enter += (sender, args) => ViewportHelper.SelectAllText(sender);
                    (control as ComboBox).MouseWheel += (sender, args) =>
                    {
                        var component = (ComboBox)sender;
                        if (!component.DroppedDown)
                        {
                            OnMouseWheel(args);
                            ((HandledMouseEventArgs)args).Handled = true;
                        }
                    };
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
            IsEditable = false;
            if (position > 0)
                GeneralBindingSource.Position = position;
            IsEditable = true;
        }

        public override Viewport Duplicate()
        {
            var viewport = (Viewport)Activator.CreateInstance(GetType(), this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (GeneralBindingSource == null || GeneralDataModel == null || GeneralBindingSource.Count <= 0 || 
                GeneralDataModel.Select() == null || !GeneralDataModel.Select().PrimaryKey.Any()) return viewport;
            var fileName = GeneralDataModel.Select().PrimaryKey[0].ColumnName;
            viewport.LocateEntityBy(fileName,
                (((DataRowView)GeneralBindingSource[GeneralBindingSource.Position])[fileName] as int?) ?? -1);
            return viewport;
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override void ForceClose()
        {
            if (ViewportState == ViewportState.NewRowState)
                GeneralDataModel.EditingNewRecord = false;
            ViewportState = ViewportState.ReadState;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Activate();
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            base.OnClosing(e);
        }
    }
}
