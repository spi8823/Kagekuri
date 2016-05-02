using UnityEngine;
using System.Collections;


namespace Kagekuri
{
    public enum SkillType
    {
        Test, 
    }

    /// <summary>
    /// キャラクターが持つ技
    /// クラスメソッド：Skill.GetSkill(SkillType)でインスタンスを取得
    /// IsAvailable()で使用可能か判定
    /// Do()で使用
    /// </summary>
    public abstract class Skill
    {
        public int RequiredAP { get; protected set; }
        public int RequiredMP { get; protected set; }

        public Range Range { get; protected set; }
        public int Level { get; protected set; }
        public int ExP { get; protected set; }

        public Skill(CharacterUnit unit, SkillData data)
        {
            SetLevel(data.Level);
            ExP = data.ExP;
        }

        public virtual IEnumerator Use()
        {
            Debug.Log("実装されてないよ！！");
            yield break;
        }

        protected virtual void SetLevel(int level)
        {
            Level = level;
            Range = new Range();
        }

        public virtual bool IsAvailable() { return true; }

        public static Skill GetSkill(CharacterUnit unit, SkillData data)
        {
            switch(data.Type)
            {
                case SkillType.Test:
                    break;  
            }
            return null;
        }
    }

    public class SkillData
    {
        public SkillType Type;
        public int Level;
        public int ExP;
    }
}