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
using System.Diagnostics;

namespace MazeScreenSaver
{
    public partial class ScreenSaverForm : Form
    {
        private Point m_MousePosition;
        private Maze m_Maze;
        private int m_TilesWide, m_TilesHigh;
        private int m_OffsetX, m_OffsetY;
        private Tile m_WallTile, m_FloorTile;
        private Timer m_RegenTimer;
        private int MAX_FLOORS = 30;
        private Tile[] m_FloorTiles;

        public ScreenSaverForm()
        {
            InitializeComponent();
            Log.m_LogOn = false;
            Log.Write("Screensaver Started");
            // Use double buffering to improve drawing performance
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            // Capture the mouse
            this.Capture = true;

            // Set the application to full screen mode and hide the mouse
            Cursor.Hide();
            int width = 0, height = 0;
            foreach(Screen screen in Screen.AllScreens)
            {
                width = Math.Max(width, screen.Bounds.Width + screen.Bounds.X);
                height = Math.Max(height, screen.Bounds.Height + screen.Bounds.Y);
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

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            try
            {
                m_TilesWide = Width / 32;
                m_TilesHigh = Height / 32;
                m_OffsetX = (Width - m_TilesWide * 32) / 2;
                m_OffsetY = (Height - m_TilesHigh * 32) / 2;

                m_WallTile = new Tile(Resources.wall1, 0, 0);
                m_FloorTile = new Tile((Bitmap)Resources.ResourceManager.GetObject("floor1"), 0, 0);

                m_FloorTiles = new Tile[MAX_FLOORS+1];

                for (int i = 1; i <= MAX_FLOORS; i++)
                {
                    m_FloorTiles[i] = new Tile((Bitmap)Resources.ResourceManager.GetObject("floor" + i.ToString()), 0, 0);
                }
            }
            catch (Exception error)
            {
                Log.Write("Exception encountered: " + error.Message);
                Application.Exit();
            }
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            Log.Write("MouseMove Event, Mouse Location:" + e.Location.ToString());
            if (Math.Sqrt(Math.Pow(m_MousePosition.X - e.X, 2) + Math.Pow(m_MousePosition.Y - e.Y, 2)) >= 3)
                Close();
        }

        private void ScreenSaverForm_KeyDown(object sender, KeyEventArgs e)
        {
            Log.Write("KeyDown Event");
            Close();
        }

        private void ScreenSaverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Write("Screensaver Closing");
            Cursor.Show();
        }

        private void ScreenSaverForm_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (m_Maze != null)
                {
                    Stopwatch timer = Stopwatch.StartNew();
                    for (int r = 0; r < m_TilesWide; r++)
                    {
                        for (int c = 0; c < m_TilesHigh; c++)
                        {
                            if (m_Maze[r, c] == -1)
                                m_WallTile.Draw(e.Graphics, r * 32 + m_OffsetX, c * 32 + m_OffsetY);
                            else
                            {
                                if (m_Maze[r, c] < MAX_FLOORS)
                                    m_FloorTiles[m_Maze[r, c]].Draw(e.Graphics, r * 32 + m_OffsetX, c * 32 + m_OffsetY);
                                else
                                    m_FloorTile.Draw(e.Graphics, r * 32 + m_OffsetX, c * 32 + m_OffsetY);
                            }
                        }
                    }
                    timer.Stop();
                    Log.Write("Redrawing maze took " + timer.ElapsedTicks.ToString() + " ticks.");
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
                m_Maze.FindFloorRegions();
            }
            catch (Exception error)
            {
                Log.Write("Exception encountered: " + error.Message);
                Application.Exit();
            }
        }

        void m_RegenTimer_Tick(object sender, EventArgs e)
        {
            Log.Write("RegenTimer Tick");
            this.Invalidate();
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            Log.Write("MouseClick Event");
            Close();
        }
    }
}
