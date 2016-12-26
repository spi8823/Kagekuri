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
                    return;
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
                var activeUnits = Stage.Units.FindAll(u => u is ActiveUnit).Select(u => u as ActiveUnit).ToList();
                var elapseTime = activeUnits.Min(u => u.GetWaitTime());
                foreach(var activeUnit in activeUnits)
                {
                    activeUnit.Elapse(elapseTime);
                }

                var unit = activeUnits.Find(u => u.GetWaitTime() <= 0.01);
                unit.SetAPMax();
                var coroutine = unit.Act();
                while (coroutine.MoveNext()) yield return null;
            }
        }

        public bool CheckBattleProceed()
        {
            return true;
        }

        public IEnumerator ProceedEndPhase()
        {
            yield return null;
        }
    }
}