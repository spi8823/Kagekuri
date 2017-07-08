using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using Newtonsoft.Json;

namespace Kagekuri
{
    /// <summary>
    /// バトル一回分の情報を持ってる
    /// つまり、Field(マップの情報)とEventScript(イベントの情報)
    /// </summary>
    public class Stage
    {
        public static Stage Instance { get { return BattleSceneManager.Instance.Stage; } }
        public Field Field { get; private set; }
        public Script EventScript { get; private set; }
        public List<Unit> Units { get; private set; }
        public List<ActiveUnit> ActiveUnits { get { return Units.OfType<ActiveUnit>().ToList(); } }

        public Stage(string filename)
        {
            Units = new List<Unit>();
            var data = JsonConvert.DeserializeObject<StageData>(Resources.Load<TextAsset>("Stage/StageData/" + filename).text);
            Field = Field.Create(data.FieldFileName, this);
            foreach(var a in data.ActiveUnits)
            {
                var unit = ActiveUnit.Create(a);
                Field[unit.Position].SetUnit(unit);
                Units.Add(unit);
            }
            InitializeEventScript(data.EventFileName);
        }

        public Stage(Stages stage) : this(stage.ToString() + ".stage") { }

        public void InitializeEventScript(string filename)
        {
            return;
            var script = Resources.Load<TextAsset>("EventScript/" + filename).text;

            EventScript = new Script();
            EventScript.DoString(script);
        }
    }

    public enum Stages
    {
        Test
    }

    [JsonObject("StageData")]
    [System.Serializable]
    public class StageData
    {
        [JsonProperty("Name")]
        public string Name;
        [JsonProperty("Discription")]
        [TextArea()]
        public string Discription;
        [JsonProperty("FieldFileName")]
        public string FieldFileName;
        [JsonProperty("EventFileName")]
        public string EventFileName;
        [JsonProperty("ActiveUnits")]
        public List<ActiveUnitData> ActiveUnits;

        public StageData()
        {
            Name = "";
            Discription = "";
            FieldFileName = "";
            EventFileName = "";
            ActiveUnits = new List<ActiveUnitData>();
        }
    }
}