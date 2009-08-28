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
        public string name;
        
        public static Direction North;
        public static Direction Northeast;
        public static Direction East;
        public static Direction Southeast;
        public static Direction South;
        public static Direction Southwest;
        public static Direction West;
        public static Direction Northwest;
        public static Direction None;
        public static List<Direction> Directions;


        static Direction() // Initialize directions
        {
            North = new Direction(new Point(-1, 0), "North");
            Northeast = new Direction(new Point(-1, 1), "Northeast");
            East = new Direction(new Point(0, 1),"East");
            Southeast = new Direction(new Point(1, 1), "Southeast");
            South = new Direction(new Point(1, 0), "South");
            Southwest = new Direction(new Point(1, -1), "Southwest");
            West = new Direction(new Point(0, -1), "West");
            Northwest = new Direction(new Point(-1, -1), "Northwest");
            None = new Direction(new Point(0, 0), "None");

            North.SetAdjacents(new Direction[] { Northwest, Northeast });
            Northeast.SetAdjacents(new Direction[] { North, East });
            East.SetAdjacents(new Direction[] { Northeast, Southeast });
            Southeast.SetAdjacents(new Direction[] { East, South });
            South.SetAdjacents(new Direction[] { Southeast, Southwest });
            Southwest.SetAdjacents(new Direction[] { South, West });
            West.SetAdjacents(new Direction[] { Southwest, Northwest });
            Northwest.SetAdjacents(new Direction[] { West, North });
            None.SetAdjacents(new Direction[] { None });

            Directions = new List<Direction>(new Direction[] { North, Northeast, East, Southeast, South, Southwest, West, Northwest });
        }

        private Direction(Point dirDelta, string in_name)
        {
            delta = dirDelta;
            name = in_name;
        }

        private void SetAdjacents(Direction[] adjacents)
        {
            adjacent = adjacents;
        }

        public int AngleDifference(Direction other)
        {
            int thisIndex = Directions.IndexOf(this);
            int otherIndex = Directions.IndexOf(other);

            int turns;
            if (thisIndex < otherIndex)
                turns = otherIndex - thisIndex;
            else
                turns = thisIndex - otherIndex;

            if (turns > 4)
                turns = 8 - turns;
            return turns * 45;
        }

        public static Direction getDirection(Point delta)
        {
            foreach (Direction d in Directions)
            {
                if (d.delta == delta)
                    return d;
            }
            return Direction.None;
        }
    }
}
