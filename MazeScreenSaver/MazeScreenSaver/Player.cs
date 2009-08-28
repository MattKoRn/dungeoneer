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
            destination = new Point(-999, -999);
            direction = Direction.None;
        }

        public Move(Point dest, Direction dir)
        {
            destination = dest;
            direction = dir;
        }

        public static bool operator==(Move a, Move b)
        {
            if (a.direction == b.direction && a.destination == b.destination)
                return true;
            else
                return false;
        }

        public static bool operator !=(Move a, Move b)
        {
            return !(a == b);
        }

        public bool isNull()
        {
            return destination == new Point(-999, -999);
        }

        public override string ToString()
        {
            return destination.ToString() + direction.name;
        }
    }

    class Player
    {
        public Point coord;
        private List<Move> visitedMoves;
        private List<Move> newMoves;
        private List<Move> explorableMoves;
        public Point previousCoord;
        private List<Point> explorableCoords;
        private List<Point> visitedCoords;
        private List<Point> coordHistory;
        private bool is_backtracking;
        public bool has_moved = false;
        public Point stairsCoord;
        private Random rand;
        private Direction facing;
        private int rotationDirection;
        public Logger log;
        private Direction goalDirection;
        private int[,] knownMaze;
        private int mazeRows, mazeCols;

        public Player()
        {
            coord = new Point(0, 0);
            visitedCoords = new List<Point>();
            explorableCoords = new List<Point>();
            coordHistory = new List<Point>();
            rand = new Random();
            facing = Direction.None;
            rotationDirection = rand.Next(1, 3); // 1 or 2, 1 = clockwise, 2 = counterclockwise
            mazeRows = mazeCols = 1;
            knownMaze = new int[1, 1];
        }

        public void setMazeSize(int rows, int cols)
        {
            knownMaze = new int[rows, cols];
            mazeRows = rows;
            mazeCols = cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    knownMaze[r, c] = -1;
                }
            }
        }

        private void resetGoalDirection()
        {
            int deltaR, deltaC;
            deltaR = stairsCoord.X - coord.X;
            deltaC = stairsCoord.Y - coord.Y;

            if (deltaC != 0 && deltaR != 0)
            {
                double slope = Math.Abs((double)deltaR / deltaC);
                if (slope > 0.666 && slope < 1.5)
                {
                    deltaR = deltaR / Math.Abs(deltaR);
                    deltaC = deltaC / Math.Abs(deltaC);
                }
                else if (slope >= 1.5)
                {
                    deltaC = 0;
                    deltaR = deltaR / Math.Abs(deltaR);
                }
                else // if(slope <= 0.666)
                {
                    deltaR = 0;
                    deltaC = deltaC / Math.Abs(deltaC);
                }
            }
            else if (deltaC == 0 && deltaR != 0)
                deltaR = deltaR / Math.Abs(deltaR);
            else if (deltaR == 0 && deltaC != 0)
                deltaC = deltaC / Math.Abs(deltaC);

            goalDirection = Direction.getDirection(new Point(deltaR, deltaC));
        }

        public void setStartingCoord(Point coord)
        {
            this.coord = coord;
            coordHistory.Add(coord);
            resetGoalDirection();
            knownMaze[coord.X, coord.Y] = 1;
        }

        public void setAvailableMoves(Dictionary<Direction,Point> moves)
        {
            visitedMoves = new List<Move>();
            explorableMoves = new List<Move>();
            newMoves = new List<Move>();

            int r, c;

            for (int deltaR = -1; deltaR <= 1; deltaR++)
            {
                for (int deltaC = -1; deltaC <= 1; deltaC++)
                {
                    r = deltaR + coord.X;
                    c = deltaC + coord.Y;
                    if (r >= 0 && r < mazeRows && c >= 0 && c < mazeCols)
                    {
                        Point p = new Point(r, c);
                        if (moves.Values.Contains(p))
                            knownMaze[r, c] = 1;
                        else
                            knownMaze[r, c] = 0;
                    }
                }
            }

            int currentBacktrackIndex = coordHistory.Count;
            if (is_backtracking)
                currentBacktrackIndex = coordHistory.IndexOf(coord);

            foreach (KeyValuePair<Direction, Point> kvp in moves)
            {
                Move m = new Move(kvp.Value, kvp.Key);
                if (visitedCoords.Contains(m.destination))
                {
                    if (coordHistory.IndexOf(m.destination) < currentBacktrackIndex)
                        visitedMoves.Add(m);
                }
                else if (explorableCoords.Contains(m.destination))
                {
                    explorableMoves.Add(m);
                }
                else
                {
                    newMoves.Add(m);
                }
            }

            if (visitedMoves.Count == 0 && explorableMoves.Count == 0 && newMoves.Count == 0)
            {
                // Stuck, reset state and try again
                coordHistory = new List<Point>();
                visitedCoords = new List<Point>();
                explorableCoords = new List<Point>();
                resetGoalDirection();
                setAvailableMoves(moves);
            }
        }

        public void move()
        {
            Move moveTo = new Move();
            previousCoord = coord;
            List<Move> movePrefOrder = new List<Move>();
            Move stairsMove = new Move(), newMove = new Move(), exploreMove = new Move(), visitedMove = new Move();

            foreach (Move m in newMoves)
            {
                if (m.destination == stairsCoord)
                    stairsMove = m;
            }

            foreach (Move m in newMoves)
            {
                if (m != stairsMove)
                {
                    if (newMove.isNull())
                    {
                        newMove = m;
                    }
                    else
                    {
                        if (goalDirection.AngleDifference(m.direction) < goalDirection.AngleDifference(newMove.direction))
                            newMove = m;
                        else if (goalDirection.AngleDifference(m.direction) == goalDirection.AngleDifference(newMove.direction) && rand.Next(2) == 0)
                            newMove = m;
                    }
                }
            }

            foreach (Move e in explorableMoves)
            {
                if (exploreMove.isNull())
                    exploreMove = e;
                else
                {
                    if (goalDirection.AngleDifference(e.direction) < goalDirection.AngleDifference(exploreMove.direction))
                        exploreMove = e;
                    else if (goalDirection.AngleDifference(e.direction) == goalDirection.AngleDifference(exploreMove.direction) && rand.Next(2) == 0)
                        exploreMove = e;
                }
            }

            foreach (Move v in visitedMoves)
            {
                if (visitedMove.isNull())
                {
                    visitedMove = v;
                }
                else
                {
                    if (visitedCoords.IndexOf(v.destination) > visitedCoords.IndexOf(visitedMove.destination))
                    {
                        visitedMove = v;
                    }
                }
            }

            if (!stairsMove.isNull())
            {
                moveTo = stairsMove;
                is_backtracking = false;
            }
            else if (!newMove.isNull())
            {
                moveTo = newMove;
                is_backtracking = false;
            }
            else if (!exploreMove.isNull())
            {
                moveTo = exploreMove;
                explorableCoords.Remove(moveTo.destination);
                is_backtracking = true;
            }
            else if (!visitedMove.isNull())
            {
                moveTo = visitedMove;
                is_backtracking = true;
            }
            else
                throw new Exception("I ran out of moves!");

            coord = moveTo.destination;
            if (!is_backtracking)
            {
                resetGoalDirection();
                coordHistory.Add(moveTo.destination);
            }

            if (newMoves.Count > 1)
            {
                if (!explorableCoords.Contains(previousCoord))
                {
                    explorableCoords.Add(previousCoord);
                }
            }
            else
            {
                if(!visitedCoords.Contains(previousCoord))
                {
                    visitedCoords.Add(previousCoord);
                }
            }

            has_moved = true;
        }

        public List<Point> getVisitedCoords()
        {
            return visitedCoords;
        }

        public List<Point> getExploredCoords()
        {
            return explorableCoords;
        }

        public int getKnownMaze(int r, int c)
        {
            if (r < 0 || r >= mazeRows)
                throw new ArgumentOutOfRangeException("r", "R out of range: valid values are 0-" + mazeRows.ToString());
            if (c < 0 || c >= mazeCols)
                throw new ArgumentOutOfRangeException("c", "C out of range: valid values are 0-" + mazeCols.ToString());
            return knownMaze[r, c];
        }
    }
}
