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

        private IEnumerator _ButtleCoroutine;
        private IEnumerator _EventCoroutine;
        public bool IsExclusiveEvent;

        public Stage(StageData data)
        {
            Field = new Field(data.FieldData, this);
            InitializeEventScript(data.EventNumber);
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

            while (!isButtleEnd && !isEventEnd)
            {
                if (!IsExclusiveEvent && !isButtleEnd)
                    isButtleEnd = !_ButtleCoroutine.MoveNext();

                if (!isEventEnd)
                    isEventEnd = !_EventCoroutine.MoveNext();

                yield return null;
            }

            yield break;
        }

        private IEnumerator ProceedButtle()
        {
            IEnumerator coroutine;

            coroutine = StartButtle();
            yield return new WaitWhile(() => coroutine.MoveNext());

            while(!JudgeButtleEnd())
            {
                coroutine = Buttle();
                yield return new WaitWhile(() => coroutine.MoveNext());

                if (!(bool)coroutine.Current)
                    break;
            }

            coroutine = EndButtle();
            yield return new WaitWhile(() => coroutine.MoveNext());

            yield break;
        }

        private IEnumerator StartButtle()
        {
            yield break;
        }

        private IEnumerator Buttle()
        {
            double? minWaitTime = null;
            foreach (var activeUnit in Field.ActiveUnits)
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

            foreach (var activeUnit in Field.ActiveUnits)
            {
                if (activeUnit.Wait((double)minWaitTime))
                {
                    IEnumerator act = activeUnit.Act();
                    yield return new WaitWhile(() => act.MoveNext());
                }
            }

            yield return true;
            yield break;
        }

        private bool JudgeButtleEnd()
        {
            return true;
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