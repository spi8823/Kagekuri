using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class TestAttackSkill : Skill
    {
        public override int CostSP { get { return 10; } }
        public override int CostAP { get { return 25; } }
        public override Range UsableRange { get { return new Range(2, 1, d => d == 0 ? 0 : 1); } }
        public override Range EffectiveRange { get { return new Range(0, 0, d => 1); } }
        public override Func<Square, bool> Selector { get { return null; } }

        public TestAttackSkill(SkillData data, ActiveUnit owner) : base(data, owner)
        {
        }

        public override IEnumerator<bool?> Perform(Square target)
        {
            IEnumerator coroutine = base.Perform(target);
            while (coroutine.MoveNext()) yield return null;

            var unit = target.Unit as ActiveUnit;
            if (unit == null)
                yield break;

            coroutine = unit.Damaged(20);
            while (coroutine.MoveNext()) yield return null;

            StatusPanel.Instance.ShowMain(Owner);
            StatusPanel.Instance.ShowSub(unit);
            yield return false;
        }

        /// <summary>
        /// このスキルを使うとどれくらい嬉しいかを評価
        /// AIに使う
        /// </summary>
        /// <param name="unit">スキルの対象</param>
        /// <returns>点数</returns>
        public override double Evaluate(ActiveUnit unit)
        {
            if (unit.IsDead)
                return 0;
            if (Owner.Status.Camp != unit.Status.Camp)
                return 100;
            else
                return 0;
        }
    }
}