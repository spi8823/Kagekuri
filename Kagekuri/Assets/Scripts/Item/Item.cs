using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Kagekuri
{
    public abstract class Item
    {
        public ActiveUnit Owner { get; protected set; }
        public string Name { get; protected set; }
        public string Discription { get; protected set; }
        public ItemGroup Group { get; protected set; }
        public int Count { get; protected set; }
        public int CostAP { get; protected set; }

        public Item(ItemData data, ActiveUnit owner)
        {
            Owner = owner;
            Count = data.Count;
        }

        /// <summary>
        /// アイテムを使う
        /// </summary>
        /// <returns>ターンを終了するかどうか nullならキャンセルされた</returns>
        public abstract IEnumerator<bool?> Use();

        public static Item Get(ItemData data, ActiveUnit owner)
        {
            Item item = null;
            switch(data.Type)
            {
            }

            return item;
        }
    }

    public enum ItemType
    {
    }

    public enum ItemGroup
    {
        Supply, Goods, Equipment, Event
    }

    [JsonObject("ItemData")]
    [System.Serializable]
    public class ItemData
    {
        [JsonProperty("Type")]
        public ItemType Type;
        [JsonProperty("Count")]
        public int Count;
    }
}