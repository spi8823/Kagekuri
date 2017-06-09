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

        public override IEnumerator<bool?> Perform(Square square, double value)
        {
            if(square.Unit is ActiveUnit)
            {
                var unit = square.Unit as ActiveUnit;
            }
            yield return false;
        }
    }
}