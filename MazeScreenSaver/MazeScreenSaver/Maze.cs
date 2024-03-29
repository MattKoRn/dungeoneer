﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MazeScreenSaver
{
    class FOVLine
    {
        private Point m_Near, m_Far;

        public FOVLine()
        {
            m_Near = new Point(0,0);
            m_Far = new Point(0,0);
        }

        public FOVLine(Point near, Point far)
        {
            m_Near = near;
            m_Far = far;
        }

        public Point Near
        {
            get { return m_Near; }
            set { m_Near = value; }
        }

        public Point Far
        {
            get { return m_Far; }
            set { m_Far = value; }
        }

        public bool isBelow(Point pt)
        {
            return relativeSlope(pt) > 0;
        }

        public bool isBelowOrContains(Point pt)
        {
            return relativeSlope(pt) >= 0;
        }

        public bool isAbove(Point pt)
        {
            return relativeSlope(pt) < 0;
        }

        public bool isAboveOrContains(Point pt)
        {
            return relativeSlope(pt) <= 0;
        }

        public bool contains(Point pt)
        {
            return relativeSlope(pt) == 0;
        }

        /// <summary>
        /// Returns slope relative to the supplied point
        /// Negative: line is above point. Positive: line is below point. Zero: line contains point.
        /// </summary>
        private int relativeSlope(Point pt)
        {
            return (m_Far.Y - m_Near.Y) * (m_Far.X - pt.X) - (m_Far.Y - pt.Y) * (m_Far.X - m_Near.X);
        }
    }

    class FOVBump
    {
        private Point m_Bump;

        public Point Bump
        {
            get { return m_Bump; }
            set { m_Bump = value; }
        }

        private FOVBump m_Parent;

        public FOVBump Parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        public FOVBump()
        {
            m_Bump = Point.Empty;
            m_Parent = null;
        }
    }

    class FOVField
    {
        public FOVLine steep, shallow;
        public FOVBump steepBump, shallowBump;
    }

    class Maze
    {
        int[,] m_maze;
        int m_rows, m_cols;
        List<int> floorTypeCount;
        public List<List<Point>> floorTypeMembers;
        Random rand;
        public Logger log;

        public Maze(int rows, int cols)
        {
            if (rows < 4)
                throw new ArgumentException("Maze Rows must be at least 4");

            if (cols < 4)
                throw new ArgumentException("Maze Cols must be at least 4");

            m_rows = rows;
            m_cols = cols;
            rand = new Random();

            m_maze = new int[rows, cols];
        }

        public void log_maze()
        {
            string logstring;
            for (int r = 0; r < m_rows; r++)
            {
                logstring = "";
                for (int c = 0; c < m_cols; c++)
                {
                    if (m_maze[r, c] == -1)
                        logstring += "*";
                    else
                        logstring += m_maze[r, c].ToString();
                }
                log.Write(logstring);
            }
        }

        public int this[int row, int col]
        {
            get
            {                
                if(row >= m_rows
                    || row < 0)
                    throw new ArgumentOutOfRangeException("row");
                if (col >= m_cols
                    || col < 0)
                    throw new ArgumentOutOfRangeException("col");
                return m_maze[row, col];
            }
        }

        public int this[Point coord]
        {
            get
            {
                return this[coord.X, coord.Y];
            }
            set
            {
                m_maze[coord.X, coord.Y] = value;
            }
        }

        public void generate()
        {
            for (int r = 0; r < m_rows; r++)
            {
                for (int c = 0; c < m_cols; c++)
                {
                    if (r == 0 || r == m_rows - 1 || c == 0 || c == m_cols - 1)
                        m_maze[r, c] = 0;
                    else if (rand.Next(2) == 0)
                        m_maze[r, c] = -1;
                    else
                        m_maze[r, c] = 0;
                }
            }
        }

        public void FindFloorRegions()
        {
            int floorNum = 1;
            floorTypeCount = new List<int>();
            floorTypeCount.Add(0);

            floorTypeMembers = new List<List<Point>>();
            floorTypeMembers.Add(new List<Point>());
            for (int r = 0; r < m_rows; r++)
            {
                for (int c = 0; c < m_cols; c++)
                {
                    if (this[r, c] == 0)
                    {
                        floorTypeCount.Add(0);
                        floorTypeMembers.Add(new List<Point>());
                        FloodFillFloor(r, c, floorNum);
                        floorNum++;
                    }
                }
            }
        }

        public void fixFloorType()
        {
            // Default: use floor type 1
            fixFloorType(1);
        }

        public void fixFloorType(int normalFloorType)
        {
            if (floorTypeCount == null)
                FindFloorRegions();
            // Find most common floor type
            int mostCommonFloorType = 0;
            int mostCommonFloorTypeCount = 0;

            // starts at 1 because 1 is the first floor type
            for (int index = 1; index < floorTypeCount.Count; index++)
            {
                if (floorTypeCount[index] > mostCommonFloorTypeCount)
                {
                    mostCommonFloorType = index;
                    mostCommonFloorTypeCount = floorTypeCount[index];
                }
            }

            // If it's not the normal one
            if (mostCommonFloorType != normalFloorType)
            {
                // Swap normal and most common
                for (int r = 0; r < m_rows; r++)
                {
                    for (int c = 0; c < m_cols; c++)
                    {
                        if (m_maze[r, c] == mostCommonFloorType)
                            m_maze[r, c] = normalFloorType;
                        else if (m_maze[r, c] == normalFloorType)
                            m_maze[r, c] = mostCommonFloorType;
                    }
                }
            }
        }

        private void FloodFillFloor(int row, int col, int floorval)
        {
            FixCoords(ref row, ref col);
            if (this[row, col] == 0)
            {
                m_maze[row, col] = floorval;
                floorTypeCount[floorval] += 1;
                floorTypeMembers[floorval].Add(new Point(row, col));

                FloodFillFloor(row - 1, col - 1, floorval);
                FloodFillFloor(row - 1, col, floorval);
                FloodFillFloor(row - 1, col + 1, floorval);
                FloodFillFloor(row, col - 1, floorval);
                FloodFillFloor(row, col + 1, floorval);
                FloodFillFloor(row + 1, col - 1, floorval);
                FloodFillFloor(row + 1, col, floorval);
                FloodFillFloor(row + 1, col + 1, floorval);
            }
        }

        public Point RandomSquare()
        {
            return this.RandomSquare(1);
        }

        public Point RandomSquare(int floortype)
        {
            int size = floorTypeMembers[floortype].Count();
            return floorTypeMembers[floortype][rand.Next(size)];
        }

        private void FixCoords(ref int row, ref int col)
        {
            while (row >= m_rows)
                row -= m_rows;
            while (row < 0)
                row += m_rows;
            while (col >= m_cols)
                col -= m_cols;
            while (col < 0)
                col += m_cols;
        }

        public List<Point> GetFOVAt(Point source, int radius)
        {
            List<Point> FOVTiles = new List<Point>();
            FOVTiles.Add(source);
            return FOVTiles;
        }

        public Dictionary<Direction,Point> GetAdjacentFloors(Point source)
        {
            Dictionary<Direction, Point> retval = new Dictionary<Direction, Point>();
            int floortype = this[source];
            foreach (Direction dir in Direction.Directions)
            {
                int r = source.X + dir.delta.X;
                int c = source.Y + dir.delta.Y;
                if(r >= 0 && r < m_rows && c >= 0 && c < m_cols)
                    if (m_maze[r, c] == floortype)
                        retval[dir] = new Point(r, c);
            }

            return retval;
        }

        //private void 
    }
}