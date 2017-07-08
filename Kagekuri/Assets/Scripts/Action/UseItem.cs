using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class UseItem : Action
    {
        public UseItem(ActiveUnit owner) : base(owner)
        {
            Name = "アイテム";
        }

        public override IEnumerator<bool?> Do()
        {
            IEnumerator coroutine = SelectItem();
            while (coroutine.MoveNext()) yield return null;

            var item = coroutine.Current as Item;
            if (item == null)
                yield break;

            coroutine = item.Use();
            while (coroutine.MoveNext()) yield return null;

            yield return coroutine.Current as bool?;
        }

        public IEnumerator<Item> SelectItem()
        {
            Debug.Log("実装してない");
            yield return null;
        }

        public override bool IsAvailable()
        {
            return false;
        }
    }
}