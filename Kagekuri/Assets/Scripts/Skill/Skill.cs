using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Kagekuri
{
    public abstract class Skill
    {
        public ActiveUnit Owner;
        public string Name { get { throw new NotImplementedException(); } }
        public string Discription { get { throw new NotImplementedException(); } }
        public int Level { get; protected set; }
        public int Exp { get; protected set; }
        public virtual int CostSP { get { throw new NotImplementedException(); } }
        public virtual int CostAP { get { throw new NotImplementedException(); } }
        public virtual Range UsableRange { get { throw new NotImplementedException(); } }
        public virtual Range EffectiveRange { get { throw new NotImplementedException(); } }
        public virtual Func<Square, bool> Selector { get { throw new NotImplementedException(); } }

        public Skill(SkillData data, ActiveUnit owner)
        {
            Owner = owner;
            Level = data.Level;
            Exp = data.Exp;
        }

        public virtual bool IsAvailable()
        {
            return (CostSP <= Owner.Status.SP) && (CostAP <= Owner.Status.AP); 
        }

        /// <summary>
        /// スキルが選択された（使用された）ときに呼ぶ関数
        /// </summary>
        /// <returns>スキルを使用した後、ターンを終了するかどうか</returns>
        public IEnumerator<bool?> Use()
        {
            IEnumerator coroutine = BattleSceneManager.Instance.Stage.Field.SelectSquare(Owner.Position, UsableRange + Owner.Position, Selector);
            while (coroutine.MoveNext()) yield return null;
            BattleSceneManager.Instance.Stage.Field.DeletePanels();

            var square = coroutine.Current as Square;
            if(square == null)
            {
                yield return false;
                yield break;
            }
            var value = UsableRange[square.Position - Owner.Position];
            coroutine = Perform(square, value);
            while (coroutine.MoveNext()) yield return null;
            var result = (bool?)coroutine.Current;
            coroutine = Owner.ConsumeSP(CostSP);
            while (coroutine.MoveNext()) yield return null;
            coroutine = Owner.ConsumeAP(CostAP);
            while (coroutine.MoveNext()) yield return null;
            yield return result;
        }

        /// <summary>
        /// スキルを実行したときの関数
        /// オーバーライトして
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract IEnumerator<bool?> Perform(Square target, double value);

        public static Skill Get(SkillData data, ActiveUnit owner)
        {
            Skill skill = null;
            switch(data.Type)
            {
                case SkillType.TestAttackSkill:
                    skill = new TestAttackSkill(data, owner);
                    break;
                case SkillType.TestBuffSkill:
                    skill = new TestBuffSkill(data, owner);
                    break;
            }
            return skill;
        }
    }

    public enum SkillType
    {
        TestAttackSkill, TestBuffSkill
    }

    [JsonObject("SkillData")]
    [System.Serializable]
    public class SkillData
    {
        [JsonProperty("Type")]
        public SkillType Type;
        [JsonProperty("Level")]
        public int Level;
        [JsonProperty("Exp")]
        public int Exp;
    }
}