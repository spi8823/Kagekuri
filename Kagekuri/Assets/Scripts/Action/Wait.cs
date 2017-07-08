using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class Wait : Action
    {
        public Wait(ActiveUnit owner) : base(owner)
        {
            Name = "待機";
        }

        public override IEnumerator<bool?> Do()
        {
            if (Owner.Status.MaxAP <= Owner.Status.AP)
                Owner.SetIsCharging(true);
            else
            {
                Owner.SetAP(0);
                Owner.SetIsCharging(false);
            }
            yield return true;
        }

        public override bool IsAvailable()
        {
            return true;
        }
    }
}