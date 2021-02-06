using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace xmltv
{
    public class MyTextBox : TextBox
    {
        private ContextMenu m_ContextMenu;

        private bool m_DrawBorder = true;
        private Color m_BorderColor = SystemColors.ControlDarkDark;

        public MyTextBox()
        {
            //SetStyle(ControlStyles.DoubleBuffer, true);
            base.BorderStyle = BorderStyle.FixedSingle;

            m_ContextMenu = new ContextMenu();
            m_ContextMenu.Popup += ContextMenu_PopUp;
            var miUndo = new MenuItem("Undo", Undo_Click);
            var miCut = new MenuItem("Cut", Cut_Click);
            var miCopy = new MenuItem("Copy", Copy_Click);
            var miPaste = new MenuItem("Paste", Paste_Click);
            var miSelectAll = new MenuItem("Select All", SelectAll_Click);
            miUndo.Name = "Undo";
            miCut.Name = "Cut";
            miCopy.Name = "Copy";
            miPaste.Name = "Paste";
            miSelectAll.Name = "SelectAll";
            m_ContextMenu.MenuItems.Add(miUndo);
            m_ContextMenu.MenuItems.Add("-");
            m_ContextMenu.MenuItems.Add(miCut);
            m_ContextMenu.MenuItems.Add(miCopy);
            m_ContextMenu.MenuItems.Add(miPaste);
            m_ContextMenu.MenuItems.Add("-");
            m_ContextMenu.MenuItems.Add(miSelectAll);
            this.ContextMenu = m_ContextMenu;
        }

        private void ContextMenu_PopUp(object sender, EventArgs e)
        {
            m_ContextMenu.MenuItems["Undo"].Enabled = this.CanUndo;

            bool b = this.SelectedText.Length > 0;
            m_ContextMenu.MenuItems["Cut"].Enabled = b;
            m_ContextMenu.MenuItems["Copy"].Enabled = b;

            m_ContextMenu.MenuItems["Paste"].Enabled = Clipboard.ContainsText();

            m_ContextMenu.MenuItems["SelectAll"].Enabled = this.Text.Length > 0;
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            this.Undo();
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            this.Cut();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            this.Copy();
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            this.Paste();
        }

        private void SelectAll_Click(object sender, EventArgs e)
        {
            this.SelectAll();
        }


        private const int WM_ERASEBKGND = 0x14;
        private const int WM_PAINT = 0xF;
        private const int WM_NC_PAINT = 0x85;


        [DllImport("user32.dll", CharSet = CharSet.Ansi, EntryPoint = "SendMessageA", ExactSpelling = true,
            SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);


        [Category("Behavior")]
        [DefaultValue(false)]
        public bool IsDate { get; set; } = false;


        [Category("Appearance")]
        [DefaultValue(true)]
        public bool DrawBorder
        {
            get { return m_DrawBorder; }
            set
            {
                m_DrawBorder = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get { return m_BorderColor; }
            set
            {
                m_BorderColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(BorderStyle.FixedSingle)]
        public new BorderStyle BorderStyle
        {
            get { return base.BorderStyle; }
            set {  }
        }

        protected override void WndProc(ref Message m)
        {
            IntPtr hDC = IntPtr.Zero;
            Graphics gdc = null;
            switch (m.Msg)
            {
                case WM_PAINT:
                    base.WndProc(ref m);
                    if (!DrawBorder) break;
                    // flatten the border area again
                    //hDC = GetWindowDC(this.Handle);
                    //gdc = Graphics.FromHdc(hDC);
                    using (gdc = Graphics.FromHwnd(Handle))
                    {
                        //Pen p = new Pen((this.Enabled ? BackColor : SystemColors.Control), 2);
                        //gdc.DrawRectangle(p, new Rectangle(2, 2, this.Width - 3, this.Height - 3));
                        PaintFlatControlBorder(this, gdc);
                        //ReleaseDC(m.HWnd, hDC);
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        public static Color Dark(Color baseColor)
        {
            return new HLSColor(baseColor).Darker(0.5f);
        }

        private void PaintFlatControlBorder(Control ctrl, Graphics g)
        {
            if (!DrawBorder) return;
            if (BorderStyle != BorderStyle.FixedSingle) return;

            Rectangle rect;

            rect = new Rectangle(0, 0, ctrl.Width, ctrl.Height);

            if (ctrl.Enabled)
                ControlPaint.DrawBorder(g, rect, m_BorderColor, ButtonBorderStyle.Solid);
            else
                ControlPaint.DrawBorder(g, rect, Dark(m_BorderColor), ButtonBorderStyle.Solid);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                SendKeys.Send("\t");
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.SelectAll();
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);
            if (e.Cancel) return;
            if (IsDate)
            {
                DateTime dt;
                if (string.IsNullOrEmpty(this.Text)) return;
                if (!Utils.StringToDate(this.Text, out dt))
                {
                    e.Cancel = true;
                }
                else
                {
                    var tx = Utils.DateToString(dt);
                    if (this.Text != tx)
                        this.Text = tx;
                }
            }
            
        }


    }
}
