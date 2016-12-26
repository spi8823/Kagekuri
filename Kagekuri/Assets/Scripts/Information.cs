using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class Information
    {
        public static int Money { get; set; }
        public static Dictionary<ActiveUnitType, ActiveUnitData> Allies;
    }
}