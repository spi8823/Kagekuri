using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kagekuri
{
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        } 

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.Equals(b);
        }

        public static bool operator != (Point a, Point b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Point))
                return false;
            else
            {
                Point point = (Point)obj;
                if (X == point.X && Y == point.Y)
                    return true;
                else
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }

        public static readonly Point Zero = new Point(0, 0);
        public static readonly Point DefaultDirection = Right;
        public static readonly Point Right = new Point(1, 0);
        public static readonly Point Left = new Point(-1, 0);
        public static readonly Point Up = new Point(0, 1);
        public static readonly Point Down = new Point(0, -1);
    }
}