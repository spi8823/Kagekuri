using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Kagekuri
{
    /// <summary>
    /// バトル一回分の情報を持ってる
    /// つまり、Field(マップの情報)とEventScript(イベントの情報)
    /// </summary>
    public class Stage
    {
        public Field Field { get; private set; }
        public Script EventScript { get; private set; }

        public List<Unit> Units { get; private set; }
        public List<ActiveUnit> ActiveUnits { get; private set; }
        public List<CharacterUnit> CharacterUnits { get; private set; }

        private IEnumerator _ButtleCoroutine;
        private IEnumerator _EventCoroutine;
        public bool IsExclusiveEvent { get; private set; }

        public Stage(StageData data)
        {
            Field = new Field(data.FieldData, this);
            InitializeEventScript(data.EventNumber);

            _ButtleCoroutine = ProceedButtle();
            _EventCoroutine = ProceedEvent();
            IsExclusiveEvent = false;

            Units = new List<Unit>();
            ActiveUnits = new List<ActiveUnit>();
            CharacterUnits = new List<CharacterUnit>();
            foreach(var item in data.UnitDatas)
            {
                Unit unit = Unit.GetUnit(item, Field);
                Units.Add(unit);
            }
            foreach(var item in data.ActiveUnitDatas)
            {
                ActiveUnit unit = ActiveUnit.GetActiveUnit(item, Field);
                Units.Add(unit);
                ActiveUnits.Add(unit);
            }
            foreach(var item in data.CharacterUnitDatas)
            {
                CharacterUnit unit = CharacterUnit.GetCharacterUnit(item, Field);
                Units.Add(unit);
                ActiveUnits.Add(unit);
                CharacterUnits.Add(unit);
            }
        }

        public void InitializeEventScript(int eventNumber)
        {
            var assetName = "LuaScripts/" + eventNumber + ".lua";
            var asset = Resources.Load<TextAsset>(assetName);

            EventScript = new Script();
            EventScript.DoString(asset.text);
        }

        public IEnumerator Proceed()
        {
            bool isButtleEnd = false;
            bool isEventEnd = false;

            _ButtleCoroutine = ProceedButtle();
            _EventCoroutine = ProceedEvent();

            while (!isButtleEnd || !isEventEnd)
            {
                if (!IsExclusiveEvent && !isButtleEnd)
                    isButtleEnd = !_ButtleCoroutine.MoveNext();

                if (!isEventEnd)
                    isEventEnd = !_EventCoroutine.MoveNext();

                yield return true;
            }

            yield return false;
            yield break;
        }

        private IEnumerator ProceedButtle()
        {
            IEnumerator coroutine;

            //バトル開始
            coroutine = StartButtle();
            while (coroutine.MoveNext()) { yield return true; }

            //バトル本番
            while(!JudgeButtleEnd())
            {
                coroutine = Buttle();
                while (coroutine.MoveNext()) { yield return true; }

                bool? flag = coroutine.Current as bool?;
                if (flag != null && flag == false)
                    break;
            }

            //バトル終了
            coroutine = EndButtle();
            while (coroutine.MoveNext()) { yield return true; }

            yield return true;
            yield break;
        }

        /// <summary>
        /// バトル開始
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartButtle()
        {
            yield break;
        }

        /// <summary>
        /// バトル
        /// </summary>
        /// <returns></returns>
        private IEnumerator Buttle()
        {
            double? minWaitTime = null;
            foreach (var activeUnit in ActiveUnits)
            {
                if (activeUnit == null)
                    continue;

                double waitTime = activeUnit.GetWaitTime();

                if (minWaitTime == null || waitTime < minWaitTime)
                    minWaitTime = waitTime;
            }
            
            if (minWaitTime == null)
            {
                yield return false;
                yield break;
            }

            Debug.Log("MinWaitTime:" + minWaitTime);

            foreach (var activeUnit in ActiveUnits)
            {
                if (activeUnit.Wait((double)minWaitTime))
                {
                    Debug.Log("Act");
                    IEnumerator act = activeUnit.Act();
                    while (act.MoveNext()) { yield return null; }
                }
            }

            yield return true;
            yield break;
        }

        private bool JudgeButtleEnd()
        {
            return false;
        }

        private IEnumerator EndButtle()
        {
            yield break;
        }

        private IEnumerator ProceedEvent()
        {
            yield break;
        }
    }

    public class StageData
    {
        public FieldData FieldData;
        public List<CharacterUnitData> CharacterUnitDatas;
        public List<ActiveUnitData> ActiveUnitDatas;
        public List<UnitData> UnitDatas;
        public int EventNumber;
    }
}