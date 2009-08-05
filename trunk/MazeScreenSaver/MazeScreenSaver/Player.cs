using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MazeScreenSaver
{
    class Move
    {
        public Point destination;
        public Direction direction;

        public Move()
        {
            destination = new Point(-1, -1);
            direction = Direction.None;
        }

        public Move(Point dest, Direction dir)
        {
            destination = dest;
            direction = dir;
        }
    }

    class Player
    {
        public Point coord;
        private List<Move> availableMoves;
        private List<Move> preferredMoves;
        public Point previousCoord;
        private List<Point> visitedCoords;
        private Move oldestBacktrack;
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
            availableMoves = new List<Move>();
            preferredMoves = new List<Move>();
            oldestBacktrack = new Move();
            foreach (KeyValuePair<Direction, Point> kvp in moves)
            {
                Move m = new Move(kvp.Value, kvp.Key);
                if (!visitedCoords.Contains(m.destination))
                    preferredMoves.Add(m);
                else
                {
                    if (oldestBacktrack.destination == new Point(-1, -1))
                        oldestBacktrack = m;
                    else
                    {
                        if (visitedCoords.IndexOf(m.destination) < visitedCoords.IndexOf(oldestBacktrack.destination))
                            oldestBacktrack = m;
                    }
                }
            }

            if (preferredMoves.Count == 0 && oldestBacktrack.destination == visitedCoords[1] && coord == visitedCoords[0])
            {
                // Reset the list to keep us from getting in a loop. By this point actually we would have to give up but we're moving randomly so it can happen.
                visitedCoords = new List<Point>();
                facing = Direction.None;
                visitedCoords.Add(coord);
                setAvailableMoves(moves); // Try again
            }
        }

        public void move()
        {
            Move moveTo = new Move();
            previousCoord = coord;
            List<List<Move>> goodMoves = new List<List<Move>>();
            goodMoves.Add(new List<Move>(1));
            goodMoves.Add(new List<Move>(2));
            goodMoves.Add(new List<Move>(2));
            goodMoves.Add(new List<Move>(2));
            goodMoves.Add(new List<Move>(1));
            foreach (Move m in preferredMoves)
            {
                if (m.destination == stairsCoord)
                    moveTo = m;
            }
            if (moveTo.destination == new Point(-1, -1) && preferredMoves.Count() == 0)
            {
                moveTo = oldestBacktrack;
                facing = Direction.getDirection(coord, previousCoord);
                is_backtracking = true;
            }

            if (moveTo.destination == new Point(-1, -1) && facing == Direction.None)
            {
                moveTo = preferredMoves[rand.Next(preferredMoves.Count)];
                is_backtracking = false;
            }

            if(moveTo.destination == new Point(-1, -1))
            {
                // Keep moving in the same direction if you can
                foreach (Move move in preferredMoves)
                {
                    if (move.direction == facing)
                    {
                        goodMoves[0].Add(move);
                    }
                    else if (move.direction == facing.adjacent[0] || move.direction == facing.adjacent[1])
                    {
                        goodMoves[1].Add(move);
                    }
                    else if (move.direction == facing.adjacent[0].adjacent[0] || move.direction == facing.adjacent[1].adjacent[1])
                    {
                        goodMoves[2].Add(move);
                    }
                    else if (move.direction == facing.adjacent[0].adjacent[0].adjacent[0] || move.direction == facing.adjacent[1].adjacent[1].adjacent[1])
                    {
                        goodMoves[3].Add(move);
                    }
                    else if (move.direction == facing.adjacent[0].adjacent[0].adjacent[0].adjacent[0]) // should be the same as 4x[1]
                    {
                        goodMoves[4].Add(move);
                    }
                }

                moveTo = preferredMoves[rand.Next(preferredMoves.Count)];
                foreach (List<Move> movelist in goodMoves)
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
            coord = moveTo.destination;
            visitedCoords.Add(coord);
            if (!is_backtracking)
                facing = moveTo.direction;
            else
                facing = moveTo.direction.adjacent[0].adjacent[0].adjacent[0].adjacent[0];
            has_moved = true;
        }

        public List<Point> getVisitedCoords()
        {
            return visitedCoords;
        }
    }
}
