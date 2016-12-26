using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kagekuri
{
    public class Square : MonoBehaviour
    {
        private static GameObject _SquarePrefab = null;
        public Field Field { get; private set; }
        public Unit Unit { get; private set; }

        public Point Position { get; private set; }
        public string Discription { get; private set; }

        public double Height { get; private set; }
        public List<Block> Blocks { get; private set; }
        public SquareType Type { get; private set; }

        public void SetType(string tag)
        {
            var type = SquareType.Normal;
            switch (tag)
            {
            }

            if (Blocks.Count == 0)
                type = SquareType.Null;

            Type = type;
        }

        public void SetUnit(Unit unit)
        {
            Unit = unit;
        }

        public static Square Create(SquareData data, Field field)
        {
            if (_SquarePrefab == null)
                _SquarePrefab = Resources.Load<GameObject>("Prefab/Stage/SquarePrefab");

            var square = Instantiate(_SquarePrefab).GetComponent<Square>();

            square.Field = field;
            square.Unit = null;

            square.Position = data.Position;
            square.Discription = data.Discription;
            square.Height = data.Height;
            square.Blocks = new List<Block>();
            foreach(var item in data.Blocks)
            {
                var block = Block.Create(item, square);
                square.Blocks.Add(block);
                block.transform.SetParent(square.transform);
            }
            square.SetType(data.Tag);

            return square;
        }

        public class SquareData
        {
            public Point Position;
            public double Height;
            public string Discription;
            public string Tag;
            public List<Block.BlockData> Blocks;
        }
    }

    public enum SquareType
    {
        Null, Normal, 
    }
}