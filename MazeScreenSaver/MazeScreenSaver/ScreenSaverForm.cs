﻿using System;
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
        private Tile m_StairsTile;
        private Tile m_PlayerTile;
        private Tile m_PlayerVisitedTrailTile;
        private Tile m_PlayerExlporedTrailTile;
        private Timer m_RegenTimer;
        private Timer m_MoveTimer;
        private int MAX_FLOORS = 30;
        private Tile[] m_FloorTiles;
        private Point stairsCoords;
        private Player p;
        private bool newMaze = true;
        private string registryKey = "HKEY_CURRENT_USER\\Software\\Dungeoneer";
        private bool can_log;
        private bool show_trails;
        private Logger Log;
        private int moves;
        private Font movesFont;
        private int moveInterval;
        private int nextMoveInterval;

        public ScreenSaverForm()
        {
            InitializeComponent();
            getSettings();
            Log = new Logger();
            Log.enableLogging(can_log);
            //Log.m_LogOn = false;
            Log.Write("Screensaver Started (" + DateTime.Now.ToString("T") + ")", 2);
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
            movesFont = new Font("Courier", 7);
            nextMoveInterval = moveInterval;
        }

        private bool getRegistrySetting(string valueName)
        {
            object value = Microsoft.Win32.Registry.GetValue(registryKey, valueName, null);
            if(value != null)
                return ((int)value == 1);
            else
                return false;
        }

        private int? getRegistryIntSetting(string valueName)
        {
            int? value = (int?)Microsoft.Win32.Registry.GetValue(registryKey, valueName, null);
            return value;
        }

        private void getSettings()
        {
            can_log = getRegistrySetting("enableLogging");
            show_trails = getRegistrySetting("enableTrails");
            int? interval = getRegistryIntSetting("speed");
            if (!interval.HasValue)
                moveInterval = 250;
            else
                moveInterval = (int)interval * 50;
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
                m_StairsTile = new Tile(Resources.stairs, 0, 0);
                Bitmap playerBMP = Resources.player;
                playerBMP.MakeTransparent(Color.Black);
                m_PlayerTile = new Tile(playerBMP, 0, 0);
                Bitmap playerTrailBMP = Resources.playerVisitedTrail;
                playerTrailBMP.MakeTransparent(Color.Black);
                m_PlayerVisitedTrailTile = new Tile(playerTrailBMP, 0, 0);
                Bitmap playerExploredTrailBMP = Resources.playerExploreTrail;
                playerExploredTrailBMP.MakeTransparent(Color.Black);
                m_PlayerExlporedTrailTile = new Tile(playerExploredTrailBMP, 0, 0);

                m_FloorTiles = new Tile[MAX_FLOORS+1];

                for (int i = 1; i <= MAX_FLOORS; i++)
                {
                    m_FloorTiles[i] = new Tile((Bitmap)Resources.ResourceManager.GetObject("floor" + i.ToString()), 0, 0);
                }
                Cursor.Hide();
            }
            catch (Exception error)
            {
                Log.Write("Exception encountered in Load: " + error.Message,2);
                Application.Exit();
            }
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            Log.Write("MouseMove Event, Mouse Location:" + e.Location.ToString() + " Current Mouse Location:" + m_MousePosition.ToString());
            if (Math.Sqrt(Math.Pow(m_MousePosition.X - e.X, 2) + Math.Pow(m_MousePosition.Y - e.Y, 2)) >= 3)
            {
                Log.Write("MouseMove was too large", 2);
                Close();
            }
        }

        private void ScreenSaverForm_KeyDown(object sender, KeyEventArgs e)
        {
            Log.Write("KeyDown Event",2);
            Close();
        }

        private void ScreenSaverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Write("Screensaver Closing (" + DateTime.Now.ToString("T") + ")",2);
            Cursor.Show();
        }

        private void ScreenSaverForm_Paint(object sender, PaintEventArgs e)
        {
            int tile = 0;
            try
            {
                if (m_Maze != null)
                {
                    Stopwatch timer = Stopwatch.StartNew();
                    bool playerOnStairs = (p.coord == stairsCoords);
                    for (int r = 0; r < m_TilesHigh; r++)
                    {
                        for (int c = 0; c < m_TilesWide; c++)
                        {
                            if (playerOnStairs || p.getKnownMaze(r, c) != -1)
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
                    }
                    m_StairsTile.Draw(e.Graphics, stairsCoords.X * 32 + m_OffsetX, stairsCoords.Y * 32 + m_OffsetY);
                    if (show_trails)
                    {
                        foreach (Point point in p.getVisitedCoords())
                        {
                            m_PlayerVisitedTrailTile.Draw(e.Graphics, point.X * 32 + m_OffsetX, point.Y * 32 + m_OffsetY);
                        }
                        foreach (Point point in p.getExploredCoords())
                        {
                            m_PlayerExlporedTrailTile.Draw(e.Graphics, point.X * 32 + m_OffsetX, point.Y * 32 + m_OffsetY);
                        }
                    }
                    m_PlayerTile.Draw(e.Graphics, p.coord.X * 32 + m_OffsetX, p.coord.Y * 32 + m_OffsetY);
                    moves += 1;
                    e.Graphics.DrawString(moves.ToString(), movesFont, Brushes.White, new Point(stairsCoords.Y * 32 + m_OffsetY, stairsCoords.X * 32 + m_OffsetX));

                    timer.Stop();
                    //Log.Write("Redrawing maze took " + timer.ElapsedTicks.ToString() + " ticks.");
                    if (playerOnStairs)
                        m_MoveTimer.Interval = moveInterval * 2;
                }
                else
                {
                    this.Invalidate();
                    m_RegenTimer = new Timer();

                    m_MoveTimer = new Timer();
                    m_MoveTimer.Tick += new EventHandler(m_MoveTimer_Tick);
                    m_MoveTimer.Interval = moveInterval ;
                    m_MoveTimer.Start();
                }

                if (newMaze)
                {
                    bool mazeGenComplete = false;
                    while(!mazeGenComplete)
                        mazeGenComplete = RegenMaze();
                    moves = 0;
                    m_MoveTimer.Interval = moveInterval;
                }
            }
            catch (Exception error)
            {
                Log.Write("Exception encountered in Paint: " + error.Message + error.StackTrace, 2);
                Application.Exit();
            }
        }

        private bool RegenMaze()
        {
            try
            {
                Log.Write("New maze");
                m_Maze = new Maze(m_TilesHigh, m_TilesWide);
                m_Maze.log = Log;
                m_Maze.generate();
                m_Maze.fixFloorType();
                do
                {
                    stairsCoords = m_Maze.RandomSquare();
                } while (stairsCoords.X == 0 || stairsCoords.Y == 0 || stairsCoords.X == m_TilesHigh - 1 || stairsCoords.Y == m_TilesWide - 1);
                p = new Player();
                p.log = Log;
                int tries;
                tries = 0;
                bool giveup = false;
                bool okCoord = false;
                Point startingCoord;
                do
                {
                    startingCoord = m_Maze.RandomSquare();
                    okCoord = true;
                    tries += 1;
                    if(startingCoord.X == 0 || startingCoord.Y == 0 || startingCoord.X == m_TilesHigh - 1 || startingCoord.Y == m_TilesWide - 1)
                        okCoord = false;
                    if(startingCoord == stairsCoords)
                        okCoord = false;
                    if (tries > 1000)
                    {
                        Log.Write("Stuck trying to find open square. (tries = " + tries.ToString() + ")", 2);
                        Log.Write("Stairs at: " + stairsCoords.ToString(), 2);
                        string squaresString = "Squares in this region:";
                        foreach (Point pt in m_Maze.floorTypeMembers[1])
                            squaresString += pt.ToString();
                        Log.Write(squaresString, 2);
                        Log.Write("Anyway, I'm giving up now.", 2);
                        giveup = true;
                        okCoord = true;
                    }
                } while (!okCoord);
                if (!giveup)
                {
                    p.stairsCoord = stairsCoords;
                    p.setMazeSize(m_TilesHigh, m_TilesWide);
                    p.setStartingCoord(startingCoord);
                    p.setAvailableMoves(m_Maze.GetAdjacentFloors(p.coord));
                    newMaze = false;

                    m_RegenTimer.Stop();
                    m_RegenTimer.Start();
                    Log.Write("New Maze created");
                    return true;
                }
                return false;
            }
            catch (Exception error)
            {
                Log.Write("Exception occurred in RegenMaze:" + error.Message + error.StackTrace, 2);
                Application.Exit();
                return false;
            }
        }

        void m_MoveTimer_Tick(object sender, EventArgs e)
        {
            if (p.coord == stairsCoords)
            {
                newMaze = true;
            }
            else
            {
                p.move();
                p.setAvailableMoves(m_Maze.GetAdjacentFloors(p.coord));
            }
            this.Invalidate();
        }

        void m_RegenTimer_Tick(object sender, EventArgs e)
        {
            //Log.Write("RegenTimer Tick");
            //newMaze = true;
            this.Invalidate();
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            Log.Write("MouseClick Event", 2);
            Close();
        }
    }
}
