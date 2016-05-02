using UnityEngine;
using System.Collections;
using System;

namespace Kagekuri
{
    public abstract class ActiveUnitController : UnitController
    {
        public new ActiveUnit Unit
        {
            get { return Unit as ActiveUnit; }
            protected set { base.Unit = value as Unit; }
        }

        public double ActivePoint { get; protected set; }
    }
}