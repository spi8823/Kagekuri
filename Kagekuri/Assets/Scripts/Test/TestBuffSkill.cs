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
        public override Range UsableRange
        {
            get
            {
                var range = new Range();
                for(var i = -3;i < 3;i++)
                {
                    for(var j = -3 + Math.Abs(i);j < 3 - Math.Abs(i);j++)
                    {
                        range.Add(new Point(i, j), 4 - (Math.Abs(i) + Math.Abs(j)));
                    }
                }
                return range;
            }
        }
        public override Range EffectiveRange
        {
            get
            {
                var range = new Range();
                range.Add(Point.Zero, 1);
                return range;
            }
        }

        public TestBuffSkill(SkillData data, ActiveUnit owner) : base(data, owner) { }

        public override bool IsAvailable()
        {
            return CostSP <= Owner.Status.SP && CostAP <= Owner.Status.AP;
        }

        public override IEnumerator Perform(Square square, double value)
        {
            if(square.Unit is ActiveUnit)
            {
                var unit = square.Unit as ActiveUnit;

            }
            yield break;
        }
    }
}