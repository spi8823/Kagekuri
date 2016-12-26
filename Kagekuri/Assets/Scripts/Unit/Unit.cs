using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public abstract class Unit : MonoBehaviour
    {
        public Point Position { get; protected set; }
        public Point Direction { get; protected set; }
        public string Discription { get; protected set; }

        public virtual void SetPosition(Point position)
        {
            var square = BattleSceneManager.Instance.Stage.Field[position];
            if (square.Unit != null)
                return;
            BattleSceneManager.Instance.Stage.Field[Position].SetUnit(null);
            position.Z = square.Height;
            Position = position;
            transform.position = Position.ToUnityPosition();
            square.SetUnit(this);
        }
    }
}