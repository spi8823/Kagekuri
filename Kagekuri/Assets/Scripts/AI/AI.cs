using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kagekuri
{
    public class AI
    {
        public readonly ActiveUnit Owner;

        public AI(ActiveUnit owner)
        {
            Owner = owner;
        }

        public IEnumerator Automate()
        {
            IEnumerator coroutine = null;
            var move = Owner.Actions.OfType<Move>().First();
            while(Owner.Status.AP > 0)
            {
                Debug.Log("Start AI");
                var map = new double?[Field.Instance.Width, Field.Instance.Depth];
                
                //移動コストで点数マップを初期化
                var paths = Path.GetPaths(Owner.Position, move.CanMove, move.GetMoveCost);
                for (var i = 0;i < map.GetLength(0);i++)
                {
                    for(var j = 0;j < map.GetLength(1);j++)
                    {
                        var path = paths[i, j];
                        var c = move.GetMoveCost(path);
                        if (c == null)
                            map[i, j] = null;
                        else
                            map[i, j] = -c;
                    }
                }
                map[Owner.Position.RoundX, Owner.Position.RoundY] = 0;

                //他のUnitまでの距離で点数を加減する
                for(var i = 0;i < map.GetLength(0);i++)
                {
                    for(var j = 0;j < map.GetLength(1);j++)
                    {
                        if (map[i, j] == null)
                            continue;

                        var ps = Path.GetPaths(new Point(i, j), move.CanMove, move.GetMoveCost, true);

                        foreach(var unit in Stage.Instance.ActiveUnits)
                        {
                            if (unit == Owner)
                                continue;
                            var c = move.GetMoveCost(ps[unit.Position.RoundX, unit.Position.RoundY]);
                            if (c == null)
                                continue;

                            if (Owner.Status.Camp != unit.Status.Camp)
                                map[i, j] += 10000 / c;
                            else
                                map[i, j] += 500 / c;
                        }
                    }
                }

                //他のUnitからの距離で点数を加減する
                foreach (var unit in Stage.Instance.ActiveUnits)
                {
                    if (unit == Owner)
                        continue;
                    var m = unit.Actions.OfType<Move>().First();
                    //unitの移動コスト
                    var ps_from = Path.GetPaths(unit.Position, m.CanMove, m.GetMoveCost);
                    for(var i= 0;i < map.GetLength(0);i++)
                    {
                        for(var j = 0;j < map.GetLength(1);j++)
                        {
                            if (map[i, j] == null)
                                continue;
                            var c = m.GetMoveCost(ps_from[i, j]);
                            if (c == null)
                                continue;

                            if (Owner.Status.Camp != unit.Status.Camp)
                                map[i, j] -= 500 / c;
                            else
                                map[i, j] += 300 / c;
                        }
                    }
                }

                //スキル・アイテムを使えるかどうかで点数を加算する
                Skill skillbuff = null;
                Point? skillDirectionbuff = null;
                for(var i = 0;i < map.GetLength(0);i++)
                {
                    for(var j = 0;j < map.GetLength(1);j++)
                    {
                        if (map[i, j] == null)
                            continue;

                        double? evaluationMax = null;
                        foreach(var skill in Owner.Skills)
                        {
                            if (!skill.IsAvailable())
                                continue;

                            //スキルを使用できる対象座標それぞれについて
                            foreach(var pair in from p in skill.UsableRange.Points select p)
                            {
                                var point = pair.Key + new Point(i, j);
                                if (Field.Instance.Contains(point))
                                    point.Z += Field.Instance[i, j].Height;
                                else
                                    continue;
                                var usableRangeValue = pair.Value;
                                if (usableRangeValue == 0)
                                    continue;
                                var direction = (point - Owner.Position).ToDirection();
                                double evaluation = 0;
                                //効果範囲内にいるターゲットそれぞれについてスキルの点数を求め、足し合わせる
                                foreach (var target in Stage.Instance.ActiveUnits)
                                {
                                    evaluation += (skill.EffectiveRange.GetValue(point, target.Position, direction) ?? 0) * usableRangeValue * skill.Evaluate(target);
                                }
                                if (evaluation <= 0)
                                    continue;
                                //足し合わせた点数の最大値を記憶しておく
                                if (evaluationMax == null || evaluationMax.Value < evaluation)
                                {
                                    evaluationMax = evaluation;
                                    if(Owner.Position.EqualsRoughly(new Point(i, j)))
                                    {
                                        skillbuff = skill;
                                        skillDirectionbuff = point;
                                    }
                                }
                            }

                        }
                        //その座標でのスキルの点数の最大値を点数マップに足す
                        if (evaluationMax != null)
                            map[i, j] += evaluationMax;
                    }
                }

                double? max = null;
                int? ibuff = null;
                int? jbuff = null;
                //点数の一番高いところに移動する
                //今いる場所が一番の場合、スキルやアイテムを使う
                for(var i = 0;i < map.GetLength(0);i++)
                {
                    for(var j = 0;j < map.GetLength(1);j++)
                    {
                        var value = map[i, j];
                        if (value == null)
                            continue;
                        if(max == null || max.Value < value)
                        {
                            max = value;
                            ibuff = i;
                            jbuff = j;
                        }
                    }
                }
                if (max == null)
                    break;

                //移動しない
                if (Owner.Position.EqualsRoughly(new Point(ibuff.Value, jbuff.Value)))
                {
                    if(skillbuff != null)
                    {
                        Debug.Log("Skill = " + skillbuff.ToString() + ", x = " + ibuff.ToString() + ", y = " + jbuff.ToString() + ", point = " + map[ibuff.Value, jbuff.Value].ToString());

                        coroutine = skillbuff.Perform(Field.Instance[skillDirectionbuff.Value]);
                        while (coroutine.MoveNext()) yield return null;
                    }
                    else
                    {
                        Debug.Log("None");

                        break;
                    }
                }
                //移動する
                else
                {
                    Debug.Log("x = " + ibuff.ToString() + ", y = " + jbuff.ToString() + ", point = " + map[ibuff.Value, jbuff.Value].ToString());

                    coroutine = move.Shift(paths[ibuff.Value, jbuff.Value]);
                    while (coroutine.MoveNext()) yield return null;
                }
            }
            var wait = Owner.Actions.OfType<Wait>().First();
            coroutine = wait.Do();
            while (coroutine.MoveNext()) yield return true;

            yield return null;
        }
    }
}