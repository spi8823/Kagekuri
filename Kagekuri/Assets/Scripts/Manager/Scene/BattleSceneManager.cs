using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kagekuri
{
    public class BattleSceneManager : Singleton<BattleSceneManager>
    {
        public Stage Stage { get; private set; }
        public bool IsPausing { get; private set; }
        private IEnumerator _BattleCoroutine;

        protected override void Awake()
        {
            InputController.Update();
            base.Awake();
            IsPausing = false;
            Stage = new Stage(DataManager.Instance.SelectedStage);
            foreach(var data in DataManager.Instance.StartPositions ?? new Dictionary<ActiveUnitType, Point>())
            {
                Information.Allies[data.Key].Position = data.Value;
                var unit = ActiveUnit.Create(Information.Allies[data.Key]);
                Stage.Field[unit.Position].SetUnit(unit);
                Stage.Units.Add(unit);
            }
            _BattleCoroutine = Proceed();
        }

        public void Start()
        {
        }

        public void Update()
        {
            InputController.Update();
            if (InputController.GetButtonDown(InputController.Button.Start))
                IsPausing = true;

            if(!IsPausing)
            {
                if (_BattleCoroutine == null) return;
                if (!_BattleCoroutine.MoveNext())
                    Application.Quit();
            }
        }

        public IEnumerator Proceed()
        {
            var coroutine = ProceedStartPhase();
            while (coroutine.MoveNext()) yield return null;

            coroutine = ProceedBattlePhase();
            while (coroutine.MoveNext()) yield return null;

            coroutine = ProceedEndPhase();
            while (coroutine.MoveNext()) yield return null;
        }

        public IEnumerator ProceedStartPhase()
        {
            yield return null;
        }

        public IEnumerator ProceedBattlePhase()
        {
            while(CheckBattleProceed())
            {
                var elapseTime = Stage.ActiveUnits.Min(u => u.GetWaitTime());
                foreach(var activeUnit in Stage.ActiveUnits)
                {
                    activeUnit.Elapse(elapseTime);
                }

                var unit = Stage.ActiveUnits.Find(u => u.GetWaitTime() <= 0.01);
                unit.SetAPMax();
                Debug.Log(unit.ToString());
                var coroutine = unit.Act();
                while (coroutine.MoveNext()) yield return null;
            }
        }

        public bool CheckBattleProceed()
        {
            if (!Stage.ActiveUnits.Any(u => u.Status.Camp == Camp.Ally && !u.IsDead) || !Stage.ActiveUnits.Any(u => u.Status.Camp == Camp.Hostile && !u.IsDead))
                return false;
            else
                return true;
        }

        public IEnumerator ProceedEndPhase()
        {
            Debug.Log("Buttle End");
            yield return null;
        }
    }
}