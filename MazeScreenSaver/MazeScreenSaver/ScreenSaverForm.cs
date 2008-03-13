using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MazeScreenSaver.Properties;

namespace MazeScreenSaver
{
    public partial class ScreenSaverForm : Form
    {
        private StreamWriter m_logFile = null;
        private Point m_MousePosition;
        private Maze m_Maze;
        private int m_TilesWide, m_TilesHigh;
        private int m_OffsetX, m_OffsetY;
        private Tile m_WallTile, m_FloorTile;
        private Timer m_RegenTimer;

        public ScreenSaverForm()
        {
            InitializeComponent();
//            m_logFile = new StreamWriter("MazeScreenSaverLog.txt");
//            m_logFile.WriteLine(DateTime.Now.ToString() + ": Screensaver Started");
            // Use double buffering to improve drawing performance
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            // Capture the mouse
            this.Capture = true;

            // Set the application to full screen mode and hide the mouse
            Cursor.Hide();
            int width = 0, height = 0;
            foreach(Screen screen in Screen.AllScreens)
            {
                width += screen.Bounds.Width;
                height += screen.Bounds.Height;
            }
            Size = new Size(width, height);            
            FormBorderStyle = FormBorderStyle.None;
            //WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            ShowInTaskbar = false;
            DoubleBuffered = true;
            BackgroundImageLayout = ImageLayout.Stretch;
            m_MousePosition = Cursor.Position;
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_logFile != null)
                m_logFile.WriteLine(DateTime.Now.ToString() + ": MouseMove Event, Mouse Location:" + e.Location.ToString());
            if (Math.Sqrt(Math.Pow(m_MousePosition.X - e.X, 2) + Math.Pow(m_MousePosition.Y - e.Y, 2)) >= 3)
                Close();
        }

        private void ScreenSaverForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_logFile != null)
                m_logFile.WriteLine(DateTime.Now.ToString() + ": KeyDown Event");
            Close();
        }

        private void ScreenSaverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_logFile != null)
            {
                m_logFile.WriteLine(DateTime.Now.ToString() + ": Screensaver Closing");
                m_logFile.Close();
            }
            Cursor.Show();
        }

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            try
            {
                m_TilesWide = Width / 32;
                m_TilesHigh = Height / 32;
                m_OffsetX = (Width - m_TilesWide * 32)/2;
                m_OffsetY = (Height - m_TilesHigh * 32)/2;

                m_WallTile = new Tile(Resources.mazeTiles, 0, 0);
                m_FloorTile = new Tile(Resources.mazeFeatures, 2, 0);
            }
            catch (Exception error)
            {
                m_logFile.Write("Exception encountered: " + error.Message);
                m_logFile.Close();
                Application.Exit();
            }
        }

        private void ScreenSaverForm_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (m_Maze != null)
                {
                    for (int r = 0; r < m_TilesWide; r++)
                    {
                        for (int c = 0; c < m_TilesHigh; c++)
                        {
                            if (m_Maze[r, c])
                                m_WallTile.Draw(e.Graphics, r * 32 + m_OffsetX, c * 32 + m_OffsetY);
                            else
                                m_FloorTile.Draw(e.Graphics, r * 32 + m_OffsetX, c * 32 + m_OffsetY);
                        }
                    }
                }
                else
                {
                    this.Invalidate();
                    m_RegenTimer = new Timer();
                    m_RegenTimer.Tick += new EventHandler(m_RegenTimer_Tick);
                    m_RegenTimer.Interval = 1000 * 15;
                    m_RegenTimer.Start();
                }
                m_Maze = new Maze(m_TilesWide, m_TilesHigh);
                m_Maze.generate();
            }
            catch (Exception error)
            {
                m_logFile.Write("Exception encountered: " + error.Message);
                m_logFile.Close();
                Application.Exit();
            }
        }

        void m_RegenTimer_Tick(object sender, EventArgs e)
        {
            if(m_logFile != null)
                m_logFile.Write(DateTime.Now.ToString() + ": RegenTimer_Tick");
            this.Invalidate();
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_logFile != null)
                m_logFile.WriteLine(DateTime.Now.ToString() + ": MouseClick Event");
            Close();
        }
    }
}
