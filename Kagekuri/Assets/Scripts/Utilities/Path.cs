using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kagekuri
{
    public static class Path
    {
        public static List<Square>[,] GetPaths(Point center, Func<Point, Point, bool, bool> canMove, Func<List<Square>, int?> getMoveCost, bool ignoreUnit = false)
        {
            var paths = new List<Square>[Field.Instance.Width, Field.Instance.Depth];
            var flags = new bool[Field.Instance.Width, Field.Instance.Depth];

            paths[center.RoundX, center.RoundY] = new List<Square>();

            while (true)
            {
                var flag = false;
                for (var i = 0; i < Field.Instance.Width; i++)
                {
                    for (var j = 0; j < Field.Instance.Depth; j++)
                    {
                        if (!flags[i, j] && getMoveCost(paths[i, j]) != null)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        break;
                }
                if (!flag)
                    break;

                int? ibuffer = null;
                int? jbuffer = null;
                int? minCost = null;
                for (var i = 0; i < Field.Instance.Width; i++)
                {
                    for (var j = 0; j < Field.Instance.Depth; j++)
                    {
                        if (flags[i, j])
                            continue;
                        var cost = getMoveCost(paths[i, j]);
                        if (cost == null)
                            continue;
                        if ((ibuffer == null && jbuffer == null && minCost == null) || cost < minCost)
                        {
                            ibuffer = i;
                            jbuffer = j;
                            minCost = cost;
                        }
                    }
                }
                flags[ibuffer.Value, jbuffer.Value] = true;
                var now = new Point(ibuffer.Value, jbuffer.Value);
                var left = now + Point.Left;
                var right = now + Point.Right;
                var down = now + Point.Down;
                var up = now + Point.Up;
                foreach (var pos in new[] { left, right, down, up })
                {
                    if (!Field.Instance.Contains(pos))
                        continue;
                    if (flags[pos.RoundX, pos.RoundY])
                        continue;
                    if (canMove(now, pos, ignoreUnit))
                    {
                        paths[pos.RoundX, pos.RoundY] = paths[now.RoundX, now.RoundY].Concat(new[] { Field.Instance[pos] }).ToList();
                    }
                }
            }

            return paths;
        }
    }
}