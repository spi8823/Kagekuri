using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kagekuri
{
    public class SelectSkillWindow : Singleton<SelectSkillWindow>
    {
        private const int RowCount = 6;
        private const int ColumnCount = 4;
        private const int ButtonWidth = 100;
        private const int ButtonHeight = 50;
        private RectTransform _Panel;
        private GameObject _ButtonPrefab = null;

        protected override void Awake()
        {
            base.Awake();
        }

        public IEnumerator<Skill> Show(List<Skill> skills)
        {
            _Panel.gameObject.SetActive(true);

            var x = 0;
            var y = 0;

            var showingRow = 0;
            DisplaySkills(skills, showingRow);

            Skill skill = null;

            while(true)
            {
                //決定ボタンが押された
                if(InputController.GetButtonDown(InputController.Button.A))
                {
                    if (x + y * ColumnCount < skills.Count)
                    {
                        skill = skills[x + y * ColumnCount];
                        break;
                    }
                }

                //キャンセルボタンが押された
                if(InputController.GetButtonDown(InputController.Button.B))
                {
                    skill = null;
                    break;
                }

                //選択位置の更新
                x += InputController.GetAxisPeriodicly(InputController.Axis.Cross_Horizontal);
                x = (x + ColumnCount) % ColumnCount;

                y += InputController.GetAxisPeriodicly(InputController.Axis.Cross_Vertical);
                if(y < 0)
                {
                    showingRow = Mathf.Max(showingRow - 1, 0);
                    y = 0;
                    DisplaySkills(skills, showingRow);
                }
                else if(RowCount <= y)
                {
                    showingRow = Mathf.Min(showingRow + 1, Mathf.Max(0, skills.Count / ColumnCount - 1));
                    y = RowCount - 1;
                    DisplaySkills(skills, showingRow);
                }

                var index = (showingRow + y) * ColumnCount + x;

                //選択中のスキルの詳細を表示
                if(index < skills.Count)
                {
                    ShowSkillDetail(skills[index]);
                }
            }

            _Panel.gameObject.SetActive(false);
            yield return skill;
        }

        private void DisplaySkills(List<Skill> skills, int startRow)
        {
            if (_ButtonPrefab == null)
                _ButtonPrefab = Resources.Load<GameObject>("SkillButtonPrefab");

            for(var i = 0;i < Mathf.Min(Mathf.CeilToInt((float)skills.Count / ColumnCount) - startRow, RowCount);i++)
            {
                for(var j = 0;j < Mathf.Min(skills.Count - (startRow + i) * ColumnCount, ColumnCount);j++)
                {
                    var skill = skills[(startRow + i) * ColumnCount + j];
                    var g = Instantiate(_ButtonPrefab);
                    g.transform.SetParent(transform);
                    g.GetComponentInChildren<Text>().text = skill.Name;
                    g.transform.localPosition = new Vector3(j * ButtonWidth, (startRow + i) * ButtonHeight, 0);
                }
            }
        }

        private void ShowSkillDetail(Skill skill)
        {

        }
    }
}