using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MazeScreenSaver
{
    class Player
    {
        public Point coord;
        private List<Point> availableMoves;
        private List<Point> preferredMoves;
        public Point previousCoord;
        private List<Point> visitedCoords;
        private Point oldestBacktrack;
        private bool is_backtracking;

        public Player()
        {
            coord = new Point(0, 0);
            visitedCoords = new List<Point>();
        }

        public void setAvailableMoves(List<Point> moves)
        {
            availableMoves = moves;
            preferredMoves = new List<Point>();
            oldestBacktrack = new Point(-1, -1);
            foreach (Point p in moves)
            {
                if (!visitedCoords.Contains(p))
                    preferredMoves.Add(p);
                else
                {
                    if (oldestBacktrack == new Point(-1, -1))
                        oldestBacktrack = p;
                    else
                    {
                        if (visitedCoords.IndexOf(p) < visitedCoords.IndexOf(oldestBacktrack))
                            oldestBacktrack = p;
                    }
                }
            }
        }

        public void move()
        {
            Random r = new Random();
            previousCoord = coord;
            if (preferredMoves.Count() > 0)
            {
                coord = preferredMoves[r.Next(preferredMoves.Count())];
                visitedCoords.Add(coord);
                is_backtracking = true;
            }
            else
            {
                coord = oldestBacktrack;
                is_backtracking = true;
            }
        }
    }
}
