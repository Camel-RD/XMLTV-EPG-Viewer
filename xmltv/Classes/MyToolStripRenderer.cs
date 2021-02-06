using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace xmltv
{
    public class MyToolStripRenderer : ToolStripProfessionalRenderer
    {

        public void SetColorTheme(MyColorTheme thene)
        {
            MyColorTable mc = this.ColorTable as MyColorTable;
            mc.MyColorTheme = thene;
        }

        public MyToolStripRenderer()
            : base(new MyColorTable())
        {
            SetUp();
        }

        public MyToolStripRenderer(MyColorTheme theme)
            : base(new MyColorTable(theme))
        {
            SetUp();
        }

        private void SetUp()
        {
            RenderItemCheck += MyToolStripRenderer_RenderItemCheck;
        }

        private void ScalePoint(ref Point p, float fx, float fy, int dx = 0, int dy = 0)
        {
            p.X = (int)((float)p.X * fx) + dx;
            p.Y = (int)((float)p.Y * fy) + dy;
        }

        private void MyToolStripRenderer_RenderItemCheck(object sender, ToolStripItemImageRenderEventArgs e)
        {
            Rectangle rc = new Rectangle(e.ImageRectangle.Left - 2, 1, e.ImageRectangle.Width + 4, e.Item.Height - 2);
            var ct = ColorTable as MyColorTable;
            var pen = new Pen(ct.MyColorTheme.ControlTextColor, 2f);
            var p1 = new Point(5, 6);
            var p2 = new Point(7, 9);
            var p3 = new Point(11, 4);
            float fx = rc.Width / 16.0f;
            float fy = rc.Height / 16.0f;
            ScalePoint(ref p1, fx, fy, rc.Left, rc.Top);
            ScalePoint(ref p2, fx, fy, rc.Left, rc.Top);
            ScalePoint(ref p3, fx, fy, rc.Left, rc.Top);
            var ps = new[] { p1, p2, p3 };
            e.Graphics.DrawLines(pen, ps);
            pen.Dispose();
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            var mc = ColorTable as MyColorTable;
            if(mc != null && mc.MyColorTheme.UsingSystemColors)
            {
                base.OnRenderSeparator(e);
                return;
            }
            ToolStripSeparator item = e.Item as ToolStripSeparator;
            if (e.Vertical || item == null)
                base.OnRenderSeparator(e);
            else
            {
                ToolStripDropDownMenu dropDownMenu = item.GetCurrentParent() as ToolStripDropDownMenu;
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                Brush br = new SolidBrush(item.BackColor);
                Pen pen = new Pen(item.ForeColor);
                e.Graphics.FillRectangle(br, bounds);
                bounds.X += dropDownMenu.Padding.Left - 2;
                bounds.Width = dropDownMenu.Width - bounds.X;
                int startY = bounds.Height / 2;
                e.Graphics.FillRectangle(br, bounds);
                e.Graphics.DrawLine(pen, bounds.Left, startY, bounds.Right-1, startY);
                pen.Dispose();
                br.Dispose();
            }
        }
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {

            ToolStripItem item = e.Item;

            if (item is ToolStripDropDownItem)
            {
                e.ArrowColor = (item.Enabled) ? item.ForeColor : SystemColors.ControlDark;
            }
            base.OnRenderArrow(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;


            if (toolStrip is ToolStripDropDown)
            {
                RenderToolStripDropDownBorder(e);
                return;
            }

            base.OnRenderToolStripBorder(e);
        }

        private void RenderToolStripDropDownBorder(ToolStripRenderEventArgs e)
        {
            ToolStripDropDown toolStripDropDown = e.ToolStrip as ToolStripDropDown;
            Graphics g = e.Graphics;

            if (toolStripDropDown != null)
            {
                Rectangle bounds = new Rectangle(Point.Empty, toolStripDropDown.Size);

                using (Pen p = new Pen(toolStripDropDown.ForeColor))
                {
                    g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }

                if (!(toolStripDropDown is ToolStripOverflow))
                {
                    // make the neck connected.
                    using (Brush b = new SolidBrush(ColorTable.ToolStripDropDownBackground))
                    {
                        g.FillRectangle(b, e.ConnectedArea);
                    }
                }
            }
        }
        private void RenderPressedButtonFill(Graphics g, Rectangle bounds)
        {

            if ((bounds.Width == 0) || (bounds.Height == 0))
            {
                return;  // can't new up a linear gradient brush with no dimension.
            }
            if (!UseSystemColors)
            {
                using (Brush b = new LinearGradientBrush(bounds, ColorTable.ButtonPressedGradientBegin, ColorTable.ButtonPressedGradientEnd, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(b, bounds);
                }
            }
            else
            {

                Color fillColor = ColorTable.ButtonPressedHighlight;
                using (Brush b = new SolidBrush(fillColor))
                {
                    g.FillRectangle(b, bounds);
                }
            }
        }

        private void RenderItemInternal(ToolStripItemRenderEventArgs e, bool useHotBorder)
        {
            Graphics g = e.Graphics;
            ToolStripItem item = e.Item;
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);
            bool drawHotBorder = false;

            Rectangle fillRect = (item.Selected) ? item.ContentRectangle : bounds;

            if (item.BackgroundImage != null)
            {
                MyControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
            }

            if (item.Pressed)
            {
                RenderPressedButtonFill(g, bounds);
                drawHotBorder = useHotBorder;
            }
            else if (item.Selected)
            {
                RenderSelectedButtonFill(g, bounds);
                drawHotBorder = useHotBorder;
            }
            else if (item.Owner != null && item.BackColor != item.Owner.BackColor)
            {
                using (Brush b = new SolidBrush(item.BackColor))
                {
                    g.FillRectangle(b, bounds);
                }
            }

            if (drawHotBorder)
            {
                using (Pen p = new Pen(ColorTable.ButtonSelectedBorder))
                {
                    g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
            }
        }

        private void RenderCheckedButtonFill(Graphics g, Rectangle bounds)
        {
            if ((bounds.Width == 0) || (bounds.Height == 0))
            {
                return;  // can't new up a linear gradient brush with no dimension.
            }

            if (!UseSystemColors)
            {
                using (Brush b = new LinearGradientBrush(bounds, ColorTable.ButtonCheckedGradientBegin, ColorTable.ButtonCheckedGradientEnd, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(b, bounds);
                }
            }
            else
            {

                Color fillColor = ColorTable.ButtonCheckedHighlight;

                using (Brush b = new SolidBrush(fillColor))
                {
                    g.FillRectangle(b, bounds);

                }
            }
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripButton item = e.Item as ToolStripButton;
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);

            if (item.CheckState == CheckState.Unchecked)
            {
                RenderItemInternal(e, /*useHotBorder = */ true);
            }
            else
            {
                Rectangle fillRect = (item.Selected) ? item.ContentRectangle : bounds;

                if (item.BackgroundImage != null)
                {
                    MyControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
                }

                if (UseSystemColors)
                {
                    if (item.Selected)
                    {
                        RenderPressedButtonFill(g, bounds);
                    }
                    else
                    {
                        RenderCheckedButtonFill(g, bounds);
                    }

                    using (Pen p = new Pen(ColorTable.ButtonSelectedBorder))
                    {
                        g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }

                }
                else
                {
                    if (item.Selected)
                    {
                        RenderPressedButtonFill(g, bounds);
                    }
                    else
                    {
                        RenderCheckedButtonFill(g, bounds);
                    }
                    using (Pen p = new Pen(ColorTable.ButtonSelectedBorder))
                    {
                        g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }

                }
            }

        }

        private bool UseSystemColors { get { return false; } }

        private void RenderSelectedButtonFill(Graphics g, Rectangle bounds)
        {
            if ((bounds.Width == 0) || (bounds.Height == 0))
            {
                return;  // can't new up a linear gradient brush with no dimension.
            }

            if (!UseSystemColors)
            {
                using (Brush b = new LinearGradientBrush(bounds, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(b, bounds);
                }
            }
            else
            {
                Color fillColor = ColorTable.ButtonSelectedHighlight;
                using (Brush b = new SolidBrush(fillColor))
                {
                    g.FillRectangle(b, bounds);
                }
            }
        }

        private void RenderSelectedButtonFill(Graphics g, Rectangle bounds, Color fillColor)
        {
            if ((bounds.Width == 0) || (bounds.Height == 0))
            {
                return;  // can't new up a linear gradient brush with no dimension.
            }

            using (Brush b = new SolidBrush(fillColor))
            {
                g.FillRectangle(b, bounds);
            }
        }

        private void RenderPressedGradient(Graphics g, Rectangle bounds)
        {
            if ((bounds.Width == 0) || (bounds.Height == 0))
            {
                return;  // can't new up a linear gradient brush with no dimension.
            }

            // Paints a horizontal gradient similar to the image margin.
            using (Brush b = new LinearGradientBrush(bounds, ColorTable.MenuItemPressedGradientBegin, ColorTable.MenuItemPressedGradientEnd, LinearGradientMode.Vertical))
            {
                g.FillRectangle(b, bounds);
            }

            // draw a box around the gradient
            using (Pen p = new Pen(ColorTable.MenuBorder))
            {
                g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }

        private Padding dropDownMenuItemPaintPadding = new Padding(2, 0, 1, 0);

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripItem item = e.Item;
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);

            if ((bounds.Width == 0) || (bounds.Height == 0))
            {
                return;  // can't new up a linear gradient brush with no dimension.
            }
            
            //ControlBoxMenuItem
            bool itemIsControlBoxMenuItem = item.GetType().Name == "ControlBoxMenuItem";
            if (itemIsControlBoxMenuItem)
            {
                if (item.BackColor != SystemColors.Control)
                {
                    item.BackColor = SystemColors.Control;
                }
            }

            if (item.GetType().Name == "MdiControlStrip")
            {
                return; // no highlights are painted behind a system menu item
            }


            if (item.IsOnDropDown)
            {

                bounds = MyControlPaint.DeflateRect(bounds, dropDownMenuItemPaintPadding);

                if (item.Selected)
                {
                    Color borderColor = ColorTable.MenuItemBorder;
                    if (item.Enabled)
                    {
                        if (UseSystemColors)
                        {
                            borderColor = SystemColors.Highlight;
                            RenderSelectedButtonFill(g, bounds);
                        }
                        else
                        {
                            using (Brush b = new SolidBrush(ColorTable.MenuItemSelected))
                            {
                                g.FillRectangle(b, bounds);
                            }
                        }
                    }
                    // draw selection border - always drawn regardless of Enabled.
                    using (Pen p = new Pen(borderColor))
                    {
                        g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }
                }
                else
                {
                    Rectangle fillRect = bounds;

                    
                    if (item.BackgroundImage != null)
                    {
                        MyControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
                    }
                    else
                    if (item.Owner != null && item.BackColor != item.Owner.BackColor)
                    {
                        using (Brush b = new SolidBrush(item.BackColor))
                        {
                            g.FillRectangle(b, fillRect);
                        }
                    }
                }
            }
            else
            {

                if (item.Pressed)
                {
                    // Toplevel toolstrip rendering
                    RenderPressedGradient(g, bounds);

                }
                else if (item.Selected)
                {
                    //Hot, Pressed behavior 
                    // Fill with orange
                    Color borderColor = ColorTable.MenuItemBorder;

                    if (item.Enabled)
                    {

                        if (itemIsControlBoxMenuItem)
                        {
                            borderColor = SystemColors.Highlight;
                            RenderSelectedButtonFill(g, bounds, SystemColors.ButtonHighlight);
                        }
                        else if (UseSystemColors)
                        {
                            borderColor = SystemColors.Highlight;
                            RenderSelectedButtonFill(g, bounds);
                        }
                        else
                        {
                            using (Brush b = new LinearGradientBrush(bounds, ColorTable.MenuItemSelectedGradientBegin, ColorTable.MenuItemSelectedGradientEnd, LinearGradientMode.Vertical))
                            {
                                g.FillRectangle(b, bounds);
                            }
                        }
                    }

                    // draw selection border - always drawn regardless of Enabled.
                    using (Pen p = new Pen(borderColor))
                    {
                        g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }
                }
                else
                {
                    Rectangle fillRect = bounds;

                    
                    if (item.BackgroundImage != null)
                    {
                        MyControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
                    }
                    else 
                    if (item.Owner != null && item.BackColor != item.Owner.BackColor)
                    {
                        using (Brush b = new SolidBrush(item.BackColor))
                        {
                            g.FillRectangle(b, fillRect);
                        }
                    }
                }
            }

        }

    }


    public class MyControlPaint
    {
        public static Rectangle DeflateRect(Rectangle rect, Padding padding)
        {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;
            return rect;
        }

        public static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage,
            ImageLayout imageLayout)
        {

            Rectangle result = bounds;

            if (backgroundImage != null)
            {
                switch (imageLayout)
                {
                    case ImageLayout.Stretch:
                        result.Size = bounds.Size;
                        break;

                    case ImageLayout.None:
                        result.Size = backgroundImage.Size;
                        break;

                    case ImageLayout.Center:
                        result.Size = backgroundImage.Size;
                        Size szCtl = bounds.Size;

                        if (szCtl.Width > result.Width)
                        {
                            result.X = (szCtl.Width - result.Width)/2;
                        }
                        if (szCtl.Height > result.Height)
                        {
                            result.Y = (szCtl.Height - result.Height)/2;
                        }
                        break;

                    case ImageLayout.Zoom:
                        Size imageSize = backgroundImage.Size;
                        float xRatio = (float) bounds.Width/(float) imageSize.Width;
                        float yRatio = (float) bounds.Height/(float) imageSize.Height;
                        if (xRatio < yRatio)
                        {
                            //width should fill the entire bounds.
                            result.Width = bounds.Width;
                            // preserve the aspect ratio by multiplying the xRatio by the height
                            // adding .5 to round to the nearest pixel
                            result.Height = (int) ((imageSize.Height*xRatio) + .5);
                            if (bounds.Y >= 0)
                            {
                                result.Y = (bounds.Height - result.Height)/2;
                            }
                        }
                        else
                        {
                            // width should fill the entire bounds
                            result.Height = bounds.Height;
                            // preserve the aspect ratio by multiplying the xRatio by the height
                            // adding .5 to round to the nearest pixel
                            result.Width = (int) ((imageSize.Width*yRatio) + .5);
                            if (bounds.X >= 0)
                            {
                                result.X = (bounds.Width - result.Width)/2;
                            }
                        }

                        break;
                }
            }
            return result;
        }

        public static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor,
            ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect)
        {
            DrawBackgroundImage(g, backgroundImage, backColor, backgroundImageLayout, bounds, clipRect, Point.Empty,
                RightToLeft.No);
        }

        public static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor,
            ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset)
        {
            DrawBackgroundImage(g, backgroundImage, backColor, backgroundImageLayout, bounds, clipRect, scrollOffset,
                RightToLeft.No);
        }

        public static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor,
            ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset,
            RightToLeft rightToLeft)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (backgroundImageLayout == ImageLayout.Tile)
            {
                // tile

                using (TextureBrush textureBrush = new TextureBrush(backgroundImage, WrapMode.Tile))
                {
                    // Make sure the brush origin matches the display rectangle, not the client rectangle,
                    // so the background image scrolls on AutoScroll forms.
                    if (scrollOffset != Point.Empty)
                    {
                        Matrix transform = textureBrush.Transform;
                        transform.Translate(scrollOffset.X, scrollOffset.Y);
                        textureBrush.Transform = transform;
                    }
                    g.FillRectangle(textureBrush, clipRect);
                }
            }

            else
            {
                // Center, Stretch, Zoom

                Rectangle imageRectangle = CalculateBackgroundImageRectangle(bounds, backgroundImage,
                    backgroundImageLayout);

                //flip the coordinates only if we don't do any layout, since otherwise the image should be at the center of the
                //displayRectangle anyway.

                if (rightToLeft == RightToLeft.Yes && backgroundImageLayout == ImageLayout.None)
                {
                    imageRectangle.X += clipRect.Width - imageRectangle.Width;
                }

                // We fill the entire cliprect with the backcolor in case the image is transparent.
                // Also, if gdi+ can't quite fill the rect with the image, they will interpolate the remaining
                // pixels, and make them semi-transparent. This is another reason why we need to fill the entire rect.
                // If we didn't where ever the image was transparent, we would get garbage. VS Whidbey #504388
                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, clipRect);
                }

                if (!clipRect.Contains(imageRectangle))
                {
                    if (backgroundImageLayout == ImageLayout.Stretch || backgroundImageLayout == ImageLayout.Zoom)
                    {
                        imageRectangle.Intersect(clipRect);
                        g.DrawImage(backgroundImage, imageRectangle);
                    }
                    else if (backgroundImageLayout == ImageLayout.None)
                    {
                        imageRectangle.Offset(clipRect.Location);
                        Rectangle imageRect = imageRectangle;
                        imageRect.Intersect(clipRect);
                        Rectangle partOfImageToDraw = new Rectangle(Point.Empty, imageRect.Size);
                        g.DrawImage(backgroundImage, imageRect, partOfImageToDraw.X, partOfImageToDraw.Y,
                            partOfImageToDraw.Width,
                            partOfImageToDraw.Height, GraphicsUnit.Pixel);
                    }
                    else
                    {
                        Rectangle imageRect = imageRectangle;
                        imageRect.Intersect(clipRect);
                        Rectangle partOfImageToDraw =
                            new Rectangle(new Point(imageRect.X - imageRectangle.X, imageRect.Y - imageRectangle.Y)
                                , imageRect.Size);

                        g.DrawImage(backgroundImage, imageRect, partOfImageToDraw.X, partOfImageToDraw.Y,
                            partOfImageToDraw.Width,
                            partOfImageToDraw.Height, GraphicsUnit.Pixel);
                    }
                }
                else
                {
                    ImageAttributes imageAttrib = new ImageAttributes();
                    imageAttrib.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(backgroundImage, imageRectangle, 0, 0, backgroundImage.Width, backgroundImage.Height,
                        GraphicsUnit.Pixel, imageAttrib);
                    imageAttrib.Dispose();

                }

            }

        }
    }


    public class MyColorTable : ProfessionalColorTable
    {

        public MyColorTheme MyColorTheme { get; set; }

        public MyColorTable(MyColorTheme theme)
        {
            MyColorTheme = theme;
            UseSystemColors = false;
        }

        public MyColorTable()
        {
            MyColorTheme = ColorThemeHelper.ColorTheme_System;
            UseSystemColors = false;
        }

        public override Color MenuItemSelected => MyColorTheme.MenuHighlight;
        public override Color MenuItemSelectedGradientBegin => MyColorTheme.MenuHighlight;
        public override Color MenuItemSelectedGradientEnd => MyColorTheme.MenuHighlight;
        public override Color MenuItemPressedGradientBegin => MyColorTheme.MenuHighlight;
        public override Color MenuItemPressedGradientEnd => MyColorTheme.MenuHighlight;
        public override Color ButtonCheckedHighlight => MyColorTheme.MenuHighlight;
        public override Color ButtonSelectedHighlight => MyColorTheme.MenuHighlight;
        public override Color ButtonSelectedBorder => MyColorTheme.ControlColorLight;
        public override Color ButtonSelectedHighlightBorder => MyColorTheme.MenuHighlight;
        public override Color ButtonSelectedGradientBegin => MyColorTheme.MenuHighlight;
        public override Color ButtonSelectedGradientEnd => MyColorTheme.MenuHighlight;
        public override Color ToolStripBorder => MyColorTheme.ControlColorLight;
        public override Color CheckSelectedBackground => MyColorTheme.MenuHighlight;
        public override Color CheckBackground => MyColorTheme.MenuHighlight;
        public override Color CheckPressedBackground => MyColorTheme.MenuHighlight;

        public override Color ImageMarginGradientBegin => MyColorTheme.ControlColor;
        public override Color ImageMarginGradientMiddle => MyColorTheme.ControlColor;
        public override Color ImageMarginGradientEnd => MyColorTheme.ControlColor;
        public override Color RaftingContainerGradientBegin => MyColorTheme.ControlColor;
        public override Color RaftingContainerGradientEnd => MyColorTheme.ControlColor;
        public override Color ToolStripDropDownBackground => MyColorTheme.ControlColor;

        public override Color ToolStripGradientBegin => MyColorTheme.ControlColor;
        public override Color ToolStripGradientMiddle => MyColorTheme.ControlColor;
        public override Color ToolStripGradientEnd => MyColorTheme.ControlColor;
        public override Color ToolStripContentPanelGradientBegin => MyColorTheme.ControlColor;
        public override Color ToolStripContentPanelGradientEnd => MyColorTheme.ControlColor;
        public override Color ToolStripPanelGradientBegin => MyColorTheme.ControlColor;
        public override Color ToolStripPanelGradientEnd => MyColorTheme.ControlColor;

    }

}