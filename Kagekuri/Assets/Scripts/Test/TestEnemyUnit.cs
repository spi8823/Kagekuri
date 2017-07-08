using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kagekuri
{
    public class TestEnemyUnit : AIUnit
    {
        public override void Initialize(ActiveUnitData data)
        {
            base.Initialize(data);
            AI = new AI(this);
        }
    }
}