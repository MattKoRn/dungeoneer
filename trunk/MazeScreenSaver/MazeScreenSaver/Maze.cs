using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeScreenSaver
{
    class Maze
    {
        int[,] m_maze;
        int m_rows, m_cols;

        public Maze(int rows, int cols)
        {
            m_rows = rows;
            m_cols = cols;

            m_maze = new int[rows, cols];
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

        public void generate()
        {
            Random rng = new Random();

            for (int r = 0; r < m_rows; r++)
            {
                for (int c = 0; c < m_cols; c++)
                {
                    if (rng.Next(2) == 0)
                        m_maze[r, c] = -1;
                    else
                        m_maze[r, c] = 0;
                }
            }
        }

        public void FindFloorRegions()
        {
            int floorNum = 1;
            for (int r = 0; r < m_rows; r++)
            {
                for (int c = 0; c < m_cols; c++)
                {
                    if (this[r, c] == 0)
                    {
                        FloodFillFloor(r, c, floorNum);
                        floorNum++;
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
    }
}