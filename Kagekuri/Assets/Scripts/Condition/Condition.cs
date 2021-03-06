﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public abstract class Condition
    {
        public ActiveUnit Owner { get; protected set; }
        public virtual int Duration { get; protected set; }
        public virtual double ElapsedTime { get; protected set; }

        public Condition(ActiveUnit owner)
        {
            Owner = owner; 
        }

        public bool Elapse(double time)
        {
            ElapsedTime += time;
            if (ElapsedTime < Duration)
                return false;
            else
                return true;
        }

        public abstract IEnumerator Affect();
        public abstract void Overlaid(Condition condition);
    }
}