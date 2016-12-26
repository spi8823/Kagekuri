using UnityEngine;
using System.Collections;

namespace Kagekuri
{
    public class Block : MonoBehaviour
    {
        private static GameObject _BlockPrefab = null;
        public Square Square { get; private set; }
        public Point Position { get; private set; }

        public void SetPosition(Point position)
        {
            Position = position;
            transform.position = Position.ToUnityPosition();
        }

        public void SetSprite(Sprite sprite)
        {
            var sr = GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
        }

        public static Block Create(BlockData data, Square square)
        {
            if (_BlockPrefab == null)
                _BlockPrefab = Resources.Load<GameObject>("Prefab/Stage/BlockPrefab");

            var block = Instantiate(_BlockPrefab).GetComponent<Block>();
            block.SetPosition(data.Position);
            block.SetSprite(GetMaptip(data.Name));

            return block;
        }

        public static Sprite GetMaptip(string name)
        {
            var sprite = Resources.Load<Sprite>("Texture/Maptip/" + name);
            return sprite;
        }

        public class BlockData
        {
            public Point Position;
            public string Name;
        }
    }

}