using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class TestAttackSkill : Skill
    {
        public override int CostSP { get { return 20; } }
        public override int CostAP { get { return 25; } }
        public override Range UsableRange { get { return new Range(2, 1, d => 1); } }
        public override Range EffectiveRange { get { return new Range(0, 0, d => 1); } }
        public override Func<Square, bool> Selector { get { return null; } }

        public TestAttackSkill(SkillData data, ActiveUnit owner) : base(data, owner)
        {
        }

        public override IEnumerator<bool?> Perform(Square target, double value)
        {
            var unit = target.Unit as ActiveUnit;
            if (unit == null)
                yield break;

            var coroutine = unit.Damaged(20);
            while (coroutine.MoveNext()) yield return null;
            yield return false;
        }
    }
}