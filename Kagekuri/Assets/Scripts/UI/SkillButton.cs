using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kagekuri
{
    public class SkillButton : MonoBehaviour
    {
        private static GameObject _Prefab = null;
        private const int Width = 160;
        private const int Height = 60;

        private Text SkillNameText = null;
        private Text CostSPText = null;
        private Text CostAPText = null;

        public Skill Skill { get; private set; }

        private void Awake()
        {
            SkillNameText = transform.Find("SkillNameText").GetComponent<Text>();
            CostSPText = transform.Find("CostSPText").GetComponent<Text>();
            CostAPText = transform.Find("CostAPText").GetComponent<Text>();
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        private void SetPosition(int x, int y)
        {
            transform.localPosition = new Vector3(Width * x, Height * y, 0);
        }

        private void SetSkill(Skill skill)
        {
            Skill = skill;
            SkillNameText.text = Skill.Name;
            CostSPText.text = Skill.CostSP.ToString();
            CostAPText.text = Skill.CostAP.ToString();
        }

        public static SkillButton Create(Transform parent, Skill skill, int x, int y)
        {
            if (_Prefab == null)
                _Prefab = Resources.Load<GameObject>("Prefab/UI/SkillButtonPrefab");

            var g = Instantiate(_Prefab);
            g.transform.SetParent(parent);

            var button = g.GetComponent<SkillButton>();
            button.Skill = skill;
            button.SetPosition(x, y);

            return button;
        }

    }
}