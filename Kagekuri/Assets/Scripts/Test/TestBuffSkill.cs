using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class TestBuffSkill : Skill
    {
        public override int CostSP { get { return 8; } }
        public override int CostAP { get { return 5; } }
        public override Range UsableRange { get { return new Range(3, 1, distance => 1); } }
        public override Range EffectiveRange { get { return new Range(0, 0, distance => 1); } }
        public override Func<Square, bool> Selector { get { return null; } }

        public TestBuffSkill(SkillData data, ActiveUnit owner) : base(data, owner) { }

        public override IEnumerator<bool?> Perform(Square target)
        {
            var coroutine = base.Perform(target);
            while (coroutine.MoveNext()) yield return null;

            if(target.Unit is ActiveUnit)
            {
                var unit = target.Unit as ActiveUnit;
                unit.SetSPMax();
                StatusPanel.Instance.ShowSub(unit);
            }
            yield return false;
        }

        public override double Evaluate(ActiveUnit unit)
        {
            return 1;
        }
    }
}