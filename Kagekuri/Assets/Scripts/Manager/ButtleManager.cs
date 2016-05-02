using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kagekuri
{
    public class ButtleManager : MonoBehaviour
    {
        private GameManager _GameManager;
        private Stage _Stage;
        private IEnumerator _StageCoroutine;
        private bool IsPaused = false;

        private void Awake()
        {
            Initialize(null, Test.GetStageData());
        }

        public void Initialize(GameManager gameManager, StageData data)
        {
            _GameManager = gameManager;
            _Stage = new Stage(data);
        }

        // Use this for initialization
        private void Start()
        {
            _StageCoroutine = _Stage.Proceed();
        }

        // Update is called once per frame
        private void Update()
        {
            if (InputController.GetButtonDown(InputController.Button.Start))
                IsPaused = !IsPaused;

            if(!IsPaused)
            {
                _StageCoroutine.MoveNext();
            }
            else
            {

            }
        }
    }
}