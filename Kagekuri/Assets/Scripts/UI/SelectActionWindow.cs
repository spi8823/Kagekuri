using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kagekuri
{
    public class SelectActionWindow : Singleton<SelectActionWindow>
    {
        private int _SelectedIndex = 0;
        private RectTransform _Panel;
        private RectTransform _Arrow;
        private GameObject _ButtonPrefab;

        protected override void Awake()
        {
            base.Awake();
            _Panel = GameObject.Find("/Canvas/SelectActionWindow").transform as RectTransform;
            _Arrow = GameObject.Find("/Canvas/SelectActionWindow/ArrowPanel/Arrow").transform as RectTransform;
            _Panel.gameObject.SetActive(false);
        }

        public IEnumerator<Action> Show(List<Action> actions)
        {
            _Panel.gameObject.SetActive(true);
            _Panel.anchorMax = new Vector2(0.2f, 0.4f + 0.3f * actions.Count / 4f);
            if (_ButtonPrefab == null)
                _ButtonPrefab = Resources.Load<GameObject>("Prefab/UI/SelectActionButton");

            var buttons = new List<GameObject>();
            for(var i = 0;i < actions.Count;i++)
            {
                var r = Instantiate(_ButtonPrefab).transform as RectTransform;
                r.SetParent(_Panel.GetChild(0));
                r.sizeDelta = new Vector2(0, 0);
                r.localPosition = new Vector3();
                r.anchoredPosition = new Vector2();
                r.anchorMin = new Vector2(0, 1 - 1f / actions.Count * (i + 1));
                r.anchorMax = new Vector2(1, 1 - 1f / actions.Count * i);
                buttons.Add(r.gameObject);
                var text = r.GetComponentInChildren<Text>();
                text.text = actions[i].Name;
                text.color = actions[i].IsAvailable() ? Color.white : Color.gray;
            }

            Action action = null;
            while (true)
            {
                if(InputController.GetButtonDown(InputController.Button.B))
                {
                    action = null;
                    break;
                }

                if(InputController.GetButtonDown(InputController.Button.A) && actions[_SelectedIndex].IsAvailable())
                {
                    action = actions[_SelectedIndex];
                    break;
                }

                _SelectedIndex = (_SelectedIndex - InputController.GetAxisPeriodicly(InputController.Axis.Cross_Vertical) + actions.Count) % actions.Count;
                _Arrow.anchorMin = new Vector2(0, 1 - 1f / actions.Count * (_SelectedIndex + 1) + 0.05f);
                _Arrow.anchorMax = new Vector2(1, 1 - 1f / actions.Count * _SelectedIndex - 0.05f);
                yield return null;
            }

            foreach (var button in buttons)
                Destroy(button);

            _Panel.gameObject.SetActive(false);
            yield return action;
        }
    }
}