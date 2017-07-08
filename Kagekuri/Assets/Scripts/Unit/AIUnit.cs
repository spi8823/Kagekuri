using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class AIUnit : ActiveUnit
    {
        public virtual AI AI { get; protected set; }

        public override IEnumerator Act()
        {
            var coroutine = base.Act();
            while (coroutine.MoveNext()) yield return null;

            coroutine = AI.Automate();
            while (coroutine.MoveNext()) yield return null;
        }
    }
}