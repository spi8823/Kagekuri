using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kagekuri
{
    public class Range
    {
        public Dictionary<Point, double> Points { get; private set; }

        public Range()
        {
            Points = new Dictionary<Point, double>();
        }

        public void Add(Point point, double value)
        {
            if (!Points.ContainsKey(point))
                Points.Add(point, value);
        }

        public double? GetValue(Unit from, Unit to)
        {
            Point vector = to.Position - from.Position;
            return GetValue(vector, from.Direction);
        }

        public double? GetValue(Point vector, Point direction)
        {
            if (vector == Point.Right)
            {
                return GetValue(vector);
            }
            else if (vector == Point.Left)
            {
                return GetValue(new Point(-vector.X, -vector.Y));
            }
            else if (vector == Point.Up)
            {
                return GetValue(new Point(vector.Y, -vector.X));
            }
            else if (vector == Point.Down)
            {
                return GetValue(new Point(-vector.Y, vector.X));
            }
            else
            {
                return null;
            }
        }

        public double? GetValue(Point vector)
        {
            foreach (var item in Points)
            {
                if (item.Key == vector)
                    return item.Value;
            }
            return null;
        }

        public double? GetValue(Unit unit)
        {
            return GetValue(unit.Position);
        }

        public bool IsContains(Unit from, Unit to)
        {
            double? value = GetValue(from, to);
            return value != null;
        }

        public bool IsContains(Point vector, Point direction)
        {
            double? value = GetValue(vector, direction);
            return value != null;
        }

        public bool IsContains(Point vector)
        {
            double? value = GetValue(vector);
            return value != null;
        }

        public bool IsContains(Unit unit)
        {
            return IsContains(unit.Position);
        }
    }
}