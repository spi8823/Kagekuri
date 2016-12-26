using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class SkillLog : Condition
    {
        public List<Skill> List { get; protected set; }

        public SkillLog(ActiveUnit owner)
            : base(owner)
        {
            List = new List<Skill>();
        }

        public override IEnumerator Affect()
        {
            yield return null;
        }

        public void Add(Skill skill)
        {
            List.Add(skill);
        }
    }
}