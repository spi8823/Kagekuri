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
        public string Name { get; protected set; }
        public string Discription { get; protected set; }
        public int Level { get; protected set; }
        public int Exp { get; protected set; }
        public virtual int CostSP { get { throw new NotImplementedException(); } }
        public virtual int CostAP { get { throw new NotImplementedException(); } }
        public virtual Range UsableRange { get { throw new NotImplementedException(); } }
        public virtual Range EffectiveRange { get { throw new NotImplementedException(); } }

        public Skill(SkillData data, ActiveUnit owner)
        {
            Owner = owner;
            Level = data.Level;
            Exp = data.Exp;
        }

        public abstract bool IsAvailable();

        public IEnumerator<bool?> Use()
        {
            IEnumerator coroutine = BattleSceneManager.Instance.Stage.Field.SelectSquare(Owner.Position, UsableRange);
            while (coroutine.MoveNext()) yield return null;

            var square = coroutine.Current as Square;
            var value = UsableRange.Points[square.Position - Owner.Position];
            coroutine = Perform(square, value);
        }

        public abstract IEnumerator Perform(Square target, double value);

        public static Skill Get(SkillData data, ActiveUnit owner)
        {
            Skill skill = null;
            switch(data.Type)
            {
            }
            return skill;
        }
    }

    public enum SkillType
    {
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