using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Kagekuri
{
    public abstract class Action
    {
        public CharacterUnit Unit { get; protected set; }

        public Action(CharacterUnit unit)
        {
            Unit = unit;
        }

        /// <summary>
        /// 行動を実行
        /// </summary>
        /// <returns>完了したかどうか（キャンセルされたらfalseを返す）</returns>
        public abstract IEnumerator Do();

        public abstract bool IsAvailable();
    }

    public class Move : Action
    {
        public const int MoveCost = 5;
        public Range Range { get; protected set; }

        public Move(CharacterUnit unit)
            : base(unit)
        {
            Range = new Range();
            Range.Points.Add(Point.Right, 1);
            Range.Points.Add(Point.Left, 1);
            Range.Points.Add(Point.Up, 1);
            Range.Points.Add(Point.Down, 1);
        }

        public override IEnumerator Do()
        {
            yield break;
        }

        public override bool IsAvailable()
        {
            return false;
        }

        public virtual bool IsMovable(Square square)
        {
            bool movable = true;

            if (square.Type != SquareType.Normal)
                movable = false;
            if (square.Unit != null)
                movable = false;

            return movable;
        }
    }

    public class UseItem : Action
    {
        public UseItem(CharacterUnit unit)
            : base(unit)
        {

        }

        public override IEnumerator Do()
        {
            yield break;
        }

        public override bool IsAvailable()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 待機行動
    /// </summary>
    public class Charge : Action
    {
        public Charge(CharacterUnit unit)
            : base(unit)
        {

        }

        public override IEnumerator Do()
        {
            yield break;
        }

        public override bool IsAvailable()
        {
            return false;
        }
    }

    public class UseSkill : Action
    {
        public List<Skill> Skills { get; protected set; }

        public UseSkill(CharacterUnit unit, List<SkillData> skillDatas)
            :base(unit)
        {
            InitializeSkills(unit, skillDatas);
        }

        /// <summary>
        /// スキルを使用する
        /// スキルの選択→スキルの使用
        /// </summary>
        /// <returns>完了されたかどうか（キャンセルされたらfalse）</returns>
        public override IEnumerator Do()
        {
            IEnumerator coroutine;

            while (true)
            {
                coroutine = SelectSkill();

                //スキル選択を実行
                yield return new WaitWhile(() => coroutine.MoveNext());

                //スキルの選択結果がnullだったら（キャンセルされたら）
                if (coroutine.Current == null)
                {
                    //falseを返す
                    yield return false;
                    yield break;
                }
                //スキルの選択結果がnullでなければ（スキルが選択されたら）
                else
                {
                    Skill skill = (Skill)coroutine.Current;
                    coroutine = skill.Use();

                    //スキルを使用
                    yield return new WaitWhile(() => coroutine.MoveNext());

                    //スキルの使用された
                    if ((bool)coroutine.Current)
                    {
                        yield return true;
                        yield break;
                    }
                    //スキルがキャンセルされたらスキル選択をやり直し
                    else
                        continue;
                }
            }
        }

        private IEnumerator SelectSkill()
        {
            yield break;
        }

        public override bool IsAvailable()
        {
            return false;
        }

        private void InitializeSkills(CharacterUnit unit, List<SkillData> skillDatas)
        {
            foreach(var data in skillDatas)
            {
                var skill = Skill.GetSkill(unit, data);
                Skills.Add(skill);
            }
        }
    }
}