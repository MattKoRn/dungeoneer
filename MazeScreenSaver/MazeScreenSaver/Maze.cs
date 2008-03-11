using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeScreenSaver
{
    class Maze
    {
        bool[,] m_maze;
        int m_rows, m_cols;

        public Maze(int rows, int cols)
        {
            m_rows = rows;
            m_cols = cols;

            m_maze = new bool[rows, cols];
        }

        public bool this[int row, int col]
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
                        m_maze[r, c] = true;
                    else
                        m_maze[r, c] = false;
                }
            }
        }
    }
}