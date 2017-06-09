using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kagekuri
{
    [System.Serializable]
    public struct Point
    {
        [SerializeField]
        public double X;
        [SerializeField]
        public double Y;
        [SerializeField]
        public double Z;

        public int RoundX { get { return (int)X; } }
        public int RoundY { get { return (int)Y; } }
        public int RoundZ { get { return (int)Z; } }

        public static readonly Point Zero = new Point(0, 0);
        public static readonly Point DefaultDirection = Right;
        public static readonly Point Right = new Point(1, 0);
        public static readonly Point Left = new Point(-1, 0);
        public static readonly Point Up = new Point(0, 1);
        public static readonly Point Down = new Point(0, -1);

        public const int SideLength = 32;
        public const int SquareWidth = SideLength;
        public const int SquareHeight = SideLength / 2;

        public Point(double x, double y, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        public Vector3 ToUnityPosition()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            x += X * (SquareWidth / 2);
            x -= Y * (SquareWidth / 2);

            y += X * SquareHeight / 2;
            y += Y * SquareHeight / 2;
            y += Z * SquareHeight;

            z += (int)X;
            z += (int)Y;
            z -= Z / 100.0;

            return new Vector3((float)x, (float)y, (float)z);
        }

        public static Point GetQuaterviewPositionFromUnityPosition(Vector3 vector)
        {
            double x = -vector.x / SquareWidth + vector.y / SquareHeight;
            double y = vector.x / SquareWidth + vector.y / SquareHeight;

            return new Point(x, y, 0);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        } 

        public static Point operator -(Point a)
        {
            return new Point(-a.X, -a.Y, -a.Z);
        }

        public static Point operator -(Point a, Point b)
        {
            return a + -b;
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
                if (X == point.X && Y == point.Y && Z == point.Z)
                    return true;
                else
                    return false;
            }
        }

        public bool EqualsRoughly(Point point)
        {
            if (RoundX == point.RoundX && RoundY == point.RoundY)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }
}