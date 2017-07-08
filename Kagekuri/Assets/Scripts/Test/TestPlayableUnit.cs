using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class TestPlayableUnit : PlayableUnit
    {
        public override IEnumerator Act()
        {
            var coroutine = base.Act();
            while (coroutine.MoveNext()) yield return null;
        }
    }
}