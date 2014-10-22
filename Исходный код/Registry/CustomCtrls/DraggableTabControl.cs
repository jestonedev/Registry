using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomControls
{
    public class DraggableTabControl : TabControl
    {
        private TabPage predraggedTab;

        public DraggableTabControl()
        {
            this.AllowDrop = true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            predraggedTab = getPointedTab();
            
            if (e.Button == MouseButtons.Left && predraggedTab != null)
            {
                DataObject d = new DataObject();
                d.SetData(typeof(TabPage), predraggedTab);
                this.DoDragDrop(d, DragDropEffects.Move);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            predraggedTab = null;

            base.OnMouseUp(e);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            TabPage draggedTab = (TabPage)drgevent.Data.GetData(typeof(TabPage));
            TabPage pointedTab = getPointedTab();

            if (draggedTab == predraggedTab && pointedTab != null)
            {
                drgevent.Effect = DragDropEffects.Move;

                if (pointedTab != draggedTab)
                    swapTabPages(draggedTab, pointedTab);
            }

            base.OnDragOver(drgevent);
        }

        private TabPage getPointedTab()
        {
            for (int i = 0; i < this.TabPages.Count; i++)
                if (this.GetTabRect(i).Contains(this.PointToClient(Cursor.Position)))
                    return this.TabPages[i];

            return null;
        }

        private void swapTabPages(TabPage src, TabPage dst)
        {
            int srci = this.TabPages.IndexOf(src);
            int dsti = this.TabPages.IndexOf(dst);

            this.TabPages[dsti] = src;
            this.TabPages[srci] = dst;

            if (this.SelectedIndex == srci)
                this.SelectedIndex = dsti;
            else if (this.SelectedIndex == dsti)
                this.SelectedIndex = srci;

            this.Refresh();
        }
    }
}