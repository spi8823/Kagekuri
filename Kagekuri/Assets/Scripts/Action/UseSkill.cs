using System;
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

                coroutine = skill.Use();
                while (coroutine.MoveNext()) yield return null;

                var result = coroutine.Current as bool?;

                if (result != null)
                {
                    if (!Owner.Conditions.Exists(c => c is SkillLog))
                        Owner.Conditions.Add(new SkillLog(Owner));

                    (Owner.Conditions.Find(c => c is SkillLog) as SkillLog).Add(skill);
                    yield return result;
                    yield break;
                }
            }
        }

        public override bool IsAvailable()
        {
            Debug.Log("実装してない");
            return true;
        }

        public IEnumerator<Skill> SelectSkill()
        {
            Debug.Log("実装してない");
            yield return null;
        }
    }
}