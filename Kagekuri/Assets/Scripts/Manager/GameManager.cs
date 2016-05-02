using UnityEngine;
using System.Collections;

namespace Kagekuri
{
    /// <summary>
    /// ゲーム全体を管理するコンポーネント
    /// 最初から最後まで生きてる
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            InputController.SetForKeyboard();
        }

        private void Start()
        {

        }

        private void Update()
        {
            InputController.Update();
        }
    }
}