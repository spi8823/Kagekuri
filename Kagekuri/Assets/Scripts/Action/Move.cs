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
            var paths = Path.GetPaths(Owner.Position, CanMove, GetMoveCost);
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
            coroutine = Shift(path);
            while (coroutine.MoveNext()) yield return null;

            yield return false;
        }

        public IEnumerator Shift(List<Square> path)
        {
            IEnumerator coroutine;
            foreach(var square in path)
            {
                coroutine = Owner.Go(square);
                while (coroutine.MoveNext()) yield return null;
            }

            coroutine = Owner.ConsumeAP(GetMoveCost(path).Value);
            while (coroutine.MoveNext()) yield return null;
        }

        public override bool IsAvailable()
        {
            if (Owner.Status.AP >= CostAP)
                return true;
            else
                return false;
        }

        public int? GetMoveCost(List<Square> path)
        {
            if (path == null)
                return null;
            else
                return path.Count * CostAP + BaseCost;
        }

        public bool CanMove(Point from, Point to, bool ignoreUnit = false)
        {
            var fromSquare = BattleSceneManager.Instance.Stage.Field[from];
            var toSquare = BattleSceneManager.Instance.Stage.Field[to];
            if (!ignoreUnit && toSquare.Unit != null)
                return false;
            else if (Math.Abs(fromSquare.Height - toSquare.Height) <= Owner.Status.Jump)
                return true;
            else
                return false;
        }
    }
}