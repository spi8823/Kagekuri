using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public abstract class Action
    {
        public string Name { get; protected set; }
        public string Dictionary { get; protected set; }
        public readonly ActiveUnit Owner;

        public Action(ActiveUnit owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// 行動を実行する
        /// </summary>
        /// <returns>ターンを終了するかどうか nullならばキャンセル</returns>
        public abstract IEnumerator<bool?> Do();

        public abstract bool IsAvailable();
    }
}