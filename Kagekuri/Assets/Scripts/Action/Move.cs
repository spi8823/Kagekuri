using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kagekuri
{
    public class Move : Action
    {
        private Field _Field = null;
        public int BaseCost { get { return 5; } }
        public int CostAP { get { return Mathf.CeilToInt(100f / Owner.Status.Agility); } }

        public Move(ActiveUnit owner) : base(owner)
        {
            Name = "移動";
        }

        public override IEnumerator<bool?> Do()
        {
            if (_Field == null)
                _Field = BattleSceneManager.Instance.Stage.Field;
            var max = (int)Owner.Status.AP / CostAP;
            var range = new Range();
            var paths = GetShortestPaths();
            for(var i = -max;i <= max;i++)
            {
                for(var j = -max + Mathf.Abs(i);j <= max - Mathf.Abs(i);j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    var destination = Owner.Position + new Point(i, j);
                    if (BattleSceneManager.Instance.Stage.Field.Contains(destination))
                        destination.Z = BattleSceneManager.Instance.Stage.Field[destination].Height;
                    else
                        continue;
                    if (BattleSceneManager.Instance.Stage.Field[destination].Unit != null)
                        continue;

                    var cost = GetMoveCost(paths[destination.RoundX, destination.RoundY]);
                    if(cost != null && cost <= Owner.Status.AP)
                        range.Add(destination, 1);
                }
            }
            if (range.Points.Count == 0)
                yield break;

            IEnumerator coroutine = BattleSceneManager.Instance.Stage.Field.SelectSquare(Owner.Position, range);
            while (coroutine.MoveNext()) yield return null;

            BattleSceneManager.Instance.Stage.Field.DeletePanels();
            if (coroutine.Current as Square == null)
                yield break;

            var to = (coroutine.Current as Square).Position;
            var path = paths[to.RoundX, to.RoundY];
            foreach(var square in path)
            {
                coroutine = Owner.Go(square);
                while (coroutine.MoveNext()) yield return null;
            }

            Owner.SetAP(Math.Max(Owner.Status.AP - GetMoveCost(path).Value - BaseCost, 0));
            yield return false;
        }

        public override bool IsAvailable()
        {
            if (Owner.Status.AP >= CostAP)
                return true;
            else
                return false;
        }

        public List<Square>[,] GetShortestPaths()
        {
            var paths = new List<Square>[_Field.Width, _Field.Depth];
            var flags = new bool[_Field.Width, _Field.Depth];

            paths[Owner.Position.RoundX, Owner.Position.RoundY] = new List<Square>();

            while (true)
            {
                var flag = false;
                for (var i = 0; i < _Field.Width; i++)
                {
                    for (var j = 0; j < _Field.Depth; j++)
                    {
                        if (!flags[i, j] && GetMoveCost(paths[i, j]) != null)
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
                for(var i = 0;i < _Field.Width;i++)
                {
                    for (var j = 0; j < _Field.Depth; j++)
                    {
                        if (flags[i, j])
                            continue;
                        var cost = GetMoveCost(paths[i, j]);
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
                foreach(var pos in new[] { left, right, down, up})
                {
                    if (!_Field.Contains(pos))
                        continue;
                    if (flags[pos.RoundX, pos.RoundY])
                        continue;
                    if(CanMove(now, pos))
                    {
                        paths[pos.RoundX, pos.RoundY] = paths[now.RoundX, now.RoundY].Concat(new[] { _Field[pos] }).ToList();
                    }
                }
            }

            return paths;
        }

        public int? GetMoveCost(List<Square> path)
        {
            if (path == null)
                return null;
            else
                return path.Count * CostAP;
        }

        public bool CanMove(Point from, Point to)
        {
            var fromSquare = BattleSceneManager.Instance.Stage.Field[from];
            var toSquare = BattleSceneManager.Instance.Stage.Field[to];
            if (toSquare.Unit != null)
                return false;
            else if (Math.Abs(fromSquare.Height - toSquare.Height) <= Owner.Status.Jump)
                return true;
            else
                return false;
        }
    }
}