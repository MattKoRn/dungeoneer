using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MazeScreenSaver
{
    public partial class PreviewDisplayForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, ref RECT rect);
        [DllImport("user32.DLL", EntryPoint = "IsWindowVisible")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        public IntPtr m_parentHwnd;
        private Rectangle m_clientRect;
        private Timer m_timer;

        public PreviewDisplayForm()
        {
            InitializeComponent();
        }

        private struct RECT
        {
            public int left, top, right, bottom;
        }

        private void PreviewDisplay_Load(object sender, EventArgs e)
        {
            RECT rect = new RECT();
            GetClientRect(m_parentHwnd, ref rect);
            m_clientRect = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            m_timer = new Timer();
            m_timer.Interval = 500;
            m_timer.Tick += timer_tick;
            m_timer.Start();
            this.Visible = false;
            Draw();
        }

        private void Draw()
        {
            if (!IsWindowVisible(m_parentHwnd))
                Application.Exit();

            Graphics g = Graphics.FromHwnd(m_parentHwnd);
            g.Clear(Color.Blue);

        }

        private void timer_tick(object Sender, EventArgs e)
        {
            Draw();
        }
    }
}
