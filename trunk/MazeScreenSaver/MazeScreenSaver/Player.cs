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
        private Dictionary<Direction,Point> availableMoves;
        private Dictionary<Direction,Point> preferredMoves;
        public Point previousCoord;
        private List<Point> visitedCoords;
        private Point oldestBacktrack;
        private bool is_backtracking;
        public bool has_moved = false;
        public Point stairsCoord;
        private Random rand;
        private Direction facing;
        private int rotationDirection;

        public Player()
        {
            coord = new Point(0, 0);
            visitedCoords = new List<Point>();
            rand = new Random();
            facing = Direction.None;
            rotationDirection = rand.Next(1, 3); // 1 or 2, 1 = clockwise, 2 = counterclockwise
        }

        public void setStartingCoord(Point coord)
        {
            this.coord = coord;
            visitedCoords.Add(coord);
        }

        public void setAvailableMoves(Dictionary<Direction,Point> moves)
        {
            availableMoves = moves;
            preferredMoves = new Dictionary<Direction, Point>();
            oldestBacktrack = new Point(0,0);
            foreach (KeyValuePair<Direction, Point> kvp in moves)
            {
                Point p = kvp.Value;
                if (!visitedCoords.Contains(p))
                    preferredMoves[kvp.Key] = p;
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
            Point moveTo;
            previousCoord = coord;
            List<List<Point>> goodMoves = new List<List<Point>>();
            goodMoves.Add(new List<Point>(1));
            goodMoves.Add(new List<Point>(2));
            goodMoves.Add(new List<Point>(2));
            goodMoves.Add(new List<Point>(2));
            goodMoves.Add(new List<Point>(1));
            if (preferredMoves.ContainsValue(stairsCoord))
            {
                moveTo = stairsCoord;
                is_backtracking = false;
            }
            else if (preferredMoves.Count() == 0)
            {
                moveTo = oldestBacktrack;
                facing = Direction.getDirection(coord, previousCoord);
                is_backtracking = true;
            }
            else
            {
                if (facing == Direction.None)
                {
                    Point move = preferredMoves[preferredMoves.Keys.ElementAt(rand.Next(preferredMoves.Count))];
                    coord = move;
                    facing = Direction.getDirection(previousCoord, coord);
                    is_backtracking = false;
                }

                // Keep moving in the same direction if you can
                foreach (KeyValuePair<Direction, Point> move in preferredMoves)
                {
                    if (move.Key == facing)
                    {
                        List<Point> sameDir = new List<Point>();
                        goodMoves[0].Add(move.Value);
                    }
                    else if (move.Key == facing.adjacent[0] || move.Key == facing.adjacent[1])
                    {
                        goodMoves[1].Add(move.Value);
                    }
                    else if (move.Key == facing.adjacent[0].adjacent[0] || move.Key == facing.adjacent[1].adjacent[1])
                    {
                        goodMoves[2].Add(move.Value);
                    }
                    else if (move.Key == facing.adjacent[0].adjacent[0].adjacent[0] || move.Key == facing.adjacent[1].adjacent[1].adjacent[1])
                    {
                        goodMoves[3].Add(move.Value);
                    }
                    else if (move.Key == facing.adjacent[0].adjacent[0].adjacent[0].adjacent[0]) // should be the same as 4x[1]
                    {
                        goodMoves[4].Add(move.Value);
                    }
                }

                moveTo = preferredMoves[preferredMoves.Keys.ElementAt(rand.Next(preferredMoves.Count))];
                foreach (List<Point> movelist in goodMoves)
                {
                    if (movelist.Count == 0)
                        continue;
                    if (movelist.Count == 1)
                    {
                        moveTo = movelist[0];
                        break;
                    }
                    else
                    {
                        moveTo = movelist[rand.Next(movelist.Count)];
                        break;
                    }
                }
                is_backtracking = false;
            }
            coord = moveTo;
            visitedCoords.Add(coord);
            if(!is_backtracking)
                facing = Direction.getDirection(previousCoord, coord);
            else
                facing = Direction.getDirection(coord, previousCoord);
            has_moved = true;
        }

        public List<Point> getVisitedCoords()
        {
            return visitedCoords;
        }
    }
}
