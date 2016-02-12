using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace xmltv
{
    public delegate void DListBoxDragDropHelperEventListener(
        ListBoxDragDropHelper sender, int dragitemindex, int dropitemindex);

    public class ListBoxDragDropHelper
    {
        private ListBox DragListBox = null;
        private ListBox DropListBox = null;

        private int indexOfItemUnderMouseToDrag;
        private int indexOfItemUnderMouseToDrop;

        private Rectangle dragBoxFromMouseDown;
        private Point screenOffset;

        private bool DragStarted = false;

        private System.Timers.Timer ScrollTimer = null;
        private int ScrollDelta = 0;

        private DListBoxDragDropHelperEventListener OnDrop = null;

        public ListBoxDragDropHelper(ListBox draglistbox, ListBox droplistbox, DListBoxDragDropHelperEventListener ondrop)
        {
            if (draglistbox == null || droplistbox == null)
            {
                throw new Exception("Null listbox");
            }
            if (draglistbox.FindForm() != droplistbox.FindForm())
            {
                throw new Exception("ListBox not on same form");
            }
            DragListBox = draglistbox;
            DropListBox = droplistbox;
            OnDrop = ondrop;
            DragListBox.MouseDown += DragListBox_MouseDown;
            DragListBox.MouseMove += DragListBox_MouseMove;
            DragListBox.MouseUp += DragListBox_MouseUp;
            DragListBox.QueryContinueDrag += DragListBox_QueryContinueDrag;
            DragListBox.GiveFeedback += DragListBox_GiveFeedback;
            DropListBox.DragOver += DropListBox_DragOver;
            DropListBox.DragDrop += DropListBox_DragDrop;
            DropListBox.DragLeave += DropListBox_DragLeave;
        }

        private void DropListBox_DragDrop(object sender, DragEventArgs e)
        {
            // Ensure that the list item index is contained in the data. 
            if (!DragStarted) return;
            DragStarted = false;

            DoScrollKeepChannelsListBox(0);
            if (e.Data.GetDataPresent(typeof(ListBox)))
            {

                ListBox lbox = (ListBox)e.Data.GetData(typeof(ListBox));

                // Perform drag-and-drop, depending upon the effect. 
                if (e.Effect == DragDropEffects.Move)
                {

                    if (lbox == DragListBox && indexOfItemUnderMouseToDrag > -2)
                    {
                        if (OnDrop != null)
                        {
                            OnDrop(this, indexOfItemUnderMouseToDrag, indexOfItemUnderMouseToDrop);
                        }
                    }
                }
            }
        }

        private void DragListBox_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            indexOfItemUnderMouseToDrag = DragListBox.IndexFromPoint(e.X, e.Y);

            if (indexOfItemUnderMouseToDrag != ListBox.NoMatches)
            {

                // Remember the point where the mouse down occurred. The DragSize indicates 
                // the size that the mouse can move before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being 
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                               e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;

        }

        private void DragListBox_MouseUp(object sender, MouseEventArgs e)
        {
            // Reset the drag rectangle when the mouse button is raised.
            dragBoxFromMouseDown = Rectangle.Empty;

        }

        private void DragListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;

            // If the mouse moves outside the rectangle, start the drag. 
            if (dragBoxFromMouseDown != Rectangle.Empty &&
                !dragBoxFromMouseDown.Contains(e.X, e.Y))
            {


                // The screenOffset is used to account for any desktop bands  
                // that may be at the top or left side of the screen when  
                // determining when to cancel the drag drop operation.
                screenOffset = SystemInformation.WorkingArea.Location;

                DragStarted = true;

                // Proceed with the drag-and-drop, passing in the list item.                    
                DragDropEffects dropEffect =
                    DragListBox.DoDragDrop(DragListBox, DragDropEffects.All | DragDropEffects.Link);

                DragStarted = false;
            }

        }


        private void DragListBox_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = true;
        }

        private void DragListBox_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            // Cancel the drag if the mouse moves off the form.
            if (!DragStarted) return;

            ListBox lb = sender as ListBox;
            if (lb == null) return;

            Form f = lb.FindForm();

            // Cancel the drag if the mouse moves off the form. The screenOffset 
            // takes into account any desktop bands that may be at the top or left 
            // side of the screen. 
            if (((Control.MousePosition.X - screenOffset.X) < f.DesktopBounds.Left) ||
                ((Control.MousePosition.X - screenOffset.X) > f.DesktopBounds.Right) ||
                ((Control.MousePosition.Y - screenOffset.Y) < f.DesktopBounds.Top) ||
                ((Control.MousePosition.Y - screenOffset.Y) > f.DesktopBounds.Bottom))
            {

                DragStarted = false;
                e.Action = DragAction.Cancel;
                DoScrollKeepChannelsListBox(0);
            }

        }


        private void DropListBox_DragOver(object sender, DragEventArgs e)
        {
            if (!DragStarted) return;

            // Determine whether string data exists in the drop data. If not, then 
            // the drop effect reflects that the drop cannot occur. 
            if (!e.Data.GetDataPresent(typeof(ListBox)))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            ListBox lbox = (ListBox)e.Data.GetData(typeof(ListBox));
            if (lbox != DragListBox)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;

            // Get the index of the item the mouse is below.  

            // The mouse locations are relative to the screen, so they must be  
            // converted to client coordinates.

            Point pt = DropListBox.PointToClient(new Point(e.X, e.Y));
            indexOfItemUnderMouseToDrop = DropListBox.IndexFromPoint(pt);
            if (pt.Y < DropListBox.ItemHeight / 2)
            {
                DoScrollKeepChannelsListBox(-1);
            }
            else if (pt.Y > DropListBox.ClientSize.Height - DropListBox.ItemHeight / 2)
            {
                DoScrollKeepChannelsListBox(1);
            }
            else
            {
                DoScrollKeepChannelsListBox(0);
            }

        }

        private void DropListBox_DragLeave(object sender, EventArgs e)
        {
            DoScrollKeepChannelsListBox(0);
        }

        private void OnScrollTimerEvent(object source, ElapsedEventArgs e)
        {
            lock (this)
            {
                System.Timers.Timer tr = (System.Timers.Timer) source;
                if (ScrollDelta == 0)
                {
                    if (tr != null) tr.Enabled = false;
                    return;
                }
                DropListBox.Invoke(new Action(DoScroll));
            }
        }

        private void DoScroll()
        {
            if (ScrollDelta == 0)
            {
                if (ScrollTimer != null) ScrollTimer.Enabled = false;
                return;
            }
            if (ScrollDelta < 0)
            {
                if (DropListBox.TopIndex == 0) return;
                DropListBox.TopIndex--;
            }
            else
            {
                int visibleItems = DropListBox.ClientRectangle.Height / DropListBox.ItemHeight;
                if (DropListBox.Items.Count - DropListBox.TopIndex <= visibleItems) return;
                DropListBox.TopIndex++;

            }
        }

        void DoScrollKeepChannelsListBox(int scrollDelta)
        {
            lock (this)
            {
                if (scrollDelta == 0)
                {
                    ScrollDelta = 0;
                    if (ScrollTimer != null) ScrollTimer.Enabled = false;
                    return;
                }
                if (ScrollTimer == null)
                {
                    ScrollTimer = new System.Timers.Timer(300);
                    ScrollTimer.AutoReset = true;
                    ScrollTimer.Elapsed += OnScrollTimerEvent;
                }
                ScrollDelta = scrollDelta;
                ScrollTimer.Enabled = true;
            }
        }

    }
}
