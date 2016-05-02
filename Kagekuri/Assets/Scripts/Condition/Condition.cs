using UnityEngine;
using System.Collections;

namespace Kagekuri
{
    /// <summary>
    /// 状態異常
    /// 攻撃力アップや素早さダウンなど
    /// ユニットがリストで所持している
    /// そのユニットが行動するとき、あるいはそのユニットに対して行動がなされるとき、Affetを実行する
    /// </summary>
    public abstract class Condition
    {
        public CharacterUnit Unit { get; protected set; }
        public double StartTime { get; protected set; }

        public Condition(CharacterUnit unit, double time)
        {
            Unit = unit;
            StartTime = time;
        }

        public abstract bool Affect(AffectType type);
    }

    public enum AffectType
    {

    }
}