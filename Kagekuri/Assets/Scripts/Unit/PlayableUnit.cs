using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kagekuri
{
    public class PlayableUnit : ActiveUnit
    {
        public override IEnumerator Act()
        {
            var coroutine = base.Act();
            while (coroutine.MoveNext()) yield return true;

            while(true)
            {
                StatusPanel.Instance.ShowMain(this);
                coroutine = SelectAction();
                while (coroutine.MoveNext()) yield return null;
                var action = coroutine.Current as Action;
                if(action != null)
                {
                    coroutine = action.Do();
                    while (coroutine.MoveNext()) yield return null;
                    var result = coroutine.Current as bool?;
                    if (result ?? false)
                        break;
                }
                else
                {
                    coroutine = BattleSceneManager.Instance.Stage.Field.ViewField(Position);
                    while (coroutine.MoveNext()) yield return null;
                }
            }
        }

        public IEnumerator<Action> SelectAction()
        {
            var coroutine = SelectActionWindow.Instance.Show(Actions);
            while (coroutine.MoveNext()) yield return null;
            yield return coroutine.Current;
        }
    }
}