using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class MapObject : Unit
    {
        public static MapObject Create(MapObjectData data)
        {
            MapObject mo = null;
            switch(data.Name)
            {
            }
            return mo;
        }
    }

    public class MapObjectData
    {
        public string Name;
        public string Tag;
        public string Discription;
        public Point Position;
        public Dictionary<string, string> Parameters;
    }
}