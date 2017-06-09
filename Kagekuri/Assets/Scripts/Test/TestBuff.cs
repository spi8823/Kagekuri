using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class TestBuff : Condition
    {
        public TestBuff(ActiveUnit owner) : base(owner)
        {
            Duration = 100;
        }

        public override IEnumerator Affect()
        {
            Owner.Status.Agility *= 2;
            yield return null;
        }

        public override void Overlaid(Condition condition)
        {
            Duration += condition.Duration;
        }
    }
}