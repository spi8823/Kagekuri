using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kagekuri
{
    public class Range
    {
        public Dictionary<Point, double> Points { get; private set; }
        public double EffectiveHeight { get; private set; }

        public Range()
        {
            Points = new Dictionary<Point, double>();
            EffectiveHeight = 0;
        }

        public Range(Range range) : this()
        {
            foreach (var point in range.Points)
            {
                Points.Add(point.Key, point.Value);
            }
            EffectiveHeight = range.EffectiveHeight;
        }

        public Range(int distance, double height, Func<int, double> distanceToValue)
            : this()
        {
            for (var i = -distance; i <= distance; i++)
            {
                for (var j = -distance + Mathf.Abs(i); j <= distance - Mathf.Abs(i); j++)
                {
                    Add(new Point(i, j), distanceToValue(Math.Abs(i) + Math.Abs(j)));
                }
            }

            EffectiveHeight = height;
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

        public double this[Point point]
        {
            get
            {
                if (Points.Any(p => p.Key.RoundX == point.RoundX && p.Key.RoundY == point.RoundY))
                {
                    var pp = Points.First(p => p.Key.RoundX == point.RoundX && p.Key.RoundY == point.RoundY);
                    if (Mathf.Abs((float)pp.Key.Z - (float)point.Z) <= EffectiveHeight)
                        return pp.Value;
                    else
                        return 0;
                }
                else return 0;
            }
        }

        public static Range operator +(Range r, Point p)
        {
            var range = new Range();
            foreach (var point in r.Points)
            {
                range.Points.Add(point.Key + p, point.Value);
            }

            return range;
        }
    }
}