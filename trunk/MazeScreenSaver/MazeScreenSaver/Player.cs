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
        public bool has_moved = false;
        public Point stairsCoord;
        private Random rand;

        public Player()
        {
            coord = new Point(0, 0);
            visitedCoords = new List<Point>();
            rand = new Random();
        }

        public void setAvailableMoves(List<Point> moves)
        {
            availableMoves = moves;
            preferredMoves = new List<Point>();
            oldestBacktrack = new Point(0,0);
            foreach (Point p in moves)
            {
                if (!visitedCoords.Contains(p))
                    preferredMoves.Add(p);
                else
                {
                    if (oldestBacktrack == new Point(0,0))
                        oldestBacktrack = p;
                    else
                    {
                        if (visitedCoords.IndexOf(p) < visitedCoords.IndexOf(oldestBacktrack))
                            oldestBacktrack = p;
                    }
                }
            }

            if (preferredMoves.Count == 0 && oldestBacktrack == visitedCoords[1] && coord == visitedCoords[0])
            {
                // Reset the list to keep us from getting in a loop. By this point actually we would have to give up but we're moving randomly so it can happen.
                visitedCoords = new List<Point>();
                visitedCoords.Add(coord);
                setAvailableMoves(moves); // Try again
            }
        }

        public void move()
        {
            previousCoord = coord;
            if (preferredMoves.Contains(stairsCoord))
            {
                coord = stairsCoord;
            }
            if (preferredMoves.Count() > 0)
            {
                coord = preferredMoves[rand.Next(preferredMoves.Count())];
                visitedCoords.Add(coord);
                is_backtracking = false;
            }
            else
            {
                coord = oldestBacktrack;
                is_backtracking = true;
            }
            has_moved = true;
        }
    }
}
