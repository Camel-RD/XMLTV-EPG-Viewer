using System.Windows.Forms;
using System.Drawing;

namespace xmltv
{
    public class ColorListViewHeader
    {
        //List view header formatters
        public static void colorListViewHeader(ListView list)
        {
            list.OwnerDraw = true;
            list.DrawColumnHeader +=
                new DrawListViewColumnHeaderEventHandler
                (
                    (sender, e) => headerDraw(sender, e, list)
                );
            list.DrawItem += new DrawListViewItemEventHandler(bodyDraw);
        }

        private static void headerDraw(object sender, DrawListViewColumnHeaderEventArgs e, ListView list)
        {
            using (SolidBrush backBrush = new SolidBrush(list.FindForm().BackColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }
            using (Pen forePen = new Pen(list.FindForm().ForeColor))
            {
                var b = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1); ;
                e.Graphics.DrawRectangle(forePen, b);
            }
            using (SolidBrush foreBrush = new SolidBrush(list.FindForm().ForeColor))
            {
                var b = e.Bounds;
                b.Inflate(-1, -1);
                e.Graphics.DrawString(e.Header.Text, e.Font, foreBrush, b);
            }
        }

        private static void bodyDraw(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}