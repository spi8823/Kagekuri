using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using System;

namespace Kagekuri
{
    public class Test : MonoBehaviour
    {
        public void Awake()
        {
        }

        public static StageData GetStageData()
        {
            SquareData[][] squareDatas = new SquareData[10][];
            for(var i = 0;i < 10;i++)
            {
                squareDatas[i] = new SquareData[10];
                for(var j = 0;j < 10;j++)
                {
                    SquareData squareData = new SquareData();
                    squareData.Location = new Point(i, j);
                    squareData.Type = SquareType.Normal;
                    squareDatas[i][j] = squareData;
                }
            }
            FieldData fieldData = new FieldData();
            fieldData.SquareDatas = squareDatas;
            fieldData.Size = new Point(10, 10);
            List<UnitData> unitDatas = new List<UnitData>();
            for(var i = 0;i < 10;i++)
            {
                UnitData unitData = new UnitData();
                unitData.Type = (UnitType)(i % 2);
                unitData.Direction = Point.Right;
                unitData.Location = new Point(i, i);
                unitDatas.Add(unitData);
            }
            List<ActiveUnitData> activeUnitDatas = new List<ActiveUnitData>();
            for(var i = 0;i < 6;i++)
            {
                ActiveUnitData activeUnitData = new ActiveUnitData();
                activeUnitData.Type = (ActiveUnitType)(i % 2);
                activeUnitData.Direction = Point.Left;
                activeUnitData.Location = new Point(i + 4, i);
                activeUnitData.RawMaxAP = i * i * 10 + 1;
                activeUnitData.RawSpeed = i + 1;
                activeUnitData.WaitingAPExpantionRate = 1.5;
                activeUnitData.WaitingSpeedRate = 0.5;
                activeUnitDatas.Add(activeUnitData);
            }
            List<CharacterUnitData> characterUnitDatas = new List<CharacterUnitData>();
            for(var i = 0;i < 6;i++)
            {
                CharacterUnitData characterUnitData = new CharacterUnitData();
                characterUnitData.Type = (CharacterUnitType)(i % 2);
                characterUnitData.Direction = Point.Down;
                characterUnitData.Location = new Point(i, 4 + i);
                characterUnitData.Level = i * 2 + 1;
                characterUnitData.ExP = 10 + i * i;
                characterUnitData.RawAttack = 10 * i + 1;
                characterUnitData.RawDefence = 15 * i + 1;
                characterUnitData.RawMaxAP = i * 30 + 1;
                characterUnitData.RawMaxHP = i * 40 + 1;
                characterUnitData.RawMaxMP = i * 50 + 1;
                characterUnitData.RawSpeed = i + 3;
                characterUnitData.WaitingAPExpantionRate = 1.5;
                characterUnitData.WaitingSpeedRate = 0.5;
                characterUnitData.SkillDatas = new List<SkillData>();
                for(var j = 0;j < 4;j++)
                {
                    SkillData skillData = new SkillData();
                    skillData.Level = i + 1;
                    skillData.ExP = i * i * i + 1;
                    skillData.Type = (SkillType)(i % 2);
                    characterUnitData.SkillDatas.Add(skillData);
                }
                characterUnitDatas.Add(characterUnitData);
            } 
            StageData stageData = new StageData();
            stageData.FieldData = fieldData;
            stageData.UnitDatas = unitDatas;
            stageData.ActiveUnitDatas = activeUnitDatas;
            stageData.CharacterUnitDatas = characterUnitDatas;
            stageData.EventNumber = 0;
            return stageData;
        }
    }

    public class TestUnit1 : Unit
    {
        public TestUnit1(UnitData data, Field field)
            :base(data, field)
        {

        }
    }

    public class TestUnit2 : Unit
    {
        public TestUnit2(UnitData data, Field field)
            :base(data, field)
        {

        }
    }

    public class TestActiveUnit1 : ActiveUnit
    {
        public TestActiveUnit1(ActiveUnitData data, Field field)
            : base(data, field)
        {

        }
    }

    public class TestActiveUnit2 : ActiveUnit
    {
        public TestActiveUnit2(ActiveUnitData data, Field field)
            : base(data, field)
        {

        }
    }

    public class TestCharacterUnit1 : CharacterUnit
    {
        public TestCharacterUnit1(CharacterUnitData data, Field field)
            : base(data, field)
        {

        }
    }

    public class TestCharacterUnit2 : OperableUnit
    {
        public TestCharacterUnit2(CharacterUnitData data, Field field)
            : base(data, field)
        {

        }
    }

    public class TestSkill1 : Skill
    {
        public TestSkill1(CharacterUnit unit, SkillData data)
            :base(unit, data)
        {

        }
    }

    public class TestSkill2 : Skill
    {
        public TestSkill2(CharacterUnit unit, SkillData data)
            : base(unit, data)
        {

        }
    }
}