using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kagekuri
{
    public abstract class UnitController : MonoBehaviour
    {
        public Unit Unit { get; protected set; }

        public Square Square { get; protected set; }

        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void Update() { }
    }
}