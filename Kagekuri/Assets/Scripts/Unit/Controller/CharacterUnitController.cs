using UnityEngine;
using System.Collections;

namespace Kagekuri
{
    public class CharacterUnitController : ActiveUnitController
    {
        public new CharacterUnit Unit
        {
            get { return base.Unit as CharacterUnit; }
            protected set { base.Unit = value as ActiveUnit; }
        }
    }
}