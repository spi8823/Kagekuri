using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class TestPlayableUnit : PlayableUnit
    {
        public override IEnumerator Act()
        {
            Debug.Log("Test Playable Unit Start");
            var coroutine = base.Act();
            while (coroutine.MoveNext()) yield return null;
            Debug.Log("Test Playable Unit End");
        }
    }
}