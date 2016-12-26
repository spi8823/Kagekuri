using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class QuarterviewCamera : Singleton<QuarterviewCamera>
    {
        private Camera _Camera;
        public Unit Target;
        public Point Position;
        public void Update()
        {
            if (Target != null) ;
        }

        public void LookAt(Point position)
        {

        }
    }
}