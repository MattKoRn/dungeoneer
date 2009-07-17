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
            List<List<Point>> goodMoves = new List<List<Point>>();
            goodMoves.Add(new List<Point>(1));
            goodMoves.Add(new List<Point>(2));
            goodMoves.Add(new List<Point>(2));
            goodMoves.Add(new List<Point>(2));
            goodMoves.Add(new List<Point>(1));
            if (preferredMoves.Contains(stairsCoord))
            {
                coord = stairsCoord;
                is_backtracking = false;
            }

            if (preferredMoves.Count() == 0)
            {
                coord = oldestBacktrack;
                facing = Direction.getDirection(coord, previousCoord);
                is_backtracking = true;
            }
            else
            {
                if (facing == Direction.None)
                {
                    Point move = preferredMoves[rand.Next(preferredMoves.Count)];
                    coord = move;
                    facing = Direction.getDirection(previousCoord, coord);
                    is_backtracking = false;
                }

                // Keep moving in the same direction if you can
                foreach (Point move in preferredMoves)
                {
                    if (Direction.getDirection(coord, move).delta == facing.delta)
                    {
                        List<Point> sameDir = new List<Point>();
                        goodMoves[0].Add(move);
                    }
                    else if (Direction.getDirection(coord, move).delta == facing.adjacent[0].delta ||
                        Direction.getDirection(coord, move).delta == facing.adjacent[1].delta)
                    {
                        goodMoves[1].Add(move);
                    }
                    else if (Direction.getDirection(coord, move).delta == facing.adjacent[0].adjacent[0].delta ||
                        Direction.getDirection(coord, move).delta == facing.adjacent[1].adjacent[1].delta)
                    {
                        goodMoves[2].Add(move);
                    }
                    else if (Direction.getDirection(coord, move).delta == facing.adjacent[0].adjacent[0].adjacent[0].delta ||
                        Direction.getDirection(coord, move).delta == facing.adjacent[1].adjacent[1].adjacent[1].delta)
                    {
                        goodMoves[3].Add(move);
                    }
                    else if (Direction.getDirection(coord, move).delta == facing.adjacent[0].adjacent[0].adjacent[0].adjacent[0].delta) // should be the same as 4x[1]
                    {
                        goodMoves[4].Add(move);
                    }
                }

                Point moveTo = preferredMoves[rand.Next(preferredMoves.Count)];
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
                coord = moveTo;
                visitedCoords.Add(coord);
                facing = Direction.getDirection(previousCoord, coord);
                is_backtracking = false;
            }
            has_moved = true;
        }
    }
}
