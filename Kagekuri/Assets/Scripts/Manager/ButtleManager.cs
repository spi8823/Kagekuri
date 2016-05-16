using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kagekuri
{
    public class ButtleManager : MonoBehaviour
    {
        public GameManager GameManager { get; private set; }
        public Stage Stage { get; private set; }
        private IEnumerator _StageCoroutine;
        public bool IsPaused { get; private set; }

        private void Awake()
        {
            Initialize(null, Test.GetStageData());
        }

        public void Initialize(GameManager gameManager, StageData data)
        {
            IsPaused = false;
            GameManager = gameManager;
            Stage = new Stage(data);
            _StageCoroutine = Stage.Proceed();
        }

        // Use this for initialization
        private void Start()
        {
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