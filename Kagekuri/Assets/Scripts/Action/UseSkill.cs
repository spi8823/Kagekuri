using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class UseSkill : Action
    {
        public UseSkill(ActiveUnit owner) : base(owner)
        {
            Name = "スキル";
        }

        public override IEnumerator<bool?> Do()
        {
            while (true)
            {
                IEnumerator coroutine = SelectSkill();
                while (coroutine.MoveNext()) yield return null;

                var skill = coroutine.Current as Skill;
                if (skill == null)
                    yield break;

                coroutine = skill.Select();
                while (coroutine.MoveNext()) yield return null;

                var result = coroutine.Current as bool?;

                if (result != null)
                {
                    Owner.SkillLog.Add(skill);
                    coroutine = Owner.ConsumeAP(skill.CostAP);
                    while (coroutine.MoveNext()) yield return null;
                    coroutine = Owner.ConsumeSP(skill.CostSP);
                    while (coroutine.MoveNext()) yield return null;

                    yield return result;
                    yield break;
                }
            }
        }

        public override bool IsAvailable()
        {
            return Owner.Skills.Any(s => s.IsAvailable());
        }

        public IEnumerator<Skill> SelectSkill()
        {
            Debug.Log("実装してない");
            yield return Owner.Skills[0];
        }
    }
}