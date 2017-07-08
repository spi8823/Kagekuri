using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class Death : Condition
    {
        public Death(ActiveUnit owner) : base(owner)
        {
        }

        public override IEnumerator Affect()
        {
            Owner.SetAP(0);
            yield return null;
        }

        public override void Overlaid(Condition condition)
        {
        }
    }
}
