using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MazeScreenSaver
{
    class Direction
    {
        public Point delta;
        public Direction[] adjacent;
        
        public static Direction North;
        public static Direction Northeast;
        public static Direction East;
        public static Direction Southeast;
        public static Direction South;
        public static Direction Southwest;
        public static Direction West;
        public static Direction Northwest;
        public static Direction None;
        public static Direction[] Directions;


        static Direction() // Initialize directions
        {
            North = new Direction(new Point(-1, 0));
            Northeast = new Direction(new Point(-1, 1));
            East = new Direction(new Point(0, 1));
            Southeast = new Direction(new Point(1, 1));
            South = new Direction(new Point(1, 0));
            Southwest = new Direction(new Point(1, -1));
            West = new Direction(new Point(0, -1));
            Northwest = new Direction(new Point(-1, -1));
            None = new Direction(new Point(0, 0));

            North.SetAdjacents(new Direction[] { Northwest, Northeast });
            Northeast.SetAdjacents(new Direction[] { North, East });
            East.SetAdjacents(new Direction[] { Northeast, Southeast });
            Southeast.SetAdjacents(new Direction[] { East, South });
            South.SetAdjacents(new Direction[] { Southeast, Southwest });
            Southwest.SetAdjacents(new Direction[] { South, West });
            West.SetAdjacents(new Direction[] { Southwest, Northwest });
            Northwest.SetAdjacents(new Direction[] { West, North });
            None.SetAdjacents(new Direction[] { None });

            Directions = new Direction[] { North, Northeast, East, Southeast, South, Southwest, West, Northwest };
        }

        private Direction(Point dirDelta)
        {
            delta = dirDelta;
        }

        private void SetAdjacents(Direction[] adjacents)
        {
            adjacent = adjacents;
        }

        public static Direction getDirection(Point from, Point to)
        {
            Point delta = new Point(0,0);
            delta.X = to.X - from.X;
            if(delta.X != 0)
                delta.X = delta.X / (Math.Abs(delta.X));
            delta.Y = to.Y - from.Y;
            if(delta.Y != 0)
                delta.Y = delta.Y / (Math.Abs(delta.Y));

            Direction retval = Direction.None;
            foreach (Direction dir in Directions)
            {
                if (dir.delta == delta)
                    retval = dir;
            }
            return retval;
        }
    }
}
