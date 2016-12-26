using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class DataManager : Singleton<DataManager>
    {
        public Stages SelectedStage;
        public Dictionary<ActiveUnitType, Point> StartPositions;
    }
}