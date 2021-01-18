using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace xmltv
{
    public class MyListView : ListView
    {
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            try
            {
                base.ScaleControl(factor, specified);
                float scale = factor.Width;
                for(int i = 0; i < this.Columns.Count; i++)
                {
                    var col = this.Columns[i];
                    col.Width = (int)Math.Round((float)col.Width * scale);
                }
            }
            finally
            {
            }
        }
    }
}
