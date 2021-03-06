﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace Kagekuri
{
    public abstract class ActiveUnit : Unit
    {
        public const double ChargingAPRate = 1.5;
        public const double ChargingAgilityRate = 0.5;
        public ActiveUnitType Type { get; protected set; }
        public ActiveUnitStatus Status { get; protected set; }
        public bool IsDead { get { return Conditions.Any(c => c is Death); } }

        public List<Action> Actions { get; protected set; }
        public List<Skill> Skills { get; protected set; }
        public List<Condition> Conditions { get; protected set; }
        public List<Item> Items { get; protected set; }

        public List<Skill> SkillLog { get; protected set; }

        public bool IsCharging { get; protected set; }

        public double GetWaitTime(int turn = 0)
        {
            var time = 0.0;
            if (IsCharging)
                time += (Status.MaxAP * ChargingAPRate - Status.AP) / (Status.Agility * ChargingAgilityRate);
            else
                time += (Status.MaxAP - Status.AP) / Status.Agility;

            time += (double)Status.MaxAP / Status.Agility * turn;

            return Mathf.Max((float)time, 0);
        }

        public virtual IEnumerator Act()
        {
            Status.Reset();
            SkillLog = new List<Skill>();
            IEnumerator coroutine;
            foreach (var condition in Conditions)
            {
                coroutine = condition.Affect();
                while (coroutine.MoveNext()) yield return null;
            }

            StatusPanel.Instance.HideAll();
            StatusPanel.Instance.ShowMain(this);

            yield return null;
        }

        public virtual void SetIsCharging(bool isCharging)
        {
            if (isCharging)
                SetAP(Status.MaxAP);
            IsCharging = isCharging;
        }

        public virtual void InitializeActions()
        {
            Actions = new List<Action>();
            Actions.Add(new Move(this));
            Actions.Add(new UseSkill(this));
            Actions.Add(new UseItem(this));
            Actions.Add(new Wait(this));
        }

        public virtual void InitializeSkills(List<SkillData> datas)
        {
            Skills = new List<Skill>();
            foreach(var data in datas)
            {
                var skill = Skill.Get(data, this);
                Skills.Add(skill);
            }
        }

        public virtual void InitializeConditions()
        {
            Conditions = new List<Condition>();
        }

        public virtual void InitializeItems(List<ItemData> datas)
        {
            Items = new List<Item>();
        }

        public void Elapse(double time)
        {
            SetAP(Status.AP + time * Status.Agility);
        }

        public virtual void Initialize(ActiveUnitData data)
        {
            Status = new ActiveUnitStatus(data);
            Position = data.Position;
            transform.position = Position.ToUnityPosition();
            InitializeActions();
            InitializeSkills(data.Skills);
            InitializeConditions();
            InitializeItems(data.Items);
        }

        public void SetAP(double ap)
        {
            Status.AP = (int)Mathf.Min(Status.MaxAP, Mathf.Max((float)ap, 0));
        }

        public void SetAPMax()
        {
            Status.AP = (int)(IsCharging ? Status.MaxAP * ChargingAPRate : Status.MaxAP);
        }

        public void SetSP(double sp)
        {
            Status.SP = (int)Mathf.Min(Status.MaxSP, Mathf.Max((float)sp, 0));
        }

        public void SetSPMax()
        {
            Status.SP = Status.MaxSP;
        }

        public virtual IEnumerator Go(Square square)
        {
            Debug.Log("実装してない");
            SetPosition(square.Position + new Point(0, 0, square.Height));
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
        }

        public static ActiveUnit Create(ActiveUnitData data)
        {
            ActiveUnit unit = null;
            switch(data.Type)
            {
                case ActiveUnitType.TestPlayableUnit:
                    unit = Instantiate(Resources.Load<GameObject>("Prefab/Unit/Ally/TestPlayableUnit")).GetComponent<ActiveUnit>();
                    break;
                case ActiveUnitType.TestEnemyUnit:
                    unit = Instantiate(Resources.Load<GameObject>("Prefab/Unit/Enemy/TestEnemyUnit")).GetComponent<ActiveUnit>();
                    break;
            }
            unit.Initialize(data);
            return unit;
        }

        public virtual IEnumerator Damaged(int damage)
        {
            Status.HP -= damage;
            if (Status.HP <= 0)
            {
                Conditions.Add(new Death(this));
            }
            yield return null;
        }

        public virtual IEnumerator ConsumeSP(int sp)
        {
            SetSP(Status.SP - sp);
            yield return null;
        }

        public virtual IEnumerator ConsumeAP(int ap)
        {
            SetAP(Status.AP - ap);
            yield return null;
        }

        public virtual bool AddCondition(Condition condition, double probability)
        {
            var r = Random.Range(0, 1);
            if (r <= probability)
            {
                if (Conditions.Exists(c => c.GetType() == condition.GetType()))
                    Conditions.Find(c => c.GetType() == condition.GetType()).Overlaid(condition);
                else
                    Conditions.Add(condition);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 戦闘中、もろもろの計算に用いるステータス
    /// </summary>
    public class ActiveUnitStatus
    {
        public const int DefaultMaxAP = 100;

        public string Name;
        public string Discription;

        public int Level;
        public int Exp;

        public Camp Camp;

        public int AttackLevel;
        public int AttackExp;
        public int DefenceLevel;
        public int DefenceExp;
        public int DexterityLevel;
        public int DexterityExp;
        public int ResistanceLevel;
        public int ResistanceExp;

        public int RawMaxHP;
        public int RawMaxSP;
        public int RawAttack;
        public int RawDefence;
        public int RawDexterity;
        public int RawResistance;
        public int RawAgility;
        public int RawLuck;
        public int RawJump;

        public int MaxHP;
        public int MaxSP;
        public int MaxAP;

        public int HP;
        public int SP;
        public int AP;

        public int Attack;
        public int Defence;
        public int Dexterity;
        public int Resistance;
        public int Agility;
        public int Luck;
        public int Jump;

        public ActiveUnitStatus(ActiveUnitData data)
        {
            Name = data.Name;
            Discription = data.Discription;

            Camp = data.Camp;

            Level = data.Level;
            Exp = data.Exp;

            AttackLevel = data.AttackLevel;
            AttackExp = data.AttackExp;
            DefenceLevel = data.DefenceLevel;
            DefenceExp = data.DefenceExp;
            DexterityLevel = data.DexterityLevel;
            DexterityExp = data.DexterityExp;
            ResistanceLevel = data.ResistanceLevel;
            ResistanceExp = data.ResistanceExp;

            RawMaxHP = data.RawMaxHP;
            RawMaxSP = data.RawMaxSP;
            MaxHP = RawMaxHP;
            MaxSP = RawMaxSP;
            MaxAP = DefaultMaxAP;
            HP = MaxHP;
            SP = MaxSP;
            AP = 0;

            RawAttack = data.RawAttack;
            RawDefence = data.RawDefence;
            RawDexterity = data.RawDexterity;
            RawResistance = data.RawResistance;
            RawAgility = data.RawAgility;
            RawLuck = data.RawLuck;
            RawJump = data.RawJump;

            Attack = RawAttack;
            Defence = RawDefence;
            Dexterity = RawDexterity;
            Resistance = RawResistance;
            Agility = RawAgility;
            Luck = RawLuck;
            Jump = RawJump;
        }

        public void Reset()
        {
            MaxHP = RawMaxHP;
            MaxSP = RawMaxSP;
            MaxAP = DefaultMaxAP;

            Attack = RawAttack;
            Defence = RawDefence;
            Dexterity = RawDexterity;
            Resistance = RawResistance;
            Agility = RawAgility;
            Luck = RawLuck;
            Jump = RawJump;
        }
    }

    public enum ActiveUnitType
    {
        TestPlayableUnit, TestEnemyUnit
    }

    public enum Camp
    {
        Ally, Hostile, Neutral
    }

    /// <summary>
    /// 戦闘以外の時にユニットのデータを保持しておくためのクラス
    /// </summary>
    [JsonObject("ActiveUnitData")]
    [System.Serializable]
    public class ActiveUnitData
    {
        [JsonProperty("Name")]
        public string Name;
        [JsonProperty("Discription")]
        [TextArea()]
        public string Discription;

        [JsonProperty("Type")]
        public ActiveUnitType Type;

        [JsonProperty("Camp")]
        public Camp Camp;

        [JsonProperty("Position")]
        public Point Position;

        [Header("各レベル")]
        [JsonProperty("Level")]
        public int Level;
        [JsonProperty("Exp")]
        public int Exp;

        [JsonProperty("AttackLevel")]
        public int AttackLevel;
        [JsonProperty("AttackExp")]
        public int AttackExp;
        [JsonProperty("DefenceLevel")]
        public int DefenceLevel;
        [JsonProperty("DefenceExp")]
        public int DefenceExp;
        [JsonProperty("DexterityLevel")]
        public int DexterityLevel;
        [JsonProperty("DexterityExp")]
        public int DexterityExp;
        [JsonProperty("ResistanceLevel")]
        public int ResistanceLevel;
        [JsonProperty("ResistanceExp")]
        public int ResistanceExp;

        [Header("能力値")]
        [JsonProperty("RawMaxHP")]
        public int RawMaxHP;
        [JsonProperty("RawMaxMP")]
        public int RawMaxSP;
        [JsonProperty("RawAttack")]
        public int RawAttack;
        [JsonProperty("RawDefence")]
        public int RawDefence;
        [JsonProperty("RawDexterity")]
        public int RawDexterity;
        [JsonProperty("RawResistance")]
        public int RawResistance;
        [JsonProperty("RawAgility")]
        public int RawAgility;
        [JsonProperty("RawLuck")]
        public int RawLuck;
        [JsonProperty("RawJump")]
        public int RawJump;

        [JsonProperty("Skills")]
        public List<SkillData> Skills;
        [JsonProperty("Items")]
        public List<ItemData> Items;
    }
}
