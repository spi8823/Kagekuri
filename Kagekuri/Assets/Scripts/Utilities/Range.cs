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

        public double? GetValue(Point vector, Direction direction)
        {
            switch(direction)
            {
                case Direction.Right:
                    return GetValue(vector);
                case Direction.Left:
                    return GetValue(-vector);
                case Direction.Forward:
                    return GetValue(new Point(vector.Y, -vector.X));
                case Direction.Back:
                    return GetValue(new Point(-vector.Y, vector.X));
                default:
                    return null;
            }
        }

        public double? GetValue(Point from, Point to, Direction direction)
        {
            Point vector = to - from;
            return GetValue(vector, direction);
        }

        private double? GetValue(Point vector)
        {
            foreach (var item in Points)
            {
                if (item.Key == vector)
                    return item.Value;
            }
            return null;
        }

        public double? this[Point point]
        {
            get
            {
                if (Points.Any(p => p.Key.RoundX == point.RoundX && p.Key.RoundY == point.RoundY))
                {
                    var pp = Points.First(p => p.Key.RoundX == point.RoundX && p.Key.RoundY == point.RoundY);
                    if (Mathf.Abs((float)pp.Key.Z - (float)point.Z) <= EffectiveHeight)
                        return pp.Value;
                    else
                        return null;
                }
                else return null;
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